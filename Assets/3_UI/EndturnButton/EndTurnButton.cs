using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurnButton : MonoBehaviour
{
    public enum CurrentState { warmingOnWait, canFinishTurn, enemyTurnWaiting }
    public CurrentState currentState;
    private Unit unitGoTo;
    public GameObject stage1_waitWarm, stage2_FinishTurn, stage3_enemyTurn;
    public GameObject EndTurnGuide;
    public Animator anim;
    private void Start()
    {
        EndTurnGuide.SetActive(false);
    }

    private void FixedUpdate()
    {
        stage1_waitWarm.SetActive(false);
        stage2_FinishTurn.SetActive(false);
        stage3_enemyTurn.SetActive(false);
        if (currentState == CurrentState.warmingOnWait) stage1_waitWarm.SetActive(true);
        if (currentState == CurrentState.canFinishTurn) stage2_FinishTurn.SetActive(true);
        if (currentState == CurrentState.enemyTurnWaiting) stage3_enemyTurn.SetActive(true);
    }
    public void CheckCurrentStage()
    {
        GameController GC = FindObjectOfType<GameController>();

        if (GC.gameState == GameController._gameState.enemyTurn_ChooseUnit ||
            GC.gameState == GameController._gameState.enemyTurn_ChooseUnit ||
            GC.gameState == GameController._gameState.enemyTurn_Prepare) return;

        unitGoTo = null;
        foreach (Unit unit in GameController.playerList)
        {
            if (unit.unitType != Unit.UnitSummonType.sentry)
            {
                if (!unit.isSelectedInThisTurn)
                {
                    unitGoTo = unit;
                    break;
                }
            }
        }
        if (unitGoTo == null) currentState = CurrentState.canFinishTurn;
        else currentState = CurrentState.warmingOnWait;
    }
    public void ClickOn()
    {

        GameController GC = FindObjectOfType<GameController>();
        if (GC.isAttacking || GC.isMoving || GameController.frozenGame == true) return;
        if (GC.gameState != GameController._gameState.playerTurn) return;
        CheckCurrentStage();
        anim.SetTrigger("trigger");

        if (unitGoTo != null && currentState == CurrentState.warmingOnWait)
        {
            FindNextUnit(unitGoTo);
        }

        else { GC.Button_PlayerTurnEnd(); EndTurnGuide.SetActive(false); }

        CheckCurrentStage();
    }
    private void FindNextUnit(Unit unitToSelect)
    {
        GameController GC = FindObjectOfType<GameController>();
        GC.Select_Unit(unitToSelect.nodeAt, true);
    }
}
