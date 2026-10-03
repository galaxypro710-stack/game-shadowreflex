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

    public enum HapticIntensity
    {
        Light,
        Medium,
        Heavy
    }

    [Header("Core Audio Clips")]
        [SerializeField] private AudioClip _bgmGameplay;
        [SerializeField] private AudioClip _sfxClick;
        [SerializeField] private AudioClip _sfxWin;
        [SerializeField] private AudioClip _sfxLose;
        [SerializeField] private AudioClip _sfxReward;

        [Header("Combat & Timing SFX")]
        [SerializeField] private AudioClip _sfxSlash;
        [SerializeField] private AudioClip _sfxWhoosh;
        [SerializeField] private AudioClip _sfxParryClang;
        [SerializeField] private AudioClip _sfxHitImpact;
        [SerializeField] private AudioClip _sfxTelegraphWarning;

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
            EnsureCombatClipsLoaded();
            PlaySFX(_sfxClick);
            TriggerHaptic(HapticIntensity.Light);
        }

        public void PlayWin()
        {
            EnsureCombatClipsLoaded();
            PlaySFX(_sfxWin);
            TriggerHaptic(HapticIntensity.Medium);
        }

        public void PlayLose()
        {
            EnsureCombatClipsLoaded();
            PlaySFX(_sfxLose);
            TriggerHaptic(HapticIntensity.Heavy);
        }

        public void PlayReward()
        {
            EnsureCombatClipsLoaded();
            PlaySFX(_sfxReward);
            TriggerHaptic(HapticIntensity.Light);
        }

        public void PlaySlash()
        {
            EnsureCombatClipsLoaded();
            PlaySFX(_sfxSlash);
            TriggerHaptic(HapticIntensity.Light);
        }

        public void PlayWhoosh()
        {
            EnsureCombatClipsLoaded();
            PlaySFX(_sfxWhoosh);
            TriggerHaptic(HapticIntensity.Light);
        }

        public void PlayTelegraphWarning()
        {
            EnsureCombatClipsLoaded();
            PlaySFX(_sfxTelegraphWarning);
            TriggerHaptic(HapticIntensity.Light);
        }

        public void PlayParryClang(int comboStreak = 0)
        {
            EnsureCombatClipsLoaded();
            if (_sfxSource != null && _sfxParryClang != null)
            {
                // Tăng dần cao độ (pitch scaling) theo combo streak
                float originalPitch = _sfxSource.pitch;
                float bonusPitch = Mathf.Min(comboStreak * 0.04f, 0.5f);
                _sfxSource.pitch = 1.0f + bonusPitch;
                _sfxSource.PlayOneShot(_sfxParryClang);
                _sfxSource.pitch = originalPitch;
            }
            TriggerHaptic(HapticIntensity.Heavy);
        }

        public void PlayHitImpact()
        {
            EnsureCombatClipsLoaded();
            PlaySFX(_sfxHitImpact);
            TriggerHaptic(HapticIntensity.Medium);
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        private static AndroidJavaObject _androidVibrator;
        private static AndroidJavaClass _vibrationEffectClass;
        private static bool _vibratorInitialized = false;

        private static void InitAndroidVibrator()
        {
            if (_vibratorInitialized) return;
            _vibratorInitialized = true;
            try
            {
                using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    _androidVibrator = activity.Call<AndroidJavaObject>("getSystemService", "vibrator");
                }

                using (var version = new AndroidJavaClass("android.os.Build$VERSION"))
                {
                    int sdk = version.GetStatic<int>("SDK_INT");
                    if (sdk >= 26)
                    {
                        _vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect");
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("[AudioManager] Native vibrator init exception: " + ex.Message);
            }
        }
#endif

        public void TriggerHaptic(HapticIntensity intensity = HapticIntensity.Medium)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                InitAndroidVibrator();
                if (_androidVibrator != null)
                {
                    int ms = 35;
                    int amp = 150;
                    switch (intensity)
                    {
                        case HapticIntensity.Light:
                            ms = 20;
                            amp = 80;
                            break;
                        case HapticIntensity.Medium:
                            ms = 40;
                            amp = 180;
                            break;
                        case HapticIntensity.Heavy:
                            ms = 70;
                            amp = 255;
                            break;
                    }

                    if (_vibrationEffectClass != null)
                    {
                        using (var effect = _vibrationEffectClass.CallStatic<AndroidJavaObject>("createOneShot", (long)ms, amp))
                        {
                            _androidVibrator.Call("vibrate", effect);
                        }
                    }
                    else
                    {
                        _androidVibrator.Call("vibrate", (long)ms);
                    }
                    return;
                }
            }
            catch { }
#endif

#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            try
            {
                Handheld.Vibrate();
            }
            catch { }
#endif
        }

        private void EnsureCombatClipsLoaded()
        {
            // 1. Tự động tải từ Resources (Độc lập runtime Android Standalone APK)
            if (_sfxSlash == null) _sfxSlash = Resources.Load<AudioClip>("Audio/Clips/sfx_slash");
            if (_sfxWhoosh == null) _sfxWhoosh = Resources.Load<AudioClip>("Audio/Clips/sfx_whoosh");
            if (_sfxParryClang == null) _sfxParryClang = Resources.Load<AudioClip>("Audio/Clips/sfx_parry_clang");
            if (_sfxHitImpact == null) _sfxHitImpact = Resources.Load<AudioClip>("Audio/Clips/sfx_hit_impact");
            if (_sfxTelegraphWarning == null) _sfxTelegraphWarning = Resources.Load<AudioClip>("Audio/Clips/sfx_telegraph_warning");
            if (_sfxClick == null) _sfxClick = Resources.Load<AudioClip>("Audio/Clips/sfx_click");
            if (_sfxReward == null) _sfxReward = Resources.Load<AudioClip>("Audio/Clips/sfx_reward");
            if (_bgmGameplay == null) _bgmGameplay = Resources.Load<AudioClip>("Audio/Clips/bgm_combat_zen");

#if UNITY_EDITOR
            // 2. Fallback AssetDatabase trong Editor
            if (_sfxSlash == null) _sfxSlash = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/_Project/Audio/Clips/sfx_slash.wav");
            if (_sfxWhoosh == null) _sfxWhoosh = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/_Project/Audio/Clips/sfx_whoosh.wav");
            if (_sfxParryClang == null) _sfxParryClang = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/_Project/Audio/Clips/sfx_parry_clang.wav");
            if (_sfxHitImpact == null) _sfxHitImpact = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/_Project/Audio/Clips/sfx_hit_impact.wav");
            if (_sfxTelegraphWarning == null) _sfxTelegraphWarning = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/_Project/Audio/Clips/sfx_telegraph_warning.wav");
            if (_sfxClick == null) _sfxClick = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/_Project/Audio/Clips/sfx_click.wav");
            if (_sfxReward == null) _sfxReward = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/_Project/Audio/Clips/sfx_reward.wav");
            if (_bgmGameplay == null) _bgmGameplay = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/_Project/Audio/Clips/bgm_combat_zen.wav");
#endif
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
