# Claude review

## 039 · `check/039-home-icons-2.png` · **KHÔNG DÙNG** · user quyết 2026-11-04

User giữ bộ icon dock cũ (`shop`, `orb`, `mercenaries`, `relics` trong `Resources/HomeChrome`
và `Resources/Summon`). Bộ mới bị chê **"quá một màu"** — đúng nhận xét: 039 vẫn đi theo
hướng tối giản của 038, chỉ bớt tệ hơn, chưa bằng độ phong phú của bộ cũ.

**Codex ĐỪNG cắt, ĐỪNG ghi đè `HomeChrome/mercenaries.png`.** INPUT 015 huỷ.

Claude đã trả `HomeChrome.cs` về đọc `Summon/orb` như cũ; không còn `HomeChrome/summon`.

## Việc còn lại · chỉ 2 ký hiệu, đã xong
`037-glyphs-extra.png` (replay, home) **đã CHỐT và đã cắt** vào `Resources/Glyphs/`.
Đó mới là thứ user cần sửa: nút RETRY và HOME ở màn kết quả trận trước đây dùng ký tự
Unicode `↻` `⌂` của font hệ thống.

## 042 · Audit asset + gắn tái dùng · **ĐẠT** · 2026-11-04

Codex soát trước khi vẽ và kết luận **không nhóm art nào bắt buộc phải generate**. Tôi đã
tự kiểm lại và xác nhận: `ResultPanel.cs:145` thật sự dùng `ResultScreenArt.Get(index)`,
nên **giai đoạn B trong kế hoạch của tôi là sai** — ru-băng Thắng/Thua đã có trong
`ResultScreen/atlas.png` từ trước.

Đã gắn và đã biên dịch sạch (173 nguồn):
- `SharedSettings.cs` — Restart→`Glyphs/replay`, Home/Leave→`Glyphs/home`, Close→`Glyphs/close`;
  nút mở menu thành đĩa tròn `SmallButtons/navy` + `Glyphs/menu`; các nút trong menu và hộp
  xác nhận chuyển sang `ScreenChrome.Action` theo vai trò; font hệ thống → `UiFonts.Display`.
- `CoinRewardEffect.cs` — `SharedUI/gold` (túi) → `HomeChrome/coin`, đồng nhất với ví.

Thứ tự gọi đúng: `BattleUiSkin.Button` chạy trước `ScreenChrome.Action`, nên art vai trò
đè lên chứ không bị đè ngược — đây là lỗi đã mắc ở 033.

**Còn treo, chờ người dùng:** icon `vibration` vẫn là điện thoại hiện đại.
**Chưa chạy Unity** — mới biên dịch, chưa Play Mode.

## 047 + 048 · **ĐẠT** · 2026-11-05
- 048 item trong suốt: sạch trên cả nền navy và nền sáng, không còn ô nền xanh. Đạt.
- 047: Claude đã sửa mục 1,2,3,5 — `BattleUiSkin.Icon` nhận đường dẫn đầy đủ khi tên có "/";
  Leave→`Glyphs/home`, Close (menu + nút X panel)→`Glyphs/close`, Sell→`HomeChrome/coin`,
  ô menu dock ☰→`Glyphs/menu`, nút › Mythic→`Glyphs/next` (giữ chữ làm dự phòng).
- Còn treo: mục 4 (icon Contribution biểu đồ — cần art mới, chờ duyệt), mục 6 (mũi tên sort ↑↓).
Biên dịch sạch; **chưa chạy Unity**.

## 049–056 · demo duyệt · 2026-11-05 · Claude review (chưa phải user duyệt)
- 054 G1 glyph `contribution` + `sort`: **ĐẠT** — viền đen dày, đọc rõ ở cỡ nhỏ. Cắt được.
- 056 lớp sáng (glow, swirl) + arrow + orb tái dùng: **ĐẠT** để làm lớp. `chest-light` hơi mờ/nhỏ — Claude tăng alpha bằng mã được, không cần vẽ lại.
- 052 rương: **CHƯA ĐẠT** — Codex tự nêu đúng. Rương đóng cao hơn thân+nắp ghép lại, ổ khoá nằm hẳn ở thân trong khi bản đóng nằm vắt qua khe. Cách sửa: vẽ MỘT rương đóng làm gốc rồi cắt ngang ở đường khe → thân và nắp khớp mép tuyệt đối. Không vẽ lại từ đầu thành hai vật riêng.
- 051 burst / 053 level-up: chưa xem chi tiết; Codex nói vòng 1 của 053 bị tô đặc thay vì vòng — sửa trước khi cắt.
- D1 049: chỉ báo cáo, đúng luật. Claude sẽ xác minh 31 dòng mã trước khi dời.

## 057 · Claude review · 2026-11-05 (chưa phải user duyệt)
- Rương: **ĐẠT khớp** (diff 0, cùng master). Lưu ý chuyển động: mở 80° chỉ thấy nắp dẹt xuống, không thấy miệng rương → khi mở Claude sẽ phủ `chest-light` lên khe để che, không cần vẽ mặt trong.
- Level-up 4 vòng rỗng: **ĐẠT**. Burst 0–5: **ĐẠT**; ô "6 overlay" chỉ là ảnh chứng minh, **không gắn**.
- Ribbon Defeat xám: **CHƯA ĐẠT** — khác dáng bản Victory/Defeat cũ (đuôi khía chữ V, vành tím) mà lại thêm viền vàng + bóng kính. Phải giữ **đúng dáng** ribbon cũ, chỉ đổi màu sang xám đá, không thêm viền vàng.
- D1: dời 20 asset, biên dịch sạch 173 nguồn. Chưa chạy Unity.

## 058 · Claude review · 2026-11-05
- 24 sprite đã gắn vào `Resources/Anim/{Common,Summon,Chest,LevelUp}` + contribution + sort: đủ, đúng danh sách. Biên dịch sạch, chưa chạy Unity.
- Defeat xám v2: **ĐẠT** — tô lại đúng dáng ribbon tím gốc, không viền vàng. Chờ user gật để Codex ghi vào atlas.

## 059 · Claude review · 2026-11-05
- swirl-a/b, rune-circle, streak: **ĐẠT**.
- orb-core, flash (và mote): **gần đạt** — mặt bị **lấm tấm hạt nhiễu** (rõ trên core). Sửa: dựng lại 3 lớp này bằng **gradient tròn toán học** (PIL, script trong tools/), không cần máy tạo ảnh — mịn tuyệt đối, tâm đúng tuyệt đối. core giữ trắng→#BFEFFF.
- Mã đã viết sẵn dùng đủ 7 lớp (SummonReveal), thiếu lớp nào thì bỏ lớp đó. Gắn xong là chạy.

## 060 · Claude review · 2026-11-05
- chest-open: **ĐẠT** — thân trùng pixel (diff 0), nắp mở thấy mặt trong, lòng rương sáng. Mã ANIM-003 tự dùng khi gắn.
- chest-light: **tạm đạt** — đã thành một cột, nhưng đáy cắt thẳng cứng và mép trên lấm tấm. Nên dựng lại bằng gradient toán học như core/flash (đáy mềm, loe nhẹ).
- Chưa thấy báo đã dựng lại orb-core/flash/mote bằng gradient (yêu cầu sau 059).
