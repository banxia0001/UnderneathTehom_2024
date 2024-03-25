using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;


public class NamelessRotation_Mouse : MonoBehaviour
{
    public Transform player;
    public Transform offsetFolder;
    public bool namelessCanFakeRotate;
    public SkeletonAnimation skeletonAnimation;
    public Spine.Bone bone;

    public float x, y;

    private void Start()
    {
        this.bone = skeletonAnimation.Skeleton.FindBone("HeadControl_Front");
    }
    private void Update()
    {
        if (bone != null)
            if (namelessCanFakeRotate)
                GetMousePosition();
    }
    public void GetMousePosition()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        x =  player.transform.position.x - mousePos.x;
        y =  player.transform.position.y - mousePos.y;

        Vector2 targetPositoin = new Vector3(-0.4f * x,-0.6f * y);

        if (offsetFolder.localScale.x < 0)  
            targetPositoin = new Vector3(0.4f * x, -0.6f * y);

        bone.SetLocalPosition(targetPositoin);
    }
}
