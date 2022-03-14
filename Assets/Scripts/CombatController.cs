using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    private Animator anim;
    public GameObject blade;
    private GameObject bladeLeft;
    private GameObject bladeRight;
    private SpriteRenderer bladeSR;

    public Sprite[] sprites;
    private Vector2[] bladeLocationData;

    private float isBladeAtDestination;

    private Collider2D[] nearbyEnemyL;
    
    private Vector2 OverlapStart;
    private Vector2 OverlapEnd;
    public LayerMask layer;
    


    public EnemyDetector leftEnemyDetector;
    public EnemyDetector rightEnemyDetector;
    private bool previousDirection;

    public GameObject BladeEntry;
    public GameObject BladeExit;

    public GameObject panel;

    void Awake()
    {
        bladeRight = GameObject.Find("/Canvas/Panel/Arm-Right");
        bladeLeft = GameObject.Find("/Canvas/Panel/Arm-Left");
        blade = bladeRight;
        blade.SetActive(true);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
        anim = gameObject.GetComponent<Animator>();
        bladeSR = blade.GetComponent<SpriteRenderer>();

        OverlapStart = new Vector2(transform.position.x, transform.position.y - 3);

        OverlapEnd = new Vector2(transform.position.x - 5, transform.position.y + 5);

        previousDirection = false; //left is false, right is true

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

        //Movement Logic - Walking Left and Right
        if (Input.GetAxis("Dpad-Horizontal") < 0 || Input.GetKey("a"))
        {
            Move(true);
            if(previousDirection == true)
            {
                rightEnemyDetector.resetViewControl();
                previousDirection = false;
            }
        }
        else if(Input.GetAxis("Dpad-Horizontal") > 0 || Input.GetKey("d"))
        {
            Move(false);
            if(previousDirection == false)
            {
                leftEnemyDetector.resetViewControl();
                previousDirection = true;
            }
        }

        if (Input.GetAxis("Combat-Vertical") < 0 || Input.GetKey("up"))
        {
            MoveStance(1);
        }
        else if (Input.GetAxis("Combat-Vertical") > 0 || Input.GetKey("down"))
        {
            MoveStance(0);
        }

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
        }

    }

    void Move(bool mod)
    {
        if(mod == false) //Left
        {
            blade = bladeLeft;
            blade.SetActive(true);
            bladeRight.SetActive(false);
            flipPositionLeft();

        }
        else //Right
        {
            blade = bladeRight;
            blade.SetActive(true);
            bladeLeft.SetActive(false);
            flipPositionRight();
        }

    }

    void MoveStance(int axis)
    {
        if(axis == 1)
        {

            bladeSR = blade.GetComponent<SpriteRenderer>();
            Vector2 bladePos = blade.GetComponent<Transform>().position;
            bladePos = new Vector2(bladePos.x, bladePos.y -5);
            bladeSR.sprite = sprites[1];
        }
        else
        {
            
            bladeSR = blade.GetComponent<SpriteRenderer>();
            blade.GetComponent<Transform>().position = new Vector2(blade.GetComponent<Transform>().position.x, gameObject.transform.position.y -4);
            bladeSR.sprite = sprites[0];
        }
    }

   public void flipPositionLeft()
   {
       BladeExit.transform.position = new Vector2(panel.transform.position.x + 6.3f, panel.transform.position.y + 1);
       BladeEntry.transform.position = new Vector2(panel.transform.position.x + 3, panel.transform.position.y + 1);
   }

   public void flipPositionRight()
   {
       BladeExit.transform.position = new Vector2(panel.transform.position.x - 9f, panel.transform.position.y + 1);
       BladeEntry.transform.position = new Vector2(panel.transform.position.x - 5.7f, panel.transform.position.y + 1);
   }


}
