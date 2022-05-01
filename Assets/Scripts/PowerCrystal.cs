using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Crystals that stun the third boss on hit and cause a longer stun if all are hit- allowing the Player to do damage to the Boss
*/

public class PowerCrystal : MonoBehaviour
{
    
    public GameObject[] AnimationSet;
    public GameObject LAS;
    private int currAnim;
    public bool State; //Is the crystal broken or not?
    private GameObject Particles;

    // Start is called before the first frame update
    void Start()
    {
        //Start Idle Animation and setup values
        currAnim = 0;
        enableAnimation(0);
        State = true;
        Particles = gameObject;
    }

    void Awake()
    {
        LAS = GameObject.FindWithTag("LAS");
        AnimationSet = LAS.GetComponent<LogicalAnimationSystem>().getAnimationDataArray(gameObject);
    }

    //Return to a idle unbroken state
    public void ResetState()
    {
        enableAnimation(0);
        Particles.GetComponent<ParticleSystem>().Play();
        State = true;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "PlayerAttackRange")
        {
            //Change to be broken
            enableAnimation(1);
            State = false;
            Particles.GetComponent<ParticleSystem>().Stop();
            
        }
    }

     //Each AnimationData Object acts as a State that changes the visual of the GameObject
     public void enableAnimation(int num)
    {
        //Run set animation and deactivate current running Animation
        AnimationSet[num].GetComponent<AnimationData>().Running = true;
        if(num != currAnim)
        {
            AnimationSet[currAnim].GetComponent<AnimationData>().Running = false;
            currAnim = num;
        }
    }

    public bool getAnimationProgress(int id)
    {
        //Check if certain frames are running / Attacking frames that should cause damage
        if(AnimationSet[id].GetComponent<AnimationData>().getActiveFrame() > 3 && AnimationSet[id].GetComponent<AnimationData>().getActiveFrame() < 9)
        {
            return true;
        }
        return false;
    }
}
