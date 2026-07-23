using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class TimerManager : MonoBehaviour
{
    [Header("Difficulty (data-driven, not hardcoded)")]
    [SerializeField] private DifficultyProfile difficulty;       // Chill / Normal / Crunch asset
    [SerializeField] private float fallbackSecondsPerCount = 1f;  // used only when no profile is assigned

    [Header("Countdown")]
    [SerializeField] private int startCount = 60;   // always reads 60 -> 0; difficulty changes tick length, not this number
    [SerializeField] private bool autoStart = true;

    [Header("Pin")]
    [SerializeField] private RectTransform pin;              // the clock hand
    [SerializeField] private float fullSweepDegrees = 360f;  // 360 / 60 = 6 degrees per count
    [SerializeField] private bool clockwise = true;
    [SerializeField] private bool tickInSteps = false;       // false = glide smoothly, true = jump one interval per count

    [Header("Readout (optional)")]
    [SerializeField] private TMP_Text countLabel;

    [Header("Events")]
    public UnityEvent onTimeUp;   // wire the "missed the deadline" canvas here, the same way LogIn toggles canvases

    public int CountRemaining { get; private set; }
    public bool Running { get; private set; }

    // 1 at the start of the run, easing smoothly to 0. Handy for urgency effects (music tempo, vignette).
    public float NormalizedTimeRemaining
    {
        get
        {
            if (startCount <= 0) return 0f;
            float spc = SecondsPerCount;
            float intra = spc > 0f ? Mathf.Clamp01(countTimer / spc) : 0f;
            return Mathf.Clamp01((CountRemaining - intra) / startCount);
        }
    }

    private float SecondsPerCount => difficulty != null ? difficulty.secondsPerCount : fallbackSecondsPerCount;

    private float countTimer;           // real seconds elapsed inside the current count
    private Quaternion pinBaseRotation;  // pin's authored rotation, used as the zero mark

    private void Awake()
    {
        if (pin != null) pinBaseRotation = pin.localRotation;
    }

    private void Start()
    {
        ResetCountdown();
        if (autoStart) StartCountdown();
    }

    private void Update()
    {
        if (!Running) return;

        countTimer += Time.deltaTime;
        float spc = SecondsPerCount;

        // while-loop so a long frame can burn more than one count without drifting
        while (Running && countTimer >= spc && CountRemaining > 0)
        {
            countTimer -= spc;
            CountRemaining--;
            RefreshLabel();
            if (CountRemaining <= 0) TimeUp();
        }

        RefreshPin(spc);
    }

    public void StartCountdown() => Running = CountRemaining > 0;
    public void PauseCountdown() => Running = false;                 // freeze, keep progress (e.g. a cutscene)
    public void ResumeCountdown() => Running = CountRemaining > 0;
    public void StopCountdown() => Running = false;                  // win: freeze on whatever is left

    public void ResetCountdown()
    {
        CountRemaining = startCount;
        countTimer = 0f;
        Running = false;
        RefreshLabel();
        RefreshPin(SecondsPerCount);
    }

    private void TimeUp()
    {
        CountRemaining = 0;
        Running = false;
        RefreshLabel();
        onTimeUp?.Invoke();
        // Prefer the shared bus? -> EventBus<PlayerDeadEvent>.Raise(new PlayerDeadEvent());
    }

    private void RefreshLabel()
    {
        if (countLabel != null) countLabel.text = CountRemaining.ToString();
    }

    private void RefreshPin(float spc)
    {
        if (pin == null) return;

        float elapsedCounts = startCount - CountRemaining;
        float intra = tickInSteps || spc <= 0f ? 0f : Mathf.Clamp01(countTimer / spc);
        float sweep = (elapsedCounts + intra) / startCount;    // 0 at start, 1 when it hits zero
        float angle = sweep * fullSweepDegrees;
        if (clockwise) angle = -angle;

        pin.localRotation = pinBaseRotation * Quaternion.Euler(0f, 0f, angle);
    }
}