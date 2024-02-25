using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] obj;
    public int size;
    public float[] WaitTime;
    int i = 0;
    bool canSpawn = true;
   
    private void Start()
    {
        
    }

    private void Update()
    {
        if(canSpawn && i<size)
            StartCoroutine(MyCoroutine());
    }

    IEnumerator MyCoroutine()
    {
       
        //while (true)
        {
            Instantiate(obj[i]);
           
            canSpawn = false;

            // wait for seconds
            yield return new WaitForSeconds(WaitTime[i]);
            i++;
            canSpawn = true;
        }
    }
    // Update is called once per frame
    
}
