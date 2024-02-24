using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class temp : MonoBehaviour
{

    [Space]
    [Header("Huzaifa")]
    bool gravityOn = true;
    float usage = 2;
    float cooldown = 2;
    bool cooldownON = false;
    float timeCD = 2;
    float timeU = 2;
    public ParticleSystem dust;
    public AudioSource src;
    public AudioClip clip1;
    public AudioClip clip2;
    private float gravSign = 1f;

    public Rigidbody2D rb;
    private Collision coll;


    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<Collision>();
    }

    // Update is called once per frame
    void Update()
    {
        Gravity();
    }

    void Gravity()
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
                gravSign = 1f;
                rb.gravityScale = 3 * gravSign;
                src.PlayOneShot(clip2);
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
            gravSign = -1f;
            rb.gravityScale *= gravSign;
            src.PlayOneShot(clip1);
            StartCoroutine(flip(gravityOn));
        }
        if (Input.GetKeyDown("2") && gravityOn == false)
        {
            gravityOn = true;
            usage = timeU;
            cooldownON = true;
            gravSign = 1f;
            rb.gravityScale = 3 * gravSign;
            src.PlayOneShot(clip2);
            StartCoroutine(flip(gravityOn));
        }
    }

    IEnumerator flip(bool grav)
    {
        dust.Play();
        if (grav == true)
        {
            yield return new WaitForSeconds(0.6f);
            if (gameObject.transform.position.y > 4.4)
            {
                gameObject.transform.position = new Vector2(gameObject.transform.position.x, (float)-3.9);
            }
            gameObject.transform.Rotate(0, 0, -180);
        }
        else if (grav == false)
        {
            yield return new WaitForSeconds(0.6f);
            gameObject.transform.Rotate(0, 0, -180);
        }
    }
}
