using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatUI_PortraitSelection : MonoBehaviour
{
    public GameObject por_Nameless, por_Skel_Archer, por_Skel_GreatS, por_Skel_Shield, por_Zombie_Rat, por_Lancer;
    public GameObject por_Rat,por_Rat_Ranger,por_Rat_Iron,por_Rat_Spore,por_Rat_BoneRunner;

    public void InpuUnit(Unit SelectedUnit)
    {
        por_Nameless.SetActive(false);
        por_Skel_Archer.SetActive(false);
        por_Skel_Shield.SetActive(false);
        por_Skel_GreatS.SetActive(false);
        por_Zombie_Rat.SetActive(false);
        por_Lancer.SetActive(false);
        por_Rat_Ranger.SetActive(false);
        por_Rat_Iron.SetActive(false);
        por_Rat_Spore.SetActive(false);
        por_Rat.SetActive(false);
        por_Rat_BoneRunner.SetActive(false);

        if (SelectedUnit.data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._1_Avatar) { por_Nameless.SetActive(true); }
        if (SelectedUnit.data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._4_Skel_Shield) { por_Skel_Shield.SetActive(true); }
        if (SelectedUnit.data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._5_Skel_GreatSword) { por_Skel_GreatS.SetActive(true); }
        if (SelectedUnit.data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._6_Skel_Archer) { por_Skel_Archer.SetActive(true); }
        if (SelectedUnit.data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._10_Rat_Zombie) { por_Zombie_Rat.SetActive(true); }
        if (SelectedUnit.data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._3_Skel_Lance) { por_Lancer.SetActive(true); }
        if (SelectedUnit.data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._11_Rat_BoneRunner) { por_Rat_BoneRunner.SetActive(true); }
        if (SelectedUnit.data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._12_Rat_SporeFighter) { por_Rat_Spore.SetActive(true); }
        if (SelectedUnit.data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._7_Rat_Minor) { por_Rat.SetActive(true); }
        if (SelectedUnit.data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._8_Rat_Shield) { por_Rat_Iron.SetActive(true); }
        if (SelectedUnit.data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._9_Rat_Archer) { por_Rat_Ranger.SetActive(true); }
    }
}
