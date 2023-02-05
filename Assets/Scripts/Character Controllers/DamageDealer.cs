using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DamageDealer : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public CharacterType attackCharacterOfType;
    
    [HideInInspector]
    public float attackStrength = 50;
    public AttackType attackStrengthType;

    [HideInInspector]
    public bool hasHit;

    public bool isAttacking;


    public virtual void Start()
    {

    }
    
    /*void Update()
    {
        if(meshRenderer)meshRenderer.enabled = isAttacking;
    }*/

    public virtual void OnTriggerStay(Collider other)
    {
        if (!isAttacking) return;

        CharacterTrigger character = other.gameObject.GetComponent<CharacterTrigger>();

        if (character)
        {
            if(character.characterType == attackCharacterOfType)
            {
                DealDamage(character);
            }
        }
    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        if (hasHit) return;

        CharacterSetup characterSettings = collision.gameObject.GetComponent<CharacterSetup>();
        CharacterTrigger character;

        if (characterSettings)
        {
            character = characterSettings?.GetCharacterTriggerType();
        } else
        {
            character = collision.gameObject.GetComponent<CharacterTrigger>();
        }

        if (character)
        {
            if (character.characterType == attackCharacterOfType)
            {
                DealDamage(character);
            }
        }

        hasHit = true;
    }

    public virtual void DealDamage(CharacterTrigger character)
    {
        character?.OnHit(attackStrength, attackStrengthType);
    }

    public virtual void SetAttackInfo(bool state, CharacterType targetCharacter = CharacterType.Enemy, float attackAmmount = 50f, AttackType attackType = AttackType.Low, float attackRadius = 0.05f)
    {
        attackCharacterOfType = targetCharacter;

        AttackStateChange(state, attackAmmount, attackType, attackRadius);
    }

    public virtual void AttackStateChange(bool state, float attackAmmount = 50f, AttackType attackType = AttackType.Low, float attackRadius = 0.05f)
    {
        Collider targetCollider = gameObject.GetComponent<Collider>();

        isAttacking = state;
        attackStrength = attackAmmount;
        attackStrengthType = attackType;
        
        if (targetCollider && targetCollider.GetType() == typeof(SphereCollider))
        {
            SphereCollider sphere = (SphereCollider)targetCollider;

            sphere.radius = attackRadius;
        }
    }
}