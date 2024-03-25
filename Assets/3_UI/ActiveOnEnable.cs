using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveOnEnable : MonoBehaviour
{
    public Unit unit;
    private void OnEnable()
    {
        unit.UnitBar.Set_Bar(unit);
    }
}
