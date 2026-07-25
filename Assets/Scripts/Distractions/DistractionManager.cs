using System.Collections;
using UnityEngine;

// seed RNG once for a reproducible run,  and schedule distractions.

public class DistractionManager : MonoBehaviour
{
    [Header("Run")]
    [SerializeField] private DifficultyProfile difficulty;
    [SerializeField] private bool useFixedSeed = false;   
    [SerializeField] private int seed = 12345;
    [SerializeField] private bool autoStartRun = true;

    [Header("Distractions")]
    [SerializeField] private Distraction[] distractionPool;
    [SerializeField] private bool runDistractions = true;

    private Coroutine distractionRoutine;
    private EventBinding<PlayerWonEvent> wonBinding;
    private EventBinding<PlayerDeadEvent> deadBinding;

    private void Awake()
    {
        if (useFixedSeed) Random.InitState(seed);
    }

    private void OnEnable()
    {
        wonBinding = new EventBinding<PlayerWonEvent>(OnRunWon);
        EventBus<PlayerWonEvent>.Register(wonBinding);

        deadBinding = new EventBinding<PlayerDeadEvent>(OnRunLost);
        EventBus<PlayerDeadEvent>.Register(deadBinding);
    }

    private void OnDisable()
    {
        EventBus<PlayerWonEvent>.Deregister(wonBinding);
        EventBus<PlayerDeadEvent>.Deregister(deadBinding);
    }

    private void Start()
    {
        if (autoStartRun) BeginRun();
    }

    public void BeginRun()
    {
        if (runDistractions && distractionRoutine == null)
            distractionRoutine = StartCoroutine(DistractionQueue());
    }

    private IEnumerator DistractionQueue()
    {
        if (difficulty == null || distractionPool == null || distractionPool.Length == 0)
            yield break;

        int remaining = difficulty.distractionCount;
        Distraction last = null;

        while (remaining > 0)
        {
            float gap = Random.Range(difficulty.distractionInterval.x, difficulty.distractionInterval.y);
            yield return new WaitForSeconds(gap);

            Distraction pick = PickDistraction(last);
            if (pick != null) { pick.Trigger(); last = pick; }
            remaining--;
        }
    }

    private Distraction PickDistraction(Distraction avoid)
    {
        if (distractionPool.Length == 1) return distractionPool[0];

        float total = 0f;
        foreach (var d in distractionPool)
            if (d != null && d != avoid) total += Mathf.Max(0f, d.weight);
        if (total <= 0f) return null;

        float r = Random.value * total;
        foreach (var d in distractionPool)
        {
            if (d == null || d == avoid) continue;
            r -= Mathf.Max(0f, d.weight);
            if (r <= 0f) return d;
        }
        return null;
    }

    private void OnRunWon()
    {
        StopDistractions();
    }

    private void OnRunLost() => StopDistractions();

    private void StopDistractions()
    {
        if (distractionRoutine != null)
        {
            StopCoroutine(distractionRoutine);
            distractionRoutine = null;
        }
    }
}