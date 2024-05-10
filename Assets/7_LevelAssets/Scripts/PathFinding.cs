using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    public static void ClearAllNodes()
    {
        GridManager grid = GameObject.FindGameObjectWithTag("GameController").GetComponent<GridManager>();
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
               
                PathNode pathNode = grid.GetPath(x, y);
                if (pathNode != null && pathNode.enabled)
                {
                    pathNode.gCost = 999999;
                    pathNode.CalculateFCost();
                    pathNode.cameFromNode = null;
                }
            }
        }
    }

    public static List<PathNode> FindPath(PathNode startNode, PathNode endNode, Unit unit, Unit targetAttackUnit, bool igoreUnit, bool igoreCorpse, int refleshRange, bool igoreDestinationObstancle)
    {
        if (startNode == null || endNode == null)
        {
            return null;
        }

        List<PathNode> openList = new List<PathNode> { startNode };
        List<PathNode> closedList = new List<PathNode>();

        if (refleshRange <= 1)
        {
            ClearAllNodes();
        }

        else
        {
            List<PathNode> clearList = GameFunctions.FindNodes_ByDistance(startNode, refleshRange, true);

            foreach (PathNode path in clearList)
            {
                path.gCost = 99999999;
                path.CalculateFCost();
                path.cameFromNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();


        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(openList);
            if (currentNode == endNode)
            {
                return CalculatePath(endNode);
            }
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (PathNode neighbourNode in GetNeighbourList(currentNode))
            {
                if(neighbourNode == null) continue;
                if (closedList.Contains(neighbourNode)) continue;

                int sizeX = neighbourNode.x;
                int sizeY = neighbourNode.y;

                //If blocked
                if (neighbourNode.isBlocked)
                {
                    if (neighbourNode == endNode && igoreDestinationObstancle)
                    { 
                    }
                    else
                    {
                        closedList.Add(neighbourNode);
                        continue;
                    }
                }

                int heightDiff = neighbourNode.height - currentNode.height;

                if (heightDiff > 3 || heightDiff < -3)
                {
                    //should't add it to closedList
                    continue;
                }

                //If grid contain enemy, if yes, if it's the enemy this unit gonna attack?
                if (neighbourNode.unit != null && neighbourNode.unit != unit && igoreUnit == false && 
                    neighbourNode.unit != targetAttackUnit && neighbourNode.unit.unitAttribute == Unit.UnitAttribute.alive)
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                //If contain corpse
                if (neighbourNode.unit != null && neighbourNode.unit.unitAttribute != Unit.UnitAttribute.alive && !igoreCorpse && neighbourNode.unit != targetAttackUnit)
                {
                    closedList.Add(neighbourNode);
                    continue;
                }



                //[Unit will larger size]
                bool checkLeftNode = false;
                bool checkRightNode = false;
                bool checkLeftUpNode = false;
                bool checkRightUpNode = false;
                bool checkLeftDownNode = false;
                bool checkRightDownNode = false;

                if (unit.data.unitSize == UnitData.Unit_Size.size1) { }
                if (unit.data.unitSize == UnitData.Unit_Size.size2_OnLeftNode) { checkRightNode = true; }
                if (unit.data.unitSize == UnitData.Unit_Size.size2_OnRightNode) { checkLeftNode = true; }

                if (unit.data.unitSize == UnitData.Unit_Size.size3_OnLeftNode) { checkRightNode = true; checkRightUpNode = true; }
                if (unit.data.unitSize == UnitData.Unit_Size.size3_OnRightNode) { checkLeftNode = true; checkLeftUpNode = true; }
                if (unit.data.unitSize == UnitData.Unit_Size.size3_OnTopNode) { checkRightDownNode = true; checkLeftDownNode = true; }

                GridManager grid = GameObject.FindGameObjectWithTag("GameController").GetComponent<GridManager>();

                if (checkLeftNode)
                {
                    //[Cannot Walk cross the grid]
                    PathNode nodeNearby = grid.GetPath(sizeX - 1, sizeY);

                    if (nodeNearby == null){ closedList.Add(neighbourNode); continue;}
                    if (nodeNearby.isBlocked) { closedList.Add(neighbourNode); continue; }
                    if (nodeNearby.unit != null && nodeNearby.unit != unit) { closedList.Add(neighbourNode); continue; }

                    int heightDiff_2 = neighbourNode.height - nodeNearby.height;
                    if (heightDiff_2 > 3 || heightDiff_2 < -3) { closedList.Add(neighbourNode); continue; }
                }
                if (checkRightNode)
                {
                    //[Cannot Walk cross the grid]
                    PathNode nodeNearby = grid.GetPath(sizeX + 1, sizeY);

                    if (nodeNearby == null) { closedList.Add(neighbourNode); continue; }
                    if (nodeNearby.isBlocked) { closedList.Add(neighbourNode); continue; }
                    if (nodeNearby.unit != null && nodeNearby.unit != unit) { closedList.Add(neighbourNode); continue; }

                    int heightDiff_2 = neighbourNode.height - nodeNearby.height;
                    if (heightDiff_2 > 3 || heightDiff_2 < -3) { closedList.Add(neighbourNode); continue; }
                }
                if (checkLeftUpNode)
                {
                    //[Cannot Walk cross the grid]
                    PathNode nodeNearby = GameFunctions.FindNode_LeftUp(neighbourNode);

                    if (nodeNearby == null) { closedList.Add(neighbourNode); continue; }
                    if (nodeNearby.isBlocked) { closedList.Add(neighbourNode); continue; }
                    if (nodeNearby.unit != null && nodeNearby.unit != unit) { closedList.Add(neighbourNode); continue; }

                    int heightDiff_2 = neighbourNode.height - nodeNearby.height;
                    if (heightDiff_2 > 3 || heightDiff_2 < -3) { closedList.Add(neighbourNode); continue; }
                }

                if (checkRightUpNode)
                {
                    //[Cannot Walk cross the grid]
                    PathNode nodeNearby = GameFunctions.FindNode_RightUp(neighbourNode);

                    if (nodeNearby == null) { closedList.Add(neighbourNode); continue; }
                    if (nodeNearby.isBlocked) { closedList.Add(neighbourNode); continue; }
                    if (nodeNearby.unit != null && nodeNearby.unit != unit) { closedList.Add(neighbourNode); continue; }

                    int heightDiff_2 = neighbourNode.height - nodeNearby.height;
                    if (heightDiff_2 > 3 || heightDiff_2 < -3) { closedList.Add(neighbourNode); continue; }
                }
                if (checkLeftDownNode)
                {
                    //[Cannot Walk cross the grid]
                    PathNode nodeNearby = GameFunctions.FindNode_LeftDown(neighbourNode);

                    if (nodeNearby == null) { closedList.Add(neighbourNode); continue; }
                    if (nodeNearby.isBlocked) { closedList.Add(neighbourNode); continue; }
                    if (nodeNearby.unit != null && nodeNearby.unit != unit) { closedList.Add(neighbourNode); continue; }

                    int heightDiff_2 = neighbourNode.height - nodeNearby.height;
                    if (heightDiff_2 > 3 || heightDiff_2 < -3) { closedList.Add(neighbourNode); continue; }
                }
                if (checkRightDownNode)
                {
                    //[Cannot Walk cross the grid]
                    PathNode nodeNearby = GameFunctions.FindNode_RightDown(neighbourNode);

                    if (nodeNearby == null) { closedList.Add(neighbourNode); continue; }
                    if (nodeNearby.isBlocked) { closedList.Add(neighbourNode); continue; }
                    if (nodeNearby.unit != null && nodeNearby.unit != unit) { closedList.Add(neighbourNode); continue; }

                    int heightDiff_2 = neighbourNode.height - nodeNearby.height;
                    if (heightDiff_2 > 3 || heightDiff_2 < -3) { closedList.Add(neighbourNode); continue; }
                }
              

                //[Calculate Cost]
                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);

                if (tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.cameFromNode = currentNode;
                   
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }

        return null;
    }

    public static PathNode GetNode(int x, int y)
    {
        GridManager grid = GameObject.FindGameObjectWithTag("GameController").GetComponent<GridManager>();
        return grid.GetPath(x, y);
    }


    private static List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();
        path.Add(endNode);
        PathNode currentNode = endNode;
        while (currentNode.cameFromNode != null)
        {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }
        path.Reverse();
        return path;
    }

    public static List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        GridManager grid = GameObject.FindGameObjectWithTag("GameController").GetComponent<GridManager>();
        List<PathNode> neighbourList = new List<PathNode>();

        if (currentNode.y % 2 == 0)
        {
            if (currentNode.y - 1 >= 0 && currentNode.x - 1 >= 0 && grid.pathArray[currentNode.x - 1, currentNode.y - 1] != null) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y - 1));
            if (currentNode.y + 1 < grid.GetHeight() && currentNode.x - 1 >= 0 && grid.pathArray[currentNode.x - 1, currentNode.y + 1] != null) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y + 1));
        }
        else
        {
            if (currentNode.y + 1 < grid.GetHeight() && currentNode.x + 1 < grid.GetWidth() && grid.pathArray[currentNode.x + 1, currentNode.y + 1] != null) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y + 1));
            if (currentNode.y - 1 >= 0 && currentNode.x + 1 < grid.GetWidth() && grid.pathArray[currentNode.x + 1, currentNode.y - 1] != null) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y - 1));
        }

        if (currentNode.y - 1 >= 0 && grid.pathArray[currentNode.x, currentNode.y - 1] != null) neighbourList.Add(GetNode(currentNode.x, currentNode.y - 1));
        if (currentNode.x - 1 >= 0 && grid.pathArray[currentNode.x - 1, currentNode.y] != null) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y));
        if (currentNode.y + 1 < grid.GetHeight() && grid.pathArray[currentNode.x, currentNode.y + 1] != null) neighbourList.Add(GetNode(currentNode.x, currentNode.y + 1));
        if (currentNode.x + 1 < grid.GetWidth() && grid.pathArray[currentNode.x + 1, currentNode.y] != null) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y));

        return neighbourList;
    }


    private static PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostNode = pathNodeList[0];

        for (int i = 1; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = pathNodeList[i];
            }
        }
        return lowestFCostNode;
    }

    private static int CalculateDistanceCost(PathNode a, PathNode b)
    {
        float num = 10 * GameFunctions.CalculateDis_WithoutY(a, b) / 1.73f;
        return (int)num;
    }  
}
