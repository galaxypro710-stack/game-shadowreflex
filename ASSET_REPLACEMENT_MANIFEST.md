# ASSET REPLACEMENT MANIFEST: ShadowReflex

> **Tài liệu Living Spec Quản lý Tài nguyên Kế thừa & Thay thế**  
> **Áp dụng:** Chu trình 3 Giai đoạn (V1 Kế thừa Game Feel $\rightarrow$ V2 Hoàn thiện Logic $\rightarrow$ V3 Reskin Độc quyền).  
> **Mục tiêu:** Bàn giao danh sách 100% tài nguyên Staging cần thay thế cho Team Art/2D/Audio trước khi Release lên Google Play / App Store.

---

## 📊 1. TỔNG QUAN TRẠNG THÁI KIỂM DỊCH (QUARANTINE STATUS)

- **Trạng thái hiện tại:** `STAGING_ACTIVE (V1/V2 R&D)`
- **Số lượng Staging Assets đang dùng:** 0 files
- **Số lượng đã Reskin độc quyền:** 0 files
- **Điều kiện Đóng gói Release:** Bắt buộc 100% trạng thái chuyển sang `[RESKINNED]` trước khi build APK/AAB nộp Store.

---

## 🎨 2. DANH MỤC TÀI NGUYÊN KẾ THỪA & KẾ HOẠCH THAY THẾ (V3 BACKLOG)

| STT | Đường dẫn Asset (`Assets/Art/Staging_Reference/...`) | Loại (Sprite / Audio / JSON) | Đối tượng / Prefab sử dụng | Trạng thái | Yêu cầu Thiết kế lại (Art Direction cho V3) |
| :---: | :--- | :---: | :--- | :---: | :--- |
| 1 | `Assets/Art/Staging_Reference/Sprites/character_hero.png` | Sprite 2D | `Prefabs/Player.prefab` | `[STAGING]` | Vẽ mới nhân vật độc quyền, giữ nguyên kích thước canvas và pivot point (Bottom Center). |
| 2 | `Assets/Art/Staging_Reference/Audio/sfx_win.wav` | SFX | `AudioService / SoundManager` | `[STAGING]` | Tạo sound fx chiến thắng mới (SFXR / AI Audio), thời lượng < 2s. |

---

## 🛡️ 3. QUY TRÌNH THAY THẾ CHO TEAM ART / V3 RELEASE TEAM

1. **Bước 1 (Nhận Asset):** Dựa vào cột *Đường dẫn Asset* và *Kích thước Canvas*, tạo asset độc quyền mới tương ứng (AI generation bằng Midjourney/Flux hoặc vẽ tay).
2. **Bước 2 (Ghi đè hoặc Trỏ lại Prefab):** Lưu asset mới vào thư mục chính thức `Assets/Art/Textures/` hoặc `Assets/Art/Audio/`.
3. **Bước 3 (Cập nhật Manifest):** Đổi nhãn `[STAGING]` thành `[RESKINNED]`.
4. **Bước 4 (Dọn sạch Staging):** Khi 100% assets đã reskin, xóa sạch thư mục `Assets/Art/Staging_Reference/` để Quarantine Gate mở khóa cho lệnh build Release APK.
