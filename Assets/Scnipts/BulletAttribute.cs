using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletAttribute : MonoBehaviour
{
    [HideInInspector]
    public float damage; //子彈傷害
    public float speed; //子彈速度
    public GameObject webObj; //漁網物件
    public Transform webHolder; //holder區域

    void Start()
    {
        webHolder = GameObject.Find("Canvas_90/WebHolder").transform;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //超過邊界時消失
        if (collision.CompareTag("Border"))
        {
            Destroy(gameObject);
            GameControler.instance.bulletMissCount++; //未擊中子彈數增加
        }

        //觸碰魚時消失並生成網子
        if (collision.CompareTag("Fish"))
        {
            GameObject web = Instantiate(webObj);
            AudioManager.instance.PlaySound(AudioManager.instance.soundClips.sound_openWeb); //播放音效
            web.transform.SetParent(webHolder, false);
            web.transform.position = transform.position;
            web.GetComponent<WebAttribute>().damage = damage * GameControler.instance.ExtraDamage;
            collision.SendMessage("Injure", damage * GameControler.instance.ExtraDamage);
            Destroy(gameObject);
        }

    }
}
