using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Text healthText;
    public float amount;
    public Text amountText;
    public static GameController instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateAmount(float value)
    {
        amount += value;
        amountText.text = amount.ToString();
    }

    public void UpdateLives(float value)
    {
        healthText.text = "x " + value.ToString();
    }
}
