using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class CampFire : MonoBehaviour
{
    public enum campFireType { none, playerSpawnPoint, enemySpawnPoint}
    public campFireType type;
    public Unit.UnitTeam campfireTeam;

    public PathNode nodeAt;
    public int controlZone = 1;
    public int healAmout = 1;

    [Header("Fire_Settings")]

    public GameObject popTextCanvas;
    public GameObject firePlayer,fireEnemy;
    public GameObject lightMapFolder;
    public Campfire_EnemySpawn CE;


    private IEnumerator GameSetUp()
    {
        yield return new WaitForSeconds(.05f);
        generateFire();
        generateControlZone();
    }

    public void Start()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.TransformDirection(new Vector3(0, 0, -1)), LayerMask.GetMask("Ground"));
        nodeAt = hit.collider.transform.gameObject.GetComponent<PathNode>();
        transform.position = nodeAt.gameObject.transform.position;
        nodeAt.campFire = this;
        nodeAt.isBlocked = true;
        StartCoroutine(GameSetUp());
    }


    public void CheckControl()
    {
        bool haveFriendlyUnitNear = false;
        bool haveEnemyUnitNear = false;
        Unit.UnitTeam comingEnemyTeam = Unit.UnitTeam.neutral;
        List<PathNode> nearby = GameFunctions.FindNodes_ByDistance(nodeAt, controlZone, false);

        for (int i = 0; i < nearby.Count; i++)
        {
            if (nearby[i].unit != null && nearby[i].unit.unitAttribute == Unit.UnitAttribute.alive && nearby[i].unit.unitTeam == campfireTeam) haveFriendlyUnitNear = true;
            if (nearby[i].unit != null && nearby[i].unit.unitAttribute == Unit.UnitAttribute.alive && nearby[i].unit.unitTeam != campfireTeam)
            {
                haveEnemyUnitNear = true;
                comingEnemyTeam = nearby[i].unit.unitTeam;
            }
        }

        if (haveFriendlyUnitNear == false && haveEnemyUnitNear)
        {
            campfireTeam = comingEnemyTeam;
            UIFunctions.Gene_PopText(this.popTextCanvas, new textInformation("Conquered!", null));
            generateControlZone();
            generateFire();
        }
    }

    public void CampfireCheck_healNearbyUnit(bool isEnemyTurn)
    {
        List<PathNode> nearby = GameFunctions.FindNodes_ByDistance(nodeAt, controlZone, false);
        List<Unit> myList = new List<Unit>();

        for (int i = 0; i < nearby.Count; i++)
        {
            if (nearby[i].unit != null && nearby[i].unit.unitAttribute == Unit.UnitAttribute.alive && nearby[i].unit.unitTeam == campfireTeam)
            {
                nearby[i].unit.HealthChange(healAmout, 0, "Heal");
                //myList.Add(nearby[i].unit);
            }
        }

        //if (myList != null && myList.Count != 0)
        //{
        //    Unit final = myList[Random.Range(0, myList.Count)];
        //    final.HealthChange(healAmout, 0, "Heal");
        //}
      
        if (isEnemyTurn)
        {
            CE.CheckNewTurn();
        }
    }

    public void geneCommandPoint()
    {
        string text = "+ 1";
        Sprite sprite = Resources.Load<Sprite>("Image/Star");
        UIFunctions.Gene_PopText(this.popTextCanvas, new textInformation(text, sprite));
    }


    public void generateFire()
    {
        if (campfireTeam == Unit.UnitTeam.playerTeam) { firePlayer.SetActive(true); fireEnemy.SetActive(false); }
        if (campfireTeam == Unit.UnitTeam.enemyTeam) { firePlayer.SetActive(false); fireEnemy.SetActive(true); }
    }
    public void generateControlZone()
    {
        if (lightMapFolder != null)
            Destroy(lightMapFolder);

        if (campfireTeam == Unit.UnitTeam.neutral) 
            return;

        GameObject MyTransform = Instantiate(Resources.Load<GameObject>("GridPrefab/Folder"), new Vector3(0, 0, 0), Quaternion.identity);
        MyTransform.name = "Light Icon Folder";
        MyTransform.transform.parent = gameObject.transform;

        lightMapFolder = MyTransform;

        List<PathNode> nearby = GameFunctions.FindNodes_ByDistance(nodeAt, controlZone, false);

        GameObject LightMap = null;
        if (campfireTeam == Unit.UnitTeam.playerTeam)
            LightMap = Resources.Load<GameObject>("GridPrefab/FireFriendly");

        if (campfireTeam == Unit.UnitTeam.enemyTeam) 
            LightMap = Resources.Load<GameObject>("GridPrefab/FireEnemy");

        for (int i = 0; i < nearby.Count; i++)
        {
            if (nearby[i] == nodeAt) continue;
            GameObject LightMapG = Instantiate(LightMap, nearby[i].transform.position, Quaternion.identity);
            LightMapG.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingLayerName = "Default";
            LightMapG.transform.parent = MyTransform.transform;
        }
    }
}
