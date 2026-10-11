# PROJECT CONTEXT & LIVING SPEC: ShadowReflex

- **Project:** ShadowReflex
- **Package Name:** com.galaxypro.shadowreflex
- **Archetype:** timing_action_combat
- **Status:** V1_CLEANED_AND_STANDARDIZED (Zero Cross-Pollution, 100% Brand Isolated)
- **Updated:** 2026-10-11T22:05:00+07:00
- **Governing Skills:** `/ai-copilot-alignment`, `/behavior-model-debugger`, `/vibe-git-manager`, `/karpathy-guidelines`
- **Rollback Anchor:** `6dc3ead` (Branch `main` - Pre-Hygiene & Cross-Pollution Cleanup)

---

## 🏛️ 1. HỒ SƠ KỸ THUẬT & TRẠNG THÁI HIỆN TẠI
- **Kiến trúc:** 2-Tier C# Clean Architecture (Core Services + Feature Plugins)
- **Root Namespace:** `GalaxyPro.ShadowReflex`
- **Save Service Salt:** `ShadowReflex_SecretSalt_a2abd944225b40c79ab61956154f42d9_v1`
- **Keystore:** `keystore/ShadowReflex.keystore`
- **Tài nguyên Staging:** 19 files (13 Sprites, 6 WAVs - Đã chuẩn hóa quy chuẩn kích thước/pivot)
- **Dọn sạch Blind Spots (Completed):**
  - Đã chuyển đổi 100% PlayerPrefs key sang tiền tố `ShadowReflex_`.
  - Đã chuẩn hóa IAP Product IDs: `shadowreflex.noads`, `shadowreflex.boosters.small`.
  - Đã thay thế seed data Ai Cập bằng võ sĩ bóng đêm (Shadow Warriors).
- **Artifacts:**
  - Standalone: `Builds/Windows/ShadowReflex.exe`
  - Android APK: `Builds/Android/ShadowReflex.apk`

---

## 🛡️ 2. QUY TẮC BẢO MẬT & VỆ SINH MÃ NGUỒN
- Toàn bộ secret thực tế (Keystore password, API Keys) nằm trong `.env.local` (được `.gitignore` bảo vệ).
- Nghiệm thu bắt buộc xuất trình Terminal Log với Exit Code 0 trước khi commit.
