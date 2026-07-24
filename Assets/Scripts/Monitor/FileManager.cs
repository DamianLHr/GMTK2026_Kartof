using UnityEngine;
using TMPro;

public class FileManager : MonoBehaviour
{
    public GameObject[] typeOfFiles;
    public int nrOfFiles = 36; // Set your total number of files here
    
    private GameObject[] currentFiles;
    private TextMeshProUGUI[] textDisplay;
    private int specialNumber;
    
    [Header("Text Generation Settings")]
    [SerializeField] private string[] wordPool;
    [SerializeField] private string[] exclamationPool;
    [SerializeField] private int wordsToGenerate = 5;
    [SerializeField] private int exclamationsToGenerate = 5;
    [SerializeField] private string specialText;

    void Start()
    {
        currentFiles = new GameObject[nrOfFiles];
        textDisplay = new TextMeshProUGUI[nrOfFiles];

        specialNumber = Random.Range(0, nrOfFiles);
        
        for (int i = 0; i < nrOfFiles; i++)
        {
            currentFiles[i] = Instantiate(typeOfFiles[Random.Range(0, typeOfFiles.Length)], transform, false);
            
            textDisplay[i] = currentFiles[i].GetComponentInChildren<TextMeshProUGUI>();
            
            if (i == specialNumber)
            {
                textDisplay[i].text = specialText;
                currentFiles[i].name = specialText;
            }
            else
            {
                GenerateRandomSentence(textDisplay[i], currentFiles[i]);
            }
        }
    }
    
    public void GenerateRandomSentence(TextMeshProUGUI text, GameObject fileObject)
    {
        if (wordPool == null || wordPool.Length == 0)
        {
            Debug.LogWarning("Word pool is empty!");
            return;
        }

        string finalSentence = "";

        int actualNumber = Random.Range(1, wordsToGenerate + 1); 
        for (int i = 0; i < actualNumber; i++)
        {
            int randomIndex = Random.Range(0, wordPool.Length);
            finalSentence += wordPool[randomIndex] + " ";
        }
        
        actualNumber = Random.Range(0, exclamationsToGenerate + 1);
        for (int i = 0; i < actualNumber; i++)
        {
            int randomIndex = Random.Range(0, exclamationPool.Length);
            finalSentence += exclamationPool[randomIndex] + " ";
        }
        
        string finishedText = finalSentence.TrimEnd();
        
        text.text = finishedText;
        
        fileObject.name = finishedText; 
    }
}