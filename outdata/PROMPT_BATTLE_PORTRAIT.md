# Yêu cầu art — Màn giao tranh dọc + ba bảng chỉ số

Hai đơn hàng rời: **một tấm nền** và **một bộ khung bảng**.
Sơ đồ: `outdata/layout_battle_portrait.png`.

---

## Trận đánh chạy thế nào

Một **con đường NGANG** cắt giữa màn. Quân địch vào từ **mép trái** đi sang
phải; quân ta vào từ **mép phải** đi sang trái. Hai bên gặp nhau ở giữa.

Thả theo cột của lưới: ta thả từ cột **phải nhất** trước, địch từ cột
**trái nhất** trước — nên con đứng gần mặt trận nhất ra trước.

Nhân vật vẽ nhìn ngang, nên đi ngang là đúng hướng mặt. Đây là lý do con
đường phải nằm ngang chứ không dọc.

---

# ĐƠN 1 — `bg_battle_portrait.png`

## Cỡ và vùng

**1242 × 2796.** Máy 16:9 là máy ngắn nhất, chỉ thấy `y 294..2502`.

| vùng | y | yêu cầu |
|---|---|---|
| tràn trên | 0 – 294 | bị cắt trên máy 16:9 — chỉ trang trí |
| HUD | 294 – 700 | **yên tĩnh** — bảng wave và buff đè lên |
| **mặt đường** | **700 – 1900** | nơi lính đi, xem bên dưới |
| ba bảng chỉ số | 1900 – 2502 | **bị che kín** — vẽ gì cũng không ai thấy |
| tràn dưới | 2502 – 2796 | bị cắt |

## Mặt đường — phần quan trọng nhất

Cao 1200px, chạy **suốt từ mép trái tới mép phải**, không đứt.

Lính xếp thành **bốn hàng ngang** trong dải này, mỗi hàng cao ~280px. Nên:

* **phải phẳng và đều theo chiều ngang** — lính đi từ mép này sang mép kia,
  nền đổi màu giữa chừng là thấy ngay
* **không vật thể nào trong dải** — không đá to, không bụi cây, không vũng
  nước. Lính đè lên là hỏng
* hai mép trên/dưới của đường được phép có cỏ viền, bụi nhỏ

## Prompt — dán vào công cụ sinh ảnh

Đính kèm `outdata/layout_battle_portrait.png` và `artsrc/portrait/bg_prep_portrait.png`.

> A battlefield background for a cartoon medieval strategy game, tall
> portrait format, 1242 × 2796 pixels. Flat cartoon style, thick dark
> outlines, saturated greens and warm ochre packed earth, soft shading,
> light from the upper left. Match the palette and horizon of the attached
> style reference.
>
> Vertical composition, top to bottom:
> * Top eighth — distant scenery only: pale sky, hazy blue mountains, a far
>   treeline. Low contrast, nothing eye-catching.
> * Below that, a calm band of short grass.
> * **The central 43% of the height is a broad horizontal road of packed
>   earth**, running unbroken from the extreme left edge to the extreme right
>   edge. Its upper and lower rims wave gently and are fringed with grass.
>   The road surface itself must be EVEN and UNIFORM all the way across —
>   no rocks, bushes, puddles, cracks, ruts or colour changes. Armies walk
>   along it from both ends.
> * Below the road: grass again, fading gradually darker toward the bottom.
>
> Only the extreme left and right edges, a strip no wider than a twentieth
> of the image, may carry pine trees, fences or boulders — and NOT where
> they would reach into the road.
>
> No characters, no creatures, no UI, no text, no watermark, no border.

---

# ĐƠN 2 — Khung ba bảng chỉ số

## Cần gì

Ba bảng: **sát thương gây ra**, **chịu sát thương**, **hồi phục**. Chỉ một
bảng mở tại một lúc; ba cái thẻ ở trên để đổi.

Một tấm **1400 × 900**, nền trong suốt (PNG có alpha, **không dùng magenta**),
chứa **bốn món**:

```
+------------------+------------------+------------------+
|  the SAT THUONG  |  the CHIU DON    |  the HOI PHUC    |   3 the, moi cai
|   360 x 150      |   360 x 150      |   360 x 150      |   360x150
+------------------+------------------+------------------+
|                                                        |
|            KHUNG BANG   1300 x 560                     |
|                                                        |
+--------------------------------------------------------+
```

### Ba thẻ — 360 × 150 mỗi cái

Cùng hình dáng, cùng cỡ, **chỉ khác biểu tượng và màu**:

1. **Sát thương** — thanh kiếm bắt chéo, ánh cam-đỏ
2. **Chịu đòn** — cái khiên, ánh lam-thép
3. **Hồi phục** — chiếc lá hoặc bình thuốc, ánh lục

Vẽ ở trạng thái **sáng/đang mở**. Tôi tự làm bản tối cho thẻ đang đóng.

### Khung bảng — 1300 × 560

Khung gỗ-kim loại giống bộ giao diện đang có (`artsrc/ui/ui_sheet.png`).

**Ruột khung phải TRỐNG và TỐI đều** — tôi vẽ tên lính, thanh số và chữ số
lên đó. Hoa văn trong ruột là chữ không đọc được.

**Viền mỏng, không quá 40px**, vì khung chỉ cao 560 mà phải chứa 4 dòng.

## Prompt — dán vào công cụ sinh ảnh

Đính kèm `artsrc/ui/ui_sheet.png` làm tham chiếu phong cách.

> One image, 1400 × 900 pixels, **transparent background (PNG with a true
> alpha channel — do NOT fill the background with any colour)**. The alpha
> must be ANTI-ALIASED: edges fade smoothly, not a hard on/off cutout.
>
> Four separate objects, not touching each other, each fully inside the
> image with clear empty space around it:
>
> Top row — three TABS, each about 360 × 150, side by side, identical in
> shape, size and material, differing only in the emblem and its glow:
> 1. two crossed swords, warm orange-red glow
> 2. a round shield, cool steel-blue glow
> 3. a leaf or potion flask, green glow
>
> Bottom — one wide PANEL FRAME, about 1300 × 560. A rectangular wooden
> frame with iron corner brackets and rivets. Its INTERIOR must be flat,
> dark and completely EMPTY — a plain recessed surface with no pattern,
> no texture detail, no engraving. Text and bars will be drawn on it.
> The frame border must be THIN, no more than 40 pixels thick.
>
> Same art style as the attached UI reference: flat cartoon, thick dark
> outlines, warm wood and grey iron, soft shading, light from the upper left.
>
> No text, no numbers, no icons other than the three emblems described,
> no drop shadow outside the objects, no watermark.

---

## Ba thứ hậu kỳ không cứu được

1. **Vật thể nằm trên mặt đường** (`y 700–1900`). Lính đè lên là lộ.
2. **Ba thẻ khác hình dáng nhau.** Chúng thay nhau tại chỗ, lệch là lúc đổi
   thẻ bị nhảy.
3. **Ruột khung bảng có hoa văn.** Chữ và thanh số đè lên là không đọc được.

Nền **trong suốt**, không magenta. Lần trước tôi xin magenta là sai: công cụ
xuất alpha thật từ đầu, mà tách magenta là phép mất mát — rìa khử răng cưa
chính là màu trộn với nền nên vừa khó tách vừa để lại quầng hồng.

---

## Đặt file ở đâu

```
artsrc/portrait/bg_battle_portrait.png
artsrc/ui/stat_panels.png
```

---

## Xong rồi tôi làm gì

```
1. do lai mep tren/duoi mat duong trong anh -> dat BattleLane
2. cat 3 the + khung bang, do vien 9 manh
3. them bo cong don sat thuong / chiu don / hoi phuc vao Unit  (MA MOI)
4. doi thu tu tha o: ta tu cot phai sang trai, dich tu trai sang phai
5. render thu ca man hinh gui ban xem
```
