    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatPanelUI : MonoBehaviour
{
    [Header("Skills")]
    public SkillButtom[] bottoms;
    public TMP_Text manaText;
    //public TMP_Text manaPerTurnText;
    public TMP_Text enemyPanelText;
    public Transform skill_Transform;

    [Header("2 stats")]
    public TMP_Text moveText;
    public TMP_Text armorText;
    public TMP_Text damageText;



    public GameObject enemyPanel, guide_1,guide_2;
    public GameObject skill_UI;
    public GameObject panel_Skill;
    public Image panel_SkillIcon;
    //public CombatUnitList_V2 combatUnitList_V2;
    public PortraitManager PM;



    [Header("Animation")]
    public Animator ManaIcon;

    private void Start()
    {
        guide_1.SetActive(false);
        guide_2.SetActive(false);
    }
    public void SwitchUnitIconList(bool open)
    {
        if (open)
        {
            //combatUnitList_V2.gameObject.SetActive(true);
            //combatUnitList_V2.SpawnIcons();
        }
        else
        { 
        //combatUnitList_V2.gameObject.SetActive(false);
        } 
    }
    public void InputFriendUnit(Unit SelectedUnit)
    {
        PM.UpdatePortrait(SelectedUnit.data);
        //friendlyIcons.InpuUnit(SelectedUnit);
    }
    public void Input_Panel_SkillIcon(Sprite sprite)
    {
        panel_Skill.SetActive(true);
        panel_SkillIcon.sprite = sprite;
    }
    
    //Skill
    public void DisplaySkill(List<_Skill> displaySkill, Unit unit)
    {
        InputFriendUnit(unit);

        for (int i = 0; i < bottoms.Length; i++)
        {
            if (displaySkill.Count > i)
            {
                bottoms[i].gameObject.SetActive(true);
                bottoms[i].Input(i, displaySkill[i], unit);
            }
        }
    }
    public void DeleteSkill()
    {
        foreach (SkillButtom bottom in bottoms)
        {
            bottom.gameObject.SetActive(false);
        }
    }
    public void SetSkillButtomAnimationOff()
    {
        foreach (SkillButtom gm in bottoms)
        {
            if (gm.gameObject.activeSelf == true)
                gm.gameObject.GetComponent<Animator>().SetBool("enter", false);
        }
    }
    public void SetSkillButtomAnimationOn(int i)
    {
        if (bottoms[i].gameObject.activeSelf == true)
            bottoms[i].gameObject.GetComponent<Animator>().SetBool("enter", true);
    }

    public void SetMoveText(Unit unit)
    {
        if(unit != null)
        {
            moveText.text = unit.currentData.movePointNow.ToString();
            armorText.text = unit.currentData.armorNow.ToString();
            int dam = (unit.currentData.damage.damMax + unit.currentData.damage.damMin) / 2 + unit.currentData.damage.damBonus;
            damageText.text = dam.ToString();
        }
    }
   
    public void SetManaPool(GameController GC)
    {
        string PA =  FPManager.FP.ToString();
        
        string PB = "+1";

        if (GC.gameState == GameController._gameState.playerTurn || GC.gameState == GameController._gameState.playerInSpell)
        {
            if (FPManager.FPInUse != 0)
            {
                PB = "<color=#FF0000>-" + FPManager.FPInUse+ "</color>";
             }
        }
           
        manaText.text = manaText.text = PA;
        //manaPerTurnText.text = PB;
    }




    //public void InputUnitClickableIconList(List<Unit> unitList)
    //{
    //    foreach (CombatUniClickabletIcon icon in IconLists)
    //    {
    //        icon.gameObject.SetActive(false);
    //    }

    //    for (int i = 0; i < unitList.Count; i++)
    //    {
    //        if (i == 8) break;
    //        IconLists[i].gameObject.SetActive(true);
    //        IconLists[i].InputFriendUnit(unitList[i]);
    //    }
    //}

    //public void InputUnitClickableIconList_Active(Unit selectUnit)
    //{
    //    for (int i = 0; i < IconLists.Count; i++)
    //    {
    //        IconLists[i].CheckIfActive(selectUnit);
    //    } 
    //}
    //public void SetEnemyButtonRight(Unit unit)
    //{
    //    if (unit.unitAttribute != Unit.UnitAttribute.alive)
    //    {
    //        enemyPanel_ButtonRight.SetActive(true); return;
    //    }

    //    enemyPanel_ButtonRight.SetActive(true);
    //    enemyIcons.InpuUnit(unit);
    //    moveText_E.text = unit.currentData.movePointNow.ToString();
    //    armorText_E .text = unit.currentData.armorNow.ToString();
    //}

}
