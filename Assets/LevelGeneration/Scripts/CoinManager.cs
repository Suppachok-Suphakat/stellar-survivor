using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    private TMP_Text goldText;
    public int currentGold = 0;

    const string COIN_AMOUNT_TEXT = "Gold Amount Text";

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        
    }

    private void Update()
    {

    }

    public void UpdateCurrentGold(int pickupAmout)
    {
        currentGold += pickupAmout;

        if (goldText == null)
        {
            goldText = GameObject.Find(COIN_AMOUNT_TEXT).GetComponent<TMP_Text>();
        }

        goldText.text = currentGold.ToString("D4");
    }

    public void DecreaseCurrentGold(int itemPrice)
    {
        currentGold -= itemPrice;

        if (goldText == null)
        {
            goldText = GameObject.Find(COIN_AMOUNT_TEXT).GetComponent<TMP_Text>();
        }

        goldText.text = currentGold.ToString("D4");
    }
}
