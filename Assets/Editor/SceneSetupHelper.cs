using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Game.Core;
using Game.Data;
using Game.Gameplay;
using Game.UI;

namespace Game.Editor
{
    [InitializeOnLoad]
    public static class SceneSetupHelper
    {
        private const string ScenePath = "Assets/_Project/Scenes/Game.unity";

        [MenuItem("Tools/ShadowReflex/Setup Combat Scene", false, 1)]
        public static void SetupScene()
        {
            Debug.Log("[SceneSetupHelper] === KHỞI TẠO SHADOWREFLEX COMBAT SCENE ===");

            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

            // 1. Đảm bảo Main Camera & AudioListener
            var cam = Camera.main;
            if (cam == null)
            {
                var camObj = new GameObject("Main Camera");
                cam = camObj.AddComponent<Camera>();
                camObj.tag = "MainCamera";
                cam.orthographic = true;
                cam.orthographicSize = 5f;
                cam.backgroundColor = new Color(0.1f, 0.1f, 0.15f, 1f); // Dark shadow tone
                cam.clearFlags = CameraClearFlags.SolidColor;
                camObj.transform.position = new Vector3(0f, 0f, -10f);
            }

            if (cam.GetComponent<AudioListener>() == null)
            {
                cam.gameObject.AddComponent<AudioListener>();
            }

            // 2. Đảm bảo EventSystem
            var eventSystem = Object.FindAnyObjectByType<EventSystem>();
            if (eventSystem == null)
            {
                var esObj = new GameObject("EventSystem");
                eventSystem = esObj.AddComponent<EventSystem>();
                esObj.AddComponent<StandaloneInputModule>();
            }

            // 3. Đảm bảo Core Infrastructure (GameManager, GameMaster, SaveService)
            var gm = Object.FindAnyObjectByType<GameManager>();
            if (gm == null)
            {
                var gmObj = new GameObject("GameManager");
                gm = gmObj.AddComponent<GameManager>();
            }

            var master = Object.FindAnyObjectByType<GameMaster>();
            if (master == null)
            {
                var masterObj = new GameObject("GameMaster");
                master = masterObj.AddComponent<GameMaster>();
            }

            var saveService = Object.FindAnyObjectByType<SaveService>();
            if (saveService == null)
            {
                var saveObj = new GameObject("SaveService");
                saveService = saveObj.AddComponent<SaveService>();
            }

            // 4. Khởi tạo Gameplay Entities (Hero, SpawnPoint, TimingCombatManager)
            var hero = Object.FindAnyObjectByType<HeroCombatController>();
            if (hero == null)
            {
                var heroObj = new GameObject("Hero_Shadow");
                heroObj.transform.position = new Vector3(-2f, 0f, 0f);
                var sr = heroObj.AddComponent<SpriteRenderer>();
                sr.color = Color.white;
                hero = heroObj.AddComponent<HeroCombatController>();
            }

            var spawnPoint = GameObject.Find("EnemySpawnPoint");
            if (spawnPoint == null)
            {
                spawnPoint = new GameObject("EnemySpawnPoint");
                spawnPoint.transform.position = new Vector3(4f, 0f, 0f);
            }

            var combatManager = Object.FindAnyObjectByType<TimingCombatManager>();
            if (combatManager == null)
            {
                var combatObj = new GameObject("TimingCombatManager");
                combatManager = combatObj.AddComponent<TimingCombatManager>();
            }

            // Wire serialized fields to combatManager
            var so = new SerializedObject(combatManager);
            var heroProp = so.FindProperty("_hero");
            if (heroProp != null) heroProp.objectReferenceValue = hero;
            var spawnProp = so.FindProperty("_enemySpawnPoint");
            if (spawnProp != null) spawnProp.objectReferenceValue = spawnPoint.transform;
            so.ApplyModifiedProperties();

            // 5. Khởi tạo UI Canvas & CombatHUD
            var hud = Object.FindAnyObjectByType<CombatHUD>();
            if (hud == null)
            {
                var canvasObj = new GameObject("Canvas");
                var canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();

                hud = canvasObj.AddComponent<CombatHUD>();

                // Tạo các nút cảm ứng và text
                var attackBtnObj = new GameObject("AttackButton");
                attackBtnObj.transform.SetParent(canvasObj.transform, false);
                var atkBtn = attackBtnObj.AddComponent<Button>();
                var atkImg = attackBtnObj.AddComponent<Image>();
                atkImg.color = new Color(0.9f, 0.2f, 0.2f, 0.5f);
                var atkRect = attackBtnObj.GetComponent<RectTransform>();
                atkRect.anchorMin = new Vector2(0.5f, 0f);
                atkRect.anchorMax = new Vector2(1f, 0.5f);
                atkRect.offsetMin = Vector2.zero;
                atkRect.offsetMax = Vector2.zero;

                var dodgeBtnObj = new GameObject("DodgeButton");
                dodgeBtnObj.transform.SetParent(canvasObj.transform, false);
                var dodgeBtn = dodgeBtnObj.AddComponent<Button>();
                var dodgeImg = dodgeBtnObj.AddComponent<Image>();
                dodgeImg.color = new Color(0.2f, 0.6f, 0.9f, 0.5f);
                var dodgeRect = dodgeBtnObj.GetComponent<RectTransform>();
                dodgeRect.anchorMin = new Vector2(0f, 0f);
                dodgeRect.anchorMax = new Vector2(0.5f, 0.5f);
                dodgeRect.offsetMin = Vector2.zero;
                dodgeRect.offsetMax = Vector2.zero;

                var hudSO = new SerializedObject(hud);
                var cmProp = hudSO.FindProperty("_combatManager");
                if (cmProp != null) cmProp.objectReferenceValue = combatManager;
                var hProp = hudSO.FindProperty("_hero");
                if (hProp != null) hProp.objectReferenceValue = hero;
                var abProp = hudSO.FindProperty("_attackButton");
                if (abProp != null) abProp.objectReferenceValue = atkBtn;
                var dbProp = hudSO.FindProperty("_dodgeButton");
                if (dbProp != null) dbProp.objectReferenceValue = dodgeBtn;
                hudSO.ApplyModifiedProperties();
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();

            Debug.Log("[SceneSetupHelper] Hoàn tất thiết lập Game.unity cho ShadowReflex!");
        }
    }
}
