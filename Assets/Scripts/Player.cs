using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private int speed;
    private int moveBuffer;
    public bool attacking;

    private Animator anim;
    private SpriteRenderer SR;
    private Rigidbody2D RB;
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

        speed = 6;
        moveBuffer = 1;
        attacking = false;

        anim = gameObject.GetComponent<Animator>();
        SR = gameObject.GetComponent<SpriteRenderer>();
        RB = gameObject.GetComponent<Rigidbody2D>();
        bladeSR = blade.GetComponent<SpriteRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Dpad-Horizontal") < 0 || Input.GetKey("a"))
        {
            Move(-1.0f);
            SR.flipX = false;
        }
        else if(Input.GetAxis("Dpad-Horizontal") > 0 || Input.GetKey("d"))
        {
            Move(1.0f);
            SR.flipX = true;
        }
        else
        {
            anim.SetBool("Running", false);
        }

        if (Input.GetAxis("Combat-Vertical") < 0 || Input.GetKey("up"))
        {
            MoveStance(1);
        }
        else if (Input.GetAxis("Combat-Vertical") < 0 || Input.GetKey("down"))
        {
            MoveStance(0);
        }

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        if (Input.GetButton("Fire1"))
        {
           anim.SetTrigger("Attack");
        }



        if(RB.velocity.y < 0.1)
        {
            RB.gravityScale = 8;
        }
        else
        {
            RB.gravityScale = 4.5f;
        }

    }

    void Jump()
    {
        if(RB.velocity.y == 0)
        {
            GetComponent<Rigidbody2D>().AddForce(transform.up * 1000);
        }
    }

    void Move(float mod)
    {
        Vector3 change;
        anim.SetBool("Running", true);

        change = new Vector3(mod * speed * Time.deltaTime, 0, 0);
        transform.position += change;



        if(mod > 0)
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

    void MoveStance(int axis)
    {

    }


}
