using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MinigameWindowFocus : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    
    [SerializeField] private string minigameSceneName;
    
    private PlayerInput playerInput;

    private void OnEnable()
    { 
        EndGates.OnMinigameFinished += HandleMinigameFinished;
    }
    
    private void Start()
    {
        SceneManager.LoadScene(minigameSceneName, LoadSceneMode.Additive);
    }
    
    private void Update()
    {
        if (playerInput == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("MinigamePlayer");
            Debug.Log(player);
            if (player != null)
            {
                playerInput = player.GetComponent<PlayerInput>();
                
                playerInput.DeactivateInput(); 
            }
        }
    }
    
    private void HandleMinigameFinished()
    {
        Debug.Log("Main Scene: Minigame finished signal received!");
        
        // Put whatever you want to happen here! Examples:
        // - Unload the minigame scene: SceneManager.UnloadSceneAsync(minigameSceneName);
        // - Close the window UI: gameObject.SetActive(false);
        // - Give the player money/points in the main OS.
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (playerInput != null)
        {
            playerInput.ActivateInput();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (playerInput != null)
        {
            playerInput.DeactivateInput();


            Rigidbody2D rb = playerInput.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
    }
    
}