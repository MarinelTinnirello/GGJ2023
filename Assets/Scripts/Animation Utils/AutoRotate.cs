using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    public Vector3 movement;
    public bool haltOnGameOver = true;
    [Space]
    public bool isActive;

    private void Update()
    {
        if (!isActive) return;

        if (GameManager.Instance)
            if (GameManager.Instance.gamePaused || GameManager.Instance.gameOverCalled && haltOnGameOver) return;

        transform.localRotation =  Quaternion.Euler(transform.localRotation.eulerAngles + movement); 
    }
}
