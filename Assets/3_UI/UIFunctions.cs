using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UIFunctions : MonoBehaviour
{
    public static void InputStatsExplain(RaycastResult RR, GameObject Display_TermDetail, Vector3 screenPos)
    {
        //[If raycast found some text with explains, display termDetail panel.]
        if (RR.gameObject.name == "Text_Damage") UIFunctions.EXTEND_DISPLAY_Update("Text_Damage", Display_TermDetail, screenPos);
        if (RR.gameObject.name == "Text_Range") UIFunctions.EXTEND_DISPLAY_Update("Text_Range", Display_TermDetail, screenPos);
        if (RR.gameObject.name == "Text_Hitrate") UIFunctions.EXTEND_DISPLAY_Update("Text_Hitrate", Display_TermDetail, screenPos);
        if (RR.gameObject.name == "Text_Amr") UIFunctions.EXTEND_DISPLAY_Update("Text_Amr", Display_TermDetail, screenPos);
        if (RR.gameObject.name == "Text_AmrSunder") UIFunctions.EXTEND_DISPLAY_Update("Text_AmrSunder", Display_TermDetail, screenPos);
        if (RR.gameObject.name == "Text_Dodge") UIFunctions.EXTEND_DISPLAY_Update("Text_Dodge", Display_TermDetail, screenPos);
        if (RR.gameObject.name == "Text_Power") UIFunctions.EXTEND_DISPLAY_Update("Text_Power", Display_TermDetail, screenPos);
        if (RR.gameObject.name == "Text_AmrMg") UIFunctions.EXTEND_DISPLAY_Update("Text_AmrMg", Display_TermDetail, screenPos);
        if (RR.gameObject.name == "Text_AmrSunder") UIFunctions.EXTEND_DISPLAY_Update("Text_AmrSunder", Display_TermDetail, screenPos);
        if (RR.gameObject.name == "Text_MgSunder") UIFunctions.EXTEND_DISPLAY_Update("Text_MgSunder", Display_TermDetail, screenPos);
        if (RR.gameObject.name == "Text_Move") UIFunctions.EXTEND_DISPLAY_Update("Text_Move", Display_TermDetail, screenPos);
        if (RR.gameObject.name == "Text_Nbr") UIFunctions.EXTEND_DISPLAY_Update("Text_Nbr", Display_TermDetail, screenPos);
        if (RR.gameObject.name == "Text_MR") UIFunctions.EXTEND_DISPLAY_Update("Text_MR", Display_TermDetail, screenPos);
        if (RR.gameObject.name == "Text_SR") UIFunctions.EXTEND_DISPLAY_Update("Text_SR", Display_TermDetail, screenPos);
        if (RR.gameObject.name == "Text_Health") UIFunctions.EXTEND_DISPLAY_Update("Text_Health", Display_TermDetail, screenPos);
        if (RR.gameObject.name == "Text_Flesh") UIFunctions.EXTEND_DISPLAY_Update("Text_Flesh", Display_TermDetail, screenPos);
    }
    public static void InputMouseFolder(Canvas canvas, GameObject MouseFolder)
    {
        Vector3 screenPos = UIFunctions.ScreenToRectPos(Input.mousePosition, canvas);
        UIFunctions.UIElement_Transform_local(screenPos, MouseFolder.gameObject, -180, false); ;
    }
    public static Vector2 ScreenToRectPos(Vector2 screen_pos, Canvas canvas)
    {
        if (canvas.renderMode != RenderMode.ScreenSpaceOverlay && canvas.worldCamera != null)
        {
            Vector2 anchorPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.gameObject.GetComponent<RectTransform>(), screen_pos, canvas.worldCamera, out anchorPos);
            return anchorPos;
        }
        else
        {
            //Canvas is in Overlay mode
            Vector2 anchorPos = screen_pos - new Vector2(canvas.gameObject.GetComponent<RectTransform>().position.x, canvas.gameObject.GetComponent<RectTransform>().position.y);
            anchorPos = new Vector2(anchorPos.x / canvas.gameObject.GetComponent<RectTransform>().lossyScale.x, anchorPos.y / canvas.gameObject.GetComponent<RectTransform>().lossyScale.y);
            return anchorPos;
        }
    }
    public static void UIElement_Transform_local(Vector3 toPos, GameObject UIElement, float offset, bool countY)
    {
        UIElement.gameObject.SetActive(true);
        UIElement.GetComponent<RectTransform>().transform.localPosition = toPos;

        float offset_X = 0;
        float offset_Y = 0;
        if (toPos.x > 700) { offset_X = offset; }
        if (toPos.y > 100 && countY)
        {
            offset_Y = - toPos.y;
        } 

        UIElement.transform.GetChild(0).GetComponent<RectTransform>().transform.localPosition = new Vector3(offset_X, offset_Y, 0);
    }



    public static void EXTEND_DISPLAY_Update(string input, GameObject EXTEND_DISPLAY, Vector3 toPos)
    {
        EXTEND_DISPLAY.gameObject.SetActive(true);
        UIElement_Transform_local(toPos, EXTEND_DISPLAY, -170, true);

        Display_TermDetail DT = EXTEND_DISPLAY.GetComponent<Display_TermDetail>();
        TMP_Text NAME = DT.text_T;
        TMP_Text TEXT = DT.text_D;

        if (input == "Text_Flesh")
        {
            NAME.text = "Flesh";
            TEXT.text = "Represents the remaining Flesh inside. After being attacked, units may drop Flesh \n";
        }

        if (input == "Text_Health")
        {
            NAME.text = "Health";
            TEXT.text = "Represents the ability to take damage. Health is often difficult to recover, remember to protect vulnerable units from losing health.\n";
        }

        if (input == "Text_Damage")
        {
            NAME.text = "Physical Damage";
            TEXT.text = "Represents the physical damage of this unit.\n";
        }

        if (input == "Text_Nbr")
        {
            NAME.text = "Number of Attack";
            TEXT.text = "Represents the number unit will attack in single attack action.\n";
        }

        if (input == "Text_Range")
        {
            NAME.text = "Range";
            TEXT.text = "Represents the range of physical damage.\n";
        }

        if (input == "Text_Hitrate")
        {
            NAME.text = "Accurate";
            TEXT.text = "Represents the chance to hit the target unit.\n";
        }
       
        if (input == "Text_Amr")
        {
            NAME.text = "Armor";
            TEXT.text = "Represents the protection. Each point of armor reduces incoming damage.\n";
        }

        if (input == "Text_AmrSunder")
        {
            NAME.text = "Armor Sunder";
            TEXT.text = "Represents ability to destroy enemy armor.\n";
        }

        if (input == "Text_Dodge")
        {
            NAME.text = "Dodge";
            TEXT.text = "Represents the ability to avoid upcoming attack.\n";
        }

        if (input == "Text_Power")
        {
            NAME.text = "Power";
            TEXT.text = "Represents the willpower of the unit.\nThis will affect the unit's ability to cast magic spells.\n";
        }

        if (input == "Text_MR")
        {
            NAME.text = "Magic Armor";
            TEXT.text = "Represents the magic protection of the unit. Each magic armor will reduce one coming magic damage.\n";
        }

        if (input == "Text_MgSunder")
        {
            NAME.text = "Magic Armor Sunder";
            TEXT.text = "Represents the ability to sunder target unit's magic armor.\n";
        }

        if (input == "Text_Move")
        {
            NAME.text = "Move";
            TEXT.text = "Represents the Mobility of the unit.\n";
        }

        if (input == "Text_SR")
        {
            NAME.text = "Resist";
            TEXT.text = "Represents the chance the unit can avoid a debuff or recover from a debuff.";
        }
    }



    //public static void Gene_RevivalPanel(GameObject canvas)
    //{
    //    if (canvas == null) return;

    //    GameObject panel = Resources.Load<GameObject>("UI/Revival_ButtonFolder");
      
    //    GameObject panel_G = Instantiate(panel, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
    //    panel_G.transform.SetParent(canvas.transform);
    //    panel_G.GetComponent<RectTransform>().localEulerAngles = new Vector3(0, 0, 0);
    //    panel_G.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
    //    panel_G.GetComponent<RectTransform>().localScale = new Vector3(15, 15, 15);
    //}

    public static void Gene_PopText(GameObject canvas, textInformation text)
    {
        if (canvas == null) return;

        GameObject textOb = Resources.Load<GameObject>("UI/PopText_1");
        if (text.sprite != null)
        {
            Sprite sprite = text.sprite;
            textOb = Resources.Load<GameObject>("UI/PopText_1");
        }

        GameObject go = Instantiate(textOb, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        go.transform.SetParent(canvas.transform);

        go.GetComponent<RectTransform>().localEulerAngles = new Vector3(0, 0, 0);
        go.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        go.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);

        go.transform.GetChild(0).GetChild(0).transform.GetChild(2).GetComponent<TMP_Text>().text = text.text;

        if (text.sprite != null)
        {
            go.gameObject.transform.GetChild(0).GetChild(0).transform.GetChild(1).GetComponent<Image>().sprite = text.sprite;
        }
    }
}
