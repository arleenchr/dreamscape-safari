using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseButtonPressed : MonoBehaviour
{
    [SerializeField] Button Button;
    [SerializeField] TextMeshProUGUI ButtonText;

    public void OnButtonPurchasedClicked()
    {
        Debug.Log("KEPENCET");
        ButtonText.color = Color.white;
        ButtonText.text = "Purchased";

        Button.interactable = false;
    }
}
