using UnityEngine;

// A distraction: spawns annoying pop-ups when the DistractionManager schedules it.
// No longer spawns on its own - the scheduler decides when and how often.
public class SpawnPopUps : Distraction
{
    [Header("Settings")]
    [SerializeField] private GameObject[] popUpPrefabs;
    [SerializeField] private int[] x = new int[2];
    [SerializeField] private int[] y = new int[2];

    [Tooltip("How many pop-ups to spawn each time this distraction fires.")]
    [SerializeField] private int burstCount = 1;

    public override void Trigger()
    {
        for (int i = 0; i < burstCount; i++)
            SpawnOne();
    }

    private void SpawnOne()
    {
        if (popUpPrefabs == null || popUpPrefabs.Length == 0) return;

        GameObject newPopUp = Instantiate(popUpPrefabs[Random.Range(0, popUpPrefabs.Length)], transform, false);

        RectTransform rectTransform = newPopUp.GetComponent<RectTransform>();
        if (rectTransform != null)
            rectTransform.anchoredPosition = new Vector2(Random.Range(x[0], x[1]), Random.Range(y[0], y[1]));
    }
}
