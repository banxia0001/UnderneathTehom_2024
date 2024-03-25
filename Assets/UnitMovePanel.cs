using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitMovePanel : MonoBehaviour
{
    public TMP_Text text_Move;


    public void InputPanel(Unit unit, int moveN)
    {
        if (moveN < 0) moveN = 0;
        text_Move.text = "<size=25>" + moveN + "</size>/" + unit.currentData.movePointMax;
    }
}
