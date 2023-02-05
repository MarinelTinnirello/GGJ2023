using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseButton;
    public GameObject pauseScreen;

    [Space]
    public bool gamePaused;

    private float defaultTimeScale;

    private void Start()
    {
        defaultTimeScale = GameManager.Instance? GameManager.Instance.timeScale : Time.timeScale;
    }

    public void GamePause(bool state = true)
    {
        if (gamePaused == state) return;

        gamePaused = state;

        Time.timeScale = gamePaused ? 0f : defaultTimeScale;

        if (pauseButton) pauseButton.SetActive(!gamePaused);
        if (pauseScreen) pauseScreen.SetActive(gamePaused);

        if (GameManager.Instance)
            GameManager.Instance.gamePaused = state;
    }
}
