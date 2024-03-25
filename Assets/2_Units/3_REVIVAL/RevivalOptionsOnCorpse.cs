using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevivalOptionsOnCorpse : MonoBehaviour
{
    public List<GameObject> revivalOptions;
    private void wake()
    {
        Unit unit = this.gameObject.GetComponent<Unit>();
        if (unit.unitAttribute == Unit.UnitAttribute.alive || unit.unitAttribute == Unit.UnitAttribute.staticObject) Destroy(this);
    }
}
