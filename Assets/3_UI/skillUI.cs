using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class skillUI : MonoBehaviour
{
    public float rightOffset;
    public TMP_Text skillName;
    public TMP_Text skillDes;
    public TMP_Text skillCost;
    public TMP_Text skillCD;
    public Image icon;


    public GameObject isFreindly,instant, range, dam, pow, hit, knock, splash, armorS, num, summonD,summon, buff;
    public GameObject charge, attackOneline, taunt, heal,exchange, line1, line2;
    public buffUI myBuffUI;

    public void skillInput(Skill skill, UnitData data)
    {
        if (skill == null) return;
        rightOffset = -300;
        skillName.text = skill.name + "";

        icon.sprite = skill.skillSprite;
        myBuffUI.gameObject.SetActive(false);
        isFreindly.SetActive(false);
        instant.SetActive(false);
        range.SetActive(false);
        dam.SetActive(false);
        hit.SetActive(false);
        pow.SetActive(false);
        knock.SetActive(false);
        splash.SetActive(false);
        armorS.SetActive(false);
        num.SetActive(false);
        summonD.SetActive(false);
        summon.SetActive(false);
        heal.SetActive(false);
        exchange.SetActive(false);

        charge.SetActive(false);
        attackOneline.SetActive(false);
        taunt.SetActive(false);
        buff.SetActive(false);

        line1.SetActive(false);
        line2.SetActive(false);
   
        int damMin = 0;
        int damMax = 0;

        //UnitData data = unit.currentData;
        //if(button.isUsingStatsFromData) data = unit.data;

        if (skill.type != Skill._Type.Summon && skill.type != Skill._Type.SummonFromDeath && skill.type != Skill._Type.TargetToBuff && skill.type != Skill._Type.Reload)
        {
            if (skill.damageType == Skill._DamageType.none)
            {
            }
            else
            {
                switch (skill.damageType)
                {
                    case Skill._DamageType.useDam:
                        damMin = data.damage.damMin + skill.damMin + data.damage.damBonus;
                        damMax = data.damage.damMax + skill.damMax + data.damage.damBonus;

                        dam.SetActive(true);
                        TMP_Text text3 = dam.transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_Text>();
                        text3.text = "Dam: " + damMin + " ~ " + damMax + "";
                        break;

                    case Skill._DamageType.usePow:
                        damMin = skill.damMin + data.power;
                        damMax = skill.damMax + data.power;

                        pow.SetActive(true);
                        TMP_Text text2 = pow.transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_Text>();
                        //damColor = "<color=#DD73FF>";
                        text2.text = "Dam: " + damMin + " ~ " + damMax + "";
                        break;
                }
            }
        }

          

        skillCost.text = "" + skill.Cost;
        skillCD.text = "" + skill.CD;

        string addoneList = "";

        range.SetActive(true);
        TMP_Text text = range.transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_Text>();
        text.text = "Range: " + skill.range;
        if(skill.isSelfUse) text.text = "Self";


        if (skill.type == Skill._Type.SummonFromDeath)
        {
            summonD.SetActive(true);
            return;
        }

        if (skill.type == Skill._Type.Summon)
        {
            summon.SetActive(true);
            return;
        }

        //[Same List]
        //[Same List]
        //[Same List]

        addoneList += skill.description;
        skillDes.text = addoneList;

        if (skill.type != Skill._Type.Summon && skill.type != Skill._Type.SummonFromDeath)
        {
            if (skill.isFriendly)
            { isFreindly.SetActive(true);
                if (skill.causeHeal) heal.SetActive(true);
            }
         
            if (skill.isInstant) instant.SetActive(true);

            if (skill.type == Skill._Type.MoveInLine) charge.SetActive(true);
            if (skill.type == Skill._Type.ExchangePosition) exchange.SetActive(true);
            if (skill.type == Skill._Type.AttackALine) attackOneline.SetActive(true);
            if (skill.type == Skill._Type.TeleportToTargetAndAttackRoad) { attackOneline.SetActive(true); charge.SetActive(true); }
            if (skill.buff != null)
                if (skill.buff.buffType == Buff._buffType.tuant) taunt.SetActive(true);



            if (skill.buff != null)
            {
                rightOffset = -600;
                line1.SetActive(true);
                line2.SetActive(true);

                text = buff.transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_Text>();
                
                Image image = buff.transform.GetChild(0).transform.GetChild(1).GetComponent<Image>();

                myBuffUI.gameObject.SetActive(true);
                //myBuffUI.GetComponent<RectTransform>().position = buffUIStartPos.GetComponent<RectTransform>().transform.position + new Vector3(100, 0, 0);
                buff.SetActive(true);

                if (skill.buff.isBuff)
                {
                    text.text = "Buff: " + skill.buff.name;
                    image.sprite = skill.buff.sprite;
                }

               else
                {
                    text.text = "Debuff: " + skill.buff.name;
                    image.sprite = skill.buff.sprite;
                }

                myBuffUI.extendBuffUIUpdate(new _Buff(skill.buff),false);
            }

            if (skill.splashRange >= 1)
            {
                splash.SetActive(true);
                text = splash.transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_Text>();
                text.text = "AOE Area: " + skill.splashRange;
            }

            if (skill.num != 1)
            {
                num.SetActive(true);
                text = num.transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_Text>();
                text.text = "Nbr of Attack: " + skill.num;
            }

            if (skill.hitBonus != 0)
            {
                hit.SetActive(true);
                text = hit.transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_Text>();
                text.text = "Hit bonus: " + skill.hitBonus;

            }

            if (skill.autoHit)
            {
                hit.SetActive(true);
                text = hit.transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_Text>();
                text.text = "Auto-Hit";
            }

            if (skill.armorSunder != 0)
            {
                armorS.SetActive(true);
                text = armorS.transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_Text>();
                text.text = "ArmorSunder: " +skill.armorSunder;
            }

            if (skill.causeKnockBack)
            {
                knock.SetActive(true);
                text = knock.transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_Text>();
                text.text = "Knock Back: " + skill.KnockBack_Distance;
            }
        }
    }


}
