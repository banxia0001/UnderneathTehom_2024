using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class locktoobject : MonoBehaviour
{
    public GameObject target;
    public Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        if(target!=null)
             offset = transform.position - target.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
            transform.position = target.transform.TransformPoint( offset);
    }
}
