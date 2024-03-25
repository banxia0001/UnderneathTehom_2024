using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatUniClickabletIcon : MonoBehaviour
{
    public GameObject por_Nameless, por_Skel_Archer, por_Skel_GreatS, por_Skel_Shield, por_Zombie_Rat, por_Lancer;


    
    [HideInInspector]
    public Unit unit;
    public GameObject outline;
    public void InputFriendUnit(Unit SelectedUnit)
    {
        if (SelectedUnit == null || SelectedUnit.name == null)
        {
            this.gameObject.SetActive(false);
            return;
        }
        unit = SelectedUnit;

        por_Nameless.SetActive(false);
        por_Skel_Archer.SetActive(false);
        por_Skel_Shield.SetActive(false);
        por_Skel_GreatS.SetActive(false);
        por_Zombie_Rat.SetActive(false);
        por_Lancer.SetActive(false);

        if (SelectedUnit.data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._1_Avatar) { por_Nameless.SetActive(true); CheckColor(por_Nameless.GetComponent<Image>()); }
        if (SelectedUnit.data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._4_Skel_Shield) { por_Skel_Shield.SetActive(true); CheckColor(por_Skel_Shield.GetComponent<Image>()); }
        if (SelectedUnit.data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._5_Skel_GreatSword) { por_Skel_GreatS.SetActive(true); CheckColor(por_Skel_GreatS.GetComponent<Image>()); }
        if (SelectedUnit.data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._6_Skel_Archer) { por_Skel_Archer.SetActive(true); CheckColor(por_Skel_Archer.GetComponent<Image>()); }
        if (SelectedUnit.data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._10_Rat_Zombie) { por_Zombie_Rat.SetActive(true); CheckColor(por_Zombie_Rat.GetComponent<Image>()); }
        if (SelectedUnit.data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._3_Skel_Lance) { por_Lancer.SetActive(true); CheckColor(por_Lancer.GetComponent<Image>()); }

        //MoveText.text = SelectedUnit.currentData.movePointNow.ToString();
        //if (SelectedUnit.isActive) { activeIcon.SetActive(true); activeIcon2.SetActive(false); }
        //else { activeIcon.SetActive(false); activeIcon2.SetActive(true); }
    }

    private void Awake()
    {
        outline.SetActive(false);
    }
    public void CheckIfActive(Unit activeUnit)
    {
        if (activeUnit == null) { outline.SetActive(false); return; }
        if (unit == activeUnit)
        {
            outline.SetActive(true);
        }
        else outline.SetActive(false); ;
    }

    private void CheckColor(Image image)
    {
        if (unit.isActive)
        {
            image.color = new Color32(255, 255, 255, 255);  
        }
        else image.color = new Color32(255, 255, 255, 75);

    }

    public void SelectUnit()
    {
        if (unit == null) return;
        GameController GC = FindObjectOfType<GameController>();
        GC.Select_Unit(unit.nodeAt,true);
    }
}
