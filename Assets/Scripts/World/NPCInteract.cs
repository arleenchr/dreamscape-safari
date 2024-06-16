using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class NPCInteract : MonoBehaviour, IInteractable
{
    [Header("UI Elements")]
    [SerializeField] private GameObject storyBoardPanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private string dialogueContent;

    public void OnInteract()
    {
        storyBoardPanel.SetActive(!storyBoardPanel.activeSelf);
        dialogueText.enabled = !!dialogueText.enabled;
        dialogueText.text = dialogueContent;
    }
}
