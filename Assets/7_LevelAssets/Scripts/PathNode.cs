using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class PathNode : MonoBehaviour
{
    public int x;
    public int y;

    public int gCost;
    public int hCost;
    public int fCost;
    public int MOVE_COST = 1;
    public int height = 0;
    public int heightOrigin = 0;

    public bool isBlocked = false;

    public Unit unit;
    public PathNode cameFromNode;

    public CampFire campFire = null;

    public enum GridType { normal, water }
    public GridType type;

    [Header("Dynamic Height")]
    public DynamicState dynamicState;
    public enum DynamicState { inStatic, moveToPos}
    private Vector3 dynamicDirection;
    public List<FleshDrop> FleshList;

    [HideInInspector]public SortingGroup sortingGroup;
    [HideInInspector] public DynamicFolder_Scripts dynamicFolder;
    private float moveSpeed;
    public int[] heightChangeList;

    public List<Vector2Int> heightChangeListInGame;


    private void Awake()
    {
        if (SaveData.mapdataTrans == null) SaveData.mapdataTrans = FindObjectOfType<MapData>().gameObject.transform.GetChild(0);

        dynamicFolder = this.transform.parent.GetComponent<DynamicFolder_Scripts>();
        moveSpeed = 1;
        FleshList = new List<FleshDrop>();
        sortingGroup = this.transform.parent.parent.GetComponent<SortingGroup>();
        sortingGroup.sortingOrder = (int)(GridManager.GetY(y) * -100) - 50;
        sortingGroup.sortingOrder += height;
        this.unit = null;
        this.name = x + "," + y + " H:" + sortingGroup.sortingOrder;
        heightChangeListInGame = new List<Vector2Int>();
    }
    void Start()
    {
        GridManager myGrid =  GameObject.FindGameObjectWithTag("GameController").GetComponent<GridManager>();
        myGrid.pathArray[x, y] = this;
        if (type == GridType.water) MOVE_COST = 2;
        //parent.transform.position = parent.transform.position + new Vector3(0, this.height * -0.0001f, 0);
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost + (MOVE_COST * 15);
       // fCost = gCost + hCost;
    }
    public void CheckHeightChangeListInGame()
    {
        if (heightChangeListInGame != null && heightChangeListInGame.Count != 0)
        {
            List<Vector2Int> newList = new List<Vector2Int>();

            for (int i = 0; i < heightChangeListInGame.Count; i++)
            {
                heightChangeListInGame[i] = new Vector2Int(heightChangeListInGame[i].x, heightChangeListInGame[i].y-1);

                if (heightChangeListInGame[i].y < 0)
                {
                   dynamicFolder.InputAnimation("BackToNormal","");
                   AddHeight(-heightChangeListInGame[i].x, 1.5f);
                }
                else
                {
                    newList.Add(heightChangeListInGame[i]);
                }
            }

            heightChangeListInGame = newList;
        }
    }
    public void SetValue(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
   
    public void AddHeight(int heightAdd, float moveSpeed)
    {
        this.moveSpeed = moveSpeed;
        this.height += heightAdd;
        dynamicDirection = GetHeight(this, height);
        sortingGroup.sortingOrder += heightAdd;

        dynamicState = DynamicState.moveToPos;
    }
    public void AddHeight_WithCD(int heightAdd, float moveSpeed, int CD)
    {
        this.moveSpeed = moveSpeed;
        this.height += heightAdd;
       

        dynamicState = DynamicState.moveToPos;
        sortingGroup.sortingOrder += heightAdd;
        heightChangeListInGame.Add(new Vector2Int(heightAdd, CD));
    }

    public static Vector3 GetHeight(PathNode path, int height)
    {
        float yOffset = 0.14f * height;
        Vector3 orginPos = GridManager.GetWorldPosition(path.x, path.y);
        Vector3 offset = SaveData.mapdataTrans.position;
        return offset + orginPos + new Vector3(0, yOffset, 0);
    }

    public static void LerpMoving(Transform A, Vector3 B, float moveSpeed)
    {
        float speedModi = 1;
        float dist = Vector3.Distance(A.position, B);
        if (dist > 0.1)
        {
            speedModi = 1 + (dist * 0.6f);
            if (speedModi > 8) speedModi = 8;
        }
        A.position = Vector3.MoveTowards(A.position, B, Time.fixedDeltaTime * moveSpeed * speedModi * moveSpeed * 0.2f);
    }

    private void Update()
    {
        dynamicDirection = GetHeight(this, height);


        float dist = Vector3.Distance(this.gameObject.transform.position, dynamicDirection);

        if (dist <= 0.002f)//[If Reach the Direciton]
        {
            if (dynamicState == DynamicState.moveToPos)
            {
                dynamicState = DynamicState.inStatic;
                this.transform.position = dynamicDirection;
                if (height < -50)
                {
                    Debug.Log("destroy name:" + name);
                    if (this.unit != null)
                    {
                        if (unit.data.unitSpriteAssetType == UnitPrefabList.Unit_SpriteAsset_Type._17_BOSS)
                        {
                            this.unit.GetComponent<BossAI>().EmergenceJumpToCliff();
                        }
                        else
                        {
                            UnitAI AI = this.unit.gameObject.GetComponent<UnitAI>();
                            if (AI != null && AI.VisualGuideFolder != null)
                            {
                                Destroy(AI.VisualGuideFolder);
                            }
                            Destroy(this.unit.gameObject);
                            FindObjectOfType<GameController>().Check_UnitInList();
                            FindObjectOfType<GameController>().Check_EndGame();
                        }
                    }

                    if (FleshList != null)
                        foreach (FleshDrop flesh in FleshList)
                        {
                            Destroy(flesh.gameObject);
                        }

                    Destroy(this.gameObject);
                }
            }
        }

        else LerpMoving(transform, dynamicDirection, moveSpeed);

    }
}
