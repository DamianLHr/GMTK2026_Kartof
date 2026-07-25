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
    
    public static bool FACodeCorrect;
    public static bool PasswordCorrect;
    public static bool RouterIDCorrect;
    public static bool CaptchaCorrect;
    public static bool Submitted;
    
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

    private void Update()
    {
        if (Submitted)
        {
            //This is what happens if you manage to submit the assignment/win game
            Debug.Log("YOU WON!!! YIPPEEE!!!");
        }
    }
    
}
