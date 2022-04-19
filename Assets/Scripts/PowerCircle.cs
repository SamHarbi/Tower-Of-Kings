using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerCircle : MonoBehaviour
{
    public GameObject Boss;
    public float speed;
    private float maxSpeed;
    private float minSpeed;
    public bool speeding; //You can't do that! You'll get a ticket :P
    // Start is called before the first frame update
    void Start()
    {
        speed = 20;
        maxSpeed = 500;
        speeding = false;
        minSpeed = -speed;
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(Boss.transform.position, Vector3.back, speed * Time.deltaTime);

        if(speeding == true && speed < maxSpeed)
        {
            speed = speed + 1;
        }

        if(speeding == false && speed > minSpeed)
        {
            speed = speed - 1;
        }
    }

    public void SpeedUP()
    {
        speeding = true;
        Debug.Log("Showtime");
    }

    public void SpeedDown()
    {
        speeding = false;
    }
}
