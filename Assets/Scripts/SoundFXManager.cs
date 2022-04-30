using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    A collection of sound effects that can be called by an Object to play
*/

public class SoundFXManager : MonoBehaviour
{
    public GameObject AudioSource; //Where Audio is played from
    public GameObject lowPriorAudioSource; //Where low priority audio is played from

    //Sound Effects and variations in arrays
    public AudioClip[] hit;
    public AudioClip[] slash;
    public AudioClip jump;
    public AudioClip pickup;
    public AudioClip win;
    public AudioClip lose;
    public AudioClip[] chainMail;

    //pointers to which variation to play next
    private int currSlash;
    private int currChainMail;
    private int currHit;
    
    // Start is called before the first frame update
    void Start()
    {
        //Setup values
        currSlash = 0;
        currChainMail = 0;
        currHit = 0;
    }

    public void Slash()
    {
        //Set the Audio to one pointed to
        AudioSource.GetComponent<AudioSource>().clip = slash[currSlash];

        //Play the Audio
        AudioSource.GetComponent<AudioSource>().Play();

        //Change pointer to next Audio
        currSlash = (currSlash+1) % slash.Length;
    }

    public void Jump()
    {
        AudioSource.GetComponent<AudioSource>().clip = jump;
        AudioSource.GetComponent<AudioSource>().Play();
    }
    public void clothMove()
    {
        if(!lowPriorAudioSource.GetComponent<AudioSource>().isPlaying) //Play only if nothing is already playing
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

    public void Pickup()
    {
        AudioSource.GetComponent<AudioSource>().clip = pickup;
        AudioSource.GetComponent<AudioSource>().Play();
    }

    public void Win()
    {
        AudioSource.GetComponent<AudioSource>().clip = win;
        AudioSource.GetComponent<AudioSource>().Play();
    }

    public void Lose()
    {
        AudioSource.GetComponent<AudioSource>().clip = lose;
        AudioSource.GetComponent<AudioSource>().Play();
    }

}
