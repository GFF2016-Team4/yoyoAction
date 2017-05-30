using UnityEngine;
using System.Collections;

public class GameOver : MonoBehaviour
{
    public FadeManager m_fademanager;
    public string sceneName;

    private bool judge = false;

    void Update()
    {
        if (judge == false) m_fademanager.FadeIn();
        else m_fademanager.SceneChange(sceneName);

    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.name == "Enemy")
        {
            judge = true;
            //m_fademanager.FadeMainOut(sceneName);
        }
    }

}
