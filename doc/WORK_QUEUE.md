# Việc tiếp theo
Cập nhật: 2026-11-03. Owner “—” nghĩa là chưa nhận việc; không phải đã giao cho một agent cụ thể.

| ID | Trạng thái | Owner | Việc / điều kiện nghiệm thu |
|---|---|---|---|
| DOC-001 | DONE | Codex | Dọn tài liệu cũ, thống nhất doc/, thêm entry point Codex/Claude và quy ước file ownership; đã kiểm tra liên kết nội bộ. |
| UI-001 | REVIEW | Codex; chờ user test | Guardian sẵn ở 1-3, burn đỏ + lửa động, mana orbit + nhiều status icon đã gắn. Chờ báo cáo combat; kiểm tra icon không đè HP/MP, lật hướng, chết/hồi sinh, pause/x1–x3 và Effects off. Không regenerate tiếp khi chưa có lỗi cụ thể. |
| UI-002 | READY | — | G06: buff card chỉ hiện 3 nhóm Mythic. Đề xuất +N/scroll để không mất nhóm thứ 4 trở đi; demo rồi user duyệt, scope đúng với logic. Files chính: EffectBuffChoiceView. |
| UI-003 | READY | — | QA vòng UI mới: Relic Details close/prev/next/scroll, Shop confirm và thiếu gems, Summon results dài, roster→detail→Back giữ vị trí. Chụp lỗi hiện tại, không sửa theo ảnh audit cũ. |
| UI-004 | READY | — | Kiểm tra iPhone X và một tỉ lệ khác: safe area, chữ số, tap targets, modal/raycast và Recipe/Item không che vị trí kéo thả. Đề xuất từng screen có lỗi thật. |
| TEST-001 | READY | — | G07: kiểm tra/sửa harness `tools/test_ally_stats.sh` theo Mythic hiện tại. Lần audit cũ lỗi stub; không kết luận runtime sai trước khi chạy lại. |
| PERF-001 | READY | — | G05: đo battle đông quân cùng focus/cùng kịch bản trước–sau. Ghi frame time, GC, số VFX; không dùng sample Editor bị background throttle làm benchmark. |
| CORE-001 | READY | — | Audit lại atomic save, chest debit/grant/pity và settlement phụ thuộc ResultPanel. Chỉ sửa sau khi xác định scope và regression tương ứng; không gộp vào polish UI. |
| DESIGN-001 | WAITING_DESIGN | — | Event chưa chốt loại/trigger/reward/persistence; bàn với user trước khi generate hay xây hệ event. |

Thứ tự đề xuất: phản hồi UI-001 → UI-002/UI-003 → TEST-001 → PERF-001. Claude Code có thể nhận UI-002 hoặc UI-003 trong khi Codex nhận phản hồi VFX; phải khóa file trước.
