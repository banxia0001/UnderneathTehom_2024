using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptObject/MyNewDialog")]
public class NewDialog : ScriptableObject
{
    public List<NewDialogContent> NewDialogContent;
}

[System.Serializable]
public class NewDialogContent
{
    [TextArea(4, 10)]
    public string text;
    public AudioClip audio;
    public enum Speaker { narrator, nameless_one, shrunken_head, rat, skeleton, nameless_WannaFight, nameless_Shock, nameless_Sad, nameless_DownEat,
    nameless_Awake, nameless_DownEat_Shock, nameless_DownEat_WannaFight, nameless_DownEat_Sad
    }
    public Speaker speaker;

    public enum ContinueTrigger { continueDialog, endDialog, endGame, endBattle }
    public ContinueTrigger continueTrigger;

    public enum EventTrigger 
    { 
        none, 
        avatarGerDown_n_Eat, gain_Flesh_n_Health, avatarFinishEat,
        avatar_GetDown_2, avatar_Full, avatar_Think, avatar_Idea,
        
        rockFall_0, event_RatCall, rockFall_GameEnd,
        caveShake,caveShake2,
        allSkeletonCheer,

        allUnitRestedInCamp,

        spawnRatRaid_1, spawnRatRaid_2, spawnRatRaid_3,caveShake_New,
        bossSpawn,

    }
    public EventTrigger eventTrigger;


    public enum CamLookAt { player, transform_0, transform_1, transform_2 , transform_3, transform_4, transform_5, transform_6, transform_7,transform_8, transform_9 }
    public CamLookAt camLookAt;
}