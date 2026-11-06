# INPUT 057 — checklist thực thi trọn lô

2026-11-05 · Codex · KẾ HOẠCH, chưa thực thi.
Nguồn hợp đồng: `chat/chatgpt.md`, INPUT 057. Đây là phần cụ thể hoá của
`doc/ART_ANIMATION_EXECUTION.md`, không tuyên bố art/Unity đã hoàn tất.

## Phạm vi và trình tự

1. Snapshot đầu vào và claim → 2. Sửa rương → 3. Sửa level-up →
4. Căn burst → 5. Soát D1 → 6. Kiểm art và gom bộ duyệt →
7. Import phần được duyệt → 8. Kiểm file/diff/compile nếu có thay đổi tài nguyên →
9. Báo cáo đủ sáu mục, kể cả mục bị chặn.

Không sửa `.cs`, không đụng dock Home, không đổi economy hoặc tự làm task animation của Claude.
Không generate lại bộ đã đạt. Không đánh dấu XONG chỉ vì demo chạy.
Lượt user hiện tại chỉ yêu cầu lập kế hoạch; chưa chạy các bước sửa/cắt/gắn dưới đây.

## 0. Khoá đầu vào

- Đọc lại claims và thay đổi mới của Claude trước mỗi đợt ghi; claim danh sách đường dẫn thực tế.
- Lưu danh sách file nguồn, SHA256, size, alpha, GUID trước thay đổi; giữ nguyên bản 051–056.
- Tạo nguồn mới có phiên bản, importer tái chạy được, manifest crop/pivot/scale.
- Tách staging trong `artsrc/ui/` khỏi runtime `Assets/_Project/Resources/`.
- Đối chiếu lời duyệt từng nguồn; Q1–Q5 là duyệt hướng thiết kế, không tự đồng nghĩa duyệt mọi PNG.

## 1. Rương — một master, hai lớp

**Làm:**
- Soát closed master hiện có. Nếu không có master đạt khe ngang, tạo MỘT rương đóng mới
  cùng concept 052; không yêu cầu máy tạo ảnh vẽ thân và nắp riêng.
- Master có đường khe ngang rõ, mép dưới nắp phẳng; ổ khoá thuộc thân.
- Cắt theo khe bằng script, không resample riêng hai mảnh, không feather đường cắt.
  Phần ổ khoá nhô lên trên khe được gán bằng mask cho thân và loại khỏi mask nắp.
- Hai mask phủ hết pixel gốc, không trùng vùng visible. Ghi offset của mỗi crop trong master,
  cùng hệ scale, hinge của nắp theo cạnh khe. Có thể dùng canvas khác cỡ nhưng không fit riêng.

**Nghiệm thu:**
- Ghép đúng offset trên canvas gốc; so toàn bộ RGBA với master:
  `different_pixels = 0`, `max_channel_error = 0`.
- `chat/check/057-chest-fit.png`: master / ghép lại / diff, kèm số đo thật.
- JSON ghi mask/crop/offset/pivot/scale/hash; kiểm mở nắp 0°, góc giữa, góc mở đầy.
  Pixel diff bằng 0 KHÔNG thay thế kiểm bản lề khi xoay.
- Phần bên trong chưa có art không được gọi là đã vẽ; không tự thêm đáy/miệng rương khác
  làm mất tính chứng minh hai mảnh từ cùng master.

**Bị chặn khi:** master mới chưa được duyệt, khe không đạt, hoặc phép ghép không bằng 0.
Vẫn hoàn thành sửa các mục khác trước khi báo cuối.

## 2. Level-up 053 — vòng rỗng

- Chỉ sửa chuỗi flash, giữ arrow/swirl đã đạt nếu không có lỗi mới.
- Bốn pha vòng trắng rỗng: vòng nhỏ → nở → vòng mỏng đứt → tan.
- Cùng canvas 256² và cùng tâm; không auto-fit từng pha cho cùng kích thước.
- Có alpha thật, RGB visible trắng 255; vùng tâm giữ trong suốt qua các pha còn vòng.
- Nếu máy generate vẫn tô đặc, sửa lại đúng vòng; không bỏ qua lỗi đã biết.

**Bằng chứng:** `chat/check/057-levelup-v2.png`, thứ tự 4 pha trên nền navy và sáng;
JSON alpha vùng tâm, bbox, size; demo tốc độ thật. Giữ source trước và sau sửa.

## 3. Burst 051 — cùng tâm, giữ quá trình nở/tan

- Đo từng pha từ source, loại vùng lẫn pha bên cạnh trước xuất.
- Dùng tâm đối xứng của tia và mốc giữa khi pha cuối chỉ còn tia rời;
  không dùng centroid alpha một cách mù quáng vì pha không đối xứng sẽ kéo lệch tâm.
- Xuất 6 canvas 512², cùng tâm (256,256), một hệ scale cho cả chuỗi.
  Ghi offset gốc và phép dịch từng frame. Không phóng riêng từng frame để lấp đầy khung.
- RGB trắng, alpha giữ độ sáng; padding đủ để không cắt tia.

**Bằng chứng:** `chat/check/057-burst-aligned.png`: sáu pha + overlay tint khác màu
chỉ dùng trong ảnh kiểm, crosshair chung. Kèm số mốc tâm/offset và demo playback.
Overlay một mình không chứng minh tâm đúng nếu chỉ đặt crosshair; cần đo từ hình thực.

## 4. Cắt/gắn — sau duyệt, không sửa code

| Nguồn đã đạt | Đích dưới `Assets/_Project/Resources/` | Cỡ |
|---|---|---|
| 054 glyph chart | `ButtonIcons/contribution.png` | 256² |
| 054 glyph arrow | `Glyphs/sort.png` | 150² |
| glow / swirl / chest-light / sparkle / rarity-ray | `Anim/Common/<slug>.png` | Theo manifest đã đối chiếu; 056 hiện 512² |
| Summon/orb tái dùng | `Anim/Summon/orb-core.png` | 512² |
| card-back 051 | `Anim/Summon/card-back.png` | 400×560 |
| Burst mục 3 | `Anim/Summon/burst-0..5.png` | 512² |
| Rương mục 1 | `Anim/Chest/chest-body.png`, `chest-lid.png` | Ghi cỡ thật, cùng scale và offset master |
| shard | `Anim/Chest/shard.png` | 96² |
| Flash mục 2 / arrow | `Anim/LevelUp/levelup-flash-0..3.png`, `levelup-arrow.png` | 256² |
| Swirl | `Anim/LevelUp/merge-swirl.png` | 512² |
| Ribbon slate | `ResultScreen/atlas.png` rect Unity (793,669,707,216) | 707×216 |

- Chuẩn hoá lại tên `rays` của preview thành `rarity-ray`; không tự tạo tên khác contract.
- Common/swirl và LevelUp/merge-swirl cùng source nếu cần cả hai path như INPUT;
  ghi rõ alias cùng nguồn, không generate hai bản.
- Trước atlas patch: backup PNG và `.meta` vào `artsrc/ui/retired-2026-11-05/057-backup/`.
  Chuyển rect Unity bottom-left sang top-left theo chiều cao atlas thật.
- Kiểm pixel ngoài rect không đổi, atlas giữ kích thước, GUID cũ giữ nguyên, backup hash khớp.
- Các file mới có `.meta` sprite, border 0; file tồn tại giữ `.meta`/GUID, kiểm import setting.
- Importer có dry-run liệt kê ghi đè và từ chối đích ngoài whitelist.
- Asset được duyệt mới import; nguồn chưa được user gật thì gom bằng chứng, ghi BỊ CHẶN DUYỆT,
  không tự cho đã duyệt để ép bảng thành XONG.
- “Đã đặt vào Resources” khác “đã được code gọi”; bàn giao path cho Claude nối C#.

## 5. D1 — kiểm từng tham chiếu thực tế

**Sửa cách đếm:** báo cáo 049 có 47 asset, trong đó **31 asset có dòng mã ứng viên**,
không phải tổng cộng 31 dòng. Phải duyệt mọi dòng dưới 31 asset này, đồng thời soát 16 asset
0/0 để tránh thiếu dynamic load và tools.

Mỗi dòng ghi: `asset · file:dòng hiện tại · helper/call chain · Resources key thực tế ·
có / không / không rõ · lý do`. Có nghĩa là có nhánh mã có thể nạp, không khẳng định đã
quan sát nhánh đó chạy trong Unity.

- Literal Resources key; key ghép chuỗi; helper ApprovedUiArt/ButtonFamilySkin; fallback;
  vòng lặp/array/mapping; Editor scripts và importer/tools cần mẫu `.meta`.
- Token trùng trong comment, tên stat, tên reward khác loader không được tính là dùng asset đó.
- Nhánh fallback hợp lệ vẫn là còn dùng, dù chưa chạy ở cấu hình hiện tại.
- Không rõ đường nạp → giữ nguyên và liệt kê cho Claude, không đoán chết.
- Quét GUID trong serialized Assets và tham chiếu khác trong repo theo phạm vi thực tế,
  loại chính `.meta` và backup để không tự tạo false positive; ghi rõ phạm vi quét.
- Chỉ dời khi không mã runtime/editor, không GUID consumer, không phụ thuộc tools/dynamic.
  Dời PNG/bytes **và `.meta` cùng nhau**, giữ đường dẫn tương đối dưới retired, có move manifest
  `old → new · hash · GUID · reason`; không xoá vĩnh viễn.
- Không dời cả folder chỉ vì một số file chết. Không sửa C# để làm asset trở thành chết.
- Sau dời chạy lại kiểm Resources key và importer dependency; lỗi mới phải khôi phục đúng file
  từ move manifest (không git restore/reset và không đụng thay đổi người khác).

**Output:** `chat/check/057-d1-verified.md`, JSON machine-readable, bảng giữ/dời/chưa rõ,
callsite cần Claude sửa. Giữ 049 làm lịch sử, không ghi đè báo cáo cũ để che khác biệt.

## 6. Báo cáo trọn lô

Chỉ viết OUTPUT 057 sau khi đã xử lý hết 6 mục (hoặc xác định blocker cụ thể cho từng mục).
Progress update ngắn vẫn được, nhưng không gọi là hoàn thành sớm.

| Mục | Trạng thái cuối | Bằng chứng bắt buộc |
|---|---|---|
| 1 Rương | XONG / BỊ CHẶN + lý do | fit PNG + RGBA diff JSON + hinge preview |
| 2 Level-up | XONG / BỊ CHẶN + lý do | v2 PNG + alpha tâm + playback |
| 3 Burst | XONG / BỊ CHẶN + lý do | overlay PNG + tâm/offset + playback |
| 4 Import | XONG / BỊ CHẶN + lý do | path/cỡ/GUID/border/hash, backup atlas, diff ngoài rect |
| 5 D1 | XONG / BỊ CHẶN + lý do | bảng từng callsite + keep/move manifest |
| 6 Báo cáo | XONG / BỊ CHẶN + lý do | OUTPUT 057 + danh sách file và hạn chế |

Ghi nguyên văn **“chưa chạy Unity”**. Không gọi production-ready hoặc tuyên bố toàn bộ
animation game xong. Chạy compile nếu cần xác minh project sau import/dọn; compile không
chứng minh alpha, pivot, Resources runtime hay chuyển động đúng.

Gom ảnh mới/corrected cần duyệt thành một contact sheet cuối lô; không hỏi rời từng ảnh.
