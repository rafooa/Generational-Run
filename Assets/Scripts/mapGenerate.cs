using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mapGenerate : MonoBehaviour
{
    public GameObject[] platforms;
    int rand;
    int temp = 0;

    float xPos;
    float yPos;
    Quaternion rot;

    float timeSpent = 1;

    float timer = 5;

    // Start is called before the first frame update
    void Start()
    {
        xPos = transform.position.x;
        yPos = transform.position.y;
        rot = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        timeSpent -= Time.deltaTime;


        if (timeSpent <= 0)
        {
            timeSpent = 1;
            Time.timeScale = Time.timeScale + 0.008f;
        }

        timer -= Time.deltaTime;

        if (timer < 0)
        {
            rand = Random.Range(0, platforms.Length);
            if (rand == 0)
            {
                yPos = transform.position.y + 2.18f;
            }
            else if (rand == 3)
            {
                yPos = transform.position.y - 2.5f;
                rot.y = 0;
                
            }
            else if (rand == 4)
            {
                rot = platforms[4].transform.rotation;
                xPos = transform.position.x + 40;
                yPos = 15;
            }
            else if (rand == 5)
            {
                yPos = transform.position.y;
            }
            yPos = transform.position.y - 3.3f;
            rot = transform.rotation;

            Instantiate(platforms[5], new Vector3(xPos, yPos, 1), rot);
            timer = 6;
        }
    }
}
