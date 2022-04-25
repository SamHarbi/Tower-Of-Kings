using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    
    public GameObject Anim;
    public GameObject Crown;
    public GameObject AnimationSystem;
    public GameObject tryAgainText;
    public GameObject tryAgainPrompt;
    private bool breaking;
    private bool endOfScreen;
    
    // Start is called before the first frame update
    void Start()
    {
        breaking = false;
        endOfScreen = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(endOfScreen == true && Input.anyKey)
        {
            SceneManager.LoadScene("StartMenu");
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
        //Code inspired by https://answers.unity.com/questions/1429656/how-can-i-check-if-the-alpha-color-or-the-material.html
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
