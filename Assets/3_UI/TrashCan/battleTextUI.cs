using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class battleTextUI : MonoBehaviour
{
    private Unit unitCurrent = null;
    private GameController GC = null;
    private Animator anim;
    public TMP_Text showText;
    private void Start()
    {
        GC = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        anim = GetComponent<Animator>();
        //showText.text = "";
    }
    void Update()
    {
        if (GC.gameState == GameController._gameState.playerTurn || GC.gameState == GameController._gameState.playerInSpell)
        {
            showText.text = "";
            return;
         }

       if (GC.AIUnitController.selectedUnit_AI != null) 
        {
            if (unitCurrent != null && unitCurrent != GC.AIUnitController.selectedUnit_AI)
            {
                anim.SetTrigger("enlarger");
            }

            unitCurrent = GC.AIUnitController.selectedUnit_AI;
            showText.text = "<color=#F84444>" + unitCurrent.currentData.Name + " is Moving..." + "</color>";
        }
    }
}
