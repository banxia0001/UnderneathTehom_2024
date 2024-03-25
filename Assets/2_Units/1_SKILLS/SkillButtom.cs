using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillButtom : MonoBehaviour
{
    [HideInInspector] public _Skill skill;
    [HideInInspector] public int skillOrder;
    [HideInInspector] public Unit skillUser;

    public Sprite nullImage;

    public Image skillIcon;
    public TMP_Text CD;
    public GameObject panel;

    [Header("DetailSkill_Section")]
    public bool isBriefSkill;
    public bool isLocked;
    public void SetAsLockedAbility()
    {
        isLocked = true;
        panel.SetActive(false);
        skillIcon.sprite = nullImage;
    }

    public void Output()
    {
        if (skillOrder == 0)
        {
            CombatPanelUI combat = FindObjectOfType<CombatPanelUI>(true);
            combat.guide_1.SetActive(false);
        }
        if (skillOrder == 1)
        {
            CombatPanelUI combat = FindObjectOfType<CombatPanelUI>();
            combat.guide_2.SetActive(false);
        }
        GameController GC = FindObjectOfType<GameController>();
        GC.UseSkill(skillOrder);
    }
    public void Input(int order, _Skill thisSkill, Unit unit)
    {
        

        if (thisSkill == null)
        {
            SetAsLockedAbility();
            return;
        } 

        if (thisSkill.skill == null)
        {
            SetAsLockedAbility();
            return;
        }

        this.gameObject.transform.GetChild(0).gameObject.SetActive(true);
        skillOrder = order;
        skill = thisSkill;
        skillUser = unit;
        isLocked = false;
        
        if (thisSkill.skill.skillSprite == null)
        {
            Debug.Log(thisSkill.skill.name + " missing Image");
        }
        else skillIcon.sprite = thisSkill.skill.skillSprite;
     
      
        CD.text = "" + thisSkill.CD;
        panel.SetActive(true);

        if (isBriefSkill)
        {
            panel.SetActive(false);
            return;
        }

        if (thisSkill.CD <= 0)
        {
            panel.SetActive(false);

            if (!unit.isActive && !thisSkill.skill.isInstant)
            {
                panel.SetActive(true);
                CD.text = "";
            }
            if (FPManager.FP < thisSkill.skill.Cost)
            {
                //panel.SetActive(true);
                //CD.text = "";
            }
        }
    }
}
