using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorfulGuideLine : MonoBehaviour
{
    private GameController GC;
    public GameObject red, green, purple;
    public GameObject guide_Skill, guide_Select;

    private Unit unit;


    private void Start()
    {
        GC = FindObjectOfType<GameController>();
        CloseSkillVisual();
        //
        //
    }


    public void CloseSkillVisual()
    {
        red.SetActive(false);
        green.SetActive(false);
        purple.SetActive(false);
    }

    public void SkillVisual_FixedUpdate(_Skill skillInUse, PathNode mouseAtNode)
    {
        if (GC.skillInUse == null) return;
        GameController.DeleteAllWalkingPath();

        int range = skillInUse.skill.range;
        bool isRange = false;
        if (range > 1) isRange = true;
        bool isFriendly = skillInUse.skill.isFriendly;

        #region skillVisual_ByType
        switch (skillInUse.skill.type)
        {
            case Skill._Type.TargetToAttack:
                SkillVisual_DrawLine(range, isRange, isFriendly, mouseAtNode);
                break;

            case Skill._Type.TargetToBuff:
                SkillVisual_DrawLine(range, isRange, true, mouseAtNode);
                break;

            case Skill._Type.TargetToDebuff:
                SkillVisual_DrawLine(range, isRange, false, mouseAtNode);
                break;

            case Skill._Type.Reload:
                SkillVisual_DrawLine(range, isRange, isFriendly, mouseAtNode);
                break;

            case Skill._Type.ExchangePosition:
                SkillVisual_DrawLine(range, isRange, isFriendly, mouseAtNode);
                break;

            case Skill._Type.MoveInLine:
                GC.Del_TargetMap();
                SkillVisual_DrawStaightline(skillInUse.skill.range, false, mouseAtNode);
                break;

            case Skill._Type.AttackALine:
                GC.Del_TargetMap();
                SkillVisual_DrawStaightline(skillInUse.skill.range, true, mouseAtNode);
                break;

            case Skill._Type.TeleportToTargetAndAttackRoad:
                GC.Del_TargetMap();
                SkillVisual_DrawStaightline(skillInUse.skill.range, true, mouseAtNode);
                break;

            case Skill._Type.SummonFromDeath:
                SkillVisual_DrawLine_Summon(range, isRange, isFriendly, mouseAtNode);
                break;

            case Skill._Type.Summon:
                SkillVisual_DrawLine_Summon(range, isRange, isFriendly, mouseAtNode);
                break;

            case Skill._Type.BiteTheDeath:
                SkillVisual_DrawLine_Summon(range, isRange, isFriendly, mouseAtNode);
                break;


            case Skill._Type.SummonFromDeath_Type2:
                SkillVisual_DrawLine_RevivalPreveiw(range, isRange, mouseAtNode);
                break;
        }
        #endregion
    }

    private void SkillVisual_DrawStaightline(int range, bool isTeleport, PathNode mouseAtNode)
    {
       GameController.DeleteAllWalkingPath();

        GC.chargeRoad = GameFunctions.FindNodes_InOneLine(GC.selectedUnit.nodeAt, mouseAtNode, range, true, false, false,false);

        if (!isTeleport)
        {
            GC.chargeRoad = GameFunctions.FindNodes_Charge(GC.chargeRoad, GC.selectedUnit);
        }

        if (GC.chargeRoad != null)
        {
            GameFunctions.Gene_LightMap_Charge(GC.chargeRoad, GC, isTeleport);
        }
    }

    private void SkillVisual_DrawLine(int range, bool isRange, bool isFriendly, PathNode mouseAtNode)
    {
        if (mouseAtNode.unit != null)
        {
            if (!isRange)
            {
                int heightDiff = GC.selectedUnit.nodeAt.height - mouseAtNode.height;
                //Debug.Log(heightDiff);
                if (heightDiff > 3 || heightDiff < -3)
                {
                    return;
                }

            }

            if (GameFunctions.CheckAttackRange(GC.selectedUnit, mouseAtNode.unit, range, isRange, isFriendly, false))
            {

                if (isFriendly)
                {
                    green.SetActive(true);
                    Vector3 pos = mouseAtNode.unit.canvas.transform.GetChild(1).GetComponent<RectTransform>().transform.position;
                    guide_Skill.transform.position = pos;

                }

                else
                {
                    red.SetActive(true);
                    Vector3 pos = mouseAtNode.unit.canvas.transform.GetChild(1).GetComponent<RectTransform>().transform.position;
                    guide_Skill.transform.position = pos;
                }

            }
        }
    }

    private void SkillVisual_DrawLine_Summon(int range, bool isRange, bool isFriendly, PathNode mouseAtNode)
    {
        if (mouseAtNode.unit != null && mouseAtNode.unit.unitAttribute == Unit.UnitAttribute.corpse)
        {
            if (GameFunctions.CheckAttackRange(GC.selectedUnit, mouseAtNode.unit, range, isRange, isFriendly, false))
            {
                //curveRenderer_Purple.gameObject.SetActive(true);
                //GameFunctions.DrawCurve_BetweenTwoObject(selectedUnit.transform, mouseAtNode.unit.transform, curveRenderer_Purple, 0.5f);

                purple.SetActive(true);
                Vector3 pos = mouseAtNode.unit.canvas.transform.GetChild(1).GetComponent<RectTransform>().transform.position;
                guide_Skill.transform.position = pos;
            }
        }
    }

    private void SkillVisual_DrawLine_RevivalPreveiw(int range, bool isRange, PathNode mouseAtNode)
    {
        if (GameFunctions.CheckPathRange(GC.selectedUnit.nodeAt, mouseAtNode, range, true))
        {
            bool canSummon = true;
            if (mouseAtNode.unit != null)
            {
                canSummon = false;
                if (mouseAtNode.unit.unitAttribute == Unit.UnitAttribute.corpse) canSummon = true;
            }

            if (canSummon)
            {
                GC.revivalController.SwitchPreviewEnable(true);
            }

            else
            {
                GC.revivalController.SwitchPreviewEnable(false);
            }
        }
        else GC.revivalController.SwitchPreviewEnable(false);
    }


    private void FixedUpdate()
    {
        if (unit != null)
        {
            guide_Select.SetActive(true);
            Vector3 pos = unit.canvas.transform.GetChild(1).GetComponent<RectTransform>().transform.position;
            guide_Select.transform.position = pos;
        }
        else
        {
            guide_Select.SetActive(false);
        }
    }

    public void Update_GuideMove(Unit unit)
    {
        this.unit = unit;
    }

}
