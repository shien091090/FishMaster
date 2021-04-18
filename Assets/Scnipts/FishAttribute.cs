using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishAttribute : MonoBehaviour {

    public float hp; //魚的血量
    public int gold; //死亡獎勵
    public int exp; //獲得經驗值
    public int maxAmount; //魚的最大數量
    public int maxSpeed; //魚的最大速度
    public int probabilityWeight = 1; //機率權重(預設為1)
    public GameObject deadPrefab; //魚的死亡動畫(預置體)
    public GameObject goldPrefab; //金幣動畫(預置體)

    //爆破按鈕偵測
    void Update()
    {
        if (StaticScript.bomb)
        {
            this.Injure(this.hp);
            GameControler.instance.killCount_bomb++; //爆破擊殺數增加
            GameControler.instance.killCount_fish--; //魚擊殺數平衡補正(否則會重複計算)
        }
    }

    //超過邊界時消失
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Border"))
        {
            Destroy(gameObject);
        }
    }

    //接受傷害
    public void Injure(float v)
    {
        //Debug.Log(v); //測試魚受到傷害
        hp -= v;
        if (hp <= 0)
        {
            if (GetComponent<Ef_EffectPlayer>() != null) //若魚有死亡特效則撥放
            {
                GetComponent<Ef_EffectPlayer>().PlayEffect();
            }
            GameControler.instance.killCount_fish++; //魚擊殺數增加
            GameObject deadFish = Instantiate(deadPrefab); //魚死亡的動畫預置體
            deadFish.transform.SetParent(transform.parent, false);
            deadFish.transform.position = transform.position;
            deadFish.transform.rotation = transform.rotation;
            GameObject coin = Instantiate(goldPrefab); //金幣的動畫預置體
            coin.transform.SetParent(transform.parent, false);
            coin.transform.position = transform.position;
            coin.transform.rotation = transform.rotation;
            coin.GetComponent<Ef_GoldBehavior>().CarryGold = (int)(gold * GameControler.instance.ExtraGoldRate(GameControler.instance.extraGoldLevel)); //讓金幣預置體攜帶金幣(進入收集區時才會加錢)
            GameControler.instance.moneyGain_kill += (int)(gold * GameControler.instance.ExtraGoldRate(GameControler.instance.extraGoldLevel)); //獲得金錢(擊殺)增加
            GameControler.instance.exp += (int)(exp * GameControler.instance.ExtraExpRate(GameControler.instance.extraExpLevel) * GameControler.instance.expRate); //經驗值 * 額外經驗值升級 * 測試用倍率
            Destroy(gameObject);
        }
    }

}
