using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardUI : MonoBehaviour
{
    public enum  RewardState{ waitingForReward, selectedReward, confirmedReward }
    public RewardState rewardState;

    [Header("Miscs")]
    public ListPanelUI listPanelUI;
    public UnitStatsPanel_V2 unitStats;
    public GameObject button_GoR, button_GoL;
    private Unit unitShowing;//Only use for team confirm
    private UnitData unitShowingData;
    public PortraitManager PM;
    private bool isInLeft;
    private int rewardLoadOrder;
    private SlotContainer_V2 slot;


    private List<RewardList> rewardList;
    private CampaignMapUI UI;

    public void InputReward_Setup(List<RewardList> rewardList)
    {
        FindObjectOfType<CampaignMapController>().popup.Play();
        this.rewardList = rewardList;
        rewardLoadOrder = 0;
        slot = null;
        Switch_AvatarData();
        InputReward(rewardList[rewardLoadOrder]);
        GoRight();
    }
    

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
        {
            Switch_AvatarData();
            DeselectReward();
        }
    }
   
    public void ComfirmReward(Transform targetReward, SlotContainer_V2 slot)
    {
        if (rewardState == RewardState.confirmedReward) return;
        FindObjectOfType<CampaignMapController>().reward.Play();
        rewardState = RewardState.confirmedReward;
        if (targetReward != null) targetReward.SetParent(this.transform);
        AddingRewardToPlayerList(slot);


        rewardLoadOrder++;
        if (rewardLoadOrder < rewardList.Count) StartCoroutine(nextReward(targetReward));
        else  StartCoroutine(closePanel(targetReward));
    }

    public void SlotInput_GainReward(Transform targetReward, SlotContainer_V2 slot)
    {
        if (rewardState == RewardState.confirmedReward) return;

        if (this.slot == null)
        { SelectReward(slot); return; }

        else
        {
            if (rewardState == RewardState.selectedReward && this.slot == slot) DeselectReward();
            else { SelectReward(slot); }
        }
    }
    private void DeselectReward()
    {
        CampaignMapUI.notShowingFloatngPanels = false;
        if (this.slot != null)
        {
            this.slot.SelectContainer(false);
            this.slot = null;
        }
        rewardState = RewardState.waitingForReward;
    }
    private void SelectReward(SlotContainer_V2 slot)
    {
        CampaignMapUI.notShowingFloatngPanels = true;
        FindObjectOfType<CampaignMapController>().reward.Play();
        DeselectReward();
        this.slot = slot;
        rewardState = RewardState.selectedReward;
        this.slot.SelectContainer(true);

        if (slot.slotType == SlotContainer_V2.SlotType.servant)
        {
            Switch_PrefabData(slot.targetUnitPrefab);
        }
    }
    private IEnumerator nextReward(Transform targetReward)
    {
        listPanelUI.ClearField();
        yield return new WaitForSeconds(0.1f);
        InputReward(rewardList[rewardLoadOrder]);
        yield return new WaitForSeconds(0.25f);
        targetReward.gameObject.SetActive(false);
        DeselectReward();
    }
    private IEnumerator closePanel(Transform targetReward)
    {
        UI = FindObjectOfType<CampaignMapUI>();
        CampaignMapUI.notShowingFloatngPanels = false;
        CampaignMapController.gameFrozen = true;
        UI.loadingPanel.SetBool("Trigger", true);

        listPanelUI.ClearField();
        this.gameObject.GetComponent<Animator>().SetTrigger("trigger");
        yield return new WaitForSeconds(1.5f);

        targetReward.gameObject.SetActive(false);

        CampaignMapController CMC = FindObjectOfType<CampaignMapController>();
        CMC.ReturningFromRewardPanel();
        CampaignMapController.gameFrozen = false;
        UI.loadingPanel.SetBool("Trigger", false);
        this.gameObject.SetActive(false);
    }

    private void AddingRewardToPlayerList(SlotContainer_V2 slot)
    {
        if (slot.slotType == SlotContainer_V2.SlotType.skill) { SaveData.namelessOneData.Skill.Add(slot.skill); }
        if (slot.slotType == SlotContainer_V2.SlotType.perk) { SaveData.namelessOneData.traitList.Add(slot.buff); }
        if (slot.slotType == SlotContainer_V2.SlotType.servant) { SaveData.servantsData.Add(slot.targetUnitPrefab.GetComponent<Unit>().data); }
    }


 

    public void Switch_AvatarData()
    {
        DeselectReward();
        unitShowing = UnitFunctions.Get_UnitPrefab(UnitPrefabList.Unit_SpriteAsset_Type._1_Avatar).GetComponent<Unit>();
        unitShowingData = SaveData.namelessOneData;
        unitStats.InputData(unitShowing, unitShowingData, UnitStatsPanel_V2.PanelType.openUnitInPrefab, isInLeft);
        PM.CloseAll();
    }

    public void Switch_PrefabData(GameObject prefab)
    {

        unitShowing = prefab.GetComponent<Unit>();
        unitShowingData = prefab.GetComponent<Unit>().data;
        unitStats.InputData(unitShowing, unitShowingData, UnitStatsPanel_V2.PanelType.openUnitInPrefab, isInLeft);
        PM.UpdatePortrait(unitShowingData);
    }
    public void GoRight()
    {
        DeselectReward();
        isInLeft = false;
        button_GoL.SetActive(false);
        button_GoR.SetActive(true);
        this.gameObject.GetComponent<Animator>().SetBool("left", false);
        unitStats.InputData(unitShowing, unitShowingData, UnitStatsPanel_V2.PanelType.openUnitInPrefab, isInLeft);
    }
    public void GoLeft()
    {
        DeselectReward();
        isInLeft = true;
        button_GoL.SetActive(true);
        button_GoR.SetActive(false);
        this.gameObject.GetComponent<Animator>().SetBool("left", true);
        unitStats.InputData(unitShowing, unitShowingData, UnitStatsPanel_V2.PanelType.openUnitInPrefab, isInLeft);
    }

    private void InputReward(RewardList rewardList)
    {
        bool firstOfType = true;
        foreach (Reward reward in rewardList.rewards)
        {
            if (reward.type == Reward.Type.skill)
            {
                if (firstOfType) { firstOfType = false; listPanelUI.InputBlockLine("SKILL"); }
                listPanelUI.InputSkill(new _Skill(reward.skill, 0), SaveData.namelessOneData, true);
            }
        }

        firstOfType = true;
        foreach (Reward reward in rewardList.rewards)
        {
            if (reward.type == Reward.Type.servant)
            {
                if (firstOfType) { firstOfType = false; listPanelUI.InputBlockLine_Servant(); }
                listPanelUI.InputServant(reward.unitPrefab,reward.unitPrefab.GetComponent<Unit>().data, true, reward.unitPrefabText);
            }
        }

        firstOfType = true;
        foreach (Reward reward in rewardList.rewards)
        {
            if (reward.type == Reward.Type.perk)
            {
                if (firstOfType) { firstOfType = false; listPanelUI.InputBlockLine("PERK"); }
                listPanelUI.InputPerk(new _Buff(reward.perk), SaveData.namelessOneData, true);
            }
        }
    }
}
