using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class buffUI : MonoBehaviour
{
    public float rightOffset;
    public TMP_Text extendBuffName, extendBuffText,remainTimeText, buffText;
    public Image buffImage;

    public GameObject  Curse, DamB, DamD, DodgeB, DodgeD,HitB, HitD, Tuant, Aura, Tag_Undead, MvB,MvD, Tag_Rat, GeneCorpse, lifeSteal;
    public GameObject regenB, regenD;
    public GameObject AutoAttacker, AutoHealer,AutoTaunt,FPCollector, AN;
    public GameObject ArrowDodge,ArmorB,Tile, healthMax;

    public void extendBuffUIUpdate(_Buff buff, bool isTrait)
    {
        if (buff == null || buff.buff == null) return;
        extendBuffName.transform.parent.gameObject.SetActive(true);
        string CD = "" + buff.remainTime;

        if (isTrait) CD = "/";
        remainTimeText.text = CD;
   

        string name = buff.buff.name;
        if (buff.level != 0) name += " LV." + buff.level;

        extendBuffName.text = name;

        extendBuffText.text = buff.buff.description;
        buffImage.sprite = buff.buff.sprite;

      
        Curse.SetActive(false);
        DamB.SetActive(false);
        DamD.SetActive(false);
        DodgeB.SetActive(false);
        DodgeD.SetActive(false);
        Tuant.SetActive(false);
        Aura.SetActive(false);
        Aura.SetActive(false);
        Tag_Undead.SetActive(false);
        MvB.SetActive(false);
        MvD.SetActive(false);
        Tag_Rat.SetActive(false);
        GeneCorpse.SetActive(false);
        HitB.SetActive(false);
        HitD.SetActive(false);
        lifeSteal.SetActive(false);
        regenB.SetActive(false);
        regenD.SetActive(false);
        AutoAttacker.SetActive(false);
        AutoHealer.SetActive(false);
        AutoTaunt.SetActive(false);
        AutoTaunt.SetActive(false);
        FPCollector.SetActive(false);
        AN.SetActive(false);
        ArrowDodge.SetActive(false);
        ArmorB.SetActive(false);
        Tile.SetActive(false);
        healthMax.SetActive(false);

        if (isTrait) buffText.text = "Trait";

        else
        {
            if (buff.buff.isBuff) buffText.text = "Buff";
            else buffText.text = "Debuff";
        }

        if(buff.buff.buffType == Buff._buffType.tuant) Tuant.SetActive(true);
        if(buff.buff.aura_GrowingDeath == true) Aura.SetActive(true);
        if(buff.buff.aura_Tarish == true) Aura.SetActive(true);



        if (buff.buff.changePerTurnType == Buff._changePerTurnType.Healing || buff.buff.changePerTurnType == Buff._changePerTurnType.Poisoning)
        {
            if (buff.buff.changeNum > 0) { regenB.SetActive(true); regenB.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "Heal " + buff.buff.changeNum  + "/T"; }
            if (buff.buff.changeNum < 0) { regenD.SetActive(true); regenD.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "Lose " + buff.buff.changeNum + "/T"; }
        }


        if (buff.buff.dam > 0) { DamB.SetActive(true); DamB.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "Dmg + " + buff.buff.dam; }
        if (buff.buff.dam < 0) { DamD.SetActive(true); DamD.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "Dmg " + buff.buff.dam; }

        if(buff.buff.dodge < 0) { DodgeD.SetActive(true); DodgeD.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "Dod Penalty " + buff.buff.dodge + "%"; }
        if (buff.buff.dodge > 0) { DodgeB.SetActive(true); DodgeB.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "Dod + " + buff.buff.dodge + "%"; }

        if (buff.buff.hit < 0) { HitD.SetActive(true); HitD.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "Acc Penatly " + buff.buff.hit + "%"; }
        if (buff.buff.hit > 0) { HitB.SetActive(true); HitB.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "Acc + " + buff.buff.hit + "%"; }

        if (buff.buff.movePointMax < 0) { MvD.SetActive(true); MvD.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "Mov Penalty " + buff.buff.movePointMax; }
        if (buff.buff.movePointMax > 0) { MvB.SetActive(true); MvB.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "Mov + " + buff.buff.movePointMax; }
        if (buff.buff.lifesteal > 0) { lifeSteal.SetActive(true); lifeSteal.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "Life Steal " + buff.buff.lifesteal; }

       
        if(buff.buff.tag_Undead) Tag_Undead.SetActive(true);
        if(buff.buff.tag_Rat) Tag_Rat.SetActive(true);

        if (buff.buff.gain_Fp_Kill > 0) { GeneCorpse.SetActive(true); GeneCorpse.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "Necromancer " + buff.buff.gain_Fp_Kill; }

        if(buff.buff.auto_Attack) AutoAttacker.SetActive(true);
        if(buff.buff.auto_Heal) AutoHealer.SetActive(true);
        if(buff.buff.auto_Taunt) AutoTaunt.SetActive(true);
        if (buff.buff.gain_FP_per_Turn > 0)
        {
            FPCollector.SetActive(true);
        }

        if (buff.buff.armorNegative > 0)
        {
            AN.SetActive(true); AN.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "Armor Negative " + buff.buff.armorNegative;
        }

        if (buff.buff.dam_InHightGround > 0)
        {
            Tile.SetActive(true); Tile.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "Height Bonus " + buff.buff.dam_InHightGround;
        }
        if (buff.buff.armorMax > 0)
        {
            ArmorB.SetActive(true); ArmorB.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "Armor + " + buff.buff.armorMax;
        }
        if (buff.buff.dodgeRate_FacingRange > 0)
        {
            ArrowDodge.SetActive(true); ArrowDodge.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "Arrow Dodge + " + buff.buff.dodgeRate_FacingRange+"%";
        }
        if (buff.buff.healthMax > 0)
        {
            healthMax.SetActive(true); healthMax.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "Health + " + buff.buff.healthMax;
        }


    }
}
