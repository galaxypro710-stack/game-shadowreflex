using System;
using UnityEngine;

namespace Game.Gameplay
{
    public enum HeroCombatState
    {
        Idle,
        Attacking,
        Dodging,
        Stunned,
        Dead
    }

    public enum EnemyCombatState
    {
        Idle,
        Approaching,
        WindupTelegraph,
        ActiveStrike,
        Stunned,
        Dead
    }

    [Serializable]
    public class EnemyConfig
    {
        public string enemyName = "Shadow Ninja";
        public int maxHp = 100;
        public int attackDamage = 15;
        public float approachSpeed = 3.0f;
        public float windupDuration = 0.65f;
        public float strikeWindow = 0.25f;
        public int scoreReward = 50;
    }
}
