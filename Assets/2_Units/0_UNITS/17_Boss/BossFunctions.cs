using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFunctions : MonoBehaviour
{
    public static int CheckSword()
    {
        int num = 0;
        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
            if (unit.unitSpecialState == Unit.UnitSpecialState.boneFireTower) num++;
        }
        return num;
    }
    public static IEnumerator Boss_SummonSwordToHand(BossAI Boss, Unit thisUnit)
    {
        GameController GC = FindObjectOfType<GameController>();
        GC.isAttacking = true;
       
        yield return new WaitForSeconds(0.2f);
        Boss.ChangeSword(20);
        Boss.ActionEnd();
    }
    public static IEnumerator Boss_ThrowSwordToBattlefield(BossAI Boss, Unit bossUnit, GameObject prefab)
    {
        GameController GC = FindObjectOfType<GameController>();
        GC.isAttacking = true;
        yield return new WaitForSeconds(0.2f);

        List<PathNode> chosedPath = Get_RandomPathInField(true,false);
        if(chosedPath != null)
        {
            Boss.ChangeSword(-1);

            PathNode finalDir = chosedPath[Random.Range(0, chosedPath.Count)];
            List<PathNode> chosedPath_Nearby = GameFunctions.FindNodes_InRange1_HardCode(finalDir, 7, true);

            Debug.Log(chosedPath_Nearby.Count);

            GameObject unitObject = Instantiate(prefab, finalDir.transform.position, Quaternion.identity);
            Unit myUnit = unitObject.GetComponent<Unit>();
            myUnit.nodeAt = finalDir;
            myUnit.nodeAt.unit = myUnit;
            GameController.enemyList.Add(myUnit);

            //if (SpecialEffect != null)
            //{
            //    Instantiate(SpecialEffect, geneNode.transform.position, Quaternion.identity);
            //}

            foreach (PathNode path in chosedPath_Nearby)
            {
                if (path != null)
                {
                    GridFallController.AddNodeToDynamicFolder(path, "Down", "");
                    path.AddHeight_WithCD(-2, 3, 1);

                    if (path.unit != null && path.unit.unitTeam == Unit.UnitTeam.playerTeam)
                    {
                        SkillFunctions.Skill_SimpleDamageCalculate(3, bossUnit, path.unit, null);
                       Boss_KnockBack(myUnit, path.unit,1, finalDir);
                    }
                }
            }

            yield return new WaitForSeconds(0.4f);
        }

        yield return new WaitForSeconds(1f);
        Boss.ActionEnd();
    }


    public static IEnumerator BossSkill_1_1(BossAI Boss, Unit TargetUnit, Unit bossUnit)
    {
        GameController GC = FindObjectOfType<GameController>();
        GC.isAttacking = true;

        List<PathNode> chargeNode = Get_Size3ChargePath(GameFunctions.FindNodes_InOneLine(bossUnit.nodeAt, TargetUnit.nodeAt, 5, true, false,true, true));
        yield return new WaitForSeconds(0.2f);

        if (chargeNode != null)
        {
            //Boss Animation
            Boss.NodeAttackInHold = chargeNode;
            Boss.Draw_NodeGuideInHold(chargeNode);
            yield return new WaitForSeconds(0.2f);
        }
        else
        {
            Boss.Stage1_NoSkill_BackToNormal();
        }

        yield return new WaitForSeconds(1f);
        Boss.ActionEnd();
    }
    public static IEnumerator BossSkill_1_2(BossAI Boss, Unit thisUnit)
    {
        GameController GC = FindObjectOfType<GameController>();
        GC.isAttacking = true;
        //Boss Animation
        yield return new WaitForSeconds(0.2f);

        foreach (PathNode path in Boss.NodeAttackInHold)
        {
            if (path == null) continue;
            GridFallController.AddNodeToDynamicFolder(path, "Up", "");
            path.AddHeight_WithCD(2,5,1);

            if (path.unit != null)
            {
                SkillFunctions.Skill_SimpleDamageCalculate(3,thisUnit,path.unit,null);
                Boss_KnockBack(thisUnit, path.unit,2,null);
            }
        }

        Boss.NodeAttackInHold = new List<PathNode>();

        yield return new WaitForSeconds(1f);
        Boss.ActionEnd();
    }
    public static IEnumerator BossSkill_2_1(BossAI Boss)
    {
        GameController GC = FindObjectOfType<GameController>();
        GC.isAttacking = true;
        List<PathNode> chosedPath = Get_RandomPathInField(false,true);
        yield return new WaitForSeconds(0.2f);

        if (chosedPath != null)
        {
            //Boss Animation
            Boss.NodeAttackInHold = chosedPath;
            Boss.Draw_NodeGuideInHold(chosedPath);
            yield return new WaitForSeconds(0.2f);
        }
        else
        {
            Boss.Stage1_NoSkill_BackToNormal();
        }

        yield return new WaitForSeconds(1f);
        Boss.ActionEnd();
    }

    public static IEnumerator BossSkill_2_2(BossAI Boss, Unit thisUnit, GameObject VFX)
    {
        GameController GC = FindObjectOfType<GameController>();
        GC.isAttacking = true;
        //Boss Animation
        yield return new WaitForSeconds(0.2f);

        foreach (PathNode path in Boss.NodeAttackInHold)
        {
            if (path == null) continue;
            GridFallController.AddNodeToDynamicFolder(path, "Down", "");
            path.AddHeight_WithCD(-2, 5, 1);
            if (VFX != null)
            {
                Instantiate(VFX, path.transform.position, Quaternion.identity);
            }
            if (path.unit != null)
            {
                SkillFunctions.Skill_SimpleDamageCalculate(3, thisUnit, path.unit, null);
                Boss_KnockBack(thisUnit, path.unit, 2,null);
            }
        }
        Boss.NodeAttackInHold = new List<PathNode>();
        yield return new WaitForSeconds(1f);
        Boss.ActionEnd();
    }


    public static IEnumerator Prepare_JumpDown(Unit thisUnit, PathNode gotoPath, BossAI AI)
    {
        List<NodeList> shockList = BossFunctions.Get_Size3_ShockWaveList(gotoPath);
        yield return new WaitForSeconds(0.2f);
        List<PathNode> node = new List<PathNode>();

        foreach(PathNode mynode in shockList[0].node)
        {
            if (mynode != null) node.Add(mynode);
        }
        foreach (PathNode mynode in shockList[1].node)
        {
            if (mynode != null) node.Add(mynode);
        }
        foreach (PathNode mynode in shockList[2].node)
        {
            if(mynode !=null)node.Add(mynode);
        }

        AI.Draw_NodeGuideInHold(node);
        AI.ActionEnd();
    }

    public static List<NodeList> Get_Size3_ShockWaveList(PathNode nodeAt)
    {
        List<NodeList> returnList = new List<NodeList>();
        GridManager GM = GameObject.FindGameObjectWithTag("GameController").GetComponent<GridManager>();

        List<PathNode> groupANode = new List<PathNode>();
        groupANode.Add(nodeAt);
       
        if (nodeAt.y % 2 == 0)
        {
            groupANode.Add(GameFunctions.FindNode_ByOffset(nodeAt, 1, 0, GM));
            groupANode.Add(GameFunctions.FindNode_ByOffset(nodeAt, 0, 1, GM));
        } returnList.Add(new NodeList(groupANode));


        List<PathNode> groupBNode = new List<PathNode>();
        if (nodeAt.y % 2 == 0)
        {
            groupBNode.Add(GameFunctions.FindNode_ByOffset(nodeAt, -1, -1, GM));
            groupBNode.Add(GameFunctions.FindNode_ByOffset(nodeAt, 0, -1, GM));
            groupBNode.Add(GameFunctions.FindNode_ByOffset(nodeAt, -1, 0, GM));
            groupBNode.Add(GameFunctions.FindNode_ByOffset(nodeAt, -1, 1, GM));
            groupBNode.Add(GameFunctions.FindNode_ByOffset(nodeAt, 0, 2, GM));
            groupBNode.Add(GameFunctions.FindNode_ByOffset(nodeAt, 1, 2, GM));
            groupBNode.Add(GameFunctions.FindNode_ByOffset(nodeAt, 1, 1, GM));
            groupBNode.Add(GameFunctions.FindNode_ByOffset(nodeAt, 2, 0, GM));
            groupBNode.Add(GameFunctions.FindNode_ByOffset(nodeAt, 1, -1, GM));
        } returnList.Add(new NodeList(groupBNode));


        List<PathNode> groupCNode = new List<PathNode>();
        if (nodeAt.y % 2 == 0)
        {
            groupCNode.Add(GameFunctions.FindNode_ByOffset(nodeAt, -2, 0, GM));
            groupCNode.Add(GameFunctions.FindNode_ByOffset(nodeAt, -2, -1, GM));
            groupCNode.Add(GameFunctions.FindNode_ByOffset(nodeAt, -1, -2, GM));
            groupCNode.Add(GameFunctions.FindNode_ByOffset(nodeAt, 0, -2, GM));
            groupCNode.Add(GameFunctions.FindNode_ByOffset(nodeAt, 1, -2, GM));
            groupCNode.Add(GameFunctions.FindNode_ByOffset(nodeAt, 2, -2, GM));
            groupCNode.Add(GameFunctions.FindNode_ByOffset(nodeAt, 3, 0, GM));
            groupCNode.Add(GameFunctions.FindNode_ByOffset(nodeAt, 2, 1, GM));
            groupCNode.Add(GameFunctions.FindNode_ByOffset(nodeAt, 2, 2, GM));      
            groupCNode.Add(GameFunctions.FindNode_ByOffset(nodeAt, 1, 3, GM));
            groupCNode.Add(GameFunctions.FindNode_ByOffset(nodeAt, 0, 3, GM));
            groupCNode.Add(GameFunctions.FindNode_ByOffset(nodeAt, -1, 3, GM));
            groupCNode.Add(GameFunctions.FindNode_ByOffset(nodeAt, -1, 2, GM));
            groupCNode.Add(GameFunctions.FindNode_ByOffset(nodeAt, -2, 1, GM));
        } returnList.Add(new NodeList(groupCNode));

        return returnList;
    }


    public static List<PathNode> Get_Size3ChargePath(List<PathNode> startPath)
    {
        if (startPath == null || startPath.Count == 0) return null;

        List<PathNode> addPath = new List<PathNode>();
        for (int i = 1; i < startPath.Count; i++)
        {
            PathNode calculatePath = startPath[i];

            PathNode nodeTopRight = GameFunctions.FindNode_RightUp(calculatePath);
            if (nodeTopRight != null && !addPath.Contains(nodeTopRight)) addPath.Add(nodeTopRight);

            PathNode nodeRight = GameFunctions.FindNode_Right(calculatePath);
            if (nodeRight != null && !addPath.Contains(nodeRight)) addPath.Add(nodeRight);
        }

        foreach (PathNode path in addPath)
        {
            startPath.Add(path);
        }

        List<PathNode> chosedPath_Final = new List<PathNode>();

        foreach (PathNode path in startPath)
        {
            if (chosedPath_Final.Contains(path)) continue;
            chosedPath_Final.Add(path);
        }
        return chosedPath_Final;
    }

    
    public static List<PathNode> Get_RandomPathInField(bool onlyRandomNodeNearbyPlayer, bool igonreUnit)
    {
        PathNode[] array_allPath = FindObjectsOfType<PathNode>();
        List<PathNode> allPath = new List<PathNode>();
        List<PathNode> chosedPath = new List<PathNode>();

        foreach (PathNode node in array_allPath)
        {
            allPath.Add(node);
        }

        foreach (Unit unit in GameController.playerList)
        {
            if (Random.Range(0, 1) == 0)
            {
                List<PathNode> nearbyNode = GameFunctions.FindNodes_InRange1_HardCode(unit.nodeAt,7, igonreUnit);
                
                foreach (PathNode path in nearbyNode)
                {
                    if (path == null) continue;
                    chosedPath.Add(path);
                    if (allPath.Contains(path)) allPath.Remove(path);
                }
            }
        }

     
        if (!onlyRandomNodeNearbyPlayer)
        {
            while (allPath.Count > 1 && chosedPath.Count < 15)
            {
                PathNode calculatePath = allPath[Random.Range(0, allPath.Count)];
                allPath.Remove(calculatePath);
                //Debug.Log("Random: calculatePath: " + calculatePath.x + "," + calculatePath.y);
                if (calculatePath == null) continue;
                if (calculatePath.isBlocked) continue;
                if (calculatePath.unit != null && calculatePath.unit.unitTeam == Unit.UnitTeam.enemyTeam) continue;

                List<PathNode> nearbyNode = GameFunctions.FindNodes_InRange1_HardCode(calculatePath, 7, igonreUnit);
                foreach (PathNode path in nearbyNode)
                {
                    if (path == null) continue;
                    chosedPath.Add(path);
                    if (allPath.Contains(path)) allPath.Remove(path);
                }
            }
        }

    
        List<PathNode> chosedPath_Final = new List<PathNode>();
      
        foreach (PathNode path in chosedPath)
        {
            if (chosedPath_Final.Contains(path)) continue;
            chosedPath_Final.Add(path);
        }
        return chosedPath_Final;
    }
    public static PathNode Get_TeleportPathnode_Size3(bool fixedYAxis)
    {
        GridManager GM = FindObjectOfType<GridManager>();

        PathNode[] array_allPath = FindObjectsOfType<PathNode>();
        List<PathNode> allPath = new List<PathNode>();

        foreach (Unit unit in GameController.playerList)
        {
            if (Random.Range(0, 1) == 0)
            {
                List<PathNode> nearbyNode = GameFunctions.FindNodes_InRange1_HardCode(unit.nodeAt, 7, true);

                foreach (PathNode path in nearbyNode)
                {
                    if (path == null) continue;
                    allPath.Add(path);
                }
            }
        }

     

        Debug.Log(allPath.Count);
        PathNode addone = GM.GetPath(7,6);
        if (addone != null) allPath.Add(addone);

        addone = GM.GetPath(6, 8);
        if (addone != null) allPath.Add(addone);

        addone = GM.GetPath(6, 9);
        if (addone != null) allPath.Add(addone);

        addone = GM.GetPath(5, 5);
        if (addone != null) allPath.Add(addone);

        addone = GM.GetPath(7, 9);
        if (addone != null) allPath.Add(addone);


        PathNode finalDir = null;
        GridManager grid = FindObjectOfType<GridManager>();

        while (allPath.Count > 1 || finalDir == null)
        {
            PathNode calculatePath = allPath[Random.Range(0, allPath.Count)];
            allPath.Remove(calculatePath);

            //Debug.Log("Random: calculatePath: " + calculatePath.x + "," + calculatePath.y);
            if (calculatePath == null) continue;
            if (fixedYAxis && calculatePath.y % 2 != 0) continue;
            if (!Check_IfBossCanTeleport(calculatePath, calculatePath, false)) continue;

            PathNode nodeTopRight = GameFunctions.FindNode_RightUp(calculatePath);
            if (nodeTopRight == null) continue;
            if (!Check_IfBossCanTeleport(nodeTopRight, calculatePath, false)) continue;

            PathNode nodeRight = GameFunctions.FindNode_Right(calculatePath);
            if (nodeRight == null) continue;
            if (!Check_IfBossCanTeleport(nodeRight, calculatePath, false)) continue;

            finalDir = calculatePath;
        }

        Debug.Log(finalDir);
        return finalDir;
    }

    public static bool Check_IfBossCanTeleport(PathNode path, PathNode startPath, bool ignoreUnit)
    {
        int heightDiff = path.height - startPath.height;
        if (Mathf.Abs(heightDiff) > 4) return false;
        if (path.unit != null) return false;
        if (path.isBlocked) return false;
        return true;
    }

    public static void AddBossToTeleportoNode(PathNode pathToGo, Unit thisUnit)
    {
        if (pathToGo == null) return;
        if(thisUnit.nodeAt!=null) thisUnit.nodeAt.unit = null;
        if (thisUnit.nodeAt_Right != null) thisUnit.nodeAt_Right.unit = null;
        if (thisUnit.nodeAt_Top != null) thisUnit.nodeAt_Top.unit = null;

       
        if(pathToGo != null)
        {
            thisUnit.nodeAt = pathToGo;
            if (pathToGo.unit != null) pathToGo.unit.HealthChange(-1000, 0, "damage");
            thisUnit.nodeAt.unit = thisUnit;
        }
        
        PathNode node2 = GameFunctions.FindNode_Right(pathToGo);
        if (node2 != null)
        {
            if (node2.unit != null) node2.unit.HealthChange(-1000, 0, "damage");
            thisUnit.nodeAt_Right = node2;
            node2.unit = thisUnit;
        }

        PathNode node3 = GameFunctions.FindNode_RightUp(pathToGo);
        if (node3 != null)
        {
            if (node3.unit != null) node3.unit.HealthChange(-1000, 0, "damage");
            thisUnit.nodeAt_Top = node3;
            node3.unit = thisUnit;
        }
        thisUnit.transform.position = pathToGo.transform.position + new Vector3(0.75f, 0, 0);
    }

    public static void Boss_KnockBack(Unit attacker, Unit defender, int distance, PathNode startNode)
    {
        if (attacker == null || defender == null) return;
        if (attacker == defender) return;
        if (attacker.nodeAt == defender.nodeAt) return;

        GridManager GM = GameObject.FindGameObjectWithTag("GameController").GetComponent<GridManager>();

        if(startNode == null) startNode = attacker.nodeAt;

        List<PathNode> knockBackNode = GameFunctions.FindNodes_InOneLine(startNode, defender.nodeAt, distance, true, true, false,true);
        defender.popTextString.Add(new textInformation("Knock-Back!", null));

        if (knockBackNode.Count > 1)
        {
            for (int i = 1; i < knockBackNode.Count; i++)
            {
                if (knockBackNode[i].unit != null)
                {
                    //If skill Hit some unit. 
                    PathNode toPath = knockBackNode[i];
                    defender.Input_CombatPos_StraightToStaying(toPath.transform.position, 1.2f, 0.4f, 0.1f);
                    SkillFunctions.Skill_SimpleDamageCalculate(i+1, defender, knockBackNode[i].unit, null);
                    break;
                }

                defender.UnitPosition_CanMove(knockBackNode[i], 0, false, 1f, false);
            }
        }

        else
        {
            Vector3 knockbackPos = UnitFunctions.Find_UnitShouldGoPosition_WhenKnochBack(attacker, defender);
            defender.Input_CombatPos(knockbackPos, 1f, 0.3f, 0.02f);
        }
    }
}


[System.Serializable]
public class NodeList
{
    public List<PathNode> node;
    public NodeList(List<PathNode> node)
    {
        this.node = node;
    }
}

