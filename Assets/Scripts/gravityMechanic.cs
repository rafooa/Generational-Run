using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gravityMechanic : MonoBehaviour
{
    public GameObject player;
    public Rigidbody2D body;
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
                player.transform.Rotate(-180, 0, 0);
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
            player.transform.Rotate(180, 0, 0);
        }
        if (Input.GetKeyDown("2") && gravityOn == false && cooldownON == false) 
        {
            gravityOn = true;
            usage = timeU;
            cooldownON = true;
//            GameObject temp = Instantiate(jumpEffect, new Vector2(player.transform.position.x, (float)3.44), player.transform.rotation);
//            StartCoroutine(destroyEffect(temp));
            body.gravityScale = 3;
            player.transform.Rotate(-180, 0, 0);
        }

    }

    IEnumerator destroyEffect(GameObject efect)
    {
        yield return new WaitForSeconds((float)0.5);
        Destroy(efect);
    }
}
