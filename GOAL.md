# GOAL.md: Káº¾ HOáº CH & TIÃŠU CHUáº¨N NGHIá»†M THU ShadowReflex

> **Má»¥c tiÃªu:** TÃ¡i táº¡o thÃ nh cÃ´ng Core Gameplay Loop & Xuáº¥t xÆ°á»Ÿng Android Release APK cá»§a ShadowReflex  
> **NguyÃªn táº¯c:** Evidence-First, 0 Console Errors, AutoQA PASS 100%, Build Pass Exit Code 0  
> **Ãp dá»¥ng:** `/ai-copilot-alignment`, `/behavior-model-debugger`, `/harness-engineering`  

---

## ðŸŽ¯ DANH SÃCH MILESTONES Cá»T LÃ•I

- [ ] **Milestone 1: Khá»Ÿi Táº¡o & Náº¡p Cáº¥u HÃ¬nh MÃ n ChÆ¡i**
  - Verify Scene `Assets/_Project/Scenes/Game.unity` náº¡p Ä‘á»§ Core Systems (GameManager, AudioManager, SaveService, ThemeManager).
  - Khá»Ÿi táº¡o thÆ° má»¥c Data, load cáº¥u hÃ¬nh JSON/CSV Ä‘á»™c quyá»n.
  - Kiá»ƒm chá»©ng: `python tools/run_unity.py -g ShadowReflex -m Game.Editor.BuildScript.AutoQACheck` -> Exit Code 0.

- [ ] **Milestone 2: Triá»ƒn Khai Gameplay Mechanics & Input Interaction**
  - Thi cÃ´ng FSM Gameplay State Machine Ä‘á»™c quyá»n theo Archetype `timing_action_combat`.
  - CÆ¡ cháº¿ cháº¡m/vuá»‘t/kÃ©o tháº£ Ä‘Æ°á»£c clamp an toÃ n, cÃ³ timeout watchdog chá»‘ng káº¹t tÆ°Æ¡ng tÃ¡c.
  - TÃ­ch há»£p hiá»‡u á»©ng Ã¢m thanh vÃ  háº¡t Particle FX.

- [ ] **Milestone 3: Nghiá»‡m Thu Tá»± Äá»™ng & ÄÃ³ng GÃ³i Xuáº¥t XÆ°á»Ÿng (Release Packaging)**
  - AutoQA Check 5/5 tiÃªu chÃ­ PASS 100%.
  - BiÃªn dá»‹ch Standalone Windows: `Builds/Windows/ShadowReflex.exe`.
  - Sinh Keystore riÃªng & biÃªn dá»‹ch Android Release APK: `Builds/Android/ShadowReflex.apk`.
  - Kiá»ƒm tra tÃ­nh toÃ n váº¹n `.meta` (zero orphan) vÃ  quÃ©t sáº¡ch secret (0 token rÃ² rá»‰).

