using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance; //單例模式
    public static AudioManager instance
    {
        get
        {
            return _instance;
        }
    }
    public float volume = 0.8f; //全域音量
    public float effectVolume = 0.5f; //音效音量
    public AudioSource bgmAudioSource;//背景音樂

    [System.Serializable]
    public struct BGMClips
    {
        public AudioClip bgm1;
        public AudioClip bgm2;
        public AudioClip bgm3;
        public AudioClip bgm4;
        public AudioClip bgm5;
    }
    public BGMClips bgm;

    [System.Serializable]
    public struct SoundClips
    {
        public AudioClip sound_seaWave; //海浪音效
        public AudioClip sound_gold; //金幣落入金幣收集器的音效
        public AudioClip sound_reward; //發獎金的音效
        public AudioClip sound_fire; //發射子彈音效
        public AudioClip sound_changeWeapon; //換槍的音效
        public AudioClip sound_levelUp; //等級升級音效
        public AudioClip sound_openWeb; //開網音效
        public AudioClip sound_upgrade; //組件升級音效
        public AudioClip sound_cancel; //取消音效
        public AudioClip sound_bomb; //爆破音效
        public AudioClip sound_win; //勝利音效
    }
    public SoundClips soundClips;

    void Awake()
    {
        _instance = this; //賦予單例實體物件(自身)
    }

    void Start()
    {
        bgmAudioSource.volume = PlayerPrefs.GetFloat("UNIV_volume", volume); //讀取BGM音量
        effectVolume = PlayerPrefs.GetFloat("UNIV_effectVolume", effectVolume); //讀取音效音量
        bgmAudioSource.mute = PlayerPrefs.GetInt("UNIV_isMute", 0) == 0 ? false : true; //讀取全域靜音狀態
    }

    //播放音效
    public void PlaySound(AudioClip clip)
    {
        if (PlayerPrefs.GetInt("UNIV_isMute", 0) == 0) //非靜音狀態
        {
            AudioSource.PlayClipAtPoint(clip, Vector3.zero, effectVolume);
        }        
    }
}
