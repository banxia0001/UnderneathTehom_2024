using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTrigger : MonoBehaviour
{
    public int order;
    public Vector2Int _A;
    public Vector2Int _B;
    public StoryManager sm;

    private GridManager GM;
    private void Start()
    {
        GM = FindObjectOfType<GridManager>();
    }

    //private void Update()
    //{
    //    PathNode path_A = GM.GetPath(_A.x,_A.y);
    //    PathNode path_B = GM.GetPath(_B.x, _B.y);

    //    if (path_A != null)
    //        if (path_A.unit != null)
    //            if (path_A.unit.data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._6_Skel_Archer)
    //            {
    //                sm.story_Lv1.summonedArcher = true;
    //            }
    //    if (path_B != null)
    //        if (path_B.unit != null)
    //            if (path_B.unit.data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._4_Skel_Shield)
    //            {
    //                sm.story_Lv1.summonedWarrior = true;
    //                Destroy(this);
    //            }
    //}
}
