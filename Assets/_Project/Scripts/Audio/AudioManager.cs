using System.Collections;
using UnityEngine;
using Game.Core;
using Game.Events;
using Game.UI;

namespace Game.Audio
{
    /// <summary>
    /// Quản lý phát nhạc nền (BGM) và hiệu ứng âm thanh (SFX) cho trò chơi.
    /// Chuẩn Universal Core: Độc lập với gameplay, hỗ trợ WebGL Autoplay Gesture & Decoupled State Events.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource _bgmSource;
        [SerializeField] private AudioSource _sfxSource;

        [Header("Core Audio Clips")]
        [SerializeField] private AudioClip _bgmGameplay;
        [SerializeField] private AudioClip _sfxClick;
        [SerializeField] private AudioClip _sfxWin;
        [SerializeField] private AudioClip _sfxLose;
        [SerializeField] private AudioClip _sfxReward;

        [Header("Event Channels")]
        [SerializeField] private GameStateEventChannelSO _onGameStateChangedChannel;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (_bgmSource == null)
            {
                _bgmSource = gameObject.AddComponent<AudioSource>();
                _bgmSource.loop = true;
                _bgmSource.playOnAwake = false;
            }

            if (_sfxSource == null)
            {
                _sfxSource = gameObject.AddComponent<AudioSource>();
                _sfxSource.loop = false;
                _sfxSource.playOnAwake = false;
            }

            ApplySavedAudioSettings();
        }

        private void OnEnable()
        {
            if (_onGameStateChangedChannel != null)
            {
                _onGameStateChangedChannel.OnEventRaised += HandleGameStateChanged;
            }
        }

        private void OnDisable()
        {
            if (_onGameStateChangedChannel != null)
            {
                _onGameStateChangedChannel.OnEventRaised -= HandleGameStateChanged;
            }
        }

        private void Start()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            StartCoroutine(WaitForFirstUserGestureRoutine());
#else
            if (_bgmGameplay != null)
            {
                PlayBGM(_bgmGameplay);
            }
#endif
        }

        private IEnumerator WaitForFirstUserGestureRoutine()
        {
            while (!Input.anyKeyDown && !Input.GetMouseButtonDown(0) && Input.touchCount == 0)
            {
                yield return null;
            }
            if (_bgmGameplay != null)
            {
                PlayBGM(_bgmGameplay);
            }
        }

        public void PlayBGM(AudioClip clip)
        {
            if (_bgmSource != null && clip != null)
            {
                if (_bgmSource.clip == clip && _bgmSource.isPlaying) return;
                _bgmSource.clip = clip;
                _bgmSource.Play();
            }
        }

        public void StopBGM()
        {
            if (_bgmSource != null)
            {
                _bgmSource.Stop();
            }
        }

        public void PlaySFX(AudioClip clip)
        {
            if (_sfxSource != null && clip != null)
            {
                _sfxSource.PlayOneShot(clip);
            }
        }

        public void PlayClick()
        {
            PlaySFX(_sfxClick);
        }

        public void PlayWin()
        {
            PlaySFX(_sfxWin);
        }

        public void PlayLose()
        {
            PlaySFX(_sfxLose);
        }

        public void PlayReward()
        {
            PlaySFX(_sfxReward);
        }

        public void SetSFXMuted(bool isMuted)
        {
            if (_sfxSource != null)
            {
                _sfxSource.mute = isMuted;
            }
        }

        public void SetBGMMuted(bool isMuted)
        {
            if (_bgmSource != null)
            {
                _bgmSource.mute = isMuted;
            }
        }

        private void ApplySavedAudioSettings()
        {
            bool soundMuted = PlayerPrefs.GetInt(SettingsPopup.PrefSfx, 1) == 0;
            bool musicMuted = PlayerPrefs.GetInt(SettingsPopup.PrefBgm, 1) == 0;
            SetSFXMuted(soundMuted);
            SetBGMMuted(musicMuted);
        }

        private void HandleGameStateChanged(GameState newState)
        {
            switch (newState)
            {
                case GameState.Victory:
                    PlayWin();
                    break;
                case GameState.GameOver:
                    PlayLose();
                    break;
            }
        }
    }
}
