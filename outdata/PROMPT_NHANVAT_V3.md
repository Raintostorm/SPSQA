# Yêu cầu art — nhân vật, đo từ ảnh mẫu

Ảnh mẫu: `outdata/reference/style_target.png` — **đính kèm nó vào prompt.**

Mọi ràng buộc dưới đây **đo ra từ chính ảnh đó**, không phải tôi nghĩ ra.

---

## 1. Số đo của ảnh mẫu

| | đo được |
|---|---|
| Nhân vật | 132 × 154 px |
| **Số màu (kể cả nét)** | **5** |
| **Nét đen chiếm** | **40.1% diện tích nhân vật** |
| **Bề dày nét** | **6px trên chiều cao 154px = 3.9%** |
| Bề rộng nửa trên / nửa dưới | 91px / 121px |

Bảng màu đầy đủ, không có màu thứ sáu:

```
#000000  34.6%   nét viền — ĐEN TUYỀN
#686868  20.5%   mũ sắt
#EAD0B6  19.4%   xương
#683434   8.3%   khiên gỗ
#828282   2.2%   ánh sáng trên mũ
```

### Hai điều số đo này nói, mà tôi đã đoán sai

**Nét viền là ĐEN TUYỀN `#000000`**, không phải "màu sẫm cùng tông" như tôi
khuyên ở bản trước. Bản đó tôi viết theo lý thuyết; ảnh mẫu bác bỏ nó. Dùng
số đo.

**Nét chiếm 40% diện tích.** Gần một nửa nhân vật là nét. Art hiện tại của
ta đo ra **0.3%**. Đây không phải "dày hơn một chút" — đây là khác hẳn
về chất.

---

## 2. Prompt — dán nguyên khối, kèm ảnh mẫu

> I have attached a reference image showing the exact art style I need.
> Match that style precisely. The following numbers are measured from it
> and are hard requirements, not suggestions.
>
> **TASK**
> Draw one game character in 8 poses of a walk cycle, as ONE horizontal
> row of 8 figures, on a fully transparent background.
>
> **THE CHARACTER**
> A small, friendly cartoon skeleton soldier — the same character as in
> the reference image. A dented grey pot helmet, a round brown shield on
> its left arm, a short grey sword in its right hand. Cute and clumsy,
> never scary or detailed.
>
> **COLOUR — exactly five, no more**
> Use ONLY these five colours in the entire drawing:
> * #000000 pure black — every outline and every interior line
> * #EAD0B6 bone cream — skull, ribs, limbs
> * #686868 medium grey — helmet and sword
> * #828282 light grey — a single flat highlight shape on the helmet
> * #683434 dark brown — the shield
>
> Do not introduce a sixth colour. Do not tint, blend, or shade between
> them. Each area is ONE of these five flat colours with a hard edge.
>
> **LINE — this is the single most important rule**
> * Every outline is PURE BLACK #000000. Never dark brown, never a dark
>   version of the fill colour, never grey. Pure black only.
> * The line has ONE CONSTANT THICKNESS everywhere: about 4% of the
>   figure's total height. If a figure is 1000 pixels tall, its outline
>   is about 40 pixels thick. This is VERY THICK — much thicker than a
>   normal illustration outline.
> * The line never tapers, never fades, never thins at corners, and
>   never breaks.
> * Interior lines — ribs, the helmet rim, fingers — use the SAME
>   thickness as the outer silhouette.
> * When finished, roughly 40% of each figure's area should be black
>   line. If it looks like less than a third, the lines are too thin.
>
> **SHAPE**
> * Build the figure from a few large simple shapes. Big round skull,
>   short thick limbs, small feet.
> * The head is nearly as wide as the body.
> * Face: two solid black oval eye sockets and a simple mouth line.
>   Nothing else — no nose, no pupils, no teeth, no eyebrows.
> * Pose each figure asymmetrically: weight on one leg, slight lean.
>
> **MUST NOT APPEAR**
> No gradients. No soft or airbrushed shading. No blur. No glow. No
> specular highlight. No rim light. No drop shadow. No bone texture, no
> scratches, no wood grain, no metal sheen. No small ornaments, rivets,
> studs, buckles or straps. No background, no ground, no scenery.
>
> **DISPLAY SIZE**
> This character will be shown about 50 pixels tall in the game, with
> twenty others around it. Any detail thinner than one fifteenth of the
> figure's height will vanish at that size — leave it out. The figure
> must be recognisable from its solid black silhouette alone.
>
> **THE SEQUENCE**
> The 8 figures form a smooth loopable walk. Side view, facing RIGHT,
> slightly three-quarter. The body bobs across the cycle, but the FEET
> all rest on one shared ground line: the lowest point of every figure
> is at the same height.
>
> **LAYOUT**
> At least 80 pixels of empty transparent space between neighbouring
> figures — nothing may touch, including the sword and shield. At least
> 60 pixels of transparent margin around the whole row. Every figure
> drawn COMPLETE, nothing cropped at any edge.
>
> **BACKGROUND**
> TRUE TRANSPARENT alpha. No magenta, no white, no checkerboard pattern.
> The alpha must be ANTI-ALIASED, fading over about one pixel. Do not
> threshold or posterise the transparency.
>
> No text, no labels, no numbers, no watermark.

---

## 3. Ba câu làm nó không sai được

Nếu phải cắt prompt cho ngắn, giữ đúng ba câu này:

1. **`roughly 40% of each figure's area should be black line`**
   Ràng buộc **đếm được**, và đúng cái đang sai (0.3%). Công cụ sinh ảnh
   tuân theo con số tốt hơn tuân theo tính từ.

2. **`ONLY these five colours`** kèm năm mã hex
   Bảng màu lan man là dấu hiệu AI rõ nhất. Liệt kê hex thì không chệch được.

3. **`If a figure is 1000 pixels tall, its outline is about 40 pixels thick`**
   Cho một ví dụ có con số thật. "Dày" là tính từ, "40 trên 1000" thì không
   hiểu sai được.

---

## 4. Tôi sẽ kiểm bằng gì

| phép đo | đạt khi | hiện tại |
|---|---|---|
| Tách được 8 cụm rời | đúng 8 | — |
| **Nét đen chiếm** | **35–45%** | 0.3% |
| **Số màu >2% diện tích** | **5–6** | nhiều |
| **Bề dày nét / chiều cao** | **3–5%** | — |
| Chân cùng mặt đất | lệch < 1% | — |

Cộng hai phép nhìn: **thu xuống 50px**, và **đổ đặc một màu** xem bóng
hình có đọc ra không.

Tấm nào trượt thì **sửa prompt rồi sinh lại một con**, không làm tiếp 19 con.

---

## 5. Sau khi con thử đạt

Mười chín con còn lại dùng **y nguyên prompt này**, chỉ đổi hai đoạn:
`THE CHARACTER` và năm mã màu. Mọi ràng buộc về nét, cỡ, bố cục giữ nguyên
từng chữ — đó là thứ làm cả dàn hai mươi con trông cùng một bộ.

Và mỗi con **vẫn phải đo lại bốn số** trước khi nhập. Hai mươi con sinh ra
cùng một prompt vẫn lệch nhau, vì công cụ sinh ảnh không lặp lại chính xác.
