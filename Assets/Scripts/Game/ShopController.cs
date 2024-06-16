using BondomanShooter.Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class ShopController : MonoBehaviour
{
    public int PlayerCoins {
        get => GameController.Instance.Coins; 
        set { GameController.Instance.Coins = value; }
    }
    public TextMeshProUGUI playerCoinsText;

    private const int petPrice = 30;

    public GameObject attackerPetPanel;
    public GameObject healingPetPanel;

    public Button attackerPurchaseButton;
    public Button healingPurchaseButton;

    public TextMeshProUGUI attackerPurchaseButtonText;
    public TextMeshProUGUI healingPurchaseButtonText;

    public InventoryController inventoryController;
    void Start()
    {
        playerCoinsText.text = GameController.Instance.IsMotherlode ? "unlimited" : PlayerCoins.ToString();

        attackerPurchaseButton.onClick.AddListener(() => PurchasePet("Attacker"));
        healingPurchaseButton.onClick.AddListener(() => PurchasePet("Healing"));

        Debug.Log("ShopController initialized.");
        Debug.Log($"Player starts with {(GameController.Instance.IsMotherlode ? "unlimited" : PlayerCoins.ToString())} coins.");

        //inventoryController = GetComponent<InventoryController>();

        UpdatePurchaseButtonStates();
    }

    private void Update()
    {
        playerCoinsText.text = GameController.Instance.IsMotherlode ? "unlimited" : PlayerCoins.ToString();
        UpdatePurchaseButtonStates();
    }

    void PurchasePet(string petType)
    {
        int petTypeIdx = (petType=="Healing") ? 0 : 1;

        Debug.Log($"Attempting to purchase {petType} {petTypeIdx} pet.");

        if (GameController.Instance.player.PetOwner.OwnedPets[petTypeIdx])
        {
            Debug.Log($"Player already owns a {petType} pet. Cannot purchase another.");
            return;
        }

        if (PlayerCoins >= petPrice || GameController.Instance.IsMotherlode)
        {
            PlayerCoins -= GameController.Instance.IsMotherlode ? 0 : petPrice;
            playerCoinsText.text = GameController.Instance.IsMotherlode ? "unlimited" : PlayerCoins.ToString();
            GameController.Instance.CoinsSpent += petPrice;
            Debug.Log($"Pet purchased! Deducted {petPrice} coins. Player now has {PlayerCoins} coins.");

            if (petType == "Attacker")
            {
                //inventoryController.AddPetToInventory("Attacker");
                //attackerPurchaseButtonText.text = "Purchased";
                //attackerPurchaseButtonText.color = Color.white;

                GameController.Instance.player.PetOwner.OwnedPets[1] = true;
                GameController.Instance.player.PetOwner.Pets[1].RevivePet();

                Debug.Log("Purchased an Attacker pet.");
            }
            else if (petType == "Healing")
            {
                //inventoryController.AddPetToInventory("Healing");
                //healingPurchaseButtonText.text = "Purchased";
                //healingPurchaseButtonText.color = Color.white;

                GameController.Instance.player.PetOwner.OwnedPets[0] = true;
                GameController.Instance.player.PetOwner.Pets[0].RevivePet();

                Debug.Log("Purchased a Healing pet.");
            }
        }
        else
        {
            Debug.Log("Not enough coins to purchase the pet.");
        }

        UpdatePurchaseButtonStates();
    }

    void UpdatePurchaseButtonStates()
    {
        if (GameController.Instance.player.PetOwner.OwnedPets[1])
        {
            attackerPurchaseButton.interactable = false;
            attackerPurchaseButtonText.color = Color.white;
            attackerPurchaseButtonText.text = "Purchased";
        }
        else if (!GameController.Instance.IsMotherlode && PlayerCoins < petPrice)
        {
            attackerPurchaseButton.interactable = false;
            attackerPurchaseButtonText.color = Color.gray;
            attackerPurchaseButtonText.text = "Purchase";
        }
        else
        {
            attackerPurchaseButton.interactable = true;
            attackerPurchaseButtonText.color = new Color(1f, 194f / 255, 69f / 255, 1f);
            attackerPurchaseButtonText.text = "Purchase";
        }

        if (GameController.Instance.player.PetOwner.OwnedPets[0])
        {
            healingPurchaseButton.interactable = false;
            healingPurchaseButtonText.color = Color.white;
            healingPurchaseButtonText.text = "Purchased";
        }
        else if (!GameController.Instance.IsMotherlode && PlayerCoins < petPrice)
        {
            healingPurchaseButton.interactable = false;
            healingPurchaseButtonText.color = Color.gray;
            healingPurchaseButtonText.text = "Purchase";
        }
        else 
        {
            healingPurchaseButton.interactable = true;
            healingPurchaseButtonText.color = new Color(1f, 194f / 255, 69f / 255, 1f);
            healingPurchaseButtonText.text = "Purchase";
        }
    }
}
