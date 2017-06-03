using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

namespace TestScript
{
    public class GotoResult : MonoBehaviour
    {
        [SerializeField]
        UnityEvent sceneChangeEvent;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                sceneChangeEvent.Invoke();
                SceneManager.LoadScene("timerResult");
            }
        }
    }
}