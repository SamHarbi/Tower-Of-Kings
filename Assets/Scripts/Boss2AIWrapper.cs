using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2AIWrapper : MonoBehaviour
{
    private float BossHealth;
    public GameObject BossDialog;
    public AIController Controller;
    private GameObject Boss;
    public GameObject healthHider;
    public GameObject bossUIControl;
    public GameObject Inventory;
    public GameObject attackRange;
    public GameObject healthBar;
    public GameObject[] AnimationSet;
    public GameObject LAS;

    // Start is called before the first frame update
    void Start()
    {
        //Get Animation Data
        LAS = GameObject.FindWithTag("LAS");
        AnimationSet = LAS.GetComponent<LogicalAnimationSystem>().getAnimationDataArray(gameObject);

        StartCoroutine(LateStart());
    }

    IEnumerator LateStart()
    {
        yield return new WaitForSeconds(0.1f);
        BossHealth = 20;

        //
        Controller = gameObject.GetComponent<AIController>();
        Controller.setDamageFrames(6, 8);
        Controller.wrapperOverride = true;
        Boss = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        search();

        if(getAnimationProgress(2) == true)
        {
            attackRange.SetActive(true);
        }
        else
        {
            attackRange.SetActive(false);
        }
    }

    void search()
    {
        if(BossDialog.GetComponent<DialogSystem>().getDialogProgress() == false)
        {
            Controller.State_Idle();
            return;
        }
        Controller.search();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "PlayerAttackRange")
        {
                Controller.setDeathAnim(true);
                Controller.enableAnimation(3);
                StartCoroutine(BossHitTimer());
                BossHealth = BossHealth - 1;
                healthHider.transform.localScale = new Vector3(healthHider.transform.localScale.x+0.5f, healthHider.transform.localScale.y, healthHider.transform.localScale.z);
                healthHider.transform.position = new Vector3(healthHider.transform.position.x - 0.25f, healthHider.transform.position.y, healthHider.transform.position.z);
                if(BossHealth <= 0)
                {
                    //On Death
                    bossUIControl.GetComponent<BossUIControl>().endBossFight();
                    Inventory.GetComponent<Inventory>().addItem(1);
                    Controller.enabled = false;
                    healthBar.SetActive(false);
                    healthHider.SetActive(false);
                    
                }
        }
    }

    public IEnumerator BossHitTimer()
    {
        yield return new WaitForSeconds(0.5f);
        Controller.setDeathAnim(false);
    }

    IEnumerator BossDeathTimer()
    {
        yield return new WaitForSeconds(2.5f);
        gameObject.SetActive(false);
    }

     public bool getAnimationProgress(int id)
    {
        //Check if certain frames are running / Attacking frames that should cause damage
        if(AnimationSet[id].GetComponent<AnimationData>().activeFrame > 6 && AnimationSet[id].GetComponent<AnimationData>().activeFrame < 8)
        {
            return true;
        }
        return false;
    }
}
