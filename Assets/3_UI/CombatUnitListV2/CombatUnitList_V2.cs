    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatUnitList_V2 : MonoBehaviour
{
    public GameObject heroIcon;
    public GameObject sentryIcon;
    public GameObject summonIcon;
    public GameObject portrait;

    public Transform statsFolder;
    private void DestroyOld()
    {
        foreach (Transform child in statsFolder.transform)
        {
            if (child.name == "Icon") Destroy(child.gameObject);
            if (child.name == "Portrait") Destroy(child.gameObject);
        }
    }

    public void SpawnIcons()
    {
        DestroyOld();

        if (GameController.heroList != null && GameController.heroList.Count != 0)
        {
            GameObject icon_0 = Instantiate(heroIcon, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            icon_0.transform.SetParent(statsFolder.transform);
            icon_0.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = GameController.heroList.Count + "/" + SaveData.heroMax;
            icon_0.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);
            icon_0.name = "Icon";
            icon_0.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);

            foreach (Unit myUnit in GameController.heroList)
            {
                GameObject unit = Instantiate(portrait, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                unit.transform.SetParent(statsFolder.transform);
                unit.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);
                unit.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
                unit.transform.GetComponent<ClickablePortrait>().GetUnit(myUnit);
                unit.name = "Portrait";
            }
        }

        if (GameController.sentryList != null && GameController.sentryList.Count != 0)
        {

            GameObject icon_0 = Instantiate(sentryIcon, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            icon_0.transform.SetParent(statsFolder.transform);
            icon_0.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = GameController.sentryList.Count + "/" + SaveData.sentryMax;
            icon_0.name = "Icon";
            icon_0.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);
            icon_0.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);

            foreach (Unit myUnit in GameController.sentryList)
            {
                GameObject unit = Instantiate(portrait, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                unit.transform.SetParent(statsFolder.transform);
                unit.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);
                unit.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
                unit.transform.GetComponent<ClickablePortrait>().GetUnit(myUnit);
                unit.name = "Portrait";
            }
        }

        if (GameController.summonList != null && GameController.summonList.Count != 0)
        {

            GameObject icon_0 = Instantiate(summonIcon, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            icon_0.transform.SetParent(statsFolder.transform);
            icon_0.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = GameController.summonList.Count + "/" + SaveData.summonMax;
            icon_0.name = "Icon";
            icon_0.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);
            icon_0.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);  

            foreach (Unit myUnit in GameController.summonList)
            {
                GameObject unit = Instantiate(portrait, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                unit.transform.SetParent(statsFolder.transform);
                unit.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
                unit.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);
                unit.transform.GetComponent<ClickablePortrait>().GetUnit(myUnit);
                unit.name = "Portrait";
            }
        }
    }


}
