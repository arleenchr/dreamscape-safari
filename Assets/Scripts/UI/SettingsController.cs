using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BondomanShooter.Audio;
using UnityEngine.Localization.Settings;

public class SettingsController : MonoBehaviour
{
    public static SettingsController Instance { get; private set; }

    public TMP_InputField playerNameInput;
    public Slider bgmVolumeSlider;
    public Slider sfxVolumeSlider;
    public ToggleGroup difficultyToggleGroup;
    public Toggle easyToggle;
    public Toggle mediumToggle;
    public Toggle hardToggle;
    public ToggleGroup languageToggleGroup;
    public Toggle englishToggle;
    public Toggle bahasaToggle;

    public string Difficulty { get; private set; }

    private const string PlayerNameKey = "PlayerName";
    private const string BGMVolumeKey = "BGMVolume";
    private const string SFXVolumeKey = "SFXVolume";
    private const string DifficultyKey = "Difficulty";
    private const string LanguageKey = "Language";

    private void Awake()
    {
        if (Instance != null) Destroy(this);
        Instance = this;
    }

    private void Start()
    {
        LoadPrefs();

        playerNameInput.onValueChanged.AddListener(value => PlayerPrefs.SetString(PlayerNameKey, value));
        bgmVolumeSlider.onValueChanged.AddListener(value => PlayerPrefs.SetFloat(BGMVolumeKey, value));
        sfxVolumeSlider.onValueChanged.AddListener(value => PlayerPrefs.SetFloat(SFXVolumeKey, value));

        easyToggle.onValueChanged.AddListener(value => PlayerPrefs.SetString(DifficultyKey, "Easy"));
        mediumToggle.onValueChanged.AddListener(value => PlayerPrefs.SetString(DifficultyKey, "Medium"));
        hardToggle.onValueChanged.AddListener(value => PlayerPrefs.SetString(DifficultyKey, "Hard"));

        englishToggle.onValueChanged.AddListener(value => PlayerPrefs.SetString(LanguageKey, "EN"));
        bahasaToggle.onValueChanged.AddListener(value => PlayerPrefs.SetString(LanguageKey, "ID"));

        bgmVolumeSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        englishToggle.onValueChanged.AddListener(OnLanguageToggleChanged);
        englishToggle.onValueChanged.AddListener(OnLanguageToggleChanged);
    }


    public void SavePrefs()
    {
        string playerName = playerNameInput.text;
        PlayerPrefs.SetString(PlayerNameKey, playerName);

        float bgmVolume = bgmVolumeSlider.value;
        PlayerPrefs.SetFloat(BGMVolumeKey, bgmVolume);
        AudioController.Instance.SetBGMVolume(bgmVolume);

        float sfxVolume = sfxVolumeSlider.value;
        PlayerPrefs.SetFloat(SFXVolumeKey, sfxVolume);
        AudioController.Instance.SetSFXVolume(sfxVolume);

        string difficulty = GetSelectedToggle(difficultyToggleGroup);
        PlayerPrefs.SetString(DifficultyKey, difficulty);

        string language = GetSelectedToggle(languageToggleGroup);
        PlayerPrefs.SetString(LanguageKey, language);

        PlayerPrefs.Save();
    }

    public void LoadPrefs()
    {
        if (PlayerPrefs.HasKey(PlayerNameKey))
        {
            string playerName = PlayerPrefs.GetString(PlayerNameKey);
            Debug.Log($"Loaded player name: {playerName}");
            playerNameInput.text = playerName;
        }

        //if (PlayerPrefs.HasKey(BGMVolumeKey))
        //{
        float bgmVolume = PlayerPrefs.GetFloat(BGMVolumeKey, 50f);
        Debug.Log($"Loaded BGM volume: {bgmVolume}");
        bgmVolumeSlider.value = bgmVolume;
        OnBGMVolumeChanged(bgmVolume);

        //AudioController.Instance.SetBGMVolume(bgmVolume);
        //}

        //if (PlayerPrefs.HasKey(SFXVolumeKey))
        //{
        float sfxVolume = PlayerPrefs.GetFloat(SFXVolumeKey, 80f);
        Debug.Log($"Loaded SFX volume: {sfxVolume}");
        sfxVolumeSlider.value = sfxVolume;
        OnSFXVolumeChanged(sfxVolume);
        //AudioController.Instance.SetSFXVolume(sfxVolume);
        //}

        string difficulty = "Medium";
        if (PlayerPrefs.HasKey(DifficultyKey))
        {
            difficulty = PlayerPrefs.GetString(DifficultyKey);
            Debug.Log($"Loaded difficulty: {difficulty}");
            SetSelectedToggle(difficultyToggleGroup, difficulty);
        }
        Difficulty = difficulty;

        if (PlayerPrefs.HasKey(LanguageKey))
        {
            string language = PlayerPrefs.GetString(LanguageKey);
            Debug.Log($"Loaded language: {language}");
            SetSelectedToggle(languageToggleGroup, language);
        }
    }


    private string GetSelectedToggle(ToggleGroup toggleGroup)
    {
        Toggle selectedToggle = toggleGroup.ActiveToggles().FirstOrDefault();
        return selectedToggle != null ? selectedToggle.name : string.Empty;
    }

    private void SetSelectedToggle(ToggleGroup toggleGroup, string name)
    {
        Toggle[] toggles = toggleGroup.GetComponentsInChildren<Toggle>();
        foreach (var toggle in toggles)
        {
            toggle.isOn = toggle.name == name;
        }
    }
    private void OnBGMVolumeChanged(float volume)
    {
        AudioController.Instance.SetBGMVolume(volume);
    }

    private void OnSFXVolumeChanged(float volume)
    {
        AudioController.Instance.SetSFXVolume(volume);
    }

    private void OnLanguageToggleChanged(bool isEnglish)
    {
        if (isEnglish)
        {
            PlayerPrefs.SetString(LanguageKey, "EN");
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
        }
        else
        {
            PlayerPrefs.SetString(LanguageKey, "ID");
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[1];
        }
    }
}
