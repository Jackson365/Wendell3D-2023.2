using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHealth : MonoBehaviour
{
    public float healthValue;

    private void OnTriggerEnter(Collider coll)
    {
        if(coll.gameObject.tag == "Player")
        {
            coll.gameObject.GetComponent<Player>().IncreaseHealth(healthValue);
            Destroy(gameObject);
        }
    }
}
