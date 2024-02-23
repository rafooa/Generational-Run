using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gravityMechanic : MonoBehaviour
{
    public GameObject player;
    public Rigidbody2D body;
    public Animator m_animator;
    bool gravityOn = true;
    float usage = 2;
    float cooldown = 2;
    bool cooldownON = false;
    float timeCD = 2;
    float timeU = 2;

    public GameObject jumpEffect;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!gravityOn)
        {
            if (usage >= 0)
            {
                usage -= Time.deltaTime;
            }
            else
            {
                usage = timeU;
                gravityOn = true;
                cooldownON = true;
                body.gravityScale = 3;
                StartCoroutine(flip(gravityOn));
            }
        }

        if (cooldownON)
        {
            if (cooldown >= 0)
            {
                cooldown -= Time.deltaTime;
            }
            else
            {
                cooldown = timeCD;
                cooldownON = false;
            }
        }

        if (Input.GetKeyDown("1") && gravityOn == true && cooldownON == false)
        {
            gravityOn = false;
//            GameObject temp = Instantiate(jumpEffect, new Vector2(player.transform.position.x,(float)-3.27), transform.rotation);
//            StartCoroutine(destroyEffect(temp));
            body.gravityScale = (float)-2;
            StartCoroutine(flip(gravityOn));
//            player.transform.Rotate(180, 0, 0);

        }
        if (Input.GetKeyDown("2") && gravityOn == false) 
        {
            gravityOn = true;
            usage = timeU;
            cooldownON = true;
//            GameObject temp = Instantiate(jumpEffect, new Vector2(player.transform.position.x, (float)3.44), player.transform.rotation);
//            StartCoroutine(destroyEffect(temp));
            body.gravityScale = 3;
            StartCoroutine(flip(gravityOn));
//            player.transform.Rotate(-180, 0, 0);
        }

    }

    IEnumerator flip(bool grav)
    {
        yield return new WaitForSeconds((float)0.2);
        if (grav == true)
        {
            Animator m = player.GetComponent<Animator>();
//            Debug.Log("Animator ki info is : " + m_animator.);
            m.SetTrigger("Jump");
            player.transform.Rotate(-180, 0, 0);
        }
        else if (grav == false)
        {
            player.transform.Rotate(-180, 0, 0);
        }
    }

    IEnumerator destroyEffect(GameObject efect)
    {
        yield return new WaitForSeconds((float)0.2);
        Destroy(efect);
    }
}
