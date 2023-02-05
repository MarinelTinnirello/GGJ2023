using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    public CharacterType attackCharacterOfType;
    [HideInInspector]
    public float attackStrength = 50;
    public AttackType attackStrengthType;

    public Collider weaponCollider;

    //[Header("Character References")]
    [HideInInspector]
    public CharacterTrigger characterController;
    [HideInInspector]
    public CharacterSoundEffects characterSoundEffects;
    [HideInInspector]
    public CharacterEffects characterEffects;

    public virtual void Start()
    {
        if (!weaponCollider) weaponCollider = GetComponent<Collider>();
    }


    public virtual void SetCharacter(CharacterTrigger characterTrigger)
    {
        characterController = characterTrigger;

        if (characterController)
        {
            characterSoundEffects = characterController.characterSoundEffects;
            characterEffects = characterController.characterEffects;
        }
    }

    public virtual void Attack()
    {

    }
}
