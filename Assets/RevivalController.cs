using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RevivalController : MonoBehaviour
{
    private Skill currentSkill;

    [Header("Mouse Panel")]
    public TMP_Text fleshText;
    public GameObject ParentFolder;
    public GameObject Preveiw_Meatblock, Preveiw_Turret, Preview_HealTower, Preview_Skel_Greatsword, Preview_ZombieRat;

    public GameObject Prefab_Meatblock, Prefab_Turret, Prefab_HealTower, Prefab_Skel_Greatsword, Prefab_ZombieRat, Prefab_AOETurret;

    [Header("Stats")]
    public int fleshCost_Max;
    public int fleshCost;
    private GameController GC;


    private void Start()
    {
        GC = FindObjectOfType<GameController>();
    }

    public void InputPreveiw(Skill currentSkill)
    {
        this.currentSkill = currentSkill;
        if (Preveiw_Meatblock != null) Preveiw_Meatblock.SetActive(false);
        if (Preveiw_Turret != null) Preveiw_Turret.SetActive(false);
        if (Preview_HealTower != null) Preview_HealTower.SetActive(false);
        if (Preview_Skel_Greatsword != null) Preview_Skel_Greatsword.SetActive(false);
        if (Preview_ZombieRat != null) Preview_ZombieRat.SetActive(false);

        switch (currentSkill.revivalOption)
        {
            case Skill.RevivalOption.healTower:
                Preview_HealTower.SetActive(true);
                break;

            case Skill.RevivalOption.turret:
                Preveiw_Turret.SetActive(true);
                break;

            case Skill.RevivalOption.meatBlock:
                Preveiw_Meatblock.SetActive(true);
                break;

            case Skill.RevivalOption.zombieRat:
                Preview_ZombieRat.SetActive(true);
                break;

            case Skill.RevivalOption.skelSword:
                Preview_Skel_Greatsword.SetActive(true);
                break;

            case Skill.RevivalOption.aoeTurret:
                Preveiw_Turret.SetActive(true);
                break;
        }

        fleshCost_Max = currentSkill.Cost;
    }

    public GameObject GetPrefab(Skill currentSkill)
    {
        GameObject prefab = null;
        switch (currentSkill.revivalOption)
        {
            case Skill.RevivalOption.healTower:
                prefab = Prefab_HealTower;
                break;

            case Skill.RevivalOption.turret:
                prefab = Prefab_Turret;
                break;

            case Skill.RevivalOption.meatBlock:
                prefab = Prefab_Meatblock;
                break;

            case Skill.RevivalOption.skelSword:
                prefab = Prefab_Skel_Greatsword;
                break;

            case Skill.RevivalOption.zombieRat:
                prefab = Prefab_ZombieRat;
                break;

            case Skill.RevivalOption.aoeTurret:
                prefab = Prefab_AOETurret;
                break;
        }


        return prefab;
    }


    private void Update()
    {
        fleshCost = fleshCost_Max;

        if (GC.gameState == GameController._gameState.playerInRevivalPreview)
            if (GC.mouseAtNode != null)
                if (GC.mouseAtNode.unit != null)
                    if (GC.mouseAtNode.unit.unitAttribute == Unit.UnitAttribute.corpse)
                    {
                        fleshCost -= 1;
                    }
                        

        if (fleshCost < fleshCost_Max)
        {
            fleshText.text = "<color=#00FF01>" + fleshCost.ToString() + "</color>";
        }

        else
            fleshText.text = fleshCost.ToString();
    }
    public void QuitPreview()
    {
        this.currentSkill = null;
        if (Preveiw_Meatblock != null) Preveiw_Meatblock.SetActive(false);
        if (Preveiw_Turret != null) Preveiw_Turret.SetActive(false);
        if (Preview_HealTower != null) Preview_HealTower.SetActive(false);
        SwitchPreviewEnable(false);
    }

    public void SwitchPreviewEnable(bool enable)
    {
        if (enable) ParentFolder.SetActive(true);
        else ParentFolder.SetActive(false);
    }
 
    public  IEnumerator Skill_RevivalSummon(PathNode geneNode, Unit attacker, GameObject geneOb, Skill skillInUse, Unit.UnitTeam unitTeam, GameObject SpecialEffect)
    {
        if (geneNode.unit != null)
        {
            Destroy(geneNode.unit);
            Destroy(geneNode.unit.gameObject, skillInUse.timer_PerformAction + skillInUse.timer_AttackHit);
            geneNode.unit = null;
        }

        GameController GC = FindObjectOfType<GameController>();
        GC.isAttacking = true;
        GC.isMoving = false;
        GC.isAIThinking = false;


        FindObjectOfType<SFX_Controller>().InputVFX_Simple(13);

        GameController.currentActionUnit = attacker;
        attacker.InputAnimation(skillInUse.animTriggerType.ToString());

        yield return new WaitForSeconds(skillInUse.timer_PerformAction);

        StartCoroutine(GridDynamic_ShockWave(geneNode, "MinorMinor"));

        if (skillInUse.castSpecialEffect != null)
        {
            GameObject specialEffect = Instantiate(skillInUse.castSpecialEffect, UnitFunctions.GetUnitMiddlePoint(attacker), Quaternion.identity);
        }
        yield return new WaitForSeconds(skillInUse.timer_AttackHit);

        SkillFunctions.Skill_GeneUnit(geneNode, geneOb, unitTeam, SpecialEffect);
        yield return new WaitForSeconds(skillInUse.timer_WaitAfterAttack);

        if (!skillInUse.isInstant) attacker.UnitEnable(false);
        GC.Select_Unit(attacker.nodeAt, false);
        GC.isAttacking = false;
    }

    public static IEnumerator GridDynamic_ShockWave(PathNode startNode, string name)
    {
        //FireDown
        //"MinorMinor"
        List<PathNode> closedList = new List<PathNode>();
        List<PathNode> nodeInArea_2 = GameFunctions.FindNodes_ByDistance(startNode, 1, true);
        GridFallController.AddNodeToDynamicFolder(startNode, "Minor","");

        foreach (PathNode triggerednode in nodeInArea_2)
        {
            if (triggerednode != startNode)
            {
                GridFallController.AddNodeToDynamicFolder(triggerednode, "MinorMinor","");
            }
        }
        yield return new WaitForSeconds(0.1f);
    }
}
