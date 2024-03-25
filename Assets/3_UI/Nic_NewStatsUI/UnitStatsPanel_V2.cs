using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitStatsPanel_V2 : MonoBehaviour
{
    public enum PanelType{ openUnitInGame, openUnitInPrefab }
    public PanelType panelType;

    public enum DisplayType { skill, buff, perk }
    public DisplayType displayType;

    public ListPanelUI listPanelUI;
    public bool isDisplayedInRewardPanel = false;
    private UnitData dataInHold;
    private Unit unit;


    [Header("Stats")]
    public TMP_Text text_Health;
    public TMP_Text text_Name, text_Type;

    public TMP_Text text_Dam, text_Range, text_Hit, text_Armor, text_Dodge, text_Move, text_Flesh;
    public Color32 myGreen, myRed, myGrey;

    public GameObject line_1,line_2,line_3;

    [Header("Hide")]
    public GameObject line_L;
    public GameObject line_S, T_L, T_S, B_L, B_S, List;

    
    public void InputData(Unit unit, UnitData realData, PanelType type, bool IsLong)
    {
        if (FindObjectOfType<GameController>() != null) isDisplayedInRewardPanel = false;
        else isDisplayedInRewardPanel = true;


        if (IsLong)
        {
            line_L.SetActive(true); line_S.SetActive(false);
            T_L.SetActive(true); T_S.SetActive(false);
            B_L.SetActive(true); B_S.SetActive(false);
            List.SetActive(true);
        }
        else
        {
            line_L.SetActive(false); line_S.SetActive(true);
            T_L.SetActive(false); T_S.SetActive(true);
            B_L.SetActive(false); B_S.SetActive(true);
            List.SetActive(false);
        }

        panelType = type;
        this.unit = unit;
        if (panelType == PanelType.openUnitInGame)
        {
            //Cam towards the unit
            //GameController GC = FindObjectOfType<GameController>();
            CamMove cam = FindObjectOfType<CamMove>();
            cam.camMoveToUnit_WhenStatsPanelOpen(unit);
            dataInHold = unit.currentData;
        }

        if (panelType == PanelType.openUnitInPrefab)
        {
            dataInHold = new UnitData(realData);
            foreach (_Buff _buff in dataInHold.traitList)
            { UnitFunctions.CheckBuff(dataInHold, _buff.buff, null); }
            //add portrait
        }

        InputStatsPanel(dataInHold);
        SwitchDisplayType(displayType);
    }


    private void InputStatsPanel(UnitData data)
    {
        if (unit.unitTeam == Unit.UnitTeam.playerTeam)
        {
            text_Type.color = myGreen;
            if (unit.unitType == Unit.UnitSummonType.hero) text_Type.text = "Hero";
            if (unit.unitType == Unit.UnitSummonType.summon) text_Type.text = "Summon";
            if (unit.unitType == Unit.UnitSummonType.sentry) text_Type.text = "Sentry"; 
        }

        else if (unit.unitTeam == Unit.UnitTeam.neutral)
        {
            text_Type.color = myGrey; text_Type.text = "Neutral";
            if(unit.unitAttribute == Unit.UnitAttribute.corpse) text_Type.text = "Corpse";
        }

        else if (unit.unitTeam == Unit.UnitTeam.enemyTeam)
        {
            text_Type.color = myRed; text_Type.text = "Enemy";
            if (unit.data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._12_Rat_SporeFighter  ) text_Type.text = "Boss";
        }

        //[Name]
        text_Name.text = data.Name;
        text_Health.text = data.healthNow + "/" + data.healthMax;

        //[Weapon]
        UnitWeapon dam = data.damage;
        string AddOne = "";
        if (dam.aoeEffect) AddOne = " <color=#B7B7B7>(AOE 1)</color>";

        int Dam = dam.damMin + dam.damBonus;
        int maxDam = dam.damMax + dam.damBonus;
        text_Dam.text = Dam + "~" + maxDam + AddOne;
        text_Range.text = data.damage.range.ToString();
        text_Hit.text = data.damage.hit + "%";

        //[Prot]
        text_Armor.text = data.armorNow + "/" + data.armorMax;
        text_Dodge.text = data.dodge + "%";
        text_Flesh.text = data.FleshDrop.ToString();
        text_Move.text = data.movePointNow + "/" + data.movePointMax;
    }

   

    public void SwitchDisplayType(int i)
    {
        if (i == 0) SwitchDisplayType(DisplayType.skill);
        if (i == 1) SwitchDisplayType(DisplayType.buff);
        if (i == 2)SwitchDisplayType(DisplayType.perk);
    }
    private void SwitchDisplayType(DisplayType displayType)
    {
        line_1.SetActive(false);
        line_2.SetActive(false);
        line_3.SetActive(false);
        listPanelUI.ClearField();
        this.displayType = displayType;
        if (displayType == DisplayType.skill) { InputSkill(dataInHold); line_1.SetActive(true); }
        if (displayType == DisplayType.buff && panelType == PanelType.openUnitInGame) {InputBuff(unit); line_2.SetActive(true); }
        if (displayType == DisplayType.perk) {InputPerk(dataInHold); line_3.SetActive(true); }
    }

    public void InputSkill(UnitData data)
    {
        if (data.Skill != null && data.Skill.Count > 0)
        {
            foreach (_Skill skill in data.Skill)
            {
                listPanelUI.InputSkill(skill, data, isDisplayedInRewardPanel);
            }
        }
    }

    public void InputBuff(Unit unit)
    {
        if (unit.buffList != null && unit.buffList.Count > 0)
        {
            foreach (_Buff buff in unit.buffList)
            {
                listPanelUI.InputBuff(buff, dataInHold, isDisplayedInRewardPanel);
            }
        }
    }

    public void InputPerk(UnitData data)
    {
        if (data.traitList != null && data.traitList.Count > 0)
        {
            foreach (_Buff buff in data.traitList)
            {
                listPanelUI.InputPerk(buff, data, isDisplayedInRewardPanel);
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
