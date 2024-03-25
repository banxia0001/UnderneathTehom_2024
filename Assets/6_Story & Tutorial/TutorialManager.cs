using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public List<PageController> allPanels;
    public List<PageController_V2> allPanels_v2;

    private void Start()
    {
        Close_Panels();
    }

    public void Close_Panels()
    {
        foreach (PageController ob in allPanels)
        {
            foreach (GameObject obs in ob.panel_Tutorial)
            {
                if (obs == null) { Debug.Log(ob.name); continue; }
                obs.SetActive(false);
            } 
            ob.gameObject.SetActive(false);
        }

        foreach (PageController_V2 ob in allPanels_v2)
        {
            foreach (GameObject obs in ob.pages)
            {
                if (obs == null) { Debug.Log(ob.name); continue; }
                obs.SetActive(false);
            }
            ob.gameObject.SetActive(false);
        }
    }
}
