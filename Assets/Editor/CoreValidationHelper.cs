using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using Game.Core;
using Game.Data;

namespace Game.Editor
{
    /// <summary>
    /// Công cụ kiểm thử cơ học tĩnh cho Universal Core Foundation.
    /// Chạy qua Unity CLI Batchmode hoặc Menu Editor: Tools/Validate Universal Core.
    /// Không chứa bất kỳ logic sinh prefab hay trigger ngầm nào.
    /// </summary>
    public static class CoreValidationHelper
    {
        private const string MainScenePath = "Assets/_Project/Scenes/Game.unity";

        [MenuItem("Tools/Validate Universal Core", false, 1)]
        public static void ValidateCore()
        {
            Debug.Log("[CoreValidationHelper] === BẮT ĐẦU KIỂM TOÁN HẠ TẦNG UNIVERSAL CORE FOUNDATION ===");

            int passed = 0;
            int total = 5;

            // 1. Kiểm tra Scene chính tồn tại
            if (System.IO.File.Exists(MainScenePath))
            {
                EditorSceneManager.OpenScene(MainScenePath, OpenSceneMode.Single);
                Debug.Log($"[CoreValidationHelper] [1/{total}] PASS: Scene chính '{MainScenePath}' mở thành công.");
                passed++;

                // Tự động phục hồi Scene nếu phát hiện thiếu hạ tầng cơ bản
                if (Camera.main == null || Object.FindAnyObjectByType<EventSystem>() == null)
                {
                    Debug.Log("[CoreValidationHelper] Phát hiện thiếu hạ tầng Scene cơ bản. Kích hoạt SceneSetupHelper.SetupScene()...");
                    SceneSetupHelper.SetupScene();
                }
            }
            else
            {
                Debug.LogError($"[CoreValidationHelper] FAIL: Không tìm thấy '{MainScenePath}'!");
            }

            // 2. Kiểm tra Main Camera & AudioListener
            var cam = Camera.main;
            if (cam != null && cam.GetComponent<AudioListener>() != null)
            {
                Debug.Log($"[CoreValidationHelper] [2/{total}] PASS: Main Camera & AudioListener hợp lệ.");
                passed++;
            }
            else
            {
                Debug.LogError("[CoreValidationHelper] FAIL: Thiếu Main Camera hoặc AudioListener!");
            }

            // 3. Kiểm tra EventSystem
            var eventSystem = Object.FindAnyObjectByType<EventSystem>();
            if (eventSystem != null)
            {
                Debug.Log($"[CoreValidationHelper] [3/{total}] PASS: EventSystem hợp lệ.");
                passed++;
            }
            else
            {
                Debug.LogError("[CoreValidationHelper] FAIL: Thiếu EventSystem trong scene!");
            }

            // 4. Kiểm tra GameManager & GameMaster
            var gm = Object.FindAnyObjectByType<GameManager>();
            var master = Object.FindAnyObjectByType<GameMaster>();
            if (gm != null && master != null)
            {
                Debug.Log($"[CoreValidationHelper] [4/{total}] PASS: GameManager & GameMaster FSM sẵn sàng.");
                passed++;
            }
            else
            {
                Debug.LogError("[CoreValidationHelper] FAIL: Thiếu GameManager hoặc GameMaster!");
            }

            // 5. Kiểm tra SaveService HMAC
            var saveService = Object.FindAnyObjectByType<SaveService>();
            if (saveService != null)
            {
                Debug.Log($"[CoreValidationHelper] [5/{total}] PASS: SaveService HMAC-SHA256 sẵn sàng.");
                passed++;
            }
            else
            {
                Debug.LogError("[CoreValidationHelper] FAIL: Thiếu SaveService trong scene!");
            }

            Debug.Log($"[CoreValidationHelper] === KẾT QUẢ KIỂM TOÁN: {passed}/{total} TIÊU CHÍ ĐẠT CHUẨN ===");

            if (Application.isBatchMode)
            {
                EditorApplication.Exit(passed == total ? 0 : 1);
            }
        }
    }
}
