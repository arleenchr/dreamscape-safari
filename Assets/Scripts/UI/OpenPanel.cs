using BondomanShooter.Game;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BondomanShooter.UI {
    public class OpenPanel : MonoBehaviour {
        [Header("UI Elements")]
        [SerializeField] private GameObject darkenOverlay;
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private GameObject pausePanel;

        public void OnInputToggleInventory(InputAction.CallbackContext ctx) {
            if(ctx.performed) {
                darkenOverlay.SetActive(!darkenOverlay.activeSelf);
                inventoryPanel.SetActive(!inventoryPanel.activeSelf);
                TimeControl.Instance.IsPaused = !TimeControl.Instance.IsPaused;
            }
        }

        public void OnInputTogglePause(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                darkenOverlay.SetActive(!darkenOverlay.activeSelf);
                pausePanel.SetActive(!inventoryPanel.activeSelf);
                TimeControl.Instance.IsPaused = !TimeControl.Instance.IsPaused;
            }
        }
    }
}
