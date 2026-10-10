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
using Game.Audio;

namespace Game.Editor
{
    [InitializeOnLoad]
    public static class SceneSetupHelper
    {
        private const string ScenePath = "Assets/_Project/Scenes/Game.unity";

        [MenuItem("Tools/ShadowReflex/Setup Combat Scene", false, 1)]
        public static void SetupScene()
        {
            Debug.Log("[SceneSetupHelper] === KHỞI TẠO SHADOWREFLEX COMBAT SCENE VỚI ASSETS V1.5 ===");

            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

            // 1. Tải toàn bộ Sprites & AudioClips V1.5
            var heroIdle = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/Characters/hero_idle.png");
            var heroAttack = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/Characters/hero_attack.png");
            var heroDodge = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/Characters/hero_dodge.png");

            var scoutIdle = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/Characters/enemy_scout_idle.png");
            var scoutWindup = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/Characters/enemy_scout_windup.png");
            var scoutStrike = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/Characters/enemy_scout_strike.png");

            var roninIdle = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/Characters/enemy_ronin_idle.png");
            var roninWindup = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/Characters/enemy_ronin_windup.png");
            var roninStrike = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/Characters/enemy_ronin_strike.png");

            var bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/Environment/bg_bamboo_night.png");

            var bgmClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/_Project/Audio/bgm_shadow_battle.wav");
            var sfxSlash = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/_Project/Audio/sfx_slash.wav");
            var sfxDodge = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/_Project/Audio/sfx_dodge_whoosh.wav");
            var sfxParry = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/_Project/Audio/sfx_parry_clang.wav");
            var sfxHit = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/_Project/Audio/sfx_hit_impact.wav");
            var sfxAlert = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/_Project/Audio/sfx_enemy_alert.wav");

            // 2. Đảm bảo Main Camera & AudioListener
            var cam = Camera.main;
            if (cam == null)
            {
                var camObj = new GameObject("Main Camera");
                cam = camObj.AddComponent<Camera>();
                camObj.tag = "MainCamera";
                cam.orthographic = true;
                cam.orthographicSize = 5f;
                cam.backgroundColor = new Color(0.05f, 0.06f, 0.09f, 1f); // Deep shadow night
                cam.clearFlags = CameraClearFlags.SolidColor;
                camObj.transform.position = new Vector3(0f, 0f, -10f);
            }

            if (cam.GetComponent<AudioListener>() == null)
            {
                cam.gameObject.AddComponent<AudioListener>();
            }

            // 3. Đảm bảo Phông nền Silhouette (Background_Night)
            var bgObj = GameObject.Find("Background_Night");
            if (bgObj == null)
            {
                bgObj = new GameObject("Background_Night");
            }
            bgObj.transform.position = new Vector3(0f, 0f, 5f);
            bgObj.transform.localScale = new Vector3(1.9f, 1.9f, 1f);
            var bgRenderer = bgObj.GetComponent<SpriteRenderer>();
            if (bgRenderer == null) bgRenderer = bgObj.AddComponent<SpriteRenderer>();
            if (bgSprite != null) bgRenderer.sprite = bgSprite;
            bgRenderer.sortingOrder = -20;

            // 4. Đảm bảo EventSystem
            var eventSystem = Object.FindAnyObjectByType<EventSystem>();
            if (eventSystem == null)
            {
                var esObj = new GameObject("EventSystem");
                eventSystem = esObj.AddComponent<EventSystem>();
                esObj.AddComponent<StandaloneInputModule>();
            }

            // 5. Đảm bảo Core Infrastructure
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

            // 6. Đảm bảo AudioManager & Gán toàn bộ Audio Clips
            var audioMgr = Object.FindAnyObjectByType<AudioManager>();
            if (audioMgr == null)
            {
                var audioObj = new GameObject("AudioManager");
                audioMgr = audioObj.AddComponent<AudioManager>();
            }
            var audioSO = new SerializedObject(audioMgr);
            var bgmProp = audioSO.FindProperty("_bgmGameplay");
            if (bgmProp != null && bgmClip != null) bgmProp.objectReferenceValue = bgmClip;
            var slashProp = audioSO.FindProperty("_sfxSlash");
            if (slashProp != null && sfxSlash != null) slashProp.objectReferenceValue = sfxSlash;
            var dodgeProp = audioSO.FindProperty("_sfxDodgeWhoosh");
            if (dodgeProp != null && sfxDodge != null) dodgeProp.objectReferenceValue = sfxDodge;
            var parryProp = audioSO.FindProperty("_sfxParryClang");
            if (parryProp != null && sfxParry != null) parryProp.objectReferenceValue = sfxParry;
            var hitProp = audioSO.FindProperty("_sfxHitImpact");
            if (hitProp != null && sfxHit != null) hitProp.objectReferenceValue = sfxHit;
            var alertProp = audioSO.FindProperty("_sfxEnemyAlert");
            if (alertProp != null && sfxAlert != null) alertProp.objectReferenceValue = sfxAlert;
            audioSO.ApplyModifiedProperties();

            // 7. Khởi tạo Hero và gán Sprite Set
            var heroObj = GameObject.Find("Hero_Shadow");
            HeroCombatController hero;
            if (heroObj == null)
            {
                heroObj = new GameObject("Hero_Shadow");
                hero = heroObj.AddComponent<HeroCombatController>();
            }
            else
            {
                hero = heroObj.GetComponent<HeroCombatController>() ?? heroObj.AddComponent<HeroCombatController>();
            }
            heroObj.transform.position = new Vector3(-1.8f, -1.2f, 0f);
            heroObj.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
            var heroSr = heroObj.GetComponent<SpriteRenderer>() ?? heroObj.AddComponent<SpriteRenderer>();
            heroSr.sortingOrder = 5;
            if (heroIdle != null) heroSr.sprite = heroIdle;

            var heroSO = new SerializedObject(hero);
            var idlProp = heroSO.FindProperty("_idleSprite");
            if (idlProp != null && heroIdle != null) idlProp.objectReferenceValue = heroIdle;
            var atkProp = heroSO.FindProperty("_attackSprite");
            if (atkProp != null && heroAttack != null) atkProp.objectReferenceValue = heroAttack;
            var ddgProp = heroSO.FindProperty("_dodgeSprite");
            if (ddgProp != null && heroDodge != null) ddgProp.objectReferenceValue = heroDodge;
            heroSO.ApplyModifiedProperties();

            // 8. Đảm bảo SpawnPoint
            var spawnPoint = GameObject.Find("EnemySpawnPoint");
            if (spawnPoint == null)
            {
                spawnPoint = new GameObject("EnemySpawnPoint");
            }
            spawnPoint.transform.position = new Vector3(3.2f, -1.2f, 0f);

            // 9. Khởi tạo TimingCombatManager & Gán Enemy Sprite Sets
            var combatManager = Object.FindAnyObjectByType<TimingCombatManager>();
            if (combatManager == null)
            {
                var combatObj = new GameObject("TimingCombatManager");
                combatManager = combatObj.AddComponent<TimingCombatManager>();
            }

            var so = new SerializedObject(combatManager);
            var heroRefProp = so.FindProperty("_hero");
            if (heroRefProp != null) heroRefProp.objectReferenceValue = hero;
            var spawnRefProp = so.FindProperty("_enemySpawnPoint");
            if (spawnRefProp != null) spawnRefProp.objectReferenceValue = spawnPoint.transform;

            var scIdl = so.FindProperty("_scoutIdle");
            if (scIdl != null && scoutIdle != null) scIdl.objectReferenceValue = scoutIdle;
            var scWin = so.FindProperty("_scoutWindup");
            if (scWin != null && scoutWindup != null) scWin.objectReferenceValue = scoutWindup;
            var scStr = so.FindProperty("_scoutStrike");
            if (scStr != null && scoutStrike != null) scStr.objectReferenceValue = scoutStrike;

            var rnIdl = so.FindProperty("_roninIdle");
            if (rnIdl != null && roninIdle != null) rnIdl.objectReferenceValue = roninIdle;
            var rnWin = so.FindProperty("_roninWindup");
            if (rnWin != null && roninWindup != null) rnWin.objectReferenceValue = roninWindup;
            var rnStr = so.FindProperty("_roninStrike");
            if (rnStr != null && roninStrike != null) rnStr.objectReferenceValue = roninStrike;

            so.ApplyModifiedProperties();

            // 10. Khởi tạo UI Canvas & CombatHUD
            var hud = Object.FindAnyObjectByType<CombatHUD>();
            if (hud == null)
            {
                var canvasObj = new GameObject("Canvas");
                var canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                var scaler = canvasObj.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1080, 1920);
                scaler.matchWidthOrHeight = 0.5f;
                canvasObj.AddComponent<GraphicRaycaster>();

                hud = canvasObj.AddComponent<CombatHUD>();

                // Tạo các nút cảm ứng và text
                var attackBtnObj = new GameObject("AttackButton");
                attackBtnObj.transform.SetParent(canvasObj.transform, false);
                var atkBtn = attackBtnObj.AddComponent<Button>();
                var atkImg = attackBtnObj.AddComponent<Image>();
                atkImg.color = new Color(0.9f, 0.2f, 0.2f, 0.35f);
                var atkRect = attackBtnObj.GetComponent<RectTransform>();
                atkRect.anchorMin = new Vector2(0.5f, 0f);
                atkRect.anchorMax = new Vector2(1f, 0.45f);
                atkRect.offsetMin = Vector2.zero;
                atkRect.offsetMax = Vector2.zero;

                var dodgeBtnObj = new GameObject("DodgeButton");
                dodgeBtnObj.transform.SetParent(canvasObj.transform, false);
                var dodgeBtn = dodgeBtnObj.AddComponent<Button>();
                var dodgeImg = dodgeBtnObj.AddComponent<Image>();
                dodgeImg.color = new Color(0.2f, 0.6f, 0.9f, 0.35f);
                var dodgeRect = dodgeBtnObj.GetComponent<RectTransform>();
                dodgeRect.anchorMin = new Vector2(0f, 0f);
                dodgeRect.anchorMax = new Vector2(0.5f, 0.45f);
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

            Debug.Log("[SceneSetupHelper] Hoàn tất tích hợp toàn bộ Sprites & Audio V1.5 vào Game.unity!");
        }
    }
}
