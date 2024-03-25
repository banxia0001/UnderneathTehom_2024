using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleshBlock_Controller : MonoBehaviour
{
    private Unit thisUnit;
    public GameObject hitSpecialEffect;
    public Buff tauntBuff;
    private void Start()
    {
        thisUnit = this.GetComponent<Unit>();
    }
    public void TriggerTaunt(GameController GC)
    {
        GC.isAttacking = true;
        StartCoroutine(SearchForTaunt(GC));
    }
    private IEnumerator SearchForTaunt(GameController GC)
    {
        GC.isAttacking = true;
        //List<PathNode> nearbyNode = GameFunctions.FindNodes_ByDistance(thisUnit.nodeAt, 2, false);
        //List<Unit> nearbyEnemy = GameFunctions.FindUnits_ByDistance(nearbyNode, Unit.UnitTeam.enemyTeam, true);
        Unit nearestUnit = GameFunctions.FindClosestUnit_By_Grid(1,thisUnit);
        yield return new WaitForSeconds(1f);

        SFX_Controller sfx = FindObjectOfType<SFX_Controller>();
        sfx.InputVFX(SFX_Controller.VFX.rat_Great_Roar);
        GC.cameraAnim.SetTrigger("Shake1");

        if (nearestUnit != null)
        {
            this.thisUnit.InputAnimation("attack");
            yield return new WaitForSeconds(1.5f);
            tauntBuff.tuantTarget = thisUnit;
            Instantiate(hitSpecialEffect, nearestUnit.transform.position + new Vector3(0, 0, -.1f), Quaternion.identity);
            nearestUnit.InputBuff(tauntBuff, true);
            nearestUnit.InputAnimation("hurt");
        }

        yield return new WaitForSeconds(1.5f);
        this.thisUnit.UnitEnable(false);
        GC.isAttacking = false;
    }
}
