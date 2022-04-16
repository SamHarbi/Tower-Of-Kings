using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAIWrapper : MonoBehaviour
{

    private float BossHealth;
    public GameObject hitBoss;
    private int startDamageFrame;
    private int endDamageFrame;
    public GameObject bossWave;
    private bool bossWaveStarted;
    public GameObject bossWavePos;
    public GameObject BossDialog;
    public GameObject Boss;
    public AIController Controller;
    public GameObject[] AnimationSet;
    public GameObject LAS;

    public GameObject healthHider;
    public GameObject deathParticle;
    public GameObject bossUIControl;
    public GameObject Inventory;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LateStart());
    }

    IEnumerator LateStart()
    {
        yield return new WaitForSeconds(0.1f);
        BossHealth = 10;
        Controller = Boss.GetComponent<AIController>();
        Controller.setDamageFrames(8, 9);
        Controller.setDirections(false, false);
        Controller.setSearchRange(60f);
        Controller.wrapperOverride = true;
        bossWaveStarted = false;
        LAS = GameObject.FindWithTag("LAS");
        AnimationSet = LAS.GetComponent<LogicalAnimationSystem>().getAnimationDataArray(Boss);
    }

    // Update is called once per frame
    void Update()
    {
        if(Controller == null)
        {
            return;
        }

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
        Controller.search();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "PlayerAttackRange")
        {
                Controller.setDeathAnim(true);
                Controller.enableAnimation(3);
                StartCoroutine(Controller.BossHitTimer());
                BossHealth = BossHealth - 1;
                healthHider.transform.localScale = new Vector3(healthHider.transform.localScale.x+1, healthHider.transform.localScale.y, healthHider.transform.localScale.z);
                healthHider.transform.position = new Vector3(healthHider.transform.position.x - 0.5f, healthHider.transform.position.y, healthHider.transform.position.z);
                if(BossHealth <= 0)
                {
                    //On Death
                    deathParticle.GetComponent<ParticleSystem>().Play();
                    gameObject.GetComponent<Collider2D>().enabled = false;
                    gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX;
                    gameObject.GetComponent<Rigidbody2D>().gravityScale = 10f;
                    bossUIControl.GetComponent<BossUIControl>().endBossFight();
                    Inventory.GetComponent<Inventory>().addItem(0);
                }
               //Vector3 hitPos = new Vector3(col.transform.position.x + 2, col.transform.position.y + 2, gameObject.transform.position.z);
                //Instantiate(hitBoss, hitPos, Quaternion.identity);
        }
    }

    private bool getAnimationProgress(int id)
    {
        //Check if certain frames are running / Attacking frames that should cause damage
        if(AnimationSet[id].GetComponent<AnimationData>().activeFrame > 3 && AnimationSet[id].GetComponent<AnimationData>().activeFrame < 9)
        {
            return true;
        }
        return false;
    }

}
