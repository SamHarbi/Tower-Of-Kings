using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogSystem : MonoBehaviour
{
    
    public Text dialogText; //Object that holds text
    public string[] DialogRaw; //Dialog by line

    private int currPage; //Pages / line of dialog shown
    private float interDialogTiming; //Timing between letters being printed
    private float postDialogTiming; //Timing beteween new pages
    private bool dialogProgress; //Has Dialog finished?

    // Start is called before the first frame update
    void Start()
    {
       //Set Values
       interDialogTiming = 0.1f; 
       postDialogTiming = 1.2f;
       dialogProgress = false;
       currPage = 0;
       gameObject.SetActive(false);
    }

    public void setTiming(float newInter, float newPost)
    {
        interDialogTiming = newInter;
        postDialogTiming = newPost;
    }

    public bool getDialogProgress()
    {
        return dialogProgress;
    }

    public void StartDialog()
    {
        gameObject.SetActive(true);

        dialogText.text = ""; //Start empty

        if(currPage >= DialogRaw.Length) //If all dialog was shown
        {
            gameObject.SetActive(false);
            dialogProgress = true;
            return;
        }

        //Start typing chars from charArray 
        char[] Dialog1 = DialogRaw[currPage].ToCharArray();
        StartCoroutine(TypeText(Dialog1));
    }

    //Types each character from a single dialog page, then wipes and moves to next page till all done
    IEnumerator TypeText(char[] fullDialog)
    {
        for(int i=0; i<fullDialog.Length; i++)
        {
            dialogText.text += fullDialog[i]; //type a single character
            yield return new WaitForSeconds(interDialogTiming);
        }

        yield return new WaitForSeconds(postDialogTiming);
        currPage++;
        StartDialog(); //Check if dialog has finished and cleanup
    }
}
