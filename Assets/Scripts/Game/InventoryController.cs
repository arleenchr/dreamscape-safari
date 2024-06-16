using BondomanShooter.Game;
using System.Runtime.ConstrainedExecution;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    public int PlayerCoins
    {
        get => GameController.Instance.Coins;
        set { GameController.Instance.Coins = value; }
    }
    public TextMeshProUGUI playerCoinsText;

    public GameObject petPanel1; 
    public GameObject petPanel2; 

    public Button equipButton1;
    public TextMeshProUGUI equipButton1Text;
    public Button equipButton2;
    public TextMeshProUGUI equipButton2Text;

    //private bool hasAttackerPet = GameController.Instance.player.PetOwner.OwnedPets[1];
    //private bool hasHealingPet = GameController.Instance.player.PetOwner.OwnedPets[0];

    //private bool isAttackerPetEquipped = GameController.Instance.player.PetOwner.EquippedPets[1];
    //private bool isHealingPetEquipped = GameController.Instance.player.PetOwner.EquippedPets[0];

    void Start()
    {
        playerCoinsText.text = GameController.Instance.IsMotherlode ? "unlimited" : PlayerCoins.ToString();

        equipButton1.onClick.AddListener(() => OnEquipPet("Attacker"));
        equipButton2.onClick.AddListener(() => OnEquipPet("Healing"));

        Debug.Log("InventoryController initialized.");

        UpdateEquipButtonStates();
    }

    private void Update()
    {
        playerCoinsText.text = GameController.Instance.IsMotherlode ? "unlimited" : PlayerCoins.ToString();

        //if (!GameController.Instance.player.PetOwner.OwnedPets[1])
        //{
        //    equipButton1.interactable = false; // attacker pet
        //    equipButton1Text.color = Color.gray;
        //}
        //else
        //{
        //    equipButton1.interactable = true; // attacker pet
        //    equipButton1Text.color = new Color(1f, 194f / 255, 69f / 255, 1f);
        //}

        //if (!GameController.Instance.player.PetOwner.OwnedPets[0])
        //{
        //    equipButton2.interactable = false; // healing pet
        //    equipButton2Text.color = Color.gray;
        //}
        //else
        //{
        //    equipButton2.interactable = true; // healing pet
        //    equipButton2Text.color = new Color(1f, 194f / 255, 69f / 255, 1f);
        //}

        UpdateEquipButtonStates();
    }

    //public void AddPetToInventory(string petType)
    //{
    //    if (petType == "Attacker")
    //    {
    //        hasAttackerPet = true;
    //        Debug.Log("Added Attacker pet to inventory.");
    //    }
    //    else if (petType == "Healing")
    //    {
    //        hasHealingPet = true;
    //        Debug.Log("Added Healing pet to inventory.");
    //    }

    //    UpdateEquipButtonStates();
    //}

    void OnEquipPet(string petType)
    {
        if (petType == "Attacker")
        {
            if (!GameController.Instance.player.PetOwner.OwnedPets[1])
            {
                Debug.Log("Cannot equip Attacker pet. You do not have it in your inventory.");
                return;
            }

            //GameController.Instance.player.PetOwner.EquippedPets[1] = !GameController.Instance.player.PetOwner.EquippedPets[1];
            //GameController.Instance.player.PetOwner.EquippedPets[1] = true;
            GameController.Instance.player.PetOwner.EquipPet(1);

            Debug.Log(GameController.Instance.player.PetOwner.EquippedPets[1] ? "Attacker pet equipped." : "Attacker pet unequipped.");

            //equipButton1.interactable = false;
        }
        else if (petType == "Healing")
        {
            if (!GameController.Instance.player.PetOwner.OwnedPets[0])
            {
                Debug.Log("Cannot equip Healing pet. You do not have it in your inventory.");
                return;
            }

            //GameController.Instance.player.PetOwner.EquippedPets[0] = true;
            GameController.Instance.player.PetOwner.EquipPet(0);

            Debug.Log(GameController.Instance.player.PetOwner.EquippedPets[0] ? "Healing pet equipped." : "Healing pet unequipped.");

            //equipButton2.interactable = false;
        }

        UpdateEquipButtonStates();
    }

    //public bool HasPet(string petType)
    //{
    //    if (petType == null)
    //    {
    //        Debug.LogWarning("HasPet was called with a null petType.");
    //        return false;
    //    }

    //    if (petType == "Attacker")
    //    {
    //        return hasAttackerPet;
    //    }
    //    else if (petType == "Healing")
    //    {
    //        return hasHealingPet;
    //    }
    //    return false;
    //}

    void UpdateEquipButtonStates()
    {
        equipButton1.interactable = GameController.Instance.player.PetOwner.OwnedPets[1] && !GameController.Instance.player.PetOwner.EquippedPets[1];
        if (equipButton1.interactable)
        {
            equipButton1Text.color = new Color(1f, 194f / 255, 69f / 255, 1f);
            equipButton1Text.text = "Equip";
        }
        else if (GameController.Instance.player.PetOwner.EquippedPets[1])
        {
            equipButton1Text.color = Color.white;
            equipButton1Text.text = "Equipped";
        }
        else
        {
            equipButton1Text.color = Color.gray;
            equipButton1Text.text = "Equip";
        }

        equipButton2.interactable = GameController.Instance.player.PetOwner.OwnedPets[0] && !GameController.Instance.player.PetOwner.EquippedPets[0];
        if (equipButton2.interactable)
        {
            equipButton2Text.color = new Color(1f, 194f / 255, 69f / 255, 1f);
            equipButton2Text.text = "Equip";
        }
        else if (GameController.Instance.player.PetOwner.EquippedPets[0])
        {
            equipButton2Text.color = Color.white;
            equipButton2Text.text = "Equipped";
        }
        else
        {
            equipButton2Text.color = Color.gray;
            equipButton2Text.text = "Equip";
        }
    }
}

