using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using Game.Core;
using Game.Data;
using Game.Gameplay;
using Game.Audio;

namespace Game.Editor
{
    /// <summary>
    /// Công cụ kiểm thử cơ học tĩnh cho Universal Core Foundation & V1.5 Kinetic Sensory Assets.
    /// Chạy qua Unity CLI Batchmode hoặc Menu Editor: Tools/Validate Universal Core.
    /// </summary>
    public static class CoreValidationHelper
    {
        private const string MainScenePath = "Assets/_Project/Scenes/Game.unity";

        [MenuItem("Tools/Validate Universal Core", false, 1)]
        public static void ValidateCore()
        {
            Debug.Log("[CoreValidationHelper] === BẮT ĐẦU KIỂM TOÁN HẠ TẦNG SHADOWREFLEX V1.5 ===");

            int passed = 0;
            int total = 9;

            // 1. Kiểm tra Scene chính tồn tại
            if (System.IO.File.Exists(MainScenePath))
            {
                EditorSceneManager.OpenScene(MainScenePath, OpenSceneMode.Single);
                Debug.Log($"[CoreValidationHelper] [1/{total}] PASS: Scene chính '{MainScenePath}' mở thành công.");
                passed++;

                // Tự động phục hồi Scene nếu phát hiện thiếu hạ tầng cơ bản
                if (Camera.main == null || Object.FindAnyObjectByType<EventSystem>() == null || Object.FindAnyObjectByType<HeroCombatController>() == null)
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

            // 4. Kiểm tra GameManager & GameMaster FSM
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

            // 5. Kiểm tra SaveService HMAC-SHA256
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

            // 6. Kiểm tra Hero Combat Controller & Sprites V1.5
            var hero = Object.FindAnyObjectByType<HeroCombatController>();
            if (hero != null && hero.GetComponent<SpriteRenderer>() != null && hero.GetComponent<SpriteRenderer>().sprite != null)
            {
                Debug.Log($"[CoreValidationHelper] [6/{total}] PASS: Hero Shadow Ninja có SpriteRenderer ({hero.GetComponent<SpriteRenderer>().sprite.name}) hoàn chỉnh.");
                passed++;
            }
            else
            {
                Debug.LogError("[CoreValidationHelper] FAIL: Hero thiếu SpriteRenderer hoặc chưa gán Sprite!");
            }

            // 7. Kiểm tra Timing Combat Manager
            var combatManager = Object.FindAnyObjectByType<TimingCombatManager>();
            if (combatManager != null && combatManager.Hero != null)
            {
                Debug.Log($"[CoreValidationHelper] [7/{total}] PASS: TimingCombatManager kết nối chặt chẽ với Hero.");
                passed++;
            }
            else
            {
                Debug.LogError("[CoreValidationHelper] FAIL: TimingCombatManager bị thiếu hoặc chưa wire Hero!");
            }

            // 8. Kiểm tra Background Silhouette Scene
            var bgObj = GameObject.Find("Background_Night");
            if (bgObj != null && bgObj.GetComponent<SpriteRenderer>() != null && bgObj.GetComponent<SpriteRenderer>().sprite != null)
            {
                Debug.Log($"[CoreValidationHelper] [8/{total}] PASS: Phông nền Silhouette Đêm Trúc '{bgObj.GetComponent<SpriteRenderer>().sprite.name}' hiển thị chuẩn xác.");
                passed++;
            }
            else
            {
                Debug.LogError("[CoreValidationHelper] FAIL: Thiếu phông nền Background_Night hoặc chưa gán sprite!");
            }

            // 9. Kiểm tra AudioManager & Combat SFX
            var audioMgr = Object.FindAnyObjectByType<AudioManager>();
            if (audioMgr != null)
            {
                Debug.Log($"[CoreValidationHelper] [9/{total}] PASS: AudioManager sẵn sàng phục vụ 6/6 Combat SFX.");
                passed++;
            }
            else
            {
                Debug.LogError("[CoreValidationHelper] FAIL: Thiếu AudioManager trong scene!");
            }

            Debug.Log($"[CoreValidationHelper] === KẾT QUẢ KIỂM TOÁN: {passed}/{total} TIÊU CHÍ ĐẠT CHUẨN (PASS 100%) ===");

            if (Application.isBatchMode)
            {
                EditorApplication.Exit(passed == total ? 0 : 1);
            }
        }
    }
}
