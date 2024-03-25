using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextTrigger : MonoBehaviour
{
    public TMP_Text text;
  
    // Update is called once per frame
    void Update()
    {
        if (GameController.playerList != null)
        {
            int num = GameController.playerList.Count - 1;
            text.text = "Summon your undead army\n( " + num + " / 3 )";
        }
    }
}
