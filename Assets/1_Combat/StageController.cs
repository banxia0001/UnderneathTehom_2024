using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageController : MonoBehaviour
{
    public GameController GC;

    public void GameStart(bool haveStartTurnT, bool dontReadBuffCD)
    {
        GC.storyState = GameController._storyState.game;
        CamMove cm = FindObjectOfType<CamMove>();
        GC.UI.combatPanelUI.gameObject.SetActive(true);
        GC.UI.combatPanelUI.skill_UI.SetActive(false);
        GC.UI.combatPanelUI.SwitchUnitIconList(true);
        GC.Update_MachineGunPosition();
        StartCoroutine(cm.ChangeCamSlowMove(1, false));
        StartCoroutine(PlayerTurnStart(haveStartTurnT, dontReadBuffCD));
    }
    public void GameStart_Enemy()
    {
        if (GC.storyState != GameController._storyState.game) return;
        if (GC.gameState == GameController._gameState.playerTurnPrepare || GC.gameState == GameController._gameState.enemyTurn_ChooseUnit ||
              GC.gameState == GameController._gameState.enemyTurn_Prepare || GC.gameState == GameController._gameState.enemyTurn_Action ||
              GC.gameState == GameController._gameState.gamePause) return;

        GC.storyState = GameController._storyState.game;
        CamMove cm = FindObjectOfType<CamMove>();
        StartCoroutine(cm.ChangeCamSlowMove(1, false));
        StartCoroutine(EnemyTurnStart());
    }


    public IEnumerator PlayerTurnStart(bool haveStartTurnT, bool dontReadBuffCD)
    {
        GC.gameState = GameController._gameState.playerTurnPrepare;
        GC.UI.unitPanelUI.gameObject.SetActive(false);
        GC.UI.combatPanelUI.enemyPanel.SetActive(false);
        GC.UI.combatPanelUI.skill_UI.SetActive(false);
        GC.UI.combatPanelUI.gameObject.SetActive(true);
        GC.AIUnitController.selectedUnit_AI = null;
        GC.UI.combatPanelUI.SwitchUnitIconList(true);
        FindObjectOfType<EndTurnButton>(true).CheckCurrentStage();

        if (haveStartTurnT)
        {
            yield return new WaitForSeconds(0.15f);
            GC.UI.StartText.SetActive(true);
            yield return new WaitForSeconds(0.25f);
        }

        //FPManager.FPChange(1);
        CheckCampfireHeal(0);
        CheckCD(0, dontReadBuffCD);

        foreach (PathNode path in FindObjectsOfType<PathNode>())
        {
            path.CheckHeightChangeListInGame();
        }

        foreach (Unit unit in GameController.enemyList)
        {
            unit.UnitEnable(true);
        }

        yield return new WaitForSeconds(0.25f);
        GC.UI.StartText.SetActive(false);

        if (GC.SM != null) GC.SM.DeceaseCountDownTimer();

        //[Turn Start]
        Unit unitP = AIFunctions.FindPlayer();
        if (unitP == null) unitP =  GameController.playerList[GameController.playerList.Count - 1];
        StartCoroutine(GC.CamMove(unitP.transform));
        GC.gameState = GameController._gameState.playerTurn;
        GC.Update_MachineGunPosition();
    }

    public IEnumerator PlayerTurn_AutoAttack()
    {
        GC.gameState = GameController._gameState.playerTurnPrepare;
        GC.AIUnitController.selectedUnit_AI = null;
        GC.Deselect_Unit();
        FindObjectOfType<EndTurnButton>(true).currentState = EndTurnButton.CurrentState.enemyTurnWaiting;

        yield return new WaitForSeconds(0.1f);
        GC.gameState = GameController._gameState.playerTurnEnd_AutoAttack;
    }


    public IEnumerator EnemyTurnStart()
    {
        FindObjectOfType<EndTurnButton>(true).currentState = EndTurnButton.CurrentState.enemyTurnWaiting;

        if (GC.selectedUnit != null) GC.Deselect_Unit();
        GC.UI.unitPanelUI.gameObject.SetActive(false);
        bool canEnemyMove = true;
        if (GameController.enemyList == null) canEnemyMove = false;
        if (GameController.enemyList.Count == 0) canEnemyMove = false;
       
        if (canEnemyMove)
        {
            GC.gameState = GameController._gameState.enemyTurn_Prepare;
            GC.AIUnitController.selectedUnit_AI = null;
            GC.Deselect_Unit();
            GameController.enemyList = UnitFunctions.SortingEnemyListByX(GameController.enemyList);

            if (GameController.enemyList != null && GameController.enemyList.Count != 0)
            {
                GC.CM.addPos(GameController.enemyList[0].transform.position, true);
            }
            yield return new WaitForSeconds(0.01f);
            GC.CM.addZoom(4f, true);

            //[Campfire Check]
            CheckCampfireHeal(1);
            //[Unit Check]
            CheckCD(1,false);

            foreach (Unit unit in GameController.playerList)
            {
                if(unit != null)
                unit.UnitEnable(true);
            }
            GC.gameState = GameController._gameState.enemyTurn_ChooseUnit;
        }
        else
        {
            GC.gameState = GameController._gameState.playerTurnPrepare;
            yield return new WaitForSeconds(0.2f);
            StartCoroutine(PlayerTurnStart(true, false));
        }
    }




    public void Check_PlayerTurnStart()
    {
        if (EndTurnCheck_IfAllUnitDisable(1) == false)
            StartCoroutine(PlayerTurnStart(true, false));
    }
    public void Check_PlayerTurnEnd()
    {
        //if (EndTurnCheck_IfAllUnitDisable(0) == false)
        //    StartCoroutine(PlayerTurn_AutoAttack());
    }




    public bool EndTurnCheck_IfAllUnitDisable(int teamNum)
    {
        bool still_UnitEnable = false;
        if (teamNum == 0)
            foreach (Unit unit in GameController.playerList)
            {
                if (unit != null)
                    if (unit.unitSpecialState == Unit.UnitSpecialState.normalUnit && unit.isActive)
                        still_UnitEnable = true;
            }

        if (teamNum == 1)
            foreach (Unit unit in GameController.enemyList)
            {
                if (unit != null)
                    if (unit.isActive == true) still_UnitEnable = true;
            }

        return still_UnitEnable;
    }

    private void CheckCampfireHeal(int team)
    {
        //[Campfire Check]
        if (team == 0)
            foreach (CampFire camp in FindObjectsOfType<CampFire>())
            {
                if (camp.campfireTeam == Unit.UnitTeam.playerTeam)
                    camp.CampfireCheck_healNearbyUnit(false);
            }

        if (team == 1)
            foreach (CampFire camp in FindObjectsOfType<CampFire>())
            {
                if (camp.campfireTeam == Unit.UnitTeam.enemyTeam)
                    camp.CampfireCheck_healNearbyUnit(true);
            }
    }

    private void CheckCD(int team, bool dontReadBUFFCD)
    {
        if (team == 0)
        {
            foreach (Unit unit in GameController.playerList)
            {
                if (unit != null)
                    if (!dontReadBUFFCD)
                {
                    unit.Buff_Reflesh();
                    unit.CD_Reflesh();
                }
          
                unit.UnitEnable(true);
            }
        }

        if (team == 1)
        {
            foreach (Unit unit in GameController.enemyList)
            {
                if (unit != null)
                {
                    unit.Buff_Reflesh();
                    unit.CD_Reflesh();
                    unit.UnitEnable(true);
                }
                   
            }
        }
    }
}
