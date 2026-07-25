using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

/// <summary>
/// Drives a sequence of ComicPanels: owns input, the slide transition, and the
/// hand-off when the comic ends. Panels own their own content and reveal logic.
/// </summary>
[DisallowMultipleComponent]
public class ComicController : MonoBehaviour
{
    [Header("Panels, in reading order")]
    [SerializeField] private List<ComicPanel> panels = new List<ComicPanel>();

    [Header("Layout markers")]
    [Tooltip("Empty transforms parented alongside the panels, so the layout follows the canvas / camera " +
             "instead of being baked into absolute world coordinates.")]
    [SerializeField] private Transform entryMarker;
    [SerializeField] private Transform stayMarker;
    [SerializeField] private Transform exitMarker;

    [Header("Transition")]
    [Min(0f)]
    [SerializeField] private float moveDuration = 0.5f;
    [SerializeField] private AnimationCurve moveEase = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Input")]
    [SerializeField] private bool advanceOnClick = true;
    [SerializeField] private KeyCode[] advanceKeys = { KeyCode.Space, KeyCode.Return };
    [Tooltip("Ignore clicks that land on UI, so a Skip button does not also advance the comic.")]
    [SerializeField] private bool ignoreClicksOverUI = true;

    [Header("Finish")]
    [Tooltip("Leave empty to load no scene and only fire the event below.")]
    [SerializeField] private string nextSceneName = "PuzzleScene";
    [SerializeField] private UnityEvent onComicFinished = new UnityEvent();

    private int panelIndex;
    private bool isTransitioning;
    private bool skipTransitionRequested;
    private bool isFinished;

    public UnityEvent OnComicFinished => onComicFinished;

    private void Start()
    {
        int removed = panels.RemoveAll(panel => panel == null);
        if (removed > 0)
        {
            Debug.LogWarning($"{nameof(ComicController)}: ignored {removed} empty panel slot(s).", this);
        }

        if (panels.Count == 0 || entryMarker == null || stayMarker == null || exitMarker == null)
        {
            Debug.LogError($"{nameof(ComicController)}: needs at least one panel and all three layout markers.", this);
            enabled = false;
            return;
        }

        // Explicit init, so this never races ComicPanel's own Start.
        for (int i = 0; i < panels.Count; i++)
        {
            panels[i].Initialize();
            panels[i].transform.position = entryMarker.position;
            panels[i].gameObject.SetActive(false);
        }

        panelIndex = 0;
        panels[0].transform.position = stayMarker.position;
        panels[0].Show();
    }

    private void Update()
    {
        if (isFinished || !WasAdvancePressed()) return;

        // Input during a transition fast-forwards it instead of being dropped.
        if (isTransitioning)
        {
            skipTransitionRequested = true;
            return;
        }

        if (panels[panelIndex].TryAdvance()) return;

        AdvancePanel();
    }

    /// <summary>Hook this up to a Skip button.</summary>
    public void Skip()
    {
        if (isFinished) return;

        StopAllCoroutines();
        isTransitioning = false;
        skipTransitionRequested = false;

        for (int i = 0; i < panels.Count; i++)
        {
            panels[i].Hide();
        }

        Finish();
    }

    private void AdvancePanel()
    {
        if (panelIndex >= panels.Count - 1)
        {
            Finish();
            return;
        }

        ComicPanel outgoing = panels[panelIndex];
        panelIndex++;
        StartCoroutine(TransitionRoutine(outgoing, panels[panelIndex]));
    }

    private IEnumerator TransitionRoutine(ComicPanel outgoing, ComicPanel incoming)
    {
        isTransitioning = true;
        skipTransitionRequested = false;

        Vector3 outFrom = outgoing.transform.position;
        Vector3 outTo = exitMarker.position;
        Vector3 inFrom = entryMarker.position;
        Vector3 inTo = stayMarker.position;

        incoming.transform.position = inFrom;
        incoming.Show();

        float elapsed = 0f;
        while (elapsed < moveDuration && !skipTransitionRequested)
        {
            elapsed += Time.deltaTime;
            float t = moveEase.Evaluate(Mathf.Clamp01(elapsed / moveDuration));

            // Unclamped, so ease curves that overshoot (back / bounce) actually work.
            outgoing.transform.position = Vector3.LerpUnclamped(outFrom, outTo, t);
            incoming.transform.position = Vector3.LerpUnclamped(inFrom, inTo, t);

            yield return null;
        }

        outgoing.transform.position = outTo;
        incoming.transform.position = inTo;
        outgoing.Hide();

        skipTransitionRequested = false;
        isTransitioning = false;
    }

    private void Finish()
    {
        if (isFinished) return;
        isFinished = true;

        onComicFinished.Invoke();

        if (string.IsNullOrEmpty(nextSceneName)) return;

        if (LevelLoader.Instance == null)
        {
            Debug.LogError($"{nameof(ComicController)}: no LevelLoader available, cannot load '{nextSceneName}'.", this);
            return;
        }

        LevelLoader.Instance.LoadLevel(nextSceneName);
    }

    private bool WasAdvancePressed()
    {
        if (advanceOnClick && Input.GetMouseButtonDown(0) && !(ignoreClicksOverUI && IsPointerOverUI()))
        {
            return true;
        }

        for (int i = 0; i < advanceKeys.Length; i++)
        {
            if (Input.GetKeyDown(advanceKeys[i])) return true;
        }

        return false;
    }

    private static bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}
