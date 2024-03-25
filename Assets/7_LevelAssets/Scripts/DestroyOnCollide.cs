using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnCollide : MonoBehaviour
{
    public int type= 0;
    // Start is called before the first frame update
    void Start()
    {
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<DestroyOnCollide>() != null)
        {
            if (other.gameObject.GetComponent<DestroyOnCollide>().type == type)
            {
                if (other.gameObject.name == this.gameObject.name)
                {
                    Destroy(other.gameObject);
                    Destroy(this.gameObject);
                }
            }

            if (other.gameObject.GetComponent<DestroyOnCollide>().type > type)
            {
                Destroy(this.gameObject);
            }
        }
    }

}
