using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ShowParticles : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject particle,undelineParticle;
    public GameObject textobj;
    //TextMeshPro text;
    public Material glowingText,normalText;
    // Start is called before the first frame update
    void Start()
    {
        //text.GetComponent<TextMeshProUGUI>();
        // = Resources.Load<Material>("Assets/Sprites/jancient Atlas-glow Material.mat");
    }

    // Update is called once per frame
    void Update()
    {
        //if (text == null)
        //{
        //    Debug.Log("text is null");
        //}
        //if (glowingText == null)
        //{
        //    Debug.Log("material is null");
        //}
        //else if(textobj == null)
        //{
        //    Debug.Log("textobj is null");
        //}
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        particle.SetActive(true);
        undelineParticle.SetActive(true);
        textobj.GetComponent<TextMeshProUGUI>().fontMaterial = glowingText;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        particle.SetActive(false);
        undelineParticle.SetActive(false);
        textobj.GetComponent<TextMeshProUGUI>().fontMaterial = normalText;
    }

    //public void ChangeText()
    //{
    //    textobj.GetComponent<TextMeshProUGUI>().fontMaterial = glowingText;
    //}

    
}
