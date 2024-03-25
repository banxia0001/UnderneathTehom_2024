using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PrintLog : MonoBehaviour
{
    private string words;
    private bool isPrinting = false;
    public float charsPerSecond = 0.01f;
    private float timer;
    private int currentPos = 0;
    public TMP_Text displayText;
    void Update()
    {
        PrintWords();
    }
    public void TransferPrinting(string stringToAdd)
    {
        currentPos = 0;
        words = stringToAdd;
        displayText.text = "";
        isPrinting = true;
    }
    void PrintWords()
    {
        if (isPrinting)
        {
            timer += Time.deltaTime;
            if (timer >= charsPerSecond)
            {
                timer = 0;
                currentPos++;
                displayText.text = words.Substring(0, currentPos);

                if (currentPos >= words.Length)
                {
                    OnFinish();
                }
            }

        }
    }
    void OnFinish()
    {
        isPrinting = false;
        timer = 0;
        displayText.text = words;
    }
}
