using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffDisplay : MonoBehaviour
{
    public _Buff buff;
    public bool isTrait;
    public Image image;
    public TMP_Text text;

    public void InputBuff(_Buff buff, bool isTrait)
    {
        this.buff = buff;
        this.isTrait = isTrait;

        string name = buff.buff.name;
        if (buff.level != 0) name += " LV." + buff.level;
        transform.GetChild(1).gameObject.GetComponent<Image>().sprite = buff.buff.sprite;
        //transform.GetChild(2).gameObject.GetComponent<TMP_Text>().text = name;
    }
}
