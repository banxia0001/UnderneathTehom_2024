using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampaignMapEvent : MonoBehaviour
{
    //public Color32 red, grey, yellow;

    public List<GameObject> CollapseSprite;
    public CampaignMapEvent NodeCanGo;
    public enum EventType { combat, reward, none }
    public EventType eventType;

    public enum EventType_2 { none,enterLV2_1, enterLV2_2 }
    public EventType_2 eventType_2;

    [TextArea(5,5)]
    public string enterString;
    public string choiceString;

    public List<RewardList> rewardList;

    public IEnumerator ShakeSprite()
    {
        foreach (GameObject ob in CollapseSprite)
        {
            yield return new WaitForSeconds(Random.Range(0.1f, 0.2f));
            ob.GetComponent<Animator>().SetTrigger("trigger1");
        }
    }
}
