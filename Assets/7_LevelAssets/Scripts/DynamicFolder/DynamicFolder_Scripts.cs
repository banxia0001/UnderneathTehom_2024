using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicFolder_Scripts : MonoBehaviour
{
    private void Start()
    {
        this.gameObject.GetComponent<Animator>().enabled = false;
        this.gameObject.GetComponent<Animator>().SetBool("S_L", false);
        this.gameObject.GetComponent<Animator>().SetBool("S_G_L", false);

    }
    public void InputAnimation(string animTrigger, string nextTrigger)
    {
        this.gameObject.GetComponent<Animator>().enabled = true;
        this.GetComponent<Animator>().SetTrigger(animTrigger);
        if(nextTrigger == "Shake_Loop") this.gameObject.GetComponent<Animator>().SetBool("S_L", true);
        if (nextTrigger == "Shake_G_Loop")
        { this.gameObject.GetComponent<Animator>().SetBool("S_L", true);
            this.gameObject.GetComponent<Animator>().SetBool("S_G_L", true); }

    }
    public void DestroyMyFolder()
    {
    }
}
