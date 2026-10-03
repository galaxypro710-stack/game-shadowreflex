using System.Collections.Generic;
using UnityEngine;
using Game.Core;
using Game.Events;

namespace Game.Gameplay
{
    /// <summary>
    /// Bộ điều phối chiến đấu 1-Tap Timing Action.
    /// Tích hợp khép kín với Core FSM của Meta-Factory qua ScriptableObject Event Channels.
    /// </summary>
    public class TimingCombatManager : MonoBehaviour
    {
        public static TimingCombatManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private HeroCombatController _hero;
        [SerializeField] private Transform _enemySpawnPoint;
        [SerializeField] private GameObject _enemyPrefab;

        [Header("Outgoing Core Events")]
        [SerializeField] private VoidEventChannelSO _onVictoryTriggeredChannel;
        [SerializeField] private VoidEventChannelSO _onDefeatTriggeredChannel;

        [Header("Combat Settings")]
        [SerializeField] private int _totalEnemiesInWave = 3;
        [SerializeField] private List<EnemyConfig> _enemyConfigs = new List<EnemyConfig>();

        private List<EnemyCombatController> _activeEnemies = new List<EnemyCombatController>();
        private EnemyCombatController _currentEngagedEnemy;
        private int _enemiesDefeated = 0;
        private int _currentScore = 0;
        private int _currentWave = 1;

        public HeroCombatController Hero => _hero;
        public EnemyCombatController CurrentEnemy => _currentEngagedEnemy;
        public int Score => _currentScore;
        public int CurrentWave => _currentWave;
        public int EnemiesRemaining => Mathf.Max(0, _totalEnemiesInWave - _enemiesDefeated);

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // Cấu hình danh sách quái mặc định mở rộng theo chuẩn Version 2
            if (_enemyConfigs.Count == 0)
            {
                _enemyConfigs.Add(new EnemyConfig 
                { 
                    enemyName = "Ninja Scout", 
                    maxHp = 90, 
                    attackDamage = 12, 
                    approachSpeed = 3.0f, 
                    windupDuration = 0.65f, 
                    strikeWindow = 0.22f, 
                    scoreReward = 50,
                    isBoss = false,
                    comboStrikesCount = 1,
                    spriteName = "Enemy_Scout"
                });

                _enemyConfigs.Add(new EnemyConfig 
                { 
                    enemyName = "Swift Assassin", 
                    maxHp = 75, 
                    attackDamage = 18, 
                    approachSpeed = 4.2f, 
                    windupDuration = 0.40f, // Quái nhanh chém 0.4s
                    strikeWindow = 0.16f, 
                    scoreReward = 100,
                    isBoss = false,
                    comboStrikesCount = 1,
                    spriteName = "Enemy_Swift"
                });

                _enemyConfigs.Add(new EnemyConfig 
                { 
                    enemyName = "Iron Brute", 
                    maxHp = 220, 
                    attackDamage = 35, 
                    approachSpeed = 1.8f, 
                    windupDuration = 0.80f, // Quái búa nặng chém 0.8s
                    strikeWindow = 0.35f, 
                    scoreReward = 150,
                    isBoss = false,
                    comboStrikesCount = 1,
                    spriteName = "Enemy_Brute"
                });

                _enemyConfigs.Add(new EnemyConfig 
                { 
                    enemyName = "Shadow Shogun (Boss)", 
                    maxHp = 380, 
                    attackDamage = 45, 
                    approachSpeed = 2.4f, 
                    windupDuration = 0.50f, 
                    strikeWindow = 0.20f, 
                    scoreReward = 300,
                    isBoss = true,
                    comboStrikesCount = 3, // Boss 3 nhịp chém liên hoàn
                    spriteName = "Enemy_Boss"
                });

                _totalEnemiesInWave = _enemyConfigs.Count;
            }
        }

        private void Start()
        {
            if (_hero != null)
            {
                _hero.OnDefeated += HandleHeroDefeated;
            }

            StartNewWave(1);
        }

        private void OnDestroy()
        {
            if (_hero != null)
            {
                _hero.OnDefeated -= HandleHeroDefeated;
            }
        }

        public void StartNewWave(int waveIndex)
        {
            _currentWave = waveIndex;
            _enemiesDefeated = 0;

            if (_hero != null)
            {
                _hero.ResetHero();
            }

            SpawnNextEnemy();
        }

        private void SpawnNextEnemy()
        {
            if (_enemiesDefeated >= _totalEnemiesInWave)
            {
                HandleWaveCleared();
                return;
            }

            Vector3 spawnPos = _enemySpawnPoint != null ? _enemySpawnPoint.position : new Vector3(5f, 0f, 0f);
            
            GameObject enemyObj = null;
            if (_enemyPrefab != null)
            {
                enemyObj = Instantiate(_enemyPrefab, spawnPos, Quaternion.identity);
            }
            else
            {
                // Fallback tạo nhanh GameObject quái nếu chưa gán prefab
                enemyObj = new GameObject($"Enemy_{_enemiesDefeated + 1}");
                enemyObj.transform.position = spawnPos;
                var sr = enemyObj.AddComponent<SpriteRenderer>();
                sr.color = Color.gray;
                enemyObj.AddComponent<EnemyCombatController>();
            }

            var enemyCtrl = enemyObj.GetComponent<EnemyCombatController>();
            if (enemyCtrl != null)
            {
                int cfgIdx = Mathf.Min(_enemiesDefeated, _enemyConfigs.Count - 1);
                Transform heroTr = _hero != null ? _hero.transform : transform;
                enemyCtrl.Initialize(_enemyConfigs[cfgIdx], heroTr, _hero);
                enemyCtrl.OnDefeated += HandleEnemyDefeated;

                _currentEngagedEnemy = enemyCtrl;
            }
        }

        private void HandleEnemyDefeated(EnemyCombatController defeatedEnemy)
        {
            if (defeatedEnemy != null)
            {
                defeatedEnemy.OnDefeated -= HandleEnemyDefeated;
                _currentScore += defeatedEnemy.ScoreReward;

                // Thưởng tiền vàng cho người chơi tích lũy nâng cấp
                int coinReward = Mathf.Max(10, defeatedEnemy.ScoreReward / 2);
                Game.Data.SaveService.Instance?.AddCoins(coinReward);
            }

            _enemiesDefeated++;
            _currentEngagedEnemy = null;

            if (_enemiesDefeated >= _totalEnemiesInWave)
            {
                HandleWaveCleared();
            }
            else
            {
                // Gọi quái tiếp theo sau 0.5s
                Invoke(nameof(SpawnNextEnemy), 0.5f);
            }
        }

        private void HandleWaveCleared()
        {
            Debug.Log($"[TimingCombatManager] Wave {_currentWave} hoàn tất vẻ vang! Điểm: {_currentScore}");
            Game.Data.SaveService.Instance?.AddCoins(100);
            if (_onVictoryTriggeredChannel != null)
            {
                _onVictoryTriggeredChannel.RaiseEvent();
            }
        }

        private void HandleHeroDefeated()
        {
            Debug.Log("[TimingCombatManager] Người chơi bị hạ gục! Kích hoạt Thất bại.");
            if (_onDefeatTriggeredChannel != null)
            {
                _onDefeatTriggeredChannel.RaiseEvent();
            }
        }

        // ================= INPUT BINDINGS (1-TAP ACTION) =================

        public void OnTapAttack()
        {
            if (_hero != null)
            {
                _hero.PerformAttack(_currentEngagedEnemy);
            }
        }

        public void OnTapDodge()
        {
            if (_hero != null)
            {
                _hero.PerformDodge();
            }
        }
    }
}
