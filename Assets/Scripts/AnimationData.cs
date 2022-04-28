using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationData : MonoBehaviour
{
    public Sprite[] frames; //Frames making the Animation
    private int activeFrame; //Currently running frame
    private int lastFrame; //Last frame in an Animation
    private int prevActiveFrame; //Previous frame that was running
    private float updates; //How many ticks remain before frame is changed
    public float Timing; //How many ticks should a frame be active for
    public GameObject parent; //GameObject to which animation is applied to
    public bool Running; //Is the animation actively ticking down / running
    public bool nonloop; //Should the animation stop on last frame and not loop
    
    //Initializer for creating animations through code
    public void init(int newId, Sprite[] newFrames, float newTiming, GameObject newParent)
    {
        frames = new Sprite[newFrames.Length];
        for(int i=0; i<newFrames.Length; i++)
        {
            frames[i] = newFrames[i];
        }
        Timing = newTiming;
        activeFrame = 0;
        prevActiveFrame = 0;

        parent = newParent;
    }

    void Start()
    {
        //initialize frame values to enter loop
        prevActiveFrame = 9999;
        lastFrame = frames.Length - 1;
    }

    void Update()
    {
        //Active frame has been changed
        if(activeFrame != prevActiveFrame && Running == true)
        {
            UpdateFrame();
        }
    }

    public int getActiveFrame()
    {
        return activeFrame;
    }

    public int getLastFrame()
    {
        return  lastFrame;
    }

    //tickDown is called by Logical Animation System every time a set number of updates passes
    public void tickDown()
    {
        if(Running == false && nonloop == false)
        {
            activeFrame = 0;
            return;
        }
        
        //Secondary clock, check how many ticks down have occured
        updates = updates - 1;
        if(updates <= 0)
        {
            //reset counting and update frame
            updates = Timing;
            activeFrame = (activeFrame + 1) % frames.Length;
        }
        if(activeFrame + 1 == frames.Length && nonloop == true)
        {
            updates = Timing;
            activeFrame = frames.Length - 1;
            Running = false; //stop running on last frame
        }
    }

    public void UpdateFrame()
    {
        prevActiveFrame = activeFrame;
        parent.GetComponent<SpriteRenderer>().sprite = frames[activeFrame];
    }

}
