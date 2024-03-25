using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CampaignEventController : MonoBehaviour
{
    public CampaignMapController CMC;
    private CampaignMapEvent NodeAt;
    public TMP_Text descrip, choice;

    public void InputEvent(CampaignMapEvent NodeAt)
    {
        CMC.state = CampaignMapController.State.inEventChoice;
        this.NodeAt = NodeAt;
        descrip.text = NodeAt.enterString.ToString();
        choice.text = NodeAt.choiceString.ToString();
    }

    public void SelectEvent()
    {
        if (NodeAt.eventType == CampaignMapEvent.EventType.combat)
        {
            SaveData.rewardList = new List<RewardList>();
            SaveData.rewardList = NodeAt.rewardList;
            if (NodeAt.eventType_2 == CampaignMapEvent.EventType_2.enterLV2_1) CMC.EnterCombat_1(3);
            if (NodeAt.eventType_2 == CampaignMapEvent.EventType_2.enterLV2_2) CMC.EnterCombat_1(4);
        }

        if (NodeAt.eventType == CampaignMapEvent.EventType.reward)
        {
            CMC.state = CampaignMapController.State.inReward;
            CMC.InputReward(NodeAt.rewardList);
        }

        if (NodeAt.eventType == CampaignMapEvent.EventType.none)
        {
            CMC.state = CampaignMapController.State.inCampaignMap;
        }

        this.gameObject.SetActive(false);
    }
}
