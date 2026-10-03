using UnityEngine;
using Game.Data;
using Game.Events;

namespace Game.Core
{
    /// <summary>
    /// Điều phối vòng đời khởi động toàn hệ thống trò chơi (Game Bootstrapper).
    /// Chuẩn Universal Core: Khởi tạo Save, Audio, FSM và quản lý tự động lưu khi Pause/Quit.
    /// Hoàn toàn không phụ thuộc vào bất kỳ cơ chế gameplay đặc thù nào.
    /// </summary>
    public class GameMaster : MonoBehaviour
    {
        [Header("Core Dependencies")]
        [SerializeField] private GameManager _gameManager;
        [SerializeField] private SaveService _saveService;

        [Header("Event Channels")]
        [SerializeField] private GameStateEventChannelSO _onGameStateChangedChannel;

        private void Awake()
        {
#if UNITY_STANDALONE || UNITY_EDITOR
            if (!Application.isMobilePlatform)
            {
                Screen.SetResolution(540, 960, FullScreenMode.Windowed);
            }
#endif
            if (_gameManager == null) _gameManager = GameManager.Instance;
            if (_saveService == null) _saveService = SaveService.Instance;
        }

#if UNITY_STANDALONE || UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F11))
            {
                Screen.fullScreen = !Screen.fullScreen;
            }
        }
#endif

        private void OnEnable()
        {
            if (_onGameStateChangedChannel != null)
            {
                _onGameStateChangedChannel.OnEventRaised += HandleGameStateChanged;
            }
        }

        private void OnDisable()
        {
            if (_onGameStateChangedChannel != null)
            {
                _onGameStateChangedChannel.OnEventRaised -= HandleGameStateChanged;
            }
        }

        private void Start()
        {
            InitializeGameSession();
        }

        /// <summary>
        /// Khởi tạo phiên chơi: nạp lại ván đấu dở dang nếu có, hoặc phát lệnh bắt đầu chơi mới.
        /// </summary>
        public void InitializeGameSession()
        {
            if (_saveService != null && _saveService.HasSavedGame())
            {
                GameSaveData savedData = _saveService.LoadGame();
                if (savedData != null)
                {
                    Debug.Log($"[GameMaster] Khôi phục phiên chơi dở dang: Level {savedData.currentLevel}, Score {savedData.score}.");
                }
            }

            if (_gameManager != null)
            {
                _gameManager.ChangeState(GameState.Playing);
            }
        }

        private void HandleGameStateChanged(GameState newState)
        {
            // Khi thắng hoặc thua, xóa dữ liệu dở dang để ván sau bắt đầu mới
            if (newState == GameState.Victory || newState == GameState.GameOver)
            {
                if (_saveService != null)
                {
                    _saveService.ClearSaveData();
                }
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                SaveCurrentSession();
            }
        }

        private void OnApplicationQuit()
        {
            SaveCurrentSession();
        }

        private void SaveCurrentSession()
        {
            if (_saveService == null || _gameManager == null) return;
            if (_gameManager.CurrentState != GameState.Playing) return;

            // Mặc định lưu trữ cơ bản của phiên chơi
            GameSaveData currentData = _saveService.LoadGame() ?? new GameSaveData(1, 0, _saveService.GetHighScore(), _saveService.GetCoins());
            _saveService.SaveGame(currentData);
        }
    }
}
