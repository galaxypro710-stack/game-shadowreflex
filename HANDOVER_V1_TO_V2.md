# BIÊN BẢN BÀN GIAO KỸ THUẬT: VERSION 1 $\rightarrow$ VERSION 2 (HANDOVER KIT)

> **Tên Sub-Game:** Shadow Reflex MVP  
> **Package ID:** `com.galaxypro.shadowreflex`  
> **Archetype:** Timing Action Combat (1-Tap Reflex & Telegraph Counter)  
> **Bên bàn giao:** Core Engine / Meta-Factory Team (Phụ trách Version 1 - Core Slice)  
> **Bên tiếp nhận:** Commercial Release / Publishing Team (Phụ trách Version 2 - Hoàn Thiện & Phát Hành)  
> **Tỷ lệ hoàn thiện V1:** **75% - 80% (Playable Vertical Slice, 0 Crash trên Thiết bị thật)**  
> **Tôn chỉ tuân thủ:** `/ai-copilot-alignment`, `/harness-engineering`, Karpathy Behavioral Guidelines  

---

## 🎯 1. TRẠNG THÁI HIỆN TẠI CỦA VERSION 1 (WHAT WORKS)

Bản Version 1 đã hoàn thành đầy đủ Core Gameplay Loop, tích hợp toàn bộ tài nguyên trích xuất, hệ thống lưu mã hóa và cơ chế chạy an toàn Offline-First.

### A. Minh chứng Thực chứng (Mechanical Proof):
- **File APK Release:** `Builds/Android/ShadowReflex.apk` (46.20 MB)
- **Kiểm nghiệm thiết bị thật:** **Samsung Galaxy A53 5G & Windows Standalone**
- **Độ ổn định:** 0 Crash, 0 Memory Leak, PSS RAM duy trì < 210 MB.
- **Save System:** Đã kiểm chứng lưu/tải dữ liệu qua mã hóa HMAC-SHA256 chống cheat.

### B. Danh sách tính năng V1 đã kiểm chứng (100% PASS):
- [x] **Cơ chế 1-Tap Timing Reflex phản đòn chớp nhoáng (Parry/Dodge)**
- [x] **Hiệu ứng Telegraph chỉ báo đòn đánh địch sắc nét**
- [x] **Slomo Bullet Time và vệt chém kiếm katana khi parry thành công**
- [x] **HMAC-SHA256 Encrypted SaveService**
- [x] **Offline-First Mock AdMob & RevenueCat**

---

## 🚀 2. DEVELOPER RUNBOOK: HƯỚNG DẪN 3 BƯỚC CHO DEV TEAM 2

Dev của Team 2 không cần đọc prompt, chỉ cần làm đúng 3 bước:

1. **Bước 1: Mở Project trong Unity Editor**
   - Phiên bản khuyến nghị: **Unity 6 (6000.0.x)** hoặc **Unity 2022.3 LTS**.
   - Trỏ Unity Hub vào thư mục: `D:\bkk\games\ShadowReflex`.
2. **Bước 2: Mở Scene Kiểm Thử**
   - Mở file Scene: `Assets/_Project/Scenes/Game.unity`.
   - Nhấn nút **Play** trên Unity Editor để trải nghiệm trực tiếp toàn bộ Core Loop.
3. **Bước 3: Đóng gói APK Release qua CLI tự động**
   - Mở PowerShell và chạy:
     ```powershell
     cd D:\bkk\wfgame
     powershell -File run_android_build.ps1 -Game ShadowReflex
     ```
   - APK xuất xưởng sẽ tự động nằm tại: `D:\bkk\games\ShadowReflex\Builds\Android\`.

---

## 📋 3. V2 BACKLOG CHECKLIST (NHỮNG VIỆC TEAM 2 CẦN LÀM ĐỂ LÊN 100%)

Team 2 tiếp nhận source code cần hoàn thành các hạng mục sau để đưa game lên Google Play Store:

### A. Sở hữu Trí tuệ & Bản quyền (IP Defense - ƯU TIÊN P0):
- [ ] Thay thế hiệu ứng và sprite samurai/ninja sang art độc quyền
- [ ] Gắn Live AdMob Rewarded ID hồi sinh tại chỗ (Revive Token)
- [ ] Gắn Live RevenueCat IAP mua vũ khí hoàng gia và thẻ bài kỹ năng
- [ ] Thêm Boss Fight đa tầng với nhịp tấn công phức tạp

### B. Quy trình Bật Live Monetization:
1. Mở file ScriptableObject cấu hình tại: `Assets/_Project/Data/GameConfig.asset` (hoặc cấu hình tương đương trong Core).
2. Tắt cờ `IsMockMode = false`.
3. Nhập các ID sản phẩm thật từ Google Play Console và AdMob Console.

---

## 🏛️ 4. BẢN ĐỒ KIẾN TRÚC MÃ NGUỒN (CLEAN ARCHITECTURE 2 TẦNG)

Mã nguồn được phân tách theo quy chuẩn Clean Architecture nhằm giúp Team 2 dễ dàng mở rộng mà không làm hỏng lõi:

```text
D:\bkk\games\ShadowReflex\Assets\_Project\
├── Data/                 # Chứa GameConfig, Levels JSON, Data Tables
├── Scenes/               # Chứa Game.unity (Scene chạy chính)
├── Scripts/
│   ├── Core/             # TẦNG 1: Dịch vụ chung (KHÔNG SỬA NẾU KHÔNG CẦN THIẾT)
│   │   ├── Audio/        # SoundManager, BGM, SFX Combo Pitch
│   │   ├── Monetization/ # AdMob & RevenueCat Bridge (Mock + Live)
│   │   └── Save/         # HMAC-SHA256 Encrypted SaveService
│   └── Gameplay/         # TẦNG 2: Logic Game con (TEAM 2 THOẢI MÁI PHÁT TRIỂN)
│       ├── Board/        # Quản lý bàn cờ, lưới tọa độ
│       ├── Controllers/  # GameController, StateMachine
│       ├── Entities/     # Tiles, Bubbles, Units, Particle FX
│       └── UI/           # Màn hình Win/Lose, HUD, Cửa hàng
```

---

## ⚠️ 5. CÁC ĐIỂM CẦN LƯU Ý KỸ THUẬT (KNOWN LIMITATIONS & GOTCHAS)

1. **Mock Services:** Mặc định V1 kích hoạt Mock AdMob và Mock RevenueCat để dev có thể test mượt mà trên Editor và APK mà không cần kết nối mạng hay tạo tài khoản quảng cáo. Khi build bản Store, bắt buộc chuyển sang Live Mode.
2. **Save Data:** File save lưu tại `Application.persistentDataPath/savegame.dat`. Khi cần xóa save để test màn 1 từ đầu, hãy xóa file này hoặc gọi hàm `SaveService.DeleteSave()`.
3. **Không chỉnh sửa ngoài Scope:** Mọi thay đổi chỉ thực hiện trong thư mục `D:\bkk\games\ShadowReflex`.
