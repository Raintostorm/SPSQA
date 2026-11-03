# Checkpoint toàn bộ dự án · 2026-11-03
User yêu cầu commit và push toàn bộ tiến độ hiện có; cho phép checkpoint cả thay đổi đã stage từ các lượt trước.

1. Kiểm tra code runtime và regression buff/status/stage.
2. Commit Assets: code, scene, resource và .meta, giữ nguyên gameplay hiện tại.
3. Commit tools: pipeline import, kiểm tra và regression.
4. Commit artsrc/outdata: ảnh nguồn, demo, prompt, backup art và dữ liệu tham khảo. Đây là archive nghiên cứu, không phải toàn bộ đều dùng runtime.
5. Push main; đối chiếu hash remote và số C# trong tree.

Không publish Unity cache/build, khóa agent hoặc scene recovery tự tạo; giữ nguyên trên máy. Các mẫu APK đã trích là dữ liệu tham khảo, không phải source gameplay của dự án. Không thay đổi luật hoặc tái generate asset trong checkpoint này.

Compile runtime không thay thế Unity/device QA; combat VFX mới vẫn chờ người dùng test theo STATUS.
