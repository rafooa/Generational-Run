using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public class bulletMovement : MonoBehaviour
{
    public sniperGenerator snipGen;
    public Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = new Vector2(-1 * snipGen.currSpeed, rb.velocity.y);
        GetComponent<SpriteRenderer>().flipX = true;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("FinishLine"))
        {
            snipGen.GenerateLaser();
            Destroy(this.gameObject);
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("HIT PLAYER");
            if (collision.gameObject.GetComponent<HeroKnight>().isAttacking)
            {
                Debug.Log("PARRYINGGG");
                GetComponent<SpriteRenderer>().flipX = false;
                rb.velocity = new Vector2(-1 * rb.velocity.x, rb.velocity.y);
            }
        }
    }
}
