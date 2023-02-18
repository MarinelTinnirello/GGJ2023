using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;
    [HideInInspector]
    public Camera activeCamera;

    [Header("UI Settings")]
    public UIManager UIManager;

    [HideInInspector]
    public bool UIHidden;

    [HideInInspector]
    public bool UIDisabled;

    [Header("Character Prefabs")]
    
    public CharacterTrigger mainCharacterPrefab;
    public NPCController nonPlayableCharacter;
    public EnemyController enemyPrefab;
    public BossController bossPrefab;

    [HideInInspector]
    public GameObject mainCharacter;

    [Space]
    public Transform SortIndexStartPoint;

    [Space]
    public int mainCharacterID = 0;

    [Header("Player Status")]
    public CarStates currentCarState = CarStates.Idle;
    public CarGasTankStates gasTank = CarGasTankStates.HasFuel;
    [Space]
    public string playerEarnings = "$0.00";

    [Header("Items Prefabs")]
    public MoveableItemContainer[] moveableItems;
    public Transform[] itemContainerSpawnPositions;

    [Header("Enemy Spawn Settings")]
    public Transform[] enemySpawnPositions;
    public Transform enemySpawnParent;
    public float enemySpawnRate = 5f;
    public int maxEnemyCount = 4;
    private int enemyStartID = 2;
    private int currentEnemyID;
    [Space, Tooltip("Switch To Boss When Time Remaining Is At")]
    public float switchToBossTime = 60;
    [Space]
    public Transform parentTransformOnKO;
    [Space]
    public Transform jumpLine;

    [Header("Item Spawn Settings")]
    public Transform[] itemSpawnPositions;
    public Transform itemSpawnParent;
    public float itemSpawnRate = 5f;

    [Space, Space]
    public bool touchMode = false;

    [Header("Audio Settings")]
    public AudioMixer audioMixer;

    [Space]
    public AudioSource audioSourceBackgroundMusic;
    public AudioSource audioSourcePersistentBackgroundMusic;
    public AudioSource audioSourceVoiceOvers;
    public AudioSource ambientSoundBackgroundMusic;
    public AudioSource audioSourceSoundEffects;
    public AudioSource audioSourcePauseScreenBackgroundMusic;

    [Space]
    public AudioClip[] soundEffects;
    public Dictionary<string, AudioClip> soundEffectsDictionary;
    public AudioClip[] musicInteruptedSoundEffects;

    [Space]
    public AudioClip onWinLoop;
    public AudioClip onWinSFX;
    public AudioClip onResetSFX;

    [Header("Camera Settings")]
    public CameraRig cameraRig;
    public Camera UICamera;
    
    [HideInInspector]
    public float timeScale = 1.0f;

    [HideInInspector]
    public bool cutSceneActive;

    [HideInInspector]
    public bool worldTransitioning;

    [HideInInspector]
    public bool levelReady = false;

    [HideInInspector]
    public bool levelIntroPlayed;

    [HideInInspector]
    public bool resetSceneCalled;

    [HideInInspector]
    public bool resetSceneWasCalled;
    
    //[HideInInspector]
    public CarController mainCharacterController;
    
    private bool playerInitCalled;
    private bool sceneLoading;

    [HideInInspector]
    public bool gameOverCalled;

    [HideInInspector]
    public bool gameOverScreenActive;

    [Header("General Settings")]
    public Transform worldTransform;
    public BeatTracker beatTracker;
    public BeatTrackerDisplay beatTrackerDisplay;
    public Image songScoreBar;
    public float gameDuration = 180f;
    public float beatTrackerDuration = 0.982f;
    private float timeElapsed;
    private float timeRemaining;
    private bool timerActive;
    public Text timerDisplay;
    public Animator crowdAnimator;

    private readonly string animatorLabelMusicPlaying = "musicPlaying";
    private readonly string animatorLabelGameOverState = "gameOverState";

    [Space]
    public GameObject[] enableObjectsOnWin;

    private bool isBeingTimed;

    [Space]
    public float gameStartDelay = 1.5f;

    private FinalScoreState winState;
    private float finalScorePercentage;

    [HideInInspector]
    public bool playerWon, playerLost, gameInProgress, inBossFight, bossDefeated;

    [Space]
    public bool gamePaused;
    private bool pauseScreenActive;

    [Space]
    public bool dontDestroyOnLoad;

    private bool exitCalled;
    
    public static GameManager Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            if (dontDestroyOnLoad) DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        soundEffectsDictionary = new Dictionary<string, AudioClip>();

        for (int i = 0; i < soundEffects.Length; i++) soundEffectsDictionary.Add(soundEffects[i].name, soundEffects[i]);

        SpawnMainCharacter();

        //if(beatTracker)StartCoroutine(beatTracker.CallStartGameAudio(gameStartDelay));

        currentEnemyID = enemyStartID;

        UIManager?.CursorActive(false);

        ActivateOnWinObjects(false);

        StartCoroutine(CallStartTimer(gameStartDelay));
    }
    
    private void Update()
    {
        if (gameInProgress) UpdateTimerDisplay();

        if (songScoreBar && beatTracker) songScoreBar.fillAmount = beatTracker.totalAwayPercentage;

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            TogglePause();
            //Application.Quit();
        }

        if (Input.GetButtonDown("SubmitGamePad") && gameOverScreenActive)
        {
            InstantResetSene();
        }

        if (pauseScreenActive != gamePaused)
        {
            PauseGame(gamePaused);
        }
    }

    public void StartGame()
    {
        gameInProgress = true;

        mainCharacterController?.StartPowerChangeTimer(10f, 6f);

        StartEnemySpawner();
        StartItemContainerSpawner();

        BlackLetterBox.Instance?.Show(1f, 1.25f, 2f);
    }

    public bool GameActive()
    {
        return !gamePaused && !exitCalled && !worldTransitioning && gameInProgress;
    }

    public void TogglePause()
    {
        PauseGame(!pauseScreenActive);
    }

    public void PauseGame(bool state)
    {
        pauseScreenActive = gamePaused = state;

        UIManager?.OnPauseStateChange(state);
    }

    public void Restart(bool _resetSceneCalled)
    {
        if (resetSceneCalled) return;

        resetSceneCalled = resetSceneWasCalled = _resetSceneCalled;

        OnSceneExit();
        
        if (WorldManager.Instance)
        {
            WorldManager.Instance.ReloadActiveScene();
        } else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        
        /*if (sceneLoading)
            return;
        
        levelReady = false;
        sceneLoading = true;
        worldTransitioning = true;
        cutSceneActive = false;

        if (!resetSceneCalled)
        {
            levelIntroPlayed = false;
        }*/
    }

    public void InstantResetSene()
    {
        if (sceneLoading || worldTransitioning || resetSceneCalled)
            return;
        
        PlaySoundEffect(onResetSFX);

        Restart(true);

        StartCoroutine(DelayedSceneReset(0f));
    }

    public void ResetSene(float _resetTime = 2f)
    {
        if (sceneLoading || resetSceneCalled)
            return;

        Restart(true);

        StartCoroutine(DelayedSceneReset(_resetTime));
    }

    private IEnumerator DelayedSceneReset(float _delay)
    {
        yield return new WaitForSeconds(_delay);

        resetSceneCalled = false;
    }

    public void ExitToMainMenu(float _delay)
    {
        if (exitCalled) return;
        exitCalled = true;

        StartCoroutine(LoadMainMenu(_delay));
    }

    private IEnumerator LoadMainMenu(float _delay = 0f, int sceneID = 2)
    {
        yield return new WaitForSeconds(_delay);

        if (WorldManager.Instance)
        {
            if(WorldManager.Instance.CallExitGame())OnSceneExit();
        }
        else
        {
            print("Couldn't find world manager");
            OnSceneExit();
            SceneManager.LoadScene(sceneID);
        }
    }

    public void OnSceneExit()
    {
        Time.timeScale = timeScale;
        UIManager?.OnSceneExit();
    }

    public void SpawnMainCharacter()
    {
        CharacterTrigger _mainCharacter = null;
        CharacterSetup _characterSetup;

        if (mainCharacterPrefab)
        {
            _mainCharacter = Instantiate(mainCharacterPrefab, new Vector3(-1.471f, 0.0f, -4.39f), Quaternion.identity);
            _mainCharacter.name = "MainCharacterObj";
            
        } else if (mainCharacterController)
        {
            _mainCharacter = mainCharacterController;
        }

        if (_mainCharacter != null)
        {
            _characterSetup = _mainCharacter.GetComponent<CharacterSetup>();
            _characterSetup?.AddCharacterModelByID(mainCharacterID, CharacterType.MainPlayer);

            SetMainCharacter(_mainCharacter, true);// mainCharacterController = mainCharacter.GetComponent<CharacterInputController>();
        }
    }

    public void SetMainCharacter(CharacterTrigger characterTrigger, bool _override = false)
    {
        if (mainCharacter && !_override) return;

        mainCharacterController = (CarController)characterTrigger;
        mainCharacter = mainCharacterController.gameObject;

        UIManager?.AssignMainPlayer(mainCharacterController);

        AddTargetToCameraRig(mainCharacter.transform);
    }

    public void AddTargetToCameraRig(Transform target, bool clearAllTargets = false)
    {
        cameraRig?.AddTargetToCameraRig(target, clearAllTargets);
    }
    
    public void UpdateGameDuration(float duration)
    {
        gameDuration = duration;
    }

    private IEnumerator CallStartTimer(float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        timeRemaining = gameDuration;// - gameStartDelay;
        timerActive = true;

        StartGame();

        StartCoroutine(StartGameTimer());
    }

    /*public void OnMusicStarted(float trackDuration)
    {
        timeRemaining = gameDuration = trackDuration;

        beatTrackerDisplay?.InitTracker(beatTrackerDuration);
        UIManager?.OnMusicStarted();
        
        StartCoroutine(CallStartTimer());
    }*/

    private IEnumerator StartGameTimer()
    {
        isBeingTimed = true;

        yield return new WaitForSeconds(gameDuration);//-gameStartDelay

        OnCallGameOver(GameOverState.Win);
    }

    private void UpdateTimerDisplay()
    {
        int minutesElapsed = Mathf.FloorToInt(timeElapsed / 60f);
        int secondsElapsed = Mathf.RoundToInt(timeElapsed % 60f);

        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.RoundToInt(timeRemaining % 60f);
        int mseconds = Mathf.RoundToInt(timeRemaining / 600f);

        if (secondsElapsed == 60)
        {
            secondsElapsed = 0;
            minutesElapsed += 1;
        }

        if (seconds == 60)
        {
            seconds = 0;
            minutes += 1;
        }
        
        if (timerDisplay && timeRemaining > 0 && timerActive)
        {
            timerDisplay.text = minutes.ToString("00") + ":" + seconds.ToString("00") + ":" + mseconds.ToString("00");//"▶ " + 
        } else
        {
            return;
        }

        timeElapsed += Time.deltaTime;
        timeRemaining -= Time.deltaTime;
    }

    private void StartEnemySpawner()
    {
        StartCoroutine(CallEnemySpawner());
    }

    private IEnumerator CallEnemySpawner()
    {
        while (gameInProgress)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(enemySpawnRate);
        }
    }

    private void SpawnEnemy()
    {
        if (inBossFight && !bossDefeated) return;

        if(timeRemaining <= switchToBossTime && !bossDefeated && bossPrefab)
        {
            BossController newBoss = Instantiate(bossPrefab, GetEnemySpawnPosition(), Quaternion.identity);
            newBoss.SetCharacterVariantByID(currentEnemyID);

            if (enemySpawnParent) newBoss.transform.parent = enemySpawnParent;

            OnBossBattleStart();

            return;
        }

        if (!enemyPrefab || !CanSpawnNewEnemy()) return;

        EnemyController newEnemy;

        newEnemy = Instantiate(enemyPrefab, GetEnemySpawnPosition(), Quaternion.identity);
        //CharacterSetup newEnemySetup = newEnemy.GetComponent<CharacterSetup>();

        //newEnemySetup.SetCharacterType(CharacterType.Enemy);
        //newEnemy.characterType = CharacterType.Enemy;
        newEnemy.SetCharacterVariantByID(currentEnemyID);

        if (enemySpawnParent) newEnemy.transform.parent = enemySpawnParent;

        if (currentEnemyID <= 3)
        {
            currentEnemyID++;
        } else
        {
            currentEnemyID = enemyStartID;
        }
    }

    private Vector3 GetEnemySpawnPosition()
    {
        if (enemySpawnPositions.Length < 1)
        {
            return Vector3.zero;
        } else
        {
            return enemySpawnPositions[UnityEngine.Random.Range(0, enemySpawnPositions.Length)].position;
        }
    }

    private bool CanSpawnNewEnemy()
    {
        EnemyController[] activeEnemyControllers = FindObjectsOfType<EnemyController>();
        return activeEnemyControllers.Length < maxEnemyCount;
    }

    private void StartItemContainerSpawner()
    {
        StartCoroutine(CallItemContainerSpawner());
    }

    private IEnumerator CallItemContainerSpawner()
    {
        while (gameInProgress)
        {
            yield return new WaitForSeconds(itemSpawnRate);
            SpawnItemContainer();
        }
    }

    private void SpawnItemContainer()
    {
        if (moveableItems.Length<=0) return;

        int targetItemContainerID = UnityEngine.Random.Range(0, moveableItems.Length);

        if (!moveableItems[targetItemContainerID]) return;

        Transform itemSpawnAtTransform = GetItemContainerSpawnTransform();

        ItemContainer newItemContainer = Instantiate(moveableItems[targetItemContainerID], itemSpawnAtTransform.position, Quaternion.identity);
        
        if (itemSpawnAtTransform) newItemContainer.transform.parent = itemSpawnAtTransform;
    }

    private Transform GetItemContainerSpawnTransform()
    {
        if (itemContainerSpawnPositions.Length < 1)
        {
            return worldTransform? worldTransform : null;
        }
        else
        {
            return itemContainerSpawnPositions[UnityEngine.Random.Range(0, itemContainerSpawnPositions.Length)];
        }
    }

    public void UpdateCarState(CarStates _state)
    {
        UIManager?.UpdateCarState(_state);
        currentCarState = _state;
    }

    public void UpdateGasTank(CarGasTankStates _state)
    {
        UIManager?.UpdateGasTank(_state);
        mainCharacterController?.UpdateGasTank(_state);
        gasTank = _state;

        if(gasTank == CarGasTankStates.EmptyTank) OnCallGameOver(GameOverState.EmptyTank);
    }

    public void SetEarningsTotal(string _total)
    {
        playerEarnings = _total;
    }

    public float GetJumpPoint()
    {
        if (jumpLine)
        {
            return jumpLine.position.z;
        }
        else
        {
            return -1f;
        }
    }

    public AttackType OnPlayerInputAttack()
    {
        float accuracy = beatTrackerDisplay? beatTrackerDisplay.GetAccuracy() : 0f;
        AttackType inputAttackStrengh = AttackType.Low;
        
        if (accuracy > .88f || accuracy<0f && accuracy>-0.1f)//.775f
        {
            inputAttackStrengh = AttackType.High;
        }
        else if (accuracy > .7f)//.57f
        {
            inputAttackStrengh = AttackType.Medium;
        } else
        {
            inputAttackStrengh = AttackType.Low;
        }

        UIManager?.UserAttackInput(true, inputAttackStrengh, accuracy);

        return inputAttackStrengh;
    }

    public void OnPlayerReleaseAttackButton()
    {
        UIManager?.UserAttackInput(false);
    }

    public void OnPlayerFail()
    {
        if (!gameInProgress) return;

        OnCallGameOver(GameOverState.Lose);
    }

    public void OnNPCPulledAway()
    {
        PlayRandomSoundClip(musicInteruptedSoundEffects);

        crowdAnimator?.SetBool(animatorLabelMusicPlaying, false);
    }

    public void OnNPCReturn()
    {
        crowdAnimator?.SetBool(animatorLabelMusicPlaying, true);
    }

    public void OnNPCAwayTimeOut()
    {
        if (!gameInProgress) return;

        OnCallGameOver(GameOverState.AwayTimeOut);
    }

    public void OnBossBattleStart()
    {
        inBossFight = true;
    }
    public void OnBossDefeated()
    {
        bossDefeated = true;
    }

    private void OnCallGameOver(GameOverState state)
    {
        if (gameOverCalled) return;

        gameOverCalled = true;
        gameInProgress = false;
        timerActive = false;

        mainCharacterController?.EndPowerChangeTimer();

        UIManager?.CursorActive(true, CursorLockMode.None);

        cameraRig?.UpdateSmoothTime(2f);

        beatTrackerDisplay?.EndTracker();
        beatTracker?.OnGameOver();

        //finalScorePercentage = beatTracker.GetFinalScore();

        /*if (state == GameOverState.Win && finalScorePercentage < 0.5)
        {
            state = GameOverState.BadScore;
        }*/

        OnGameEnded(state);
    }

    private void OnGameEnded(GameOverState state)
    {
        switch (state)
        {
            case GameOverState.Win:
                winState = EvaluateWinState(finalScorePercentage);
                playerWon = true;

                print("You Win | Track Finished crowd loved it! || Win state: " + winState + " || Final score: " + finalScorePercentage);

                break;
            case GameOverState.Lose:
                print("Stage Failed | Out of HP");
                playerLost = true;

                break;
            case GameOverState.BadScore:
                print("Song Failed | Track Finished though crowd was not satified with the interuptions");
                playerLost = true;

                break;
            case GameOverState.AwayTimeOut:
                print("Song Failed | DJ was away from stage for too long");
                playerLost = true;

                break;
            case GameOverState.CarDamaged:
                print("Car Immobile | Car Damaged");
                playerLost = true;

                break;
            case GameOverState.OutOfBounds:
                print("Car Immobile | Out Of Bounds");
                playerLost = true;

                break;
            case GameOverState.EmptyTank:
                print("Car Immobile | Out Of Gas");
                playerLost = true;

                break;
        }

        if (playerWon)
        {
            if (ambientSoundBackgroundMusic)
            {
                if (ambientSoundBackgroundMusic.isPlaying) ambientSoundBackgroundMusic?.Stop();

                if (onWinLoop)
                {
                    ambientSoundBackgroundMusic.clip = onWinLoop;
                    ambientSoundBackgroundMusic.loop = false;
                    ambientSoundBackgroundMusic?.Play();
                }
            }

            PlaySoundEffect(onWinLoop);
            PlaySoundEffect(onWinSFX);
        } else
        {
            if(ambientSoundBackgroundMusic && !ambientSoundBackgroundMusic.isPlaying) ambientSoundBackgroundMusic?.Play();
            
            cameraRig?.SetFocus(mainCharacter.transform);
        }

        ambientSoundBackgroundMusic?.DOFade(1, 25f);
        audioSourceBackgroundMusic?.DOFade(0, 25f);
        audioSourcePersistentBackgroundMusic?.DOFade(0, 25f);
        
        CharacterTrigger[] activeCharacters = FindObjectsOfType<CharacterTrigger>();
        for (int i = 0; i < activeCharacters.Length; i++) activeCharacters[i]?.OnGameOver(state, playerWon);
        
        UIManager?.OnGameOver(state, winState, playerWon? 4f : 1.5f);

        if (crowdAnimator) crowdAnimator?.SetInteger(animatorLabelGameOverState, (int)state);

        ActivateOnWinObjects(playerWon);
    }

    private void ActivateOnWinObjects(bool state)
    {
        for (int j = 0; j < enableObjectsOnWin.Length; j++) enableObjectsOnWin[j].SetActive(state);
    }

    private FinalScoreState EvaluateWinState(float finalScorePercentage)
    {
        return (finalScorePercentage < 0.7f) ? FinalScoreState.OneStar : (finalScorePercentage < 0.9f) ? FinalScoreState.TwoStars : FinalScoreState.ThreeStars;
    }

    public void PlayRandomSoundClip(AudioClip[] clips)
    {
        if (clips.Length <= 0) return;
        PlaySoundEffect(clips[UnityEngine.Random.Range(0, clips.Length)]);
    }

    public void PlaySoundEffectByName(string _name, float _delay = 0f)
    {
        StartCoroutine(OnDelayedPlaySoundEffect(_name, null, _delay, false, true));
    }

    public void PlayVoiceOverClipByName(string _name, float _delay = 0f)
    {
        StartCoroutine(OnDelayedPlaySoundEffect(_name, null, _delay, true, true));
    }
    private IEnumerator OnDelayedPlaySoundEffect(string _name, AudioClip _audioClip, float _delay, bool _isVoiceOver, bool _useDictionary)
    {
        yield return new WaitForSeconds(_delay);

        if (_useDictionary)
        {
            PlaySoundClipFromDic(_name, _isVoiceOver);
        }
        else
        {
            PlaySoundEffect(_audioClip);
        }
    }

    public void PlaySoundEffect(AudioClip clip)
    {
        if (!clip || !audioSourceSoundEffects) return;

        audioSourceSoundEffects.PlayOneShot(clip);
    }

    public void PlaySoundClipFromDic(string audioName, bool announcer = false)
    {
        if (soundEffectsDictionary.ContainsKey(audioName))
        {
            if (announcer == true && audioSourceVoiceOvers)
            {
                audioSourceVoiceOvers.Stop();
                audioSourceVoiceOvers.clip = soundEffectsDictionary[audioName];
                audioSourceVoiceOvers.Play();
            }
            else
            {
                audioSourceSoundEffects.PlayOneShot(soundEffectsDictionary[audioName]);
            }

        }
        else
        {
            Debug.LogWarning("Could not find AudioClip in dictionary [ " + audioName + " ] ");
        }
    }
}