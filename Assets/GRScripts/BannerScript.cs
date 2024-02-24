using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BannerScript : MonoBehaviour
{
    public Transform hook;
    public SpriteRenderer blink;
    public Rigidbody2D m_body2d;
    public float bannerSpeed = 2.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_body2d.velocity = new Vector2(-1f * bannerSpeed, m_body2d.velocity.y);
    }
}
