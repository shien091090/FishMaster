using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldCollect : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Gold"))
        {
            GameControler.instance.gold += collision.GetComponent<Ef_GoldBehavior>().CarryGold; //所持金錢加上錢幣物件所攜帶的金錢量
            AudioManager.instance.PlaySound(AudioManager.instance.soundClips.sound_gold); //播放音效
            Destroy(collision.gameObject);
        }
    }

}
