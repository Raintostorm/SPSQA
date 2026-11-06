# Trao đổi Codex / Claude qua file

## Mỗi lượt nhận yêu cầu
1. Đọc `chatgpt.md`, `claude.md` và ảnh liên quan trong `check/` trước khi làm; đối chiếu task/khóa ở `doc/`.
2. Khi nhận batch INPUT mới: lưu prompt, kết quả và quyết định batch trước vào `archive/<ngày>-<ID>/` hoặc README art tương ứng; sau đó xóa nội dung batch trước khỏi hai file chat. Không xóa ảnh. Việc chưa xong phải mang sang bản tóm tắt bàn giao, không bỏ mất.
3. Generate đúng prompt, tự kiểm tra ảnh; đặt PNG gốc vào `check/<ID>-<tên>.png`. Bản sửa dùng hậu tố `-v2`, `-v3`, không ghi đè ảnh đã review.
4. Codex ghi OUTPUT trong `chatgpt.md`: file, kích thước, alpha, lỗi còn lại, nơi đề xuất đặt asset. Claude ghi REVIEW/đề xuất triển khai trong `claude.md`.
5. Hai bên thống nhất thư mục/đặt tên qua hai file; không ghi đè bộ asset đang dùng. Nếu chưa có đường dẫn do bên kia yêu cầu, dùng thư mục mới theo ID/version và ghi rõ. Kiểu hình mới vẫn cần user duyệt trước tích hợp; review kỹ thuật không phải user approval.

## Phân công file
- `chatgpt.md`: INPUT từ Claude + OUTPUT từ Codex. Codex giữ nguyên INPUT đang xử lý; chỉ dọn batch cũ sau khi lưu trữ.
- `claude.md`: review/triển khai của Claude; Codex không viết thay nhận xét. Khi dọn batch cũ, được lưu trữ và bỏ phần đã hoàn tất theo yêu cầu user.
- `check/`: ảnh nguồn và screenshot để kiểm tra; nằm ngoài Assets nên Unity không import.
- `archive/`: lịch sử batch đã dọn để tra cứu; không nạp lại mặc định.

Chưa có watcher hay cơ chế tự đánh thức: mỗi agent đọc file khi bắt đầu lượt làm việc. `doc/.claims/` vẫn quản lý quyền sửa file, `doc/WORK_QUEUE.md` quản lý tiến độ và `doc/MESSAGES.md` giữ bàn giao chính thức.


## Luật cắt và gắn art (user chốt 2026-11-04)

Trước đây: Codex generate → Claude xem → **Claude tự cắt và gắn**. Tốn nhiều lượt và
nhiều token cho việc cơ học.

Từ nay:
1. Claude viết INPUT, trong đó **bắt buộc có mục "Cắt và gắn luôn"**: đường dẫn đích,
   kích thước chuẩn hoá, viền 9-slice, file nào bị ghi đè và sao lưu đi đâu.
2. Codex generate → lưu ảnh vào `chat/check/`.
3. **User xem ảnh và đồng ý** — bước này không bỏ được, art mới luôn cần user duyệt.
4. Codex cắt, gắn, viết script vào `tools/`, báo lại ở OUTPUT.
5. Claude **vẫn kiểm**: đúng chỗ chưa, có ghi đè nhầm không, mã có cần đổi đường dẫn không.
   Claude không cắt lại nếu Codex đã làm đúng.

Codex **không** được sửa file mã đang có người khác khoá trong `doc/.claims/`.
