using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MouseStats_Ver3 : MonoBehaviour
{
    public PortraitManager PM;
    private Unit unitInHand;
    public Transform statsFolder;

    [Header("Prefabs")]
    public GameObject spanwperk;
    public GameObject spanwLine;
    public GameObject stats_healTower;
    public GameObject stats_enemyBoneFire;
    public GameObject stats_normalUnit;
    public GameObject note;

    public GameObject sentry_HealT;
    public GameObject sentry_Turret;
    public GameObject sentry_Turret_2;
    public GameObject sentry_FBlock;

    public UnitMouseUI UMI;



    [Header("Stats")]
    public TMP_Text unitName, health;
    private void DestroyOld()
    {
        foreach (Transform child in statsFolder.transform)
        {
            if (child.name == "Buff") Destroy(child.gameObject);
            if (child.name == "Perk") Destroy(child.gameObject);
            if (child.name == "Line") Destroy(child.gameObject);
            if (child.name == "Note") Destroy(child.gameObject);
            if (child.name == "Stats") Destroy(child.gameObject);
        }
    }

    public void InputStats(Unit unitInput, Unit unitSelected)
    {
        UMI.gameObject.SetActive(false);

        if (unitSelected != null)
        {
            if (unitInput.unitTeam != Unit.UnitTeam.playerTeam)
            {
                if (unitSelected.unitSpecialState == Unit.UnitSpecialState.normalUnit)
                {
                    UMI.gameObject.SetActive(true);
                    UMI.InputPanel(unitSelected, unitInput);
                }
            }
        }

        bool canInput = false;
        if (unitInHand == null) canInput = true;
        else if (unitInput != unitInHand) canInput = true;
        if (!canInput) return;

        
        DestroyOld();
        unitInHand = unitInput;
        unitName.text = unitInput.currentData.Name;
        health.text = unitInput.currentData.healthNow + "/" + unitInput.currentData.healthMax;
        PM.UpdatePortrait(unitInput.data);
        SpawnStats(unitInHand);
    }

    private void SpawnStats(Unit currentUnit)
    {

        if (currentUnit.unitSpecialState == Unit.UnitSpecialState.healTower)
        {
            GameObject stats = Instantiate(stats_healTower, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            stats.transform.SetParent(statsFolder.transform);
            stats.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            stats.name = "Stats";
            stats.GetComponent<MouseStats_BasesStats>().InputStats(unitInHand, 2);
        }

        else if (currentUnit.unitSpecialState == Unit.UnitSpecialState.boneFireTower)
        {
            GameObject stats = Instantiate(stats_enemyBoneFire, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            stats.transform.SetParent(statsFolder.transform);
            stats.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            stats.name = "Stats";
            stats.GetComponent<MouseStats_BasesStats>().InputStats(unitInHand, 3);
        }

        else
        {
            GameObject stats = Instantiate(stats_normalUnit, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            stats.transform.SetParent(statsFolder.transform);
            stats.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            stats.name = "Stats";
            stats.GetComponent<MouseStats_BasesStats>().InputStats(unitInHand,1);
        }


        //SpeicalStates

        if (currentUnit.unitSpecialState == Unit.UnitSpecialState.healTower)
        {
            GameObject stats = Instantiate(sentry_HealT, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            stats.transform.SetParent(statsFolder.transform);
            stats.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            stats.name = "Stats";
        }
        if (currentUnit.unitSpecialState == Unit.UnitSpecialState.machineGun)
        {
            if (currentUnit.GetComponent<AutoMachineGun_Controller>().type_Pene)
            {
                GameObject stats = Instantiate(sentry_Turret_2, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                stats.transform.SetParent(statsFolder.transform);
                stats.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
                stats.name = "Stats";
            }
            else
            {
                GameObject stats = Instantiate(sentry_Turret, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                stats.transform.SetParent(statsFolder.transform);
                stats.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
                stats.name = "Stats";

            }
        }
        if (currentUnit.unitSpecialState == Unit.UnitSpecialState.block)
        {
            GameObject stats = Instantiate(sentry_FBlock, new Vector3(0, 0, 0), Quaternion.identity)as GameObject;
            stats.transform.SetParent(statsFolder.transform);
            stats.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            stats.name = "Stats";
        }





        //Buff

        if (currentUnit.data.traitList != null && currentUnit.data.traitList.Count != 0)
        {
            GameObject line = Instantiate(spanwLine, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            line.transform.SetParent(statsFolder.transform);
            line.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            line.name = "Line";

            foreach (_Buff buff in currentUnit.data.traitList)
            {
                GameObject perk = Instantiate(spanwperk, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                perk.transform.SetParent(statsFolder.transform);
                perk.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);

                perk.GetComponent<MouseStats_Buff_V3>().Input_Buff(buff, false);
                perk.name = "Perk";
            }
        }

        if (currentUnit.buffList != null && currentUnit.buffList.Count != 0)
        {
            GameObject line = Instantiate(spanwLine, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            line.transform.SetParent(statsFolder.transform);
            line.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            line.name = "Line";

            foreach (_Buff buff in currentUnit.buffList)
            {
                GameObject perk = Instantiate(spanwperk, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                perk.transform.SetParent(statsFolder.transform);
                perk.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
                perk.GetComponent<MouseStats_Buff_V3>().Input_Buff(buff, true);
                perk.name = "Perk";
            }
        }

        GameObject note = Instantiate(this.note, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        note.transform.SetParent(statsFolder.transform);
        note.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        note.name = "Note";
    }
}
