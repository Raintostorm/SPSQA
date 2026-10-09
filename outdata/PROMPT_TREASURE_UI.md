# Yêu cầu art — Màn hình Treasure (1125×2436)

Bộ này độc lập với `PROMPT_HOMEPAGE.md`. Làm được song song.

---

## Bốn ràng buộc — áp cho MỌI tấm trừ ảnh nền

Bốn thứ này đã khiến phải nhập lại ba lần ở các đợt trước. Đọc kỹ.

1. **Alpha trong suốt thật** — không magenta, không trắng, không kẻ ô.
   Magenta là phép mất mát: điểm rìa khử răng cưa chính là màu trộn giữa
   magenta và hình, tách ra luôn còn quầng hồng.
2. **Alpha khử răng cưa** — viền mờ dần qua khoảng một điểm ảnh. Đừng
   ngưỡng hoá, đừng posterise. Đợt trước nhận về alpha chỉ 2 mức.
3. **Không cắt cụt** — mỗi hình vẽ trọn vẹn, chừa lề trong suốt ≥40px,
   không đóng khung, không kẻ ô quanh hình.
4. **Không chữ, không nhãn, không watermark, không bóng đổ xuống nền.**

**Phong cách:** khớp bộ đang có — hoạt hoạ 2D, viền đen đậm đều nét, tô
khối phẳng có chuyển sáng nhẹ, bảng màu gỗ–đá–vàng đồng. Game mobile
casual. Không pixel art, không tả thực.

---

## Bố cục màn Treasure — toạ độ thật

```
┌─────────────────────────────┐  y=0
│  [🗝 24]            [✕]     │  thanh chìa + nút thoát, y=40, cao 128
│                             │
│         KHO BÁU             │  tiêu đề, y≈240
│  ┌───────────────────────┐  │
│  │                       │  │
│  │      [ RƯƠNG LỚN ]    │  │  rương hiện ở giữa, 560×560, y≈420
│  │                       │  │
│  └───────────────────────┘  │
│                             │
│   [ x1 ]  [ x10 ]  [ x100 ] │  ba nút quay, y≈1060, mỗi nút 320×150
│                             │
│  ┌──┬──┬──┬──┬──┐           │  lưới 30 ô vật phẩm
│  ├──┼──┼──┼──┼──┤           │  5 cột × 6 hàng, ô 180×180, khe 25
│  ├──┼──┼──┼──┼──┤           │  lề 62px mỗi bên, cao tổng 1205
│  ├──┼──┼──┼──┼──┤           │  y≈1260…2465  → cuộn được
│  ├──┼──┼──┼──┼──┤           │
│  └──┴──┴──┴──┴──┘           │
└─────────────────────────────┘
```

Lưới cuộn được vì 1205px không vừa phần còn lại của màn.

---

## Tấm 1 — Ảnh nền kho báu

Chính xác **1125×2436**.

Khác nền trận (bãi cỏ) và khác nền trang chủ (có chiều sâu, nhìn ra xa).
Đây là **trong nhà**: một hầm kho báu, tối hơn, ấm hơn, có ánh đuốc.

Tối hơn là cố ý — vàng và đá quý trên các vật phẩm chỉ nổi bật khi nền
tối. Nền sáng thì 30 icon vàng chìm hết.

Hai vùng phải **trống và tối đều**:
- **y = 1060…1210** (dải ba nút quay)
- **y = 1260…2436** (lưới vật phẩm)

Chi tiết đẹp để ở **y = 0…1050**: trần vòm, đuốc, cột đá, vài đống vàng
ở góc.

> Prompt:
>
> A vertical 2D mobile game background, exactly 1125x2436 pixels,
> portrait orientation: the interior of a fantasy treasure vault.
>
> Upper half: a stone vault with an arched ceiling, carved pillars, lit
> wall torches casting warm orange light, piles of gold coins and gems
> heaped in the corners, a worn red carpet on the floor.
>
> Lower half: the same stone floor and wall, but kept DELIBERATELY PLAIN
> AND DARK — flat, evenly lit, low contrast, no coins, no torches, no
> carved detail. UI panels will sit on top of this area and must stay
> readable.
>
> Overall the scene is DIM and warm, not bright: deep browns, dark stone
> grey, warm torchlight. Gold items placed on top must pop against it.
>
> Style: hand-painted 2D cartoon, bold shapes, mobile casual game art.
> Not pixel art, not photorealistic.
>
> Fill the entire 1125x2436 canvas edge to edge. No border, no frame,
> no vignette, no text, no UI elements, no characters, no watermark.

Đặt file: `artsrc/ui/bg_treasure_portrait.png`

---

## Tấm 2 — Ba nút quay

Một hàng ngang **6 hình**: ba nút, mỗi nút hai trạng thái thường/bấm.
Mỗi hình **320×150px**, cách nhau ≥80px trong suốt.

Ba nút phải **phân biệt được bằng màu**, không chỉ bằng chữ — chữ sẽ do
code vẽ đè lên, không nằm trong art:

| nút | màu |
|---|---|
| x1 | đồng / nâu, khiêm tốn |
| x10 | bạc / xanh dương, nổi hơn |
| x100 | vàng ròng, rực nhất, có ánh sáng viền |

> **Sáu hình phải cùng cỡ, cùng bố cục, trùng khít từng điểm ảnh.** Chúng
> thay nhau tại chỗ lúc bấm; lệch một chút là nút giật.

> Prompt:
>
> A horizontal row of SIX rectangular mobile-game buttons on a fully
> transparent background, each 320x150 pixels with rounded corners, at
> least 80 pixels of transparent space between neighbours.
>
> They are three buttons, each shown twice — first its normal state, then
> its pressed state (darker, inner bevel inverted):
>
> 1-2. A modest COPPER/BROWN metal button with a simple rim.
> 3-4. A brighter SILVER/BLUE metal button with a decorated rim.
> 5-6. A radiant GOLD button with an ornate rim and a soft glowing edge.
>
> All six must share the exact same shape, size and internal layout —
> only the metal, colour and shading differ. Each pair must align pixel
> for pixel.
>
> The button faces must be mostly EMPTY and evenly lit in the middle:
> text will be drawn on top by the game. No numbers, no symbols, no icons
> on the faces.
>
> Style: bold even black outline, flat shading with soft gradients,
> mobile casual game art. Not pixel art, not photorealistic.
>
> At least 40px of empty transparent margin around each button. TRUE
> TRANSPARENT alpha background — no magenta, no white. The alpha must be
> ANTI-ALIASED, fading over about one pixel; do not threshold or
> posterise it.
>
> No text, no labels, no watermark, no drop shadow.

Đặt file: `artsrc/ui/buttons_roll.png`

---

## Tấm 3 — Khung ô vật phẩm, 5 loại

Một hàng ngang **5 hình vuông**, mỗi hình **180×180px**, cách nhau ≥60px.

Đây là cái khung rỗng; icon vật phẩm sẽ được code đặt vào giữa. Nên
**lòng khung phải trống và phẳng**, trang trí dồn ra viền.

| # | loại | viền |
|---|---|---|
| 1 | **Chưa có** (khoá) | đá xám trơn, tối, không trang trí |
| 2 | Common | gỗ nâu, đai sắt xỉn |
| 3 | Rare | viền bạc, góc khảm đá xanh |
| 4 | Epic | viền vàng, góc khảm đá tím, phát sáng nhẹ |
| 5 | Legendary | vàng ròng chạm trổ, phát sáng mạnh, có tia sáng |

> Năm khung **cùng cỡ ngoài, cùng bề dày viền, cùng kích thước lòng
> trong**. Chúng xếp cạnh nhau thành lưới 5×6; lệch là cả lưới so le.

> Prompt:
>
> A horizontal row of FIVE square UI item-slot frames on a fully
> transparent background, each 180x180 pixels, at least 60 pixels of
> transparent space between neighbours.
>
> Left to right, increasing in rarity:
> 1. LOCKED: plain dark grey stone, no decoration, flat and dull.
> 2. COMMON: brown wood with dull iron corner brackets.
> 3. RARE: silver border with small blue gems set in the corners.
> 4. EPIC: gold border with purple gems in the corners and a faint glow.
> 5. LEGENDARY: ornate carved solid gold, strong warm glow, small light
>    rays at the corners.
>
> All five frames must share the EXACT same outer size, the same border
> thickness and the same inner opening size, so they tile together evenly
> in a grid. Only the material, colour and glow change.
>
> The inside of each frame must be EMPTY and flat — a plain dark recess.
> Item icons will be drawn inside by the game. Put all decoration on the
> border, never in the middle.
>
> Style: bold even black outline, flat shading with soft gradients,
> mobile casual game art. Not pixel art, not photorealistic.
>
> At least 40px of empty transparent margin around each frame. TRUE
> TRANSPARENT alpha background — no magenta, no white. The alpha must be
> ANTI-ALIASED, fading over about one pixel; do not threshold or
> posterise it.
>
> No text, no numbers, no labels, no watermark, no drop shadow.

Đặt file: `artsrc/ui/item_slots.png`

---

## Tấm 4 — Dải trang trí hai bên

Màn dọc rộng 1125, lưới vật phẩm chỉ chiếm 1000 — còn 62px mỗi bên. Và
dải ba nút quay cũng để trống hai đầu.

Hai dải đứng, mỗi dải **120×1400px**: cột đá có đuốc, xích sắt, vài đồng
tiền mắc kẹt trong khe. Một dải úp ngược là ra dải kia, nên **chỉ cần vẽ
MỘT**, code lật.

> Prompt:
>
> A single vertical decorative border strip for a mobile game UI, 120x1400
> pixels, on a fully transparent background.
>
> A carved stone pillar running the full height, with an iron chain
> hanging along it, a small wall torch near the top, and a few gold coins
> wedged in the cracks. Lit from the right side.
>
> The strip must TILE VERTICALLY: the pattern at the very top must
> continue seamlessly into the pattern at the very bottom, so several
> copies stacked end to end look like one continuous pillar.
>
> Style: bold even black outline, flat shading with soft gradients, dark
> stone and warm gold palette, mobile casual game art. Not pixel art,
> not photorealistic.
>
> TRUE TRANSPARENT alpha background — no magenta, no white. The alpha
> must be ANTI-ALIASED, fading over about one pixel; do not threshold or
> posterise it.
>
> No text, no labels, no watermark, no drop shadow.

Đặt file: `artsrc/ui/side_pillar.png`

---

## Chưa viết — và vì sao

**30 icon vật phẩm** là tấm lớn nhất, chia 4 tấm theo bậc (12/8/6/4). Tôi
chưa viết prompt vì nó phụ thuộc **tên 30 món** — bạn đang duyệt ở
`outdata/TREASURE_30_ITEMS.md`. Tên quyết định hình; viết prompt trước khi
chốt tên là chắc chắn phải làm lại.

**Rương 4 bậc** (8 hình) đã có prompt sẵn ở `PROMPT_HOMEPAGE.md` mục 3c.

---

## Sau khi có art

Báo tôi. Tool nhập sẽ:
- tách bằng **gán nhãn liên thông**, không cắt theo lưới cứng
- ép ba cặp nút quay và năm khung ô về **chung một khung**, vì chúng
  đứng cạnh nhau hoặc thay nhau tại chỗ
- kiểm dải trang trí có **nối liền được theo chiều dọc** không: so hàng
  điểm ảnh trên cùng với hàng dưới cùng, lệch quá thì báo chứ không nhập
- đo và in số **trước khi ghi**, để bạn duyệt
