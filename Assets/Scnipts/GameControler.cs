using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameControler : MonoBehaviour
{
    private static GameControler _instance; //單例模式
    public static GameControler instance
    {
        get
        {
            return _instance;
        }
    }
    public Text goldText; //金幣文字
    public Text lvText; //等級文字
    public Text AppellationText; //等級名稱
    public Text smallCountdownText; //小計時器
    public Text bigCountdownText; //大計時器
    public Button rewardButtom; //大計時器的按鈕
    public Button backButton; //返回按鈕
    public Button settingButton; //設定按鈕
    public Slider expSlider; //經驗條
    public GameObject lvUpTip; //升級提示
    public Text ExtraDamageText; //額外傷害
    public Text recordTimerText; //紀錄時間Text
    public GameObject pauseIcon; //暫停圖示
    public Image coolDown; //連射冷卻圖示
    public Text winNotice; //勝利告示文字 
    public GameObject winButton; //勝利&返回主選單按鈕
    public GameObject playLogButton; //通關過程紀錄按鈕
    [Space(10)]
    public int stage; //關卡數(每20級1關共4關)
    public int exp; //初始經驗值
    public float expRate; //經驗值倍率(測試模式)
    public int gold; //初始金錢
    public int resetGoldUnit; //重製時所獲得的金錢量(乘等級)
    public const int bigCountdown = 60; //大計時器秒數(倒數秒數定值)
    public const int smallCountdown = 30; //獎勵計時器秒數(倒數秒數定值)
    public float bigTimer = bigCountdown; //大計時器時間現值(變動值)
    public float smallTimer = smallCountdown; //獎勵計時器時間現值(變動值)
    public int smallTimerReward; //小計時器獎勵
    public float recordTimer; //玩家紀錄計時器
    private string[] appellation = { "海草級", "蝌蚪級", "小丑魚級", "龍蝦級", "劍魚級", "魟魚級", "海豚級", "鯊魚級", "座頭鯨", "海龍王" };//稱謂
    [Space(10)]
    public GameObject[] weaponsType; //武器種類
    public Transform bulletHolder; //子彈屏幕
    public Text oneShootCost_text; //每發價格顯示
    public int level; //等級
    public bool autoFireLock; //連射上鎖
    public float fireFreq; //射擊頻率(秒)
    public float ExtraDamage = 1.0f; //連射的額外傷害加成
    [Space(10)]
    public int weaponLevel = 0; //武器升級等級
    public int extraExpLevel = 0; //額外經驗值升級等級
    public int extraGoldLevel = 0; //額外金錢升級等級
    public int extraDamageLevel = 0; //連射傷害加成升級等級
    public int[] weaponUpgrade_cost; //武器升級花費
    public int[] extraExpUpgrade_cost; //額外經驗值升級花費
    public int[] extraGoldUpgrade_cost; //額外金錢升級花費
    public int[] extraDamageUpgrade_cost; //連射傷害加成升級花費
    [Space(10)]
    public int shootCount = 0; //子彈擊發數
    public int bulletMissCount = 0; //未擊中子彈數
    public int killCount_fish = 0; //擊殺數(魚)
    public int killCount_bomb = 0; //擊殺數(爆破)
    public int moneyCost_shoot = 0; //花費金錢(子彈)
    public int moneyCost_upgrade = 0; //花費金錢(升級)
    public int moneyGain_kill = 0; //獲得金錢(魚)
    public int moneyGain_smallTimer = 0; //獲得金錢(小計時器)
    public int moneyGain_reset = 0; //獲得金錢(降等)

    [System.Serializable]
    public struct Effects
    {
        public GameObject fireEffect;//開火特效
        public GameObject rewardEffect;//發金幣特效
        public GameObject bombEffect; //爆破特效
        public GameObject seaWave; //更換關卡的海浪特效
        public Animator goldNotEnoughAnimator; //金錢不足動畫
        public Animator extraDamageAppearAnimator; //額外傷害加成跳動動畫
        public Animator winAuroraAnimator; //勝利動畫(光)
        public Animator winLabelAnimator; //勝利動畫(文字)
        public Animator seaWaveAnimator; //海浪動畫
    }
    [Space(10)]
    public Effects effects;
    [Space(10)]
    public Image bgImage;//實際背景
    public Image effectBgImage; //更換特效背景
    public Sprite[] bgSprite;//背景圖的sprite(精靈)
    public int bgIndex;//背景圖的索引值
    [Space(10)]
    public GameObject[] bulletA_ary;
    public GameObject[] bulletB_ary;
    public GameObject[] bulletC_ary;
    public GameObject[] bulletD_ary;
    public GameObject[] bulletE_ary;

    private int[] oneShootCost_array = { 5, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000 }; //各子彈單發花費
    private int costIndex = 0; //使用中子彈的編號索引
    public int GetCostIndex
    {
        get
        {
            return costIndex;
        }
    }
    private float fireTimer; //連射計時器

    //-------------------------------------------------------------------------------------------
    void Awake()
    {
        _instance = this; //單例模式賦予
    }

    void Start()
    {
        fireTimer = fireFreq; //初始化連射計時器時間(直接設為最大值, 按下左鍵的瞬間才能射擊)

        //參數讀取
        gold = PlayerPrefs.GetInt("GAME_gold", gold);
        level = PlayerPrefs.GetInt("GAME_level", level);
        exp = PlayerPrefs.GetInt("GAME_exp", exp);
        bigTimer = PlayerPrefs.GetFloat("GAME_bigTimer", bigCountdown);
        smallTimer = PlayerPrefs.GetFloat("GAME_smallTimer", smallCountdown);
        weaponLevel = PlayerPrefs.GetInt("GAME_weaponLevel", weaponLevel);
        extraExpLevel = PlayerPrefs.GetInt("GAME_extraExpLevel", extraExpLevel);
        extraGoldLevel = PlayerPrefs.GetInt("GAME_extraGoldLevel", extraGoldLevel);
        extraDamageLevel = PlayerPrefs.GetInt("GAME_extraDamageLevel", extraDamageLevel);
        recordTimer = PlayerPrefs.GetFloat("GAME_redordTimer", 0);
        autoFireLock = PlayerPrefs.GetInt("UNIV_isAutoFire", 1) == 1 ? false : true;
        shootCount = PlayerPrefs.GetInt("GAME_shootCount", shootCount);
        bulletMissCount = PlayerPrefs.GetInt("GAME_bulletMissCount", bulletMissCount);
        killCount_fish = PlayerPrefs.GetInt("GAME_killCount_fish", killCount_fish);
        killCount_bomb = PlayerPrefs.GetInt("GAME_killCount_bomb", killCount_bomb);
        moneyCost_shoot = PlayerPrefs.GetInt("GAME_moneyCost_shoot", moneyCost_shoot);
        moneyCost_upgrade = PlayerPrefs.GetInt("GAME_moneyCost_upgrade", moneyCost_upgrade);
        moneyGain_kill = PlayerPrefs.GetInt("GAME_moneyGain_kill", moneyGain_kill);
        moneyGain_smallTimer = PlayerPrefs.GetInt("GAME_moneyGain_smallTimer", moneyGain_smallTimer);
        moneyGain_reset = PlayerPrefs.GetInt("GAME_moneyGain_reset", moneyGain_reset);

        stage = level / 20 >= 5 ? 4 : level / 20; //關卡數=每20級1關共4關
        ChangeBGM(); //更換背景音樂

        //背景選取與索引值賦予
        if (bgIndex < stage)
        {
            bgIndex = stage;
            bgImage.sprite = bgSprite[bgIndex - 1];
        }

        //切換武器
        weaponsType[costIndex / (oneShootCost_array.Length / 5)].SetActive(false);
        costIndex = PlayerPrefs.GetInt("GAME_costIndex", costIndex);
        weaponsType[costIndex / (oneShootCost_array.Length / 5)].SetActive(true);
        oneShootCost_text.text = oneShootCost_array[costIndex] + " $";

        //UI初始化
        AppellationText.text = level > 99 ? appellation[9] : appellation[level / 10]; //更新稱號(隨等級變化)
        UpdateUI(); //UI更新

        //關卡魚群產生
        this.SendMessage("FishMakerControl", stage);
    }

    void Update()
    {
        if (StaticScript.activeSwitch)
        {
            ButtonListen(); //按鍵監聽
            UpdateUI(); //UI更新(升級判斷 & 計時器倒數判斷)
        }
    }
    //-------------------------------------------------------------------------------------------

    //按鍵監聽
    public void ButtonListen()
    {
        if (!StaticScript.pause)
        {
            //滑鼠滾輪切換武器
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                coolDown.fillAmount = 0;
                OnButtonPDown();
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                coolDown.fillAmount = 0;
                OnButtonMDown();
            }

            //滑鼠左鍵開火(若滑鼠在UI範圍內)
            if (!autoFireLock)
            {
                if (Input.GetMouseButton(0) && EventSystem.current.IsPointerOverGameObject() == false)
                {
                    Fire();
                }
                else if (Input.GetMouseButton(0) && EventSystem.current.IsPointerOverGameObject() == true) //當滑鼠於UI範圍內視同停火
                {
                    fireTimer = fireFreq;
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject() == false)
                {
                    Fire();
                }
            }

            //滑鼠左鍵彈起時判定為連射中止
            if (Input.GetMouseButtonUp(0))
            {
                fireTimer = fireFreq;
            }

            //滑鼠右鍵快速領取獎勵
            //if (Input.GetMouseButtonDown(1))
            //{
            //    if (rewardButtom.gameObject.activeSelf)
            //    {
            //        this.SendMessage("BTN_RewardButton");
            //    }
            //}
        }
    }

    //更換背景(每20LV更換一次)
    public void ChangeBack()
    {
        if (bgIndex < stage)
        {
            bgIndex = stage; //更換的背景編號

            AudioManager.instance.PlaySound(AudioManager.instance.soundClips.sound_seaWave); //播放音效
            effectBgImage.sprite = bgSprite[bgIndex - 1]; //特效用背景滯留為上一張作為特效更換用
            effects.seaWave.SetActive(true);
            bgImage.sprite = bgSprite[bgIndex]; //更換背景
            effects.seaWaveAnimator.PlayInFixedTime("Ef_ChangeStage", 0, 0);
            ChangeBGM(); //更換背景音樂
            this.SendMessage("FishMakerControl", stage); //關卡魚群產生
        }
    }

    //UI更新
    public void UpdateUI()
    {
        if (!StaticScript.pause)
        {
            bigTimer -= Time.deltaTime; //大計時器倒數
            smallTimer -= Time.deltaTime; //獎勵計時器倒數
            recordTimer += Time.deltaTime; //玩家紀錄時間
            coolDown.fillAmount = coolDown.fillAmount <= Time.deltaTime * 0.5f ? 0 : coolDown.fillAmount - Time.deltaTime * 0.5f; //連射冷卻
            if (coolDown.fillAmount <= 0) //冷卻歸零時中止連射傷害加成
            {
                ExtraDamage = 1.0f;
                ExtraDamageText.text = ExtraDamage.ToString("0.00");
            }
        }

        //大計時器倒數至0時給予獎勵
        if (bigTimer <= 0)
        {
            bigCountdownText.gameObject.SetActive(false); //計時數字隱藏
            rewardButtom.gameObject.SetActive(true); //獎金按鈕顯示
        }

        //獎勵計時器倒數至0時重置並給予獎勵
        if (smallTimer <= 0)
        {
            smallTimer = smallCountdown; //計時器重置
            gold += smallTimerReward; //給予獎勵
            moneyGain_smallTimer += smallTimerReward; //獲得金錢(小計時器)增加
        }

        //升級條件判斷
        while (exp >= (stage + 1) * 100 * level + 1)
        {
            exp = exp - ((stage + 1) * 100 * level + 1);
            level++;
            if (level == 100)
            {
                StaticScript.pause = true;
                this.SendMessage("FishMakerControl", -1); //停止魚群生成
                pauseIcon.GetComponent<Image>().color = new Color32(255, 255, 255, 0); //隱藏暫停圖示
                winNotice.text = "恭 喜 通 關\n所用時間為" + ((int)recordTimer / 60).ToString("00") + "分" + ((int)recordTimer % 60).ToString("00") + "秒"; //顯示玩家成績
                this.SendMessage("PlayLog_AddStatistic", ((int)recordTimer / 60).ToString("00") + " : " + ((int)recordTimer % 60).ToString("00") + "　成功通關！\n"); //增加勝利時間至PlayLog
                AudioManager.instance.PlaySound(AudioManager.instance.soundClips.sound_win); //播放音效
                AudioManager.instance.bgmAudioSource.Stop(); //停止播放BGM
                effects.winAuroraAnimator.Play("Magnify", 0, 0);
                effects.winLabelAnimator.Play("Win Label Appear", 0, 0);
                winButton.SetActive(true); //顯示返回按鈕
                playLogButton.SetActive(true); //顯示通關紀錄按鈕
            }
            else
            {
                lvUpTip.SetActive(true);
                lvUpTip.SendMessage("ShowTip", level);
                AppellationText.text = level > 99 ? appellation[9] : appellation[level / 10]; //更新稱號(隨等級變化)
                AudioManager.instance.PlaySound(AudioManager.instance.soundClips.sound_levelUp); //播放音效
                stage = level / 20 >= 5 ? 4 : level / 20; //關卡數=每20級1關共4關
                ChangeBack(); //判斷是否更換背景
            }

        }

        goldText.text = gold + "$"; //更新金錢Text
        lvText.text = level.ToString(); //更新等級Text
        smallCountdownText.text = ((int)smallTimer / 10) + "  " + ((int)smallTimer % 10); //更新獎勵計時器
        bigCountdownText.text = ((int)bigTimer).ToString(); //更新大計時器
        recordTimerText.text = ((int)recordTimer / 60).ToString("00") + " : " + ((int)recordTimer % 60).ToString("00"); //更新玩家紀錄計時器
        pauseIcon.SetActive(StaticScript.pause); //暫停圖示
        expSlider.value = ((float)exp) / ((stage + 1) * 100 * level + 1); //當前經驗值/升到下一級所需的經驗值

    }

    //切換武器(減)
    public void OnButtonPDown()
    {
        if (!StaticScript.pause)
        {
            weaponsType[costIndex / (oneShootCost_array.Length / 5)].SetActive(false);
            costIndex++;
            costIndex = costIndex > (weaponLevel + 1) * 4 - 1 ? 0 : costIndex;
            weaponsType[costIndex / (oneShootCost_array.Length / 5)].SetActive(true);
            AudioManager.instance.PlaySound(AudioManager.instance.soundClips.sound_changeWeapon); //播放音效
            oneShootCost_text.text = oneShootCost_array[costIndex] + " $";
            ExtraDamage = 1.0f; //連射傷害加成重製
            ExtraDamageText.text = ExtraDamage.ToString("0.00");
        }
    }

    //切換武器(加)
    public void OnButtonMDown()
    {
        if (!StaticScript.pause)
        {
            weaponsType[costIndex / (oneShootCost_array.Length / 5)].SetActive(false);
            costIndex--;
            costIndex = costIndex < 0 ? (weaponLevel + 1) * 4 - 1 : costIndex;
            weaponsType[costIndex / (oneShootCost_array.Length / 5)].SetActive(true);
            AudioManager.instance.PlaySound(AudioManager.instance.soundClips.sound_changeWeapon); //播放音效
            oneShootCost_text.text = oneShootCost_array[costIndex] + " $";
            ExtraDamage = 1.0f; //連射傷害加成重製
            ExtraDamageText.text = ExtraDamage.ToString("0.00");
        }
    }

    //開火
    public void Fire()
    {
        GameObject[] selectedBulletTypes = null; //選擇中子彈種類
        int selectedBulletIndex; //選擇中子彈索引

        fireTimer += Time.deltaTime; //連射計時器計時
        if (fireTimer >= fireFreq)
        {
            Instantiate(effects.fireEffect);
            if (gold >= oneShootCost_array[costIndex])
            {
                switch (costIndex / 4)
                {
                    case 0:
                        selectedBulletTypes = bulletA_ary;
                        break;

                    case 1:
                        selectedBulletTypes = bulletB_ary;
                        break;

                    case 2:
                        selectedBulletTypes = bulletC_ary;
                        break;

                    case 3:
                        selectedBulletTypes = bulletD_ary;
                        break;

                    case 4:
                        selectedBulletTypes = bulletE_ary;
                        break;
                }
                selectedBulletIndex = level / 10 >= 10 ? 9 : level / 10; //使用的子彈種類
                gold -= oneShootCost_array[costIndex]; //扣除金幣
                moneyCost_shoot += oneShootCost_array[costIndex]; //花費金錢(子彈)增加
                GameObject bullet = Instantiate(selectedBulletTypes[selectedBulletIndex]); //產生子彈物件
                AudioManager.instance.PlaySound(AudioManager.instance.soundClips.sound_fire); //撥放音效
                shootCount++; //子彈擊發數增加
                //設定子彈父物件&位置&旋轉
                bullet.transform.SetParent(bulletHolder, false);
                bullet.transform.position = weaponsType[costIndex / (oneShootCost_array.Length / 5)].transform.Find("FirePos").transform.position;
                bullet.transform.rotation = weaponsType[costIndex / (oneShootCost_array.Length / 5)].transform.Find("FirePos").transform.rotation;
                //設定子彈傷害&速度
                bullet.GetComponent<BulletAttribute>().damage = oneShootCost_array[costIndex];
                bullet.AddComponent<Ef_AutoMove>().direction = Vector3.up;
                bullet.GetComponent<Ef_AutoMove>().speed = bullet.GetComponent<BulletAttribute>().speed;
                coolDown.fillAmount = 1.0f; //連射冷卻開始
                ExtraDamage = ExtraDamage + (0.01f * ExtraDamageRate_ChargeSpeed(extraDamageLevel)) >= (1.5f * ExtraDamageRate_Max(extraDamageLevel)) ? (1.5f * ExtraDamageRate_Max(extraDamageLevel)) : ExtraDamage + (0.01f * ExtraDamageRate_ChargeSpeed(extraDamageLevel)); //連射的額外傷害加成
                ExtraDamageText.text = ExtraDamage.ToString("0.00");
                effects.extraDamageAppearAnimator.PlayInFixedTime("Appear", 0, 0); //播放特效
            }
            else
            {
                AudioManager.instance.PlaySound(AudioManager.instance.soundClips.sound_cancel); //播放音效
                effects.goldNotEnoughAnimator.Play("Disable Shake", 0, 0);
            }

            fireTimer = 0; //重置連射計時器
        }

    }

    //額外經驗值加成公式
    public float ExtraExpRate(int level)
    {
        //Max = 10倍(線性)
        float r = 1 + level;
        return r;
    }

    //額外金錢加成公式
    public float ExtraGoldRate(int level)
    {
        //Max = 3倍(線性)
        float r = 1 + (2.0f / 9.0f) * (float)level;
        return r;
    }

    //額外傷害加成公式(上限)
    public float ExtraDamageRate_Max(int level)
    {
        //Max = 上限2倍(線性)
        float r = 1 + (1.0f / 9.0f) * (float)level;
        return r;
    }

    //額外傷害加成公式(累加速度)
    public float ExtraDamageRate_ChargeSpeed(int level)
    {
        //Max = 累加速度3倍(線性)
        float r = 1 + (2.0f / 9.0f) * (float)level;
        return r;
    }

    //更換背景音樂
    public void ChangeBGM()
    {
        switch (stage)
        {
            case 0:
                AudioManager.instance.bgmAudioSource.clip = AudioManager.instance.bgm.bgm1;
                AudioManager.instance.bgmAudioSource.Play();
                break;

            case 1:
                AudioManager.instance.bgmAudioSource.clip = AudioManager.instance.bgm.bgm3;
                AudioManager.instance.bgmAudioSource.Play();
                break;

            case 2:
                AudioManager.instance.bgmAudioSource.clip = AudioManager.instance.bgm.bgm4;
                AudioManager.instance.bgmAudioSource.Play();
                break;

            case 3:
                AudioManager.instance.bgmAudioSource.clip = AudioManager.instance.bgm.bgm2;
                AudioManager.instance.bgmAudioSource.Play();
                break;

            case 4:
                AudioManager.instance.bgmAudioSource.clip = AudioManager.instance.bgm.bgm5;
                AudioManager.instance.bgmAudioSource.Play();
                break;
        }
    }
}
