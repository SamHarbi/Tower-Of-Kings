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
    public GameObject effectOverlay;
    private IEnumerator upEffectCoroutine;
    private IEnumerator downEffectCoroutine;
  



    // Start is called before the first frame update
    void Start()
    {
        upEffectCoroutine = bossEffectFadeUP();
        downEffectCoroutine = bossEffectFadeDOWN();

    }

    // Update is called once per frame
    void Update()
    {
        if(dialogSystem.GetComponent<DialogSystem>().getDialogProgress())
        {
            Gate2.SetActive(false);
        }

        if (Input.GetButtonDown("Skip-Cutscene"))
        {
            if(dialogSystem.active)
            {
                dialogSystem.GetComponent<DialogSystem>().setTiming(0.001f, 0.01f);
            }
            if(dialogSystemEnd.active)
            {
                dialogSystemEnd.GetComponent<DialogSystem>().setTiming(0.001f, 0.1f);
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
            Gate.SetActive(false);
            Gate2.SetActive(false);
            bossGate.GetComponent<BossGate>().SetExit(true);
            dialogSystemEnd.GetComponent<DialogSystem>().StartDialog();

            StopCoroutine(downEffectCoroutine);
            StopCoroutine(upEffectCoroutine);
            StartCoroutine(bossEffectFadeUPWhite());
            
        }
    }

    IEnumerator bossTextFade()
    {
        while(!Mathf.Approximately(bossName.GetComponent<Text>().color.a, 0f))
        {
            yield return new WaitForSeconds(0.2f);
            Color OriginalColor = bossName.GetComponent<Text>().color;
            bossName.GetComponent<Text>().color = new Color(OriginalColor.r,OriginalColor.g,OriginalColor.b,  bossName.GetComponent<Text>().color.a - 0.05f);
        }
        
        bossName.SetActive(false);
        yield return null;
        
    }

    IEnumerator bossEffectFadeUP()
    {
            while(true)
            {
                yield return new WaitForSeconds(0.2f);
                Color OriginalColor = effectOverlay.GetComponent<SpriteRenderer>().color;
                effectOverlay.GetComponent<SpriteRenderer>().color = new Color(OriginalColor.r,OriginalColor.g,OriginalColor.b, effectOverlay.GetComponent<SpriteRenderer>().color.a + 0.01f);
            }
        
        yield return null;

        
    }

    IEnumerator bossEffectFadeDOWN()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.2f);
            Color OriginalColor = effectOverlay.GetComponent<SpriteRenderer>().color;
            effectOverlay.GetComponent<SpriteRenderer>().color = new Color(OriginalColor.r,OriginalColor.g,OriginalColor.b, effectOverlay.GetComponent<SpriteRenderer>().color.a - 0.01f);
        }

        yield return null;
        
    }

    IEnumerator bossEffectFadeUPWhite()
    {
            while(true)
            {
                yield return new WaitForSeconds(0.2f);
                Color OriginalColor = effectOverlay.GetComponent<SpriteRenderer>().color;
                effectOverlay.GetComponent<SpriteRenderer>().color = new Color(1,1,1, effectOverlay.GetComponent<SpriteRenderer>().color.a + 0.05f);
            }
        //StartCoroutine(bossEffectFadeDOWNWhite());
        yield return null;
        
    }

    IEnumerator bossEffectFadeDOWNWhite()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.2f);
            Color OriginalColor = effectOverlay.GetComponent<SpriteRenderer>().color;
            effectOverlay.GetComponent<SpriteRenderer>().color = new Color(1,1,1, effectOverlay.GetComponent<SpriteRenderer>().color.a - 0.05f);
        }

        yield return null;
        
    }

    public void FadeEffectStartUP()
    {
        StopCoroutine(downEffectCoroutine);
        Color OriginalColor = effectOverlay.GetComponent<SpriteRenderer>().color;
        effectOverlay.GetComponent<SpriteRenderer>().color = new Color(OriginalColor.r,OriginalColor.g,OriginalColor.b, 0.01f);
        StartCoroutine(upEffectCoroutine);
    }

    public void FadeEffectStartDOWN()
    {
        StopCoroutine(upEffectCoroutine);
        StartCoroutine(downEffectCoroutine);
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
