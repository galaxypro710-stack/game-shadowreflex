using System;
using UnityEngine;

namespace Game.Gameplay
{
    /// <summary>
    /// Điều khiển hành vi đối thủ: Tiến lại gần -> Giương đòn báo hiệu (Telegraph) -> Ra đòn (Active Strike).
    /// </summary>
    public class EnemyCombatController : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private EnemyConfig _config = new EnemyConfig();
        [SerializeField] private float _attackDistance = 1.8f;

        [Header("Visual Feedback")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Color _normalColor = new Color(0.9f, 0.9f, 0.9f, 1f);
        [SerializeField] private Color _telegraphColor = new Color(1f, 0.6f, 0.1f, 1f); // Cam báo hiệu
        [SerializeField] private Color _strikeColor = new Color(1f, 0.1f, 0.1f, 1f);    // Đỏ chớp nhoáng
        [SerializeField] private Color _hitColor = new Color(1f, 1f, 0.3f, 1f);       // Vàng khi bị đánh

        private int _currentHp;
        private EnemyCombatState _state = EnemyCombatState.Idle;
        private float _stateTimer = 0f;
        private Transform _targetHero;
        private HeroCombatController _heroController;

        public bool IsActive => _state != EnemyCombatState.Dead;
        public EnemyCombatState State => _state;
        public int CurrentHp => _currentHp;
        public int MaxHp => _config.maxHp;
        public int ScoreReward => _config.scoreReward;

        public event Action<EnemyCombatController> OnDefeated;
        public event Action<int, int> OnHpChanged;
        public event Action<EnemyCombatState> OnStateChanged;

        private void Awake()
        {
            if (_spriteRenderer == null)
            {
                _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }
        }

        public void Initialize(EnemyConfig config, Transform targetHero, HeroCombatController heroController)
        {
            _config = config ?? new EnemyConfig();
            _currentHp = _config.maxHp;
            _targetHero = targetHero;
            _heroController = heroController;
            _state = EnemyCombatState.Approaching;

            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = _normalColor;
            }

            OnHpChanged?.Invoke(_currentHp, _config.maxHp);
            OnStateChanged?.Invoke(_state);
        }

        private void Update()
        {
            if (_state == EnemyCombatState.Dead || _targetHero == null) return;

            switch (_state)
            {
                case EnemyCombatState.Approaching:
                    HandleApproaching();
                    break;

                case EnemyCombatState.WindupTelegraph:
                    HandleWindup();
                    break;

                case EnemyCombatState.ActiveStrike:
                    HandleActiveStrike();
                    break;

                case EnemyCombatState.Stunned:
                    HandleStunned();
                    break;
            }
        }

        private void HandleApproaching()
        {
            float dist = Vector3.Distance(transform.position, _targetHero.position);
            if (dist > _attackDistance)
            {
                Vector3 dir = (_targetHero.position - transform.position).normalized;
                transform.position += dir * (_config.approachSpeed * Time.deltaTime);
            }
            else
            {
                // Đã vào tầm đánh -> Bắt đầu giương đòn báo hiệu
                _state = EnemyCombatState.WindupTelegraph;
                _stateTimer = _config.windupDuration;
                if (_spriteRenderer != null) _spriteRenderer.color = _telegraphColor;
                OnStateChanged?.Invoke(_state);
            }
        }

        private void HandleWindup()
        {
            _stateTimer -= Time.deltaTime;
            if (_stateTimer <= 0f)
            {
                // Hết thời gian giương đòn -> Bắt đầu vung đòn (Active Strike)
                _state = EnemyCombatState.ActiveStrike;
                _stateTimer = _config.strikeWindow;
                if (_spriteRenderer != null) _spriteRenderer.color = _strikeColor;
                OnStateChanged?.Invoke(_state);
            }
        }

        private void HandleActiveStrike()
        {
            _stateTimer -= Time.deltaTime;
            if (_stateTimer <= 0f)
            {
                // Kết thúc cửa sổ vung đòn -> Gây sát thương nếu người chơi không né
                if (_heroController != null && !_heroController.IsInvulnerable)
                {
                    _heroController.TakeDamage(_config.attackDamage);
                }

                // Trở về trạng thái Windup cho đòn tiếp theo
                _state = EnemyCombatState.WindupTelegraph;
                _stateTimer = _config.windupDuration;
                if (_spriteRenderer != null) _spriteRenderer.color = _telegraphColor;
                OnStateChanged?.Invoke(_state);
            }
        }

        private void HandleStunned()
        {
            _stateTimer -= Time.deltaTime;
            if (_stateTimer <= 0f)
            {
                _state = EnemyCombatState.WindupTelegraph;
                _stateTimer = _config.windupDuration;
                if (_spriteRenderer != null) _spriteRenderer.color = _telegraphColor;
                OnStateChanged?.Invoke(_state);
            }
        }

        public void TakeDamage(int damage, bool isPerfectCounter)
        {
            if (_state == EnemyCombatState.Dead) return;

            _currentHp = Mathf.Max(0, _currentHp - damage);
            OnHpChanged?.Invoke(_currentHp, _config.maxHp);

            if (_currentHp <= 0)
            {
                Die();
            }
            else if (isPerfectCounter)
            {
                // Bị Counter Hoàn Hảo -> Choáng 0.5s
                _state = EnemyCombatState.Stunned;
                _stateTimer = 0.5f;
                if (_spriteRenderer != null) _spriteRenderer.color = _hitColor;
                OnStateChanged?.Invoke(_state);
            }
            else
            {
                if (_spriteRenderer != null) _spriteRenderer.color = _hitColor;
            }
        }

        private void Die()
        {
            _state = EnemyCombatState.Dead;
            OnStateChanged?.Invoke(_state);
            OnDefeated?.Invoke(this);
            gameObject.SetActive(false);
        }
    }
}
