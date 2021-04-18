using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ef_EffectPlayer : MonoBehaviour
{
    public GameObject[] effectPrefab; //效果預置體

    public void PlayEffect()
    {
        foreach (GameObject effect in effectPrefab)
        {
            GameObject effect_ins = Instantiate(effect);
            effect_ins.transform.SetParent(gameObject.transform.parent, false);
            effect_ins.transform.position = gameObject.transform.position;
            effect_ins.transform.rotation = gameObject.transform.rotation;
        }
    }

}
