using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasStation : MonoBehaviour
{
    public GasMeter gasMeter;
    public CharacterFundManager playerFunds;
    private CharacterTrigger player;

    [Space]
    public float refillTime = 4.0f;
    public float fillDelayTime = 2.0f;
    public bool isFuelingCar;

    [Space]
    public float costPerFullTank = 40.0f;
    private float costToFill;

    [Space]
    public GasPump[] pumps;

    [Space]
    public AudioSource audioSource;

    private CarController carController;
    private bool carInStation;
    
    private void Start()
    {
        if (!gasMeter && GameManager.Instance)
        {
            if (GameManager.Instance.UIManager)
            {
                if (GameManager.Instance.UIManager.gasMeter)
                    gasMeter = GameManager.Instance.UIManager.gasMeter;

                if (GameManager.Instance.UIManager.funds)
                    playerFunds = GameManager.Instance.UIManager.funds;
            }

            player = GameManager.Instance.mainCharacterController;
        }   
    }

    private void Update()
    {
        if (isFuelingCar && gasMeter)
        {
            if (!gasMeter.refillInProgress) OnPumpComplete();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isMainCharacterCar(other.gameObject)) OnCarEnter();
    }

    private void OnTriggerStay(Collider other)
    {
        if (isMainCharacterCar(other.gameObject)) OnCarEnter();
    }

    private void OnTriggerExit(Collider other)
    {
        if (isMainCharacterCar(other.gameObject)) OnCarExit();
    }

    private bool isMainCharacterCar(GameObject other)
    {
        carController = other.GetComponent<CarController>();

        return carController;
    }

    private void OnCarEnter()
    {
        float _refillTime;

        if (carInStation) return;

        if (!CanFillTank())
        {
            print("doesnt have funds for gas");
            return;
        }

        GasPump _pump = GetNearestPump();

        _refillTime = gasMeter ? refillTime - (gasMeter.GetGameAmount() * refillTime) : refillTime;

        carController?.RefillTank(_refillTime, fillDelayTime, _pump);
        GameManager.Instance?.UIManager?.funds?.UpdateFunds(-costToFill);
        CameraRig.Instance?.DoEventZoom(3.5f);

        carInStation = isFuelingCar = true;

        if (audioSource) audioSource.Play();
    }

    private void OnCarExit()
    {
        carInStation = false;

        OnPumpComplete();

        if (player)
            if (player.isRunning) return;

        CameraRig.Instance?.DoEventZoom(0f);
    }

    private void OnPumpComplete()
    {
        if (audioSource) audioSource.Stop();
        for (int i = 0; i < pumps.Length; i++) pumps[i]?.ActivatePump(false);

        isFuelingCar = false;
    }

    private GasPump GetNearestPump()
    {
        if (pumps.Length < 0) return null;

        float _shortestDistance = 100f;
        float _currentPumpDistance;
        GasPump _nearestPump = pumps[0];

        for (int i = 0; i < pumps.Length; i++)
        {
            _currentPumpDistance = Vector3.Distance(player.gameObject.transform.position, pumps[i].gameObject.transform.position);

            if(_shortestDistance > _currentPumpDistance)
            {
                _shortestDistance = _currentPumpDistance;
                _nearestPump = pumps[i];
            }
        }

        return _nearestPump;
    }

    private bool CanFillTank()
    {
        return playerFunds ? playerFunds.HasEnoughFunds(SetCost()) : true;
    }

    private float SetCost()
    {
        costToFill = gasMeter ? costPerFullTank - (gasMeter.GetGameAmount()* costPerFullTank) : costPerFullTank;
        print("Current cost to fill tank = " + costToFill);

        return costToFill;
    }
}
