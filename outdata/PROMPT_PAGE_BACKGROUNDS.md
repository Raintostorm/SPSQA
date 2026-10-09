# Yêu cầu art — Nền các trang còn thiếu (1125×2436)

Màn **dọc** đã chốt. Mọi tấm dưới đây đúng **1125×2436**.

## Tình trạng bốn trang

| trang | nền | ghi chú |
|---|---|---|
| Battle | ✅ `bg_prep_portrait`, `bg_battle_portrait` | đang chạy |
| Treasure | ✅ `bg_treasure_portrait` | đã generate, đo đạt (độ lệch chuẩn vùng trống 0.7) |
| **Home** | ❌ | prompt đã có ở `PROMPT_HOMEPAGE.md` mục 3 — chưa generate |
| **Upgrade** | ❌ | prompt ở dưới |
| **Shop** | ❌ | prompt ở dưới |

---

## Điều quan trọng nhất: VÙNG TRỐNG

Nền treasure đạt vì prompt ghi **chính xác dải pixel** phải trống, và đo
lại được: độ lệch chuẩn `0.74` và `0.84` trên hai dải — phẳng gần tuyệt đối.

Nền nào cũng phải làm đúng vậy. Chi tiết đẹp dồn lên **phần trên**, phần
còn lại phải **phẳng, tối đều, ít tương phản**, vì bảng và nút sẽ nằm đè
lên. Nền đẹp đều khắp là nền không đọc được chữ.

## Bốn ràng buộc chung

Nền là ảnh **đặc**, phủ kín khung, **không có alpha** — nên chỉ áp hai điều:

1. **Phủ kín 1125×2436**, không viền, không khung, không vignette
2. **Không chữ, không nhãn, không UI, không nhân vật, không watermark**

**Phong cách:** khớp bộ đang có — vẽ tay 2D hoạt hoạ, hình khối rõ, màu no
nhưng không chói. Không pixel art, không tả thực.

**Năm trang phải nhận ra là cùng một game.** Cùng bảng màu gỗ–đá–vàng đồng,
cùng độ dày nét. Khác nhau ở **nơi chốn**, không khác ở phong cách:

| trang | nơi chốn |
|---|---|
| Battle | bãi cỏ ngoài trời, nhìn từ trên |
| Home | trại quân ngoài trời, có chiều sâu, nhìn ra xa |
| Treasure | hầm kho báu trong nhà, tối, có đuốc |
| **Upgrade** | lò rèn trong nhà, tối, có lửa lò |
| **Shop** | quầy hàng có mái che, nửa trong nửa ngoài, sáng hơn cả ba cái trong nhà |

---

## Tấm 1 — Nền trang Upgrade

### Bố cục

```
┌─────────────────────────────┐  y=0
│  [🪙 1,250]          [✕]    │  thanh vàng + thoát, y=40, cao 128
│          LÒ RÈN             │  tiêu đề, y≈240
│  ┌───────────────────────┐  │
│  │ [ảnh] Tên   Lv.3  500¢│  │  mỗi nhân vật MỘT DÒNG
│  ├───────────────────────┤  │  dòng cao 200, khe 16
│  │ [ảnh] Tên   Lv.1  200¢│  │  20 dòng → cuộn 4320px
│  ├───────────────────────┤  │  thấy 5 dòng cùng lúc
│  │ ...                   │  │
│  └───────────────────────┘  │  y≈420…2380
└─────────────────────────────┘
```

Một dòng mỗi nhân vật chứ không phải lưới: còn phải ghi cấp, giá, và nút
nâng. Lưới ô vuông không đủ chỗ cho ba thứ đó.

Ảnh nhân vật lấy **khung idle đầu tiên** của chính con đó — không tốn art.

### Vùng trống bắt buộc

**y = 400 … 2436** phải phẳng và tối đều. Đó là 83% chiều cao — gần như cả
tấm. Chi tiết chỉ được nằm ở **y = 0 … 390**.

> Prompt:
>
> A vertical 2D mobile game background, exactly 1125x2436 pixels,
> portrait orientation: the interior of a blacksmith's forge.
>
> TOP AREA ONLY (the upper 16% of the image): a stone forge with glowing
> orange coals, an anvil, hanging hammers and tongs, a wooden workbench,
> sparks in the air, warm firelight.
>
> EVERYTHING BELOW that — the entire lower 84% of the image — must be a
> PLAIN DARK STONE WALL: flat, evenly lit, very low contrast, no tools,
> no sparks, no fire, no texture detail, no highlights. UI list rows will
> cover this whole area and must stay readable. This large plain region
> is intentional and required.
>
> The scene is DIM and warm overall: dark grey stone, deep browns, a warm
> orange glow confined to the top.
>
> Style: hand-painted 2D cartoon, bold shapes, mobile casual game art,
> matching a warm wood-stone-brass palette. Not pixel art, not
> photorealistic.
>
> Fill the entire 1125x2436 canvas edge to edge. No border, no frame, no
> vignette, no text, no UI elements, no characters, no watermark.

Đặt file: `artsrc/ui/bg_upgrade_portrait.png`

---

## Tấm 2 — Nền trang Shop

### Bố cục

```
┌─────────────────────────────┐  y=0
│  [🪙 1,250]  [🗝 24]   [✕]  │  thanh tài nguyên, y=40, cao 128
│          CỬA HÀNG           │  tiêu đề, y≈240
│  ┌─────┬─────┬─────┐        │  lưới 3 cột × n hàng
│  │     │     │     │        │  ô 320×320, khe 30
│  ├─────┼─────┼─────┤        │  lề 52px mỗi bên
│  │     │     │     │        │  y≈420…2380, cuộn được
│  └─────┴─────┴─────┘        │
└─────────────────────────────┘
```

### Vùng trống bắt buộc

Giống Upgrade: **y = 400 … 2436** phẳng và tối đều, chi tiết chỉ ở
**y = 0 … 390**.

Shop sáng hơn Treasure và Upgrade — nó là chỗ mời chào, không phải hầm
kín. Nhưng *sáng hơn* không có nghĩa là *rối hơn*: vùng trống vẫn phải
phẳng.

> Prompt:
>
> A vertical 2D mobile game background, exactly 1125x2436 pixels,
> portrait orientation: a fantasy market stall.
>
> TOP AREA ONLY (the upper 16% of the image): a striped canvas awning
> stretched over wooden posts, hanging lanterns, a few bundled sacks and
> barrels, daylight coming in from outside behind the stall.
>
> EVERYTHING BELOW that — the entire lower 84% of the image — must be a
> PLAIN wooden plank wall in shadow: flat, evenly lit, very low contrast,
> no goods, no lanterns, no shelves, no strong grain, no highlights. UI
> cards will cover this whole area and must stay readable. This large
> plain region is intentional and required.
>
> Warmer and brighter than a dungeon, but still MUTED in the lower area
> so that colourful item cards placed on top stand out.
>
> Style: hand-painted 2D cartoon, bold shapes, mobile casual game art,
> matching a warm wood-stone-brass palette. Not pixel art, not
> photorealistic.
>
> Fill the entire 1125x2436 canvas edge to edge. No border, no frame, no
> vignette, no text, no UI elements, no characters, no watermark.

Đặt file: `artsrc/ui/bg_shop_portrait.png`

---

## Tấm 3 — Nền trang chủ

Prompt đã viết sẵn ở `PROMPT_HOMEPAGE.md` mục 3, tấm A. **Nhưng sửa một
chỗ trước khi dùng:** bản đó nói vùng trống là *"25% đến 66% chiều cao"*.
Ba thẻ level đã dời, vùng đúng phải là:

> CRITICAL — the vertical band from **y=600 to y=2280** (that is, from 25%
> to 94% of the image height) must stay visually QUIET: low contrast, no
> bright highlights, no busy detail, no important subject.

Phần còn lại của prompt giữ nguyên.

Đặt file: `artsrc/ui/bg_home_portrait.png`

---

## Shop bán gì — chưa định nghĩa

Tôi viết nền Shop vì bạn yêu cầu, nhưng cần nói rõ: **chưa có thiết kế nào
cho Shop.** Nó chưa từng xuất hiện trong `Handoff/`, chưa bàn trong các
phiên trước.

Vàng hiện đã **vô hạn** (level 1 cho 1000 vàng, chơi lại vẫn thưởng — xem
`PROMPT_HOMEPAGE.md` mục 5b). Nếu Shop bán bằng vàng thì mọi thứ trong đó
đều miễn phí trên thực tế.

Nền thì làm trước được, không sao. Nhưng trước khi dựng UI Shop, cần chốt:
nó bán gì, bằng tài nguyên nào, và tài nguyên đó kiếm ở đâu mà không vô hạn.

---

## Thứ tự nên làm

1. Generate 3 nền này (Upgrade, Shop, Home)
2. Tôi viết tool nhập **cả 14 tấm một lượt** — 11 tấm đang chờ + 3 nền mới
3. `SaveSystem` JSON — mọi trang đều cần nó trước khi có dữ liệu để hiện
4. Dựng từng trang một, bắt đầu từ Home vì nó là cửa vào
