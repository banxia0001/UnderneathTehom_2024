using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    void Start()
    {
        this.transform.eulerAngles = new Vector3(10, 0, 0);
    }
}
