using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicalAnimationSystem : MonoBehaviour
{
    //Singleton Pattern- There can only one LAS Instance in the game. Manages AnimationData Objects and Informs them when to update through ticks
    //Although I could just use Unity's Time.deltaTime I would not be able to modify it without affecting other Unity Systems. Think of LAS as a time manager for Animations only

    /*
        Awake Method copied from 
        French, J. (2021). Singletons in Unity (done right). [online] Game Dev Beginner. 
        Available at: https://gamedevbeginner.com/singletons-in-unity-the-right-way/ [Accessed 1 May 2022].
    */
    
    public static LogicalAnimationSystem LAS;


    public GameObject animationPrefab; //Animation Data Prefab
    public List<GameObject> Animations; //List of all Animations in the Game
    public float Tick; //How long a single Animation update is

    private float timePassed;

    private void Awake() 
    { 
        //If there is another LAS Instance, delete this one
        if (LAS != null && LAS != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            LAS = this; 
        } 
    }

    // Start is called before the first frame update
    void Start()
    {
        Animations = new List<GameObject>();
        updateAnimationList();
        //StartCoroutine(AnimationUpdate()); //Start Animation Update loop
    }

    void FixedUpdate()
    {
        if(timePassed <= 0)
        {
            updateFrames(); //Update frames once a certain number of updates (Tick) have passed
            //StartCoroutine(AnimationUpdate());
            timePassed = Tick;
        }
        
        timePassed = timePassed - (1);
    }

    //Create an Array with all AnimationData Items in scene
    public void updateAnimationList()
    {
        GameObject[] toaddAnimations = GameObject.FindGameObjectsWithTag("AnimationData");

        for(int i=0; i<toaddAnimations.Length; i++)
        {
            Animations.Add(toaddAnimations[i]);
        }
    }

    //Add an AnimationData Object after updateAnimationList() was ran
    public void addLateAnimation(GameObject AD)
    {
        Animations.Add(AD);
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
    //Observor Pattern - LAS informs all subscribed AnimationData objects (all have the same interface tickDown()) that a Tick has passed
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

    //Currently not used- Set one animation to run for a given gameObject
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

    //Returns an array with all AnimtionData Objects that modify the method input GO GameObject
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

    //Deletes all AnimationData Objects in Animations Array
    public void deleteObjectAll()
    {
        Animations.Clear();
        Debug.Log("Animations Cleared");
    }


}
