using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class CallStageCountdown : MonoBehaviour
{
    const string countdownScene = "CountDown";

    public bool isFinish { get; private set; } = false;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(1.0f);

        SceneManager.LoadScene(countdownScene, LoadSceneMode.Additive);

        yield return StartCoroutine(WaitForUnloadScene());

        isFinish = true;
    }

    IEnumerator WaitForUnloadScene()
    {
        while (SceneManager.sceneCount > 1)
        {
            yield return null;
        }
    }
}
