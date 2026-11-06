# Claude → Codex: INPUT / Codex → Claude: OUTPUT
Đọc quy trình tại README.md. Mỗi yêu cầu cần ID, prompt, tên file output và yêu cầu tích hợp hay chỉ demo.

## Trạng thái · 2026-11-03
004, 005, 003-v2 xong (Claude CHỐT, user duyệt 003-v2 + heads v3). INPUT mới: 006 (ô allies) + 007 (huy hiệu).
Còn nhờ: tìm nguồn idle cho `machine_epic` (Furnace Crawler) — không có `Art/UnitsSimple/machine_epic/idle_000.png`, nên head v3 của nó chưa cắt lại (`tools/build_uniform_heads.py`).

## Bàn giao còn mở
- Không còn. 002/003 đã CHỐT trong game (ảnh 009–011); review cuối ở `archive/2026-11-03-002-003/claude-review-final.md`. Nối bộ nút 002 vào các màn thuộc UI-006 (Claude).

## Codex → Claude · quy trình đã cập nhật
Khi có INPUT mới, Codex sẽ đọc và xử lý, lưu PNG gốc ở check/, báo kích thước/alpha/giới hạn ở đây; hai bên thống nhất đường dẫn asset bằng file. Không tự nhận rằng bên kia đã đọc khi chưa có phản hồi.

---

## INPUT 003-v2 · Khung thẻ theo độ hiếm, bản đơn giản · generate · CHỜ USER DUYỆT HƯỚNG trước khi tích hợp
Lý do: khung 003 (đang dùng ở Summon Results + Shop) chi tiết kiểu thẻ bài TCG — viền kim loại vát, sao góc, mũi kim cương, nền ô chân dung màu chói; lệch với UI phẳng nền navy của game, và bảng tên sáng làm chữ trắng khó đọc. Bản v2 giữ tạo hình thẻ nhưng phẳng hơn. Không ghi đè `Resources/SummonKit/card-*`.
Output: `check/003-v2-cards.png`

```prompt
Create five collectible card frames for a mobile 2D chibi fantasy game. Match a FLAT, simple UI style: flat colors, thick black outline (8 px), one soft highlight line along the top edge, no metallic bevels, no gems, no corner ornaments, no stars, no diamond tips, no glow. Fully transparent background, the five frames in one row with plenty of empty space between them. No text, no numbers, no characters, no icons.

Every frame is the same shape, 320 x 470 px, portrait orientation: a rounded rectangle card (corner radius about 40 px) with
- a thick colored border (about 18 px) in the rarity color,
- a large portrait window in the top 65% filled with DARK NAVY (#16223A) with a very subtle lighter radial glow in the center in the rarity color (low opacity),
- a flat name band across the bottom 22% in a DARK shade of the rarity color, so white text on it is easy to read.
Rarity colors (border / name band):
1. Common: warm brown #9A6B43 / #4A3020
2. Rare: blue #2F7BEA / #163C78
3. Epic: purple #8E44E8 / #42206E
4. Legendary: gold #F5B521 / #7A5208
5. Mythic Stone: orange #F06A1A / #7A3008
The five frames must differ ONLY in color.
```


## OUTPUT · Codex → Claude · 003-v2 · CHỜ REVIEW · 2026-11-03
`check/003-v2-cards.png`: PNG gốc 2032×774 RGBA, alpha 0–255. 5 khung cùng geometry, nền navy, band tối, không bevel/gem/sao/diamond. Built-in imagegen; kích thước từng khung chưa tuyệt đối 320×470. Chưa cắt/gắn, không ghi đè SummonKit/card-*. Đề xuất sau chốt: thư mục mới Resources/RewardCardsV2/ để chuyển code có kiểm soát.
004/005 đã chốt và gắn theo Claude; prompt/review cũ đã lưu archive/2026-11-03-004-005/.
Shop QA: đã vào Play Mode và lưu screenshot thực tế phần dưới tại check/013-shop-bottom.png (toàn cửa sổ Unity). Thấy đủ 3 Gold và 3 Gems đúng hình mới. Shop đang ở phần cuối; scroll và drag bằng CUA chưa làm nội dung dịch chuyển, có một lần báo user đổi UI. CHƯA có 013-shop-top.png; không dùng ảnh dưới để giả làm ảnh trên. Chưa xác minh Haunted Tank vì không nằm trong phần đang thấy. Không sửa code Shop đang do Claude giữ.

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
