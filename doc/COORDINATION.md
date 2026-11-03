# Phối hợp Codex / Claude Code

## Cơ chế
Cùng đọc/ghi file trong repo này. Không có kết nối chat tự động hoặc tiến trình theo dõi Claude Code; mỗi bên phải đọc lại trạng thái trước khi bắt đầu. User vẫn là người quyết định hướng thiết kế và chốt demo.

## Bắt đầu một việc
1. Đọc STATUS, GAME_RULES, WORK_QUEUE và `git status --short`.
2. Chọn một task chưa có owner. Trước khi sửa, tạo khóa bằng `mkdir doc/.claims/UI-002` (thay bằng ID thật). `mkdir` phải thành công; nếu đã tồn tại thì đọc owner và chọn việc khác, không ghi đè hay xóa khóa đó.
3. Ghi `owner.md` trong khóa: agent, thời gian, mục tiêu và danh sách file dự định sửa. Cập nhật owner/trạng thái task trong WORK_QUEUE. Khóa là cơ chế nhận việc; bảng việc chỉ là tóm tắt.
4. Không sửa file đang được bên kia giữ. Nếu task khác ID nhưng đụng chung file, ghi yêu cầu trong MESSAGES và chờ bên đang giữ bàn giao; có thể làm phần độc lập trước.

## Trong khi làm
- Mỗi task nhỏ, có điều kiện nghiệm thu; không refactor toàn hệ UI hoặc đổi luật trong một lượt polish.
- Không đổi giá, shard, pity, skill, công thức Mythic hoặc test grants trừ khi user yêu cầu.
- Ý tưởng art/UI mới: đưa demo để user duyệt trước khi gắn. Demo được duyệt rồi thì gắn trong phạm vi đã chốt, không hỏi lại.
- Tránh hai agent cùng điều khiển Unity. Ghi rõ ai đang kiểm tra Play Mode; nếu bên kia dùng Editor thì kiểm tra mã/asset độc lập trước.
- Giữ PNG nguồn, manifest crop, GUID `.meta`, pivot/scale và thông tin prompt. Không xoá asset vì không thấy trong scene: nhiều asset được nạp bằng Resources/runtime manifest.
- Tôn trọng thay đổi đang có. Không `git reset --hard`, không restore file của bên kia, không stage/commit toàn repo.

## Bàn giao
1. Ghi kết quả vào MESSAGES: ID, file đã đổi, check đã chạy, giới hạn và ảnh/demo nếu có.
2. Sửa trạng thái task: READY → IN_PROGRESS → REVIEW → DONE; BLOCKED phải ghi điều kiện cụ thể. Chỉ DONE khi đạt điều kiện nghiệm thu.
3. Cập nhật STATUS/GAME_RULES nếu thực sự đổi hành vi. Không ghi “đã test” khi chỉ compile.
4. Xóa riêng thư mục claim do mình tạo sau khi đã bàn giao; không bỏ khóa âm thầm khi còn sửa dở.

## Mẫu tin nhắn
```
2026-11-03 · Codex → Claude Code · UI-002 · REVIEW
Files: ...
Kết quả: ...
Kiểm tra: ...
Chưa xác minh / cần người dùng: ...
```
