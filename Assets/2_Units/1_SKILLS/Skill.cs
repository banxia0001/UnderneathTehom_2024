using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class _Skill
{
    public Skill skill;
    public int CD = 0;

    public _Skill(Skill skill, int CD)
    {
        this.skill = skill;
        this.CD = CD;
    }
}

[System.Serializable]
public class _SkillAI
{
    public Skill skill;
    public enum _SkillPriority {Heal, Summon, AttackHighArmor_Range, AttackHighArmor_Melee, AttackClosest_Melee, BuffSelf, TargetAllUnit }
    public _SkillPriority skillPriority;


    public enum _SkillSpeicalFunction{ Normal, Hold_n_Charge, Hold_n_Cast }
    public _SkillSpeicalFunction skillSpecialFunction;

}

[CreateAssetMenu(menuName = "ScriptObject/Skill")]
public class Skill : ScriptableObject
{
    [Header("BaseAttribute")]
    public Sprite skillSprite;
    public int CD = 1;
    public int Cost = 1;

    [TextArea(10,10)]
    public string description;
    public ListPanelUI.SlotInfo slotInfo = ListPanelUI.SlotInfo.minor;

    public enum _Type { 
        TargetToAttack,
        MoveInLine,
        TargetToBuff, 
        AttackALine, 
        Summon,
        SummonFromDeath,
        TeleportToTargetAndAttackRoad, 
        Reload,
        BiteTheDeath,
        TargetAllUnit,
        TargetToDebuff, 
        ExchangePosition,
        SummonFromDeath_Type2,
        SelfDestroy,
        Teleport,
        TargetTile,
    }

    public _Type type;
    public bool causeGridShock;
    public enum _DamageType { useDam,usePow,none}
    public _DamageType damageType;

    public enum _AnimTriggerType { idle,attack,attack2,use,cast,cast2, taunt,gainBuff,shieldAttack, charge,chant2,revival_3, enterBattle }
    public _AnimTriggerType animTriggerType;

    public bool isUsingWeapon = true;
    public bool isFriendly = false;
    public bool isSelfUse = false;
    public bool isInstant = false;

    [Header("PerformAction")]
    [Range(0f, 2f)]
    public float timer_PerformAction = 0.12f;
    public GameObject castSpecialEffect;

    [Header("Attacking")]
    [Range(0f, 2f)]
    public float timer_AttackHit = 0.12f;
    public GameObject hitSpecialEffect;

    [Header("WaitingAfterAttack")]
    [Range(0f, 2f)]
    public float timer_WaitAfterAttack = 0.35f;

    [Header("Melee Moving")]
    [Range(0.2f, 2f)]
    public float moveToGrid_SpeedRatio = 1f;

    [Range(0.2f, 0.9f)]
    public float moveToGrid_DistanceRatio = 0.5f;

    [Header("Projectile")]
    public GameObject projectile;
    public bool isCurveProjectile;
    public float flyingSpeed = 20;

    [Header("Buff")]
    public bool buffSelf;
    public Buff buff;
    public bool causeHeal = false;
    public int addArmor;

    [Header("DamageValue")]
    public int num = 1;
    public int damMin;
    public int damMax;

    [Header("ArmorBreak")]
    public int armorSunder;
    public int armorMgSunder;

    [Header("Splash")]
    public bool castAsCirclePoint = false;
    public int splashRange = 0;

    [Header("Hit")]
    public int range = 1;
    public bool autoHit = false;
    public int hitBonus = 0;

    [Header("SpecialEffect")]
    public bool causeKnockBack = false;
    public int KnockBack_Distance = 2;

    [Header("Summon")]
    public bool SummonDeath = false;
    public GameObject[] SommonBeing;

    [Header("Random Effect")]
    public Buff[] randomBuffs;


    public enum RevivalOption { meatBlock, turret, healTower, skelSword, zombieRat, aoeTurret }
    [Header("Summon From Death Type2")]
    public RevivalOption revivalOption;
    public GameObject revivalPrefab;
}
