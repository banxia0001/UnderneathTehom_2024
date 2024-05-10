using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIUnitController : MonoBehaviour
{
    //public List<Unit> MachineGun;
    //public List<Unit> MeatBlock;
    //public List<Unit> HealTower;
    public Unit selectedUnit_AI;
    public GameController GC;

    /// <summary>
    /// MachineGun  /// MachineGun  ///MachineGun  /// MachineGun
    /// </summary>
    public void Revival_MachineGun_Choosing()
    {
        if (GC.isMoving || GC.isAttacking || GC.isAIThinking) return;
      
        if (selectedUnit_AI != null)
        {
            if (selectedUnit_AI.isActive == false)
            {
                ResetSelectedAI();
            }

            else return;
        }

        bool AllSentryActived = true;

        if (selectedUnit_AI == null)
            for (int i = 0; i < GameController.playerList.Count; i++)
            {
                Unit unit = GameController.playerList[i];
                if (unit.unitSpecialState == Unit.UnitSpecialState.machineGun && unit.isActive == true)
                {
                    AllSentryActived = false;
                    selectedUnit_AI = unit;
                    Revival_MachineGun_Active();
                    break;
                }

                if (unit.unitSpecialState == Unit.UnitSpecialState.healTower && unit.isActive == true)
                {
                    AllSentryActived = false;
                    selectedUnit_AI = unit;
                    Revival_HealTower_Active();
                    break;
                }

                if (unit.unitSpecialState == Unit.UnitSpecialState.block && unit.isActive == true)
                {
                    AllSentryActived = false;
                    selectedUnit_AI = unit;
                    Revival_FleshBlock_Active();
                    break;
                }
            }

        if (AllSentryActived)
        {
            Sentry_StageEnd();
            return;
        }
    }

    private void Revival_MachineGun_Active()
    {
        GC.isAIThinking = true;
        GC.Update_MachineGunPosition();
        if (selectedUnit_AI == null)
        {
            Debug.LogWarning("Unit Missing");
        }
        else
        {
            selectedUnit_AI.GetComponent<AutoMachineGun_Controller>().Trigger_AttackEnemy();
        }
        GC.isAIThinking = false;
    }
    private void Revival_FleshBlock_Active()
    {
        GC.isAIThinking = true;
        if (selectedUnit_AI == null)
        {
            Debug.LogWarning("Unit Missing");
        }
        else
        {
            selectedUnit_AI.GetComponent<FleshBlock_Controller>().TriggerTaunt(GC);
        }
        GC.isAIThinking = false;
    }

    private void Revival_HealTower_Active()
    {
        GC.isAIThinking = true;
        if (selectedUnit_AI == null)
        {
            Debug.LogWarning("Unit Missing");
        }
        else
        {
            selectedUnit_AI.GetComponent<HealTower_Controller>().TriggerHeal(GC);
        }
        GC.isAIThinking = false;
    }
    public void Sentry_StageEnd()
    {
        selectedUnit_AI = null;
        StartCoroutine(GC.stageController.EnemyTurnStart());
    }










    /// <summary>
    /// Enemy Update  /// Enemy Update  /// Enemy Update  /// Enemy Update
    /// </summary>
    /// 
    public void EnemyChoosing()
    {
        if (selectedUnit_AI != null) return;
        if (GC.isMoving || GC.isAttacking || GC.isAIThinking) return;
        if (GameController.enemyList.Count == 0 || GameController.enemyList == null) return;

        if (selectedUnit_AI == null)
            for (int i = 0; i < GameController.enemyList.Count; i++)
            {
                if (GameController.enemyList[i] != null)
                    if (GameController.enemyList[i].isActive == true)
                    {
                        if (i == 0) StartCoroutine(EnemyTurnStart(GameController.enemyList[i], false));
                        else StartCoroutine(EnemyTurnStart(GameController.enemyList[i], true));
                        break;
                    }
            }
    }
    public IEnumerator EnemyTurnStart(Unit unit, bool waitMore)
    {
        GC.isAIThinking = true;
        selectedUnit_AI = unit;
        Debug.Log(selectedUnit_AI.name + " is Selected:" + selectedUnit_AI.isActive);
        yield return new WaitForSeconds(0.2f);
      
        if (waitMore) yield return new WaitForSeconds(.2f);
        GC.isAIThinking = false;
        GC.gameState = GameController._gameState.enemyTurn_Action;
    }

    public void EnemyThinking()
    {
        if (selectedUnit_AI == null)
        {
            GC.chooseAiCoolDown = 0f;
            GC.gameState = GameController._gameState.enemyTurn_ChooseUnit;
            return;
        }

        if (selectedUnit_AI.isActive == false)
        {
            ResetSelectedAI();
            GC.gameState = GameController._gameState.enemyTurn_ChooseUnit;
            return;
        }

        if (GC.isMoving || GC.isAttacking || GC.isAIThinking) return;
        selectedUnit_AI.GetComponent<UnitAI>().AI_Thinking();
    }


    private void ResetSelectedAI()
    {
        GC.chooseAiCoolDown = 0.4f;
        selectedUnit_AI = null;
        GC.isMoving = false;
        GC.isAttacking = false;
        GC.isAIThinking = false;
    }
}
