using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LaserMovement : MonoBehaviour
{
    public TurretGenerator turrGen;
    public Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.left * turrGen.currSpeed * Time.deltaTime);
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    Debug.Log("TESTING 1");
    //    if (collision.gameObject.name == "StartLine" || collision.gameObject.CompareTag("StartLine"))
    //    {
    //        turrGen.GenerateLaser();
    //    }

    //    if (collision.gameObject.CompareTag("FinishLine"))
    //    {
    //        Destroy(this.gameObject);
    //    }

    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        Debug.Log("TESTINGGGGGGG");
    //    }
    //}


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("TESTING 2");
        if (collision.gameObject.CompareTag("StartLine"))
        {
            turrGen.GenerateLaser();
        }

        if (collision.gameObject.CompareTag("FinishLine"))
        {
            Destroy(this.gameObject);
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("TESTINGGGGGGG");
        }
    }
}
