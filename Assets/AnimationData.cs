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

    void Update()
    {
        if(activeFrame != prevActiveFrame)
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
        Debug.Log("Ticked Down"); 
    }

    public void UpdateFrame()
    {
        prevActiveFrame = activeFrame;
        parent.GetComponent<SpriteRenderer>().sprite = frames[activeFrame];
    }

}
