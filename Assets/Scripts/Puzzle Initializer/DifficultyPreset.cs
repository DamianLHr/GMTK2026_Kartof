using UnityEngine;

// One asset per difficulty (Chill / Normal / Crunch). Retune in the inspector, no code changes.
// Both the timer and the run director read this, so all of a run's difficulty lives in one place.
[CreateAssetMenu(fileName = "Difficulty", menuName = "Game/Difficulty Profile")]
public class DifficultyProfile : ScriptableObject
{
    [Header("Timer")]
    [Tooltip("Real seconds each of the 60 counts lasts. Bigger = easier. Normal = 1 (a true 60s run).")]
    [Range(0.4f, 2f)] public float secondsPerCount = 1f;

    [Header("Distractions (read by the run director, NOT the timer)")]
    [Tooltip("How many distractions this run may throw at you.")]
    [Min(0)] public int distractionCount = 0;

    [Tooltip("Random gap between distractions, in seconds (min, max).")]
    public Vector2 distractionInterval = new Vector2(6f, 12f);

    [Tooltip("Extra spice: raises the odds of the nastier distractions.")]
    [Range(0f, 1f)] public float distractionIntensity = 0f;
}