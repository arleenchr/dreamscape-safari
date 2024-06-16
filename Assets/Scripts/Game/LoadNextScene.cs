using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNextScene : MonoBehaviour
{
    [SerializeField] string nextScene;
    [SerializeField] private string canTriggerByTag;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(canTriggerByTag))
        {
            DataPersistenceManager.Instance.StoreCurrentState();
            SceneManager.LoadScene(nextScene);
        }
    }
}
