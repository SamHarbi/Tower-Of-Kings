using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogSystem : MonoBehaviour
{
    
    public Text dialogText;
    public string[] DialogRaw;
    private int currPage;
    private float interDialogTiming;
    private float postDialogTiming;
    private bool dialogProgress;

    // Start is called before the first frame update
    void Start()
    {
       interDialogTiming = 0.001f;
       postDialogTiming = 0.1f;

       dialogProgress = false;
       
       currPage = 0;

       gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool getDialogProgress()
    {
        return dialogProgress;
    }

    public void StartDialog()
    {
        dialogText.text = "";
        if(currPage >= DialogRaw.Length)
        {
            gameObject.SetActive(false);
            dialogProgress = true;
            return;
        }
        char[] Dialog1 = DialogRaw[currPage].ToCharArray();
        StartCoroutine(TypeText(Dialog1));
    }

    IEnumerator TypeText(char[] fullDialog)
    {
        for(int i=0; i<fullDialog.Length; i++)
        {
            dialogText.text += fullDialog[i];
            yield return new WaitForSeconds(interDialogTiming);
        }

        yield return new WaitForSeconds(postDialogTiming);
        currPage++;
        StartDialog();
    }
}
