using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeSprite : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite[] sprites;

    private void Start()
    {
        if (!spriteRenderer) spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        SetRandomSprite();
    }

    public void SetRandomSprite()
    {
        if (!spriteRenderer || sprites.Length <= 0) return;

        int randomID = Random.Range(0, sprites.Length);

        if (randomID > sprites.Length - 1) randomID = sprites.Length - 1;

        spriteRenderer.sprite = sprites[randomID];
    }
}
