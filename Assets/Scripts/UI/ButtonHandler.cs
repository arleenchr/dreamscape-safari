using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] TextMeshProUGUI buttonText;
    private AudioSource clickEffect;

    private void Awake()
    {
        clickEffect = GetComponent<AudioSource>();
    }
    private void Start()
    {
        foreach(Button button in FindObjectsByType<Button>(FindObjectsSortMode.None))
        {
            button.onClick.AddListener(PlayClickSound);
        }
    }

    private void OnDestroy()
    {
        foreach (Button button in FindObjectsByType<Button>(FindObjectsSortMode.None))
        {
            button.onClick.RemoveListener(PlayClickSound);
        }
    }

    public void PlayClickSound()
    {
        Debug.Log("bunyi gan");
        clickEffect.Play();
    }

    public void OnButtonSelected()
    {
        buttonText.color = Color.white;
    }
}
