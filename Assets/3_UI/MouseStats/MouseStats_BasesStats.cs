using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MouseStats_BasesStats : MonoBehaviour
{
    [Header("Stats")]
    public TMP_Text attack, armor, move, range;
    public PortraitManager PM;

    public void InputStats(Unit unitInput, int type)
    {
        UnitData data = unitInput.currentData;
        UnitWeapon dam = data.damage;

        //Normal

        if (type == 1)
        {
            int Dam = dam.damMin + dam.damBonus;
            int maxDam = dam.damMax + dam.damBonus;
            string AddOne = "";
            //if (dam.aoeEffect) AddOne = " (A0E1)";
            attack.text = Dam + "~" + maxDam + AddOne;
            range.text = data.damage.range.ToString();
            armor.text = data.armorNow.ToString();
            move.text = data.movePointNow + "/" + data.movePointMax;
        }

        if (type == 2)
        {
            range.text = data.damage.range.ToString();
            armor.text = data.armorNow.ToString();
        }

        if (type == 3)
        {
            EnemyBoneFire ebf = unitInput.gameObject.GetComponent<EnemyBoneFire>();
            int time = ebf.timer;
            attack.text = "Spawn in <color=#FF4343>" + time +"T";
            PM.UpdatePortrait(ebf.unitToSpawn.data);
        }

    }
}
