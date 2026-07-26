using UnityEngine;

/// <summary>
/// Gently bobs a UI element around its resting position for a subtle
/// "hovering" / floating effect. Each axis can be toggled and tuned
/// independently, so you can make it drift side-to-side, up-and-down,
/// in-and-out (depth), or any combination of those.
///
/// Attach to a UI GameObject that has a RectTransform.
/// </summary>
[AddComponentMenu("UI/UI Hover Bob")]
[RequireComponent(typeof(RectTransform))]
public class UIHoverBob : MonoBehaviour
{
    [System.Serializable]
    public class BobAxis
    {
        [Tooltip("Whether this axis moves at all.")]
        public bool enabled = true;

        [Tooltip("Peak offset from the resting position, in UI units (~pixels at 1:1 canvas scale).")]
        public float amplitude = 6f;

        [Tooltip("Oscillation speed. Higher = faster bobbing.")]
        public float speed = 1.5f;

        [Tooltip("Phase offset in degrees. Offset X vs Y (e.g. 90) so the motion traces an ellipse instead of a straight diagonal.")]
        public float phaseDegrees = 0f;
    }

    [Header("Axes")]
    [Tooltip("Side to side.")]
    public BobAxis x = new BobAxis { enabled = true, amplitude = 6f, speed = 1.2f, phaseDegrees = 0f };

    [Tooltip("Up and down.")]
    public BobAxis y = new BobAxis { enabled = true, amplitude = 6f, speed = 1.6f, phaseDegrees = 90f };

    [Tooltip("In and out (depth). Usually left off for flat UI.")]
    public BobAxis z = new BobAxis { enabled = false, amplitude = 0f, speed = 1f, phaseDegrees = 0f };

    [Header("Rotation wobble (optional)")]
    [Tooltip("Adds a gentle z-rotation sway on top of the movement.")]
    public bool rotate = false;
    [Tooltip("Peak tilt in degrees.")]
    public float rotateAmplitude = 3f;
    public float rotateSpeed = 1f;
    public float rotatePhaseDegrees = 45f;

    [Header("General")]
    [Tooltip("Randomize the starting phase so multiple copies don't bob in sync.")]
    public bool randomizeStartPhase = true;

    [Tooltip("Use unscaled time so it keeps bobbing while the game is paused (Time.timeScale = 0).")]
    public bool useUnscaledTime = true;

    RectTransform _rect;
    Vector3 _basePosition;
    Quaternion _baseRotation;
    float _startTime;
    float _randomOffset;

    void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        // Cache the resting pose so we always animate as an offset from it.
        _basePosition = _rect.anchoredPosition3D;
        _baseRotation = _rect.localRotation;
        _startTime = CurrentTime;
        _randomOffset = randomizeStartPhase ? Random.Range(0f, 1000f) : 0f;
    }

    void OnDisable()
    {
        // Snap back to rest so we don't leave the element drifted off-center.
        if (_rect != null)
        {
            _rect.anchoredPosition3D = _basePosition;
            _rect.localRotation = _baseRotation;
        }
    }

    void Update()
    {
        float t = (CurrentTime - _startTime) + _randomOffset;

        Vector3 offset = Vector3.zero;
        if (x.enabled) offset.x = Evaluate(x, t);
        if (y.enabled) offset.y = Evaluate(y, t);
        if (z.enabled) offset.z = Evaluate(z, t);

        _rect.anchoredPosition3D = _basePosition + offset;

        if (rotate)
        {
            float angle = Mathf.Sin(t * rotateSpeed + rotatePhaseDegrees * Mathf.Deg2Rad) * rotateAmplitude;
            _rect.localRotation = _baseRotation * Quaternion.Euler(0f, 0f, angle);
        }
    }

    static float Evaluate(BobAxis axis, float t)
    {
        return Mathf.Sin(t * axis.speed + axis.phaseDegrees * Mathf.Deg2Rad) * axis.amplitude;
    }

    float CurrentTime => useUnscaledTime ? Time.unscaledTime : Time.time;

    /// <summary>Re-cache the resting pose (call if you move the element at runtime).</summary>
    public void ResetRestPose()
    {
        _basePosition = _rect.anchoredPosition3D;
        _baseRotation = _rect.localRotation;
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (x.amplitude < 0f) x.amplitude = 0f;
        if (y.amplitude < 0f) y.amplitude = 0f;
        if (z.amplitude < 0f) z.amplitude = 0f;
    }
#endif
}
