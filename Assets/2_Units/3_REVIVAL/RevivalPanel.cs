using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RevivalPanel : MonoBehaviour
{
    public PathNode nodeAt;
    public TMP_Text text_FleshAmout;
    public TMP_Text text_SummonLimit;

    public List<RevivalBotton> RevivalBottons;
    public void InputPanel(List<GameObject> units, PathNode nodeAt)
    {
        this.nodeAt = nodeAt;
        GameController GC = FindObjectOfType<GameController>();
        GC.gameState = GameController._gameState.playerInRevivalPanel;
        //SaveData.CheckSummon();

       // text_FleshAmout.text = "Remain Flesh\n" + SaveData.flesh;
        //text_SummonLimit.text = "Undead Limit\n" + SaveData.summonNow + "/" + SaveData.summonMax;

        for (int i = 0; i < RevivalBottons.Count; i++)
        {
            RevivalBottons[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < units.Count; i++)
        {
            RevivalBottons[i].gameObject.SetActive(true);
            RevivalBottons[i].InputUnit(units[i]);
        }
    }

    public void QuitPanel()
    {
        GameController GC = FindObjectOfType<GameController>();
        if (GC.SM != null)
            if (GC.SM.story_Lv1 != null)
                if (GC.SM.story_Lv1.stage_1 == Story_Lv1.StoryStage_LV1.tut_InRevivalPanel) return;

        GC.PlayerPressEscape();
        
        this.gameObject.SetActive(false);
    }
}
