using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using DG.Tweening;

public class SceneTransition : MonoBehaviour {

    public Image fadeImage;
    public Image swipeImage;

    [Space]
    public float fadeSpeed = 2f;

    [Space]
    public float delay = .5f;

    [Space, Space]
    public Ease fadeEaseType = Ease.InSine;

    [Space, Space]
    //public TitleTextAnimation titleTextAnimation;
    
    private Animator animator;

    private int loadSceneID = 1;

    private bool isTransitioning;

    private void Awake()
    {
        if (fadeImage) fadeImage.gameObject.SetActive(true);
        if (swipeImage) swipeImage.gameObject.SetActive(true);
    }

    void Start () {
        if(GetComponent<Animator>())animator = GetComponent<Animator>();

        fadeImage.DOFade(0, (fadeSpeed * 1f)).SetEase(fadeEaseType).SetDelay(delay);

        ShowTitleText();
    }

    private void ShowTitleText()
    {
        //if (titleTextAnimation) titleTextAnimation.CallTextAnimation();
    }
    
    public void FadeToLevel(int _index)
    {
        if (isTransitioning) return;

        loadSceneID = _index;

        if (animator)
        {
            animator.SetTrigger("FadeOut");
        } else
        {
            fadeImage.DOColor(Color.black, fadeSpeed).SetEase(fadeEaseType).SetDelay(delay);
            fadeImage.DOFade(1, fadeSpeed).SetEase(fadeEaseType).SetDelay(delay).OnComplete(OnFadeOutComplete);

            isTransitioning = true;
        }

        if (GameManager.Instance)
        {
            //GameManager.Instance.FadeAllSoundsOut((fadeSpeed * .9f));
        }
    }

    public void OnFadeOutComplete()
    {
        isTransitioning = false;

        if (WorldManager.Instance)
        {
            WorldManager.Instance.LoadMainScene(MainScenes.MainMenu);
        }
        else
        {
            print("Couldn't find world manager");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
