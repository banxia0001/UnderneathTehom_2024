using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Spine.Unity;
using UnityEngine.Rendering;

public class Unit : MonoBehaviour
{
    public enum UnitTeam { playerTeam, enemyTeam, neutral }
    public enum UnitAttribute { alive, staticObject, corpse }
    public enum UnitSpecialState { normalUnit, machineGun, block, healTower, boneFireTower }
    public enum UnitSummonType { hero,summon,sentry}

   
    [Space(5)]
    [Header("STATS")]
    [HideInInspector] public bool isInBattle = false;
    public GameController GC;
    public UnitTeam unitTeam;
    public UnitAttribute unitAttribute;
    public UnitSummonType unitType;
    public UnitSpecialState unitSpecialState;

    public UnitData data;
    public UnitData currentData;
    public UnitDeathOption deathData;
    private int healthLost_ThisTurn;

    [Space(5)]
    [Header("NODE")]
    public PathNode nodeAt;
    public PathNode nodeAt_Right;
    public PathNode nodeAt_Top;

    [Space(5)]
    [Header("BUFFS")]
    public List<_Buff> buffList = new List<_Buff>();

    [Space(5)]
    [Header("BOOLS")]
    public bool isActive = false;
    [HideInInspector] public bool isSelectedInThisTurn = false;
    [HideInInspector] public bool canRepel;
    [HideInInspector] public bool expertHealer = false;
    [HideInInspector] public bool giantSlayer = false;
    [HideInInspector] public bool canFightBack = false;
    [HideInInspector] public bool isCurse = false;

    private bool canFlip = false;
    public bool canNormalFlip = true;
    private float xNow;

    public enum MoveState { standing,combatMove_Forward, combatMove_Back, moving, moving_Down,combatMove_StayOnMoveToGrid, knockBack_Moving, none}

    [Space(5)]
    [Header("MOVE")]
    public MoveState moveState;
    private Vector3 combatPos = new Vector3(0,0,0);
    private Vector3 sortingGroupFolderPos = new Vector3(0, 0, 0);
    public GameObject TranformOffsetFolder;
    public float moveToGridTimer;
    public float combatPosDistance = 1;
    [HideInInspector] public float moveSpeed;
    [Range(6, 10)]
    public float moveSpeedMax;

    [Space(5)]
    [Header("ANIM")]
    public SkeletonAnimation skeletonAnim;
    public SortingGroup sortingGroup;
    public bool jumpIntoBattle = false;
    [Space(5)]
    [Header("UI")]
    public GameObject canvas, popTextCanvas;
    public UnitBarController UnitBar;
    public List<textInformation> popTextString;
    private float popTextTime;
    public Image[] BuffImages;
    private float timer_GoBack;
    private int addOnLayer;
  
    private void Awake()
    {
        addOnLayer = 0;
        //[If unit is not in battle, it will not Update.]
        if (GameObject.FindGameObjectWithTag("GameController") != null)
        {
            isInBattle = true;
            GC = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        }
        else isInBattle = false;

        popTextString = new List<textInformation>();
        this.transform.eulerAngles = new Vector3(0, 0, 0);
        sortingGroupFolderPos = TranformOffsetFolder.transform.parent.transform.localPosition;
        moveSpeed = moveSpeedMax;
    }

    void Start()
    {
        if(nodeAt == null) UnitFunctions.PlaceUnitOnTile(this);

        healthLost_ThisTurn = 0;

        if (unitAttribute == UnitAttribute.corpse || unitAttribute == UnitAttribute.staticObject) 
        { SetAsCorpse(); return; }

        if (!isInBattle) return;

        InputCurrentData();
        BuffUpdate(true);
        UnitEnable(true);

        GC.Check_UnitInList();
        Check_MoveToNewGrid(nodeAt, this);

        if (skeletonAnim.enabled) 
        {
            bool noAnim = false;
            if (data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._1_Avatar)
                if (FindObjectOfType<StoryManager>() != null) 
                    if (FindObjectOfType<StoryManager>().level == StoryManager.Story_Level.level1_1) noAnim = true;

            if (noAnim) return;

            if (this.unitTeam == UnitTeam.playerTeam)
            {
                InputAnimation("enterBattle");
            }

            else if(GC.storyState != GameController._storyState.setup)
            {
                if (jumpIntoBattle) InputAnimation("enterBattle2");
                else InputAnimation("enterBattle");
            }
            else if (!jumpIntoBattle) InputAnimation("idle");
        } 
    }
    public void InputData(UnitData data)
    {
        Debug.Log("LoadDataFromSave" + this.name);
        this.data = new UnitData(data);
    }
    private void InputCurrentData()
    {
        Debug.Log("LoadSave" + this.name);
        currentData = new UnitData(data);
        UnitBar.Set_Bar_Initial(this);
        UnitBar.Set_Icons(unitTeam, isActive);
    }

    public void SetAsCorpse()
    {
        if (unitTeam == UnitTeam.playerTeam && GameController.playerList != null) GameController.playerList.Remove(this);
        if (unitTeam == UnitTeam.enemyTeam && GameController.enemyList != null) GameController.enemyList.Remove(this);
        unitTeam = UnitTeam.neutral;
        nodeAt.unit = this;
        this.currentData = new UnitData(data);
        this.gameObject.transform.position = nodeAt.transform.position;
        UnitBar.SetAsCorpse();
        UnitBar.Set_Bar_Initial(this);
    }

    public void InputAnimation_Single(string animString)
    {
        if (this.skeletonAnim == null) return;

        if (skeletonAnim.AnimationState.bool_HaveAnimation(animString))
        {
            this.skeletonAnim.AnimationState.SetAnimation(0, animString, true);

            if (this.skeletonAnim.AnimationState != null)
                this.skeletonAnim.AnimationState.Event += GC.MSE.HandleEvent;
        }
    }

    public void InputAnimation_GainFP(string animString)
    {
        if (this.skeletonAnim == null) return;

        if (skeletonAnim.AnimationState.bool_HaveAnimation(animString))
        {
            if (skeletonAnim.AnimationState.ToString() == "idle")
            {
                this.skeletonAnim.AnimationState.SetAnimation(0, "gainBuff", true);
                this.skeletonAnim.AnimationState.AddAnimation(0, "idle", true, 0);
            }
        }
    }
    public void AddAnimation(string animString)
    {
        if (this.skeletonAnim == null) return;
        this.skeletonAnim.AnimationState.AddAnimation(0, animString, true, 0);
    }
    public void InputAnimation(string animString)
    {
        if (this.skeletonAnim == null) return;

        if (skeletonAnim.AnimationState.bool_HaveAnimation(animString))
        {
            this.skeletonAnim.AnimationState.SetAnimation(0, animString, true);

            if (this.skeletonAnim.AnimationState != null)
                this.skeletonAnim.AnimationState.Event += GC.MSE.HandleEvent;
        }

        bool enterIdle = true;
        if (this.gameObject.GetComponent<UnitAI>() != null)
        {
            UnitAI ai = this.gameObject.GetComponent<UnitAI>();
            if (ai.unitHoldingSkill)
            {
                enterIdle = false;
                if (ai.skill_HoldFromLastTurn.skill.type == Skill._Type.MoveInLine) this.skeletonAnim.AnimationState.AddAnimation(0, "chargehold", true, 0);
            }
        }
        if(enterIdle)
        this.skeletonAnim.AnimationState.AddAnimation(0, "idle", true, 0);
    }
    public void UnitEnable(bool enable)
    {
        if (unitAttribute != UnitAttribute.alive) return;

        if (enable)
        { isActive = true; canFightBack = true; currentData.movePointNow = currentData.movePointMax; isSelectedInThisTurn = false; }
        
        else
        {  isActive = false; canFightBack = false; }

        UnitBar.Set_Icons(unitTeam, isActive);
        UnitBar.Set_Bar(this);
        BuffUpdate(false);
    }

    private void FixedUpdate()
    {
        if (!isInBattle) return;


        int A = (int)(GridManager.GetY(nodeAt.y) * -100);

        //Debug.Log(A);
        float B_ = nodeAt.transform.position.y - TranformOffsetFolder.transform.position.y;
        int B = (int)(B_ * -100);
        this.sortingGroup.sortingOrder = A-B+50 + addOnLayer;
        if (popTextString.Count > 0)
        {
            UIFunctions.Gene_PopText(popTextCanvas, popTextString[0]);
            popTextString.Remove(popTextString[0]);
        }



        switch (moveState)
        {
            case MoveState.combatMove_StayOnMoveToGrid:
                
                moveToGridTimer -= Time.fixedDeltaTime;
                if (moveToGridTimer < 0)
                {
                    canFlip = false;
                    moveState = MoveState.combatMove_Back;
                    timer_GoBack = 1f;
                }

                break;

            case MoveState.combatMove_Back:

                float dist = Vector3.Distance(TranformOffsetFolder.transform.position, TranformOffsetFolder.transform.parent.position);
                timer_GoBack -= Time.fixedDeltaTime;
                if (dist < 0.001f || timer_GoBack < 0)
                {
                    addOnLayer = 0;
                    canFlip = false;
                    moveState = MoveState.standing;
                    TranformOffsetFolder.transform.localPosition = new Vector3(0, 0, 0);
                    moveSpeed = moveSpeedMax;     
                }
                else
                {
                    TranformOffsetFolder.transform.position = Vector3.Lerp(TranformOffsetFolder.transform.position, TranformOffsetFolder.transform.parent.position, Time.fixedDeltaTime * 8f);         
                }
                   
             
                break;

            case MoveState.combatMove_Forward:
                
                timer_GoBack -= Time.fixedDeltaTime;

                float dist2 = Vector3.Distance(TranformOffsetFolder.transform.position, TranformOffsetFolder.transform.parent.position);
                if (dist2 > combatPosDistance || timer_GoBack < 0)//[Knock back clear, back to normal position]
                {
                    moveState = MoveState.combatMove_StayOnMoveToGrid;
                }
                else TranformOffsetFolder.transform.position = Vector3.Lerp(TranformOffsetFolder.transform.position, combatPos, Time.fixedDeltaTime * moveSpeed);

                break;

            case MoveState.moving:
                Moving_InMoveState(false);
                break;

            case MoveState.standing:
                this.canFlip = false;
                Moving_InMoveState(false);
                break;

            case MoveState.moving_Down:
                Moving_InMoveState(false);
                break;

            case MoveState.knockBack_Moving:
                KnockBack_Moving();
                break;
        }
    }



    private void KnockBack_Moving()
    {
        float dist2 = Vector3.Distance(this.transform.position, combatPos);
        if (dist2 < 0.6f)//[Knock back clear, back to normal position]
        {
            TranformOffsetFolder.transform.localPosition = new Vector3(0, 0, 0);
            moveState = MoveState.moving;
        }
        else this.transform.position = Vector3.Lerp(this.transform.position, combatPos, 0.6f * Time.fixedDeltaTime * moveSpeed);
    }


    public static void LerpMoving(Transform A,Vector3 B, float moveSpeed)
    {
        float speedModi = 1;
        float dist = Vector3.Distance(A.position,B);
        if (dist > 0.1)
        {
            speedModi = 1 + (dist * 0.6f);
            if (speedModi > 3) speedModi = 3;
        }
        A.position = Vector3.MoveTowards(A.position, B, Time.fixedDeltaTime * moveSpeed * speedModi * 0.7f);
    }

    private void Moving_InMoveState(bool isCharging)
    {
        //Normal Grid
        Vector3 pos = UnitFunctions.UnitPosition_WithSize(this, data);
        if (isCharging) pos = combatPos;

        LerpMoving(transform, pos, moveSpeed);
        if (!GC.isAttacking)
            if (canFlip && canNormalFlip)
                UnitFunctions.Flip(xNow, transform.position.x, this);

        xNow = transform.position.x;
        float dist = Vector3.Distance(transform.position, pos);

        if (dist < 0.005f)
        {
            moveSpeed = moveSpeedMax;
            moveState = MoveState.standing;
            TranformOffsetFolder.transform.localPosition = new Vector3(0, 0, 0);
            TranformOffsetFolder.transform.parent.localPosition = sortingGroupFolderPos;
            //sortingGroup.sortingOrder = 0;
            if (isCharging)
            {
                moveState = MoveState.combatMove_Back;
            }
        }
    }

    //[Attack, Knock Back, Or charge, Attack one line, Use this to move unit]
    public void Input_CombatPos(Vector3 pos, float moveToGridSpeedModi, float moveToGridRatio, float moveToGridWait)
    {
        if (unitAttribute != UnitAttribute.alive) return;
        combatPos = pos;
        combatPosDistance = 1.3f * moveToGridRatio;
        this.moveSpeed = moveSpeedMax * moveToGridSpeedModi;
        this.moveToGridTimer = moveToGridWait;
        moveState = MoveState.combatMove_Forward;
        timer_GoBack = 1f;
        addOnLayer = 15;
    }

  
    public void Input_CombatPos_StraightToStaying(Vector3 pos, float moveToGridSpeedModi, float moveToGridRatio, float moveToGridWait)
    {
        if (unitAttribute != UnitAttribute.alive) return;
        canFlip = false;
        combatPos = pos;
        combatPosDistance = 1.3f * moveToGridRatio;
        this.moveSpeed = 6f * moveToGridSpeedModi;
        this.moveToGridTimer = moveToGridWait;
        moveState = MoveState.knockBack_Moving;
    }


    public void Check_MoveToNewGrid(PathNode moveToNode, Unit unit)
    {
        if (moveToNode.type == PathNode.GridType.normal)
        {
            RemoveDebuff_StandOnWater();
        }

        if (moveToNode.type == PathNode.GridType.water)
        {
            StartCoroutine(AddBuff(Resources.Load<Buff>("Buff/Stand On Water"), false));
        }

        this.BuffUpdate(false);
    }

    //[Each time move in one grid, Gamecontroller will send this.]
    public bool UnitPosition_CanMove(PathNode moveToNode, int moveCost, bool canFlip, float moveSpeedModi, bool force_IdleAnimation)
    {
        if (unitAttribute != UnitAttribute.alive) return false;
        //isMoving = true;
        this.canFlip = canFlip;
        moveSpeed = moveSpeedMax * moveSpeedModi;
        if (currentData.movePointNow - moveCost < 0) return false;
        if (moveToNode.unit != null && moveToNode.unit != this) return false;
        if (moveToNode.isBlocked) return false;

        if (force_IdleAnimation)
        {
            InputAnimation_Single("idle");
        }

        Check_MoveToNewGrid(moveToNode, this);

        currentData.movePointNow -= moveCost;
        moveState = MoveState.moving;
        UnitBar.Set_Bar(this);

        if (data.unitSize == UnitData.Unit_Size.size3_OnLeftNode || data.unitSize == UnitData.Unit_Size.size3_OnRightNode || data.unitSize == UnitData.Unit_Size.size3_OnTopNode)
            UnitFunctions.MoveUnitToNewGrid_Size3(this, moveToNode);

        if (data.unitSize == UnitData.Unit_Size.size2_OnLeftNode || data.unitSize == UnitData.Unit_Size.size2_OnRightNode)
            UnitFunctions.MoveUnitToNewGrid_Size2(this, moveToNode);


        //[offset]
        else if (data.unitSize == UnitData.Unit_Size.size1)
        {
            UnitFunctions.MoveUnitToNewGrid_Size1(this, moveToNode);
        }
        
        return true;
    }

   
    private IEnumerator AddBuff(Buff buff, bool canRepeatBuff)
    {
        yield return new WaitForSeconds(0.02f);
        InputBuff(buff, canRepeatBuff);
    }

    private void RemoveDebuff_StandOnWater()
    {
        List<_Buff> newBuff = new List<_Buff>();
  

        foreach (_Buff buff in buffList)
        {
            if (buff.buff.standOn_Water)
            {

            }
            else newBuff.Add(buff);
        }

        buffList = newBuff;
    }

    //[Input Buff]
    public void InputBuff(Buff buff, bool canBuffRepeat)
    {
        if (this.unitAttribute != UnitAttribute.alive) return;

        bool buffAddTimerToOldBuff = false;

        if (!canBuffRepeat)
        {
            foreach (_Buff _buff in buffList)
            {
                if (_buff.buff == buff)
                {
                    _buff.remainTime = buff.remainTime;
                    buffAddTimerToOldBuff = true;
                }
            }
        }

        if (buffAddTimerToOldBuff == true) return;
        buffList.Add(new _Buff(buff));

        //[Define Debuff or Buff]
        string __buff = "Buff";  
       
        if (!buff.isBuff)
        {
            __buff = "Debuff";
            if(buff.buffType == Buff._buffType.tuant) __buff = "Taunt";
        }

       
        //[Add Buff PopText]
        popTextString.Add(new textInformation(__buff, buff.sprite));

        //[If buff add instant stats change, input here.]
        if (buff.movePointMax > 0)
        {
            currentData.movePointNow += buff.movePointMax;
        }

        if (buff.healthMax > 0)
        {
            currentData.healthMax += buff.healthMax;
            HealthChange(buff.healthMax, 0,"Heal");
        }

        if (buff.armorMax > 0)
        {
            currentData.armorMax += buff.armorMax;
            ArmorChange(buff.healthMax);
        }
        BuffUpdate(false);
    }

    //[Update Buff, it will not decease Buff remain time.]
    public void BuffUpdate(bool firstTime)
    {
        UnitFunctions.ResetUnitValueToUnitData(this, this.data);

        foreach (_Buff _buff in data.traitList)
        { UnitFunctions.CheckBuff(this.currentData, _buff.buff,this);
            if (firstTime) this.currentData.healthNow += _buff.buff.healthMax;
        }

        foreach (_Buff _buff in buffList)
        { UnitFunctions.CheckBuff(this.currentData, _buff.buff,this); }
        DisplayAllBuffs();

        if (this.currentData.healthNow > this.currentData.healthMax) this.currentData.healthNow = this.currentData.healthMax;
    }

    //[Reflesh Buff, it will decease Buff remain time.]
    public void Buff_Reflesh()
    {
        //[Loop though traits]
        if (data.traitList != null && data.traitList.Count > 0)
            for (int i = 0; i < data.traitList.Count; i++)
            { Buff_Update_PerTurn(data.traitList[i].buff); }

        //[Loop though buffs]
        if (buffList != null && buffList.Count > 0)
        {
            List<_Buff> newBuff = buffList;
            List<_Buff> newBuff2 = buffList;

            for (int i = 0; i < newBuff2.Count; i++)
            {
                newBuff2[i].remainTime--;
                Buff_Update_PerTurn(newBuff2[i].buff);

                if (newBuff2[i].remainTime <= 0)
                    newBuff.Remove(newBuff2[i]);
            }
            buffList = newBuff;
        }
        DisplayAllBuffs();
    }

    //[For those buff update once per turn]
    public void Buff_Update_PerTurn(Buff buff)
    {
        Unit unit = this;

        if (buff.standAlone == true && UnitFunctions.Check_TriggerStandAlone(unit.nodeAt, unit.unitTeam))
        {
            unit.popTextString.Add(new textInformation("<color=#B7FF74>Buff:</color>", Resources.Load<Sprite>("Image/Standalone")));
            unit.currentData.damage.damBonus += 2;
            unit.currentData.dodge += 10;
        }

        if (buff.aura_GrowingDeath)
        {
            UnitFunctions.Check_Aura_OfGrowingDeath(this.nodeAt, unitTeam);
        }

        if (buff.aura_Tarish)
        {
            UnitFunctions.Check_Aura_OfTarish(this.nodeAt, unitTeam);
        }

        switch (buff.changePerTurnType)
        {
            case Buff._changePerTurnType.None:
                break;

            case Buff._changePerTurnType.Bleeding:
                unit.HealthChange(buff.changeNum,0,"Bleeding");
                break;

            case Buff._changePerTurnType.Poisoning:
                unit.HealthChange(buff.changeNum,0,"Poison");
                break;

            case Buff._changePerTurnType.Healing:
                unit.HealthChange(buff.changeNum,0,"Heal");
                break;

            case Buff._changePerTurnType.Rusting:
                //unit.popTextString.Add(new textInformation(color1 + buff.changeNum + color2, Resources.Load<Sprite>("Image/Armor")));
                ArmorChange(buff.changeNum);
                break;
        }
    }

    public void CD_Reflesh()
    {
        foreach (_Skill skill in currentData.Skill)
        {
            skill.CD--;
        }
    }

    private void DisplayAllBuffs()
    {
        //Debug.Log(name);
        foreach (Image img in BuffImages)
        {
            img.gameObject.SetActive(false);
        }

        for (int i = 0; i < BuffImages.Length; i++)
        {
            if (buffList.Count == i) break;

            BuffImages[i].gameObject.SetActive(true);
            BuffImages[i].gameObject.GetComponent<BuffDisplay>().buff = buffList[i];

            if (buffList[i].buff.sprite != null)
                BuffImages[i].sprite = buffList[i].buff.sprite;
        }
    }



    ///////////////////////////////////////////////////////////////
    /// ValueChange /// ValueChange /// ValueChange /// ValueChange
    /// ValueChange /// ValueChange /// ValueChange /// ValueChange
    /// ValueChange /// ValueChange /// ValueChange /// ValueChange
    /// ValueChange /// ValueChange /// ValueChange /// ValueChange
    ///////////////////////////////////////////////////////////////
    ///

    public void ArmorChange(int value)
    {
        //[Add PopText]
        string color1 = "<color=#FFFFFF>";
        string string2 = "";
        if (value > 0) { color1 = "<color=#FFFFFF>"; string2 = "+"; }
        string text = color1 + string2 + value + "</color>";
        popTextString.Add(new textInformation(text, Resources.Load<Sprite>("Image/Armor")));

        currentData.armorNow += value;
        if (currentData.armorNow < 0) currentData.armorNow = 0;
        if (currentData.armorNow > currentData.armorMax) currentData.armorNow = currentData.armorMax;

        //armorText.text = currentData.armorNow.ToString();
    }


    public void SelfDestroy()
    {
        Gene_DropFlesh(1);
        HealthChange(-100, 0, "Damage");
    }

    public void HealthChange(int value, int rateToGeneCorpse,string damageType)
    {
        if (damageType == "Damage")
            GC.cameraAnim.SetTrigger("Shake1");

        if (unitAttribute != UnitAttribute.alive && value > 0) return;

        if (value <= 0 && unitAttribute == UnitAttribute.alive)
        {
            if (this.gameObject.GetComponent<UnitAI>() != null)
            {
                UnitAI ai = this.gameObject.GetComponent<UnitAI>();
                if (ai.unitHoldingSkill) if (value >= 3) ai.CancelUnitHoldAction();
            }

            if (unitSpecialState == UnitSpecialState.boneFireTower)
            {
                this.GetComponent<EnemyBoneFire>().anim.SetTrigger("hurt");
            }

            InputAnimation("hurt");

            //If Drop Flesh
            if (this.unitTeam == UnitTeam.enemyTeam)
            {
                if (this.unitSpecialState == UnitSpecialState.normalUnit)
                {
                    healthLost_ThisTurn += Mathf.Abs(value);
                    if (healthLost_ThisTurn > data.healthMax/data.FleshDrop)
                    {
                        healthLost_ThisTurn = 0;
                        if (currentData.FleshDrop > 0)
                        {
                            Debug.Log("haha");
                            currentData.FleshDrop--; 
                            Gene_DropFlesh(1);
                        }
                    }
                }
            }
        }

        //[Add PopText]
        string color1 = "<color=#FFFFFF>";
        string string2 = "";
        if (value > 0) { color1 = "<color=#FFFFFF>"; string2 = "+"; }
        string text = color1 + string2 + value + "</color>";

        //Debug.Log(popTextString.Count);
        //Heal
        //Damage
        //Poison
        //Sword
        //Star...
        popTextString.Add(new textInformation(text, Resources.Load<Sprite>("Image/"+ damageType)));
        //Debug.Log(popTextString.Count);
        currentData.healthNow += value;


        if (currentData.healthNow > currentData.healthMax) currentData.healthNow = currentData.healthMax;

        if (currentData.healthNow <= 0)
        {
            currentData.healthNow = 0;
            isActive = false;
            StartCoroutine(Death(rateToGeneCorpse));
        }

        // healthText.text = currentData.healthNow.ToString();
        UnitBar.Set_Bar(this);
    }

    public void FleshChange(int value)
    {
        if (unitTeam != UnitTeam.playerTeam) return;
        popTextString.Add(new textInformation("+" + value, Resources.Load<Sprite>("Image/Flesh")));
        SaveData.flesh += value;
    }


    public void Gene_DropFlesh(int value)
    {
        Debug.Log(currentData.FleshDrop +"/"+ value);
        List<PathNode> NearbyPath = GameFunctions.FindNodes_ByDistance(this.nodeAt, 1, false);
        List<PathNode> NearbyPath_Empty = new List<PathNode>();
        NearbyPath_Empty.Add(this.nodeAt);
   
        for (int i = 0; i < value; i++)
        {
            if (NearbyPath_Empty != null && NearbyPath_Empty.Count > 0)
            {
                PathNode targetLocation = NearbyPath_Empty[Random.Range(0, NearbyPath_Empty.Count)];
                if (targetLocation != null)
                {
                    GameObject Flesh = Instantiate(Resources.Load<GameObject>("Flesh_Drop"), this.transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity);
                    Flesh.GetComponent<FleshDrop>().InputDropLocation(targetLocation, false);
                }
            }
        }
    }


    ///////////////////////////////////////////////////////////
    /// Death /// Death /// Death /// Death /// Death /// Death
    /// Death /// Death /// Death /// Death /// Death /// Death
    /// Death /// Death /// Death /// Death /// Death /// Death
    /// Death /// Death /// Death /// Death /// Death /// Death
    ///////////////////////////////////////////////////////////

    IEnumerator Death(int attacker_Modi_KillFP)
    {
        int FP = currentData.FleshDrop;
        FP += attacker_Modi_KillFP;

        if (FP > 0 && this.unitTeam != UnitTeam.playerTeam)
        {
            Gene_DropFlesh(FP);
        }
        FindObjectOfType<SFX_Controller>().InputVFX_Simple(14);

        GC.CheckCampFire();


        if (this.unitTeam == UnitTeam.enemyTeam)
        {
            if (this.GetComponent<UnitAI>().VisualGuideFolder != null)
            {
                Destroy(this.GetComponent<UnitAI>().VisualGuideFolder);
            }

            if (this.GetComponent<BossAI>() != null)
            {
                this.GetComponent<BossAI>().Destroy_NodeGuideInHold();
            }
        }

        if (GC.selectedUnit != null)
            if (GC.selectedUnit == this)
            {
                GC.Deselect_Unit();
            }

        isActive = false;
        unitTeam = UnitTeam.neutral;
        this.tag = "Untagged";

        GC.Check_UnitInList();
        GC.Check_EndGame();
        canvas.GetComponent<DestroyInTime>().enabled = true;
        canvas.transform.SetParent(GC.transform);

        UnitBar.Disactive_AllIcon();

        if (unitAttribute == UnitAttribute.alive)
        {
            int rateToGeneBody = 0;

            switch (deathData.deathType)
            {
                case UnitDeathOption.Unit_Death_Type.neverGeneBody:
                    rateToGeneBody = -1000;
                    break;

                case UnitDeathOption.Unit_Death_Type.alwaysGeneBody:
                    rateToGeneBody = 1000;
                    break;

                case UnitDeathOption.Unit_Death_Type.chanceToGeneBody:
                    rateToGeneBody = deathData.rate_GeneCorpse;
                    break;
            }

            //if (isSummoned) rateToGeneBody = -1000;

            if (Random.Range(0, 100) < rateToGeneBody)
            {
                yield return new WaitForSeconds(0.2f);
                GameObject geneUnit = null;

                if (deathData.specialPrefab != null) geneUnit = deathData.specialPrefab;
                else geneUnit = UnitFunctions.Find_UnitCorpse(deathData.bodyType);

                if(geneUnit != null)
                SkillFunctions.Skill_GeneUnit(nodeAt, geneUnit, UnitTeam.neutral, null);
            }
            else nodeAt.unit = null;
        }

        if (unitAttribute == UnitAttribute.corpse || unitAttribute == UnitAttribute.staticObject)
        {
           nodeAt.unit = null;
        }


        if (this.data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._1_Avatar)
        {
            GC.PauseGame();
            InputAnimation_Single("dead");
            yield return new WaitForSeconds(8f);
            //Death panel

        }
        else yield return new WaitForSeconds(0.3f);

        Destroy(this.gameObject);
    }
}

