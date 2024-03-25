using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridFallController : MonoBehaviour
{
    public enum Level { LV1_1, LV2_1, LV2_2 }
    public Level level;

    public int timer;
    private GameController GC;
    private bool startFall = false;
    public int order;
    public GameObject ROCK;

    private void Start()
    {
        GC = FindObjectOfType<GameController>();
        timer = 0;
        order = 0;
        startFall = false;
    }

    public void DecreaseTimer()
    {
        if (!startFall) return;

        timer--;
        if (timer <= 0)
        {
            Fall();
        }
    }
    public void StartAOE1Shock(PathNode path)
    {
        StartCoroutine(RevivalController.GridDynamic_ShockWave(path, "FireDown"));
    }
    public void StartFalling()
    {
        startFall = true;
        Fall();
        StartCoroutine(GridFall_Loop());
    }

    private void Fall()
    {
        timer = 1;
        order++;
        if (order >= FindObjectOfType<MapData>().heightChangeStageNum) return;

        if (order == 1 && level == Level.LV1_1) StartCoroutine(GridFall_0());
        else StartCoroutine(GridFall_Normal());
    }

    public IEnumerator GridFall_0()
    {
        timer = 3;
        GameController.frozenGame = true;
        SFX();
        GeneRocksAroundPlayer(10);
        AllGridShake();
        yield return new WaitForSeconds(0.35f);
        GridFall();

        yield return new WaitForSeconds(0.3f);
        GameController.frozenGame = false;
    }

    public IEnumerator GridFall_Normal()
    {
        GameController.frozenGame = true;
        SFX();
        GeneRocksAroundPlayer(10);
        yield return new WaitForSeconds(0.1f);
        GridFall();
        yield return new WaitForSeconds(0.3f);
        GameController.frozenGame = false;
    }

    public void GridFall()
    {
        foreach (PathNode node in FindObjectsOfType<PathNode>())
        {
           CheckNodeMove(node, order, "Shake");
        }
    }

















    public void SFX()
    {
        GameController GC = FindObjectOfType<GameController>();
        GC.SC.StartRockFall();
        GC.cameraAnim.SetTrigger("Shake3");
    }
    public void GeneRocksAroundPlayer(int chance)
    {
        Unit player = AIFunctions.FindPlayer();
        List<PathNode> pathAroundPlayer = GameFunctions.FindNodes_ByDistance(player.nodeAt, 5, true);
        GameObject rock = ROCK;

        foreach (PathNode path in pathAroundPlayer)
        {
            if (Random.Range(0, 100) < chance)
            {
                Vector3 pos = path.transform.position + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0);
                Instantiate(rock, pos, Quaternion.identity);
            }
        }
    }

    public IEnumerator GridFall_Loop()
    {
        CamMove cam = FindObjectOfType<CamMove>();
        Transform camTrans = cam.transform;
        while(true)
        {
            yield return new WaitForSeconds(Random.Range(0.3f,0.69f));
            Vector3 pos = new Vector3(camTrans.position.x + Random.Range(3f, -3f), camTrans.position.y + Random.Range(9f, -3f), 0); 
            Instantiate(ROCK, pos, Quaternion.identity);
        }
    }


    public void AllGridShake()
    {
        foreach (PathNode path in FindObjectsOfType<PathNode>())
        {
            StartCoroutine(GridMove(path, "Shake", "", Random.Range(0, 0.3f)));
        }
    }
    public void GridListMove_Height(List<Vector2Int> Vectors, string trigger, string nextTrigger,int heightChange, float speedModi)
    {
        List<PathNode> myPath = FindObjectOfType<GridManager>().GetPathList(Vectors);
        if (myPath != null && myPath.Count != 0)
            foreach (PathNode path in myPath)
            {
                StartCoroutine(GridMove_Height(path, trigger, nextTrigger, Random.Range(0, 0.3f), heightChange, speedModi));
            }
    }
    public void GridMove_AnimationNext(PathNode node, string trigger)
    {
        StartCoroutine(GridMove(node, trigger,"", 0.02f));
    }
    private IEnumerator GridMove(PathNode node, string trigger, string nextTrigger, float timeWait)
    {
        yield return new WaitForSeconds(timeWait);
        GridFallController.AddNodeToDynamicFolder(node, trigger, nextTrigger);
    }
    private IEnumerator GridMove_Height(PathNode node, string trigger, string nextTrigger, float timeWait, int heightChange, float speedModi)
    {
        yield return new WaitForSeconds(timeWait);
        node.AddHeight(heightChange, speedModi);
        GridFallController.AddNodeToDynamicFolder(node, trigger, nextTrigger);
    }

    public static void AddNodeToDynamicFolder(PathNode node, string animTrigger, string nextTrigger)
    {
        if (node == null) return;
        if (node.dynamicFolder == null) Debug.Log("Node folder null:" + node.x + "," + node.y);
        node.dynamicFolder.InputAnimation(animTrigger, nextTrigger);
    }

    public void CheckNodeMove(PathNode node, int stage, string trigger)
    {
        if (stage < node.heightChangeList.Length)
        {
            float speedModi = 2.5f;
            string nextTrigger = "Shake_Loop";
          
            float time = node.y * 0.15f;
            int heightChange = node.heightChangeList[stage];
            if (heightChange == 0) nextTrigger = "";    
            if (stage < node.heightChangeList.Length - 1)
            {
                if (node.heightChangeList[stage+=1] < -50)
                {
                    nextTrigger = "Shake_G_Loop";
                    speedModi = 4;
                }
            }
            StartCoroutine(GridMove_Height(node, trigger, nextTrigger, time + Random.Range(0, 0.3f), heightChange, speedModi));
        }
    }

    //public static IEnumerator GridDynamic_ShockWave(PathNode startNode)
    //{
    //    List<PathNode> closedList = new List<PathNode>();
    //    List<PathNode> nodeInArea_2 = GameFunctions.FindNodes_ByDistance(startNode, 1, true);

    //    startNode.SetDynamicHeight_Stay(-0.35f, 1f);
    //    yield return new WaitForSeconds(0.1f);
    //    foreach (PathNode triggerednode in nodeInArea_2)
    //    {
    //        if (triggerednode != startNode) triggerednode.SetDynamicHeight_Stay(-0.25f,1f);
    //    }

    //    yield return new WaitForSeconds(0.15f);

    //    startNode.SetDynamicHeight_Stay(0.2f, .5f);
    //    foreach (PathNode triggerednode in nodeInArea_2)
    //    {
    //        if (triggerednode != startNode) triggerednode.SetDynamicHeight_Stay(0.15f, .5f);
    //    }
    //    yield return new WaitForSeconds(0.3f);

    //    startNode.SetDynamicHeight_Back(.5f);
    //    foreach (PathNode triggerednode in nodeInArea_2)
    //    {
    //        if (triggerednode != startNode) triggerednode.SetDynamicHeight_Back(.5f);
    //    }
    //}

    //public static IEnumerator GridDynamic_ShockWave(PathNode startNode, float timeGap)
    //{

    //    List<PathNode> closedList = new List<PathNode>();

    //    for (int i = 0; i < 4; i++)
    //    {
    //        if (i == 0)
    //        {
    //            closedList.Add(startNode);
    //            startNode.SetDynamicHeight(-0.2f);
    //            continue;
    //        }

    //        if (i == 1) yield return new WaitForSeconds(timeGap * 0.5f);
    //        if (i == 2) yield return new WaitForSeconds(timeGap * 0.95f);
    //        if (i == 3) yield return new WaitForSeconds(timeGap * 0.25f);


    //        List<PathNode> nodesInArea = GameFunctions.FindNodes_ByDistance(startNode, i, true);

    //        Debug.Log(i + ": " + nodesInArea.Count);
    //        if (nodesInArea == null) continue;
    //        if (nodesInArea.Count == 0) continue;

    //        //[Active]
    //        if (nodesInArea != null)
    //        {
    //            for (int i2 = 0; i2 < nodesInArea.Count; i2++)
    //            {
    //                bool canTrigger = true;
    //                foreach (PathNode triggerednode in closedList)
    //                {
    //                    if (triggerednode == nodesInArea[i2]) canTrigger = false;
    //                }
    //                if (canTrigger)
    //                {
    //                    Debug.Log(nodesInArea[i2].name);
    //                    closedList.Add(nodesInArea[i2]);
    //                    if (i == 1) nodesInArea[i2].SetDynamicHeight(-0.2f);
    //                    if (i == 2) nodesInArea[i2].SetDynamicHeight(-0.2f);
    //                    if (i == 3) nodesInArea[i2].SetDynamicHeight(0.1f);
    //                }
    //            }
    //        }
    //    }
    //}
}
