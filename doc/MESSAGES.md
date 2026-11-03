# Sổ bàn giao chung
Ghi bổ sung ở cuối, không xóa lịch sử trao đổi của bên kia. Đây là file đọc khi bắt đầu công việc, không tự gửi notification.

## 2026-11-03 · Codex → Claude Code · Handoff ban đầu
- User muốn phối hợp để tăng tốc improve UI; chưa giao một task cụ thể cho Claude Code.
- Đọc README/STATUS/COORDINATION/WORK_QUEUE trước. Các bản trong reference/legacy chỉ để tra cứu, không dùng làm luật mới.
- VFX vừa gắn nhưng user chưa xác nhận combat; chapter 1-3 đã có Guardian để họ test.
- Có thể nhận UI-002 (overflow Mythic trên card) hoặc UI-003 (QA screen hiện tại). Không chỉnh BuffProcVisual/SlowFrostVisual/Shader cùng lúc với task VFX của Codex.
- Chưa có agent nào được khởi chạy hay tự kết nối qua việc tạo tài liệu này.

## 2026-11-03 · Codex → Claude Code · DOC-001 · DONE
- Hoàn tất thống nhất doc/, loại 10 file obsolete, thay 3 summary cũ, giữ 10 tài liệu kỹ thuật có nhãn lịch sử trong reference.
- README gốc, CLAUDE.md và AGENTS.md trỏ cùng tài liệu; không có background agent/messaging.
- Không thay runtime/assets/save trong task dọn doc. Tool Python chỉ đổi comment đường dẫn cũ.
- Entry cần đọc: doc/STATUS.md; việc độc lập có thể nhận: UI-002 hoặc UI-003.

2026-11-03 · Codex → Claude Code · GIT-001 · REVIEW
Theo yêu cầu user, checkpoint toàn bộ code/assets/tools/artsrc/outdata, giữ scene recovery và cache Unity ngoài Git. Compile 169 runtime C# và test_effect_buffs, test_status_tint, test_stage_buff_roll pass. Không đổi gameplay; combat VFX vẫn chờ user test. Đang push và đối chiếu remote.
