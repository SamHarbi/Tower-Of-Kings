using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private int speed;
    private int moveBuffer;
    private Animator anim;
    private SpriteRenderer SR;
    private Rigidbody2D RB;

    public GameObject leftEnemyDetect;
    public GameObject rightEnemyDetect;

    private EnemyDetector RED;
    private EnemyDetector LED;


    // Start is called before the first frame update
    void Start()
    {
        speed = 6;
        moveBuffer = 1;

        anim = gameObject.GetComponent<Animator>();
        SR = gameObject.GetComponent<SpriteRenderer>();
        RB = gameObject.GetComponent<Rigidbody2D>();

        RED = rightEnemyDetect.GetComponent<EnemyDetector>();
        LED = leftEnemyDetect.GetComponent<EnemyDetector>();

    }

    // Update is called once per frame
    void Update()
    {
        //Movement Logic - Walking Left and Right
        if (Input.GetAxis("Dpad-Horizontal") < 0 || Input.GetKey("a"))
        {
            Move(-1.0f);
            SR.flipX = false;

            LED.setView(true);
            RED.setView(false);

        }
        else if(Input.GetAxis("Dpad-Horizontal") > 0 || Input.GetKey("d"))
        {
            Move(1.0f);
            SR.flipX = true;

            LED.setView(false);
            RED.setView(true);
        }
        else
        {
            anim.SetBool("Running", false);
        }

        //Movement Logic - Jumping
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        //Gravity Logic - Heavy Fall and light jump
        if(RB.velocity.y < 0.1)
        {
            RB.gravityScale = 8;
        }
        else
        {
            RB.gravityScale = 4.5f;
        }

    }

    void Jump()
    {
        if(RB.velocity.y == 0)
        {
            GetComponent<Rigidbody2D>().AddForce(transform.up * 1000);
        }
    }

    void Move(float mod)
    {
        Vector3 change;
        anim.SetBool("Running", true);

        change = new Vector3(mod * speed * Time.deltaTime, 0, 0);
        transform.position += change;
    }


}
