using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMaker : MonoBehaviour
{
    public float[] generation_frequency; //生成頻率
    public float generation_delay; //(防止魚群重疊)生成間隔頻率
    public Transform fishHolder;
    public Transform[] generationPos; //生成位置的陣列
    public GameObject[] fishObject; //魚物件的陣列

    //各關卡所出現的魚群
    [System.Serializable]
    public struct StageFishes
    {
        public GameObject[] stage1_fishes;
        public GameObject[] stage2_fishes;
        public GameObject[] stage3_fishes;
        public GameObject[] stage4_fishes;
        public GameObject[] specialStage_fishes;
    }
    public StageFishes stageFishes;

    ////生成魚(全部隨機
    //void MakeFish()
    //{
    //    int genPosIndex = Random.Range(0, generationPos.Length); //魚的產生點編號(隨機)
    //    int fishIndex = Random.Range(0, fishObject.Length); //魚的種類編號(隨機)

    //    int maxAmount = fishObject[fishIndex].GetComponent<FishAttribute>().maxAmount; //取得各個魚物件的最大數量(maxAmount)變數
    //    int maxSpeed = fishObject[fishIndex].GetComponent<FishAttribute>().maxSpeed; //取得各個魚物件的最大速度(maxSpeed)變數
    //    int amount = Random.Range((maxAmount / 2) > 0 ? maxAmount : 1, maxAmount); //實際生成數量為 -> 最大數量/2(最小值1) ~ 最大數量
    //    int moveSpeed = Random.Range((maxSpeed / 1.5) > 0 ? maxSpeed : 1, maxSpeed); //實際速度為 -> 最大速度/1.5(最小值1) ~ 最大速度

    //    int moveType = Random.Range(0, 2); //0 = 直行 / 1 = 轉彎
    //    float angleValue = Random.Range(-22, 22); //偏移角度範圍為-22~22度之間
    //    float angularSpeed = 0; //轉彎角速度

    //    if (moveType == 1) //生成轉彎魚
    //    {
    //        if (Random.Range(0, 2) == 0)//是否取負的角速度
    //        {
    //            angularSpeed = Random.Range(-40, -10);//轉彎的角速度(負)
    //        }
    //        else
    //        {
    //            angularSpeed = Random.Range(40, 10);//轉彎的角速度(正)
    //        }

    //    }
    //    StartCoroutine(GenerateFish(genPosIndex, fishIndex, amount, moveSpeed, moveType, angleValue, angularSpeed));
    //}

    ////生成魚(指定種類與數量
    //void MakeFish_Appoint(int indexC, int amountC)
    //{
    //    int genPosIndex = Random.Range(0, generationPos.Length); //魚的產生點編號(隨機)
    //    int fishIndex = indexC; //魚的種類編號(指定)

    //    int maxAmount = fishObject[fishIndex].GetComponent<FishAttribute>().maxAmount; //取得各個魚物件的最大數量(maxAmount)變數
    //    int maxSpeed = fishObject[fishIndex].GetComponent<FishAttribute>().maxSpeed; //取得各個魚物件的最大速度(maxSpeed)變數
    //    int amount = amountC; //指定數量
    //    int moveSpeed = Random.Range((maxSpeed / 1.5) > 0 ? maxSpeed : 1, maxSpeed); //實際速度為 -> 最大速度/1.5(最小值1) ~ 最大速度

    //    int moveType = Random.Range(0, 2); //0 = 直行 / 1 = 轉彎
    //    float angleValue = Random.Range(-22, 22); //偏移角度範圍為-22~22度之間
    //    float angularSpeed = 0; //轉彎角速度

    //    if (moveType == 1) //生成轉彎魚
    //    {
    //        if (Random.Range(0, 2) == 0)//是否取負的角速度
    //        {
    //            angularSpeed = Random.Range(-40, -10);//轉彎的角速度(負)
    //        }
    //        else
    //        {
    //            angularSpeed = Random.Range(40, 10);//轉彎的角速度(正)
    //        }

    //    }
    //    StartCoroutine(GenerateFish(fishObject, genPosIndex, fishIndex, amount, moveSpeed, moveType, angleValue, angularSpeed));
    //}

    //生成魚(從關卡內隨機(各類魚的出生機率以各自的probabilityWeight做比例分配)
    void MakeFish_StageRandom()
    {
        if (!StaticScript.pause)
        {
            GameObject[] fishes; //魚陣列的參考
            int[] weightLayer; //權重階梯
            int totalWeight = 0; //總權重
            int dice; //權重職骰
            int allotIndex = 0; //分配區間轉換成索引

            //將指定關卡的魚群陣列做為參考指定給fishes陣列
            switch (GameControler.instance.stage)
            {
                case 0:
                    fishes = stageFishes.stage1_fishes;
                    break;

                case 1:
                    fishes = stageFishes.stage2_fishes;
                    break;

                case 2:
                    fishes = stageFishes.stage3_fishes;
                    break;

                case 3:
                    fishes = stageFishes.stage4_fishes;
                    break;

                case 4:
                    fishes = stageFishes.specialStage_fishes;
                    break;

                default:
                    fishes = stageFishes.stage1_fishes;
                    //Debug.Log("[BUG] : stage超過範圍");
                    break;
            }

            weightLayer = new int[fishes.Length]; //權重階梯數=魚群陣列長度

            //分配權重階梯(第一項為總權重,往後遞減) & 計算總權重
            for (int i = weightLayer.Length - 1; i >= 0; i--)
            {
                totalWeight += fishes[i].GetComponent<FishAttribute>().probabilityWeight;
                for (int j = i; j >= 0; j--)
                {
                    weightLayer[i] += fishes[j].GetComponent<FishAttribute>().probabilityWeight;
                }
            }

            dice = Random.Range(1, totalWeight + 1); //權重職骰

            //將職骰數和權重階梯的每層做判定,抓出區間層數
            for (int i = 0; i < weightLayer.Length; i++)
            {
                if (dice <= weightLayer[i])
                {
                    allotIndex = i;
                    break;
                }
            }

            int fishIndex = allotIndex; //魚的種類編號(隨機)
            int genPosIndex = Random.Range(0, generationPos.Length); //魚的產生點編號(隨機)

            int maxAmount = fishes[fishIndex].GetComponent<FishAttribute>().maxAmount; //取得各個魚物件的最大數量(maxAmount)變數
            float maxSpeed = fishes[fishIndex].GetComponent<FishAttribute>().maxSpeed; //取得各個魚物件的最大速度(maxSpeed)變數
            int amount = Random.Range((maxAmount / 2) > 0 ? maxAmount / 2 : 1, maxAmount); //實際生成數量為 -> 最大數量/2(最小值1) ~ 最大數量
            float moveSpeed = Random.Range((maxSpeed / 1.5f) > 0 ? maxSpeed / 1.5f : 1, maxSpeed); //實際速度為 -> 最大速度/1.5(最小值1) ~ 最大速度

            int moveType = Random.Range(0, 2); //0 = 直行 / 1 = 轉彎
            float angleValue = Random.Range(-22, 22); //偏移角度範圍為-22~22度之間
            float angularSpeed = 0; //轉彎角速度

            if (moveType == 1) //生成轉彎魚
            {
                if (Random.Range(0, 2) == 0)//是否取負的角速度
                {
                    angularSpeed = Random.Range(-25, -10);//轉彎的角速度(負)
                }
                else
                {
                    angularSpeed = Random.Range(10, 25);//轉彎的角速度(正)
                }

            }
            StartCoroutine(GenerateFish(fishes, genPosIndex, fishIndex, amount, moveSpeed, moveType, angleValue, angularSpeed));
        }
    }

    //控制魚群生成頻率
    public void FishMakerControl(int status)
    {
        if (status == -1)
        {
            CancelInvoke();
        }
        else
        {
            CancelInvoke();
            InvokeRepeating("MakeFish_StageRandom", 0, generation_frequency[status]);
        }
    }

    //魚物件生成的實作方法
    IEnumerator GenerateFish(GameObject[] fishes, int genPosIndex, int fishIndex, int amount, float moveSpeed, int moveType, float angleValue, float angularSpeed)
    {
        for (int i = 0; i < amount; i++)
        {
            if (StaticScript.pause) //暫停時使魚群不會重疊
            {
                i--;
                yield return new WaitForEndOfFrame();
            }
            else //正常進行時
            {
                GameObject fish = Instantiate(fishes[fishIndex]);

                fish.GetComponent<SpriteRenderer>().sortingOrder += i;
                fish.transform.SetParent(fishHolder, false);
                fish.transform.localPosition = generationPos[genPosIndex].localPosition;
                fish.transform.localRotation = generationPos[genPosIndex].localRotation;
                fish.transform.Rotate(0, 0, angleValue);
                fish.GetComponent<Ef_AutoMove>().speed = moveSpeed;
                if (moveType == 1) { fish.AddComponent<Ef_AutoRotate>().angularSpeed = angularSpeed; } //轉彎魚的旋轉行為
                yield return new WaitForSeconds(generation_delay);
            }
        }
    }
}
