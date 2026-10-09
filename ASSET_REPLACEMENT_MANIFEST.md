# ASSET REPLACEMENT MANIFEST: ShadowReflex

> **Tài liệu Living Spec Quản lý Tài nguyên Kế thừa & Thay thế**  
> **Áp dụng:** Chu trình 3 Giai đoạn (V1 Kế thừa Game Feel $\rightarrow$ V2 Hoàn thiện Logic $\rightarrow$ V3 Reskin Độc quyền).  
> **Mục tiêu:** Bàn giao danh sách 100% tài nguyên Staging cần thay thế cho Team Art/2D/Audio trước khi Release lên Google Play / App Store.

---

## 📊 1. TỔNG QUAN TRẠNG THÁI KIỂM DỊCH (QUARANTINE STATUS)

- **Trạng thái hiện tại:** `CLEAN_CORE (0 Staging Assets - Procedural / Pure Code)`
- **Tổng số Staging Assets kế thừa:** **0 files**
  - **Sprites 2D / Textures:** 0 files
  - **Audio Clips (SFX / BGM):** 0 files
- **Số lượng đã Reskin độc quyền:** 0 files
- **Điều kiện Đóng gói Release:** Bắt buộc 100% tài nguyên staging phải được thay thế trước khi ký Keystore Release Store.

---

## 🎨 2. DANH MỤC NHÓM TÀI NGUYÊN KẾ THỪA & KẾ HOẠCH THAY THẾ (V3 BACKLOG)

| STT | Nhóm Tài Nguyên | Đường dẫn Thư mục | Số lượng | Trạng thái | Yêu cầu Thiết kế lại (Art Direction cho V3) |
| :---: | :--- | :--- | :---: | :---: | :--- |
| 1 | Sprites 2D / UI / Characters | `Assets/_Project/Art/` | 0 files | `[STAGING]` | Vẽ mới toàn bộ Art độc quyền studio hoặc sinh bằng Midjourney/Flux, giữ đúng tỷ lệ Canvas và Pivot. |
| 2 | Audio Clips (SFX & BGM) | `Assets/_Project/Audio/` | 0 files | `[STAGING]` | Tạo mới toàn bộ âm thanh sound fx bằng SFXR / AI Audio Generator, thời lượng và âm lượng cân bằng. |

---

## 🛡️ 3. QUY TRÌNH THAY THẾ CHO TEAM ART / V3 RELEASE TEAM

1. **Bước 1 (Nhận Asset):** Dựa vào cột *Đường dẫn Thư mục*, tạo asset độc quyền mới tương ứng (AI generation hoặc vẽ tay).
2. **Bước 2 (Ghi đè hoặc Trỏ lại Prefab):** Lưu asset mới vào thư mục chính thức và cập nhật tham chiếu Prefab.
3. **Bước 3 (Cập nhật Manifest):** Chuyển trạng thái từ `[STAGING]` sang `[RESKINNED]`.
4. **Bước 4 (Dọn sạch Staging):** Đảm bảo Quarantine Gate mở khóa cho lệnh build Release APK (`run_android_build.ps1 -Release`).
