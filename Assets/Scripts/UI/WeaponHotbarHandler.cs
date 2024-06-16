using BondomanShooter.Entities;
using BondomanShooter.Entities.Player;
using BondomanShooter.Items.Weapons;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace BondomanShooter.UI {
    public class WeaponHotbarHandler : MonoBehaviour {
        [Header("UI Elements")]
        [SerializeField] private Button[] hotbarButtons;

        [SerializeField] private Sprite unselectedSprite;
        [SerializeField] private Sprite selectedSprite;
        [SerializeField] private Sprite disabledSprite;

        private PlayerController player;
        private WeaponOwner playerWeaponOwner;

        private int index;
        private int SelectedIndex {
            get => index;
            set {
                if (!((IHealth)player).IsDead && playerWeaponOwner.EnabledWeapons[value]) {
                    // Change old selected weapon button to sprite unselected
                    hotbarButtons[index].GetComponent<Image>().sprite = unselectedSprite;

                    // Update index value and update new selected weapon button sprite to selected
                    index = value;
                    hotbarButtons[index].GetComponent<Image>().sprite = selectedSprite;

                    // Change the selected weapon of the player
                    playerWeaponOwner.Select(index);
                }
            }
        }

        private void Awake() {
            player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            playerWeaponOwner = player.GetComponent<WeaponOwner>();
        }

        private void Start() {
            int i = 0;
            foreach(Button button in hotbarButtons) {
                if (!playerWeaponOwner.EnabledWeapons[i++])
                {
                    button.interactable = false;
                }
                button.GetComponent<Image>().sprite = button.interactable ? unselectedSprite : disabledSprite;
            }

            SelectedIndex = playerWeaponOwner.SelectedWeaponIndex;
        }

        public void OnHotbarClick(int index) {
            SelectedIndex = index;
        }

        public void OnInputHotbar1(InputAction.CallbackContext ctx) {
            if(ctx.performed) OnHotbarClick(0);
        }

        public void OnInputHotbar2(InputAction.CallbackContext ctx) {
            if(ctx.performed) OnHotbarClick(1);
        }

        public void OnInputHotbar3(InputAction.CallbackContext ctx) {
            if(ctx.performed) OnHotbarClick(2);
        }
    }
}
