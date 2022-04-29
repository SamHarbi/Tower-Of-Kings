using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    This class is a wrapper that wraps AIController, see that class or the report for more info on the pattern 
*/

public class Boss3AIWrapper : MonoBehaviour
{

    //This can be considered as a Prototype Design pattern as a single prefab is used to create multiple copies of bossWave
    public GameObject bossWave; //The prefab object that is spawned during a wave
    public GameObject bossFireWave; //Alternative Firewave prefab for bossWave
    public GameObject bossBulletWave; //Alternative Bullets prefab for bossWave
    private bool bossWaveStarted;
    public GameObject bossWavePos; //The spawn position represented by the position of this GameObject
    public GameObject bulletPos; 
    public GameObject fireBallPos;
    public GameObject BossDialog; //GameObject with Dialog Component for this Boss Battle 
    public AIController Controller; //AI Controller that is being wrapped
    public GameObject[] AnimationSet; //All AnimationData Objects with animations that affect this Object
    public GameObject LAS; //Logical Animation System
    public GameObject[] powerWheels; //Visual extension of boss
    public GameObject deathParticle; 
    public GameObject bossUIControl; //Controls UI elements of Boss battle
    public GameObject Inventory;
    public GameObject[] powerCrystals; //Objects that stagger Boss and allows the player to deal damage if enough are broken
    public int brokenCrystals; //Numbe of powerCrystals that have been broken

    private float waveTimer; //How long is left for a wave
    private int currWaveID; //Current running wave
    private int numOfWaves; //Total number of wave types
    private bool firstWaveIter; //Is this the first update when a specific wave is running
    private bool stagger; //Is the Boss Staggered?
    private bool shortStaggerInProgress; //Type of Stagger
    private bool longStaggerInProgress; //Type of Stagger
    private bool dead; //Is the boss "dead"?
    private float BossHealth;

    void Awake()
    {
        Controller = gameObject.GetComponent<AIController>();
        Controller.WrappedAwake(); //Pass on to wrapped
    }
    
    // Start is called before the first frame update
    void Start()
    {
        Controller.WrappedStart(); //Pass on to wrapped
        
        //Override values from wrapped AI Controller
        Controller.setDamageFrames(8, 9);
        Controller.setSearchRange(60f);
        Controller.wrapperOverride = true;

        //Get Animation Data
        LAS = GameObject.FindWithTag("LAS");
        AnimationSet = LAS.GetComponent<LogicalAnimationSystem>().getAnimationDataArray(gameObject);

        //Assign other variables
        powerWheels = GameObject.FindGameObjectsWithTag("PowerWheel-Boss3");
        bossWaveStarted = false;
        BossHealth = 20;
        currWaveID = 1;
        numOfWaves = 2;
        firstWaveIter = true;
        powerCrystals = GameObject.FindGameObjectsWithTag("PowerGem-Boss3");
        brokenCrystals = 0;
        stagger = false;
        shortStaggerInProgress = false;
        longStaggerInProgress = false;
        dead = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(dead == true)
        {
            return; //If "dead" do not execute any other update code
        }
        
        if(Controller == null)
        {
            return; //If AI Controller is not set
        }

        //If Attack Animations are running and a wave has not been started yet- start it and Instantiate bossWave Objects
        if(getAnimationProgress(2) == true || getAnimationProgress(4) == true)
        {
            if(bossWaveStarted == false)
            {
                Instantiate(bossWave, bossWavePos.transform.position, Quaternion.identity);
                bossWaveStarted = true;
            }
        }
        else
        {
            bossWaveStarted = false; //Once Animations end the boss wave ends
        }

        if(Controller.getDeathAnim() == true)
        {
            return; //Ensure no other code runs during the death Animation
        }

        search();
        Controller.WrappedUpdate(); //Pass on to wrapped
    }

    void search()
    {
        //If dialog is not done stay at idle state
        if(BossDialog.GetComponent<DialogSystem>().getDialogProgress() == false)
        {
            Controller.State_Idle();
            return;
        }

        //Get how many Crystals are broken
        int currBrokeCrystals = 0;
        for(int i=0; i<powerCrystals.Length; i++)
        {
            if(powerCrystals[i].GetComponent<PowerCrystal>().State == false)
            {
                currBrokeCrystals++;
            }
        }

        //If number of broken crystals has increased since last search
        if(currBrokeCrystals > brokenCrystals && currBrokeCrystals < 6)
        {
            //Stun/stagger the boss by setting to an Idle State but with a death Animation
            Controller.State_Idle();
            Controller.enableAnimation(5);
            brokenCrystals = currBrokeCrystals;

            //Check if a stagger is already running
            if(longStaggerInProgress == false && shortStaggerInProgress == false)
            {
                //Start Short Stagger
                stagger = true;
                shortStaggerInProgress = true;
                StartCoroutine(shortStagger()); 

                if (Debug.isDebugBuild)
                {
                    Debug.Log("Short Stagger Started - Boss3AIWrapper");
                }
            }
            
            return;
        }

        //If there are 6 Broken Crystals
        if(currBrokeCrystals == 6)
        {
            //Stun/stagger the boss by setting to an Idle State but with a death Animation
            Controller.State_Idle();
            Controller.enableAnimation(5);
            
            //Check if a stagger is already running
            if(longStaggerInProgress == false && shortStaggerInProgress == false)
            {
                //Start Long Stagger
                stagger = true;
                longStaggerInProgress = true;
                StartCoroutine(longStagger());

                if (Debug.isDebugBuild)
                {
                    Debug.Log("Long Stagger Started - Boss3AIWrapper");
                }
            }
            
            return;
        }

        //If any stagger is running, set State to Idle and keep first frame of animation active
        if(stagger == true)
        {
            Controller.State_Idle();
            Controller.enableAnimation(5);
            return;
        }
        
        //Wave has run it's time
        if(waveTimer <= 0)
        {
            currWaveID = (currWaveID + 1) % numOfWaves; // Next wave
            firstWaveIter = true;
            
            if (Debug.isDebugBuild)
            {
                Debug.Log("Wave " + currWaveID +" Has Started - Boss3AIWrapper");
            }

            if(currWaveID == 0)
            {
                waveTimer = 1060f * Time.deltaTime; //Make this wave last longer 

                //Tell all wheels to slow down
                for(int i=0; i<powerWheels.Length; i++)
                {
                    powerWheels[i].GetComponent<PowerCircle>().SpeedDown();
                }
            }
            else
            {
                waveTimer = 530f * Time.deltaTime; //Set a time for the wave
            }
        }

        waveTimer = waveTimer - (0.8f* Time.deltaTime); //Update how much time has passed 

        /*These could be moved to other classes to make it easier to add more waves in a strategy design pattern (each wave is one way of damaging the player) or as States
          that the Boss is in

          But due to there being only 2 waves and not enough time, I decided to keep it this way
        */
        if(currWaveID == 0) //BulletWave
        {
            if (Debug.isDebugBuild)
            {
                Debug.Log("Wave " + currWaveID + " is running - Boss3AIWrapper");
            }

            if(firstWaveIter == true) //Is this the first time this is running this wave
            {
                //Set prefab and spawn position
                bossWave = bossBulletWave;
                bossWavePos = bulletPos;

                bossUIControl.GetComponent<BossUIControl>().FadeEffectStartDOWN();
                firstWaveIter = false;
            }

            Controller.search();
            //Debug.Log("Wave" + waveTimer);
            return;
        }
        else if(currWaveID == 1)
        {
            if (Debug.isDebugBuild)
            {
                Debug.Log("Wave " + currWaveID + " is running - Boss3AIWrapper");
            }
            
            Controller.enableAnimation(4); //Enable a special animation for this wave

            if(firstWaveIter == true) //Is this the first time this is running this wave
            {
                //Set prefab and spawn position
                bossWave = bossFireWave;
                bossWavePos = fireBallPos;
                
                bossUIControl.GetComponent<BossUIControl>().FadeEffectStartUP();
                firstWaveIter = false;

                //Speed up Power Wheels
                for(int i=0; i<powerWheels.Length; i++)
                {
                    powerWheels[i].GetComponent<PowerCircle>().SpeedUP();
                }
            }
            
            return;
        }
    }

    //Timer for how long a shortStagger should last
    IEnumerator shortStagger()
    {
        //Debug.Log("Short Stagger now...");
        yield return new WaitForSeconds(1.5f);
        stagger = false;
        shortStaggerInProgress = false;

        if (Debug.isDebugBuild)
        {
            Debug.Log("Short Stagger Ended");
        }
    }

    //Timer for how long a longStagger should last
    IEnumerator longStagger()
    {
        LongStagger_State(); //Start a State that is only available in this wrapper

        yield return new WaitForSeconds(8f);

        //End Stagger
        stagger = false;
        longStaggerInProgress = false;

        //Reset all crystals back to an unbroken state
        for(int i=0; i<powerCrystals.Length; i++)
        {
            powerCrystals[i].GetComponent<PowerCrystal>().ResetState();
        }

        LongStagger_State_Reset(); //Set to a not staggered state 

        if (Debug.isDebugBuild)
        {
            Debug.Log("Long Stagger Ended");
        }
    }

    private void LongStagger_State()
    {
        gameObject.GetComponent<Rigidbody2D>().gravityScale = 1;
    }

    private void LongStagger_State_Reset()
    {
        gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
        StartCoroutine(moveUpwards());
    }

    //Move upwards till roughly back to being out of reach by the Player
    IEnumerator moveUpwards()
    {
        while(gameObject.transform.position.y < 351.4)
        {
            gameObject.transform.position = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + 0.1f);
        }
        yield return null;
    }

    //Completely replaces OnTriggerEnter2d Interface on AIController
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "PlayerAttackRange")
        {
                //***On hit***
                //Set temporary death animation to give a "stuned boss" visual
                Controller.setDeathAnim(true);
                Controller.enableAnimation(3);
                StartCoroutine(BossHitTimer());

                //Update Health value
                BossHealth = BossHealth - 1;

                //Extend health hider to hide more of the health bar
                bossUIControl.GetComponent<BossUIControl>().updateHealthBar(0.5f);

                if(BossHealth <= 0)
                {
                    //***On Death***
                    deathParticle.GetComponent<ParticleSystem>().Play();

                    //Stop Boss from moving on X Axis
                    gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX;
                    gameObject.GetComponent<Rigidbody2D>().gravityScale = 0f;

                    StartCoroutine(moveUpwards());
                    bossUIControl.GetComponent<BossUIControl>().endBossFight();
                    Inventory.GetComponent<Inventory>().addItem(2); //Add key piece
                    StartCoroutine(finalDeathScene());

                    dead = true;

                    //Collect all attackRange spawned wave objects and deactivate them to stop player from dying while end cutscene is running
                    GameObject[] strayBullets = GameObject.FindGameObjectsWithTag("BulletBall");
                    GameObject[] strayFire = GameObject.FindGameObjectsWithTag("FireBall");

                    for(int i=0; i<strayBullets.Length; i++)
                    {
                        strayBullets[i].SetActive(false);
                    }

                    for(int i=0; i<strayFire.Length; i++)
                    {
                        strayFire[i].SetActive(false);
                    }
                }
        }
    }

    //Resets death state to stop stunned effect on boss
    public IEnumerator BossHitTimer()
    {
        yield return new WaitForSeconds(0.5f);
        Controller.setDeathAnim(false);
    }

    //Time delay before boss "dies" and is set to be inactive
    IEnumerator finalDeathScene()
    {
        yield return new WaitForSeconds(6f);
        gameObject.SetActive(false);
    }

    //Check if the current running animation is currently a within a certain range of frames
    private bool getAnimationProgress(int id)
    {
        //Check if certain frames are running / Attacking frames that should cause damage
        if(AnimationSet[id].GetComponent<AnimationData>().getActiveFrame() >= 9 && AnimationSet[id].GetComponent<AnimationData>().getActiveFrame() <= 9)
        {
            return true;
        }
        return false;
    }

}
