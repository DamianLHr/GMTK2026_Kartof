using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A single comic page. Owns an ordered list of sub-panels that are revealed one
/// advance-input at a time. Knows nothing about input, scene flow, or the controller.
/// Sub-panels are world-space objects; fades run on their SpriteRenderers.
/// </summary>
[DisallowMultipleComponent]
public class ComicPanel : MonoBehaviour
{
    [Serializable]
    public class SubPanelEvent : UnityEvent<int> { }

    [Header("Sub-panels, in reveal order")]
    [SerializeField] private List<GameObject> subPanels = new List<GameObject>();

    [Header("Reveal")]
    [Tooltip("Fade-in time per sub-panel, applied to every SpriteRenderer under it. " +
             "Set to 0 to disable fading and have sub-panels pop in.")]
    [Min(0f)]
    [SerializeField] private float revealDuration = 0.15f;

    [Header("Events")]
    [Tooltip("Fired with the index of each sub-panel as it is revealed. Hook up SFX / VO / camera shake here.")]
    [SerializeField] private SubPanelEvent onSubPanelRevealed = new SubPanelEvent();

    [Tooltip("Fired once the last sub-panel has been revealed.")]
    [SerializeField] private UnityEvent onPanelCompleted = new UnityEvent();

    private int revealedCount;
    private bool isShown;
    private bool completionFired;
    private Coroutine revealRoutine;

    // Renderers currently fading, with the target alpha each should end on
    // (its authored alpha, so a sprite drawn at 0.8 fades to 0.8, not 1).
    private readonly List<SpriteRenderer> fadingRenderers = new List<SpriteRenderer>();
    private readonly List<float> fadingTargetAlphas = new List<float>();

    public SubPanelEvent OnSubPanelRevealed => onSubPanelRevealed;
    public UnityEvent OnPanelCompleted => onPanelCompleted;

    /// <summary>True once every sub-panel has been revealed.</summary>
    public bool IsComplete => revealedCount >= subPanels.Count;

    /// <summary>True while a sub-panel is still fading in.</summary>
    public bool IsRevealing => revealRoutine != null;

    /// <summary>
    /// Resets the panel to its pre-show state. Called explicitly by the controller
    /// rather than from Awake/Start, because ordering between Start callbacks on
    /// different objects is undefined and the old version relied on it.
    /// </summary>
    public void Initialize()
    {
        FinishRevealImmediately();

        revealedCount = 0;
        isShown = false;
        completionFired = false;

        for (int i = 0; i < subPanels.Count; i++)
        {
            if (subPanels[i] == null) continue;
            subPanels[i].SetActive(false);
        }
    }

    /// <summary>Makes the panel live and reveals its first sub-panel.</summary>
    public void Show()
    {
        if (isShown) return;

        isShown = true;
        gameObject.SetActive(true);
        RevealNext();
    }

    /// <summary>Takes the panel out of play once it has left the screen.</summary>
    public void Hide()
    {
        FinishRevealImmediately();
        isShown = false;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Consumes one advance input. Returns true if this panel handled it (snapped a
    /// fade to done, or revealed another sub-panel), false if the panel is finished
    /// and the controller should turn the page.
    /// </summary>
    public bool TryAdvance()
    {
        if (!isShown) return false;

        // A click during a fade completes the fade instead of being swallowed.
        if (IsRevealing)
        {
            FinishRevealImmediately();
            return true;
        }

        if (IsComplete) return false;

        RevealNext();
        return true;
    }

    private void RevealNext()
    {
        if (revealedCount < subPanels.Count)
        {
            GameObject sub = subPanels[revealedCount];
            int revealedIndex = revealedCount;
            revealedCount++;

            if (sub != null)
            {
                sub.SetActive(true);

                if (revealDuration > 0f)
                {
                    BeginFade(sub);
                }

                onSubPanelRevealed.Invoke(revealedIndex);
            }
        }

        if (IsComplete && !completionFired)
        {
            completionFired = true;
            onPanelCompleted.Invoke();
        }
    }

    private void BeginFade(GameObject sub)
    {
        // Works whether the sub-panel is a single SpriteRenderer or a parent
        // with several child renderers.
        SpriteRenderer[] renderers = sub.GetComponentsInChildren<SpriteRenderer>();
        if (renderers.Length == 0) return;

        fadingRenderers.Clear();
        fadingTargetAlphas.Clear();

        for (int i = 0; i < renderers.Length; i++)
        {
            SpriteRenderer r = renderers[i];
            fadingRenderers.Add(r);
            fadingTargetAlphas.Add(r.color.a); // authored alpha = fade destination

            Color c = r.color;
            c.a = 0f;
            r.color = c;
        }

        revealRoutine = StartCoroutine(FadeInRoutine());
    }

    private IEnumerator FadeInRoutine()
    {
        float elapsed = 0f;
        while (elapsed < revealDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / revealDuration);

            for (int i = 0; i < fadingRenderers.Count; i++)
            {
                SpriteRenderer r = fadingRenderers[i];
                if (r == null) continue;

                Color c = r.color;
                c.a = Mathf.Lerp(0f, fadingTargetAlphas[i], t);
                r.color = c;
            }

            yield return null;
        }

        SnapFadeToEnd();
        revealRoutine = null;
    }

    private void FinishRevealImmediately()
    {
        if (revealRoutine != null)
        {
            StopCoroutine(revealRoutine);
            revealRoutine = null;
        }

        SnapFadeToEnd();
    }

    private void SnapFadeToEnd()
    {
        for (int i = 0; i < fadingRenderers.Count; i++)
        {
            SpriteRenderer r = fadingRenderers[i];
            if (r == null) continue;

            Color c = r.color;
            c.a = fadingTargetAlphas[i];
            r.color = c;
        }

        fadingRenderers.Clear();
        fadingTargetAlphas.Clear();
    }
}