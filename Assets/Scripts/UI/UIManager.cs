using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Canvas Settings")]
    public Canvas inGameCanvas;
    public Canvas pauseCanvas;

    [Header("VCR Display Settings")]
    public Text titleText;

    [Header("Animation Settings")]
    public Animator animator;


    [Header("Audio Settings")]
    public AudioSource backgroundMusicAudioSource;
    public AudioSource soundEffectsAudioSource;
    public AudioSource pauseMusicAudioSource;

    [Header("Character Status")]
    public MinimapController minimap;
    public GasMeter gasMeter;
    public CharacterFundManager funds;

    [Header("BeatTracker Settings")]
    public RectTransform beatTrackerBar;
    private Vector2 beatTrackerPos;
    public Image beatHint;
    private RectTransform beatHintTransform;
    private Tween beatHintScaleTween;
    private Tween beatHintFadeTween;

    [Space]
    public Image attackIcon;
    public Sprite attackIconDefault;
    public Sprite attackIconPressed;

    [Header("Accuracy Display Settings")]
    public AccuracyDisplay accuracyDisplayPrefab;
    public Transform accuracyDisplayContainer;
    public Sprite[] displaySprites;

    [Header("Crowd Display Settings")]
    public RectTransform crowdBar;
    public Image crowdBarFill;

    [Header("Pause Menu Settings")]
    public PauseManager pauseManager;

    [Header("GameOver Display Settings")]
    public Image gameOverDisplayImage;
    public Sprite[] gameOverSprites;
    public Button ReplayButton;
    [Space]
    public GameObject onWinDisplayObject;
    public GameObject onLoseDisplayObject;

    private bool playerWon;

    private string gameStateLabel = "gameState";

    private bool UIDisabled, UIHidden;

    private bool isReady, gameOverCalled;

    private void Start()
    {
        if (!animator) animator = GetComponent<Animator>();

        if (beatTrackerBar) beatTrackerPos = beatTrackerBar.anchoredPosition;

        if (attackIcon && attackIconDefault) attackIcon.sprite = attackIconDefault;

        if (beatHint) beatHintTransform = beatHint.rectTransform;

        if (GameManager.Instance)
        {
            if (!backgroundMusicAudioSource) backgroundMusicAudioSource = GameManager.Instance.audioSourceBackgroundMusic;
            if (!pauseMusicAudioSource) pauseMusicAudioSource = GameManager.Instance.audioSourcePauseScreenBackgroundMusic;
            if (!soundEffectsAudioSource) soundEffectsAudioSource = GameManager.Instance.audioSourceSoundEffects;
        }
        
        BeatTrackShown(false, 0f);

        isReady = true;
    }

    public void AssignMainPlayer(CharacterTrigger _player)
    {
        minimap?.AssignMainPlayer(_player);
    }

    public void OnGameStarted()
    {
        if (animator) animator.SetBool("GameStarted", true);
    }

    public void OnMusicStarted()
    {
        BeatTrackShown(true, 1.5f, .5f);
    }

    public void ShowUI(float _animationTime, Ease _easeType, float _delay)
    {
        UpdateUIState(true);
    }

    public void HideUI(float _animationTime, float _delay)
    {
        UpdateUIState(false);
    }

    public void DisableUI(bool _state)
    {
        /*if (characterDisplayUICanvas) characterDisplayUICanvas.enabled = !_state && !demoMode;
        if (blackFramesCanvas) blackFramesCanvas.enabled = !gamePaused;*/

        UIDisabled = _state;

        if (GameManager.Instance) GameManager.Instance.UIDisabled = _state;
    }

    public void UpdateUIState(bool _shown)
    {
        UIHidden = !_shown;

        if (GameManager.Instance) GameManager.Instance.UIHidden = UIHidden;
    }

    public void CursorActive(bool state, CursorLockMode cursorLockMode = CursorLockMode.Confined)
    {
        Cursor.visible = state;
        Cursor.lockState = cursorLockMode;
    }

    public void OnPauseStateChange(bool state)
    {
        pauseManager?.GamePause(state);

        if (titleText)
        {
            titleText.text = !gameOverCalled ? state ? "Paused" : " " : playerWon ? "You Win" : "GameOver";
            titleText.text = titleText.text.ToUpper();
        }

        if (state)
        {
            if (backgroundMusicAudioSource) backgroundMusicAudioSource.Pause();
            if (pauseMusicAudioSource) pauseMusicAudioSource.Play();

            GameManager.Instance?.PlaySoundEffectByName("pause");
        }
        else
        {
            if (backgroundMusicAudioSource) backgroundMusicAudioSource.Play();
            if (pauseMusicAudioSource) pauseMusicAudioSource.Stop();
        }

        if (onWinDisplayObject) onWinDisplayObject.SetActive(state ? false : gameOverCalled && playerWon);
        if (onLoseDisplayObject) onLoseDisplayObject.SetActive(state ? false : gameOverCalled && !playerWon);

        if (animator) animator.SetBool("GamePaused", state);

        CursorActive(state, CursorLockMode.None);
    }

    public void UpdateCarState(CarStates _state)
    {
        gasMeter?.UpdateCarStatus(_state);
    }

    public void UpdateGasTank(CarGasTankStates _state)
    {
        //gasTank = _state;
    }

    private void BeatTrackShown(bool state, float animationTime = 1f, float delayTime = 0f)
    {
        if (state)
        {
            beatTrackerBar?.DOAnchorPosX(beatTrackerPos.x, animationTime).SetEase(Ease.OutBounce).SetDelay(delayTime);
        } else
        {
            beatTrackerBar?.DOAnchorPosX(beatTrackerPos.x+150f, animationTime).SetEase(Ease.InBack).SetDelay(delayTime);
        }
    }

    public void UserAttackInput(bool state, AttackType attackStrength = AttackType.Low, float accuracy = 0f)
    {
        if (state)
        {
            if (accuracyDisplayPrefab && accuracyDisplayContainer && isReady)
            {
                Sprite currentSprite = displaySprites.Length < 4? null : accuracy < -.1f? displaySprites[3] : displaySprites[(int)attackStrength];
                AccuracyDisplay newAccuracyDisplayObject = Instantiate(accuracyDisplayPrefab, accuracyDisplayContainer.position, Quaternion.identity, accuracyDisplayContainer);

                newAccuracyDisplayObject.ShowAccuracyType(currentSprite);
            }
        }

        if (attackIcon) attackIcon.sprite = !state ? attackIconDefault : attackIconPressed;
    }

    public void ShowBeatHint(float delay = .5f, float animateTime = .9f, float hintScale = 3f)
    {
        if (!beatHint || !beatHintTransform) return;
        
        beatHintScaleTween.Kill();
        beatHintFadeTween.Kill();
        
        beatHintTransform.localScale = Vector3.zero;
        beatHintScaleTween = beatHintTransform.DOScale(new Vector3(hintScale, hintScale, hintScale), animateTime).SetEase(Ease.OutCirc);

        beatHint.DOFade(1f, 0f);
        beatHintFadeTween = beatHint.DOFade(0f, animateTime).SetEase(Ease.OutCirc); ;
    }

    public void DecreaseCrowdBar(float amount)
    {
        if (gameOverCalled) return;

        if (crowdBarFill)
        {
            crowdBarFill.fillAmount = amount;
        }
    }

    public void ResetCrowdBar(float resetTime = .2f)
    {
        if (crowdBarFill)
        {
            crowdBarFill.fillAmount = 1;
        }
    }

    public void OnGameOver(GameOverState state, FinalScoreState score, float displayWindowDelay = 2f)
    {
        int gameOverWindowID;

        playerWon = state == GameOverState.Win;
        gameOverCalled = true;

        if (titleText)
        {
            titleText.text = state == GameOverState.Win ? "You Win" : "Game Over";
            titleText.text = titleText.text.ToUpper();
        }
        

        BeatTrackShown(false, .75f);

        BlackLetterBox.Instance?.Show(1f, 1.25f, .5f);

        attackIcon?.rectTransform.DOScale(Vector3.zero, 1f).SetEase(Ease.InBack);

        if (state != GameOverState.Win)
        {
            if (onLoseDisplayObject) onLoseDisplayObject.SetActive(true);
            gameOverWindowID = (int)state;
        }
        else
        {
            if (onWinDisplayObject) onWinDisplayObject.SetActive(true);
            gameOverWindowID = (int)state + ((int)score-1);
        }
            

        DisplayGameOverWindow(gameOverWindowID, displayWindowDelay);

        if (animator) animator.SetBool("GameStarted", false);
    }

    private void DisplayGameOverWindow(int windowID, float delay = 2f)
    {
        if (gameOverSprites.Length < 1) return;
        
        windowID = Mathf.Clamp(windowID, 0, gameOverSprites.Length-1);

        if (gameOverDisplayImage)
        {
            gameOverDisplayImage.sprite = gameOverSprites[windowID];
            gameOverDisplayImage.enabled = true;
            gameOverDisplayImage.rectTransform.DOScale(1f, .5f).SetDelay(delay).SetEase(Ease.OutBack).OnComplete(OnGameOverWindowDisplayed);
        }
    }

    private void OnGameOverWindowDisplayed()
    {
        if (GameManager.Instance)
        {
            GameManager.Instance.gameOverScreenActive = true;
        }
    }

    public void OnSceneExit()
    {
        if (ReplayButton) ReplayButton.interactable = false;

        gameOverDisplayImage?.rectTransform?.DOScale(0f, .5f).SetEase(Ease.InBack);
    }
}