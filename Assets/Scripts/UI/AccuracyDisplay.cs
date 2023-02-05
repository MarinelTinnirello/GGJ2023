using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

public class AccuracyDisplay : MonoBehaviour
{
    public Image displayImage;
    public Transform displayContainer;
    Tween showTween;

    private float displayTime = 1.5f;
    private float fadeOutTime = 0.35f;

    public void ShowAccuracyType(Sprite sprite, float shakeAmmount = 30f)
    {
        if (displayContainer)
        {
            displayContainer.DOMoveY(70f, 2.5f);//.SetDelay(.25f);
        }

        if (displayImage)
        {
            showTween = displayImage.rectTransform.DOShakeRotation(.75f, shakeAmmount);

            displayImage.DOFade(0, fadeOutTime).SetDelay(displayTime).OnComplete(Remove);
            if(sprite)displayImage.sprite = sprite;
        }
    }

    public void Remove()
    {
        Destroy(this);
    }
}
