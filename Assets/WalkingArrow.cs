using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingArrow : MonoBehaviour
{
    public void Start()
    {
        trans();
    }
    public void trans()
    { this.transform.GetChild(0).transform.position = this.transform.position + new Vector3(0, -0.05f, 0); }

}
