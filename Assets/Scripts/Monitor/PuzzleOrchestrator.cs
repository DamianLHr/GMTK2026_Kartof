using UnityEngine;
using TMPro;

public class PuzzleOrchestrator : MonoBehaviour
{
    [SerializeField] private int seed = 12345;
    
    public static int RouterID;
    public static string CanvasPassword;
    public static int FACode;
    [SerializeField] private TextMeshProUGUI routerIDText;
    [SerializeField] private TextMeshProUGUI canvasPasswordText;

    public static bool correctFile;
    public static bool FACodeCorrect;
    public static bool PasswordCorrect;
    public static bool RouterIDCorrect;
    public static bool CaptchaCorrect;
    public static bool Submitted;
    public static bool Internet = true;

    private bool hasTriggeredWin = false;
    
    private void Awake()
    {
        RouterID = Random.Range(1000, 10000);
        FACode = Random.Range(10, 100);
        CanvasPassword = GenerateRandomPassword(4);
        routerIDText.text += RouterID.ToString();
        canvasPasswordText.text += CanvasPassword;
    }

    private string GenerateRandomPassword(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        char[] result = new char[length];
        for (int i = 0; i < length; i++)
        {
            result[i] = chars[Random.Range(0, chars.Length)];
        }
        return new string(result);
    }

    private EventBinding<PlayerWonEvent> playerWon;
    private EventBinding<PlayerDeadEvent> playerDead;

    private void OnEnable()
    {
        playerWon = new EventBinding<PlayerWonEvent>(Won);
        playerDead = new EventBinding<PlayerDeadEvent>(Lost); 
        EventBus<PlayerWonEvent>.Register(playerWon);
        EventBus<PlayerDeadEvent>.Register(playerDead);
    }

    // I know there were better ways of doing it by calling events... but I didn't know when I started writing this and I am tired :(
    private void Update()
    {
        if (Submitted && !hasTriggeredWin)
        {
            hasTriggeredWin = true;
            EventBus<PlayerWonEvent>.Raise(new PlayerWonEvent());
        }
    }

    private void Won()
    {
        LevelLoader.Instance.LoadLevel("Victory Screen");
    }

    private void Lost()
    {
        LevelLoader.Instance.LoadLevel("Loss Screen");
    }

    private void OnDisable()
    {
        EventBus<PlayerWonEvent>.Deregister(playerWon);
        EventBus<PlayerDeadEvent>.Deregister(playerDead);
    }
}