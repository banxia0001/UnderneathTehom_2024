using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tut_UpdateText : MonoBehaviour
{
    // Start is called before the first frame update
    public TMP_Text text;
    void Update()
    {
        text.text = "Collect Flesh Drop (" + FPManager.FP + "/4)";
    }
}
