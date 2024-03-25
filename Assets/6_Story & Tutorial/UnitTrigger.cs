using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTrigger : MonoBehaviour
{
    public int num;

    public bool dontGeneIfPlayerHave1ZombieRat;
    public bool GeneIfPlayerHave0ZombieRat;

    public void Update()
    {
        if (GeneIfPlayerHave0ZombieRat)
        {
            int i = 0;
            foreach (Unit unit in GameController.playerList)
            {
                if (unit.data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._10_Rat_Zombie) i++;
            }
            if (i == 0)
            {
                this.GetComponent<Unit>().deathData.deathType = UnitDeathOption.Unit_Death_Type.alwaysGeneBody;
                Destroy(this);
            }
        }


        if (GameController.enemyList != null)
            if (GameController.enemyList.Count <= num)
            {
                Change();
            }
    }

    private void Change()
    {
        if (dontGeneIfPlayerHave1ZombieRat)
        {
            int i = 0;
            foreach (Unit unit in GameController.playerList)
            {
                if (unit.data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._10_Rat_Zombie) i++;
            }
            if (i >= 1)
            {
                Destroy(this);
                return;
            }
        }

        this.GetComponent<Unit>().deathData.deathType = UnitDeathOption.Unit_Death_Type.alwaysGeneBody;
        Destroy(this);
    }
}
