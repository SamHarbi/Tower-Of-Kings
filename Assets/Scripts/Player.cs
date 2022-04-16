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
    private bool direction;

    private EnemyDetector RED;
    private EnemyDetector LED;

    private bool continousMovementLeft;
    private bool continousMovementRight;

    private int health;
    public GameObject[] Hearts;
    //public GameObject[] Tokens;
    //public GameObject[] InventoryItems;
    public GameObject InventorySet;
    private bool invincibility;
    private float invincibilityTimer;
    private float dashInvincibilityTimer;
    private bool attacking;
    private bool beingHit;
    public GameObject[] AnimationSet;
    public GameObject LAS;
    public GameObject hitEffect;
    public GameObject Cam;
    public GameObject[] BossCams; //Change cams when entering diff bosses else no screeen shake
    public GameObject attackRangeLeft;
    public GameObject attackRangeRight;
    public GameObject Fade;
    private bool DashTimeout;


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

        attacking = false;
        beingHit = false;

        //attackRange = GameObject.FindWithTag("PlayerAttackRange");
        attackRangeLeft.SetActive(false);
        attackRangeRight.SetActive(false);

        DashTimeout = false;

        direction = false;

        //InventoryItems = new GameObject[6];
        

    }

    // Update is called once per frame
    void Update()
    {
        if(health <= 0)
        {
            Fade.GetComponent<GameOver>().StartFade();
            return;
        }

        if(Input.GetKey("d"))
        {
            if(Input.GetKey("e"))
            {
                if(Input.GetKey("b"))
                {
                    for(int i=0; i<9; i++)
                    {
                        InventorySet.GetComponent<Inventory>().addItem(i);
                    }
                    Debug.Log("Debug Activated");
                }
            }
        }

        
        
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

            if(Input.GetButton("Dash"))
            {
                Dash(-1000f);
            }

            continousMovementLeft = true;
            continousMovementRight = false;

            direction = false;

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

            if(Input.GetButton("Dash"))
            {
                Dash(1000f);
            }

            continousMovementLeft = false;
            continousMovementRight = true;

            direction = true;
        }
        else
        {
            //anim.SetBool("Running", false);
            
            
            if(attacking == false)
            {
                enableAnimation(0);
            }
            
            
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
            RB.gravityScale = 4.0f;
        }

        if(invincibilityTimer > 0)
        {
            invincibilityTimer -= 1 * Time.deltaTime;
            invincibility = true;
            hitEffect.GetComponent<SpriteRenderer>().enabled = true;
        }
        else
        {
            invincibility = false;
            hitEffect.GetComponent<SpriteRenderer>().enabled = false;
        }

        if(DashTimeout)
        {
            invincibility = true;
        }
        

        //Combat Logic - Attacking
        if (Input.GetButtonDown("Fire1"))
        {
           if(currAnim == 1)
           {
               if(attacking == false)
               {
                   enableAnimation(2);
                   attacking = true;
                   StartCoroutine(AttackTimer());
               }
           }
           else if(currAnim == 0)
           {
               if(attacking == false)
               {
                   enableAnimation(3);
                   attacking = true;
                   StartCoroutine(AttackTimer());
               }
           }
        }

        if(getAnimationProgress(2) == true)
        {
            attacking = false;
        }
        if(getAnimationProgress(5) == true)
        {
            beingHit = false;
        }

    }

    public bool CheckForItem(int searchTag)
    {
        return InventorySet.GetComponent<Inventory>().CheckForItem(searchTag);
    }

    void Dash(float mod)
    {
        if(InventorySet.GetComponent<Inventory>().CheckForItem(4))
        {
            return;
        }
        
        if(DashTimeout == false)
        {
            RB.AddForce(transform.right * mod);
            DashTimeout = true;
            enableAnimation(6);
            StartCoroutine(DashTimeoutTimer());
        }
    }

    IEnumerator DashTimeoutTimer()
    {
        yield return new WaitForSeconds(0.8f);
        DashTimeout = false;
    }

    void Jump()
    {
        if(Mathf.Approximately(RB.velocity.y, 0f))
        {
            GetComponent<Rigidbody2D>().AddForce(transform.up * 1000);
        }
    }

    void enableAnimation(int num)
    {
        if(attacking == true && num < 3)
        {
            AnimationSet[num +2].GetComponent<AnimationData>().Running = true;
            if(num+2 != currAnim)
            {
                AnimationSet[currAnim].GetComponent<AnimationData>().Running = false;
                currAnim = num+2;
            }
        }
        else if(beingHit == true)
        {
            AnimationSet[5].GetComponent<AnimationData>().Running = true;
            if(5 != currAnim)
            {
                AnimationSet[currAnim].GetComponent<AnimationData>().Running = false;
                currAnim = 5;
            }
        }
        else if(DashTimeout)
        {
            AnimationSet[6].GetComponent<AnimationData>().Running = true;
            if(6 != currAnim)
            {
            AnimationSet[currAnim].GetComponent<AnimationData>().Running = false;
            currAnim = 6;
            }
        }
        else
        {
            AnimationSet[num].GetComponent<AnimationData>().Running = true;
            if(num != currAnim)
            {
            AnimationSet[currAnim].GetComponent<AnimationData>().Running = false;
            currAnim = num;
            }
        }
    }

    private bool getAnimationProgress(int id)
    {
        if(AnimationSet[id].GetComponent<AnimationData>().activeFrame == AnimationSet[id].GetComponent<AnimationData>().lastFrame)
        {
            return true;
        }
        return false;
    }

    void Move(float mod)
    {
        if(health > 0)
        {
            Vector3 change;
            //anim.SetBool("Running", true);
            //anim.SetBool("Running", false);
            
            
            enableAnimation(1);
            
            
            change = new Vector3(mod * speed * Time.deltaTime, 0, 0);
            transform.position += change;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "AttackRange" && invincibility == false && DashTimeout == false)
        {
            invincibility = true;
            invincibilityTimer = Time.deltaTime * 100;
            setHealth(health - 1);
            return;
        }

        if(col.tag == "InstantDeath")
        {
            setHealth(0);
            return;
        }

        if(col.tag == "Pick")
        {
            InventorySet.GetComponent<Inventory>().addItem(3);
            col.gameObject.SetActive(false);
        }

        if(col.tag == "Dash")
        {
            InventorySet.GetComponent<Inventory>().addItem(4);
            col.gameObject.SetActive(false);
        }

        checkDash(col);

    }

    void OnTriggerStay2D(Collider2D col)
    {
       checkDash(col);
    }

    public void addToken(int tokenID)
    {
        InventorySet.GetComponent<Inventory>().addItem(tokenID);
    }

    void checkDash(Collider2D col)
    {
        if(DashTimeout == true)
        {
            if(col.gameObject.tag == "Enemy")
            {
                Physics2D.IgnoreCollision(col.gameObject.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>());
            }
        }
        else
        {
             Physics2D.IgnoreCollision(col.gameObject.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>(), false);
             DashTimeout = false;
        }
    }

    public void setHealth(int newHealth)
    {
        health = newHealth;
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

        if(health <= 0)
        {
            enableAnimation(4);
            Fade.GetComponent<GameOver>().StartFade();
        }
        else
        {
            enableAnimation(5);
            beingHit = true;
            StartCoroutine(Cam.GetComponent<CameraShake>().CamShake(0.1f, 0.2f));
        }
    }

    public int getHealth()
    {
        return health;
    }

    void Attack()
    {

    }

    IEnumerator AttackTimer()
    {
        if(direction == false)
        {
            attackRangeLeft.SetActive(true);
            attackRangeRight.SetActive(false);
        }
        else
        {
            attackRangeRight.SetActive(true);
            attackRangeLeft.SetActive(false);
        }
        
        yield return new WaitForSeconds(0.3f);
        
        if(direction == false)
        {
            attackRangeLeft.SetActive(false);
        }
        else
        {
            attackRangeRight.SetActive(false);
        }

        attacking = false;
    }
}
