using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private PathNode targetNode;
    //USE FOR LINE 
    private List<PathNode> gotoList;
    //USE TO CURVE
    private List<Vector3> flyToList;

    private Unit attacker;
    private UnitWeapon damage;
    private Skill skill;
    private Unit finalHitDefender;

    private int order;
    private bool start;
    private float speed;
    public bool isCurve;
    public bool isComplexParticleSystem;

    private GameController GC;
    public ParticleSystem[] PCS;

    public IEnumerator Death()
    {
        start = false;

        if (PCS != null)
            foreach (ParticleSystem pc in PCS)
            {
                pc.Stop();
            }

        yield return new WaitForSeconds(0.3f);
        Destroy(this.gameObject);
    }
    public void TriggerProjectile(Unit attacker, List<PathNode> gotoList, UnitWeapon damage, Skill skill)
    {
        GC = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        float dist = 0;
        this.gotoList = gotoList;
        this.attacker = attacker;
        this.damage = damage;
        this.skill = skill;

        if (skill != null)
        {
            isCurve = skill.isCurveProjectile;
            this.speed = skill.flyingSpeed;
        }

        if (isCurve == true)
        {
            Vector3 point1 = this.transform.position;
            Vector3 point3 = gotoList[gotoList.Count - 1].transform.position + new Vector3(0, 0, 0);
            finalHitDefender = gotoList[1].unit;
            dist = Vector3.Distance(point1, point3) * .85f;
            if (dist > 8f) dist = 8f;
            if (dist < 4f) dist = 4f;
            Vector3 point2 = (point1 + point3) / 2 + new Vector3(0, dist, 0);

            int vertexCount = 12;
            var pointList = new List<Vector3>();

            for (float ratio = 0; ratio <= 1; ratio += 1.0f / vertexCount)
            {
                var tangentLineVertex1 = Vector3.Lerp(point1, point2, ratio);
                var tangentLineVertex2 = Vector3.Lerp(point2, point3, ratio);
                var bezierpoint = Vector3.Lerp(tangentLineVertex1, tangentLineVertex2, ratio);
                pointList.Add(bezierpoint);
            }

            flyToList = pointList;
        }


        order = 0;
        start = true;
    }


    public void FixedUpdate()
    {
        if (start)
        {
            if (!isCurve)
            {
                if (order == gotoList.Count)
                {
                    if (isComplexParticleSystem) StartCoroutine(Death());
                    else Destroy(this.gameObject);
                    return;
                }

                var step = speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, gotoList[order].transform.position + new Vector3(0, 0.1f, 0), step);

                Vector3 nowPos = transform.position;

                Vector3 toPos1 = gotoList[order].transform.position + new Vector3(0, 0.1f, 0);
                float dist1 = Vector3.Distance(nowPos, toPos1);

                if (dist1 < 0.01f)
                {
                    if (order != 0)
                    {
                        if (gotoList[order].unit != null)
                        {
                            GC.Attack(attacker, gotoList[order].unit, damage, skill, true);
                            //StartCoroutine(GameFunctions.Attack_Calculate(attacker, gotoList[i].unit, GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>(), damage, skill));
                        }

                        else
                        {
                            GameObject specialEffect = Instantiate(skill.hitSpecialEffect, gotoList[order].transform.position + new Vector3(0, 0, 0), Quaternion.identity);
                        }
                    } 
                    order++;
                }
            }

            else
            {
                float dist2 = Vector3.Distance(transform.position, flyToList[order]);
                if (dist2 < 0.01f)
                {
                    order++;
                    if (order < flyToList.Count)
                    {
                        transform.LookAt(flyToList[order]);
                    }
                }

                if (order >= flyToList.Count - 1)
                {
                    if (finalHitDefender != null && isCurve)
                    {
                        //Debug.Log("!!");


                        GC.Attack(attacker, finalHitDefender, damage, skill, true);
                    }
                    if (isComplexParticleSystem) StartCoroutine(Death());
                    else Destroy(this.gameObject);
                    return;
                }

                else
                {
                    var step = speed * Time.fixedDeltaTime;
                    transform.position = Vector3.MoveTowards(transform.position, flyToList[order], step);
                }
            }
        }
    }

}
