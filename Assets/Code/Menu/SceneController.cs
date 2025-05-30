using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField] private float _sceneFadeDuration = 1f;
    [SerializeField] private SceneFade _sceneFade; // ต้อง drag เข้ามาใน Inspector

    private void Start()
    {
        if (_sceneFade != null)
        {
            StartCoroutine(_sceneFade.FadeInCoroutine(_sceneFadeDuration));
        }
        else
        {
            Debug.LogError("SceneFade ยังไม่ได้ assign ใน SceneController");
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        if (_sceneFade != null)
        {
            yield return _sceneFade.FadeOutCoroutine(_sceneFadeDuration);
        }

        yield return SceneManager.LoadSceneAsync(sceneName);
    }
}