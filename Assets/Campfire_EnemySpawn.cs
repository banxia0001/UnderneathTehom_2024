using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campfire_EnemySpawn : MonoBehaviour
{
    public int timer;
    public int order;
    public int timerMax;
    public List<GameObject> enemyList;

    public CampFire camp;


    public void CheckNewTurn()
    {
        timer--;
        if (timer < 0)
        {
            timer = timerMax;

            GameObject ob = enemyList[order];
            order++;
            if (enemyList.Count == order) order = 0;

            SpawnUnit(ob);
        }
    
    }

    public void SpawnUnit(GameObject ob)
    {
        List<PathNode> spawnNode = GameFunctions.FindNodes_OneGridNearby(camp.nodeAt, false);

        for (int i = 0; i < spawnNode.Count; i++)
        {
            if (spawnNode[i].unit == null && spawnNode[i].isBlocked == false)
            {
                GameObject unitObject = Instantiate(ob, spawnNode[i].transform.position, Quaternion.identity);
                break;
            }
        }
    }

}
