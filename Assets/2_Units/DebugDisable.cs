using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugDisable : MonoBehaviour
{
    public void Start()
    {
        
    }
    public void OnDisable()
    {
        Debug.Log("Disable");
    }
    public void OnEnable()
    {
        Debug.Log("Able");
    }
}
