using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouseLight : MonoBehaviour
{
    private GameController GC;

    private void Start()
    {
        GC = FindObjectOfType<GameController>();
    }

    public GameObject folder_Blocked;

    public void FixedUpdate()
    {
        folder_Blocked.SetActive(false);
        if (GC.mouseAtNode != null)
            if (GC.mouseAtNode.isBlocked)
            { folder_Blocked.SetActive(true);}
    }

}
