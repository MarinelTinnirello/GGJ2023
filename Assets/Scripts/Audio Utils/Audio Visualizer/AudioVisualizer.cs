using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioVisualizer : MonoBehaviour
{
    #region Variables
    private AudioSource audioSource;
    [HideInInspector]public static float[] samples = new float[512];        // for instantiated cubes
    [HideInInspector]public static float[] freqBands = new float[8];
    [HideInInspector] public static float[] bandBuffers = new float[8];
    private float[] bufferDec = new float[8];
    #endregion

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        GetSpectrumAudioSource();
        MakeFrequencyBands();
        BandBuffers();
    }

    void GetSpectrumAudioSource()
    {
        audioSource.GetSpectrumData(samples, 0, FFTWindow.Blackman);
    }

    void MakeFrequencyBands()
    {
        int count = 0;

        for (int i = 0; i < 8; i++)
        {
            int sampleCount = (int)Mathf.Pow(2, i) * 2;
            float avg = 0;

            if (i == 7)
            {
                sampleCount += 2;
            }

            for (int j = 0; j < sampleCount; j++)
            {
                avg += samples[count] * (count + 1);
                count++;
            }

            avg /= count;
            freqBands[i] = avg * 10;
        }
    }

    void BandBuffers()
    {
        for (int i = 0; i < 8; ++i)
        {
            if (freqBands[i] > bandBuffers[i])
            {
                bandBuffers[i] = freqBands[i];
                bufferDec[i] = 0.005f;
            }

            if (freqBands[i] < bandBuffers[i])
            {
                bandBuffers[i] -= bufferDec[i];
                bufferDec[i] *= 1.2f;
            }
        }
    }
}
