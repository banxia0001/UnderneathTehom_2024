using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeSelfDetectTrigger : MonoBehaviour
{
    public List<Vector2Int> triggers;

    private GameController GC;
    private void Start()
    {
        GC = FindObjectOfType<GameController>();
    }

    private void Update()
    {
        if (CheckUnitOnTargetGrid(triggers))
        {
            this.gameObject.SetActive(false);
        }
    }
    private bool CheckUnitOnTargetGrid(List<Vector2Int> triggers)
    {
        bool canTrigger = false;

        foreach (Vector2Int xy in triggers)
        {
            if (GC.GM.pathArray[xy.x, xy.y] != null)
                if (GC.GM.pathArray[xy.x, xy.y].unit != null)
                    if (GC.GM.pathArray[xy.x, xy.y].unit.unitTeam == Unit.UnitTeam.playerTeam) canTrigger = true;
        }

        return canTrigger;
    }
}
