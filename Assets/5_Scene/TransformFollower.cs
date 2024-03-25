using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformFollower : MonoBehaviour
{
    public Vector2Int pos;
    private Transform trans;
    private bool canstart = false;
    void start()
    {
        canstart = false;
        StartCoroutine(Find());
    }
    public IEnumerator Find()
    {
        yield return new WaitForSeconds(0.12f);
        FindObjectOfType<GridManager>().GetPath(pos.x, pos.y);
        canstart = true;
    }
    void Update()
    {
        if (!canstart) return;
        if (trans == null) Destroy(this.gameObject);
        this.transform.position = trans.position;
    }
}
