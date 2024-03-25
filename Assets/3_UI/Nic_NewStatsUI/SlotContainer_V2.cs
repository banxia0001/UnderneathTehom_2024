using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotContainer_V2 : MonoBehaviour
{
    public enum SlotType { skill, perk, buff, servant}
    public GameObject selectLight, button_Select, rightFolder;
    [Header("Container")]
    public bool isDisplayedInRewardPanel;
    public SlotType slotType;
    public TMP_Text time, cost, text_nane;
    public Image Icon;

    [HideInInspector] public _Skill skill;
    [HideInInspector] public _Buff buff;
    [HideInInspector] public UnitData targetUnitData;
    [HideInInspector] public GameObject targetUnitPrefab;

    public TMP_Text Description;

    [Header("Container_Servant")]
    public TMP_Text attack;
    public TMP_Text hp, amr;
    public PortraitManager PM;

    //[Header("Height")]
    //public float minHeight = 148.02f;
    //public float spaceForEachLine = 16.09f;
    private void Start()
    {
        selectLight.SetActive(false);
    }
    public void Comfirm()
    {
        FindObjectOfType<RewardUI>().ComfirmReward(this.transform, this);
    }
    public void SlotInput_GainReward()
    {
        if (isDisplayedInRewardPanel)
        {
            FindObjectOfType<RewardUI>(true).SlotInput_GainReward(this.transform, this);
        }
    }

    public void SelectContainer(bool select)
    {
        if (select) { selectLight.SetActive(true); button_Select.SetActive(true); rightFolder.SetActive(false); }
        else {selectLight.SetActive(false); button_Select.SetActive(false); rightFolder.SetActive(true); }
    }

    public void InputUnitToUnitStatsPanelV2()
    {
        if (isDisplayedInRewardPanel)
        {
            FindObjectOfType<RewardUI>().Switch_PrefabData(targetUnitPrefab);
        }

        else
        {
            Unit unit = targetUnitPrefab.GetComponent<Unit>();
            FindObjectOfType<GameUI>().unitPanelUI.InputData(unit,unit.data,UnitStatsPanel_V2.PanelType.openUnitInPrefab,true);
        }
    }
    public void InputServant(GameObject targetUnitPrefab, UnitData unit, bool isDisplayedInRewardPanel, string description)
    {
        this.targetUnitPrefab = targetUnitPrefab;
        text_nane.text = unit.Name;
        PM.UpdatePortrait(unit);
        this.isDisplayedInRewardPanel = isDisplayedInRewardPanel;
        this.attack.text = ((unit.damage.damMin + unit.damage.damMax) / 2 + unit.damage.damBonus).ToString();
        this.hp.text = unit.healthMax.ToString();
        this.amr.text = unit.armorMax.ToString();
        this.targetUnitData = unit;
        this.Description.text = description;
    }
    public void InputBuff(_Buff buff, bool isPerk, UnitData unit, bool isDisplayedInRewardPanel)
    {
        this.isDisplayedInRewardPanel = isDisplayedInRewardPanel;
        this.buff = buff;
        text_nane.text = buff.buff.name;
        this.targetUnitData = unit;
        cost.gameObject.transform.parent.gameObject.SetActive(false);
        time.gameObject.transform.parent.gameObject.SetActive(false);

        if (isPerk) slotType = SlotType.perk;
        else { slotType = SlotType.buff; time.gameObject.transform.parent.gameObject.SetActive(true); }

        Icon.sprite = buff.buff.sprite;
        time.text = buff.remainTime.ToString();
        Description.text = buff.buff.description; //ExtendHeight();
    }

    public void InputSkill(_Skill skill, UnitData unit, bool isDisplayedInRewardPanel,GameObject prefab)
    {
        this.targetUnitPrefab = prefab;
        this.isDisplayedInRewardPanel = isDisplayedInRewardPanel;
        this.skill = skill;
        text_nane.text = skill.skill.name;
        this.targetUnitData = unit;
        cost.gameObject.transform.parent.gameObject.SetActive(true);
        time.gameObject.transform.parent.gameObject.SetActive(true);

        slotType = SlotType.skill;

        Icon.sprite = skill.skill.skillSprite;
        cost.text = skill.skill.Cost.ToString();
        time.text = skill.skill.CD.ToString();
        Description.text = skill.skill.description; //ExtendHeight();
    }

    //private void ExtendHeight()
    //{
    //    Debug.Log(Description.text);
    //    Debug.Log(Description.textInfo.linkCount + "Height2");
    //    float Line = Description.textInfo.lineCount;
    //    float HeightModi = 0;
    //    if (Line > 3) HeightModi = (Line - 3) * spaceForEachLine;
    //    Debug.Log(Line + "Height");
    //    RectTransform rt = this.GetComponent(typeof(RectTransform)) as RectTransform;
    //    rt.sizeDelta = new Vector2(rt.sizeDelta.x, HeightModi + minHeight);
    //}
  
}
