using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyController : MonoBehaviour
{
    #region Variables
    public Material[] skyboxes;
    [SerializeField]
    float RotationPerSecond = 1;
    [SerializeField] 
    bool _rotate;

    [Header("Day/Night Controllers")]
    [SerializeField] [Range(0.0f, 1.0f)]
    float time;
    [SerializeField]
    float fullDayLength;
    [SerializeField]
    float startTime = 0.4f;
    private float timeRate;
    [SerializeField]
    Vector3 noon;

    [Header("Sun")]
    [SerializeField]
    Light sun;
    [SerializeField]
    Gradient sunColor;
    [SerializeField]
    AnimationCurve sunIntensity;

    [Header("Moon")]
    [SerializeField]
    Light moon;
    [SerializeField]
    Gradient moonColor;
    [SerializeField]
    AnimationCurve moonIntensity;

    [Header("Other Lighting")]
    [SerializeField]
    AnimationCurve lightingIntensityMultiplier;
    [SerializeField]
    AnimationCurve reflectionIntensityMultiplier;
    #endregion

    void Start()
    {
        timeRate = 1.0f / fullDayLength;
        time = startTime;
    }

    void Update()
    {
        Timer();

        if (_rotate) RenderSettings.skybox.SetFloat("_Rotation", Time.time * RotationPerSecond);
    }

    void Timer()
    {
        time += timeRate * Time.deltaTime;
        if (time >= 1.0f) time = 0.0f;

        WorldLighting();
    }

    void WorldLighting()
    {
        // rotation
        sun.transform.eulerAngles = (time - 0.25f) * noon * 4.0f;
        moon.transform.eulerAngles = (time - 0.75f) * noon * 4.0f;

        // intensity
        sun.intensity = sunIntensity.Evaluate(time);
        moon.intensity = moonIntensity.Evaluate(time);

        // color
        sun.color = sunColor.Evaluate(time);
        moon.color = moonColor.Evaluate(time);

        // enable/disable Sun
        if (sun.intensity == 0.1 && sun.gameObject.activeInHierarchy) sun.gameObject.SetActive(false);
        if (sun.intensity > 0.1 && !sun.gameObject.activeInHierarchy) sun.gameObject.SetActive(true);

        // enable/disable Moon
        if (moon.intensity == 0.1 && moon.gameObject.activeInHierarchy) moon.gameObject.SetActive(false);
        if (moon.intensity > 0.1 && !moon.gameObject.activeInHierarchy) moon.gameObject.SetActive(true);

        // other lighting
        RenderSettings.ambientIntensity = lightingIntensityMultiplier.Evaluate(time);
        RenderSettings.reflectionIntensity = reflectionIntensityMultiplier.Evaluate(time);
    }
}
