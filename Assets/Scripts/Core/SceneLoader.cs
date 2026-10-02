using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    private const string SCENE_HOME = "HomeScene";
    private const string SCENE_MAIN = "MainScene";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void GoToHome()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SCENE_HOME);
    }

    public void GoToMain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SCENE_MAIN);
    }
}
