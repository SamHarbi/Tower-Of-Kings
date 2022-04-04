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
        List<GameObject> Animations = new List<GameObject>();

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
            Animations[i].GetComponent<AnimationData>().tickDown();
        }
    }

    //Animation Update, runs every tick
    IEnumerator AnimationUpdate()
    {
        yield return new WaitForSeconds(Tick);
        Debug.Log("Courountine Ended");
        updateFrames();
        StartCoroutine(AnimationUpdate());
    }


}
