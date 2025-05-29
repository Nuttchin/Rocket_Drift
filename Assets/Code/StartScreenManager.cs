using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenManager : MonoBehaviour
{
    public CanvasGroup pausePanel;
    public GameObject boostUI;
    private bool gameStarted = false;

    void Start()
    {
        pausePanel.alpha = 0f;
        pausePanel.gameObject.SetActive(false);
        boostUI.SetActive(false);
        Time.timeScale = 0f;
    }

    void Update()
    {
        if (!gameStarted)
        {
            if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
            {
                StartGame();
            }
        }
    }

    public void StartGame()
    {
        gameStarted = true;
        Time.timeScale = 1f;
        StartCoroutine(FadeInUI());
    }

    IEnumerator FadeInUI()
    {
        pausePanel.gameObject.SetActive(true);
        float duration = 1f;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Clamp01(timer / duration);
            pausePanel.alpha = alpha;
            yield return null;
        }

        pausePanel.interactable = true;
        pausePanel.blocksRaycasts = true;
        boostUI.SetActive(true);
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
