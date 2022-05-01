using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Shakes Camera (or any other gameObject for that matter) at a set intensity within a set timeframe
*/

public class CameraShake : MonoBehaviour
{
    //Code Heavily inspired by https://www.youtube.com/watch?v=9A9yj8KnM8c
    
    Vector3 originalPos;

    public void startShake(float magnitude, float duration)
    {
        StartCoroutine(CamShake(magnitude, duration * Time.deltaTime));
    }

    public IEnumerator CamShake(float magnitude, float duration)
    {
        //Get original camera position
        originalPos = gameObject.transform.position;
        float elapsedTime = 0.0f;

        //While shaking
        while(elapsedTime < duration)
        {
            float x = Random.Range(-1.0f, 1.0f) * magnitude;
            float y = Random.Range(-1.0f, 1.0f) * magnitude;

            yield return new WaitForSeconds(0.05f);

            //set camera to a random offset positon calculated from magintude
            transform.position = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsedTime += Time.deltaTime;

        }

        //return back to original positions
        transform.position = originalPos;

        yield return null;
    }
}
