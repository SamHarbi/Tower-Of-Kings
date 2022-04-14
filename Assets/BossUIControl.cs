using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossUIControl : MonoBehaviour
{
    
    //Boss UI control
    
    public GameObject mainCam;
    public GameObject bossCam;
    public GameObject Canvas;
    public GameObject Gate;
    public GameObject Gate2;
    public GameObject bossName;
    public GameObject dialogSystem;
    public GameObject dialogSystemEnd;
    public GameObject bossGate;
  



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(dialogSystem.GetComponent<DialogSystem>().getDialogProgress())
        {
            Gate2.SetActive(false);
        }

        if (Input.GetAxis("Skip Cutscene") < 0 || Input.GetKey("q"))
        {
            if(dialogSystem.active)
            {
                dialogSystem.GetComponent<DialogSystem>().setTiming(0.01f, 0.1f);
            }
            if(dialogSystemEnd.active)
            {
                dialogSystemEnd.GetComponent<DialogSystem>().setTiming(0.01f, 0.1f);
            }
        }
    }

    public void startBossFight()
    {
        mainCam.SetActive(false);
        bossCam.SetActive(true);
        Canvas.GetComponent<Canvas>().worldCamera = bossCam.GetComponent<Camera>();
        Gate.SetActive(true);
        Gate2.SetActive(true);
        bossName.SetActive(true);
        StartCoroutine(bossTextFade());
            
        if(!dialogSystem.active)
        {
            dialogSystem.SetActive(true);
            dialogSystem.GetComponent<DialogSystem>().StartDialog();
        }

    }

    public void endBossFight()
    {
        if(!dialogSystemEnd.active)
        {
            dialogSystemEnd.SetActive(true);
            dialogSystemEnd.GetComponent<DialogSystem>().StartDialog();
            Gate.SetActive(false);
            Gate2.SetActive(false);
            bossGate.GetComponent<BossGate>().SetExit(true);
            
        }
    }

    IEnumerator bossTextFade()
    {
        while(!Mathf.Approximately(bossName.GetComponent<Text>().color.a, 0f))
        {
            yield return new WaitForSeconds(0.2f);
            bossName.GetComponent<Text>().color = new Color(0.6705883f,0.2588235f,0.08235294f,  bossName.GetComponent<Text>().color.a - 0.05f);
        }
        
        bossName.SetActive(false);
        yield return null;
        
    }

    public bool getDialogProgress()
    {
        return dialogSystem.GetComponent<DialogSystem>().getDialogProgress();
    }

    public void resetUI()
    {
        mainCam.SetActive(true);
        bossCam.SetActive(false);
        Canvas.GetComponent<Canvas>().worldCamera = mainCam.GetComponent<Camera>();
    }
}
