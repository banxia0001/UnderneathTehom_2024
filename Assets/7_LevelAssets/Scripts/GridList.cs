using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LocalData/GridList")]
public class GridList : ScriptableObject
{
    public GameObject moveableNode;
    public GameObject nodeUnderUnit ;
    public GameObject moveableNode_FriendOn;

    public GameObject greenNode_Light;
    public GameObject greenNode;

    public GameObject redNode_Light;
    public GameObject redNode;
    public GameObject redNode_Boss;
    public GameObject redNode_Boss_Large;
    public GameObject redNode_Boss_Middle;

    public GameObject purpleNode_Light;
    public GameObject purpleNode;

    public GameObject staticNode;

    public GameObject chargeNode;

    public GameObject chargeNode_Attack;
    public GameObject chargeNode_Final;
    public GameObject chargeNode_Wrong;

    public GameObject chargeNode_Enemy;
}
