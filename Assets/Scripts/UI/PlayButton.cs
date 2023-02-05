using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PlayButton : MonoBehaviour
{
    public bool disabledAfterClick;
    private bool hasClicked;

    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("SubmitGamePad"))
        {
            OnPlayButtonHit();
        }
    }

    public void OnPlayButtonHit()
    {
        if (hasClicked && disabledAfterClick) return;
        if (button && disabledAfterClick) button.interactable = false;

        if (WorldManager.Instance)
        {
            hasClicked = WorldManager.Instance.LoadMainScene(MainScenes.InGame);
        }
        else
        {
            print("Couldn't find world manager");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            hasClicked = true;
        }

        Screen.fullScreen = true;
    }
}
