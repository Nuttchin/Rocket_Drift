using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class PauseController : MonoBehaviour // ไม่ได้ใช้
{
    public CanvasGroup pauseButtonGroup;
    public GameObject pauseMenu;

    public Button resumeButton;
    public Button restartButton;
    public Button quitButton;

    private bool isPaused = false;

    void Start()
    {
        
        pauseMenu.SetActive(false);

        
        resumeButton.onClick.AddListener(OnPauseButtonPressed);
        restartButton.onClick.AddListener(RestartGame);
        quitButton.onClick.AddListener(QuitGame);

        
        pauseButtonGroup.alpha = 0f;
        pauseButtonGroup.interactable = false;
        pauseButtonGroup.blocksRaycasts = false;
        StartCoroutine(FadeInPauseButton());
    }

    IEnumerator FadeInPauseButton()
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime;
            pauseButtonGroup.alpha = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }
        pauseButtonGroup.interactable = true;
        pauseButtonGroup.blocksRaycasts = true;
    }

    public void OnPauseButtonPressed()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
        pauseMenu.SetActive(isPaused);
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
