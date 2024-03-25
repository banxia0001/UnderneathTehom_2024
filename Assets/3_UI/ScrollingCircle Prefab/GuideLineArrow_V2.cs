using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideLineArrow_V2 : MonoBehaviour
{
    public LineRenderer line;
    public GameObject arrow;
    public void DrawLine_AI_ChargeVisualGuide(List<PathNode> chargeNode)
    {
        var pointList = new List<Vector3>();

        for (int i = 0; i < chargeNode.Count; i++)
        {
            if (i == chargeNode.Count - 1)
            {
                Vector3 A = chargeNode[i - 1].gameObject.transform.position;
                Vector3 B = chargeNode[i].gameObject.transform.position;
                Vector3 C = (A + B) / 2;
                Vector3 D = (B + C) / 2;

                pointList.Add(D);
                arrow.transform.position = D;
                //arrow.transform.LookAt(B);
                arrow.transform.right = B - arrow.transform.position;
                break;
            }
            pointList.Add(chargeNode[i].transform.position);

        }

        Vector3[] V = new Vector3[pointList.Count];
        for (int i = 0; i < pointList.Count; i++)
        {
            V[i] = pointList[i];
        }

        line.positionCount = pointList.Count;
        line.SetPositions(V);
    }
}
