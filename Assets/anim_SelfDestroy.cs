using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class anim_SelfDestroy : MonoBehaviour
{
    public ParticleSystem pc;
    public void DestroyMyself()
    {
        Destroy(this.gameObject.transform.parent.gameObject);
    }

    public void Destroy_WithParticle()
    {
        StartCoroutine(Death());
    }

    public IEnumerator Death()
    {
        pc.Stop();
        yield return new WaitForSeconds(0.2f);
        Destroy(this.gameObject);
    }
}
