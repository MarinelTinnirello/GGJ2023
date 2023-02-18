using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasPump : MonoBehaviour
{
    public Transform dockingPoint;
    public Animator animator;
    public bool isActive;

    private readonly string animatorLabelIsFueling = "IsFueling";

    public void ActivatePump(bool _isActive)
    {
        isActive = _isActive;

        if (animator)
        {
            animator.SetBool(animatorLabelIsFueling, _isActive);
        }
    }
}
