using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLevelEffects : MonoBehaviour
{
    
    //Colors that fill the background on each level of the tower
    public Color midLevelColor;
    public Color lowerLevelColor;
    public Color upperLevelColor;

    //Music Theme for each level of the tower
    public AudioClip midLevelTheme;
    public AudioClip upperLevelTheme;
    public AudioClip lowerLevelTheme;

    //Theme that is played when needed by a boss 
    public AudioClip BossTheme;


    public GameObject AudioSourceTheme; //reference to Object that has Audio Component
    private int currTheme; //Currently playing music theme
    //private bool fadeDone; //Has a sound fully faded in?
    private bool BossFight; //Is a boss fight happening?
    
    // Start is called before the first frame update
    void Start()
    {
        //Set initial variable values
        AudioSourceTheme.GetComponent<AudioSource>().clip = midLevelTheme; //Since the Player starts on the middle floors 
        AudioSourceTheme.GetComponent<AudioSource>().Play();
        currTheme = 0;
        //fadeDone = false;
        BossFight = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(BossFight == true)
        {
            return; //Don't change anything while a boss fight is in progress
        }
        
        //If on middle floors
        if(gameObject.transform.position.y >= 81f && gameObject.transform.position.y < 143f)
        {
            gameObject.GetComponent<Camera>().backgroundColor = midLevelColor;
            if(currTheme != 0)
            {
                AudioSourceTheme.GetComponent<AudioSource>().clip = midLevelTheme;
                AudioSourceTheme.GetComponent<AudioSource>().Play();
                currTheme = 0;
                if (Debug.isDebugBuild)
                {
                    Debug.Log("Changed Audio");
                }
            }
        }
        else if(gameObject.transform.position.y >= 143f) //If on upper floors
        {
           gameObject.GetComponent<Camera>().backgroundColor = upperLevelColor;
           if(currTheme != 1)
            {
                AudioSourceTheme.GetComponent<AudioSource>().clip = upperLevelTheme;
                AudioSourceTheme.GetComponent<AudioSource>().Play();
                currTheme = 1;
                if (Debug.isDebugBuild)
                {
                    Debug.Log("Changed Audio");
                }
            }
        }
        else //If on lower floors
        {
            gameObject.GetComponent<Camera>().backgroundColor = lowerLevelColor;
            if(currTheme != 2)
            {
                AudioSourceTheme.GetComponent<AudioSource>().clip = lowerLevelTheme;
                AudioSourceTheme.GetComponent<AudioSource>().Play();
                currTheme = 2;
                if (Debug.isDebugBuild)
                {
                    Debug.Log("Changed Audio");
                }
            }
        } 

    }

    public void StartBossTheme()
    {
        if(BossFight == true)
        {
            return; //Boss fight already started
        }
        
        //Set to boss music
        AudioSourceTheme.GetComponent<AudioSource>().clip = BossTheme;
        AudioSourceTheme.GetComponent<AudioSource>().Play();
        BossFight = true;
    }

    public void EndBossTheme()
    {
        //End all music while player stays on floor
        AudioSourceTheme.GetComponent<AudioSource>().Stop();
        BossFight = false;
    }

    //Fade audio step by step represented by change
    IEnumerator audioFade(float goal, float change)
    {
        //AudioSourceTheme.GetComponent<AudioSource>().Play();
        //fadeDone = false;

        while(AudioSourceTheme.GetComponent<AudioSource>().volume != goal)
        {
            AudioSourceTheme.GetComponent<AudioSource>().volume = AudioSourceTheme.GetComponent<AudioSource>().volume + change;
            if(AudioSourceTheme.GetComponent<AudioSource>().volume != goal)
            {
                //fadeDone = true;
            }
            yield return new WaitForSeconds(0.5f);
        }
        
    }
}
