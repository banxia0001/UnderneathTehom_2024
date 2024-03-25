using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorpseSpecialFunction : MonoBehaviour
{
    public GameObject corpseGuide;
    public Unit corpseUnit;

    public void Start()
    {
        corpseGuide.SetActive(false);
        StoryManager SM = FindObjectOfType<StoryManager>();
        if (SM != null)
        {
            SM.story_Lv1.specialCorpse = this;
        }
    }
}
