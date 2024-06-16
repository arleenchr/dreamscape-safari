using UnityEngine;
using UnityEngine.Audio;

namespace BondomanShooter.Audio {
    [RequireComponent(typeof(AudioSource))]
    public class AudioController : MonoBehaviour {
        public static AudioController Instance { get ; private set; }

        [Header("References")]
        [SerializeField] private AudioMixer mixer;

        private const string SFXVolumeKey = "EffectsVolume";
        private const string BGMVolumeKey = "MusicVolume";

        public float Pitch {
            get => bgmSource.pitch;
            set => bgmSource.pitch = value;
        }

        private AudioSource bgmSource;

        private void Awake() {
            if(Instance != null) Destroy(this);
            Instance = this;

            bgmSource = GetComponent<AudioSource>();
        }

        private void Start() {
            /// TODO: Set mixer volumes from PlayerPrefs
            ///
            //if (PlayerPrefs.HasKey(SFXVolumeKey))
            //{
            //    float sfxVolume = PlayerPrefs.GetFloat(SFXVolumeKey);
            //    SetSFXVolume(sfxVolume);
            //}
            //if(PlayerPrefs)
        }

        public void ChangeBGM(AudioClip bgmClip) {
            bgmSource.clip = bgmClip;
            bgmSource.time = 0f;
            PlayBGM();
        }

        public void PlayBGM() {
            bgmSource.Play();
        }

        public void StopBGM() {
            bgmSource.Stop();
        }

        public void SetBGMVolume(float volume)
        {
            mixer.SetFloat(BGMVolumeKey, Mathf.Lerp(-80f, 0f, volume / 100f));
        }

        public void SetSFXVolume(float volume)
        {
            mixer.SetFloat(SFXVolumeKey, Mathf.Lerp(-80f, 0f, volume / 100f));
        }
    }
}
