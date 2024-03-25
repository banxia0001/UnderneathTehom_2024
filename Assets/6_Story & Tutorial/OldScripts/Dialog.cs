using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptObject/Dialog")]
public class Dialog : ScriptableObject
{
    [Header("Paragraphs")]
    public List<DialogContent> paragraphContents;

    [Header("Initial Choices")]
    public List<ChoiceContent> choiceContents_0;

}


[System.Serializable]
public class DialogContent
{
    public enum Speaker { narrator, nameless_one, shrunken_head, rat }
    public Speaker speaker;

    [TextArea(10, 10)]
    public string text;
}

[System.Serializable]
public class ChoiceContent
{
    public enum EventTrigger { continueDialog, endDialog, endGame }
    public EventTrigger eventTrigger;


    public enum AnimTrigger { none, avatarGerDown_n_Eat, gain_Flesh_n_Health, avatarFinishEat, rockFall_0, event_RatCall, rockFall_GameEnd }
    public AnimTrigger animTrigger;


    public enum CamLookAt { player, transform_0, transform_1, transform_2 }
    public CamLookAt camLookAt;

    [TextArea(2, 10)]
    public string text;
    public Dialog nextDialog;
    public List<Dialog> dialogueToUnlockThisChoice;
}



