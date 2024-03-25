using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("BuildMapSetting")]
    [Range(5, 50)]
    public int mapX;
    [Range(5, 50)]
    public int mapY;


    [Header("Prefabs")]
    public GameObject gridPrefab;
    public GameObject blockPrefab;
    public GameObject blockPrefab_River;
    public GameObject nullPrefab;
    public GameObject sortingFolderPrefab;
    private GameObject mapFolder;
    private GameObject blockFolder;
    private MapData MD;    
    public PathNode[,] pathArray;

    [Header("DynamicFolder")]
    public GameObject dyanimicFolder;


    private void Awake()
    {
        MD = FindObjectOfType<MapData>(false);
        pathArray = new PathNode[MD.mapX, MD.mapY];
    }

    public int GetWidth()
    {
        return MD.mapX;
    }

    public int GetHeight()
    {
        return MD.mapY;
    }
    public void GreateOriginCollapseList()
    { 
    
    

    }

    public void GetOriginHeight()
    {
        MapData data = FindObjectOfType<MapData>();
        foreach (PathNode path in FindObjectsOfType<PathNode>())
        {
            path.heightOrigin = path.height;
            path.heightChangeList = new int[data.heightChangeStageNum];
        }
    }
    public void SetOriginHeight()
    {
        mapFolder = FindObjectOfType<MapData>().gameObject;
        foreach (PathNode path in FindObjectsOfType<PathNode>())
        {
            for (int i = 0; i < path.heightChangeList.Length; i++)
            {
                if (path.heightChangeList[i] < -30) path.heightChangeList[i] = -100;
            }

            
            path.height = path.heightOrigin;
            MapDrawing_Editor.SetHeight(path, path.heightOrigin);
        }
    }
    public void SetHeightInStage()
    {
        MapDrawing_Editor editor = FindObjectOfType<MapDrawing_Editor>();
        foreach (PathNode path in FindObjectsOfType<PathNode>())
        {
            int addone = 0;
            for (int i = 0; i <= editor.collapseOrder; i++)
            {
                addone += path.heightChangeList[i];
            }

            path.height = path.heightOrigin + addone;
            MapDrawing_Editor.SetHeight(path, path.height);
        }
    }
    public void BuildSortingLayerFolder()
    {
        mapFolder = FindObjectOfType<MapData>().gameObject;
        foreach (PathNode path in FindObjectsOfType<PathNode>())
        {
            GameObject MyTransform = Instantiate(sortingFolderPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            MyTransform.transform.parent = path.transform;
            MyTransform.transform.localPosition = new Vector3(0, 1.5f, 0);
            MyTransform.transform.parent = path.transform.parent;
            path.transform.parent = MyTransform.transform;
        }
    }
    public void SetEqualSortingLayerFolder()
    {
        MapData data = FindObjectOfType<MapData>();
        Vector3 offset = data.transform.GetChild(0).transform.position;
        foreach (PathNode path in FindObjectsOfType<PathNode>())
        {
            path.transform.parent.parent = null;
            Vector3 currentPos = path.transform.position;
            path.transform.parent.position = GetWorldPosition(path.x, path.y) + new Vector3(0, 0.6f, 0) + offset;
            path.transform.position = currentPos;
            path.transform.parent.parent = data.transform.GetChild(0).transform;
            path.transform.parent.name = path.name;

        }
    }
    public void BuildMyAnimatinoFolder()
    {
        MapData data = FindObjectOfType<MapData>();
        GameObject folder = FindObjectOfType<GridManager>().dyanimicFolder;

        foreach (PathNode path in FindObjectsOfType<PathNode>())
        {
            GameObject myFolder = Instantiate(folder, new Vector3(0, 0, 0), Quaternion.identity);
            Vector3 V = path.transform.localPosition;
            myFolder.transform.parent = path.transform.parent;
            myFolder.transform.localPosition = new Vector3(0, 0, 0);
            path.transform.parent = myFolder.transform;
            path.dynamicFolder = myFolder.GetComponent<DynamicFolder_Scripts>();
        }
    }
    public void BuildMyGrids()
    {
        mapFolder = FindObjectOfType<MapData>().gameObject;
        mapFolder.transform.position = new Vector3(0, 0, 0);
        MD = mapFolder.GetComponent<MapData>();
        MD.mapX = this.mapX;
        MD.mapY = this.mapY;

        GameObject MyTransform = Instantiate(nullPrefab, new Vector3(0,0,0), Quaternion.identity);
        MyTransform.name = "Grids";
        MyTransform.transform.parent = mapFolder.gameObject.transform;
       
        for (int y = 0; y < mapY; y++)
        {
            for (int x = 0; x < mapX; x++)
            {
                GameObject Grid = Instantiate(gridPrefab, GetWorldPosition(x, y), Quaternion.identity);
                Grid.name = "Grid" + x + "." + y;
                Grid.transform.parent = MyTransform.transform;
                Grid.GetComponent<PathNode>().SetValue(x, y);
            }
        }

        MyTransform.transform.position = GetWorldPosition(-MD.mapX / 2, -MD.mapY / 2);
    }



    public void Reset_GridChildHeight()
    {
        GameObject[] Grids = GameObject.FindGameObjectsWithTag("Grid");
        foreach (GameObject thisGrid in Grids)
        {
            thisGrid.transform.GetChild(0).transform.localPosition = new Vector3(0,0,0);
            thisGrid.transform.GetChild(1).transform.localPosition = new Vector3(0, -1.25f, 0);
        }
    }

    public void Reset_GridHeight()
    {
        GameObject[] Grids = GameObject.FindGameObjectsWithTag("Grid");
        foreach (GameObject thisGrid in Grids)
        {
            thisGrid.transform.GetChild(1).transform.localPosition = new Vector3(0, 0, 0);
            //PathNode pn = thisGrid.GetComponent<PathNode>();
            //thisGrid.transform.localPosition = GetWorldPosition(pn.x, pn.y);
        }
    }
    public void Reset_GridColor()
    {
        MapDrawing_Editor ME = GameObject.FindGameObjectWithTag("GameController").GetComponent<MapDrawing_Editor>();
        foreach (PathNode thisGrid in FindObjectsOfType<PathNode>())
        {
            ME.textureColorChangeByHeight(thisGrid);
        }
    }

    public void Clear_Block()
    {
        GameObject blockFolder = GameObject.Find("BlockFolder");
        if (blockFolder != null) DestroyImmediate(blockFolder);
    }

    public void Gene_Block()
    {
        Clear_Block();

        blockFolder = Instantiate(nullPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        blockFolder.name = "BlockFolder";

        foreach (PathNode thisGrid in FindObjectsOfType<PathNode>())
        {
            if (thisGrid.isBlocked)
            {
                GameObject icon = Instantiate(blockPrefab, thisGrid.gameObject.transform.position, Quaternion.identity);
                icon.transform.parent = blockFolder.transform;
            }
        }
    }
    public static Vector3 GetWorldPosition(int x, int y)
    {
        float finalX = 0;
        float finalY = 0;

        if (y % 2 == 0)
        {
            finalX = x * 1.73f;
            finalY = y * 1.5f;
        }
        else
        {
            finalX = (x + 0.5f) * 1.73f;
            finalY = y * 1.5f;
        }

        return new Vector3(finalX * 0.8f, finalY * 0.65f, 0);
    }


    public static Vector3 GetGridCalculatePosition(int x, int y)
    {
        float finalX = 0;
        float finalY = 0;

        if (y % 2 == 0)
        {
            finalX = x * 1.73f;
            finalY = y * 1.5f;
        }
        else
        {
            finalX = (x + 0.5f) * 1.73f;
            finalY = y * 1.5f;
        }

        return new Vector3(finalX, finalY, 0);
    }

    public Vector3 GetWorldPosition_WithHeight(int x, int y)
    {
        return PathNode.GetHeight(pathArray[x,y], pathArray[x, y].height);
    }
    public PathNode GetPath(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < MD.mapX && y < MD.mapY) return pathArray[x, y];
        else return null;
    }
    
    public static float GetY(int height)
    {
       return height * 1.5f;
    }
    public List<PathNode> GetPathList(List<Vector2Int> myList)
    {
        List<PathNode> returnList = new List<PathNode>();
        foreach (Vector2Int xy in myList)
        {
            if (xy.x >= 0 && xy.y >= 0 && xy.x < MD.mapX && xy.y < MD.mapY)
                if (pathArray[xy.x, xy.y] != null)
                {
                    returnList.Add(pathArray[xy.x, xy.y]);
                }
        }
        return returnList;
    }
}
