using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class AutoMachineGun_Controller : MonoBehaviour
{
    public enum autoMachineGun_State { waiting, waitingForTrigger, searchingForEnemy }
    public bool type_Pene = true;
    public Skill aoeSkill;

    [Header("STATE")]
    public autoMachineGun_State state;
    private GameController GC;
    
    public Unit targetEnemy;
    //public PathNode targetNode;


    private Unit thisUnit;
    public Transform target;

    [Space(5)]
    [Header("ANIM")]
    public SkeletonAnimation skeletonAnim;
    private Spine.Bone bone;
    public GameObject gunEffect;

    public Transform boneFollower_Gun;


    [Header("TRANSFORM")]
    private Vector3 targetDiretion;

    public void Start()
    {
        bone = skeletonAnim.Skeleton.FindBone("Unity_Target");
        GC = FindObjectOfType<GameController>();
        state = autoMachineGun_State.waitingForTrigger;
        thisUnit = this.GetComponent<Unit>();
    }

    public void Update()
    {
        SetBone();
        //TestTarget_Spine();

        if (GC.isMoving || GC.isAttacking)
        {
            if (state == autoMachineGun_State.waiting)
                state = autoMachineGun_State.waitingForTrigger;
        }

        else
        {
            if (state == autoMachineGun_State.waitingForTrigger)
            {
                state = autoMachineGun_State.searchingForEnemy;
                SearchForEnemy();
            }
        }
    }

    public void Trigger_AttackEnemy()
    {
        if (thisUnit.isActive == false) return;

        SearchForEnemy();
        if (targetEnemy != null)
        {
            if (type_Pene)
                StartCoroutine(GameFunctions.Attack(thisUnit, targetEnemy, thisUnit.currentData.damage, true, aoeSkill, false));

            else
            {
                StartCoroutine(GameFunctions.Attack(thisUnit, targetEnemy, thisUnit.currentData.damage, true, aoeSkill, false));
            }
        }
        else
        {
            thisUnit.UnitEnable(false);
        }
    }

    public void SpawnGunFire(Unit defender)
    {
        GameObject specialEffect = Instantiate(gunEffect, boneFollower_Gun.transform.position, Quaternion.identity);
        specialEffect.transform.eulerAngles = boneFollower_Gun.transform.eulerAngles;
        specialEffect.transform.localScale = boneFollower_Gun.transform.localScale;
    }


    public void SearchForEnemy()
    {
        Unit closestEnemy = null;

        if (type_Pene)
        {
            List<Unit> unitResult = AIFunctions.FindClosestUnits_Range(thisUnit, GameController.enemyList, thisUnit.currentData.damage.range);
            if (unitResult != null) closestEnemy = AIFunctions.FindUnit_ByDam_AI(unitResult, thisUnit);
        }
        else
        {
            closestEnemy = AIFunctions.FindClosestUnit_Range(thisUnit, GameController.enemyList);
        }
        
        if (closestEnemy == null) 
            targetEnemy = null;

        else if (!GameFunctions.CheckAttackRange(thisUnit, closestEnemy, thisUnit.currentData.damage.range, true, false, false)) 
            targetEnemy = null;

        else 
            targetEnemy = closestEnemy;

        targetDiretion = this.transform.position + getTargetPos();
        state = autoMachineGun_State.waiting;
    }

    private void SetBone()
    {
        target.transform.position = Vector3.Lerp(target.transform.position, targetDiretion, 0.06f);
        UnitFunctions.Flip_Simple(this.transform.position.x, target.position.x, this.thisUnit.TranformOffsetFolder);
        var pos = target.position;
        var skeletonSpacePoint = skeletonAnim.transform.InverseTransformPoint(pos);
        bone.SetLocalPosition(skeletonSpacePoint);
    }


    private Vector3 getTargetPos()
    {
        if (targetEnemy == null)
        {
            return new Vector3(0.6f, 0.8f);
        }

        float y = this.transform.position.y - targetEnemy.transform.position.y;
        float x = this.transform.position.x - targetEnemy.transform.position.x;

        if (y < -0.3f)
        {
            if (x > 0.45f) return new Vector3(-0.5f, 1.65f);//up left
            else if (x < -0.45f) return new Vector3(0.5f, 1.65f);//up right
            else return new Vector3(0.2f, 1.7f);//middle
        }
        else if (y > 0.3f)
        {
            if (x > 0.45f) return new Vector3(-.76f, 1f);//down left
            else if (x < -0.45f) return new Vector3(.76f, 1f);//down right
            else return new Vector3(0.45f, 1f);//middle
        }

        else
        {
            if (x > 0) return new Vector3(-0.7f, 1.35f);//mid left
            else return new Vector3(0.7f, 1.35f);//mid right
        }
    }
}
