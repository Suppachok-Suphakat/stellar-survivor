using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    private TMP_Text goldText;
    public int currentGold = 100; // Start with 100 gold

    const string COIN_AMOUNT_TEXT = "Gold Amount Text";

    private void Awake()
    {
        // Ensure only one instance of CoinManager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Initialize goldText and update the displayed gold amount
        goldText = GameObject.Find(COIN_AMOUNT_TEXT).GetComponent<TMP_Text>();
        UpdateGoldText();
    }

    private void UpdateGoldText()
    {
        if (goldText != null)
        {
            goldText.text = currentGold.ToString("D4");
        }
    }

    public void UpdateCurrentGold(int pickupAmount)
    {
        currentGold += pickupAmount;
        UpdateGoldText();
    }

    public void DecreaseCurrentGold(int itemPrice)
    {
        currentGold -= itemPrice;
        UpdateGoldText();
    }
}
