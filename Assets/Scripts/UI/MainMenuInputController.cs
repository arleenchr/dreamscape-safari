using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuInputController : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject statsPanel;
    [SerializeField] GameObject settingsPanel;

    void Start()
    {
        mainMenu.SetActive(true);
        statsPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    public void OnStatsClick()
    {
        mainMenu.SetActive(false);
        statsPanel.SetActive(true);
    }

    public void OnSettingsPanel()
    {
        mainMenu.SetActive(false);
        settingsPanel.SetActive(true);
    }
}
