# HƯỚNG DẪN QUY CHUẨN BẢN QUYỀN FONT CHỮ (FONT HYGIENE & STORE LICENSING DOCTRINE)

> **Mục tiêu:** Triệt tiêu hoàn toàn rủi ro bị xưởng font (Font Foundries) quét trát DMCA và kiện bản quyền thương mại ($5,000 - $25,000+) khi xuất bản game từ Meta-Factory lên Google Play & Apple App Store.  
> **Cơ sở pháp lý:** SIL Open Font License (OFL) 1.1 / Apache 2.0.  

---

## ⚠️ 1. TẠI SAO FONT TỪ APK GỐC LÀ "BẪY PHÁP LÝ TỬ THẦN"?

1. **Bot Quét Tự Động:** Các hãng typography lớn (Monotype, Linotype, Adobe, DaFont commercial owners) sử dụng bot tự động tải APK/IPA từ Store về, giải nén thư mục `Assets/` để trích xuất file `.ttf`, `.otf`.
2. **Chữ Ký Số & Glyph Hash:** Mỗi file font đều có metadata nhúng, tên tác giả, version và bảng ký tự (Glyphs) mang dấu vân tay độc nhất.
3. **Mức phạt DMCA:** Khác với tranh chấp đồ họa thông thường, bản quyền font chữ tại Mỹ và EU có khung phạt cố định rất nặng và Store sẽ gỡ bỏ app ngay lập tức nếu không xuất trình được Commercial License Invoice tương ứng với Bundle ID / Company Name.

---

## 🛡️ 2. NGUYÊN TẮC "ZERO-FONT RISK" CHO 3 GIAI ĐOẠN (V1 $\rightarrow$ V2 $\rightarrow$ V3)

| Giai đoạn | Thao tác với Font chữ | Quy tắc an toàn |
| :--- | :--- | :--- |
| **V1 (Local Staging / Prototype)** | **CẤM** sử dụng font trích xuất từ APK gốc. Sử dụng ngay Google Fonts OFL miễn phí thương mại. | Đặt font vào `Assets/_Project/Fonts/` kèm file `OFL.txt`. |
| **V2 (Monetization & Backend)** | Giữ nguyên font OFL hoặc nâng cấp bộ font mở có hỗ trợ đa ngôn ngữ (Localization CJK, Cyrillic, Latin). | Test hiển thị số tiền IAP, bảng chữ cái các ngôn ngữ. |
| **V3 (Store Release)** | Chạy `python tools/quarantine_build_gate.py --release` để xác thực 100% font có file license đi kèm. | Tuyệt đối không để sót file font không rõ nguồn gốc. |

---

## 🎨 3. TOP 5 GOOGLE FONTS KHUYÊN DÙNG CHO CASUAL & PUZZLE GAMES (OFL 100% FREE)

Các font này miễn phí 100% cho mục đích thương mại, đã được tối ưu hóa cho TextMeshPro và UI Canvas trên di động:

1. **Fredoka (hoặc Fredoka One):**  
   - *Phong cách:* Bo tròn, thân thiện, vui nhộn.  
   - *Phù hợp archetype:* Match-3, Puzzle, Bubble Shooter, Merge Battle.  
   - *Link nguồn:* Google Fonts (`https://fonts.google.com/specimen/Fredoka`)
2. **Nunito (hoặc Nunito Sans):**  
   - *Phong cách:* Hiện đại, thanh thoát, dễ đọc ở kích thước nhỏ.  
   - *Phù hợp:* UI nút bấm, Settings Popup, Leaderboard, Shop.  
   - *Link nguồn:* Google Fonts (`https://fonts.google.com/specimen/Nunito`)
3. **Titan One:**  
   - *Phong cách:* Chữ đậm (Chunky), nổi khối, tạo điểm nhấn mạnh.  
   - *Phù hợp:* Banner "VICTORY", "DEFEAT", "LEVEL COMPLETE", Điểm số Combo.  
   - *Link nguồn:* Google Fonts (`https://fonts.google.com/specimen/Titan+One`)
4. **Sniglet:**  
   - *Phong cách:* Dễ thương, phong cách hoạt hình (Cartoon).  
   - *Phù hợp:* Save The Doge, Bubble Panda, các game có nhân vật động vật.
5. **Bungee / Bangers:**  
   - *Phong cách:* Comic book, hành động, comic punchy text.  
   - *Phù hợp:* Action combat, timing games.

---

## 📋 4. CHECKLIST NGHIỆM THU TRƯỚC KHI COMMIT FONT MỚI

- [ ] File font tải từ nguồn chính thức của Google Fonts (hoặc Font Squirrel OFL).
- [ ] Đi kèm file `OFL.txt` hoặc `LICENSE.txt` trong cùng thư mục font.
- [ ] Đã tạo TextMeshPro Font Asset (`.asset`) với bảng ký tự Unicode Extended (hỗ trợ Tiếng Việt có dấu).
- [ ] Chạy `python tools/quarantine_build_gate.py` cho kết quả **Exit Code 0** và không có cảnh báo unverified fonts.
