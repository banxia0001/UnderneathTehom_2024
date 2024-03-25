using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFunctions : MonoBehaviour
{

    //////////////////////////////////////////////////////////////////////////////////////
    /// Find Unit assets  /// Find Unit assets  /// Find Unit assets  /// Find Unit assets
    /// Find Unit assets  /// Find Unit assets  /// Find Unit assets  /// Find Unit assets
    /// Find Unit assets  /// Find Unit assets  /// Find Unit assets  /// Find Unit assets
    /// Find Unit assets  /// Find Unit assets  /// Find Unit assets  /// Find Unit assets
    //////////////////////////////////////////////////////////////////////////////////////

    public static GameObject Get_UnitPrefab(UnitPrefabList.Unit_SpriteAsset_Type type)
    {
        UnitPrefabList myList = Resources.Load<UnitPrefabList>("Unit/MyUnitList");
        GameObject unitPrefab = null;
        foreach (UnitPrefab list in myList.prefabs)
        {
            if (list.type == type)
            {
                unitPrefab = list.prefab_Variations[0];
            }
        }
        return unitPrefab;
    }

    public static GameObject Find_UnitCorpse(UnitPrefab_Corpse.Unit_Corpse_Type type)
    {
        UnitPrefabList myList = Resources.Load<UnitPrefabList>("Unit/MyUnitList");
        GameObject prefab = null;

        if(type == UnitPrefab_Corpse.Unit_Corpse_Type.humanoid)
            prefab = myList.corpsePrefabs[0].prefab_Variations[Random.Range(0, myList.corpsePrefabs[0].prefab_Variations.Count)];

        if (type == UnitPrefab_Corpse.Unit_Corpse_Type.rat)
            prefab = myList.corpsePrefabs[1].prefab_Variations[Random.Range(0, myList.corpsePrefabs[1].prefab_Variations.Count)];

        return prefab;
    }

   
    public static void FindNewPosition_Size1(Unit unit)
    {
        Debug.Log("Unit is collide with other unit in same tile.");
        unit.nodeAt = GameFunctions.FindNode_Nearby(unit.nodeAt, 2);
        if (unit.nodeAt == null) Destroy(unit.gameObject);
        else unit.nodeAt.unit = unit;
    }

    ////////////////////////////////////////////////////////////////////////////////////
    /// Unit_BuffUpdate  ///Unit_BuffUpdate  /// Unit_BuffUpdate  ///Unit_BuffUpdate  ///
    /// Unit_BuffUpdate  ///Unit_BuffUpdate  /// Unit_BuffUpdate  ///Unit_BuffUpdate  ///
    /// Unit_BuffUpdate  ///Unit_BuffUpdate  /// Unit_BuffUpdate  ///Unit_BuffUpdate  ///
    /// Unit_BuffUpdate  ///Unit_BuffUpdate  /// Unit_BuffUpdate  ///Unit_BuffUpdate  ///
    ////////////////////////////////////////////////////////////////////////////////////
    public static void ResetUnitValueToUnitData(Unit unit, UnitData data)
    {
        unit.currentData.Name = data.Name;
        unit.currentData.healthMax = data.healthMax;
        unit.currentData.movePointMax = data.movePointMax;

        //DEFEND
        unit.currentData.armorMax = data.armorMax;
        unit.currentData.dodge = data.dodge;

        //MAGIC
        unit.currentData.power = data.power;
        unit.currentData.MR = data.MR;
        unit.currentData.SR = data.SR;

        //DAMAGE
        unit.currentData.damage = new UnitWeapon(data.damage);

        unit.canRepel = false;
        unit.expertHealer = false;
        unit.giantSlayer = false;
        unit.isCurse = false;
    }

    public static void CheckBuff(UnitData unit, Buff buff, Unit myUnit)
    {
        if (buff == null) return;
        unit.healthMax += buff.healthMax;

        //Weapon
        unit.damage.damBonus += buff.dam;
        unit.damage.hit += buff.hit;
        unit.damage.armorSunder += buff.armorSunder;

        //Defence
        unit.dodge += buff.dodge;

        //Magic
        unit.power += buff.power;
        unit.MR += buff.MR;
        unit.SR += buff.SR;

        //MovePoint
        unit.movePointMax += buff.movePointMax;

        if (myUnit != null)
        {
            if (buff.canRepel == true) myUnit.canRepel = true;
            if (buff.expertHeal == true) myUnit.expertHealer = true;
            if (buff.giantSlayer == true) myUnit.giantSlayer = true;
            if (buff.isCurse == true) myUnit.isCurse = true;
        }
        //if (buff.isUnholyCreature == true) unit.isUnholyCreature = true;
    }

  

    public static Vector3 GetUnitMiddlePoint(Unit unit)
    {
        return unit.TranformOffsetFolder.transform.GetChild(1).transform.position;
    }

    public static bool checkUnit_Rat(Unit unit)
    {
        bool isRat = false;
        foreach (_Buff buff in unit.buffList)
        {
            if (buff.buff.tag_Rat) isRat = true;
        }
        foreach (_Buff buff in unit.data.traitList)
        {
            if (buff.buff.tag_Rat) isRat = true;
        }
        return isRat;
    }

    public static int Check_Hitrate_WithBuff(Unit unit)
    {
        int hit = 0;

        foreach (_Buff buff in unit.buffList)
        {
            hit += buff.buff.hit;
        }
        foreach (_Buff buff in unit.data.traitList)
        {
            hit += buff.buff.hit;
        }
        return hit;
    }
    public static int Check_Damage_WithBuff(Unit unit)
    {
        int hit = 0;

        foreach (_Buff buff in unit.buffList)
        {
            hit += buff.buff.dam;
        }
        foreach (_Buff buff in unit.data.traitList)
        {
            hit += buff.buff.dam;
        }
        return hit;
    }
    public static int Check_Hitrate_WithRat(Unit unit)
    {
        if (!checkUnit_Rat(unit)) return 0;

        List<PathNode> nearbyList = GameFunctions.FindNodes_ByDistance(unit.nodeAt, 1, false);
        int num = 0;
        //Debug.Log(nearbyList.Count);

        foreach (PathNode path in nearbyList)
        {
            if (path.unit != null)

                if (path.unit != unit && path.unit.unitTeam == unit.unitTeam)
                {
                    if (checkUnit_Rat(path.unit))
                    {
                            num += 5;
                    }
                }
        }
        return num;
    }
    public static int Check_DamBonus_HighGround(Unit unit)
    {
        int dam = 0;
        foreach (_Buff buff in unit.buffList)
        {
            dam += buff.buff.dam_InHightGround;
        }
        foreach (_Buff buff in unit.data.traitList)
        {
            dam += buff.buff.dam_InHightGround;
        }

        return dam;
    }

    public static int Check_AN(Unit unit)
    {
        int AN = 0;
        foreach (_Buff buff in unit.buffList)
        {
            AN += buff.buff.armorNegative;
        }
        foreach (_Buff buff in unit.data.traitList)
        {
            AN += buff.buff.armorNegative;
        }

        return AN;
    }

    public static int Check_KillFPBonus(Unit unit)
    {
        int dam = 0;
        foreach (_Buff buff in unit.buffList)
        {
            dam += buff.buff.gain_Fp_Kill;
        }
        foreach (_Buff buff in unit.data.traitList)
        {
            dam += buff.buff.gain_Fp_Kill;
        }

        return dam;
    }

    public static int Check_DamBonus_TowardCursedUnit(Unit unit)
    {
        int dam = 0;
        foreach (_Buff buff in unit.buffList)
        {
            dam += buff.buff.dam_ToCursedUnit;
        }
        foreach (_Buff buff in unit.data.traitList)
        {
            dam += buff.buff.dam_ToCursedUnit;
        }

        return dam;
    }

    public static int Check_Lifesteal(Unit attacker)
    {
        int lifesteal = 0;
        foreach (_Buff buff in attacker.buffList)
        {
            lifesteal += buff.buff.lifesteal;
        }
        foreach (_Buff buff in attacker.data.traitList)
        {
            lifesteal += buff.buff.lifesteal;
        }
        return lifesteal;
    }

    public static int Check_DodgeRateAddOnRange(Unit defender)
    {
        int rangeReduce = 0;
        foreach (_Buff buff in defender.buffList)
        {
            rangeReduce += buff.buff.dodgeRate_FacingRange;
        }
        foreach (_Buff buff in defender.data.traitList)
        {
            rangeReduce += buff.buff.dodgeRate_FacingRange;
        }
        return rangeReduce;
    }
    public static int AddFP()
    {
        int value = 0;
        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
          
            foreach (_Buff buff in unit.data.traitList)
            {
                //Debug.Log(buff.buff.gain_Fp_EnterBattle);
                value += buff.buff.gain_Fp_EnterBattle;
            }
        }
        return value;
    }
    public static bool Check_TriggerStandAlone(PathNode path, Unit.UnitTeam unitTeam)
    {
        List<PathNode> nearby = GameFunctions.FindNodes_ByDistance(path, 1, true);

        bool canTrigger = true;
        for (int i = 0; i < nearby.Count; i++)
        {
            if (nearby[i] == path) continue;
            if (nearby[i].unit != null)
            {
                if (nearby[i].unit.unitTeam == unitTeam) canTrigger = false;
            }
        }

        return canTrigger;
    }

    public static void Check_Aura_OfGrowingDeath(PathNode path, Unit.UnitTeam unitTeam)
    {
        List<PathNode> nearby = GameFunctions.FindNodes_ByDistance(path, 1, true);

        for (int i = 0; i < nearby.Count; i++)
        {
            if (nearby[i] == path) continue;

            if (nearby[i].unit != null)
            {
                if (nearby[i].unit.unitTeam == unitTeam)
                {
                    bool isUnholyCreature = false;
                    foreach (_Buff buff in nearby[i].unit.data.traitList)
                    {
                        if (buff.buff.tag_Undead) isUnholyCreature = true;
                    }
                    if (isUnholyCreature)
                    {
                        nearby[i].unit.HealthChange(1, 0, "Heal");
                    }
                 
                }
            }
        }
    }

    public static void Check_Aura_OfTarish(PathNode path, Unit.UnitTeam unitTeam)
    {
        List<PathNode> nearby = GameFunctions.FindNodes_ByDistance(path, 1, true);

        Buff damBonus = Resources.Load<Buff>("Buff/Damage I");
        for (int i = 0; i < nearby.Count; i++)
        {
            if (nearby[i] == path) continue;

            if (nearby[i].unit != null)
            {
                if (nearby[i].unit.unitTeam == unitTeam)
                {
                    bool isUnholyCreature = false;
                    foreach (_Buff buff in nearby[i].unit.data.traitList)
                    {
                        if (buff.buff.tag_Undead) isUnholyCreature = true;
                    }
                    if (isUnholyCreature)
                    {
                        nearby[i].unit.InputBuff(damBonus, true);
                    }
                }
            }
        }
    }




    ////////////////////////////////////////////////////////////////////////////////////
    /// UnitMove  ///UnitMove  /// UnitMove  ///UnitMove  /// UnitMove  ///UnitMove  /// 
    /// UnitMove  ///UnitMove  /// UnitMove  ///UnitMove  /// UnitMove  ///UnitMove  /// 
    /// UnitMove  ///UnitMove  /// UnitMove  ///UnitMove  /// UnitMove  ///UnitMove  /// 
    /// UnitMove  ///UnitMove  /// UnitMove  ///UnitMove  /// UnitMove  ///UnitMove  /// 
    ////////////////////////////////////////////////////////////////////////////////////

    public static void MoveUnitToNewGrid_Size1(Unit unit, PathNode moveToNode)
    {
        if (unit.nodeAt != null) unit.nodeAt.unit = null;
        if (unit.nodeAt_Right != null) unit.nodeAt.unit = null;
        if (unit.nodeAt_Top != null) unit.nodeAt.unit = null;

        moveToNode.unit = unit;
        unit.nodeAt = moveToNode;
    }
    public static void MoveUnitToNewGrid_Size2(Unit unit, PathNode moveToNode)
    {
        GridManager grid = GameObject.FindGameObjectWithTag("GameController").GetComponent<GridManager>();
        if (unit.nodeAt != null) unit.nodeAt.unit = null;
        if (unit.nodeAt_Right != null) unit.nodeAt.unit = null;
        if (unit.nodeAt_Top != null) unit.nodeAt.unit = null;

        moveToNode.unit = unit;

        if (unit.data.unitSize == UnitData.Unit_Size.size2_OnLeftNode)
        {
            unit.nodeAt = moveToNode;
            Debug.Log("L_Moving to: " + moveToNode.name);

            //[Set the right node]
            PathNode _nodeAt_Right = grid.GetPath(unit.nodeAt.x + 1, unit.nodeAt.y);
            if (_nodeAt_Right != null && _nodeAt_Right.unit == null)
            {
                unit.nodeAt_Right = _nodeAt_Right;
                unit.nodeAt_Right.unit = unit;
            }
            else Debug.LogError("Size2_Left Unit Move Error, Cannot find nearby node or nearby node Contain Unit.");
        }

        if (unit.data.unitSize == UnitData.Unit_Size.size2_OnRightNode)
        {
            unit.nodeAt_Right = moveToNode;

            //[Set the left node]
            PathNode _nodeAt_Left = grid.GetPath(unit.nodeAt_Right.x - 1, unit.nodeAt_Right.y);
            if (_nodeAt_Left != null && _nodeAt_Left.unit == null)
            {
                unit.nodeAt = _nodeAt_Left;
                unit.nodeAt.unit = unit;
                Debug.Log("Moving to: " + moveToNode.name + "   |||Current NodeAt: " + unit.nodeAt.name + "   |||Current NodeAt_Right: " + unit.nodeAt_Right.name);
            }
            else Debug.LogError("Size2_Right Unit Move Error, Cannot find nearby node or nearby node Contain Unit.");
        }
    }

    public static void MoveUnitToNewGrid_Size3(Unit unit, PathNode moveToNode)
    {
        GridManager grid = GameObject.FindGameObjectWithTag("GameController").GetComponent<GridManager>();
        if (unit.nodeAt != null) unit.nodeAt.unit = null;
        if (unit.nodeAt_Right != null) unit.nodeAt_Right.unit = null;
        if (unit.nodeAt_Top != null) unit.nodeAt_Top.unit = null;

        moveToNode.unit = unit;

        if (unit.data.unitSize == UnitData.Unit_Size.size3_OnLeftNode)
        {
            unit.nodeAt = moveToNode;
            Debug.Log("L_Moving to: " + moveToNode.name);

            //[Set the right node]
            PathNode _nodeAt_Right = grid.GetPath(unit.nodeAt.x + 1, unit.nodeAt.y);
            if (_nodeAt_Right != null && _nodeAt_Right.unit == null)
            {
                unit.nodeAt_Right = _nodeAt_Right;
                unit.nodeAt_Right.unit = unit;
            }
            else Debug.LogError("Size3_Left Unit Move Error, Cannot find right node.");

            //[Set the top node]
            PathNode _nodeAt_Top = GameFunctions.FindNode_RightUp(unit.nodeAt);
            if (_nodeAt_Top != null && _nodeAt_Top.unit == null)
            {
                unit.nodeAt_Top = _nodeAt_Top;
                unit.nodeAt_Top.unit = unit;
            }
            else Debug.LogError("Size3_Left Unit Move Error, Cannot find top node.");
        }

        if (unit.data.unitSize == UnitData.Unit_Size.size3_OnRightNode)
        {
            unit.nodeAt_Right = moveToNode;

            //[Set the left node]
            PathNode _nodeAt_Left = grid.GetPath(unit.nodeAt_Right.x - 1, unit.nodeAt_Right.y);
            if (_nodeAt_Left != null && _nodeAt_Left.unit == null)
            {
                unit.nodeAt = _nodeAt_Left;
                unit.nodeAt.unit = unit;
            }
            else Debug.LogError("Size3_Left Unit Move Error, Cannot find left node.");

            //[Set the top node]
            PathNode _nodeAt_Top = GameFunctions.FindNode_LeftUp(unit.nodeAt_Right);
            if (_nodeAt_Top != null && _nodeAt_Top.unit == null)
            {
                unit.nodeAt_Top = _nodeAt_Top;
                unit.nodeAt_Top.unit = unit;
            }
            else Debug.LogError("Size3_Left Unit Move Error, Cannot find top node.");
        }


        if (unit.data.unitSize == UnitData.Unit_Size.size3_OnTopNode)
        {
            unit.nodeAt_Top = moveToNode;

            //[Set the left node]
            PathNode _nodeAt_Left = GameFunctions.FindNode_LeftDown(unit.nodeAt_Top);
            if (_nodeAt_Left != null && _nodeAt_Left.unit == null)
            {
                unit.nodeAt = _nodeAt_Left;
                unit.nodeAt.unit = unit;
            }
            else Debug.LogError("Size3_Left Unit Move Error, Cannot find left node.");

            //[Set the right node]
            PathNode _nodeAt_Right = GameFunctions.FindNode_RightDown(unit.nodeAt_Top);
            if (_nodeAt_Right != null && _nodeAt_Right.unit == null)
            {
                unit.nodeAt_Right = _nodeAt_Right;
                unit.nodeAt_Right.unit = unit;
            }
            else Debug.LogError("Size3_Left Unit Move Error, Cannot find right node.");
        }
    }





    //////////////////////////////////////////////////////////////////////////////
    /// SetIcon  ///SetIcon  /// SetIcon  ///SetIcon  /// SetIcon  ///SetIcon  /// 
    /// SetIcon  ///SetIcon  /// SetIcon  ///SetIcon  /// SetIcon  ///SetIcon  /// 
    /// SetIcon  ///SetIcon  /// SetIcon  ///SetIcon  /// SetIcon  ///SetIcon  /// 
    /// SetIcon  ///SetIcon  /// SetIcon  ///SetIcon  /// SetIcon  ///SetIcon  /// 
    //////////////////////////////////////////////////////////////////////////////
   
















    public static void PlaceUnitOnTile(Unit unit)
    {
        //[Place Unit in grids]
        RaycastHit2D hit = Physics2D.Raycast(unit.transform.position, unit.transform.TransformDirection(new Vector3(0, 0, -1)), LayerMask.GetMask("Ground"));
        if (hit.collider != null)
        {
            unit.nodeAt = hit.collider.transform.gameObject.GetComponent<PathNode>();
            if (unit.nodeAt.unit == null) unit.nodeAt.unit = unit;
            else if(unit.nodeAt.unit.currentData.healthNow <= 0) unit.nodeAt.unit = unit;
            else UnitFunctions.FindNewPosition_Size1(unit);
            unit.transform.position = unit.nodeAt.transform.position;
        }
        else { Debug.Log("Unit is not on Ground"); Destroy(unit.gameObject); }




        //[Size2 Unit]
        if (unit.data.unitSize == UnitData.Unit_Size.size2_OnLeftNode || unit.data.unitSize == UnitData.Unit_Size.size2_OnRightNode)
        {
            hit = Physics2D.Raycast(unit.nodeAt.transform.position + new Vector3(1.5f, 0, 0), unit.transform.TransformDirection(new Vector3(0, 0, -1)), LayerMask.GetMask("Ground"));
            if (hit.collider != null)
            {
                unit.nodeAt_Right = hit.collider.transform.gameObject.GetComponent<PathNode>();
                if (unit.nodeAt_Right.unit == null) unit.nodeAt_Right.unit = unit;
            }
            else { Debug.Log("Unit is not on Ground"); Destroy(unit.gameObject); }
        }



        //[Size3 Unit]
        if (unit.data.unitSize == UnitData.Unit_Size.size3_OnLeftNode || unit.data.unitSize == UnitData.Unit_Size.size3_OnRightNode || unit.data.unitSize == UnitData.Unit_Size.size3_OnTopNode)
        {
            hit = Physics2D.Raycast(unit.nodeAt.transform.position + new Vector3(1.5f, 0, 0), unit.transform.TransformDirection(new Vector3(0, 0, -1)), LayerMask.GetMask("Ground"));
            if (hit.collider != null)
            {
                unit.nodeAt_Right = hit.collider.transform.gameObject.GetComponent<PathNode>();
                if (unit.nodeAt_Right.unit == null) unit.nodeAt_Right.unit = unit;
            }
            else { Debug.Log("Unit is not on Ground"); Destroy(unit.gameObject); }

            hit = Physics2D.Raycast(unit.nodeAt.transform.position + new Vector3(0.75f, 1f, 0), unit.transform.TransformDirection(new Vector3(0, 0, -1)), LayerMask.GetMask("Ground"));
            if (hit.collider != null)
            {
                unit.nodeAt_Top = hit.collider.transform.gameObject.GetComponent<PathNode>();
                if (unit.nodeAt_Top.unit == null) unit.nodeAt_Top.unit = unit;
            }
            else { Debug.Log("Unit is not on Ground"); Destroy(unit.gameObject); }
        }
    }



    ///////////////////////////////////////////////////////////////////
    /// UnitPosition /// UnitPosition /// UnitPosition /// UnitPosition
    /// UnitPosition /// UnitPosition /// UnitPosition /// UnitPosition
    /// UnitPosition /// UnitPosition /// UnitPosition /// UnitPosition
    /// UnitPosition /// UnitPosition /// UnitPosition /// UnitPosition
    ///////////////////////////////////////////////////////////////////
    public static void Flip(float x1, float x2, Unit target)
    {
        GameObject gameObject = target.TranformOffsetFolder;
        if (target.unitAttribute != Unit.UnitAttribute.alive) return;
        if (!target.canNormalFlip) return;
        if (target.unitSpecialState == Unit.UnitSpecialState.normalUnit || target.unitSpecialState == Unit.UnitSpecialState.machineGun)
        {
            bool isInHoldStage = false;
            if (target.unitTeam == Unit.UnitTeam.enemyTeam)
                if (target.GetComponent<UnitAI>().unitHoldingSkill) isInHoldStage = true;

            if (!isInHoldStage)
            {
                float dis = x1 - x2;
                if (dis > 0) gameObject.transform.localScale = new Vector3(-1, 1, 1);
                else if (dis < 0) gameObject.transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    public static void Flip_Simple(float x1, float x2, GameObject target)
    {
        float dis = x1 - x2;
        if (dis > 0) target.transform.localScale = new Vector3(-1, 1, 1);
        else if (dis < 0) target.transform.localScale = new Vector3(1, 1, 1);
    }


    public static Vector3 Find_UnitShouldGoPosition_WhenKnochBack(Unit attacker, Unit defender)
    {
        Vector3 shouldKnockBackPos = new Vector3(0, 0, 0);
        Vector2 faceDirection; faceDirection = (attacker.transform.position - defender.transform.position).normalized;
        float angleRotation = Mathf.Atan2(faceDirection.y, faceDirection.x) * Mathf.Rad2Deg;
        Vector3 distantPosition = defender.transform.position + Quaternion.Euler(0, 0, angleRotation) * Vector3.right * -2f;
        return distantPosition;
    }
    public static Vector3 UnitPosition_WithSize(Unit unit, UnitData data)
    {
        Vector3 pos = unit.nodeAt.transform.position;
        if (data.unitSize == UnitData.Unit_Size.size1)
            pos = unit.nodeAt.transform.position;
        if (data.unitSize == UnitData.Unit_Size.size2_OnLeftNode || data.unitSize == UnitData.Unit_Size.size2_OnRightNode)
            pos += new Vector3(0.75f, 0, 0);
        if (data.unitSize == UnitData.Unit_Size.size3_OnLeftNode || data.unitSize == UnitData.Unit_Size.size3_OnRightNode || data.unitSize == UnitData.Unit_Size.size3_OnTopNode)
        {
            if (unit.nodeAt != null && unit.nodeAt_Right != null)
            {
                pos = (unit.nodeAt.transform.position + unit.nodeAt_Right.transform.position) / 2;
            }
            else pos += new Vector3(0.75f, 0, 0);
        }
          
        return pos;
    }




    public static List<Unit> SortingEnemyListByX(List<Unit> enemyList)
    {
        List<Unit> newEnemyList = new List<Unit>();

        for (int x = 0; x < enemyList.Count; x++)
        {
            if (enemyList[x] == null) continue;

            Unit closedUnit = enemyList[0];
            float closedDistance = 99999;

            for (int i = 0; i < enemyList.Count; i++)
            {
                if(enemyList[i] == null) continue;
                bool isInList = false;
                if (newEnemyList != null)
                    foreach (Unit unit in newEnemyList)
                    {
                        if (enemyList[i] == unit) { isInList = true; }
                    }

                if (!isInList)
                {
                    float distance = enemyList[i].transform.position.x;
                    if (distance < closedDistance)
                    {
                        closedDistance = distance;
                        closedUnit = enemyList[i];
                    }
                }
            }
            newEnemyList.Add(closedUnit);
        }
        return newEnemyList;
    }


}
