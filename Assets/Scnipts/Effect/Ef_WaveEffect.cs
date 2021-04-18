using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ef_WaveEffect : MonoBehaviour
{
    public GameObject waveObj;
    public Material material; //材質球
    public Texture[] textures; //貼圖(陣列)
    private int index; //編號

    void Start()
    {
        InvokeRepeating("ChangeTexture", 0, 0.05f);
    }

    void ChangeTexture()
    {
        material.mainTexture = textures[index];
        index = (index + 1) % textures.Length;
        waveObj.SetActive(false);
        waveObj.SetActive(true);
    }
}
