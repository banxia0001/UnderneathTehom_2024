using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX_Controller : MonoBehaviour
{
    public enum VFX
    {
        none,
        flame,
        shield,
        arrow,
        hit,
        hit2,
        hit3,
        heal,
        groundSound,
        hitShield,
        roat_Minor,
        roat_Banshee,
        roat_Great,
        eat_Hit,
        revival,
        ding,
        gainFlesh,

        rat_Minor_Scream,
        rat_Minor_Attack,

        rat_Great_Roar,
        rat_Great_Buff,
        rat_Great_Attack,
        rat_Great_PoisonSpread,
        rat_Great_Hurt,

        rat_Ranger_Shoot,
        rat_Ranger_Hurt,
        rat_Ranger_Scream,

        rat_ChargePepare,
        rat_ChargeHit,

        rat_Minor_JumpTo,
        rat_Minor_Roar,
    }
    public VFX VFX_Sample;
    public GameController GC;
  
    public AudioSource ClickSound;
    public AudioSource HitSound;
    public AudioSource SkillSoundA;
    public AudioSource SkillSoundB;
    public AudioSource SkillSoundC;
    public AudioSource SkillSoundD;
    public AudioSource SkillSoundE;
    public AudioSource SkillSoundF;
    public AudioSource SkillSoundG;
    public AudioSource SkillSoundH;
    public AudioSource SkillSoundI;
    public AudioSource SkillSoundJ;
    public AudioSource SkillSoundK;
    public AudioSource SkillSoundL;
    public AudioSource SkillSoundM;
    public AudioSource SkillSoundN;
    public AudioSource SkillSound_0;
    public AudioSource SkillSound_1;
    public AudioSource SkillSound_2;
    public AudioSource SkillSound_3;
    public AudioSource SkillSound_4;
    public AudioSource Warming;
    public AudioSource hit2,hit3;


    public AudioSource RockSound_A;
    public AudioSource[] RockSound;

    public Animator moveSound;

    [Header("BGM")]
  
    public AudioSource BGM_1;
    public Animator BGM_1_Add;

    [Header("Addone")]
    public AudioClip Shield;
    public AudioClip Spell_Fire;
    public AudioClip arrow;
    public AudioClip hit;
    public AudioClip hit_Ground;
    public AudioClip heal;
    public AudioClip roar_MinorEnemy;
    public AudioClip roar_Banshee;
    public AudioClip roar_GreatEnemy;
    public AudioClip eatSound;
    public AudioClip revival;
    public AudioClip ding;

    public AudioSource EatSound_Loop;


    public AudioClip obsorbFlesh;

    [Header("Normal_SFX")]
    public AudioClip hit_Shield;

    [Header("Rats_SFX")]
    public AudioClip rat_Minor_Scream;
    public AudioClip rat_Minor_Attack;

    public AudioClip rat_Great_Roar;
    public AudioClip rat_Great_Buff;

    public AudioClip rat_Great_Hurt;
    public AudioClip rat_Great_Attack;
    public AudioClip rat_Great_PoisonSpread;

    public AudioClip rat_Ranger_Shoot;
    public AudioClip rat_Ranger_Hurt;
    public AudioClip rat_Ranger_Scream;
    public AudioClip chargePerpare, chargeHit;

    public AudioClip rat_JumpToGround, rat_MinorRoar;
    public AudioClip summon_1, summon_2, summon_3, summon_4;

    public AudioClip tuantEffect, tuantHit, bossFireball;


    public void SwitchBGM(bool activeCombat)
    {
        if (activeCombat)
        {
            BGM_1_Add.SetBool("active", true);
        }
        else BGM_1_Add.SetBool("active", false);
    }
    public void Click()
    {
        ClickSound.Play();
    }
    public void Hit()
    {
        HitSound.Play();
    }
    public void StartRockFall()
    {
        StartCoroutine(_RockFall());
    }

    public void StartRockFall_Initial()
    {
        RockSound_A.Play();
    }



    public IEnumerator _RockFall()
    {
        RockSound[0].Play();
        yield return new WaitForSeconds(Random.Range(0.3f, 0.6f));
        RockSound[1].Play();
    }


    [Header("SimpleSound")]
    public AudioClip nameless_Hit;
    public AudioClip nameless_GainBuff;
    public AudioClip nameless_Death;
    public AudioClip nameless_CastDown;
    public AudioClip nameless_Awake;
    public AudioClip nameless_Attack;

    public AudioClip turret_Hit,turret_EnterBattle,turret_Attack;
    public AudioClip healTower_Hurt,healTower_Heal;

    [Header("BossSound")]
    public AudioClip slash_1;
    public AudioClip slash_2,boss_throw, Boss_CastLoop, Boss_SwordInsert, Boss_Falling, Boss_Death, Boss_Wind, Boss_Hurt, ratMage_Cast, ratMage_Cast2,boss_Warming;

    public void InputVFX_Boss(int order)
    {
        AudioSource SkillSound = SkillSoundA;
        if (SkillSoundA.isPlaying) SkillSound = SkillSoundB;
        if (SkillSoundB.isPlaying) SkillSound = SkillSoundC;
        if (SkillSoundC.isPlaying) SkillSound = SkillSoundD;
        if (SkillSoundD.isPlaying) SkillSound = SkillSoundE;
        if (SkillSoundE.isPlaying) SkillSound = SkillSoundF;
        if (SkillSoundF.isPlaying) SkillSound = SkillSoundG;
        if (SkillSoundG.isPlaying) SkillSound = SkillSoundH;
        if (SkillSoundH.isPlaying) SkillSound = SkillSoundI;
        if (SkillSoundI.isPlaying) SkillSound = SkillSoundJ;
        if (SkillSoundJ.isPlaying) SkillSound = SkillSoundK;
        if (SkillSoundK.isPlaying) SkillSound = SkillSoundL;
        if (SkillSoundL.isPlaying) SkillSound = SkillSoundM;
        if (SkillSoundM.isPlaying) SkillSound = SkillSoundN;
        if (SkillSoundN.isPlaying) SkillSound = SkillSound_0;
        if (SkillSound_0.isPlaying) SkillSound = SkillSound_1;
        if (SkillSound_1.isPlaying) SkillSound = SkillSound_2;
        if (SkillSound_2.isPlaying) SkillSound = SkillSound_3;
        if (SkillSound_3.isPlaying) SkillSound = SkillSound_4;
        if (SkillSound_4.isPlaying) SkillSound = SkillSoundA;
        SkillSound.clip = null;

        if (order == 0) SkillSound.clip = slash_1;
        if (order == 1) SkillSound.clip = slash_2;
        if (order == 2) SkillSound.clip = boss_throw;
        if (order == 3) SkillSound.clip = Boss_CastLoop;
        if (order == 4) SkillSound.clip = Boss_SwordInsert;
        if (order == 5) SkillSound.clip = Boss_CastLoop;
        if (order == 6) SkillSound.clip = Boss_Falling;
        if (order == 7) SkillSound.clip = Boss_Death;
        if (order == 8) SkillSound.clip = Boss_Wind;
        if (order == 9) SkillSound.clip = Boss_Hurt;
        if (order == 10) SkillSound.clip = ratMage_Cast;
        if (order == 11) SkillSound.clip = ratMage_Cast2;
        if (order == 12)
        {

            Warming.Play();
            return;
        }

               

        if (SkillSound.clip != null)
            SkillSound.Play();
    }

    public void InputVFX_Simple(int order)
    {
        AudioSource SkillSound = SkillSoundA;
        if (SkillSoundA.isPlaying) SkillSound = SkillSoundB;
        if (SkillSoundB.isPlaying) SkillSound = SkillSoundC;
        if (SkillSoundC.isPlaying) SkillSound = SkillSoundD;
        if (SkillSoundD.isPlaying) SkillSound = SkillSoundE;
        if (SkillSoundE.isPlaying) SkillSound = SkillSoundF;
        if (SkillSoundF.isPlaying) SkillSound = SkillSoundG;
        if (SkillSoundG.isPlaying) SkillSound = SkillSoundH;
        if (SkillSoundH.isPlaying) SkillSound = SkillSoundI;
        if (SkillSoundI.isPlaying) SkillSound = SkillSoundJ;
        if (SkillSoundJ.isPlaying) SkillSound = SkillSoundK;
        if (SkillSoundK.isPlaying) SkillSound = SkillSoundL;
        if (SkillSoundL.isPlaying) SkillSound = SkillSoundM;
        if (SkillSoundM.isPlaying) SkillSound = SkillSoundN;
        if (SkillSoundN.isPlaying) SkillSound = SkillSoundA;
        SkillSound.clip = null;

        if (order == 0) SkillSound.clip = nameless_Hit;
        if (order == 1) { SkillSound.clip = nameless_GainBuff; }
        if (order == 2) SkillSound.clip = nameless_Death;
        if (order == 3) SkillSound.clip = nameless_CastDown;
        if (order == 4) SkillSound.clip = nameless_Awake;
        if (order == 5) SkillSound.clip = nameless_Attack;

        if (order == 6) SkillSound.clip = turret_Hit;
        if (order == 7) SkillSound.clip = turret_EnterBattle;
        if (order == 8) SkillSound.clip = turret_Attack;
        if (order == 9) SkillSound.clip = healTower_Heal;
        if (order == 10) SkillSound.clip = healTower_Hurt;

        if (order == 11) SkillSound.clip = summon_1;
        if (order == 12) SkillSound.clip = summon_2;
        if (order == 13) SkillSound.clip = summon_3;
        if (order == 14) SkillSound.clip = summon_4;
        if (order == 15) SkillSound.clip = tuantEffect;
        if (order == 16) SkillSound.clip = tuantHit;
        if (order == 17) SkillSound.clip = bossFireball;

        if (SkillSound.clip != null)
            SkillSound.Play();
    }
    public void InputVFX(VFX vfx)
    {
        //Debug.Log("VFX:" + vfx.ToString());
        AudioSource SkillSound = SkillSoundA;
        if (SkillSoundA.isPlaying) SkillSound = SkillSoundB;
        if (SkillSoundB.isPlaying) SkillSound = SkillSoundC;
        if (SkillSoundC.isPlaying) SkillSound = SkillSoundD;
        if (SkillSoundD.isPlaying) SkillSound = SkillSoundE;
        if (SkillSoundE.isPlaying) SkillSound = SkillSoundF;
        if (SkillSoundF.isPlaying) SkillSound = SkillSoundG;
        if (SkillSoundG.isPlaying) SkillSound = SkillSoundH;
        if (SkillSoundH.isPlaying) SkillSound = SkillSoundI;
        if (SkillSoundI.isPlaying) SkillSound = SkillSoundJ;
        if (SkillSoundJ.isPlaying) SkillSound = SkillSoundK;
        if (SkillSoundK.isPlaying) SkillSound = SkillSoundL;
        if (SkillSoundL.isPlaying) SkillSound = SkillSoundM;
        if (SkillSoundM.isPlaying) SkillSound = SkillSoundN;
        if (SkillSoundN.isPlaying) SkillSound = SkillSoundA;
        SkillSound.clip = null;

        if (vfx == VFX.none) return;

        if (vfx == VFX.flame) { SkillSound.clip = Spell_Fire; }
        if (vfx == VFX.shield) { SkillSound.clip = Shield; }

        if (vfx == VFX.hit) { SkillSound.clip = hit; }
        if (vfx == VFX.hit2) { hit2.Play(); return; }
        if (vfx == VFX.hit3) { hit3.Play(); return; }

        if (vfx == VFX.arrow) { SkillSound.clip = arrow; }
        if (vfx == VFX.groundSound) { SkillSound.clip = hit_Ground; }

        if (vfx == VFX.hitShield) { SkillSound.clip = hit_Shield; }
        if (vfx == VFX.roat_Minor) { SkillSound.clip = roar_MinorEnemy; }
        if (vfx == VFX.roat_Great) { SkillSound.clip = roar_Banshee; }
        if (vfx == VFX.roat_Banshee) { SkillSound.clip = roar_Banshee; }
        if (vfx == VFX.heal) { SkillSound.clip = heal; }
        if (vfx == VFX.ding) { SkillSound.clip = ding; }
        if (vfx == VFX.revival) { SkillSound.clip = revival; }


        //[Rats SFX]
        if (vfx == VFX.rat_Minor_Attack) { SkillSound.clip = rat_Minor_Attack; }
        if (vfx == VFX.rat_Minor_Scream) { SkillSound.clip = rat_Minor_Scream; }

        if (vfx == VFX.rat_Great_Attack) { SkillSound.clip = rat_Great_Attack; }
        if (vfx == VFX.rat_Great_Buff) { SkillSound.clip = rat_Great_Buff; }
        if (vfx == VFX.rat_Great_PoisonSpread) { SkillSound.clip = rat_Great_PoisonSpread; }
        if (vfx == VFX.rat_Great_Roar) { SkillSound.clip = rat_Great_Roar; }
        if (vfx == VFX.rat_Great_Hurt) { SkillSound.clip = rat_Great_Hurt; }

        if (vfx == VFX.rat_Ranger_Shoot) { SkillSound.clip = rat_Ranger_Shoot; }
        if (vfx == VFX.rat_Ranger_Hurt) { SkillSound.clip = rat_Ranger_Hurt; }
        if (vfx == VFX.rat_Ranger_Scream) { SkillSound.clip = rat_Ranger_Scream; }
        if (vfx == VFX.rat_ChargeHit) { SkillSound.clip = chargeHit; }
        if (vfx == VFX.rat_ChargePepare) { SkillSound.clip = chargePerpare; }
        if (vfx == VFX.gainFlesh) { SkillSound.clip = obsorbFlesh; }

        if (vfx == VFX.rat_Minor_JumpTo) { SkillSound.clip = rat_JumpToGround; }
        if (vfx == VFX.rat_Minor_Roar) { SkillSound.clip = rat_MinorRoar; }

        if(SkillSound.clip != null)
        SkillSound.Play();
    }



    public void FixedUpdate()
    {
        if (GC.isMoving) moveSound.SetBool("walk", true);
        else moveSound.SetBool("walk", false);
    }
}
