using UnityEngine;

public class NoInternetDistraction : Distraction
{
    public override void Trigger()
    {
        PuzzleOrchestrator.Internet = false;
    }
}
