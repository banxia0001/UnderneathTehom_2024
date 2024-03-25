using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridManager))]
public class MapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GridManager GM = (GridManager)target;

        if (GUILayout.Button("BuildMyGrids"))
            GM.BuildMyGrids();

        if (GUILayout.Button("Reset_ChildPosition"))
            GM.Reset_GridChildHeight();

        if (GUILayout.Button("Reset_Height"))
            GM.Reset_GridHeight();

        if (GUILayout.Button("Reset_Color"))
            GM.Reset_GridColor();

        if (GUILayout.Button("Clear_Block"))
            GM.Clear_Block();

        if (GUILayout.Button("Gene_Block"))
            GM.Gene_Block();

        if (GUILayout.Button("Gene_SortingFolder"))
            GM.BuildSortingLayerFolder();

        if (GUILayout.Button("Equal_SortingFolder"))
            GM.SetEqualSortingLayerFolder();

        if (GUILayout.Button("Gene_AnimationFolder"))
            GM.BuildMyAnimatinoFolder();

        if (GUILayout.Button("GetOriginHeight"))
            GM.GetOriginHeight();

        if (GUILayout.Button("SetOriginHeight"))
            GM.SetOriginHeight();

        if (GUILayout.Button("SetStageHeight"))
            GM.SetHeightInStage();
    }
}


//[CustomEditor(typeof(StoryManager))]
//public class StoryManagerEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        DrawDefaultInspector();
//        StoryManager SM = (StoryManager)target;

//        if (GUILayout.Button("Debug_StartGame"))
//            SM.StartGamePlot(SM.GameEventInHand);
//    }
//}
