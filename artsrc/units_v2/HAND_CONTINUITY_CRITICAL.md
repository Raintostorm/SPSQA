# NOTE SINH TỬ — không đổi tay cầm trang bị

Đây là cổng duyệt bắt buộc cho TẤT CẢ nhân vật và TẤT CẢ clip.

- Chốt tay theo giải phẫu nhân vật, không theo trái/phải của màn hình. Ghi tay nào giữ từng món trước khi sinh ảnh.
- Một món phải ở cùng tay qua idle → walk → attack → dead → skill và khi quay về idle. Xoay người, che khuất hoặc vung qua thân KHÔNG cho phép đổi tay.
- Kiểm chuỗi vai → khuỷu → cổ tay → cán vũ khí từng frame. Không chỉ nhìn vị trí mũi kiếm.
- Không lật gương riêng frame để sửa hướng. Không tráo kiếm/khiên, không mọc thêm tay, không biến đòn một tay thành hai tay.
- Vũ khí và bàn tay phải đi thành một chuyển động liên tục; cán nằm trong cùng bàn tay. Nếu buông vũ khí lúc chết phải có diễn biến buông rõ ràng, không nhảy sang tay kia.
- Với cung: tay giữ cung và tay kéo dây cố định. Với súng hai tay: tay cò và tay đỡ cố định.
- So frame cuối với frame đầu và so attack với idle. Chỉ duyệt khi cả vòng chuyển clip đúng.

## dead_common — kiểm tra lại 2026-10-09

Mẫu idle chốt: kiếm bên phải ảnh, khiên bên trái ảnh khi nhân vật hướng phải. Giữ đúng tay của mẫu này; không suy ra tên tay giải phẫu từ một ảnh 3/4 thiếu thông tin.

Attack chibi hiện tại KHÔNG ĐẠT: frame 1, 2, 7, 8 đặt kiếm bên trái và khiên bên phải, ngược cấu hình idle; frame 3–6 đổi bố cục/che khuất khiến chuỗi tay cầm không nhất quán. Frame 1 và 8 đã sai ngay tư thế nghỉ, không thể giải thích bằng quỹ đạo vung kiếm.

Thu hồi trạng thái duyệt attack và full demo skeleton. Chưa sửa ảnh trong lượt kiểm tra này. Không nhập bản này vào game, không dùng nó làm mẫu chuyển động cho nhân vật khác.

## Mandatory generation clause

HAND OWNERSHIP IS A HARD CONSTRAINT. Establish weapon and shield hand ownership from the approved idle reference. Preserve those SAME anatomical hands in EVERY frame and across EVERY animation. Screen position may change with a continuous swing, but anatomical ownership NEVER changes. Trace shoulder–elbow–wrist–grip continuity. Never mirror individual frames, swap weapon and shield, add a hand, or change a one-handed grip into a two-handed grip. The attack ready and recovery poses must match the approved idle equipment ownership. A hand-ownership failure rejects the entire clip, regardless of visual quality.
