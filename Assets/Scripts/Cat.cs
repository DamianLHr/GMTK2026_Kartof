using UnityEngine;

public class Cat : MonoBehaviour, IInteractable
{
    // Disclaimer
    // This script has been written like ass
    // I can be excused because I have only a few hours left
    [Header("Settings")]
    [Tooltip("The axis to track rotation around.")]
    public Vector3 rotationAxis = Vector3.up;

    [Header("Block player from interacting with the fucking monitor")] 
    [SerializeField] private FocusInteract monitorInteract;
    [SerializeField] private MeshCollider collider;
    
    
    [Header("Track Player Rotation")] 
    [SerializeField] private GameObject player;
    [SerializeField] private float totalDegreesRotated = 0f;
    [SerializeField] private int fullRotationsCount = 0;
    [SerializeField] private bool isRotating = false;

    private float lastAngle;
    private Quaternion lastRotation;

    [Header("Elements")] 
    [SerializeField] private GameObject stationaryCat;
    [SerializeField] private GameObject uiTarget;
    [SerializeField] private GameObject floatingCat;
    [SerializeField] private Animator anim;
    [SerializeField] private BoxCollider catCollider;

    [Header("Optional References")] 
    [SerializeField] private MovementController3D movementController;

    [Header("Cursor Settings")]
    [SerializeField] private bool manageCursor = true;
    
    private bool isActive = false;
    private bool isAscending = false;
    
    private void OnEnable()
    {
        if (floatingCat != null)
        {
            floatingCat.SetActive(false);
            catCollider.enabled = true;
        }
        
        stationaryCat.SetActive(false);
        monitorInteract.UnFocus();
        collider.enabled = false;
    }

    public void OnInteraction()
    {
        if (!isActive)
        {
            OnPickUp();
        }
    }

    public void OnPickUp()
    {
        isActive = true;
        floatingCat.SetActive(true);
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        gameObject.transform.GetChild(1).gameObject.SetActive(false);
        catCollider.enabled = false;
        uiTarget.SetActive(true);
        if (player != null)
        {
            lastRotation = player.transform.rotation;
            lastAngle = GetCurrentAngle();
        }
        else
        {
            Debug.LogWarning("Player reference is missing on the Cat script! Rotation tracking won't work.");
        }
    }
    
    void Update()
    {
        // 1. Don't do anything if the cat hasn't been picked up or if player is unassigned
        if (!isActive || player == null) return;

        // 2. Track the PLAYER's rotation
        Quaternion currentRotation = player.transform.rotation;
        
        if (currentRotation != lastRotation)
        {
            isRotating = true;
        }
        else
        {
            isRotating = false;
            return;
        }

        float currentAngle = GetCurrentAngle();
        
        float deltaAngle = Mathf.DeltaAngle(lastAngle, currentAngle);
        
        totalDegreesRotated += deltaAngle;
        lastAngle = currentAngle;
        lastRotation = currentRotation;

        int newRotationCount = Mathf.FloorToInt(Mathf.Abs(totalDegreesRotated) / 360f);
        if (newRotationCount > fullRotationsCount)
        {
            fullRotationsCount = newRotationCount;
            OnFullRotationCompleted(Mathf.Sign(totalDegreesRotated));
        }

        if (fullRotationsCount >= 3 && !isAscending)
        { 
            isAscending = true;
            CatAscends();
        }
    }

    private void CatAscends() //PUT SOUND HERE!!!
    {
        Debug.Log("Cat ascends"); 
        anim.enabled = true;
        Destroy(gameObject, 10f);
        Destroy(floatingCat, 10f);
        floatingCat.transform.parent = transform.parent;
        collider.enabled = true;
        uiTarget.SetActive(false);
    }

    
    private float GetCurrentAngle()
    {
        if (player == null) return 0f;

        // Use the PLAYER's forward direction
        Vector3 forward = player.transform.forward;
        if (rotationAxis == Vector3.up)
        {
            return Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
        }
        
        // Fallback using the PLAYER's rotation
        return Quaternion.Angle(Quaternion.identity, player.transform.rotation);
    }

    private void OnFullRotationCompleted(float direction)
    {
        string dirString = direction > 0 ? "Clockwise" : "Counter-Clockwise";
        Debug.Log($"Completed full rotation #{fullRotationsCount} ({dirString})!");
    }
}