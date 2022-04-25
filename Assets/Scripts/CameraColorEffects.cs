using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraColorEffects : MonoBehaviour
{
    public Color midLevelColor;
    public Color lowerLevelColor;
    public Color upperLevelColor;

    public GameObject midLevelTheme;
    public GameObject upperLevelTheme;
    public GameObject lowerLevelTheme;

    public GameObject AudioSourceTheme;
    
    // Start is called before the first frame update
    void Start()
    {
        AudioSourceTheme = midLevelTheme;

        StartCoroutine(audioFade(0.2f, 0.05f));
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.transform.position.y > 81f)
        {
            gameObject.GetComponent<Camera>().backgroundColor = midLevelColor;
            AudioSourceTheme = midLevelTheme;
        }
        else
        {
            gameObject.GetComponent<Camera>().backgroundColor = lowerLevelColor;
            AudioSourceTheme = lowerLevelTheme;
        }

        if(gameObject.transform.position.y > 143f)
        {
            gameObject.GetComponent<Camera>().backgroundColor = upperLevelColor;
            AudioSourceTheme = upperLevelTheme;
        }

        Debug.Log(Time.timeScale);
    }

    IEnumerator audioFade(float goal, float change)
    {
        AudioSourceTheme.GetComponent<AudioSource>().Play();
        while(AudioSourceTheme.GetComponent<AudioSource>().volume != goal)
        {
            AudioSourceTheme.GetComponent<AudioSource>().volume = AudioSourceTheme.GetComponent<AudioSource>().volume + change;
            yield return new WaitForSeconds(0.5f);
        }
        yield return null;
    }
}
