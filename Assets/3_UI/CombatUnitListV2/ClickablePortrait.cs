using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickablePortrait : MonoBehaviour
{
    private Unit unitInHold;
    public Transform trans;

    public void SendUnitToGameController()
    {
        GameController GC = FindObjectOfType<GameController>();
        GC.Select_Unit(unitInHold.nodeAt,true);
    }

    public void GetUnit(Unit unit)
    {
        unitInHold = unit;
        trans.GetComponent<PortraitManager>().UpdatePortrait(unit.data);
    }
}
