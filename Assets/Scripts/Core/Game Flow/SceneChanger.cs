using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneChanger : MonoBehaviour
{
    public static SceneChanger instance;
    private void Awake()
    {
        instance = this;
    }
    public void ChangeScene(string sceneName)
    {
        if (!sceneName.Contains("Levels"))
        {
            if (AudioManager.Instance != null && AudioManager.Instance.MainTheme() != null && !AudioManager.Instance.IsMusicPlaying(AudioManager.Instance.MainTheme()))
            {
                AudioManager.Instance.PlayBackgroundMusic(AudioManager.Instance.MainTheme());
            }
        }
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    public void ChangeScene(int sceneIndex)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneIndex);
    }

    
    public void QuitGame()
    {
        Application.Quit();
    }
}