using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    private Animator anim; //Animation Controller
    public GameObject blade; //Current active UI blade - left or right
    private GameObject bladeLeft; //Left UI Blade 
    private GameObject bladeRight;//Right UI Blade
    private SpriteRenderer bladeSR; //Sprite Renderer for Blade - used to change stances UI

    public Sprite[] stanceSprites; //Hold sprites representing UI for each Stance

    public EnemyDetector leftEnemyDetector;
    public EnemyDetector rightEnemyDetector;

    private int currStance;

    
    // Start is called before the first frame update
    void Start()
    {
        //Import UI blades and initialize them 
        bladeRight = GameObject.FindWithTag("ArmRight");
        bladeLeft = GameObject.FindWithTag("ArmLeft");
        
        bladeLeft.SetActive(false);
        bladeRight.SetActive(false);
        blade = bladeRight;

        bladeSR = blade.GetComponent<SpriteRenderer>();
       
        //Assign Animator to anim
        anim = gameObject.GetComponent<Animator>();

        currStance = 0;


        //OverlapStart = new Vector2(transform.position.x, transform.position.y - 3);
        //OverlapEnd = new Vector2(transform.position.x - 5, transform.position.y + 5);


        //previousDirection = false; //left is false, right is true
        //Instantiate(testItem, OverlapStart, Quaternion.identity);
        //Instantiate(testItem, OverlapEnd, Quaternion.identity);
        

    }

    // Update is called once per frame
    void Update()
    {
        //Combat Logic - Attacking
        if (Input.GetButtonDown("Fire1"))
        {
           anim.ResetTrigger("Attack");
           anim.SetTrigger("Attack");
        }

        //Combat Stances - Change UI and modify attack
        if (Input.GetAxis("Combat-Vertical") < 0 || Input.GetKey("up"))
        {
            MoveStance(1); //Upper Stance
        }
        else if (Input.GetAxis("Combat-Vertical") > 0 || Input.GetKey("down"))
        {
            MoveStance(0); //Lower Stance
        }

        //Movement Logic - Walking Left and Right
        if (Input.GetAxis("Dpad-Horizontal") < 0 || Input.GetKey("a")) //Left
        {
            Move(true);
            rightEnemyDetector.resetViewControl(); //Clear Enemy Sprite from opposite combat panel area 
        }
        else if(Input.GetAxis("Dpad-Horizontal") > 0 || Input.GetKey("d")) //Right
        {
            Move(false);
            leftEnemyDetector.resetViewControl(); //Clear Enemy Sprite from opposite combat panel area 
        }
/*
        if(isBladeAtDestination != 0)
        {
            if(isBladeAtDestination > 0)
            {
                transform.position = Vector2.Lerp(transform.position, bladeLocationData[1], 0.5f);
            }
            if(isBladeAtDestination < 0)
            {
                transform.position = Vector3.Lerp(transform.position, bladeLocationData[2], 0.5f);
            }
        }*/

    }

    //Basic Left & Right Movement
    void Move(bool mod)
    {
        if(mod == false) //Left
        {
            blade = bladeLeft; //Change Blades

            blade.SetActive(true);
            bladeRight.SetActive(false);
        }
        else //Right
        {
            blade = bladeRight; //Change Blades

            blade.SetActive(true);
            bladeLeft.SetActive(false);
        }
    }

    //Change Stances 
    void MoveStance(int axis)
    {
        if(axis == 1) // Upper Stance
        {

            bladeSR = blade.GetComponent<SpriteRenderer>();
            Vector2 bladePos = blade.GetComponent<Transform>().position;
            bladePos = new Vector2(bladePos.x, bladePos.y -5);
            bladeSR.sprite = stanceSprites[1];
            currStance = 0;
        }
        else //Lower Stance
        {
            
            bladeSR = blade.GetComponent<SpriteRenderer>();
            blade.GetComponent<Transform>().position = new Vector2(blade.GetComponent<Transform>().position.x, gameObject.transform.position.y -4);
            bladeSR.sprite = stanceSprites[0];
            currStance = 1;
        }
    }

    

   


}
