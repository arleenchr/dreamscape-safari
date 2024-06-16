using UnityEngine;
using UnityEngine.EventSystems;

namespace BondomanShooter.UI {
    public class ClosePanel : MonoBehaviour, ICancelHandler {
        [Header("UI Elements")]
        [SerializeField] private GameObject mainMenu;
        [SerializeField] private GameObject panel;

        public void OnCancel(BaseEventData data)
        {
            Debug.Log("ESCAPEEEEE");
            mainMenu.SetActive(true);
            panel.SetActive(false);

        }

        //public void OnInputTogglePanel(InputAction.CallbackContext ctx) {
        //    if(ctx.performed) {
        //        panel.SetActive(!panel.activeSelf);
        //    }
        //}
    }
}
