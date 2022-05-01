using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Player class - Translates User's Input into action in game through a controllable player character
*/

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
    public GameObject attackRangeLeft;
    public GameObject attackRangeRight;
    public GameObject Fade; //Fade to black cutscene when player loses 
    public GameObject FadeWin; //Fade to black cutscene when player wins
    public bool loading; //Is the game loading?

    private bool DashTimeout; //Delay before Player can dash again
    private bool onPlatform; //Is Player on a platform?
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
    private bool loseAudioStarted;

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
        Cam = GameObject.FindWithTag("MainCamera");
        health = 3;
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
        loseAudioStarted = false;
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
            FadeWin.GetComponent<GameOver>().StartFade(); //Victory fade
            return;
        }
        
        if(health <= 0) //If Dead
        {
            Fade.GetComponent<GameOver>().StartFade(); //GameOver fade
            if(loseAudioStarted == false)
            {
                GetComponent<SoundFXManager>().Lose(); 
                Cam.GetComponent<CameraLevelEffects>().EndBossTheme();
                loseAudioStarted = true;
            }
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
                    Debug.Log("Debug Health Activated");
                }

                if(Input.GetKey("s")) //Faster Movement
                {
                    speed = 15;
                    Debug.Log("Debug Speed Activated");
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

        //If InvincibilityTimer was set, then Player was hit
        if(invincibilityTimer > 0)
        {
            //Count down
            invincibilityTimer -= 1 * Time.deltaTime;
            invincibility = true;

            //Show hit effect
            hitEffect.GetComponent<SpriteRenderer>().enabled = true;
        }
        else //Invincibility timed out
        {
            invincibility = false;
            hitEffect.GetComponent<SpriteRenderer>().enabled = false; //hide hit effect
        }

        //Cannot get hit / Invincibility during dashing
        if(DashTimeout)
        {
            invincibility = true;
        }
        
        //Combat Logic - Attacking
        if (Input.GetButtonDown("Fire1") || Input.GetKey("z"))
        {
           if(currAnim == 1) //If Moving while attacking
           {
               if(attacking == false)
               {
                   enableAnimation(2); //Move + Attack Animation
                   attacking = true;
                   StartCoroutine(AttackTimer());
                   GetComponent<SoundFXManager>().Slash();
               }
           }
           else if(currAnim == 0) //If Idle while attacking
           {
               if(attacking == false)
               {
                   enableAnimation(3); //Idle + Attack Animation
                   attacking = true;
                   StartCoroutine(AttackTimer());
                   GetComponent<SoundFXManager>().Slash();
               }
           }
        }

        //When an animation ends that state should also ends
        if(getAnimationProgress(2) == true)
        {
            attacking = false;
        }
        if(getAnimationProgress(5) == true)
        {
            beingHit = false;
        }
    }

    //If directions are switched as Player attacks, ensure that only one attackRange is active throughout the attack
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

    //Check if Player has Inventory Item
    public bool CheckForItem(int searchTag)
    {
        return InventorySet.GetComponent<Inventory>().CheckForItem(searchTag);
    }

    //Apply a Physics force to the player to quickly move across a space
    void Dash(float mod)
    {
        if(!InventorySet.GetComponent<Inventory>().CheckForItem(4))
        {
            return; //Player doesn't have Gem of Dashing in Inventory - Cannot dash
        }
        
        if(DashTimeout == false) //If not dashing
        {
            RB.AddForce(transform.right * mod);
            DashTimeout = true;
            enableAnimation(6);
            StartCoroutine(DashTimeoutTimer());
            GetComponent<SoundFXManager>().Jump();
        }
    }

    //Dash cooldown before can be activated again
    IEnumerator DashTimeoutTimer()
    {
        yield return new WaitForSeconds(0.8f);
        DashTimeout = false;
    }

    //Apply Physics force to jump into the Air
    void Jump()
    {
        if(Mathf.Approximately(RB.velocity.y, 0f) || onPlatform == true) //Check if player is on the ground or on a platform 
        {
            GetComponent<Rigidbody2D>().AddForce(transform.up * 1000);
            GetComponent<SoundFXManager>().Jump();
        }
    }

    //Picks an Animation to play based on Player's State. Each AnimationData Object acts as a State that changes the visual of the GameObject
    void enableAnimation(int num)
    {
        if(attacking == true && num < 3)// If attacking while Idle or moving
        {
            /*
                Order of AnimationData is important as it allows less if statements to decide what animation to play by grouping together similar types 
                In this case, an attack version of a running animation is always relatively +2 from the currently running animation.

                This is not the best way to do this, but it is possible to do since there aren't that many states- otherwise a State Machine Pattern would be perfect here to
                lower complexity
            */

            AnimationSet[num +2].GetComponent<AnimationData>().Running = true; //Play Animation that is a certain displacment from current state
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
        else //Misc all other future and undefined above animations
        {
            AnimationSet[num].GetComponent<AnimationData>().Running = true;
            if(num != currAnim)
            {
            AnimationSet[currAnim].GetComponent<AnimationData>().Running = false;
            currAnim = num;
            }
        }
    }

    //Has an animation of id finished
    private bool getAnimationProgress(int id)
    {
        if(AnimationSet[id].GetComponent<AnimationData>().getActiveFrame() == AnimationSet[id].GetComponent<AnimationData>().getLastFrame())
        {
            return true;
        }
        return false;
    }

    //Moves the player by changing it's transform position by a set amount mod calculated with speed and time
    void Move(float mod)
    {
        if(health > 0) //Must be alive to move
        {
            Vector3 change;

            //anim.SetBool("Running", true); Legacy unity builtin animation
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
            return; //Ignore all collisons while loading
        }
        
        //Pick up Crown
        if(col.transform.tag == "Crown") 
        {
            enableAnimation(7);
            King = true;
            Cam.GetComponent<CameraLevelEffects>().EndBossTheme();
            GetComponent<SoundFXManager>().Win();
            FadeWin.GetComponent<GameOver>().StartFade();
        }
        
        //Takes a hit if not dashing ot invincible on coming into contact with an Active AttackRange
        if(col.tag == "AttackRange" && invincibility == false && DashTimeout == false)
        {
            //Become invincible for a short time
            invincibility = true;
            invincibilityTimer = Time.deltaTime * 100;

            //Subtract health
            setHealth(health - 1, true);
            GetComponent<SoundFXManager>().Hit();
            return;
        }

        //Instantly lose game- used pits that lead off screen 
        if(col.tag == "InstantDeath")
        {
            setHealth(0, false);
            return;
        }

        //Pick up Pick
        if(col.tag == "Pick")
        {
            InventorySet.GetComponent<Inventory>().addItem(3);
            col.gameObject.SetActive(false);
            GetComponent<SoundFXManager>().Pickup();
        }

        //Pick up Dash Gem
        if(col.tag == "Dash")
        {
            InventorySet.GetComponent<Inventory>().addItem(4);
            col.gameObject.SetActive(false);
            GetComponent<SoundFXManager>().Pickup();
        }

        //Pick up a heart
        if(col.tag == "Heart")
        {
            setHealth(health + 1, false);
            col.gameObject.SetActive(false);
            GetComponent<SoundFXManager>().Pickup();
        }

        if(col.tag == "throneDoor")
        {
            GetComponent<SoundFXManager>().Pickup();
        }

        checkDash(col);

    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if(loading == true)
        {
            return;
        }

        //Player is standing on a moving platform
        if(collision.transform.tag == "MovingPlatform")
        {
            onPlatform = true;

            //Move player in the same direction as the platform
            int MovingPlatformdir = collision.transform.GetComponent<MovingPlatform>().dir;
            gameObject.transform.position = new Vector2(gameObject.transform.position.x + MovingPlatformdir * Time.deltaTime, gameObject.transform.position.y);
        }

        //If on a regular non-moving platform
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
        
        //Reset OnPlatform 

        if(collision.transform.tag == "Platform")
        {
            onPlatform = false;
        }

        if(collision.transform.tag == "MovingPlatform")
        {
            onPlatform = false;
        }
    }

    //Avoid bugs with enemy colliders by always checking if dash should ignore colliders
    void OnTriggerStay2D(Collider2D col)
    {
       if(loading == true)
       {
          return;
       }

       checkDash(col);
    }

    //Add item to Inventory
    public void addToken(int tokenID)
    {
        InventorySet.GetComponent<Inventory>().addItem(tokenID);
    }

    //Ignore enemy colliders while dashing
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

    //Change the Player's health
    public void setHealth(int newHealth, bool shake)
    {
        //Change health and update heart visual
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

        if(health <= 0) //no more health, start gameover cutscene
        {
            enableAnimation(4);
            Fade.GetComponent<GameOver>().StartFade();
        }
        else if(shake == true) //Shake cam if needed, this is used if player was hit by an enemy instead of new health being loaded, cannot shake on death
        {
            enableAnimation(5);
            beingHit = true;
            if(shake == true)
            {
                Cam = GameObject.FindWithTag("MainCamera");
                Cam.GetComponent<CameraShake>().startShake(0.3f, 1.5f);
            }
        }
    }

    public int getHealth()
    {
        return health;
    }

    //Activates attackRange for a set amount of time before turning it off again
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
