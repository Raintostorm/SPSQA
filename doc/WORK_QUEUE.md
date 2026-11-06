# Việc tiếp theo
Cập nhật: 2026-11-03. Owner “—” nghĩa là chưa nhận việc; không phải đã giao cho một agent cụ thể.

| ID | Trạng thái | Owner | Việc / điều kiện nghiệm thu |
|---|---|---|---|
| GIT-001 | DONE | Codex | Theo yêu cầu user: kiểm tra và commit/push toàn bộ code, asset, tools và art nguồn hiện có theo nhóm; giữ file tạm và dữ liệu APK trích ngoài runtime trên máy. |
| DOC-001 | DONE | Codex | Dọn tài liệu cũ, thống nhất doc/, thêm entry point Codex/Claude và quy ước file ownership; đã kiểm tra liên kết nội bộ. |
| UI-001 | REVIEW | Codex; chờ user test | Guardian sẵn ở 1-3, burn đỏ + lửa động, mana orbit + nhiều status icon đã gắn. Chờ báo cáo combat; kiểm tra icon không đè HP/MP, lật hướng, chết/hồi sinh, pause/x1–x3 và Effects off. Không regenerate tiếp khi chưa có lỗi cụ thể. |
| UI-002 | READY | — | G06: buff card chỉ hiện 3 nhóm Mythic. Đề xuất +N/scroll để không mất nhóm thứ 4 trở đi; demo rồi user duyệt, scope đúng với logic. Files chính: EffectBuffChoiceView. |
| UI-003 | READY | — | QA vòng UI mới: Relic Details close/prev/next/scroll, Shop confirm và thiếu gems, Summon results dài, roster→detail→Back giữ vị trí. Chụp lỗi hiện tại, không sửa theo ảnh audit cũ. |
| UI-004 | READY | — | Kiểm tra iPhone X và một tỉ lệ khác: safe area, chữ số, tap targets, modal/raycast và Recipe/Item không che vị trí kéo thả. Đề xuất từng screen có lỗi thật. |
| TEST-001 | READY | — | G07: kiểm tra/sửa harness `tools/test_ally_stats.sh` theo Mythic hiện tại. Lần audit cũ lỗi stub; không kết luận runtime sai trước khi chạy lại. |
| PERF-001 | READY | — | G05: đo battle đông quân cùng focus/cùng kịch bản trước–sau. Ghi frame time, GC, số VFX; không dùng sample Editor bị background throttle làm benchmark. |
| CORE-001 | DONE | Claude Code | Save nguyên tử + khôi phục, mở rương một giao dịch, thưởng qua màn ở BattleFlow.Settlement, hoà = thua, Second Wind cứu hoà — `tools/test_save_transactions.sh`. User xác nhận Play Mode chạy bình thường 2026-11-03. |
| FONT-001 | DONE | Codex | Lilita One có sẵn; thêm Fredoka SemiBold static 600, OFL và nguồn trong Resources/Fonts; chưa nối UI. |
| UI-005 | DONE | Claude Code | Font Lilita/Fredoka (`UiFonts`), ruy băng + back chung (`ScreenChrome`) cho Rewards/Codex/Details/Select Chapter/Summon(+Results)/Shop/Mercenaries/Upgrade/Relics/Chọn buff; art chat 001–008 đã cắt; head v3 đồng đều; `SpriteFit`. User xác nhận trong Play Mode 2026-11-03. Chi tiết: `doc/UI_PLAN.md`. |
| UI-006 | DONE | Claude Code | Hệ thống nút: gom các skin nút rải ở ~30 file UI về một chỗ. Quy ước màu — vàng = hành động chính, xanh = phụ, xám tối = back/huỷ, đỏ = nguy hiểm; mỗi kiểu có normal/pressed/disabled rõ ràng, tap target ≥ 44pt. Nghiệm thu: Battle/Confirm/Open All/Summon/Back/Continue/Shop đúng vai trò màu. **Xong 04/11**: bộ nút vẽ lại hẹp (`ButtonsV2`, viền khoá 48%, xem `doc/UI_ART_SPEC.md`), icon nút ở `ButtonIcons`, nút tắt làm xám cả nội dung. Còn lại: nút tròn/☰ chờ art 012; tap target kiểm ở UI-004. |
| UI-007 | DONE | Claude Code | Battle HUD: ẩn stat +0 %, hàng icon buff không đè cảnh, nút tốc độ x1 cố định một chỗ ở prep và fight, Gold gộp vào cụm Roll, chữ hướng dẫn đủ đọc, bảng Damage thu gọn để sân đánh rộng hơn. **Xong 04/11**: ẩn ô chỉ số khi phần trăm làm tròn ra 0 và dồn ô còn lại, nền dải co theo số ô; nút tốc độ cố định góc trên trái cả hai giai đoạn; chữ gợi ý 26→34; Roll/Start theo vai trò màu; bảng Damage 620→470; Gold hạ vào thanh đáy. Hàng ô buff mặt đất (Blood/Gold Tile) **không phải lỗi** — chỉ hiện ở Prep, user xác nhận. |
| UI-008 | DONE | Claude Code | Báo hiệu: chấm đỏ trên dock Home; relic đủ shard có mũi tên ⬆ và thanh đổi màu; đã nhận/chưa nhận phân biệt ở Rewards; Select Chapter có ✓ màn đã qua. **Xong 04/11**: chấm đỏ Lính/Relic trên dock Home (`HomeSignals`, quét lại mỗi 0.5s, dùng ĐÚNG điều kiện của huy hiệu ⬆ — chỉ xét mảnh, không xét vàng); ✓ cho màn đã qua ở Select Chapter; ✓ + chữ xanh cho quái đã nhận thưởng ở Rewards/Codex. Huy hiệu ⬆ relic/lính đã có sẵn từ trước. Icon dock giữ bộ cũ — user từ chối bộ generate mới. |
| UI-009 | READY | — | Còn lại theo ảnh user 03/11: Home (bỏ trống nửa trên, chọn chapter cạnh BATTLE, chữ dưới dock), Shop (thẻ navy, sửa crop Haunted Tank, bỏ thanh dưới 1 tab), Summon (tỉ lệ + bộ đếm bảo hiểm), Rewards (ẩn thưởng 0, chữ đè ảnh), kiểm số wave 1-2 = 10. Thẻ buff gộp vào UI-002 (đưa Mythic vào trong thẻ, tiêu đề không phải nút). |
| UI-010 | READY | — | Soát lại bộ icon (xem `doc/UI_PLAN.md` giai đoạn 9). Nhóm 3: xoá art chết (`settings-polish`, `recruit`, `btn_blue`, `btn_primary`) — **không xoá nhầm `Art/UI/btn_blue.png.meta`, đó là khuôn .meta của tools/**. Nhóm 1: một khái niệm hai hình (relic, chìa khoá, vàng, nâng cấp) — chọn một bản, sửa bằng mã. Nhóm 2: `restart`/`resume`/`sound`/`vibration` lạc concept — cần ChatGPT generate; `close` chuyển sang `Glyphs/close` đã có. Phương pháp soát: `tools/icon_contact_sheet.py`, xem ở cỡ hiển thị thật chứ không xem file gốc. |
| ANIM-001 | READY | — | Hạ tầng hoạt ảnh: helper `UiTween` (scale/fade/slide/số chạy + Ease), cơ chế bỏ qua dùng chung, bảng thời lượng chuẩn. **Bắt buộc dùng unscaledDeltaTime** — nút tốc độ x1/x2/x3 đổi `Time.timeScale`. Các ANIM sau đều dựa vào đây. |
| ANIM-002 | READY | — | Hoạt ảnh triệu hồi: cầu sáng → loé → thẻ lật lần lượt, nhịp theo độ hiếm, ×10 hiện dần + nút bỏ qua. Hiệu ứng hiếm **không được hiện trước khi thẻ lật**. Hiện `GemEconomyScreen`/`SummonResultsView` không có hoạt ảnh nào. |
| ANIM-003 | READY | — | Hoạt ảnh mở rương relic: rương rung → bật nắp → shard bay vào thanh đếm rồi số mới nhảy. Open All nhiều chìa gộp thành một chuỗi. |
| ANIM-004 | READY | — | Hoạt ảnh nâng cấp/gộp: thanh shard chạy đầy, số chạy dần (`AnimatedCounter`), loé khi lên level, gộp Mythic (`MergeReveal`). |
| ANIM-005 | READY | — | Thưởng cuối trận bay vào ví, số trong ví chạy dần. `CoinRewardEffect` đã có — kiểm lại trước khi viết mới. |
| ANIM-006 | READY | — | Chuyển màn mờ dần thay cắt cứng; popup phóng nhẹ 0.92→1. |
| DESIGN-001 | WAITING_DESIGN | — | Event chưa chốt loại/trigger/reward/persistence; bàn với user trước khi generate hay xây hệ event. |

Kế hoạch UI chi tiết từng bước: `doc/UI_PLAN.md`. Thứ tự đề xuất (user 03/11 ưu tiên UI): UI-005 → UI-006 → UI-007 → UI-008 → UI-002 → UI-009; song song phản hồi UI-001. TEST-001/PERF-001 sau. Claude Code có thể nhận UI-002 hoặc UI-003 trong khi Codex nhận phản hồi VFX; phải khóa file trước.

| ART-ANIM-056 | REVIEW | Codex | Chuẩn hoá lớp art vào demo riêng; generate A3 còn thiếu; kiểm alpha, chuyển động và ghi handoff. Không sửa C# hay gắn art chưa duyệt vào runtime. |

| PLAN-057 | DONE | Codex | Đối chiếu INPUT 057, lập kế hoạch đủ 6 mục và tiêu chí bằng chứng; chưa triển khai asset. |

| ART-057 | REVIEW | Codex | INPUT057 sáu mục art/align/import/D1/evidence; không sửa C#. |

| ART-058 | REVIEW | Codex | Gắn057 đã duyệt trừDefeat; recolor ribbon gốc, preview chờ duyệt; không C#. |

| ART-059 | REVIEW | Codex | 7 lớp cầu năng lượng, stack/GIF/RGB-center proof, preview trước import; không C#. |

| ART-060 | REVIEW | Codex | Rương mở giữ body pixel gốc và cột sáng mới; gom duyệt059+060. |

| ART-061 | DONE | Codex | Gradient script core/flash/mote; gắn059+060 được duyệt,backup/GUID/check. |

| ART-062 | DONE | Codex | Gradient toán học4lớp inclchest-light mềm/loe; reimport059+060, giữmeta. |

| UI-063 | REVIEW | Codex | Relics header/gallery cùng cuộn; patch riêng, chờ bàn giao TreasureReviewUi từ ANIM003. |

| ART-063 | REVIEW | Codex | Lớp chibi LevelUp/Relic/Result + 3GIF/timing/board; chờ duyệt trước import. |

| PUBLISH-065 | WAITING_APPROVAL | Codex | User cho phép commit/push toàn bộ còn lại; kiểmchapter1-2/testharness; ghi mứcQAthật. |
