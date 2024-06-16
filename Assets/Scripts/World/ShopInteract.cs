using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShopInteract : MonoBehaviour, IInteractable
{
    [Header("UI Elements")]
    [SerializeField] private GameObject DarkenOverlay;
    [SerializeField] private GameObject ShopPanel;

    public void OnInteract()
    {
        DarkenOverlay.SetActive(!DarkenOverlay.activeSelf);
        ShopPanel.SetActive(!ShopPanel.activeSelf);
    }
}
