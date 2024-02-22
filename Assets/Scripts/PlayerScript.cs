using System.Collections;
using UnityEngine;
public class PlayerScript : MonoBehaviour
{
    public float jumpForce = 500f;
    public bool isGrounded = false;
    public bool isParrying = false;
    public Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask groundLayer;
    private float groundCheckRadius = 1.75f;
    public Animator anim;
    public RuntimeAnimatorController jumpAnim, runAnim, parryAnim;
    public float parryWindow = 0.5f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded && !isParrying)
            anim.runtimeAnimatorController = runAnim;
        
        if (Input.GetButton("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(0, jumpForce);
            isGrounded = false;
            anim.runtimeAnimatorController = jumpAnim;
        }
        else if(Input.GetKey(KeyCode.Q) && isGrounded && !isParrying)
        {
            StartCoroutine(ParryAttack());
        }
        else if (Input.GetKey("d"))
        {
            //move right
            transform.position += transform.right * 3 * Time.deltaTime;
        }
        else if (Input.GetKey("a"))
        {
            //move left
            transform.position += -transform.right * 3 * Time.deltaTime;
        }
    }
    IEnumerator ParryAttack()
    {
        isParrying = true;
        anim.runtimeAnimatorController = parryAnim;

        while (parryWindow > Mathf.Epsilon)
        {
            parryWindow -= Time.deltaTime;
            yield return null;
        }

        parryWindow = 0.5f;
        isParrying = false;
        yield break;
    }
}
