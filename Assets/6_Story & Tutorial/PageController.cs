using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageController : MonoBehaviour
{
    public bool haveStorySystem;
    public bool startGameAfterTutorial;
    public bool gotoNextStage = false;
    public GameObject  confirmButton;

    public List<GameObject> panel_Tutorial;
    public List<GameObject> panel_CheckMark;

    private int num;

    public void CloseTut()
    {
        if (gotoNextStage)
        {
            GameController GC = FindObjectOfType<GameController>();
            GC.SM.GoNextStage();
        }

        if (startGameAfterTutorial)
        {
            GameController GC = FindObjectOfType<GameController>();
            GC.stageController.GameStart(true,false);
        }

        this.gameObject.SetActive(false);
    }


    public void OnEnable()
    {
        if (haveStorySystem)
        {
            GameController GC = FindObjectOfType<GameController>();
            GC.gameState = GameController._gameState.gamePause;

            if (confirmButton != null)
                confirmButton.SetActive(false);

            Close_Panels();

            num = 0;
            panel_Tutorial[num].SetActive(true);
        }
    }




    public void GoNext()
    {
        Close_Panels();

        if (num < panel_Tutorial.Count - 1) num++;

        if (num == panel_Tutorial.Count - 1) confirmButton.SetActive(true);

        panel_Tutorial[num].SetActive(true);
    }

    public void GoBack()
    {
        Close_Panels();

        if (num >= 1) num--;

        panel_Tutorial[num].SetActive(true);
    }

    public void Close_Panels()
    {
        foreach (GameObject ob in panel_Tutorial)
        {
            ob.SetActive(false);
        }
    }


  
}
