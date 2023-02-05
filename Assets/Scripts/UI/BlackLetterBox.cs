using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity;
using UnityEngine.UI;
using DG.Tweening;

public class BlackLetterBox : MonoBehaviour
{
    public static BlackLetterBox _instance;

    public Image frameTop;
    public Image frameBottom;

    private Tween frameA;
    private Tween frameB;

    [Space]
    public GameObject skipButton;
    public Image skipButtonImage;
    public Image skipButtonCircleImage;
    [Space]
    public Sprite skipButtonUpState;
    public Sprite skipButtonDownState;
    [Space]
    public float barHeight = 160f;

    private Tween skipButtonHoldAnimation;
    private readonly float defaultAnimationTime = 0.5f;
    private bool skipDisplayShown;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    
    public static BlackLetterBox Instance
    {
        get { return _instance; }
    }

    private void Start()
    {
        UpdateDisplay(0f, 0f, 0f, false, 0f);
    }

    public void Show(float _value, float _time, float _delay, bool _showSkipButton = false, float _size = -1f)
    {
        if (_size == -1) _size = barHeight;

        UpdateDisplay(_value, _time, _delay, _showSkipButton, _size);
    }

    public void Hide(float _time, float _delay)
    {
        UpdateDisplay(0f, defaultAnimationTime, 0f, false, 0f);
    }

    private void UpdateDisplay(float _value, float _time, float _delay, bool _showSkipButton, float _size)
    {
        frameA.Kill();
        frameB.Kill();

        frameA = frameTop?.rectTransform.DOAnchorPosY(0-_size,_time).SetDelay(_delay);
        frameB = frameBottom?.rectTransform.DOAnchorPosY(-barHeight + _size, _time).SetDelay(_delay);

        if (skipButtonCircleImage) skipButtonCircleImage.fillAmount = 0;

        SkipButtonEnabled(_showSkipButton && _value>0f);
    }

    public void SkipButtonEnabled(bool _state)
    {
        ShowSkipDisplay(_state, true);

        if (_state)
        {
            skipButtonImage?.DOFade(0f, .75f).From().SetDelay(1.1f);
        } else
        {
            skipButtonImage?.DOFade(1f, 0f);
        }
    }

    public void SkipButtonDown(bool _state, float _animationTime)
    {
        skipButtonHoldAnimation.Kill();

        if (skipButtonImage)
        {
            if (_state)
            {
                if (skipButtonDownState) skipButtonImage.sprite = skipButtonDownState;

                skipButtonHoldAnimation = skipButtonCircleImage?.DOFillAmount(1f, _animationTime);

            } else
            {
                if (skipButtonUpState) skipButtonImage.sprite = skipButtonUpState;

                skipButtonHoldAnimation = skipButtonCircleImage?.DOFillAmount(0f, _animationTime/2);
            }
        }
    }

    public void ShowSkipDisplay(bool _state, bool _lockState)
    {
        if (_lockState)
        {
            skipDisplayShown = _state;
        }

        if (_state == true && skipDisplayShown == true)
        {
            skipButton?.SetActive(true);
        } else
        {
            skipButton?.SetActive(false);
        }
    }
}
