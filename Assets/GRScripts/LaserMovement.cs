using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public class LaserMovement : MonoBehaviour
{
    public TurretGenerator turrGen;
    public Rigidbody2D rb;
    public bool isDeflected = false;
    public GameObject explosion;
    public GameObject deflection;
    public Transform target;
    public float angleChangingSpeed;
    bool dead = false;
    GameObject exp;
    public SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().flipX = true;
        //rb.velocity = new Vector2(-1 * turrGen.currSpeed, rb.velocity.y);
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(dead)
        { 
            sr.enabled = false;
            return; 
        }
        Vector2 actualHitbox = new Vector2(target.transform.position.x, target.transform.position.y);
        Vector2 point2Target = (Vector2)transform.position - actualHitbox;
        point2Target.Normalize();
        float value = Vector3.Cross(point2Target, transform.right).z;
        rb.angularVelocity = angleChangingSpeed * value * -1;
        rb.velocity = -1 * (point2Target * turrGen.currSpeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        Debug.Log("coll");
        if (collision.gameObject.CompareTag("FinishLine"))
        {
            Instantiate(explosion, transform.position, transform.rotation);
            turrGen.GenerateLaser();
            Destroy(this.gameObject);
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("HIT PLAYER");
            if (collision.gameObject.GetComponent<Movement>().isAttacking)
            {
                Debug.Log("PARRYINGGG");
                //GetComponent<SpriteRenderer>().flipX = false;
                isDeflected = true;
                rb.velocity = new Vector2(turrGen.currSpeed, rb.velocity.y);
                //                target = GameObject.FindGameObjectWithTag("Enemy").transform;
                target = turrGen.gameObject.transform;
                Instantiate(deflection, transform.position, transform.rotation);
            }
            else 
            {
                Debug.Log("dead");
                exp = Instantiate(explosion, transform.position, transform.rotation);
                turrGen.GenerateLaser();
                dead = true;
                StartCoroutine(DeleteExp(exp));
                
            }
        }
        
        if (collision.gameObject.CompareTag("Enemy") && isDeflected)
        {
            exp = Instantiate(explosion, transform.position, transform.rotation);
            dead = true;
            //turrGen.GenerateLaser();
            StartCoroutine(DeleteExpNTurr(exp));
            
            
        }
    }

    IEnumerator DeleteExpNTurr(GameObject exp)
    {
        Destroy(turrGen.gameObject);
        yield return new WaitForSecondsRealtime(0.667f);
        Destroy(exp);
        
        Destroy(this.gameObject);
    }

    IEnumerator DeleteExp(GameObject exp)
    {
        yield return new WaitForSecondsRealtime(0.667f);
        Destroy(exp);
        Debug.Log("dead2");
        Destroy(this.gameObject);
    }

}
