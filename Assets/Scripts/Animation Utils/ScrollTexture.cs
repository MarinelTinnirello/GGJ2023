using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollTexture : MonoBehaviour
{
    #region Variables
    [Header("Controls")]
    public float scrollX = 0.5f;        // speed of X-axis scrolling
    public float scrollY = 0.5f;        // speed of Y-axis scrolling

    public int index = 0;               // index of the material in array
    #endregion

    void Update()
    {
        float offsetX = Time.time * scrollX;
        float offsetY = Time.time * scrollY;

        GetComponent<Renderer>().materials[index].mainTextureOffset = new Vector2(offsetX, offsetY);
    }
}
