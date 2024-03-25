using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LearnSkillPanel : MonoBehaviour
{
    public AudioSource Ding;
    public Image image;
    public void DisableThis()
    {
        this.gameObject.SetActive(false);
    }


    public void Input(Sprite sprite)
    {
        Ding.Play();
        this.GetComponent<Animator>().SetTrigger("Play");
        image.sprite = sprite;
    }

}
