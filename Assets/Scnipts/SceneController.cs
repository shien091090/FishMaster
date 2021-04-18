using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class SceneController : MonoBehaviour
{
    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //變數宣告
    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public GameObject inputNamePanel; //名稱輸入面板
    public InputField inputName; //名稱
    private ConfirmState confirmState; //確認視窗狀態值
    private int[] DPIChange = new int[2]; //DPI設定值
    private int DPIRestoreIndex = 0; //DPI還原值
    private bool windowLock = true; //DPI設定視窗鎖(防止script更動DPI設定值時觸發確認視窗)
    private int logIndex = 0; //目前開啟通關紀錄的index

    [System.Serializable]
    public struct RankingObjects
    {
        public GameObject rankingPabel; //排行榜UI
        public Text firstName;
        public Text firstTime;
        public Text secondName;
        public Text secondTime;
        public Text thirdName;
        public Text thirdTime;
        public Text fourthName;
        public Text foutthTime;
        public Text fivethName;
        public Text fivethTime;
        public Text sixthName;
        public Text sixthTime;
        public GameObject playLogWindow; //通關過程紀錄面板
        public RectTransform playLogRect; //通關過程紀錄Text的RectTransform
        public Text playLogText; //通關過程紀錄Text
    }
    public RankingObjects rankingObjects;

    [System.Serializable]
    public struct ConfirmWindows
    {
        public GameObject blackBack; //黑背景
        public GameObject confirmWindow; //確認視窗
        public Text confirmText; //確認文字
    }
    public ConfirmWindows confirmWindows;

    [System.Serializable]
    public struct LoadingObjects
    {
        public GameObject loadingScreen; //讀取畫面
        public Slider slider; //讀取條
        public Text text; //讀取文字
    }
    public LoadingObjects loadingObjects;

    [System.Serializable]
    public struct SettingObjects
    {
        public GameObject settingPanel; //設定面板
        public Image[] settingButtons; //設定按鈕集合
        public GameObject[] settingItems; //設定項目集合
        public AudioSource bgmAudioSource; //音源
        public Toggle soundMuteToggle; //靜音開關
        public Slider volumeSlider; //背景音樂音量拉條
        public Slider effectVolumeSlider; //音效音量拉條
        public Dropdown DPISettingDropdown; //DPI設定下拉選單
        public Toggle isFullScreenToggle; //全螢幕開關
        public Toggle useVsyncToggle; //垂直同步開關
    }
    public SettingObjects settingObjects;

    public enum ConfirmState //確認視窗列舉
    {
        開新遊戲, 繼續遊戲, 離開遊戲, 調整分辨率, 啟用全螢幕, 取消全螢幕, 啟用垂直同步, 關閉垂直同步
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //內建方法
    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    //初始化
    void Start()
    {
        //排行榜資訊
        rankingObjects.firstName.text = PlayerPrefs.GetString("RANK_1st_Name", "-");
        rankingObjects.firstTime.text = PlayerPrefs.GetInt("RANK_1st_Time", -1) == -1 ? "00:00" : PlayerPrefs.GetInt("RANK_1st_Time", -1).ToString("00:00");

        rankingObjects.secondName.text = PlayerPrefs.GetString("RANK_2nd_Name", "-");
        rankingObjects.secondTime.text = PlayerPrefs.GetInt("RANK_2nd_Time", -1) == -1 ? "00:00" : PlayerPrefs.GetInt("RANK_2nd_Time", -1).ToString("00:00");

        rankingObjects.thirdName.text = PlayerPrefs.GetString("RANK_3rd_Name", "-");
        rankingObjects.thirdTime.text = PlayerPrefs.GetInt("RANK_3rd_Time", -1) == -1 ? "00:00" : PlayerPrefs.GetInt("RANK_3rd_Time", -1).ToString("00:00");

        rankingObjects.fourthName.text = PlayerPrefs.GetString("RANK_4th_Name", "-");
        rankingObjects.foutthTime.text = PlayerPrefs.GetInt("RANK_4th_Time", -1) == -1 ? "00:00" : PlayerPrefs.GetInt("RANK_4th_Time", -1).ToString("00:00");

        rankingObjects.fivethName.text = PlayerPrefs.GetString("RANK_5th_Name", "-");
        rankingObjects.fivethTime.text = PlayerPrefs.GetInt("RANK_5th_Time", -1) == -1 ? "00:00" : PlayerPrefs.GetInt("RANK_5th_Time", -1).ToString("00:00");

        rankingObjects.sixthName.text = PlayerPrefs.GetString("RANK_6th_Name", "-");
        rankingObjects.sixthTime.text = PlayerPrefs.GetInt("RANK_6th_Time", -1) == -1 ? "00:00" : PlayerPrefs.GetInt("RANK_6th_Time", -1).ToString("00:00");

        //設定項讀取
        settingObjects.soundMuteToggle.isOn = PlayerPrefs.GetInt("UNIV_isMute", 0) == 0 ? true : false; //讀取全域靜音狀態
        settingObjects.volumeSlider.value = PlayerPrefs.GetFloat("UNIV_volume", AudioManager.instance.volume); //讀取BGM音量
        settingObjects.effectVolumeSlider.value = PlayerPrefs.GetFloat("UNIV_effectVolume", AudioManager.instance.effectVolume); //讀取音效音量
        windowLock = true;
        settingObjects.DPISettingDropdown.value = PlayerPrefs.GetInt("UNIV_DPI", 1); //讀取DPI設定值
        DPIRestoreIndex = PlayerPrefs.GetInt("UNIV_DPI", 1); //存入DPI還原值
        settingObjects.isFullScreenToggle.isOn = Screen.fullScreen; //讀取全屏狀態
        settingObjects.useVsyncToggle.isOn = QualitySettings.vSyncCount == 1 ? true : false; //讀取垂直同步狀態
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //自定義方法
    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    //呼叫確認視窗
    public void CW_ShowWindow(string stateStr)
    {
        ConfirmState state = (ConfirmState)Enum.Parse(typeof(ConfirmState), stateStr);
        confirmState = state; //儲存目前確認視窗狀態
        bool processPass = false; //進程許可開關

        if (!StaticScript.pause)
        {
            switch (state) //展示確認內容
            {
                case ConfirmState.開新遊戲:
                    if (PlayerPrefs.GetInt("UNIV_saveGame", 0) == 0) //若沒有紀錄的遊戲則直接開始
                    {
                        StaticScript.pause = true; //暫停遊戲
                        confirmWindows.blackBack.SetActive(true); //展開黑背景
                        inputName.text = "";
                        inputNamePanel.SetActive(true); //開啟輸入名稱面板
                    }
                    else //若有紀錄的遊戲,詢問是否開新局
                    {
                        confirmWindows.confirmText.text = "開始遊戲會導致紀錄喪失\n確定要開始新遊戲 ? ";
                        processPass = true;
                    }
                    break;

                case ConfirmState.繼續遊戲:
                    if (PlayerPrefs.GetInt("UNIV_saveGame", 0) == 0) //若沒有紀錄的遊戲,顯示是否要開新遊戲
                    {
                        confirmWindows.confirmText.text = "無可用記錄\n是否要開新遊戲 ? ";
                        processPass = true;
                    }
                    else
                    {
                        loadingObjects.loadingScreen.SetActive(true); //開啟讀取畫面
                        StartCoroutine(Loading(false));
                    }
                    break;

                case ConfirmState.離開遊戲:
                    confirmWindows.confirmText.text = "確定要離開遊戲嗎 ?";
                    processPass = true;
                    break;
            }
        }
        else if (StaticScript.pause && (int)state >= 3)
        {
            switch (state)
            {
                case ConfirmState.調整分辨率:
                    confirmWindows.confirmText.text = "是否要更改解析度？";
                    processPass = settingObjects.settingItems[1].activeSelf ? true : false;
                    break;

                case ConfirmState.啟用全螢幕:
                    confirmWindows.confirmText.text = "是否要使用全螢幕模式？";
                    processPass = settingObjects.settingItems[1].activeSelf ? true : false;
                    break;

                case ConfirmState.取消全螢幕:
                    confirmWindows.confirmText.text = "是否要使用視窗模式？";
                    processPass = settingObjects.settingItems[1].activeSelf ? true : false;
                    break;

                case ConfirmState.啟用垂直同步:
                    confirmWindows.confirmText.text = "是否啟用垂直同步？";
                    processPass = settingObjects.settingItems[1].activeSelf ? true : false;
                    break;

                case ConfirmState.關閉垂直同步:
                    confirmWindows.confirmText.text = "是否關閉垂直同步？";
                    processPass = settingObjects.settingItems[1].activeSelf ? true : false;
                    break;
            }
        }

        if (processPass)
        {
            StaticScript.pause = true; //暫停遊戲
            confirmWindows.blackBack.SetActive(true); //展開黑背景
            confirmWindows.confirmWindow.SetActive(true); //展開確認視窗
        }
    }

    //確認視窗中的是否選擇
    public void CW_ShowSelection(int Y_N)
    {
        if (Y_N == 0) //否(取消)
        {
            confirmWindows.blackBack.SetActive(false); //消去黑背景
            confirmWindows.confirmWindow.SetActive(false); //消去確認視窗
            if (confirmState == ConfirmState.啟用全螢幕 || confirmState == ConfirmState.取消全螢幕)
            {
                settingObjects.isFullScreenToggle.isOn = !settingObjects.isFullScreenToggle.isOn; //全螢幕狀態返回原值
            }
            else
            if (confirmState == ConfirmState.啟用垂直同步 || confirmState == ConfirmState.關閉垂直同步)
            {
                settingObjects.useVsyncToggle.isOn = !settingObjects.useVsyncToggle.isOn; //垂直同步狀態返回原值
            }
            else
            if (confirmState == ConfirmState.調整分辨率)
            {
                windowLock = true;
                settingObjects.DPISettingDropdown.value = DPIRestoreIndex; //還原DPI設定
                windowLock = false;
            }
            else //除例外狀況否則繼續遊戲
            {
                StaticScript.pause = false; //繼續遊戲
            }
        }
        else //是
        {
            switch (confirmState)
            {
                case ConfirmState.開新遊戲:
                    confirmWindows.confirmWindow.SetActive(false); //消去確認視窗
                    inputName.text = "";
                    inputNamePanel.SetActive(true); //開啟輸入名稱面板
                    break;

                case ConfirmState.繼續遊戲:
                    confirmWindows.confirmWindow.SetActive(false); //消去確認視窗
                    inputName.text = ""; //清空名稱輸入框
                    inputNamePanel.SetActive(true); //開啟輸入名稱面板
                    break;

                case ConfirmState.離開遊戲:
                    StaticScript.pause = false;
                    Application.Quit(); //離開遊戲
                    break;

                case ConfirmState.調整分辨率:
                    Screen.SetResolution(DPIChange[0], DPIChange[1], Screen.fullScreen); //改變分辨率
                    PlayerPrefs.SetInt("UNIV_DPI", settingObjects.DPISettingDropdown.value); //儲存DPI設定值
                    DPIRestoreIndex = PlayerPrefs.GetInt("UNIV_DPI", 1); //存入DPI還原值
                    confirmWindows.blackBack.SetActive(false); //消去黑背景
                    confirmWindows.confirmWindow.SetActive(false); //消去確認視窗
                    break;

                case ConfirmState.啟用全螢幕:
                    Screen.fullScreen = true;
                    confirmWindows.blackBack.SetActive(false); //消去黑背景
                    confirmWindows.confirmWindow.SetActive(false); //消去確認視窗
                    break;

                case ConfirmState.取消全螢幕:
                    Screen.fullScreen = false;
                    confirmWindows.blackBack.SetActive(false); //消去黑背景
                    confirmWindows.confirmWindow.SetActive(false); //消去確認視窗
                    break;

                case ConfirmState.啟用垂直同步:
                    QualitySettings.vSyncCount = 1;
                    PlayerPrefs.SetInt("UNIV_useVsync", 1);
                    confirmWindows.blackBack.SetActive(false); //消去黑背景
                    confirmWindows.confirmWindow.SetActive(false); //消去確認視窗
                    break;

                case ConfirmState.關閉垂直同步:
                    QualitySettings.vSyncCount = 0;
                    PlayerPrefs.SetInt("UNIV_useVsync", 0);
                    confirmWindows.blackBack.SetActive(false); //消去黑背景
                    confirmWindows.confirmWindow.SetActive(false); //消去確認視窗
                    break;
            }
        }

    }

    //設定面板開啟&關閉
    public void BTN_SettingPanel()
    {
        if ((StaticScript.pause && settingObjects.settingPanel.activeSelf) || !StaticScript.pause)
        {
            settingObjects.settingPanel.SetActive(!settingObjects.settingPanel.activeSelf); //開&關面板
            if (settingObjects.settingPanel.activeSelf)
            {
                for (int i = 0; i < settingObjects.settingItems.Length; i++)
                {
                    settingObjects.settingItems[i].SetActive(i == 0 ? true : false);
                    settingObjects.settingButtons[i].color = i == 0 ? new Color32(255, 255, 255, 255) : new Color32(50, 50, 50, 255);
                }
            }
            StaticScript.pause = settingObjects.settingPanel.activeSelf; //開&關暫停
        }
    }

    //設定面板選項切換
    public void BTN_ChangeSettingItems(int index)
    {
        for (int i = 0; i < settingObjects.settingItems.Length; i++)
        {
            windowLock = true; //DPI設定視窗鎖
            settingObjects.settingItems[i].SetActive(i == index ? true : false); //顯示&隱藏設定面板
            settingObjects.settingButtons[i].color = i == index ? new Color32(255, 255, 255, 255) : new Color32(50, 50, 50, 255); //設定類按鈕高亮&淡暗
        }
        if (settingObjects.settingItems[1].activeSelf)
        {
            windowLock = false; //取消DPI設定視窗鎖
        }
    }

    //名稱輸入框
    public void InputName(int status) //1 : 否 / 2 : 是
    {
        if (status == 1)
        {
            confirmWindows.blackBack.SetActive(false); //消去黑背景
            confirmWindows.confirmWindow.SetActive(false); //消去確認視窗
            inputNamePanel.SetActive(false); //消去名稱輸入視窗
            StaticScript.pause = false;
        }
        else if (status == 2)
        {
            if (inputName.text == "")
            {
                AudioManager.instance.PlaySound(AudioManager.instance.soundClips.sound_cancel);
            }
            else
            {
                PlayerPrefs.SetString("GAME_playerName", inputName.text); //儲存名稱
                inputNamePanel.SetActive(false); //關閉輸入名稱面板
                confirmWindows.blackBack.SetActive(false); //消去黑背景
                loadingObjects.loadingScreen.SetActive(true); //開啟讀取畫面
                StaticScript.pause = false;
                StartCoroutine(Loading(true)); //進入讀取程序
            }
        }
    }

    //靜音開啟&關閉
    public void Tog_IsMute(bool OnOff)
    {
        PlayerPrefs.SetInt("UNIV_isMute", OnOff ? 0 : 1); //儲存狀態
        settingObjects.bgmAudioSource.mute = !OnOff; //音訊源靜音與否
    }

    //設定音量
    public void Slid_SetVolume(int mode) //mode = 0 : 設定BGM音量 / 1 : 設定音效音量
    {
        switch (mode)
        {
            case 0:
                AudioManager.instance.volume = settingObjects.volumeSlider.value;
                settingObjects.bgmAudioSource.volume = settingObjects.volumeSlider.value;
                PlayerPrefs.SetFloat("UNIV_volume", AudioManager.instance.volume);
                break;

            case 1:
                AudioManager.instance.effectVolume = settingObjects.effectVolumeSlider.value;
                PlayerPrefs.SetFloat("UNIV_effectVolume", AudioManager.instance.effectVolume);
                break;
        }
    }

    //設定解析度
    public void Drop_ChangeDPI(int v)
    {
        if (!windowLock)
        {
            switch (v) //設置即將更換的分辨率
            {
                case 0:
                    DPIChange[0] = 1024;
                    DPIChange[1] = 768;
                    break;

                case 1:
                    DPIChange[0] = 1280;
                    DPIChange[1] = 720;
                    break;

                case 2:
                    DPIChange[0] = 1280;
                    DPIChange[1] = 1024;
                    break;

                case 3:
                    DPIChange[0] = 1440;
                    DPIChange[1] = 900;
                    break;

                case 4:
                    DPIChange[0] = 1600;
                    DPIChange[1] = 900;
                    break;

                case 5:
                    DPIChange[0] = 1920;
                    DPIChange[1] = 1080;
                    break;
            }
            CW_ShowWindow("調整分辨率"); //呼叫確認視窗
        }
    }

    //全螢幕開啟&關閉
    public void Tog_IsFullScreen()
    {
        settingObjects.isFullScreenToggle.isOn = !settingObjects.isFullScreenToggle.isOn;
        CW_ShowWindow(Screen.fullScreen ? "取消全螢幕" : "啟用全螢幕");
    }

    //垂直同步開啟&關閉
    public void Tog_useVsync()
    {
        settingObjects.useVsyncToggle.isOn = !settingObjects.useVsyncToggle.isOn;
        CW_ShowWindow(QualitySettings.vSyncCount == 0 ? "啟用垂直同步" : "關閉垂直同步");
    }

    //排行榜呼叫按鈕
    public void BTN_RankingPanel()
    {
        if ((StaticScript.pause && rankingObjects.rankingPabel.activeSelf) || !StaticScript.pause)
        {
            rankingObjects.rankingPabel.SetActive(!rankingObjects.rankingPabel.activeSelf); //開&關排行榜
            rankingObjects.playLogWindow.SetActive(!rankingObjects.rankingPabel.activeSelf); //
            StaticScript.pause = rankingObjects.rankingPabel.activeSelf; //開&關暫停
        }
    }

    //顯示通關過程紀錄
    public void BTN_ShowPlayLog(int index) //-1=close按鈕
    {
        if ((rankingObjects.playLogWindow.activeSelf && logIndex == index) || index == -1) //關閉通關過程面板
        {
            rankingObjects.playLogWindow.SetActive(false);
        }
        else //開啟通關過程面板
        {
            logIndex = index;
            string[] playLogPres = { "RANK_1st_Log", "RANK_2nd_Log", "RANK_3rd_Log", "RANK_4th_Log", "RANK_5th_Log", "RANK_6th_Log" };

            rankingObjects.playLogWindow.SetActive(true);
            rankingObjects.playLogText.text = PlayerPrefs.GetString(playLogPres[index], null); ;
            rankingObjects.playLogRect.sizeDelta = new Vector2(rankingObjects.playLogText.flexibleWidth, rankingObjects.playLogText.preferredHeight);
        }
    }

    //複製內容至剪貼簿
    public void BTN_CopyToClipBord()
    {
        TextEditor textEditor = new TextEditor();
        textEditor.text = rankingObjects.playLogText.text;
        textEditor.OnFocus();
        textEditor.Copy();
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //協同程序
    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    //讀取
    IEnumerator Loading(bool reset)
    {
        if (reset)
        {
            //重製遊戲
            PlayerPrefs.SetInt("UNIV_saveGame", 0); //遊戲為非儲存狀態
            PlayerPrefs.DeleteKey("GAME_gold");
            PlayerPrefs.DeleteKey("GAME_level");
            PlayerPrefs.DeleteKey("GAME_exp");
            PlayerPrefs.DeleteKey("GAME_bigTimer");
            PlayerPrefs.DeleteKey("GAME_smallTimer");
            PlayerPrefs.DeleteKey("GAME_costIndex");
            PlayerPrefs.DeleteKey("GAME_weaponLevel");
            PlayerPrefs.DeleteKey("GAME_extraExpLevel");
            PlayerPrefs.DeleteKey("GAME_extraGoldLevel");
            PlayerPrefs.DeleteKey("GAME_extraDamageLevel");
            PlayerPrefs.DeleteKey("GAME_redordTimer");
            PlayerPrefs.DeleteKey("GAME_playLog");
            PlayerPrefs.DeleteKey("GAME_shootCount");
            PlayerPrefs.DeleteKey("GAME_bulletMissCount");
            PlayerPrefs.DeleteKey("GAME_killCount_fish");
            PlayerPrefs.DeleteKey("GAME_killCount_bomb");
            PlayerPrefs.DeleteKey("GAME_moneyCost_shoot");
            PlayerPrefs.DeleteKey("GAME_moneyCost_upgrade");
            PlayerPrefs.DeleteKey("GAME_moneyGain_kill");
            PlayerPrefs.DeleteKey("GAME_moneyGain_smallTimer");
            PlayerPrefs.DeleteKey("GAME_moneyGain_reset");
        }
        AsyncOperation async = SceneManager.LoadSceneAsync("Main");
        while (!async.isDone)
        {
            loadingObjects.text.text = (async.progress * 100).ToString("0") + "%";
            loadingObjects.slider.value = async.progress;
            yield return null;
        }
    }


}
