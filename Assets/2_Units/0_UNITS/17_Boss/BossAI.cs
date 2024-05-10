using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
public class BossAI : MonoBehaviour
{
    [Header("DebugMode")]
    public bool noSwordSummon = false;
    public enum BossState
    {
        stage_1_OnField,
        stage_2_OnCliff,
        stage_3_OnCliff_JumpBack,
    }
    public enum BossHoldSkill
    {
       none,holdSkill_1,holdSkill_2
    }

    [Header("Basic Stats")]
    public BossState bossState;
    public BossHoldSkill bossHoldSkill;
    public int timer, timer_Skill;

    public bool secondActioned;
    public bool secondAction_Attacked;

    [Header("SwordStats")]
    public int swordInHand;
    public int remainSword_SummonTimer;
    public int remainSword_SummonTimer_2;
    public GameObject[] SwordSprite;
    public GameObject[] Prefab_Swords;

    [Header("Vectors")]
    public List<Vector2Int> Vector_PlayerDeselectAroundNodes;

    [Header("Holders")]
    public List<GameObject> NodeGuideInHold = new List<GameObject>();
    public List<PathNode> NodeAttackInHold;

    [Header("Misc")]
    public GameObject Shadow;
    public GameObject HealthUI;
    public GameObject ShockWaveVFX;
    public GameObject Minor_ShockWave_VFX;

    public Unit thisUnit;
    private UnitAI thisUnitAI;
    PathNode gobackPath = null;
    public SortingGroup swordSortingA, swordSortingB;

    public int bossSummonOrder = 0;
    private void Start()
    {
        thisUnit = this.GetComponent<Unit>();
        thisUnitAI = this.GetComponent<UnitAI>();
        JumpToMiddle();
        swordInHand = 2;
        remainSword_SummonTimer_2 = 4;
    }
    private void DefaultSetting()
    {
        Destroy_NodeGuideInHold();
        secondActioned = true;
        secondAction_Attacked = true;
        Stage1_NoSkill_BackToNormal();
        bossState = BossState.stage_1_OnField;
        timer = 3;
        timer_Skill = 1;
    }
    private void JumpToMiddle()
    {
        DefaultSetting();
        StartCoroutine(JumpingBetweenTiles(this.thisUnit.nodeAt, true, true,true,false,true));
    }

    public void CheckNewTurn()
    {
        Debug.Log("Boss Check New Turn "+thisUnit.activeNumber + ":" + thisUnit.GC.isAIThinking + "/" + thisUnit.GC.isMoving + "/" + thisUnit.GC.isAttacking);
        thisUnit.activeNumber++;
        thisUnit.GC.isAIThinking = true;
        secondActioned = false;
        secondAction_Attacked = false;
      
        switch (bossState)
        {
            case BossState.stage_1_OnField:
                timer--;
                if (timer <= 0 && bossHoldSkill == BossHoldSkill.none) Enter_Stage_2_JumpToCliff();
                else Stage_1_Action();
                break;

            case BossState.stage_2_OnCliff:
                secondActioned = false;
                gobackPath = BossFunctions.Get_TeleportPathnode_Size3(true);
                if (gobackPath == null)
                {
                    Debug.Log("gobackPath is null");
                    gobackPath = FindObjectOfType<GridManager>().GetPath(7, 6);
                } 
                //Debug.Log(gobackPath.name);
                StartCoroutine(BossFunctions.Prepare_JumpDown(thisUnit, gobackPath, this));
                bossState = BossState.stage_3_OnCliff_JumpBack;
                break;

            case BossState.stage_3_OnCliff_JumpBack:
                timer = Random.Range(3,6);
                if (gobackPath == null)
                {
                    gobackPath = BossFunctions.Get_TeleportPathnode_Size3(true);
                    if (gobackPath = null) gobackPath = FindObjectOfType<GridManager>().GetPath(7, 6);
                    StartCoroutine(BossFunctions.Prepare_JumpDown(thisUnit, gobackPath, this));
                }

                else
                {
                    DefaultSetting();
                    StartCoroutine(JumpingBetweenTiles(gobackPath, true, false, true,false,false));
                }
                break;
        }
    }


    public Unit targetUnit;
    private void Stage_1_Action()
    {
        thisUnit.GC.isAIThinking = true;

        this.targetUnit = GameFunctions.FindClosestUnit_By_Grid(0, thisUnit);
        foreach (_Buff buff in thisUnit.buffList)
        {
            if (buff.buff.buffType == Buff._buffType.tuant) 
                if (buff.tuantTarget != null) 
                    this.targetUnit = buff.tuantTarget;
        }

        if (!secondActioned)
        {
            if (bossHoldSkill != BossHoldSkill.none)
            { Debug.Log("Boss cast skill"); Skill_Attack(); Stage1_NoSkill_BackToNormal(); return; }
            timer_Skill--;
        }

        remainSword_SummonTimer--;
        if (bossHoldSkill == BossHoldSkill.none)
        {
            if (BossFunctions.CheckSword() < 1 && swordInHand > 1)
            {
                Debug.Log("Boss throw sword");
                StartCoroutine(BossFunctions.Boss_ThrowSwordToBattlefield(this, thisUnit));
                return;
            }
        }

        if (timer_Skill <= Random.Range(-1, 0)) 
        {
            Debug.Log("Boss may cast skill");
            timer_Skill = 6; Skill_Hold(); 
            return;
        }


        remainSword_SummonTimer_2--;
        if (swordInHand < 1 && remainSword_SummonTimer_2 < 0)
        {
            Debug.Log("Boss summon sword");
            remainSword_SummonTimer_2 = Random.Range(4, 5); ;
            StartCoroutine(BossFunctions.Boss_SummonSwordToHand(this,thisUnit));
            return;
        }

        if (!secondAction_Attacked && bossHoldSkill == BossHoldSkill.none)
        {
            Debug.Log("Boss attack");
            Normal_Attack(); return;
        }

        ActionEnd();
    }

    public void ChangeSword(int num)
    {
        swordInHand += num;
        if (swordInHand > 2) swordInHand = 2;
        if (swordInHand <= 0) swordInHand = 0;
        ShowSwordSprite();
    }
    private void ShowSwordSprite()
    {
        if (swordInHand == 2) { SwordSprite[0].SetActive(true); SwordSprite[1].SetActive(true); }
        if (swordInHand == 1) { SwordSprite[0].SetActive(false); SwordSprite[1].SetActive(true); }
        if (swordInHand == 0) { SwordSprite[0].SetActive(false); SwordSprite[1].SetActive(false); }
    }

    private void Normal_Attack()
    {
        secondAction_Attacked = true;
        if (targetUnit != null) MoveCloserToTarget_Melee();
        else { ActionEnd(); }
    }

    public void ActionEnd()
    {
        Debug.Log("Boss Action End:" + thisUnit.GC.isAIThinking + "/"+thisUnit.GC.isMoving + "/" + thisUnit.GC.isAttacking);
        thisUnit.GC.isMoving = false;
        thisUnit.GC.isAttacking = false;

        if (!secondActioned && bossState == BossState.stage_1_OnField)
        {
            Debug.Log("Second Action Start");
            thisUnit.GC.isAIThinking = true;
            secondActioned = true;
            Stage_1_Action();
            return;
        }

        thisUnit.UnitEnable(false);
        thisUnit.GC.isAIThinking = false;
        thisUnit.activeNumber = 0;
    }



    public void Stage1_NoSkill_BackToNormal()
    {
        Debug.Log("Boss Skill Reset");
        timer = 0;
        timer_Skill = 4;
        bossHoldSkill = BossHoldSkill.none; 
        Destroy_NodeGuideInHold();
    }
    private void Skill_Hold()
    {
        Debug.Log("Boss Skill Prepare");
        bossHoldSkill = BossHoldSkill.holdSkill_2;
        StartCoroutine(BossFunctions.BossSkill_2_1(this));
    }
    private void Skill_Attack()
    {
        Debug.Log("Boss Skill Attack");
        //if (bossHoldSkill == BossHoldSkill.holdSkill_1)
        //{
        //    StartCoroutine(BossFunctions.BossSkill_1_2(this,thisUnit));
        //}

        if (bossHoldSkill == BossHoldSkill.holdSkill_2)
        {
            StartCoroutine(BossFunctions.BossSkill_2_2(this,thisUnit, Minor_ShockWave_VFX));
        }
    }
   
    public void EmergenceJumpToCliff()
    {
        Destroy_NodeGuideInHold();
        thisUnit.GC.isAIThinking = true;
        thisUnit.GC.isAttacking = true;
        Enter_Stage_2_JumpToCliff();
    }
    private void Enter_Stage_2_JumpToCliff()
    {
        Stage1_NoSkill_BackToNormal();
        bossState = BossState.stage_2_OnCliff;
        timer = 3;
        GridManager GM = FindObjectOfType<GridManager>();
        int i = Random.Range(0, Vector_PlayerDeselectAroundNodes.Count);
        StartCoroutine(JumpingBetweenTiles(GM.GetPath(Vector_PlayerDeselectAroundNodes[i].x, Vector_PlayerDeselectAroundNodes[i].y),false,false, false,true,false));
    }
    private void Stage_2_Action()
    {
        ActionEnd();
    }





    private IEnumerator JumpingBetweenTiles(PathNode pathTo, bool jumpAttack, bool onlyJumpDown, bool dontDisactive, bool toPlatform, bool firstTime)
    {
        thisUnit.moveState = Unit.MoveState.none;
        HealthUI.SetActive(false);
        Shadow.SetActive(false);

        if (!onlyJumpDown)
        {
            if (toPlatform)
            {
                this.thisUnit.InputAnimation_Single_NoLoop("Fly up");
                yield return new WaitForSeconds(1.2f);
                Generate_FlyUp_ShockWave(thisUnit);
                yield return new WaitForSeconds(2.8f);
            }
            else
            {
                this.thisUnit.InputAnimation_Single_NoLoop("fly back up");
                yield return new WaitForSeconds(1f);
                Generate_FlyUp_ShockWave(thisUnit);
                yield return new WaitForSeconds(2f);
            }
        }

        BossFunctions.AddBossToTeleportoNode(pathTo, thisUnit);
        if (!onlyJumpDown) yield return new WaitForSeconds(0.3f);

        thisUnit.moveState = Unit.MoveState.none;
        HealthUI.SetActive(false);
        Shadow.SetActive(false);

        if (firstTime)
        {
            this.thisUnit.InputAnimation_Single("fly back AOE");
            this.thisUnit.AddAnimation("enterbattle");
            this.thisUnit.AddAnimation("idle");

            yield return new WaitForSeconds(0.65f);
            if (jumpAttack) StartCoroutine(Generate_JumpDown_ShockWave(thisUnit));
            yield return new WaitForSeconds(5.5f);
        }

        else if (toPlatform)
        {
            this.thisUnit.InputAnimation("touch down");
            yield return new WaitForSeconds(3f);
        }
        else
        {
            this.thisUnit.InputAnimation("fly back aoe full");
            yield return new WaitForSeconds(0.65f);
            if (jumpAttack) StartCoroutine(Generate_JumpDown_ShockWave(thisUnit));
            yield return new WaitForSeconds(3f);
        } 

        HealthUI.SetActive(true);
        Shadow.SetActive(true);

       
        if (dontDisactive)
        {
            thisUnit.GC.isAIThinking = false;
            thisUnit.GC.isAttacking = false;
            thisUnit.GC.isMoving = false;
            thisUnit.activeNumber = 0;
        }
        else ActionEnd();
    }





    private void MoveCloserToTarget_Melee()
    {
        if (thisUnitAI.path != null) thisUnitAI.path.Clear();

        thisUnitAI.path = AIFunctions.FindClosestPathToTargetNode_Size3_AI(thisUnit, targetUnit, targetUnit.nodeAt);

        if (thisUnitAI.path != null && thisUnitAI.path.Count > 0)
        {
            thisUnitAI.aiState = UnitAI.AI_State.attack;
            StartCoroutine(thisUnitAI.AI_MoveWithTimeDelay());
            return;
        }
        else
        {
            ActionEnd();
            return;
        }
    }



    public void Destroy_NodeGuideInHold()
    {
        if (NodeGuideInHold != null && NodeGuideInHold.Count > 0)
        {
            foreach (GameObject ob in NodeGuideInHold)
            {
                Destroy(ob);
            }
        }
        NodeGuideInHold = new List<GameObject>();
    }

    public void Draw_NodeGuideInHold(List<PathNode> NodeEffect, int num, bool destroyOld)
    {
        if(destroyOld)
        Destroy_NodeGuideInHold();

        Debug.Log("DrawGuideMap:" + NodeEffect.Count + ":" + num);
        if (NodeEffect.Count < 1) return;
        GameObject prefab = null;

        if (num == 0) prefab = GameFunctions.LoadGrid("redNode_Boss");
        if (num == 1) prefab = GameFunctions.LoadGrid("redNode_Boss_Middle");
        if (num == 2) prefab = GameFunctions.LoadGrid("redNode_Boss_Large");

        Debug.Log("DrawGuideName:" + prefab.name);

        for (int i = 0; i < NodeEffect.Count; i++)
        {
            if (NodeEffect != null)
            {
                GameObject NodeEff = Instantiate(prefab, NodeEffect[i].transform.position, Quaternion.identity);
                NodeEff.transform.parent = NodeEffect[i].transform;
                NodeGuideInHold.Add(NodeEff);
            }
        }
    }


    public static void Generate_FlyUp_ShockWave(Unit thisUnit)
    {
        FindObjectOfType<GameController>().cameraAnim.SetTrigger("Shake1");
        GridFallController.AddNodeToDynamicFolder(thisUnit.nodeAt_Top, "Minor", "");
        GridFallController.AddNodeToDynamicFolder(thisUnit.nodeAt, "Minor", "");
        GridFallController.AddNodeToDynamicFolder(thisUnit.nodeAt_Right, "Minor", "");
    }



    public static IEnumerator Generate_JumpDown_ShockWave(Unit thisUnit)
    {
        List<NodeList> shockList = BossFunctions.Get_Size3_ShockWaveList(thisUnit.nodeAt);
       
        foreach (PathNode path in shockList[0].node)
        {
            if (path != null)
            {
                Debug.Log(path.name);
                GridFallController.AddNodeToDynamicFolder(path, "Down", "");
                path.AddHeight_WithCD(-2, 3, 1);

                if (path.unit != null && path.unit != thisUnit)
                {
                    SkillFunctions.Skill_SimpleDamageCalculate(40, thisUnit, path.unit, null);
                    BossFunctions.Boss_KnockBack(thisUnit, path.unit, 2, null);
                }
            }

        }

        yield return new WaitForSeconds(0.05f);
        FindObjectOfType<GameController>().cameraAnim.SetTrigger("Shake3_Down");

        foreach (PathNode path in shockList[1].node)
        {
            if (path != null)
            {
                GridFallController.AddNodeToDynamicFolder(path, "Down", "");

                if (path.unit != null && path.unit != thisUnit)
                {
                    SkillFunctions.Skill_SimpleDamageCalculate(10, thisUnit, path.unit, null);
                    BossFunctions.Boss_KnockBack(thisUnit, path.unit, 4, null);
                }
            } 
        }

     
        yield return new WaitForSeconds(0.1f);
        foreach (PathNode path in shockList[2].node)
        {
            if (path != null) 
            {
                GridFallController.AddNodeToDynamicFolder(path, "Down_Up", "");
                path.AddHeight_WithCD(2, 3, 1);
                
                if (path.unit != null && path.unit != thisUnit)
                {
                    SkillFunctions.Skill_SimpleDamageCalculate(5, thisUnit, path.unit, null);
                    BossFunctions.Boss_KnockBack(thisUnit, path.unit, 2, null);
                }
            }
        }
    }



}
