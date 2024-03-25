using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoryManager : MonoBehaviour
{
    [Header("Game")]
    public List<Transform> story_lookatPoint;
    public List<GameObject> lightFolder;
    [HideInInspector] public List<GameObject> guideNode;

    [HideInInspector] public TutorialManager TM;
    [HideInInspector] public GameController GC;
    [HideInInspector] public NewDialogSystem DC;

    public enum Story_Level { level_1, level_2, level1_1, level2_1, level2_2 }

    public Unit bossUnit;
    public enum Check_StoryTrigger
    {
        none,
        moveToArea,
        moveToArea_Team,
        killAllEnemy,
        destroyTargetUnits,
        collectFP,

        timeCountdown,
        time_Float_Countdown,

        playerTeamNumber,

        tutorial_MoveCam,
        tutorial_Move,
        tutorial_Select,
        tutorial_Attack,
        tutorial_SelectSkill,

        tutorial_Cast,
        tutorial_Deselect,
        tutorial_OpenUnitPanel,
        tutorial_CloseUnitPanel,
        tutorial_ClickSkill,
        tutorial_ClickCorpse,

        checkIfBossAlive,
    }

    [HideInInspector] public int CountDownTimer;
    [HideInInspector] public int SizeNumber;
    [HideInInspector] public float CountDownTimer_Float;
    public Story_Level level;
    public Check_StoryTrigger trigger;

    [HideInInspector] public List<Vector2Int> Vectors_TriggerAreaCheck;
    [HideInInspector] public List<GameObject> Objects_TriggerStageCheck;
    [HideInInspector] public bool storyTrigger_ActiveWhenGameCanPause = false;

    private Coroutine cor;

    [Header("StoryData")]
    public SM_LV1_1 lv1;
    public SM_LV2_1 lv2;
    public SM_LV2_2 lv3;

    [Space(50)]
    [HideInInspector] public Story_Lv1 story_Lv1;
    [HideInInspector] public Story_Lv2 story_Lv2;



    public void DeceaseCountDownTimer()
    {
        CountDownTimer--;
        FindObjectOfType<GridFallController>().DecreaseTimer();
    }

    public void Awake()
    {
        checkEndTurn = false;
        GC = FindObjectOfType<GameController>(true);
        DC = FindObjectOfType<NewDialogSystem>(true);
        TM = FindObjectOfType<TutorialManager>(true);
        storyTrigger_ActiveWhenGameCanPause = false;

        foreach (GameObject ob in lightFolder)
        {
            ob.SetActive(false);
        }

        foreach (GameObject ob in guideNode)
        {
            ob.SetActive(false);
        }
    }
    public void GoNextStage()
    {
        switch (level)
        {
            case Story_Level.level1_1:
                if (cor != null) StopCoroutine(cor);
                lv1.stage++;
                cor = StartCoroutine(lv1.GoNextStage(this));
                break;

            case Story_Level.level2_1:
                if (cor != null) StopCoroutine(cor);
                lv2.stage++;
                cor = StartCoroutine(lv2.GoNextStage(this));
                break;

            case Story_Level.level2_2:
                if (cor != null) StopCoroutine(cor);
                lv3.stage++;
                cor = StartCoroutine(lv3.GoNextStage(this));
                break;
        }
    }

    public void StartGamePlot(NewDialog dialog)
    {
        GC.can_MoveCam = false;
        CamMove MOVE = FindObjectOfType<CamMove>();
        MOVE.addZoom(3.9f,true);
        GC.PauseGame();

        if (GC.debug_SkipPlot)
        {
            GoNextStage();
            return;
        }
        GC.storyState = GameController._storyState.story;

        DC.anim.SetBool("Open", true);
        DC.InputDialog(dialog);
        DC.CamUpdate(AIFunctions.FindPlayer().gameObject.transform.position);
        GC.UI.combatPanelUI.gameObject.SetActive(false);
    }


    

    private bool checkEndTurn = false;
    private Unit avatar;
    public void AddGuide_EndTurn(Unit avatar)
    {
        this.avatar = avatar;
        checkEndTurn = true;
      //  Debug.Log("@1");
    }

    public void FixedUpdate()
    {
        if (checkEndTurn)
        {
           // Debug.Log("@2");
            if (avatar == null || avatar.isActive == false)
            {
               // Debug.Log("@3");
                checkEndTurn = false;
                FindObjectOfType<EndTurnButton>().EndTurnGuide.SetActive(true);
            }
        }

        if (GameController.frozenGame == true) return;

        if (storyTrigger_ActiveWhenGameCanPause)
        {
            trigger = Check_StoryTrigger.none;
            if (GC.Check_CanGamePause())
            {
                storyTrigger_ActiveWhenGameCanPause = false;
                GoNextStage();
            }
        }

        else
        {
            switch (trigger)
            {
                case Check_StoryTrigger.none:
                    break;

                case Check_StoryTrigger.collectFP:
                    if(FPManager.FP >= 4)
                        storyTrigger_ActiveWhenGameCanPause = true;
                    break;

                case Check_StoryTrigger.destroyTargetUnits:
                    if(CheckAllUnitIsDestroyed(Objects_TriggerStageCheck)) 
                        storyTrigger_ActiveWhenGameCanPause = true;
                    break;
                   
                case Check_StoryTrigger.moveToArea:
                    if (CheckUnitOnTargetGrid(Vectors_TriggerAreaCheck) > 0)
                        storyTrigger_ActiveWhenGameCanPause = true;
                    break;

                case Check_StoryTrigger.moveToArea_Team:
                    if (CheckUnitOnTargetGrid(Vectors_TriggerAreaCheck) >= SizeNumber)
                        storyTrigger_ActiveWhenGameCanPause = true;
                    break;

                case Check_StoryTrigger.killAllEnemy:
                    if (GameController.enemyList == null)
                        storyTrigger_ActiveWhenGameCanPause = true;

                    else if (GameController.enemyList.Count == 0)
                        storyTrigger_ActiveWhenGameCanPause = true;
                    break;


                case Check_StoryTrigger.timeCountdown:
                    if (CountDownTimer <= 0)
                        storyTrigger_ActiveWhenGameCanPause = true;
                    break;

                case Check_StoryTrigger.time_Float_Countdown:
                    CountDownTimer_Float -= Time.fixedDeltaTime;
                    if (CountDownTimer_Float <= 0)
                        storyTrigger_ActiveWhenGameCanPause = true;
                    break;

                case Check_StoryTrigger.tutorial_Move:
                    if (GC.isMoving == true)
                        storyTrigger_ActiveWhenGameCanPause = true;
                    break;

                case Check_StoryTrigger.tutorial_Attack:
                    if (GC.isAttacking == true && GC.gameState == GameController._gameState.playerTurn)
                        storyTrigger_ActiveWhenGameCanPause = true;
                    break;

                case Check_StoryTrigger.tutorial_Select:
                    if (GC.selectedUnit != null && GC.gameState == GameController._gameState.playerTurn)
                        storyTrigger_ActiveWhenGameCanPause = true;
                    break;

                case Check_StoryTrigger.tutorial_SelectSkill:
                    if (GC.gameState == GameController._gameState.playerInSpell)
                        storyTrigger_ActiveWhenGameCanPause = true;
                    break;

                case Check_StoryTrigger.tutorial_Cast:
                    if (GC.isAttacking == true && GC.gameState == GameController._gameState.playerTurn)
                        storyTrigger_ActiveWhenGameCanPause = true;
                    break;

                case Check_StoryTrigger.tutorial_MoveCam:
                    if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) ||
                        Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) ||
                        Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) ||
                        Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) ||
                        Input.GetAxis("Mouse ScrollWheel") != 0)
                        storyTrigger_ActiveWhenGameCanPause = true;
                    break;

                case Check_StoryTrigger.tutorial_Deselect:
                    if (GC.selectedUnit == null)
                        storyTrigger_ActiveWhenGameCanPause = true;
                    break;

                case Check_StoryTrigger.tutorial_OpenUnitPanel:
                    if (GC.gameState == GameController._gameState.playerInUnitPanel)
                        storyTrigger_ActiveWhenGameCanPause = true;
                    break;

                case Check_StoryTrigger.tutorial_CloseUnitPanel:
                    if (GC.gameState != GameController._gameState.playerInUnitPanel)
                        storyTrigger_ActiveWhenGameCanPause = true;
                    break;

                case Check_StoryTrigger.tutorial_ClickSkill:
                    if (GC.gameState == GameController._gameState.playerInSpell)
                        storyTrigger_ActiveWhenGameCanPause = true;

                    break;

                case Check_StoryTrigger.tutorial_ClickCorpse:
                    if (GC.gameState == GameController._gameState.playerInRevivalPanel)
                        storyTrigger_ActiveWhenGameCanPause = true;

                    break;


                case Check_StoryTrigger.playerTeamNumber:
                    if (GameController.playerList != null)
                        if (GameController.playerList.Count >= SizeNumber)
                            storyTrigger_ActiveWhenGameCanPause = true;

                    break;

                case Check_StoryTrigger.checkIfBossAlive:
                    if (bossUnit == null)
                    {
                        storyTrigger_ActiveWhenGameCanPause = true;
                        KillAllEnemy();

                    }
                      

                    break;
            }
        }
    }


    public void KillAllEnemy()
    { 
        foreach(Unit unit in FindObjectsOfType<Unit>())
        {
            if (unit.unitTeam == Unit.UnitTeam.enemyTeam)
            {
                unit.HealthChange(-1000, 0, "damage");
            }
        }
    }
    private int CheckUnitOnTargetGrid(List<Vector2Int> myTrigger)
    {
        int num = 0;

        foreach (Vector2Int xy in myTrigger)
        {
            if (GC.GM.pathArray[xy.x, xy.y] != null)
                if (GC.GM.pathArray[xy.x, xy.y].unit != null)
                    if (GC.GM.pathArray[xy.x, xy.y].unit.unitTeam == Unit.UnitTeam.playerTeam) num++;
        }

        return num;
    }

    private bool CheckAllUnitIsDestroyed(List<GameObject> objects)
    {
        bool allDestroyed = true;
        foreach (GameObject ob in objects)
        {
            if (ob != null) allDestroyed = false;
        }
        return allDestroyed;
    }

    private void SwitchGrid(List<Vector2Int> myTrigger, bool canWalk)
    {
        foreach (Vector2Int xy in myTrigger)
        {
            if (GC.GM.pathArray[xy.x, xy.y] != null)
            {
                //Debug.Log(GC.GM.pathArray[xy.x, xy.y].gameObject.name);
                if (canWalk) GC.GM.pathArray[xy.x, xy.y].isBlocked = false;
                else GC.GM.pathArray[xy.x, xy.y].isBlocked = true;
            }
        }
    }


    public void GridsFall(int order)
    {
        if (order == 0) { GC.SC.StartRockFall_Initial(); }
    }

    public void Event_CaveShake(int i)
    {
        GC.SC.StartRockFall();
        GC.cameraAnim.SetTrigger("Shake3");
    }
    public void Event_SpawnEnemy(int num)
    {
        if (level == Story_Level.level1_1)
        {
            lv1.SpawnEnemy(num, GC.GM);
        }
    }
    public void Event_PlayerAwake()
    {
        Unit unit = AIFunctions.FindPlayer();
        unit.InputAnimation_Single("getupnew");
        unit.AddAnimation("idle");
    }

    public void Event_UndeadUnitCheer()
    {
        foreach (Unit unit in GameController.playerList)
        {
            unit.InputAnimation("gainBuff");
        }
    }

    public void Event_GetDown_n_Eat()
    {
        Unit player = AIFunctions.FindPlayer();
        player.TranformOffsetFolder.transform.localScale = new Vector3(-1, 1, 1);
        player.InputAnimation_Single("squat");
        player.AddAnimation("eat1");
        player.AddAnimation("eat2");

        GC.SC.EatSound_Loop.gameObject.SetActive(true);
    }

    public void Event_Eating()
    {
        Unit player = AIFunctions.FindPlayer();

        if (story_Lv1 != null)
            if (story_Lv1.stage_1 == Story_Lv1.StoryStage_LV1.plot_EatCorpse_n_Collapse) player.TranformOffsetFolder.transform.localScale = new Vector3(-1, 1, 1);

        player.InputAnimation_Single("eat3");
        player.AddAnimation("eat2");
        player.HealthChange(2, 0, "heal");
        player.FleshChange(10);

        GC.SC.EatSound_Loop.gameObject.SetActive(true);
    }

    public void Event_FinishEating()
    {
        Debug.Log("standup");
        Unit player = AIFunctions.FindPlayer();
        player.InputAnimation("standup");

        GC.SC.EatSound_Loop.gameObject.SetActive(false);
    }

    public void Event_GetDown()
    {
        Unit player = AIFunctions.FindPlayer();
        player.InputAnimation_Single("squat2");
        player.AddAnimation("eat");
        player.AddAnimation("eat2");
    }
    public void Event_Full()
    {
        Unit player = AIFunctions.FindPlayer();
        player.InputAnimation_Single("eat4");
        player.AddAnimation("detect");
        player.AddAnimation("thinking");

        GC.SC.EatSound_Loop.gameObject.SetActive(false);
    }
    public void Event_Thinking()
    {
        Unit player = AIFunctions.FindPlayer();
        player.AddAnimation("thinking");
    }


    public void Event_ThinkingOfSolution()
    {
        Debug.Log("!!!");
        Unit player = AIFunctions.FindPlayer();
        player.InputAnimation("idea");
    }

    public void Event_RatCall()
    {
        foreach (Unit unit in GameController.enemyList)
        {
            unit.InputAnimation("gainBuff");
        }
    }

    public void Event_AllRecover()
    {
        foreach (Unit unit in GameController.playerList)
        {
            int heal = unit.currentData.healthMax - unit.currentData.healthNow;
            unit.HealthChange(heal, 0, "Heal");
        }
    }

    #region LV1 STAGE
    //private IEnumerator GoNextStage_Level_1()
    //{
    //    Unit avatar = AIFunctions.FindPlayer();


    //    if (story_Lv1.debugStage == Story_Lv1.DebugStage.attackDoor)
    //    {
    //        #region LV1_Setting_attackDoor
    //        story_Lv1.debugStage = Story_Lv1.DebugStage.normal;

    //        yield return new WaitForSeconds(0.1f);
    //        //Lv1_DefaultSetting();

    //        story_Lv1.stage_1 = Story_Lv1.StoryStage_LV1.tut_Attack;
    //        GC.stageController.GameStart(true);
    //        story_Lv1.door.currentData.healthNow = 1;

    //        SwitchGrid(story_Lv1.setWalkableGrids_Area0_ToDoor, true);
    //        TM.allPanels[1].gameObject.SetActive(true);
    //        guideNode[4].SetActive(true);
    //        trigger = Check_StoryTrigger.tutorial_Attack;

    //        avatar.HealthChange(10, 0, "heal");
    //        avatar.currentData.movePointMax = 3;
    //        avatar.currentData.movePointNow = 3;
    //        avatar.currentData.damage.damBonus = 100;
    //        avatar.data.movePointMax = 3;
    //        avatar.data.movePointNow = 3;
    //        avatar.data.damage.damBonus = 100;
    //        SaveData.flesh = 50;
    //        #endregion
    //    }

    //    else if (story_Lv1.debugStage == Story_Lv1.DebugStage.attackRat)
    //    {
    //        #region LV1_Setting_attackRat
    //        story_Lv1.debugStage = Story_Lv1.DebugStage.normal;

    //        yield return new WaitForSeconds(0.1f);
    //       //Lv1_DefaultSetting();

    //        story_Lv1.stage_1 = Story_Lv1.StoryStage_LV1.game_MoveIn_Area1_2;
    //        lightFolder[0].SetActive(true);
    //        GC.storyState = GameController._storyState.game;
    //        GC.stageController.GameStart(false);
    //        trigger = Check_StoryTrigger.moveToArea;
    //        Vectors_TriggerAreaCheck = story_Lv1.Vectors_Trigger_EnterArea_1_Combat;

    //        avatar.HealthChange(10, 0, "heal");
    //        avatar.currentData.movePointMax = 3;
    //        avatar.currentData.movePointNow = 3;
    //        avatar.currentData.damage.damBonus = 100;
    //        avatar.data.movePointMax = 3;
    //        avatar.data.movePointNow = 3;
    //        avatar.data.damage.damBonus = 100;
    //        SaveData.flesh = 50;
    //        #endregion
    //    }

    //    else switch (story_Lv1.stage_1)
    //        {

    //            #region LV1_PART_0
    //            //case Story_Lv1.StoryStage_LV1.cutscene_walkup:

    //            //    Event_PlayerAwake();
    //            //    CountDownTimer_Float = story_Lv1.wakeup_timer;
    //            //    trigger = Check_StoryTrigger.time_Float_Countdown;

    //            //    yield return new WaitForSeconds(0.1f);
    //            //    //Lv1_DefaultSetting();
    //            //    break;

    //            //case Story_Lv1.StoryStage_LV1.plot_WakeUpInCave:
    //            //    StartGamePlot(story_Lv1.dialog_Awake);
    //            //    break;

    //            //case Story_Lv1.StoryStage_LV1.tut_moveCam:
    //            //    GC.storyState = GameController._storyState.tutorial;

    //            //    GC.can_MoveCam = true;
    //            //    TM.allPanels[0].gameObject.SetActive(true);
    //            //    TM.allPanels[0].panel_Tutorial[0].SetActive(true);
    //            //    TM.allPanels[0].GetComponent<Animator>().SetTrigger("trigger");

    //            //    yield return new WaitForSeconds(0.12f);

    //            //    trigger = Check_StoryTrigger.tutorial_MoveCam;

    //            //    break;


    //            //case Story_Lv1.StoryStage_LV1.tut_Select:
    //            //    GC.storyState = GameController._storyState.game;

    //            //    GC.stageController.GameStart(false);

    //            //    yield return new WaitForSeconds(0.12f);

    //            //    TM.allPanels[0].panel_CheckMark[0].SetActive(true);
    //            //    TM.allPanels[0].panel_Tutorial[1].SetActive(true);

    //            //    trigger = Check_StoryTrigger.tutorial_Select;
    //            //    guideNode[2].SetActive(true);
    //            //    break;


    //            //case Story_Lv1.StoryStage_LV1.tut_Move:
    //            //    yield return new WaitForSeconds(0.15f);

    //            //    GC.storyState = GameController._storyState.game;
    //            //    GC.can_MoveCam = true;

    //            //    TM.allPanels[0].panel_Tutorial[0].SetActive(false);//cam
    //            //    TM.allPanels[0].panel_CheckMark[1].SetActive(true);//select
    //            //    TM.allPanels[0].panel_Tutorial[2].SetActive(true);//move
    //            //                                                      // TM.allPanels[0].GetComponent<Animator>().SetTrigger("trigger");

    //            //    trigger = Check_StoryTrigger.moveToArea;
    //            //    Vectors_TriggerAreaCheck = story_Lv1.Vectors_Trigger_FirstMove;
    //            //    guideNode[2].SetActive(false);
    //            //    guideNode[3].SetActive(true);
    //            //    break;


    //            //case Story_Lv1.StoryStage_LV1.tut_Deselect:
    //            //    yield return new WaitForSeconds(0.2f);

    //            //    SwitchGrid(story_Lv1.Vectors_BlockToCorpse, false);

    //            //    GC.storyState = GameController._storyState.game;

    //            //    GC.ReselectUnit();

    //            //    TM.allPanels[0].panel_Tutorial[1].SetActive(false);//cam
    //            //    TM.allPanels[0].panel_CheckMark[2].SetActive(true);//select
    //            //    TM.allPanels[0].panel_Tutorial[3].SetActive(true);//move
    //            //                                                      // TM.allPanels[0].GetComponent<Animator>().SetTrigger("trigger");

    //            //    trigger = Check_StoryTrigger.tutorial_Deselect;
    //            //    guideNode[3].SetActive(false);
    //            //    guideNode[5].SetActive(true);
    //            //    break;


    //            //case Story_Lv1.StoryStage_LV1.plot_Hungry:

    //            //    SwitchGrid(story_Lv1.Vectors_BlockToCorpse, true);
    //            //    guideNode[5].SetActive(false);
    //            //    TM.allPanels[0].panel_CheckMark[3].SetActive(true);//select

    //            //    yield return new WaitForSeconds(0.3f);

    //            //    GC.storyState = GameController._storyState.story;

    //            //    StartGamePlot(story_Lv1.dialog_GuideToCorpse);
    //            //    TM.Close_Panels();

    //            //    guideNode[0].SetActive(true);
    //            //    break;


    //            case Story_Lv1.StoryStage_LV1.game_MoveToBody:
    //                yield return new WaitForSeconds(0.3f);

    //                GC.storyState = GameController._storyState.game;
    //                GC.stageController.GameStart(true);

    //                trigger = Check_StoryTrigger.moveToArea;
    //                Vectors_TriggerAreaCheck = story_Lv1.Vectors_Trigger_StartEatCorpse;
    //                break;


    //            case Story_Lv1.StoryStage_LV1.plot_EatCorpse_n_Collapse:
    //                yield return new WaitForSeconds(0.12f);

    //                avatar.TranformOffsetFolder.transform.localScale = new Vector3(-1, 1, 1);

    //                GC.storyState = GameController._storyState.story;
    //                StartGamePlot(story_Lv1.dialog_EatCorpse_n_Collapse);

    //                TM.Close_Panels();

    //                guideNode[0].SetActive(false);
    //                break;

    //            case Story_Lv1.StoryStage_LV1.cutscene_RockFall_1:

    //                GC.SC.SwitchBGM(true);
    //                GC.can_MoveCam = false;
    //                GC.storyState = GameController._storyState.tutorial;

    //                yield return new WaitForSeconds(0.3f);
    //                GC.can_MoveCam = false;
    //                GridsFall(0);

    //                yield return new WaitForSeconds(0.5f);

    //                Unit player = AIFunctions.FindPlayer();
    //                player.data.movePointMax = 3;


    //                GC.can_MoveCam = false;
    //                GoNextStage();
    //                break;




    //            case Story_Lv1.StoryStage_LV1.plot_Run:

    //                story_Lv1.StaticUnit_UIPanels[0].SetActive(true);

                   
    //                GC.storyState = GameController._storyState.story;
    //                StartGamePlot(story_Lv1.dialog_RunAway);
    //                break;


    //            case Story_Lv1.StoryStage_LV1.game_RockFall_2:
                   
    //                yield return new WaitForSeconds(0.2f);


    //                GC.storyState = GameController._storyState.game;
    //                GC.stageController.GameStart(false);

    //                trigger = Check_StoryTrigger.moveToArea;
    //                Vectors_TriggerAreaCheck = story_Lv1.Vectors_Trigger_Rockfall2;

    //                guideNode[1].SetActive(true);
    //                break;


    //            case Story_Lv1.StoryStage_LV1.game_RockFall_3:
                   
    //                SwitchGrid(story_Lv1.setWalkableGrids_Area0_ToDoor, true);

    //                //rock fall in 1
    //                GridsFall(1);
    //                GC.storyState = GameController._storyState.game;
    //                trigger = Check_StoryTrigger.moveToArea;
    //                Vectors_TriggerAreaCheck = story_Lv1.Vectors_Trigger_Rockfall3;

    //                break;

    //            case Story_Lv1.StoryStage_LV1.game_RockFall_4:
                   
    //                //rock fall in 2
    //                GridsFall(2);
    //                GC.storyState = GameController._storyState.game;
    //                trigger = Check_StoryTrigger.moveToArea;
    //                Vectors_TriggerAreaCheck = story_Lv1.Vectors_Trigger_Rockfall4;

    //                break;




    //            //Area1    //Area1    //Area1    //Area1    //Area1
    //            case Story_Lv1.StoryStage_LV1.tut_Attack:

    //                guideNode[1].SetActive(true);
    //                story_Lv1.door.currentData.healthNow = 1;
    //                GridsFall(3);

    //                yield return new WaitForSeconds(0.75f);
    //                GC.storyState = GameController._storyState.tutorial;

    //                TM.allPanels[1].gameObject.SetActive(true);
    //                guideNode[4].SetActive(true);

    //                trigger = Check_StoryTrigger.tutorial_Attack;
    //                break;



    //            case Story_Lv1.StoryStage_LV1.game_MoveTo_Area1:
    //                guideNode[1].SetActive(false);
    //                guideNode[4].SetActive(false);
    //                GC.SC.SwitchBGM(false);
    //                TM.Close_Panels();
    //                GridsFall(4);

    //                trigger = Check_StoryTrigger.moveToArea;
    //                Vectors_TriggerAreaCheck = story_Lv1.Vectors_Trigger_EnterArea_1_Plot;

    //                yield return new WaitForSeconds(0.3f);
    //                lightFolder[0].SetActive(true);
    //                SwitchGrid(story_Lv1.setWalkableGrids_1_2, false);
    //                break;



    //            case Story_Lv1.StoryStage_LV1.plot_LearnFight_SmellRats:
    //                GC.UI.combatPanelUI.gameObject.SetActive(false);
    //                StartGamePlot(story_Lv1.dialog_SmellRat);
    //                yield return new WaitForSeconds(0.1f);
    //                GC.UI.combatPanelUI.gameObject.SetActive(false);
    //                break;

    //            case Story_Lv1.StoryStage_LV1.game_MoveIn_Area1_1:
    //                yield return new WaitForSeconds(0.3f);
    //                GC.storyState = GameController._storyState.game;
    //                GC.stageController.GameStart(false);
    //                trigger = Check_StoryTrigger.moveToArea;
    //                Vectors_TriggerAreaCheck = story_Lv1.Vectors_Trigger_EnterArea_1_Tut;
    //                break;

    //            case Story_Lv1.StoryStage_LV1.tut_Height:
    //                GC.PauseGame();
    //                GC.storyState = GameController._storyState.tutorial;
    //                TM.allPanels[2].gameObject.SetActive(true);
    //                break;



    //            case Story_Lv1.StoryStage_LV1.game_MoveIn_Area1_2:
    //                GC.storyState = GameController._storyState.game;
    //                GC.stageController.GameStart(false);
    //                trigger = Check_StoryTrigger.moveToArea;
    //                Vectors_TriggerAreaCheck = story_Lv1.Vectors_Trigger_EnterArea_1_Combat;
    //                break;



    //            case Story_Lv1.StoryStage_LV1.plot_FightRats_Area1:

    //                GC.SC.SwitchBGM(true);

    //                GC.PauseGame();
    //                GC.SC.SwitchBGM(true);

    //                GC.gameState = GameController._gameState.gamePause;
    //                GC.storyState = GameController._storyState.tutorial;

    //                //Turn on enemies
    //                foreach (GameObject unit in story_Lv1.Rats_Area_1)
    //                {
    //                    unit.SetActive(true);
    //                    unit.GetComponent<Unit>().InputAnimation_Single("rest");
    //                }
    //                yield return new WaitForSeconds(0.3f);
    //                StartGamePlot(story_Lv1.dialog_FirstRat);
    //                break;


    //            case Story_Lv1.StoryStage_LV1.tut_OpenUnitPanel:
    //                GC.storyState = GameController._storyState.tutorial;
    //                guideNode[6].SetActive(true);
    //                //avatar = AIFunctions.FindPlayer();
    //                guideNode[6].transform.position = story_Lv1.Rats_Area_1[0].transform.position + new Vector3(0, -0.1f, 0);
    //                TM.allPanels[3].gameObject.SetActive(true);
    //                TM.allPanels[3].panel_Tutorial[0].gameObject.SetActive(true);
    //                TM.allPanels[3].GetComponent<Animator>().SetTrigger("trigger");
    //                trigger = Check_StoryTrigger.tutorial_OpenUnitPanel;
    //                break;


    //            case Story_Lv1.StoryStage_LV1.tut_CheckUnitStates:
    //                GC.storyState = GameController._storyState.tutorial;
    //                TM.Close_Panels();

    //                yield return new WaitForSeconds(0.3f);
    //                TM.allPanels[4].gameObject.SetActive(true);
    //                break;



    //            case Story_Lv1.StoryStage_LV1.game_FightRats_Area1:

    //                //[Open Notes for height]
    //                TM.Close_Panels();
    //                TM.allPanels[3].gameObject.SetActive(true);
    //                TM.allPanels[3].panel_Tutorial[1].gameObject.SetActive(true);
    //                TM.allPanels[3].GetComponent<Animator>().SetTrigger("trigger");

    //                guideNode[6].SetActive(false);
    //                guideNode[7].SetActive(true);
    //                GC.storyState = GameController._storyState.game;
    //                GC.stageController.GameStart(true);
    //                trigger = Check_StoryTrigger.killAllEnemy;
    //                SwitchGrid(story_Lv1.setWalkableGrids_1_2, false);
    //                break;

    //            #endregion
    //            #region LV1_PART_1
    //            //PART1       //PART1       //PART1       //PART1       //PART1
    //            case Story_Lv1.StoryStage_LV1.plot_FeelPowerInside:

    //                GC.SC.SwitchBGM(false);

    //                guideNode[7].SetActive(false);
    //                TM.Close_Panels();
    //                GC.SC.SwitchBGM(false);
    //                GC.PauseGame();
    //                StartGamePlot(story_Lv1.dialog_GainRevival);
    //                break;


    //            case Story_Lv1.StoryStage_LV1.tut_WatchSkill:
    //                GC.PauseGame();
    //                yield return new WaitForSeconds(0.5f);
    //                GC.UI.learnSkillPanel.gameObject.SetActive(true);
    //                GC.UI.learnSkillPanel.Input(story_Lv1.revivalSkill.skillSprite);
    //                avatar.currentData.Skill.Add(new _Skill(story_Lv1.revivalSkill, 0));
    //                avatar.currentData.Skill[0].CD = 0;

    //                yield return new WaitForSeconds(3.5f);

    //                GC.PauseGame();
    //                GC.storyState = GameController._storyState.tutorial;
    //                yield return new WaitForSeconds(0.2f);
    //                TM.allPanels[5].gameObject.SetActive(true);
    //                break;


    //            //open grid
    //            case Story_Lv1.StoryStage_LV1.tut_UseSkill_1_ClickIcon:
    //                GC.stageController.GameStart(true);
    //                GC.Select_Unit(AIFunctions.FindPlayer().nodeAt, false);

    //                yield return new WaitForSeconds(0.2f);
    //                TM.allPanels[6].gameObject.SetActive(true);
    //                TM.allPanels[6].panel_Tutorial[0].SetActive(true);
    //                TM.allPanels[6].panel_Tutorial[2].SetActive(true);
    //                TM.allPanels[6].GetComponent<Animator>().SetTrigger("trigger");
    //                trigger = Check_StoryTrigger.tutorial_ClickSkill;
    //                break;

    //            case Story_Lv1.StoryStage_LV1.tut_UseSkill_2_SelectCorpses:

    //                TM.allPanels[6].panel_CheckMark[0].SetActive(true);
    //                TM.allPanels[6].panel_Tutorial[2].SetActive(false);

    //                yield return new WaitForSeconds(0.2f);
    //                TM.allPanels[6].panel_Tutorial[1].SetActive(true);
    //                TM.allPanels[6].panel_Tutorial[2].SetActive(true);

    //                trigger = Check_StoryTrigger.tutorial_ClickCorpse;
    //                story_Lv1.specialCorpse.corpseGuide.SetActive(true);
    //                break;



    //            case Story_Lv1.StoryStage_LV1.tut_InRevivalPanel:
    //                GC.storyState = GameController._storyState.tutorial;
    //                TM.Close_Panels();
    //                yield return new WaitForSeconds(0.2f);
    //                TM.allPanels[7].gameObject.SetActive(true);
    //                break;


    //            case Story_Lv1.StoryStage_LV1.game_Summon1stUndead:
    //                GC.storyState = GameController._storyState.game;
    //                GC.gameState = GameController._gameState.playerInRevivalPanel;
    //                trigger = Check_StoryTrigger.playerTeamNumber;
    //                SizeNumber = 2;
    //                break;

    //            #endregion
    //            #region LV1_PART_2
    //            //PART2     //PART2     //PART2     //PART2     //PART2
    //            case Story_Lv1.StoryStage_LV1.plot_FightRats_Area2:

    //                GC.SC.SwitchBGM(true);


    //                FPManager.FPChange(3);


    //                GC.gameState = GameController._gameState.gamePause;
    //                GC.storyState = GameController._storyState.setup;

    //                yield return new WaitForSeconds(0.3f);

    //                SwitchGrid(story_Lv1.setWalkableGrids_1_2, true);
    //                SwitchGrid(story_Lv1.setWalkableGrids_2To3, false);


    //                lightFolder[1].SetActive(true);

    //                foreach (GameObject unit in story_Lv1.Rats_Area_2)
    //                {
    //                    unit.SetActive(true);
    //                }

    //                yield return new WaitForSeconds(0.3f);
    //                StartGamePlot(story_Lv1.dialog_Rat_2);
    //                break;

    //            case Story_Lv1.StoryStage_LV1.tutorial_Curse:
    //                GC.storyState = GameController._storyState.tutorial;
    //                GC.gameState = GameController._gameState.gamePause;
    //                TM.allPanels[8].gameObject.SetActive(true);
    //                break;

    //            case Story_Lv1.StoryStage_LV1.game_FightRats_Area2:
    //                GC.stageController.GameStart(true);
    //                trigger = Check_StoryTrigger.killAllEnemy;
    //                TM.allPanels[3].gameObject.SetActive(true);
    //                TM.allPanels[3].panel_Tutorial[3].gameObject.SetActive(true);
    //                TM.allPanels[3].GetComponent<Animator>().SetTrigger("trigger");
    //                break;

    //            #endregion
    //            #region LV1_PART_3
    //            //PART3       //PART3       //PART3       //PART3       //PART3
    //            case Story_Lv1.StoryStage_LV1.plot_MemoryOfLongdead_Area3:

    //                GC.SC.SwitchBGM(false);
    //                GC.gameState = GameController._gameState.gamePause;
    //                GC.storyState = GameController._storyState.setup;
                    
    //                FPManager.FPChange(3);
    //                TM.Close_Panels();

    //                SwitchGrid(story_Lv1.setWalkableGrids_2To3, true);
    //                SwitchGrid(story_Lv1.setWalkableGrids_3To4, false);

    //                story_Lv1.corpse_Warrior.SetActive(true);
    //                story_Lv1.corpse_Archer.SetActive(true);


    //                lightFolder[2].SetActive(true);
    //                yield return new WaitForSeconds(0.3f);
    //                StartGamePlot(story_Lv1.dialog_RevivalLongdead_3);
    //                break;

    //            case Story_Lv1.StoryStage_LV1.tutorial_Archer:
    //                GC.storyState = GameController._storyState.tutorial;
    //                GC.gameState = GameController._gameState.gamePause;
    //                TM.allPanels[9].gameObject.SetActive(true);
    //                break;


    //            case Story_Lv1.StoryStage_LV1.game_Summon3rdUndead:
    //                TM.Close_Panels();
    //                TM.allPanels[3].gameObject.SetActive(true);
    //                TM.allPanels[3].panel_Tutorial[4].gameObject.SetActive(true);
    //                TM.allPanels[3].GetComponent<Animator>().SetTrigger("trigger");
    //                GC.stageController.GameStart(false);
    //                trigger = Check_StoryTrigger.playerTeamNumber;
    //                SizeNumber = 4;
    //                break;

    //            case Story_Lv1.StoryStage_LV1.plot_ContinueWalkingToArea3:
    //                TM.Close_Panels();
    //                yield return new WaitForSeconds(0.3f);
    //                StartGamePlot(story_Lv1.dialog_GatherTogether);
    //                break;

    //                #endregion
    //        }

    //    Debug.Log("Current Tutorial:" + story_Lv1.stage_1.ToString());
    //}
    #endregion
    //    private IEnumerator GoNextStage_Level_2()
    //    {
    //        Unit avatar = AIFunctions.FindPlayer();

    //        switch (story_Lv2.stage_2)
    //        {
    //            #region LV2_Start
    //            case Story_Lv2.StoryStage_LV2.plot_FindCamp_Area4:

    //                GC.SC.SwitchBGM(true);
    //                GC.gameState = GameController._gameState.gamePause;
    //                GC.storyState = GameController._storyState.setup;

    //               // story_Lv2.guide_MoveToCampArrow.SetActive(false);
    //                SwitchGrid(story_Lv1.setWalkableGrids_3To4, true);

    //                lightFolder[0].gameObject.SetActive(true);
    //                foreach (GameObject unit in story_Lv2.Rats_Area_4)
    //                {
    //                    unit.SetActive(true);
    //                }

    //                yield return new WaitForSeconds(0.2f);
    //                StartGamePlot(story_Lv2.dialog_findCamp);
    //                break;

    //            case Story_Lv2.StoryStage_LV2.tutorial_Camp:
    //                GC.storyState = GameController._storyState.tutorial;
    //                GC.gameState = GameController._gameState.gamePause;
    //                TM.allPanels[10].gameObject.SetActive(true);
    //                break;


    //            case Story_Lv2.StoryStage_LV2.game_FightRatInCamp_Area4:
    //                GC.stageController.GameStart(true);
    //                trigger = Check_StoryTrigger.killAllEnemy;
    //                break;




    //            case Story_Lv2.StoryStage_LV2.plot_AfterKillCamp:
    //                GC.SC.SwitchBGM(false);
    //                FPManager.FPChange(3);

    //                GC.gameState = GameController._gameState.gamePause;
    //                GC.storyState = GameController._storyState.setup;
    //                yield return new WaitForSeconds(0.3f);
    //                story_Lv2.skeleton.SetActive(true);

    //                SwitchGrid(story_Lv2.setWalkableGrids_4To5, true);
    //                lightFolder[1].SetActive(true);
    //                yield return new WaitForSeconds(0.3f);
    //                StartGamePlot(story_Lv2.dialog_exitCamp);
    //                break;



    //            case Story_Lv2.StoryStage_LV2.game_MoveTo_Area5:

    //                GC.gameState = GameController._gameState.gamePause;
    //                yield return new WaitForSeconds(0.5f);
    //                GC.UI.learnSkillPanel.gameObject.SetActive(true);
    //                GC.UI.learnSkillPanel.Input(story_Lv2.banefireSkill.skillSprite);
    //                avatar.currentData.Skill.Add(new _Skill(story_Lv2.banefireSkill, 0));
    //                avatar.currentData.Skill[1].CD = 0;
    //                yield return new WaitForSeconds(3.5f);

    //                GC.stageController.GameStart(true);
    //                trigger = Check_StoryTrigger.moveToArea_Team;
    //                Vectors_TriggerAreaCheck = story_Lv2.Vectors_Trigger_5;
    //                SwitchGrid(story_Lv2.setWalkableGrids_5To6, false);

    //                story_Lv2.guides_Area5.SetActive(true);
    //                SizeNumber = 3;
    //                break;

    //            case Story_Lv2.StoryStage_LV2.plot_FinalFight:
    //                GC.gameState = GameController._gameState.gamePause;
    //                GC.storyState = GameController._storyState.setup;

    //                foreach (GameObject unit in story_Lv2.Rats_Area_6)
    //                {
    //                    unit.SetActive(true);
    //                }
    //                yield return new WaitForSeconds(0.1f);
    //                story_Lv2.guides_Area5.SetActive(false);
    //                GC.SC.SwitchBGM(true);
    //                lightFolder[2].SetActive(true);
    //                yield return new WaitForSeconds(0.3f);
    //                StartGamePlot(story_Lv2.dialog_finalFight);
    //                SwitchGrid(story_Lv2.setWalkableGrids_5To6, true);
    //                break;

    //            case Story_Lv2.StoryStage_LV2.game_FinalFight:
    //                GC.stageController.GameStart(true);
    //                trigger = Check_StoryTrigger.killAllEnemy;
    //                break;

    //            case Story_Lv2.StoryStage_LV2.plot_AfterKillFinal:
    //                GC.gameState = GameController._gameState.gamePause;
    //                GC.storyState = GameController._storyState.setup;
    //                yield return new WaitForSeconds(0.3f);
    //                StartGamePlot(story_Lv2.dialog_afterFinalFight);
    //                break;
    //                #endregion
    //        }
    //        Debug.Log("Current Tutorial:" + story_Lv2.stage_2.ToString());
    //    }
}




[System.Serializable]
public class Story_Lv1
{
    public enum DebugStage
    {
        normal,
        attackDoor,
        attackRat,
        attackCamp,
    }
    [Header("Debug Tools")]
    public DebugStage debugStage;


    public enum StoryStage_LV1
    {
        none,
        cutscene_walkup,
        plot_WakeUpInCave,
        tut_moveCam,
        tut_Select,
        tut_Move,
        tut_Deselect,

        plot_Hungry,

        game_MoveToBody,
        plot_EatCorpse_n_Collapse,

        cutscene_RockFall_1,
        plot_Run,
        game_RockFall_2,
        game_RockFall_3,
        game_RockFall_4,


        tut_Attack,

        game_MoveTo_Area1,
        plot_LearnFight_SmellRats,

        game_MoveIn_Area1_1,
        tut_Height,
        game_MoveIn_Area1_2,


        plot_FightRats_Area1,
        tut_OpenUnitPanel,
        tut_CheckUnitStates,
        game_FightRats_Area1,

        //[think about revival]
        plot_FeelPowerInside,

        tut_WatchSkill,
        tut_UseSkill_1_ClickIcon,
        tut_UseSkill_2_SelectCorpses,
        tut_InRevivalPanel,
        game_Summon1stUndead,


        //[P2_GotoNextPlace]
        plot_FightRats_Area2,
        tutorial_Curse,
        game_FightRats_Area2,

        //[P3_GotoNextPlace]
        plot_MemoryOfLongdead_Area3,
        tutorial_Archer,
        game_Summon3rdUndead,

        plot_ContinueWalkingToArea3,
        game_MoveTo_Area3,
    }


    public StoryStage_LV1 stage_1;



    [Header("Default")]
    public List<GameObject> StaticUnit_UIPanels;


    [Header("WakeUp")]
    [Space(100)]
    public float wakeup_timer;

    public NewDialog dialog_Awake;

    public List<Vector2Int> Vectors_Trigger_FirstMove;
    public List<Vector2Int> Vectors_BlockToCorpse;
    public NewDialog dialog_GuideToCorpse;
    public List<Vector2Int> setWalkableGrids_Area0_ToDoor;

    public List<Vector2Int> Vectors_Trigger_StartEatCorpse;
    public NewDialog dialog_EatCorpse_n_Collapse;

    public NewDialog dialog_RunAway;

    public Unit door;

    [Header("Rockes")]
    [Space(100)]

    public List<Vector2Int> Vectors_Trigger_Rockfall0;
    public List<Vector2Int> Vectors_Trigger_Rockfall1;
    public List<Vector2Int> Vectors_Trigger_Rockfall2;
    public List<Vector2Int> Vectors_Trigger_Rockfall3;
    public List<Vector2Int> Vectors_Trigger_Rockfall4;


    [Header("Area_1_Rats")]
    [Space(100)]
    public NewDialog dialog_SmellRat;
    public NewDialog dialog_FirstRat;
    public NewDialog dialog_GainRevival;

    public List<GameObject> Rats_Area_1;
    public List<Vector2Int> Vectors_Trigger_EnterArea_1_Plot;
    public List<Vector2Int> Vectors_Trigger_EnterArea_1_Tut;
    public List<Vector2Int> Vectors_Trigger_EnterArea_1_Combat;
    public Skill revivalSkill;

    [HideInInspector]
    public CorpseSpecialFunction specialCorpse;
    [Space(5)]
    public List<Vector2Int> setWalkableGrids_1_2;



    [Header("Area_2_Curse")]
    [Space(100)]
    public NewDialog dialog_Rat_2;
    public List<GameObject> Rats_Area_2;
    [Space(5)]
    public List<Vector2Int> setWalkableGrids_2To3;




    [Header("Area_3_Archer&Warrior")]
    [Space(100)]
    public NewDialog dialog_RevivalLongdead_3;
    public GameObject corpse_Warrior;
    public GameObject corpse_Archer;

    [Space(5)]
    public List<Vector2Int> setWalkableGrids_3To4;

    [Space(100)]
    public GameObject guide_MoveToCampArrow;
    public NewDialog dialog_SummonMoreUndead;
    public NewDialog dialog_GatherTogether;
    public List<Vector2Int> Vectors_Trigger_2_1_ToCamp;
}
[System.Serializable]
public class Story_Lv2
{

    //public enum DebugStage
    //{
    //    normal,
    //    attackDoor,
    //    attackRat,
    //    attackCamp,
    //}
    //[Header("Debug Tools")]
    //public DebugStage debugStage;


    public enum StoryStage_LV2
    {
        //[P4_Gamp]
        plot_FindCamp_Area4,
        tutorial_Camp,
        game_FightRatInCamp_Area4,
        plot_AfterKillCamp,


        //[P5_GotoNextPlace]
        game_MoveTo_Area5,


        plot_FinalFight,
        game_FinalFight,

        plot_AfterKillFinal,
    }


    public StoryStage_LV2 stage_2;

    [Header("Area_4_Camp")]
    [Space(100)]
    public List<GameObject> Rats_Area_4;
    public NewDialog dialog_findCamp;
    public NewDialog dialog_exitCamp;
    public Skill banefireSkill;

    [Header("Area_5_MoreCamp")]
    [Space(100)]
    public List<Vector2Int> setWalkableGrids_4To5;
    public GameObject guides_Area5;
    public GameObject skeleton;
    public List<Vector2Int> Vectors_Trigger_5;


    [Header("Area_6_FinalFight")]
    [Space(100)]
    public NewDialog dialog_finalFight;
    public List<Vector2Int> setWalkableGrids_5To6;
    public List<GameObject> Rats_Area_6;
    public NewDialog dialog_afterFinalFight;
}