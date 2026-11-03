# Yêu cầu art — bộ UI, đo từ game tham chiếu

**Đính kèm cả bốn ảnh:** `outdata/reference/ss_153.webp` (menu),
`ss_154.webp` (mercenary), `ss_155.webp` (xếp quân), `ss_156.webp` (đánh nhau).

Mọi mã màu dưới đây **đo ra từ chính bốn ảnh đó**.

---

## 1. Số đo

| | |
|---|---|
| Nút chính | `#F09000` cam |
| Thẻ Common | `#706050` |
| Thẻ Rare | `#0070B0` |
| Thẻ Epic | `#9000D0` |
| Thẻ Legendary | `#E0A000` |
| Thanh tài nguyên | xám-xanh rất tối, hơi trong |
| Bão hoà / độ sáng | 0.42–0.66 / 0.39–0.69 |

### Ba điều ảnh cho thấy mà bộ hiện tại đang thiếu

**Mặt nút hai tông, không phải một màu phẳng.** Nửa trên sáng hơn, nửa dưới
sẫm hơn, ranh giới **cứng** — không phải chuyển sắc. Đây là chỗ bộ tôi tự vẽ
bằng code trông chết cứng.

**Viền đen dày hơn tôi tưởng.** Dày và đều, ôm sát mép bo tròn.

**Bậc nhận ra bằng MÀU NỀN thẻ**, không bằng khung viền chạm trổ.

---

## 2. Khối STYLE — dán vào mọi prompt

```
STYLE
* Flat vector UI for a casual mobile game. Match the attached screenshots.
* Every shape has a THICK, UNIFORM BLACK OUTLINE hugging its rounded
  corners. The outline is the same weight on every shape.
* Each face is TWO FLAT TONES, split by a HARD horizontal edge: a lighter
  tone across the upper half, a darker tone across the lower half. This is
  a hard-edged two-tone fill, NOT a gradient and NOT a bevel.
* Generous corner rounding — about one fifth of the shape's short side.
* NO gradients, NO airbrush, NO glass shine, NO emboss, NO drop shadow,
  NO texture, NO noise, NO ornament, NO rivets, NO metal studs, NO wood.
* The face of every button must be COMPLETELY EMPTY — no text, no icons,
  no numbers. The game draws those on top.

Not pixel art. Not 3D. Not skeuomorphic wood or stone.
```

---

## 3. Tấm 1 — Nút và khung, 12 hình

Bố cục **3 hàng × 4 cột**, nền trong suốt.

| # | hình | màu |
|---|---|---|
| 1 | Nút bo góc, **nút chính** | `#F09000` |
| 2 | Như #1, **bấm xuống** (tối hơn, lún) | — |
| 3 | Nút bo góc, **nút phụ** | `#7B3FD4` tím |
| 4 | Như #3, bấm xuống | — |
| 5 | Nút bo góc, **bị khoá** | `#5A5A5A` xám |
| 6 | Ô vuông bo góc nhỏ, nút ✕ | `#3A3F46` |
| 7 | **Viên thuốc rất tối, hơi trong**, thanh tài nguyên | `#1C2430` ~85% đục |
| 8 | Thẻ Common | `#706050` |
| 9 | Thẻ Rare | `#0070B0` |
| 10 | Thẻ Epic | `#9000D0` |
| 11 | Thẻ Legendary | `#E0A000` |
| 12 | **Ô lưới**, rất mờ nhạt, gần như trong suốt | `#000000` ~18% đục |

> Prompt:
>
> Draw 12 UI shapes for a casual mobile game, as a 3 row x 4 column grid
> on a fully transparent background.
>
> Read left to right, top to bottom:
> 1. a rounded rectangle button, orange #F09000
> 2. the same button pressed — noticeably darker, pushed down
> 3. a rounded rectangle button, purple #7B3FD4
> 4. the same purple button pressed — darker, pushed down
> 5. a rounded rectangle button, flat grey #5A5A5A, looking disabled
> 6. a small rounded SQUARE button, dark slate #3A3F46
> 7. a long rounded PILL bar, very dark blue-grey #1C2430, slightly
>    translucent so a background would show faintly through it
> 8. a wide rounded card, muted grey-brown #706050
> 9. the same card in blue #0070B0
> 10. the same card in purple #9000D0
> 11. the same card in gold #E0A000
> 12. a rounded SQUARE grid cell, almost transparent dark — a faint
>     shadow of a square, only just visible
>
> Cards 8, 9, 10 and 11 must be the SAME shape and size, differing only
> in colour. Buttons 1 and 2 must align pixel for pixel, and so must 3
> and 4.
>
> [DÁN KHỐI STYLE Ở MỤC 2]
>
> At least 60 pixels of transparent space between neighbours and 40
> pixels of margin around the whole grid. Nothing may touch.
>
> TRUE TRANSPARENT alpha background — no magenta, no white, no
> checkerboard. The alpha must be ANTI-ALIASED, fading over about one
> pixel; do not threshold or posterise it.
>
> No text, no labels, no numbers, no watermark.

Đặt file: `artsrc/ui/ui_v3_shapes.png`

---

## 4. Tấm 2 — Icon tài nguyên, 8 hình

Trong ảnh tham chiếu, icon **chìa ra ngoài mép trái** của thanh, không nằm
gọn bên trong. Nên mỗi icon phải là một hình **độc lập, tròn trịa, đọc được
khi đứng riêng**.

Một hàng ngang 8 hình:

| # | hình |
|---|---|
| 1 | Tia sét — năng lượng |
| 2 | Vương miện — vàng |
| 3 | Viên ngọc xanh lam — đá quý |
| 4 | Chìa khoá vàng |
| 5 | Trái tim đỏ — máu |
| 6 | Thanh kiếm đỏ — sát thương |
| 7 | Đôi cánh xanh — tốc đánh |
| 8 | Vụ nổ cam — chí mạng |

> Bốn cái sau (5–8) là bộ icon buff, trong ảnh tham chiếu chúng nằm trên
> thanh tối và **rất nhỏ**. Nên vẽ cực kỳ đơn giản: một hình, một hai màu,
> viền đen dày.

> Prompt:
>
> Draw 8 small game icons in ONE horizontal row on a fully transparent
> background: (1) a lightning bolt, (2) a crown, (3) a blue gem,
> (4) a golden key, (5) a red heart, (6) a red sword, (7) a pair of blue
> wings, (8) an orange starburst.
>
> Each icon is ONE simple symbol filled with one or two flat colours and
> wrapped in a thick uniform black outline. They will be displayed about
> 40 pixels tall, so there must be NO small detail at all — no facets, no
> engraving, no sparkle, no shine, no inner lines.
>
> All eight must read at the same visual weight: none noticeably heavier
> or lighter than the others.
>
> [DÁN KHỐI STYLE Ở MỤC 2]
>
> At least 80 pixels of transparent space between neighbours, 40 pixels
> of margin around the row. Nothing may touch.
>
> TRUE TRANSPARENT alpha background — no magenta, no white. The alpha
> must be ANTI-ALIASED, fading over about one pixel; do not threshold or
> posterise it.
>
> No text, no labels, no numbers, no watermark, no drop shadow.

Đặt file: `artsrc/ui/ui_v3_icons.png`

---

## 5. Tôi sẽ kiểm bằng gì

| | đạt khi |
|---|---|
| Tách được đúng số hình | 12 và 8 |
| Alpha | 256 mức, 0% magenta |
| **Mặt nút có đúng 2 tông** | 2–3 cụm màu mỗi hình |
| Cặp thường/bấm trùng khít | lệch alpha < 1/255 |
| 4 thẻ bậc cùng cỡ | lệch < 3px |
| 8 icon cùng độ nặng | lệch `sqrt(diện tích)` < 1.3× |

Cặp nào lệch hình học thì tôi **sinh lại bản bấm từ bản thường bằng code** —
đã làm vậy một lần và nó cho kết quả lệch đúng 0.00/255.
