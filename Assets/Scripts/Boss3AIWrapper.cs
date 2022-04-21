using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss3AIWrapper : MonoBehaviour
{

    private float BossHealth;
    public GameObject hitBoss;
    private int startDamageFrame;
    private int endDamageFrame;
    public GameObject bossWave;
    public GameObject bossFireWave;
    public GameObject effectOverlay;
    public GameObject bossBulletWave;
    private bool bossWaveStarted;
    public GameObject bossWavePos;
    public GameObject bulletPos;
    public GameObject fireBallPos;
    public GameObject BossDialog;
    public GameObject Boss;
    public AIController Controller;
    public GameObject[] AnimationSet;
    public GameObject LAS;
    public GameObject[] powerWheels;

    public GameObject healthHider;
    public GameObject deathParticle;
    public GameObject bossUIControl;
    public GameObject Inventory;
    private float waveTimer;
    private int currWaveID;
    private int numOfWaves;
    private bool firstWaveIter;
    public GameObject[] powerCrystals;
    public int brokenCrystals;
    private bool stagger;
    private bool shortStaggerInProgress;
    private bool longStaggerInProgress;
    private bool dead;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LateStart());
    }

    IEnumerator LateStart()
    {
        yield return new WaitForSeconds(0.1f);
        BossHealth = 20;
        Controller = Boss.GetComponent<AIController>();
        Controller.setDamageFrames(8, 9);
        Controller.setSearchRange(60f);
        Controller.wrapperOverride = true;
        bossWaveStarted = false;
        LAS = GameObject.FindWithTag("LAS");
        AnimationSet = LAS.GetComponent<LogicalAnimationSystem>().getAnimationDataArray(Boss);
        powerWheels = GameObject.FindGameObjectsWithTag("PowerWheel-Boss3");

        currWaveID = 1;
        numOfWaves = 2;
        //waveTimer = 10f * Time.deltaTime;

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
            return;
        }
        
        if(Controller == null)
        {
            return;
        }


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
            bossWaveStarted = false;
        }

        if(Controller.getDeathAnim() == true)
        {
            return; //Ensure no other code runs during the death Animation
        }

        search();
    }

    void search()
    {
        
        if(BossDialog.GetComponent<DialogSystem>().getDialogProgress() == false)
        {
            Controller.State_Idle();
            return;
        }

        int currBrokeCrystals = 0;
        for(int i=0; i<powerCrystals.Length; i++)
        {
            if(powerCrystals[i].GetComponent<PowerCrystal>().State == false)
            {
                currBrokeCrystals++;
            }
        }

        if(currBrokeCrystals > brokenCrystals && currBrokeCrystals < 6)
        {
            Controller.State_Idle();
            Controller.enableAnimation(5);
            
            brokenCrystals = currBrokeCrystals;
            //StopCoroutine(shortStaggerCoroutine);
            if(longStaggerInProgress == false && shortStaggerInProgress == false)
            {
                stagger = true;
                StartCoroutine(shortStagger());
                Debug.Log("Short Stagger Started");
                shortStaggerInProgress = true;
            }
            
            return;
        }

        if(currBrokeCrystals == 6)
        {
            Controller.State_Idle();
            Controller.enableAnimation(5);
            
            //StopCoroutine(shortStaggerCoroutine);
            if(longStaggerInProgress == false && shortStaggerInProgress == false)
            {
                stagger = true;
                StartCoroutine(longStagger());
                longStaggerInProgress = true;
            }
            
            return;
        }

        if(stagger == true)
        {
            Controller.State_Idle();
            Controller.enableAnimation(5);
            return;
        }
        
        if(waveTimer <= 0)
        {
            currWaveID = (currWaveID + 1) % numOfWaves;
            firstWaveIter = true;
            Debug.Log(currWaveID);

            if(currWaveID == 0)
            {
                waveTimer = 1060f * Time.deltaTime; //Make this wave last longer 
                
                for(int i=0; i<powerWheels.Length; i++)
                {
                    powerWheels[i].GetComponent<PowerCircle>().SpeedDown();
                }
            }
            else
            {
                waveTimer = 530f * Time.deltaTime; 
            }
        }

        //Controller.search();
        waveTimer = waveTimer - (0.8f* Time.deltaTime);

        if(currWaveID == 0)
        {
            bossWave = bossBulletWave;
            bossWavePos = bulletPos;
            if(firstWaveIter == true)
            {
                bossUIControl.GetComponent<BossUIControl>().FadeEffectStartDOWN();
                firstWaveIter = false;
            }
            Controller.search();
            //Debug.Log("Wave" + waveTimer);
            return;
        }
        else if(currWaveID == 1)
        {
            Controller.enableAnimation(4);
            //Debug.Log("Alt Wave");
            if(firstWaveIter == true)
            {
                bossUIControl.GetComponent<BossUIControl>().FadeEffectStartUP();
                firstWaveIter = false;
            }
            bossWave = bossFireWave;
            bossWavePos = fireBallPos;
            for(int i=0; i<powerWheels.Length; i++)
            {
                powerWheels[i].GetComponent<PowerCircle>().SpeedUP();
            }
            return;
        }
    }

    IEnumerator shortStagger()
    {
        Debug.Log("Short Stagger now...");
        yield return new WaitForSeconds(2f);
        stagger = false;
        shortStaggerInProgress = false;
        Debug.Log("Short Stagger Ended");
    }

    IEnumerator longStagger()
    {
        LongStagger_State();
        yield return new WaitForSeconds(8f);
        stagger = false;
        longStaggerInProgress = false;
        for(int i=0; i<powerCrystals.Length; i++)
        {
            powerCrystals[i].GetComponent<PowerCrystal>().ResetState();
        }
        LongStagger_State_Reset();
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

    IEnumerator moveUpwards()
    {
        while(gameObject.transform.position.y < 351.4)
        {
            gameObject.transform.position = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + 0.1f);
        }
        yield return null;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "PlayerAttackRange")
        {
                Controller.setDeathAnim(true);
                Controller.enableAnimation(3);
                StartCoroutine(Controller.BossHitTimer());
                BossHealth = BossHealth - 1;
                healthHider.transform.localScale = new Vector3(healthHider.transform.localScale.x+0.5f, healthHider.transform.localScale.y, healthHider.transform.localScale.z);
                healthHider.transform.position = new Vector3(healthHider.transform.position.x - 0.25f, healthHider.transform.position.y, healthHider.transform.position.z);
                if(BossHealth <= 0)
                {
                    //On Death
                    deathParticle.GetComponent<ParticleSystem>().Play();
                    gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX;
                    gameObject.GetComponent<Rigidbody2D>().gravityScale = 0f;
                    StartCoroutine(moveUpwards());
                    bossUIControl.GetComponent<BossUIControl>().endBossFight();
                    Inventory.GetComponent<Inventory>().addItem(2);
                    StartCoroutine(finalDeathScene());

                    dead = true;

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
               //Vector3 hitPos = new Vector3(col.transform.position.x + 2, col.transform.position.y + 2, gameObject.transform.position.z);
                //Instantiate(hitBoss, hitPos, Quaternion.identity);
        }
    }

    IEnumerator finalDeathScene()
    {
        yield return new WaitForSeconds(6f);
        gameObject.SetActive(false);
    }

    private bool getAnimationProgress(int id)
    {
        //Check if certain frames are running / Attacking frames that should cause damage
        if(AnimationSet[id].GetComponent<AnimationData>().activeFrame >= 9 && AnimationSet[id].GetComponent<AnimationData>().activeFrame <= 9)
        {
            return true;
        }
        return false;
    }

}
