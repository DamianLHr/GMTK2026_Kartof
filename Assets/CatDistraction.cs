using UnityEngine;

public class CatDistraction : Distraction
{
    
    public override void Trigger()
    {
        Debug.Log("CatDistraction Trigger");
        transform.GetChild(0).gameObject.SetActive(true);
    }
}
