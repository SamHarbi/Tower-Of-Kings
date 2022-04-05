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
    private int currAnim;

    public GameObject leftEnemyDetect;
    public GameObject rightEnemyDetect;

    private EnemyDetector RED;
    private EnemyDetector LED;

    private bool continousMovementLeft;
    private bool continousMovementRight;

    private int health;
    public GameObject[] Hearts;
    private bool invincibility;
    private float invincibilityTimer;
    public GameObject[] AnimationSet;
    public GameObject LAS;


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

        health = 3;
        Hearts = new GameObject[7];

        invincibility = false;
        invincibilityTimer = 0;

        for(int i=1; i<=6; i++)
        {
            Hearts[i] = GameObject.Find("heart" + i);
            if(Hearts[i] != null && i <= health)
            {
                Hearts[i].SetActive(true);
            }
            else
            {
                Hearts[i].SetActive(false);
            }
        }

        AnimationSet = LAS.GetComponent<LogicalAnimationSystem>().getAnimationDataArray(gameObject);
        currAnim = 1;
        enableAnimation(0);
        

    }

    // Update is called once per frame
    void Update()
    {
        //Movement Logic - Walking Left and Right
        if (Input.GetAxis("Dpad-Horizontal") < 0 || Input.GetKey("a"))
        {
            Move(-1.0f);
            SR.flipX = false;

            if(continousMovementLeft == false)
            {
                LED.setView(true, false);
                RED.setView(false, false);
            }

            continousMovementLeft = true;
            continousMovementRight = false;

        }
        else if(Input.GetAxis("Dpad-Horizontal") > 0 || Input.GetKey("d"))
        {
            Move(1.0f);
            SR.flipX = true;

            if(continousMovementRight == false)
            {
                LED.setView(false, true);
                RED.setView(true, true);
            }

            continousMovementLeft = false;
            continousMovementRight = true;
        }
        else
        {
            //anim.SetBool("Running", false);
            enableAnimation(0);
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

        if(invincibilityTimer > 0)
        {
            invincibilityTimer -= 1 * Time.deltaTime;
            invincibility = true;
        }
        else
        {
            invincibility = false;
        }

    }

    void Jump()
    {
        if(RB.velocity.y == 0)
        {
            GetComponent<Rigidbody2D>().AddForce(transform.up * 1000);
        }
    }

    void enableAnimation(int num)
    {
        AnimationSet[num].GetComponent<AnimationData>().Running = true;
        if(num != currAnim)
        {
            AnimationSet[currAnim].GetComponent<AnimationData>().Running = false;
            currAnim = num;
        }
    }

    void Move(float mod)
    {
        Vector3 change;
        //anim.SetBool("Running", true);
        enableAnimation(1);

        change = new Vector3(mod * speed * Time.deltaTime, 0, 0);
        transform.position += change;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "AttackRange" && invincibility == false)
        {
            setHealth(health - 1);
        }
    }

    public void setHealth(int newHealth)
    {
        health = newHealth;
        invincibilityTimer = Time.deltaTime * 50;
        for(int i=1; i<=6; i++)
        {
            if(Hearts[i] != null && i <= health)
            {
                Hearts[i].SetActive(true);
            }
            else
            {
                Hearts[i].SetActive(false);
            }
        }

    }

    public int getHealth()
    {
        return health;
    }
}
