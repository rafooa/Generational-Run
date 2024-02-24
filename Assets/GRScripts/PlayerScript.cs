using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
public class PlayerScript : MonoBehaviour
{
    public float jumpForce = 500f;
    public float recentreSpeed = 0.1f;
    public bool isGrounded = false;
    public bool isParrying = false;
    public Rigidbody2D rb;
    public Transform playerpos;
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
        isGrounded = Physics2D.OverlapCircle(playerpos.position, groundCheckRadius, groundLayer);
        if (isGrounded && !isParrying)
        {
            anim.runtimeAnimatorController = runAnim;
            rb.MovePosition(Vector2.Lerp(playerpos.position, new Vector2(-6.38f, playerpos.position.y), Time.fixedDeltaTime*recentreSpeed));
        }
            

        if (Input.GetButton("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(0, jumpForce);
            isGrounded = false;
            anim.runtimeAnimatorController = jumpAnim;
        }

        if (Input.GetKey(KeyCode.G) && !isGrounded)
        {
            rb.velocity = new Vector2(0, jumpForce);
            isGrounded = false;
            anim.runtimeAnimatorController = jumpAnim;
        }

        if(Input.GetKey(KeyCode.Q) && isGrounded && !isParrying)
        {
            StartCoroutine(ParryAttack());
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
