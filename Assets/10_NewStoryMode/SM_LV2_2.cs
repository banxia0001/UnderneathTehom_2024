using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SM_LV2_2 : MonoBehaviour
{
    public enum StoryStage
    {
        none,
        plot_PlayerEnter,
        game_EnterBattle,

        plot_BossEnter,
        game_KillBoss,

        plot_Collapse,
        game_FinalCombat,
        plot_GameEnd,
    }

    public Unit boss;
    public StoryStage stage;
    public List<Vector2Int> Vectors_TriggerBoss;
    public NewDialog dialog_0;
    public NewDialog dialog_1;
    public NewDialog dialog_2;
    public NewDialog dialog_3;
    public IEnumerator GoNextStage(StoryManager SM)
    {
        Unit avatar = AIFunctions.FindPlayer();
        TutorialManager TM = SM.TM;
        GameController GC = SM.GC;

        switch (stage)
        {
            case StoryStage.plot_PlayerEnter:
                FPManager.FP = UnitFunctions.AddFP();
                GC.PauseGame();
                yield return new WaitForSeconds(0.12f);
                SM.StartGamePlot(dialog_0);
                break;

            case StoryStage.game_EnterBattle:
               
                yield return new WaitForSeconds(0.12f);
                GC.storyState = GameController._storyState.game;
                GC.stageController.GameStart(true,true);
                StoryFuncitons.StageControl_MoveToTarget(SM, Vectors_TriggerBoss);
                break;

            case StoryStage.plot_BossEnter:
                GC.PauseGame();
                yield return new WaitForSeconds(0.12f);
                SM.StartGamePlot(dialog_1);
                break;

            case StoryStage.game_KillBoss:

                FindObjectOfType<BossHealthbar>(true).gameObject.SetActive(true);
                yield return new WaitForSeconds(0.12f);
                GC.storyState = GameController._storyState.game;
                GC.stageController.GameStart(true,true);
                StoryFuncitons.StageControl_SetIntTimer(SM, 2);
                break;

            case StoryStage.plot_Collapse:
                GC.PauseGame();
                yield return new WaitForSeconds(0.12f);
                SM.StartGamePlot(dialog_2);
                break;

            case StoryStage.game_FinalCombat:
                GC.storyState = GameController._storyState.game;
                GC.stageController.GameStart(true,true);
             
                //SM.bossUnit = FindObjectOfType<BossAI>().gameObject.GetComponent<Unit>();
                SM.trigger = StoryManager.Check_StoryTrigger.none;
                break;

            case StoryStage.plot_GameEnd:
                GC.PauseGame();
                yield return new WaitForSeconds(0.12f);
                SM.StartGamePlot(dialog_3);
                break;
        }
    }
}
