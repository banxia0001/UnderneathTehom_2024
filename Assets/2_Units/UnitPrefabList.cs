using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LocalData/UnitPrefabList")]
public class UnitPrefabList : ScriptableObject
{
    public enum Unit_SpriteAsset_Type
    {
        _0_Null = 0,
        _1_Avatar = 1,

        _2_Skel_Snake = 2,
        _3_Skel_Lance = 3,
        _4_Skel_Shield = 4,
        _5_Skel_GreatSword = 5,
        _6_Skel_Archer = 6,

        _7_Rat_Minor = 7,
        _8_Rat_Shield = 8,
        _9_Rat_Archer = 9,
        _10_Rat_Zombie = 10,

        _11_Rat_BoneRunner = 11,
        _12_Rat_SporeFighter = 12,

        _13_Rev_Turret = 13,
        _14_Rev_FleshBlock = 14,
        _15_Rev_HealTower = 15,
        _16_AliveBonfire = 16,

        _17_BOSS = 17,
        _18_MAGE = 18,
        _19_AOE_Turret = 19,

    }

    public List<UnitPrefab> prefabs;
    public List<UnitPrefab_Corpse> corpsePrefabs;
}

[System.Serializable]
public class UnitPrefab
{
    public UnitPrefabList.Unit_SpriteAsset_Type type;
    public List<GameObject> prefab_Variations;
}

[System.Serializable]
public class UnitPrefab_Corpse
{
    public enum Unit_Corpse_Type
    {
        none = 0,
        humanoid = 1,
        rat = 2,
    }
    public Unit_Corpse_Type type;
    public List<GameObject> prefab_Variations;
}


