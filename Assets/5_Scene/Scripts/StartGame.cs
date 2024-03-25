using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class StartGame : MonoBehaviour
{
    private bool startGame = false;
    public Animator loadingPanel;
    private void Start()
    {
        startGame = false;
        loadingPanel.SetBool("Trigger", false);
    }
    public void StartMyGame()
    {
        if (startGame == true) return;
        startGame = true;
        SaveData.firstEnterComat_LV_1_1 = true;
        StartCoroutine(StartMyGame_2());
    }
    public IEnumerator StartMyGame_2()
    {
        startGame = true;
        loadingPanel.SetBool("Trigger", true);
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(1);
    }
    public void ExitMyGame()
    {
        Application.Quit();
    }
}
