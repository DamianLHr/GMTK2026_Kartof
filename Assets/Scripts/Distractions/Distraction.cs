using UnityEngine;

// Minimal contract for anything the PuzzleInitializer can schedule.
// Make each distraction (cat on keyboard, sibling lights, pop-ups...) a subclass and override Trigger().
public abstract class Distraction : MonoBehaviour
{
    [Tooltip("Relative likelihood of being picked by the scheduler.")]
    [Min(0f)] public float weight = 1f;

    // Called by PuzzleInitializer when this distraction should start.
    public abstract void Trigger();
}