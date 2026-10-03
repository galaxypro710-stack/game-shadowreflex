# SPEC.md: ShadowReflex SPECIFICATION

> **Dá»± Ã¡n:** ShadowReflex (Unity 6 / C# Clean Architecture)  
> **Package ID:** com.galaxypro.shadowreflex  
> **Archetype:** timing_action_combat  
> **TÃ´n chá»‰ tá»‘i thÆ°á»£ng:** `/ai-copilot-alignment`, `/behavior-model-debugger`, `/karpathy-guidelines`, `/vibe-git-manager`  

---

## 1. Tá»”NG QUAN KIáº¾N TRÃšC & Há»† THá»NG (SYSTEM OVERVIEW)
- **Core Loop:** VÃ²ng láº·p gameplay cá»‘t lÃµi Ä‘á»™c quyá»n cá»§a ShadowReflex.
- **Kiáº¿n trÃºc:** 2-Tier Clean Architecture thá»«a hÆ°á»Ÿng tá»« Meta-Factory Pure Engine Template.
- **Root Namespace:** `GalaxyPro.ShadowReflex` (PhÃ¢n hÃ³a mÃ£ nguá»“n chá»‘ng quÃ©t Store).
- **Monetization:** Dual-Mode Offline-First:
  - **In-App Purchase (`IIAPService.cs`):** Mock Purchases trÃªn Editor/Desktop; RevenueCat Purchases SDK trÃªn Android Release.
  - **Google Mobile Ads (`IAdService.cs`):** Mock Ad vá»›i Watchdog Timer 8.0s chá»‘ng treo state; Live AdMob IDs khi phÃ¡t hÃ nh.
- **Báº£o máº­t LÆ°u trá»¯ (Storage Isolation):** HMAC-SHA256 Encrypted `SaveService.cs` vá»›i Salt Ä‘á»™c quyá»n: `ShadowReflex_SecretSalt_a2abd944225b40c79ab61956154f42d9_v1`.
- **Archetype Recipe Tham Chieu:** D:\bkk\wfgame\docs\archetypes\timing_action_combat\RECIPE.md (AI doc recipe de sinh ma nguon C# doc quyen, khong copy ten class cu).

---

## 2. QUY CHUáº¨N CÃCH LY & ANTI-BAN STORE DEFENSE
1. **Zero Authentic Assets on Store:** Má»i asset trÃ­ch xuáº¥t tá»« APK gá»‘c chá»‰ phá»¥c vá»¥ R&D ná»™i bá»™. Báº£n release thÆ°Æ¡ng máº¡i báº¯t buá»™c 100% Art Ä‘á»™c quyá»n.
2. **Keystore Äá»™c Láº­p:** Keystore Android riÃªng biá»‡t Ä‘Æ°á»£c lÆ°u táº¡i `keystore/ShadowReflex.keystore` (RSA 2048-bit, SHA-256).
3. **PlayerPrefs Isolation:** Má»i key lÆ°u trá»¯ báº¯t buá»™c mang tiá»n tá»‘ `ShadowReflex_`.
4. **Secret Hygiene:** Credential thá»±c táº¿ chá»‰ lÆ°u táº¡i `.env.local` (náº±m trong `.gitignore`).

