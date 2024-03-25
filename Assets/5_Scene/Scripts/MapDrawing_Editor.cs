using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapDrawing_Editor : MonoBehaviour
{
    [Header("Collapse")]
    public int collapseOrder;

    [Header("Default")]
    public GridManager GM;
    public Camera Cam;
    public enum _drawwingState { draw_Height, draw_Block, draw_Texture, set_Height, set_Water, set_Grid, set_Background, draw_Texture_Water, drawing_Color, drawing_Collapse_Height }
    public _drawwingState state;

    public PathNode currentNode;

    public List<DrawingTexture> drawingList;
    public List<DrawingTexture> drawingList_Water;
    public List<Color32> drawingList_Color;

    public int drawingOrder;
    public int HeightToBe;
    public int colorOrder;





    private void Start()
    {
        //colorOrder = 0;
        //foreach (PathNode path in FindObjectsOfType<PathNode>())
        //{
        //    Debug.Log(path.name);
        //    //path.gameObject.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
        //    //path.gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
        //}


        //foreach (PathNode path in FindObjectsOfType<PathNode>())
        //{
        //    //textureReplace(path, drawingList[drawingOrder]);
        //    if (path.height % 2 != 0)
        //    {
        //        path.height++;
        //    }
        //    if (path.type == PathNode.GridType.normal)
        //        SetHeight(path, path.height);
        //}


        //foreach (PathNode path in FindObjectsOfType<PathNode>())
        //{
        //    if (path.height > 6)
        //    {

        //        path.gameObject.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
        //        path.gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);

        //    }

        //    path.gameObject.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
        //    path.gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
        //}

        //NodesReorder();
        //SetAsDefaultNodeTexture_ByHeight();

        //foreach (PathNode path in FindObjectsOfType<PathNode>())
        //{
        //    if (path.x > 11)
        //    {
        //        path.x += 5;
        //        //path.gameObject.GetComponent<SpriteRenderer>().color = new Color32(255, 244, 237, 255);
        //        //path.gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>().color = new Color32(255, 244, 237, 255);

        //        //path.gameObject.GetComponent<SpriteRenderer>().color = new Color32(235, 255, 233, 255);
        //        //path.gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>().color = new Color32(235, 255, 233, 255);
        //        SetHeight(path);
        //    }

        //    path.gameObject.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
        //    path.gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
        //}

    }
    public Text text;
    public void Update()
    {
        text.text = collapseOrder.ToString();
        if (Input.GetMouseButtonDown(0))
        {
            Check_Drawing(0);
        }

        if (Input.GetMouseButtonDown(1))
        {
            Check_Drawing(1);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GM.Clear_Block();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (state == _drawwingState.drawing_Collapse_Height)
            {
                collapseOrder--;
                if (collapseOrder < 0) collapseOrder = FindObjectOfType<MapData>().heightChangeStageNum - 1;
                GM.SetHeightInStage();
                return;
            }
            drawingOrder--;
            colorOrder--;
            if (drawingOrder < 0) drawingOrder = drawingList.Count - 1;
            if (colorOrder < 0) colorOrder = drawingList_Color.Count -1;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (state == _drawwingState.drawing_Collapse_Height)
            {
                collapseOrder++;
                if (collapseOrder >= FindObjectOfType<MapData>().heightChangeStageNum) collapseOrder = 0;
                GM.SetHeightInStage();
                return;
            }

            drawingOrder++;
            colorOrder++;
            if (drawingOrder >= drawingList.Count) drawingOrder = 0;
            if (colorOrder >= drawingList_Color.Count) colorOrder = 0;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) state = _drawwingState.draw_Height;
        if (Input.GetKeyDown(KeyCode.Alpha2)) state = _drawwingState.draw_Block;
        if (Input.GetKeyDown(KeyCode.Alpha3)) state = _drawwingState.draw_Texture;
        if (Input.GetKeyDown(KeyCode.Alpha4)) state = _drawwingState.set_Height;
        if (Input.GetKeyDown(KeyCode.Alpha6)) state = _drawwingState.set_Water;
        if (Input.GetKeyDown(KeyCode.Alpha7)) state = _drawwingState.set_Grid;

       // if (Input.GetKeyDown(KeyCode.Alpha5)) SetAsDefaultNodeTexture();
        if (Input.GetKeyDown(KeyCode.Alpha9)) NodesMoveRight();
        if (Input.GetKeyDown(KeyCode.Alpha8)) NodesReorder();
    }

    public void SetAsDefaultNodeTexture()
    {
        foreach (PathNode thisGrid in FindObjectsOfType<PathNode>())
        {
            textureReplace(thisGrid, drawingList[0]);
        }
    }

    public void NodesMoveRight()
    {
        foreach (PathNode thisGrid in FindObjectsOfType<PathNode>())
        {
            if (thisGrid.x > 10)
            {
                thisGrid.x += 6;
                SetHeight(thisGrid, thisGrid.height);
                textureColorChangeByHeight(thisGrid);
            }

        }
    }

    public void NodesReorder()
    {
        foreach (PathNode thisGrid in FindObjectsOfType<PathNode>())
        {
            foreach (PathNode myGrids in FindObjectsOfType<PathNode>())
            {
                if (myGrids.x == thisGrid.x && myGrids.y == thisGrid.y)
                {
                    if (myGrids != thisGrid)
                        if (thisGrid.transform.parent.name == "noo")
                        {
                            Destroy(thisGrid.gameObject);
                            continue;
                        }
                }
            }
        }
    }


    public void SetAsDefaultNodeTexture_ByHeight()
    {
        foreach (PathNode thisGrid in FindObjectsOfType<PathNode>())
        {
            textureColorChangeByHeight(thisGrid);
        }
    }

    public void Check_Drawing(int mouse)
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, LayerMask.GetMask("Ground"));

        if (hit.collider != null)
        {
            PathNode MouseAtNodeThis = hit.collider.transform.gameObject.GetComponent<PathNode>();

            if (MouseAtNodeThis != null)
            {
                currentNode = MouseAtNodeThis;
                switch (state)
                {
                    case _drawwingState.draw_Height:
                        if (mouse == 0) MouseAtNodeThis.height+=2;
                        else MouseAtNodeThis.height-=2;

                        SetHeight(MouseAtNodeThis, MouseAtNodeThis.height);
                        textureColorChangeByHeight(MouseAtNodeThis);
                        break;


                    case _drawwingState.draw_Block:
                        if (mouse == 0)
                            MouseAtNodeThis.isBlocked = true;
                        else
                            MouseAtNodeThis.isBlocked = false;
                        GM.Gene_Block();
                        break;

                    case _drawwingState.draw_Texture:
                        if (mouse == 0)
                            textureReplace(MouseAtNodeThis, drawingList[drawingOrder]);

                        else
                        {
                            int textureOrder = textureGet(MouseAtNodeThis);
                            if (textureOrder != -1) drawingOrder = textureOrder;
                        }
                        break;

                    case _drawwingState.set_Height:
                        if (mouse == 1)
                            HeightToBe = MouseAtNodeThis.height;

                        else
                        {
                            MouseAtNodeThis.height = HeightToBe;
                            SetHeight(MouseAtNodeThis, MouseAtNodeThis.height);
                            textureColorChangeByHeight(MouseAtNodeThis);
                        }

                        break;

                    case _drawwingState.set_Water:
                        geneWaterGrid(MouseAtNodeThis);
                        break;

                    case _drawwingState.set_Grid:
                        geneNormalGrid(MouseAtNodeThis);
                        break;

                    case _drawwingState.set_Background:
                        MouseAtNodeThis.isBlocked = true;
                        SetAsBackground(MouseAtNodeThis);
                        break;

                    case _drawwingState.draw_Texture_Water:
                        if (mouse == 0)
                            textureReplace(MouseAtNodeThis, drawingList_Water[drawingOrder]);

                        break;

                    case _drawwingState.drawing_Color:
                        if (mouse == 0) colorReplace(MouseAtNodeThis, drawingList_Color[colorOrder]);
                        break;

                    case _drawwingState.drawing_Collapse_Height:
                        if (Input.GetKey(KeyCode.C)) SetHeight_InCollapse(MouseAtNodeThis, -60);
                        else if (mouse == 0) SetHeight_InCollapse(MouseAtNodeThis, 2);
                        else if(mouse == 1) SetHeight_InCollapse(MouseAtNodeThis, -2);
                        break;
                }
            }
            else Debug.Log("Node Null!");
        }
    }

    private void SetHeight_InCollapse(PathNode path, int height)
    {
        path.heightChangeList[collapseOrder] += height;
        path.height += height;
        SetHeight(path, path.height);
    }
    public void geneWaterGrid(PathNode path)
    {
        GridManager GM = this.gameObject.GetComponent<GridManager>();
        MapData DATA = FindObjectOfType<MapData>();
        int x = path.x;
        int y = path.y;
        
        GameObject N_Grid = Instantiate(GM.blockPrefab_River, transform.position, Quaternion.identity);
        PathNode pathN = N_Grid.GetComponent<PathNode>();

        Transform trans = path.transform.parent;
        N_Grid.transform.SetParent(trans);

        pathN.x = x;
        pathN.y = y;
        pathN.type = PathNode.GridType.water;
        pathN.height = -1;
        SetHeight(pathN, pathN.heightOrigin);

        Destroy(path.gameObject);
    }

    public void geneNormalGrid(PathNode path)
    {
        GridManager GM = this.gameObject.GetComponent<GridManager>();

        int x = path.x;
        int y = path.y;

        GameObject N_Grid = Instantiate(GM.gridPrefab, transform.position, Quaternion.identity);
        PathNode pathN = N_Grid.GetComponent<PathNode>();
        MapData DATA = FindObjectOfType<MapData>();

        Transform trans = path.transform.parent;
        N_Grid.transform.SetParent(trans);

        pathN.x = x;
        pathN.y = y;
        pathN.height = 0;
        SetHeight(pathN, pathN.heightOrigin);

        Destroy(path.gameObject);
    }

    public void textureReplace(PathNode path, DrawingTexture dt)
    {
        path.gameObject.GetComponent<SpriteRenderer>().sprite = dt.face;
        path.gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = dt.body;
    }

    public void colorReplace(PathNode path, Color32 dt)
    {
        path.gameObject.GetComponent<SpriteRenderer>().color = dt;
        path.gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>().color = dt;
    }

    public int textureGet(PathNode path)
    {
        Sprite compareSprite = path.gameObject.GetComponent<SpriteRenderer>().sprite;
        for (int i = 0; i < drawingList.Count; i++)
        {
            if (drawingList[i].face == compareSprite) return i;
        }
        return -1;
    }

    public void SetAsBackground(PathNode path)
    {
        path.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Ground";
        path.gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>().sortingLayerName = "Ground";
    }
    public static void SetHeight(PathNode path, int heightValue)
    {
        if (SaveData.mapdataTrans == null) SaveData.mapdataTrans = FindObjectOfType<MapData>().gameObject.transform.GetChild(0);
        if (path.type == PathNode.GridType.water) heightValue--;
        path.transform.position = PathNode.GetHeight(path, heightValue);
    }

    public void textureColorChangeByHeight(PathNode path)
    {
        if (path.height > 0)
        {
            path.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Default";
        }

        else
        {
            path.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Ground";
        }
    }
}
[System.Serializable]
public class DrawingTexture
{
    public string textureName;
    public Sprite face;
    public Sprite body;
}