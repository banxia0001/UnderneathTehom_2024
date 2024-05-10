using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewDialogSystem : MonoBehaviour
{
    public TMP_Text speakerName, speakerText;
    [HideInInspector] public Animator anim;
    StoryManager SM;
    private GameController GC;
    public GameObject TitleBack;

    public GameObject por_Book;
    public GameObject por_Rat;
    public GameObject por_Avatar;
    public GameObject por_Undead;

    public NewDialog dialogInHand;
    public int dialogNum;


    public Sprite nameless_Idle, nameless_Awake, nameless_WannaFight, nameless_FinishEat, nameless_Shock, nameless_Sads, nameless_FinishEat_Sad;
    public Sprite nameless_FinishEat_WannaFight, nameless_FinishEat_Shock;

    private bool inWaiting;
    private bool inWaiting2;

    public AudioSource audioSource;
    public void Awake()
    {
        SM = FindObjectOfType<StoryManager>();
        GC = FindObjectOfType<GameController>();
        speakerText.text = "";
        speakerName.text = "";
        anim = this.gameObject.GetComponent<Animator>();
        anim.SetBool("Open", false);

        por_Book.SetActive(false);
        por_Avatar.SetActive(false);
        por_Rat.SetActive(false);
        por_Undead.SetActive(false);

        inWaiting = false;
        inWaiting2 = false;
    }

    public void InputDialog(NewDialog dialog)
    {
        por_Book.SetActive(false);
        por_Avatar.SetActive(false);
        por_Rat.SetActive(false);

        dialogNum = 0;
        this.dialogInHand = dialog;
        TriggerDialog_Start();
    }

    public void CamUpdate(Vector3 v)
    {
        CamMove cm = FindObjectOfType<CamMove>();
        StartCoroutine(cm.ChangeCamSlowMove(0, true));
        v += new Vector3(0, -0.275f, 0);
        cm.addPos(v, true);
    }


    public IEnumerator ClickToNextStory()
    {
        inWaiting = true;
        yield return new WaitForSeconds(0.1f);
        speakerText.text = "";
        speakerName.text = "";
        TriggerDialog_Start();
        yield return new WaitForSeconds(0.2f);
        inWaiting = false;
    }

    public void GetDialogSpeaker(NewDialogContent.Speaker speaker)
    {
        por_Book.SetActive(false);
        por_Avatar.SetActive(false);
      
        por_Rat.SetActive(false);
        por_Undead.SetActive(false);
        TitleBack.SetActive(true);
        //speakerName.gameObject.transform.parent.gameObject.SetActive(true);

        switch (speaker)
        {
            case NewDialogContent.Speaker.narrator:
                TitleBack.SetActive(false);
                //speakerName.gameObject.transform.parent.gameObject.SetActive(false);
                por_Book.SetActive(true);
                //speakerName.text = "Narrator";
                break;

            case NewDialogContent.Speaker.nameless_one:
                speakerName.text = "Nameless One";
                por_Avatar.SetActive(true);
                por_Avatar.GetComponent<Image>().sprite = nameless_Idle;
                break;

            case NewDialogContent.Speaker.nameless_DownEat:
                speakerName.text = "Nameless One";
                por_Avatar.SetActive(true);
                por_Avatar.GetComponent<Image>().sprite = nameless_FinishEat;
                break;

            case NewDialogContent.Speaker.nameless_Sad:
                speakerName.text = "Nameless One";
                por_Avatar.SetActive(true);
                por_Avatar.GetComponent<Image>().sprite = nameless_Sads;
                break;

            case NewDialogContent.Speaker.nameless_Shock:
                speakerName.text = "Nameless One";
                por_Avatar.SetActive(true);
                por_Avatar.GetComponent<Image>().sprite = nameless_Shock;
                break;

            case NewDialogContent.Speaker.nameless_WannaFight:
                speakerName.text = "Nameless One";
                por_Avatar.SetActive(true);
                por_Avatar.GetComponent<Image>().sprite = nameless_WannaFight;
                break;

            case NewDialogContent.Speaker.nameless_Awake:
                speakerName.text = "Nameless One";
                por_Avatar.SetActive(true);
                por_Avatar.GetComponent<Image>().sprite = nameless_Awake;
                break;

            case NewDialogContent.Speaker.nameless_DownEat_WannaFight:
                speakerName.text = "Nameless One";
                por_Avatar.SetActive(true);
                por_Avatar.GetComponent<Image>().sprite = nameless_FinishEat_WannaFight;
                break;

            case NewDialogContent.Speaker.nameless_DownEat_Shock:
                speakerName.text = "Nameless One";
                por_Avatar.SetActive(true);
                por_Avatar.GetComponent<Image>().sprite = nameless_FinishEat_Shock;
                break;

            case NewDialogContent.Speaker.nameless_DownEat_Sad:
                speakerName.text = "Nameless One";
                por_Avatar.SetActive(true);
                por_Avatar.GetComponent<Image>().sprite = nameless_FinishEat_Sad;
                break;

            case NewDialogContent.Speaker.shrunken_head:
                //speakerName.text = "Shrunken Head";
                //por_Book.SetActive(true);
                break;

            case NewDialogContent.Speaker.rat:
                por_Rat.SetActive(true);
                speakerName.text = "The Rat";
                break;

            case NewDialogContent.Speaker.skeleton:
                por_Undead.SetActive(true);
                speakerName.text = "Undead Legion";
                break;

        }
    }
    public void TriggerDialog_Start()
    {
        NewDialogContent content = dialogInHand.NewDialogContent[dialogNum];
        GetDialogSpeaker(content.speaker);

        speakerText.text = content.text;
        audioSource.clip = null;
        audioSource.clip = content.audio;
        if (audioSource.clip != null)
        { audioSource.Play(); }

        switch (content.camLookAt)
        {
            case NewDialogContent.CamLookAt.player:
                CamUpdate(AIFunctions.FindPlayer().gameObject.transform.position);
                break;

            case NewDialogContent.CamLookAt.transform_0:
                CamUpdate(SM.story_lookatPoint[0].gameObject.transform.position);
                break;

            case NewDialogContent.CamLookAt.transform_1:
                CamUpdate(SM.story_lookatPoint[1].gameObject.transform.position);
                break;

            case NewDialogContent.CamLookAt.transform_2:
                CamUpdate(SM.story_lookatPoint[2].gameObject.transform.position);
                break;

            case NewDialogContent.CamLookAt.transform_3:
                CamUpdate(SM.story_lookatPoint[3].gameObject.transform.position);
                break;

            case NewDialogContent.CamLookAt.transform_4:
                CamUpdate(SM.story_lookatPoint[4].gameObject.transform.position);
                break;

            case NewDialogContent.CamLookAt.transform_5:
                CamUpdate(SM.story_lookatPoint[5].gameObject.transform.position);
                break;

            case NewDialogContent.CamLookAt.transform_6:
                CamUpdate(SM.story_lookatPoint[6].gameObject.transform.position);
                break;

            case NewDialogContent.CamLookAt.transform_7:
                CamUpdate(SM.story_lookatPoint[7].gameObject.transform.position);
                break;

            case NewDialogContent.CamLookAt.transform_8:
                CamUpdate(SM.story_lookatPoint[8].gameObject.transform.position);
                break;

            case NewDialogContent.CamLookAt.transform_9:
                CamUpdate(SM.story_lookatPoint[9].gameObject.transform.position);
                break;
        }

        switch (content.eventTrigger)
        {
            case NewDialogContent.EventTrigger.rockFall_0:
                SM.GridsFall(0);
                break;

            case NewDialogContent.EventTrigger.caveShake:
                SM.Event_CaveShake(1);
                break;

            case NewDialogContent.EventTrigger.caveShake2:
                SM.Event_CaveShake(2);
                break;

            case NewDialogContent.EventTrigger.event_RatCall:
                SM.Event_RatCall();
                break;

            case NewDialogContent.EventTrigger.avatarGerDown_n_Eat:
                SM.Event_GetDown_n_Eat();
                break;

            case NewDialogContent.EventTrigger.gain_Flesh_n_Health:
                SM.Event_Eating();
                break;

            case NewDialogContent.EventTrigger.avatarFinishEat:
                SM.Event_FinishEating();
                break;

            case NewDialogContent.EventTrigger.rockFall_GameEnd:
                SM.GridsFall(1);
                SM.GridsFall(2);
                break;

            case NewDialogContent.EventTrigger.allSkeletonCheer:
                SM.Event_UndeadUnitCheer();
                break;

            case NewDialogContent.EventTrigger.avatar_Idea:
                SM.Event_ThinkingOfSolution();
                break;

            case NewDialogContent.EventTrigger.avatar_Full:
                SM.Event_Full();
                break;

            case NewDialogContent.EventTrigger.avatar_GetDown_2:
                SM.Event_GetDown();
                break;

            case NewDialogContent.EventTrigger.avatar_Think:
                SM.Event_Thinking();
                break;

            case NewDialogContent.EventTrigger.allUnitRestedInCamp:
                SM.Event_AllRecover();
                break;

            case NewDialogContent.EventTrigger.spawnRatRaid_1:
                SM.Event_SpawnEnemy(0);
                break;

            case NewDialogContent.EventTrigger.spawnRatRaid_2:
                SM.Event_SpawnEnemy(1);
                break;

            case NewDialogContent.EventTrigger.spawnRatRaid_3:
                SM.Event_SpawnEnemy(2);
                break;

            case NewDialogContent.EventTrigger.caveShake_New:
                FindObjectOfType<GridFallController>().StartFalling();
                break;

            case NewDialogContent.EventTrigger.bossSpawn:
                StartCoroutine(WaitForBoss());
               
                break;
        }
    }

    public void TriggerDialog_End()
    {
        if (inWaiting) return;
        if (inWaiting2) return;
      
        if (GameController.frozenGame) return;
        NewDialogContent content = dialogInHand.NewDialogContent[dialogNum];

        switch (content.continueTrigger)
        {
            case NewDialogContent.ContinueTrigger.continueDialog:
                dialogNum++;
                StartCoroutine( ClickToNextStory());
                break;

            case NewDialogContent.ContinueTrigger.endGame:
                por_Book.SetActive(false);
                por_Avatar.SetActive(false);
                por_Rat.SetActive(false);

                Application.Quit();
                break;

            case NewDialogContent.ContinueTrigger.endDialog:

                por_Book.SetActive(false);
                por_Avatar.SetActive(false);
                por_Rat.SetActive(false);

                StartCoroutine(CloseThis());
                GC.can_MoveCam = true;
              
                GC.storyState = GameController._storyState.setup;
                SM.GoNextStage();
                break;

            case NewDialogContent.ContinueTrigger.endBattle:
                por_Book.SetActive(false);
                por_Avatar.SetActive(false);
                por_Rat.SetActive(false);
                GC.PauseGame();
                GC.saveloadSys.EndBattle();
                break;
        }
    }
    private IEnumerator CloseThis()
    {
        GC.can_MoveCam = false;
        yield return new WaitForSeconds(0.1f);
        this.GetComponent<Animator>().SetBool("Open", false);

        CamMove MOVE = FindObjectOfType<CamMove>();
        MOVE.addZoom(3.4f,true);
        yield return new WaitForSeconds(0.35f);
        GC.can_MoveCam = true;

        por_Book.SetActive(false);
        por_Avatar.SetActive(false);
        por_Rat.SetActive(false);
        por_Undead.SetActive(false);
    }

    private IEnumerator WaitForBoss()
    {
        inWaiting2 = true;
        FindObjectOfType<SFX_Controller>().InputVFX_Boss(12);
        yield return new WaitForSeconds(0.35f);
        FindObjectOfType<SM_LV2_2>().boss.gameObject.SetActive(true);

        yield return new WaitForSeconds(3.85f);
        inWaiting2 = false;
    }

}
