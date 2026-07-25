using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SubmitController : MonoBehaviour
{
    [SerializeField] private SpawnError errorScript;
    [SerializeField] private GameObject doItButton;
    [SerializeField] private float cooldown = 0.5f;
    [SerializeField] private bool onCooldown = false;
    
    public void Submit()
    {
        if(PuzzleOrchestrator.correctFile) 
            doItButton.SetActive(true);
        else
            StartCoroutine(errorScript.SpawnCoroutine());
    }

    public void DoItButton()
    {
        PuzzleOrchestrator.Submitted = true;
    }

    private void Update()
    {
        if (doItButton.activeSelf && !onCooldown)
            StartCoroutine(ChangePosition());
    }

    public IEnumerator ChangePosition()
    {
        onCooldown = true;
        doItButton.GetComponent<RectTransform>().localPosition = new Vector2(Random.Range(-385, 385), Random.Range(-160, 160));
        yield return new WaitForSeconds(cooldown);
        onCooldown = false;
    }
}
