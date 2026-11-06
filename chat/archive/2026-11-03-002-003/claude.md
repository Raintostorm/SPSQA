# Claude xem ảnh và chốt

Mỗi ảnh trong `chat/check/` được xem ở đây TRƯỚC khi đưa vào game. Ghi: đạt gì, lỗi gì, quyết định (CHỐT / SỬA + prompt sửa ở `chatgpt.md`), và dùng ở đâu.

---

Luật dọn chat: khi một yêu cầu đã xong (có output, đã CHỐT và đã cắt/gắn), XOÁ mục đó khỏi `chatgpt.md` và `claude.md` để lượt sau bắt đầu gọn. Quyết định lâu dài ghi ở `doc/UI_PLAN.md` hoặc README trong `artsrc/ui/<tên>-vN/` trước khi xoá. Ảnh trong `check/` giữ lại.

---

## 007–008 · Summon + Summon Results (ảnh user) · SỬA
- Đạt: ruy băng SUMMON không che lều; back trái; ví Mythic Stone/Gem gọn; nút Summon dời xuống.
- Lỗi: ô "Ally shards" hiện ô vuông trắng — `Shop/fragment` import dạng Texture nên `Load<Sprite>` ra null. Đã sửa bằng cách bọc texture thành sprite trong code.
- User muốn diện mạo mới: nút hiện là khối xanh phẳng, sơ sài. Thẻ kết quả: ảnh đầu bị cắt ngang cụt, header là hộp chữ trơn, hàng cuối bị cắt không có fade.
- Quyết định: chờ art 002 (bộ nút chung — dùng cho cả UI-006) và 003 (nút Summon + khung thẻ theo độ hiếm). Sau khi CHỐT: Results dùng ruy băng thay hộp header, thẻ khung theo rarity, chân dung vừa khung, có fade cuối danh sách.

## 002 · Bộ nút chung · CHỐT (2026-11-03)
- Đạt: 4 màu đúng vai trò, viền đen dày, highlight trên + lip dưới, nền trong suốt, đầu tròn đều nên kéo ngang được.
- Hạn chế: dáng viên thuốc (bo tròn hết) thay vì bo 30%; trạng thái pressed chỉ đậm hơn chút, lip vẫn còn → trong code sẽ thêm hạ chữ 4 px + tối màu khi bấm. 4 bản disabled giống nhau → giữ 1.
- Đã cắt (Claude, `tools/import_buttons_summon_kit.py`): `Resources/Buttons/{gold,blue,slate,red}{,-pressed}.png` + `disabled.png`, border 75/62. Chưa nối code — để UI-006.

## 003 · Summon kit · CHỐT (2026-11-03)
- Đạt: nút ×1 xanh / ×10 tím viền vàng có sao rõ "gói tốt hơn"; pill giá; 5 khung thẻ đúng màu độ hiếm, ô chân dung và bảng tên trống.
- Đã cắt: `Resources/SummonKit/{summon-x1,summon-x10,price-pill,card-common,card-rare,card-epic,card-legendary,card-stone}.png`. Preview: `check/002-003-cut-preview.png`. Nguồn + manifest: `artsrc/ui/buttons-v1/`, `artsrc/ui/summon-kit-v1/`.
- Đã nối: nút Summon ×1/×10 + pill giá (GemEconomyScreen); Results dùng ruy băng + khung thẻ theo rarity, đầu nhân vật đặt sát đáy ô chân dung (SummonResultsView). Chờ ảnh user 009/010.
