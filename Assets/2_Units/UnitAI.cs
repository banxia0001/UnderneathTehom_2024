using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAI : MonoBehaviour
{
    private Unit unit;

    private int unit_BuffItself_CD = -10;
    public int unit_BuffItself_CD_Max = -10;
    private int unit_CastSpell_CD = -10;
    public int unit_CastSpell_CD_Max = -10;

    public enum AI_State {attack, shoot, healing, castSpell, flee, moveToCampfire, guardCampfire }
    public AI_State aiState;

    private enum AI_State_Action { attack_FindClosedUnit, attack_FindHighValueUnit, healNearbyUnit, castSpell_Harm, castSpell_Buffitself }

    public _SkillAI[] skill_harm;
    public _SkillAI[] skill_heal;
    public _SkillAI[] skill_buffself;

    public _SkillAI skillInUse;


    private bool isRange = false;
    [Range(0, 100)]
    public int rate_FindClosetUnit_AsTarget = 100;
    [Range(0, 100)]
    public int rate_FindArcher_AsTarget;

    [Range(0, 100)]
    public int rate_Heal;
    [Range(0, 100)]
    public int rate_moveToCampfire;
    [Range(0, 100)]
    public int rate_guardCampfire;
    [Range(0, 100)]
    public int rate_Flee;

    [Header("AI")]
    public Unit targetUnit;
    public Unit tuantUnit;
    public Unit nearbyFleshBlock;
    public List<PathNode> path;

    [Header("WAITING ATTACK")]
    public bool unitHoldingSkill;
    public _SkillAI skill_HoldFromLastTurn;
    public PathNode chargePath_HoldFromLastTurn;
    public GameObject VisualGuideFolder;

    void Start()
    {
        //if player control this unit, delete AI
        unit = gameObject.GetComponent<Unit>();
        if (unit.unitTeam == Unit.UnitTeam.playerTeam) { Destroy(this); return; }
        if (unit.data.damage.range > 1) isRange = true;
        unitHoldingSkill = false;
        nearbyFleshBlock = null;
    }

    //INPUT HERE, START THINKING.
    public void AI_Thinking()
    {
        if (unit.GC.isAttacking|| unit.GC.isMoving|| unit.GC.isAIThinking) return;
        Debug.Log(unit.data.Name + ":Thinking");

        if (unit.unitSpecialState == Unit.UnitSpecialState.boneFireTower)
        {
            unit.GC.isAIThinking = true;
            EnemyBoneFire boneFire = this.unit.gameObject.GetComponent<EnemyBoneFire>();
            boneFire.CheckNewTurn(unit.GC);
            return;
        }

        if (unit.data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._17_BOSS)
        {
            unit.GC.isAIThinking = true;
            BossAI bossAI = this.unit.gameObject.GetComponent<BossAI>();
            bossAI.CheckNewTurn();
            return;
        }


        unit.GC.isAIThinking = true;
        skillInUse = null;
        targetUnit = null;
        tuantUnit = null;
        chargeNode = null;
        unit_CastSpell_CD--;
        unit_BuffItself_CD--;

        isRange = false;
        if (unit.currentData.damage.range > 1) isRange = true;
        if (GameController.playerList.Count == 0) return;
        Unit closedUnit = GameFunctions.FindClosestUnit_By_Grid(0, unit);


        //USESKILL FROM LAST ROUND
        if (unitHoldingSkill)
        {
            if (VisualGuideFolder != null) Destroy(VisualGuideFolder);
            unitHoldingSkill = false;
            skillInUse = skill_HoldFromLastTurn;
            skill_HoldFromLastTurn = null;

            if (skillInUse.skillSpecialFunction == _SkillAI._SkillSpeicalFunction.Hold_n_Charge)
            {
                

                Debug.Log("Here");

                chargeNode = GameFunctions.FindNodes_InOneLine(unit.nodeAt, chargePath_HoldFromLastTurn, skillInUse.skill.range, true, false, false, false);

                if (chargeNode != null)
                {
                    unit.popTextString.Add(new textInformation(skillInUse.skill.name, skillInUse.skill.skillSprite));
                    StartCoroutine(Skill_Charge(chargeNode, this.unit, skillInUse.skill, true));
                }
                else
                {
                    if (VisualGuideFolder != null) Destroy(VisualGuideFolder);
                    Debug.Log("Here2");
                    StartCoroutine(AI_Defend());
                }
                return;
            }

            else if (skillInUse.skillSpecialFunction == _SkillAI._SkillSpeicalFunction.Hold_n_Cast)
            {
                List<Unit> groupedlUnitlist = GameController.enemyList;
                List<Unit> unitResult = AIFunctions.FindClosestUnitGroup_Range_AI(5, unit, groupedlUnitlist);
                targetUnit = AIFunctions.FindUnit_ByLowHP_AI(unitResult, unit);
                StartCoroutine(SkillFunctions.Skill_Buff(unit, targetUnit, skillInUse.skill, false));

                return;
            } 
        }

        //TUANT
        foreach (_Buff buff in unit.buffList)
        {
            if (buff.buff.buffType == Buff._buffType.tuant)
                if (buff.tuantTarget != null)
                    tuantUnit = buff.tuantTarget;
        }
        


        if (tuantUnit != null)
        {
            Debug.Log("Taunt");
            aiState = AI_State.attack; if (isRange) aiState = AI_State.shoot;
            targetUnit = tuantUnit;
            if (isRange) MoveCloserToTarget_Range();
            else MoveCloserToTarget_Melee();
            return;
        }


        else if (GameFunctions.CalculateDis_WithoutY(unit.nodeAt, closedUnit.nodeAt) < 2 * 1.733f)
        {
            if (Random.Range(0, 100) < rate_Flee)
            {
                aiState = AI_State.flee;
                PathNode moveToNode = GameFunctions.FindFarestNode(closedUnit.nodeAt, GameFunctions.FindNodes_ByDistance(unit.nodeAt, 1, false), true);
                path = PathFinding.FindPath(unit.nodeAt, moveToNode, unit, null, false, false, 5, false);
                StartCoroutine(AI_MoveWithTimeDelay());
                return;
            }
        }

        //if unit was try to move to campfire, igore this.
        if (Random.Range(0, 100) < rate_moveToCampfire)
        {
            List<CampFire> camp = new List<CampFire>();
            foreach (CampFire fire in FindObjectsOfType<CampFire>())
            {
                if (fire.campfireTeam != unit.unitTeam){camp.Add(fire);}
            }
            if (camp != null && camp.Count > 0)
            {
                CampFire closestCamp = GameFunctions.FindClosestCampfire(this.unit.nodeAt, camp).GetComponent<CampFire>();

                Unit target = null;
                foreach (PathNode path in GameFunctions.FindNodes_ByDistance(closestCamp.nodeAt, 1, false))
                {
                    if (path.unit != null && path.unit.unitTeam != unit.unitTeam && path.unit.unitAttribute == Unit.UnitAttribute.alive) {target = path.unit; break;}
                }

                if (target == null)
                {
                    aiState = AI_State.moveToCampfire;
                    PathNode moveToNode = closestCamp.nodeAt;
                    path = PathFinding.FindPath(unit.nodeAt, moveToNode, unit, null, false, false, 0, true);
                    StartCoroutine(AI_MoveWithTimeDelay());
                }
                else
                {
                    targetUnit = target; 
                    if (!isRange) MoveCloserToTarget_Melee();
                    else MoveCloserToTarget_Range();
                }

                return;
            }
        }


        if (Random.Range(0, 100) < rate_guardCampfire)
        {
            Unit closestUnit = GameFunctions.FindClosestUnit_By_Grid(0, unit);
            List<CampFire> camp = new List<CampFire>();
            foreach (CampFire fire in FindObjectsOfType<CampFire>())
            {
                if (fire.campfireTeam == unit.unitTeam)
                {
                    camp.Add(fire);
                }
            }
            if (camp != null && camp.Count > 0 && closestUnit != null)
            {
                CampFire closestCamp = GameFunctions.FindClosestCampfire(closestUnit.nodeAt, camp).GetComponent<CampFire>();
                aiState = AI_State.guardCampfire;
                PathNode moveToNode = closestCamp.nodeAt;
                path = PathFinding.FindPath(unit.nodeAt, moveToNode, unit, null, false, false, 0, true);
                StartCoroutine(AI_MoveWithTimeDelay());
                return;
            }
        }

        int sum = rate_FindClosetUnit_AsTarget + rate_FindArcher_AsTarget + rate_Heal;
        int range = Random.Range(0, sum);

        AI_State_Action aiAction = AI_State_Action.attack_FindClosedUnit;

        if (range < rate_FindClosetUnit_AsTarget)aiAction = AI_State_Action.attack_FindClosedUnit;
        else if (range < rate_FindClosetUnit_AsTarget + rate_FindArcher_AsTarget) aiAction = AI_State_Action.attack_FindHighValueUnit;
        else if (range < rate_FindClosetUnit_AsTarget + rate_FindArcher_AsTarget + rate_Heal) aiAction = AI_State_Action.healNearbyUnit;


        if (unit_BuffItself_CD <= 0 && unit_BuffItself_CD_Max > 0)
        {
            unit_BuffItself_CD = unit_BuffItself_CD_Max;
            aiAction = AI_State_Action.castSpell_Buffitself;
        }

        if (unit_CastSpell_CD <= 0 && unit_CastSpell_CD_Max > 0)
        {
            unit_CastSpell_CD = unit_CastSpell_CD_Max;
            aiAction = AI_State_Action.castSpell_Harm;
        }

        //FIND CLOSEST ENEMY
        if (aiAction == AI_State_Action.attack_FindClosedUnit)
        {
            aiState = AI_State.attack; if (isRange) aiState = AI_State.shoot;
            FindTargetAndDecideMove(AI_State_Action.attack_FindClosedUnit);
            return;
        }


        //FIND WEAK ENEMY
        else if (aiAction == AI_State_Action.attack_FindHighValueUnit)
        {
            aiState = AI_State.attack; if (isRange) aiState = AI_State.shoot;
            FindTargetAndDecideMove(AI_State_Action.attack_FindHighValueUnit);
            return;
        }

        //HEAL FRIENDLY UNIT AS TARGET
        else if (aiAction == AI_State_Action.healNearbyUnit)
        {
            if (skill_heal.Length == 0)
            {
                FindTargetAndDecideMove(0);
                return;
            }

            aiState = AI_State.healing;
            foreach (_SkillAI myskill in skill_heal)
            {
                if (myskill.skillPriority == _SkillAI._SkillPriority.Heal) skillInUse = myskill;
            }  

            if (skillInUse == null) { FindTargetAndDecideMove(0); rate_Heal = 0; }
            else
            {
                if (skillInUse.skillSpecialFunction == _SkillAI._SkillSpeicalFunction.Hold_n_Cast)
                {
                    unit.popTextString.Add(new textInformation("Hold", skillInUse.skill.skillSprite));
                    StartCoroutine(Skill_CastBuff_Hold());
                    return;
                }

                else
                {
                    List<Unit> groupedlUnitlist = GameController.enemyList;
                    List<Unit> unitResult = AIFunctions.FindClosestUnitGroup_Range_AI(5, unit, groupedlUnitlist);
                    targetUnit = AIFunctions.FindUnit_ByLowHP_AI(unitResult, unit);
                    if (targetUnit == this.unit) StartCoroutine(SkillFunctions.Skill_Buff(unit, targetUnit, skillInUse.skill, false));
                    else FindTargetAndDecideMove(0);
                }
            }
            return;
        }




        else if (aiAction == AI_State_Action.castSpell_Buffitself)
        {
            if (skill_buffself.Length == 0)
            {
                FindTargetAndDecideMove(0);
                return;
            }

            aiState = AI_State.castSpell;
            foreach (_SkillAI myskill in skill_buffself)
            {
                if (myskill.skillPriority == _SkillAI._SkillPriority.BuffSelf) skillInUse = myskill;
            }
            if (skillInUse == null) { FindTargetAndDecideMove(0);}
            else
            {
                targetUnit = this.unit;
                StartCoroutine(SkillFunctions.Skill_Buff(unit, targetUnit, skillInUse.skill, false));
            }
            return;
        }


        //CAST SPELLS
        else if (aiAction == AI_State_Action.castSpell_Harm)
        {
            aiState = AI_State.castSpell;

            if (skill_harm.Length == 0)
            {
                FindTargetAndDecideMove(0);
                return;
            }

            _SkillAI skillHold = skill_harm[Random.Range(0, skill_harm.Length)];
            skillInUse = skillHold;

            List<Unit> groupedlUnitlist = new List<Unit>();
            List<Unit> unitResult = new List<Unit>();

            switch (skillHold.skillPriority)
            {
                case _SkillAI._SkillPriority.Summon:
                    AI_Summoning();
                    break;

                case _SkillAI._SkillPriority.AttackHighArmor_Range:

                    if (nearbyFleshBlock != null) targetUnit = nearbyFleshBlock;
                    else
                    {
                        groupedlUnitlist = GameController.playerList;
                        unitResult = AIFunctions.FindClosestUnitGroup_Range_AI(5, unit, groupedlUnitlist);
                        targetUnit = AIFunctions.FindUnit_ByHighArmor_AI(unitResult, unit);
                    }
                    MoveCloserToTarget_Range();
                    break;

                case _SkillAI._SkillPriority.AttackHighArmor_Melee:

                    if (nearbyFleshBlock != null) targetUnit = nearbyFleshBlock;
                    else
                    {
                        groupedlUnitlist = GameController.playerList;
                        unitResult = AIFunctions.FindClosestUnitGroup_Range_AI(3, unit, groupedlUnitlist);
                        targetUnit = AIFunctions.FindUnit_ByHighArmor_AI(unitResult, unit);
                    }
                    MoveCloserToTarget_Melee();
                    break;

                case _SkillAI._SkillPriority.AttackClosest_Melee:

                    if (nearbyFleshBlock != null) targetUnit = nearbyFleshBlock;
                    else
                    {
                        groupedlUnitlist = GameController.playerList;
                        unitResult = AIFunctions.FindClosestUnitGroup_Range_AI(1, unit, groupedlUnitlist);
                        targetUnit = unitResult[0];
                    }
                    MoveCloserToTarget_Melee();
                    break;

                case _SkillAI._SkillPriority.TargetAllUnit:
                    StartCoroutine(SkillFunctions.Skill_AttackAllPlayer(unit, skillInUse.skill));
                    break;
            }
        }
    }


    private void FindTargetAndDecideMove(AI_State_Action state)
    {
        targetUnit = null;
        aiState = AI_State.attack; 
        if (isRange) aiState = AI_State.shoot;

        List<Unit> groupedlUnitlist = GameController.playerList;
        List<Unit> unitResult = new List<Unit>();

        if (state == AI_State_Action.attack_FindClosedUnit)
        {
            if (nearbyFleshBlock != null) targetUnit = nearbyFleshBlock;
            else
            {
                //WE GET ALL UNITS, AND FIND THE TOP 1 NEAREST
                if (isRange) unitResult = AIFunctions.FindClosestUnitGroup_Range_AI(1, unit, groupedlUnitlist);
                else unitResult = AIFunctions.FindClosestUnitGroup_Melee_AI(1, unit, groupedlUnitlist);
                if (unitResult != null) targetUnit = unitResult[0];
            }
        }

        else if (state == AI_State_Action.attack_FindHighValueUnit)
        {
            if (nearbyFleshBlock != null) targetUnit = nearbyFleshBlock;

            else
            {
                //WE GET ALL UNITS, AND FIND THE TOP 4 NEAREST, FIND WHICH ONE HAVE THE HIGHEST VALUE, AND ATTACK.
                unitResult = AIFunctions.FindClosestUnitGroup_Range_AI(4, unit, groupedlUnitlist);
                if (unitResult != null) targetUnit = AIFunctions.FindUnit_ByValue_AI(unitResult, unit);
            }
        }

        if (targetUnit == null) { StartCoroutine(AI_Defend()); return; }

        else
        {
            if (isRange) MoveCloserToTarget_Range();
            else MoveCloserToTarget_Melee();
        }
    }


    private void MoveCloserToTarget_Range()
    {
        if (path != null)
            path.Clear();

        List<PathNode> moveToListWithUnit = PathFinding.FindPath(unit.nodeAt, targetUnit.nodeAt, unit, targetUnit, false, false,0,false);
        List<PathNode> moveToListWithoutUnit = PathFinding.FindPath(unit.nodeAt, targetUnit.nodeAt, unit, targetUnit, true, false,0,false);

        bool tryToGetCloserRatherThenFindAWayToReach = false;   

        if (moveToListWithoutUnit != null)
        {
            if (moveToListWithUnit != null)
            {
                if (moveToListWithoutUnit.Count < moveToListWithUnit.Count - 1)
                {
                    tryToGetCloserRatherThenFindAWayToReach = true;
                }
                else tryToGetCloserRatherThenFindAWayToReach = false;
            }
            else tryToGetCloserRatherThenFindAWayToReach = true;
        }
        else tryToGetCloserRatherThenFindAWayToReach = true;

        if (tryToGetCloserRatherThenFindAWayToReach == false)
        {
            path = moveToListWithUnit;
        }

        else
        {
            path = moveToListWithoutUnit;
        }

        if (path != null)
            StartCoroutine(AI_MoveWithTimeDelay());

        else StartCoroutine(AI_Defend());
    }




    //单位试图靠近另一个单位，如果有路则走，没有路则试图接近。
    private void MoveCloserToTarget_Melee()
    {
        if (path != null)
            path.Clear();

        if (unit.data.unitSize == UnitData.Unit_Size.size1)
        {
            path = AIFunctions.FindClosestPathToTargetNode_Size1_AI(unit, targetUnit, targetUnit.nodeAt);
        }

        else if (unit.data.unitSize == UnitData.Unit_Size.size2_OnLeftNode || unit.data.unitSize == UnitData.Unit_Size.size2_OnRightNode)
        {
            path = AIFunctions.FindClosestPathToTargetNode_Size2_AI(unit, targetUnit, targetUnit.nodeAt);
        }

        else if (unit.data.unitSize == UnitData.Unit_Size.size3_OnLeftNode || unit.data.unitSize == UnitData.Unit_Size.size3_OnRightNode || unit.data.unitSize == UnitData.Unit_Size.size3_OnTopNode)
        {
            path = AIFunctions.FindClosestPathToTargetNode_Size3_AI(unit, targetUnit, targetUnit.nodeAt);
        }

        if (path != null && path.Count > 0)
        {
            StartCoroutine(AI_MoveWithTimeDelay());
            return;
        }

        else
        {
            PathNode mayMoveNode = closestPathToTarget();
            if (mayMoveNode != null)
            {
                path = PathFinding.FindPath(unit.nodeAt, mayMoveNode, unit, null, false, false,0,false);

                if (path != null)
                    StartCoroutine(AI_MoveWithTimeDelay());
                else 
                    StartCoroutine(AI_Defend());
            }
            else 
                StartCoroutine(AI_Defend());
        }
    }









    private void AI_Summoning()
    {
        List<PathNode> nearbyNode = GameFunctions.FindNodes_ByDistance(unit.nodeAt, 1, false);
        PathNode geneNode = null;

        for (int i = 1; i < nearbyNode.Count; i++)
        {
            if (nearbyNode[i].unit == null)
            {
                float heightDiff = unit.transform.position.y - nearbyNode[i].transform.position.y;
                if (heightDiff > .75f || heightDiff < -.75f) continue;
                if (nearbyNode[i].isBlocked)continue;
                if (nearbyNode[i].campFire != null)continue;

                geneNode = nearbyNode[i];
                break;
            }
        }

        if (geneNode != null)
        {
            GameObject GeneOB = skillInUse.skill.SommonBeing[Random.Range(0, skillInUse.skill.SommonBeing.Length)];
            StartCoroutine(SkillFunctions.Skill_Summoning(geneNode, this.unit, GeneOB, skillInUse.skill, Unit.UnitTeam.enemyTeam, skillInUse.skill.hitSpecialEffect));
        }

        else StartCoroutine(AI_Defend());
    }

    private IEnumerator AI_Defend()
    {
        if (unit.GC.isAttacking == false)
        {
            unit.GC.isAttacking = true;
            unit.InputAnimation("use");
            yield return new WaitForSeconds(0.2f);

            unit.HealthChange(1,0,"Heal");
            unit.UnitEnable(false);

            unit.GC.isAttacking = false;
            unit.GC.isAIThinking = false;
        }

        else yield return null;
    }

    public IEnumerator AI_MoveWithTimeDelay()
    {
        unit.GC.isMoving = true;
        unit.GC.isAIThinking = true;
        Debug.Log("UnitDecideMoving");
        Skill holdSkill = null;
        if (skillInUse != null) holdSkill = skillInUse.skill;

        if(unitHoldingSkill)
        { 
            unitHoldingSkill = false;
            skill_HoldFromLastTurn = null;
            if (VisualGuideFolder != null) Destroy(VisualGuideFolder);
        }

        if (path != null && path.Count > 0)
        {
            int range = unit.currentData.damage.range;
            bool isRange = false;
            if (range > 1) isRange = true;

            bool canMoveAgain = true;

            for (int i = 1; i < path.Count; i++)
            {
                yield return new WaitForSeconds(0.3f);
                if (unit.GC.isMoving == false) break;
                //if move and attack
                switch (aiState)
                {
                    case AI_State.attack:
                        Unit moveToUnit = path[i].unit;
                        if (moveToUnit != null && moveToUnit != unit) //attack nearby
                        {
                            Debug.Log("StopMoving");
                            if (moveToUnit.unitTeam != unit.unitTeam)  //if the enemy in the way
                            {
                                yield return new WaitForSeconds(0.15f);
                            
                                StartCoroutine(GameFunctions.Attack(unit, moveToUnit, unit.currentData.damage, true, holdSkill, false));
                            }
                            unit.GC.isMoving = false;
                        }
                        break;

                    case AI_State.shoot:
                        if (GameFunctions.CheckAttackRange(unit, targetUnit, range, true, false, true) == true)
                        {
                            yield return new WaitForSeconds(0.12f);
                            Debug.Log("Unit Attack");
                            StartCoroutine(GameFunctions.Attack(unit, targetUnit, unit.currentData.damage, false, holdSkill, false));
                            unit.GC.isMoving = false;
                        }
                        break;

                    case AI_State.healing:
                        range =holdSkill.range; isRange = false; if (range > 1) isRange = true;
                        if (GameFunctions.CheckAttackRange(unit, targetUnit, range, isRange, true, true) == true)
                        {
                            yield return new WaitForSeconds(0.12f);
                            StartCoroutine(SkillFunctions.Skill_Buff(unit, targetUnit, holdSkill, false));
                            unit.GC.isMoving = false;
                        }
                        break;

                    case AI_State.castSpell:
                        canMoveAgain = UseSkill_Choice();
                        break;
                }

                if (unit.GC.isMoving && canMoveAgain)
                {
                    bool canMove = unit.UnitPosition_CanMove(path[i], path[i].MOVE_COST, true, 1f,true);
                    if (!canMove) { Debug.Log("MovingStop"); break; }
                    unit.GC.CheckCampFire();
                }

                else break;
            }
        }
       

        yield return new WaitForSeconds(0.1f);

        //if enemy moved to end and still not attack yet, will choose a random target
        //if enemy moved to end and still not attack yet, will choose a random target
        //if enemy moved to end and still not attack yet, will choose a random target
        if (unit.isActive == true && unit.GC.isAttacking == false)
        {
            //Debug.Log("!!!");
            Unit thisTurnAttackedUnit = null;

            //FIND A UNIT NEAR TO ATTACK
            foreach (PathNode path in GameFunctions.FindNodes_ByDistance(unit.nodeAt, unit.currentData.damage.range, isRange))
            {
                if (path.unit != null && path.unit.unitTeam != unit.unitTeam && path.unit.unitAttribute == Unit.UnitAttribute.alive)
                { thisTurnAttackedUnit = path.unit; break; }
            }

            if (thisTurnAttackedUnit != null)
            {
                Debug.Log("Unit Attack");
                unit.GC.isAttacking = true;
                unit.GC.isMoving = false;
                yield return new WaitForSeconds(0.15f);
                StartCoroutine(GameFunctions.Attack(unit, thisTurnAttackedUnit, unit.currentData.damage, false, holdSkill, false));
            }

            //IF NO UNIT NEAR, THEN, DESTROY A NEARBY CORPSE

            else if(unit.data.damage.range == 1)
            {
                foreach (PathNode path in GameFunctions.FindNodes_ByDistance(unit.nodeAt, unit.currentData.damage.range, isRange))
                {
                    if (path.unit != null && path.unit.unitAttribute != Unit.UnitAttribute.alive)
                    { thisTurnAttackedUnit = path.unit; break; }
                }

                if (thisTurnAttackedUnit != null)
                {
                    Debug.Log("Unit Attack");
                    yield return new WaitForSeconds(0.15f);
                    StartCoroutine(GameFunctions.Attack(unit, thisTurnAttackedUnit, unit.currentData.damage, false, holdSkill, false));
                    unit.GC.isMoving = false; 
                }

                //STILL NO? THEN REST.
                else { unit.GC.isMoving = false; unit.isActive = false; }
            }
            else { unit.GC.isMoving = false; unit.isActive = false; }
        }
        else { unit.GC.isMoving = false; unit.isActive = false; }


        unit.GC.isAIThinking = false;
    }


    private bool UseSkill_Choice()
    {
        if(skillInUse == null) unit.isActive = false;
        int range = 1;

        Skill holdSkill = null;
        if (skillInUse != null) holdSkill = skillInUse.skill;

        switch (skillInUse.skill.type)
        {
            case Skill._Type.AttackALine:

                chargeNode = AIFunctions.CheckUnit_CanAttackOneRoad_AI(this.unit, targetUnit, holdSkill);
               
                if (chargeNode != null)
                {
                    foreach (PathNode path in chargeNode)
                    {
                        if (path.unit != null)
                        {
                            if (path.unit.unitTeam != unit.unitTeam)
                            {
                                if (skillInUse.skillSpecialFunction == _SkillAI._SkillSpeicalFunction.Hold_n_Charge)
                                {
                                    unitHoldingSkill = true;
                                    unit.popTextString.Add(new textInformation(holdSkill.name, holdSkill.skillSprite));

                                
                                    //do smting.
                                    //do smt animation
                                    this.unit.UnitEnable(false);
                                    return false;
                                }

                                else if (skillInUse.skillSpecialFunction == _SkillAI._SkillSpeicalFunction.Normal)
                                {
                                    unit.popTextString.Add(new textInformation(holdSkill.name, holdSkill.skillSprite));
                                    StartCoroutine(SkillFunctions.Skill_AttackALine(this.unit, targetUnit.nodeAt, this.unit.currentData.damage, holdSkill));
                                    unit.GC.isMoving = false;
                                    return false;
                                }
                            }
                        }
                    }
                }
               
                break;

            case Skill._Type.TargetToAttack:
                range = skillInUse.skill.range; isRange = false; if (range > 1) isRange = true;
                if (GameFunctions.CheckAttackRange(unit, targetUnit, range, isRange, false, true) == true)
                {
                    bool canFightBack = true; if (isRange) canFightBack = false;
                    unit.popTextString.Add(new textInformation(skillInUse.skill.name, skillInUse.skill.skillSprite));
                    StartCoroutine(GameFunctions.Attack(unit, targetUnit, unit.currentData.damage, canFightBack, skillInUse.skill, false));
                    unit.GC.isMoving = false;
                    return false;
                }
                break;

            case Skill._Type.Reload:
                unit.isActive = false;
                break;

            case Skill._Type.MoveInLine:
                chargeNode = AIFunctions.CheckUnit_CanCharge_AI(this.unit, targetUnit, skillInUse.skill);

                if (chargeNode != null)
                {
                    if (skillInUse.skillSpecialFunction == _SkillAI._SkillSpeicalFunction.Hold_n_Charge)
                    {
                        unit.popTextString.Add(new textInformation("Hold", skillInUse.skill.skillSprite));
                        StartCoroutine(Skill_Charge_Hold());
                        return false;
                    }

                    else if (skillInUse.skillSpecialFunction == _SkillAI._SkillSpeicalFunction.Normal)
                    {
                        unit.popTextString.Add(new textInformation(skillInUse.skill.name, skillInUse.skill.skillSprite));
                        StartCoroutine(Skill_Charge(chargeNode, this.unit, skillInUse.skill,false));
                        return false;
                    }
                }
                break;

            case Skill._Type.Summon:
                unit.isActive = false;
               
                break;

            case Skill._Type.SummonFromDeath:
                unit.isActive = false;
            
                break;

            case Skill._Type.TeleportToTargetAndAttackRoad:
                unit.isActive = false;

                break;

            case Skill._Type.TargetToBuff:

                if (skillInUse.skillSpecialFunction == _SkillAI._SkillSpeicalFunction.Hold_n_Cast)
                {
                    unit.popTextString.Add(new textInformation("Hold", skillInUse.skill.skillSprite));
                    StartCoroutine(Skill_CastBuff_Hold());
                    return false;
                }
                break;
        }

        return true;
    }


    public List<PathNode> chargeNode;
    private PathNode closestPathToTarget()
    {
        if (targetUnit == null) { return null; }
        PathNode moveToNode = null;
        List<PathNode> canMoveList = new List<PathNode>();
        canMoveList = GameFunctions.FindNodes_ByMoveableArea(unit.currentData.movePointNow, unit.nodeAt, unit);

        if (canMoveList.Count != 0)
        {
            moveToNode = GameFunctions.FindClosestNode(targetUnit.nodeAt, canMoveList);
        }

        return moveToNode;
    }
















    private void DrawLine_AI_ChargeVisualGuide(List<PathNode> chargeNode)
    {
        if (VisualGuideFolder != null) Destroy(VisualGuideFolder);
        if (chargeNode.Count < 1) return;

        GameObject MyTransform = Instantiate(Resources.Load<GameObject>("GridPrefab/Folder"), new Vector3(0, 0, 0), Quaternion.identity);
        MyTransform.name = "AI Visual Icon Folder";
        VisualGuideFolder = MyTransform;
        GameObject arrow = Instantiate(Resources.Load<GameObject>("GridPrefab/chargeArrow_V2"), new Vector3(0, 0, 0), Quaternion.identity);
        arrow.transform.parent = VisualGuideFolder.transform;
        arrow.GetComponent<GuideLineArrow_V2>().DrawLine_AI_ChargeVisualGuide(chargeNode);
    }

    private IEnumerator Skill_Charge_Hold()
    {
        unit.GC.isAttacking = true;
        unit.GC.isAIThinking = true;
        unitHoldingSkill = true;

        unit.InputAnimation_Single("charge");
        unit.AddAnimation("chargehold");

        skill_HoldFromLastTurn = skillInUse;
        chargePath_HoldFromLastTurn = chargeNode[chargeNode.Count - 1];
        DrawLine_AI_ChargeVisualGuide(chargeNode);
        UnitFunctions.Flip_Simple(this.unit.transform.position.x, chargePath_HoldFromLastTurn.transform.position.x, this.unit.TranformOffsetFolder);

        yield return new WaitForSeconds(1f);
        this.unit.UnitEnable(false);
        unit.GC.isAIThinking = false;
        unit.GC.isAttacking = false;
    }

    private IEnumerator Skill_CastBuff_Hold()
    {
        unit.GC.isAttacking = true;
        unit.GC.isAIThinking = true;
        unitHoldingSkill = true;

        unit.InputAnimation_Single("castStart");
        unit.AddAnimation("castLoop");

        skill_HoldFromLastTurn = skillInUse;
        unit.canNormalFlip = false;
     
        yield return new WaitForSeconds(1f);
        this.unit.UnitEnable(false);
        unit.GC.isAIThinking = false;
        unit.GC.isAttacking = false;
    }

    private IEnumerator Skill_Charge(List<PathNode> gotoNode, Unit unit_In_Moving, Skill skill, bool activeToMove)
    {
        unit.GC.isMoving = true;
        unit.GC.isAttacking = true;
        unit.GC.isAIThinking = true;
        GameController.currentActionUnit = unit_In_Moving;

        if (gotoNode != null && gotoNode.Count > 1)
        {
            yield return new WaitForSeconds(0.3f);
            UnitFunctions.Flip(unit_In_Moving.transform.position.x, gotoNode[1].transform.position.x, unit_In_Moving);
            unit_In_Moving.InputAnimation(skill.animTriggerType.ToString());

            unit.InputAnimation_Single("chargeattack1");
            unit.AddAnimation("chargeattack2");
            yield return new WaitForSeconds(0.2f);
            yield return new WaitForSeconds(skill.timer_PerformAction);

            if (skill.castSpecialEffect != null)
            {
                GameObject specialEffect = Instantiate(skill.castSpecialEffect, unit_In_Moving.transform.position + new Vector3(0, 0, -.1f), Quaternion.identity);
            }

            List<PathNode> myNode = gotoNode;
            bool attacked = false;
            for (int i = 1; i < myNode.Count; i++)
            {
                if (myNode[i].unit != null)
                {
                    unit.InputAnimation("chargeattack3");
                    attacked = true;
                    #region FOLDERS
                    Unit unitOnWay = myNode[i].unit;
                    List<PathNode> knockBackNode1 = new List<PathNode>();
                    if (unitOnWay.unitTeam == unit_In_Moving.unitTeam) break;

                    else
                    {
                        yield return new WaitForSeconds(0.175f);
                        if (skill.hitSpecialEffect != null)
                        {
                            GameObject specialEffect = Instantiate(skill.hitSpecialEffect, UnitFunctions.GetUnitMiddlePoint(unitOnWay), Quaternion.identity);
                        }

                        SkillFunctions.Skill_ChargeDamage(unit_In_Moving, unitOnWay, unit_In_Moving.currentData.damage, false, skill, i);
                        yield return new WaitForSeconds(0.01f);
                        bool canMove = unit_In_Moving.UnitPosition_CanMove(myNode[i], 0, true, 1.33f, false);
                        break;
                    }
                    #endregion
                }
                else
                {
                    bool canMove = unit_In_Moving.UnitPosition_CanMove(myNode[i], 0, true, 1.33f, false);
                }
            }

            if (!attacked)
            {
                unit.InputAnimation("chargeattack3");
            }
            if (VisualGuideFolder != null) Destroy(VisualGuideFolder);
        }



        yield return new WaitForSeconds(skill.timer_WaitAfterAttack);
        yield return new WaitForSeconds(1f);
        unit.GC.isMoving = false;
        unit.GC.isAttacking = false;
        unit.GC.isAIThinking = false;
        if (activeToMove)
        {
            unit_In_Moving.currentData.movePointNow = 0;
            unit_In_Moving.GetComponent<UnitAI>().AI_Thinking();
        }
        else
        {
            unit_In_Moving.UnitEnable(false);
        }
    }

    public void CancelUnitHoldAction()
    {
        if (VisualGuideFolder != null) Destroy(VisualGuideFolder);
        this.unitHoldingSkill = false;
        this.skill_HoldFromLastTurn = null;
    }

}
