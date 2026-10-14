# GOAL.md: KẾ HOẠCH & TIÊU CHUẨN NGHIỆM THU ShadowReflex

> **Mục tiêu:** Tái tạo thành công Core Gameplay Loop & Xuất xưởng Android Release APK của ShadowReflex  
> **Nguyên tắc:** Evidence-First, 0 Console Errors, AutoQA PASS 100%, Build Pass Exit Code 0  
> **Áp dụng:** `/ai-copilot-alignment`, `/behavior-model-debugger`, `/harness-engineering`  

---

## 🎯 DANH SÁCH MILESTONES CỐT LÕI (CORE V1 - 100% PASS)

- [x] **Milestone 1: Khởi Tạo & Nạp Cấu Hình Màn Chơi**
  - Verify Scene `Assets/_Project/Scenes/Game.unity` nạp đủ Core Systems (GameManager, AudioManager, SaveService, ThemeManager).
  - Khởi tạo thư mục Data, load cấu hình JSON/CSV độc quyền.
  - Kiểm chứng: `python tools/run_unity.py -g ShadowReflex -m Game.Editor.BuildScript.AutoQACheck` -> Exit Code 0.

- [x] **Milestone 2: Triển Khai Gameplay Mechanics & Input Interaction**
  - Thi công FSM Gameplay State Machine độc quyền theo Archetype `timing_action_combat`.
  - Cơ chế chạm/vuốt/kéo thả được clamp an toàn, có timeout watchdog chống kẹt tương tác.
  - Tích hợp hiệu ứng âm thanh và hạt Particle FX.

- [x] **Milestone 3: Nghiệm Thu Tự Động & Đóng Gói Xuất Xưởng (Release Packaging)**
  - AutoQA Check 5/5 tiêu chí PASS 100%.
  - Biên dịch Standalone Windows: `Builds/Windows/ShadowReflex.exe`.
  - Sinh Keystore riêng & biên dịch Android Release APK: `Builds/Android/ShadowReflex.apk` (**46.20 MB**).
  - Kiểm tra tính toàn vẹn `.meta` (zero orphan) và quét sạch secret (0 token rò rỉ).
