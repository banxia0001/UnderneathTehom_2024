using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CampaignMapController : MonoBehaviour
{
    public static bool gameFrozen;
    public enum State { inCampaignMap,inCampaignMap_Moving,inReward, inEventChoice}
    public State state;
    private RewardUI rewardUI;
    private CampaignMapUI UI;
    public CampaignEventController CEC;
    public UnitData namelessOneData;
    public List<UnitData> servantsData;

    [Header("Movement")]
    public CampaignMapEvent NodeAt;
    public CampaignMapEvent NodeMouseAt;
    public CampaignMapEvent NodeWasAt;
    public List<CampaignMapEvent> AllArea;
    public GameObject PlayerIcon;


    [Header("Debug")]
    public bool debugMode;
    public List<RewardList> debugList;

    [Header("SFX")]
    public AudioSource click; 
    public AudioSource reward, popup;

    [Header("MISC")]
    public GameObject tile1;
    public GameObject tile2;

    private void Awake()
    {
        CEC.gameObject.SetActive(false);

        if (SaveData.AreaAt == 0)
        {
            PlayerIcon.transform.position = AllArea[0].transform.position;
            NodeAt = AllArea[0];
            SaveData.rewardList = new List<RewardList>();
            SaveData.rewardList = NodeAt.rewardList;
            StartCoroutine(AllArea[0].ShakeSprite());
        }

        if (SaveData.AreaAt == 1)
        {
            tile1.SetActive(false);
            tile2.SetActive(false);
            PlayerIcon.transform.position = AllArea[4].transform.position;
            NodeAt = AllArea[4];
            CloseAllSprite(AllArea[0],false,this);
            CloseAllSprite(AllArea[1],false,this);
            CloseAllSprite(AllArea[2],false,this);
            CloseAllSprite(AllArea[3],false,this);
            StartCoroutine(AllArea[4].ShakeSprite());
        }

        SaveData.firstEnterComat_LV_1_1 = false;

        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;
        UI = FindObjectOfType<CampaignMapUI>(true);
        rewardUI = FindObjectOfType<RewardUI>(true);

        if (debugMode)
        {
            InputReward(debugList); 
        }
        else
        {
            if (SaveData.rewardList != null) InputReward(SaveData.rewardList);
            else rewardUI.gameObject.SetActive(false);
        }
    }
    public void ReturningFromRewardPanel()
    {
        state = State.inCampaignMap;
        this.namelessOneData = SaveData.namelessOneData;
        this.servantsData = SaveData.servantsData;

        if (SaveData.AreaAt == 0)
        {
            CollaspeOfTile(tile1);
            CollaspeOfTile(tile2);
        }  
    }
    public void InputReward(List<RewardList> rewardList)
    {
        state = State.inReward;
        rewardUI.gameObject.SetActive(true);
        rewardUI.InputReward_Setup(rewardList);
    }

    public void Update()
    {
        if (state == State.inCampaignMap)
        {
            RaycastOnTerrain();

            if (Input.GetMouseButtonDown(0))
            {
                click.Play();
                if (NodeMouseAt != null)
                    if (NodeAt != null)
                        if (NodeAt != NodeMouseAt)
                            if (NodeMouseAt == NodeAt.NodeCanGo)
                            {
                                NodeWasAt = NodeAt;
                                NodeAt = NodeMouseAt;
                                EnterNewZone_1();
                            }
            }
        }
       
        else if(state == State.inCampaignMap_Moving)
        {
            LerpMoving(PlayerIcon.transform, NodeAt.transform.position, 2);
            float dist = Vector3.Distance(PlayerIcon.transform.position, NodeAt.transform.position);

            Debug.Log(NodeAt.name + ":" + dist);
            if (dist < 0.02f)
            {
                StartCoroutine(EnterNewZone_2());
            }
        }
    }
    public static void LerpMoving(Transform A, Vector3 B, float moveSpeed)
    {
        float speedModi = 1;
        float dist = Vector3.Distance(A.position, B);
        if (dist > 0.1)
        {
            speedModi = 1 + (dist * 0.6f);
            if (speedModi > 3) speedModi = 3;
        }
        A.position = Vector3.MoveTowards(A.position, B, Time.fixedDeltaTime * moveSpeed * speedModi * 3.5f);
    }

    private void RaycastOnTerrain()
    {
        if (UI.isOnUI) return;

        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, LayerMask.GetMask("Ground"));

        if (hit.collider != null)
        {
         
            if (hit.collider.gameObject.transform.parent.GetComponent<CampaignMapEvent>() != null)
            { 
                NodeMouseAt = hit.collider.gameObject.transform.parent.GetComponent<CampaignMapEvent>();
            }
        }   
    }

    private void EnterNewZone_1()
    {
        CloseAllSprite(NodeWasAt, true, this);
        StartCoroutine(NodeAt.ShakeSprite());
        state = State.inCampaignMap_Moving;
    }

    private IEnumerator EnterNewZone_2()
    {
        state = State.inEventChoice;
        //Debug.Log("EnterLocation");
        yield return new WaitForSeconds(0.1f);

        if (NodeWasAt != null)
        {
            //Debug.Log("Collapse");
            yield return new WaitForSeconds(0.1f);
        }

        CEC.gameObject.SetActive(true);
        CEC.InputEvent(NodeAt);
    }

    public void EnterCombat_1(int num)
    {
        if (gameFrozen == true)return;
        if (num == 3) SaveData.AreaAt = 1;
        if (num == 4) SaveData.AreaAt = 2;
        gameFrozen = true;
        StartCoroutine(EnterCombat(num));
    }
    private IEnumerator EnterCombat(int num)
    {
        UI.loadingPanel.SetBool("Trigger", true);
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(num);
    }

    public void CollaspeOfTile(GameObject tile)
    {
        StartCoroutine(CollaspeOfTile_2(tile));
    }
    public IEnumerator CollaspeOfTile_2(GameObject tile)
    {
        tile.GetComponent<Animator>().SetTrigger("trigger2");
        yield return new WaitForSeconds(Random.Range(0.2f,1f));
        //Debug.Log("Collapse3");
        tile.GetComponent<Rigidbody2D>().gravityScale = 3f;

        yield return new WaitForSeconds(2f);
        tile.gameObject.SetActive(false);
    }

    public void CloseAllSprite(CampaignMapEvent node,bool showCollapse, CampaignMapController CM)
    {
        //Debug.Log("Collapse1");
        if (!showCollapse)
        {
            foreach (GameObject ob in node.CollapseSprite)
            {
                ob.SetActive(false);
            }
        }

        else
        {
            //Debug.Log("Collapse2");
            foreach (GameObject ob in node.CollapseSprite)
            {
                //Debug.Log("Name" + ob.name);
                CollaspeOfTile(ob);
            }
        }
        node.gameObject.SetActive(false);
    }
}
