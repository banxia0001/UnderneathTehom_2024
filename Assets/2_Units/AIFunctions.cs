using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFunctions : MonoBehaviour
{
    ///////////////////////////////////////////////////
    ///Find_Path ///Find_Path ///Find_Path ///Find_Path
    ///Find_Path ///Find_Path ///Find_Path ///Find_Path
    ///Find_Path ///Find_Path ///Find_Path ///Find_Path
    ///Find_Path ///Find_Path ///Find_Path ///Find_Path
    /// ///////////////////////////////////////////////
    public static List<PathNode> FindClosestPathToTargetNode_Size3_AI(Unit unit, Unit targetUnit, PathNode tartgetNode)
    {
        List<PathNode> path = null;
        GridManager grid = GameObject.FindGameObjectWithTag("GameController").GetComponent<GridManager>();
        unit.data.unitSize = UnitData.Unit_Size.size3_OnLeftNode;
        List<PathNode> moveToList_size3_OnLeftNode = PathFinding.FindPath(unit.nodeAt, tartgetNode, unit, targetUnit, false, false, 0, false);

        unit.data.unitSize = UnitData.Unit_Size.size3_OnRightNode;
        List<PathNode> moveToList_size3_OnRightNode = PathFinding.FindPath(unit.nodeAt_Right, tartgetNode, unit, targetUnit, false, true, 0, false);

        unit.data.unitSize = UnitData.Unit_Size.size3_OnTopNode;
        List<PathNode> moveToList_size3_OnTopNode = PathFinding.FindPath(unit.nodeAt_Top, tartgetNode, unit, targetUnit, false, true, 0, false);

        int leftCost = 1000;
        if (moveToList_size3_OnLeftNode != null) { leftCost = moveToList_size3_OnLeftNode.Count; unit.data.unitSize = UnitData.Unit_Size.size3_OnLeftNode; path = moveToList_size3_OnLeftNode; }

        int rightCost = 1000;
        if (moveToList_size3_OnRightNode != null) { rightCost = moveToList_size3_OnRightNode.Count; unit.data.unitSize = UnitData.Unit_Size.size3_OnRightNode; path = moveToList_size3_OnRightNode; }

        int topCost = 1000;
        if (moveToList_size3_OnTopNode != null) { topCost = moveToList_size3_OnTopNode.Count; unit.data.unitSize = UnitData.Unit_Size.size3_OnTopNode; path = moveToList_size3_OnTopNode; }

        //Go Lefts
        if (leftCost < topCost && leftCost < rightCost)
        {
            if (leftCost != 1000)
            {
                Debug.Log("Unit move by LeftPart" + "||LeftSideCost" + leftCost + "||RightSideCost" + rightCost);
                unit.data.unitSize = UnitData.Unit_Size.size3_OnLeftNode; path = moveToList_size3_OnLeftNode;
            }
        }
        //Go right
        if (rightCost < topCost && rightCost < leftCost)
        {
            if (leftCost != 1000)
            {
                Debug.Log("Unit move by LeftPart" + "||LeftSideCost" + leftCost + "||RightSideCost" + rightCost);
                unit.data.unitSize = UnitData.Unit_Size.size3_OnRightNode; path = moveToList_size3_OnRightNode;
            }
        }
        //Go top
        if (topCost < rightCost && topCost < leftCost)
        {
            if (leftCost != 1000)
            {
                Debug.Log("Unit move by LeftPart" + "||LeftSideCost" + leftCost + "||RightSideCost" + rightCost);
                unit.data.unitSize = UnitData.Unit_Size.size3_OnTopNode; path = moveToList_size3_OnTopNode;
            }
        }

        return path;
    }


    public static List<PathNode> FindClosestPathToTargetNode_Size2_AI(Unit unit, Unit targetUnit, PathNode tartgetNode)
    {
        List<PathNode> path = null;

        GridManager grid = GameObject.FindGameObjectWithTag("GameController").GetComponent<GridManager>();

        unit.data.unitSize = UnitData.Unit_Size.size2_OnLeftNode;
        List<PathNode> moveToList_size2_OnLeftNode = PathFinding.FindPath(unit.nodeAt, tartgetNode, unit, targetUnit, false, false, 0, false);

        unit.data.unitSize = UnitData.Unit_Size.size2_OnRightNode;
        List<PathNode> moveToList_size2_OnRightNode = PathFinding.FindPath(unit.nodeAt_Right, tartgetNode, unit, targetUnit, false, true, 0, false);

        int leftCost = 1000;
        if (moveToList_size2_OnLeftNode != null) leftCost = moveToList_size2_OnLeftNode.Count;

        int rightCost = 1000;
        if (moveToList_size2_OnRightNode != null) rightCost = moveToList_size2_OnRightNode.Count;

        if (leftCost < rightCost)
        {
            //If there is a path, and is's closer to move by left side.
            if (leftCost != 1000)
            {
                Debug.Log("Unit move by LeftPart" + "||LeftSideCost" + leftCost + "||RightSideCost" + rightCost);
                unit.data.unitSize = UnitData.Unit_Size.size2_OnLeftNode;
                path = moveToList_size2_OnLeftNode;
            }
        }

        else
        {
            //If there is a path, and is's closer to move by right side.
            if (rightCost != 1000)
            {
                Debug.Log("Unit move by RightPart" + "||LeftSideCost" + leftCost + "||RightSideCost" + rightCost);
                unit.data.unitSize = UnitData.Unit_Size.size2_OnRightNode;
                path = moveToList_size2_OnRightNode;
            }
        }

        return path;
    }

    public static List<PathNode> FindClosestPathToTargetNode_Size1_AI(Unit unit, Unit targetUnit, PathNode tartgetNode)
    {
        List<PathNode> path = null;

        //[We calculate a moving path with corpse or without corpse.]
        //[If the unit moving and avoid corpse cost 2MP higher, it will first destroy the corpse on the way]
        List<PathNode> moveToListWithCorpse = PathFinding.FindPath(unit.nodeAt, tartgetNode, unit, targetUnit, false, false, 0, false);
        List<PathNode> moveToListWithoutCorpse = PathFinding.FindPath(unit.nodeAt, tartgetNode, unit, targetUnit, false, true, 0, false);

        if (moveToListWithoutCorpse != null)
        {
            if (moveToListWithCorpse != null)
            {
                int num1 = moveToListWithoutCorpse.Count;
                int num2 = moveToListWithCorpse.Count;
                if (moveToListWithoutCorpse.Count < moveToListWithCorpse.Count - 2)
                {
                    //Debug.Log("this is my path!");
                    path = moveToListWithoutCorpse;
                }
                else path = moveToListWithCorpse;
            }
            else path = moveToListWithoutCorpse;
        }

        return path;
    }


    ///////////////////////////////////////////////////
    ///Find_Unit ///Find_Unit ///Find_Unit ///Find_Unit
    ///Find_Unit ///Find_Unit ///Find_Unit ///Find_Unit
    ///Find_Unit ///Find_Unit ///Find_Unit ///Find_Unit
    ///Find_Unit ///Find_Unit ///Find_Unit ///Find_Unit
    /// ///////////////////////////////////////////////
    /// 
    public static Unit FindClosestUnit_Range(Unit selectedUnit, List<Unit> groupedlUnitlist)
    {
        if (groupedlUnitlist == null) return null;
        if (groupedlUnitlist.Count == 0) return null;

        Unit closedUnit = groupedlUnitlist[0];
        float closedDistance = 99999;

        for (int i = 0; i < groupedlUnitlist.Count; i++)
        {
                float disToTarget = GameFunctions.CalculateDis_WithoutY(selectedUnit.nodeAt, groupedlUnitlist[i].nodeAt);

                if (disToTarget < closedDistance)
                {
                    closedDistance = disToTarget;
                    closedUnit = groupedlUnitlist[i];
                }
        }

        return closedUnit;
    }

    public static List<Unit> FindClosestUnits_Range(Unit selectedUnit, List<Unit> groupedlUnitlist, int range)
    {
        if (groupedlUnitlist == null) return null;
        if (groupedlUnitlist.Count == 0) return null;

        List<Unit> myList = new List<Unit>();

        for (int i = 0; i < groupedlUnitlist.Count; i++)
        {
            float disToTarget = GameFunctions.CalculateDis_WithoutY(selectedUnit.nodeAt, groupedlUnitlist[i].nodeAt);

            if (disToTarget < range * 1.73f)
            {
                myList.Add(groupedlUnitlist[i]);
            }
        }

        return myList;
    }

    public static List<Unit> FindClosestUnitGroup_Range_AI(int repeatTime, Unit selectedUnit, List<Unit> groupedlUnitlist)
    {
        List<Unit> unitGroup = new List<Unit>();
        if (groupedlUnitlist == null) return null;

        for (int x = 0; x < repeatTime; x++)
        {
            Unit closedUnit = groupedlUnitlist[0];
            float closedDistance = 99999;

            for (int i = 0; i < groupedlUnitlist.Count; i++)
            {
                bool isInList = false;
                if (unitGroup != null)
                    if (unitGroup.Contains(groupedlUnitlist[i]))
                        isInList = true;


                if (!isInList)
                {
                    List<PathNode> moveToList = PathFinding.FindPath(selectedUnit.nodeAt, groupedlUnitlist[i].nodeAt, selectedUnit, groupedlUnitlist[i], true, true, 0, false);
                    if (moveToList != null)
                    {
                        int distance = moveToList.Count;
                        if (distance < closedDistance)
                        {
                            closedDistance = distance;
                            closedUnit = groupedlUnitlist[i];
                        }
                    }
                }
            }
            //Debug.Log(closedUnit);
            unitGroup.Add(closedUnit);
        }
        return unitGroup;
    }

    public static List<Unit> FindClosestUnitGroup_Melee_AI(int chooseNum, Unit selectedUnit, List<Unit> groupedlUnitlist)//FOR UNIT AI
    {
        List<Unit> unitGroup = new List<Unit>();
        if (groupedlUnitlist == null) return null;

        for (int x = 0; x < chooseNum; x++)
        {
            Unit closedUnit = groupedlUnitlist[0];
            float closetValue = 99999;

            for (int i = 0; i < groupedlUnitlist.Count; i++)
            {
                bool isInList = false;
                if (unitGroup != null)
                    if (unitGroup.Contains(groupedlUnitlist[i]))
                        isInList = true;

                if (!isInList)
                {
                    List<PathNode> moveToListWithCorpse = PathFinding.FindPath(selectedUnit.nodeAt, groupedlUnitlist[i].nodeAt, selectedUnit, groupedlUnitlist[i], false, false, 0, false);
                    List<PathNode> moveToListWithoutCorpse = PathFinding.FindPath(selectedUnit.nodeAt, groupedlUnitlist[i].nodeAt, selectedUnit, groupedlUnitlist[i], false, true, 0, false);
                    int value = 0;
                    if (moveToListWithCorpse == null) value += 1000;
                    if (moveToListWithoutCorpse == null) value += 1000;

                    if (moveToListWithCorpse != null && moveToListWithoutCorpse != null)
                    {
                        //IF DESTROY A CORPSE WILL SAVE 3 STRP TO MOVE, THEN DO SO.
                        if (moveToListWithoutCorpse.Count < moveToListWithCorpse.Count - 3)
                        {
                            value += moveToListWithoutCorpse.Count - 3;
                        }

                        else
                        {
                            value += moveToListWithCorpse.Count;
                        }

                        if (value < closetValue)
                        {

                            //Debug.Log(groupedlUnitlist[i] + " Value:" + value + "  " + closetValue);
                            closetValue = value;
                            closedUnit = groupedlUnitlist[i];
                        }
                    }
                }
            }
            unitGroup.Add(closedUnit);
        }
        return unitGroup;
    }


    public static Unit FindUnit_ByValue_AI(List<Unit> unitGroup, Unit selectedUnit)//FOR UNIT AI
    {
        Unit closedUnit = unitGroup[0];
        float closetValue = -99999;

        for (int i = 0; i < unitGroup.Count; i++)
        {
            float thisValue = (GameFunctions.Get_AverageDmg(unitGroup[i].currentData.damage) + unitGroup[i].currentData.power) * 2 - unitGroup[i].currentData.armorNow + Random.Range(0, 5);
            if (unitGroup[i].currentData.damage.range > 1) thisValue += Random.Range(5, 10);

            float disToTarget = GameFunctions.CalculateDis_WithoutY(selectedUnit.nodeAt, unitGroup[i].nodeAt);
            disToTarget = disToTarget / 1.73f;
            if (disToTarget < 3) disToTarget = 0;
            thisValue -= disToTarget;

            //Debug.Log(unitGroup[i].name + "" + thisValue);
            if (thisValue > closetValue)
            {

                closetValue = thisValue;
                closedUnit = unitGroup[i];
            }
        }
        return closedUnit;
    }


    public static Unit FindUnit_ByHighArmor_AI(List<Unit> unitGroup, Unit selectedUnit)//FOR UNIT AI
    {
        if (unitGroup == null || unitGroup.Count == 0) return null;
        Unit closedUnit = unitGroup[0];
        float closetValue = -99999;

        for (int i = 0; i < unitGroup.Count; i++)
        {
            float thisValue = unitGroup[i].currentData.armorNow;
          
            if (thisValue > closetValue)
            {
                closetValue = thisValue;
                closedUnit = unitGroup[i];
            }
        }
        return closedUnit;
    }

    public static Unit FindUnit_ByDam_AI(List<Unit> unitGroup, Unit selectedUnit)//FOR UNIT AI
    {
        if (unitGroup == null || unitGroup.Count == 0) return null;
        Unit closedUnit = unitGroup[0];
        float closetValue = -99999;

        for (int i = 0; i < unitGroup.Count; i++)
        {
            float thisValue = unitGroup[i].currentData.damage.damMin + unitGroup[i].currentData.damage.damMax;

            if (thisValue > closetValue)
            {
                closetValue = thisValue;
                closedUnit = unitGroup[i];
            }
        }
        return closedUnit;
    }

    public static Unit FindUnit_ByLowestHP_AI(List<Unit> unitGroup)//FOR UNIT AI
    {
        if (unitGroup == null || unitGroup.Count == 0) return null;
        Unit closedUnit = unitGroup[0];
        float closetValue = -99999;

        for (int i = 0; i < unitGroup.Count; i++)
        {
            float thisValue = unitGroup[i].currentData.healthMax -unitGroup[i].currentData.healthNow;

            if (thisValue > closetValue)
            {
                closetValue = thisValue;
                closedUnit = unitGroup[i];
            }
        }
        return closedUnit;
    }
    public static Unit FindUnit_ByLowHP_AI(List<Unit> groupedlUnitlist, Unit selectedUnit)
    {
        if (groupedlUnitlist == null) return selectedUnit;
        Unit unitNeedHeal = groupedlUnitlist[0];
        int lowestHP = -99999;

        for (int i = 0; i < groupedlUnitlist.Count; i++)
        {
            int HP = groupedlUnitlist[i].currentData.healthMax - groupedlUnitlist[i].currentData.healthNow;
            //Debug.Log("findUnit_ByLowHP USER:" + selectedUnit.name + " Target:" + groupedlUnitlist[i].name + "" + lowestHP);
            if (HP > lowestHP)
            {
                lowestHP = HP;
                unitNeedHeal = groupedlUnitlist[i];
            }
        }
        return unitNeedHeal;
    }

    public static Unit FindPlayer()
    {
        Unit[] units = FindObjectsOfType<Unit>();

        foreach (Unit unit in units)
        {
            if ( unit.data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._1_Avatar)
                return unit;
        }
        return null;
    }

    public static List<PathNode> CheckUnit_CanCharge_AI(Unit attacker, Unit defender,Skill skill)
    {
        List<PathNode> chargeRoad =  GameFunctions.FindNodes_InOneLine(attacker.nodeAt, defender.nodeAt, skill.range, true, false, false,false);

        if (chargeRoad != null)
        {
            foreach (PathNode path in chargeRoad)
            {
                if (path.unit != null)
                {
                    if (path.unit.unitTeam != attacker.unitTeam)
                    {
                        return chargeRoad;
                    }
                    else if (path.unit == attacker)
                    {
                        continue;
                    }
                    else return null;
                }    
            
            }

        }
        return null;
    }

    public static List<PathNode> CheckUnit_CanAttackOneRoad_AI(Unit attacker, Unit defender, Skill skill)
    {
        List<PathNode> chargeRoad = GameFunctions.FindNodes_InOneLine(attacker.nodeAt, defender.nodeAt, skill.range, true, false, false,false);

     
        return chargeRoad;
    }
}
