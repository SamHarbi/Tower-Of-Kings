using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public GameObject AudioSource;
    public GameObject lowPriorAudioSource;
    public AudioClip[] slash;
    public AudioClip jump;
    public AudioClip[] chainMail;
    private int currSlash;
    private int currChainMail;
    public AudioClip[] hit;
    private int currHit;
    
    // Start is called before the first frame update
    void Start()
    {
        currSlash = 0;
        currChainMail = 0;
        currHit = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Slash()
    {
        AudioSource.GetComponent<AudioSource>().clip = slash[currSlash];
        AudioSource.GetComponent<AudioSource>().Play();
        currSlash = (currSlash+1) % slash.Length;
    }

    public void Jump()
    {
        AudioSource.GetComponent<AudioSource>().clip = jump;
        AudioSource.GetComponent<AudioSource>().Play();
    }
    public void clothMove()
    {
        if(!lowPriorAudioSource.GetComponent<AudioSource>().isPlaying)
        {
            lowPriorAudioSource.GetComponent<AudioSource>().clip = chainMail[currChainMail];
            lowPriorAudioSource.GetComponent<AudioSource>().Play();
            currChainMail = (currChainMail+1) % chainMail.Length;
        }
        
    }

    public void Hit()
    {
        AudioSource.GetComponent<AudioSource>().clip = hit[currHit];
        AudioSource.GetComponent<AudioSource>().Play();
        currSlash = (currHit+1) % hit.Length;
    }

}
