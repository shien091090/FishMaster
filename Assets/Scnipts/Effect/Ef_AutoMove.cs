using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ef_AutoMove : MonoBehaviour {

    public float speed;
    public Vector3 direction;

    private void Update()
    {
        if (!StaticScript.pause)
        {
            transform.Translate(direction * speed * Time.deltaTime);
        }
    }
}
