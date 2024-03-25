using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitBarController : MonoBehaviour
{
    public BarController healthBar;
    public BarController moveBar;
    public Image healthImage, moveImage, healthBarIcon;
    public GameObject Move2, Move3, Move4;
    public GameObject Eye_Active, Eye_Disable, Skeleton;
    public Image Background;
    public void CheckMoveBarSprite(Unit Unit)
    {
        Move2.SetActive(false);
        Move3.SetActive(false);
        Move4.SetActive(false);
        if (Unit.unitAttribute != Unit.UnitAttribute.alive) return;
        if (Unit.currentData.movePointMax == 3) Move3.SetActive(true);
        if (Unit.currentData.movePointMax == 2) Move2.SetActive(true);
        if (Unit.currentData.movePointMax == 4) Move4.SetActive(true);
    }
    public void Set_Bar_Initial(Unit unit)
    {
        CheckMoveBarSprite(unit);
        moveBar.SetValue_Initial(unit.currentData.movePointNow, unit.currentData.movePointMax);
        healthBar.SetValue_Initial(unit.currentData.healthNow, unit.currentData.healthMax);

    }
    public void Set_Bar(Unit unit)
    {
        CheckMoveBarSprite(unit);
        moveBar.SetValue(unit.currentData.movePointNow, unit.currentData.movePointMax);
        healthBar.SetValue(unit.currentData.healthNow, unit.currentData.healthMax);
    }

    public void SetAsCorpse()
    {
        if (Eye_Active != null)
            Eye_Active.SetActive(false);

        if (Eye_Disable != null)
            Eye_Disable.SetActive(false);

        if (Skeleton != null)
            Skeleton.SetActive(true);

        healthImage.color = new Color32(180, 180, 180, 255);
        moveImage.color = new Color32(180, 180, 180, 255);
    }

    public void Disactive_AllIcon()
    {
        foreach (Transform child in this.transform)
        {
            if (child.name != "PopCanvas")
            {
                child.gameObject.SetActive(false);
            }
        }

        this.GetComponent<Image>().enabled = false;
    }
    public void Set_Icons(Unit.UnitTeam unitTeam, bool isActive)
    {
        if (isActive)
        {
            Eye_Active.SetActive(true);
            Eye_Disable.SetActive(false);
            Background.color = new Color32(255, 255, 255, 255);
            //unit.armorText.transform.parent.GetComponent<UnityEngine.UI.Image>().color = new Color32(0, 0, 0, 255);
            //unit.healthImage.transform.parent.GetComponent<UnityEngine.UI.Image>().color = new Color32(0, 0, 0, 255);
            //healthBarIcon.color = new Color32(255, 255, 255, 255);

            if (unitTeam == Unit.UnitTeam.playerTeam)
            {
                healthImage.color = new Color32(134, 255, 0, 255);
                healthImage.gameObject.transform.parent.parent.parent.GetComponent<Image>().color = new Color32(108, 111, 75, 255);
                moveImage.color = new Color32(158, 208, 255, 255);
            }

            if (unitTeam == Unit.UnitTeam.enemyTeam)
            {
                healthImage.color = new Color32(255, 25, 1, 255);
                healthImage.gameObject.transform.parent.parent.parent.GetComponent<Image>().color = new Color32(111, 78, 75, 255);
                moveImage.color = new Color32(158, 208, 255, 255);
            }
        }

        else
        {
            Eye_Active.SetActive(false);
            Eye_Disable.SetActive(true);
            Background.color = new Color32(255, 255, 255, 135);
            //unit.armorText.transform.parent.GetComponent<UnityEngine.UI.Image>().color = new Color32(0, 0, 0, 125);
            //unit.healthImage.transform.parent.GetComponent<UnityEngine.UI.Image>().color = new Color32(0, 0, 0, 125);
            //healthBarIcon.color = new Color32(255, 255, 255, 155);

            if (unitTeam == Unit.UnitTeam.playerTeam)
            {
                //healthImage.color = new Color32(134, 255, 0, 200);
                //moveImage.color = new Color32(158, 208, 255, 200);
            }

            if (unitTeam == Unit.UnitTeam.enemyTeam)
            {
                //healthImage.color = new Color32(255, 25, 1, 200);
                //moveImage.color = new Color32(158, 208, 255, 200);
            }
        }
    }

}
