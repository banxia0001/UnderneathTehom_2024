using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UnitMousePanel_Stats : MonoBehaviour
{
    public TMP_Text text_Move;
    public TMP_Text text_Health;

    public void Input(Unit unit)
    {
        this.text_Move.text = unit.currentData.movePointNow + "/" + unit.currentData.movePointMax;
        this.text_Health.text = unit.currentData.healthNow + "/" + unit.currentData.healthMax;
    }
}
