# GOAL.md: KẾ HOẠCH & TIÊU CHUẨN NGHIỆM THU SHADOWREFLEX

> **Mục tiêu:** Tái tạo thành công Core Gameplay Loop & Xuất xưởng Android Release APK của ShadowReflex  
> **Tiến độ Thực tế Hiện tại:** **92% (Core Loop, Art, Audio, Windows & Android APK Đã Chạy Thực Tế Trên Thiết Bị)**  
> **Nguyên tắc:** Evidence-First, 0 Console Errors, AutoQA PASS 100%, Build Pass Exit Code 0  
> **Áp dụng:** `/harness-engineering`, `/ai-copilot-alignment`, `/karpathy-guidelines`  

---

## 📊 1. MA TRẬN ĐỐI CHIẾU TIẾN ĐỘ THỰC TẾ (REALITY AUDIT MATRIX)

### A. Core Version 1 (Đã Kiểm Chứng Thực Tế 100%):
- [x] **Milestone 1: Khởi Tạo & Nạp Cấu Hình Màn Chơi**
  - Scene `Assets/_Project/Scenes/Game.unity` nạp đủ Core Systems (GameManager, AudioManager, SaveService, EventSystem, Camera, CombatManager).
  - Tích hợp 100% Art & Audio độc quyền studio (không dính DMCA).
  - Kiểm chứng cơ học: `AutoQACheck` -> **PASS 8/8 (Exit Code: 0)**.
- [x] **Milestone 2: Triển Khai Gameplay Mechanics & Combat Loop**
  - FSM Hero & Enemy FSM theo Archetype `timing_action_combat`.
  - Cơ chế I-Frames 0.35s né đòn, Perfect Counter x2.5, Combo Streak Decay 2.0s.
  - Zero-GC Combat HUD delta caching triệt tiêu garbage collection.
  - Quái mới: Quái nhanh chém 0.4s, Quái búa nặng chém 0.8s, Boss 3 nhịp chém liên hoàn.
  - Shop nâng cấp Stats (HP, Damage, Dodge) lưu an toàn vào `SaveService` qua HMAC-SHA256.
- [x] **Milestone 3: Nghiệm Thu Tự Động & Đóng Gói Xuất Xưởng**
  - [x] AutoQA Check 8/8 tiêu chí PASS 100% (56.35s batchmode).
  - [x] Kiểm tra tính toàn vẹn `.meta` (48 assets, 70 metas, 0 missing, 0 orphan).
  - [x] Quét sạch secret (0 token rò rỉ trên 156 files).
  - [x] Sinh Keystore riêng biệt: `keystore/ShadowReflex.keystore` (2,756 bytes, RSA 2048-bit).
  - [x] Biên dịch Standalone Windows: `Builds/Windows/ShadowReflex.exe` (667 KB, Exit Code 0).
  - [x] Biên dịch Android APK: `Builds/Android/ShadowReflex.apk` (35.9 MB, Exit Code 0).
  - [x] Cài đặt & Khởi chạy thành công trên LDPlayer 9 Android 14 (`127.0.0.1:5555`).
  - [x] Trích xuất ảnh chụp màn hình máy thật: `Builds/shadowreflex_device.png`.

---

### B. Hoãn Sang Version 2 (Hậu kỳ Store & Live Monetization):
- [ ] Thay AdMob App ID và Unit ID Live thực tế vào `.env.local`.
- [ ] Cấu hình RevenueCat Public API Key thực tế vào `.env.local`.
- [ ] Thiết kế bộ ảnh Store: Icon 512x512, Feature Graphic 1024x500, Screenshots 9:16.
- [ ] Nộp bản build lên Google Play Console (Closed Testing Track).

---

## 🎯 2. DANH SÁCH VIỆC CÒN THIẾU ĐỂ HOÀN THIỆN TRẢI NGHIỆM V1 (MISSING GAPS)

Dựa trên ảnh chụp màn hình thực tế trên thiết bị (`shadowreflex_device.png`), 3 việc cốt tử còn thiếu để chốt hạ 100% trải nghiệm V1:

1. **[Gap 1 - Thẩm mỹ Touch Controls (UI Touch Zones)]:** Hai vùng bấm NÉ và TẤN CÔNG dưới màn hình hiện là 2 mảng màu đặc (Xanh lam / Đỏ bầm). Cần chuyển sang dạng bán trong suốt viền bo tròn bo góc (Pill/Rounded) chuẩn theo Design System Stitch MCP và gắn nhãn Text rõ ràng ("NÉ ĐÒN", "PHẢN ĐÒN").
2. **[Gap 2 - Nút Mở Shop Nâng Cấp trên HUD]:** Hiện tại `ShopPopup.cs` và `HeroUpgradeManager.cs` đã code hoàn chỉnh nhưng chưa có nút bấm "SHOP / NÂNG CẤP" trên `CombatHUD` để người chơi mở giao diện tiêu vàng.
3. **[Gap 3 - Visual Feedback Vết Chém & Lóe Sáng]:** Đảm bảo khi bấm Tấn công hoặc né đòn thành công, `VFX_Slash` và `VFX_CounterBurst` hiển thị đúng vị trí Transform của Hero/Enemy trên màn hình.
