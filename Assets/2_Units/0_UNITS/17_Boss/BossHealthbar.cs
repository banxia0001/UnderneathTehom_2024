using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthbar : MonoBehaviour
{
    public float speed = 2;
    float fillAmountShould;
    float fillAmount;

    public Image bar;
    public Unit unit;

    public GameObject eye1, eye2;
    private void Start()
    {
        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
            if (unit.currentData.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._17_BOSS)
            {
                this.unit = unit;
            }
        }
    }

    void Update()
    {
        if (unit == null) { this.gameObject.SetActive(false); return; } 
        fillAmountShould = (float)unit.currentData.healthNow / (float)unit.currentData.healthMax;
        if (fillAmount < fillAmountShould - 0.012f) fillAmount += Time.deltaTime * speed;
        if (fillAmount > fillAmountShould + 0.012f) fillAmount -= Time.deltaTime * speed;
        bar.fillAmount = fillAmount;

        if (unit.isActive) { eye1.SetActive(true); eye2.SetActive(false); }
        else { eye1.SetActive(false); eye2.SetActive(true); }
    }
}
