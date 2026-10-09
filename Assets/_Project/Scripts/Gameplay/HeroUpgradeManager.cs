using System;
using UnityEngine;
using Game.Data;

namespace Game.Gameplay
{
    [Serializable]
    public class HeroUpgradeData
    {
        public int hpLevel = 0;
        public int damageLevel = 0;
        public int dodgeLevel = 0;

        public const int MaxHpLevel = 10;
        public const int MaxDamageLevel = 10;
        public const int MaxDodgeLevel = 5;

        public const int BaseHpBonusPerLevel = 25;
        public const int BaseDamageBonusPerLevel = 8;
        public const float BaseDodgeBonusPerLevel = 0.03f;

        public int GetUpgradeCost(int currentLevel)
        {
            return (currentLevel + 1) * 150;
        }
    }

    /// <summary>
    /// Quản lý tiến trình nâng cấp chỉ số Hero (Meta Progression).
    /// Tích hợp trực tiếp với SaveService (HMAC-SHA256 Encrypted).
    /// </summary>
    public class HeroUpgradeManager : MonoBehaviour
    {
        public static HeroUpgradeManager Instance { get; private set; }

        private const string UpgradeSaveKey = "ShadowReflex_HeroUpgrades";
        private HeroUpgradeData _data = new HeroUpgradeData();

        public HeroUpgradeData Data => _data;

        public event Action OnUpgradesChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            LoadUpgrades();
        }

        public void LoadUpgrades()
        {
            if (SaveService.Instance != null)
            {
                var save = SaveService.Instance.LoadGame();
                if (save != null && !string.IsNullOrEmpty(save.customPayload))
                {
                    try
                    {
                        var loaded = JsonUtility.FromJson<HeroUpgradeData>(save.customPayload);
                        if (loaded != null)
                        {
                            _data = loaded;
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"[HeroUpgradeManager] Không thể parse customPayload: {ex.Message}");
                    }
                }
            }

            // Fallback nạp từ PlayerPrefs nếu có
            if (PlayerPrefs.HasKey(UpgradeSaveKey))
            {
                string json = PlayerPrefs.GetString(UpgradeSaveKey);
                _data = JsonUtility.FromJson<HeroUpgradeData>(json) ?? new HeroUpgradeData();
            }
        }

        public void SaveUpgrades()
        {
            string json = JsonUtility.ToJson(_data);
            PlayerPrefs.SetString(UpgradeSaveKey, json);
            PlayerPrefs.Save();

            if (SaveService.Instance != null)
            {
                var save = SaveService.Instance.LoadGame() ?? new GameSaveData();
                save.customPayload = json;
                SaveService.Instance.SaveGame(save);
            }

            OnUpgradesChanged?.Invoke();
        }

        public bool TryUpgradeHP()
        {
            if (_data.hpLevel >= HeroUpgradeData.MaxHpLevel) return false;
            int cost = _data.GetUpgradeCost(_data.hpLevel);

            if (SaveService.Instance != null && SaveService.Instance.GetCoins() >= cost)
            {
                SaveService.Instance.AddCoins(-cost);
                _data.hpLevel++;
                SaveUpgrades();
                return true;
            }
            return false;
        }

        public bool TryUpgradeDamage()
        {
            if (_data.damageLevel >= HeroUpgradeData.MaxDamageLevel) return false;
            int cost = _data.GetUpgradeCost(_data.damageLevel);

            if (SaveService.Instance != null && SaveService.Instance.GetCoins() >= cost)
            {
                SaveService.Instance.AddCoins(-cost);
                _data.damageLevel++;
                SaveUpgrades();
                return true;
            }
            return false;
        }

        public bool TryUpgradeDodge()
        {
            if (_data.dodgeLevel >= HeroUpgradeData.MaxDodgeLevel) return false;
            int cost = _data.GetUpgradeCost(_data.dodgeLevel);

            if (SaveService.Instance != null && SaveService.Instance.GetCoins() >= cost)
            {
                SaveService.Instance.AddCoins(-cost);
                _data.dodgeLevel++;
                SaveUpgrades();
                return true;
            }
            return false;
        }

        public int GetBonusHp() => _data.hpLevel * HeroUpgradeData.BaseHpBonusPerLevel;
        public int GetBonusDamage() => _data.damageLevel * HeroUpgradeData.BaseDamageBonusPerLevel;
        public float GetBonusDodgeDuration() => _data.dodgeLevel * HeroUpgradeData.BaseDodgeBonusPerLevel;
    }
}
