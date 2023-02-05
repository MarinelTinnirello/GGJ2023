using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

using DG.Tweening;

public class WorldManager : MonoBehaviour
{
    public static WorldManager _instance;

    public event Action BeforeSceneUnload;          // Event delegate that is called just before a scene is unloaded.
    public event Action AfterSceneLoad;             // Event delegate that is called just after a scene is loaded.

    public CanvasGroup faderCanvasGroup;            // The CanvasGroup that controls the Image used for fading to black.
    public float fadeDuration = 1f;                 // How long it should take to fade to and from black.
    [Space]
    public Camera targetCamera;
    public Ease transistionEasing;

    [Space]
    public float screenTransitionTime = 1f;
    private bool screenTransitioning;
    private bool fadeOut;
    private bool sceneReloadComplete = true;
    [Space]

    private string activeSceneName;

    private bool isFading;                          // Flag used to determine if the Image is currently fading to or from black.
    private bool isTransitioning;
    private bool sceneReady;

    [Space]
    public bool setSceneToActiveOnLoad;

    [Space, Space]
    public string[] worldScenes;

    [Space, Space]
    public int startOnWorldScene = 0;

    private int currentSceneID;

    private bool exitGameCalled;

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

    public static WorldManager Instance
    {
        get { return _instance; }
    }

    private IEnumerator Start()
    {
        if (faderCanvasGroup)
        {
            faderCanvasGroup.alpha = 1f;
            faderCanvasGroup.blocksRaycasts = false;
        }

        if (!targetCamera)
        {
            targetCamera = Camera.main;
        }

        currentSceneID = startOnWorldScene;

        yield return StartCoroutine(LoadSceneAndSetActive(worldScenes[startOnWorldScene]));

        while (!sceneReady)
        {
            yield return null;
        }

        yield return new WaitForSeconds(.5f);

        yield return StartCoroutine(Fade(0f, screenTransitionTime));
    }

    public void OnSceneReady()
    {
        sceneReady = true;
        exitGameCalled = false;
    }

    public bool LoadMainScene(MainScenes _targetScene)
    {
        if (isFading || isTransitioning) return false;

        StartCoroutine(LoadMainSceneByID(_targetScene));
        
        return true;
    }

    public IEnumerator LoadMainSceneByID(MainScenes _targetScene)
    {
        isTransitioning = true;

        yield return StartCoroutine(Fade(1f, screenTransitionTime));

        SceneManager.UnloadSceneAsync(activeSceneName, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
        Resources.UnloadUnusedAssets();

        StartCoroutine(LoadScene(_targetScene.ToString(), (int)_targetScene));
    }

    public IEnumerator LoadScene(string sceneName, int sceneID)
    {   
        yield return StartCoroutine(LoadSceneAndSetActive(sceneName));

        while (!sceneReady)
        {
            yield return null;
        }

        yield return new WaitForSeconds(.1f);

        yield return StartCoroutine(Fade(0f, screenTransitionTime));

        currentSceneID = sceneID;
        isTransitioning = false;
    }


    private IEnumerator LoadSceneAndSetActive(string sceneName)
    {
        // Allow the given scene to load over several frames and add it to the already loaded scenes (just the Persistent scene at this point).
        //yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        AsyncOperation nScene = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        while (nScene.progress < 0.9f)
        {
            //Debug.Log("Loading scene " + " [][] Progress: " + nScene.progress);
            yield return null;
        }

        nScene.allowSceneActivation = true;

        while (!nScene.isDone)
        {
            //Debug.Log("Loading scene " + " [][] Progress: " + nScene.progress);
            yield return null;
        }

        // Find the scene that was most recently loaded (the one at the last index of the loaded scenes).
        Scene newlyLoadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

        yield return new WaitForSeconds(.1f);

        // Set the newly loaded scene as the active scene (this marks it as the one to be unloaded next).
        if (setSceneToActiveOnLoad)
        {
            SceneManager.SetActiveScene(newlyLoadedScene);
            activeSceneName = sceneName;
        }

        OnSceneReady();
    }

    public void ReloadActiveScene()
    {
        if (isFading || isTransitioning || !sceneReloadComplete) return;

        StartCoroutine(OnReloadActiveScene());
    }

    private IEnumerator OnReloadActiveScene()
    {
        sceneReloadComplete = false;

        yield return StartCoroutine(Fade(1f, screenTransitionTime));

        SceneManager.UnloadSceneAsync(activeSceneName, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);

        Resources.UnloadUnusedAssets();

        yield return StartCoroutine(LoadSceneAndSetActive(activeSceneName));

        sceneReloadComplete = true;

        StartCoroutine(Fade(0f, screenTransitionTime));
    }

    public bool LoadPrevScene()
    {
        if (isFading || isTransitioning) return false;

        int _targetSceneID = 0;

        if (currentSceneID > 0)
        {
            _targetSceneID = currentSceneID - 1;
        }
        else
        {
            Debug.LogWarning("SceneID [" + (_targetSceneID - 1) + "] out of range");
            return false;
        }

        return SwitchScene(_targetSceneID);
    }

    public bool LoadNextScene()
    {
        if (isFading || isTransitioning) return false;

        int _targetSceneID = 0;

        if (currentSceneID < worldScenes.Length - 1)
        {
            _targetSceneID = currentSceneID + 1;
        }
        else
        {
            Debug.LogWarning("SceneID [" + (_targetSceneID + 1) + "] out of range");
            return false;
        }

        return SwitchScene(_targetSceneID);
    }

    public bool SwitchScene(int _id)
    {
        if (isFading || isTransitioning) return false;

        GameManager.Instance?.Restart(false);

        isTransitioning = true;
        sceneReady = false;

        StartCoroutine(SwitchSceneByID(_id));

        return true;
    }

    private IEnumerator SwitchSceneByID(int _id)
    {
        yield return StartCoroutine(Fade(1f, screenTransitionTime));

        SceneManager.UnloadSceneAsync(activeSceneName, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);

        Resources.UnloadUnusedAssets();

        //GameManager.Instance?.OnSceneTransitionStart(_id);

        yield return StartCoroutine(LoadSceneAndSetActive(worldScenes[_id]));

        while (!sceneReady)
        {
            yield return null;
        }

        yield return StartCoroutine(Fade(0f, screenTransitionTime + .5f));

        currentSceneID = _id;
        isTransitioning = false;

        //GameManager.Instance?.OnSceneTransitionComplete(currentSceneID);
    }

    public bool CallExitGame(MainScenes _targetScene = MainScenes.MainMenu)
    {
        if (exitGameCalled) return false;

        exitGameCalled = true;

        StartCoroutine(OnExitGame(_targetScene));

        return true;
    }

    private IEnumerator OnExitGame(MainScenes _targetScene = MainScenes.MainMenu)
    {
        isTransitioning = true;

        yield return StartCoroutine(Fade(1f, screenTransitionTime));

        SceneManager.UnloadSceneAsync(activeSceneName, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
        Resources.UnloadUnusedAssets();

        if (GameManager.Instance)
        {
            Destroy(GameManager.Instance.gameObject);
        }

        StartCoroutine(LoadScene(_targetScene.ToString(), (int)_targetScene));
    }

    private IEnumerator Fade(float finalAlpha, float _fadeTime)
    {
        if (faderCanvasGroup)
        {
            // Set the fading flag to true so the FadeAndSwitchScenes coroutine won't be called again.
            isFading = true;

            // Make sure the CanvasGroup blocks raycasts into the scene so no more input can be accepted.
            faderCanvasGroup.blocksRaycasts = true;

            screenTransitioning = true;
            faderCanvasGroup.DOFade(finalAlpha, _fadeTime).SetEase(transistionEasing).OnComplete(ScreenTransitionComplete);

            while (screenTransitioning)
            {
                yield return null;
            }

            // Set the flag to false since the fade has finished.
            isFading = false;

            // Stop the CanvasGroup from blocking raycasts so input is no longer ignored.
            faderCanvasGroup.blocksRaycasts = false;

            //if(sceneReloadComplete && !isTransitioning) GameManager.Instance?.OnSceneTransitionComplete(currentSceneID);
        }
    }

    private void ScreenTransitionComplete()
    {
        screenTransitioning = false;
    }
}

