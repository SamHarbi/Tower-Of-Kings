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
    public GameObject healthHider; //GameObject that is scaled to hide health bar giving the illusion of health going down
    public GameObject healthBar;
    public GameObject Gate;
    public GameObject Gate2;
    public GameObject bossName;
    public GameObject dialogSystem;
    public GameObject dialogSystemEnd;
    public GameObject bossGate;
    public GameObject effectOverlay;
    public GameObject ThemeAudio;
    private IEnumerator upEffectCoroutine;
    private IEnumerator downEffectCoroutine;
    private float currAlpha;
    private bool fadeDown;
    private bool bossFightStarted;
  



    // Start is called before the first frame update
    void Start()
    {
        upEffectCoroutine = bossEffectFadeUP();
        downEffectCoroutine = bossEffectFadeDOWN();

        currAlpha = 0f;
        fadeDown = false;

        bossFightStarted = false;

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
        if(bossFightStarted == false)
        {
            Debug.Log("Theme Started");
            ThemeAudio.GetComponent<CameraColorEffects>().StartBossTheme();
            
        }
        
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
            ThemeAudio.GetComponent<CameraColorEffects>().StartBossTheme();
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
            StartCoroutine(bossEffectFadeUPVertical());
            
            ThemeAudio.GetComponent<CameraColorEffects>().EndBossTheme();

            healthBar.SetActive(false);
            healthHider.SetActive(false);
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

    IEnumerator bossEffectFadeUPVertical()
    {

            while(true)
            {
                yield return new WaitForSeconds(0.3f);
                Color OriginalColor = effectOverlay.GetComponent<SpriteRenderer>().color;
                effectOverlay.GetComponent<SpriteRenderer>().color = new Color(1,1,1, effectOverlay.GetComponent<SpriteRenderer>().color.a + 0.05f);
                currAlpha += 0.05f;
                if(Mathf.Approximately(currAlpha, 1.0f))
                {
                    while(true)
                    {
                        yield return new WaitForSeconds(0.3f);
                        OriginalColor = effectOverlay.GetComponent<SpriteRenderer>().color;
                        effectOverlay.GetComponent<SpriteRenderer>().color = new Color(1,1,1, effectOverlay.GetComponent<SpriteRenderer>().color.a - 0.05f);
                        currAlpha -= 0.05f;
                        if(currAlpha == 0)
                        {
                            yield return null;
                        }
                    }
                }
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

    public void updateHealthBar(float mod)
    {
        //Extend health hider to hide more of the health bar- This gives the illusion of health going down in the health bar that is otherwise a static image
        healthHider.transform.localScale = new Vector3(healthHider.transform.localScale.x+mod, healthHider.transform.localScale.y, healthHider.transform.localScale.z);
        healthHider.transform.position = new Vector3(healthHider.transform.position.x - (mod/2), healthHider.transform.position.y, healthHider.transform.position.z);
    }
}
