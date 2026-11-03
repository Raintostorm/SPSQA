# Yêu cầu art — bộ UI đời 3

**Đính kèm:** `outdata/reference/ss_153.webp` và `ss_154.webp`.

---

## 1. Vì sao hai đời trước đều hỏng — số đo

| | số màu | viền sắc nét |
|---|---|---|
| Đời 1, gỗ chạm trổ | 10 | 4.2% |
| **Đời 2, phẳng** | **2** | **2.6%** |
| **Game tham chiếu** | **5–6** | **7.5–21%** |

Hai kết luận, và cả hai đều ngược với trực giác:

**Đời 2 hỏng vì quá ÍT MÀU, không phải vì thiếu hoa văn.** Hai màu một hình
là mảng phẳng, không phải nút. Thêm hoa văn vào không chữa được — thêm
**tầng màu** mới chữa được.

**Đời 1 hỏng vì viền NHOÈ, dù lắm chi tiết.** 4.2% cạnh sắc nghĩa là mọi thứ
chuyển sắc mềm. Chi tiết nhiều mà nét mềm thì thành rối, không thành sắc nét.

Nên đích là: **5–6 màu phẳng, cạnh giữa chúng CỨNG, viền đen dày.**

---

## 2. Cấu tạo một nút — năm tầng, không hơn

Nhìn nút `Strike!` của họ, tách ra đúng năm thứ:

```
1. viền ngoài     đen tuyền, dày
2. mặt trên       màu gốc làm sáng lên    (chiếm ~40% chiều cao)
3. mặt dưới       màu gốc làm tối đi      (chiếm ~60%)
4. gờ sáng        một vạch mảnh sáng ngay dưới viền trên
5. gờ tối         một vạch mảnh tối ngay trên viền dưới
```

Ranh giới giữa 2 và 3 là **một đường ngang cứng**, không phải chuyển sắc.

Đó là toàn bộ "độ phức tạp" cần có. Không đinh tán, không vân gỗ, không
hoa văn góc, không bo kép.

---

## 3. Khối STYLE — dán vào mọi prompt

```
STYLE — flat vector UI for a casual mobile game.

STRUCTURE — every shape is built from exactly these layers:
* a THICK PURE BLACK outline (#000000) hugging the rounded corners
* an UPPER face, the base colour lightened, covering the top ~40%
* a LOWER face, the base colour darkened, covering the bottom ~60%
* one THIN BRIGHT line just inside the top edge
* one THIN DARK line just inside the bottom edge

Every boundary between these layers is a HARD EDGE. No gradient, no
blur, no airbrush anywhere.

COLOUR
* 5 to 6 flat colours per shape, counting the black outline. Not fewer —
  two or three flat tones looks unfinished. Not more — it turns to mud.

CORNERS
* Generously rounded, about one fifth of the shape's shorter side.

MUST NOT APPEAR
No gradients. No gloss, no glass shine, no emboss, no 3D bevel. No drop
shadow. No wood grain, no stone, no metal plating, no rivets, no studs,
no corner ornaments, no double borders, no engraved patterns. No texture
or noise of any kind.

The face of every button is EMPTY — no text, no icons, no numbers. The
game draws those on top.

Not pixel art. Not 3D. Not skeuomorphic.
```

---

## 4. Tấm 1 — Nút và khung, 12 hình (3 hàng × 4 cột)

| # | hình | màu gốc |
|---|---|---|
| 1 | Nút chính | `#F09000` cam |
| 2 | Như #1, **bấm xuống** | — |
| 3 | Nút phụ | `#7B3FD4` tím |
| 4 | Như #3, bấm xuống | — |
| 5 | Nút khoá | `#5A5A5A` xám |
| 6 | Nút vuông nhỏ (✕, tạm dừng) | `#3A3F46` |
| 7 | Viên thuốc dài, hơi trong | `#1C2430` |
| 8 | Thẻ Common | `#706050` |
| 9 | Thẻ Rare | `#0070B0` |
| 10 | Thẻ Epic | `#9000D0` |
| 11 | Thẻ Legendary | `#E0A000` |
| 12 | Ô lưới, rất mờ | đen ~20% đục |

> Prompt:
>
> Draw 12 UI shapes for a casual mobile game as a 3 row x 4 column grid
> on a fully transparent background.
>
> Left to right, top to bottom:
> 1. a rounded rectangle button, base colour orange #F09000
> 2. the same button pressed: clearly darker overall, and the bright top
>    line and dark bottom line swapped so it reads as pushed in
> 3. a rounded rectangle button, base colour purple #7B3FD4
> 4. the same purple button pressed, in the same way
> 5. a rounded rectangle button, base colour grey #5A5A5A, flat and
>    lifeless so it reads as disabled
> 6. a small rounded SQUARE button, base colour dark slate #3A3F46
> 7. a long rounded PILL bar, base colour very dark blue-grey #1C2430,
>    slightly translucent
> 8. a wide rounded card, base colour muted grey-brown #706050
> 9. the same card, base colour blue #0070B0
> 10. the same card, base colour purple #9000D0
> 11. the same card, base colour gold #E0A000
> 12. a rounded SQUARE grid cell, a very faint dark shape, only just
>     visible, with a thin outline
>
> Cards 8 to 11 are the SAME shape and size, differing only in colour.
> Buttons 1 and 2 must align pixel for pixel, and so must 3 and 4.
>
> [DÁN KHỐI STYLE Ở MỤC 3]
>
> At least 60 pixels of transparent space between neighbours and 40
> pixels of margin around the grid. Nothing may touch.
>
> TRUE TRANSPARENT alpha background — no magenta, no white, no
> checkerboard. The alpha must be ANTI-ALIASED, fading over about one
> pixel; do not threshold or posterise it.
>
> No text, no labels, no numbers, no watermark.

Đặt file: `artsrc/ui/ui_v4_shapes.png`

---

## 5. Tấm 2 — Nền, 6 tấm, mỗi tấm 1125×2436

Nền đời 2 đã **đạt** — đo ra vùng trống phẳng (độ lệch chuẩn 0.5–0.6) và
sáng đúng mức. **Không cần làm lại.**

Chỉ một tấm cần sửa: `bg_home_portrait` có dãy lều ở đáy (từ y=2138) rơi
đúng chỗ nút CHƠI. Nếu làm lại thì chỉ tấm đó, với vùng trống kéo tới
y=2436.

---

## 6. Tôi kiểm bằng gì

| | đạt khi | đời 2 đo được |
|---|---|---|
| Tách đúng 12 hình | 12 | 12 ✓ |
| **Số màu mỗi hình** | **5–6** | **2** ✗ |
| **Viền sắc nét** | **8–15%** | **2.6%** ✗ |
| Cặp thường/bấm trùng khít | lệch < 1/255 | — |
| 4 thẻ bậc cùng cỡ | lệch < 3px | — |
| Alpha | 256 mức, 0% magenta | ✓ |

Hai dòng in đậm là hai dòng đời 2 trượt. Nếu tấm mới đạt hai dòng đó thì
coi như xong — các dòng khác đời 2 đã đạt sẵn.
