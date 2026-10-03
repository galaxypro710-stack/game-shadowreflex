using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Game.Gameplay;

namespace Game.UI
{
    /// <summary>
    /// Giao diện chiến đấu thời gian thực chuẩn Zero-GC Delta Caching.
    /// Triệt tiêu hoàn toàn rác bộ nhớ chuỗi trong vòng lặp Update.
    /// Hỗ trợ cả EventTrigger bắt chạm toàn màn hình (Left=Dodge, Right=Attack) và nút cổ điển.
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
        [SerializeField] private Button _shopButton;
        [SerializeField] private ShopPopup _shopPopup;

        [Header("Full-Screen Anti-Overflow Touch Zones")]
        [SerializeField] private EventTrigger _dodgeTouchZone;
        [SerializeField] private EventTrigger _attackTouchZone;

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

            // Gán sự kiện EventTrigger cho 2 Touch Zones trong suốt full-screen
            BindTouchZone(_dodgeTouchZone, HandleDodgeClicked);
            BindTouchZone(_attackTouchZone, HandleAttackClicked);

            // Gán sự kiện click nút cảm ứng (nếu có)
            if (_attackButton != null)
            {
                _attackButton.onClick.AddListener(HandleAttackClicked);
            }

            if (_dodgeButton != null)
            {
                _dodgeButton.onClick.AddListener(HandleDodgeClicked);
            }

            if (_shopButton != null)
            {
                _shopButton.onClick.AddListener(HandleShopClicked);
            }
        }

        private void BindTouchZone(EventTrigger trigger, System.Action action)
        {
            if (trigger == null || action == null) return;
            var entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
            entry.callback.AddListener((_) => action());
            trigger.triggers.Add(entry);
        }

        private void OnDestroy()
        {
            if (_attackButton != null) _attackButton.onClick.RemoveListener(HandleAttackClicked);
            if (_dodgeButton != null) _dodgeButton.onClick.RemoveListener(HandleDodgeClicked);
            if (_shopButton != null) _shopButton.onClick.RemoveListener(HandleShopClicked);
        }

        private void HandleShopClicked()
        {
            if (_shopPopup != null)
            {
                _shopPopup.Open();
            }
            else
            {
                var popup = Object.FindAnyObjectByType<ShopPopup>(FindObjectsInactive.Include);
                if (popup != null)
                {
                    _shopPopup = popup;
                    popup.Open();
                }
            }
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
