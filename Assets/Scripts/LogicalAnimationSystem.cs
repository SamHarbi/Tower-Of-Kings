using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicalAnimationSystem : MonoBehaviour
{
    
    public GameObject animationPrefab; //Animation Data Prefab
    public List<GameObject> Animations; //List of all Animations in the Game
    public float Tick; //How long a single update is


    // Start is called before the first frame update
    void Start()
    {
        Animations = new List<GameObject>();
        GameObject[] toaddAnimations = GameObject.FindGameObjectsWithTag("AnimationData");

        for(int i=0; i<toaddAnimations.Length; i++)
        {
            Animations.Add(toaddAnimations[i]);
        }

        StartCoroutine(AnimationUpdate()); //Start Animation Update loop
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Create a new Animation through Code
    public void newAnimation(int newId, Sprite[] newFrames, float newTiming, GameObject parent)
    {
        GameObject newAnimation = Instantiate(animationPrefab);
        newAnimation.GetComponent<AnimationData>().init(newId, newFrames, newTiming, parent);

        Animations.Add(newAnimation);
        newAnimation.transform.parent = parent.transform;
    }

    //Change Tick Through Code
    public void setAnimationTick(float newTick)
    {
        Tick = newTick;
    }

    //Inform all AnimationData Objects that one Tick has passed
    private void updateFrames()
    {
        for(int i=0; i<Animations.Count; i++)
        {
            AnimationData[] AD = Animations[i].GetComponents<AnimationData>();
            for(int j=0; j<AD.Length; j++)
            {
                AD[j].tickDown();
            }
        }
    }

    //Currently no use
    public void setActiveAnim(int num, GameObject parent)
    {
        AnimationData[] AD = parent.GetComponents<AnimationData>();
        for(int i=0; i<AD.Length; i++)
        {
            AD[i].Running = false;
        }
        AD[num].Running = true;
    }

    //Animation Update, runs every tick
    IEnumerator AnimationUpdate()
    {
        yield return new WaitForSeconds(Tick);
        updateFrames();
        StartCoroutine(AnimationUpdate());
    }

    public GameObject[] getAnimationDataArray(GameObject GO)
    {
        GameObject[] AnimData = new GameObject[GO.transform.childCount]; //Some array spaces will be null
        int iterator = 0;

        for(int i=0; i<GO.transform.childCount; i++)
        {
            Transform tempChild = GO.transform.GetChild(i);
            if(tempChild.tag == "AnimationData")
            {
                AnimData[iterator] = tempChild.gameObject;
                iterator++;
            }
        }

        return AnimData;
    }


}
