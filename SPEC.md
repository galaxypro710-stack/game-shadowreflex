# SPEC.md: ShadowReflex SYSTEM SPECIFICATION & ARCHITECTURE

> **Dự án:** ShadowReflex (Unity 6 / C# Clean Architecture 2 Tầng)  
> **Package ID:** `com.galaxypro.shadowreflex`  
> **Archetype:** Timing Action Combat / Telegraph Reflex  
> **Tôn chỉ tuân thủ:** `/harness-engineering`, `/ai-copilot-alignment`, `/karpathy-guidelines`  

---

## 1. TỔNG QUAN KIẾN TRÚC & PHÂN TẦNG HỆ THỐNG
Dự án được xây dựng theo mô hình **2-Tier Clean Architecture**:

### Tầng 1: Universal Foundation Core (Bất biến)
- **Game State Machine (`GameManager.cs`, `GameMaster.cs`):** Quản lý trạng thái vòng đời game (`Boot`, `MainMenu`, `Playing`, `Paused`, `GameOver`, `Victory`).
- **Save Service (`SaveService.cs`):** Lưu trữ mã hóa HMAC-SHA256 chống can thiệp file save (`ShadowReflex_SecretSalt_a2abd944225b40c79ab61956154f42d9_v1`). Hỗ trợ trường `customPayload` JSON cho dữ liệu nâng cấp.
- **Event-Driven SO Channels (`VoidEventChannelSO`, `GameStateEventChannelSO`):** Giao tiếp phi tập trung (Decoupled), không phụ thuộc tham chiếu chéo.
- **Audio Core (`AudioManager.cs`):** BGM & SFX Engine độc lập, hỗ trợ Combo Pitch Scaling và WebGL First-Gesture Watchdog.
- **Offline-First Monetization:** Mock AdMob & Mock RevenueCat với Watchdog 8.0s chống đơ giao diện khi mất mạng; cung cấp sẵn toggle `UseLiveProduction`.

### Tầng 2: Gameplay Combat Archetype (Timing Action Reflex)
- **Hero Controller (`HeroCombatController.cs`):** 
  - Cơ chế 1-Tap Attack & Dodge (I-Frames 0.35s).
  - Perfect Counter (x2.5 sát thương khi né trúng active strike window).
  - Combo Streak Decay (2.0s).
  - Nạp chỉ số nâng cấp từ `HeroUpgradeManager`.
- **Enemy Controller (`EnemyCombatController.cs`):** 
  - FSM 5 trạng thái: `Approaching` $\rightarrow$ `WindupTelegraph` $\rightarrow$ `ActiveStrike` $\rightarrow$ `Stunned` $\rightarrow$ `Dead`.
  - Hỗ trợ Boss Triple-Strike Combo (3 nhịp chém liên hoàn dồn dập).
- **Combat Spawner & Scoring (`TimingCombatManager.cs`):** 
  - Điều phối 4 loại quái: Ninja Scout, Swift Assassin (chém 0.4s), Iron Brute (búa nặng chém 0.8s), Boss Shogun.
  - Tích hợp thưởng Coins và Score khi vượt Wave.
- **VFX & Camera Shake (`CombatVFXController.cs`, `CameraShakeController.cs`):**
  - Hiệu ứng Slash, Ghost Dash Trail, Telegraph Warning Mark, Counter Burst Spark, và Zero-GC Camera Shake.
- **Zero-GC HUD (`CombatHUD.cs`):** Cache delta giá trị HP, combo, score, wave, triệt tiêu Garbage Collection trong combat loop.

---

## 2. STORE DEFENSE & PHÒNG THỦ BẢN QUYỀN
1. **100% Studio-Exclusive Art & Audio:** Không sử dụng bất kỳ asset rip nào từ bên ngoài. Toàn bộ Sprite nhân vật và file âm thanh SFX .WAV đều được tổng hợp thủ tục (procedural synthesis) độc quyền.
2. **Keystore Cô Lập:** Keystore Android riêng biệt lưu tại `keystore/ShadowReflex.keystore` (RSA 2048-bit, SHA-256).
3. **Secret Hygiene:** 0 token/credential cứng trong code; toàn bộ thông tin nhạy cảm lưu trong `.env.local` (được `.gitignore` bảo vệ).
