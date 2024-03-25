using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptObject/Reward")]
public class RewardList : ScriptableObject
{
    public List<Reward> rewards;
}

[System.Serializable]
public class Reward
{
    public enum Type { skill, servant, perk }
    public Type type;

    [Header("Skill")]
    public Skill skill;

    [Header("Servant")]
    public GameObject unitPrefab;
    [TextArea(5,5)]
    public string unitPrefabText;

    [Header("Perk")]
    public Buff perk;
}
