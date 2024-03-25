using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rockfall : MonoBehaviour
{
    public Sprite[] sprites;

    public Transform rock,boncingPlatform;
    public GameObject vfx;
    public float timer;

    public Transform[] rockPos;

    public PhysicsMaterial2D[] materials;
    public void Start()
    {
        rock.gameObject.GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];
        boncingPlatform.GetComponent<CapsuleCollider2D>().sharedMaterial = materials[Random.Range(0, materials.Length)];
        rock.transform.eulerAngles = new Vector3(0, 0, Random.Range(-1, 1));
        rock.transform.position = rockPos[Random.Range(0, rockPos.Length)].position;
    }
    public void Update()
    {
        timer -= Time.deltaTime;

        float distance = Vector3.Distance(this.transform.position, rock.transform.position);
        if (timer <= 0)
        {
            Instantiate(vfx, rock.transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }


}
