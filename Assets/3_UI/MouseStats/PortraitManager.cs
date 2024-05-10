using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortraitManager : MonoBehaviour
{
    public GameObject portraitBackImage;
    public GameObject rat_Minor, rat_Melee, rat_Spore;
    public GameObject rat_Archer, rat_BoneRunner,enemy_BoneTower, zombieRat, undeaArcher,undeadLancer;
    public GameObject Corpse, Nameless, Skel_Sword, Skel_Shield, Sentry_FB, Sentry_Turret, Sentry_HealTower, aoeT,boss,mage;

    public void CloseAll()
    {
        foreach (Transform child in this.transform)
        {
            child.gameObject.SetActive(false);
        }

    }
    public void UpdatePortrait(UnitData data)
    {
        CloseAll();

        if (portraitBackImage != null) portraitBackImage.SetActive(true);

        if (data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._11_Rat_BoneRunner) rat_BoneRunner.SetActive(true);
        if (data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._12_Rat_SporeFighter) rat_Spore.SetActive(true);
        if (data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._8_Rat_Shield) rat_Melee.SetActive(true);
        if (data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._7_Rat_Minor) rat_Minor.SetActive(true);
        if (data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._9_Rat_Archer) rat_Archer.SetActive(true);

        if (data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._0_Null) Corpse.SetActive(true);

        if (data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._1_Avatar) Nameless.SetActive(true);
        if (data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._5_Skel_GreatSword) Skel_Sword.SetActive(true);
        if (data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._4_Skel_Shield) Skel_Shield.SetActive(true);

        if (data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._13_Rev_Turret) Sentry_Turret.SetActive(true);
        if (data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._14_Rev_FleshBlock) Sentry_FB.SetActive(true);
        if (data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._15_Rev_HealTower) Sentry_HealTower.SetActive(true);
        if (data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._16_AliveBonfire) enemy_BoneTower.SetActive(true);
        if (data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._10_Rat_Zombie) zombieRat.SetActive(true);
        if (data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._6_Skel_Archer) undeaArcher.SetActive(true);
        if (data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._3_Skel_Lance) undeadLancer.SetActive(true);
        if (data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._17_BOSS) boss.SetActive(true);
        if (data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._18_MAGE) mage.SetActive(true);
        if (data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._19_AOE_Turret) aoeT.SetActive(true);
    }
}
