# Claude review

## 010 · `check/027-button-icons.png` · **CHỐT** · 2026-11-04

8 icon tách nền sạch, đúng thứ tự, không khung/không đổ bóng, bạc trắng viền đen nên
đọc được trên cả bốn màu nút. Đã cắt vào `Resources/ButtonIcons/` bằng
`tools/import_button_icons.py` — gom theo khe cột chứ không theo connected component,
vì tia sáng quả cầu và vạch xanh của dấu tick là mảnh rời, label sẽ tách nhầm.

Mỗi icon chuẩn hoá vào khung 256², cạnh dài nhất 224px:
`battle 221×224 · summon-1 122×224 · summon-10 129×224 · key 203×224 ·
upgrade 192×224 · continue 224×178 · confirm 224×220 · cancel 224×223`.

**Mới gắn 3 chỗ** (nút chỉ có chữ trơn): Home BATTLE, Relics CONTINUE, Summon Results CONTINUE.

**Chưa gắn, và lý do** — không phải quên:
- `key` / `upgrade` ở OPEN ALL và nút Upgrade: hai nút đó **đã có sẵn** icon chìa khoá /
  đồng vàng kèm số. Thêm nữa thành hai icon cùng nghĩa.
- `summon-1` / `summon-10`: nút Summon đang dùng art SummonKit (003) có quả cầu vẽ sẵn,
  nhãn và pill giá đặt tay theo art đó. Đổi sang icon rời phải dựng lại bố cục trong nút;
  để user quyết giữ art cũ hay chuyển.
- `confirm` / `cancel`: nút Confirm Purchase ngắn, chữ đã đủ rõ.

Không sửa file của Codex. Chưa chạy Unity.
