using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptObject/Buff")]
public class Buff : ScriptableObject
{
    public Sprite sprite;
    [TextArea (10,10)]
    public string description;
    public enum _buffType { stateChange, tuant }
    public _buffType buffType;
    public Unit tuantTarget;
    public bool isBuff = true;
    public bool canRepeatBuff = true;
    public int remainTime;
    
    
    [Header("State")]
    public int healthMax = 0;
    public int dam = 0;
    public int hit = 0;
    public int lifesteal = 0;
    public int armorSunder;
    public int armorNegative = 0;
    public int armorMax = 0;
    public int dodge = 0;

    public int power = 0;
    public int MR;
    public int SR = 0;
    public int movePointMax = 0;
    public int gain_FP_per_Turn;
    public int gain_Fp_EnterBattle;



    [Header("Bools")]
    public bool canRepel;
    public bool expertHeal;
    public bool giantSlayer;
    public bool standAlone;

    public bool isCurse;
    public bool cannotBeKnockBack;
    public bool auto_Attack, auto_Heal, auto_Taunt;

    [Header("Tags")]
    public bool tag_Undead;
    public bool tag_Rat;

    [Header("Aura")]
    public bool aura_GrowingDeath;
    public bool aura_Tarish;

    [Header("Terrain")]
    public bool standOn_Water;


    [Header("Special")]
    public int dodgeRate_FacingRange;
    public int dam_InHightGround;
    public int dam_ToCursedUnit;
    public int gain_Fp_Kill;


    [Header("Stats Change")]
    public _changePerTurnType changePerTurnType;
    public int changeNum;
    public enum _changePerTurnType { None, Poisoning, Bleeding, Healing, Rusting, GrowingArmor}

}

[System.Serializable]
public class _Buff 
{
    public int level;
    public int remainTime;
    public Buff buff;
    public Unit tuantTarget;
    public _Buff(Buff buff)
    {
        this.buff = buff;
        this.remainTime = buff.remainTime;
        if (buff.buffType == Buff._buffType.tuant)
            this.tuantTarget = buff.tuantTarget;
    }
}
