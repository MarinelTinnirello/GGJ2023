using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSync : MonoBehaviour
{
    public AudioSource leader;
    public AudioSource follower;

    private float timeElapsed = 0;
    private bool syncStarted;

    void LateUpdate()
    {
        if (timeElapsed >= 5)
        {
            timeElapsed = 0;
        }

        if (leader.isPlaying && timeElapsed == 0 || leader.isPlaying && !syncStarted)
        {
            if (!follower.isPlaying) follower.Play();
            follower.timeSamples = leader.timeSamples;
        }

        timeElapsed += Time.deltaTime;
    }
}