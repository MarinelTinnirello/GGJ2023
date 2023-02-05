using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : Weapon
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public Transform parentTransform;
    public Transform forwardLookAt;

    [Space]
    public AudioClip[] projectileSoundEffects;

    [Space, Range(0f, 40f)]
    public float radiusMultiplier = 1f;

    [Header("Muzzle Settings")]
    public GameObject muzzlePrefab;
    public ParticleSystem[] muzzleParticleSystems;
    public override void Start()
    {
        base.Start();
    }

    public override void Attack()
    {
        if (projectilePrefab)
        {
            GameObject currentProjectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
            Projectile projectile = currentProjectile.GetComponent<Projectile>();
            SphereCollider projectileCollider = currentProjectile.GetComponent<SphereCollider>();

            currentProjectile.transform.forward = transform.forward;
            currentProjectile.transform.parent = parentTransform;

            if (forwardLookAt)
            {
                currentProjectile.transform.forward = forwardLookAt.transform.forward;
            }

            projectile?.SetAttackInfo(true, attackCharacterOfType, 50f, AttackType.Low, projectileCollider? projectileCollider.radius * radiusMultiplier : 0.05f);

            characterSoundEffects?.PlayRandomSoundClip(projectileSoundEffects);

            characterEffects?.AddEffect(muzzlePrefab);
        }

        base.Attack();
    }
}
