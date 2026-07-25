using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private Vector2 respawn;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Respawn")
            transform.position = respawn;
    }
    
}
