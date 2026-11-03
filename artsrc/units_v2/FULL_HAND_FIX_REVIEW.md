# dead_common — bộ thử đủ bốn animation

Sinh bằng imagegen tích hợp, dùng identity từ sheet walk cũ; STYLE lấy nguyên từ outdata/PROMPT_NHANVAT_V2.md. Không nhập Assets/game. Bốn sheet *_handfix.png giữ alpha gốc.

Prompt yêu cầu kiếm luôn tay phải, khiên tay trái; ngón nắm chuôi sau crossguard, không đổi tay; walk vung khuỷu theo bước, attack ready–raise–overhead–downslash–strike–followthrough–recover–ready, dead ngã ra sau trái rồi nằm ngửa. Lượt sửa walk điều chỉnh kiếm theo tay ở frame1/4/5/8.

Tất cả tách được tám cụm chính. Lệch baseline: idle0px, walk2px, attack1px. Dead14px qua quá trình ngã; chưa đạt điều kiện baseline cũ. Khe vài frame thấp hơn80px; palette vẫn nhiều sắc độ, chưa chứng nhận năm màu hoặc metric viền6–11%. Đây là bản duyệt chuyển động, không phải bộ production đã đạt tất cả.

Preview dùng một scale chung, không cắt ô vuông, không rescale từng frame. Các cụm được cô lập để không lấy nhầm parts của frame bên cạnh khi hình vươn rộng. Attack/dead vẫn cần người dùng duyệt; dead giữ frame cuối2giây trong full preview rồi vòng lại đầu demo, không có hiệu ứng xuất hồn.
