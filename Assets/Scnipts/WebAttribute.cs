using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebAttribute : MonoBehaviour
{
    public float lifeTime; //持續時間
    public float damage; //傷害

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //觸碰魚時給予傷害
        if (collision.CompareTag("Fish"))
        {
            collision.SendMessage("Injure", damage);
        }

    }
}
