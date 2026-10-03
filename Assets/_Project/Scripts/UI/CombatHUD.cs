using UnityEngine;
using UnityEngine.UI;
using Game.Gameplay;

namespace Game.UI
{
    /// <summary>
    /// Giao diện chiến đấu thời gian thực chuẩn Zero-GC Delta Caching.
    /// Triệt tiêu hoàn toàn rác bộ nhớ chuỗi trong vòng lặp Update.
    /// </summary>
    public class CombatHUD : MonoBehaviour
    {
        [Header("Managers")]
        [SerializeField] private TimingCombatManager _combatManager;
        [SerializeField] private HeroCombatController _hero;

        [Header("UI Elements")]
        [SerializeField] private Text _healthText;
        [SerializeField] private Slider _healthSlider;
        [SerializeField] private Text _comboText;
        [SerializeField] private Text _scoreText;
        [SerializeField] private Text _waveText;
        [SerializeField] private Button _attackButton;
        [SerializeField] private Button _dodgeButton;

        // Zero-GC Cache
        private int _cachedHealth = -1;
        private int _cachedMaxHealth = -1;
        private int _cachedCombo = -1;
        private int _cachedScore = -1;
        private int _cachedWave = -1;

        private void Start()
        {
            if (_combatManager == null)
            {
                _combatManager = TimingCombatManager.Instance;
            }

            if (_hero == null && _combatManager != null)
            {
                _hero = _combatManager.Hero;
            }

            // Gán sự kiện click nút cảm ứng
            if (_attackButton != null)
            {
                _attackButton.onClick.AddListener(HandleAttackClicked);
            }

            if (_dodgeButton != null)
            {
                _dodgeButton.onClick.AddListener(HandleDodgeClicked);
            }
        }

        private void OnDestroy()
        {
            if (_attackButton != null) _attackButton.onClick.RemoveListener(HandleAttackClicked);
            if (_dodgeButton != null) _dodgeButton.onClick.RemoveListener(HandleDodgeClicked);
        }

        private void Update()
        {
            UpdateHealthHUD();
            UpdateComboHUD();
            UpdateScoreHUD();
            UpdateWaveHUD();
        }

        private void UpdateHealthHUD()
        {
            if (_hero == null) return;

            int currentHp = _hero.CurrentHealth;
            int maxHp = _hero.MaxHealth;

            if (currentHp != _cachedHealth || maxHp != _cachedMaxHealth)
            {
                _cachedHealth = currentHp;
                _cachedMaxHealth = maxHp;

                if (_healthSlider != null && maxHp > 0)
                {
                    _healthSlider.value = (float)currentHp / maxHp;
                }

                if (_healthText != null)
                {
                    _healthText.text = $"HP: {currentHp}/{maxHp}";
                }
            }
        }

        private void UpdateComboHUD()
        {
            if (_hero == null) return;

            int combo = _hero.ComboCount;
            if (combo != _cachedCombo)
            {
                _cachedCombo = combo;
                if (_comboText != null)
                {
                    if (combo > 1)
                    {
                        _comboText.gameObject.SetActive(true);
                        _comboText.text = $"COMBO x{combo}!";
                    }
                    else
                    {
                        _comboText.gameObject.SetActive(false);
                    }
                }
            }
        }

        private void UpdateScoreHUD()
        {
            if (_combatManager == null) return;

            int score = _combatManager.Score;
            if (score != _cachedScore)
            {
                _cachedScore = score;
                if (_scoreText != null)
                {
                    _scoreText.text = $"SCORE: {score}";
                }
            }
        }

        private void UpdateWaveHUD()
        {
            if (_combatManager == null) return;

            int wave = _combatManager.CurrentWave;
            if (wave != _cachedWave)
            {
                _cachedWave = wave;
                if (_waveText != null)
                {
                    _waveText.text = $"WAVE {wave}";
                }
            }
        }

        private void HandleAttackClicked()
        {
            if (_combatManager != null)
            {
                _combatManager.OnTapAttack();
            }
        }

        private void HandleDodgeClicked()
        {
            if (_combatManager != null)
            {
                _combatManager.OnTapDodge();
            }
        }
    }
}
