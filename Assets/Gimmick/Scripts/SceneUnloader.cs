using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class SceneUnloader : MonoBehaviour
{
    //このコンポーネントがあるシーンを削除
    //UnityActionに登録するためにMonoBehaviour継承
    public void UnloadScene()
    {
        Scene unloadScene = gameObject.scene;
        SceneManager.UnloadSceneAsync(unloadScene);
    }
}
