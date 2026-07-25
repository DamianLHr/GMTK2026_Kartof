using System.Collections;
using UnityEngine;

public class SpawnError : MonoBehaviour
{
    public GameObject errorTab;
    public string text;
    
    private bool hasSpawned = false; 

    public void Spawn()
    {
        GameObject tab = Instantiate(errorTab, transform.parent.parent, false);
        tab.GetComponent<ChangeErrorText>().text = text;
        tab.transform.SetAsLastSibling();
    }

    public IEnumerator SpawnCoroutine()
    { 
        if(!hasSpawned)
            Spawn();
        hasSpawned = true;
        yield return new WaitForSeconds(0.2f);
        hasSpawned = false;
    }
}