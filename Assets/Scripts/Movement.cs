using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Runtime.CompilerServices;

public class Movement : MonoBehaviour
{
    

    private Collision coll;
    [HideInInspector]
    public Rigidbody2D rb;
    private AnimationScript anim;

    [Space]
    [Header("Stats")]
    public float speed = 10;
    public float jumpForce = 50;
    public float WalljumpForce = 10;
    public float slideSpeed = 5;
    public float wallJumpLerp = 10;
    public float dashSpeed = 20;

    [Space]
    [Header("Booleans")]
    public bool canMove;
    public bool wallGrab;
    public bool wallJumped;
    public bool wallSlide;
    public bool isDashing;

    [Space]

    private bool groundTouch;
    private bool hasDashed;

    public int side = 1;

    [Space]
    [Header("Polish")]
    public ParticleSystem dashParticle;
    public ParticleSystem jumpParticle;
    public ParticleSystem wallJumpParticle;
    public ParticleSystem slideParticle;

    //abdullah
    [Space]
    [Header("Abdullah")]
    public Camera mainCamera;
    public LineRenderer _lineRenderer;
    public DistanceJoint2D _distanceJoint;
    public bool canSwing = false;
    public float hookReach = 10f;
    public float hookspeed = 12f;
    public float hookspeedstep = 0.1f;
    public float hookstopspeedstep = 0.1f;
    private Transform hookpos;
    float tempspeed = 0;
    bool slung = false;

    //huzaifa
    [Space]
    [Header("Huzaifa")]
    public bool gravityOn = true;
    public float usage = 2;
    float cooldown = 2;
    bool cooldownON = false;
    float timeCD = 2;
    float timeU = 2;
    public ParticleSystem dust;
    public AudioSource src;
    public AudioClip clip1;
    public AudioClip clip2;
    public AudioClip walking;
    public AudioClip jumping;
    public AudioClip dashing;
    public AudioClip parrying;
    private float gravSign = 1f;
    Vector2 oldColl;

    //rafay
    [Space]
    [Header("Rafay")]
    public bool isAttacking = false;
    private float parryWindow = 0.5f;
    private float m_timeSinceAttack = 0f;

    //hassan

    

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        _distanceJoint.enabled = false;
        coll = GetComponent<Collision>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<AnimationScript>();
    }

    // Update is called once per frame
    void Update()
    {
        Grapple();

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");
        Vector2 dir = new Vector2(x, y);

        Walk(dir);
        if (x != 0)
        {
            if (!src.isPlaying) src.PlayOneShot(walking);
            anim.SetHorizontalMovement(x, y, rb.velocity.y);
        }

        if (coll.onWall && Input.GetButton("Fire3") && canMove)
        {
            if (side != coll.wallSide)
                //anim.Flip(side*-1);
                wallGrab = true;
            wallSlide = false;
        }

        if (Input.GetButtonUp("Fire3") || !coll.onWall || !canMove)
        {
            wallGrab = false;
            wallSlide = false;
        }

        if (coll.onGround && !isDashing)
        {
            wallJumped = false;
            GetComponent<BetterJumping>().enabled = true;
        }

        if (wallGrab && !isDashing)
        {
            rb.gravityScale = 0;
            if (x > .2f || x < -.2f)
                rb.velocity = new Vector2(rb.velocity.x, 0);

            float speedModifier = y > 0 ? .5f : 1;

            rb.velocity = new Vector2(rb.velocity.x, y * (speed * speedModifier));
        }
        else
        {
            rb.gravityScale = 3 * gravSign;
        }

        if (coll.onWall && !coll.onGround)
        {
            if (x != 0 && !wallGrab)
            {
                wallSlide = true;
                WallSlide();
            }
        }

        if (!coll.onWall || coll.onGround)
            wallSlide = false;

        if (Input.GetButtonDown("Jump"))
        {
            anim.SetTrigger("jump");
            src.PlayOneShot(jumping);

            if (coll.onGround)
            {
                if (gravityOn)
                {
                    Jump(Vector2.up, false);
                }
                else
                {
                    Jump(Vector2.down, false);
                }
            }

            if (coll.onWall && !coll.onGround)
                WallJump();
        }

        if (Input.GetButtonDown("Fire1") && !hasDashed)
        {
            src.PlayOneShot(dashing);
            if (xRaw != 0 || yRaw != 0)
                Dash(xRaw, yRaw);
        }

        if (coll.onGround && !groundTouch)
        {
            GroundTouch();
            groundTouch = true;
        }

        if (!coll.onGround && groundTouch)
        {
            groundTouch = false;
        }

        WallParticle(y);

        if (wallGrab || wallSlide || !canMove)
            return;

        if (x > 0)
        {
            side = 1;
            anim.Flip(side);
        }
        if (x < 0)
        {
            side = -1;
            anim.Flip(side);
        }


        Gravity();

        m_timeSinceAttack += Time.deltaTime;

        
        Attack();

    }

    void Grapple()
    {
        float minDist = 1000;
        if (!_distanceJoint.enabled)
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
            if (Input.GetButtonDown("grapple"))
            {
                Vector2 Pos = (Vector2)hookpos.position;
                _lineRenderer.SetPosition(0, Pos);
                _lineRenderer.SetPosition(1, _lineRenderer.gameObject.transform.position);
                _distanceJoint.connectedAnchor = Pos;
                _distanceJoint.enabled = true;
                _lineRenderer.enabled = true;
                

            }

            else if (Input.GetButtonUp("grapple"))
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
                if (rb.velocity.y > 5f)
                {
                    rb.velocity = new Vector2(speed + tempspeed, 5f);
                }
                else
                {
                    rb.velocity = new Vector2(2f * speed + tempspeed, rb.velocity.y);
                }



            }



        }
    }
    void Gravity()
    {
        /*        if (!gravityOn)
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

        */
        
            
        if (Input.GetButtonDown("Fire3") && gravityOn == true)
        {
            Debug.Log("tri");
            gravityOn = false;
            gravSign = -1f;
            rb.gravityScale *= gravSign;
            oldColl = coll.bottomOffset;
            coll.bottomOffset = new Vector2(coll.bottomOffset.x, 1f);
            src.PlayOneShot(clip1);
            StartCoroutine(flip(gravityOn));
        }
        else if (Input.GetButtonDown("Fire3") && gravityOn == false)
        {
            gravityOn = true;
            usage = timeU;
//            cooldownON = true;
            gravSign = 1f;
            rb.gravityScale = 3 * gravSign;
            coll.bottomOffset = oldColl;
            src.PlayOneShot(clip2);
            StartCoroutine(flip(gravityOn));
        }
    }

     void Attack()
    {
        if (Input.GetButtonDown("Fire2") && m_timeSinceAttack > 1.5f /*&& !m_rolling*/) // CHANGED TIME SINCE ATTACK FOR PARRY COOLDOWN
        {
            src.PlayOneShot(parrying);
            //m_currentAttack++;

            //// Loop back to one after third attack
            //if (m_currentAttack > 3)
            //    m_currentAttack = 1;

            // REMOVED RESET COMBO

            // Call one of three attack animations "Attack1", "Attack2", "Attack3"
            anim.SetTrigger("attack");
            StartCoroutine(ParryAttack());
            // Reset timer
            m_timeSinceAttack = 0.0f;
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
            //gameObject.transform.position = new Vector2(gameObject.transform.position.x, 7f);
        }
        else if (grav == false)
        {
            
            yield return new WaitForSeconds(0.6f);
                        gameObject.transform.Rotate(0, 0, -180);
            //gameObject.transform.position = new Vector2(gameObject.transform.position.x, 7f);

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

    void GroundTouch()
    {
        hasDashed = false;
        isDashing = false;

        side = anim.sr.flipX ? -1 : 1;

        jumpParticle.Play();
    }

    private void Dash(float x, float y)
    {
        Camera.main.transform.DOComplete();
        Camera.main.transform.DOShakePosition(.2f, .5f, 14, 90, false, true);
        FindObjectOfType<RippleEffect>().Emit(Camera.main.WorldToViewportPoint(transform.position));

        hasDashed = true;

        anim.SetTrigger("dash");

        rb.velocity = Vector2.zero;
        Vector2 dir = new Vector2(x, y);

        rb.velocity += dir.normalized * dashSpeed;
        StartCoroutine(DashWait());
    }

    IEnumerator DashWait()
    {
        FindObjectOfType<GhostTrail>().ShowGhost();
        StartCoroutine(GroundDash());
        DOVirtual.Float(14, 0, .8f, RigidbodyDrag);

        dashParticle.Play();
        rb.gravityScale = 0;
        GetComponent<BetterJumping>().enabled = false;
        wallJumped = true;
        isDashing = true;

        yield return new WaitForSeconds(0.3f);

        dashParticle.Stop();
        rb.gravityScale = 3;
        GetComponent<BetterJumping>().enabled = true;
        wallJumped = false;
        isDashing = false;
    }

    IEnumerator GroundDash()
    {
        yield return new WaitForSeconds(.15f);
        if (coll.onGround)
            hasDashed = false;
    }

    private void WallJump()
    {
        if ((side == 1 && coll.onRightWall) || side == -1 && !coll.onRightWall)
        {
            side *= -1;
            anim.Flip(side);
        }

        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(.1f));

        Vector2 wallDir = coll.onRightWall ? Vector2.left : Vector2.right;

        Jump((Vector2.up / 1.5f + wallDir / 1.5f), true);

        wallJumped = true;
    }

    private void WallSlide()
    {
        if(coll.wallSide != side)
         anim.Flip(side * 1);

        if (!canMove)
            return;

        bool pushingWall = false;
        if((rb.velocity.x > 0 && coll.onRightWall) || (rb.velocity.x < 0 && coll.onLeftWall))
        {
            pushingWall = true;
        }
        float push = pushingWall ? 0 : rb.velocity.x;

        rb.velocity = new Vector2(push, -slideSpeed);
    }

    private void Walk(Vector2 dir)
    {
        if (!canMove)
            return;

        if (wallGrab)
            return;

        if (!wallJumped)
        {
            rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);
        }
        else
        {
            //Debug.Log("ok");
            rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(dir.x * speed, rb.velocity.y)), wallJumpLerp * Time.deltaTime);
            rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);
        }
    }

    private void Jump(Vector2 dir, bool wall)
    {
        slideParticle.transform.parent.localScale = new Vector3(ParticleSide(), 1, 1);
        ParticleSystem particle = wall ? wallJumpParticle : jumpParticle;

        rb.velocity = new Vector2(rb.velocity.x, 0);
        if (!wall)
        {
            if (gravityOn)
            {
                rb.velocity += dir * jumpForce;
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, -1 * jumpForce);
//                rb.velocity -= dir * jumpForce;

            }
        }
        else
            rb.velocity += dir * WalljumpForce;

        particle.Play();
    }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    void RigidbodyDrag(float x)
    {
        rb.drag = x;
    }

    void WallParticle(float vertical)
    {
        var main = slideParticle.main;

        if (wallSlide || (wallGrab && vertical < 0))
        {
            slideParticle.transform.parent.localScale = new Vector3(ParticleSide(), 1, 1);
            main.startColor = Color.white;
        }
        else
        {
            main.startColor = Color.clear;
        }
    }

    int ParticleSide()
    {
        int particleSide = coll.onRightWall ? 1 : -1;
        return particleSide;
    }
}
