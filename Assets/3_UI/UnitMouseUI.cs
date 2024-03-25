using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitMouseUI : MonoBehaviour
{
    private GameController GC;
    public PortraitManager PM;
    public GameObject activeBuff_D,activeBuff_H,melee, range, highGround, curseBonus, repel, lowerGround, staticTarget, ratDodgeReduce, shieldBearer;
    public TMP_Text text_Dam, text_Hit;
    public GameObject active, disactive;
    private void Awake()
    {
        GC = FindObjectOfType<GameController>();
        activeBuff_D.SetActive(false);
        activeBuff_H.SetActive(false);
        melee.SetActive(false);
        range.SetActive(false);
        highGround.SetActive(false);
        curseBonus.SetActive(false);
        repel.SetActive(false);
        lowerGround.SetActive(false);
        staticTarget.SetActive(false);
        ratDodgeReduce.SetActive(false);
        shieldBearer.SetActive(false);
    }

    public void InputPanel(Unit attacker,Unit defender)
    {
        if (attacker == null) return;
        if (defender == null) return;

        PM.UpdatePortrait(attacker.data);


        activeBuff_D.SetActive(false);
        activeBuff_H.SetActive(false);
        melee.SetActive(false);
        range.SetActive(false);
        highGround.SetActive(false);
        curseBonus.SetActive(false);
        repel.SetActive(false);
        lowerGround.SetActive(false);
        staticTarget.SetActive(false);
        ratDodgeReduce.SetActive(false);
        shieldBearer.SetActive(false);

        active.SetActive(false);
        disactive.SetActive(false);

        if(!attacker.isActive)
        {
            disactive.SetActive(true);
            return; 
        }

        active.SetActive(true);
        bool canRange = false;
        if (attacker.currentData.damage.range > 1) canRange = true;

        int hitrate = GameFunctions.CalculateHit_Start(attacker, defender, attacker.currentData.damage, null, canRange);

        PathNode path = attacker.nodeAt;
        if(attacker.currentData.damage.range < 1)
        {
            if (GC.nodeWithEnemyNear != null)
                if (GC.nodeWithEnemyNear != attacker.nodeAt)
                {
                    path = GC.nodeWithEnemyNear;
                } 
        }
        int[] dam = GameFunctions.calculateRandomDamage(attacker, defender, attacker.currentData.damage, null, false, path);


        if (canRange) range.SetActive(true);
        else melee.SetActive(true);

        //[Curse]
        if (defender.isCurse)
        {
            int bonus = UnitFunctions.Check_DamBonus_TowardCursedUnit(attacker);
            curseBonus.SetActive(true);
            curseBonus.transform.GetChild(0).GetChild(4).GetComponent<TMP_Text>().text = "+" + bonus;
        }

        if (defender.unitAttribute != Unit.UnitAttribute.alive)
        {
            staticTarget.SetActive(true);
        }


        //DAM
        int dam_B = UnitFunctions.Check_Damage_WithBuff(attacker);
        if (dam_B > 0)
        {
            activeBuff_D.SetActive(true);
            activeBuff_D.transform.GetChild(0).GetChild(4).GetComponent<TMP_Text>().text = "+" + dam_B;
        }


        //HIT
        int hit_B = UnitFunctions.Check_Hitrate_WithBuff(attacker);
        if (hit_B > 0)
        {
            activeBuff_H.SetActive(true);
            activeBuff_H.transform.GetChild(0).GetChild(4).GetComponent<TMP_Text>().text = "+" + hit_B + "%";
        }



        //Grid Height to Dam
        if (canRange)
        {
            if (attacker.nodeAt.height - 1 > defender.nodeAt.height)
            {
                int dam2 = 1 + UnitFunctions.Check_DamBonus_HighGround(attacker);
                highGround.SetActive(true);
                highGround.transform.GetChild(0).GetChild(4).GetComponent<TMP_Text>().text = "+" + dam2;

            }

            //if (attacker.nodeAt.height + 1 < defender.nodeAt.height)
            //{
            //    lowerGround.SetActive(true);
            //}
        }

        //Grid Height to Dam
        else
        {
            if (GC.nodeWithEnemyNear != null)
            {
                if (GC.nodeWithEnemyNear.height - 1 > defender.nodeAt.height)
                {
                    int dam2 = 1 + UnitFunctions.Check_DamBonus_HighGround(attacker);
                    highGround.SetActive(true);
                    highGround.transform.GetChild(0).GetChild(4).GetComponent<TMP_Text>().text = "+" + dam2;
                }

                //if (GC.nodeWithEnemyNear.height + 1 < defender.nodeAt.height)
                //{
                //    lowerGround.SetActive(true);
                //}
            }
        }



        if (attacker.canRepel)
        {
            repel.SetActive(true);
        }




        //Tags: Race advantages
        int enemyDodge = UnitFunctions.Check_Hitrate_WithRat(defender);

        if (enemyDodge != 0)
        {
            ratDodgeReduce.SetActive(true);

            ratDodgeReduce.transform.GetChild(0).GetChild(4).GetComponent<TMP_Text>().text = "-" + enemyDodge + "%";
            hitrate -= enemyDodge;
        }


        //ShieldBearer
        if (canRange)
        {
            int enemyDodgeToRange = UnitFunctions.Check_DodgeRateAddOnRange(defender);
            if (enemyDodgeToRange != 0)
            {
                shieldBearer.SetActive(true);
                shieldBearer.transform.GetChild(0).GetChild(4).GetComponent<TMP_Text>().text = "-" + enemyDodgeToRange + "%";
            }
            hitrate -= enemyDodgeToRange;
        }

        if (dam[0] < 0) dam[0] = 0;
        if (dam[1] < 0) dam[1] = 0;

        if (hitrate < 0) hitrate = 0;
        if (hitrate > 100) hitrate = 100;
        text_Dam.text =dam[0]  + "~" + dam[1];
        text_Hit.text = hitrate + "%";

    }
}
