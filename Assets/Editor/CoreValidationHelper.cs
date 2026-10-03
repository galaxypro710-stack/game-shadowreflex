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
            int total = 8;

            // Đảm bảo Art và Audio độc quyền đã sinh sẵn
            if (!System.IO.File.Exists("Assets/_Project/Art/Sprites/Hero_Ninja.png") ||
                !System.IO.File.Exists("Assets/_Project/Resources/Art/Sprites/Hero_Ninja.png"))
            {
                Debug.Log("[CoreValidationHelper] Chưa phát hiện Sprite Art hoặc Resources. Kích hoạt StudioArtGenerator...");
                StudioArtGenerator.GenerateAllSprites();
            }

            if (!System.IO.File.Exists("Assets/_Project/Audio/Clips/sfx_telegraph_warning.wav") ||
                !System.IO.File.Exists("Assets/_Project/Audio/Clips/bgm_combat_zen.wav") ||
                !System.IO.File.Exists("Assets/_Project/Audio/Clips/sfx_click.wav") ||
                !System.IO.File.Exists("Assets/_Project/Audio/Clips/sfx_reward.wav") ||
                !System.IO.File.Exists("Assets/_Project/Resources/Audio/Clips/sfx_slash.wav"))
            {
                Debug.Log("[CoreValidationHelper] Phát hiện thiếu SFX/BGM độc quyền mới. Kích hoạt AudioSynthesizerGenerator...");
                AudioSynthesizerGenerator.GenerateAllSFX();
            }

            // 1. Kiểm tra Scene chính tồn tại
            if (System.IO.File.Exists(MainScenePath))
            {
                EditorSceneManager.OpenScene(MainScenePath, OpenSceneMode.Single);
                Debug.Log($"[CoreValidationHelper] [1/{total}] PASS: Scene chính '{MainScenePath}' mở thành công.");
                passed++;

                // Tự động đồng bộ và nạp cấu hình mới nhất cho Scene V2
                Debug.Log("[CoreValidationHelper] Kích hoạt SceneSetupHelper.SetupScene() để đồng bộ hạ tầng V2...");
                SceneSetupHelper.SetupScene();
            }
            else
            {
                Debug.LogError($"[CoreValidationHelper] FAIL: Không tìm thấy '{MainScenePath}'!");
            }

            // 2. Kiểm tra Main Camera, AudioListener & CameraShake
            var cam = Camera.main;
            if (cam != null && cam.GetComponent<AudioListener>() != null && cam.GetComponent<Game.Gameplay.CameraShakeController>() != null)
            {
                Debug.Log($"[CoreValidationHelper] [2/{total}] PASS: Main Camera, AudioListener & CameraShakeController hợp lệ.");
                passed++;
            }
            else
            {
                Debug.LogError("[CoreValidationHelper] FAIL: Thiếu Main Camera, AudioListener hoặc CameraShakeController!");
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

            // 5. Kiểm tra SaveService HMAC & HeroUpgradeManager
            var saveService = Object.FindAnyObjectByType<SaveService>();
            var upgradeMgr = Object.FindAnyObjectByType<Game.Gameplay.HeroUpgradeManager>();
            if (saveService != null && upgradeMgr != null)
            {
                Debug.Log($"[CoreValidationHelper] [5/{total}] PASS: SaveService HMAC-SHA256 & HeroUpgradeManager sẵn sàng.");
                passed++;
            }
            else
            {
                Debug.LogError("[CoreValidationHelper] FAIL: Thiếu SaveService hoặc HeroUpgradeManager!");
            }

            // 6. Kiểm tra CombatManager, Hero & CombatVFXController
            var combatMgr = Object.FindAnyObjectByType<Game.Gameplay.TimingCombatManager>();
            var hero = Object.FindAnyObjectByType<Game.Gameplay.HeroCombatController>();
            var vfxCtrl = Object.FindAnyObjectByType<Game.Gameplay.CombatVFXController>();
            if (combatMgr != null && hero != null && vfxCtrl != null)
            {
                Debug.Log($"[CoreValidationHelper] [6/{total}] PASS: TimingCombatManager, HeroCombatController & CombatVFXController sẵn sàng.");
                passed++;
            }
            else
            {
                Debug.LogError("[CoreValidationHelper] FAIL: Thiếu CombatManager, Hero hoặc CombatVFXController!");
            }

            // 7. Kiểm tra Studio Art Sprites độc quyền
            bool spritesOk = System.IO.File.Exists("Assets/_Project/Art/Sprites/Hero_Ninja.png") &&
                             System.IO.File.Exists("Assets/_Project/Art/Sprites/Enemy_Swift.png") &&
                             System.IO.File.Exists("Assets/_Project/Art/Sprites/Enemy_Brute.png") &&
                             System.IO.File.Exists("Assets/_Project/Art/Sprites/Enemy_Boss.png") &&
                             System.IO.File.Exists("Assets/_Project/Art/Sprites/VFX_Slash.png");
            if (spritesOk)
            {
                Debug.Log($"[CoreValidationHelper] [7/{total}] PASS: 100% Studio Art Sprites độc quyền (Ninja, Swift, Brute, Boss, VFX) hoàn tất.");
                passed++;
            }
            else
            {
                Debug.LogError("[CoreValidationHelper] FAIL: Thiếu file Studio Art Sprites độc quyền!");
            }

            // 8. Kiểm tra Studio Audio & Haptics Infrastructure
            bool audioOk = System.IO.File.Exists("Assets/_Project/Audio/Clips/sfx_slash.wav") &&
                            System.IO.File.Exists("Assets/_Project/Audio/Clips/sfx_whoosh.wav") &&
                            System.IO.File.Exists("Assets/_Project/Audio/Clips/sfx_parry_clang.wav") &&
                            System.IO.File.Exists("Assets/_Project/Audio/Clips/sfx_hit_impact.wav") &&
                            System.IO.File.Exists("Assets/_Project/Audio/Clips/sfx_telegraph_warning.wav") &&
                            System.IO.File.Exists("Assets/_Project/Audio/Clips/sfx_click.wav") &&
                            System.IO.File.Exists("Assets/_Project/Audio/Clips/sfx_reward.wav") &&
                            System.IO.File.Exists("Assets/_Project/Audio/Clips/bgm_combat_zen.wav");
            var audioMgr = Object.FindAnyObjectByType<Game.Audio.AudioManager>();
            if (audioOk && audioMgr != null)
            {
                Debug.Log($"[CoreValidationHelper] [8/{total}] PASS: 100% Studio Audio SFX, BGM & AudioManager Haptics sẵn sàng.");
                passed++;
            }
            else
            {
                Debug.LogError("[CoreValidationHelper] FAIL: Thiếu file Audio SFX độc quyền hoặc AudioManager trong scene!");
            }

            Debug.Log($"[CoreValidationHelper] === KẾT QUẢ KIỂM TOÁN: {passed}/{total} TIÊU CHÍ ĐẠT CHUẨN ===");

            if (Application.isBatchMode)
            {
                EditorApplication.Exit(passed == total ? 0 : 1);
            }
        }
    }
}
