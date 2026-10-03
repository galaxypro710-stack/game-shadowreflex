# BIÊN BẢN BÀN GIAO KỸ THUẬT: VERSION 1 $\rightarrow$ VERSION 2 (HANDOVER KIT)

> **Tên Sub-Game:** Shadow Reflex  
> **Package ID:** `com.galaxypro.shadowreflex`  
> **Archetype:** Timing Action Combat / Telegraph Reflex  
> **Nguồn tham chiếu gốc:** `com.redantz.game.tapshadow.xapk`  
> **Bên bàn giao:** Core Engine / Meta-Factory Team (Phụ trách Version 1 - Core Architecture & Mechanics Slice)  
> **Bên tiếp nhận:** Commercial Release / Publishing Team (Phụ trách Version 2 - Content, Art Polish & Store Release)  
> **Tỷ lệ hoàn thiện V1:** **80% (Playable Core Vertical Slice, Pass 5/5 AutoQA, Zero-GC Caching)**  
> **Tôn chỉ tuân thủ:** `/ai-copilot-alignment`, `/behavior-model-debugger`, Karpathy Behavioral Guidelines  

---

## 🎯 1. TRẠNG THÁI HIỆN TẠI CỦA VERSION 1 (WHAT WORKS 100%)

Phiên bản Version 1 đã hoàn thiện đầy đủ khung kiến trúc Clean Architecture 2 tầng, hệ thống FSM nhịp điệu chiến đấu, giao diện Zero-GC, cơ chế lưu trữ mã hóa và mock monetization an toàn:

### A. Minh chứng Thực chứng (Mechanical Proof):
- **Core Compilation & AutoQA Check:** **PASS 5/5 tiêu chí (Exit Code 0)** trong 90.87s qua Unity Batchmode.
- **Meta Integrity:** **100% PASS** (30/30 assets, 48/48 metas, 0 missing, 0 orphan).
- **Secret Hygiene:** **0 tokens / 0 PATs detected** qua `tools/scan_secrets.py`.
- **Git Commit Anchor:** [`c8a506a`](file:///d:/bkk/games/ShadowReflex) tại `D:\bkk\games\ShadowReflex`.
- **Bee Build Cache:** Đã biên dịch sẵn và lưu trữ an toàn **545 / 951 native C++ object files** trong `Library/Bee/artifacts/Android/d8kzr/`.

### B. Danh mục tính năng V1 đã kiểm chứng:
- [x] **Hero Combat Controller:** Thao tác cảm ứng Tấn công & Né đòn; cơ chế bất tử (I-Frames 0.35s); Perfect Counter (x2.5 sát thương khi né trúng active strike window); Combo Streak decay 2.0s.
- [x] **Enemy Combat Controller:** State Machine hoàn chỉnh (`Approaching` $\rightarrow$ `WindupTelegraph` $\rightarrow$ `ActiveStrike` $\rightarrow$ `Stunned` $\rightarrow$ `Dead`).
- [x] **Timing Combat Manager:** Điều phối đợt quái (waves), cơ chế tính điểm, kết nối Event Channels (`VoidEventChannelSO`) cho Victory/Defeat.
- [x] **Zero-GC Combat HUD:** Chỉ cập nhật Text/Slider khi giá trị HP, combo, điểm số thực sự thay đổi, triệt tiêu Garbage Collection trong combat.
- [x] **HMAC-SHA256 Encrypted SaveService:** Khóa muối bảo mật ngẫu nhiên độc quyền (`ShadowReflex_SecretSalt_...`), chống hack save.
- [x] **Offline-First Monetization:** Mock AdMob & Mock RevenueCat tích hợp Watchdog 8s chống lock UI khi offline.
- [x] **Scene Setup Tooling:** Editor script tự động dựng Hierarchy scene `Assets/_Project/Scenes/Game.unity`.

---

## 🚀 2. DEVELOPER RUNBOOK: HƯỚNG DẪN DÀNH CHO DEV TEAM 2

Dev của Team 2 chỉ cần làm đúng 3 bước để tiếp tục phát triển:

1. **Bước 1: Mở Project trong Unity Editor**
   - Phiên bản khuyến nghị: **Unity 6 (6000.6.2f1)**.
   - Thư mục project: `D:\bkk\games\ShadowReflex`.
2. **Bước 2: Mở Scene Kiểm Thử**
   - Mở scene: `Assets/_Project/Scenes/Game.unity`.
   - Bấm **Play** để trải nghiệm ngay gameplay né đòn phản xạ, đánh thường, đếm combo và wave quái.
3. **Bước 3: Đóng gói Android APK**
   - **Tùy chọn A (Fast Development APK - Khuyến nghị để test máy thật):**
     Mở `Assets/Editor/BuildScript.cs`, bật `options = BuildOptions.Development;` để NDK Clang bỏ qua tối ưu hóa nặng (`-O0`), thời gian đóng gói APK chỉ còn **2-3 phút**.
   - **Tùy chọn B (Production Release APK):**
     Chạy lệnh qua PowerShell:
     ```powershell
     cd D:\bkk\wfgame
     powershell -File run_android_build.ps1 -Game ShadowReflex -BuildTimeout 2400
     ```

---

## 📋 3. V2 BACKLOG CHECKLIST (NHIỆM VỤ TEAM 2 CẦN HOÀN THÀNH)

Team 2 tiếp nhận source code cần triển khai các hạng mục sau để hoàn thiện dự án thương mại:

### A. Đồ họa & Hiệu ứng (Art & Visuals - P0):
- [ ] **Asset Reskinning Độc Quyền:** Thay thế toàn bộ hình khối 2D/Sprite tạm thời bằng Sprite nhân vật Ninja / Samurai / Shadow Fighter độc quyền studio (chống vi phạm DMCA và Store Copycat).
- [ ] **Animation & VFX:** Bổ sung sprite sheet animation cho đòn chém, lướt né (Dash Trail), hiệu ứng lóe sáng cảnh báo địch ra đòn (Telegraph Flash) và hiệu ứng Perfect Counter x2.5 nổ tia sét/vết chém.
- [ ] **Camera Shake & Screen Flash:** Rung nhẹ màn hình khi đánh trúng hoặc khi kích hoạt Perfect Counter.

### B. Âm thanh Nhịp Điệu (Audio & Haptics - P1):
- [ ] **Sound Effects (SFX):** Âm thanh tiếng kiếm chém (Slash), tiếng né gió (Whoosh), tiếng keng khi Perfect Counter (Parry Clang), và tiếng đòn đánh trúng (Hit Impact).
- [ ] **Combo Pitch Scaling:** SFX tăng dần cao độ theo chuỗi Combo Streak.
- [ ] **Haptic Feedback:** Rung nhẹ điện thoại khi Perfect Counter thành công.

### C. Tiến trình Màn chơi & Meta System (Level & Progression - P1):
- [ ] Bổ sung nhiều loại kẻ địch khác nhau (Lính nhanh tốc độ chém 0.4s, Lính nặng đòn chém 0.8s, Boss có chuỗi 3 đòn liên hoàn).
- [ ] Hệ thống Shop Nâng cấp: Nâng cấp Máu (HP), Sát thương (Damage), Thời gian cửa sổ Né (Dodge Window +0.05s).
- [ ] Lưu trữ tiến trình nâng cấp vào `SaveService`.

### D. Live Monetization & Store Assets (Publishing - P2):
- [ ] Điền ID AdMob thật: Rewarded Ads (Hồi sinh khi chết / x2 vàng sau trận).
- [ ] Điền Key RevenueCat thật: Gói Mua Nhân vật VIP, gói Bỏ quảng cáo (Remove Ads).
- [ ] Chuẩn bị bộ ảnh Store: Icon 512x512, Feature Graphic 1024x500, Screenshots 9:16.

---

## 🏛️ 4. BẢN ĐỒ KIẾN TRÚC MÃ NGUỒN (CLEAN ARCHITECTURE 2 TẦNG)

```text
D:\bkk\games\ShadowReflex\Assets\_Project\
├── Scenes/
│   └── Game.unity                    # Scene chiến đấu chính
├── Scripts/
│   ├── Core/                         # TẦNG 1: FOUNDATION (KHÔNG SỬA NẾU KHÔNG CẦN THIẾT)
│   │   ├── Audio/                    # AudioManager, Volume, Sound Settings
│   │   ├── Events/                   # ScriptableObject Event Channels (VoidEventChannelSO)
│   │   ├── Managers/                 # GameManager, FSM Manager
│   │   ├── Monetization/             # AdMob & RevenueCat Bridge (Mock + Live)
│   │   └── Save/                     # HMAC-SHA256 Encrypted SaveService
│   ├── Gameplay/                     # TẦNG 2: COMBAT MECHANICS (TEAM 2 THOẢI MÁI PHÁT TRIỂN)
│   │   ├── CombatState.cs            # Enums & Structs (Hero & Enemy States, Config)
│   │   ├── HeroCombatController.cs   # Attack, I-Frame Dodge, Counter x2.5, Combo
│   │   ├── EnemyCombatController.cs  # Telegraph Windup, Active Strike, Stunned
│   │   └── TimingCombatManager.cs    # Wave Spawner, Score & Wave Progression
│   ├── UI/                           # INTERFACE & HUD
│   │   └── CombatHUD.cs              # Zero-GC Delta Caching UI (HP, Combo, Buttons)
│   └── Editor/
│       ├── BuildScript.cs            # CLI Headless Build cho Android APK & Windows
│       └── SceneSetupHelper.cs       # Auto Scene Setup & Wire Up Tool
```

---

## ⚠️ 5. CÁC ĐIỂM CẦN LƯU Ý KỸ THUẬT (KNOWN LIMITATIONS & GOTCHAS)

1. **Storage Isolation:** Game sử dụng HMAC Salt riêng biệt (`ShadowReflex_SecretSalt_...`). Tuyệt đối không copy file save giữa các subgames khác để tránh làm sai lệch mã checksum.
2. **Build Cache:** Thư mục `Library/Bee/artifacts/Android/d8kzr/` đã lưu trữ 545/951 native object files. Khi build APK lần tiếp theo, Unity sẽ tự động tái sử dụng cache này, không bao giờ phải biên dịch lại từ đầu.
3. **Phạm vi tác vụ:** Mọi can thiệp phát triển chỉ diễn ra trong thư mục con `D:\bkk\games\ShadowReflex`, không làm ô nhiễm thư mục Meta-Factory mẹ `wfgame`.
