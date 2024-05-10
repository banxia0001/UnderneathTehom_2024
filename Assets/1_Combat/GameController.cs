using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public enum _storyState
    {
        setup,
        story,
        tutorial,
        game,
    }

    //public enum _gameWinCondition
    //{
    //    none,//game will not end
    //    noEnemySurvive,//kill all
    //    enterLocation,//enter location
    //    enterLocation_WithoutEnemy
    //}

    public enum _gameState
    {
        playerTurn,
        playerInSpell,
        playerInRevivalPanel,
        playerInUnitPanel,
        playerInRevivalPreview,

        playerTurnEnd_AutoAttack,

        enemyTurn_ChooseUnit,
        enemyTurn_Action,

        playerTurnPrepare,
        enemyTurn_Prepare,
        gamePause,
    }

    [Header("GameStateControl")]
    public int sceneNumber;
    public StoryManager SM;
    public _storyState storyState;
    public _gameState gameState;
   
    [HideInInspector] public StageController stageController;
    [HideInInspector] public AIUnitController AIUnitController;

    [Header("Game_Debug_Setting")]
    public bool debug_SkipPlot;
    public bool debug_InfiniteMana;
    public bool debug_CancelCameraMoveInNewTurn;
    public bool debug_autoWin = false;
    [Header("AbilityLock")]
    public bool can_MoveCam;

    [Header("InGameBools")]
    public static bool frozenGame;
    public bool isMoving;
    public bool isAttacking;
    public bool isAIThinking;
    public bool isOnUi;
  
    public bool updateWhenMouseOnDiffGrid = true;

    public static List<Unit> playerList;
    public static List<Unit> heroList;
    public static List<Unit> sentryList;
    public static List<Unit> summonList;
    public static List<Unit> enemyList;

    [Header("Game_Stats")]
    public PathNode mouseAtNode;
    public Unit selectedUnit;
  
    public _Skill skillInUse;
    private int skillNumber;
    public bool bool_DontOpenMouseStatsPanel;


    public PathNode nodeWithEnemyNear; //[Grid nearby a enemy unit]
    public PathNode nodeWillGo;  //[Grid unit is targeting to go]
    public Unit shouldAttackUnit; //[Target this unit is going to hit]

    [HideInInspector] public SFX_Controller SC;
    [HideInInspector] public MySpineEvent MSE;
    [HideInInspector] public FPManager FP;

    public static Unit currentActionUnit;

    //[Mana]
    //[HideInInspector] public int CP = 0;
    //[HideInInspector] public int CPInUse = 0;
    [HideInInspector] public CamMove CM;
    public Animator cameraAnim;


    [HideInInspector]
    public float chooseAiCoolDown;

    //////////////////////////
    //Set Up//Set Up//Set Up//
    //Set Up//Set Up//Set Up//
    //////////////////////////



    [Header("Other")]
    public Camera Cam;
    public GameUI UI;
     public List<PathNode> path;
    [HideInInspector] public GridManager GM;
    public GameObject popText;
    public GameObject mouseLight;
    public MouseStats_Ver3 mouseStatsV3;
    public GameObject attackArrow;
    public GameObject spellVisual_FollowMouse;
    [HideInInspector]
    public List<GameObject> lightMaps;
    public List<GameObject> target_lightMaps;
    [HideInInspector]
    
    //private float doubleClickTimer;
    private ColorfulGuideLine colorfulLine;
    public RevivalController revivalController;

    //public LineRenderer lineRenderer; //[Use to draw walking path]
    public LineRenderer curveRenderer; //[Use to draw shooting/range spell path]
    public LineRenderer curveRenderer_Purple; //[Use for necro spell]
                                              //public GameObject lineRendererCircle; //[Center point of unit targeting to go position]
    [Header("Saves")]
    public Saveload_System saveloadSys;

    private void Awake()
    {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;
        frozenGame = false;
        //Time.timeScale = 0.75f;
        playerList = new List<Unit>();
        enemyList = new List<Unit>();
        storyState = _storyState.setup;
        gameState = _gameState.gamePause;

        GM = FindObjectOfType<GridManager>(false);
        UI = FindObjectOfType<GameUI>(true);
        CM = FindObjectOfType<CamMove>(true);
        SM = FindObjectOfType<StoryManager>(false);
        SC = FindObjectOfType<SFX_Controller>(true);
        MSE = FindObjectOfType<MySpineEvent>(true);
        FP = FindObjectOfType<FPManager>(true);
        saveloadSys = FindObjectOfType<Saveload_System>(true);

        stageController = FindObjectOfType<StageController>(true);
        AIUnitController = FindObjectOfType<AIUnitController>(true);
        colorfulLine = FindObjectOfType<ColorfulGuideLine>(true);

        UI.gameObject.SetActive(true);
        UI.combatPanelUI.gameObject.SetActive(false);
        spellVisual_FollowMouse.SetActive(false);
        StartCoroutine(GameSetUp());

        revivalController.QuitPreview();
        revivalController.SwitchPreviewEnable(false);
        UI.Switch_UnitPanel(false);
        can_MoveCam = true;
    }

    public void ReselectUnit()
    {
        if (selectedUnit != null) Select_Unit(selectedUnit.nodeAt, false);
    
    }
    private IEnumerator GameSetUp()
    {
        //[Wait .1S.]
        //All the grids in the scene will add themself into the pathArray[,] in their start function.
        //So here we wait for a little, them start next move.
        FPManager.FP = UnitFunctions.AddFP();
        yield return new WaitForSeconds(.05f);
        player = AIFunctions.FindPlayer();

        mouseAtNode = GM.GetPath(0, 0);
        UI.loadingPanel.SetBool("Trigger", false);
        UI.combatPanelUI.gameObject.SetActive(false);

        if(!SaveData.firstEnterComat_LV_1_1)
        saveloadSys.StartBattle_SpawnPlayer(GM);

        Check_UnitInList();
        //If StoryMananger is null, then start game.
        if (SM == null) { stageController.GameStart(true,false); SC.SwitchBGM(true); }
        else {  SM.GoNextStage(); }
    }

    public void FindNextUnit()
    {
        Debug.Log("!!!!");
        Unit myUnit = null;
        foreach (Unit unit in playerList)
        {
            if (unit.isActive == true)
            {
                if (selectedUnit != null)
                {
                    if (unit == selectedUnit) continue;
                }
                myUnit = unit;
            }
        }

        if(myUnit == null && selectedUnit != null) myUnit = selectedUnit;

        if (myUnit != null)
        {
            Select_Unit(myUnit.nodeAt, true);
        }
    }

    public bool Check_CanGamePause()
    {
        bool canPause = true;
        if (this.gameState == _gameState.enemyTurn_Action ||
            gameState == _gameState.enemyTurn_ChooseUnit ||
            gameState == _gameState.enemyTurn_Prepare)
            canPause = false;

        if (isAttacking || isAIThinking) canPause = false;

        return canPause;
    }
    public void PauseGame()
    {
        Deselect_Unit();
        gameState = _gameState.gamePause;
        if (moveCor != null) StopCoroutine(moveCor);
        isMoving = false;
    }


    //////////////////////////////////////////
    //Update//Update//Update//Update//Update//
    //Update//Update//Update//Update//Update//
    //Update//Update//Update//Update//Update//
    //////////////////////////////////////////

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                foreach (Unit unit in enemyList)
                {
                    unit.HealthChange(-1000, 0, "damage");
                }
            }
        }

        if (debug_autoWin) saveloadSys.EndBattle();
        switch (storyState)
        {
            case _storyState.setup:

                break;

            case _storyState.story: 
                UI.CheckUIRaycast_StoryMode();
                break;

            case _storyState.tutorial:
                if (SM != null)
                {
                    if(SM.story_Lv1.stage_1 == Story_Lv1.StoryStage_LV1.tut_OpenUnitPanel) PlayerInput_RightClick_OpenPanel();
                   //PlayerInput_DoubleClick_OpenPanel();
                }
                Update_Mouse();
                break;

            case _storyState.game:
                Update_Mouse();
                Check_PlayerPressEscape();
                Check_PlayerInput();
                break;
        }
    }
    private void FixedUpdate()
    {
        if (debug_InfiniteMana) FPManager.FP = 9;
        FixedUpdate_UI_ClosePanels();

        //[Update Story]
        switch (storyState)
        {
            case _storyState.setup:
                mouseStatsV3.gameObject.SetActive(false);
                UI.Display_UnitMoveUI.gameObject.SetActive(false);
                break;

            case _storyState.story:
                mouseStatsV3.gameObject.SetActive(false);
                UI.Display_UnitMoveUI.gameObject.SetActive(false);
                break;

            case _storyState.tutorial:
                mouseStatsV3.gameObject.SetActive(false);
                UI.Display_UnitMoveUI.gameObject.SetActive(false); 
                UI.CheckUIRaycast_GameMode();
                break;

            case _storyState.game:

                FixedUpdate_GameStateAction();
                Check_EndGame();

                UI.CheckUIRaycast_GameMode();
                UI.combatPanelUI.SetManaPool(this);
                UI.combatPanelUI.SetMoveText(selectedUnit);
                if (gameState == _gameState.playerInRevivalPreview)
                {
                    FPManager.FPInUse = revivalController.fleshCost;
                }
              
                break;
        }

        FixedUpdate_UI_ActivePanels();
    }
    private void FixedUpdate_GameStateAction()
    {
        if (AIUnitController.selectedUnit_AI != null)
        {
            QuickCamFollow_Update(AIUnitController.selectedUnit_AI.transform.position);
        }

        //[Game State Update]
        if (isAttacking || isAIThinking || isMoving) return;

        switch (gameState)
        {
            case _gameState.playerTurn: //Player is selecting unit or selected unit.
                #region playerInput
                if (selectedUnit != null && !isMoving && !isAttacking && !isOnUi && selectedUnit.unitSpecialState == Unit.UnitSpecialState.normalUnit)  //[Player visual guide is calculate here] 
                {
                    if (updateWhenMouseOnDiffGrid)//When mouse move
                    {
                        if (nodeWithEnemyNear != null)
                            if (nodeWithEnemyNear != mouseAtNode)
                                if (nodeWithEnemyNear.unit != null)
                                    nodeWithEnemyNear = null;

                        updateWhenMouseOnDiffGrid = false;
                        if (GameFunctions.CheckPathRange(mouseAtNode, selectedUnit.nodeAt, selectedUnit.currentData.movePointNow + 1, true))
                        {
                            //Debug.Log("DrawLine");
                            curveRenderer.gameObject.SetActive(false);
                            curveRenderer_Purple.gameObject.SetActive(false);
                            Drawline();
                        }
                        CheckShoot_CurveLine();
                    }
                }

                else { DeleteAllWalkingPath(); }   //Close line.
                #endregion
                if (storyState == GameController._storyState.game) 
                    stageController.Check_PlayerTurnEnd();
                break;

            case _gameState.playerInSpell: //[Unit gonna cast spell Update]
                #region PlayerInSpellInput

                if (updateWhenMouseOnDiffGrid) //[Player visual guide is calculate here]
                {
                    updateWhenMouseOnDiffGrid = false;
                    if (GameFunctions.CheckPathRange(mouseAtNode, selectedUnit.nodeAt, 2
                        + skillInUse.skill.range, true))
                    {
                        colorfulLine.CloseSkillVisual();
                        colorfulLine.SkillVisual_FixedUpdate(skillInUse, mouseAtNode);
                    }

                }
                #endregion
                if (storyState == GameController._storyState.game)
                    stageController.Check_PlayerTurnEnd();
                break;

            case _gameState.playerInRevivalPreview: //[Unit gonna cast spell Update]
                if (updateWhenMouseOnDiffGrid) //[Player visual guide is calculate here]
                {
                    mouseStatsV3.gameObject.SetActive(false); 
                    updateWhenMouseOnDiffGrid = false;

                    if (GameFunctions.CheckPathRange(mouseAtNode, selectedUnit.nodeAt, 2
                        + skillInUse.skill.range, true))
                    {
                        colorfulLine.CloseSkillVisual();
                        colorfulLine.SkillVisual_FixedUpdate(skillInUse, mouseAtNode);
                    }

                }
                if (storyState == GameController._storyState.game)
                    stageController.Check_PlayerTurnEnd();
                break;

            case _gameState.playerInRevivalPanel: //[Unit gonna cast spell Update]

                break;

            case _gameState.playerTurnEnd_AutoAttack: //[Unit gonna cast spell Update]

                AIUnitController.Revival_MachineGun_Choosing();
                break;

            case _gameState.enemyTurn_ChooseUnit:   //[Choose Enemy Unit]
                
                chooseAiCoolDown += Time.fixedDeltaTime;
                if (chooseAiCoolDown > 0.2f)
                {
                    chooseAiCoolDown = 0;
                    AIUnitController.EnemyChoosing();
                }
                if (storyState == GameController._storyState.game) 
                    stageController.Check_PlayerTurnStart();
                break;

            case _gameState.enemyTurn_Action: //[Enemy Unit Action]
                AIUnitController.EnemyThinking();
                if (storyState == GameController._storyState.game) 
                    stageController.Check_PlayerTurnStart();
                break;
        }
    }

    private void FixedUpdate_UI_ClosePanels()
    {
        
        UI.ClosePanels();
        UI.combatPanelUI.enemyPanel.SetActive(false);
        //UI.combatPanelUI.enemyPanel_ButtonRight.SetActive(false);
        UI.combatPanelUI.panel_Skill.SetActive(false);
        UI.MouseSkillIcon.SetActive(false);
    }
    private void FixedUpdate_UI_ActivePanels()
    {
        switch (gameState)
        {
            case _gameState.playerTurn:

                bool closeV3 = true;
                if (isAttacking == false && isMoving == false && isOnUi == false && !bool_DontOpenMouseStatsPanel)
                {
                    //Debug.Log("!!2");
                    if (mouseAtNode != null)
                        if (mouseAtNode.unit != null)
                        {
                            //Debug.Log("!!3");
                            closeV3 = false;
                            mouseStatsV3.gameObject.SetActive(true);
                            mouseStatsV3.InputStats(mouseAtNode.unit, selectedUnit);
                        }
                }
                if(closeV3) mouseStatsV3.gameObject.SetActive(false);

                break;

            case _gameState.playerInRevivalPanel:
                UI.Display_UnitMoveUI.gameObject.SetActive(false);
                break;

            case _gameState.playerInRevivalPreview:
                UI.Display_UnitMoveUI.gameObject.SetActive(false);
                UI.combatPanelUI.panel_Skill.SetActive(true);
                mouseStatsV3.gameObject.SetActive(false);
                if (skillInUse != null)
                {
                    UI.MouseSkillIcon.SetActive(true);
                    UI.MouseSkillIcon.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = skillInUse.skill.skillSprite;
                }
                break;

            case _gameState.playerInSpell:
                UI.Display_UnitMoveUI.gameObject.SetActive(false);
                UI.combatPanelUI.panel_Skill.SetActive(true);
                mouseStatsV3.gameObject.SetActive(false);
                if (skillInUse != null)
                {
                    UI.MouseSkillIcon.SetActive(true);
                    UI.MouseSkillIcon.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = skillInUse.skill.skillSprite;
                }
                break;

            case _gameState.playerInUnitPanel:
                isOnUi = true;
                UI.combatPanelUI.gameObject.SetActive(false);
                UI.Display_UnitMoveUI.gameObject.SetActive(false);
                mouseStatsV3.gameObject.SetActive(false);
                break;

            case _gameState.enemyTurn_Action:
                UI.combatPanelUI.enemyPanel.SetActive(true);
                mouseStatsV3.gameObject.SetActive(false);
                if (AIUnitController.selectedUnit_AI != null)
                    UI.combatPanelUI.enemyPanelText.text = AIUnitController.selectedUnit_AI.data.Name + " Moving";
                break;
        }

        //[UI Update Raycast]
        if (isMoving || isAttacking)
            UI.Display_UnitMoveUI.gameObject.SetActive(false);
    }


    //private void Check_OpenRightButtonEnemyIcon()
    //{
    //    if (mouseAtNode != null)
    //        if (mouseAtNode.unit != null)
    //        {
    //            if (mouseAtNode.unit.unitTeam == Unit.UnitTeam.enemyTeam)
    //            {
    //                UI.combatPanelUI.SetEnemyButtonRight(mouseAtNode.unit);
    //            }
    //        }
    //}





    public void Check_UnitInList()
    {
        playerList = new List<Unit>();
        sentryList = new List<Unit>();
        heroList = new List<Unit>();
        summonList = new List<Unit>();
        enemyList = new List<Unit>();

        foreach (Unit unitIn in FindObjectsOfType<Unit>())
        {
            if (unitIn != null)
            {
                if (unitIn.unitTeam == Unit.UnitTeam.playerTeam)
                {
                    playerList.Add(unitIn);
                    if (unitIn.unitType == Unit.UnitSummonType.hero) heroList.Add(unitIn);
                    if (unitIn.unitType == Unit.UnitSummonType.sentry) sentryList.Add(unitIn);
                    if (unitIn.unitType == Unit.UnitSummonType.summon) summonList.Add(unitIn);
                }
                else if (unitIn.unitTeam == Unit.UnitTeam.enemyTeam) enemyList.Add(unitIn);
            }
        }
        //if (UI.combatPanelUI.combatUnitList_V2.enabled)
        //UI.combatPanelUI.combatUnitList_V2.SpawnIcons();
    }

    private void Check_PlayerInput()
    {
        if (isMoving || isAttacking) return;
        if (storyState == _storyState.setup || storyState == _storyState.story) return;
        PlayerInput_PlayMode();
    }

    //[ESC - Deselect Unit Select or Skill]
    private void Check_PlayerPressEscape()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PlayerPressEscape();
        }
    }
    public void PlayerPressEscape()
    {
        if (SM != null)
            if (SM.level == StoryManager.Story_Level.level_1)
                if (SM.story_Lv1.stage_1 == Story_Lv1.StoryStage_LV1.tut_WatchSkill || 
                    SM.story_Lv1.stage_1 == Story_Lv1.StoryStage_LV1.tut_UseSkill_2_SelectCorpses ||
                     SM.story_Lv1.stage_1 == Story_Lv1.StoryStage_LV1.tut_InRevivalPanel ||
                   SM.story_Lv1.stage_1 == Story_Lv1.StoryStage_LV1.game_Summon1stUndead ||
                     SM.story_Lv1.stage_1 == Story_Lv1.StoryStage_LV1.tut_UseSkill_1_ClickIcon) 
                    return;

        if (gameState == _gameState.playerInSpell)
        { 
            gameState = _gameState.playerTurn; 
            colorfulLine.CloseSkillVisual();
            spellVisual_FollowMouse.SetActive(false);
            Select_Unit(selectedUnit.nodeAt, false);
        }

        else if (gameState == _gameState.playerInRevivalPanel)
        {
            gameState = GameController._gameState.playerTurn;
            UI.revivalPanel.gameObject.SetActive(false);
            UI.unitPanelUI.gameObject.SetActive(false);
            Select_Unit(selectedUnit.nodeAt, false);

            if (selectedUnit.data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._1_Avatar)
            {
                bool canWalk = true;
                if (SM != null)
                    if (SM.story_Lv1 != null)
                        if ((int)SM.story_Lv1.stage_1 <= 10) canWalk = false;


                if (canWalk) selectedUnit.InputAnimation_Single("idle");
                else selectedUnit.InputAnimation_Single("idle2");
            }


            else selectedUnit.InputAnimation("idle");
        }

        else if (gameState == _gameState.playerInRevivalPreview)
        {
            gameState = GameController._gameState.playerTurn;
            revivalController.QuitPreview();
            Select_Unit(selectedUnit.nodeAt, false);
            selectedUnit.InputAnimation("idle");
        }

        else if (gameState == _gameState.playerInUnitPanel)
        {
            //Cam Zoom Back
            if (UI.unitPanelUI.panelType == UnitStatsPanel_V2.PanelType.openUnitInGame )
            {
                CamMove cam = FindObjectOfType<CamMove>();
                cam.camBackToNormal_WhenStatsPanelClose();
            }

            UI.unitPanelUI.gameObject.SetActive(false);
            gameState = _gameState.playerTurn;
            UI.combatPanelUI.gameObject.SetActive(true);
            Deselect_Unit();
        }

        else Deselect_Unit();

        UI.Switch_UnitPanel(false);
    }


    public void Update_MachineGunPosition()
    {
        foreach (Unit UNIT in GameController.playerList)
        {
            if (UNIT.unitSpecialState == Unit.UnitSpecialState.machineGun)
            {
                AutoMachineGun_Controller AMC = UNIT.GetComponent<AutoMachineGun_Controller>();
                AMC.SearchForEnemy();
            }
        }
    }
    private void Update_Mouse()
    {
        checkMouseOnGrid();
    }
    public void Button_PlayerTurnEnd()
    {
        if (storyState == _storyState.game)
            StartCoroutine(stageController.PlayerTurn_AutoAttack());
    }

    private Unit player;

    public void Check_EndGame()
    {
        if (frozenGame) return;
        if (player == null)
        {
            player = AIFunctions.FindPlayer();
            if (player == null)
            {
                StartCoroutine(GameLose(sceneNumber));
                return;
            }
        }

        if (heroList == null || heroList.Count == 0)
        {
            StartCoroutine(GameLose(sceneNumber));
            return;
        }
    }

    private IEnumerator GameLose(int sceneNum)
    {
        frozenGame = true;
        storyState = GameController._storyState.setup;
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(sceneNum);
    }

    public static void DeleteAllWalkingPath()
    {
        WalkingArrow[] arrows = FindObjectsOfType<WalkingArrow>(true);
        foreach (WalkingArrow arrow in arrows)
        {
            Destroy(arrow.gameObject);
        }
    }




    ////////////////////////////////////////////////////
    // SELECT UNIT    // SELECT UNIT    // SELECT UNIT//
    // SELECT UNIT    // SELECT UNIT    // SELECT UNIT//
    // SELECT UNIT    // SELECT UNIT    // SELECT UNIT//
    ////////////////////////////////////////////////////
    public void Select_Unit(PathNode currentNode, bool CamMoveTo)
    {
        Check_UnitInList();
        spellVisual_FollowMouse.SetActive(false);

        UI.unitPanelUI.gameObject.SetActive(false);
        if (CamMoveTo)
        {
            CM.addPos(currentNode.transform.position,true);
        }
        //[Check if Unit can be selected]
        if (currentNode.unit == null) return;
        if (currentNode.unit.unitTeam != Unit.UnitTeam.playerTeam) return;
        if (GameController.frozenGame == true) return;
        if (gameState == _gameState.enemyTurn_Prepare) return;
        if (gameState == _gameState.enemyTurn_ChooseUnit) return;
        if (gameState == _gameState.enemyTurn_Action) return;
        if (gameState == _gameState.gamePause) return;
        if (gameState == _gameState.playerTurnPrepare) return;
        if (gameState == _gameState.playerTurnEnd_AutoAttack) return;

        if (currentNode.unit.isActive == false)
        {
            if (enemyList == null || enemyList.Count == 0)
            {
                currentNode.unit.UnitEnable(true);
            }
        }

        Debug.Log("Select:" + currentNode.unit.name);

        //[Clear previous data]
        updateWhenMouseOnDiffGrid = false;
        
        nodeWillGo = null;
        if (this.path != null)
            this.path.Clear();

        FPManager.FPInUse = 0;
        Del_LightMap();
        Del_TargetMap();
        Deselect_Unit();
        skillInUse = null;
        bool_DontOpenMouseStatsPanel = false;

        selectedUnit = currentNode.unit;
        selectedUnit.isSelectedInThisTurn = true;
        FindObjectOfType<EndTurnButton>(true).CheckCurrentStage();

        UI.combatPanelUI.gameObject.SetActive(true);
        UI.combatPanelUI.DeleteSkill();
        UI.combatPanelUI.skill_UI.SetActive(true);
        UI.combatPanelUI.gameObject.SetActive(true);
        UI.combatPanelUI.SwitchUnitIconList(false);


        UI.combatPanelUI.DisplaySkill(selectedUnit.currentData.Skill, selectedUnit);

        if (currentNode.unit.unitSpecialState == Unit.UnitSpecialState.normalUnit)
        {
            _GenerateAllNodeMap(currentNode.unit.isActive);
            List<PathNode> path = GameFunctions.FindNodes_ByDistance(selectedUnit.nodeAt, 1, false);
            foreach (PathNode node in path)
            {
                if (node.unit != null)
                    if (node.unit.unitTeam != selectedUnit.unitTeam)
                    {
                        nodeWithEnemyNear = selectedUnit.nodeAt;
                        break;
                    }
            }
        }

        else if (currentNode.unit.unitSpecialState == Unit.UnitSpecialState.healTower)
        {
            List<PathNode> moveableList = new List<PathNode>();
            moveableList.Add(selectedUnit.nodeAt);
            GameFunctions.Gene_LightMap(moveableList, this);
        }

        else if (currentNode.unit.unitSpecialState == Unit.UnitSpecialState.machineGun)
        {
            AutoMachineGun_Controller AMC = currentNode.unit.GetComponent<AutoMachineGun_Controller>();

            List<PathNode> moveableList = new List<PathNode>();
            moveableList.Add(selectedUnit.nodeAt);
            GameFunctions.Gene_LightMap(moveableList, this);


            if (AMC.targetEnemy != null)
            {
                curveRenderer.gameObject.SetActive(true);
                GameFunctions.DrawCurve_BetweenTwoObject(selectedUnit.transform, AMC.targetEnemy.transform, curveRenderer, 0.5f);
            }
        }

        colorfulLine.Update_GuideMove(selectedUnit);
        colorfulLine.CloseSkillVisual();
        nodeWithEnemyNear = null;
    }
    public void Deselect_Unit()
    {
        if (SM != null)
            if (SM.level == StoryManager.Story_Level.level_1)
                if (SM.story_Lv1.stage_1 == Story_Lv1.StoryStage_LV1.tut_WatchSkill ||
                    SM.story_Lv1.stage_1 == Story_Lv1.StoryStage_LV1.tut_InRevivalPanel ||
                   SM.story_Lv1.stage_1 == Story_Lv1.StoryStage_LV1.tut_UseSkill_2_SelectCorpses ||
                   SM.story_Lv1.stage_1 == Story_Lv1.StoryStage_LV1.game_Summon1stUndead ||
                    SM.story_Lv1.stage_1 == Story_Lv1.StoryStage_LV1.tut_UseSkill_1_ClickIcon) return;

        UI.unitPanelUI.gameObject.SetActive(false);

        //[Clear previous data]
        //if (selectedUnit == null) return;

        colorfulLine.Update_GuideMove(null);
        colorfulLine.CloseSkillVisual();
        selectedUnit = null;
        nodeWithEnemyNear = null;
        updateWhenMouseOnDiffGrid = false;
        StartCoroutine(openDetailPanelLater());
        FPManager.FPInUse = 0;


       // UI.Switch_Portrait_BottomLeft(false);
        curveRenderer.gameObject.SetActive(false);
        curveRenderer_Purple.gameObject.SetActive(false);

        DeleteAllWalkingPath();
        attackArrow.SetActive(false);

        skillInUse = null;
        skillNumber = -1;
        if (gameState == _gameState.playerInSpell) gameState = _gameState.playerTurn;
        if (gameState == _gameState.playerInRevivalPanel)gameState = _gameState.playerTurn;
        if (gameState == _gameState.playerInRevivalPreview)gameState = _gameState.playerTurn;

        attackArrow.SetActive(false);

        Del_LightMap();
        Del_TargetMap();
        UI.combatPanelUI.gameObject.SetActive(true);
        UI.combatPanelUI.DeleteSkill();
        UI.combatPanelUI.skill_UI.SetActive(false);
        UI.combatPanelUI.SwitchUnitIconList(true);
    }

    ///////////////////////////////////////////////////
    //[MouseUpdate]  //[MouseUpdate]  //[MouseUpdate]//
    //[MouseUpdate]  //[MouseUpdate]  //[MouseUpdate]//
    //[MouseUpdate]  //[MouseUpdate]  //[MouseUpdate]//
    ///////////////////////////////////////////////////
    private void checkMouseOnGrid()
    {
        if (isOnUi) return;

        //Raycast on the world
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        spellVisual_FollowMouse.transform.position = pos;
        mouseStatsV3.transform.position = pos;

        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, LayerMask.GetMask("Ground"));
        mouseIsNotOnNode = false;

        if (hit.collider != null)
        {
            //[Get the PahtNode]
            PathNode MouseAtNodeThis = hit.collider.transform.gameObject.GetComponent<PathNode>();
            if (MouseAtNodeThis != null)
            {
                mouseLight.transform.position = MouseAtNodeThis.transform.position + new Vector3(0, -0.001f, 0);

                if (mouseAtNode != MouseAtNodeThis)
                {
                    mouseAtNode = MouseAtNodeThis;
                    updateWhenMouseOnDiffGrid = true;
                }
            }
        }

        else mouseIsNotOnNode = true;
    }

    private bool mouseIsNotOnNode = true;

    private void PlayerInput_RightClick_OpenPanel()
    {
        if (Input.GetMouseButtonDown(1))
        {
            SC.Click();
            if (mouseAtNode != null)
                if (mouseAtNode.unit != null)
                {
                    gameState = _gameState.playerInUnitPanel;
                    UI.unitPanelUI.gameObject.SetActive(true);
                    UI.InputUnitPanelUI(mouseAtNode.unit);
                    return;
                }
        }
    }
    private void PlayerInput_PlayMode()
    {
        //[Left Click]
        if (Input.GetMouseButtonDown(0))
        {
            switch (gameState)
            {
                case _gameState.playerTurn:

                    if (!isOnUi)
                    {
                        if (mouseAtNode != null && !mouseIsNotOnNode && !isOnUi)
                        {
                            if (mouseAtNode.unit == null && selectedUnit != null && selectedUnit.unitSpecialState == Unit.UnitSpecialState.normalUnit)
                            {
                                DecideUnitAction(selectedUnit, mouseAtNode);
                            }
                            else if (mouseAtNode.unit != null)
                            {
                                if (mouseAtNode.unit.unitTeam == Unit.UnitTeam.playerTeam) Select_Unit(mouseAtNode, false);

                                else if (mouseAtNode.unit.unitTeam == Unit.UnitTeam.enemyTeam || mouseAtNode.unit.unitTeam == Unit.UnitTeam.neutral)
                                {
                                    AIUnitController.selectedUnit_AI = null;
                                    if (selectedUnit != null && selectedUnit.unitSpecialState == Unit.UnitSpecialState.normalUnit)
                                        DecideUnitAction(selectedUnit, mouseAtNode);
                                }
                            }
                        }
                        else Deselect_Unit(); //×óÑ¡Ðé¿Õ
                    }
                    break;

                case _gameState.playerInSpell:
                    if (!isOnUi) UseSkill_2();
                    break;

                case _gameState.playerInRevivalPreview:
                    if (!isOnUi) UseSkill_2();
                    break;
            }
        }

        //[Right Click]
        if (Input.GetMouseButtonDown(1))
        {
            SC.Click();

            if (gameState == _gameState.playerInRevivalPanel || gameState == _gameState.playerInSpell || gameState == _gameState.playerInUnitPanel || gameState == _gameState.playerInRevivalPreview)
            {
                Debug.Log("!!!");PlayerPressEscape();return;
            }

            if (gameState == _gameState.playerTurn && !isOnUi && mouseAtNode != null && !mouseIsNotOnNode)
            {
                if (mouseAtNode.unit == null) Deselect_Unit();
                if (mouseAtNode != null && !mouseIsNotOnNode && mouseAtNode.unit != null)
                {
                    if (mouseAtNode.unit.unitTeam == Unit.UnitTeam.playerTeam) Select_Unit(mouseAtNode, false);
                    else if (mouseAtNode.unit.unitTeam == Unit.UnitTeam.enemyTeam || mouseAtNode.unit.unitTeam == Unit.UnitTeam.neutral) Deselect_Unit();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            SC.Click();
            if (gameState == _gameState.playerInUnitPanel) PlayerPressEscape();
            else if (gameState == _gameState.playerTurn && mouseAtNode != null && !mouseIsNotOnNode && !isOnUi && mouseAtNode.unit != null)
            {
                gameState = _gameState.playerInUnitPanel;
                UI.unitPanelUI.gameObject.SetActive(true);
                UI.InputUnitPanelUI(mouseAtNode.unit);
                return;
            }
        }
    }

    private void DecideUnitAction(Unit MoveUnit, PathNode targetNode)
    {
        if (MoveUnit == null) { Debug.LogWarning("Unit is null"); return; }
        if (MoveUnit.unitSpecialState != Unit.UnitSpecialState.normalUnit) return;

        //[Clear Visual Guide]
        Del_LightMap();
        Del_TargetMap();
        DeleteAllWalkingPath();

        curveRenderer.gameObject.SetActive(false);
        curveRenderer_Purple.gameObject.SetActive(false);
        attackArrow.SetActive(false);
        if (MoveUnit == null) return;
        UnitWeapon dam = MoveUnit.currentData.damage;

        if (targetNode.unit != null && targetNode.unit.unitTeam != Unit.UnitTeam.playerTeam)
        {
            if (MoveUnit.isActive)
            {//[Shoot]
                if (dam.range > 1 && targetNode.unit != null && targetNode.unit.unitTeam != Unit.UnitTeam.playerTeam && GameFunctions.CheckAttackRange(selectedUnit, targetNode.unit, dam.range, true, false, false) == true)
                {
                    StartCoroutine(GameFunctions.Attack(selectedUnit, targetNode.unit, dam, false, null, false));
                    nodeWithEnemyNear = null;
                    return;
                }

                //[Move and melee attack]
                if (nodeWithEnemyNear != null && targetNode == nodeWithEnemyNear)
                    path = PathFinding.FindPath(MoveUnit.nodeAt, nodeWithEnemyNear, MoveUnit, null, false, false, selectedUnit.currentData.movePointNow, false);
            }
            else Deselect_Unit();
        }
        
        //[Move without attack]
        else
        {
            path = PathFinding.FindPath(MoveUnit.nodeAt, nodeWillGo, MoveUnit, null, false, false, selectedUnit.currentData.movePointNow, false);
        }

        //[Move failed, Unit cannot find a path to target pathnode]
        if (path == null)
        {
            return;
        }
            
        //[Move Start]
        if (path.Count > 0)
            if (MoveUnit != null)
                moveCor = StartCoroutine(MoveUnitWithTimeDelay(MoveUnit));


        nodeWithEnemyNear = null;
    }

    private Coroutine moveCor;
    private IEnumerator MoveUnitWithTimeDelay(Unit unit_In_Moving)
    {
        //[Move Start]
        isMoving = true;
        for (int i = 1; i < path.Count; i++)
        {
            yield return new WaitForSeconds(0.3f);
            if (gameState == _gameState.gamePause || path == null) continue;
            bool canMove = unit_In_Moving.UnitPosition_CanMove(path[i], path[i].MOVE_COST, true, 1f, true);
          
            //[Check if unit conquered a campfire]
            CheckCampFire();
            if (!canMove)
            {
                isMoving = false;
                break;
            }
        }

        yield return new WaitForSeconds(0.2f);

        //[If melee attack unit moving, and meet 'Target attack unit', Attack!]
        if (shouldAttackUnit != null && unit_In_Moving.isActive)
        {
            StartCoroutine(GameFunctions.Attack(unit_In_Moving, shouldAttackUnit, unit_In_Moving.currentData.damage, true, null, false));
            shouldAttackUnit = null;
        }

        //[Move End]
        if (isMoving && unit_In_Moving != null)
        {
           
            if (enemyList != null)
                if (enemyList.Count == 0)
                {
                    bool t = true;
                    //[StoryMode CD--]
                    if (SM != null)
                    {
                        if (SM.level == StoryManager.Story_Level.level_1)
                        {
                            if (SM.story_Lv1.stage_1 == Story_Lv1.StoryStage_LV1.tut_Deselect) t = false;
                        }
                        SM.CountDownTimer--;
                    }
                    if (t)
                        unit_In_Moving.currentData.movePointNow = unit_In_Moving.currentData.movePointMax;
                }
          
            _GenerateAllNodeMap(unit_In_Moving.isActive);
        }

        isMoving = false;
        if (path != null) path.Clear();
    }


    public IEnumerator CamMove(Transform pos)
    {
        yield return new WaitForSeconds(.02f);
        if (!debug_CancelCameraMoveInNewTurn)
        {
            if (pos != null)
                CM.addPos(pos.position, true);
            CM.addZoom(3.55f, true);
        }
    }
    public void QuickCamFollow_Update(Vector3 pos)
    {
        CM.addPos(pos, true);
    }

    ////////////////////////////////////////////////////
    //CampFire//CampFire//CampFire//CampFire//CampFire//
    //CampFire//CampFire//CampFire//CampFire//CampFire//
    //CampFire//CampFire//CampFire//CampFire//CampFire//
    ////////////////////////////////////////////////////
    public void CheckCampFire()
    {
        foreach (CampFire fire in FindObjectsOfType<CampFire>())
        {
            fire.CheckControl();
        }
    }


  




    ///////////////////////////////////////////////
    //[Skill]//[Skill]//[Skill]//[Skill]//[Skill]//
    //[Skill]//[Skill]//[Skill]//[Skill]//[Skill]//
    //[Skill]//[Skill]//[Skill]//[Skill]//[Skill]//
    ///////////////////////////////////////////////

    public void UseSkill(int i)
    {
        colorfulLine.CloseSkillVisual();
        if (isAttacking || isMoving ) return;
        if (!selectedUnit.isActive && !selectedUnit.currentData.Skill[i].skill.isInstant)
        {
            selectedUnit.popTextString.Add(new textInformation("Action finished", null)); 
            return;
        }
        UI.combatPanelUI.SetSkillButtomAnimationOff();
        Del_LightMap();
        Del_TargetMap();

        attackArrow.SetActive(false);
        chargeRoad = null;
        //Sound
        SC.Click();

        if (selectedUnit.currentData.Skill[i].CD > 0) { selectedUnit.popTextString.Add(new textInformation("Skill not ready", null)); return; }

        if (selectedUnit.currentData.Skill[i].skill.Cost > FPManager.FP)
            if (selectedUnit.currentData.Skill[i].skill.type != Skill._Type.SummonFromDeath_Type2)
            { selectedUnit.popTextString.Add(new textInformation("Lack of FP", null));
                FindObjectOfType<CombatPanelUI>().ManaIcon.SetTrigger("trigger"); return; }

        UI.combatPanelUI.SetSkillButtomAnimationOn(i);
        skillNumber = i;
        
        //change state to avoid conflict
        gameState = _gameState.playerInSpell;
        spellVisual_FollowMouse.SetActive(true);
        skillInUse = selectedUnit.currentData.Skill[i];
        UI.combatPanelUI.Input_Panel_SkillIcon(skillInUse.skill.skillSprite);
        FPManager.FPInUse = skillInUse.skill.Cost;
        colorfulLine.Update_GuideMove(null);

        bool isRange = false;

        if (skillInUse.skill.range > 1) isRange = true;

        if (skillInUse.skill.isSelfUse == true)
        {
            //Debug.Log("-1");
            UseSkill_2();
            return;
        }

        switch (skillInUse.skill.type)
        {
            case Skill._Type.TargetToAttack:
                 SkillVisual_OpenTargetArea(isRange, false);
                break;

            case Skill._Type.TargetToBuff:
                SkillVisual_OpenTargetArea(isRange, true);
                break;

            case Skill._Type.TargetToDebuff:
                SkillVisual_OpenTargetArea(isRange, true);
                break;

            case Skill._Type.Reload:
                SkillVisual_OpenTargetArea(isRange, true);
                break;

            case Skill._Type.ExchangePosition:
                SkillVisual_OpenTargetArea(isRange, true);
                break;

            case Skill._Type.Teleport:
                SkillVisual_OpenTargetArea_Teleport(false);
                break;

            case Skill._Type.TargetTile:
                SkillVisual_OpenTargetArea_Teleport(false);
                break;

            case Skill._Type.BiteTheDeath:
                SkillVisual_OpenTargetArea_Death(isRange);
                break;

            case Skill._Type.SummonFromDeath:
                SkillVisual_OpenTargetArea_Death(isRange);
                break;

            case Skill._Type.SummonFromDeath_Type2:
                gameState = _gameState.playerInRevivalPreview;
                SkillVisual_OpenTargetArea_Death(isRange);
                revivalController.InputPreveiw(skillInUse.skill);
                selectedUnit.InputAnimation_Single("revival_1");
                selectedUnit.AddAnimation("revival_2");
                break;
        }
    }
  
    private void SkillVisual_OpenTargetArea(bool igoreHeight, bool igoreCorpse)//USE FOR SKILL
    {
        Unit.UnitTeam team = Unit.UnitTeam.playerTeam;
        if (skillInUse.skill.isFriendly == false) team = Unit.UnitTeam.enemyTeam;
        int range = skillInUse.skill.range;
        GameFunctions.Gene_TargetMap(GameFunctions.FindNodes_ByDistance(selectedUnit.nodeAt, range, igoreHeight), team, igoreCorpse, this);
    }

    private void SkillVisual_OpenTargetArea_Death(bool igoreHeight)//USE FOR SKILL
    {
        int range = skillInUse.skill.range;
        GameFunctions.Gene_TargetMap_Corpse(GameFunctions.FindNodes_ByDistance(selectedUnit.nodeAt, range, igoreHeight), this);
    }

    private void SkillVisual_OpenTargetArea_Teleport(bool igoreHeight)//USE FOR SKILL
    {
        int range = skillInUse.skill.range;
        GameFunctions.Gene_LightMap_Telepirt(GameFunctions.FindNodes_ByDistance(selectedUnit.nodeAt, range, igoreHeight), this);
    }

    private void UseSkill_2()
    {
        if (skillInUse.skill.isSelfUse)
        {
            //THERE ARE DIFF TYPE OF SKILLS
            if (skillInUse.skill.type == Skill._Type.TargetToBuff || skillInUse.skill.type == Skill._Type.Reload)
            {
                selectedUnit.popTextString.Add(new textInformation(skillInUse.skill.name, skillInUse.skill.skillSprite));
                StartCoroutine(SkillFunctions.Skill_Buff(selectedUnit, selectedUnit, skillInUse.skill, false));
            }

            if (skillInUse.skill.type == Skill._Type.SelfDestroy)
            {
                selectedUnit.SelfDestroy();
            }
            UseSkill_3_End(FPManager.FPInUse);
            return;
        }
        
        ////SELF USE
        if (mouseIsNotOnNode) return;
        if (skillInUse == null) return;

        int range = skillInUse.skill.range;
        bool isRange = false;
        if (range > 1) isRange = true;

        switch (skillInUse.skill.type)
        {
            #region SkillsTypeUse
            case Skill._Type.MoveInLine:
                if (chargeRoad != null)
                {
                    if (chargeRoad.Count != 0)
                    {
                        selectedUnit.popTextString.Add(new textInformation(skillInUse.skill.name, skillInUse.skill.skillSprite));
                        StartCoroutine(Skill_Charge(chargeRoad, selectedUnit, skillInUse.skill));
                    } 
                } 
                break;

            case Skill._Type.AttackALine:
                if (mouseAtNode != null)
                {
                    selectedUnit.popTextString.Add(new textInformation(skillInUse.skill.name, skillInUse.skill.skillSprite));
                    StartCoroutine(SkillFunctions.Skill_AttackALine(selectedUnit, mouseAtNode, selectedUnit.currentData.damage, skillInUse.skill));
                    UseSkill_3_End(FPManager.FPInUse);
                }
                break;

            case Skill._Type.TargetToAttack:
                if (mouseAtNode != null && mouseAtNode.unit != null && mouseAtNode.unit.unitTeam != Unit.UnitTeam.playerTeam)
                {   //TARGET TO ATTACK CAN ATTACK CORPSES
                    if (GameFunctions.CheckAttackRange(selectedUnit, mouseAtNode.unit, range, isRange, false, false))
                    {
                        if (!isRange)
                        {
                            int heightDiff = selectedUnit.nodeAt.height - mouseAtNode.height;
                            //Debug.Log(heightDiff);
                            if (heightDiff > 3 || heightDiff < -3)
                            {
                                return;
                            }

                        }
                        selectedUnit.popTextString.Add(new textInformation(skillInUse.skill.name, skillInUse.skill.skillSprite));
                        StartCoroutine(GameFunctions.Attack(selectedUnit, mouseAtNode.unit, selectedUnit.currentData.damage, true, skillInUse.skill, false));
                        UseSkill_3_End(FPManager.FPInUse);
                    }
                }
                break;

            case Skill._Type.TargetToBuff://WHEN BUFF, ALSO CHECK IF BUFF CAUSE HEAL
                if (mouseAtNode != null && mouseAtNode.unit != null && mouseAtNode.unit.unitTeam == Unit.UnitTeam.playerTeam)
                {
                    if (GameFunctions.CheckAttackRange(selectedUnit, mouseAtNode.unit, range, isRange, true, true))
                    {
                        selectedUnit.popTextString.Add(new textInformation(skillInUse.skill.name, skillInUse.skill.skillSprite));
                        StartCoroutine(SkillFunctions.Skill_Buff(selectedUnit, mouseAtNode.unit, skillInUse.skill, false));
                        UseSkill_3_End(FPManager.FPInUse);
                    }
                }
                break;

            case Skill._Type.TargetToDebuff://WHEN BUFF, ALSO CHECK IF BUFF CAUSE HEAL
                if (mouseAtNode != null && mouseAtNode.unit != null && mouseAtNode.unit.unitTeam != Unit.UnitTeam.playerTeam)
                {
                    if (GameFunctions.CheckAttackRange(selectedUnit, mouseAtNode.unit, range, isRange, true, true))
                    {
                        selectedUnit.popTextString.Add(new textInformation(skillInUse.skill.name, skillInUse.skill.skillSprite));
                        StartCoroutine(SkillFunctions.Skill_Buff(selectedUnit, mouseAtNode.unit, skillInUse.skill, false));
                        UseSkill_3_End(FPManager.FPInUse);
                    }
                }
                break;

            case Skill._Type.Reload://SAME AS BUFF
                if (mouseAtNode != null && mouseAtNode.unit != null && mouseAtNode.unit.unitTeam == Unit.UnitTeam.playerTeam)
                {
                    if (GameFunctions.CheckAttackRange(selectedUnit, mouseAtNode.unit, range, isRange, true, true))
                    {
                        selectedUnit.popTextString.Add(new textInformation(skillInUse.skill.name, skillInUse.skill.skillSprite));
                        StartCoroutine(SkillFunctions.Skill_Buff(selectedUnit, mouseAtNode.unit, skillInUse.skill, false));
                        UseSkill_3_End(FPManager.FPInUse);
                    }
                }
                break;

            case Skill._Type.ExchangePosition://SAME AS BUFF
                if (mouseAtNode != null && mouseAtNode.unit != null && mouseAtNode.unit.unitTeam == Unit.UnitTeam.playerTeam)
                {
                    if (GameFunctions.CheckAttackRange(selectedUnit, mouseAtNode.unit, range, isRange, true, true))
                    {
                        selectedUnit.popTextString.Add(new textInformation(skillInUse.skill.name, skillInUse.skill.skillSprite));
                        StartCoroutine(SkillFunctions.Skill_Exchange(selectedUnit, mouseAtNode.unit, skillInUse.skill));
                        UseSkill_3_End(FPManager.FPInUse);
                    }
                }
                break;

            case Skill._Type.TeleportToTargetAndAttackRoad:
                if (chargeRoad != null)
                {
                    if (chargeRoad[chargeRoad.Count - 1].unit == null) 
                    {
                        selectedUnit.popTextString.Add(new textInformation(skillInUse.skill.name, skillInUse.skill.skillSprite));
                        StartCoroutine(TeleportToTarget_AttackRoad(chargeRoad, selectedUnit, skillInUse.skill));
                    }
                }
                break;

            case Skill._Type.SummonFromDeath:
                if (mouseAtNode != null && mouseAtNode.unit != null && mouseAtNode.unit.unitAttribute == Unit.UnitAttribute.corpse)
                {
                    if (GameFunctions.CheckAttackRange(selectedUnit, mouseAtNode.unit, range, isRange, true, false))
                    {
                        selectedUnit.popTextString.Add(new textInformation(skillInUse.skill.name, skillInUse.skill.skillSprite));

                        gameState = _gameState.playerInRevivalPanel;

                        Unit target = mouseAtNode.unit;
                        
                        List<GameObject> myList = target.GetComponent<RevivalOptionsOnCorpse>().revivalOptions;

                        UI.revivalPanel.gameObject.SetActive(true);
                        UI.revivalPanel.InputPanel(myList, mouseAtNode);

                        selectedUnit.InputAnimation_Single("revival_1");
                        selectedUnit.AddAnimation("revival_2");
                    }
                }
                break;

            case Skill._Type.Summon:
                if (mouseAtNode != null && mouseAtNode.unit == null)
                {
                    if (GameFunctions.CheckAttackRange(selectedUnit, mouseAtNode.unit, range, isRange, true, false))
                    {
                        selectedUnit.popTextString.Add(new textInformation(skillInUse.skill.name, skillInUse.skill.skillSprite));
                        StartCoroutine(SkillFunctions.Skill_Summoning(mouseAtNode, selectedUnit, skillInUse.
                            skill.SommonBeing[Random.Range(0, skillInUse.skill.SommonBeing.Length)], skillInUse.skill, 0, skillInUse.skill.hitSpecialEffect));
                        UseSkill_3_End(FPManager.FPInUse);
                    }
                }
                break;

            case Skill._Type.Teleport:
                if (mouseAtNode != null && mouseAtNode.unit == null)
                {
                    if (GameFunctions.CheckPathRange(selectedUnit.nodeAt, mouseAtNode, range, false))
                    {
                        selectedUnit.popTextString.Add(new textInformation(skillInUse.skill.name, skillInUse.skill.skillSprite));
                        StartCoroutine(SkillFunctions.Skill_Teleport(selectedUnit, mouseAtNode, skillInUse.skill));
                        UseSkill_3_End(FPManager.FPInUse);
                    }
                }
                break;

            case Skill._Type.TargetTile:
                if (mouseAtNode != null)
                {
                    if (GameFunctions.CheckPathRange(selectedUnit.nodeAt, mouseAtNode, range, false))
                    {
                        selectedUnit.popTextString.Add(new textInformation(skillInUse.skill.name, skillInUse.skill.skillSprite));
                        StartCoroutine(SkillFunctions.Skill_TargetTile(selectedUnit, mouseAtNode, skillInUse.skill));
                        UseSkill_3_End(FPManager.FPInUse);
                    }
                }
                break;

            case Skill._Type.BiteTheDeath:
                if (mouseAtNode != null && mouseAtNode.unit != null && mouseAtNode.unit.unitAttribute == Unit.UnitAttribute.corpse)
                {
                    if (GameFunctions.CheckAttackRange(selectedUnit, mouseAtNode.unit, range, isRange, true, false))
                    {
                        selectedUnit.popTextString.Add(new textInformation(skillInUse.skill.name, skillInUse.skill.skillSprite));
                        StartCoroutine(SkillFunctions.Skill_BiteTheDeath(mouseAtNode, selectedUnit, skillInUse.skill));
                        UseSkill_3_End(FPManager.FPInUse);
                    }
                }
                break;

            case Skill._Type.SummonFromDeath_Type2:
                 
                if (GameFunctions.CheckPathRange(selectedUnit.nodeAt, mouseAtNode, range, true))
                {
                    bool canSummon = true;
                    if (mouseAtNode.unit != null) if (mouseAtNode.unit.unitAttribute != Unit.UnitAttribute.corpse) canSummon = false;
                    if (mouseAtNode.isBlocked) canSummon = false;
                    if (!canSummon)
                    {
                        selectedUnit.popTextString.Add(new textInformation("Perform Failed", null));
                        return;
                    }

                    GameObject geneOb = revivalController.GetPrefab(skillInUse.skill);
                    Unit unitGene = geneOb.GetComponent<Unit>();

                    if (unitGene.unitType == Unit.UnitSummonType.summon)
                        if (summonList.Count >= SaveData.summonMax)
                        {
                            selectedUnit.popTextString.Add(new textInformation("Max Limit", null));
                            return;
                        }

                    if (unitGene.unitType == Unit.UnitSummonType.sentry)
                        if (sentryList.Count >= SaveData.summonMax)
                        {
                            selectedUnit.popTextString.Add(new textInformation("Max Limit", null));
                            return;
                        }

                    int FP = revivalController.fleshCost;
                    if (FP > FPManager.FP)
                    {
                        selectedUnit.popTextString.Add(new textInformation("Lack of FP", null));
                        FindObjectOfType<CombatPanelUI>().ManaIcon.SetTrigger("trigger");
                        return;
                    }

                    selectedUnit.popTextString.Add(new textInformation(skillInUse.skill.name, skillInUse.skill.skillSprite));
                    
                    StartCoroutine(revivalController.Skill_RevivalSummon(mouseAtNode, selectedUnit, revivalController.GetPrefab(skillInUse.skill),
                        skillInUse.skill, Unit.UnitTeam.playerTeam, skillInUse.skill.hitSpecialEffect));

                    UseSkill_3_End(FP);
                }
                break;
                #endregion
        }
    }

    private void UseSkill_3_End(int FPCost)
    {
        colorfulLine.CloseSkillVisual();
        spellVisual_FollowMouse.SetActive(false);

        if (chargeRoad != null)
            chargeRoad.Clear();

        if (skillInUse != null)
            skillInUse.CD = skillInUse.skill.CD;

        StartCoroutine(openDetailPanelLater());

        Del_LightMap();
        Del_TargetMap();

        UI.combatPanelUI.DeleteSkill();
        UI.combatPanelUI.DisplaySkill(selectedUnit.currentData.Skill, selectedUnit);
        revivalController.ParentFolder.SetActive(false);
        FPManager.FPChange(-FPCost);
        FPManager.FPInUse = 0;

        DeleteAllWalkingPath();

        curveRenderer.gameObject.SetActive(false);
        curveRenderer_Purple.gameObject.SetActive(false);

        skillInUse = null;
        gameState = _gameState.playerTurn;
    }




    //USE FOR SPALSH DAMAGE, SO IT SHOULD NOT CALCU Self-BUFF gain
    public void Buff_Splash(Unit attacker, Unit defender, Skill skill)
    {
        StartCoroutine(SkillFunctions.Skill_Buff(attacker, defender, skill, true));
    }



    public void Attack_Splash(Unit attacker, Unit defender, Skill skill)
    {
        StartCoroutine(GameFunctions.Attack_Calculate(attacker, defender, this, attacker.currentData.damage, skill, true,true));
    }

    //when melee attack, defender will have chance to fight back.
    public void FightBack(Unit attacker, Unit defender)
    {
        isAttacking = true;
        if (defender.isActive == false) { isAttacking = false; return; }
        StartCoroutine(GameFunctions.Attack(defender, attacker, defender.currentData.damage, false, null, true));
    }
    public void RepelBack(Unit attacker, Unit defender)
    {
        isAttacking = true;
        if (defender.currentData.healthNow > 0 && attacker.currentData.healthNow > 0)
            StartCoroutine(GameFunctions.Repel(attacker, defender));
    }
    public void Attack(Unit attacker, Unit defender, UnitWeapon damage, Skill skill, bool isRange)
    {
        if (defender.currentData.healthNow > 0 && attacker.currentData.healthNow > 0)
            StartCoroutine(GameFunctions.Attack_Calculate(attacker, defender, this, damage, skill, false, isRange));
    }












    /// ////////////////////////////////////////
    //Charge //Charge //Charge //Charge //Charge 
    //Charge //Charge //Charge //Charge //Charge 
    //Charge //Charge //Charge //Charge //Charge 
    //Charge //Charge //Charge //Charge //Charge 
    //Charge //Charge //Charge //Charge //Charge 
    //Charge //Charge //Charge //Charge //Charge 
    /// ////////////////////////////////////////
    public List<PathNode> chargeRoad;

    private IEnumerator TeleportToTarget_AttackRoad(List<PathNode> gotoNode, Unit unit_In_Moving, Skill skill)
    {
      isMoving = true;
        isAttacking = true;
        if (gotoNode != null && gotoNode.Count > 1)
        {
            UnitFunctions.Flip(unit_In_Moving.transform.position.x, gotoNode[1].transform.position.x, unit_In_Moving);

            unit_In_Moving.InputAnimation(skill.animTriggerType.ToString());

            yield return new WaitForSeconds(skill.timer_PerformAction);

            if (skill.castSpecialEffect != null)
            {
                GameObject specialEffect = Instantiate(skill.castSpecialEffect, unit_In_Moving.transform.position + new Vector3(0, 0, -.1f), Quaternion.identity);
            }

            List<PathNode> myNode = gotoNode;
            bool alreadyMovingTo = false;

            for (int i = myNode.Count - 1; i > -1 ; i--)
            {
                #region FOLDERS
                if (skill.hitSpecialEffect != null)
                {
                    GameObject specialEffect = Instantiate(skill.hitSpecialEffect, myNode[i].transform.position + new Vector3(0, 0, -.1f), Quaternion.identity);
                }
                if (alreadyMovingTo)
                {
                    yield return new WaitForSeconds(0.12f);
                    if (myNode[i].unit != null && myNode[i].unit.unitTeam != unit_In_Moving.unitTeam)
                    {
                        StartCoroutine(GameFunctions.Attack_Calculate(unit_In_Moving, myNode[i].unit,this, unit_In_Moving.currentData.damage, skill, false,false));
                    }
                    continue;
                }
                if (myNode[i].unit == null && myNode[i].isBlocked == false)
                {
                    bool canMove = unit_In_Moving.UnitPosition_CanMove(myNode[i], 0, true, 1.6f,false);
                    alreadyMovingTo = true;
                }
                #endregion
            }
        }

        UseSkill_3_End(FPManager.FPInUse);
        yield return new WaitForSeconds(skill.timer_WaitAfterAttack);
        isMoving = false;
        isAttacking = false;
        if (!skill.isInstant) unit_In_Moving.UnitEnable(false);
        else Select_Unit(unit_In_Moving.nodeAt,false);
    }

    private IEnumerator Skill_Charge(List<PathNode> gotoNode, Unit unit_In_Moving, Skill skill)
    {
        isMoving = true;
        isAttacking = true;
        GameController.currentActionUnit = unit_In_Moving;

        if (gotoNode != null && gotoNode.Count > 1)
        {

            UnitFunctions.Flip(unit_In_Moving.transform.position.x, mouseAtNode.transform.position.x, unit_In_Moving);
            unit_In_Moving.InputAnimation(skill.animTriggerType.ToString());

            yield return new WaitForSeconds(skill.timer_PerformAction);
            if (skill.castSpecialEffect != null)
            {
                GameObject specialEffect = Instantiate(skill.castSpecialEffect, unit_In_Moving.transform.position + new Vector3(0, 0, -.1f), Quaternion.identity);
            }
            List<PathNode> myNode = gotoNode;

            for (int i = 1; i < myNode.Count; i++)
            {
                if (myNode[i].unit != null)
                {
                    #region FOLDERS
                    Unit unitOnWay = myNode[i].unit;
                    List<PathNode> knockBackNode1 = new List<PathNode>();
                    if (unitOnWay.unitTeam == unit_In_Moving.unitTeam) break;

                    else
                    {
                        yield return new WaitForSeconds(0.12f);
                        if (skill.hitSpecialEffect != null)
                        {
                            GameObject specialEffect = Instantiate(skill.hitSpecialEffect, UnitFunctions.GetUnitMiddlePoint(unitOnWay), Quaternion.identity);
                        }

                      // unit_In_Moving.Input_CombatPos(myNode[i].transform.position, 1f, 0.4f, 0.2f);
                        SkillFunctions.Skill_ChargeDamage(unit_In_Moving, unitOnWay, unit_In_Moving.currentData.damage, false, skill, i);
                        yield return new WaitForSeconds(0.01f);
                        bool canMove = unit_In_Moving.UnitPosition_CanMove(myNode[i], 0, true, 1.35f, false);
                        break;
                    }
                    #endregion
                }
                else
                { 
                 bool canMove = unit_In_Moving.UnitPosition_CanMove(myNode[i], 0, true, 1.35f,false);
                }
               
            }
        }

        UseSkill_3_End(FPManager.FPInUse);
        yield return new WaitForSeconds(skill.timer_WaitAfterAttack);
        isMoving = false;
        isAttacking = false;
        if (!skill.isInstant) unit_In_Moving.UnitEnable(false);
        else Select_Unit(unit_In_Moving.nodeAt,false);
    }

    public void TriggerKnockBack(Unit attacker,Unit defender,int i, float time)
    {
        bool canKnockBack = true;

        if (defender.data.unitSize != UnitData.Unit_Size.size1) canKnockBack = false;
        if (defender.unitAttribute == Unit.UnitAttribute.staticObject) canKnockBack = false;

        foreach (_Buff BUFF in defender.data.traitList)
        {
            if (BUFF.buff.cannotBeKnockBack == true) canKnockBack = false;
        }

        if (defender.unitTeam == Unit.UnitTeam.enemyTeam)
        {
            if(defender.GetComponent<UnitAI>().unitHoldingSkill) canKnockBack = false;
        }

        if (canKnockBack)
            StartCoroutine(SkillFunctions.Skill_UnitIsKnockBack_InOneLine(attacker, defender, i, time));

        else Debug.Log("Unit cannot be knock back.");
    }



    ///////////////////////////////////////////////////////////////////////
    //Draw Line    //Draw Line    //Draw Line    //Draw Line    //Draw Line    
    //Draw Line    //Draw Line    //Draw Line    //Draw Line    //Draw Line    
    //Draw Line    //Draw Line    //Draw Line    //Draw Line    //Draw Line    
    //Draw Line    //Draw Line    //Draw Line    //Draw Line    //Draw Line    
    ///////////////////////////////////////////////////////////////////////
    
    private void Drawline()
    {
        if (selectedUnit == null) return;
        int range = selectedUnit.data.damage.range;
        bool notRange = true;
        if (range > 1) notRange = false;

        if (!selectedUnit.isActive)
        {
            nodeWithEnemyNear = null;
            SetLineRender_0(false);
            return;
        }

        if (nodeWithEnemyNear == null)
        {
            if (mouseAtNode.unit != null && mouseAtNode.unit.unitTeam != selectedUnit.unitTeam && GameFunctions.CheckPathRange(selectedUnit.nodeAt, mouseAtNode, 1, false))
            {
                nodeWithEnemyNear = selectedUnit.nodeAt;
            }

            if (nodeWithEnemyNear == null)
            {
                SetLineRender_0(false);
                return;
            }
        }

        if (nodeWithEnemyNear != mouseAtNode)
        {
            UI.Display_UnitMoveUI.gameObject.SetActive(false);

            //if is on enemy
            if (mouseAtNode.unit != null && mouseAtNode.unit.unitTeam != selectedUnit.unitTeam)
            {
                Debug.Log(mouseAtNode.unit.name);
                //if is melee attack
                if (notRange && GameFunctions.CheckPathRange(nodeWithEnemyNear, mouseAtNode, 1, false) == true)
                {
                    //Debug.Log("On Arrow");
                    path = PathFinding.FindPath(selectedUnit.nodeAt, nodeWithEnemyNear, selectedUnit, null, false, false, selectedUnit.currentData.movePointNow, false);
                    if (path != null) if (path.Count > 0) SetLineRender_1(path, selectedUnit);

                   
                    //set attack arroz
                    attackArrow.SetActive(true);
                    GameFunctions.SetPos_BetweenTwoObject(attackArrow, nodeWithEnemyNear.gameObject, mouseAtNode.gameObject);
                    attackArrow.transform.GetChild(1).transform.position = attackArrow.transform.GetChild(0).position + new Vector3(0, -0.07f, 0);
                    shouldAttackUnit = mouseAtNode.unit;
                }

                //if is range attack ,disable the line. we will draw curve line in another funciton
                else if (notRange == false && GameFunctions.CheckPathRange(selectedUnit.nodeAt, mouseAtNode, range, true) == true)
                {
                    DeleteAllWalkingPath();
                }

                else SetLineRender_0(true);
            }
            else SetLineRender_0(false);
        }
        else SetLineRender_0(false);
    }

    //It will recalculate the walking path.
    private void SetLineRender_0(bool canTry)
    {
        //Debug.Log("DrawLine_0");
        UI.Display_UnitMoveUI.gameObject.SetActive(false);
        shouldAttackUnit = null;
        nodeWithEnemyNear = null;
        attackArrow.SetActive(false);
        //Unit unitAttack = null;

        path = PathFinding.FindPath(selectedUnit.nodeAt, mouseAtNode, selectedUnit, null, false, false, selectedUnit.currentData.movePointMax, false);
        if (path != null)
            if (path.Count != 0)
            {
                SetLineRender_1(path, selectedUnit);
                return;
            }

        DeleteAllWalkingPath();
        path = null;
        nodeWithEnemyNear = null;
        shouldAttackUnit = null;
        nodeWillGo = null;
        //if (canTry && mouseAtNode.unit != null && mouseAtNode.unit.unitTeam != Unit.UnitTeam.playerTeam)
        //{ 
        //    unitAttack = mouseAtNode.unit;
        //    path = PathFinding.FindPath(selectedUnit.nodeAt, mouseAtNode, selectedUnit, unitAttack, false, false, selectedUnit.currentData.movePointMax, false);
        //    if (path != null) if (path.Count != 0)
        //        {
        //            nodeWithEnemyNear = path[path.Count - 2];
        //            Drawline();
        //        } 
        //}

        //else
        //{
        //    path = PathFinding.FindPath(selectedUnit.nodeAt, mouseAtNode, selectedUnit, null, false, false, selectedUnit.currentData.movePointMax, false);
        //    if (path != null) if (path.Count != 0) SetLineRender_1(path, selectedUnit);
        //}
    }

    private void SetLineRender_1(List<PathNode> path, Unit unit)
    {
        //Debug.Log("DrawLine_1");
        DeleteAllWalkingPath();
        List<PathNode> path_ = new List<PathNode>();
        int totalMovePoint = 0;

        for (int i = 0; i < path.Count; i++)
        {
            if (i == 0)
            {
                path_.Add(path[i]);
                continue;
            }

            totalMovePoint += path[i].MOVE_COST;

            if (totalMovePoint <= unit.currentData.movePointNow)
            {
                path_.Add(path[i]);
                GameObject walkingArrow = Instantiate(Resources.Load<GameObject>("UI/WalkingLine"), this.transform.position, Quaternion.identity);
                GameFunctions.SetPos_BetweenTwoObject(walkingArrow, path[i].gameObject, path_[path_.Count - 2].gameObject);
                LineRenderer LINE = walkingArrow.GetComponent<LineRenderer>();

                Vector3 A = path_[path_.Count - 2].gameObject.transform.position;
                Vector3 B = path[i].gameObject.transform.position;
                Vector3 C = (A + B) /2;

                LINE.positionCount = 3;

                LINE.SetPosition(0, A);

                LINE.SetPosition(1, C);

                LINE.SetPosition(2, B);
            }
        }

        foreach (PathNode node in GameFunctions.FindNodes_ByDistance(path_[path_.Count - 1], 1, false))
        {
            if (node.unit != null && node.unit.unitTeam != unit.unitTeam)
            {
                PathNode myPath = path_[path_.Count - 1];
                if (myPath.unit == null)
                    nodeWithEnemyNear = path_[path_.Count - 1];
            }
        }


        Vector3[] points = new Vector3[path_.Count];
        nodeWillGo = path_[path_.Count - 1];

        for (int i1 = 0; i1 < path_.Count; i1++)
        {
            points[i1] = path[i1].gameObject.transform.position + new Vector3(0,0,0);
        }


        if (mouseAtNode.unit == null)
        {
            UI.Display_UnitMoveUI.gameObject.SetActive(true);
            int remainMP = unit.currentData.movePointNow - totalMovePoint;
            UI.Display_UnitMoveUI.InputPanel(unit, remainMP);
        }
    }
    private void CheckShoot_CurveLine()
    {
        bool canDraw = false;
        //If it is a archer and its inside attack range then draw a curve 
        if (mouseAtNode.unit != null &&
            mouseAtNode.unit.unitTeam != Unit.UnitTeam.playerTeam &&
            selectedUnit.data.damage.range > 1 &&
            selectedUnit.isActive == true &&
            GameFunctions.CheckPathRange(mouseAtNode, selectedUnit.nodeAt, selectedUnit.data.damage.range, true) == true)
            canDraw = true;

        //if (selectedUnit.damage.weaponType == _Damage._weaponType.handCannon) canDraw = false;

        if (canDraw)
        {
            DeleteAllWalkingPath();
            curveRenderer.gameObject.SetActive(true);
            GameFunctions.DrawCurve_BetweenTwoObject(selectedUnit.transform, mouseAtNode.unit.transform, curveRenderer, 0.5f);
        }
    }


    //DrawingMaps    //DrawingMaps    //DrawingMaps    //DrawingMaps    //DrawingMaps
    //DrawingMaps    //DrawingMaps    //DrawingMaps    //DrawingMaps    //DrawingMaps
    //DrawingMaps    //DrawingMaps    //DrawingMaps    //DrawingMaps    //DrawingMaps
    //DrawingMaps    //DrawingMaps    //DrawingMaps    //DrawingMaps    //DrawingMaps

    //this is for attack range, not for skill.
    private void _GenerateAllNodeMap(bool geneAttackNodes)// if a map have lightmap, it will not gene a attackrange map
    {
        if (selectedUnit == null) return;

        List<PathNode> moveableList = GameFunctions.FindNodes_ByMoveableArea(selectedUnit.currentData.movePointNow, selectedUnit.nodeAt, selectedUnit);

        moveableList.Add(selectedUnit.nodeAt);
        GameFunctions.Gene_LightMap(moveableList,this);

        if (!geneAttackNodes)return;

        if (selectedUnit.data.damage.range > 1)
        {
            List<PathNode> attackedableList = GameFunctions.FindNodes_ByDistance(selectedUnit.nodeAt, selectedUnit.data.damage.range, true);
            List<PathNode> newList = new List<PathNode>();

            for (int i = 0; i < attackedableList.Count; i++)
            {
                bool canAdd = true;
                foreach (PathNode pathCompare in moveableList)
                {
                    if (pathCompare == attackedableList[i]) canAdd = false;
                }

                if (canAdd) newList.Add(attackedableList[i]);
            }
            GameFunctions.Gene_TargetMap(newList, Unit.UnitTeam.enemyTeam, false, this);
        }

        else
        {
            List<PathNode> attackedableList = GameFunctions.FindNodes_ByCanAttack(moveableList,selectedUnit);
            GameFunctions.Gene_TargetMap(attackedableList, Unit.UnitTeam.enemyTeam, false, this);
        }
    }

    public void Del_TargetMap()
    {
        if (target_lightMaps != null && target_lightMaps.Count != 0)
        {
            foreach (GameObject ob in target_lightMaps)
            {
                if (ob != null) Destroy(ob);
            }
        }
    }

    public void Del_LightMap()
    {
        if (lightMaps != null && lightMaps.Count != 0)
        {
            foreach (GameObject ob in lightMaps)
            {
                if (ob != null) Destroy(ob);
            }
        }
    }

    private IEnumerator openDetailPanelLater()
    {
        bool_DontOpenMouseStatsPanel = true;
        yield return new WaitForSeconds(0.15f);
        bool_DontOpenMouseStatsPanel = false;
    }

}

