# Bộ 6 nền V2

Sinh bằng imagegen tích hợp theo PROMPT_ARTDIRECTION_V2.md; đoạn STYLE bắt buộc giữ nguyên. Prompt cơ sở ở BG_V2_PROMPTS.md. Các lượt sửa nhấn mạnh một màu duy nhất trên toàn bề ngang vùng y đã chỉ định, không trang trí; Upgrade/Shop được sinh lại với cảnh chỉ ở phần đầu ảnh (Upgrade lần cuối yêu cầu 10%).

Tất cả xuất 1125×2436 bằng sips, không thay nền Assets đang chạy. Ảnh gốc lưu trong generated_images.

Độ lệch chuẩn từng kênh RGB trong vùng trống (thang 0–255):

| Nền | Vùng y | std RGB |
|---|---|---|
| prep | 900–2000 | 0.80 / 0.67 / 0.83 |
| battle | 1300–1950 | 0.84 / 0.70 / 0.80 |
| home | 600–2280 | 0.80 / 0.77 / 0.78 |
| treasure | 1060–2436 | 0.72 / 0.74 / 0.75 |
| upgrade | 400–2436 | 0.69 / 0.79 / 0.81 |
| shop | 400–2436 | 0.55 / 0.65 / 0.82 |

Vùng trống gần đều, nhưng không tuyệt đối một giá trị RGB. Phần trang trí vẫn có vài chuyển sắc/tô khối nhẹ, chưa đạt hoàn toàn yêu cầu vector phẳng tuyệt đối; cần người dùng duyệt trước khi nhập game. Chưa đo tỉ lệ viền bằng thuật toán của tài liệu (không có định nghĩa đo). Không làm thêm nhân vật.
