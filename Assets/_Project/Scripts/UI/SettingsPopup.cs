using System;
using UnityEngine;
using UnityEngine.UI;
using Game.Core;
using Game.Data;

namespace Game.UI
{
    public enum GameLanguage
    {
        Vietnamese,
        English
    }

    public enum GameTheme
    {
        Pyramid,
        Ocean
    }

    /// <summary>
    /// Popup Cài Đặt (Settings Popup) chuẩn nguyên bản.
    /// Cho phép người chơi:
    /// 1. Bật/Tắt Nhạc nền (BGM)
    /// 2. Bật/Tắt Hiệu ứng âm thanh (SFX)
    /// 3. Bật/Tắt Rung (Haptic/Vibration)
    /// 4. Chuyển đổi Ngôn ngữ (Tiếng Việt 🇻🇳 / English 🇬🇧)
    /// 5. Chuyển đổi Chủ đề (Kim Tự Tháp 🏛️ / Đại Dương 🌊)
    /// 6. Chơi lại ván (Restart) hoặc Về trang chủ (Home)
    /// </summary>
    public class SettingsPopup : MonoBehaviour
    {
        [Header("Toggles / Switches")]
        [SerializeField] private Button _bgmToggleButton;
        [SerializeField] private Text _bgmStatusText;
        [SerializeField] private Button _sfxToggleButton;
        [SerializeField] private Text _sfxStatusText;
        [SerializeField] private Button _vibrationToggleButton;
        [SerializeField] private Text _vibrationStatusText;

        [Header("Language Selector")]
        [SerializeField] private Button _languageButton;
        [SerializeField] private Text _languageText;

        [Header("Theme Selector")]
        [SerializeField] private Button _themeButton;
        [SerializeField] private Text _themeText;

        [Header("Action Buttons")]
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _homeButton;

        [Header("UI Text Localized")]
        [SerializeField] private Text _titleText;

        public const string PrefBgm = "TilePyramid_BgmEnabled";
        public const string PrefSfx = "TilePyramid_SfxEnabled";
        public const string PrefVib = "TilePyramid_VibrationEnabled";
        public const string PrefLang = "TilePyramid_Language";
        public const string PrefTheme = "TilePyramid_Theme";

        private bool _isBgmOn = true;
        private bool _isSfxOn = true;
        private bool _isVibrationOn = true;
        private GameLanguage _currentLanguage = GameLanguage.Vietnamese;
        private GameTheme _currentTheme = GameTheme.Pyramid;

        public static event Action<GameLanguage> OnLanguageChanged;
        public static event Action<GameTheme> OnThemeChanged;

        private void Awake()
        {
            LoadSettings();
        }

        private void OnEnable()
        {
            if (_bgmToggleButton != null) _bgmToggleButton.onClick.AddListener(ToggleBGM);
            if (_sfxToggleButton != null) _sfxToggleButton.onClick.AddListener(ToggleSFX);
            if (_vibrationToggleButton != null) _vibrationToggleButton.onClick.AddListener(ToggleVibration);
            if (_languageButton != null) _languageButton.onClick.AddListener(SwitchLanguage);
            if (_themeButton != null) _themeButton.onClick.AddListener(SwitchTheme);
            if (_closeButton != null) _closeButton.onClick.AddListener(Close);
            if (_restartButton != null) _restartButton.onClick.AddListener(RestartGame);
            if (_homeButton != null) _homeButton.onClick.AddListener(GoHome);

            UpdateUI();
        }

        private void OnDisable()
        {
            if (_bgmToggleButton != null) _bgmToggleButton.onClick.RemoveListener(ToggleBGM);
            if (_sfxToggleButton != null) _sfxToggleButton.onClick.RemoveListener(ToggleSFX);
            if (_vibrationToggleButton != null) _vibrationToggleButton.onClick.RemoveListener(ToggleVibration);
            if (_languageButton != null) _languageButton.onClick.RemoveListener(SwitchLanguage);
            if (_themeButton != null) _themeButton.onClick.RemoveListener(SwitchTheme);
            if (_closeButton != null) _closeButton.onClick.RemoveListener(Close);
            if (_restartButton != null) _restartButton.onClick.RemoveListener(RestartGame);
            if (_homeButton != null) _homeButton.onClick.RemoveListener(GoHome);
        }

        private void LoadSettings()
        {
            _isBgmOn = PlayerPrefs.GetInt(PrefBgm, 1) == 1;
            _isSfxOn = PlayerPrefs.GetInt(PrefSfx, 1) == 1;
            _isVibrationOn = PlayerPrefs.GetInt(PrefVib, 1) == 1;
            _currentLanguage = (GameLanguage)PlayerPrefs.GetInt(PrefLang, (int)GameLanguage.Vietnamese);
            _currentTheme = (GameTheme)PlayerPrefs.GetInt(PrefTheme, (int)GameTheme.Pyramid);
        }

        public void Open()
        {
            gameObject.SetActive(true);
            UpdateUI();
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        private void ToggleBGM()
        {
            _isBgmOn = !_isBgmOn;
            PlayerPrefs.SetInt(PrefBgm, _isBgmOn ? 1 : 0);
            PlayerPrefs.Save();

            var audioMgr = FindAnyObjectByType<Game.Audio.AudioManager>();
            if (audioMgr != null)
            {
                var bgmSource = audioMgr.GetComponentInChildren<AudioSource>();
                if (bgmSource != null) bgmSource.mute = !_isBgmOn;
            }

            UpdateUI();
            Debug.Log($"[SettingsPopup] Nhạc nền (BGM): {(_isBgmOn ? "BẬT" : "TẮT")}");
        }

        private void ToggleSFX()
        {
            _isSfxOn = !_isSfxOn;
            PlayerPrefs.SetInt(PrefSfx, _isSfxOn ? 1 : 0);
            PlayerPrefs.Save();
            UpdateUI();
            Debug.Log($"[SettingsPopup] Hiệu ứng âm thanh (SFX): {(_isSfxOn ? "BẬT" : "TẮT")}");
        }

        private void ToggleVibration()
        {
            _isVibrationOn = !_isVibrationOn;
            PlayerPrefs.SetInt(PrefVib, _isVibrationOn ? 1 : 0);
            PlayerPrefs.Save();
            UpdateUI();
            Debug.Log($"[SettingsPopup] Rung phản hồi: {(_isVibrationOn ? "BẬT" : "TẮT")}");
        }

        private void SwitchLanguage()
        {
            _currentLanguage = (_currentLanguage == GameLanguage.Vietnamese) ? GameLanguage.English : GameLanguage.Vietnamese;
            PlayerPrefs.SetInt(PrefLang, (int)_currentLanguage);
            PlayerPrefs.Save();

            OnLanguageChanged?.Invoke(_currentLanguage);
            UpdateUI();
            Debug.Log($"[SettingsPopup] Ngôn ngữ chuyển sang: {_currentLanguage}");
        }

        private void SwitchTheme()
        {
            _currentTheme = (_currentTheme == GameTheme.Pyramid) ? GameTheme.Ocean : GameTheme.Pyramid;
            PlayerPrefs.SetInt(PrefTheme, (int)_currentTheme);
            PlayerPrefs.Save();

            OnThemeChanged?.Invoke(_currentTheme);
            UpdateUI();
            Debug.Log($"[SettingsPopup] Chủ đề chuyển sang: {_currentTheme}");
        }

        private void RestartGame()
        {
            Close();
            Debug.Log("[SettingsPopup] Người chơi yêu cầu chơi lại ván...");
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }

        private void GoHome()
        {
            Close();
            Debug.Log("[SettingsPopup] Người chơi quay về Trang chủ...");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ChangeState(GameState.MainMenu);
            }
        }

        private void UpdateUI()
        {
            bool isVi = _currentLanguage == GameLanguage.Vietnamese;

            if (_titleText != null)
            {
                _titleText.text = isVi ? "CÀI ĐẶT" : "SETTINGS";
            }

            if (_bgmStatusText != null)
            {
                _bgmStatusText.text = isVi ? (_isBgmOn ? "Nhạc nền: BẬT" : "Nhạc nền: TẮT")
                                           : (_isBgmOn ? "Music: ON" : "Music: OFF");
            }

            if (_sfxStatusText != null)
            {
                _sfxStatusText.text = isVi ? (_isSfxOn ? "Hiệu ứng: BẬT" : "Hiệu ứng: TẮT")
                                           : (_isSfxOn ? "Sound: ON" : "Sound: OFF");
            }

            if (_vibrationStatusText != null)
            {
                _vibrationStatusText.text = isVi ? (_isVibrationOn ? "Rung: BẬT" : "Rung: TẮT")
                                                 : (_isVibrationOn ? "Haptic: ON" : "Haptic: OFF");
            }

            if (_languageText != null)
            {
                _languageText.text = isVi ? "Ngôn ngữ: Tiếng Việt 🇻🇳" : "Language: English 🇬🇧";
            }

            if (_themeText != null)
            {
                bool isOcean = _currentTheme == GameTheme.Ocean;
                _themeText.text = isVi ? (isOcean ? "Chủ đề: Đại Dương 🌊" : "Chủ đề: Kim Tự Tháp 🏛️")
                                       : (isOcean ? "Theme: Ocean 🌊" : "Theme: Pyramid 🏛️");
            }
        }
    }
}
