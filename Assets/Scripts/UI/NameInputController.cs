using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NameInputController : MonoBehaviour
{
    public TMP_InputField inputField;
    private string playerName;

    void Start()
    {
        playerName = PlayerPrefs.GetString("PlayerName", "");
        inputField.text = playerName;
    }

    public void HandleNameChange(string name)
    {
        playerName = name;
        SavePlayerName(playerName);
    }

    void SavePlayerName(string name)
    {
        Debug.Log("Player name saved: " + name);

        PlayerPrefs.SetString("PlayerName", name);
        PlayerPrefs.Save();
    }
}
