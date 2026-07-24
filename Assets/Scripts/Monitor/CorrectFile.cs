using UnityEngine;

public class CorrectFile : MonoBehaviour
{
    private bool hasSpawned = false; 

    public void Spawn()
    {
        if (hasSpawned) 
            return;

        hasSpawned = true; 

        Debug.Log("Correct File!");
    }
}