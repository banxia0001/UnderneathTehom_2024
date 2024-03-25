using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageController_V2 : MonoBehaviour
{
    public bool gotoNextStage = false;
    public bool haveStorySystem;
    public bool startGameAfterTutorial;
    public Color32 color_grey, color_yellow;
    public List<GameObject> pages;
    public List<Image> pages_Icon; 
    private int num;
    public GameObject closeButton;
    public void OnEnable()
    {
        if (haveStorySystem)
        {
            GameController GC = FindObjectOfType<GameController>();
            GC.gameState = GameController._gameState.gamePause;
            Close_Panels();
            num = 0;
            gotoPage(num);
        }
    }
    public void Close_Panels()
    {
        foreach (GameObject ob in pages)
        {
            ob.SetActive(false);
        }

        foreach (Image ob in pages_Icon)
        {
            ob.color = color_grey;
        }
    }
    public void gotoPage(int pageNum)
    {
        if (pageNum == pages.Count - 1)
        { closeButton.SetActive(true); }
        else closeButton.SetActive(false);

        Close_Panels();
        pages[pageNum].SetActive(true);
        pages_Icon[pageNum].color = color_yellow;
    }

    public void GoNext()
    {
        if (num < pages.Count - 1) num++;
        gotoPage(num);
    }

    public void GoBack()
    {
        if (num >= 1) num--;
        gotoPage(num);
    }

    public void CloseTut()
    {
        GameController GC = FindObjectOfType<GameController>();
        if (gotoNextStage)
        {
            GC.SM.GoNextStage();
        }
        if (startGameAfterTutorial)
        {
            GC.stageController.GameStart(true,false);
        }
        this.gameObject.SetActive(false);
    }
}
