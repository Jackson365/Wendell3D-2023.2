using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinAmount : MonoBehaviour
{
    public float coinValue;

    private void OnTriggerEnter(Collider coll)
    {
        if(coll.gameObject.tag == "Player")
        {
            GameController.instance.UpdateAmount(coinValue);
            Destroy(gameObject);
        }
    }
}
