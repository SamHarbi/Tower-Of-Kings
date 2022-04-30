using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Visual wheels around a boss
*/

public class PowerCircle : MonoBehaviour
{
    public GameObject Boss; //The boss linked to these wheels
    public float speed; //Current spinning speed
    private float maxSpeed; //Maximum spinning speed
    private float minSpeed; //Minimum spinning speed
    public bool speeding; //You can't do that! You'll get a ticket :P - Is wheel increasing in speed?
    
    // Start is called before the first frame update
    void Start()
    {
        //Initialise values
        speed = 20;
        maxSpeed = 500;
        speeding = false;
        minSpeed = -speed; //rotate backwards at minimum speed
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
    }

    public void SpeedDown()
    {
        speeding = false;
    }
}
