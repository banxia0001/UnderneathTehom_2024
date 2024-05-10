using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

public class SpawnCollider : MonoBehaviour
{
    public SkeletonAnimation skeletonAnim;

    public void Awake()
    {
        var skeleton = skeletonAnim.Skeleton;
        var skeletonData = skeleton.Data;
        Skin mixAndMatchSkin = new Skin("custom-girl");

      
        mixAndMatchSkin.AddSkin(skeletonData.FindSkin("bone"));
        mixAndMatchSkin.AddSkin(skeletonData.FindSkin("metal"));

        skeleton.SetSkin(mixAndMatchSkin);
        skeleton.SetSlotsToSetupPose();
        skeletonAnim.AnimationState.Apply(skeletonAnim.Skeleton);
    }
}
