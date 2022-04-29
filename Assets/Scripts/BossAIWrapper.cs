using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    This class is a wrapper that wraps AIController, see that class or the report for more info on the pattern 
*/

public class BossAIWrapper : MonoBehaviour
{
    //This can be considered as a Prototype Design pattern as a single prefab is used to create multiple copies of bossWave
    public GameObject bossWave; //The prefab object that is spawned during a wave
    public GameObject bossWavePos; //The spawn position represented by the position of this GameObject
    public GameObject BossDialog; //GameObject with Dialog Component for this Boss Battle
    public AIController Controller; //AI Controller that is being wrapped
    public GameObject[] AnimationSet; //All AnimationData Objects with animations that affect this Object
    public GameObject LAS; //Logical Animation System
    public GameObject deathParticle;
    public GameObject bossUIControl;
    public GameObject Inventory;

    private float BossHealth;
    private bool bossWaveStarted;

    void Awake()
    {
        Controller = gameObject.GetComponent<AIController>();
        Controller.WrappedAwake(); //Pass on to wrapped

        //Get Animation Data
        LAS = GameObject.FindWithTag("LAS");
        AnimationSet = LAS.GetComponent<LogicalAnimationSystem>().getAnimationDataArray(gameObject);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        Controller.WrappedStart(); //Pass on to wrapped
        StartCoroutine(LateStart());
    }

    IEnumerator LateStart()
    {
        yield return new WaitForSeconds(0.1f);
        BossHealth = 10;

        //Override values from wrapped AI Controller
        Controller.setDamageFrames(8, 9);
        Controller.setDirections(false, false);
        Controller.setSearchRange(60f);
        Controller.wrapperOverride = true;

        //Set other Variables
        bossWaveStarted = false;

    }

    // Update is called once per frame
    void Update()
    {
        if(Controller == null)
        {
            return; //If AI Controller is not set
        }

        //If Attack Animation is running and a wave has not been started yet- start it and Instantiate bossWave Objects
        if(getAnimationProgress(2) == true)
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
        Controller.search();
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
                bossUIControl.GetComponent<BossUIControl>().updateHealthBar(1.0f);

                if(BossHealth <= 0)
                {
                    //***On Death***
                    deathParticle.GetComponent<ParticleSystem>().Play();
                    gameObject.GetComponent<Collider2D>().enabled = false;

                    //Stop Boss from moving on X Axis and make it fall
                    gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX;
                    gameObject.GetComponent<Rigidbody2D>().gravityScale = 10f;

                    bossUIControl.GetComponent<BossUIControl>().endBossFight();
                    Inventory.GetComponent<Inventory>().addItem(0); //Add key piece
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
    IEnumerator BossDeathTimer()
    {
        yield return new WaitForSeconds(2.5f);
        gameObject.SetActive(false);
    }

    //Check if the current running animation is currently a within a certain range of frames
    private bool getAnimationProgress(int id)
    {
        //Check if certain frames are running / Attacking frames that should cause damage
        if(AnimationSet[id].GetComponent<AnimationData>().getActiveFrame() > 3 && AnimationSet[id].GetComponent<AnimationData>().getActiveFrame() < 9)
        {
            return true;
        }
        return false;
    }

}
