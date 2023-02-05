using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CharacterModelSettings
{
    public Animator animator;
    public Transform centerPoint;
    public HeadLookController headLookAt;
    public GameObject onKnockOutPrefab;
    public Weapon[] weapons;
    public int defaultWeaponID;
    [Space, Tooltip("Manually Set Foot Transforms for Non-Humanoid Characters")]
    public Transform leftFoot;
    public Transform rightFoot;
}
public class CharacterModelSetup : MonoBehaviour
{
    public CharacterModelSettings modelSettings;
    void Start()
    {
        
    }
    void Update()
    {
        
    }
}
