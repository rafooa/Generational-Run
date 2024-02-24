//using UnityEngine;
//using System.Collections;
//using UnityEngine.ParticleSystemJobs;
//using Unity.VisualScripting;

//public class HeroKnight : MonoBehaviour {

//    [SerializeField] float      m_speed = 4.0f;
//    [SerializeField] float      m_jumpForce = 7.5f;
//    [SerializeField] float      m_rollForce = 6.0f;
//    [SerializeField] bool       m_noBlood = false;
//    [SerializeField] GameObject m_slideDust;

//    private Animator            m_animator;
//    private Rigidbody2D         m_body2d;
//    private Sensor_HeroKnight   m_groundSensor;
//    private Sensor_HeroKnight   m_wallSensorR1;
//    private Sensor_HeroKnight   m_wallSensorR2;
//    private Sensor_HeroKnight   m_wallSensorL1;
//    private Sensor_HeroKnight   m_wallSensorL2;
//    private bool                m_isWallSliding = false;
//    private bool                m_grounded = false;
//    private bool                m_rolling = false;
//    private int                 m_facingDirection = 1;
//    private int                 m_currentAttack = 0;
//    private float               m_timeSinceAttack = 0.0f;
//    private float               m_delayToIdle = 0.0f;
//    private float               m_rollDuration = 8.0f / 14.0f;
//    private float               m_rollCurrentTime;

//    public bool isAttacking = false;
//    private float parryWindow = 0.5f;


//    bool gravityOn = true;
//    float usage = 2;
//    float cooldown = 2;
//    bool cooldownON = false;
//    float timeCD = 2;
//    float timeU = 2;
//    public ParticleSystem dust;
//    public AudioSource src;
//    public AudioClip clip1;
//    public AudioClip clip2;

//    // Use this for initialization
//    void Start ()
//    {
//        m_animator = GetComponent<Animator>();
//        m_body2d = GetComponent<Rigidbody2D>();
//        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
//        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>();
//        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>();
//        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_HeroKnight>();
//        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_HeroKnight>();
//    }

//    // Update is called once per frame
//    void Update ()
//    {

//        // Increase timer that controls attack combo
//        m_timeSinceAttack += Time.deltaTime;

//        // Increase timer that checks roll duration
//        if(m_rolling)
//            m_rollCurrentTime += Time.deltaTime;

//        // Disable rolling if timer extends duration
//        if(m_rollCurrentTime > m_rollDuration)
//            m_rolling = false;

//        //Check if character just landed on the ground
//        if (!m_grounded && m_groundSensor.State())
//        {
//            m_grounded = true;
//            m_animator.SetBool("Grounded", m_grounded);
//        }

//        //Check if character just started falling
//        if (m_grounded && !m_groundSensor.State())
//        {
//            m_grounded = false;
//            m_animator.SetBool("Grounded", m_grounded);
//        }

//        // -- Handle input and movement --
//        float inputX = Input.GetAxis("Horizontal");

//        // Swap direction of sprite depending on walk direction
//        if (inputX > 0)
//        {
//            GetComponent<SpriteRenderer>().flipX = false;
//            m_facingDirection = 1;
//        }
            
//        else if (inputX < 0)
//        {
//           // GetComponent<SpriteRenderer>().flipX = true;
//            m_facingDirection = -1;
//        }

//        // Move
//        if (!m_rolling)
//        {
//            if (m_facingDirection == 1)
//            {
//                m_body2d.velocity = new Vector2(inputX * m_speed * (float)0.8, m_body2d.velocity.y);
//            }
//            else if (m_facingDirection == -1)
//            {
//                m_body2d.velocity = new Vector2(inputX * m_speed * (float)2, m_body2d.velocity.y);
//            }
//        }

//        //Set AirSpeed in animator
//        m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);

//        // -- Handle Animations --
//        //Wall Slide
//        m_isWallSliding = (m_wallSensorR1.State() && m_wallSensorR2.State()) || (m_wallSensorL1.State() && m_wallSensorL2.State());
//        m_animator.SetBool("WallSlide", m_isWallSliding);

//        //Death
//        if (Input.GetKeyDown("e") && !m_rolling)
//        {
//            m_animator.SetBool("noBlood", m_noBlood);
//            m_animator.SetTrigger("Death");
//        }
            
//        //Hurt
//        else if (Input.GetKeyDown("q") && !m_rolling)
//            m_animator.SetTrigger("Hurt");

//        //Attack
//        else if(Input.GetMouseButtonDown(0) && m_timeSinceAttack > 1.5f && !m_rolling)
//        {
//            //m_currentAttack++;

//            //// Loop back to one after third attack
//            //if (m_currentAttack > 3)
//            //    m_currentAttack = 1;

//            //// Reset Attack combo if time since last attack is too large
//            //if (m_timeSinceAttack > 1.0f)
//            //    m_currentAttack = 1;

//            m_currentAttack = 1;

//            // Call one of three attack animations "Attack1", "Attack2", "Attack3"
//            m_animator.SetTrigger("Attack" + m_currentAttack);
//            StartCoroutine(ParryAttack());

//            // Reset timer
//            m_timeSinceAttack = 0.0f;
//        }

//        // Block
//        else if (Input.GetMouseButtonDown(1) && !m_rolling)
//        {
//            m_animator.SetTrigger("Block");
//            m_animator.SetBool("IdleBlock", true);
//        }

//        else if (Input.GetMouseButtonUp(1))
//            m_animator.SetBool("IdleBlock", false);

//        // Roll
//        else if (Input.GetKeyDown("left shift") && !m_rolling && !m_isWallSliding)
//        {
//            m_rolling = true;
//            m_animator.SetTrigger("Roll");
//            m_body2d.velocity = new Vector2(m_facingDirection * m_rollForce, m_body2d.velocity.y);
//        }
            

//        //Jump
//        else if (Input.GetKeyDown("space") && m_grounded && !m_rolling && gravityOn)
//        {
//            Debug.Log("I do be jumping");
//            dust.Play();
//            m_animator.SetTrigger("Jump");
//            m_grounded = false;
//            m_animator.SetBool("Grounded", m_grounded);
//            m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
//            m_groundSensor.Disable(0.2f);
//        }

//        //Run
//        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
//        {
//            // Reset timer
//            m_delayToIdle = 0.05f;
//            m_animator.SetInteger("AnimState", 1);
//        }

//        //Idle
//        else
//        {
//            // Prevents flickering transitions to idle
//            //            m_delayToIdle -= Time.deltaTime;
//            //                if(m_delayToIdle < 0)
//            //                    m_animator.SetInteger("AnimState", 0);
//            m_delayToIdle = 0.05f;
//            m_animator.SetInteger("AnimState", 1);
//        }



//        //-----------------------------------------------------------------------


//        if (!gravityOn)
//        {
//            if (usage >= 0)
//            {
//                usage -= Time.deltaTime;
//            }
//            else
//            {
//                usage = timeU;
//                gravityOn = true;
//                cooldownON = true;
//                m_body2d.gravityScale = 3;
//                src.PlayOneShot(clip2);
//                StartCoroutine(flip(gravityOn));
//            }
//        }

//        if (cooldownON)
//        {
//            if (cooldown >= 0)
//            {
//                cooldown -= Time.deltaTime;
//            }
//            else
//            {
//                cooldown = timeCD;
//                cooldownON = false;
//            }
//        }

//        if (Input.GetKeyDown("1") && gravityOn == true && cooldownON == false && m_grounded == true)
//        {
//            gravityOn = false;
//            m_body2d.gravityScale = (float)-4;
//            src.PlayOneShot(clip1);
//            StartCoroutine(flip(gravityOn));
//        }
//        if (Input.GetKeyDown("space") && gravityOn == false)
//        {
//            gravityOn = true;
//            usage = timeU;
//            cooldownON = true;
//            m_body2d.gravityScale = 3;
//            src.PlayOneShot(clip2);
//            StartCoroutine(flip(gravityOn));
//        }
        

//    }

//    IEnumerator flip(bool grav)
//    {
//        dust.Play();
//        if (grav == true)
//        {
//            yield return new WaitForSeconds((float)0.2);
//            if (gameObject.transform.position.y > 4.4)
//            {
//                gameObject.transform.position = new Vector2(gameObject.transform.position.x, (float)-3.9);
//            }
//            gameObject.transform.Rotate(-180, 0, 0);
//        }
//        else if (grav == false)
//        {
//            yield return new WaitForSeconds((float)0.4);
//            gameObject.transform.Rotate(-180, 0, 0);
//        }
//    }
//    IEnumerator ParryAttack()
//    {
//        isAttacking = true;

//        while (parryWindow > Mathf.Epsilon)
//        {
//            parryWindow -= Time.deltaTime;
//            yield return null;
//        }

//        parryWindow = 0.5f;
//        isAttacking = false;
//        yield break;
//    }
//}
