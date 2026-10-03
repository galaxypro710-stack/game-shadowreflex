using System;
using UnityEngine;
using Game.Audio;
using Game.UI;

namespace Game.Core
{
    public enum GameTheme
    {
        Default = 0,
        ThemeA = 1,
        ThemeB = 2
    }

    /// <summary>
    /// Điều phối chuyển đổi chủ đề động trong Runtime (Multi-Theme Engine).
    /// Chuẩn Universal Core: Quản lý Background, BGM chủ đề và lưu cài đặt người dùng.
    /// Hoàn toàn tách biệt khỏi logic gameplay hay loại quân cờ cụ thể.
    /// </summary>
    public class ThemeManager : MonoBehaviour
    {
        public static ThemeManager Instance { get; private set; }

        [Header("Scene References")]
        [SerializeField] private SpriteRenderer _sceneBackground;
        [SerializeField] private AudioManager _audioManager;

        [Header("Default Theme Assets")]
        [SerializeField] private Sprite _defaultBackground;
        [SerializeField] private AudioClip _defaultBgm;

        [Header("Theme A Assets")]
        [SerializeField] private Sprite _themeABackground;
        [SerializeField] private AudioClip _themeABgm;

        private GameTheme _currentTheme = GameTheme.Default;
        public GameTheme CurrentTheme => _currentTheme;

        public event Action<GameTheme> OnThemeChanged;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            int savedTheme = PlayerPrefs.GetInt(SettingsPopup.PrefTheme, (int)GameTheme.Default);
            _currentTheme = (GameTheme)savedTheme;
        }

        private void Start()
        {
            ApplyTheme(_currentTheme);
        }

        public void SetTheme(GameTheme theme)
        {
            if (_currentTheme == theme) return;

            _currentTheme = theme;
            PlayerPrefs.SetInt(SettingsPopup.PrefTheme, (int)_currentTheme);
            PlayerPrefs.Save();

            ApplyTheme(_currentTheme);
            OnThemeChanged?.Invoke(_currentTheme);
        }

        public void ToggleTheme()
        {
            GameTheme next = _currentTheme == GameTheme.Default ? GameTheme.ThemeA : GameTheme.Default;
            SetTheme(next);
        }

        private void ApplyTheme(GameTheme theme)
        {
            Sprite targetBg = theme == GameTheme.ThemeA ? _themeABackground : _defaultBackground;
            AudioClip targetBgm = theme == GameTheme.ThemeA ? _themeABgm : _defaultBgm;

            if (_sceneBackground != null && targetBg != null)
            {
                _sceneBackground.sprite = targetBg;
            }

            if (_audioManager != null && targetBgm != null)
            {
                _audioManager.PlayBGM(targetBgm);
            }
        }
    }
}
