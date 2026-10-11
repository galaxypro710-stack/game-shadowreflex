# ASSET REPLACEMENT MANIFEST: ShadowReflex

> **Tài liệu Living Spec Quản lý Tài nguyên Kế thừa & Thay thế**  
> **Áp dụng:** Chu trình 3 Giai đoạn (V1 Kế thừa Game Feel $\rightarrow$ V2 Hoàn thiện Logic $\rightarrow$ V3 Reskin Độc quyền).  
> **Mục tiêu:** Bàn giao danh sách 100% tài nguyên Staging cần thay thế cho Team Art/2D/Audio trước khi Release lên Google Play / App Store.

---

## 📊 1. TỔNG QUAN TRẠNG THÁI KIỂM DỊCH (QUARANTINE STATUS)

- **Trạng thái hiện tại:** `STAGING_ACTIVE (V1/V2 R&D: 13 Sprites, 6 Audio Clips)`
- **Tổng số Staging Assets kế thừa:** **19 files**
  - **Sprites 2D / Textures:** 13 files
  - **Audio Clips (SFX / BGM):** 6 files
- **Số lượng đã Reskin độc quyền:** 0 files
- **Điều kiện Đóng gói Release:** Bắt buộc 100% tài nguyên staging phải được thay thế trước khi ký Keystore Release Store.

---

## 🎨 2. DANH MỤC NHÓM TÀI NGUYÊN KẾ THỪA & KẾ HOẠCH THAY THẾ (V3 BACKLOG)

| STT | Nhóm Tài Nguyên | Đường dẫn Thư mục | Số lượng | Trạng thái | Yêu cầu Thiết kế lại (Art Direction cho V3) |
| :---: | :--- | :--- | :---: | :---: | :--- |
| 1 | Sprites 2D / UI / Characters | `Assets/_Project/Art/` | 13 files | `[STAGING]` | Vẽ mới toàn bộ Art độc quyền studio hoặc sinh bằng Midjourney/Flux, giữ đúng tỷ lệ Canvas và Pivot. |
| 2 | Audio Clips (SFX & BGM) | `Assets/_Project/Audio/` | 6 files | `[STAGING]` | Tạo mới toàn bộ âm thanh sound fx bằng SFXR / AI Audio Generator, thời lượng và âm lượng cân bằng. |

---

## 📐 3. RÀNG BUỘC KỸ THUẬT BẮT BUỘC KHI RESKIN (ASSET TECHNICAL CONTRACT)

Để đảm bảo Team 3 khi thay thế tài nguyên không làm lệch Hitbox, lệch Pivot hoặc vỡ Prefab của Team 1 & 2:

### A. Ràng buộc Đồ họa (2D Sprites & Background):
1. **Định dạng:** PNG 32-bit (hỗ trợ kênh Alpha trong suốt hoàn toàn).
2. **Kích thước nhân vật & VFX:** Đúng chuẩn **256x256 px** (Square Canvas).
3. **Kích thước hình nền (Background):** Đúng chuẩn **540x960 px** (Tỷ lệ dọc 9:16).
4. **Điểm tựa (Pivot Point):** Mặc định đặt tại **Center `(0.5, 0.5)`** đối với nhân vật và hiệu ứng chém.
5. **Thông số Unity Import:** 
   - Texture Type: `Sprite (2D and UI)`
   - Pixels Per Unit (PPU): `100`
   - Compression: `Normal Quality / ETC2 / ASTC`

### B. Ràng buộc Âm thanh (Audio Clips):
1. **Định dạng:** WAV 16-bit Signed PCM (uncompressed).
2. **Kênh (Channels):** Mono (1 channel) cho SFX đòn đánh để tối ưu bộ nhớ; Stereo cho BGM.
3. **Tần số lấy mẫu (Sample Rate):** 44,100 Hz (44.1 kHz).
4. **Âm lượng tối đa (Peak Level):** Chuẩn hóa tối đa $\le -1.0\text{ dBFS}$ chống hiện tượng xé tiếng (clipping).

---

## 📋 4. CHI TIẾT 19 TÀI NGUYÊN STAGING & BẢNG THÔNG SỐ ĐỐI CHIẾU

| STT | Tên File Staging | Loại | Độ Phân Giải / Thời Lượng | Pivot | Trạng thái | Ghi Chú Tính Năng |
| :---: | :--- | :---: | :---: | :---: | :---: | :--- |
| 1 | `Characters/hero_idle.png` | Sprite | 256x256 px | (0.5, 0.5) | `[STAGING]` | Trạng thái đứng chờ của Ninja/Samurai |
| 2 | `Characters/hero_attack.png` | Sprite | 256x256 px | (0.5, 0.5) | `[STAGING]` | Thế đánh kiếm Katana phản công |
| 3 | `Characters/hero_dodge.png` | Sprite | 256x256 px | (0.5, 0.5) | `[STAGING]` | Động tác lướt né đòn chém |
| 4 | `Characters/enemy_scout_idle.png` | Sprite | 256x256 px | (0.5, 0.5) | `[STAGING]` | Ninja trinh sát chờ tiếp cận |
| 5 | `Characters/enemy_scout_windup.png` | Sprite | 256x256 px | (0.5, 0.5) | `[STAGING]` | Giai đoạn Telegraph giơ song đao chuẩn bị đâm |
| 6 | `Characters/enemy_scout_strike.png` | Sprite | 256x256 px | (0.5, 0.5) | `[STAGING]` | Đòn chém song đao phóng về phía người chơi |
| 7 | `Characters/enemy_ronin_idle.png` | Sprite | 256x256 px | (0.5, 0.5) | `[STAGING]` | Đại đao Ronin đứng thủ thế |
| 8 | `Characters/enemy_ronin_windup.png` | Sprite | 256x256 px | (0.5, 0.5) | `[STAGING]` | Giai đoạn Telegraph vung đại đao lên cao |
| 9 | `Characters/enemy_ronin_strike.png` | Sprite | 256x256 px | (0.5, 0.5) | `[STAGING]` | Đòn bổ đao uy lực xé gió |
| 10 | `Environment/bg_bamboo_night.png` | Sprite | 540x960 px | (0.5, 0.5) | `[STAGING]` | Phông nền rừng trúc ban đêm có trăng |
| 11 | `VFX/slash_fx.png` | Sprite | 256x128 px | (0.5, 0.5) | `[STAGING]` | Vệt chém kiếm sáng lóa |
| 12 | `VFX/counter_flash_fx.png` | Sprite | 128x128 px | (0.5, 0.5) | `[STAGING]` | Chớp sáng khi phản đòn hoàn hảo (Perfect Counter) |
| 13 | `VFX/telegraph_warning_icon.png` | Sprite | 96x96 px | (0.5, 0.5) | `[STAGING]` | Biểu tượng cảnh báo đỏ nguy hiểm trên đầu quái |
| 14 | `Audio/bgm_shadow_battle.wav` | Audio | ~4.0s (Loopable) | N/A | `[STAGING]` | Nhạc trống Taiko chiến trận dồn dập |
| 15 | `Audio/sfx_slash.wav` | Audio | ~0.22s | N/A | `[STAGING]` | Tiếng vung kiếm xé gió |
| 16 | `Audio/sfx_dodge_whoosh.wav` | Audio | ~0.28s | N/A | `[STAGING]` | Tiếng lướt thân né tránh |
| 17 | `Audio/sfx_parry_clang.wav` | Audio | ~0.50s | N/A | `[STAGING]` | Tiếng đao kiếm chạm nhau tóe lửa |
| 18 | `Audio/sfx_hit_impact.wav` | Audio | ~0.25s | N/A | `[STAGING]` | Tiếng trúng đòn thấu xương |
| 19 | `Audio/sfx_enemy_alert.wav` | Audio | ~0.24s | N/A | `[STAGING]` | Tiếng chuông cảnh báo quái ra đòn |

---

## 🛡️ 5. QUY TRÌNH THAY THẾ CHO TEAM ART / V3 RELEASE TEAM

1. **Bước 1 (Nhận Asset):** Dựa vào bảng thông số kỹ thuật ở Mục 4, tạo asset độc quyền mới tương ứng (AI generation hoặc vẽ tay 2D).
2. **Bước 2 (Ghi đè hoặc Trỏ lại Prefab):** Lưu asset mới vào thư mục chính thức và cập nhật tham chiếu Prefab.
3. **Bước 3 (Cập nhật Manifest):** Chuyển trạng thái từ `[STAGING]` sang `[RESKINNED]`.
4. **Bước 4 (Dọn sạch Staging):** Chạy `python tools/quarantine_build_gate.py --release` để xác nhận 0 staging file trước khi xuất xưởng Release AAB/APK.
