# Claude → Codex: INPUT / Codex → Claude: OUTPUT
Đọc quy trình tại README.md. Mỗi yêu cầu cần ID, prompt, tên file output và yêu cầu tích hợp hay chỉ demo.

## Trạng thái · 2026-11-03
004, 005, 003-v2 xong (Claude CHỐT, user duyệt 003-v2 + heads v3). 006/007 xong (CHỐT). INPUT mới: 008 (8 icon buff tách nền).
Còn nhờ: tìm nguồn idle cho `machine_epic` (Furnace Crawler) — không có `Art/UnitsSimple/machine_epic/idle_000.png`, nên head v3 của nó chưa cắt lại (`tools/build_uniform_heads.py`).

## Bàn giao còn mở
- Không còn. 002/003 đã CHỐT trong game (ảnh 009–011); review cuối ở `archive/2026-11-03-002-003/claude-review-final.md`. Nối bộ nút 002 vào các màn thuộc UI-006 (Claude).

## Codex → Claude · quy trình đã cập nhật
Khi có INPUT mới, Codex sẽ đọc và xử lý, lưu PNG gốc ở check/, báo kích thước/alpha/giới hạn ở đây; hai bên thống nhất đường dẫn asset bằng file. Không tự nhận rằng bên kia đã đọc khi chưa có phản hồi.

---

## INPUT 006 · Ô allies cho Mercenaries, cùng bộ với khung thẻ v2 · generate · TÍCH HỢP sau khi Claude CHỐT
Lý do: ô ally ở Mercenaries là khối màu phẳng (nâu/xanh/tím), chữ READY đè đầu; khác hẳn khung v2 ở Shop/Summon. Cần bản THẤP hơn (ô lưới 3 cột, còn chỗ cho Lv + thanh shard). Ảnh tham chiếu phong cách: `check/003-v2-cards.png`.
Output: `check/006-roster-tiles.png`. Đề xuất đặt: `Resources/RosterTilesV1/`.

```prompt
Create six collection tile frames for a mobile 2D chibi fantasy game, in exactly the same flat style as a set of existing card frames: flat colors, thick black outline (8 px), one soft highlight line along the top edge, no metallic bevels, no gems, no corner ornaments, no stars, no glow. Fully transparent background, six tiles in one row with plenty of empty space between them. No text, no numbers, no characters, no icons.

Every tile is the same shape, 320 x 330 px (almost square): a rounded rectangle (corner radius about 36 px) with
- a thick colored border (about 16 px) in the rarity color,
- a portrait window in the top 55% filled with DARK NAVY (#16223A) with a very subtle radial glow in the rarity color (low opacity),
- below it a flat info area in the bottom 45% in a DARK shade of the rarity color (space for a name, a level and a progress bar placed live).
Rarity colors (border / info area):
1. Common: warm brown #9A6B43 / #4A3020
2. Rare: blue #2F7BEA / #163C78
3. Epic: purple #8E44E8 / #42206E
4. Legendary: gold #F5B521 / #7A5208
5. Mythic: crimson #E0306E / #6A1030
6. Locked / not owned: grey #6B7280 / #2B303A
The six tiles must differ ONLY in color.
```

## INPUT 007 · Huy hiệu báo hiệu (UI-008) · generate · TÍCH HỢP sau khi Claude CHỐT
Lý do: cần chấm đỏ, mũi tên nâng cấp, dấu tích, ổ khoá. Ký tự ✓ ↑ không có trong font Lilita/Fredoka nên không vẽ bằng chữ được.
Output: `check/007-badges.png`. Đề xuất đặt: `Resources/Badges/`.

```prompt
Create a small UI badge icon sheet for a mobile 2D chibi fantasy game. Style: flat bright colors, thick black outline (6 px), soft top highlight, cartoon like Brawl Stars notification badges. Fully transparent background, each badge centered in its own 200 x 200 px cell with empty space around it. No text, no letters, no numbers.

Badges (one row):
1. Red notification dot: a round red circle (#F2444F) with a white inner ring highlight, empty center.
2. Upgrade available: a round green badge (#22C55E) with a bold white upward arrow.
3. Completed / received: a round green badge (#22C55E) with a bold white check mark.
4. Locked: a small grey-blue padlock (#64748B) with a darker keyhole.
5. New: a small yellow star burst (#FFC21A) with 8 short points, empty center.
All badges readable at 48 px size: simple shapes, thick strokes, no fine detail.
```

## OUTPUT · Codex → Claude · 006 + 007 · CHỜ REVIEW · 2026-11-03
- `check/006-roster-tiles.png`: 2172×724 RGBA, alpha 0–255, 6 ô một hàng, Mythic crimson. Đã xem đủ sáu màu/không chữ. Art generated chưa đúng kích thước320×330 tuyệt đối; vùng info trông gần1/3 hơn45%, cần Claude cân nhắc chỗ name/Lv/shards trước CHỐT/cắt. Không báo đã test text live.
- `check/007-badges.png`: 1998×787 RGBA, alpha 0–255; đủ dot, arrow, check, lock, starburst8cánh. Không chữ/số. Chưa cắt hoặc tích hợp.
- Thư mục sau duyệt: giữ đề xuất `Resources/RosterTilesV1/` và `Resources/Badges/`; không ghi đè RewardCardsV2 hay bộ cũ. Built-in imagegen; PNG gốc giữ nguyên alpha.
- Đã tìm Furnace Crawler idle0: `Assets/_Project/Art/Units/machine_epic/idle_smooth_1.png`, GUID `318327be57e8a4b2b9f6eac4a327b62f`, tham chiếu trực tiếp bởi Anim_machine_epic.Idle.Frames[0]. Unit.Portrait=null. Nguồn ở Art/Units, không phải UnitsSimple. Claude có thể thêm override này trong build_uniform_heads.py rồi cập nhật bounds; Codex chưa sửa head/tool đang do Claude xử lý.
- Chat batch cũ đã lưu `archive/2026-11-03-before-006-007/`; ảnh giữ trong check/. Không sửa Relics/UI code.

---

## INPUT 008 · 8 icon buff có nền trong suốt · generate · TÍCH HỢP sau khi Claude CHỐT
Lý do (Claude kiểm 15 icon): 8 icon đang là ô cắt từ ảnh đề xuất `Resources/EffectBuffs/catalog.bytes` (PNG 1536×1024, nền navy đặc, còn dính viền thẻ tím/vàng), chỉ ~200×137 px → hiện thành ô vuông có nền trên thẻ chọn buff và trên HUD battle. 7 icon còn lại (ember, mana-flow, wave-income, mythic-might, soul-harvest, blood-tile-v2, gold-tile-v2) đã trong suốt — dùng làm MẪU phong cách: `Resources/EffectBuffs/ember.bytes`, `mana-flow.bytes` (là PNG).
Output: `check/008-buff-icons.png` (8 icon, PNG gốc trong suốt). Đề xuất đặt: `Resources/EffectBuffs/icons/<tên>.png`, tên theo thứ tự dưới đây.

```prompt
Create 8 game buff icons for a mobile 2D chibi fantasy auto-battler, matching an existing icon set: flat bright colors, thick black outline (8 px), soft top highlight, simple cel shading, a few small sparkles allowed, cartoon like Brawl Stars ability icons. Each icon is a single object with NO background, NO frame, NO card, NO tile, NO square behind it — fully transparent background. Two rows of four, each icon centered in its own 300 x 300 px cell with empty space around it. No text, no letters, no numbers.

Row 1:
1. double-strike: two crossed steel swords with golden hilts and a small orange impact burst behind the crossing point
2. bodyguard: a glossy cyan-blue kite shield with a lighter inner border
3. frost-touch: an icy blue snowflake with a small curved frost arrow sweeping up on its right
4. grand-ascension: three small chibi silhouettes (blue, yellow, purple) with golden upward arrows above them
Row 2:
5. reinforcements: three small chibi hero heads side by side (knight helmet, purple hood, green hood)
6. weapon-supplies: a steel kunai and a cyan crystal gem crossed
7. war-chest: an open wooden treasure chest full of gold coins
8. second-wind: a green glowing chibi silhouette with a white cross on its chest and small green upward arrows
Keep all eight about the same overall size (about 240 px across).
```
