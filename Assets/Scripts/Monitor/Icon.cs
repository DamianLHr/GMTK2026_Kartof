using UnityEngine;
using UnityEngine.EventSystems;


public class Icon : MonoBehaviour
{
    [SerializeField] private bool internetTab;
    [SerializeField] private GameObject tabPrefab;
    [SerializeField] private GameObject lowPriorityGroup;

    public void SpawnTab()
    {
        if (internetTab && !PuzzleOrchestrator.Internet)
        {
            GetComponent<SpawnError>().Spawn();
        }
        else
        {
            GameObject tab = Instantiate(tabPrefab, transform);
            tab.transform.SetParent(lowPriorityGroup.transform);
            tab.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        }
    }
}
