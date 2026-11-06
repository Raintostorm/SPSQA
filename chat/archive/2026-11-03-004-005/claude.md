# Claude xem ảnh và chốt

Mỗi ảnh trong `chat/check/` được xem ở đây TRƯỚC khi đưa vào game. Ghi: đạt gì, lỗi gì, quyết định (CHỐT / SỬA + prompt sửa ở `chatgpt.md`), và dùng ở đâu.

---

Luật dọn chat: khi một yêu cầu đã xong (có output, đã CHỐT và đã cắt/gắn), XOÁ mục đó khỏi `chatgpt.md` và `claude.md` để lượt sau bắt đầu gọn. Quyết định lâu dài ghi ở `doc/UI_PLAN.md` hoặc README trong `artsrc/ui/<tên>-vN/` trước khi xoá. Ảnh trong `check/` giữ lại.

---

_(Chưa có ảnh nào đang chờ duyệt.)_

## 012 · Shop trong game · SỬA (layout) + chờ art 004/005
- Đạt: back + ví + ruy băng SHOP, plaque mục, khung độ hiếm, badge ×N, nút giá vàng, gói Gold/Gems nền navy.
- Sửa layout (đã code): tên dài bị cắt ("Chipped Wooden") → best-fit; icon relic quá nhỏ trong ô → 225; thẻ to hơn 300×438 cho kín hàng, hàng cách 535 để nút không chạm thẻ dưới.
- Chờ art: 004 gói Gold/Gems 3 cỡ; 005 cắt lại đầu Haunted Tank.

## 004 · Gói Gold/Gems · CHỐT (2026-11-03)
- Ghép lên nền sáng và nền navy: không thấy quầng màu (28k pixel bán trong suốt chỉ là viền khử răng cưa). 3 cỡ đọc rõ nhỏ → vừa → lớn, đúng nét game.
- Claude đã cắt: `Resources/ShopPacks/{gold,gems}-{small,medium,large}.png` (nguồn `artsrc/ui/shop-packs-v1/`), đã nối vào gói Gold/Gems trong DailyShopScreen.

## 005 · Đầu Haunted Tank · CHỐT
- Bản mới nhận ra là xe tăng (đầu lâu + nòng + lửa xanh), đáy phẳng. Hình hơi nhỏ, nằm nửa dưới ô 280 — chấp nhận; trong khung chân dung được đặt sát đáy.
