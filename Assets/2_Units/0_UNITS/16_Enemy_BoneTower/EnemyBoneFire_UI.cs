using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EnemyBoneFire_UI : MonoBehaviour
{
    public TMP_Text text;
    public PortraitManager PM;
    public void InsertUnit(Unit unit, int timer)
    {
        text.text = timer.ToString();
        PM.UpdatePortrait(unit.data);
    }
}
