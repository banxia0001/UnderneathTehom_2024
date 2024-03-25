using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryFuncitons : MonoBehaviour
{
    public static void StageControl_SetTimer(StoryManager SM, float time)
    {
        SM.CountDownTimer_Float = time;
        SM.trigger = StoryManager.Check_StoryTrigger.time_Float_Countdown;
    }

    public static void StageControl_SetIntTimer(StoryManager SM, int time)
    {
        SM.CountDownTimer = time;
        SM.trigger = StoryManager.Check_StoryTrigger.timeCountdown;
    }

    public static void StageControl_MoveToTarget(StoryManager SM, List<Vector2Int> Vectors_TriggerAreaCheck)
    {
        SM.Vectors_TriggerAreaCheck = Vectors_TriggerAreaCheck;
        SM.trigger = StoryManager.Check_StoryTrigger.moveToArea;
    }

    public static void SetGrid_IfWalkable(List<Vector2Int> myTrigger, bool canWalk)
    {
        GameController GC = FindObjectOfType<GameController>();
        List<PathNode> myPath = GC.GM.GetPathList(myTrigger);
        if (myPath != null && myPath.Count != 0)
            foreach (PathNode path in myPath)
            {
                if (canWalk) path.isBlocked = false;
                else path.isBlocked = true;
            }
    }
}
