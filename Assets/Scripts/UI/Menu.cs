using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject resumeButton;

    [HideInInspector]
    public PlayerBehaviour playerBehaviour;

    private void Start()
    {
        CloseMenu();
    }

    public void PlayerDeath()
    {
        OpenMenu();
        resumeButton.SetActive(false);
    }

    public void OpenMenu()
    {
        if (playerBehaviour)
            playerBehaviour.UiMode();

        gameObject.SetActive(true);
    }

    public void CloseMenu()
    {
        if (playerBehaviour)
            playerBehaviour.GameMode();

        gameObject.SetActive(false);
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
    }
}
