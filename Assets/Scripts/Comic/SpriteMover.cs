using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Moves this object between two marker transforms. Trigger a one-shot move with
/// MoveToB() / MoveToA(), or turn on <see cref="playOnStart"/> for automatic
/// looping / ping-pong motion. Uses unscaled-clamped easing, so overshoot curves
/// (back / bounce) work.
/// </summary>
[DisallowMultipleComponent]
public class SpriteMover : MonoBehaviour
{
    public enum LoopMode { Once, Loop, PingPong }

    [Header("Endpoints")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [Header("Motion")]
    [Min(0f)]
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private AnimationCurve ease = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [Tooltip("Also interpolate rotation between the two markers.")]
    [SerializeField] private bool matchRotation = false;

    [Header("Playback")]
    [SerializeField] private bool playOnStart = false;
    [SerializeField] private LoopMode loopMode = LoopMode.PingPong;
    [Tooltip("Pause between legs when looping / ping-ponging.")]
    [Min(0f)]
    [SerializeField] private float pauseBetween = 0f;
    [Tooltip("Ignore Time.timeScale, so it keeps moving while the game is paused.")]
    [SerializeField] private bool useUnscaledTime = false;

    [Header("Events")]
    [SerializeField] private UnityEvent onArrived = new UnityEvent();

    private Coroutine routine;

    /// <summary>True while a move is in progress.</summary>
    public bool IsMoving => routine != null;

    private void Start()
    {
        if (pointA == null || pointB == null)
        {
            Debug.LogError($"{nameof(SpriteMover)}: both Point A and Point B must be assigned.", this);
            enabled = false;
            return;
        }

        SnapTo(pointA);

        if (playOnStart)
        {
            Play();
        }
    }

    /// <summary>Starts automatic playback using the configured loop mode.</summary>
    public void Play()
    {
        Stop();
        routine = StartCoroutine(PlayRoutine());
    }

    /// <summary>Moves once from wherever it is to B.</summary>
    public void MoveToB() => MoveTo(pointB);

    /// <summary>Moves once from wherever it is to A.</summary>
    public void MoveToA() => MoveTo(pointA);

    /// <summary>Moves once to the given marker.</summary>
    public void MoveTo(Transform target)
    {
        if (target == null) return;
        Stop();
        routine = StartCoroutine(MoveRoutine(transform.position, transform.rotation, target, null));
    }

    /// <summary>Stops any motion and leaves the object where it is.</summary>
    public void Stop()
    {
        if (routine != null)
        {
            StopCoroutine(routine);
            routine = null;
        }
    }

    private IEnumerator PlayRoutine()
    {
        Transform target = pointB;

        while (true)
        {
            yield return MoveRoutine(transform.position, transform.rotation, target, null);

            if (loopMode == LoopMode.Once) break;

            if (pauseBetween > 0f)
            {
                yield return useUnscaledTime
                    ? new WaitForSecondsRealtime(pauseBetween)
                    : new WaitForSeconds(pauseBetween);
            }

            if (loopMode == LoopMode.PingPong)
            {
                target = target == pointB ? pointA : pointB;
            }
            else // Loop
            {
                SnapTo(pointA);
                target = pointB;
            }
        }

        routine = null;
    }

    private IEnumerator MoveRoutine(Vector3 fromPos, Quaternion fromRot, Transform target, object _)
    {
        // Endpoints are re-read from the target transform each frame so a moving
        // marker is tracked, not just its position at launch.
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            float t = ease.Evaluate(Mathf.Clamp01(elapsed / duration));

            transform.position = Vector3.LerpUnclamped(fromPos, target.position, t);
            if (matchRotation)
            {
                transform.rotation = Quaternion.SlerpUnclamped(fromRot, target.rotation, t);
            }

            yield return null;
        }

        transform.position = target.position;
        if (matchRotation)
        {
            transform.rotation = target.rotation;
        }

        routine = null;
        onArrived.Invoke();
    }

    private void SnapTo(Transform target)
    {
        transform.position = target.position;
        if (matchRotation)
        {
            transform.rotation = target.rotation;
        }
    }
}