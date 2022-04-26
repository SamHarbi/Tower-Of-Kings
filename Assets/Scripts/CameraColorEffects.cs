using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraColorEffects : MonoBehaviour
{
    public Color midLevelColor;
    public Color lowerLevelColor;
    public Color upperLevelColor;

    public AudioClip midLevelTheme;
    public AudioClip upperLevelTheme;
    public AudioClip lowerLevelTheme;

    public AudioClip BossTheme;

    public GameObject AudioSourceTheme;
    private int currTheme;
    private bool fadeDone;
    private bool BossFight;
    
    // Start is called before the first frame update
    void Start()
    {
        AudioSourceTheme.GetComponent<AudioSource>().clip = midLevelTheme;
        AudioSourceTheme.GetComponent<AudioSource>().Play();
        currTheme = 0;
        fadeDone = false;
        BossFight = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(BossFight == true)
        {
            return;
        }
        
        if(gameObject.transform.position.y >= 81f && gameObject.transform.position.y < 143f)
        {
            gameObject.GetComponent<Camera>().backgroundColor = midLevelColor;
            if(currTheme != 0)
            {
                AudioSourceTheme.GetComponent<AudioSource>().clip = midLevelTheme;
                AudioSourceTheme.GetComponent<AudioSource>().Play();
                currTheme = 0;
                Debug.Log("Changed Audio");
            }
        }
        else if(gameObject.transform.position.y >= 143f)
        {
           gameObject.GetComponent<Camera>().backgroundColor = upperLevelColor;
           if(currTheme != 1)
            {
                AudioSourceTheme.GetComponent<AudioSource>().clip = upperLevelTheme;
                AudioSourceTheme.GetComponent<AudioSource>().Play();
                currTheme = 1;
                Debug.Log("Changed Audio");
            }
        }
        else
        {
            gameObject.GetComponent<Camera>().backgroundColor = lowerLevelColor;
            if(currTheme != 2)
            {
                AudioSourceTheme.GetComponent<AudioSource>().clip = lowerLevelTheme;
                AudioSourceTheme.GetComponent<AudioSource>().Play();
                currTheme = 2;
                Debug.Log("Changed Audio");
            }
        } 

    }

    public void StartBossTheme()
    {
        if(BossFight == true)
        {
            return;
        }
        
        AudioSourceTheme.GetComponent<AudioSource>().clip = BossTheme;
        AudioSourceTheme.GetComponent<AudioSource>().Play();
        BossFight = true;
    }

    public void EndBossTheme()
    {
        AudioSourceTheme.GetComponent<AudioSource>().Stop();
        BossFight = false;
    }

    IEnumerator audioFade(float goal, float change)
    {
        //AudioSourceTheme.GetComponent<AudioSource>().Play();
        fadeDone = false;

        while(AudioSourceTheme.GetComponent<AudioSource>().volume != goal)
        {
            AudioSourceTheme.GetComponent<AudioSource>().volume = AudioSourceTheme.GetComponent<AudioSource>().volume + change;
            if(AudioSourceTheme.GetComponent<AudioSource>().volume != goal)
            {
                fadeDone = true;
            }
            yield return new WaitForSeconds(0.5f);
        }
        
    }
}
