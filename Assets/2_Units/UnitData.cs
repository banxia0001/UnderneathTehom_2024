using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class UnitData
{
    public string Name;
    //public string Order;
    public UnitPrefabList.Unit_SpriteAsset_Type unitSpriteAssetType;

    public enum Unit_Type { Warrior,Hunter,Knight,Beast,Priest,Magician,Assasin,Fallen}
    public Unit_Type unitType;

    public enum Unit_Size { size1,size2_OnLeftNode, size2_OnRightNode, size3_OnLeftNode, size3_OnRightNode, size3_OnTopNode }
    public Unit_Size unitSize;


    public int healthMax = 10;
    public int healthNow = 10;

    public int movePointMax = 3;
    public int movePointNow = 3;

    [Header("Weapon")]
    public UnitWeapon damage;

    [Header("Defend")]
    public int armorNow = 2;
    public int armorMax = 2;
    public int dodge = 5;
    public int FleshDrop = 1;

    [Header("Trait")]
    public List<_Buff> traitList;
    public List<_Skill> Skill;

    [Header("Magic")]
    public int power = 0;
    public int MR = 0;
    public int SR = 0;

    public UnitData(UnitData data)
    {
        this.Name = data.Name;

        this.unitType = data.unitType;
        this.unitSize = data.unitSize;
        this.unitSpriteAssetType = data.unitSpriteAssetType;

        this.healthMax = data.healthMax;
        this.healthNow = data.healthNow;
        this.movePointMax = data.movePointMax;
        this.movePointNow = data.movePointNow;

        this.damage = new UnitWeapon(data.damage);

        this.armorNow = data.armorNow;
        this.armorMax = data.armorMax;
        this.dodge = data.dodge;
        this.MR = data.MR;
        this.power = data.power;
        this.SR = data.SR;

        this.traitList = data.traitList;
        this.Skill = data.Skill;

        if (this.Skill != null && this.Skill.Count != 0)
        {
            foreach (_Skill theskill in this.Skill)
            {
                theskill.CD = 0;
            }
        }
           

        this.FleshDrop = data.FleshDrop;
    }
}



[System.Serializable]
public class UnitDeathOption
{
    public enum Unit_Death_Type { neverGeneBody, alwaysGeneBody, chanceToGeneBody }
    public Unit_Death_Type deathType;
    public UnitPrefab_Corpse.Unit_Corpse_Type bodyType;
    public int rate_GeneCorpse = 10;
    public GameObject specialPrefab;
}

[System.Serializable]
public class UnitWeapon
{
    [Header("Physical Damage")]
    public int range = 1;
    public int damMin = 2;
    public int damMax = 6;
    public int damBonus = 1;

    public int num = 1;
    public int hit = 95;

    public int armorSunder = 0;

    public bool aoeEffect  = false;



    [Header("PerformAction")]
    [Range(0f, 2f)]
    public float timer_PerformAction = 0.12f;
    public GameObject castSpecialEffect;

    [Header("Attacking")]
    [Range(0f, 1f)]
    public float timer_AttackHit = 0.12f;
    public GameObject hitSpecialEffect;

    [Header("WaitingAfterAttack")]
    [Range(0f, 2f)]
    public float timer_WaitAfterAttack = 0.12f;

    [Header("Melee Moving")]
    [Range(0.2f, 2f)]
    public float moveToGrid_SpeedRatio = 1f;

    [Range(0.2f, 0.9f)]
    public float moveToGrid_DistanceRatio = 0.5f;





    public UnitWeapon(UnitWeapon dam)
    {
        // [Header("Physical Damage")]
        range = dam.range;
        damMin = dam.damMin;
        damMax = dam.damMax;
        damBonus = dam.damBonus;

        num = dam.num;
        hit = dam.hit;
        armorSunder = dam.armorSunder;

        //[Header("PerformAction")]
        timer_PerformAction = dam.timer_PerformAction;
        castSpecialEffect = dam.castSpecialEffect;

        //[Header("Attacking")]
        timer_AttackHit = dam.timer_AttackHit;
        hitSpecialEffect = dam.hitSpecialEffect;

        //[Header("WaitingAfterAttack")]
        timer_WaitAfterAttack = dam.timer_WaitAfterAttack;

        //[Header("Melee Moving")]
        moveToGrid_SpeedRatio = dam.moveToGrid_SpeedRatio;
        moveToGrid_DistanceRatio = dam.moveToGrid_DistanceRatio;
        aoeEffect = dam.aoeEffect;
    }
}



