using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class GasMeter : MonoBehaviour
{
    [Header("Guage Settings")]
    public Transform needle;
    public Transform needleImage;
    private Tween needleShakeTween;

    [Space]
    public float decreaseTimeIdle = -1f;
    public float decreaseTimeOnMove = 120f;
    public float decreaseTimeOnBoost = 45f;
    private Tween decreaseTween;

    [Header("Gas Settings"), Range(0f,1f)]
    public float currentGasAmount = 1f;
    [Range(0f, 90f)]
    public float maxNeedleRotation = 74f;

    [Space, Range(0f, 1f)]
    public float lowFuelAmount = 0.3f;
    [Range(0f, 1f)]
    public float emptyTankAmount = 0.05f;

    private float currentNeedleRotation;

    [Header("Car Status")]
    public CarStates currentCarState = CarStates.Idle;
    public CarGasTankStates gasTank = CarGasTankStates.HasFuel;
    private CarGasTankStates prevGasTankStatus = CarGasTankStates.HasFuel;

    [HideInInspector]
    public bool refillInProgress;

    private void Start()
    {
        UpdateDecreaseTime(decreaseTimeIdle);
    }

    private void Update()
    {
        if (currentGasAmount <= emptyTankAmount)
        {
            UpdateGasTank(CarGasTankStates.EmptyTank);
        }
        else if (currentGasAmount <= lowFuelAmount)
        {
            UpdateGasTank(CarGasTankStates.LowFuel);
        }
        else
        {
            UpdateGasTank(CarGasTankStates.HasFuel);
        }

        UpdateNeedlePos();
    }

    public void UpdateCarStatus(CarStates _state)
    {
        if (_state == CarStates.Boosting)
        {
            UpdateDecreaseTime(decreaseTimeOnBoost);
        }
        else if (_state == CarStates.Moving)
        {
            UpdateDecreaseTime(decreaseTimeOnMove);
        }
        else
        {
            UpdateDecreaseTime(decreaseTimeIdle);
        }

        currentCarState = _state;
    }

    private void UpdateGasTank(CarGasTankStates _state)
    {
        if (_state == prevGasTankStatus) return;

        if (_state == CarGasTankStates.LowFuel || _state == CarGasTankStates.EmptyTank)
        {
            needleShakeTween.Kill();
            needleShakeTween = needleImage?.DOShakeRotation(.8f, 45f);
        }

        gasTank = prevGasTankStatus = _state;

        GameManager.Instance?.UpdateGasTank(_state);
    }

    private void UpdateDecreaseTime(float _time)
    {
        if (refillInProgress) return;

        decreaseTween.Kill();

        if (_time <= 0) return;

        decreaseTween = DOTween.To(() => currentGasAmount, x => currentGasAmount = x, 0f, _time);
    }

    public void RefillTank(float _time, float _delay = 0.0f)
    {
        if (refillInProgress) return;

        refillInProgress = true;

        decreaseTween.Kill();
        decreaseTween = DOTween.To(() => currentGasAmount, x => currentGasAmount = x, 1f, _time).SetDelay(_delay).OnComplete(OnRefillComplete);
    }

    public void OnRefillComplete()
    {
        refillInProgress = false;

        needleShakeTween.Kill();
        needleShakeTween = needleImage?.DOShakeRotation(.8f, 30f);

        GameManager.Instance?.mainCharacterController?.OnRefillComplete();
    }

    public void UpdateGasAmount(float _amount)
    {
        currentGasAmount = currentGasAmount + _amount;
    }

    public float GetGameAmount()
    {
        return currentGasAmount;
    }

    private void UpdateNeedlePos()
    {
        currentNeedleRotation = maxNeedleRotation - currentGasAmount * (maxNeedleRotation * 2f);

        if (needle)
        {
            needle.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, currentNeedleRotation));
        }
    }
}
