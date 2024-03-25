using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Saveload_System : MonoBehaviour
{
    public List<RewardList> reward;
    public List<Vector2Int> Vectors_SpawnArea;
    public void EndBattle()
    {
        if (GameController.frozenGame) return;
        GameController.frozenGame = true;
        StartCoroutine(EndBattle_2());
    }
    private IEnumerator EndBattle_2()
    {
        SaveData.firstEnterComat_LV_1_1 = false;
        SaveData.ImportPlayerteamDataToSave(FindObjectOfType<GameController>());

        GameController GC = FindObjectOfType<GameController>();
        GC.UI.loadingPanel.SetBool("Trigger", true);

        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(2);
    }

    public void StartBattle_SpawnPlayer(GridManager GM)
    {
        PathNode path = GM.GetPath(Vectors_SpawnArea[0].x, Vectors_SpawnArea[0].y);
        SpawnUnit_ByNameTag(SaveData.namelessOneData, path);

        if (SaveData.servantsData == null) return;

        for (int i = 0; i < SaveData.servantsData.Count; i++)
        {
            if (SaveData.servantsData[i] == null) continue;

            for (int i2 = 0; i2 < Vectors_SpawnArea.Count; i2++)
            {
                path = GM.GetPath(Vectors_SpawnArea[i].x, Vectors_SpawnArea[i].y);
                if (path != null && path.unit == null && path.isBlocked == false)
                {
                    SpawnUnit_ByNameTag(SaveData.servantsData[i], path);
                    break;
                }
            } 
        }
    }

    public void SpawnUnit_ByNameTag(UnitData data, PathNode node)
    {
        Debug.Log(node);
        Debug.Log(data);
        GameObject unitPrefab = UnitFunctions.Get_UnitPrefab(data.unitSpriteAssetType);
        GameObject unitObject = Instantiate(unitPrefab, node.transform.position, Quaternion.identity);
        Unit unit = unitObject.GetComponent<Unit>();
        unit.InputData(data);
    }
}
