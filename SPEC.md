# SPEC.md: ShadowReflex SPECIFICATION

> **Dự án:** ShadowReflex (Unity 6 / C# Clean Architecture)  
> **Package ID:** `com.galaxypro.shadowreflex`  
> **Archetype:** `timing_action_combat`  
> **Tôn chỉ tối thượng:** `/ai-copilot-alignment`, `/behavior-model-debugger`, `/karpathy-guidelines`, `/vibe-git-manager`  

---

## 1. TỔNG QUAN KIẾN TRÚC & HỆ THỐNG (SYSTEM OVERVIEW)
- **Core Loop:** Vòng lặp gameplay cốt lõi độc quyền của ShadowReflex (1-Tap Reflex & Telegraph Counter).
- **Kiến trúc:** 2-Tier Clean Architecture thừa hưởng từ Meta-Factory Pure Engine Template.
- **Root Namespace:** `GalaxyPro.ShadowReflex` (Phân hóa mã nguồn chống quét Store).
- **Monetization:** Dual-Mode Offline-First:
  - **In-App Purchase (`IIAPService.cs`):** Mock Purchases trên Editor/Desktop; RevenueCat Purchases SDK trên Android Release.
  - **Google Mobile Ads (`IAdService.cs`):** Mock Ad với Watchdog Timer 8.0s chống treo state; Live AdMob IDs khi phát hành.
- **Bảo mật Lưu trữ (Storage Isolation):** HMAC-SHA256 Encrypted `SaveService.cs` với Salt độc quyền: `ShadowReflex_SecretSalt_a2abd944225b40c79ab61956154f42d9_v1`.
- **Archetype Recipe Tham Chiếu:** `D:\bkk\wfgame\docs\archetypes\timing_action_combat\RECIPE.md` (AI đọc recipe để sinh mã nguồn C# độc quyền, không copy tên class cũ).

---

## 2. QUY CHUẨN CÁCH LY & ANTI-BAN STORE DEFENSE
1. **Zero Authentic Assets on Store:** Mọi asset trích xuất từ APK gốc chỉ phục vụ R&D nội bộ. Bản release thương mại bắt buộc 100% Art độc quyền.
2. **Keystore Độc Lập:** Keystore Android riêng biệt được lưu tại `keystore/ShadowReflex.keystore` (RSA 2048-bit, SHA-256).
3. **PlayerPrefs Isolation:** Mọi key lưu trữ bắt buộc mang tiền tố `ShadowReflex_`.
4. **Secret Hygiene:** Credential thực tế chỉ lưu tại `.env.local` (nằm trong `.gitignore`).
