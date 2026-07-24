using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RenderTextureManager : MonoBehaviour
{
    [Header("Render Textures")]
    [SerializeField] private RenderTexture roomEnvironment;
    [SerializeField] private RenderTexture computerScreen;

    [SerializeField] private Canvas computerCanvas;

    private RawImage rawImage;

    private void Awake()
    {
        rawImage = GetComponent<RawImage>();
    }

    public void ChangeToComputer()
    {
        computerCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        rawImage.texture = computerScreen;
        
    }

    public void ChangeToRoom()
    {
        computerCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rawImage.texture = roomEnvironment;
    }

    void Start()
    {
        ChangeToRoom();
    }
}