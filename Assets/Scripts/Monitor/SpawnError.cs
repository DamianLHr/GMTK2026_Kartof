using UnityEngine;

public class SpawnError : MonoBehaviour
{
    public GameObject errorTab;
    public string text;
    
    private bool hasSpawned = false; 

    public void Spawn()
    {
        if (hasSpawned) 
            return;

        hasSpawned = true; 

        GameObject tab = Instantiate(errorTab, transform.parent.parent, false);
        tab.GetComponent<ChangeErrorText>().text = text;
        tab.transform.SetAsLastSibling();
    }
}