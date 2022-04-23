using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrownTransfer : MonoBehaviour
{
    private int currAnim;
    public GameObject[] AnimationSet;
    public GameObject LAS;

    
    // Start is called before the first frame update
    void Start()
    {
        LAS = GameObject.FindWithTag("LAS");
        AnimationSet = LAS.GetComponent<LogicalAnimationSystem>().getAnimationDataArray(gameObject);        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Player")
        {
            enableAnimation(1);
        }
    }

    public void enableAnimation(int num)
    {
        //Run set animation and deactivate current running Animation
        AnimationSet[num].GetComponent<AnimationData>().Running = true;
        if(num != currAnim)
        {
            AnimationSet[currAnim].GetComponent<AnimationData>().Running = false;
            currAnim = num;
        }
    }
}
