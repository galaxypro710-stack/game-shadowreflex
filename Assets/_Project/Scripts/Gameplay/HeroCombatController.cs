using System;
using UnityEngine;

namespace Game.Gameplay
{
    /// <summary>
    /// Điều khiển chiến đấu nhân vật chính theo cơ chế 1-Tap Reflex.
    /// Thiết kế Zero-GC, không phụ thuộc vào thư viện ngoài, tối ưu hóa 60 FPS.
    /// </summary>
    public class HeroCombatController : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private int _maxHealth = 100;
        [SerializeField] private int _baseAttackDamage = 35;
        [SerializeField] private float _dodgeDuration = 0.35f;
        [SerializeField] private float _comboDecayTime = 2.0f;

        [Header("Visual Feedback")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private Color _dodgeColor = new Color(0.3f, 0.8f, 1f, 0.7f);
        [SerializeField] private Color _hitColor = new Color(1f, 0.2f, 0.2f, 1f);

        private int _currentHealth;
        private HeroCombatState _state = HeroCombatState.Idle;
        private int _comboCount = 0;
        private float _comboTimer = 0f;
        private float _dodgeTimer = 0f;
        private float _hitFeedbackTimer = 0f;

        private int _effectiveAttackDamage;
        private float _effectiveDodgeDuration;

        public int CurrentHealth => _currentHealth;
        public int MaxHealth => _maxHealth;
        public int ComboCount => _comboCount;
        public HeroCombatState State => _state;
        public bool IsInvulnerable => _state == HeroCombatState.Dodging;

        public event Action<int> OnComboChanged;
        public event Action<int, int> OnHealthChanged;
        public event Action OnDefeated;
        public event Action<bool> OnAttackExecuted; // true = Perfect Counter, false = Normal Attack
        public event Action OnDodgeExecuted;

        private void Awake()
        {
            if (_spriteRenderer == null)
            {
                _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }

            LoadHeroSprite();
            ApplyUpgrades();
        }

        private void LoadHeroSprite()
        {
#if UNITY_EDITOR
            if (_spriteRenderer != null && _spriteRenderer.sprite == null)
            {
                var sp = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/Sprites/Hero_Ninja.png");
                if (sp != null) _spriteRenderer.sprite = sp;
            }
#endif
        }

        public void ApplyUpgrades()
        {
            int bonusHp = HeroUpgradeManager.Instance != null ? HeroUpgradeManager.Instance.GetBonusHp() : 0;
            int bonusDmg = HeroUpgradeManager.Instance != null ? HeroUpgradeManager.Instance.GetBonusDamage() : 0;
            float bonusDodge = HeroUpgradeManager.Instance != null ? HeroUpgradeManager.Instance.GetBonusDodgeDuration() : 0f;

            _maxHealth = 100 + bonusHp;
            _effectiveAttackDamage = _baseAttackDamage + bonusDmg;
            _effectiveDodgeDuration = _dodgeDuration + bonusDodge;
            _currentHealth = _maxHealth;
        }

        private void Update()
        {
            if (_state == HeroCombatState.Dead) return;

            // Xử lý đếm ngược Dodge (I-Frames)
            if (_state == HeroCombatState.Dodging)
            {
                _dodgeTimer -= Time.deltaTime;
                if (_dodgeTimer <= 0f)
                {
                    _state = HeroCombatState.Idle;
                    if (_spriteRenderer != null) _spriteRenderer.color = _normalColor;
                }
            }

            // Xử lý đếm ngược chuỗi Combo
            if (_comboCount > 0)
            {
                _comboTimer -= Time.deltaTime;
                if (_comboTimer <= 0f)
                {
                    _comboCount = 0;
                    OnComboChanged?.Invoke(0);
                }
            }

            // Xử lý nhấp nháy đỏ khi bị thương
            if (_hitFeedbackTimer > 0f)
            {
                _hitFeedbackTimer -= Time.deltaTime;
                if (_hitFeedbackTimer <= 0f && _state != HeroCombatState.Dodging)
                {
                    if (_spriteRenderer != null) _spriteRenderer.color = _normalColor;
                }
            }
        }

        public void PerformAttack(EnemyCombatController targetEnemy)
        {
            if (_state == HeroCombatState.Dead || _state == HeroCombatState.Stunned) return;

            bool isPerfectCounter = false;
            float multiplier = 1.0f;

            if (targetEnemy != null && targetEnemy.IsActive)
            {
                // Nếu kẻ địch đang giương đòn (Windup) hoặc đang trong cửa sổ ra đòn (ActiveStrike) -> Perfect Counter!
                if (targetEnemy.State == EnemyCombatState.WindupTelegraph || targetEnemy.State == EnemyCombatState.ActiveStrike)
                {
                    isPerfectCounter = true;
                    multiplier = 2.5f;

                    // Perfect Counter VFX, Audio & Camera Shake
                    CombatVFXController.Instance?.PlayCounterBurst(targetEnemy.transform.position);
                    CameraShakeController.Instance?.ShakeCritical();
                    Game.Audio.AudioManager.Instance?.PlayParryClang(_comboCount);
                }
                else
                {
                    Game.Audio.AudioManager.Instance?.PlaySlash();
                }

                // Luôn tạo vệt chém kiếm
                CombatVFXController.Instance?.PlaySlash(targetEnemy.transform.position, isPerfectCounter);

                // Gia tăng Combo
                _comboCount++;
                _comboTimer = _comboDecayTime;
                OnComboChanged?.Invoke(_comboCount);

                // Tính toán sát thương lũy tiến theo nhịp điệu
                float comboBonus = Mathf.Min(_comboCount * 0.05f, 1.5f);
                int finalDamage = Mathf.RoundToInt(_effectiveAttackDamage * (1.0f + comboBonus) * multiplier);

                targetEnemy.TakeDamage(finalDamage, isPerfectCounter);
            }
            else
            {
                Game.Audio.AudioManager.Instance?.PlaySlash();
            }

            OnAttackExecuted?.Invoke(isPerfectCounter);
        }

        public void PerformDodge()
        {
            if (_state == HeroCombatState.Dead || _state == HeroCombatState.Dodging) return;

            _state = HeroCombatState.Dodging;
            _dodgeTimer = _effectiveDodgeDuration;

            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = _dodgeColor;
            }

            // SFX & Dash Ghost Trail
            Game.Audio.AudioManager.Instance?.PlayWhoosh();
            CombatVFXController.Instance?.PlayDashTrail(transform.position);

            OnDodgeExecuted?.Invoke();
        }

        public void TakeDamage(int damage)
        {
            if (IsInvulnerable || _state == HeroCombatState.Dead) return;

            _currentHealth = Mathf.Max(0, _currentHealth - damage);
            _comboCount = 0; // Bị trúng đòn làm đứt chuỗi combo
            OnComboChanged?.Invoke(0);
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);

            _hitFeedbackTimer = 0.2f;
            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = _hitColor;
            }

            // Hit Impact SFX & Light Shake
            Game.Audio.AudioManager.Instance?.PlayHitImpact();
            CameraShakeController.Instance?.ShakeLight();

            if (_currentHealth <= 0)
            {
                _state = HeroCombatState.Dead;
                OnDefeated?.Invoke();
            }
        }

        public void ResetHero()
        {
            ApplyUpgrades();
            _comboCount = 0;
            _comboTimer = 0f;
            _state = HeroCombatState.Idle;
            if (_spriteRenderer != null) _spriteRenderer.color = _normalColor;
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
            OnComboChanged?.Invoke(0);
        }
    }
}
