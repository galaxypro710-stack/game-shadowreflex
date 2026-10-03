# AGENTS.md: SUB-GAME REPOSITORY HARNESS & AUTHORITY BOUNDARY

> **Sub-Game:** ShadowReflex  
> **Package ID:** {{PACKAGE_ID}}  
> **Archetype:** {{ARCHETYPE}}  
> **Kiáº¿n trÃºc:** 2-Tier Clean Architecture (Decoupled Game Workspace)  
> **Cá»•ng vÃ o tháº©m quyá»n tá»‘i thÆ°á»£ng:** Khá»Ÿi Ä‘áº§u má»i tÃ¡c vá»¥ tá»« tÃ i liá»‡u nÃ y, sau Ä‘Ã³ Ä‘á»c `HANDOVER_V1_TO_V2.md`, `SPEC.md`, `GOAL.md` vÃ  `CONTEXT.md`.  

---

## ðŸ›ï¸ 1. NGUYÃŠN Táº®C HOáº T Äá»˜NG Cá»T LÃ•I (CORE INVARIANTS)

1. **Con NgÆ°á»i lÃ  Tá»•ng Chá»‰ Huy (Human Commander Protocol):**  
   Äáº¡i Ka lÃ  **Tá»•ng Chá»‰ Huy chiáº¿n lÆ°á»£c**, tuyá»‡t Ä‘á»‘i **KHÃ”NG PHáº¢I "thá»£ gÃµ lá»‡nh terminal"**. Agent pháº£i tá»± Ä‘á»™ng cháº¡y lá»‡nh kiá»ƒm thá»­, build batchmode, phÃ¢n tÃ­ch log vÃ  sá»­a lá»—i. TUYá»†T Äá»I KHÃ”NG báº¯t Äáº¡i Ka gÃµ lá»‡nh tay hoáº·c kiá»ƒm thá»­ thá»§ cÃ´ng khi chÆ°a cÃ³ báº±ng chá»©ng tá»± Ä‘á»™ng pass.
2. **Ranh giá»›i Workspace Äá»™c láº­p (Workspace Boundary Isolation):**  
   Má»i thao tÃ¡c code, thÃªm asset, táº¡o prefab, cáº¥u hÃ¬nh scene CHá»ˆ ÄÆ¯á»¢C PHÃ‰P diá»…n ra trong thÆ° má»¥c subgame nÃ y. Tuyá»‡t Ä‘á»‘i Cáº¤M sá»­a Ä‘á»•i ngÆ°á»£c lÃªn repo máº¹ `wfgame` hoáº·c Global Scope `C:\Users\DELL\.gemini\config`.
3. **Kiá»ƒm chá»©ng CÆ¡ há»c (Evidence-First / Mechanical Validation):**  
   Tuyá»‡t Ä‘á»‘i KHÃ”NG bÃ¡o cÃ¡o "em lÃ m xong rá»“i" báº±ng lÃ½ thuyáº¿t suÃ´ng. Báº¯t buá»™c xuáº¥t trÃ¬nh báº±ng chá»©ng thá»±c táº¿ tá»« Terminal (Exit Code 0 tá»« `AutoQACheck`, build log, Android APK size).
4. **Self-Debug Loop cÃ³ Circuit Breaker (Tá»‘i Äa 3 Láº§n):**  
   Sau khi chá»‰nh sá»­a mÃ£ nguá»“n, Agent tá»± Ä‘á»™ng cháº¡y test/build kiá»ƒm chá»©ng. Náº¿u phÃ¡t sinh lá»—i, tá»± Ä‘á»c toÃ n bá»™ stack trace vÃ  tá»± sá»­a **Tá»I ÄA 3 Láº¦N**. Náº¿u sau 3 láº§n váº«n tháº¥t báº¡i, **Dá»ªNG Láº I NGAY Láº¬P Tá»¨C**, hoÃ n nguyÃªn mÃ£ nguá»“n vá» Rollback Anchor (`git restore .`) vÃ  bÃ¡o cÃ¡o Ä‘iá»ƒm ngháº½n chÃ­nh xÃ¡c kÃ¨m stack trace cho Äáº¡i Ka; cáº¥m láº·p mÃ¹ quÃ¡ng Ä‘á»‘t token.
5. **Quy chuáº©n PhÃ²ng thá»§ Chá»‘ng Ban Store (Store Anti-Ban Protocol):**  
   - 100% Sprite, Model 3D, UI vÃ  Audio bÃ³c tÃ¡ch tá»« XAPK chá»‰ Ä‘Æ°á»£c dÃ¹ng lÃ m tham chiáº¿u ná»™i bá»™ (Internal R&D). TrÆ°á»›c khi ná»™p Google Play / App Store, báº¯t buá»™c thay tháº¿ báº±ng Art Ä‘á»™c quyá»n studio.
   - Root Namespace pháº£i mang tiá»n tá»‘ riÃªng (vÃ­ dá»¥: `GalaxyPro.ShadowReflex`), khÃ´ng dÃ¹ng chung `Game.*` trÃªn báº£n Store.
   - PlayerPrefs keys vÃ  HMAC Salts pháº£i mang tiá»n tá»‘ riÃªng biá»‡t (`ShadowReflex_SecretSalt_...`).
   - Keystore kÃ½ APK/AAB pháº£i lÃ  Keystore riÃªng biá»‡t (`keystore/ShadowReflex.keystore`).
6. **Secret Hygiene Chuáº©n HÃ³a:**  
   Subgame duy trÃ¬ bá»™ Ä‘Ã´i `.env.example` (máº«u cÃ´ng khai) vÃ  `.env.local` (chá»©a credentials tháº­t trÃªn mÃ¡y dev/CI, náº±m trong `.gitignore`).

---

## ðŸš€ 2. DEVELOPER RUNBOOK (3 BÆ¯á»šC CHO DEV TEAM 2 & AI AGENT)

1. **BÆ°á»›c 1 (Má»Ÿ Project):** Má»Ÿ Unity Editor (khuyáº¿n nghá»‹ Unity 6 hoáº·c 2022.3 LTS) trá» trá»±c tiáº¿p vÃ o thÆ° má»¥c subgame nÃ y.
2. **BÆ°á»›c 2 (Kiá»ƒm thá»­ Tá»± Ä‘á»™ng CÆ¡ há»c):**
   ```powershell
   python D:\bkk\wfgame\tools\run_unity.py -g ShadowReflex -m Game.Editor.BuildScript.AutoQACheck
   ```
   Chá»‰ tiáº¿p tá»¥c khi log bÃ¡o `Exit Code: 0` vÃ  100% criteria PASS.
3. **BÆ°á»›c 3 (ÄÃ³ng gÃ³i APK Release):**
   ```powershell
   powershell -File D:\bkk\wfgame\run_android_build.ps1 -Game ShadowReflex
   ```
   APK xuáº¥t xÆ°á»Ÿng sáº½ tá»± Ä‘á»™ng náº±m táº¡i `Builds/Android/ShadowReflex.apk`.

---

## ðŸ“ 3. Báº¢N Äá»’ TÃ€I LIá»†U Dá»° ÃN (DOCUMENTATION SITEMAP)

- [AGENTS.md](file:///d:/bkk/games/ShadowReflex/AGENTS.md): Hiáº¿n phÃ¡p Ä‘iá»u hÆ°á»›ng & Ranh giá»›i tháº©m quyá»n cho Agent trong subgame.
- [HANDOVER_V1_TO_V2.md](file:///d:/bkk/games/ShadowReflex/HANDOVER_V1_TO_V2.md): BiÃªn báº£n bÃ n giao ká»¹ thuáº­t Ä‘á»™c láº­p V1 $\rightarrow$ V2 (What Works, Runbook, V2 Backlog).
- [SPEC.md](file:///d:/bkk/games/ShadowReflex/SPEC.md): Äáº·c táº£ kiáº¿n trÃºc gameplay, rules vÃ  FSM cá»§a game.
- [GOAL.md](file:///d:/bkk/games/ShadowReflex/GOAL.md): Má»¥c tiÃªu vÃ  tiÃªu chuáº©n nghiá»‡m thu tá»«ng milestone.
- [CONTEXT.md](file:///d:/bkk/games/ShadowReflex/CONTEXT.md): Tráº¡ng thÃ¡i Living Spec hiá»‡n táº¡i vÃ  Rollback Anchors.
- [.env.example](file:///d:/bkk/games/ShadowReflex/.env.example): Máº«u biáº¿n mÃ´i trÆ°á»ng (AdMob, RevenueCat, Keystore).

