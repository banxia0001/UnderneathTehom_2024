using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMove : MonoBehaviour
{
    public enum _CamState { inEditor, inGame }
    public _CamState state;

    public bool canMouseMove;

    private bool slowCamMode;
    public float speedModi = 1;
    private GameController GC;

    public Transform camFolder;
    public Camera cam;

    private float cam_xMin;
    private float cam_xMax;
    private float cam_yMax;
    private float cam_yMin;

    public Vector3 positionBefore;
    public float camZoomBefore;

    public float camZoom_Current;
    public float camZoom_Should;
    public float camTimer;
    public camZoomValue camZ;


    private void Start()
    {
        camZoom_Current = cam.orthographicSize;
        camZoom_Should = cam.orthographicSize;
        cam.gameObject.transform.localPosition = new Vector3(0, 0, -10);
        canMouseMove = true;
        if(Application.isEditor) canMouseMove = false;
        if (state == _CamState.inGame)
        {
            GC = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
            MapData MapData = FindObjectOfType<MapData>();
            cam_xMax = MapData.cam_xMax;
            cam_yMin = MapData.cam_yMin;
            cam_yMax = MapData.cam_yMax;
            cam_xMin = MapData.cam_xMin;
        }
    }


    void Update()
    {

       
        if (state == _CamState.inEditor)
            CamUpdate();

        else if (GC != null && state == _CamState.inGame)
        {
            if (GC.gameState != GameController._gameState.playerTurnEnd_AutoAttack && 
                GC.gameState != GameController._gameState.enemyTurn_Action && 
                GC.gameState != GameController._gameState.enemyTurn_ChooseUnit && 
                GC.gameState != GameController._gameState.enemyTurn_Prepare)
                if (GC.can_MoveCam) 
                    if (GC.storyState == GameController._storyState.tutorial || GC.storyState == GameController._storyState.game)
                            CamUpdate();
        }

        if (camZoom_Current >= 4.85f) camZoom_Current = 4.75f;
        if (camZoom_Current <= 1.65f) camZoom_Current = 1.75f;
        cam.orthographicSize = camZoom_Current + camZ.camZoom_Anim;

        if (GC != null)
        {
            if (transform != null)
            {
                if (GC.gameState == GameController._gameState.playerInUnitPanel)
                {
                    camFolder.transform.position = Vector3.Lerp(camFolder.transform.position, transform.position, 0.35f);
                }
                else
                {
                    if (slowCamMode) camFolder.transform.position = Vector3.MoveTowards(camFolder.transform.position, transform.position, Time.deltaTime * 4 * camZoom_Current);
                    else camFolder.transform.position = Vector3.MoveTowards(camFolder.transform.position, transform.position, Time.deltaTime * 7 * camZoom_Current);
                }
            }

            if (!GC.can_MoveCam)
            {
                float speedModi = 1;
                float dist = Mathf.Abs(camZoom_Current - camZoom_Should);
                if (dist > 0.1)
                {
                    speedModi = 1 + (dist * 1f);
                    if (speedModi > 3) speedModi = 3;
                }

                camZoom_Current = Mathf.MoveTowards(camZoom_Current, camZoom_Should, Time.deltaTime * 3.5f * speedModi);
            }
        }
    }
    public float edgeValue = 3;

    private void CamInMove(int Dir)
    {
        if (Dir == 1)
        {
            if (state == _CamState.inGame)
                if (transform.position.y >= cam_yMax) return;

            transform.position += transform.up * Time.deltaTime * 30f * speedModi;
            camFolder.transform.position = this.transform.position;
        }

        if (Dir == 2)
        {
            if (state == _CamState.inGame)
                if (transform.position.y < cam_yMin) return;

            transform.position += transform.up * Time.deltaTime * -30f * speedModi;
            camFolder.transform.position = this.transform.position;
        }

        if (Dir == 3)
        {
            if (state == _CamState.inGame)
                if (transform.position.x < cam_xMin) return;

            transform.position += transform.right * Time.deltaTime * -30f * speedModi;
            camFolder.transform.position = this.transform.position;
        }

        if (Dir == 4)
        {
            if (state == _CamState.inGame)
                if (transform.position.x > cam_xMax) return;

            transform.position += transform.right * Time.deltaTime * 30f * speedModi;
            camFolder.transform.position = this.transform.position;
        }

    }
    private void CamUpdate()
    {
        if (Input.GetKey(KeyCode.Y))
        {
            if(canMouseMove) canMouseMove = false;
            else canMouseMove = true;
        }

        camZoom_Current -= Input.GetAxis("Mouse ScrollWheel") * 10f;

        float speedModi = this.speedModi * camZoom_Current * 0.1f;

        if(canMouseMove && Input.mousePosition.y > Screen.height - edgeValue) CamInMove(1);
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) CamInMove(1);

        if (canMouseMove && Input.mousePosition.y < 0 + edgeValue) CamInMove(2);
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) CamInMove(2);

        if (canMouseMove && Input.mousePosition.x < 0 + edgeValue) CamInMove(3);
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) ) CamInMove(3);

        if (canMouseMove && Input.mousePosition.x > Screen.width - edgeValue) CamInMove(4);
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) CamInMove(4);
    }

    public IEnumerator ChangeCamSlowMove(float time, bool open)
    {
        yield return new WaitForSeconds(time);
        slowCamMode = open;
    }
    public void addPos(Vector3 Pos, bool smoothMove)
    {
        if (Pos.x < cam_xMin) Pos = new Vector3(cam_xMin, Pos.y, 0);
        if (Pos.x > cam_xMax) Pos = new Vector3(cam_xMax, Pos.y, 0);
        if (Pos.y > cam_yMax) Pos = new Vector3(Pos.x, cam_yMax, 0);
        if (Pos.y < cam_yMin) Pos = new Vector3(Pos.x, cam_yMin, 0);

        transform.position = Pos;
    }

    public void addZoom(float camZoom_Should, bool smoothMove)
    {
        camTimer = 0;
        if (smoothMove)
        {
            this.camZoom_Should = camZoom_Should;
        }
        else
        {
            this.camZoom_Should = camZoom_Should;
            this.camZoom_Current = camZoom_Should;
        }
    }

    public void camBackToNormal_WhenStatsPanelClose()
    {
        addPos(positionBefore, true);
        addZoom(camZoomBefore, true);
        GC.can_MoveCam = false;
        Invoke("closeCam", 0.25f);
    }
    public void camMoveToUnit_WhenStatsPanelOpen(Unit unit)
    {
        GC.can_MoveCam = false;
        positionBefore = transform.position;
        camZoomBefore = camZoom_Current;
        addPos(unit.transform.position + new Vector3(1.9f, .25f, 0), true);
        addZoom(1.75f, true);
    }

    private void closeCam()
    {
        GC.can_MoveCam = true;
    }
}
