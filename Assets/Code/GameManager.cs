using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public string adUnitId = "Rewarded_Android";
    public static GameManager instance;

    public GameObject player;

    public GameObject continueButton;
    public GameObject quitButton;
    public GameObject resetButton;

    public CanvasGroup continuePanel;
    public float fadeDuration = 1f;
    public float clearRadius = 5f;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Advertisement.Load(adUnitId, this);
    }

    public void OnPlayerDied()
    {
        StartCoroutine(FadeInPanel());

        continueButton.SetActive(true);
        quitButton.SetActive(true);
        resetButton.SetActive(true);

        Time.timeScale = 0f;
    }

    IEnumerator FadeInPanel()
    {
        continuePanel.gameObject.SetActive(true);

        CanvasGroup continueGroup = continuePanel;
        CanvasGroup continueBtnGroup = GetOrAddCanvasGroup(continueButton);
        CanvasGroup quitBtnGroup = GetOrAddCanvasGroup(quitButton);
        CanvasGroup resetBtnGroup = GetOrAddCanvasGroup(resetButton);

        continueGroup.interactable = false;
        continueGroup.blocksRaycasts = false;

        continueGroup.alpha = 0f;
        continueBtnGroup.alpha = 0f;
        quitBtnGroup.alpha = 0f;
        resetBtnGroup.alpha = 0f;

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            continueGroup.alpha = t;
            continueBtnGroup.alpha = t;
            quitBtnGroup.alpha = t;
            resetBtnGroup.alpha = t;

            yield return null;
        }

        continueGroup.interactable = true;
        continueGroup.blocksRaycasts = true;
    }

    CanvasGroup GetOrAddCanvasGroup(GameObject obj)
    {
        CanvasGroup group = obj.GetComponent<CanvasGroup>();
        if (group == null)
        {
            group = obj.AddComponent<CanvasGroup>();
        }
        return group;
    }

    public void ShowRewardedAd()
    {
        Advertisement.Show(adUnitId, this);
    }

    public void ContinueGame()
    {
        continueButton.SetActive(false);
        quitButton.SetActive(false);
        resetButton.SetActive(false);

        continuePanel.gameObject.SetActive(false);
        Time.timeScale = 1f;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.transform.position, clearRadius);
        foreach (Collider2D col in colliders)
        {
            if (col.CompareTag("Enemy") || col.CompareTag("Meteor") || col.CompareTag("Boss"))
            {
                Destroy(col.gameObject);
            }
        }

        player.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("main_menu");
    }

    public void ResetScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId == this.adUnitId && showCompletionState == UnityAdsShowCompletionState.COMPLETED)
        {
            ContinueGame();
        }
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.LogError($"Show Ad Failed: {message}");
    }

    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }

    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log("Ad loaded: " + adUnitId);
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.LogError($"Failed to load Ad Unit {adUnitId}: {message}");
    }
}
