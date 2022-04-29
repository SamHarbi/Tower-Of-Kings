using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    //Code Heavily inspired by https://www.youtube.com/watch?v=9A9yj8KnM8c
    
    Vector3 originalPos;

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

            //set camera to a random offset positon calculated from magintude
            transform.position = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsedTime += Time.deltaTime;

        }

        //return back to original positions
        transform.position = originalPos;

        yield return null;
    }
}
