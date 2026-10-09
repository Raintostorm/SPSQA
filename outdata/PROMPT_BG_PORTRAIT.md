# Yêu cầu art — Hai tấm nền DỌC (màn điện thoại)

Thay cho hai tấm ngang 3024×1080. Xem sơ đồ `outdata/layout_portrait.png`.

---

## 1. Cỡ và vì sao

| tên file | cỡ |
|---|---|
| `bg_prep_portrait.png` | **1242 × 2796** |
| `bg_field_portrait.png` | **1242 × 2796** |

Tỉ lệ **0.444** — cao hơn cả máy 20:9, nên không máy nào hở mép.

Nhưng máy **16:9 là máy NGẮN nhất**, nó chỉ thấy phần giữa cao 2208px. Nên:

```
y    0 ..  294   TRAN — may 16:9 CAT MAT, chi trang tri
y  294 .. 2502   VUNG AN TOAN — moi may deu thay
y 2502 .. 2796   TRAN — cat mat
```

**Không được đặt thứ gì quan trọng vào hai dải tràn.**

---

## 2. Bố cục trong vùng an toàn

| vùng | y | yêu cầu |
|---|---|---|
| HUD trên | 294 – 574 | **yên tĩnh** — trời, núi xa |
| 15 ô phe địch | 574 – 1188 | **phẳng và đều** |
| lằn giao tranh | 1188 – 1568 | ranh giới hai phe, chạy NGANG |
| 15 ô phe ta | 1568 – 2182 | **phẳng và đều** |
| HUD dưới | 2182 – 2502 | **yên tĩnh** — nút bấm đè lên |

Lưới **5 cột × 3 hàng** mỗi phe, nằm trong `x 50 – 1192`:

```
buoc cot 229px   buoc hang 205px   o ve ra 215 x 192
tam hang DICH : y 676, 881, 1086
tam hang TA   : y 1670, 1875, 2080
tam lan       : y 1378
```

**AI ĐỪNG VẼ Ô.** Ô là sprite rời, game tự đặt vào đúng toạ độ. Lần trước
AI vẽ lưới có phối cảnh nên không bộ số nào khớp nổi 16 ô.

---

## 3. Vì sao 5×3 chứ không 4×4

Ô đất đã có vẽ ở tỉ lệ **1.03** (gần vuông). Trong khung dọc:

| bố cục | số ô | ô ra | kéo méo |
|---|---|---|---|
| 4 × 4 | 16 | 269 × 144 | **81%** |
| 4 × 3 | 12 | 269 × 192 | 36% |
| **5 × 3** | **15** | **215 × 192** | **8%** |

5×3 giữ được gần đủ số ô mà ô gần như không méo. Số hàng/cột nằm trong
`GameBalance`, không phải trong mã — đổi là đổi dữ liệu.

---

## 4. Prompt — dán vào công cụ sinh ảnh

Đính kèm `outdata/layout_portrait.png` và một tấm nền ngang cũ làm tham
chiếu phong cách.

> **A — `bg_prep_portrait.png`, 1242×2796, tall vertical**
>
> A battlefield seen from above for a cartoon medieval strategy game. Flat
> cartoon style, thick dark outlines, saturated greens and warm earthy browns,
> soft shading, light from the upper left.
>
> Vertical composition, top to bottom:
> * Top eighth: distant scenery only — pale sky, hazy blue mountains, a far
>   treeline. Low contrast, nothing eye-catching.
> * Then a WIDE, EVEN, FLAT expanse of short grass and packed earth filling
>   the upper-middle. Uniform tone, no objects.
> * Across the exact middle: a horizontal dirt track running from the left
>   edge to the right edge, separating the upper half from the lower half.
>   A red banner on a pole at the left end, a blue banner at the right end.
> * Below it: another WIDE, EVEN, FLAT expanse, matching the upper one.
> * Bottom eighth: calm ground fading slightly darker.
>
> The two flat expanses are where game pieces stand — they must read as one
> continuous surface each, with no rocks, puddles, tree shadows or paths
> crossing them.
>
> Only the far left and right edges may be detailed: pine trees, fences,
> boulders, bushes — a narrow strip, no wider than a twentieth of the image.
>
> IMPORTANT: do NOT draw any grid, squares, tiles, hexagons, platforms or
> placement markers. The ground must be bare.
>
> No characters, no creatures, no UI, no text, no watermark, no border.
>
> ---
>
> **B — `bg_field_portrait.png`, 1242×2796, tall vertical**
>
> The same world as image A, same art style, palette, lighting and horizon —
> but now the two armies have met and the whole middle is open ground.
>
> A broad band of packed dirt and short grass fills the central three
> quarters of the height, from the left edge to the right edge, unbroken.
> This is where soldiers walk toward each other from top and bottom.
>
> The top eighth and bottom eighth are calmer: grass, a few small bushes and
> rocks, fading darker at the very top and bottom.
>
> Nothing tall or high-contrast anywhere in the central band.
>
> Only the far left and right edges may be denser: pine trees, rocks, fences.
> Left and right must look DIFFERENT from each other, not mirrored.
>
> No characters, no creatures, no UI, no text, no watermark, no border.

---

## 5. Ba thứ hậu kỳ không cứu được

1. **Hai tấm khác tông hoặc khác đường chân trời.** Game đổi qua lại ngay
   tại chỗ, lệch là mỗi lần vào trận màn hình giật một cái.
2. **Vật thể lọt vào vùng đặt ô** (`y 574–1188` hoặc `1568–2182`). Ô là ảnh
   bán trong suốt đặt đè lên; nền chỗ đậm chỗ nhạt là lộ ngay.
3. **Trang trí hai mép quá dày.** Khung dọc chỉ rộng 1242px; mép rậm quá là
   nuốt mất cột ô ngoài cùng. Giữ trong khoảng 60px mỗi bên.

Sai kích thước thì tôi co lại, miễn **đúng tỉ lệ 1242:2796**.

---

## 6. Đặt file ở đâu

```
artsrc/portrait/bg_prep_portrait.png
artsrc/portrait/bg_field_portrait.png
```

---

## 7. Xong rồi tôi làm gì

```
1. do lai bon phan so luoi theo khung 1242x2796
2. doi GridRows/GridCols trong GameBalance: 4x4 -> 3x5
3. noi rong camera: lay BE NGANG lam chuan thay vi chieu cao
4. xoay truc giao tranh sang doc (5 cho trong ma, da ra het)
5. dung lai HUD theo khung 1125x2436
6. render thu ca man hinh gui ban xem truoc khi dung vao Unity
```
