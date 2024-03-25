using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MouseStats_Buff_V3 : MonoBehaviour
{
    public Image image;
    public TMP_Text text;
    public void Input_Buff(_Buff buff, bool isBuff)
    {
        string name = buff.buff.name;
        if (isBuff)
        {
            name += " (" + buff.remainTime + "Turn)";
        }
        text.text = name;
        image.sprite = buff.buff.sprite;
    }
}
