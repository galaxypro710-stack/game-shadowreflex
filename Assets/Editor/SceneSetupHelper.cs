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
            Debug.Log("[SceneSetupHelper] === KHỞI TẠO SHADOWREFLEX COMBAT SCENE V2 ===");

            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

            // 1. Đảm bảo Main Camera, AudioListener & CameraShakeController
            var cam = Camera.main;
            if (cam == null)
            {
                var camObj = new GameObject("Main Camera");
                cam = camObj.AddComponent<Camera>();
                camObj.tag = "MainCamera";
                cam.orthographic = true;
                cam.orthographicSize = 5f;
                cam.backgroundColor = new Color(0.043f, 0.075f, 0.149f, 1f); // #0B1326 Obsidian Dark
                cam.clearFlags = CameraClearFlags.SolidColor;
                camObj.transform.position = new Vector3(0f, 0f, -10f);
            }
            else
            {
                cam.backgroundColor = new Color(0.043f, 0.075f, 0.149f, 1f);
            }

            if (cam.GetComponent<AudioListener>() == null)
            {
                cam.gameObject.AddComponent<AudioListener>();
            }

            if (cam.GetComponent<CameraShakeController>() == null)
            {
                cam.gameObject.AddComponent<CameraShakeController>();
            }

            // 2. Đảm bảo EventSystem
            var eventSystem = Object.FindAnyObjectByType<EventSystem>();
            if (eventSystem == null)
            {
                var esObj = new GameObject("EventSystem");
                eventSystem = esObj.AddComponent<EventSystem>();
                esObj.AddComponent<StandaloneInputModule>();
            }

            // 3. Đảm bảo Core Infrastructure & Managers
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

            var upgradeMgr = Object.FindAnyObjectByType<HeroUpgradeManager>();
            if (upgradeMgr == null)
            {
                var upObj = new GameObject("HeroUpgradeManager");
                upgradeMgr = upObj.AddComponent<HeroUpgradeManager>();
            }

            // Đảm bảo AudioManager được cấu hình và nạp đầy đủ AudioClips độc quyền
            var audioMgr = Object.FindAnyObjectByType<Game.Audio.AudioManager>();
            if (audioMgr == null)
            {
                var audioObj = new GameObject("AudioManager");
                audioMgr = audioObj.AddComponent<Game.Audio.AudioManager>();
            }

            var audioSO = new SerializedObject(audioMgr);
            var slashClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/_Project/Audio/Clips/sfx_slash.wav");
            var whooshClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/_Project/Audio/Clips/sfx_whoosh.wav");
            var parryClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/_Project/Audio/Clips/sfx_parry_clang.wav");
            var hitClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/_Project/Audio/Clips/sfx_hit_impact.wav");
            var teleClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/_Project/Audio/Clips/sfx_telegraph_warning.wav");
            var clickClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/_Project/Audio/Clips/sfx_click.wav");
            var rewardClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/_Project/Audio/Clips/sfx_reward.wav");
            var bgmClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/_Project/Audio/Clips/bgm_combat_zen.wav");

            if (slashClip != null) audioSO.FindProperty("_sfxSlash").objectReferenceValue = slashClip;
            if (whooshClip != null) audioSO.FindProperty("_sfxWhoosh").objectReferenceValue = whooshClip;
            if (parryClip != null) audioSO.FindProperty("_sfxParryClang").objectReferenceValue = parryClip;
            if (hitClip != null) audioSO.FindProperty("_sfxHitImpact").objectReferenceValue = hitClip;
            if (teleClip != null) audioSO.FindProperty("_sfxTelegraphWarning").objectReferenceValue = teleClip;
            if (clickClip != null) audioSO.FindProperty("_sfxClick").objectReferenceValue = clickClip;
            if (rewardClip != null) audioSO.FindProperty("_sfxReward").objectReferenceValue = rewardClip;
            if (bgmClip != null) audioSO.FindProperty("_bgmGameplay").objectReferenceValue = bgmClip;
            audioSO.ApplyModifiedProperties();

            // Đảm bảo CombatVFXController được nạp đủ 4 sprite độc quyền vào serialized fields
            var vfxCtrl = Object.FindAnyObjectByType<CombatVFXController>();
            if (vfxCtrl == null)
            {
                var vfxObj = new GameObject("CombatVFXController");
                vfxCtrl = vfxObj.AddComponent<CombatVFXController>();
            }

            var vfxSO = new SerializedObject(vfxCtrl);
            var slashSp = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/Sprites/VFX_Slash.png");
            var dashSp = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/Sprites/Hero_Dash.png");
            var teleSp = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/Sprites/VFX_TelegraphMark.png");
            var burstSp = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/Sprites/VFX_CounterBurst.png");
            if (slashSp != null) vfxSO.FindProperty("_slashSprite").objectReferenceValue = slashSp;
            if (dashSp != null) vfxSO.FindProperty("_dashSprite").objectReferenceValue = dashSp;
            if (teleSp != null) vfxSO.FindProperty("_telegraphSprite").objectReferenceValue = teleSp;
            if (burstSp != null) vfxSO.FindProperty("_burstSprite").objectReferenceValue = burstSp;
            vfxSO.ApplyModifiedProperties();

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

            var heroSr = hero.GetComponentInChildren<SpriteRenderer>();
            if (heroSr != null && heroSr.sprite == null)
            {
                var ninjaSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/Sprites/Hero_Ninja.png");
                if (ninjaSprite != null) heroSr.sprite = ninjaSprite;
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

            var so = new SerializedObject(combatManager);
            var heroProp = so.FindProperty("_hero");
            if (heroProp != null) heroProp.objectReferenceValue = hero;
            var spawnProp = so.FindProperty("_enemySpawnPoint");
            if (spawnProp != null) spawnProp.objectReferenceValue = spawnPoint.transform;
            so.ApplyModifiedProperties();

            // 5. Khởi tạo / Cập nhật UI Canvas, Touch Zones & CombatHUD
            var canvasObj = GameObject.Find("Canvas");
            if (canvasObj == null)
            {
                canvasObj = new GameObject("Canvas");
                var canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                var scaler = canvasObj.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1080, 1920);
                scaler.matchWidthOrHeight = 0.5f;
                canvasObj.AddComponent<GraphicRaycaster>();
            }

            var hud = canvasObj.GetComponent<CombatHUD>();
            if (hud == null)
            {
                hud = canvasObj.AddComponent<CombatHUD>();
            }

            // Xóa bỏ 2 nút phẳng Cyan/Amber cũ nếu có
            var oldAtk = GameObject.Find("AttackButton");
            if (oldAtk != null) Object.DestroyImmediate(oldAtk);
            var oldDodge = GameObject.Find("DodgeButton");
            if (oldDodge != null) Object.DestroyImmediate(oldDodge);

            // 5.1. FULL-SCREEN ANTI-OVERFLOW TOUCH ZONES (EventTrigger trong suốt)
            // Nửa màn hình bên trái = Bắt chạm Né Đòn (Dodge)
            var dodgeZoneObj = GameObject.Find("DodgeTouchZone");
            if (dodgeZoneObj == null)
            {
                dodgeZoneObj = new GameObject("DodgeTouchZone");
                dodgeZoneObj.transform.SetParent(canvasObj.transform, false);
                dodgeZoneObj.AddComponent<Image>();
                dodgeZoneObj.AddComponent<EventTrigger>();
            }
            var dodgeImg = dodgeZoneObj.GetComponent<Image>();
            dodgeImg.color = Color.clear; // Hoàn toàn trong suốt
            dodgeImg.raycastTarget = true;
            var dodgeRect = dodgeZoneObj.GetComponent<RectTransform>();
            dodgeRect.anchorMin = new Vector2(0f, 0f);
            dodgeRect.anchorMax = new Vector2(0.5f, 1f); // Nửa màn hình bên trái toàn phần
            dodgeRect.offsetMin = Vector2.zero;
            dodgeRect.offsetMax = Vector2.zero;
            dodgeZoneObj.transform.SetSiblingIndex(0); // Nằm dưới các popup và top HUD
            var dodgeTrigger = dodgeZoneObj.GetComponent<EventTrigger>();

            // Nửa màn hình bên phải = Bắt chạm Phản Đòn (Attack)
            var attackZoneObj = GameObject.Find("AttackTouchZone");
            if (attackZoneObj == null)
            {
                attackZoneObj = new GameObject("AttackTouchZone");
                attackZoneObj.transform.SetParent(canvasObj.transform, false);
                attackZoneObj.AddComponent<Image>();
                attackZoneObj.AddComponent<EventTrigger>();
            }
            var atkImg = attackZoneObj.GetComponent<Image>();
            atkImg.color = Color.clear; // Hoàn toàn trong suốt
            atkImg.raycastTarget = true;
            var atkRect = attackZoneObj.GetComponent<RectTransform>();
            atkRect.anchorMin = new Vector2(0.5f, 0f);
            atkRect.anchorMax = new Vector2(1f, 1f); // Nửa màn hình bên phải toàn phần
            atkRect.offsetMin = Vector2.zero;
            atkRect.offsetMax = Vector2.zero;
            attackZoneObj.transform.SetSiblingIndex(1); // Nằm dưới các popup và top HUD
            var attackTrigger = attackZoneObj.GetComponent<EventTrigger>();

            // 5.2. THANH KIẾM HP BAR HÌNH NGANG Ở PHÍA TRÊN
            var swordBarObj = GameObject.Find("SwordHealthBar");
            if (swordBarObj == null)
            {
                swordBarObj = new GameObject("SwordHealthBar");
                swordBarObj.transform.SetParent(canvasObj.transform, false);
                var bgImage = swordBarObj.AddComponent<Image>();
                bgImage.color = new Color(0.12f, 0.16f, 0.23f, 0.85f); // #1E293B viền kiếm kim loại tối
                swordBarObj.AddComponent<Slider>();
            }
            var swordRect = swordBarObj.GetComponent<RectTransform>();
            swordRect.anchorMin = new Vector2(0.15f, 0.94f);
            swordRect.anchorMax = new Vector2(0.85f, 0.965f);
            swordRect.offsetMin = Vector2.zero;
            swordRect.offsetMax = Vector2.zero;

            var hpSlider = swordBarObj.GetComponent<Slider>();
            hpSlider.minValue = 0f;
            hpSlider.maxValue = 1f;
            hpSlider.value = 1f;

            // Fill Area
            var fillAreaObj = swordBarObj.transform.Find("Fill Area");
            if (fillAreaObj == null)
            {
                var faGo = new GameObject("Fill Area");
                faGo.transform.SetParent(swordBarObj.transform, false);
                var faRect = faGo.AddComponent<RectTransform>();
                faRect.anchorMin = Vector2.zero;
                faRect.anchorMax = Vector2.one;
                faRect.offsetMin = new Vector2(4f, 3f);
                faRect.offsetMax = new Vector2(-4f, -3f);
                fillAreaObj = faGo.transform;
            }
            var fillObj = fillAreaObj.Find("Fill");
            if (fillObj == null)
            {
                var fGo = new GameObject("Fill");
                fGo.transform.SetParent(fillAreaObj, false);
                var fImg = fGo.AddComponent<Image>();
                fImg.color = new Color(0.02f, 0.59f, 0.41f, 1f); // #059669 Ngọc Bích Zen Martial
                var fRect = fGo.GetComponent<RectTransform>();
                fRect.anchorMin = Vector2.zero;
                fRect.anchorMax = Vector2.one;
                fRect.offsetMin = Vector2.zero;
                fRect.offsetMax = Vector2.zero;
                fillObj = fGo.transform;
            }
            hpSlider.fillRect = fillObj.GetComponent<RectTransform>();
            hpSlider.targetGraphic = hpSlider.fillRect.GetComponent<Image>();

            // HP Text trên thanh kiếm
            var hpTextObj = swordBarObj.transform.Find("HPText");
            Text hpText = hpTextObj != null ? hpTextObj.GetComponent<Text>() : null;
            if (hpText == null)
            {
                var hptGo = new GameObject("HPText");
                hptGo.transform.SetParent(swordBarObj.transform, false);
                hpText = hptGo.AddComponent<Text>();
                var htRect = hptGo.GetComponent<RectTransform>();
                htRect.anchorMin = Vector2.zero;
                htRect.anchorMax = Vector2.one;
                htRect.offsetMin = Vector2.zero;
                htRect.offsetMax = Vector2.zero;
            }
            hpText.text = "HP: 100/100";
            hpText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            hpText.fontSize = 20;
            hpText.fontStyle = FontStyle.Bold;
            hpText.alignment = TextAnchor.MiddleCenter;
            hpText.color = new Color(1f, 1f, 1f, 0.95f);

            // 5.3. COMBO COUNTER TINH GỌN Ở PHÍA TRÊN
            var comboTextObj = GameObject.Find("ComboText");
            if (comboTextObj == null)
            {
                comboTextObj = new GameObject("ComboText");
                comboTextObj.transform.SetParent(canvasObj.transform, false);
                comboTextObj.AddComponent<Text>();
            }
            var comboText = comboTextObj.GetComponent<Text>();
            var comboRect = comboTextObj.GetComponent<RectTransform>();
            comboRect.anchorMin = new Vector2(0.2f, 0.88f);
            comboRect.anchorMax = new Vector2(0.8f, 0.93f);
            comboRect.offsetMin = Vector2.zero;
            comboRect.offsetMax = Vector2.zero;
            comboText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            comboText.fontSize = 34;
            comboText.fontStyle = FontStyle.Bold;
            comboText.alignment = TextAnchor.MiddleCenter;
            comboText.color = new Color(0.96f, 0.62f, 0.04f, 1f); // #F59E0B Vàng Kim
            comboText.text = "COMBO x2!";
            comboTextObj.SetActive(false); // Mặc định ẩn khi chưa có combo

            // 5.4. SCORE & WAVE HUD Ở GÓC TRÊN BÊN TRÁI
            var scoreObj = GameObject.Find("ScoreHUD");
            if (scoreObj == null)
            {
                scoreObj = new GameObject("ScoreHUD");
                scoreObj.transform.SetParent(canvasObj.transform, false);
                scoreObj.AddComponent<Text>();
            }
            var scoreText = scoreObj.GetComponent<Text>();
            var scRect = scoreObj.GetComponent<RectTransform>();
            scRect.anchorMin = new Vector2(0f, 1f);
            scRect.anchorMax = new Vector2(0f, 1f);
            scRect.pivot = new Vector2(0f, 1f);
            scRect.anchoredPosition = new Vector2(40f, -40f);
            scRect.sizeDelta = new Vector2(250f, 40f);
            scoreText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            scoreText.fontSize = 24;
            scoreText.fontStyle = FontStyle.Bold;
            scoreText.alignment = TextAnchor.MiddleLeft;
            scoreText.color = new Color(0.95f, 0.96f, 0.98f, 1f);
            scoreText.text = "SCORE: 0";

            var waveObj = GameObject.Find("WaveHUD");
            if (waveObj == null)
            {
                waveObj = new GameObject("WaveHUD");
                waveObj.transform.SetParent(canvasObj.transform, false);
                waveObj.AddComponent<Text>();
            }
            var waveText = waveObj.GetComponent<Text>();
            var wvRect = waveObj.GetComponent<RectTransform>();
            wvRect.anchorMin = new Vector2(0f, 1f);
            wvRect.anchorMax = new Vector2(0f, 1f);
            wvRect.pivot = new Vector2(0f, 1f);
            wvRect.anchoredPosition = new Vector2(40f, -85f);
            wvRect.sizeDelta = new Vector2(250f, 40f);
            waveText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            waveText.fontSize = 22;
            waveText.fontStyle = FontStyle.Bold;
            waveText.alignment = TextAnchor.MiddleLeft;
            waveText.color = new Color(0.22f, 0.74f, 0.97f, 1f); // #38BDF8
            waveText.text = "WAVE 1";

            // Nút SHOP CỬA HÀNG (Góc trên bên phải)
            var shopBtnObj = GameObject.Find("ShopButton");
            if (shopBtnObj == null)
            {
                shopBtnObj = new GameObject("ShopButton");
                shopBtnObj.transform.SetParent(canvasObj.transform, false);
                shopBtnObj.AddComponent<Button>();
                shopBtnObj.AddComponent<Image>();
            }
            var shopBtn = shopBtnObj.GetComponent<Button>();
            var shopImg = shopBtnObj.GetComponent<Image>();
            shopImg.color = new Color(0.96f, 0.62f, 0.04f, 0.9f); // #F59E0B
            var shopRect = shopBtnObj.GetComponent<RectTransform>();
            shopRect.anchorMin = new Vector2(1f, 1f);
            shopRect.anchorMax = new Vector2(1f, 1f);
            shopRect.pivot = new Vector2(1f, 1f);
            shopRect.anchoredPosition = new Vector2(-40f, -40f);
            shopRect.sizeDelta = new Vector2(180f, 65f);

            var shopTextObj = shopBtnObj.transform.Find("Label");
            Text shopText = shopTextObj != null ? shopTextObj.GetComponent<Text>() : null;
            if (shopText == null)
            {
                var labelGo = new GameObject("Label");
                labelGo.transform.SetParent(shopBtnObj.transform, false);
                shopText = labelGo.AddComponent<Text>();
                var tRect = labelGo.GetComponent<RectTransform>();
                tRect.anchorMin = Vector2.zero;
                tRect.anchorMax = Vector2.one;
                tRect.offsetMin = Vector2.zero;
                tRect.offsetMax = Vector2.zero;
            }
            shopText.text = "CỬA HÀNG 🛒";
            shopText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            shopText.fontSize = 22;
            shopText.fontStyle = FontStyle.Bold;
            shopText.alignment = TextAnchor.MiddleCenter;
            shopText.color = Color.white;

            // 6. Tạo Modal ShopPopup trong Canvas
            var shopPopupObj = GameObject.Find("ShopPopupPanel");
            if (shopPopupObj == null)
            {
                shopPopupObj = new GameObject("ShopPopupPanel");
                shopPopupObj.transform.SetParent(canvasObj.transform, false);
                var bgImg = shopPopupObj.AddComponent<Image>();
                bgImg.color = new Color(0.043f, 0.075f, 0.149f, 0.95f); // Obsidian Lacquer
                var spRect = shopPopupObj.GetComponent<RectTransform>();
                spRect.anchorMin = new Vector2(0.08f, 0.12f);
                spRect.anchorMax = new Vector2(0.92f, 0.88f);
                spRect.offsetMin = Vector2.zero;
                spRect.offsetMax = Vector2.zero;

                var shopPopup = shopPopupObj.AddComponent<ShopPopup>();

                // Tiêu đề Shop
                var titleObj = new GameObject("Title");
                titleObj.transform.SetParent(shopPopupObj.transform, false);
                var titleText = titleObj.AddComponent<Text>();
                titleText.text = "⚔️ CỬA HÀNG VÕ THUẬT (SHOP)";
                titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                titleText.fontSize = 28;
                titleText.fontStyle = FontStyle.Bold;
                titleText.alignment = TextAnchor.MiddleCenter;
                titleText.color = new Color(0.96f, 0.62f, 0.04f, 1f);
                var tRect = titleObj.GetComponent<RectTransform>();
                tRect.anchorMin = new Vector2(0f, 0.88f);
                tRect.anchorMax = new Vector2(1f, 1f);
                tRect.offsetMin = Vector2.zero;
                tRect.offsetMax = Vector2.zero;

                // Nút Đóng Shop (X)
                var closeBtnObj = new GameObject("CloseButton");
                closeBtnObj.transform.SetParent(shopPopupObj.transform, false);
                var closeBtn = closeBtnObj.AddComponent<Button>();
                var closeImg = closeBtnObj.AddComponent<Image>();
                closeImg.color = new Color(0.8f, 0.2f, 0.2f, 0.9f);
                var cRect = closeBtnObj.GetComponent<RectTransform>();
                cRect.anchorMin = new Vector2(1f, 1f);
                cRect.anchorMax = new Vector2(1f, 1f);
                cRect.pivot = new Vector2(1f, 1f);
                cRect.anchoredPosition = new Vector2(-20f, -20f);
                cRect.sizeDelta = new Vector2(60f, 50f);

                var closeTxtObj = new GameObject("Text");
                closeTxtObj.transform.SetParent(closeBtnObj.transform, false);
                var closeTxt = closeTxtObj.AddComponent<Text>();
                closeTxt.text = "X";
                closeTxt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                closeTxt.fontSize = 24;
                closeTxt.fontStyle = FontStyle.Bold;
                closeTxt.alignment = TextAnchor.MiddleCenter;
                closeTxt.color = Color.white;
                var ctRect = closeTxtObj.GetComponent<RectTransform>();
                ctRect.anchorMin = Vector2.zero;
                ctRect.anchorMax = Vector2.one;
                ctRect.offsetMin = Vector2.zero;
                ctRect.offsetMax = Vector2.zero;

                // Text Số Dư Vàng
                var coinsObj = new GameObject("CoinsBalance");
                coinsObj.transform.SetParent(shopPopupObj.transform, false);
                var coinsText = coinsObj.AddComponent<Text>();
                coinsText.text = "VÀNG: 0";
                coinsText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                coinsText.fontSize = 24;
                coinsText.fontStyle = FontStyle.Bold;
                coinsText.alignment = TextAnchor.MiddleCenter;
                coinsText.color = new Color(1f, 0.9f, 0.3f, 1f);
                var coinRect = coinsObj.GetComponent<RectTransform>();
                coinRect.anchorMin = new Vector2(0.1f, 0.76f);
                coinRect.anchorMax = new Vector2(0.9f, 0.85f);
                coinRect.offsetMin = Vector2.zero;
                coinRect.offsetMax = Vector2.zero;

                // Wire ShopPopup serialized fields
                var spSO = new SerializedObject(shopPopup);
                spSO.FindProperty("_closeButton").objectReferenceValue = closeBtn;
                spSO.FindProperty("_coinsBalanceText").objectReferenceValue = coinsText;
                spSO.ApplyModifiedProperties();

                shopPopupObj.SetActive(false); // Ẩn mặc định
            }

            var shopPopupComp = shopPopupObj.GetComponent<ShopPopup>();

            // Wire HUD properties
            var hudSO = new SerializedObject(hud);
            var cmProp = hudSO.FindProperty("_combatManager");
            if (cmProp != null) cmProp.objectReferenceValue = combatManager;
            var hProp = hudSO.FindProperty("_hero");
            if (hProp != null) hProp.objectReferenceValue = hero;

            var dtzProp = hudSO.FindProperty("_dodgeTouchZone");
            if (dtzProp != null) dtzProp.objectReferenceValue = dodgeTrigger;
            var atzProp = hudSO.FindProperty("_attackTouchZone");
            if (atzProp != null) atzProp.objectReferenceValue = attackTrigger;

            var hsProp = hudSO.FindProperty("_healthSlider");
            if (hsProp != null) hsProp.objectReferenceValue = hpSlider;
            var htProp = hudSO.FindProperty("_healthText");
            if (htProp != null) htProp.objectReferenceValue = hpText;
            var cbProp = hudSO.FindProperty("_comboText");
            if (cbProp != null) cbProp.objectReferenceValue = comboText;
            var scProp = hudSO.FindProperty("_scoreText");
            if (scProp != null) scProp.objectReferenceValue = scoreText;
            var wvProp = hudSO.FindProperty("_waveText");
            if (wvProp != null) wvProp.objectReferenceValue = waveText;

            var sbProp = hudSO.FindProperty("_shopButton");
            if (sbProp != null) sbProp.objectReferenceValue = shopBtn;
            var spProp = hudSO.FindProperty("_shopPopup");
            if (spProp != null) spProp.objectReferenceValue = shopPopupComp;
            hudSO.ApplyModifiedProperties();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();

            Debug.Log("[SceneSetupHelper] Hoàn tất thiết lập Game.unity cho ShadowReflex Version 2!");
        }
    }
}
