using System.Collections;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class sniperGenerator : MonoBehaviour
{
    public GameObject laser;
    public float minSpeed;
    public float maxSpeed;
    public float currSpeed;


    // Start is called before the first frame update
    void Awake()
    {
        currSpeed = minSpeed;
        StartCoroutine(begin());
    }

    IEnumerator begin()
    {

        yield return new WaitForSeconds(2.25f);
        GenerateLaser();

    }
    public void GenerateLaser()
    {
        UnityEngine.Vector3 temp = new UnityEngine.Vector3(transform.position.x - 1f, transform.position.y + 0.2f, transform.position.z);
        GameObject laserInst = Instantiate(laser, temp, transform.rotation);
        laserInst.GetComponent<bulletMovement>().snipGen = this;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
