using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] obj;
    public int size;
    public float WaitTime;
   
    private void Start()
    {
        StartCoroutine(MyCoroutine());
    }

    IEnumerator MyCoroutine()
    {
        while (true)
        {
            Instantiate(obj[0], obj[0].transform);

            // wait for seconds
            yield return new WaitForSeconds(WaitTime);
        }
    }
    // Update is called once per frame
    
}
