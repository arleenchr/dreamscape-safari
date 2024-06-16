using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonPressed : MonoBehaviour
{
    [SerializeField] Button ButtonEquipPet1;
    [SerializeField] TextMeshProUGUI ButtonEquipPet1Text;
    [SerializeField] Button ButtonEquipPet2;
    [SerializeField] TextMeshProUGUI ButtonEquipPet2Text;

    //public UnityEvent onButtonPet1Clicked;
    //public UnityEvent onButtonPet2Clicked;

    private void Start()
    {
        ButtonEquipPet1.onClick.AddListener(() => OnButtonPet1Clicked());
        ButtonEquipPet2.onClick.AddListener(() => OnButtonPet2Clicked());
    }

    public void OnButtonPet1Clicked()
    {
        Debug.Log("KEPENCET");
        ButtonEquipPet1Text.color = Color.white;
        ButtonEquipPet1Text.text = "Equipped";

        ButtonEquipPet2.interactable = false;
    }

    public void OnButtonPet2Clicked()
    {
        Debug.Log("KEPENCET");
        ButtonEquipPet2Text.color = Color.white;
        ButtonEquipPet2Text.text = "Equipped";

        ButtonEquipPet1.interactable = false;
    }
}
