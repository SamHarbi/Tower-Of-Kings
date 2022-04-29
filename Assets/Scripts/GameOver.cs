using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*
    Gameover Cutscene, returns to menu
*/

public class GameOver : MonoBehaviour
{
    
    public GameObject Anim; //AnimationData with animation
    public GameObject Crown; //Crown Gameobject Visual
    public GameObject AnimationSystem; //Logical Animation System
    public GameObject tryAgainText; //Text informing that player has lost
    public GameObject tryAgainPrompt; //prompt with further instructions
    private bool breaking; //Is animation running?
    private bool endOfScreen;
    
    // Start is called before the first frame update
    void Start()
    {
        //Set initial values
        breaking = false;
        endOfScreen = false;
    }

    // Update is called once per frame
    void Update()
    {
        //If animations have finished and all text has been shown
        if(endOfScreen == true && Input.anyKey) //if any key is further pressed 
        {
            SceneManager.LoadScene("StartMenu"); //Go to start menu
        }
    }

    public void StartFade()
    {
        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        yield return new WaitForSeconds(0.6f);
        gameObject.GetComponent<SpriteRenderer>().color = new Color(0,0,0,  gameObject.GetComponent<SpriteRenderer>().color.a + 0.05f);
        if(Mathf.Approximately(gameObject.GetComponent<SpriteRenderer>().color.a, 1.0f))
        {
            BreakCrown();
            yield return null;
        }
    
    }

    IEnumerator ShowText()
    {
        yield return new WaitForSeconds(1.6f);
        tryAgainText.SetActive(true);
    }

    IEnumerator ShowPrompt()
    {
        yield return new WaitForSeconds(1.8f);
        tryAgainPrompt.SetActive(true);
        StartCoroutine(FadeCyclePromptDown());
        
    }

    IEnumerator FadeCyclePromptUp()
    {
        endOfScreen = true;
        tryAgainPrompt.GetComponent<Text>().color = new Color(1,1,1,  tryAgainPrompt.GetComponent<Text>().color.a + 1f);
        yield return new WaitForSeconds(0.6f);
        StartCoroutine(FadeCyclePromptDown());
    }

    IEnumerator FadeCyclePromptDown()
    {
        tryAgainPrompt.GetComponent<Text>().color = new Color(1,1,1,  tryAgainPrompt.GetComponent<Text>().color.a - 1f);
        yield return new WaitForSeconds(0.6f);
        StartCoroutine(FadeCyclePromptUp());
    }

    public void BreakCrown()
    {
         Crown.SetActive(true);
         AnimationSystem.GetComponent<LogicalAnimationSystem>().addLateAnimation(Anim);
         Anim.GetComponent<AnimationData>().Running = true;
         StartCoroutine(ShowText());
         StartCoroutine(ShowPrompt());
    }
}
