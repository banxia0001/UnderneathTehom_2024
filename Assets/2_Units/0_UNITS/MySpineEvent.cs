using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

public class MySpineEvent : MonoBehaviour
{
    public GameController GC;

    public GameObject rat_attakVFX;

    private void SpawnVFX_RatAttack()
    {
        Unit currentUnit = GameController.currentActionUnit;
        GameObject specialEffect = Instantiate(rat_attakVFX, UnitFunctions.GetUnitMiddlePoint(currentUnit), Quaternion.identity);
        specialEffect.transform.localScale = currentUnit.TranformOffsetFolder.transform.localScale;
    }
    public void HandleEvent(TrackEntry trackEntry, Spine.Event e)
    {

        if (e.Data.Name == "rat_attakVFX")
        {
            SpawnVFX_RatAttack();
            GC.SC.InputVFX(SFX_Controller.VFX.hit3);
        }

        if (e.Data.Name == "hit")
        {
           GC.SC.InputVFX(SFX_Controller.VFX.hit);
        }

        if (e.Data.Name == "hit2")
        {
            GC.SC.InputVFX(SFX_Controller.VFX.hit2);
        }

        if (e.Data.Name == "hit3")
        {
            GC.SC.InputVFX(SFX_Controller.VFX.hit3);
        }

        if (e.Data.Name == "shieldSound")
        {
            GC.SC.InputVFX(SFX_Controller.VFX.shield);
        }

        if (e.Data.Name == "groundHit")
        {
            GC.SC.InputVFX(SFX_Controller.VFX.groundSound);
        }

        if (e.Data.Name == "arrow") GC.SC.InputVFX(SFX_Controller.VFX.arrow);

        if (e.Data.Name == "hurt_Shield") GC.SC.InputVFX(SFX_Controller.VFX.hitShield);
        if (e.Data.Name == "roat_Great") GC.SC.InputVFX(SFX_Controller.VFX.roat_Great);
        if (e.Data.Name == "roat_Banshee") GC.SC.InputVFX(SFX_Controller.VFX.roat_Banshee);
        if (e.Data.Name == "roat_Minor") GC.SC.InputVFX(SFX_Controller.VFX.roat_Minor);

        if (e.Data.Name == "revival") GC.SC.InputVFX(SFX_Controller.VFX.revival);
        if (e.Data.Name == "eat_Hit") GC.SC.InputVFX(SFX_Controller.VFX.eat_Hit);
        if (e.Data.Name == "ding") GC.SC.InputVFX(SFX_Controller.VFX.ding);


        if (e.Data.Name == "rat_Minor_Attack") GC.SC.InputVFX(SFX_Controller.VFX.rat_Minor_Attack);
        if (e.Data.Name == "rat__Minor_Scream") GC.SC.InputVFX(SFX_Controller.VFX.rat_Minor_Scream);

        if (e.Data.Name == "rat_Great_Attack") GC.SC.InputVFX(SFX_Controller.VFX.rat_Great_Attack);
        if (e.Data.Name == "rat_Great_Buff") GC.SC.InputVFX(SFX_Controller.VFX.rat_Great_Buff);
        if (e.Data.Name == "rat_Great_PoisonSpread") GC.SC.InputVFX(SFX_Controller.VFX.rat_Great_PoisonSpread);
        if (e.Data.Name == "rat_Great_Roar") GC.SC.InputVFX(SFX_Controller.VFX.rat_Great_Roar);
        if (e.Data.Name == "rat_Great_Hurt") GC.SC.InputVFX(SFX_Controller.VFX.rat_Great_Hurt);
       
        if (e.Data.Name == "rat_Ranger_Scream") GC.SC.InputVFX(SFX_Controller.VFX.rat_Ranger_Scream);
        if (e.Data.Name == "rat_Ranger_Shoot") GC.SC.InputVFX(SFX_Controller.VFX.rat_Ranger_Shoot);
        if (e.Data.Name == "rat_Ranger_Hurt") GC.SC.InputVFX(SFX_Controller.VFX.rat_Ranger_Hurt);
        if (e.Data.Name == "rat_Moca") GC.SC.InputVFX(SFX_Controller.VFX.rat_ChargePepare);
        if (e.Data.Name == "rat_chargeHit") GC.SC.InputVFX(SFX_Controller.VFX.rat_ChargeHit);


        if (e.Data.Name == "rat_Minor_Scream") GC.SC.InputVFX(SFX_Controller.VFX.rat_Minor_Scream);
        if (e.Data.Name == "rat_jumpInto") GC.SC.InputVFX(SFX_Controller.VFX.rat_Minor_JumpTo);


        if (e.Data.Name == "nameless_Hit") GC.SC.InputVFX_Simple(0);
        if (e.Data.Name == "nameless_GainBuff") GC.SC.InputVFX_Simple(1);
        if (e.Data.Name == "nameless_Death") GC.SC.InputVFX_Simple(2);
        if (e.Data.Name == "nameless_CastDone") GC.SC.InputVFX_Simple(3);
        if (e.Data.Name == "nameless_Awake") GC.SC.InputVFX_Simple(4);
        if (e.Data.Name == "nameless_Attack") GC.SC.InputVFX_Simple(5);

        if (e.Data.Name == "nameless_Hit") GC.SC.InputVFX_Simple(6);
        if (e.Data.Name == "turret_Enterbattle") GC.SC.InputVFX_Simple(7);
        if (e.Data.Name == "turret_Attack") GC.SC.InputVFX_Simple(8);
        if (e.Data.Name == "Sentry_Heal") GC.SC.InputVFX_Simple(9);
        if (e.Data.Name == "Sentry_Hurt") GC.SC.InputVFX_Simple(10);


        if (e.Data.Name == "taunt_Attack") GC.SC.InputVFX_Simple(15);
        if (e.Data.Name == "taunt_Hit") GC.SC.InputVFX_Simple(16);

    }

}
