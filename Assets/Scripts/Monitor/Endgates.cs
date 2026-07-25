using System;
using TMPro;
using Unity.VisualScripting; // Required for Actions
using UnityEngine;
using Random = System.Random;

public class EndGates : MonoBehaviour
{
    public static event Action OnMinigameFinished;
    public GameObject[] endgates;
    public int index;

    private void Start()
    {
        index = UnityEngine.Random.Range(0, endgates.Length); // I don't know why but it only works like this for some reason
        for (int i = 0; i < endgates.Length; i++)
        {
            endgates[i] = transform.GetChild(i).gameObject;
            endgates[i].tag = "Respawn";
            int ID;
            do
            {
                ID = UnityEngine.Random.Range(1000, 10000);
            } 
            while (ID == PuzzleOrchestrator.RouterID);
            endgates[i].GetComponent<TextMesh>().text = ID.ToString();
            if (i == index)
            {
                endgates[i].tag = "Finish";
                endgates[i].GetComponent<TextMesh>().text = PuzzleOrchestrator.RouterID.ToString();
            }
        }

        for (int i = 0; i < endgates.Length; i++)
        {
            if (endgates[i].CompareTag("Respawn"))
                endgates[i].transform.parent = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("MinigamePlayer"))
            CompleteMinigame();
    }
    
    public void CompleteMinigame()
    {
        Debug.Log("Minigame Manager: Broadcasting finish signal!");
        
        if (OnMinigameFinished != null)
        {
            OnMinigameFinished.Invoke();
        }
    }

}
