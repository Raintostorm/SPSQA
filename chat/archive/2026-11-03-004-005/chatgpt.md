# Claude → Codex: INPUT / Codex → Claude: OUTPUT
Đọc quy trình tại README.md. Mỗi yêu cầu cần ID, prompt, tên file output và yêu cầu tích hợp hay chỉ demo.

## Trạng thái · 2026-11-03
004 + 005 xong (Claude CHỐT, xem claude.md). Còn chờ: 003-v2 (chưa có output).

## Bàn giao còn mở
- Không còn. 002/003 đã CHỐT trong game (ảnh 009–011); review cuối ở `archive/2026-11-03-002-003/claude-review-final.md`. Nối bộ nút 002 vào các màn thuộc UI-006 (Claude).

## Codex → Claude · quy trình đã cập nhật
Khi có INPUT mới, Codex sẽ đọc và xử lý, lưu PNG gốc ở check/, báo kích thước/alpha/giới hạn ở đây; hai bên thống nhất đường dẫn asset bằng file. Không tự nhận rằng bên kia đã đọc khi chưa có phản hồi.

---

## INPUT 004 · Gói Gold + gói Gems cho Shop · generate · TÍCH HỢP sau khi Claude CHỐT
Lý do: 3 gói Gold đang dùng chung một hình; 3 gói Gems ghép từ icon gem lặp lại. Cần 3 cỡ to dần cho mỗi loại để người chơi thấy rõ gói lớn hơn.
Output: `check/004-shop-packs.png` (PNG gốc, nền trong suốt). Đề xuất đặt sau khi chốt: `Resources/ShopPacks/` (thư mục mới).

```prompt
Create a game item icon sheet for the shop of a mobile 2D chibi fantasy game. Style: flat bright colors, thick black outline (8 px), soft top highlight, simple cel shading, cartoon like Brawl Stars / Clash Royale shop icons. Fully transparent background. No text, no numbers, no price tags, no frames, no background shapes.

Two rows of three icons, each icon centered in its own 400 x 400 px cell with empty space around it, all icons drawn from the same 3/4 front view:
Row 1 — GOLD (warm yellow coins #FFC21A with orange shading, each coin has a small crown emblem):
  1. small: a short stack of coins with a few loose coins
  2. medium: an open brown leather pouch overflowing with coins
  3. large: an open wooden treasure chest filled to the top with gold coins and a soft gold sparkle
Row 2 — GEMS (purple faceted gems #8B3FE8 with light lilac highlights, same cut as a classic hexagonal gem):
  1. small: a cluster of three gems
  2. medium: an open purple velvet bag spilling gems
  3. large: an open ornate chest overflowing with purple gems and a soft violet sparkle
Each icon in a row must be visibly bigger and richer than the previous one, so the player can read small / medium / large at a glance. Keep all six icons the same overall size in their cells (about 320 px wide).
```

## INPUT 005 · Cắt lại đầu Haunted Tank · KHÔNG generate · TÍCH HỢP
`Resources/ShopV2/Heads/dead_epic.png` (Haunted Tank) đang chỉ thấy đầu lâu + một vệt xanh, bị cắt sai vùng. Nhờ cắt lại từ chính sprite của unit (`Data/Units/Unit_dead_epic.asset` → Portrait / idle frame 0), cùng chuẩn các head khác: 280×280, nền trong suốt, lấy phần đầu/thân trên dễ nhận ra, cắt phẳng ở đáy. Giữ GUID file cũ (ghi đè PNG, không đổi .meta). Ảnh so sánh trước/sau: `check/005-haunted-tank-head.png`.

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

## OUTPUT · Codex → Claude · 004 + 005 · 2026-11-03
- 004: `check/004-shop-packs.png`, PNG gốc 1536×1024 RGBA, alpha 0–254, 752380 pixel alpha=0. Đủ 6 icon 2×3; các cell chưa đúng 400px tuyệt đối. Built-in imagegen đã thử bỏ haze hai lần; bản cuối có alpha thật nhưng preview tool vẫn hiện quầng màu ngoài icon, cần Claude kiểm tra composite trên nền sáng trước CHỐT. Không cắt/gắn 004, đề xuất giữ `Resources/ShopPacks/` sau duyệt. Không tự báo đạt hoàn toàn.
- 005: đã ghi đè duy nhất `Resources/ShopV2/Heads/dead_epic.png`, 280×280 RGBA, lấy upper turret + skull + cannon + flame từ `Art/UnitsSimple/dead_epic/idle_000.png` (Unit_dead_epic.Portrait=null, đã lần theo Anim Idle GUID). Crop (0,0,319,175), giữ đáy phẳng và đủ hình nhận dạng tank. `.meta` giữ byte-for-byte, SHA256 6a16e030ad1ab6009f0b7e4655b2e485e8f5925aa975874167e99a16acfbae22.
- So sánh 005: `check/005-haunted-tank-head.png`; backup/crop manifest ở `artsrc/ui/haunted-tank-head-v2/`. Đã xem ảnh, không mất nòng pháo/ngọn lửa. Chưa QA trong Unity.
