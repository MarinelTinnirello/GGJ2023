using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : EnemyController
{
    [Header("Boss Settings")]
    public int currentPowerType;
    private float changePowerTime = 10f;

    public override void Start()
    {
        base.Start();

        StartPowerChangeTimer(4.0f, 6.0f);
    }

    public override void Update()
    {
        base.Update();
    }

    public void StartPowerChangeTimer(float delayStartTime = 8.0f, float repeatTime = 10.0f)
    {
        changePowerTime = repeatTime;
        InvokeRepeating("AdvancePowerType", delayStartTime, repeatTime);
    }

    public void EndPowerChangeTimer()
    {
        CancelInvoke("SetRandomPowerType");
    }

    public void AdvancePowerType()
    {
        int nextID = (int)currentPowerType+1;
        int powerCount = weapons.Length;

        if (nextID >= powerCount) nextID = 0;

        SetPowerType(nextID);
    }

    public void SetRandomPowerType()
    {
        int randomID = (int)currentPowerType;
        int powerCount = weapons.Length;

        while (randomID == (int)currentPowerType) randomID = Random.Range(0, powerCount);

        if (randomID > powerCount - 1) randomID = powerCount - 1;

        SetPowerType(randomID);
    }

    public void SetPowerType(int powerTypeID = 0)
    {
        currentPowerType = powerTypeID;
        OnTransform((int)currentPowerType);

        SetWeaponType(powerTypeID);

        GameManager.Instance?.cameraRig?.SetCameraAngleByID(powerTypeID);
    }

    private void OnTransform(int powerTypeID = 0)
    {
        characterAnimationController?.OnTransform(powerTypeID);
        characterSoundEffects?.CallTransformSounds(powerTypeID);
        characterEffects?.OnTransform(powerTypeID, changePowerTime);
    }

    public override void OnKnockOut(float knockOutTime = 4)
    {
        GameManager.Instance.OnBossDefeated();

        base.OnKnockOut(knockOutTime);
    }
}
