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
        transform.Translate(bannerSpeed * Time.deltaTime, 0, 0);
    }
}
