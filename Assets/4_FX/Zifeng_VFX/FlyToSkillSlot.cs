using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlyToSkillSlot : MonoBehaviour
{
    private RectTransform GotoOb;
    public int i;
    public ParticleSystem pc;
    private bool start = true;

    public void Awake()
    {
        start = true;
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 0);
        CombatPanelUI UI = FindObjectOfType<CombatPanelUI>(true);
        Debug.Log(UI.gameObject.name);
        GotoOb = UI.skill_Transform.GetComponent<RectTransform>();
    }

    public void Update()
    {
        if (!start) return;
        Vector2 pos = Camera.main.ScreenToWorldPoint(GotoOb.transform.position);
        float dist2 = Vector2.Distance(transform.position, pos);
        transform.position = Vector2.Lerp(transform.position, pos, Time.fixedDeltaTime * 0.55f);
        if (dist2 < 0.3f)
        {
            StartCoroutine(Death());
        }
    }

    public IEnumerator Death()
    {
        start = false;
        pc.Stop();
        yield return new WaitForSeconds(0.2f);
        Destroy(this.gameObject);
    }
}
