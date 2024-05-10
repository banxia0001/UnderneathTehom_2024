using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveData
{
    public static bool quickStart_FightBoss = false;
    public static int AreaAt = 0;
    public static bool firstEnterComat_LV_1_1 = true;

    public static int flesh;
    public static int heroMax = 2, sentryMax = 10, summonMax = 10;
    public static List<RewardList> rewardList;

    ///[Player data]
    public static UnitData namelessOneData = new UnitData(UnitFunctions.Get_UnitPrefab(UnitPrefabList.Unit_SpriteAsset_Type._1_Avatar).GetComponent<Unit>().data);
    public static List<UnitData> servantsData = new List<UnitData>();

    public static Transform mapdataTrans = null;
    public static void ImportPlayerteamDataToSave(GameController GC)
    {
        SaveData.servantsData = new List<UnitData>();
        List<UnitData> playerListNow = new List<UnitData>();

   

        for (int i = 0; i < GameController.heroList.Count; i++)
        {
            if (GameController.heroList[i] == null) continue;
            if(GameController.heroList[i].data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._1_Avatar)
            {
                namelessOneData = new UnitData(GameController.heroList[i].data);
                namelessOneData.healthNow = namelessOneData.healthMax;
                continue;
            }
            SaveData.servantsData.Add(GameController.heroList[i].data);
        }
    }
}
