using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class StartGame : MonoBehaviour
{

    public enum State { startScene, inCutScene, inDebugMode, inGameLoading }
    public State state;

    public Animator loadingPanel, startCutsceneAnim;
    public GameObject mlight;
    public List<Animator> textOb;

    public AudioSource Click,Scream, BGM, TriggerSelection,SmallClick, scream2,scream3,scream4,scream5,reward,hell;

    [Header("DebugMode")]
    public GameObject lightArea1;
    public GameObject lightArea2;
    public GameObject area1,area2;

    public Animator animCanvas;
    public enum OnGame {none, onLV1,onBoss}
    public OnGame onGame;

    public bool trailerMode = false;
    private void Start()
    {
        area1.SetActive(false);
        area2.SetActive(false);
        state = State.startScene;
        loadingPanel.SetBool("Trigger", false);
        foreach (Animator ob in textOb)
        {
            ob.gameObject.SetActive(false);
        }
    }

    public void Update()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mlight.transform.position = pos;

        if (state == State.inDebugMode)
        {
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, LayerMask.GetMask("Ground"));

            if (hit.collider != null)
            {
                if (hit.collider.name == "Area1")
                {
                    if (onGame != OnGame.onLV1) reward.Play();
                    onGame = OnGame.onLV1;
                }
                if (hit.collider.name == "Area2")
                {
                    if (onGame != OnGame.onBoss) reward.Play();
                    onGame = OnGame.onBoss;
                }
            }
            else onGame = OnGame.none;

            if (onGame == OnGame.onLV1) { lightArea1.SetActive(true); lightArea2.SetActive(false); }
            else if (onGame == OnGame.onBoss) { lightArea1.SetActive(false); lightArea2.SetActive(true); }
            else { lightArea1.SetActive(false); lightArea2.SetActive(false); }

            if (Input.GetMouseButtonDown(0))
            {
                if (onGame != OnGame.none)
                {
                    state = State.inGameLoading;
                    Click.Play();
                    StartCoroutine(StartGame_Select());
                }
                else SmallClick.Play();
            }
        }

        else
        {
            if (Input.GetMouseButtonDown(0)) 
            {
                SmallClick.Play();
                if (state == State.startScene) WorldSlowlyGoDown();
            } 
        }
      
    }

    public void WorldSlowlyGoDown()
    {
        if (state != State.startScene) return;
        Click.Play();
        state = State.inCutScene;
        startCutsceneAnim.SetTrigger("Trigger");
    }

    public void Start_PlaceDialogLines()
    {
        StartCoroutine(PlaceDialogLines()); 
    }

    public IEnumerator PlaceDialogLines()
    {
        animCanvas.SetTrigger("trigger");
        startCutsceneAnim.SetTrigger("A1");
        yield return new WaitForSeconds(0.5f);
     
        BGM.Play();
        hell.Play();
        textOb[0].gameObject.SetActive(true);
      
        yield return new WaitForSeconds(2f);
        scream2.Play(); 
        yield return new WaitForSeconds(1f); 
        scream3.Play(); 
        yield return new WaitForSeconds(1f);
        //textOb[0].SetTrigger("Disable");

      
        textOb[1].gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        startCutsceneAnim.SetTrigger("Trigger");
        yield return new WaitForSeconds(1f);
        //textOb[1].SetTrigger("Disable");

     
        textOb[2].gameObject.SetActive(true);
        scream4.Play();
        yield return new WaitForSeconds(1f);
        scream5.Play(); 
        yield return new WaitForSeconds(2f);
        startCutsceneAnim.SetTrigger("Trigger");
        yield return new WaitForSeconds(1f);

        textOb[3].gameObject.SetActive(true);
        yield return new WaitForSeconds(4f); 

        
        Scream.Play();
        textOb[4].gameObject.SetActive(true);
        yield return new WaitForSeconds(3.5f);
        yield return new WaitForSeconds(0.5f);
        

        yield return new WaitForSeconds(1f);
      
        if (trailerMode)
        {
            startCutsceneAnim.SetTrigger("Trigger4");
        }
        else startCutsceneAnim.SetTrigger("Trigger2");

        yield return new WaitForSeconds(2.25f);
        textOb[0].SetTrigger("Disable");
        yield return new WaitForSeconds(0.1f); textOb[1].SetTrigger("Disable");
        yield return new WaitForSeconds(0.1f); textOb[2].SetTrigger("Disable");
        yield return new WaitForSeconds(0.1f); textOb[3].SetTrigger("Disable");
        yield return new WaitForSeconds(0.1f); textOb[4].SetTrigger("Disable");

        //yield return new WaitForSeconds(3.5f);
        //startCutsceneAnim.SetTrigger("Trigger4");
    }

    public void StartSelectionDebugMode()
    {
        TriggerSelection.Play();
        state = State.inDebugMode;
        area1.SetActive(true);
        area2.SetActive(true);
    }

    public IEnumerator StartGame_Select()
    {
        loadingPanel.SetBool("Trigger", true);
        if (onGame == OnGame.onLV1)
        {
            startCutsceneAnim.SetTrigger("Trigger3");
            SaveData.firstEnterComat_LV_1_1 = true;
            yield return new WaitForSeconds(1.2f);
            SceneManager.LoadScene(1);
        }

        if (onGame == OnGame.onBoss)
        {
            SaveData.firstEnterComat_LV_1_1 = false;
            SaveData.quickStart_FightBoss = true;
            yield return new WaitForSeconds(1.2f);
            SceneManager.LoadScene(2);
        }
    }


    public void ExitMyGame()
    {
        Application.Quit();
    }
}
