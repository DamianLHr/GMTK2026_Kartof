using UnityEngine;

public class Lifetime : MonoBehaviour
{
    void Start()
    {
        Invoke(nameof(Destroy), 2);
    }
    
    void Destroy() {
        Destroy(gameObject);
    }
}
