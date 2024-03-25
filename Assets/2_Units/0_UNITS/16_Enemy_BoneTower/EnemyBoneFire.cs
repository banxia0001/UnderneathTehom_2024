using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoneFire : MonoBehaviour
{
    public EnemyBoneFire_UI ebf;
    public int timer;
    private int order;
    public List<EnemyGeneTimer> enemyList;
    private List<PathNode> genePath;
    public GameObject vfxPrefab;
    [HideInInspector]public Unit unitToSpawn;
    public Animator anim;

    private void Start()
    {
        order = 0;
        unitToSpawn = enemyList[0].unitList[0].GetComponent<Unit>();
        timer = enemyList[0].timer;
        ebf.InsertUnit(unitToSpawn, timer);
    }

    private void GetGenePath()
    {
        genePath = GameFunctions.FindNodes_ByDistance(this.GetComponent<Unit>().nodeAt,1,true);
    }

    private IEnumerator DisactiveUnit(Unit unit)
    {
        yield return new WaitForSeconds(0.4f);
        unit.HealthChange(1, 0, "Heal");
        unit.GC.isAIThinking = false;
        unit.GC.isAttacking = false;
        unit.GC.isMoving = false;
        unit.UnitEnable(false);
    }
    public void CheckNewTurn(GameController GC)
    {
        Debug.Log("Timer:" + timer.ToString());
        Unit unit = this.GetComponent<Unit>();

        timer--;
        if (timer <= 0)
        {
            Debug.Log("Timer2:" + timer.ToString());
            GetGenePath();
            timer = enemyList[order].timer;
            StartCoroutine(SpawnUnit(enemyList[order].unitList, GC));
            order++;

            if (enemyList.Count == order)
            {
                order = 0;
            }
        }

        else
        {
            StartCoroutine(DisactiveUnit(unit));
        }

        unitToSpawn = enemyList[order].unitList[0].GetComponent<Unit>();
        ebf.InsertUnit(unitToSpawn, timer);
    }

    
    public IEnumerator SpawnUnit(List<GameObject> unitList, GameController GC)
    {
        GC.isAIThinking = true;
        GC.isAttacking = true;
        yield return new WaitForSeconds(0.2f);
        anim.SetTrigger("action");
        yield return new WaitForSeconds(0.24f);

        for (int i2 = 0; i2 < unitList.Count; i2++)
        {
            bool gene = false;
            for (int i = 0; i < genePath.Count; i++)
            {
                if (genePath[i].unit == null && genePath[i].isBlocked == false && !gene)
                {
                    gene = true;
                    yield return new WaitForSeconds(0.025f);
                    FindObjectOfType<SFX_Controller>().InputVFX_Simple(11);
                    Instantiate(vfxPrefab, genePath[i].transform.position, Quaternion.identity);
                    GameObject unitObject = Instantiate(unitList[i2], genePath[i].transform.position, Quaternion.identity);
                    //Debug.Log("1");
                    unitObject.gameObject.GetComponent<Unit>().nodeAt = genePath[i];
                    genePath[i].unit = unitObject.gameObject.GetComponent<Unit>();
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }


        GC.isAttacking = false;
        Unit unit = this.GetComponent<Unit>();
        StartCoroutine(DisactiveUnit(unit));
    }
}


[System.Serializable]
public class EnemyGeneTimer
{
    public List<GameObject> unitList;
    public int timer = 1;
}

