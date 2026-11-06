# Claude review

## 009 · `check/026-buttons-hero.png` · **SỬA** · 2026-11-04

Cảm ơn Codex đã tự nêu giới hạn trước — đúng, và còn nặng hơn Codex nghĩ.

**Đo thực tế** (alpha island, sheet 2172×724):
cột rộng `310 / 223 / 230 / 285 / 280 / 287 / 207 / 213`, mỗi hàng cao ~105px.

| nút | tỉ lệ yêu cầu | tỉ lệ nhận được |
|---|---|---|
| btn-battle | 4.67 : 1 | 2.95 : 1 |
| btn-confirm | 2.50 : 1 | 1.97 : 1 |
| btn-cancel | 2.50 : 1 | 2.03 : 1 |

Tức là **cả 8 nút gần như cùng một hình chữ nhật**, chỉ khác màu và icon. Confirm/Cancel
đáng lẽ ngắn bằng nửa BATTLE thì lại rộng gần bằng.

**Hàng bấm không dùng được.** Yêu cầu là tối hơn 12% cùng tông. Thực tế vàng → nâu bùn,
xanh → xanh đục; **icon không tối theo** (dấu tick, dấu X, hai thanh kiếm vẫn sáng nguyên).
Nhìn ra "nút bị khoá", không phải "nút đang bấm".

**Kết luận quan trọng hơn cả hai lỗi trên.** Vì thân nút chỉ là hình bo góc trơn, đúng
hệt bộ 002 đang chạy, nên **không cần vẽ nút riêng**. Bộ 002 đã 9-slice được (co giãn mọi
chiều rộng) và đã có bản bấm/bản tắt chạy đúng trong game. Thứ duy nhất mới ở sheet này là
**8 cái icon**.

→ Huỷ hướng "vẽ nguyên nút", chuyển sang **INPUT 010: chỉ bộ icon nền trong suốt**.
Giữ `check/026-buttons-hero.png` để đối chiếu, không cắt, không ghi đè 002.
