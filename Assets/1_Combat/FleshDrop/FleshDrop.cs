using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


public class FleshDrop : MonoBehaviour
{
    public float speed;
    public float timer;

    private int flyListNumber = 15;
    public List<Vector3> flyToList;
    private int flyToOrder;

    public PathNode targetPath;
    private Unit moveToUnit;
    private List<PathNode> nearbyPath;

    private Vector3 offset_WhenOnGrid;
    public Transform heartFolder;

    
    public enum FleshDropState
    {
        wait,
        dropToPos_1,
        dropToPos_2,
        stayOnPos,
        flyToUnit_1,
        flyToUnit_2,
    }

    public FleshDropState state;
    public PathNode DebugPath;
    public bool DebugFlesh;
    private void Start()
    {
        FindObjectOfType<SFX_Controller>().InputVFX(SFX_Controller.VFX.gainFlesh);
        
        if (DebugFlesh)
        {
            InputDropLocation(DebugPath, true);
        }
        if(state == FleshDropState.stayOnPos)
        { LandOnTile(); }
    }

    public void InputDropLocation(PathNode targetPath, bool trigger)
    {
        //heartFolder.SetParent(layerGroupB);
        this.transform.parent = null;

        if (trigger) state = FleshDropState.dropToPos_1;
        //Debug.Log("FleshTarget: " + targetPath.gameObject.name);
        flyToOrder = 0;
        this.targetPath = targetPath;
        Vector3 point1 = transform.position + new Vector3(0, 0, 0);
        Vector3 point3 = targetPath.transform.position + new Vector3(0, 0, 0);
        Vector3 point2 = new Vector3(0, 0, 0);
        float height = 0;
        int vertexCount = flyListNumber;

        if (state == FleshDropState.dropToPos_1)
        {
            state = FleshDropState.dropToPos_2;
            //point2 = (point1 + point3) / 2;
            //point3 = (point2 + point3) / 2;
            //height = Vector3.Distance(point1, point3) * .55f;
            //if (height > 3f) height = 3f;
            //if (height < 0.3f) height = 0.3f;
            //point2 = (point1 + point3) / 2 + new Vector3(0, height, 0);
        }

        if (state == FleshDropState.dropToPos_2)
        {
            vertexCount = flyListNumber * 3;
            Debug.Log("Enter Drop3");
            height = Vector3.Distance(point1, point3) * .45f;
            if (height > 1.5f) height = 1.5f;
            if (height < 0.3f) height = 0.3f;
            point2 = (point1 + point3) / 2 + new Vector3(0, height, 0);
        }
        
       
        var pointList = new List<Vector3>();

        for (float ratio = 0; ratio <= 1; ratio += 1.0f / vertexCount)
        {
            var tangentLineVertex1 = Vector3.Lerp(point1, point2, ratio);
            var tangentLineVertex2 = Vector3.Lerp(point2, point3, ratio);
            var bezierpoint = Vector3.Lerp(tangentLineVertex1, tangentLineVertex2, ratio);
            pointList.Add(bezierpoint);
        }
        pointList.Add(point3);
        pointList.Add(point3);
        flyToList = pointList;
    }

    private void Update()
    {

        timer -= Time.deltaTime;

        switch (state)
        {
            case FleshDropState.dropToPos_1:
                //FlyTo_Curve();
                FlyToNearbyTile();
                break;

            case FleshDropState.dropToPos_2:
                //FlyTo_Curve();
                FlyToNearbyTile();
                break;

            case FleshDropState.stayOnPos:
                if (targetPath == null) Destroy(this.gameObject);
                FleshMoveIfUnitComing();
              
                if (state == FleshDropState.stayOnPos)
                {
                    FleshFindingNearbyUnit();
                }
                break;

            case FleshDropState.flyToUnit_1:
                FlyToNearbyUnit();
                break;

            case FleshDropState.flyToUnit_2:
                FlyToNearbyUnit();
                break;
        }
    }
   
    public void FleshFindingNearbyUnit()
    {
        transform.position = Vector3.Lerp(transform.position, targetPath.transform.position + offset_WhenOnGrid, Time.deltaTime * 6f);

        List<Unit> nearbyUnit = new List<Unit>();

        if (timer > 0) return;

        foreach (PathNode nearbyPath in nearbyPath)
        {
            if (nearbyPath.unit != null && nearbyPath.unit.unitTeam == Unit.UnitTeam.playerTeam && nearbyPath.unit.unitSpecialState == Unit.UnitSpecialState.normalUnit)
            {
                nearbyUnit.Add(nearbyPath.unit);
            }
        }

        if (nearbyUnit != null && nearbyUnit.Count > 0)
        {

            targetPath.FleshList.Remove(this);

            if (targetPath.FleshList.Count != 0)
            {
                foreach (FleshDrop fp in targetPath.FleshList)
                {
                    fp.timer = 0.2f;
                }
            }
            this.transform.parent = null;
            state = FleshDropState.flyToUnit_1;
            moveToUnit = AIFunctions.FindUnit_ByLowestHP_AI(nearbyUnit);
            heartFolder.GetComponent<Animator>().SetBool("trigger", false);
        }
    }

    public void FleshMoveIfUnitComing()
    {
        if (targetPath.unit != null)
        {
            PathNode nearbyNode = null;
            foreach (PathNode nearbyPath in nearbyPath)
            {
                if (nearbyPath.unit == null) nearbyNode = nearbyPath;
            }

            targetPath.FleshList.Remove(this);

            if (nearbyNode == null)
            {
                Destroy(this.gameObject);
            }

            else
            {
                FlyToNearbyNode(nearbyNode);
            }
        }
    }


    private void FlyToNearbyNode(PathNode path)
    {
        state = FleshDropState.dropToPos_2;
        InputDropLocation(path,false);
    }

    private void FlyToNearbyUnit()
    {
        Vector3 pos = moveToUnit.transform.position + new Vector3(0, 0.55f, 0);
        transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * 6f);
        float dist2 = Vector3.Distance(transform.position, pos);

        if (dist2 < 0.15f && state == FleshDropState.flyToUnit_1)
        {
            moveToUnit.HealthChange(1, 0, "Heal");
            FPManager.FPChange(1);
            FindObjectOfType<SFX_Controller>().InputVFX(SFX_Controller.VFX.gainFlesh);
            moveToUnit.InputAnimation_GainFP("gainBuff");
            heartFolder.GetComponent<Animator>().SetTrigger("disappear");
            state = FleshDropState.flyToUnit_2;
            Destroy(this.gameObject);
        }
    }

    private void FlyToNearbyTile()
    {
        Vector3 pos = targetPath.transform.position;
        transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * 9f);
        float dist2 = Vector3.Distance(transform.position, pos);

        if (dist2 < 0.01f)
        {
            Debug.Log("landed");
            LandOnTile();
        }
    }
    private void FlyTo_Curve()
    {
        float dist2 = Vector3.Distance(transform.position, flyToList[flyToOrder]);
        if (dist2 < 0.01f)
        {
            flyToOrder++;
            //Debug.Log(flyToOrder + ":Order");

            if (flyToOrder >= flyToList.Count - 1)
            {
                if (state == FleshDropState.dropToPos_1)
                {
                    state = FleshDropState.dropToPos_2;
                    InputDropLocation(targetPath,false);
                }

                else
                {
                    LandOnTile();
                }
            }
        }

        else
        {
            int maxIndex = flyToList.Count/2;
            float modi = Mathf.Abs(maxIndex - flyToOrder);
            modi = (modi / maxIndex);
            float speedModi = 1 + modi * 2;
            transform.position = Vector2.MoveTowards(transform.position, flyToList[flyToOrder], speedModi * Time.deltaTime * 60f);
        }
    }


    private void LandOnTile()
    {
        this.transform.SetParent(targetPath.transform);
        state = FleshDropState.stayOnPos;
        timer = .23f;
        nearbyPath = GameFunctions.FindNodes_ByDistance(targetPath, 1, false);
        heartFolder.GetComponent<Animator>().SetBool("trigger", true);

        if (targetPath.FleshList.Count == 4)
        {
            heartFolder.GetComponent<Animator>().SetTrigger("disappear");
            Destroy(this.gameObject,0.3f);
            return;
        }
        targetPath.FleshList.Add(this);

        foreach (FleshDrop flesh in targetPath.FleshList)
        {
            flesh.PositionCheck();
        }
    }


    public void PositionCheck()
    {
        if (targetPath.FleshList.Count == 1)
        {
            this.offset_WhenOnGrid = new Vector3(0, 0, 0);
        }

        if (targetPath.FleshList.Count == 2)
        {
            if (this == targetPath.FleshList[0])
                this.offset_WhenOnGrid = new Vector3(0.25f, 0, 0);
            if (this == targetPath.FleshList[1])
                this.offset_WhenOnGrid = new Vector3(-0.25f, 0, 0);
        }

        if (targetPath.FleshList.Count == 3)
        {
            if (this == targetPath.FleshList[0])
                this.offset_WhenOnGrid = new Vector3(0.25f, -0.1f, 0);
            if (this == targetPath.FleshList[1])
                this.offset_WhenOnGrid = new Vector3(-0.25f, -0.1f, 0);
            if (this == targetPath.FleshList[2])
                this.offset_WhenOnGrid = new Vector3(0f, 0.2f, 0);
        }

        if (targetPath.FleshList.Count == 4)
        {
            if (this == targetPath.FleshList[0])
                this.offset_WhenOnGrid = new Vector3(0.25f, -0.1f, 0);
            if (this == targetPath.FleshList[1])
                this.offset_WhenOnGrid = new Vector3(-0.25f, -0.1f, 0);
            if (this == targetPath.FleshList[2])
                this.offset_WhenOnGrid = new Vector3(0.25f, 0.2f, 0);
            if (this == targetPath.FleshList[3])
                this.offset_WhenOnGrid = new Vector3(-0.25f, 0.2f, 0);
        }
    }
}
