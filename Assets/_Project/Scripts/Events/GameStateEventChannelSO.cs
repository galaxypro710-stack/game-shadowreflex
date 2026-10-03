using UnityEngine;
using UnityEngine.Events;
using Game.Core;

namespace Game.Events
{
    /// <summary>
    /// Event Channel phát tín hiệu thay đổi GameState (Boot, Playing, Paused, GameOver, Victory).
    /// </summary>
    [CreateAssetMenu(menuName = "Game/Events/GameState Event Channel", fileName = "GameStateEventChannel")]
    public class GameStateEventChannelSO : ScriptableObject
    {
        public event UnityAction<GameState> OnEventRaised;

        public void RaiseEvent(GameState newState)
        {
            if (OnEventRaised != null)
            {
                OnEventRaised.Invoke(newState);
            }
        }
    }
}
