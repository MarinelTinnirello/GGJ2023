using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParamCube : MonoBehaviour
{
    public int band;
    public float minScale = .5f;
    public float scaleMultiplier = 5;
    public bool isUsingBuffer;

    void Start()
    {

    }

    void Update()
    {
        float targetScale = isUsingBuffer? AudioVisualizer.bandBuffers[band] : AudioVisualizer.freqBands[band];

        targetScale = targetScale / 8;
        targetScale = minScale + ((1 - minScale) * targetScale); // so if the target scale is = .5 the desired output if minscale + remainer ammount * targetscale

        transform.localScale = new Vector3(transform.localScale.x, targetScale * scaleMultiplier, transform.localScale.z);
    }
}
