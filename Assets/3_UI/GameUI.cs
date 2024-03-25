using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
public class GameUI : MonoBehaviour
{
    private GameController GC;

  
    [HideInInspector] public NewDialogSystem DC;

    [Header("Friendly Unit Stats")]
    public UnitStatsPanel_V2 unitPanelUI;
    public CombatPanelUI combatPanelUI;
    public Animator loadingPanel;
 

    [Header("UIPanels")]
    public skillUI Display_Skill;
    public buffUI Display_Buff;
    public UnitMovePanel Display_UnitMoveUI;
    public GameObject Display_TermDetail;
    public GameObject StartText;
    public GameObject MouseFolder;
    public GameObject MouseSkillIcon;
    public RevivalPanel revivalPanel;
    public LearnSkillPanel learnSkillPanel;
    public Canvas canvas;

   

    [Header("GraphicRaycaster")]
    public GraphicRaycaster myRaycaster;
    public EventSystem myEventSystem;
    PointerEventData myPointerEventData;

    private void Awake()
    {
        Display_Buff.gameObject.SetActive(false);
        revivalPanel.gameObject.SetActive(false);
        DC = FindObjectOfType<NewDialogSystem>(true);
        DC.gameObject.SetActive(true);
        GC = FindObjectOfType<GameController>(true);
        loadingPanel.gameObject.SetActive(true);
    }
    public void CheckUIRaycast_StoryMode()
    {
        if (GC.storyState == GameController._storyState.story)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                DC.TriggerDialog_End();
            }
        }
    }

    public void CheckUIRaycast_GameMode()
    {
        myPointerEventData = new PointerEventData(myEventSystem);
        myPointerEventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        myRaycaster.Raycast(myPointerEventData, results);
        bool isOnSkill = false;

        UIFunctions.InputMouseFolder(canvas,MouseFolder);

        Vector3 screenPos = UIFunctions.ScreenToRectPos(Input.mousePosition, canvas);

        if (results.Count > 0)
        {
            GC.isOnUi = true;
            foreach (RaycastResult RR in results)
            {
                if (RR.gameObject.name == "back_TriggerSlotV2")
                {
                    SlotContainer_V2 cv2 = RR.gameObject.transform.parent.GetComponent<SlotContainer_V2>();
                    if (cv2.slotType == SlotContainer_V2.SlotType.skill)
                    {
                        isOnSkill = true;
                        Display_Skill.gameObject.SetActive(true); _Skill skill = cv2.skill;
                        Display_Skill.GetComponent<skillUI>().skillInput(skill.skill, cv2.targetUnitData);
                        UIFunctions.UIElement_Transform_local(screenPos + new Vector3(15, 0, 0), Display_Skill.gameObject, Display_Skill.rightOffset,true);
                        return;
                    }

                    if (cv2.slotType == SlotContainer_V2.SlotType.buff)
                    {
                        Display_Buff.gameObject.SetActive(true);
                        Display_Buff.extendBuffUIUpdate(cv2.buff, false);
                        UIFunctions.UIElement_Transform_local(screenPos + new Vector3(15, 0, 0), Display_Buff.gameObject, Display_Buff.rightOffset, true);
                        return;
                    }

                    if (cv2.slotType == SlotContainer_V2.SlotType.perk)
                    {
                        Display_Buff.gameObject.SetActive(true);
                        Display_Buff.extendBuffUIUpdate(cv2.buff, true);
                        UIFunctions.UIElement_Transform_local(screenPos + new Vector3(15, 0, 0), Display_Buff.gameObject, Display_Buff.rightOffset, true);
                        return;
                    }


                }

                //Debug.Log(RR.gameObject.name);
                //[Skill Display]
                if (GC.gameState == GameController._gameState.playerTurn || GC.gameState == GameController._gameState.playerInUnitPanel || GC.gameState == GameController._gameState.gamePause)
                    if (RR.gameObject.tag == "Skill")
                    {
                        SkillButtom bottom = RR.gameObject.transform.parent.parent.parent.GetComponent<SkillButtom>();

                        if (!bottom.isLocked)
                        {
                            isOnSkill = true;
                            Display_Skill.gameObject.SetActive(true); _Skill skill = bottom.skill;
                            Display_Skill.GetComponent<skillUI>().skillInput(skill.skill, bottom.skillUser.currentData);

                            if (bottom.skillUser != null) if (bottom.skillUser.unitTeam == Unit.UnitTeam.playerTeam) FPManager.FPInUse = skill.skill.Cost;
                            combatPanelUI.SetManaPool(GC);UIFunctions.UIElement_Transform_local(screenPos, Display_Skill.gameObject, Display_Skill.rightOffset,false);
                            return;
                        }
                    }
                UIFunctions.InputStatsExplain(RR, Display_TermDetail, screenPos);
            }
        }
        else GC.isOnUi = false;  //if no result

   
        if (!isOnSkill) //if is not on skill
        {
            if(GC.skillInUse ==null) { FPManager.FPInUse = 0; }
            else if (GC.skillInUse.skill == null) { FPManager.FPInUse = 0; }
        }
    }

    private void OpenBuffUI(GameObject RR, Vector3 screenPos)
    {
        Display_Buff.gameObject.SetActive(true);
        BuffDisplay dis = RR.gameObject.transform.parent.GetComponent<BuffDisplay>();
        Display_Buff.extendBuffUIUpdate(dis.buff, dis.isTrait);
        UIFunctions.UIElement_Transform_local(screenPos, Display_Buff.gameObject, Display_Buff.rightOffset,false);
    }
 



    public void InputUnitPanelUI(Unit TargetUnit)
    {
        unitPanelUI.InputData(TargetUnit, TargetUnit.currentData, UnitStatsPanel_V2.PanelType.openUnitInGame, true);
        Switch_UnitPanel(true);
    }


    //public void Switch_Portrait_BottomLeft(bool active)
    //{
    //    if (active) UnitPortrait.SetActive(true);
    //    else UnitPortrait.SetActive(false);
    //}

    public void Switch_UnitPanel(bool active)
    {
        if (active) unitPanelUI.gameObject.SetActive(true);
        else unitPanelUI.gameObject.SetActive(false);
    }

    public void ClosePanels()
    {
       Display_TermDetail.SetActive(false);
       Display_Buff.gameObject.SetActive(false);
       Display_Skill.gameObject.SetActive(false);
    }


}

