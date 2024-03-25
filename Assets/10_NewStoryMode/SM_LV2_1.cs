using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SM_LV2_1 : MonoBehaviour
{
    public enum StoryStage
    {
        none,
        plot_EncounterEnemy,
        game_KillEnemy,
        plot_Collapse,
        game_KillEnemy_Collapse,
        plot_GameEnd,
    }

    public StoryStage stage;
    public NewDialog dialog_1;
    public NewDialog dialog_1_1;
    public NewDialog dialog_2;

 
    public IEnumerator GoNextStage(StoryManager SM)
    {
        Unit avatar = AIFunctions.FindPlayer();
        TutorialManager TM = SM.TM;
        GameController GC = SM.GC;

        switch (stage)
        {
            case StoryStage.plot_EncounterEnemy:
                FPManager.FP = UnitFunctions.AddFP();
                GC.PauseGame();
                yield return new WaitForSeconds(0.12f);
                SM.StartGamePlot(dialog_1);
                break;

            case StoryStage.game_KillEnemy:
                GC.storyState = GameController._storyState.game;
                GC.stageController.GameStart(true,true);
                StoryFuncitons.StageControl_SetIntTimer(SM, 4);
                GC.SC.SwitchBGM(true);
                yield return new WaitForSeconds(0.12f);
                TM.allPanels[2].gameObject.SetActive(true);
                TM.allPanels[2].Close_Panels();
                TM.allPanels[2].panel_Tutorial[3].gameObject.SetActive(true);
                break;

            case StoryStage.plot_Collapse:
                GC.PauseGame();
                yield return new WaitForSeconds(0.12f);
                SM.StartGamePlot(dialog_1_1);
                break;

            case StoryStage.game_KillEnemy_Collapse:
                GC.storyState = GameController._storyState.game;
                GC.stageController.GameStart(true,true);
                SM.trigger = StoryManager.Check_StoryTrigger.killAllEnemy;
                break;

            case StoryStage.plot_GameEnd:
                GC.PauseGame();
                yield return new WaitForSeconds(0.12f);
                SM.StartGamePlot(dialog_2);
                break;
        }
    }
}
