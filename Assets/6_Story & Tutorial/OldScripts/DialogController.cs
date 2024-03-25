using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogController : MonoBehaviour
{
    public GameObject transformTarget;

    public GameObject por_Book;
    public GameObject por_Rat;

    private List<ChoiceContent> choices_InUse;
    private List<string> choices_Clicked_String;
    public List<Dialog> choices_Clicked;
    private List<GameObject> choicesChoosed_Background;
    public TMP_Text DialogDisplayText;

    [HideInInspector] public Animator anim;

    public void Awake()
    {
        DialogDisplayText.text = "";
        choices_InUse = new List<ChoiceContent>();
        choices_Clicked = new List<Dialog>();
        choices_Clicked_String = new List<string>();
        choicesChoosed_Background = new List<GameObject>();
        anim = this.gameObject.GetComponent<Animator>();
        anim.SetBool("Open", false);
    }

    private void OnDisable()
    {
        Debug.Log("!");
    }
    public void InputDialog(Dialog dialog)
    {
        //[Set_Up]
        choices_InUse = new List<ChoiceContent>();
        choicesChoosed_Background = new List<GameObject>();


        for (var i = transformTarget.transform.childCount -1; i >= 0; i--)
        {
            if (transformTarget.transform.GetChild(i).gameObject.name != "Dialog_Content(Fixed)")
                GameObject.DestroyImmediate(transformTarget.transform.GetChild(i).gameObject);
        }

        por_Book.SetActive(false);
        por_Rat.SetActive(false);
        
        DialogContent content_Por = dialog.paragraphContents[0];
        if (content_Por.speaker == DialogContent.Speaker.narrator)
        {
            por_Book.SetActive(true);
        }
        if (content_Por.speaker == DialogContent.Speaker.rat)
        {
            por_Rat.SetActive(true);
        }

        //[Loop though all the paragraph]
        for (int i = 0; i < dialog.paragraphContents.Count; i++)
        {
            //[Input]
            DialogContent content = dialog.paragraphContents[i];
            string contextText = "\n\n" + GetDialogSpeakerName(content.speaker) + GetDialogFromString(content.text);
            DialogDisplayText.text += contextText;
        }

        //[Loop though all initial choices]
        for (int i = 0; i < dialog.choiceContents_0.Count; i++)
        {
            bool canPrintChoice = true;
            bool changeTGrey = false;

            if (dialog.choiceContents_0[i] != null)
                if (choices_Clicked_String != null)
                    if (choices_Clicked_String.Count != 0)
                    {
                        foreach (string st in choices_Clicked_String)
                        {
                            if (st == dialog.choiceContents_0[i].text) changeTGrey = true;
                        }
                    }

            //Check if can print
            if (dialog.choiceContents_0[i] != null)
                if (dialog.choiceContents_0[i].dialogueToUnlockThisChoice != null)
                    if (dialog.choiceContents_0[i].dialogueToUnlockThisChoice.Count != 0)
                    {
                        canPrintChoice = false;

                        //if the choice need some dialgue clicked.
                        if (choices_Clicked != null)
                        {
                            if (choices_Clicked.Count != 0)
                            {
                                List<bool> myBools = new List<bool>();

                                foreach (Dialog dia in dialog.choiceContents_0[i].dialogueToUnlockThisChoice)
                                {
                                    bool pass = false;
                                    foreach (Dialog dia_clicked in choices_Clicked)
                                    {
                                        if (dia == dia_clicked) pass = true;
                                    }
                                    myBools.Add(pass);
                                }

                                bool canPass2 = true;
                                foreach (bool canB in myBools)
                                {
                                    //Debug.Log(canB);
                                    if (canB == false) canPass2 = false;
                                }

                                canPrintChoice = canPass2;
                            }
                        }      
                    }

            if (canPrintChoice)
            {
                //[Generate Object]
                GameObject choiceContent = Instantiate(Resources.Load<GameObject>("UI/Dialogue/Dialog_Choice"), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                choiceContent.transform.SetParent(transformTarget.transform);
                choiceContent.transform.localScale = new Vector3(1, 1, 1);

                TMP_Text dialogContentT = choiceContent.gameObject.transform.GetChild(1).GetComponent<TMP_Text>();
                int num = i + 1;
                dialogContentT.gameObject.name = "DialogChoice_" + num;

                if (changeTGrey) dialogContentT.color = Color.gray;
                //[Input]
                ChoiceContent content = dialog.choiceContents_0[i];
                choices_InUse.Add(content);
                choicesChoosed_Background.Add(choiceContent.transform.GetChild(0).gameObject);

                dialogContentT.text = num + ". " + GetDialogFromString(content.text);

            }
        }
    }
   



    public void MouseOnDialogChoice(string name)
    {
        for (int i = 0; i < choicesChoosed_Background.Count; i++)
        {
            choicesChoosed_Background[i].SetActive(false);
        }

        if (name != "None")
        {
            choicesChoosed_Background[GetDialogChoiceIntFromString(name)].SetActive(true);
        }
    }
    public void InputDialogChoice(string name)
    {

        StoryManager SM = FindObjectOfType<StoryManager>();

        ChoiceContent content = choices_InUse[GetDialogChoiceIntFromString(name)];
       
        //Add it to clickable list
        choices_Clicked.Add(content.nextDialog);
        choices_Clicked_String.Add(content.text);


        //Cam
   

        if (content.camLookAt == ChoiceContent.CamLookAt.player)
        {
            CamUpdate(AIFunctions.FindPlayer().gameObject.transform.position);
        }
        else
        {
            if (content.camLookAt == ChoiceContent.CamLookAt.transform_0)
            {
                CamUpdate(SM.story_lookatPoint[0].gameObject.transform.position);
            }

            if (content.camLookAt == ChoiceContent.CamLookAt.transform_1)
            {
                CamUpdate(SM.story_lookatPoint[1].gameObject.transform.position);
            }

            if (content.camLookAt == ChoiceContent.CamLookAt.transform_2)
            {
                CamUpdate(SM.story_lookatPoint[2].gameObject.transform.position);
            }
        }

        string contextText = "\n\n" + GetDialogSpeakerName(DialogContent.Speaker.nameless_one) + GetDialogFromString(content.text);
        DialogDisplayText.text += contextText;



        //[Anim Trigger]

        if (content.animTrigger == ChoiceContent.AnimTrigger.rockFall_0)
        {
            SM.GridsFall(0);
        }

        if (content.animTrigger == ChoiceContent.AnimTrigger.event_RatCall)
        {
            SM.Event_RatCall();
        }

        if (content.animTrigger == ChoiceContent.AnimTrigger.avatarGerDown_n_Eat)
        {
          
            SM.Event_GetDown_n_Eat();
        }

        if (content.animTrigger == ChoiceContent.AnimTrigger.gain_Flesh_n_Health)
        {
            SM.Event_Eating();
        }

        if (content.animTrigger == ChoiceContent.AnimTrigger.avatarFinishEat)
        {
            SM.Event_FinishEating();
        }

        if (content.animTrigger == ChoiceContent.AnimTrigger.rockFall_GameEnd)
        {
            SM.GridsFall(1);
            SM.GridsFall(2);
        }
       

         GameObject block = Instantiate(Resources.Load<GameObject>("UI/Dialogue/Dialog_Block"), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        block.transform.SetParent(transformTarget.transform);
        block.transform.localScale = new Vector3(1, 1, 1);


        //[Event Trigger]
        if (content.eventTrigger == ChoiceContent.EventTrigger.continueDialog)
        {
            if (content.nextDialog != null) InputDialog(content.nextDialog);
            else Debug.LogError("Missing Attached Dialog.");
        }

        if (content.eventTrigger == ChoiceContent.EventTrigger.endDialog)
        {
            anim.SetBool("Open", false);
            DialogDisplayText.text += "\n\n\n";

            SM.GoNextStage();
            StartCoroutine(CloseThis());
        }

        if (content.eventTrigger == ChoiceContent.EventTrigger.endGame)
        {
            Application.Quit();
        }
    }

    public static string GetDialogSpeakerName(DialogContent.Speaker speaker)
    {
        switch (speaker)
        {
            case DialogContent.Speaker.narrator:
                   return "<color=#353535>Narrator: </color>";

            case DialogContent.Speaker.nameless_one:
                return "<color=#380F36>Nameless One: </color>";

            case DialogContent.Speaker.shrunken_head:
                return "<color=#6A2525>Shrunken Head: </color>";

            case DialogContent.Speaker.rat:
                return "<color=#FF0000>Rat: </color>";

        }

        return null;
    }

    public static string GetDialogFromString(string context)
    {
        string context2 = context.Replace("\r","");
 
        string newString = "";
        for (int i = 0; i < context2.Length; i++)
        {
            if (context2[i].ToString() == "[")
                newString += "¡°<color=#000000>";

            else if (context2[i].ToString() == "]")
                newString += "¡±</color>";

            else
                newString += context2[i].ToString();
        }
        return newString;
    }


    public void CamUpdate(Vector3 v)
    {
        CamMove cm = FindObjectOfType<CamMove>();
        StartCoroutine(cm.ChangeCamSlowMove(0, true));

        v += new Vector3(0, -0.45f, 0);
        //Cam
        cm.gameObject.transform.position = v;
    }
    public static int GetDialogChoiceIntFromString(string context)
    {
        if (context == "DialogChoice_1") return 0;
        if (context == "DialogChoice_2") return 1;
        if (context == "DialogChoice_3") return 2;
        if (context == "DialogChoice_4") return 3;
        if (context == "DialogChoice_5") return 4;
        return 0;
    }

    private IEnumerator CloseThis()
    {
        yield return new WaitForSeconds(1f);
        this.GetComponent<Animator>().SetBool("Open", false);
    }
}



