# AGENTS.md: SUB-GAME REPOSITORY HARNESS & AUTHORITY BOUNDARY

> **Sub-Game:** ShadowReflex  
> **Package ID:** com.galaxypro.shadowreflex  
> **Archetype:** Timing Action Combat (1-Tap Reflex & Telegraph Counter)  
> **Kiến trúc:** 2-Tier Clean Architecture (Decoupled Game Workspace)  
> **Cổng vào thẩm quyền tối thượng:** Khởi đầu mọi tác vụ từ tài liệu này, sau đó đọc `HANDOVER_V1_TO_V2.md`, `SPEC.md`, `GOAL.md` và `CONTEXT.md`.  

---

## 🏛️ 1. NGUYÊN TẮC HOẠT ĐỘNG CỐT LÕI (CORE INVARIANTS)

1. **Con Người là Tổng Chỉ Huy (Human Commander Protocol):**  
   Đại Ka là **Tổng Chỉ Huy chiến lược**, tuyệt đối **KHÔNG PHẢI "thợ gõ lệnh terminal"**. Agent phải tự động chạy lệnh kiểm thử, build batchmode, phân tích log và sửa lỗi. TUYỆT ĐỐI KHÔNG bắt Đại Ka gõ lệnh tay hoặc kiểm thử thủ công khi chưa có bằng chứng tự động pass.
2. **Ranh giới Workspace Độc lập (Workspace Boundary Isolation):**  
   Mọi thao tác code, thêm asset, tạo prefab, cấu hình scene CHỈ ĐƯỢC PHÉP diễn ra trong thư mục subgame này. Tuyệt đối CẤM sửa đổi ngược lên repo mẹ `wfgame` hoặc Global Scope `C:\Users\DELL\.gemini\config`.
3. **Kiểm chứng Cơ học (Evidence-First / Mechanical Validation):**  
   Tuyệt đối KHÔNG báo cáo "em làm xong rồi" bằng lý thuyết suông. Bắt buộc xuất trình bằng chứng thực tế từ Terminal (Exit Code 0 từ `AutoQACheck`, build log, Android APK size).
4. **Self-Debug Loop có Circuit Breaker (Tối Đa 3 Lần):**  
   Sau khi chỉnh sửa mã nguồn, Agent tự động chạy test/build kiểm chứng. Nếu phát sinh lỗi, tự đọc toàn bộ stack trace và tự sửa **TỐI ĐA 3 LẦN**. Nếu sau 3 lần vẫn thất bại, **DỪNG LẠI NGAY LẬP TỨC**, hoàn nguyên mã nguồn về Rollback Anchor (`git restore .`) và báo cáo điểm nghẽn chính xác kèm stack trace cho Đại Ka; cấm lặp mù quáng đốt token.
5. **Quy chuẩn Phòng thủ Chống Ban Store & Vòng Đời Asset (Store Anti-Ban & 3-Stage Lifecycle):**  
   - **V1 & V2 (Internal R&D / Local Staging):** BẮT BUỘC kế thừa Sprites, Audio, Level JSON từ `storagework/extracted_cache/` vào `Assets/Art/Staging_Reference/` để giữ 100% Game Feel gốc và hitbox chuẩn xác. CẤM sinh procedural placeholder pixel 9KB làm dị tật game. Đồng thời khai báo vào `ASSET_REPLACEMENT_MANIFEST.md`.
   - **V3 (Store Release Candidate):** 100% assets trong `Staging_Reference/` phải được thay thế bằng Art độc quyền studio hoặc AI-generated trước khi ký Keystore Release. Lệnh build Release sẽ tự động abort nếu còn sót staging asset chưa reskin.
   - **Phân hóa định danh:** Root Namespace riêng biệt (`GalaxyPro.ShadowReflex`), PlayerPrefs keys & HMAC Salts độc lập (`ShadowReflex_SecretSalt_...`), Keystore độc lập (`keystore/ShadowReflex.keystore`), khuyến nghị Generic Brandname/Package ID để triệt tiêu vi phạm nhãn hiệu.
   - **Poison Sanitation:** Loại bỏ 100% link web/policy/competitor tracker URLs của bên thứ ba trong mọi file config/JSON.
6. **Secret Hygiene Chuẩn Hóa:**  
   Subgame duy trì bộ đôi `.env.example` (mẫu công khai) và `.env.local` (chứa credentials thật trên máy dev/CI, nằm trong `.gitignore`).

---

## 🚀 2. DEVELOPER RUNBOOK (3 BƯỚC CHO DEV TEAM 2 & AI AGENT)

1. **Bước 1 (Mở Project):** Mở Unity Editor (khuyến nghị Unity 6 hoặc 2022.3 LTS) trỏ trực tiếp vào thư mục subgame này.
2. **Bước 2 (Kiểm thử Tự động Cơ học):**
   ```powershell
   python D:\bkk\wfgame\tools\run_unity.py -g ShadowReflex -m Game.Editor.BuildScript.AutoQACheck
   ```
   Chỉ tiếp tục khi log báo `Exit Code: 0` và 100% criteria PASS.
3. **Bước 3 (Đóng gói APK Release):**
   ```powershell
   powershell -File D:\bkk\wfgame\run_android_build.ps1 -Game ShadowReflex
   ```
   APK xuất xưởng sẽ tự động nằm tại `Builds/Android/ShadowReflex.apk`.

---

## 📁 3. BẢN ĐỒ TÀI LIỆU DỰ ÁN (DOCUMENTATION SITEMAP)

- [AGENTS.md](file:///d:/bkk/games/ShadowReflex/AGENTS.md): Hiến pháp điều hướng & Ranh giới thẩm quyền cho Agent trong subgame.
- [HANDOVER_V1_TO_V2.md](file:///d:/bkk/games/ShadowReflex/HANDOVER_V1_TO_V2.md): Biên bản bàn giao kỹ thuật độc lập V1 $\rightarrow$ V2 (What Works, Runbook, V2 Backlog).
- [SPEC.md](file:///d:/bkk/games/ShadowReflex/SPEC.md): Đặc tả kiến trúc gameplay, rules và FSM của game.
- [GOAL.md](file:///d:/bkk/games/ShadowReflex/GOAL.md): Mục tiêu và tiêu chuẩn nghiệm thu từng milestone.
- [CONTEXT.md](file:///d:/bkk/games/ShadowReflex/CONTEXT.md): Trạng thái Living Spec hiện tại và Rollback Anchors.
- [.env.example](file:///d:/bkk/games/ShadowReflex/.env.example): Mẫu biến môi trường (AdMob, RevenueCat, Keystore).
