using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class unparryLaser : MonoBehaviour
{
    private float timer = 2.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0.0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {


        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("HIT PLAYER");
        }
    }
}
