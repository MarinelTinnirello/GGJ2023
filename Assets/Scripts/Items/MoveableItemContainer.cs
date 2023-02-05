using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableItemContainer : ItemContainer
{
    [Header("Auto Move Settings")]
    public Vector3 movement;
    public bool haltOnGameOver = true;
    [Space]
    public bool isActive;

    public override void Update()
    {
        if (!isActive) return;

        if (GameManager.Instance)
            if (GameManager.Instance.gamePaused || GameManager.Instance.gameOverCalled && haltOnGameOver) return;

        transform.localPosition = transform.localPosition + movement;

        base.Update();
    }

    public override void OnContainerHit(float destroyTime = 1f)
    {
        if (!CanTakeDamage()) return;

        isActive = false;
        base.OnContainerHit(destroyTime);
    }

    private bool CanTakeDamage()
    {
        if (GameManager.Instance)
            if (GameManager.Instance.mainCharacter) return GameManager.Instance.mainCharacter.transform.position.x + 2.5f < transform.position.x;

        return true;
    }

}
