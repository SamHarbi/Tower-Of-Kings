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
        }
        else if(Input.GetAxis("Dpad-Horizontal") > 0 || Input.GetKey("d"))
        {
            Move(false);
        }
    }

    void Move(bool mod)
    {
        if(mod == false)
        {
            blade = bladeLeft;
            blade.SetActive(true);
            bladeRight.SetActive(false);
        }
        else
        {
            blade = bladeRight;
            blade.SetActive(true);
            bladeLeft.SetActive(false);
        }

    }
}
