using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ListPanelUI : MonoBehaviour
{
    public GameObject slotHolderPrefab, slotHolderPrefab_Revival, slotHolder_Servant_Prefab, slotHolderPrefab_Large, slotHolderPrefab_Mid, slotHolderPrefab_Exlarge;
    public GameObject geneFolder;
    public GameObject blockLinePrefab, servantBlockLine;

    public enum SlotInfo { minor, revival,large }

    public void InputBlockLine(string text)
    {
        GameObject icon_0 = Instantiate(blockLinePrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        icon_0.transform.SetParent(geneFolder.transform);
        icon_0.name = "Child";
        icon_0.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);
        icon_0.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        icon_0.transform.GetChild(1).GetComponent<TMP_Text>().text = text;
    }
    public void InputBlockLine_Servant()
    {
        GameObject icon_0 = Instantiate(servantBlockLine, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        icon_0.transform.SetParent(geneFolder.transform);
        icon_0.name = "Child";
        icon_0.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);
        icon_0.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
    }

    public void InputServant(GameObject perfab, UnitData unit, bool isDisplayedInRewardPanel, string description)
    {
        GameObject icon_0 = Instantiate(slotHolder_Servant_Prefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        icon_0.transform.SetParent(geneFolder.transform);
        icon_0.name = "Child";
        icon_0.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);
        icon_0.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        icon_0.GetComponent<SlotContainer_V2>().InputServant(perfab, unit, isDisplayedInRewardPanel, description);
    }
    public void InputSkill(_Skill skill, UnitData data, bool isDisplayedInReward)
    {
        GameObject prefab = null;
        if (skill.skill.slotInfo == SlotInfo.revival) prefab = slotHolderPrefab_Revival;
        else prefab = getFitHolder(skill.skill.description.Length);

        GameObject icon_0 = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;   
        icon_0.transform.SetParent(geneFolder.transform);
        icon_0.name = "Child";

        GameObject OB = null;
        if (skill.skill.type == Skill._Type.SummonFromDeath_Type2)
        {
            OB = skill.skill.revivalPrefab;
        }
        icon_0.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);
        icon_0.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        icon_0.GetComponent<SlotContainer_V2>().InputSkill(skill, data, isDisplayedInReward, OB);
    }

    public void InputBuff(_Buff buff, UnitData data, bool isDisplayedInReward)
    {
        GameObject prefab = getFitHolder(buff.buff.description.Length);

        GameObject icon_0 = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        icon_0.transform.SetParent(geneFolder.transform);
        icon_0.name = "Child";
        icon_0.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);
        icon_0.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        icon_0.GetComponent<SlotContainer_V2>().InputBuff(buff, false, data, isDisplayedInReward);
    }

    public void InputPerk(_Buff buff, UnitData data, bool isDisplayedInReward)
    {
        GameObject prefab = getFitHolder(buff.buff.description.Length);

        GameObject icon_0 = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        icon_0.transform.SetParent(geneFolder.transform);
        icon_0.name = "Child";
        icon_0.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);
        icon_0.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        icon_0.GetComponent<SlotContainer_V2>().InputBuff(buff, true, data, isDisplayedInReward);
    }
    public void ClearField()
    {
        foreach (Transform child in geneFolder.transform)
        {
            if (child.name == "Child") Destroy(child.gameObject);
        }
    }

    private GameObject getFitHolder(int Length)
    {
        if (Length > 140) return slotHolderPrefab_Exlarge;
        if (Length > 70) return slotHolderPrefab_Large;
        if (Length > 40) return slotHolderPrefab_Mid;
        return slotHolderPrefab;
    }
}
