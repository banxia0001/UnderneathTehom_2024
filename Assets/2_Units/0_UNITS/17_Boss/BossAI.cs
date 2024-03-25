using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : MonoBehaviour
{
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
    private int swordInHand;
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

    public Animator anim;
    private Unit thisUnit;
    private UnitAI thisUnitAI;
    PathNode gobackPath = null;


    int bossSummonOrder = 0;
    private void Start()
    {
        thisUnit = this.GetComponent<Unit>();
        thisUnitAI = this.GetComponent<UnitAI>();
        JumpToMiddle();
        swordInHand = 2;
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
        StartCoroutine(JumpingBetweenTiles(this.thisUnit.nodeAt, true, true,true));
    }

    public void CheckNewTurn()
    {
        Debug.Log("Boss New Turn");
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
                timer = 3;
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

                timer = 3;
                if (gobackPath == null)
                {
                    gobackPath = BossFunctions.Get_TeleportPathnode_Size3(true);
                    if (gobackPath = null) gobackPath = FindObjectOfType<GridManager>().GetPath(7, 6);
                    StartCoroutine(BossFunctions.Prepare_JumpDown(thisUnit, gobackPath, this));
                }

                else
                {
                    DefaultSetting();
                    StartCoroutine(JumpingBetweenTiles(gobackPath, true, false, true));
                }
                break;
        }
    }


    public Unit targetUnit;
    private void Stage_1_Action()
    {
        Debug.Log("STAGE_1: BOSS IS THINKING");
        thisUnit.GC.isAIThinking = true;

        this.targetUnit = GameFunctions.FindClosestUnit_By_Grid(0, thisUnit);
        foreach (_Buff buff in thisUnit.buffList)
        {
            if (buff.buff.buffType == Buff._buffType.tuant) if (buff.tuantTarget != null) this.targetUnit = buff.tuantTarget;
        }

        if (!secondActioned)
        {
            Debug.Log("Boss cast skill");
            if (bossHoldSkill != BossHoldSkill.none)
            { Skill_Attack(); Stage1_NoSkill_BackToNormal(); return; }
            timer_Skill--;
        }

        //remainSword_SummonTimer--;
        if (bossHoldSkill == BossHoldSkill.none)
        {
            Debug.Log("Boss throw sword");
            if (BossFunctions.CheckSword() < 2 && swordInHand > 0)
            {
              
                StartCoroutine(BossFunctions.Boss_ThrowSwordToBattlefield(this, thisUnit, Prefab_Swords[bossSummonOrder]));
                bossSummonOrder++;
                if (bossSummonOrder >= Prefab_Swords.Length)  bossSummonOrder = 0;
                return;
            }
        }

        if (timer_Skill <= 0) 
        {
            Debug.Log("Boss may cast skill");
            timer_Skill = 3; Skill_Hold(); return;
        }


        remainSword_SummonTimer_2--;
        if (swordInHand < 1 && remainSword_SummonTimer_2 < 0)
        {
            Debug.Log("Boss summon sword");
            remainSword_SummonTimer_2 = 2;
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
        Debug.Log("Boss Action End");
        thisUnit.GC.isAIThinking = false;
        thisUnit.GC.isMoving = false;
        thisUnit.GC.isAttacking = false;

        if (!secondActioned && bossState == BossState.stage_1_OnField)
        {
            Debug.Log("Second Action Start");
            secondActioned = true;
            Stage_1_Action();
            return;
        }
        thisUnit.UnitEnable(false);
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
        if (bossHoldSkill == BossHoldSkill.holdSkill_1)
        {
            StartCoroutine(BossFunctions.BossSkill_1_2(this,thisUnit));
        }

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
        StartCoroutine(JumpingBetweenTiles(GM.GetPath(Vector_PlayerDeselectAroundNodes[i].x, Vector_PlayerDeselectAroundNodes[i].y),false,false, false));
    }
    private void Stage_2_Action()
    {
        ActionEnd();
    }





    private IEnumerator JumpingBetweenTiles(PathNode pathTo, bool jumpAttack, bool onlyJumpDown, bool dontDisactive)
    {
     
        thisUnit.moveState = Unit.MoveState.none;
        HealthUI.SetActive(false);
        Shadow.SetActive(false);

        if (!onlyJumpDown)
        {
            anim.SetTrigger("Up");
            Generate_FlyUp_ShockWave(thisUnit);
            yield return new WaitForSeconds(1f);
        }

        BossFunctions.AddBossToTeleportoNode(pathTo, thisUnit);
        if (!onlyJumpDown) yield return new WaitForSeconds(0.3f);

        anim.SetTrigger("Down");
        thisUnit.moveState = Unit.MoveState.none;
        HealthUI.SetActive(false);
        Shadow.SetActive(false);

        yield return new WaitForSeconds(0.15f);
        if(jumpAttack) StartCoroutine(Generate_JumpDown_ShockWave(thisUnit));
        else Generate_FlyUp_ShockWave(thisUnit);

        HealthUI.SetActive(true);
        Shadow.SetActive(true);

        yield return new WaitForSeconds(2f);
        if (dontDisactive)
        {
            thisUnit.GC.isAIThinking = false;
            thisUnit.GC.isAttacking = false;
            thisUnit.GC.isMoving = false;
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

    public void Draw_NodeGuideInHold(List<PathNode> NodeEffect)
    {
        Destroy_NodeGuideInHold();
        if (NodeEffect.Count < 1) return;
        
        for (int i = 0; i < NodeEffect.Count; i++)
        {
            GameObject NodeEff = Instantiate(GameFunctions.LoadGrid("redNode_Boss"), NodeEffect[i].transform.position, Quaternion.identity);
            NodeEff.transform.parent = NodeEffect[i].transform;
            NodeGuideInHold.Add(NodeEff);
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
