using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SM_LV1_1 : MonoBehaviour
{
    [Header("Debug Tools")]
    public bool debug_Stage_WatchAttack = false;
    public bool debug_Stage_SecondWave = false;
    public enum StoryStage
    {
        none,
        cutscene_wake_up,
        plot_WakeUpInCave,
        tut_MoveCam,
        tut_Select,
        tut_Move,
        tut_Deselect,
        plot_Hungry,

        tut_WatchAttack,
        tut_Attack,
        
        plot_RatAppear,
        game_FightRat_1,

        plot_FeelPower,
        tut_Skill_1,
        game_FightRat_2,

        plot_RatAppear_2,
        tut_Skill_2,
        game_FightRat_3,

        plot_EarthShake,
        tut_Leave,

        game_ReachDoor,
        plot_EndGame,
    }

    public StoryStage stage;


    [Space(10)]
    public List<Vector2Int> Vectors_BlockOnDoor;
    public GameObject staticUIPanel_MeatDoor;
    public NewDialog dialog_Awake;

    [Header("Tut_Cam & Select")]
    public GameObject guideNode_Tut_SelectPlayer;
    public List<Vector2Int> Vector_PlayerFirstMove;
    public List<Vector2Int> Vector_FirstBlock;
    public GameObject guideNode_Tut_MoveToArea;
    
    [Header("Tut_Cam & Deselect")]
    public List<Vector2Int> Vector_PlayerDeselectAroundNodes;
    public GameObject guideNode_Tut_Deselect;

    [Header("Hungry & Eat")]
    public NewDialog dialog_Hungry;
    public GameObject guideNode_Corpses;
    public List<GameObject> Corpses;

    [Header("EnemyAppear_1")]
    public NewDialog dialog_RatRaid;
    public List<GameObject> Rat_1;
    public List<Vector2Int> Vector_Rat1;
    public NewDialog dialog_FeelPower;

    [Header("EnemyAppear_2")]
    public NewDialog dialog_RatRaid_2;
    public List<GameObject> Rat_2;
    public List<Vector2Int> Vector_Rat2;
   
    public GameObject prefab_skillLine;
    public Skill skill_1, skill_2;

    [Header("Falling")]
   
    public List<GameObject> Rat_3;
    public GameObject lightA, lightB;
    public NewDialog dialog_Falling;
    public List<Vector2Int> Vector_Rat3;
    public GameObject guideNode_End;
    private int waitTime = 3;

    [Header("EndGame")]
    public List<Vector2Int> Vector_EndGame;
    public NewDialog dialog_debugEndGame;
    private void DefaultSetting(StoryManager SM)
    {
        StoryFuncitons.SetGrid_IfWalkable(Vectors_BlockOnDoor, false);
        StoryFuncitons.SetGrid_IfWalkable(Vector_FirstBlock, false);
        staticUIPanel_MeatDoor.SetActive(false);
        foreach (GameObject ob in SM.TM.allPanels[0].panel_CheckMark)
        { ob.SetActive(false); }
        SM.GC.can_MoveCam = false;
        FPManager.FP = 0;
    }

    public void SpawnEnemy(int order, GridManager GM)
    {
        List<Vector2Int> Vectors = new List<Vector2Int>();
        List<GameObject> Enemies = new List<GameObject>();
        if (order == 0)
        {
            Enemies = Rat_1;
            Vectors = Vector_Rat1;
        }
        if (order == 1)
        {
            Enemies = Rat_2;
            Vectors = Vector_Rat2;
        }
        if (order == 2)
        {
            Enemies = Rat_3;
            Vectors = Vector_Rat3;
        }
        for (int i2 = 0; i2 < Enemies.Count; i2++)
        {
            for (int i = 0; i < Vectors.Count; i++)
            {
                PathNode path = GM.GetPath(Vectors[i].x, Vectors[i].y);
                if (path != null && path.unit == null && path.isBlocked == false)
                {
                    Debug.Log(Vectors[i].x + " " + Vectors[i].y);
                    StartCoroutine(SpawnEnemy_2(Enemies[i2], path));
                    Vectors.Remove(Vectors[i]);
                    break;
                }
            }
        }
    }

    public IEnumerator SpawnEnemy_2(GameObject unit, PathNode path)
    {
        unit.transform.position = path.transform.position;
        unit.SetActive(true);
        yield return new WaitForSeconds(0.35f);
        GridFallController.AddNodeToDynamicFolder(path,"Down","");
    }
    public IEnumerator GoNextStage(StoryManager SM)
    {
        Unit avatar = AIFunctions.FindPlayer();
        TutorialManager TM = SM.TM;
        GameController GC = SM.GC;

        if (debug_Stage_WatchAttack)
        {
            debug_Stage_WatchAttack = false;
            DefaultSetting(SM);
            stage = StoryStage.tut_WatchAttack;
            SM.GC.can_MoveCam = true;
        }
        if (debug_Stage_SecondWave)
        {
            //waitTime = 1;
            debug_Stage_SecondWave = false;
            DefaultSetting(SM);
            stage = StoryStage.plot_RatAppear_2;
            SM.GC.can_MoveCam = true;
            avatar.currentData.Skill.Add(new _Skill(skill_1, 0)); avatar.currentData.Skill[0].CD = 0;
            avatar.currentData.armorNow = 10;
            avatar.currentData.damage.damBonus = 10;
            avatar.currentData.healthNow = 20;

            foreach (GameObject corpse in Corpses)
            {
                corpse.GetComponent<Unit>().HealthChange(-100, 0, "damage");
            }
        }

        switch (stage)
        {
            case StoryStage.cutscene_wake_up:
                SM.Event_PlayerAwake();
                FPManager.FP = UnitFunctions.AddFP();
                StoryFuncitons.StageControl_SetTimer(SM, 6f);
                yield return new WaitForSeconds(0.12f);
                FPManager.FP = UnitFunctions.AddFP();
                DefaultSetting(SM);
                break;

            case StoryStage.plot_WakeUpInCave:
                SM.StartGamePlot(dialog_Awake);
                break;

            case StoryStage.tut_MoveCam:
                GC.storyState = GameController._storyState.tutorial;
                GC.can_MoveCam = true;

                yield return new WaitForSeconds(0.12f);
                TM.allPanels[0].gameObject.SetActive(true);
                TM.allPanels[0].panel_Tutorial[0].SetActive(true);
                TM.allPanels[0].GetComponent<Animator>().SetTrigger("trigger");

                yield return new WaitForSeconds(0.12f);
                SM.trigger = StoryManager.Check_StoryTrigger.tutorial_MoveCam;
                break;

            case StoryStage.tut_Select:
                GC.storyState = GameController._storyState.game;
                GC.stageController.GameStart(false,true);

                yield return new WaitForSeconds(0.12f);
                TM.allPanels[0].panel_CheckMark[0].SetActive(true);
                TM.allPanels[0].panel_Tutorial[1].SetActive(true);
                SM.trigger = StoryManager.Check_StoryTrigger.tutorial_Select;
                guideNode_Tut_SelectPlayer.SetActive(true);
                break;

            case StoryStage.tut_Move:
                yield return new WaitForSeconds(0.15f);
                GC.storyState = GameController._storyState.game;
                TM.allPanels[0].panel_Tutorial[0].SetActive(false);//cam
                TM.allPanels[0].panel_CheckMark[1].SetActive(true);//select
                TM.allPanels[0].panel_Tutorial[2].SetActive(true);//move

                StoryFuncitons.StageControl_MoveToTarget(SM,Vector_PlayerFirstMove);
                guideNode_Tut_SelectPlayer.SetActive(false);
                guideNode_Tut_MoveToArea.SetActive(true);
                break;


            case StoryStage.tut_Deselect:
                yield return new WaitForSeconds(0.2f);
                StoryFuncitons.SetGrid_IfWalkable(Vector_PlayerDeselectAroundNodes, false);
                GC.storyState = GameController._storyState.game;

                GC.ReselectUnit();

                TM.allPanels[0].panel_Tutorial[1].SetActive(false);//cam
                TM.allPanels[0].panel_CheckMark[2].SetActive(true);//select
                TM.allPanels[0].panel_Tutorial[3].SetActive(true);//move
                                                                  // TM.allPanels[0].GetComponent<Animator>().SetTrigger("trigger")
                SM.trigger = StoryManager.Check_StoryTrigger.tutorial_Deselect;
                guideNode_Tut_MoveToArea.SetActive(false);
                guideNode_Tut_Deselect.SetActive(true);
                break;


            case StoryStage.plot_Hungry:
                StoryFuncitons.SetGrid_IfWalkable(Vector_PlayerDeselectAroundNodes, true);
                StoryFuncitons.SetGrid_IfWalkable(Vector_FirstBlock, true);

                TM.allPanels[0].panel_CheckMark[3].SetActive(true);//select
                guideNode_Tut_Deselect.SetActive(false);

                yield return new WaitForSeconds(0.25f);
                SM.StartGamePlot(dialog_Hungry);
                TM.Close_Panels();
                guideNode_Corpses.SetActive(true);
                break;

          
            case StoryStage.tut_WatchAttack:
                GC.storyState = GameController._storyState.tutorial;
                TM.allPanels_v2[0].gameObject.SetActive(true);
                break;

            case StoryStage.tut_Attack:
                SM.trigger = StoryManager.Check_StoryTrigger.collectFP;
                SM.Objects_TriggerStageCheck = Corpses;
                guideNode_Corpses.SetActive(false);
                TM.allPanels[1].gameObject.SetActive(true);//select
                TM.allPanels[1].panel_Tutorial[0].SetActive(true);
                break;

            case StoryStage.plot_RatAppear:
                GC.PauseGame();
                yield return new WaitForSeconds(1f);
                SM.StartGamePlot(dialog_RatRaid);
                TM.allPanels[1].gameObject.SetActive(false);

                yield return new WaitForSeconds(0.5f);
                GC.SC.SwitchBGM(true);
                break;


            case StoryStage.game_FightRat_1:
                SM.AddGuide_EndTurn(avatar);
                GC.stageController.GameStart(true,true);
                SM.trigger = StoryManager.Check_StoryTrigger.timeCountdown;
                SM.CountDownTimer = 2;
                break;

            case StoryStage.plot_FeelPower:
                GC.PauseGame();
                GC.CM.addPos(avatar.transform.position + new Vector3(0, -0.275f, 0), true);
                GC.CM.addZoom(3.5f, true);

                //Animation-GainBuff  //Animation-GainBuff  //Animation-GainBuff
                avatar.InputAnimation("gainSkill");

                yield return new WaitForSeconds(0.5f);
                Instantiate(prefab_skillLine, avatar.transform.position, Quaternion.identity);
                avatar.currentData.Skill.Add(new _Skill(skill_1, 0)); avatar.currentData.Skill[0].CD = 0;

                yield return new WaitForSeconds(1f);
                GC.UI.learnSkillPanel.gameObject.SetActive(true);
                GC.UI.learnSkillPanel.Input(skill_1.skillSprite);

                yield return new WaitForSeconds(3f);
                SM.StartGamePlot(dialog_FeelPower);
                break;

            case StoryStage.tut_Skill_1:
                GC.storyState = GameController._storyState.tutorial;
                GC.gameState = GameController._gameState.gamePause;


                yield return new WaitForSeconds(0.3f);
                TM.allPanels_v2[1].gameObject.SetActive(true);
                break;

            case StoryStage.game_FightRat_2:
                CombatPanelUI combat = FindObjectOfType<CombatPanelUI>(true);
                combat.guide_1.SetActive(true);
                TM.Close_Panels();
                TM.allPanels[2].gameObject.SetActive(true);
                TM.allPanels[2].panel_Tutorial[0].SetActive(true);
                SM.trigger = StoryManager.Check_StoryTrigger.killAllEnemy;
                SM.CountDownTimer = 3;
                break;

            case StoryStage.plot_RatAppear_2://rat2 appear
                GC.PauseGame();
                TM.Close_Panels();
                yield return new WaitForSeconds(0.5f);
                SM.StartGamePlot(dialog_RatRaid_2);
                break;

            case StoryStage.tut_Skill_2://talking about taunt tower
                lightA.SetActive(false);
                GC.storyState = GameController._storyState.tutorial;
                GC.gameState = GameController._gameState.gamePause;

                yield return new WaitForSeconds(0.15f);
                CombatPanelUI combat2 = FindObjectOfType<CombatPanelUI>(true);
                combat2.guide_2.SetActive(true);
                TM.Close_Panels();

                yield return new WaitForSeconds(0.1f);
                GC.UI.learnSkillPanel.gameObject.SetActive(true);
                GC.UI.learnSkillPanel.Input(skill_2.skillSprite);

     

                avatar.InputAnimation("gainSkill");
                yield return new WaitForSeconds(0.5f);
                Instantiate(prefab_skillLine, avatar.transform.position, Quaternion.identity);
                avatar.currentData.Skill.Add(new _Skill(skill_2, 0)); avatar.currentData.Skill[1].CD = 0;

                yield return new WaitForSeconds(1.2f);
                TM.allPanels_v2[2].gameObject.SetActive(true);
                break;

            case StoryStage.game_FightRat_3:
                TM.Close_Panels();
                TM.allPanels[2].gameObject.SetActive(true);
                TM.allPanels[2].panel_Tutorial[0].SetActive(true);
                TM.allPanels[2].panel_Tutorial[1].SetActive(true);
                SM.trigger = StoryManager.Check_StoryTrigger.timeCountdown;
                SM.CountDownTimer = waitTime;
                break;

            case StoryStage.plot_EarthShake:
                lightB.SetActive(true);
                GC.PauseGame();
                TM.Close_Panels();
                guideNode_End.SetActive(true);
                StoryFuncitons.SetGrid_IfWalkable(Vectors_BlockOnDoor, true);
                staticUIPanel_MeatDoor.SetActive(true);
                SM.StartGamePlot(dialog_Falling);
                break;


            case StoryStage.tut_Leave:
                GC.storyState = GameController._storyState.tutorial;
                GC.gameState = GameController._gameState.gamePause;
                yield return new WaitForSeconds(0.3f);
                TM.allPanels_v2[3].gameObject.SetActive(true);
                break;


            case StoryStage.game_ReachDoor:
                
                TM.Close_Panels();
                TM.allPanels[2].gameObject.SetActive(true);
                TM.allPanels[2].panel_Tutorial[2].SetActive(true);
                GC.stageController.GameStart(true,true);
                StoryFuncitons.StageControl_MoveToTarget(SM, Vector_EndGame);
                break;

            case StoryStage.plot_EndGame:
                GC.PauseGame();
                yield return new WaitForSeconds(0.3f);
                GC.CM.addPos(avatar.transform.position + new Vector3(0, -0.275f, 0), true);
                GC.CM.addZoom(3.5f, true);
                guideNode_End.SetActive(false);

                yield return new WaitForSeconds(0.3f);
                SM.StartGamePlot(dialog_debugEndGame);
                break;
        }
    }
}
