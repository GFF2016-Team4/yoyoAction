using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SplashScene : MonoBehaviour
{
    public float showTime = 3;

    public string TitleName;

    public FadeManager manager;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(showTime);

        //シーン遷移
        manager.SceneChange(TitleName);
    }
}
