using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCollapse : MonoBehaviour
{
    public GameController GC;
    public GridFallController CFC;

    public void Start()
    {
        StartCoroutine(LoopLoop());
    }
    private IEnumerator LoopLoop()
    {
     
        yield return new WaitForSeconds(5f);
        CFC.StartFalling();

        while (true)
        {
            Event_CaveShake(0);
            yield return new WaitForSeconds(0.3f);
            CFC.DecreaseTimer();
            yield return new WaitForSeconds(0.2f);
        }
       
    }
    public void Event_CaveShake(int i)
    {
        GC.SC.StartRockFall();
        GC.cameraAnim.SetTrigger("Shake3");
    }

}
