using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class volumeTrigger : MonoBehaviour
{
    private SFX_Controller sfx;
    public SFX_Controller.VFX sound;
    public SFX_Controller.VFX hitSound;
    private void Start()
    {
        sfx = FindObjectOfType<SFX_Controller>();
        sfx.InputVFX(sound);
    }

    private void OnDestroy()
    {
        Debug.Log(this.gameObject.name);
        sfx.InputVFX(hitSound);
    }

}
