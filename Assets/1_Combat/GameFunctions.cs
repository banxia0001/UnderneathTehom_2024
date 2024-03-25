using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class textInformation
{
    public string text;
    public Sprite sprite;

    public textInformation(string text, Sprite sprite)
    {
        this.text = text;
        this.sprite = sprite;
    }
}
public class hitInformation
{
    public bool hit;
    public bool crit;
    public string addone;
    public hitInformation(bool hit, bool crit,string addone)
    {
        this.hit = hit;
        this.crit = crit;
        this.addone = addone;
    }
}
public class damageInformation
{
    public int damage;
    public string detail = "";
    public string addone = "";

    public damageInformation(int damage, string addone)
    {
        this.damage = damage;
        this.addone = addone;
    }
}
public class GameFunctions : MonoBehaviour
{  
    public static int CalculateHit_Start(Unit attacker, Unit defender, UnitWeapon damage, Skill skill, bool isRange)
    {
        int hit = 0;
        bool addWeapon = false;
        if (damage != null)
        {
            addWeapon = true;
        }

        if (skill != null)
        {
            hit += skill.hitBonus;
            if (skill.autoHit) return -100;
            if (skill.isUsingWeapon) addWeapon = true;
            else addWeapon = false;
        }

        if (addWeapon) hit += damage.hit;

        int dodge = defender.currentData.dodge;

        if (isRange)
        {
            int dodgeAdd = UnitFunctions.Check_DodgeRateAddOnRange(defender);
            dodge += dodgeAdd;
        }

        hit -= UnitFunctions.Check_Hitrate_WithRat(attacker);

        hit -= dodge;

        return hit;
    }


    public static hitInformation CalculateHit_End(Unit attacker, Unit defender, UnitWeapon damage, Skill skill, bool isRange)
    {
       

        //[Corpse will always be hit]
        if (defender.unitAttribute != Unit.UnitAttribute.alive)
        {
            return new hitInformation(true, true, "");
        }

        int hit = CalculateHit_Start(attacker, defender, damage, skill, isRange);
        bool canCrit = true;
        string addone = "";

        if (hit == -100) return new hitInformation(true, false, "<size=26>Auto-Hit </size>");

        if (skill != null)
        {
            if (skill.isFriendly || skill.isSelfUse) canCrit = false;
        }

        int random = Random.Range(0, 100) + Random.Range(0, 100);
        hit -= random / 2;

        if (hit > 0)
        {
            //[Crit Hit]
            if (hit > 75 && canCrit)
            {
                addone = "<size=26>Crit </size>";
                return new hitInformation(true, true, addone);
            }else return new hitInformation(true,false, addone);
        } else return new hitInformation(false, false,addone);    
    }


    public static int[] calculateRandomDamage(Unit attacker, Unit defender, UnitWeapon damage, Skill skill, bool Crit, PathNode startNode)
    {
        int damMin = attacker.currentData.damage.damMin;
        int damMax = attacker.currentData.damage.damMax;
        int damBonus = attacker.currentData.damage.damBonus;

        if (skill != null)
        {
            if (!skill.isUsingWeapon || skill.damageType == Skill._DamageType.none)
            {
                damMin = 0;
                damMax = 0;
            }

            damMin += skill.damMin;
            damMax += skill.damMax;

            //[Damage Bonus]

            if (skill.damageType == Skill._DamageType.usePow)
            {
                float addOnDam = attacker.currentData.power;
                damBonus = (int)addOnDam;
            }

            if (skill.damageType == Skill._DamageType.useDam)
            {
                float addOnDam = attacker.currentData.damage.damBonus;
                damBonus = (int)addOnDam;
            }

            if (skill.damageType == Skill._DamageType.none)
            {
                damBonus = 0;
            }
        }

        //[Damage Mutiple]
        float damX = 1;
        if (Crit == true) damX = 1.3f;

        float damFMin = damX * (damBonus + damMin);
        float damFMax = damX * (damBonus + damMax);
      
        //[Healing]
        if (skill != null && skill.isFriendly)
        {
            if (attacker.expertHealer)
            {
                damFMin = damFMin * 1.3f;
                damFMax = damFMax * 1.3f;
            } 
        }

        //[GaintSlayer]
        if (attacker.giantSlayer == true && defender.currentData.healthNow > 19 && !skill.isFriendly)
        {
            damFMin = damFMin * 2f;
            damFMax = damFMax * 2f;
        }
        if (defender.unitAttribute != Unit.UnitAttribute.alive)
        {
            damFMin += 3;
            damFMax += 3;
        }
        //[Curse]
        if (defender.isCurse)
        {
            int bonus = UnitFunctions.Check_DamBonus_TowardCursedUnit(attacker);
            damFMin += bonus;
            damFMax += bonus;
        }

        if (attacker.nodeAt.height -1 > defender.nodeAt.height)
        {
            int dam2 = 1 + UnitFunctions.Check_DamBonus_HighGround(attacker);
            damFMin += dam2;
            damFMax += dam2;
        }

        //if (attacker.nodeAt.height + 1 < defender.nodeAt.height)
        //{
        //    int dam2 = -1;
        //    damFMin += dam2;
        //    damFMax += dam2;
        //}
        int AN = UnitFunctions.Check_AN(attacker);
        float armor = defender.currentData.armorNow - AN;
        if (armor < 0) armor = 0;

        //[Armor Decreased here]
        damFMin -= armor;
        damFMax -= armor;
        if (damFMax < 0.5f) damFMax = 1;

        int[] myInt = new int[2];
        myInt[0] = (int)damFMin;
        myInt[1] = (int)damFMax;

        if (myInt[0] < 0) myInt[0] = 0;
        if (myInt[1] < 0) myInt[1] = 0;
        return myInt;
    }

    public static damageInformation calculateDamage(Unit attacker, Unit defender, UnitWeapon damage, Skill skill, bool crit, bool isSplash)
    {
        int[] list = calculateRandomDamage(attacker, defender, damage, skill, crit, attacker.nodeAt);
        int dam = Random.Range(list[0], list[1]+1);

        string addone = "";

        //[Healing]
        if (skill != null && skill.isFriendly)
        {
            return new damageInformation((int)dam, null); 
        }

        return new damageInformation((int)dam, addone);
    }





    //¡¾¡¾¡¾Attack Step One¡¿¡¿¡¿//
    //¡¾¡¾¡¾Attack Step One¡¿¡¿¡¿//
    //¡¾¡¾¡¾Attack Step One¡¿¡¿¡¿//

    public static IEnumerator Attack(Unit attacker, Unit defender, UnitWeapon damage, bool enemyCanFightBack, Skill skill, bool isFightBack)
    {
        //[Step.1] Change game state.
        GameController GC = FindObjectOfType<GameController>();
        GameController.currentActionUnit = attacker;
        GC.isAttacking = true;
        GC.isMoving = false;
        GC.isAIThinking = false;


        //[Step.2] Calculate Attacker Stats
        if (attacker != null && defender != null)
        {
            //[Flip]
            UnitFunctions.Flip(attacker.transform.position.x, defender.transform.position.x, attacker);
            UnitFunctions.Flip(defender.transform.position.x, attacker.transform.position.x, defender);
            if (isFightBack) attacker.popTextString.Add(new textInformation("Counter-Attack", Resources.Load<Sprite>("Image/Sword")));

            int attackNumber = damage.num;
           

            float timer_1 = damage.timer_PerformAction;
            float timer_2 = damage.timer_AttackHit;
            float timer_3 = damage.timer_WaitAfterAttack;

            float attack_MoveToGridSpeedRatio = damage.moveToGrid_SpeedRatio;
            float attack_MoveToGridRatio = damage.moveToGrid_DistanceRatio;

            string attackString = "attack";

            bool rangeAttack = false;
            if (damage.range > 1) rangeAttack = true;

            bool canFightBack = enemyCanFightBack;
            if (damage.range > 1 || defender.isActive == false || defender.unitAttribute != Unit.UnitAttribute.alive) canFightBack = false;

            GameObject geneProjectile = null;
            GameObject castEffect = null;
            if (damage != null)
                if (damage.castSpecialEffect != null) 
                    castEffect = damage.castSpecialEffect;

            if (defender.currentData.damage.range > 1) canFightBack = false;

            //[Skill Stats] If the attack is a skill, it will add-on or replace the Attacker's basic stats.
            if (skill != null && skill.projectile != null) geneProjectile = skill.projectile;

            if (skill != null)
            {
                attackString = skill.animTriggerType.ToString();
                attackNumber = skill.num;
                if (skill.range > 1 || skill.causeKnockBack) canFightBack = false;
                if (skill.range > 1) rangeAttack = true;
                else rangeAttack = false;

                timer_1 = skill.timer_PerformAction;
                
                timer_2 = skill.timer_AttackHit;

                attack_MoveToGridSpeedRatio = skill.moveToGrid_SpeedRatio;
                attack_MoveToGridRatio = skill.moveToGrid_DistanceRatio;
                timer_3 = skill.timer_WaitAfterAttack;

                if (skill.isUsingWeapon) castEffect = damage.hitSpecialEffect;
                if (skill.castSpecialEffect != null) castEffect = skill.castSpecialEffect;
                else castEffect = null;
            }



            bool dontWait = false;
            //if (attacker.data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._13_Rev_Turret)
            //{
            //    dontWait = true;
            //}

            if (attacker != null && defender != null && attacker.currentData.healthNow >= 0 && defender.currentData.healthNow >= 0)
            {
                #region Attack Loop
                //[Step.3] Loop though all the attack chance, each will calculate once the Attack Step.2(The function below)
                for (int i = 0; i < 1; i++)
                {
                    if (defender == null || attacker == null) break;
                    if (defender.currentData.healthNow <= 0) break;


                    if (!dontWait)
                    {
                        if (i != 0) yield return new WaitForSeconds(0.3f);
                        yield return new WaitForSeconds(0.12f);
                    }


                    UnitFunctions.Flip(attacker.transform.position.x, defender.transform.position.x, attacker);
                    UnitFunctions.Flip(defender.transform.position.x, attacker.transform.position.x, defender);



                    GC.isAttacking = true;
                    GC.isMoving = false;
                    GC.isAIThinking = false;

                    //[Range Attack with Projectile] 
                    //[Range Attack with Projectile] 
                    //[Range Attack with Projectile] 
                    if (rangeAttack && geneProjectile != null)
                    {
                        attacker.InputAnimation(attackString);
                        if (!dontWait) yield return new WaitForSeconds(timer_1);

                        if (attacker.unitSpecialState == Unit.UnitSpecialState.machineGun)
                        {
                            attacker.gameObject.GetComponent<AutoMachineGun_Controller>().SpawnGunFire(defender);
                        }
                        else if (castEffect != null)
                        {
                            GameObject specialEffect = Instantiate(castEffect, UnitFunctions.GetUnitMiddlePoint(attacker),  Quaternion.identity);
                            UnitFunctions.Flip_Simple(attacker.transform.position.x, defender.transform.position.x, specialEffect);
                        }

                        if (!dontWait) yield return new WaitForSeconds(timer_2);


                        Vector3 myP = UnitFunctions.GetUnitMiddlePoint(attacker);
                        if (attacker.unitSpecialState == Unit.UnitSpecialState.machineGun)
                        {
                            myP = attacker.GetComponent<AutoMachineGun_Controller>().boneFollower_Gun.transform.position;
                        }
                        GameObject Projectile = Instantiate(geneProjectile, myP, Quaternion.identity) as GameObject;
                        List<PathNode> gotoList = new List<PathNode>();
                        gotoList.Add(attacker.nodeAt);
                        gotoList.Add(defender.nodeAt);
                        Projectile.GetComponent<Projectile>().TriggerProjectile(attacker, gotoList, damage, skill);

                        if (!dontWait) yield return new WaitForSeconds(timer_3);
                    }




                    //[Range Attack] 
                    //[Range Attack] 
                    //[Range Attack] 
                    else if (rangeAttack)
                    {
                        attacker.InputAnimation(attackString);
                        if (!dontWait) yield return new WaitForSeconds(timer_1);
                       
                        float dis = Vector3.Distance(attacker.transform.position, defender.transform.position);
                        if (!dontWait) yield return new WaitForSeconds(0.1f + 0.01f * dis);

                        if (castEffect != null)
                        {
                            if (attacker.unitSpecialState == Unit.UnitSpecialState.machineGun)
                            {
                                attacker.gameObject.GetComponent<AutoMachineGun_Controller>().SpawnGunFire(defender);
                            }
                            else
                            {
                                GameObject specialEffect = Instantiate(castEffect, UnitFunctions.GetUnitMiddlePoint(attacker), Quaternion.identity);
                                UnitFunctions.Flip_Simple(attacker.transform.position.x, defender.transform.position.x, specialEffect);
                            }
                        }

                        if (!dontWait) yield return new WaitForSeconds(timer_2);

                        for (int i2 = 0; i2 < attackNumber; i2++)
                        {
                            yield return new WaitForSeconds(0.1f);
                            GC.Attack(attacker, defender, damage, skill, rangeAttack);
                        }
                          
                        //GC.Attack(attacker, defender, damage, skill, rangeAttack);

                        if (!dontWait) yield return new WaitForSeconds(timer_3);
                     
                    }

                    else
                    {
                        attacker.InputAnimation(attackString);
                        if (!dontWait) yield return new WaitForSeconds(timer_1);

                        if (castEffect != null)
                        {
                            GameObject specialEffect = Instantiate(castEffect, UnitFunctions.GetUnitMiddlePoint(attacker), Quaternion.identity);
                            UnitFunctions.Flip_Simple(attacker.transform.position.x, defender.transform.position.x, specialEffect);
                        }

                        attacker.Input_CombatPos(defender.nodeAt.transform.position + new Vector3(0,-0.2f,0), attack_MoveToGridSpeedRatio, attack_MoveToGridRatio, timer_3);

                         if(!dontWait) yield return new WaitForSeconds(timer_2);
                         GC.Attack(attacker, defender, damage, skill, rangeAttack);

                        if (!dontWait) yield return new WaitForSeconds(timer_3);
                    }

                    if(!dontWait) yield return new WaitForSeconds(0.2f);
                }
                #endregion
            }


            GC.isAttacking = true;
            GC.isMoving = false;
            GC.isAIThinking = false;

            if (defender.unitTeam == Unit.UnitTeam.enemyTeam)
            {
                if (defender.GetComponent<UnitAI>().unitHoldingSkill) canFightBack = false;
                if (defender.GetComponent<BossAI>() != null)
                    if (defender.GetComponent<BossAI>().bossHoldSkill != BossAI.BossHoldSkill.none) 
                        canFightBack = false;
            }

            if (defender.unitSpecialState == Unit.UnitSpecialState.block) canFightBack = false;
            if(defender.unitSpecialState == Unit.UnitSpecialState.boneFireTower) canFightBack = false;
            if(defender.unitSpecialState == Unit.UnitSpecialState.healTower) canFightBack = false;
            if(defender.unitSpecialState == Unit.UnitSpecialState.machineGun) canFightBack = false;

            yield return new WaitForSeconds(0.35f);

            //[Attack end, Check Repel] If enemy can action, it will couter attack.
            if (canFightBack && defender.unitAttribute == Unit.UnitAttribute.alive && defender.currentData.healthNow > 0)
            {
                yield return new WaitForSeconds(0.1f);
                if (attacker.canRepel) 
                { 
                    GC.RepelBack(attacker, defender); 
                    defender.UnitEnable(false);
                }
                else GC.FightBack(attacker, defender);
            }
            else GC.isAttacking = false;


            if (attacker != null)
            {
                bool freemove = false;
                if (skill != null && skill.isInstant) freemove = true;
                if(!freemove) attacker.UnitEnable(false);
                GC.Deselect_Unit();
                //GC.Select_Unit(attacker.nodeAt, false);
            } 
        }
    }




    //¡¾¡¾¡¾Attack Step Two¡¿¡¿¡¿//
    //¡¾¡¾¡¾Attack Step Two¡¿¡¿¡¿//
    //¡¾¡¾¡¾Attack Step Two¡¿¡¿¡¿//

    //When Attack Step.1 end, it will ask GameController to run this Coroutine since static script cannot run Coroutine function.
    //The funciton is separate into 2, because the step.1 is use for normal attack(Or skill cause attack) only
    //It contian the cooparetion with aniamtion(Wait second, VFX and animation trigger work together).
    //Some skill effects multiple units, may not run though those animation and VFX.
    //So the step.2 will do hit/damage/buff/splash calculate. Which is always the final step for causing damage.
    //Step.1 will call it when it end. Other actions will call it when they are end.
    public static IEnumerator Attack_Calculate(Unit attacker, Unit defender, GameController GC, UnitWeapon damage, Skill skill, bool isSplash, bool isRange)
    {
        //[Step.1] Calculate if the attack hit
        hitInformation hitInfo = CalculateHit_End(attacker, defender, damage, skill, isRange);
        GC.SC.Hit();

        //[Step.2] Gain the VFX
        GameObject hitEffect = null;
        if (damage != null)
            if (damage.hitSpecialEffect != null) hitEffect = damage.hitSpecialEffect;

        //[Step.3] Gain the Buff(If the attack cause buff)
        Buff buff = null;


        if (skill != null)
        {

            if (skill.hitSpecialEffect != null) hitEffect = skill.hitSpecialEffect;
            else if (skill.isUsingWeapon) hitEffect = damage.hitSpecialEffect;
            else hitEffect = null;

            buff = skill.buff;
            if (skill.randomBuffs.Length > 0) buff = skill.randomBuffs[Random.Range(0, skill.randomBuffs.Length)];

            //[Splash] If skill cuase splash effect, it will trigger here.
            if (skill.splashRange > 0 && !isSplash) SkillFunctions.Skill_Splash(attacker, defender, skill, skill.isFriendly);
        }
        damageInformation damInfo = calculateDamage(attacker, defender, damage, skill, hitInfo.crit, false);


        //Dodge and move back
        if (!hitInfo.hit)
        {
            Vector3 knockbackPos = UnitFunctions.Find_UnitShouldGoPosition_WhenKnochBack(attacker, defender);
            defender.Input_CombatPos(knockbackPos, 1.355f, 0.3f, 0.02f);
            defender.popTextString.Add(new textInformation("Dodge", Resources.Load<Sprite>("Image/Dodge")));
        }

        if (hitEffect != null)
        {
            GameObject specialEffect = Instantiate(hitEffect, UnitFunctions.GetUnitMiddlePoint(defender), Quaternion.identity);
            UnitFunctions.Flip_Simple(attacker.transform.position.x, specialEffect.transform.position.x, specialEffect);
        }

        if (skill != null && skill.causeGridShock)
        {
            GridFallController GFC = FindObjectOfType<GridFallController>();
            GFC.StartAOE1Shock(defender.nodeAt);
        }
        //[Step.4] If damage hit, cause damage.
        if (hitInfo.hit)
        {

            //GC.SC.InputVFX(vfx);

            if (defender != null)
            {
              
                int finaldam = (int)damInfo.damage;
                if (defender.unitAttribute == Unit.UnitAttribute.alive)
                {
                   
                    int lifesteal = UnitFunctions.Check_Lifesteal(attacker);
                    int lifestealNow = 0;
                    if (finaldam > 0) lifestealNow = finaldam;
                    if (lifestealNow > lifesteal) lifestealNow = lifesteal;
                    if (lifestealNow != 0)
                    {
                        attacker.HealthChange(lifestealNow, 0, "Heal");
                    }
                }
       

                defender.HealthChange(-finaldam, UnitFunctions.Check_KillFPBonus(attacker), "Damage");

                int armorSunder = attacker.currentData.damage.armorSunder;

                if (skill != null) { armorSunder += skill.armorSunder; }

                //[ArmorBreaker]
                if (armorSunder > 0)
                {
                    defender.ArmorChange(-armorSunder);
                }


                //[Step.5] If damage hit, cause damage.
                if (buff != null)
                {
                    //[Tuant] If skill cause tuant, here inplant the buff to target unit.
                    if (buff.buffType == Buff._buffType.tuant)
                    {
                        buff.tuantTarget = attacker;
                    }

                    if (!skill.buffSelf) defender.InputBuff(buff, buff.canRepeatBuff);
                    else if (skill.buffSelf && !isSplash) attacker.InputBuff(buff, buff.canRepeatBuff);
                }

                //[Unit_Shake] It will cause unit to move a little bit base on where the attack come from.
                Vector3 knockbackPos = UnitFunctions.Find_UnitShouldGoPosition_WhenKnochBack(attacker, defender);
                defender.Input_CombatPos(knockbackPos, 1.2f, 0.4f, 0.05f);

                if (skill != null && skill.causeKnockBack == true)
                    GC.TriggerKnockBack(attacker, defender, skill.KnockBack_Distance, 0);
            }
        }

        yield return new WaitForSeconds(0.1f);
    }

    public static IEnumerator Repel(Unit attacker, Unit defender)
    {
        GameController GC = FindObjectOfType<GameController>();
        GC.isAttacking = true;
        GC.isMoving = false;
        GC.isAIThinking = false;

        if (attacker != null && defender != null)
        {
            if (attacker.currentData.healthNow > 0 && defender.currentData.healthNow > 0)
            {
                //make the defender face attaker
                UnitFunctions.Flip(defender.transform.position.x, attacker.transform.position.x, defender);
                UnitFunctions.Flip(attacker.transform.position.x, defender.transform.position.x, attacker);

                yield return new WaitForSeconds(0.1f);

                defender.InputAnimation("attack");

                yield return new WaitForSeconds(0.1f);

                attacker.InputAnimation("attack");

                yield return new WaitForSeconds(0.2f);
                string text =  "Repelled!";
                attacker.popTextString.Add(new textInformation(text, Resources.Load<Sprite>("Image/Sword")));
            }
        }

        GC.isAttacking = false;
    }




    //¡¾¡¾¡¾Check Weakness¡¿¡¿¡¿//
    public static int CheckUnit_Weakness(Unit attacker, Unit defender)
    {
        int result = 0;
        if (defender.data.unitType == UnitData.Unit_Type.Beast)
        {
            if (attacker.data.unitType == UnitData.Unit_Type.Warrior || attacker.data.unitType == UnitData.Unit_Type.Hunter || attacker.data.unitType == UnitData.Unit_Type.Knight)
            { return 1; }
            if(attacker.data.unitType == UnitData.Unit_Type.Magician || attacker.data.unitType == UnitData.Unit_Type.Priest || attacker.data.unitType == UnitData.Unit_Type.Assasin)
            { return -1; }
        }
        if (defender.data.unitType == UnitData.Unit_Type.Fallen)
        {
            if (attacker.data.unitType == UnitData.Unit_Type.Warrior || attacker.data.unitType == UnitData.Unit_Type.Hunter || attacker.data.unitType == UnitData.Unit_Type.Knight)
            { return -1; }
            if (attacker.data.unitType == UnitData.Unit_Type.Magician || attacker.data.unitType == UnitData.Unit_Type.Priest || attacker.data.unitType == UnitData.Unit_Type.Assasin)
            { return 1; }
        }
        if (defender.data.unitType == UnitData.Unit_Type.Warrior)
        { 
            if (attacker.data.unitType == UnitData.Unit_Type.Knight || attacker.data.unitType == UnitData.Unit_Type.Fallen) { return 1; }
            if (attacker.data.unitType == UnitData.Unit_Type.Hunter || attacker.data.unitType == UnitData.Unit_Type.Beast) { return -1; }
        }
        if (defender.data.unitType == UnitData.Unit_Type.Hunter)
        {
            if (attacker.data.unitType == UnitData.Unit_Type.Knight || attacker.data.unitType == UnitData.Unit_Type.Fallen) { return -1; }
            if (attacker.data.unitType == UnitData.Unit_Type.Warrior || attacker.data.unitType == UnitData.Unit_Type.Beast) { return 1; }
        }
        if (defender.data.unitType == UnitData.Unit_Type.Knight)
        {
            if (attacker.data.unitType == UnitData.Unit_Type.Warrior || attacker.data.unitType == UnitData.Unit_Type.Fallen) { return -1; }
            if (attacker.data.unitType == UnitData.Unit_Type.Hunter || attacker.data.unitType == UnitData.Unit_Type.Beast) { return 1; }
        }
        if (defender.data.unitType == UnitData.Unit_Type.Magician)
        {
            if (attacker.data.unitType == UnitData.Unit_Type.Priest || attacker.data.unitType == UnitData.Unit_Type.Beast) { return -1; }
            if (attacker.data.unitType == UnitData.Unit_Type.Assasin || attacker.data.unitType == UnitData.Unit_Type.Fallen) { return 1; }
        }
        if (defender.data.unitType == UnitData.Unit_Type.Priest)
        {
            if (attacker.data.unitType == UnitData.Unit_Type.Assasin || attacker.data.unitType == UnitData.Unit_Type.Beast) { return -1; }
            if (attacker.data.unitType == UnitData.Unit_Type.Magician || attacker.data.unitType == UnitData.Unit_Type.Fallen) { return 1; }
        }
        if (defender.data.unitType == UnitData.Unit_Type.Assasin)
        {
            if (attacker.data.unitType == UnitData.Unit_Type.Magician || attacker.data.unitType == UnitData.Unit_Type.Beast) { return -1; }
            if (attacker.data.unitType == UnitData.Unit_Type.Priest || attacker.data.unitType == UnitData.Unit_Type.Fallen) { return 1; }
        }

        return result;
    }


    public static int Get_AverageDmg(UnitWeapon dam)
    {
        int DMG = (dam.damMin + dam.damMax) / 2 + dam.damBonus;
        return DMG;
    }

    //¡¾¡¾¡¾Find Grid¡¿¡¿¡¿//
    //¡¾¡¾¡¾Find Grid¡¿¡¿¡¿//
    //¡¾¡¾¡¾Find Grid¡¿¡¿¡¿//
    public static float CalculateDis_WithoutY(PathNode ob1, PathNode ob2)
    {
        Vector3 dis1 = GridManager.GetGridCalculatePosition(ob1.x, ob1.y);
        Vector3 dis2 = GridManager.GetGridCalculatePosition(ob2.x, ob2.y); 
        float dist = Vector2.Distance(dis1, dis2);
        return dist;
    }
    public static float CalculateDis_WithoutY(Vector2 ob1, PathNode ob2)
    {
        ob1 = new Vector3(ob1.x, ob1.y, 0);
        Vector2 dis2 = GridManager.GetGridCalculatePosition(ob2.x, ob2.y);
        float dist = Vector2.Distance(ob1, dis2);
        Debug.Log(dist);
        return dist;
    }
   
    public static List<PathNode> FindNodes_ByDistance(PathNode startNode, int range, bool igoreHeight)
    {
        float dis = range * 1.734f;

        GridManager grid = GameObject.FindGameObjectWithTag("GameController").GetComponent<GridManager>();
        List<PathNode> nodeList = new List<PathNode>();

        for (int x = startNode.x - range - 1; x <= startNode.x + range + 1; x++)
        {
            for (int y = startNode.y - range - 1; y <= startNode.y + range + 1; y++)
            {
                if (x < 0 || x >= grid.GetWidth() || y < 0 || y >= grid.GetHeight()) continue;
                if (grid.pathArray[x, y] == null) continue;
                if (grid.pathArray[x, y].isBlocked || grid.pathArray[x, y].campFire != null) continue;

                if (!igoreHeight)
                {
                    int heightDiff = startNode.height - grid.pathArray[x, y].height;
                    //Debug.Log(heightDiff);
                    if (heightDiff > 3 || heightDiff < -3)
                    {
                        continue;
                    }
                }

                if (dis >= CalculateDis_WithoutY(startNode, grid.GetPath(x, y)))
                {
                    nodeList.Add(grid.GetPath(x, y));
                }
            }
        }
        return nodeList;
    }

    public static List<PathNode> FindNodes_ByDistance_UseVector3(PathNode startNode ,Vector3 startPos, int range)
    {
        float dis = range * 1.734f;
        GridManager grid = GameObject.FindGameObjectWithTag("GameController").GetComponent<GridManager>();
        List<PathNode> nodeList = new List<PathNode>();

        for (int x = startNode.x - range - 2; x <= startNode.x + range + 2; x++)
        {
            for (int y = startNode.y - range - 2; y <= startNode.y + range + 2; y++)
            {
                if (x < 0 || x >= grid.GetWidth() || y < 0 || y >= grid.GetHeight()) continue;
                if (grid.pathArray[x, y] == null) continue;

                Debug.Log(x + "," + y);
                if (dis >= CalculateDis_WithoutY(startPos, grid.GetPath(x, y)))
                {
                    nodeList.Add(grid.GetPath(x, y));
                }
            }
        }
        return nodeList;
    }

    public static PathNode FindNode_Nearby(PathNode startNode, int range)
    {
        List<PathNode> nearbyNode = GameFunctions.FindNodes_ByDistance(startNode, range, true);

        PathNode selectedNode = null;

        for (int i = 1; i < nearbyNode.Count; i++)
        {
            if (nearbyNode[i].unit == null)
            {
                float heightDiff = startNode.height - nearbyNode[i].height;
                //Debug.Log(heightDiff);
                if (heightDiff > 3 || heightDiff < -3)
                {
                    continue;
                }

                selectedNode = nearbyNode[i];
                break;
            }
        }
        return selectedNode;
    }

    //[Find those nodes nearby the unit]
    public static List<PathNode> FindNodes_OneGridNearby(PathNode startNode, bool igoreHeight)
    {
        float dis = 1 * 1.734f;

        GridManager grid = GameObject.FindGameObjectWithTag("GameController").GetComponent<GridManager>();
        List<PathNode> nodeList = new List<PathNode>();

        for (int x = startNode.x - 1 - 1; x <= startNode.x + 1 + 1; x++)
        {
            for (int y = startNode.y - 1 - 1; y <= startNode.y + 1 + 1; y++)
            {
                if (x < 0 || x >= grid.GetWidth() || y < 0 || y >= grid.GetHeight()) continue;
                if (x == startNode.x && y == startNode.y) continue;
                if (grid.pathArray[x, y] == null) continue;

                if (!igoreHeight)
                {
                    float heightDiff = startNode.height - grid.pathArray[x, y].height;
                    if (heightDiff > 3 || heightDiff < -3)
                    {
                        continue;
                    }
                }

                if (dis >= CalculateDis_WithoutY(startNode, grid.GetPath(x, y)))
                    nodeList.Add(grid.GetPath(x, y));
            }
        }
        return nodeList;
    }


    //[Find those nodes the unit can attack]
    public static List<PathNode> FindNodes_ByCanAttack(List<PathNode> mayMoveList, Unit unit)
    {
        //Debug.Log(mayMoveList.Count);
        List<PathNode> myTargetNodes = new List<PathNode>();

        for (int i = 0; i < mayMoveList.Count; i++)
        {
            if (mayMoveList[i] == null) continue;
            if (mayMoveList[i].isBlocked == true) continue;

            bool canShow = true;

            if (mayMoveList[i].unit == true)
            {
                canShow = false;
                if (mayMoveList[i].unit == unit) canShow = true;
            }

            if (canShow)
            {
                List<PathNode> nearbynode = FindNodes_OneGridNearby(mayMoveList[i], false);
                //Debug.Log(nearbynode.Count);
                for (int i2 = 0; i2 < nearbynode.Count; i2++)
                {
                    if (nearbynode[i2].unit != null)
                    {
                        if (nearbynode[i2].unit.unitTeam != unit.unitTeam)
                            if (myTargetNodes != null)
                            {
                                if (!myTargetNodes.Contains(nearbynode[i2]))
                                    myTargetNodes.Add(nearbynode[i2]);
                            }
                            else myTargetNodes.Add(nearbynode[i2]);
                    }
                }
            }               
        }

        //Debug.Log("Nodes:" + myTargetNodes.Count);
        return myTargetNodes;
    }

    public static void Debug_ShowLightMap(List<PathNode> _List)
    {
        GridManager GM = GameObject.FindGameObjectWithTag("GameController").GetComponent<GridManager>();
        for (int i = 0; i < _List.Count; i++)
        {
            Instantiate(Resources.Load<GameObject>("GridPrefab/0_PurpleLine"), GM.GetWorldPosition_WithHeight(_List[i].x, _List[i].y), Quaternion.identity);
        }
    }

    public static List<PathNode> FindNodes_ByMoveableArea(int moveDistance, PathNode startPath, Unit unit)
    {
        List<PathNode> canMoveList = new List<PathNode>();
        List<PathNode> mayMoveList = FindNodes_ByDistance(startPath, moveDistance + 1, true);

        //Debug_ShowLightMap(mayMoveList);
        List<PathNode> path = new List<PathNode>();

        //[Loop] Do a loop for all the nearby nodes, check if unit have enought move point to move to.
        for (int i = 0; i < mayMoveList.Count; i++)
        {
            if (path != null) path.Clear();
            if (mayMoveList[i] == null) continue;
            if (mayMoveList[i].isBlocked == true) continue;
            if (mayMoveList[i].unit != null) continue;
            //igore unit is on, because have to get the mode with unit in it and generate lightMap 
            path = PathFinding.FindPath(startPath, mayMoveList[i], unit, null, false, false, unit.currentData.movePointNow, false);
            if (path == null) continue;

            int totalMovePoint = 0;

            for (int i2 = 1; i2 < path.Count; i2++)
            {
                totalMovePoint += path[i2].MOVE_COST;
            }

            if (unit.currentData.movePointNow >= totalMovePoint)
            {
                canMoveList.Add(mayMoveList[i]);
            }
        }
        return canMoveList;
    }

    public static PathNode FindClosestNode(PathNode startNode, List<PathNode> nearbyNode)
    {
        List<PathNode> nodeList = nearbyNode;
        PathNode closestPath = nodeList[0];
        float closeValue = 99999;

        for (int i = 0; i < nodeList.Count; i++)
        {
            float thisValue = CalculateDis_WithoutY(startNode, nodeList[i]);

            if (thisValue < closeValue)
            {
                closeValue = thisValue;
                closestPath = nodeList[i];
            }
        }
        return closestPath;
    }
    public static PathNode FindFarestNode(PathNode startNode, List<PathNode> nearbyNode, bool igoreNodeWithUnit)
    {
      
        List<PathNode> nodeList = new List<PathNode>();

        foreach (PathNode path in nearbyNode)
        {
            if (path.isBlocked || path.campFire != null) continue;
            if (path.unit != null)
            {
                if (igoreNodeWithUnit != true)
                    nodeList.Add(path);
            }

            else nodeList.Add(path);
        }
        if (nodeList == null || nodeList.Count == 0)
        { return null; }
        
        PathNode farestPath = nodeList[0];
        float farestValue = -99999;

        for (int i = 0; i < nodeList.Count; i++)
        {
            float thisValue = CalculateDis_WithoutY(startNode, nodeList[i]);

            if (thisValue > farestValue)
            {
                farestValue = thisValue;
                farestPath = nodeList[i];
            }
        }
        return farestPath;
    }

    public static List<PathNode> FindNodes_Charge(List<PathNode> paths, Unit attacker)
    {
        List<PathNode> myPath = new List<PathNode>();
        for (int i = 0; i < paths.Count; i++)
        {
            myPath.Add(paths[i]);
            if (paths[i].unit != null && paths[i] != attacker.nodeAt) break;
        }

        return myPath;
    }

    public static List<PathNode> FindNodes_InRange1_HardCode(PathNode gridAt, int heightDiffMax, bool igonreUnit)
    {
        GridManager MyGM = GameObject.FindGameObjectWithTag("GameController").GetComponent<GridManager>();
        List<PathNode> finalPath = new List<PathNode>();

        PathNode addPath = null;
        if (igonreUnit || gridAt.unit == null) finalPath.Add(gridAt);

        addPath = MyGM.GetPath(gridAt.x, gridAt.y + 1);
        if (addPath != null) if (!addPath.isBlocked) if (Mathf.Abs(addPath.height - gridAt.height) < heightDiffMax) if (igonreUnit || addPath.unit == null) finalPath.Add(addPath);

        addPath = MyGM.GetPath(gridAt.x, gridAt.y - 1);
        if (addPath != null) if (!addPath.isBlocked) if (Mathf.Abs(addPath.height - gridAt.height) < heightDiffMax) if (igonreUnit || addPath.unit == null) finalPath.Add(addPath);

        addPath = MyGM.GetPath(gridAt.x + 1, gridAt.y);
        if (addPath != null) if (!addPath.isBlocked) if (Mathf.Abs(addPath.height - gridAt.height) < heightDiffMax) if (igonreUnit || addPath.unit == null) finalPath.Add(addPath);

        addPath = MyGM.GetPath(gridAt.x - 1, gridAt.y);
        if (addPath != null) if (!addPath.isBlocked) if (Mathf.Abs(addPath.height - gridAt.height) < heightDiffMax) if (igonreUnit || addPath.unit == null) finalPath.Add(addPath);


        if (gridAt.y % 2 == 0)
        {
            addPath = MyGM.GetPath(gridAt.x - 1, gridAt.y-1);
            if (addPath != null) if (!addPath.isBlocked) if (Mathf.Abs(addPath.height - gridAt.height) < heightDiffMax) if (igonreUnit || addPath.unit == null) finalPath.Add(addPath);

            addPath = MyGM.GetPath(gridAt.x - 1, gridAt.y + 1);
            if (addPath != null) if (!addPath.isBlocked) if (Mathf.Abs(addPath.height - gridAt.height) < heightDiffMax) if (igonreUnit || addPath.unit == null) finalPath.Add(addPath);
        }

        else
        {
            addPath = MyGM.GetPath(gridAt.x + 1, gridAt.y + 1);
            if (addPath != null) if (!addPath.isBlocked) if (Mathf.Abs(addPath.height - gridAt.height) < heightDiffMax) if (igonreUnit || addPath.unit == null) finalPath.Add(addPath);

            addPath = MyGM.GetPath(gridAt.x + 1, gridAt.y - 1);
            if (addPath != null) if (!addPath.isBlocked) if (Mathf.Abs(addPath.height - gridAt.height) < heightDiffMax) if (igonreUnit || addPath.unit == null) finalPath.Add(addPath);
        }

        return finalPath;
    }


    public static List<PathNode> FindNodes_InOneLine(PathNode From, PathNode To, int range, bool igoreUnit, bool calculateBackRoad, bool igoreHeight, bool firstNodeIgonreHeightDiff)
    {
        //[Step.1] Calculate the direction.
        float angle = 0;
        Vector2 faceDirection;
        faceDirection = (To.transform.position - From.transform.position).normalized;
        angle = Mathf.Atan2(faceDirection.y, faceDirection.x) * Mathf.Rad2Deg;

        //Debug.Log(angle);

        string Dir = "rightUp";
        if (30 < angle && angle <= 90) Dir = "rightUp";          //60   degree
        else if (-30 <= angle && angle <= 30) Dir = "right";     //0    degree
        else if (90 < angle && angle <= 150) Dir = "leftUp";     //120  degree
        else if (150 < angle && angle <= 210) Dir = "left";      //180  degree
        else if (-150 > angle && angle >= -210) Dir = "left";    //-180 degree
        else if (-90 > angle && angle >= -150) Dir = "leftDown"; //-125 degree
        else if (-30 > angle && angle >= -90) Dir = "rightDown"; //-60 degree
        //Debug.Log(Dir);

        //[Step.2] Find the straight line base on direction.
        GridManager GM = GameObject.FindGameObjectWithTag("GameController").GetComponent<GridManager>();
        List<PathNode> lineNode = new List<PathNode>();
        PathNode gridAt = From;
        PathNode gridWas = From;

        if (calculateBackRoad)
        {
            gridAt = To;
            gridWas = To;
        }

        lineNode.Add(gridAt);

        for (int i = 0; i < range; i++)
        {
            gridWas = gridAt;

            if (gridAt.y % 2 == 0)
            {
                if (Dir == "rightUp") gridAt = GM.GetPath(gridAt.x, gridAt.y + 1);
                if (Dir == "rightDown") gridAt = GM.GetPath(gridAt.x, gridAt.y - 1);

                if (Dir == "leftDown") gridAt = GM.GetPath(gridAt.x - 1, gridAt.y - 1);
                if (Dir == "leftUp") gridAt = GM.GetPath(gridAt.x - 1, gridAt.y + 1);

                if (Dir == "right") gridAt = GM.GetPath(gridAt.x + 1, gridAt.y);
                if (Dir == "left") gridAt = GM.GetPath(gridAt.x - 1, gridAt.y);
            }

            else
            {
                if (Dir == "rightUp") gridAt = GM.GetPath(gridAt.x + 1, gridAt.y + 1);
                if (Dir == "rightDown") gridAt = GM.GetPath(gridAt.x + 1, gridAt.y - 1);

                if (Dir == "leftDown") gridAt = GM.GetPath(gridAt.x, gridAt.y - 1);
                if (Dir == "leftUp") gridAt = GM.GetPath(gridAt.x, gridAt.y + 1);

                if (Dir == "right") gridAt = GM.GetPath(gridAt.x + 1, gridAt.y);
                if (Dir == "left") gridAt = GM.GetPath(gridAt.x - 1, gridAt.y);
            }

            if (gridAt != null)
            {
                if (gridAt.isBlocked) break;

                if (firstNodeIgonreHeightDiff && i == 0)
                { 
                
                }

                else if (!igoreHeight)
                {
                    float heightDiff = gridAt.height - gridWas.height;
                    if (heightDiff > 3 || heightDiff < -3)
                    {
                        break;
                    }
                }

                if (igoreUnit == false && gridAt.unit != null) break;

                lineNode.Add(gridAt);
            }
            else break;
        }

        return lineNode;
    }
    public static PathNode FindNode_Right(PathNode path)
    {
        PathNode myPath = null;
        GridManager GM = GameObject.FindGameObjectWithTag("GameController").GetComponent<GridManager>();
        myPath = GM.GetPath(path.x + 1, path.y);
        return myPath;
    }
    public static PathNode FindNode_ByOffset(PathNode path, int xOffset, int yOffset, GridManager GM)
    {
        PathNode myPath = null;
        myPath = GM.GetPath(path.x + xOffset, path.y + yOffset);
        return myPath;
    }
    public static PathNode FindNode_RightUp(PathNode path)
    {
        PathNode myPath = null;
        GridManager GM = GameObject.FindGameObjectWithTag("GameController").GetComponent<GridManager>();
        if (path.y % 2 == 0) myPath = GM.GetPath(path.x, path.y + 1);
        else myPath = GM.GetPath(path.x + 1, path.y + 1);
        return myPath;
    }
    public static PathNode FindNode_LeftUp(PathNode path)
    {
        PathNode myPath = null;
        GridManager GM = GameObject.FindGameObjectWithTag("GameController").GetComponent<GridManager>();
        if (path.y % 2 == 0) myPath = GM.GetPath(path.x - 1, path.y + 1);
        else myPath = GM.GetPath(path.x, path.y + 1);
        return myPath;
    }
    public static PathNode FindNode_RightDown(PathNode path)
    {
        PathNode myPath = null;
        GridManager GM = GameObject.FindGameObjectWithTag("GameController").GetComponent<GridManager>();
        if (path.y % 2 == 0) myPath = GM.GetPath(path.x, path.y - 1);
        else myPath = GM.GetPath(path.x + 1, path.y - 1);
        return myPath;
    }
    public static PathNode FindNode_LeftDown(PathNode path)
    {
        PathNode myPath = null;
        GridManager GM = GameObject.FindGameObjectWithTag("GameController").GetComponent<GridManager>();
        if (path.y % 2 == 0) myPath = GM.GetPath(path.x - 1, path.y - 1);
        else myPath = GM.GetPath(path.x, path.y - 1);
        return myPath;
    }

    //¡¾¡¾¡¾Find Unit¡¿¡¿¡¿//
    //¡¾¡¾¡¾Find Unit¡¿¡¿¡¿//
    //¡¾¡¾¡¾Find Unit¡¿¡¿¡¿//
    public static Unit FindClosestUnit_By_Grid(int teamNum, Unit selectedUnit)//FOR UNIT AI
    {
        if (GameController.playerList.Count < 1 || GameController.enemyList.Count < 1) return null;
        List<Unit> groupedlUnitlist = new List<Unit>();
        if (teamNum == 0) groupedlUnitlist = GameController.playerList;
        if (teamNum == 1) groupedlUnitlist = GameController.enemyList;

        if (groupedlUnitlist == null) return null;
       
        Unit closedUnit = groupedlUnitlist[0];

        int closedDistance = 99999;

        for (int i = 0; i < groupedlUnitlist.Count; i++)
        {
            List<PathNode> moveToList = PathFinding.FindPath(selectedUnit.nodeAt,groupedlUnitlist[i].nodeAt, selectedUnit, groupedlUnitlist[i], false, false,0,false);
            if (moveToList != null)
            {
                int distance = moveToList.Count;
              
                if (distance < closedDistance)
                {
                    closedDistance = distance;
                    closedUnit = groupedlUnitlist[i];
                }
            }
        }
        return closedUnit;
    }

    public static List<Unit> FindUnits_ByDistance(List<PathNode> nearbyPathnode, Unit.UnitTeam unitTeam, bool igoreCorpse)
    {
        List<Unit> unitGroup = new List<Unit>();
        foreach (PathNode node in nearbyPathnode)
        {
            if (node.unit != null && node.unit.unitTeam == unitTeam) unitGroup.Add(node.unit);
            else if (node.unit != null && node.unit.unitAttribute != Unit.UnitAttribute.alive && !igoreCorpse) unitGroup.Add(node.unit);
        }
        return unitGroup;
    }
    //¡¾¡¾¡¾Check Attack Range¡¿¡¿¡¿//
    //¡¾¡¾¡¾Check Attack Range¡¿¡¿¡¿//
    //¡¾¡¾¡¾Check Attack Range¡¿¡¿¡¿//
    public static bool CheckPathRange(PathNode selectPath, PathNode targetPath, int range, bool igoreHieght)
    {
        float maxDistance = range * 1.76f;
        float disToTarget = GameFunctions.CalculateDis_WithoutY(selectPath, targetPath);
        bool canbe = false;
        //Debug.Log("disToTarget: " + disToTarget + ", MaxRange: " + maxDistance);
        if (disToTarget < maxDistance) canbe = true;

        if (range <= 1)
        {
            float heightDiff = selectPath.height - targetPath.height;
            if (heightDiff > 3 || heightDiff < -3)
            {
                canbe = false;
            }
        }
        return canbe;
    }

    public static bool CheckAttackRange(Unit attacker, Unit defender, int range, bool igoreHeight, bool isFriendly, bool igoreCorpse)
    {
        bool isCorrectTarget = false;
        bool isInRange = false;
        bool canReach = true;


        //if target is friendly unit or enemy unit
        if (isFriendly == true && attacker.unitTeam == defender.unitTeam) isCorrectTarget = true;
        else if (attacker.unitTeam != defender.unitTeam) isCorrectTarget = true;
        else if (!igoreCorpse && defender.unitAttribute != Unit.UnitAttribute.alive) isCorrectTarget = true;

        float maxDistance = range * 1.76f;
        float disToTarget = GameFunctions.CalculateDis_WithoutY(attacker.nodeAt, defender.nodeAt);
        if (disToTarget < maxDistance) isInRange = true;

        if (!igoreHeight)
        {
            float heightDiff = attacker.nodeAt.height - defender.nodeAt.height;
            if (heightDiff > 3 || heightDiff < -3)
            {
                canReach = false;
            }
        }

        canReach = true;

        if (isCorrectTarget && isInRange && canReach) return true;
        else return false;
    }

    //¡¾¡¾¡¾Find Campfire¡¿¡¿¡¿//
    public static CampFire FindClosestCampfire(PathNode nodeAt, List<CampFire> nearbyPos)
    {
        List<CampFire> OBList = nearbyPos;
        CampFire closestOB = OBList[0];
        float closeValue = 99999;

        for (int i = 0; i < OBList.Count; i++)
        {
            float thisValue = CalculateDis_WithoutY(nodeAt, OBList[i].nodeAt);

            if (thisValue < closeValue)
            {
                closeValue = thisValue;
                closestOB = OBList[i];
            }
        }
        return closestOB;
    }

    
    //[Visual Funciotns]//
    //[Visual Funciotns]//
    //[Visual Funciotns]//
    public static void SetPos_BetweenTwoObject(GameObject transOb, GameObject _from, GameObject _to )
    {
        transOb.transform.position = (_from.transform.position + _to.transform.position) / 2;
       
        float angleRotation = 0;
        Vector2 faceDirection;
        faceDirection = (_from.transform.position - transOb.transform.position).normalized;
        angleRotation = Mathf.Atan2(faceDirection.y, faceDirection.x) * Mathf.Rad2Deg;
        transOb.transform.eulerAngles = new Vector3(0, 0, angleRotation);
    }

    public static void DrawCurve_BetweenTwoObject(Transform point1, Transform point3, LineRenderer lineRenderer, float offset)
    {
        Vector3 pos1 = point1.position + new Vector3(0, offset, 0);
        Vector3 pos3 = point3.position + new Vector3(0, offset, 0);

        float dist = Vector3.Distance(pos1, pos3) * 0.5f;
        if (dist < 0.01f) 
        {
            lineRenderer.gameObject.SetActive(false);
            return;
        }
        if (dist > 10f) dist = 10f;
        if (dist < 0.5f) dist = 0.5f;

        Vector3 point2 = (point1.position + point3.position) / 2 + new Vector3(0, 1.5f + dist, 0); 

        int vertexCount = 12;

        var pointList = new List<Vector3>();
        for (float ratio = 0; ratio <= 1; ratio += 1.0f / vertexCount)
        {
            var tangentLineVertex1 = Vector3.Lerp(pos1, point2, ratio);
            var tangentLineVertex2 = Vector3.Lerp(point2, point3.position, ratio);
            var bezierpoint = Vector3.Lerp(tangentLineVertex1, tangentLineVertex2, ratio);
            pointList.Add(bezierpoint);
        }
        lineRenderer.positionCount = pointList.Count;
        lineRenderer.SetPositions(pointList.ToArray());
    }





    public static void Gene_LightMap(List<PathNode> _List,GameController GC)
    {
        GC.Del_LightMap();

        if (GC.selectedUnit == null) return;

       
        GameObject LightMapR = null;

        for (int i = 0; i < _List.Count; i++)
        {
            //this grid is control by player, gene a Yellow zone
            if (_List[i].unit != null)
            {
                if (_List[i] == GC.selectedUnit.nodeAt)
                {
                    LightMapR = Instantiate(GameFunctions.LoadGrid("unitStandOn"), _List[i].transform.position, Quaternion.identity);
                    LightMapR.transform.parent = _List[i].transform;
                    GC.lightMaps.Add(LightMapR);
                }

                else continue;
            }

            else
            {
                //if is not, gene a walkable 
                LightMapR = Instantiate(GameFunctions.LoadGrid("walkable"), GC.GM.GetWorldPosition_WithHeight(_List[i].x, _List[i].y), Quaternion.identity);
                LightMapR.transform.parent = _List[i].transform;
                GC.lightMaps.Add(LightMapR);
                int name = GC.GM.GetPath(_List[i].x, _List[i].y).height;
                foreach (Transform child in LightMapR.transform.GetChild(0))
                {
                    child.gameObject.name = "" + name;
                }
            }
        }
    }


    public static void Gene_LightMap_Charge(List<PathNode> chargeRoad, GameController GC, bool unitTeleport)
    {
        GC.Del_LightMap();

        GameObject LightMapR = null;

        for (int i = 0; i < chargeRoad.Count; i++)
        {
            if (!unitTeleport && i == chargeRoad.Count - 2)
            {
                LightMapR = Instantiate(GameFunctions.LoadGrid("chargeNode_Final"), GC.GM.GetWorldPosition_WithHeight(chargeRoad[i].x, chargeRoad[i].y), Quaternion.identity);
            }

            else if (unitTeleport && i == chargeRoad.Count - 1)
            {
                if (chargeRoad[i].unit != null) LightMapR = Instantiate(GameFunctions.LoadGrid("chargeNode_Wrong"), GC.GM.GetWorldPosition_WithHeight(chargeRoad[i].x, chargeRoad[i].y), Quaternion.identity);
                else LightMapR = Instantiate(GameFunctions.LoadGrid("chargeNode_Final"), GC.GM.GetWorldPosition_WithHeight(chargeRoad[i].x, chargeRoad[i].y), Quaternion.identity);
            }

            else if (chargeRoad[i] == GC.selectedUnit.nodeAt)
            {
                LightMapR = Instantiate(GameFunctions.LoadGrid("chargeNode"), GC.GM.GetWorldPosition_WithHeight(chargeRoad[i].x, chargeRoad[i].y), Quaternion.identity);
            }

            else if (chargeRoad[i].unit != null)
            {

                if (chargeRoad[i].unit.unitTeam != GC.selectedUnit.unitTeam)
                {
                    LightMapR = Instantiate(GameFunctions.LoadGrid("chargeNode_Attack"), GC.GM.GetWorldPosition_WithHeight(chargeRoad[i].x, chargeRoad[i].y), Quaternion.identity);
                }

                else if (!unitTeleport)
                {
                    LightMapR = Instantiate(GameFunctions.LoadGrid("chargeNode_Wrong"), GC.GM.GetWorldPosition_WithHeight(chargeRoad[i].x, chargeRoad[i].y), Quaternion.identity);
                }
            }

            else
            {
                LightMapR = Instantiate(GameFunctions.LoadGrid("chargeNode_Final"), GC.GM.GetWorldPosition_WithHeight(chargeRoad[i].x, chargeRoad[i].y), Quaternion.identity);
            }

            LightMapR.transform.parent = chargeRoad[i].transform;
            GC.lightMaps.Add(LightMapR);
        }
    }

    public static void Gene_TargetMap(List<PathNode> _List, Unit.UnitTeam team, bool igoreCorpse, GameController GC)
    {
        GC.Del_TargetMap();
        if (_List == null) return;

      
        GameObject selectedNode = null;
        GameObject selectedNode_Static = null;
        GameObject selectedNode_withoutUnit = null;

        selectedNode_Static = GameFunctions.LoadGrid("static");


        if (team == Unit.UnitTeam.playerTeam) { selectedNode = GameFunctions.LoadGrid("green"); selectedNode_withoutUnit = GameFunctions.LoadGrid("green_L") ; }
        if (team == Unit.UnitTeam.enemyTeam) { selectedNode = GameFunctions.LoadGrid("red"); selectedNode_withoutUnit = GameFunctions.LoadGrid("red_L"); }

        for (int i = 0; i < _List.Count; i++)
        {
            GameObject target = null;

            if (_List[i].unit != null)
            {
                if (!igoreCorpse && _List[i].unit.unitAttribute != Unit.UnitAttribute.alive)//GENE TARGET TO CORPSE
                {
                    target = Instantiate(selectedNode_Static, GC.GM.GetWorldPosition_WithHeight(_List[i].x, _List[i].y), Quaternion.identity);   
                }

                else if (_List[i].unit.unitTeam == team)//GENE TARGET TO TARGET GROUPS
                {
                    target = Instantiate(selectedNode, GC.GM.GetWorldPosition_WithHeight(_List[i].x, _List[i].y), Quaternion.identity);

                    Skill skillInput = null;
                    if (GC.skillInUse != null) skillInput = GC.skillInUse.skill;
                }

                else target = Instantiate(selectedNode_withoutUnit, GC.GM.GetWorldPosition_WithHeight(_List[i].x, _List[i].y), Quaternion.identity);
            }


            else
            {
                target = Instantiate(selectedNode_withoutUnit, GC.GM.GetWorldPosition_WithHeight(_List[i].x, _List[i].y), Quaternion.identity);
                int name = GC.GM.GetPath(_List[i].x, _List[i].y).height;
                foreach (Transform child in target.transform.GetChild(0))
                {
                    child.gameObject.name = "" + name;
                }
            }

            target.transform.parent = _List[i].transform;
            target.transform.localPosition = new Vector3(0, 0, 0);
            GC.target_lightMaps.Add(target);
        }
    }

    public static void Gene_TargetMap_Corpse(List<PathNode> _List,GameController GC)
    {
        GC.Del_TargetMap();
        if (_List == null) return;

        GameObject selectedNode = null;

        GameObject selectedNode_withoutUnit = null;

        selectedNode = GameFunctions.LoadGrid("purple");
        selectedNode_withoutUnit = GameFunctions.LoadGrid("purple_L");


        for (int i = 0; i < _List.Count; i++)
        {
            GameObject target = null;
            //if (_List[i].transform.position.y < -4) continue;
            //if (_List[i].transform.position.y > 2.9) continue;

            if (_List[i].unit != null && _List[i].unit.unitAttribute == Unit.UnitAttribute.corpse)
            {
                target = Instantiate(selectedNode, GC.GM.GetWorldPosition_WithHeight(_List[i].x, _List[i].y), Quaternion.identity);
            }
            else
            {
                target = Instantiate(selectedNode_withoutUnit, GC.GM.GetWorldPosition_WithHeight(_List[i].x, _List[i].y), Quaternion.identity);
                int name = GC.GM.GetPath(_List[i].x, _List[i].y).height;

                foreach (Transform child in target.transform.GetChild(0))
                {
                    child.gameObject.name = "" + name;
                }
            }
            target.transform.parent = _List[i].transform;
            GC.target_lightMaps.Add(target);
        }
    }
    public static void Gene_LightMap_Telepirt(List<PathNode> _List, GameController GC)
    {
        GC.Del_LightMap();

        if (GC.selectedUnit == null) return;
        GameObject LightMapR = null;

        for (int i = 0; i < _List.Count; i++)
        {
            //this grid is control by player, gene a Yellow zone
            if (_List[i].unit != null)
            {
            }

            else
            {
                //if is not, gene a walkable 
                LightMapR = Instantiate(GameFunctions.LoadGrid("green"), GC.GM.GetWorldPosition_WithHeight(_List[i].x, _List[i].y), Quaternion.identity);
                LightMapR.transform.parent = _List[i].transform;
                GC.lightMaps.Add(LightMapR);
                int name = GC.GM.GetPath(_List[i].x, _List[i].y).height;
                foreach (Transform child in LightMapR.transform.GetChild(0))
                {
                    child.gameObject.name = "" + name;
                }
            }
        }
    }



    public static GameObject LoadGrid(string type)
    {
        GameObject grid = null;
        GridList list = Resources.Load<GridList>("GridPrefab/MyGridList");

        if (type == "walkable") grid = list.moveableNode;
        if (type == "walkable_FriendOn") grid = list.moveableNode_FriendOn;
        if (type == "unitStandOn") grid = list.nodeUnderUnit;

        if (type == "purple_L") grid = list.purpleNode_Light;
        if (type == "purple") grid = list.purpleNode;

        if (type == "green_L") grid = list.greenNode_Light;
        if (type == "green") grid = list.greenNode;

        if (type == "red_L") grid = list.redNode_Light;
        if (type == "red") grid = list.redNode;

        if (type == "static") grid = list.staticNode;

        if (type == "chargeNode") grid = list.chargeNode;
        if (type == "chargeNode_Attack") grid = list.chargeNode_Attack;
        if (type == "chargeNode_Final") grid = list.chargeNode_Final;
        if (type == "chargeNode_Wrong") grid = list.chargeNode_Wrong;

        if (type == "chargeNode_Enemy") grid = list.chargeNode_Enemy;
        if (type == "redNode_Boss") grid = list.redNode_Boss;

        return grid;
    }
}
