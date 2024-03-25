using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
    public float time = 1f;
    void Start()
    {
       Destroy(this.gameObject, time);
    }
}
