using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* 
    Controls all UI elements for a Boss Fight, essentially everything that is on canvas and cameras
    Implements a Mediator Pattern between all Boss Scripts
*/

public class BossUIControl : MonoBehaviour
{
    
    //Boss UI control
    
    public GameObject mainCam; //Regular camera that follow player in non boss rooms
    public GameObject bossCam; //Camera for this boss fight
    public GameObject Canvas;
    public GameObject healthHider; //GameObject that is scaled to hide health bar giving the illusion of health going down
    public GameObject healthBar; //Healthbar that is under the healthHider
    public GameObject Gate; //Entry gate into Boss Fight - blocks player till fight is over
    public GameObject Gate2; //Temporary blocking gate that keeps the player in one place during dialog
    public GameObject bossName; //UI element with the name of the boss
    public GameObject dialogSystem; //GameObject with Dialog Component for this Boss Battle
    public GameObject dialogSystemEnd; //GameObject with Dialog Component for this Boss Battle - plays at end of battle
    public GameObject bossGate; //Activator that informs when the player is in the Boss Battle room / area
    public GameObject effectOverlay; //A color overlay over the screen
    public GameObject ThemeAudio; //Audio that plays during boss battle

    private IEnumerator upEffectCoroutine; //ref to coroutine to allow stopping it's execution
    private IEnumerator downEffectCoroutine; //ref to coroutine to allow stopping it's execution
    private float currAlpha; //current alpha color value of overlay
    //private bool fadeDown;
    private bool bossFightStarted;

    // Start is called before the first frame update
    void Start()
    {
        //Assign coroutine ref
        upEffectCoroutine = bossEffectFadeUP();
        downEffectCoroutine = bossEffectFadeDOWN();

        //Not fading down 
        currAlpha = 0f;
        //fadeDown = false;

        bossFightStarted = false;

    }

    // Update is called once per frame
    void Update()
    {
        //If start dialog ends, allow player to freely move into boss area
        if(dialogSystem.GetComponent<DialogSystem>().getDialogProgress())
        {
            Gate2.SetActive(false);
        }

        //If dialog is skipped, change timing of dialog much faster essentially skipping it
        if (Input.GetButtonDown("Skip-Cutscene")) //On input corresponding to Skip-Cutscene
        {
            if(dialogSystem.activeSelf)
            {
                dialogSystem.GetComponent<DialogSystem>().setTiming(0.001f, 0.01f);
            }
            if(dialogSystemEnd.activeSelf)
            {
                dialogSystemEnd.GetComponent<DialogSystem>().setTiming(0.001f, 0.1f);
            }
        }
    }

    //Called to set everything up for the boss fight
    public void startBossFight()
    {
        if(bossFightStarted == true)
        {
            return; //This code has already been run thus everything is already setup
        }
        
        if (Debug.isDebugBuild)
        {
            Debug.Log("Theme Started");
        }

        ThemeAudio.GetComponent<CameraLevelEffects>().StartBossTheme();
        
        //Change Camera's
        mainCam.SetActive(false);
        bossCam.SetActive(true);
        Canvas.GetComponent<Canvas>().worldCamera = bossCam.GetComponent<Camera>();

        //Lock player into small area in boss area
        Gate.SetActive(true);
        Gate2.SetActive(true);

        //Show the Boss name and start fading it down
        bossName.SetActive(true);
        StartCoroutine(bossTextFade());
            
        //Start Dialog
        if(!dialogSystem.activeSelf)
        {
            dialogSystem.SetActive(true);
            dialogSystem.GetComponent<DialogSystem>().StartDialog();
            //ThemeAudio.GetComponent<CameraColorEffects>().StartBossTheme();
        }
    }

    //Called to clean up post Boss Fight
    public void endBossFight()
    {
        if(!dialogSystemEnd.activeSelf)
        {
            //Play the end dialog
            dialogSystemEnd.SetActive(true);
            dialogSystemEnd.GetComponent<DialogSystem>().StartDialog();
            
            //Open all restrictions, allowing player to leave
            Gate.SetActive(false);
            Gate2.SetActive(false);

            //Inform BossGate to do final camera cleanup on exiting area
            bossGate.GetComponent<BossGate>().SetExit(true);

            //Stop all effects and start white screen wipe effect used by third boss
            StopCoroutine(downEffectCoroutine);
            StopCoroutine(upEffectCoroutine);
            StartCoroutine(bossEffectFadeUPVertical());
            
            //Stop Music
            ThemeAudio.GetComponent<CameraLevelEffects>().EndBossTheme();

            //Hide healthbar
            healthBar.SetActive(false);
            healthHider.SetActive(false);
        }
    }

    //Fade down Boss name until it disappears 
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

    //Fade effect overlay up
    IEnumerator bossEffectFadeUP()
    {
            while(true)
            {
                yield return new WaitForSeconds(0.2f);
                Color OriginalColor = effectOverlay.GetComponent<SpriteRenderer>().color;
                effectOverlay.GetComponent<SpriteRenderer>().color = new Color(OriginalColor.r,OriginalColor.g,OriginalColor.b, effectOverlay.GetComponent<SpriteRenderer>().color.a + 0.01f);
            }
    }

    //Fade effect overlay down
    IEnumerator bossEffectFadeDOWN()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.2f);
            Color OriginalColor = effectOverlay.GetComponent<SpriteRenderer>().color;
            effectOverlay.GetComponent<SpriteRenderer>().color = new Color(OriginalColor.r,OriginalColor.g,OriginalColor.b, effectOverlay.GetComponent<SpriteRenderer>().color.a - 0.01f);
        }
    }

    //Fade effectOverlay up then down
    IEnumerator bossEffectFadeUPVertical()
    {
            while(true)
            {
                //Increase Alpha
                yield return new WaitForSeconds(0.3f);
                Color OriginalColor = effectOverlay.GetComponent<SpriteRenderer>().color;
                effectOverlay.GetComponent<SpriteRenderer>().color = new Color(1,1,1, effectOverlay.GetComponent<SpriteRenderer>().color.a + 0.05f);
                currAlpha += 0.05f;
                if(Mathf.Approximately(currAlpha, 1.0f))
                {
                    while(true)
                    {
                        //Decrease Alpha
                        yield return new WaitForSeconds(0.3f);
                        OriginalColor = effectOverlay.GetComponent<SpriteRenderer>().color;
                        effectOverlay.GetComponent<SpriteRenderer>().color = new Color(1,1,1, effectOverlay.GetComponent<SpriteRenderer>().color.a - 0.05f);
                        currAlpha -= 0.05f;
                        if(currAlpha == 0)
                        {
                            //End
                            yield return null;
                        }
                    }
                }
            }
    }

    //Mediator that ensure that only one fade effect is running at any time on effectOverlay- called by BossWrapper
    public void FadeEffectStartUP()
    {
        StopCoroutine(downEffectCoroutine);

        //Set initial alpha
        Color OriginalColor = effectOverlay.GetComponent<SpriteRenderer>().color;
        effectOverlay.GetComponent<SpriteRenderer>().color = new Color(OriginalColor.r,OriginalColor.g,OriginalColor.b, 0.01f);

        StartCoroutine(upEffectCoroutine);
    }

    //Mediator that ensure that only one effect is running at any time- called by BossWrapper
    public void FadeEffectStartDOWN()
    {
        StopCoroutine(upEffectCoroutine);
        StartCoroutine(downEffectCoroutine);
    }

    //Is dialog running?
    public bool getDialogProgress()
    {
        return dialogSystem.GetComponent<DialogSystem>().getDialogProgress();
    }

    //Called by BossGate once the player has passed out of the Boss Battle Area
    public void resetUI()
    {
        //Reset camera back to how it was before the boss battle
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
