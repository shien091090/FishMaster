using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ef_DisappearEffect : MonoBehaviour
{
    public AnimationCurve disappearCurve; //消失曲線(Value[0, 255])
    private float timer; //計時器
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        timer = 0;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float a = disappearCurve.Evaluate(timer);
        spriteRenderer.color = new Color32(255, 255, 255, (byte)a);
        if (timer >= disappearCurve.keys[disappearCurve.keys.Length - 1].time)
        {
            Destroy(gameObject);
        }
        else
        {
            timer = timer + Time.deltaTime > disappearCurve.keys[disappearCurve.keys.Length - 1].time ? disappearCurve.keys[disappearCurve.keys.Length - 1].time : timer + Time.deltaTime;
        }        
    }

}
