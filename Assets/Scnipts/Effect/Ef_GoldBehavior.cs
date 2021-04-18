using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ef_GoldBehavior : MonoBehaviour
{
    private int carryGold;
    private Transform goldCollecter;

    public int CarryGold
    {
        get
        {
            return carryGold;
        }
        set
        {
            carryGold = value;
        }
    }

    void Start()
    {
        goldCollecter = GameObject.Find("Canvas_90/GoldCollecter").GetComponent<Transform>();
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, goldCollecter.transform.position, 20 * Time.deltaTime);
    }

}
