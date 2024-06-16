using TMPro;
using UnityEngine;

namespace BondomanShooter.UI {
    public class SaveGameButtonHandler : MonoBehaviour {
        [SerializeField] private TMP_InputField saveNameText;
        [SerializeField] private int saveSlotIndex;

        public void OnSaveGame() {
            if(DataPersistenceManager.Instance == null) {
                Debug.LogError($"No instance of {nameof(DataPersistenceManager)} found in the scene!");
                return;
            }

            if(string.IsNullOrEmpty(saveNameText.text)) {
                Debug.LogWarning("No save name is provided");
                return;
            }

            DataPersistenceManager.Instance.SaveGame(saveSlotIndex, saveNameText.text);
        }
    }
}
