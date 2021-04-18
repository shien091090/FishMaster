using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ef_AutoRotate : MonoBehaviour
{
    public float angularSpeed; //旋轉速度

    void Update()
    {
        if (!StaticScript.pause)
        {
            transform.Rotate(Vector3.forward * angularSpeed * Time.deltaTime);
        }        
    }
}
