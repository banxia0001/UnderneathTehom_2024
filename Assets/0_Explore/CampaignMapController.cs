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
    public GameObject PlayerIcon,PlayerLight,WorldFolder;
    public GameObject LV2ICON,LV3ICON;
    public float offsetRate;
    private float xWas;
    private Vector3 orginPos;

    [Header("Debug")]
    public bool debugMode;
    public List<RewardList> debugList;
    public List<RewardList> debugList_FastStart;

    [Header("SFX")]
    public AudioSource click; 
    public AudioSource reward, popup;

    [Header("MISC")]
    public GameObject tile1;
    public GameObject tile2;

    private void Awake()
    {
        orginPos = WorldFolder.transform.position;

        CEC.gameObject.SetActive(false);
    
        LV2ICON.SetActive(false);
        LV3ICON.SetActive(false);

        if (SaveData.quickStart_FightBoss)
        {
            SaveData.AreaAt = 1;
            SaveData.rewardList = debugList_FastStart;
        }
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

            LV2ICON.SetActive(true);
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
                                state = State.inCampaignMap_Moving;
                                order = 0;
                                NodeWasAt = NodeAt;
                                NodeAt = NodeMouseAt;
                                StartCoroutine(EnterNewZone_1());
                            }
            }
        }
       
        else if(state == State.inCampaignMap_Moving)
        {
            MovingOnMap();
        }
    }

    public int order;
    public List<Transform> gotoList;
    public void MovingOnMap()
    {
        if (order == gotoList.Count)
        {
            state = State.inEventChoice;
            StartCoroutine(EnterNewZone_2());
            return;
        }

        PlayerIcon.transform.position = Vector2.MoveTowards(PlayerIcon.transform.position, gotoList[order].transform.position, Time.deltaTime * 20f);

        Vector3 nowPos = PlayerIcon.transform.position;
        Vector3 toPos1 = gotoList[order].transform.position;
        float dist1 = Vector3.Distance(nowPos, toPos1);

        if (dist1 < 0.001f) order++;
    }

   
    private void RaycastOnTerrain()
    {
        if (UI.isOnUI) return;

        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      
        //if (pos.x > xWas) offsetRate += Time.deltaTime;
        //if (pos.x < xWas) offsetRate -= Time.deltaTime;
        //xWas = pos.x;

        //WorldFolder.transform.position =  orginPos + new Vector3(1.2f * offsetRate, 0.35f * offsetRate, 0);
        //WorldFolder.transform.eulerAngles =   new Vector3(0, 0, -0.2f * offsetRate);

        PlayerLight.transform.position = pos;
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, LayerMask.GetMask("Ground"));

        if (hit.collider != null)
        {
            if (hit.collider.gameObject.transform.parent.GetComponent<CampaignMapEvent>() != null)
            { 
                NodeMouseAt = hit.collider.gameObject.transform.parent.GetComponent<CampaignMapEvent>();
            }
        }   
    }

    private IEnumerator EnterNewZone_1()
    {
     
        LV2ICON.SetActive(false);
        LV3ICON.SetActive(false);

        StartCoroutine(NodeAt.ShakeSprite());
        gotoList = NodeWasAt.walkingLines;
        gotoList.Add(NodeAt.transform);

        if (NodeAt.NodeCanGo.gameObject.name == "SLOT (4) Fight")
        {

            LV2ICON.SetActive(true);
        }

        if (NodeAt.NodeCanGo.gameObject.name == "SLOT (6) BossFight")
        {

            LV3ICON.SetActive(true);
        }

        if (NodeAt.gameObject.name == "SLOT (6) BossFight")
        {

            LV3ICON.SetActive(true);
        }

        if (NodeAt.gameObject.name == "SLOT (4) Fight")
        {

            LV2ICON.SetActive(true);
        }

        yield return new WaitForSeconds(1f);
        CloseAllSprite(NodeWasAt, true, this);

   
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
        yield return new WaitForSeconds(Random.Range(0.1f,1.2f));
        //Debug.Log("Collapse3");

        tile.GetComponent<Animator>().SetTrigger("fall");
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
