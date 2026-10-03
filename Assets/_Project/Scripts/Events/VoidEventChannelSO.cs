using UnityEngine;
using UnityEngine.Events;

namespace Game.Events
{
    /// <summary>
    /// Event Channel không mang dữ liệu (parameterless), dùng cho các sự kiện như:
    /// RestartGame, BoardCleared, InputLocked, v.v.
    /// </summary>
    [CreateAssetMenu(menuName = "Game/Events/Void Event Channel", fileName = "VoidEventChannel")]
    public class VoidEventChannelSO : ScriptableObject
    {
        public event UnityAction OnEventRaised;

        public void RaiseEvent()
        {
            if (OnEventRaised != null)
            {
                OnEventRaised.Invoke();
            }
        }
    }
}
