# AGENTS.md: WORKSPACE HARNESS & AUTHORITY BOUNDARY

> **Dự án Sub-Game:** Shadow Reflex MVP  
> **Package ID:** `com.galaxypro.shadowreflex`  
> **Kiến trúc:** 2-Tier Decoupled Game Workspace  
> **Thư mục Workspace:** `D:\bkk\games\ShadowReflex`  
> **Hệ quy chiếu tối thượng:** `AGENTS.md` -> `HANDOVER_V1_TO_V2.md` -> `SPEC.md` -> `GOAL.md` -> `CONTEXT.md`.  

---

## 🏛️ 1. NGUYÊN TẮC HOẠT ĐỘNG BẮT BUỘC (CORE INVARIANTS)

1. **Workspace Cách Ly Tuyệt Đối (Workspace Boundary):**  
   Mọi thao tác code, thêm asset, chỉnh sửa chỉ được diễn ra trong nội bộ thư mục `D:\bkk\games\ShadowReflex`. Tuyệt đối **CẤM** sửa chéo sang các game khác hoặc repo mẹ (`D:\bkk\wfgame`).
2. **Repository là Hệ quy chiếu Tối thượng (System of Record):**  
   Không tin vào ngữ cảnh chat tạm thời. Mọi đặc tả kỹ thuật và tiến độ phải nằm trong các file `.md` của workspace này.
3. **Progressive Disclosure (Tiếp cận ngữ cảnh tuần tự):**  
   Thứ tự đọc bắt buộc khi khởi động:  
   `AGENTS.md` $\rightarrow$ `HANDOVER_V1_TO_V2.md` $\rightarrow$ `SPEC.md` $\rightarrow$ `GOAL.md` $\rightarrow$ `CONTEXT.md` $\rightarrow$ Mã nguồn Unity (`Assets/_Project/`).
4. **Quy chuẩn Bàn giao V1 $\rightarrow$ V2:**  
   V1 là Core Vertical Slice (đã chơi được, 0 crash). Team 2 tiếp nhận phát triển V2 (Commercial Polish) phải bám sát checklist trong [HANDOVER_V1_TO_V2.md](file:///D:/bkk/games/ShadowReflex/HANDOVER_V1_TO_V2.md).
5. **Kiểm chứng Cơ học (Evidence-First):**  
   Không báo cáo bằng lý thuyết suông. Mọi thay đổi code phải được kiểm tra qua Unity Editor hoặc Build CLI (`run_android_build.ps1`).
6. **Bảo vệ Global Scope:**  
   CẤM đụng chạm vào `C:\Users\DELL\.gemini\config`. Mọi kỹ năng của Agent được cấp phát tại `.agents/skills/`.

---

## 📁 2. BẢN ĐỒ ĐIỀU HƯỚNG DỰ ÁN (SITEMAP)

- [AGENTS.md](file:///D:/bkk/games/ShadowReflex/AGENTS.md): Ranh giới thẩm quyền & Điều hướng cho AI Coding Agent.
- [HANDOVER_V1_TO_V2.md](file:///D:/bkk/games/ShadowReflex/HANDOVER_V1_TO_V2.md): **Biên bản bàn giao kỹ thuật & Checklist công việc cho Team 2.**
- [SPEC.md](file:///D:/bkk/games/ShadowReflex/SPEC.md): Đặc tả kiến trúc gameplay, visual parity và mô hình dữ liệu.
- [GOAL.md](file:///D:/bkk/games/ShadowReflex/GOAL.md): Mục tiêu phát triển và tiêu chí nghiệm thu.
- [CONTEXT.md](file:///D:/bkk/games/ShadowReflex/CONTEXT.md): Trạng thái Living Spec hiện tại và Rollback Anchors.

---

## ⚡ 3. QUICKSTART DEVELOPER RUNBOOK

- **Scene chính:** `Assets/_Project/Scenes/Game.unity`
- **Mã nguồn tính năng:** `Assets/_Project/Scripts/Gameplay/`
- **Dịch vụ nền tảng (Save, Audio, Monetization):** `Assets/_Project/Scripts/Core/`
- **Lệnh build APK Release từ Root mẹ:**
  ```powershell
  cd D:\bkk\wfgame
  powershell -File run_android_build.ps1 -Game ShadowReflex
  ```
