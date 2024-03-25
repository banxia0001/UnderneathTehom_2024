using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillFunctions : MonoBehaviour
{
    public static void Skill_SimpleDamageCalculate(int dam, Unit attacker, Unit defender, GameObject hitEffect)
    {
        if (attacker == defender) return;

        dam -= defender.currentData.armorNow;
        if (dam < 0) dam = 0;

        if (hitEffect != null)
        {
            GameObject specialEffect = Instantiate(hitEffect, defender.transform.position + new Vector3(0, 0, -.1f), Quaternion.identity);
            UnitFunctions.Flip_Simple(attacker.transform.position.x, specialEffect.transform.position.x, specialEffect);
        }

        defender.HealthChange(-(int)dam, UnitFunctions.Check_KillFPBonus(attacker), "Damage");
    }

    public static void Skill_SimpleHealCalculate(Unit attacker, Unit defender, Skill skillInUse)
    {
        int dam =  Random.Range(skillInUse.damMin ,skillInUse.damMax);

        if (skillInUse.hitSpecialEffect != null)
        {
            GameObject specialEffect = Instantiate(skillInUse.hitSpecialEffect, defender.transform.position + new Vector3(0, 0, -.1f), Quaternion.identity);
        }
        defender.HealthChange(dam, UnitFunctions.Check_KillFPBonus(attacker), "Heal");
    }

    public static IEnumerator Skill_Buff(Unit attacker, Unit defender, Skill skillInUse, bool isSplash)
    {
        GameController GC = FindObjectOfType<GameController>();
        GC.isAttacking = true;
        GC.isMoving = false;
        GC.isAIThinking = false;
        GameController.currentActionUnit = attacker;

        if (attacker != null && defender != null)
        {           
            UnitFunctions.Flip(attacker.transform.position.x, defender.transform.position.x, attacker);
            attacker.InputAnimation(skillInUse.animTriggerType.ToString());
           
            yield return new WaitForSeconds(skillInUse.timer_PerformAction);

            if (skillInUse.castSpecialEffect != null)
            {
                GameObject specialEffect = Instantiate(skillInUse.castSpecialEffect, UnitFunctions.GetUnitMiddlePoint(attacker), Quaternion.identity);
            }


            //如果这个技能是AOE的，则把这个技能带回GC,对周围所有单位一起释放，因此，要在wait前面。
            if (skillInUse.splashRange > 0 && !isSplash)
                SkillFunctions.Skill_Splash(attacker, defender, skillInUse, true);

            yield return new WaitForSeconds(skillInUse.timer_AttackHit);

            SFX_Controller sfx = FindObjectOfType<SFX_Controller>();
            sfx.InputVFX(SFX_Controller.VFX.heal);

            //BUFF
            if (skillInUse.type == Skill._Type.TargetToBuff || skillInUse.type == Skill._Type.TargetToDebuff) 
            {
                

                Buff buff = skillInUse.buff;
                if (skillInUse.randomBuffs.Length > 0)
                {
                    buff = skillInUse.randomBuffs[Random.Range(0, skillInUse.randomBuffs.Length)];
                }
                if (buff != null)
                {
                    if (!skillInUse.buffSelf) defender.InputBuff(buff, buff.canRepeatBuff);
                    else if (skillInUse.buffSelf && !isSplash) attacker.InputBuff(buff, buff.canRepeatBuff);
                }
            }

            //RELOAD
            if (skillInUse.type == Skill._Type.Reload)
            {
                foreach (_Skill skill in defender.currentData.Skill)
                {
                    skill.CD = 0;
                }
                defender.popTextString.Add(new textInformation("Reload!", Resources.Load<Sprite>("Image/Sword")));
               
                GC.Select_Unit(GC.selectedUnit.nodeAt,false);
            }

            //HEAL
            if (skillInUse.causeHeal)
            {
                Skill_SimpleHealCalculate(attacker, defender, skillInUse);
            }
               

            if (skillInUse.addArmor != 0)
            {
                defender.currentData.armorNow += skillInUse.addArmor;
                defender.popTextString.Add(new textInformation("+" + "<color=#B7FF74>" + skillInUse.addArmor + "</color>", Resources.Load<Sprite>("Image/Armor")));
            }

            if (skillInUse.hitSpecialEffect != null)
            {
                GameObject specialEffect = Instantiate(skillInUse.hitSpecialEffect, UnitFunctions.GetUnitMiddlePoint(defender), Quaternion.identity);
            }

        }
        yield return new WaitForSeconds(skillInUse.timer_WaitAfterAttack);
        if (!skillInUse.isInstant)attacker.UnitEnable(false);
        else GC.Select_Unit(attacker.nodeAt,false);
        GC.isAttacking = false;
    }










    ///////////////////////////////////////////////////////
    //AOE_Skill//AOE_Skill//AOE_Skill//AOE_Skill//AOE_Skill
    //AOE_Skill//AOE_Skill//AOE_Skill//AOE_Skill//AOE_Skill
    //AOE_Skill//AOE_Skill//AOE_Skill//AOE_Skill//AOE_Skill
    ///////////////////////////////////////////////////////
    public static IEnumerator Skill_AttackALine(Unit attacker, PathNode attackNode, UnitWeapon damage, Skill skillInUse)
    {
        GameController GC = FindObjectOfType<GameController>();
        GC.isAttacking = true;
        GC.isMoving = false;
        GC.isAIThinking = false;
        GameController.currentActionUnit = attacker;

        if (attacker != null)
        {
            GameObject geneProjectile = null;
            if (skillInUse.projectile != null) geneProjectile = skillInUse.projectile;

            //[Buff]
            Buff buff = skillInUse.buff;
            if (skillInUse.randomBuffs.Length > 0)
            {
                buff = skillInUse.randomBuffs[Random.Range(0, skillInUse.randomBuffs.Length)];
            }

            if (buff != null)
                if (skillInUse.buffSelf) attacker.buffList.Add(new _Buff(buff));

            UnitFunctions.Flip(attacker.transform.position.x, attackNode.transform.position.x, attacker);
            attacker.InputAnimation(skillInUse.animTriggerType.ToString());


            yield return new WaitForSeconds(skillInUse.timer_PerformAction);

            //[When attack a line, it will generate a projectile]
            GameObject Projectile = Instantiate(geneProjectile, UnitFunctions.GetUnitMiddlePoint(attacker), Quaternion.identity) as GameObject;
            List<PathNode> nodesTogo = GameFunctions.FindNodes_InOneLine(attacker.nodeAt, attackNode, skillInUse.range, true, false, false,false);

            //Debug.Log(nodesTogo.Count);
            Projectile.GetComponent<Projectile>().TriggerProjectile(attacker, nodesTogo, damage, skillInUse);
            yield return new WaitForSeconds(skillInUse.timer_WaitAfterAttack);
        }
        yield return new WaitForSeconds(0.25f);

        if (!skillInUse.isInstant) attacker.UnitEnable(false);
        GC.Select_Unit(attacker.nodeAt,false);

        GC.isAttacking = false;
    }


    public static IEnumerator Skill_AttackAllPlayer(Unit attacker, Skill skillInUse)
    {
        GameController GC = FindObjectOfType<GameController>();
        GC.isAttacking = true;
        GC.isMoving = false;
        GC.isAIThinking = false;
        GameController.currentActionUnit = attacker;

        GameObject ob = GameObject.FindGameObjectWithTag("SkillPortraitDisplay");
        ob.GetComponent<Animator>().SetBool("enable", true);
        yield return new WaitForSeconds(1.4f);
        ob.GetComponent<Animator>().SetBool("enable", false);


        attacker.InputAnimation(skillInUse.animTriggerType.ToString());

        yield return new WaitForSeconds(skillInUse.timer_PerformAction);

        if (skillInUse.castSpecialEffect != null)
        {
            GameObject specialEffect = Instantiate(skillInUse.castSpecialEffect, UnitFunctions.GetUnitMiddlePoint(attacker), Quaternion.identity);
        }

        List<Unit> splashUnit = GameController.playerList;
        foreach (Unit unit in splashUnit)
        {
            GC.Attack_Splash(attacker, unit, skillInUse);
        }

        //Loop
        yield return new WaitForSeconds(skillInUse.timer_AttackHit);

        attacker.InputAnimation(skillInUse.animTriggerType.ToString());

        if (skillInUse.castSpecialEffect != null)
        {
            GameObject specialEffect = Instantiate(skillInUse.castSpecialEffect, UnitFunctions.GetUnitMiddlePoint(attacker), Quaternion.identity);
        }

        splashUnit = GameController.playerList;
        foreach (Unit unit in splashUnit)
        {
            GC.Attack_Splash(attacker, unit, skillInUse);
        }

        yield return new WaitForSeconds(skillInUse.timer_WaitAfterAttack);
        GC.isAttacking = false;
        GC.isMoving = false;
        GC.isAIThinking = false;
    }

    public static void Skill_Splash(Unit attacker, Unit defender, Skill skillInUse, bool isFriendly)
    {
        Unit.UnitTeam team = Unit.UnitTeam.neutral;
     
        if (attacker.unitTeam == Unit.UnitTeam.playerTeam)
        {
            if (isFriendly) team = Unit.UnitTeam.playerTeam;
            else team = Unit.UnitTeam.enemyTeam;
        }
        else
        {
            if (isFriendly) team = Unit.UnitTeam.enemyTeam;
            else team = Unit.UnitTeam.playerTeam;
        }

        List<Unit> splashUnit = new List<Unit>();

        if (!skillInUse.castAsCirclePoint)
            splashUnit = GameFunctions.FindUnits_ByDistance(GameFunctions.FindNodes_ByDistance(defender.nodeAt, skillInUse.splashRange, true), team, false);

        else
            splashUnit = GameFunctions.FindUnits_ByDistance(GameFunctions.FindNodes_ByDistance(attacker.nodeAt, skillInUse.splashRange, true), team, false);

        foreach (Unit unit in splashUnit)
        {
            if (isFriendly)
            {
                if (unit != defender)
                    GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().Buff_Splash(attacker, unit, skillInUse);
            }
            else
            {
                if (unit != attacker && unit != defender)//别打到自己
                    GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().Attack_Splash(attacker, unit, skillInUse);
            }
        }
    }









    ////////////////////////////////////////////////////////
    //Charge&KnockBack //Charge&KnockBack //Charge&KnockBack
    //Charge&KnockBack //Charge&KnockBack //Charge&KnockBack
    //Charge&KnockBack //Charge&KnockBack //Charge&KnockBack
    ////////////////////////////////////////////////////////
    public static void Skill_ChargeDamage(Unit attacker, Unit defender, UnitWeapon damage, bool enemyCanFightBack, Skill skill, int distance)
    {
        GameController GC = FindObjectOfType<GameController>();


        UnitFunctions.Flip(defender.transform.position.x, attacker.transform.position.x, defender);

        UnitFunctions.Flip(attacker.transform.position.x, defender.transform.position.x, attacker);

        hitInformation hitInfo = GameFunctions.CalculateHit_End(attacker, defender, damage, skill, false);
        GC.SC.Hit();
        Buff buff = skill.buff;

        //CHECK IF HIT
        if (hitInfo.hit)
        {
            damageInformation damInfo = GameFunctions.calculateDamage(attacker, defender, damage, skill, hitInfo.crit, false);
            int finalDam = damInfo.damage + distance / 2;
            if (defender != null)
            {
                if (buff != null)
                {
                    //TUANT
                    if (buff.buffType == global::Buff._buffType.tuant)
                    {
                        buff.tuantTarget = attacker;
                    }
                    if (!skill.buffSelf) defender.InputBuff(buff, buff.canRepeatBuff);
                    else if (skill.buffSelf) attacker.InputBuff(buff, buff.canRepeatBuff);
                }

               // defender.popTextString.Add(new textInformation(hitInfo.addone + "- " + "<color=#FF5032>" + finalDam, Resources.Load<Sprite>("Image/Damage")));
                defender.HealthChange(-finalDam, UnitFunctions.Check_KillFPBonus(attacker), "Damage");

                if (skill != null && skill.causeKnockBack == true)
                    GC.TriggerKnockBack(attacker, defender, skill.KnockBack_Distance, 0);
            }
        }
        else defender.popTextString.Add(new textInformation("Dodge", null));
    }

    public static IEnumerator Skill_UnitIsKnockBack_InOneLine(Unit attacker, Unit defender, int distance,float waitTime)
    {
        GridManager GM = GameObject.FindGameObjectWithTag("GameController").GetComponent<GridManager>();
        yield return new WaitForSeconds(waitTime);
        List<PathNode> knockBackNode = GameFunctions.FindNodes_InOneLine(attacker.nodeAt, defender.nodeAt, distance, true, true, false,false);
        defender.popTextString.Add(new textInformation( "Knock-Back!", null));

        if (knockBackNode.Count > 1)
        {
            for (int i = 1; i < knockBackNode.Count; i++)
            {
                if (knockBackNode[i].unit != null)
                {
                    //If skill Hit some unit. 
                    PathNode toPath = knockBackNode[i];
                    defender.Input_CombatPos_StraightToStaying(toPath.transform.position, 3f, 0.4f, 0.1f);

                    //[Charge Hit unit behind unit damage]
                    //If the unit during knock back, hit a unit in the way, cause damage to that unit.
                    Skill_SimpleDamageCalculate(i, defender, knockBackNode[i].unit, null);
                    break;
                }

                defender.UnitPosition_CanMove(knockBackNode[i], 0, false, 3f, false);
            }
        }

        else
        {
            Vector3 knockbackPos = UnitFunctions.Find_UnitShouldGoPosition_WhenKnochBack(attacker, defender);
            defender.Input_CombatPos(knockbackPos, 1.355f, 0.3f, 0.02f);
        }
    }



    public static IEnumerator Skill_Teleport(Unit attacker,PathNode path, Skill skillInUse)
    {
        GameController GC = FindObjectOfType<GameController>();
        GC.isAttacking = true;
        GC.isMoving = false;
        GC.isAIThinking = false;
        GameController.currentActionUnit = attacker;
        attacker.InputAnimation(skillInUse.animTriggerType.ToString());
        yield return new WaitForSeconds(skillInUse.timer_PerformAction);

        if (skillInUse.castSpecialEffect != null)
        {
            GameObject specialEffect = Instantiate(skillInUse.castSpecialEffect, UnitFunctions.GetUnitMiddlePoint(attacker), Quaternion.identity);
        }

        FindObjectOfType<SFX_Controller>().InputVFX_Simple(13);

        attacker.transform.position = path.transform.position;
        attacker.UnitPosition_CanMove(path, 0, false, 0.77f, false);
        yield return new WaitForSeconds(0.1f);

        if (!skillInUse.isInstant) attacker.UnitEnable(false);
        GC.Select_Unit(attacker.nodeAt, false);
        GC.isAttacking = false;

        GC.Update_MachineGunPosition();
    }
    public static IEnumerator Skill_TargetTile(Unit attacker, PathNode path, Skill skillInUse)
    {
        GameController GC = FindObjectOfType<GameController>();
        GC.isAttacking = true;
        GC.isMoving = false;
        GC.isAIThinking = false;
        GameController.currentActionUnit = attacker;
        attacker.InputAnimation(skillInUse.animTriggerType.ToString());
        yield return new WaitForSeconds(skillInUse.timer_PerformAction);

        if (skillInUse.castSpecialEffect != null)
        {
            GameObject specialEffect = Instantiate(skillInUse.castSpecialEffect, UnitFunctions.GetUnitMiddlePoint(attacker), Quaternion.identity);
        }

        FindObjectOfType<SFX_Controller>().InputVFX_Simple(13);

        attacker.transform.position = path.transform.position;
        attacker.UnitPosition_CanMove(path, 0, false, 0.77f, false);
        yield return new WaitForSeconds(0.1f);

        if (!skillInUse.isInstant) attacker.UnitEnable(false);
        GC.Select_Unit(attacker.nodeAt, false);
        GC.isAttacking = false;

        GC.Update_MachineGunPosition();
    }


    public static IEnumerator Skill_Exchange(Unit attacker, Unit defender, Skill skillInUse)
    {
        GameController GC = FindObjectOfType<GameController>();
        GC.isAttacking = true;
        GC.isMoving = false;
        GC.isAIThinking = false;
        GameController.currentActionUnit = attacker;
        attacker.InputAnimation(skillInUse.animTriggerType.ToString());
        yield return new WaitForSeconds(skillInUse.timer_PerformAction);


        if (skillInUse.castSpecialEffect != null)
        {
            GameObject specialEffect = Instantiate(skillInUse.castSpecialEffect, UnitFunctions.GetUnitMiddlePoint(attacker), Quaternion.identity);
        }

        yield return new WaitForSeconds(skillInUse.timer_AttackHit);

        PathNode ALocation = attacker.nodeAt;
        PathNode BLocation = defender.nodeAt;
        ALocation.unit = null;
        BLocation.unit = null;

        attacker.UnitPosition_CanMove(BLocation, 0, false, 0.77f, false);
        defender.UnitPosition_CanMove(ALocation, 0, false, 0.77f, false);

        yield return new WaitForSeconds(0.1f);

        ALocation.unit = defender;
        BLocation.unit = attacker;

        yield return new WaitForSeconds(skillInUse.timer_WaitAfterAttack);
        if (!skillInUse.isInstant) attacker.UnitEnable(false);
        GC.Select_Unit(attacker.nodeAt, false);
        GC.isAttacking = false;
    }

    ////////////////////////////////////////
    //Summon//Summon//Summon//Summon//Summon
    //Summon//Summon//Summon//Summon//Summon
    //Summon//Summon//Summon//Summon//Summon
    ////////////////////////////////////////

    public static IEnumerator Skill_Summoning(PathNode geneNode, Unit attacker, GameObject geneOb, Skill skillInUse, Unit.UnitTeam unitTeam, GameObject SpecialEffect)
    {
        if (geneNode.unit != null)
        {
            Destroy(geneNode.unit);
            Destroy(geneNode.unit.gameObject, skillInUse.timer_PerformAction + skillInUse.timer_AttackHit);
            geneNode.unit = null;
        }

        GameController GC = FindObjectOfType<GameController>();
        GC.isAttacking = true;
        GC.isMoving = false;
        GC.isAIThinking = false;
        GameController.currentActionUnit = attacker;
        attacker.InputAnimation(skillInUse.animTriggerType.ToString());

        yield return new WaitForSeconds(skillInUse.timer_PerformAction);


        if (skillInUse.castSpecialEffect != null)
        {
            GameObject specialEffect = Instantiate(skillInUse.castSpecialEffect, UnitFunctions.GetUnitMiddlePoint(attacker), Quaternion.identity);
        }


        yield return new WaitForSeconds(skillInUse.timer_AttackHit);

        Skill_GeneUnit(geneNode, geneOb, unitTeam, SpecialEffect);
        Buff buff = skillInUse.buff;
        if (skillInUse.randomBuffs.Length > 0) buff = skillInUse.randomBuffs[Random.Range(0, skillInUse.randomBuffs.Length)];
        if (buff != null) if (skillInUse.buffSelf) attacker.InputBuff(buff, buff.canRepeatBuff);

        yield return new WaitForSeconds(skillInUse.timer_WaitAfterAttack);

        if (!skillInUse.isInstant) attacker.UnitEnable(false);
        GC.Select_Unit(attacker.nodeAt, false);
        GC.isAttacking = false;
    }


    public static void Skill_GeneUnit(PathNode geneNode, GameObject unit, Unit.UnitTeam team, GameObject SpecialEffect)
    {
        GameObject unitObject = Instantiate(unit, geneNode.transform.position, Quaternion.identity);
        Unit thisUnit = unitObject.GetComponent<Unit>();
        thisUnit.nodeAt = geneNode;
        thisUnit.nodeAt.unit = thisUnit;

        if (SpecialEffect != null)
        {
            Instantiate(SpecialEffect, geneNode.transform.position, Quaternion.identity);
        }

        if (team == Unit.UnitTeam.playerTeam)
        {
            thisUnit.unitTeam = Unit.UnitTeam.playerTeam;
            GameController.playerList.Add(thisUnit); 
        }

        if (team == Unit.UnitTeam.enemyTeam)
        {
            thisUnit.unitTeam = Unit.UnitTeam.enemyTeam;
            GameController.enemyList.Add(thisUnit); 
        }
    }



    public static IEnumerator Skill_BiteTheDeath(PathNode geneNode, Unit attacker, Skill skillInUse)
    {
        GameController GC = FindObjectOfType<GameController>();
        GC.isAttacking = true;
        GC.isMoving = false;
        GC.isAIThinking = false;
        GameController.currentActionUnit = attacker;
        attacker.InputAnimation(skillInUse.animTriggerType.ToString());

        yield return new WaitForSeconds(skillInUse.timer_PerformAction);

        if (skillInUse.castSpecialEffect != null)
        {
            GameObject specialEffect = Instantiate(skillInUse.castSpecialEffect, attacker.transform.position + new Vector3(0, 0, -.1f), Quaternion.identity);
        }

        for (int i = 0; i < skillInUse.num; i++)
        {
            yield return new WaitForSeconds(skillInUse.timer_AttackHit);
            if (skillInUse.hitSpecialEffect != null)
            {
                GameObject specialEffect = Instantiate(skillInUse.hitSpecialEffect, attacker.transform.position + new Vector3(0, 0, -.1f), Quaternion.identity);
            }
            Skill_SimpleHealCalculate(attacker, attacker, skillInUse);
        }

        if (geneNode.unit != null)
        {
            Destroy(geneNode.unit);
            geneNode.unit = null;
        }


        yield return new WaitForSeconds(skillInUse.timer_WaitAfterAttack);
        if (!skillInUse.isInstant) attacker.UnitEnable(false);
        GC.Select_Unit(attacker.nodeAt, false);
        GC.isAttacking = false;
    }
}









