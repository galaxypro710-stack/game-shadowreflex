using UnityEngine;
using Game.Events;

namespace Game.Core
{
    /// <summary>
    /// Bộ điều phối trạng thái trung tâm của trò chơi (Finite State Machine).
    /// Chuẩn Universal Core: Giao tiếp hoàn toàn thông qua ScriptableObject Event Channels (Decoupled Architecture).
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("State Tracking")]
        [SerializeField] private GameState _currentState = GameState.Boot;
        public GameState CurrentState => _currentState;

        [Header("Outgoing State Event Channel")]
        [SerializeField] private GameStateEventChannelSO _onStateChangedChannel;

        [Header("Incoming Event Channels (Triggers)")]
        [SerializeField] private VoidEventChannelSO _onRestartRequestedChannel;
        [SerializeField] private VoidEventChannelSO _onVictoryTriggeredChannel;
        [SerializeField] private VoidEventChannelSO _onDefeatTriggeredChannel;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnEnable()
        {
            if (_onRestartRequestedChannel != null)
            {
                _onRestartRequestedChannel.OnEventRaised += HandleRestartRequested;
            }

            if (_onVictoryTriggeredChannel != null)
            {
                _onVictoryTriggeredChannel.OnEventRaised += HandleVictoryTriggered;
            }

            if (_onDefeatTriggeredChannel != null)
            {
                _onDefeatTriggeredChannel.OnEventRaised += HandleDefeatTriggered;
            }
        }

        private void OnDisable()
        {
            if (_onRestartRequestedChannel != null)
            {
                _onRestartRequestedChannel.OnEventRaised -= HandleRestartRequested;
            }

            if (_onVictoryTriggeredChannel != null)
            {
                _onVictoryTriggeredChannel.OnEventRaised -= HandleVictoryTriggered;
            }

            if (_onDefeatTriggeredChannel != null)
            {
                _onDefeatTriggeredChannel.OnEventRaised -= HandleDefeatTriggered;
            }
        }

        private void Start()
        {
            ChangeState(GameState.Playing);
        }

        public void ChangeState(GameState newState)
        {
            if (_currentState == newState)
            {
                return;
            }

            _currentState = newState;

            // Xử lý TimeScale theo từng state
            switch (newState)
            {
                case GameState.Playing:
                    Time.timeScale = 1.0f;
                    break;
                case GameState.Paused:
                    Time.timeScale = 0.0f;
                    break;
                case GameState.GameOver:
                case GameState.Victory:
                    Time.timeScale = 1.0f;
                    break;
                default:
                    break;
            }

            if (_onStateChangedChannel != null)
            {
                _onStateChangedChannel.RaiseEvent(newState);
            }
        }

        public void StartGame() => ChangeState(GameState.Playing);

        public void PauseGame()
        {
            if (_currentState == GameState.Playing)
            {
                ChangeState(GameState.Paused);
            }
        }

        public void ResumeGame()
        {
            if (_currentState == GameState.Paused)
            {
                ChangeState(GameState.Playing);
            }
        }

        public void SetGameOver() => ChangeState(GameState.GameOver);

        public void SetVictory() => ChangeState(GameState.Victory);

        private void HandleRestartRequested()
        {
            Time.timeScale = 1.0f;
            ChangeState(GameState.Playing);
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }

        private void HandleVictoryTriggered()
        {
            SetVictory();
        }

        private void HandleDefeatTriggered()
        {
            SetGameOver();
        }
    }
}
