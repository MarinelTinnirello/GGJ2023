using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFundManager : MonoBehaviour
{
    public float currentFunds = 100f;
    public float targetFunds = 500f;
    
    private void Start()
    {
        UpdateFundsDisplay();
    }

    public void UpdateFunds(float _amount)
    {
        currentFunds = currentFunds + _amount;
        UpdateFundsDisplay();
    }

    private void UpdateFundsDisplay()
    {
        string _funds = currentFunds.ToString("C");
        GameManager.Instance?.SetEarningsTotal(_funds);
    }

    public bool HasEnoughFunds(float _amount)
    {
        return currentFunds >= _amount;
    }
}
