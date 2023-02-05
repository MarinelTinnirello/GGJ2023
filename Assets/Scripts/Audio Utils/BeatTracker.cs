using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class BeatTracker : MonoBehaviour
{
    public AudioSource bgm, constentBgm;
    private AudioSource crowdAudioSource;
    private Tween crowdTween;

    public bool isMusicPlaying;
    public float musicVolume = 1f;

    bool isCounting = false;
    public float offTimer = 0f;
    public float timeLimit = 6f;

    private float trackLength;
    private bool audioInit, audioIsplaying, beatTrackerDisplayInit;

    bool isGameOver = false;

    float finalScoreTimer = 0f;
    float awayTimer = 0f;

    [HideInInspector]
    public float totalAwayPercentage = 1f;

    public IEnumerator CallStartGameAudio(float delay = 0f)
    {
        yield return new WaitForSeconds(delay);
        StartGameAudio();
    }

    public void StartGameAudio()
    {
        trackLength = bgm.clip.length;

        bgm.Play();
        constentBgm.Play();
        isMusicPlaying = true;
        audioInit = true;
    }
    
    void Update()
    {
        if (!audioInit) return;

        if(audioInit && !audioIsplaying)
        {
            audioIsplaying = bgm.isPlaying;
        }
        else if (audioInit && audioIsplaying && !beatTrackerDisplayInit)
        {
            if (GameManager.Instance)
            {
                crowdAudioSource = GameManager.Instance.ambientSoundBackgroundMusic;
                if (crowdAudioSource) crowdAudioSource.volume = 0;
                //GameManager.Instance?.OnMusicStarted(trackLength);
            }

            beatTrackerDisplayInit = true;
        }

        if (isCounting && !isGameOver)
        {
            WhileMusicInterupted();
        }

        if (audioIsplaying && !isGameOver && !isCounting)
        {
            finalScoreTimer += Time.deltaTime;
        }

        if (isCounting && !isGameOver)
        {
            totalAwayPercentage = 0.9f - (awayTimer / trackLength);
            awayTimer += Time.deltaTime;
        }
    }

    private void WhileMusicInterupted()
    {
        offTimer += Time.deltaTime;
        if (offTimer > timeLimit)
        {
            OnLose();
        } else if(!isMusicPlaying)
        {
            UpdateCrowdVolume(offTimer / timeLimit);
        }

        GameManager.Instance?.UIManager?.DecreaseCrowdBar(1 - (offTimer / timeLimit));
    }
    
    private void UpdateCrowdVolume(float ammount)
    {
        if (crowdAudioSource)
        {
            crowdAudioSource.volume = ammount;
        }
    }

    void OnLose()
    {
        isGameOver = true;
        GameManager.Instance?.OnNPCAwayTimeOut();
    }

    public void StopMusic()
    {
        if (!isMusicPlaying) return;
        
        bgm.volume = 0;
        isMusicPlaying = false;
        isCounting = true;

        crowdTween.Kill();
        if (crowdAudioSource) crowdAudioSource.Play();

        GameManager.Instance?.OnNPCPulledAway();
    }

    public void StartMusic()
    {
        if (isMusicPlaying) return;
        
        bgm.volume = musicVolume;
        isMusicPlaying = true;
        isCounting = false;
        offTimer = 0f;

        OnMusicResumed();
    }

    private void OnMusicResumed()
    {
        if (isGameOver) return;

        crowdTween.Kill();
        if (crowdAudioSource) crowdTween = crowdAudioSource.DOFade(0f, .4f).OnComplete(EndCrowdAudio);

        GameManager.Instance?.UIManager?.ResetCrowdBar();
        GameManager.Instance?.OnNPCReturn();
    }

    void EndCrowdAudio()
    {
        if (isGameOver) return;

        if (crowdAudioSource)
        {
            crowdAudioSource.Stop();
        }
    }

    public void OnGameOver()
    {
        isGameOver = true;
    }

    public float GetFinalScore()
    {
        return finalScoreTimer / trackLength;
    }
}