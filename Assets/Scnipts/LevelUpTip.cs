using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpTip : MonoBehaviour
{
    public float delay;
    public Text text;
    public Animator effect;

    void Awake()
    {
        if (text != null)
        {
            text = transform.Find("Text").GetComponent<Text>();
        }
    }

    public void ShowTip(int lv)
    {
        text.text = lv.ToString();
        effect.Play("Enlarge&FadeOut", 0, 0);
    }

    public void Hide ()
    {
        gameObject.SetActive(false);
    }

}
