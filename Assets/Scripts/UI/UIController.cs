using TMPro;
using BondomanShooter.Game;
using UnityEngine;
using UnityEngine.UI;
using BondomanShooter.Game.Quest;

namespace BondomanShooter.UI {
    public class UIController : MonoBehaviour {
        [SerializeField] TextMeshProUGUI timerText;
        [SerializeField] TextMeshProUGUI questTitle;
        [SerializeField] TextMeshProUGUI questDesc;
        [SerializeField] Slider healthBar;
        [SerializeField] Slider petHealthBar;

        private void Start() {
            questTitle.text = "";
            questDesc.text = "";
            timerText.text = "";
        }

        public static string FormatTime(float time, string type)
        {
            int hours = 0;
            if (type == "hms")
                hours = Mathf.Max(0, Mathf.FloorToInt(time / 3600));
                
            int minutes = Mathf.Max(0, Mathf.FloorToInt(time / 60));
            int seconds = Mathf.Max(0, Mathf.FloorToInt(time % 60));

            return type=="hms" ? string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds) : string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        void Update() {
            if(GameController.Instance == null) return;

            healthBar.value = (float)GameController.Instance.player.Health / GameController.Instance.player.MaxHealth;

            for (int i = 0; i < GameController.Instance.player.PetOwner.Pets.Count; i++)
            {
                if (GameController.Instance.player.PetOwner.EquippedPets[i])
                {
                    petHealthBar.gameObject.SetActive(true);
                    petHealthBar.value = (float)GameController.Instance.player.PetOwner.Pets[i].Health / GameController.Instance.player.PetOwner.Pets[i].MaxHealth;
                    break;
                }
                else
                {
                    petHealthBar.gameObject.SetActive(false);
                }
            }

            if (GameController.Instance.QuestStarted && !GameController.Instance.IsQuestFinished()) {
                questTitle.text = GameController.Instance.quest.Title;
                questDesc.text = GameController.Instance.quest.Description;

                if(GameController.Instance.TimeRemaining > 0) {
                    float timeRemaining = GameController.Instance.TimeRemaining;

                    timerText.text = FormatTime(timeRemaining, "ms");
                }
            }
            else if (GameController.Instance.lounge != null)
            {
                questTitle.text = GameController.Instance.lounge.Title;
                questDesc.text = GameController.Instance.lounge.Description;

                if (!GameController.Instance.IsLoungeFinished() && GameController.Instance.TimeRemaining > 0)
                {
                    float timeRemaining = GameController.Instance.TimeRemaining;

                    timerText.text = FormatTime(timeRemaining, "ms");
                }
            }
            else {
                questTitle.text = "";
                questDesc.text = "";
                timerText.text = "";
            }
        }
    }
}
