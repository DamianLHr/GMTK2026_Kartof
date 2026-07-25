using System;
using System.Collections;
using UnityEngine;

public class SpawnMail : MonoBehaviour
{
    [SerializeField] private GameObject mail;
    [SerializeField] private float speed;
    [SerializeField] private float waitTime;
    private bool canSpawn = false;

    private void Start()
    {
        waitTime = UnityEngine.Random.Range(1f, 2f);
        InvokeRepeating("Spawn", waitTime, waitTime);
        
    }

    void Spawn()
    {
        GameObject spawnedMail = Instantiate(mail, transform.position, transform.rotation);
        spawnedMail.GetComponent<Rigidbody2D>().AddForce(spawnedMail.transform.right * speed, ForceMode2D.Impulse);
    }
}