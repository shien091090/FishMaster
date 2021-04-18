using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIBehavior : MonoBehaviour
{
    [System.Serializable]
    public struct SettingSystem //設定UI
    {
        public AudioSource bgmAudioSource; //背景音樂音源
        public GameObject settingPanel; //設定面板
        public Image[] settingButtons; //設定按鈕集合
        public GameObject[] settingItems; //設定項目集合
        public Toggle isAutoFireToggle; //允許連射開關
        public Slider autoFireSpeedSlider; //連射速度控制滑桿
        public GameObject demoIcon; //演示圖示
        public Toggle soundMuteToggle; //靜音開關
        public Slider volumeSlider; //BGM音量控制滑桿
        public Slider effectVolumeSlider; //音效音量控制滑桿
        public Dropdown DPISettingDropdown; //DPI設定下拉選單
        public Toggle isFullScreenToggle; //啟用&取消全螢幕
        public Toggle useVsyncToggle; //開啟垂直同步
    }
    public SettingSystem settingSystem;

    [System.Serializable]
    public struct RewardSystem //獎勵UI
    {
        public Button rewardButtom; //大計時器的按鈕
        public Animator bombEffect; //爆破動畫
        public Text bigCountdownText; //大計時器
    }
    public RewardSystem rewardSystem;

    [System.Serializable]
    public struct UpgradeSystem //升級UI
    {
        public GameObject upgradePanel; //升級面板
        public Color active; //激活顏色
        public Color disable; //未激活顏色
        public Image[] weaponUpgrade_LevelUnit; //武器升級等級單位
        public Image[] extraExp_LevelUnit; //額外經驗值升級等級單位
        public Image[] extraGold_LevelUnit; //額外金錢升級等級單位
        public Image[] extraDamage_LevelUnit; //連射傷害加成升級等級單位
        public Text weaponUpgrade_CostText; //武器升級花費文字
        public Text extraExp_CostText; //額外經驗值升級花費文字
        public Text extraGold_CostText; //額外金錢升級花費文字
        public Text extraDamage_CostText; //連射傷害加成升級花費文字
        public Text extraExp_ContentText; //額外經驗值升級詳細屬性文字
        public Text extraGold_ContentText; //額外金錢升級詳細屬性文字
        public Text extraDamage_ContentTextA; //連射傷害加成升級詳細屬性文字A
        public Text extraDamage_ContentTextB; //連射傷害加成升級詳細屬性文字B
        public Animator weaponUpgrade_failEffect; //武器升級金錢不足特效
        public Animator extraExp_failEffect; //額外經驗值升級金錢不足特效
        public Animator extraGold_failEffect; //額外金幣升級金錢不足特效
        public Animator extraDamage_failEffect; //額外傷害升級金錢不足特效
    }
    public UpgradeSystem upgradeSystem;

    [System.Serializable]
    public struct Other //其他UI
    {
        public GameObject blackBack; //黑背景
        public GameObject confirmWindow; //確認視窗
        public Text confirmText; //確認文字
        public GameObject gold; //錢幣
        public RectTransform goldSpawnPos; //錢幣生成區域
        public Text playerName; //玩家名稱
        public Button passStageButton; //通關返回按鈕
        public Animator canNotResetEffect; //無法重製特效
        public GameObject PlayLog; //通關過程紀錄
        public Text playLogText; //通關過程紀錄Text
        public RectTransform playLogRect; //通關過程紀錄Text的RectTransform
    }
    public Other otherUI;

    private float[] xRange = new float[2]; //金幣生成X軸範圍
    private float[] yRange = new float[2]; //金幣生成Y軸範圍
    private ConfirmState confirmState; //確認視窗狀態
    private int[] DPIChange = new int[2]; //DPI設定值
    private int DPIRestoreIndex = 0; //DPI還原值
    private bool windowLock = true; //DPI設定視窗鎖(防止script更動DPI設定值時觸發確認視窗)

    //確認視窗狀態列舉
    public enum ConfirmState
    {
        清除排行榜, 重製等級, 返回主選單, 調整分辨率, 啟用全螢幕, 取消全螢幕, 啟用垂直同步, 關閉垂直同步
    }

    //初始化讀取參數
    void Start()
    {
        settingSystem.isAutoFireToggle.isOn = PlayerPrefs.GetInt("UNIV_isAutoFire", 1) == 1 ? true : false; //讀取允許連射狀態
        settingSystem.autoFireSpeedSlider.value = 1.0f - PlayerPrefs.GetFloat("UNIV_autoFireSpeed", GameControler.instance.fireFreq); //讀取連射速度
        settingSystem.soundMuteToggle.isOn = PlayerPrefs.GetInt("UNIV_isMute", 0) == 0 ? true : false; //讀取全域靜音狀態
        settingSystem.volumeSlider.value = PlayerPrefs.GetFloat("UNIV_volume", AudioManager.instance.volume); //讀取BGM音量
        settingSystem.effectVolumeSlider.value = PlayerPrefs.GetFloat("UNIV_effectVolume", AudioManager.instance.effectVolume); //讀取音效音量
        windowLock = true;
        settingSystem.DPISettingDropdown.value = PlayerPrefs.GetInt("UNIV_DPI", 1); //讀取DPI設定值
        DPIRestoreIndex = PlayerPrefs.GetInt("UNIV_DPI", 1); //存入DPI還原值
        settingSystem.isFullScreenToggle.isOn = Screen.fullScreen; //讀取全屏狀態
        settingSystem.useVsyncToggle.isOn = QualitySettings.vSyncCount == 1 ? true : false; //讀取垂直同步狀態
        otherUI.playerName.text = PlayerPrefs.GetString("GAME_playerName", null); //顯示玩家名稱
        otherUI.playLogText.text = PlayerPrefs.GetString("GAME_playLog", null); //讀取通關過程紀錄

        UpgradePanelUIUpdate(); //更新升級面板

        if (settingSystem.settingPanel.activeSelf)//關閉設定面板
        {
            BTN_SettingPanel();
        }

        if (upgradeSystem.upgradePanel.activeSelf) //關閉設定面板
        {
            BTN_UpgradePanel();
        }

        //設定金幣生成範圍
        xRange[0] = otherUI.goldSpawnPos.position.x;
        xRange[1] = otherUI.goldSpawnPos.position.x + otherUI.goldSpawnPos.sizeDelta.x;
        yRange[0] = otherUI.goldSpawnPos.position.y;
        yRange[1] = otherUI.goldSpawnPos.position.y + otherUI.goldSpawnPos.sizeDelta.y;
    }

    //武器升級面板開啟&關閉
    public void BTN_UpgradePanel()
    {
        if ((StaticScript.pause && upgradeSystem.upgradePanel.activeSelf) || !StaticScript.pause)
        {
            upgradeSystem.upgradePanel.SetActive(!upgradeSystem.upgradePanel.activeSelf); //開&關面板
            StaticScript.pause = upgradeSystem.upgradePanel.activeSelf; //開&關暫停
        }
    }

    //設定面板開啟&關閉
    public void BTN_SettingPanel()
    {
        if ((StaticScript.pause && settingSystem.settingPanel.activeSelf) || !StaticScript.pause)
        {
            settingSystem.settingPanel.SetActive(!settingSystem.settingPanel.activeSelf); //開&關面板
            if (settingSystem.settingPanel.activeSelf)
            {
                for (int i = 0; i < settingSystem.settingItems.Length; i++)
                {
                    settingSystem.settingItems[i].SetActive(i == 0 ? true : false);
                    settingSystem.settingButtons[i].color = i == 0 ? new Color32(255, 255, 255, 255) : new Color32(50, 50, 50, 255);
                }
                StartCoroutine(ShowAutoFireSpeed());
            }
            StaticScript.pause = settingSystem.settingPanel.activeSelf; //開&關暫停
        }
    }

    //獎勵按鈕(爆破)
    public void BTN_RewardButton()
    {
        //if (!StaticScript.pause)
        //{
        //    GameControler.instance.gold += 500 * ((GameControler.instance.level / 10) + 1);
        //    AudioManager.instance.PlaySound(AudioManager.instance.soundClips.sound_reward); //播放音效
        //    GameObject effect = Instantiate(GameControler.instance.effects.rewardEffect); //播放特效
        //    effect.transform.SetParent(gameObject.transform.parent, false);
        //    effect.transform.position = GameControler.instance.rewardButtom.transform.position;
        //    GameControler.instance.bigTimer = GameControler.bigCountdown; //大計時器時間重置
        //    rewardSystem.rewardButtom.gameObject.SetActive(false); //獎金按鈕隱藏
        //    rewardSystem.bigCountdownText.gameObject.SetActive(true); //計時數字顯示
        //}

        if (!StaticScript.pause)
        {
            rewardSystem.rewardButtom.gameObject.SetActive(false); //獎金按鈕隱藏
            AudioManager.instance.PlaySound(AudioManager.instance.soundClips.sound_bomb); //播放音效
            GameObject effect = Instantiate(GameControler.instance.effects.bombEffect); //播放特效
            effect.transform.SetParent(gameObject.transform.parent, false);
            effect.transform.position = Vector3.zero;
            rewardSystem.bombEffect.PlayInFixedTime("Bomb", 0, 0);
            GameControler.instance.bigTimer = GameControler.bigCountdown; //大計時器時間重置
            rewardSystem.bigCountdownText.gameObject.SetActive(true); //計時數字顯示
            StartCoroutine(Bomb());
        }
    }

    //設定面板選項切換
    public void BTN_ChangeSettingItems(int index)
    {
        for (int i = 0; i < settingSystem.settingItems.Length; i++)
        {
            windowLock = true; //DPI設定視窗鎖
            settingSystem.settingItems[i].SetActive(i == index ? true : false); //顯示&隱藏設定面板
            settingSystem.settingButtons[i].color = i == index ? new Color32(255, 255, 255, 255) : new Color32(50, 50, 50, 255); //設定類按鈕高亮&淡暗
        }
        if (settingSystem.settingItems[0].activeSelf)
        {
            StartCoroutine(ShowAutoFireSpeed()); //連射速度演示
        }
        else if (settingSystem.settingItems[2].activeSelf)
        {
            windowLock = false; //取消DPI設定視窗鎖
        }
    }

    //呼叫確認視窗
    public void BTN_ShowConfirmWindow(string stateStr)
    {
        ConfirmState state = (ConfirmState)Enum.Parse(typeof(ConfirmState), stateStr);
        bool processPass = true; //進程許可開關
        confirmState = state; //儲存目前確認視窗狀態

        switch (state) //展示確認內容
        {
            case ConfirmState.清除排行榜:
                otherUI.confirmText.text = "是否清除所有玩家排行榜紀錄？";
                processPass = settingSystem.settingItems[0].activeSelf ? true : false;
                break;

            case ConfirmState.重製等級:
                if (GameControler.instance.level > 0) //等級在0以上時才可重製
                {
                    otherUI.confirmText.text = "重製會造成等級下降10級，並換成相應的金幣。\n確定重製嗎？";
                    processPass = StaticScript.pause ? false : true;
                }
                else //等級為0時無法重製
                {
                    AudioManager.instance.PlaySound(AudioManager.instance.soundClips.sound_cancel); //播放音效
                    otherUI.canNotResetEffect.Play("Disable Shake", 0, 0); //播放特效
                    processPass = false;
                }
                break;

            case ConfirmState.返回主選單:
                otherUI.confirmText.text = "確定要存檔後返回主選單嗎？";
                processPass = StaticScript.pause ? false : true;
                break;

            case ConfirmState.調整分辨率:
                otherUI.confirmText.text = "是否要更改解析度？";
                processPass = settingSystem.settingItems[2].activeSelf ? true : false;
                break;

            case ConfirmState.啟用全螢幕:
                otherUI.confirmText.text = "是否要使用全螢幕模式？";
                processPass = settingSystem.settingItems[2].activeSelf ? true : false;
                break;

            case ConfirmState.取消全螢幕:
                otherUI.confirmText.text = "是否要使用視窗模式？";
                processPass = settingSystem.settingItems[2].activeSelf ? true : false;
                break;

            case ConfirmState.啟用垂直同步:
                otherUI.confirmText.text = "是否啟用垂直同步？";
                processPass = settingSystem.settingItems[2].activeSelf ? true : false;
                break;

            case ConfirmState.關閉垂直同步:
                otherUI.confirmText.text = "是否關閉垂直同步？";
                processPass = settingSystem.settingItems[2].activeSelf ? true : false;
                break;
        }
        if (processPass)
        {
            StaticScript.pause = true; //暫停遊戲
            otherUI.blackBack.SetActive(true); //展開黑背景
            otherUI.confirmWindow.SetActive(true); //展開確認視窗
        }
    }

    //確認視窗中的是否選擇
    public void BTN_ConfirmWindowSelect(int Y_N)
    {
        if (Y_N == 0) //否(取消)
        {
            otherUI.blackBack.SetActive(false); //消去黑背景
            otherUI.confirmWindow.SetActive(false); //消去重製等級確認視窗
            if (confirmState == ConfirmState.啟用全螢幕 || confirmState == ConfirmState.取消全螢幕)
            {
                settingSystem.isFullScreenToggle.isOn = !settingSystem.isFullScreenToggle.isOn; //全螢幕狀態返回原值
            }
            else
            if (confirmState == ConfirmState.啟用垂直同步 || confirmState == ConfirmState.關閉垂直同步)
            {
                settingSystem.useVsyncToggle.isOn = !settingSystem.useVsyncToggle.isOn; //垂直同步狀態返回原值
            }
            else
            if (confirmState == ConfirmState.調整分辨率)
            {
                windowLock = true;
                settingSystem.DPISettingDropdown.value = DPIRestoreIndex; //還原DPI設定
                windowLock = false;
            }
            else 
            if (confirmState != ConfirmState.清除排行榜 || 
                confirmState != ConfirmState.調整分辨率) //除例外狀況否則繼續遊戲
            {
                StaticScript.pause = false; //繼續遊戲
            }
        }
        else //是
        {
            switch (confirmState)
            {
                case ConfirmState.清除排行榜:
                    PlayerPrefs.DeleteKey("RANK_1st_Name");
                    PlayerPrefs.DeleteKey("RANK_1st_Time");
                    PlayerPrefs.DeleteKey("RANK_1st_Log");
                    PlayerPrefs.DeleteKey("RANK_2nd_Name");
                    PlayerPrefs.DeleteKey("RANK_2nd_Time");
                    PlayerPrefs.DeleteKey("RANK_2nd_Log");
                    PlayerPrefs.DeleteKey("RANK_3rd_Name");
                    PlayerPrefs.DeleteKey("RANK_3rd_Time");
                    PlayerPrefs.DeleteKey("RANK_3rd_Log");
                    PlayerPrefs.DeleteKey("RANK_4th_Name");
                    PlayerPrefs.DeleteKey("RANK_4th_Time");
                    PlayerPrefs.DeleteKey("RANK_4th_Log");
                    PlayerPrefs.DeleteKey("RANK_5th_Name");
                    PlayerPrefs.DeleteKey("RANK_5th_Time");
                    PlayerPrefs.DeleteKey("RANK_5th_Log");
                    PlayerPrefs.DeleteKey("RANK_6th_Name");
                    PlayerPrefs.DeleteKey("RANK_6th_Time");
                    PlayerPrefs.DeleteKey("RANK_6th_Log");
                    AudioManager.instance.PlaySound(AudioManager.instance.soundClips.sound_seaWave); //播放音效
                    otherUI.blackBack.SetActive(false); //消去黑背景
                    otherUI.confirmWindow.SetActive(false); //消去確認視窗
                    break;

                case ConfirmState.重製等級:
                    this.SendMessage("FishMakerControl", -1); //魚群停止生成
                    AudioManager.instance.PlaySound(AudioManager.instance.soundClips.sound_reward); //播放音效
                    GameObject effect = Instantiate(GameControler.instance.effects.rewardEffect); //播放特效
                    effect.transform.SetParent(gameObject.transform.parent, false);
                    effect.transform.position = otherUI.goldSpawnPos.position;
                    StartCoroutine(ResetLevel()); //開始重製等級
                    otherUI.blackBack.SetActive(false); //消去黑背景
                    otherUI.confirmWindow.SetActive(false); //消去確認視窗
                    break;

                case ConfirmState.返回主選單:
                    //儲存遊戲參數
                    PlayerPrefs.SetInt("UNIV_saveGame", 1); //紀錄儲存開關
                    PlayerPrefs.SetInt("GAME_gold", GameControler.instance.gold); //金錢
                    PlayerPrefs.SetInt("GAME_level", GameControler.instance.level); //等級
                    PlayerPrefs.SetInt("GAME_exp", GameControler.instance.exp); //經驗值
                    PlayerPrefs.SetFloat("GAME_bigTimer", GameControler.instance.bigTimer); //大計時器時間
                    PlayerPrefs.SetFloat("GAME_smallTimer", GameControler.instance.smallTimer); //小計時器時間
                    PlayerPrefs.SetInt("GAME_costIndex", GameControler.instance.GetCostIndex); //目前所選子彈編號
                    PlayerPrefs.SetInt("GAME_weaponLevel", GameControler.instance.weaponLevel); //武器升級等級
                    PlayerPrefs.SetInt("GAME_extraExpLevel", GameControler.instance.extraExpLevel); //額外經驗值升級等級
                    PlayerPrefs.SetInt("GAME_extraGoldLevel", GameControler.instance.extraGoldLevel); //額外金錢升級等級
                    PlayerPrefs.SetInt("GAME_extraDamageLevel", GameControler.instance.extraDamageLevel); //連射傷害加成升級等級
                    PlayerPrefs.SetFloat("GAME_redordTimer", GameControler.instance.recordTimer); //玩家紀錄計時器
                    PlayerPrefs.SetString("GAME_playLog", otherUI.playLogText.text); //通關過程紀錄
                    PlayerPrefs.SetInt("GAME_shootCount", GameControler.instance.shootCount); //子彈擊發數
                    PlayerPrefs.SetInt("GAME_bulletMissCount", GameControler.instance.bulletMissCount); //未擊中子彈數
                    PlayerPrefs.SetInt("GAME_killCount_fish", GameControler.instance.killCount_fish); //擊殺數(魚)
                    PlayerPrefs.SetInt("GAME_killCount_bomb", GameControler.instance.killCount_bomb); //擊殺數(爆破)
                    PlayerPrefs.SetInt("GAME_moneyCost_shoot", GameControler.instance.moneyCost_shoot); //花費金錢(子彈)
                    PlayerPrefs.SetInt("GAME_moneyCost_upgrade", GameControler.instance.moneyCost_upgrade); //花費金錢(升級)
                    PlayerPrefs.SetInt("GAME_moneyGain_kill", GameControler.instance.moneyGain_kill); //獲得金錢(魚)
                    PlayerPrefs.SetInt("GAME_moneyGain_smallTimer", GameControler.instance.moneyGain_smallTimer); //獲得金錢(小計時器)
                    PlayerPrefs.SetInt("GAME_moneyGain_reset", GameControler.instance.moneyGain_reset); //獲得金錢(降等)

                    StaticScript.pause = false;
                    otherUI.blackBack.SetActive(false); //消去黑背景
                    otherUI.confirmWindow.SetActive(false); //消去確認視窗
                    SceneManager.LoadScene("Start");
                    break;

                case ConfirmState.調整分辨率:
                    Screen.SetResolution(DPIChange[0], DPIChange[1], Screen.fullScreen); //改變分辨率
                    PlayerPrefs.SetInt("UNIV_DPI", settingSystem.DPISettingDropdown.value); //儲存DPI設定值
                    DPIRestoreIndex = PlayerPrefs.GetInt("UNIV_DPI", 1); //存入DPI還原值
                    otherUI.blackBack.SetActive(false); //消去黑背景
                    otherUI.confirmWindow.SetActive(false); //消去確認視窗
                    break;

                case ConfirmState.啟用全螢幕:
                    Screen.fullScreen = true;
                    otherUI.blackBack.SetActive(false); //消去黑背景
                    otherUI.confirmWindow.SetActive(false); //消去確認視窗
                    break;

                case ConfirmState.取消全螢幕:
                    Screen.fullScreen = false;
                    otherUI.blackBack.SetActive(false); //消去黑背景
                    otherUI.confirmWindow.SetActive(false); //消去確認視窗
                    break;

                case ConfirmState.啟用垂直同步:
                    QualitySettings.vSyncCount = 1;
                    PlayerPrefs.SetInt("UNIV_useVsync", 1);
                    otherUI.blackBack.SetActive(false); //消去黑背景
                    otherUI.confirmWindow.SetActive(false); //消去確認視窗
                    break;

                case ConfirmState.關閉垂直同步:
                    QualitySettings.vSyncCount = 0;
                    PlayerPrefs.SetInt("UNIV_useVsync", 0);
                    otherUI.blackBack.SetActive(false); //消去黑背景
                    otherUI.confirmWindow.SetActive(false); //消去確認視窗
                    break;
            }
        }

    }

    //連射開啟&關閉
    public void Tog_IsAutoFire(bool OnOff)
    {
        PlayerPrefs.SetInt("UNIV_isAutoFire", OnOff ? 1 : 0); //儲存狀態
        GameControler.instance.autoFireLock = !OnOff; //連射上鎖與否
        settingSystem.autoFireSpeedSlider.interactable = OnOff; //無效化連射速度拉條與否
    }

    //設定連射速度
    public void Slid_SetAutoFireSpeed()
    {
        GameControler.instance.fireFreq = 1.0f - settingSystem.autoFireSpeedSlider.value;
        PlayerPrefs.SetFloat("UNIV_autoFireSpeed", GameControler.instance.fireFreq);
    }

    //靜音開啟&關閉
    public void Tog_IsMute(bool OnOff)
    {
        PlayerPrefs.SetInt("UNIV_isMute", OnOff ? 0 : 1); //儲存狀態
        settingSystem.bgmAudioSource.mute = !OnOff; //音訊源靜音與否
    }

    //設定音量
    public void Slid_SetVolume(int mode) //mode = 0 : 設定BGM音量 / 1 : 設定音效音量
    {
        switch (mode)
        {
            case 0:
                AudioManager.instance.volume = settingSystem.volumeSlider.value;
                settingSystem.bgmAudioSource.volume = settingSystem.volumeSlider.value;
                PlayerPrefs.SetFloat("UNIV_volume", AudioManager.instance.volume);
                break;

            case 1:
                AudioManager.instance.effectVolume = settingSystem.effectVolumeSlider.value;
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
            BTN_ShowConfirmWindow("調整分辨率"); //呼叫確認視窗
        }
    }

    //全螢幕開啟&關閉
    public void Tog_IsFullScreen()
    {
        settingSystem.isFullScreenToggle.isOn = !settingSystem.isFullScreenToggle.isOn;
        BTN_ShowConfirmWindow(Screen.fullScreen ? "取消全螢幕" : "啟用全螢幕");
    }

    //垂直同步開啟&關閉
    public void Tog_useVsync()
    {
        settingSystem.useVsyncToggle.isOn = !settingSystem.useVsyncToggle.isOn;
        BTN_ShowConfirmWindow(QualitySettings.vSyncCount == 0 ? "啟用垂直同步" : "關閉垂直同步");
    }

    //升級面板UI更新
    public void UpgradePanelUIUpdate()
    {
        //更新武器等級
        for (int i = 0; i <= GameControler.instance.weaponLevel; i++)
        {
            upgradeSystem.weaponUpgrade_LevelUnit[i].color = upgradeSystem.active;
        }

        //更新額外經驗值等級
        for (int i = 0; i <= GameControler.instance.extraExpLevel; i++)
        {
            upgradeSystem.extraExp_LevelUnit[i].color = upgradeSystem.active;
        }

        //更新額外金錢等級
        for (int i = 0; i <= GameControler.instance.extraGoldLevel; i++)
        {
            upgradeSystem.extraGold_LevelUnit[i].color = upgradeSystem.active;
        }

        //更新連射傷害加成等級
        for (int i = 0; i <= GameControler.instance.extraDamageLevel; i++)
        {
            upgradeSystem.extraDamage_LevelUnit[i].color = upgradeSystem.active;
        }

        //---------

        //更新武器升級花費文字
        upgradeSystem.weaponUpgrade_CostText.text = GameControler.instance.weaponLevel >= GameControler.instance.weaponUpgrade_cost.Length ? "Max" : GameControler.instance.weaponUpgrade_cost[GameControler.instance.weaponLevel].ToString();

        //更新額外經驗值升級花費文字
        upgradeSystem.extraExp_CostText.text = GameControler.instance.extraExpLevel >= GameControler.instance.extraExpUpgrade_cost.Length ? "Max" : GameControler.instance.extraExpUpgrade_cost[GameControler.instance.extraExpLevel].ToString();

        //更新額外金錢升級花費文字
        upgradeSystem.extraGold_CostText.text = GameControler.instance.extraGoldLevel >= GameControler.instance.extraGoldUpgrade_cost.Length ? "Max" : GameControler.instance.extraGoldUpgrade_cost[GameControler.instance.extraGoldLevel].ToString();

        //更新連射傷害加成升級花費文字
        upgradeSystem.extraDamage_CostText.text = GameControler.instance.extraDamageLevel >= GameControler.instance.extraDamageUpgrade_cost.Length ? "Max" : GameControler.instance.extraDamageUpgrade_cost[GameControler.instance.extraDamageLevel].ToString();

        //---------

        //更新額外經驗值詳細屬性文字
        upgradeSystem.extraExp_ContentText.text = "×" + GameControler.instance.ExtraExpRate(GameControler.instance.extraExpLevel) + "倍";

        //更新額外金錢詳細屬性文字
        upgradeSystem.extraGold_ContentText.text = "×" + GameControler.instance.ExtraGoldRate(GameControler.instance.extraGoldLevel).ToString("0.0") + "倍";

        //更新連射傷害加成詳細屬性文字
        upgradeSystem.extraDamage_ContentTextA.text = "上限" + (1.5f * GameControler.instance.ExtraDamageRate_Max(GameControler.instance.extraDamageLevel)).ToString("0.00"); //上限文字
        upgradeSystem.extraDamage_ContentTextB.text = "累加速度×" + GameControler.instance.ExtraDamageRate_ChargeSpeed(GameControler.instance.extraDamageLevel).ToString("0.0"); //累加速度
    }

    //武器升級
    public void Upgrade_Weapon()
    {
        if (GameControler.instance.weaponLevel >= upgradeSystem.weaponUpgrade_LevelUnit.Length - 1) //當等級在最大時
        {
            AudioManager.instance.PlaySound(AudioManager.instance.soundClips.sound_cancel); //播放音效
        }
        else if (GameControler.instance.gold >= GameControler.instance.weaponUpgrade_cost[GameControler.instance.weaponLevel]) //金錢足夠允許升級時
        {
            AudioManager.instance.PlaySound(AudioManager.instance.soundClips.sound_upgrade); //播放音效
            GameControler.instance.gold -= GameControler.instance.weaponUpgrade_cost[GameControler.instance.weaponLevel]; //扣除金錢
            GameControler.instance.moneyCost_upgrade += GameControler.instance.weaponUpgrade_cost[GameControler.instance.weaponLevel]; //花費金錢(升級)增加
            GameControler.instance.weaponLevel += 1; //升級
            otherUI.playLogText.text += ((int)GameControler.instance.recordTimer / 60).ToString("00") + " : " + ((int)GameControler.instance.recordTimer % 60).ToString("00") + " （Level：" + GameControler.instance.level.ToString("00") + "）武器升級→ " + (GameControler.instance.weaponLevel + 1) + "\n"; //插入過程紀錄
            UpgradePanelUIUpdate(); //UI更新
        }
        else //金錢不足不允許升級時
        {
            AudioManager.instance.PlaySound(AudioManager.instance.soundClips.sound_cancel); //播放音效
            upgradeSystem.weaponUpgrade_failEffect.Play("Disable Shake", 0, 0);
        }
    }

    //額外經驗值升級
    public void Upgrade_ExtraExp()
    {
        if (GameControler.instance.extraExpLevel >= upgradeSystem.extraExp_LevelUnit.Length - 1) //當等級在最大時
        {
            AudioManager.instance.PlaySound(AudioManager.instance.soundClips.sound_cancel); //播放音效
        }
        else if (GameControler.instance.gold >= GameControler.instance.extraExpUpgrade_cost[GameControler.instance.extraExpLevel]) //金錢足夠允許升級時
        {
            AudioManager.instance.PlaySound(AudioManager.instance.soundClips.sound_upgrade); //播放音效
            GameControler.instance.gold -= GameControler.instance.extraExpUpgrade_cost[GameControler.instance.extraExpLevel]; //扣除金錢
            GameControler.instance.moneyCost_upgrade += GameControler.instance.extraExpUpgrade_cost[GameControler.instance.extraExpLevel]; //花費金錢(升級)增加
            GameControler.instance.extraExpLevel += 1; //升級
            otherUI.playLogText.text += ((int)GameControler.instance.recordTimer / 60).ToString("00") + " : " + ((int)GameControler.instance.recordTimer % 60).ToString("00") + " （Level：" + GameControler.instance.level.ToString("00") + "）額外經驗→ " + (GameControler.instance.extraExpLevel + 1) + "\n"; //插入過程紀錄
            UpgradePanelUIUpdate(); //UI更新
        }
        else //金錢不足不允許升級時
        {
            AudioManager.instance.PlaySound(AudioManager.instance.soundClips.sound_cancel); //播放音效
            upgradeSystem.extraExp_failEffect.Play("Disable Shake", 0, 0);
        }
    }

    //額外金錢升級
    public void Upgrade_ExtraGold()
    {
        if (GameControler.instance.extraGoldLevel >= upgradeSystem.extraGold_LevelUnit.Length - 1) //當等級在最大時
        {
            AudioManager.instance.PlaySound(AudioManager.instance.soundClips.sound_cancel); //播放音效
        }
        else if (GameControler.instance.gold >= GameControler.instance.extraGoldUpgrade_cost[GameControler.instance.extraGoldLevel]) //金錢足夠允許升級時
        {
            AudioManager.instance.PlaySound(AudioManager.instance.soundClips.sound_upgrade); //播放音效
            GameControler.instance.gold -= GameControler.instance.extraGoldUpgrade_cost[GameControler.instance.extraGoldLevel]; //扣除金錢
            GameControler.instance.moneyCost_upgrade += GameControler.instance.extraGoldUpgrade_cost[GameControler.instance.extraGoldLevel]; //花費金錢(升級)增加
            GameControler.instance.extraGoldLevel += 1; //升級
            otherUI.playLogText.text += ((int)GameControler.instance.recordTimer / 60).ToString("00") + " : " + ((int)GameControler.instance.recordTimer % 60).ToString("00") + " （Level：" + GameControler.instance.level.ToString("00") + "）額外金錢→ " + (GameControler.instance.extraGoldLevel + 1) + "\n"; //插入過程紀錄
            UpgradePanelUIUpdate(); //UI更新
        }
        else //金錢不足不允許升級時
        {
            AudioManager.instance.PlaySound(AudioManager.instance.soundClips.sound_cancel); //播放音效
            upgradeSystem.extraGold_failEffect.Play("Disable Shake", 0, 0);
        }
    }

    //連射傷害加成升級
    public void Upgrade_ExtraDamage()
    {
        if (GameControler.instance.extraDamageLevel >= upgradeSystem.extraDamage_LevelUnit.Length - 1) //當等級在最大時
        {
            AudioManager.instance.PlaySound(AudioManager.instance.soundClips.sound_cancel); //播放音效
        }
        else if (GameControler.instance.gold >= GameControler.instance.extraDamageUpgrade_cost[GameControler.instance.extraDamageLevel]) //金錢足夠允許升級時
        {
            AudioManager.instance.PlaySound(AudioManager.instance.soundClips.sound_upgrade); //播放音效
            GameControler.instance.gold -= GameControler.instance.extraDamageUpgrade_cost[GameControler.instance.extraDamageLevel]; //扣除金錢
            GameControler.instance.moneyCost_upgrade += GameControler.instance.extraDamageUpgrade_cost[GameControler.instance.extraDamageLevel]; //花費金錢(升級)增加
            GameControler.instance.extraDamageLevel += 1; //升級
            otherUI.playLogText.text += ((int)GameControler.instance.recordTimer / 60).ToString("00") + " : " + ((int)GameControler.instance.recordTimer % 60).ToString("00") + " （Level：" + GameControler.instance.level.ToString("00") + "）連射傷害→ " + (GameControler.instance.extraDamageLevel + 1) + "\n"; //插入過程紀錄
            UpgradePanelUIUpdate(); //UI更新
        }
        else //金錢不足不允許升級時
        {
            AudioManager.instance.PlaySound(AudioManager.instance.soundClips.sound_cancel); //播放音效
            upgradeSystem.extraDamage_failEffect.Play("Disable Shake", 0, 0);
        }
    }

    //勝利(返回主選單並記錄玩家成績排行)
    public void BTN_PassStageAndRecord()
    {
        string[] rankingPres = { "RANK_1st_Time", "RANK_2nd_Time", "RANK_3rd_Time", "RANK_4th_Time", "RANK_5th_Time", "RANK_6th_Time" };
        string[] rankingNamePres = { "RANK_1st_Name", "RANK_2nd_Name", "RANK_3rd_Name", "RANK_4th_Name", "RANK_5th_Name", "RANK_6th_Name" };
        string[] playLogPres = { "RANK_1st_Log", "RANK_2nd_Log", "RANK_3rd_Log", "RANK_4th_Log", "RANK_5th_Log", "RANK_6th_Log" };
        int playerRecord = ((int)GameControler.instance.recordTimer / 60) * 100 + (int)GameControler.instance.recordTimer % 60; //此次玩家通關紀錄
        int itemSize = 0; //排行數量
        int sortIndex = 0; //成績穿插位置

        otherUI.passStageButton.interactable = false; //按鈕無效化

        for (int i = 0; i < rankingPres.Length; i++) //取得有效成績數量
        {
            itemSize = PlayerPrefs.GetInt(rankingPres[i], -1) == -1 ? itemSize : itemSize + 1;
        }

        if (itemSize == 0) //若為第一次通關
        {
            PlayerPrefs.SetString("RANK_1st_Name", PlayerPrefs.GetString("GAME_playerName", null));
            PlayerPrefs.SetInt("RANK_1st_Time", playerRecord);
            PlayerPrefs.SetString("RANK_1st_Log", otherUI.playLogText.text);
        }
        else
        {
            for (int i = 0; i < itemSize; i++) //判斷成績插入排行榜位置
            {
                if (playerRecord < PlayerPrefs.GetInt(rankingPres[i], -1)) //判斷是否插入中間排行
                {
                    sortIndex = i;
                    break;
                }
                if (i == itemSize - 1) //若為最差成績
                {
                    sortIndex = i + 1; //排行榜位置為往後新增一個名次
                    break;
                }
            }

            for (int i = itemSize - 1; i >= sortIndex; i--) //第6名將會推出排行榜外,其餘成績依插入點向後遞推
            {
                if (i < 5)
                {
                    PlayerPrefs.SetInt(rankingPres[i + 1], PlayerPrefs.GetInt(rankingPres[i], -1));
                    PlayerPrefs.SetString(rankingNamePres[i + 1], PlayerPrefs.GetString(rankingNamePres[i], "-"));
                    PlayerPrefs.SetString(playLogPres[i + 1], PlayerPrefs.GetString(playLogPres[i], null));
                }
            }

            if (sortIndex < 6) //第6名以內時插入排行榜
            {
                //將成績插入排行榜位置
                PlayerPrefs.SetString(rankingNamePres[sortIndex], PlayerPrefs.GetString("GAME_playerName", null));
                PlayerPrefs.SetInt(rankingPres[sortIndex], playerRecord);
                PlayerPrefs.SetString(playLogPres[sortIndex], otherUI.playLogText.text);
            }
        }

        PlayerPrefs.SetInt("UNIV_saveGame", 0); //初始化紀錄狀態
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
        PlayerPrefs.DeleteKey("GAME_playerName");
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
        otherUI.passStageButton.interactable = true;
        StaticScript.pause = false;
        SceneManager.LoadScene("Start");
    }

    //增加總結算文字至PlayLog
    public void PlayLog_AddStatistic(string timeText)
    {
        otherUI.playLogText.text += timeText;
        otherUI.playLogText.text += "\n---------------------------\n\n";
        otherUI.playLogText.text += "子彈擊發數：" + GameControler.instance.shootCount + "\n";
        otherUI.playLogText.text += "子彈失誤數：" + GameControler.instance.bulletMissCount + "\n";
        otherUI.playLogText.text += "擊中率：" + ( ((float)(GameControler.instance.shootCount - GameControler.instance.bulletMissCount) / ((float)GameControler.instance.shootCount))*100 ).ToString("0.0") + "%\n";
        otherUI.playLogText.text += "擊殺數：" + (GameControler.instance.killCount_fish + GameControler.instance.killCount_bomb) + " (擊殺 " + GameControler.instance.killCount_fish + " + 爆破 " + GameControler.instance.killCount_bomb + ")\n";
        otherUI.playLogText.text += "總花費金錢：" + (GameControler.instance.moneyCost_shoot + GameControler.instance.moneyCost_upgrade).ToString() + " (子彈花費 " + GameControler.instance.moneyCost_shoot + " + 升級花費 " + GameControler.instance.moneyCost_upgrade + ")\n";
        otherUI.playLogText.text += "總獲得金錢：" + (GameControler.instance.moneyGain_kill + GameControler.instance.moneyGain_reset + GameControler.instance.moneyGain_smallTimer) + " (擊殺 " + GameControler.instance.moneyGain_kill + " + 降等 " + GameControler.instance.moneyGain_reset + " + 計時器獎勵 " + GameControler.instance.moneyGain_smallTimer + ")\n";
    }

    //顯示通關過程記錄
    public void BTN_ShowPlayLog()
    {
        if (otherUI.PlayLog.activeSelf) //關閉通關過程面板
        {
            otherUI.PlayLog.SetActive(false);
            GameControler.instance.winButton.GetComponent<Button>().interactable = true;
        }
        else //開啟通關過程面板
        {
            GameControler.instance.winButton.GetComponent<Button>().interactable = false;
            otherUI.PlayLog.SetActive(true);
            otherUI.playLogRect.sizeDelta = new Vector2(otherUI.playLogText.flexibleWidth, otherUI.playLogText.preferredHeight);
        }

    }

    //複製內容至剪貼簿
    public void BTN_CopyToClipBord()
    {
        TextEditor textEditor = new TextEditor();
        textEditor.text = otherUI.playLogText.text;
        textEditor.OnFocus();
        textEditor.Copy();
    }

    //測試功能
    public void TestFuntion(int mode)
    {
        switch (mode)
        {
            case 0: //清除排行榜
                PlayerPrefs.DeleteKey("RANK_1st_Name");
                PlayerPrefs.DeleteKey("RANK_1st_Time");

                PlayerPrefs.DeleteKey("RANK_2nd_Name");
                PlayerPrefs.DeleteKey("RANK_2nd_Time");

                PlayerPrefs.DeleteKey("RANK_3rd_Name");
                PlayerPrefs.DeleteKey("RANK_3rd_Time");

                PlayerPrefs.DeleteKey("RANK_4th_Name");
                PlayerPrefs.DeleteKey("RANK_4th_Time");

                PlayerPrefs.DeleteKey("RANK_5th_Name");
                PlayerPrefs.DeleteKey("RANK_5th_Time");

                PlayerPrefs.DeleteKey("RANK_6th_Name");
                PlayerPrefs.DeleteKey("RANK_6th_Time");

                AudioManager.instance.PlaySound(AudioManager.instance.soundClips.sound_bomb);
                break;

            case 1: //Update負重測試
                StaticScript.activeSwitch = !StaticScript.activeSwitch;
                if (StaticScript.activeSwitch)
                {
                    this.SendMessage("FishMakerControl", GameControler.instance.stage);
                }
                else
                {
                    this.SendMessage("FishMakerControl", -1);
                }
                break;

            case 2: //PlayLog測試
                if (otherUI.PlayLog.activeSelf)
                {
                    otherUI.PlayLog.SetActive(false);
                }
                else
                {
                    otherUI.PlayLog.SetActive(true);
                    BTN_ShowPlayLog();
                }
                break;

            case 3: //剪貼簿複製測試
                TextEditor textEditor = new TextEditor();
                textEditor.text = "一一一一二二二二";
                textEditor.OnFocus();
                textEditor.Copy();
                
                break;
        }

    }

    //重製等級的過程
    IEnumerator ResetLevel()
    {
        for (int i = 0; i < 10; i++)
        {
            if (GameControler.instance.level <= 0) //等級0時截斷重製
            {
                break;
            }

            GameObject coin = Instantiate(otherUI.gold); //金幣的動畫預置體
            coin.transform.SetParent(otherUI.goldSpawnPos, false);
            coin.transform.localPosition = new Vector3(UnityEngine.Random.Range(xRange[0], xRange[1]), UnityEngine.Random.Range(yRange[0], yRange[1]), otherUI.goldSpawnPos.transform.position.z); //位置在goldSpawnPos範圍內隨機生成
            coin.GetComponent<Ef_GoldBehavior>().CarryGold = GameControler.instance.level * GameControler.instance.resetGoldUnit; //錢幣攜帶金錢量
            GameControler.instance.moneyGain_reset += GameControler.instance.level * GameControler.instance.resetGoldUnit; //獲得金錢(降等)增加
            GameControler.instance.exp = 0; //保持經驗值為0
            GameControler.instance.level -= 1; //等級減一
            if (GameControler.instance.level % 20 == 19 && GameControler.instance.level < 80) //換背景判斷
            {
                GameControler.instance.stage -= 1;
                GameControler.instance.bgIndex = GameControler.instance.level / 20; //更換的背景編號
                AudioManager.instance.PlaySound(AudioManager.instance.soundClips.sound_seaWave); //播放音效
                GameControler.instance.effectBgImage.sprite = GameControler.instance.bgSprite[GameControler.instance.bgIndex + 1]; //特效用背景滯留為上一張作為特效更換用
                GameControler.instance.effects.seaWave.SetActive(true);
                GameControler.instance.bgImage.sprite = GameControler.instance.bgSprite[GameControler.instance.bgIndex]; //更換背景
                GameControler.instance.effects.seaWaveAnimator.PlayInFixedTime("Ef_ChangeStage", 0, 0);
                GameControler.instance.ChangeBGM(); //更換背景音樂
            }
            yield return new WaitForSeconds(0.13f);
        }

        otherUI.playLogText.text += ((int)GameControler.instance.recordTimer / 60).ToString("00") + " : " + ((int)GameControler.instance.recordTimer % 60).ToString("00") + "　降等後等級→ " + GameControler.instance.level.ToString("00") + "\n"; //插入過程紀錄
        this.SendMessage("FishMakerControl", GameControler.instance.stage); //魚群開始生成
        StaticScript.pause = false; //繼續遊戲
    }

    //爆破
    IEnumerator Bomb()
    {
        StaticScript.bomb = true;
        yield return new WaitForEndOfFrame();
        StaticScript.bomb = false;
    }

    //連射速度演示
    IEnumerator ShowAutoFireSpeed()
    {
        while (settingSystem.settingItems[0].activeSelf && settingSystem.settingPanel.activeSelf)
        {
            settingSystem.demoIcon.SetActive(false);
            yield return new WaitForSeconds(0.03f);
            settingSystem.demoIcon.SetActive(true);
            yield return new WaitForSeconds(GameControler.instance.fireFreq - 0.03f);
        }
    }

}
