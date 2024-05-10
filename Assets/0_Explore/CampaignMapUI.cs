using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CampaignMapUI : MonoBehaviour
{
    public static bool notShowingFloatngPanels = false;
    public bool isOnUI;
    [Header("UIPanels")]
    
    public skillUI Display_Skill;
    public buffUI Display_Buff;
    public GameObject Display_TermDetail, MouseFolder;
    public Canvas canvas;

    public Animator loadingPanel;
    [Header("GraphicRaycaster")]
    public GraphicRaycaster myRaycaster;
    public EventSystem myEventSystem;
    PointerEventData myPointerEventData;

    private void Start()
    {
        Display_Buff.gameObject.SetActive(false);
        loadingPanel.gameObject.SetActive(true);
        loadingPanel.SetBool("Trigger", false);
    }
    private void Update()
    {
        CloseUIPanels();
        CheckUIRaycast_GameMode();
    }

    private void CloseUIPanels()
    {
        Display_TermDetail.SetActive(false);
        Display_Buff.gameObject.SetActive(false);
        Display_Skill.gameObject.SetActive(false);
    }
    private void CheckUIRaycast_GameMode()
    {
        isOnUI = false;
        myPointerEventData = new PointerEventData(myEventSystem);
        myPointerEventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        myRaycaster.Raycast(myPointerEventData, results);

        //UIFunctions.InputMouseFolder(canvas, MouseFolder);

        Vector3 screenPos = UIFunctions.ScreenToRectPos(Input.mousePosition, canvas);

        if (results.Count > 0)
        {
            isOnUI = true;
            foreach (RaycastResult RR in results)
            {
                //Debug.Log(RR.gameObject.name);
                if (RR.gameObject.name == "back_TriggerSlotV2" && !notShowingFloatngPanels)
                {
                    SlotContainer_V2 cv2 = RR.gameObject.transform.parent.GetComponent<SlotContainer_V2>();
                    if (cv2.slotType == SlotContainer_V2.SlotType.skill)
                    {
                        Display_Skill.gameObject.SetActive(true); _Skill skill = cv2.skill;
                        Display_Skill.GetComponent<skillUI>().skillInput(skill.skill, cv2.targetUnitData);
                        UIFunctions.UIElement_Transform_local(screenPos + new Vector3(50, 0,0), Display_Skill.gameObject, Display_Skill.rightOffset, true);
                        return;
                    }

                    if (cv2.slotType == SlotContainer_V2.SlotType.buff)
                    {
                        Display_Buff.gameObject.SetActive(true);
                        Display_Buff.extendBuffUIUpdate(cv2.buff, false);
                        UIFunctions.UIElement_Transform_local(screenPos + new Vector3(50, 0, 0), Display_Buff.gameObject, Display_Buff.rightOffset, true);
                        return;
                    }

                    if (cv2.slotType == SlotContainer_V2.SlotType.perk)
                    {
                        Display_Buff.gameObject.SetActive(true);
                        Display_Buff.extendBuffUIUpdate(cv2.buff, true);
                        UIFunctions.UIElement_Transform_local(screenPos + new Vector3(50, 0, 0), Display_Buff.gameObject, Display_Buff.rightOffset, true);
                        return;
                    }
                }
                //[If raycast found some text with explains, display termDetail panel.]
                UIFunctions.InputStatsExplain(RR, Display_TermDetail, screenPos);
            }
        }
        else isOnUI = false;  //if no result
    }
}
