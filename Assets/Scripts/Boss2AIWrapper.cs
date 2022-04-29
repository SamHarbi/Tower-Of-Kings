using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    This class is a wrapper that wraps AIController, see that class or the report for more info on the pattern 
*/

public class Boss2AIWrapper : MonoBehaviour
{
    public GameObject BossDialog; //GameObject with Dialog Component for this Boss Battle
    public GameObject bossUIControl; //Controls UI elements of Boss battle
    public AIController Controller; //AI Controller that is being wrapped
    public GameObject attackRange; //GameObject with Collider that causes damage to Player on contact
    
    private float BossHealth;
    private GameObject[] AnimationSet; //All AnimationData Objects with animations that affect this Object
    private GameObject LAS; //Logical Animation System
    private GameObject Inventory; 
    private bool dead;
    
    void Awake()
    {
        Controller = gameObject.GetComponent<AIController>();
        Controller.WrappedAwake(); //Pass on to wrapped
    }

    // Start is called before the first frame update
    void Start()
    {
        //Get Animation Data
        LAS = GameObject.FindWithTag("LAS");
        AnimationSet = LAS.GetComponent<LogicalAnimationSystem>().getAnimationDataArray(gameObject);

        Controller.WrappedStart(); //Pass on to wrapped
        StartCoroutine(LateStart());

        dead = false;
    }

    IEnumerator LateStart()
    {
        yield return new WaitForSeconds(0.1f);
        BossHealth = 20;
        Inventory = GameObject.FindWithTag("Inventory");

        //Override values from wrapped AI Controller
        Controller.setDamageFrames(6, 8);
        Controller.wrapperOverride = true;
    }

    // Update is called once per frame
    void Update()
    {
        search();

        //If an Attack Animation is playing, ensure that damage can be done by this enemy to Player
        if(getAnimationProgress(2) == true)
        {
            attackRange.SetActive(true);
        }
        else
        {
            attackRange.SetActive(false);
        }

        Controller.WrappedUpdate(); //Pass on to wrapped
    }

    void search()
    {
        //If dialog is not done stay at idle state
        if(BossDialog.GetComponent<DialogSystem>().getDialogProgress() == false || dead == true)
        {
            Controller.State_Idle();
            return;
        }

        //else pass on to AIController
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
                bossUIControl.GetComponent<BossUIControl>().updateHealthBar(0.5f);

                if(BossHealth <= 0)
                {
                    //***On Death***
                    bossUIControl.GetComponent<BossUIControl>().endBossFight();
                    Inventory.GetComponent<Inventory>().addItem(1); //Add key piece

                    Controller.State_Idle();
                    dead = true;
                    
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
     public bool getAnimationProgress(int id)
    {
        //Check if certain frames are running / Attacking frames that should cause damage
        if(AnimationSet[id].GetComponent<AnimationData>().getActiveFrame() > 6 && AnimationSet[id].GetComponent<AnimationData>().getActiveFrame() < 8)
        {
            return true;
        }
        return false;
    }
}
