using UnityEngine;
public class PlayerScript : MonoBehaviour
{
    public float jumpForce = 500f;
    public bool isGrounded = false;
    public Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask groundLayer;
    private float groundCheckRadius = 1.75f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (Input.GetButton("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(0, jumpForce);
            //rb.AddForce(Vector2.up * jumpForce);
            isGrounded = false;
        }
    }
}
