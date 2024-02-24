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
    public float angleChangingSpeed;
    public Transform target;

    public float TTL;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().flipX = true;
        target = GameObject.FindGameObjectWithTag("Player").transform;
        rb.velocity = (target.position - transform.position).normalized * snipGen.currSpeed;
        TTL = Time.time;
        // Vector2 actualHitbox = new Vector2(target.transform.position.x, target.transform.position.y + 1f);
        // Vector2 point2Target = (Vector2)transform.position - actualHitbox;
        // point2Target.Normalize();
        // float value = Vector3.Cross(point2Target, transform.right).z;
        // rb.angularVelocity = angleChangingSpeed * value * -1;
        // rb.velocity = -1 * (point2Target * snipGen.currSpeed);

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("FinishLine"))
        {

            Destroy(this.gameObject);
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("HIT PLAYER");

            Destroy(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }


    }
}
