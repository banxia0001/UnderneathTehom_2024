using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UnitSlotUI : MonoBehaviour
{
    public BarController bar;
    public TMP_Text NAME,HEALTH;
    public Image unitImage;
    public UnitData data;
    public void Input_Data(UnitData unit)
    {
        data = unit;
        //if (unit.Protrait != null)
        //    unitImage.sprite = unit.Protrait;

        HEALTH.text = unit.healthNow + "/" + unit.healthMax;
        NAME.text = unit.Name;
        bar.SetValue_Initial(unit.healthNow, unit.healthMax);
    }
}
