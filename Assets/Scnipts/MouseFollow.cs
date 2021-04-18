using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseFollow : MonoBehaviour
{

    public RectTransform canvas_UGUI;
    public Camera mainCamera;
    private Vector3 mousePos;

    void Update()
    {
        if (StaticScript.activeSwitch)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas_UGUI, new Vector2(Input.mousePosition.x, Input.mousePosition.y), mainCamera, out mousePos); //滑鼠位置轉換(從螢幕座標到世界座標)

            float z;
            if (mousePos.x > transform.position.x)
            {
                z = -Vector3.Angle(Vector3.up, mousePos - transform.position);
                transform.localRotation = Quaternion.Euler(0, 0, z);
            }
            else
            {
                z = Vector3.Angle(Vector3.up, mousePos - transform.position);
                transform.localRotation = Quaternion.Euler(0, 0, z);
            }
        }        
    }
}
