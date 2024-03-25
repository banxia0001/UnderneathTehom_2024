using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitPanelUI : MonoBehaviour
{
    public bool isFindUnitFromLevel;
    public Unit unitHolder;
    public Image portrait;

    [Header("Stats_1")]
    public TMP_Text text_Class;
    //public TMP_Text text_Name;
    public TMP_Text text_Health;
    public TMP_Text text_Dam, text_Range, text_Nbr,text_Hit, text_ArP, 
                    text_Armor, text_Dodge, text_Power, text_MR,
                    text_SR, text_Move;


    [Header("Portrait")]
    public GameObject por_AvatarB;
    public GameObject por_Avatar,por_ShieldSkel,por_GreatswordSkel, por_ArcherSkel, por_ArcherRat, por_MeleeRat, por_LancerSkel, por_MinorRat, por_ZombieRat, por_SporeRat, por_BoneRat;


    public List<SkillButtom> skill_buttons;
    public List<BuffDisplay> buff_buttons;
    public List<BuffDisplay> perk_buttons;

    public void InputData(Unit unit, bool isUsingStatsFromPrefabData, bool isFindUnitFromLevel)
    {
        this.isFindUnitFromLevel = isFindUnitFromLevel;

        if (isFindUnitFromLevel)
        {
            GameController GC = FindObjectOfType<GameController>();
            GC.can_MoveCam = false;
            CamMove cam = FindObjectOfType<CamMove>();
            cam.positionBefore = cam.transform.position;
            cam.camZoomBefore = cam.camZoom_Current;

            cam.addPos(unit.transform.position + new Vector3(1.9f,.25f,0), true);
            cam.addZoom(1.75f, true);
        }

        UnitData data = null;
        if (isUsingStatsFromPrefabData)
        {
            data = new UnitData(unit.data);
            foreach (_Buff _buff in data.traitList)
            { UnitFunctions.CheckBuff(data, _buff.buff, null); }
        }
        else data = unit.currentData;

        unitHolder = unit;
        InputStatsPanel(data);
        //InputSkills(data, unit);
        InputPortrait(unit);
        InputBuff(unit);
        InputPerk(unit);
    }

    private void InputStatsPanel(UnitData data)
    {
        //[Name]
        text_Class.text = data.Name;
        //text_Health.text = "<size=>" + unit.health + "</size>" + "/" + unit.healthMax;
        text_Health.text = data.healthNow + "/" + data.healthMax;

        //[Weapon]
        UnitWeapon dam = data.damage;
        int Dam = dam.damMin + dam.damBonus;
        int maxDam = dam.damMax + dam.damBonus;
        text_Dam.text = Dam + "~" + maxDam;

        text_Range.text = data.damage.range.ToString();
        text_Nbr.text = "X" +  data.damage.num.ToString();
        text_Hit.text = data.damage.hit + "%";

        //[Prot]
        text_Armor.text = data.armorNow + "/" + data.armorMax;
        text_ArP.text = data.damage.armorSunder.ToString();
        text_Dodge.text = data.dodge + "%";

        //[Pow]
        text_Power.text = data.power.ToString();
        text_MR.text = data.MR + "";
        //text_SR.text = unit.SR + "%";

        text_Move.text = data.movePointNow + "/" + data.movePointMax;
    }



    private void InputPortrait(Unit unit)
    {
        por_Avatar.SetActive(false);
        por_AvatarB.SetActive(false);
        por_ShieldSkel.SetActive(false);
        por_GreatswordSkel.SetActive(false);
        por_ArcherSkel.SetActive(false);
        por_ArcherRat.SetActive(false);
        por_MeleeRat.SetActive(false);
        por_LancerSkel.SetActive(false);
        por_MinorRat.SetActive(false);
        por_ZombieRat.SetActive(false);
        por_SporeRat.SetActive(false);
        por_BoneRat.SetActive(false);

        if (unit.data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._1_Avatar) { por_Avatar.SetActive(true); por_AvatarB.SetActive(true); }
        if (unit.data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._4_Skel_Shield) { por_ShieldSkel.SetActive(true); }
        if (unit.data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._5_Skel_GreatSword) { por_GreatswordSkel.SetActive(true); }
        if (unit.data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._6_Skel_Archer) { por_ArcherSkel.SetActive(true); }
        if (unit.data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._9_Rat_Archer) { por_ArcherRat.SetActive(true); }
        if (unit.data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._8_Rat_Shield) { por_MeleeRat.SetActive(true); }
        if (unit.data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._3_Skel_Lance) { por_LancerSkel.SetActive(true); }
        if (unit.data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._10_Rat_Zombie) { por_ZombieRat.SetActive(true); }
        if (unit.data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._7_Rat_Minor) { por_MinorRat.SetActive(true); }
        if (unit.data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._12_Rat_SporeFighter) { por_SporeRat.SetActive(true); }
        if (unit.data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._11_Rat_BoneRunner) { por_BoneRat.SetActive(true); }
    }


    //private void InputSkills(UnitData data, Unit unit, bool isUsingStatsFromData)
    //{
    //    foreach (SkillButtom skill in skill_buttons)
    //    {
    //        skill.SetAsLockedAbility();
    //    }

    //    if (data.Skill != null)
    //        if (data.Skill.Count != 0)
    //        {
    //            for (int i = 0; i < data.Skill.Count; i++)
    //            {
    //                skill_buttons[i].Input(i, data.Skill[i], unit, isUsingStatsFromData);
    //            }
    //        }
    //}

    private void InputBuff(Unit unit)
    {
        foreach (BuffDisplay skill in buff_buttons)
        {
            skill.gameObject.SetActive(false);
        }

        if (unit.buffList != null)
            if (unit.buffList.Count != 0)
            {
                for (int i = 0; i < unit.buffList.Count; i++)
                {
                    buff_buttons[i].gameObject.SetActive(true);
                    buff_buttons[i].InputBuff(unit.buffList[i], false);
                }
            }
    }

    private void InputPerk(Unit unit)
    {
        foreach (BuffDisplay skill in perk_buttons)
        {
            skill.gameObject.SetActive(false);
        }

        if (unit.data.traitList != null)
            if (unit.data.traitList.Count != 0)
            {
                for (int i = 0; i < unit.data.traitList.Count; i++)
                {
                    perk_buttons[i].gameObject.SetActive(true);
                    perk_buttons[i].InputBuff(unit.data.traitList[i], true);
                }
            }
    }
    public void Close_Display_UnitPanelUI()
    {
        GameController GC = FindObjectOfType<GameController>();
        GC.PlayerPressEscape();
        this.gameObject.SetActive(false);
    }
}
