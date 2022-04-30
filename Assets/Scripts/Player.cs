using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public int speed; //Movement Speed
    public GameObject[] Hearts; //UI hearts representing Player Health
    //public GameObject[] Tokens;
    //public GameObject[] InventoryItems;
    public GameObject InventorySet; //Inventory
    public GameObject[] AnimationSet; //Array of AnimationData that pertains to Player
    public GameObject LAS; //Logical Animation System
    public GameObject hitEffect; //GameObject that shows a visual overlay when Player is hit
    public GameObject Cam; //Main Camera
    public GameObject[] BossCams; //Change cams when entering boss areas else no screen shake
    public GameObject attackRangeLeft;
    public GameObject attackRangeRight;
    public GameObject Fade; //Fade to black cutscene when player loses 
    public GameObject FadeWin; //Fade to black cutscene when player wins
    public bool loading; //Is the game loading?

    private bool DashTimeout; //Delay before Player can dash again
    private bool onPlatform; //Is Player on a moving platform?
    private bool King; //Has the player won the game?
    private bool invincibility; //Can Player take damage?
    private float invincibilityTimer; //How much time is left for player invincibility
    private bool attacking; //Is player attacking?
    private bool beingHit; //Is player being hit
    private bool direction;
    private int health; 
    private SpriteRenderer SR;
    private Rigidbody2D RB;
    private int currAnim; //Currently running Animation

    // private Animator anim; Legacy Unity built-in Animation

    /* legacy panel centric attack variables
    public GameObject leftEnemyDetect;
    public GameObject rightEnemyDetect;
    private EnemyDetector RED;
    private EnemyDetector LED;
    */


    // Start is called before the first frame update
    void Start()
    {
        //Initialise Values
        speed = 6;
        loading = true;
        SR = gameObject.GetComponent<SpriteRenderer>();
        RB = gameObject.GetComponent<Rigidbody2D>();
        health = 300;
        Hearts = new GameObject[7];
        invincibility = false;
        invincibilityTimer = 0;
        attacking = false;
        beingHit = false;
        attackRangeLeft.SetActive(false);
        attackRangeRight.SetActive(false);
        DashTimeout = false;
        direction = false;
        onPlatform = false;
        King = false;
        //anim = gameObject.GetComponent<Animator>(); Legacy Unity built-in Animation

        //Based on health value, activate a number of hearts that corresponds to health
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

        //Initilase Animations and set to idle animation
        AnimationSet = LAS.GetComponent<LogicalAnimationSystem>().getAnimationDataArray(gameObject);
        currAnim = 1;
        enableAnimation(0);
        
    }

    // Update is called once per frame
    void Update()
    {
        
        //Stop executing any other update code if one of the following

        if(Time.deltaTime == 0 || loading == true) //If Game is paused or loading
        {
            return;
        }

        if(King == true) //If the game has been won
        {
            FadeWin.GetComponent<GameOver>().StartFade();
            return;
        }
        
        if(health <= 0) //If Dead
        {
            Fade.GetComponent<GameOver>().StartFade();
            return;
        }


        //Cheat Inputs for debugging
        if(Input.GetKey("d"))
        {
            if(Input.GetKey("e"))
            {
                if(Input.GetKey("b")) //Adds every item to players Inventory
                {
                    for(int i=3; i<5; i++)
                    {
                        InventorySet.GetComponent<Inventory>().addItem(i);
                    }
                    Debug.Log("Debug Items Activated");
                }

                if(Input.GetKey("a")) //Adds every key item to players Inventory
                {
                    for(int i=0; i<3; i++)
                    {
                        InventorySet.GetComponent<Inventory>().addItem(i);
                    }
                    Debug.Log("Debug Keys Activated");
                }

                if(Input.GetKey("h")) //Lot's of health
                {
                    health = 999;
                }

                if(Input.GetKey("s")) //Faster Movement
                {
                    speed = 15;
                }
            }
        }  
        
        //Movement Logic - Walking Left and Right
        if (Input.GetAxis("Dpad-Horizontal") < 0 || Input.GetKey("a") || Input.GetKey("left")) //Input corresponding to left
        {
            Move(-1.0f);
            GetComponent<SoundFXManager>().clothMove();
            SR.flipX = false;

            if(Input.GetButton("Dash")) //Dash input key pressed in tandem with moving keys
            {
                Dash(-1000f);
            }

            //If direction of movement was previously opposite 
            if(direction == true)
            {
                switchAttack(true);
            }

            direction = false;

        }
        else if(Input.GetAxis("Dpad-Horizontal") > 0 || Input.GetKey("d") || Input.GetKey("right")) //Input corresponding to right
        {
            Move(1.0f);
            GetComponent<SoundFXManager>().clothMove();
            SR.flipX = true;

            if(Input.GetButton("Dash")) //Dash input key pressed in tandem with moving keys
            {
                Dash(1000f);
            }

            //If direction of movement was previously opposite 
            if(direction == false)
            {
                switchAttack(false);
            }

            direction = true;

        }
        else //No moving keys being clicked
        {
            //anim.SetBool("Running", false); Legacy Unity built in Animation
            
            //Not attacking
            if(attacking == false)
            {
                enableAnimation(0); //Idle Animation
            }
            
        }

        //Movement Logic - Jumping
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        //Gravity Logic - Heavy Fall and light jump
        if(RB.velocity.y < 0.1) //If falling
        {
            RB.gravityScale = 8; //High Gravity
        }
        else //If not falling
        {
            RB.gravityScale = 3.8f; //Low Gravity
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
        if (Input.GetButtonDown("Fire1") || Input.GetKey("z"))
        {
           if(currAnim == 1)
           {
               if(attacking == false)
               {
                   enableAnimation(2);
                   attacking = true;
                   StartCoroutine(AttackTimer());
                   GetComponent<SoundFXManager>().Slash();
               }
           }
           else if(currAnim == 0)
           {
               if(attacking == false)
               {
                   enableAnimation(3);
                   attacking = true;
                   StartCoroutine(AttackTimer());
                   GetComponent<SoundFXManager>().Slash();
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

    public void switchAttack(bool dir)
    {
        if(attacking == false)
        {
            return;
        }
        
        if(dir == true)
        {
            attackRangeLeft.SetActive(true);
            attackRangeRight.SetActive(false);
        }
        else
        {
            attackRangeLeft.SetActive(false);
            attackRangeRight.SetActive(true);
        }
       
        

    }

    public bool CheckForItem(int searchTag)
    {
        return InventorySet.GetComponent<Inventory>().CheckForItem(searchTag);
    }

    void Dash(float mod)
    {
        if(!InventorySet.GetComponent<Inventory>().CheckForItem(4))
        {
            return;
        }
        
        if(DashTimeout == false)
        {
            RB.AddForce(transform.right * mod);
            DashTimeout = true;
            enableAnimation(6);
            StartCoroutine(DashTimeoutTimer());
            GetComponent<SoundFXManager>().Jump();
        }
    }

    IEnumerator DashTimeoutTimer()
    {
        yield return new WaitForSeconds(0.8f);
        DashTimeout = false;
    }

    void Jump()
    {
        if(Mathf.Approximately(RB.velocity.y, 0f) || onPlatform == true)
        {
            GetComponent<Rigidbody2D>().AddForce(transform.up * 1000);
            GetComponent<SoundFXManager>().Jump();
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
        if(AnimationSet[id].GetComponent<AnimationData>().getActiveFrame() == AnimationSet[id].GetComponent<AnimationData>().getLastFrame())
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
        if(loading == true)
        {
            return;
        }
        
        if(col.tag == "AttackRange" && invincibility == false && DashTimeout == false)
        {
            invincibility = true;
            invincibilityTimer = Time.deltaTime * 100;
            setHealth(health - 1, true);
            GetComponent<SoundFXManager>().Hit();
            return;
        }

        if(col.tag == "InstantDeath")
        {
            setHealth(0, false);
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

        if(col.tag == "Heart")
        {
            setHealth(health + 1, false);
            col.gameObject.SetActive(false);
        }

        

        checkDash(col);

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(loading == true)
        {
            return;
        }
        
        if(collision.transform.tag == "Crown")
        {
            enableAnimation(7);
            King = true;
            FadeWin.GetComponent<GameOver>().StartFade();
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if(loading == true)
        {
            return;
        }

        if(collision.transform.tag == "MovingPlatform")
        {
            onPlatform = true;
            int MovingPlatformdir = collision.transform.GetComponent<MovingPlatform>().dir;
            gameObject.transform.position = new Vector2(gameObject.transform.position.x + MovingPlatformdir * Time.deltaTime, gameObject.transform.position.y);
        }

         if(collision.transform.tag == "Platform")
         {
            onPlatform = true;
         }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if(loading == true)
        {
            return;
        }
        
        if(collision.transform.tag == "Platform")
        {
            onPlatform = false;
        }

        if(collision.transform.tag == "MovingPlatform")
        {
            onPlatform = false;
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
       if(loading == true)
       {
          return;
       }

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

    public void setHealth(int newHealth, bool shake)
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
        else if(shake == true)
        {
            enableAnimation(5);
            beingHit = true;
            if(shake == true)
            {
                StartCoroutine(Cam.GetComponent<CameraShake>().CamShake(0.1f, 0.2f));
            }
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
