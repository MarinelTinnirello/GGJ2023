using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateCubes : MonoBehaviour
{
    public GameObject sampleCubePrefab;
    public float maxScale;

    private GameObject[] sampleCubes = new GameObject[512];

    void Start()
    {
        for (int i = 0; i < 512; i++)
        {
            GameObject instanceSampleCube = (GameObject)Instantiate(sampleCubePrefab);

            instanceSampleCube.transform.position = this.transform.position;
            instanceSampleCube.transform.parent = this.transform;
            instanceSampleCube.name = "SampleCube" + i;

            this.transform.eulerAngles = new Vector3(0, i * -0.703125f, 0);
            instanceSampleCube.transform.position = Vector3.forward * 100;
            sampleCubes[i] = instanceSampleCube;
        }
    }

    void Update()
    {
        for (int i = 0; i < 512; i++)
        {
            if (sampleCubes != null)
            {
                sampleCubes[i].transform.localScale = new Vector3(10, AudioVisualizer.samples[i] * maxScale, 10);
            }
        }
    }
}
