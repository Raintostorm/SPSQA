## 007–008 · Summon + Summon Results (ảnh user) · SỬA
- Đạt: ruy băng SUMMON không che lều; back trái; ví Mythic Stone/Gem gọn; nút Summon dời xuống.
- Lỗi: ô "Ally shards" hiện ô vuông trắng — `Shop/fragment` import dạng Texture nên `Load<Sprite>` ra null. Đã sửa bằng cách bọc texture thành sprite trong code.
- User muốn diện mạo mới: nút hiện là khối xanh phẳng, sơ sài. Thẻ kết quả: ảnh đầu bị cắt ngang cụt, header là hộp chữ trơn, hàng cuối bị cắt không có fade.
- Quyết định: chờ art 002 (bộ nút chung — dùng cho cả UI-006) và 003 (nút Summon + khung thẻ theo độ hiếm). Sau khi CHỐT: Results dùng ruy băng thay hộp header, thẻ khung theo rarity, chân dung vừa khung, có fade cuối danh sách.

## 002 · Bộ nút chung · CHỐT (2026-11-03)
- Đạt: 4 màu đúng vai trò, viền đen dày, highlight trên + lip dưới, nền trong suốt, đầu tròn đều nên kéo ngang được.
- Hạn chế: dáng viên thuốc (bo tròn hết) thay vì bo 30%; trạng thái pressed chỉ đậm hơn chút, lip vẫn còn → trong code sẽ thêm hạ chữ 4 px + tối màu khi bấm. 4 bản disabled giống nhau → giữ 1.
- Đã cắt (Claude, `tools/import_buttons_summon_kit.py`): `Resources/Buttons/{gold,blue,slate,red}{,-pressed}.png` + `disabled.png`, border 75/62. Chưa nối code — để UI-006.

003 đã tích hợp theo báo cáo Claude; lịch sử đầy đủ ở archive/2026-11-03-002-003/claude.md. QA màn Summon/Results vẫn theo mục 007–008 bên trên.

## 009–010 · Summon + Results trong game · gần CHỐT
- Summon: đạt — nút ×1/×10 + pill giá đẹp, đúng vai trò.
- Results: khung rarity + ruy băng + icon shard đạt. Sửa: số ×N đè lên mặt nhân vật → chuyển thành badge góc trên phải; thẻ nhỏ lại (290×423, hàng 436) để 10x hiện đủ 3 hàng, không còn hàng bị cắt. Chờ ảnh 011.


## 011 · Results sau sửa · CHỐT (2026-11-03)
- Badge ×N góc trên phải không che mặt; 3 hàng vừa khít. Còn: ảnh đầu Haunted Tank crop lỗi (UI-009).
