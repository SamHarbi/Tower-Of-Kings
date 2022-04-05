using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationData : MonoBehaviour
{
    public int id;
    public Sprite[] frames;
    public int activeFrame;
    private int prevActiveFrame;
    private float updates;
    public float Timing;
    public GameObject parent;
    public bool Running;
    public bool nonloop;
    
    public void init(int newId, Sprite[] newFrames, float newTiming, GameObject newParent)
    {
        id = newId;
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
        prevActiveFrame = 99;
        
        
    }

    void Update()
    {
        if(activeFrame != prevActiveFrame && Running == true)
        {
            UpdateFrame();
        }
    }

    public void tickDown()
    {
        updates = updates - 1;
        if(updates <= 0)
        {
            updates = Timing;
            activeFrame = (activeFrame + 1) % frames.Length;
        }
        if(activeFrame + 1 == frames.Length && nonloop == true)
        {
            updates = Timing;
            activeFrame = frames.Length - 1;
            Running = false;
        }
    }

    public void UpdateFrame()
    {
        prevActiveFrame = activeFrame;
        parent.GetComponent<SpriteRenderer>().sprite = frames[activeFrame];
    }

}
