using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSmoking : MonoBehaviour
{
    public GameObject SmokeParticle;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartCreatingSmoke()
    {
        SmokeParticle.SetActive(true);
    }
}
