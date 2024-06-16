using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNextSceneTimeline : MonoBehaviour
{
    [SerializeField] string nextScene;

    void OnEnable()
    {
        SceneManager.LoadScene(nextScene);
    }
}
