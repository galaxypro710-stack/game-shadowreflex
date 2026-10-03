# PROJECT CONTEXT & LIVING SPEC: ShadowReflex

- **Project:** ShadowReflex
- **Package Name:** com.galaxypro.shadowreflex
- **Archetype:** timing_action_combat
- **Status:** VERSION 2 COMMERCIAL & STORE RELEASE COMPLETE (Touch Zones Anti-Overflow, AdMob SDK, RevenueCat IAP, Supabase Leaderboard)
- **Updated:** 2026-10-03T22:30:00+07:00
- **Governing Skills:** `/ai-copilot-alignment`, `/behavior-model-debugger`, `/vibe-git-manager`, `/karpathy-guidelines`
- **Rollback Anchor:** `v2_store_ready` (Branch `master`)

---

## 🎯 1. MINH CHỨNG THỰC CHỨNG (MECHANICAL PROOF)
- **AutoQACheck (Unity CLI Batchmode):** **PASS 8/8 tiêu chí (Exit Code: 0)**
- **Meta Integrity:** **100% PASS** (70 assets, 97 .metas, 0 missing, 0 orphan)
- **Secret Hygiene:** **0 tokens detected** trên 417 files
- **Save Service Salt:** `ShadowReflex_SecretSalt_a2abd944225b40c79ab61956154f42d9_v1`
- **Keystore:** `keystore/ShadowReflex.keystore`
- **Android APK Xuất Xưởng:** `Builds/Android/ShadowReflex.apk` (Exit Code: 0)
- **Thiết Bị Kiểm Chứng (ADB):** LDPlayer 9 - Android 14 (`emulator-5554` / `127.0.0.1:5555`)
- **Stitch MCP Design System:** Theme `Zen Jade Martial` (Project `projects/7803309454515022893`)
- **Living Spec & PR Notes:** [implementation_notes.html](file:///D:/bkk/games/ShadowReflex/implementation_notes.html)

---

## 🏛️ 2. KIẾN TRÚC & HẠ TẦNG V2 RELEASE
- **Core (Tầng 1):** Giữ nguyên vẹn tính độc lập, SaveService HMAC-SHA256, Zero-GC Combat HUD.
- **Gameplay (Tầng 2):**
  - **Natural Touch Zones (Anti-Overflow):** Bắt chạm trong suốt toàn màn hình (Nửa trái = Né đòn, Nửa phải = Phản đòn) qua EventTrigger PointerDown phản xạ tức thì.
  - **Sword HP Bar & Combo HUD:** Thanh máu hình kiếm ngang đỉnh màn hình viền kim loại tối, ngọc bích zen martial, combo counter tinh gọn.
  - **AdMob SDK Adapter:** RewardedAd.Load & Show native từ Google Mobile Ads SDK kèm mock fallback offline-first.
  - **RevenueCat IAP Adapter:** Purchases.Configure & Purchases.PurchasePackage cho `shadowreflex.noads` và `shadowreflex.coins.starter` (1,000 vàng).
  - **Supabase Leaderboard:** Schema chuẩn `id, gamertag, score, wave, created_at` kèm SQL migration `supabase_schema.sql` có RLS policies.
