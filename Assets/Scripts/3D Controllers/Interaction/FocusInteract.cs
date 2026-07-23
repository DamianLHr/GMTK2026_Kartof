using System.Collections;
using UnityEngine;

/// <summary>
/// IInteractable that glides the player camera toward a focus
/// point relative to this object, and glides back to the
/// original position/rotation on second interaction.
/// Disables MovementController3D while focused so it doesn't
/// fight the glide by overwriting camera transform every frame.
/// Swaps the fullscreen display texture (room <-> computer)
/// once the glide completes (or partway through, for the exit glide).
/// </summary>
public class FocusInteract : MonoBehaviour, IInteractable
{
    [Header("Focus Target")]
    [SerializeField] private Transform focusPoint;
    [SerializeField] private float glideDuration = 0.6f;

    [Header("Exit Swap Timing")]
    [Tooltip("Fraction of the exit glide (0-1) at which the screen swaps back to room view, before the glide finishes.")]
    [SerializeField, Range(0f, 1f)] private float exitSwapProgress = 0.3f;

    [Header("References")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private MovementController3D movementController;
    [SerializeField] private RenderTextureManager renderTextureManager;

    private bool focused = false;

    private Vector3 originalWorldPos;
    private Quaternion originalWorldRot;
    private Transform originalParent;

    private Coroutine glideCoroutine;

    public void OnInteraction()
    {
        if (!focused)
        {
            focused = true;

            originalWorldPos = targetCamera.transform.position;
            originalWorldRot = targetCamera.transform.rotation;
            originalParent = targetCamera.transform.parent;

            movementController.enabled = false;
            targetCamera.transform.SetParent(null, true);

            if (glideCoroutine != null) StopCoroutine(glideCoroutine);
            glideCoroutine = StartCoroutine(Glide(
                focusPoint.position,
                focusPoint.rotation,
                swapProgress: -1f, // no early swap on the way in
                onSwap: null,
                onComplete: () => renderTextureManager.ChangeToComputer()
            ));
        }
        else
        {
            focused = false;

            if (glideCoroutine != null) StopCoroutine(glideCoroutine);
            glideCoroutine = StartCoroutine(Glide(
                originalWorldPos,
                originalWorldRot,
                swapProgress: exitSwapProgress,
                onSwap: () => renderTextureManager.ChangeToRoom(),
                onComplete: () =>
                {
                    targetCamera.transform.SetParent(originalParent, true);
                    movementController.enabled = true;
                }
            ));
        }
    }

    private IEnumerator Glide(Vector3 targetPos, Quaternion targetRot, float swapProgress, System.Action onSwap, System.Action onComplete)
    {
        Vector3 startPos = targetCamera.transform.position;
        Quaternion startRot = targetCamera.transform.rotation;
        float t = 0f;
        bool swapped = false;

        while (t < glideDuration)
        {
            t += Time.deltaTime;
            float normalized = Mathf.Clamp01(t / glideDuration);

            targetCamera.transform.position = Vector3.Lerp(startPos, targetPos, normalized);
            targetCamera.transform.rotation = Quaternion.Slerp(startRot, targetRot, normalized);

            if (!swapped && swapProgress >= 0f && normalized >= swapProgress)
            {
                swapped = true;
                onSwap?.Invoke();
            }

            yield return null;
        }

        targetCamera.transform.position = targetPos;
        targetCamera.transform.rotation = targetRot;

        onComplete?.Invoke();
    }
}