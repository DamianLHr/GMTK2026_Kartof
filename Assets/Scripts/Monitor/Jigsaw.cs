using UnityEngine;

public class Jigsaw : MonoBehaviour
{
    [SerializeField] private GameObject[] jigsawPrefab;
    public float distanceThreshold = 350f; 
    public float spawnRadius = 100f;
    
    private GameObject[] jigsaws;
    private bool isComplete = false;

    void Start()
    {
        jigsaws = new GameObject[4];
        int randomJigsaw = Random.Range(0, 4);
        for(int i = 0; i < jigsaws.Length; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;

            GameObject spawnedJigsaw = Instantiate(jigsawPrefab[randomJigsaw + i*4], transform);
            
            spawnedJigsaw.transform.localPosition = randomOffset;

            jigsaws[i] = spawnedJigsaw;
        }
    }

    void Update()
    {
        if (isComplete) return;

        if (CheckIfPuzzleIsComplete())
        {
            isComplete = true;
            JigsawComplete();
        }
    }

    private bool CheckIfPuzzleIsComplete()
    {
        if (jigsaws == null || jigsaws.Length == 0) 
        {
            return false;
        }

        Vector3 referencePosition = jigsaws[0].transform.position;

        for (int i = 0; i < jigsaws.Length; i++)
        {
            GameObject jigsaw = jigsaws[i];

            if (jigsaw == null)
            {
                return false;
            }

            RotateUI rotateUI = jigsaw.GetComponent<RotateUI>();
            if (rotateUI == null)
            {
                return false;
            }

            if (rotateUI.correctState != rotateUI.jigsawState)
            {
                return false;
            }

            float distance = Vector3.Distance(referencePosition, jigsaw.transform.position);
            if (distance > distanceThreshold)
            {
                return false;
            }
        }

        return true; 
    }

    private void JigsawComplete()
    {
        Debug.Log("Jigsaw Complete!");
    }
}