using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class RevivalBotton : MonoBehaviour
{
    public GameObject unitInHand;

    public int flesh;
    public TMP_Text text_Flesh, text_Name, t_d,t_a,t_h,t_m;

    public GameObject por_Shield, por_GreatSword, por_Archer, por_Spear, por_ZombieRat;

    public void InputUnit(GameObject unitInHand)
    {
        this.unitInHand = unitInHand;
        Unit unit = unitInHand.GetComponent<Unit>();
        por_Shield.SetActive(false);
        por_GreatSword.SetActive(false);
        por_Archer.SetActive(false);
        por_Spear.SetActive(false);
        por_ZombieRat.SetActive(false);

        int dam = (unit.data.damage.damMax + unit.data.damage.damMin) / 2 + unit.data.damage.damBonus;
        t_d.text = dam.ToString();
        t_h.text = unit.data.healthMax.ToString();
        t_a.text = unit.data.armorMax.ToString();
        t_m.text = unit.data.movePointMax.ToString();
        text_Name.text = unit.data.Name;

        if (unit.data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._4_Skel_Shield) { por_Shield.SetActive(true); flesh = 15; }
        if (unit.data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._5_Skel_GreatSword) { por_GreatSword.SetActive(true); flesh = 15; }
        if (unit.data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._6_Skel_Archer) { por_Archer.SetActive(true); flesh = 15; }
        if (unit.data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._3_Skel_Lance) { por_Spear.SetActive(true); flesh = 15; }
        if (unit.data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._10_Rat_Zombie) { por_ZombieRat.SetActive(true); flesh = 7; }

        text_Flesh.text = flesh.ToString();
    }

    public void InputUnitDataToUIPanel()
    {
        GameUI UI = FindObjectOfType<GameUI>();
        UI.unitPanelUI.gameObject.SetActive(true);
        UI.unitPanelUI.InputData(unitInHand.GetComponent<Unit>(),null ,UnitStatsPanel_V2.PanelType.openUnitInPrefab,true);
    }

    public void BuyUnit()
    {
        GameController GC = FindObjectOfType<GameController>();
        RevivalPanel RP = FindObjectOfType<RevivalPanel>();

        if (GC.storyState != GameController._storyState.game) return;

        if (SaveData.flesh < flesh)
        {
            GameObject textOb = Resources.Load<GameObject>("UI/PopText_1");
            GameObject go = Instantiate(textOb, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            go.transform.SetParent(this.transform);

            go.GetComponent<RectTransform>().localEulerAngles = new Vector3(0, 0, 0);
            go.GetComponent<RectTransform>().localPosition = new Vector3(417, 0, 0);
            go.GetComponent<RectTransform>().localScale = new Vector3(0.05f, 0.05f, 0.05f);
            go.transform.GetChild(0).GetChild(0).transform.GetChild(2).GetComponent<TMP_Text>().text = "Lack of Flesh";
            return;
        }

        SaveData.flesh -= flesh;
        //GC.UseSkill_SummonUndead_FromRevivalPanel(RP.nodeAt, unitInHand);
    }
}
