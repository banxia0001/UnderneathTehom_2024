using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealTower_Controller : MonoBehaviour
{
    
    private Unit thisUnit;
    public GameObject hitSpecialEffect;
    private int timer;
    private void Start()
    {
        thisUnit = this.gameObject.GetComponent<Unit>();
        timer = 1;
    }
    public void TriggerHeal(GameController GC)
    {
        GC.isAttacking = true;
        StartCoroutine(SearchForHeal(GC));
    }
    private IEnumerator SearchForHeal(GameController GC)
    {
        GC.isAttacking = true;

        yield return new WaitForSeconds(0.3f);

        List<Unit> unitList = GameFunctions.FindUnits_ByDistance(GameFunctions.FindNodes_ByDistance(thisUnit.nodeAt,5,true),Unit.UnitTeam.playerTeam,true);

        if (unitList != null && unitList.Count != 0)
        {

            Unit healUnit = AIFunctions.FindUnit_ByLowestHP_AI(unitList);

            if (healUnit != null)
            {
                this.thisUnit.InputAnimation("attack");
                yield return new WaitForSeconds(0.5f);
                healUnit.HealthChange(4, 0, "Heal");
                Instantiate(hitSpecialEffect, healUnit.transform.position + new Vector3(0, 0, -.1f), Quaternion.identity);
                yield return new WaitForSeconds(0.25f);
            }
        }

        timer--;
        if (timer < 0)
        {
            timer = 1;
            yield return new WaitForSeconds(0.1f);
            thisUnit.Gene_DropFlesh(1);
            yield return new WaitForSeconds(0.3f);
        }
       
        yield return new WaitForSeconds(0.5f);
        this.thisUnit.UnitEnable(false);
        GC.isAttacking = false;
    }
}
