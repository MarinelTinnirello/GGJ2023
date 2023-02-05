using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableItem : Item
{
    [Header("Movement Setup")]
    public CharacterType followCharacterType;
    public GameObject followTarget;
    public Vector3 followTargetOffset;
    public float followSmoothTime = .5f;
    [Space]
    public ActiveAxis lockAxis;
    public bool enabledLockedAxis;

    [HideInInspector]
    public Vector3 origPos;

    private Vector3 vel;

    public override void Start()
    {
        base.Start();

        origPos = transform.position;
    }

    public override void Update()
    {
        base.Update();

        if (followTarget) MoveTowardsTarget(followTarget);
        else MoveTowardsTargetCharacter();
    }

    public virtual void MoveTowardsTargetCharacter()
    {
        if(followCharacterType == CharacterType.MainPlayer && GameManager.Instance)
        {
            MoveTowardsTarget(GameManager.Instance.mainCharacter);
        } else
        {
            return;
        }
    }
    public virtual void MoveTowardsTarget(GameObject targetObject)
    {
        Vector3 targetPos = GetMoveToPosition(targetObject.transform.position);

        transform.position = Vector3.SmoothDamp(transform.position, targetPos + followTargetOffset, ref vel, followSmoothTime);
    }

    public virtual Vector3 GetMoveToPosition(Vector3 targetPos)
    {
        if (enabledLockedAxis)
        {
            if (lockAxis == ActiveAxis.X)
            {
                targetPos.x = origPos.x;
            }
            else if (lockAxis == ActiveAxis.Y)
            {
                targetPos.y = origPos.y;
            }
            else if (lockAxis == ActiveAxis.Z)
            {
                targetPos.z = origPos.z;
            }
            else if (lockAxis == ActiveAxis.XY)
            {
                targetPos.x = origPos.x;
                targetPos.y = origPos.y;
            }
        }

        return targetPos;
    }
}
