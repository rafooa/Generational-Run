using UnityEngine;

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
        GenerateLaser();
    }
    public void GenerateLaser()
    {
        GameObject laserInst = Instantiate(laser, transform.position, transform.rotation);
        laserInst.GetComponent<bulletMovement>().snipGen = this;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
