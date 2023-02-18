using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CharacterSoundEffects : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource soundEffectAudioSource;
    public AudioSource voiceOverAudioSource;

    [Header("Sound Effect Audio Clips"), Tooltip("Character specific sound effects.")]
    public AudioClip[] attackSounds;
    public AudioClip[] hitSounds;
    public AudioClip[] onDamage;
    public AudioClip[] onTransform;
    public AudioClip combo;
    public AudioClip jump;
    public AudioClip land;
    public AudioClip heavyLand;
    public AudioClip[] knockOut;

    [Header("Car Sound Effects"), Tooltip("Car specific sound effects.")]

    public AudioSource lowFuel;
    public AudioClip fillGas;
    public AudioClip emptyTank;

    [Header("Voice Over Audio Clips"), Tooltip("Character specific voice overs.")]
    public AudioClip[] transformedVO;
    public AudioClip knockOutVO;

    private void Start()
    {
        if (!soundEffectAudioSource) soundEffectAudioSource = GetComponent<AudioSource>();
    }
    
    public void CallAttackSoundEffect()
    {
        PlayRandomSoundClip(attackSounds);
    }

    public void CallComboSoundEffect()
    {
        PlaySoundEffect(combo);
    }

    public void CallFootStepSoundEffect()
    {

    }

    public void CallTransformSounds(int tranformID = 0)
    {
        PlayRandomSoundClip(onTransform);
    }

    public void EnableLowFuelSound(bool _state)
    {
        if (!lowFuel) return;

        lowFuel.loop = _state;

        if (_state)
        {
            lowFuel.Play();
        } else
        {
            lowFuel.Stop();
        }
    }

    public void CallHitSoundEffect(AttackType attackDamage)
    {
        /*int punchSoundID = (int)attackDamage;

        if (onDamage.Length > punchSoundID) PlaySoundEffect(onDamage[(int)attackDamage]);

        PlayRandomSoundClip(hitSounds, .1f);*/

        PlayRandomSoundClip(hitSounds);
        PlayRandomSoundClip(onDamage);
    }

    public void PlayRandomSoundClip(AudioClip[] clips, float delay = 0)
    {
        if (clips.Length<=0) return;
        PlaySoundEffect(clips[UnityEngine.Random.Range(0, clips.Length)], delay);
    }

    public void PlaySoundEffect(AudioClip clip, float delay = 0)
    {
        if (!clip || !soundEffectAudioSource) return;

        if (delay > 0){
            soundEffectAudioSource.clip = clip;
            soundEffectAudioSource.PlayDelayed(delay);
        } else
        {
            soundEffectAudioSource.PlayOneShot(clip);
        }
    }

    public void PlayVoiceOver(AudioClip clip, float delay = 0)
    {
        if (!clip || !voiceOverAudioSource) return;

        if (delay > 0)
        {
            voiceOverAudioSource.clip = clip;
            voiceOverAudioSource.PlayDelayed(delay);
        }
        else
        {
            voiceOverAudioSource.PlayOneShot(clip);
        }
    }
}
