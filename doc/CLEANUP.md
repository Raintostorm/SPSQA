# Dọn tài liệu — 2026-11-03

## Đã xóa 10 file lỗi thời
- `Handoff/00-BAT-DAU-TU-DAY.md`: chỉ mục cũ trỏ rebuild repo, đã có doc/README.
- `Handoff/05-CODE-MANG-SANG.md`: hướng dẫn copy code từ project cũ; không áp dụng runtime hiện tại.
- `Handoff/06-LO-TRINH.md`: milestone dựng game từ đầu đã qua.
- `Handoff/07-TU-LAM-VS-NHO-AI.md`: hướng dẫn AI cũ, thay quy ước phối hợp cụ thể.
- `Handoff/08-DON-DEP-REPO.md`: số liệu/Git cleanup của repo cũ.
- `Handoff/12-CHUYEN-SANG-MAN-DOC.md`: kế hoạch chuyển portrait đã thực hiện.
- `docs/reviews/2026-10-26-ui-audit.md`: audit UI cũ đã chồng lặp; phần còn cần QA được gộp vào UI_STATUS/WORK_QUEUE, không khẳng định lỗi cũ vẫn tồn tại.
- `docs/ui/UI_AUDIT_2026-10-29.md`: audit UI cũ đã chồng lặp; phần còn cần QA được gộp vào UI_STATUS/WORK_QUEUE, không khẳng định lỗi cũ vẫn tồn tại.
- `docs/ui/UI_BUG_AUDIT_2026-10-29.md`: audit UI cũ đã chồng lặp; phần còn cần QA được gộp vào UI_STATUS/WORK_QUEUE, không khẳng định lỗi cũ vẫn tồn tại.
- `docs/ui/UI_FOLLOWUP_2026-10-29.md`: audit UI cũ đã chồng lặp; phần còn cần QA được gộp vào UI_STATUS/WORK_QUEUE, không khẳng định lỗi cũ vẫn tồn tại.

## Đã thay 3 bản tổng hợp bằng tài liệu hiện hành
- `docs/plans/game-improvement-plan.md` → WORK_QUEUE / STATUS / GAME_RULES; bỏ checklist cũ và công thức 4 quân mâu thuẫn.
- `docs/plans/epic-effect-buffs.md` → BUFFS; bỏ lịch sử marker cũ/tile lifecycle mâu thuẫn, cập nhật 6/9 pool và VFX.
- `docs/ui/UI_PROGRESS_2026-10-31.md` → UI_STATUS; bỏ “buff chưa triển khai”, giữ inventory screen và ranh giới QA.

## Giữ lại
- 3 audit/coverage có dữ liệu kỹ thuật chuyển sang reference, gắn nhãn lịch sử và trạng thái cài đặt mới.
- 7 ghi chú kỹ thuật/design cũ chuyển sang reference/legacy, có cảnh báo không phải luật hiện hành.
- Không xóa README art, prompt, manifest, screenshot, asset, script, save hoặc lịch sử Git.
- Đổi liên kết có liên quan, README gốc; thêm CLAUDE.md và AGENTS.md làm điểm vào cùng tài liệu. Tool Python chỉ đổi comment đường dẫn, không đổi hành vi.

## Khôi phục nếu cần
Backup trước dọn ở máy local: `/private/tmp/spinsquad-docs-before-cleanup-20261103-110124.tar.gz`. Backup gồm README, docs và Handoff nguyên trạng trước thay đổi; không nằm trong context làm việc mặc định.
