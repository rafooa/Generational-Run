using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using UnityEngine.UIElements;
using System.Collections.Generic;
using static UnityEditor.PlayerSettings;
using Unity.VisualScripting;

public class HeroKnight : MonoBehaviour
{

    public Camera mainCamera;
    public LineRenderer _lineRenderer;
    public DistanceJoint2D _distanceJoint;

    [SerializeField] float m_speed = 4.0f;
    [SerializeField] float rightspeed = 2.5f;
    [SerializeField] float leftspeed = 4.0f;
    [SerializeField] float m_jumpForce = 7.5f;
    [SerializeField] float m_rollForce = 6.0f;
    [SerializeField] bool m_noBlood = false;
    [SerializeField] GameObject m_slideDust;

    private Animator m_animator;
    private Rigidbody2D m_body2d;
    private Sensor_HeroKnight m_groundSensor;
    private Sensor_HeroKnight m_wallSensorR1;
    private Sensor_HeroKnight m_wallSensorR2;
    private Sensor_HeroKnight m_wallSensorL1;
    private Sensor_HeroKnight m_wallSensorL2;
    private bool m_isWallSliding = false;
    private bool m_grounded = false;
    private bool m_rolling = false;
    private int m_facingDirection = 1;
    private int m_currentAttack = 0;
    private float m_timeSinceAttack = 0.0f;
    private float m_delayToIdle = 0.0f;
    public float m_rollDuration = 8.0f / 14.0f;
    private float m_rollCurrentTime = 0;
    public bool canSwing = false;

    public float hookReach = 10f;
    public float hookspeed = 12f;
    public float hookspeedstep = 0.1f;
    public float hookstopspeedstep = 0.1f;
    private Transform hookpos;
    float tempspeed = 0;
    bool slung = false;
    public float recentreSpeed = 0.1f;

    public bool isAttacking = false;
    private float parryWindow = 0.5f;


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







    public BoxCollider2D coll;
    static public bool doubleJump = false;
    public float fallspeed;
    public float slingspeed;
    private bool wallcheck = false;
    private bool ewall = false;
    public float wallBoost;


    // Use this for initialization
    void Start()
    {
        Application.targetFrameRate = 60;
        _distanceJoint.enabled = false;
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_HeroKnight>();
    }

    // Update is called once per frame
    void Update()
    {
        float minDist = 1000;
        if(!_distanceJoint.enabled)
        {
            GameObject[] banners = GameObject.FindGameObjectsWithTag("Banner");
            foreach (GameObject obj in banners)
            {

                float dist = Vector2.Distance(transform.position, obj.transform.position);

                if (dist < hookReach)
                {
                    canSwing = true;

                    if (minDist > dist)
                    {
                        minDist = dist;
                        hookpos = obj.transform;
                    }

                }

            }
        }
       

        if (canSwing)
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                Vector2 Pos = (Vector2)hookpos.position;
                _lineRenderer.SetPosition(0, Pos);
                _lineRenderer.SetPosition(1, _lineRenderer.gameObject.transform.position);
                _distanceJoint.connectedAnchor = Pos;
                _distanceJoint.enabled = true;
                _lineRenderer.enabled = true;

            }

            if (Input.GetKeyUp(KeyCode.G))
            {
                _distanceJoint.enabled = false;
                _lineRenderer.enabled = false;
                canSwing = false;
                slung = true;
                tempspeed = 0;
            }

        }
        if (_distanceJoint.enabled)
        {
            float dist = Vector2.Distance(transform.position, hookpos.position);
            if (dist > 8f && tempspeed > hookspeed)
            {
                _distanceJoint.enabled = false;
                _lineRenderer.enabled = false;
                canSwing = false;
                tempspeed = 0;
            }
            else
            {
                _lineRenderer.SetPosition(0, (Vector2)hookpos.position);
                _lineRenderer.SetPosition(1, _lineRenderer.gameObject.transform.position);
                if (tempspeed < hookspeed)
                    tempspeed += hookspeedstep;
                if (m_body2d.velocity.y > 5f)
                {
                    m_body2d.velocity = new Vector2(m_speed + tempspeed, 5f);
                }
                else
                {
                    m_body2d.velocity = new Vector2(2f * m_speed + tempspeed, m_body2d.velocity.y);
                }



            }



        }



        // Increase timer that controls attack combo
        m_timeSinceAttack += Time.deltaTime;

        // Increase timer that checks roll duration
        if (m_rolling)
            m_rollCurrentTime += Time.deltaTime;

        // Disable rolling if timer extends duration
        if (m_rollCurrentTime > m_rollDuration)
        {
            coll.offset = new Vector2(0f, 0.662f);
            coll.size = new Vector2(0.73f, 1.2f);
            m_rolling = false;
            m_rollCurrentTime = 0f;
        }
           

        //Check if character just landed on the ground
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            slung = false;
            doubleJump = true;
            m_animator.SetBool("Grounded", m_grounded);
        }

        

        //Check if character just started falling
        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        // -- Handle input and movement --
        float inputX = Input.GetAxis("Horizontal");
        float speed = 0f;
        // Swap direction of sprite depending on walk direction
        if (inputX > 0)
        {
            speed = rightspeed;
            GetComponent<SpriteRenderer>().flipX = false;
            m_facingDirection = 1;
        }

        else if (inputX < 0)
        {
            speed = leftspeed;
            GetComponent<SpriteRenderer>().flipX = true;
            m_facingDirection = -1;
        }
       
        if (!m_rolling && !_distanceJoint.enabled)
        {
            //if(!slung)
            //{
                if (m_body2d.velocity.y < 0)
                {
                    m_body2d.velocity = new Vector2(inputX * speed, m_body2d.velocity.y + (-1 * Time.fixedDeltaTime * fallspeed));
                }
                else
                {
                    m_body2d.velocity = new Vector2(inputX * speed, m_body2d.velocity.y);
                }
           // }
            //else
            //{
            //    m_body2d.velocity = new Vector2(m_body2d.velocity.x + (Time.fixedDeltaTime * slingspeed), m_body2d.velocity.y);
            //}

        }





        //Set AirSpeed in animator
        m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);

        // -- Handle Animations --
        //Wall Slide

        m_isWallSliding = (m_wallSensorR1.State() && m_wallSensorR2.State()) || (m_wallSensorL1.State() && m_wallSensorL2.State());
        m_animator.SetBool("WallSlide", m_isWallSliding);
    
        
        

        //Death
        if (Input.GetKeyDown("e") && !m_rolling)
        {
            m_animator.SetBool("noBlood", m_noBlood);
            m_animator.SetTrigger("Death");
        }

        //Hurt
        else if (Input.GetKeyDown("q") && !m_rolling)
            m_animator.SetTrigger("Hurt");

        //Attack
        else if (Input.GetMouseButtonDown(0) && m_timeSinceAttack > 1.5f && !m_rolling) // CHANGED TIME SINCE ATTACK FOR PARRY COOLDOWN
        {
            m_currentAttack++;

            // Loop back to one after third attack
            if (m_currentAttack > 3)
                m_currentAttack = 1;

            // REMOVED RESET COMBO

            // Call one of three attack animations "Attack1", "Attack2", "Attack3"
            m_animator.SetTrigger("Attack" + m_currentAttack);
            StartCoroutine(ParryAttack());
            // Reset timer
            m_timeSinceAttack = 0.0f;
        }

        // Block
        else if (Input.GetMouseButtonDown(1) && !m_rolling)
        {
            m_animator.SetTrigger("Block");
            m_animator.SetBool("IdleBlock", true);
        }

        else if (Input.GetMouseButtonUp(1))
            m_animator.SetBool("IdleBlock", false);

        // Roll
        else if (Input.GetKeyDown("left shift") && !m_rolling && !m_isWallSliding)
        {
            m_rolling = true;
            m_animator.SetTrigger("Roll");
            coll.offset = new Vector2(0f, 0.3683839f);
            coll.size = new Vector2(0.73f, 0.6127678f);
            m_body2d.velocity = new Vector2(m_facingDirection * m_rollForce, m_body2d.velocity.y);
        }


        //Jump
        else if ((Input.GetKeyDown("space") && m_grounded && !m_rolling) || Input.GetKeyDown("space") && doubleJump)
        {
            float jumpx = m_body2d.velocity.x;
            float jumpy = m_jumpForce;
            if (!m_grounded)
            {
                doubleJump = false;
                
            }
            if(wallcheck && !m_grounded)
            {
                if(!ewall)
                    jumpy += wallBoost/3;
                jumpx = wallBoost;
                Debug.Log("boost");
            }

            dust.Play();
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            if (gravityOn)
            {
                m_body2d.velocity = new Vector2(jumpx, jumpy);
            }
            else m_body2d.velocity = new Vector2(jumpx, -jumpy);
            m_groundSensor.Disable(0.2f);
        }

        //Run
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            // Reset timer
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
        }

        //Idle
        else
        {
            // Prevents flickering transitions to idle
            m_delayToIdle -= Time.deltaTime;
            if (m_delayToIdle < 0)
            {
                //GetComponent<SpriteRenderer>().flipX = false;
                //m_facingDirection = 1;
                m_animator.SetInteger("AnimState", 1);

            }

        }

        if (inputX == 0 && (6.38 - (-1f * transform.position.x)) > 0.5f && m_grounded)
        {
            //m_body2d.velocity = new Vector2(-1 * Time.fixedDeltaTime * recentreSpeed, m_body2d.velocity.y);
        }

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
                m_body2d.gravityScale = 1;
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

        if (Input.GetKeyDown("1") && gravityOn == true && cooldownON == false && m_grounded == true)
        {
            gravityOn = false;
            m_body2d.gravityScale = (float)-2;
            src.PlayOneShot(clip1);
            StartCoroutine(flip(gravityOn));
        }
        if (Input.GetKeyDown("2") && gravityOn == false)
        {
            gravityOn = true;
            usage = timeU;
            cooldownON = true;
            m_body2d.gravityScale = 1;
            src.PlayOneShot(clip2);
            StartCoroutine(flip(gravityOn));
        }

    }

    IEnumerator flip(bool grav)
    {
        dust.Play();
        if (grav == true)
        {
            yield return new WaitForSeconds((float)0.2);
            if (gameObject.transform.position.y > 4.4)
            {
                gameObject.transform.position = new Vector2(gameObject.transform.position.x, (float)-3.9);
            }
            gameObject.transform.Rotate(-180, 0, 0);
        }
        else if (grav == false)
        {
            yield return new WaitForSeconds((float)0.4);
            gameObject.transform.Rotate(-180, 0, 0);
        }
    }
    IEnumerator ParryAttack()
    {
        isAttacking = true;

        while (parryWindow > Mathf.Epsilon)
        {
            parryWindow -= Time.deltaTime;
            yield return null;
        }

        parryWindow = 0.5f;
        isAttacking = false;
        yield break;
    }


    // Animation Events
    // Called in slide animation.
    void AE_SlideDust()
    {
        Vector3 spawnPosition;

        if (m_facingDirection == 1)
            spawnPosition = m_wallSensorR2.transform.position;
        else
            spawnPosition = m_wallSensorL2.transform.position;

        if (m_slideDust != null)
        {
            // Set correct arrow spawn position
            GameObject dust = Instantiate(m_slideDust, spawnPosition, gameObject.transform.localRotation) as GameObject;
            // Turn arrow in correct direction
            dust.transform.localScale = new Vector3(m_facingDirection, 1, 1);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("EWall"))
        {
            //m_body2d.velocity = new Vector2(m_body2d.velocity.x, 1f);
            doubleJump = true;
            wallcheck = true;
            ewall = true;
            //m_animator.SetTrigger("Wall Slide");
        }
        if (collision.gameObject.CompareTag("Wall"))
        {
            //m_body2d.velocity = new Vector2(m_body2d.velocity.x, 1f);
            doubleJump = true;
            wallcheck = true;
            //m_animator.SetTrigger("Wall Slide");
        }

    }

    private void OnCollisionStay(UnityEngine.Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("EWall"))
        {
            m_body2d.velocity = Vector2.Lerp(new Vector2(m_body2d.velocity.x, 0f), new Vector2(m_body2d.velocity.x, m_body2d.velocity.x),0.07f);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("EWall"))
            wallcheck = false;
    }
}
