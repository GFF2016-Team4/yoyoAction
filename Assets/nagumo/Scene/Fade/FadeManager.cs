using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour
{
    [Header("デフォルトのフェードの速さ")]
    public const float defaultFadeSpeed = 1.2f;

    Image image;

    bool isFade        = false;
    bool isSceneChange = false;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private IEnumerator Start()
    {
        yield return null;
        FadeIn();
    }

    public Coroutine FadeIn(float fadeSpeed = defaultFadeSpeed)
    {
        if(isFade) return null;
        return StartCoroutine(FadeIn_(fadeSpeed));
    }

    IEnumerator FadeIn_(float fadeSpeed)
    {
        isFade = true;
        Time.timeScale = 0;
        for(float t = 1.0f; t >= 0.0f; t -= Time.unscaledDeltaTime * fadeSpeed)
        {
            SetAlpha(t);
            yield return null;
        }
        Time.timeScale = 1;
        SetAlpha(0.0f);
        isFade = false;
    }

    public Coroutine FadeOut(float fadeSpeed = defaultFadeSpeed)
    {
        if(isFade) return null;
        return StartCoroutine(FadeOut_(fadeSpeed));
    }

    IEnumerator FadeOut_(float fadeSpeed)
    {
        isFade = true;
        for(float t = 0.0f; t <= 1.0f; t += Time.deltaTime * fadeSpeed)
        {
            SetAlpha(t);
            yield return null;
        }
        SetAlpha(1.0f);
        isFade = false;
    }

    void SetAlpha(float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }

    public Coroutine SceneChange(string sceneName, float fadeSpeed = defaultFadeSpeed)
    {
        if(isSceneChange) return null;
        return StartCoroutine(SceneChange_(sceneName, fadeSpeed));
    }

    IEnumerator SceneChange_(string sceneName, float fadeSpeed)
    {
        isSceneChange = true;

        var eventSystem = FindObjectOfType<EventSystem>();
        eventSystem.enabled = false;

        yield return FadeOut(fadeSpeed);
        
        //SoundManager.Instance.FadeOutBGM(0.8f);

        Resources.LoadAll("Resources", typeof(GameObject));
        yield return null;

        SceneManager.LoadSceneAsync("LoadScene", LoadSceneMode.Additive);
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false;
        while(async.progress < 0.9f && async.isDone == false)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        async.allowSceneActivation = true;
        isSceneChange = false;
    }

    public Coroutine GameQuit(float fadeSpeed = defaultFadeSpeed)
    {
        if(isSceneChange) return null;
        return StartCoroutine(GameQuit_(fadeSpeed));
    }

    IEnumerator GameQuit_(float fadeSpeed)
    {
        isSceneChange = true;
        yield return FadeOut(fadeSpeed);
        Application.Quit();
        isSceneChange = false;
    }
}