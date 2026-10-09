# Yêu cầu art — Hai tấm nền SIÊU RỘNG

Đợt 6. Thay hẳn cách ghép dải. Bỏ `PROMPT_BACKGROUND.md` mục B.

---

## 1. Vì sao bỏ cách cũ

Tôi đang lấp hai bên bằng cách **lật gương rìa ảnh**. Mạch nối thì liền
thật, nhưng ảnh bị nhân đôi — hai lá cờ xanh giống hệt, hai hàng thông
giống hệt. Mắt bắt ngay.

Vẽ hẳn một tấm rộng thì không có chỗ nào lặp. Đổi lại phải vẽ lại cả hai
tấm nền.

---

## 2. Cần gì

| tên file | cỡ xin | thay cho |
|---|---|---|
| `bg_prep_wide.png` | **3024 × 1080** | `bg_prep.png` |
| `bg_field_wide.png` | **3024 × 1080** | `bg_field.png` |

**2.8 : 1.** Rộng hơn màn của tôi (2.59:1) một chút để còn dư.

Hai tấm **cùng kích thước**, và quan trọng hơn: **cùng điểm nhìn, cùng
đường chân trời, cùng bảng màu**. Game đổi qua lại giữa hai tấm ngay tại
chỗ; lệch tông là mỗi lần vào trận màn hình giật một cái.

---

## 3. Luật bố cục — chỉ một luật, nhưng nghiêm

```
+------- 552 -------+--------- 1920 ---------+------- 552 -------+
|                   |                        |                   |
|   TRANG TRI       |   KHUNG CHOI 16:9      |   TRANG TRI       |
|   thoai mai       |   bo cuc CO DINH       |   thoai mai       |
|                   |                        |                   |
+-------------------+------------------------+-------------------+
0                  552                     2472                3024
```

**Khung giữa 1920 × 1080 là nơi mọi thứ trong game xảy ra.** Toạ độ ô,
toạ độ mặt đường, chỗ lính đứng — tất cả tính theo khung đó và đã cố định
trong mã. Khung đó phải giữ đúng bố cục mô tả ở mục 4.

**Hai lề 552px mỗi bên là trang trí thuần tuý.** Không bao giờ có lính
đứng ở đó. Vẽ gì cũng được: rừng rậm, vách đá, lều trại, cờ xí.

Trên màn 16:9 thì hai lề này bị cắt mất hoàn toàn — nên **không được đặt
thứ gì quan trọng vào đó**. Nó chỉ là phần thưởng cho người có màn rộng.

---

## 4. Trong khung giữa — toạ độ tính trên khung 3024 × 1080

### `bg_prep_wide.png` (sân xếp quân)

| vùng | toạ độ | yêu cầu |
|---|---|---|
| dải HUD trên | y 0 – 170 | **yên tĩnh** — trời, núi xa, sương |
| sân xếp quân | y 334 – 870 | **phải phẳng và đều** |
| lằn giữa | x 1373 – 1651 | con đường dọc chia hai phe |
| dải HUD dưới | y 828 – 1080 | **yên tĩnh** |
| ô phe ta | x 817 – 1373 | nằm trong vùng phẳng |
| ô phe địch | x 1651 – 2207 | nằm trong vùng phẳng |

**AI ĐỪNG VẼ Ô.** Ô là sprite rời, game tự đặt vào đúng toạ độ lattice.
Lần trước AI vẽ lưới có phối cảnh — bước cột 133/141/148/153px từ trên
xuống, ô dưới to hơn ô trên 15% — mà lưới trong game là chữ nhật cứng,
không bộ số nào khớp nổi cả 16 ô.

### `bg_field_wide.png` (mặt đường giao tranh)

| vùng | toạ độ | yêu cầu |
|---|---|---|
| mặt đường | y 330 – 750 | chạy ngang **suốt chiều rộng 3024**, hai mép hơi lượn |
| trên đường | y 0 – 330 | cỏ, cây xa, thoáng |
| dưới đường | y 750 – 1080 | cỏ, bụi, thoáng |

Mặt đường chiếm **39% chiều cao** — rộng hơn bản cũ nhiều. Bản cũ đường
chỉ chiếm 24% nên lính đứng tràn ra cỏ, và phần cỏ trống phía dưới to
đến mức nhìn như lỗi.

Đường phải **liên tục từ mép trái tới mép phải**, không đứt, không rẽ.
Lính đi dọc nó từ trái sang phải.

---

## 5. Prompt — dán vào công cụ sinh ảnh

Đính kèm `artsrc/bg_prep_ai2.png` làm tham chiếu phong cách, và
`outdata/layout_wide.png` làm sơ đồ vùng.

> **A — `bg_prep_wide.png`, 3024×1080, ultra-wide landscape**
>
> A battlefield ground for a cartoon medieval strategy game. Flat cartoon
> style, thick dark outlines, bright saturated greens and warm earthy browns,
> soft shading, light from the upper left.
>
> Composition across the width: the MIDDLE 63% of the image is an open
> battlefield; the LEFT 18% and RIGHT 18% are dense decorative scenery.
>
> In the middle: a wide, EVEN, FLAT expanse of short grass and packed earth
> filling the central band of the height — uniform in tone and texture, no
> objects, no tree shadows, no puddles, no rocks crossing it. A narrow
> vertical dirt track runs straight down the exact centre of the image,
> dividing that flat area into a left half and a right half. A blue banner on
> a pole stands at the left end of the flat area, a red banner at the right
> end.
>
> The top strip is distant scenery only: pale sky, hazy blue mountains, a far
> treeline, all low contrast. The bottom strip is calm ground fading slightly
> darker.
>
> The left and right eighteen percent may be rich and detailed: dense pine
> forest, mossy cliffs, wooden fences, boulders, tents, hanging banners. The
> two sides must look DIFFERENT from each other — not mirrored.
>
> IMPORTANT: do NOT draw any grid, squares, tiles, hexagons, platforms, wooden
> boards or placement markers. The ground must be bare.
>
> No characters, no creatures, no UI, no text, no watermark, no border.
>
> ---
>
> **B — `bg_field_wide.png`, 3024×1080, ultra-wide landscape**
>
> The same world as image A, same art style, same palette, same lighting, same
> horizon height — but now a close view of the road where the armies meet.
>
> A broad dirt road runs HORIZONTALLY across the entire width of the image,
> from the far left edge to the far right edge, unbroken. Its edges wave
> gently. The road occupies the middle 39% of the image height.
>
> Above the road: grass, scattered small bushes, distant trees, calm and
> uncluttered. Below the road: grass, a few small rocks and tufts, also calm.
> Nothing tall or high-contrast anywhere near the road itself — soldiers will
> be drawn standing on it.
>
> The left and right edges may be denser: pine trees, rocks, fences, leading
> the eye inward. Left and right must look DIFFERENT, not mirrored.
>
> No characters, no creatures, no UI, no text, no watermark, no border.

---

## 6. Ba thứ hậu kỳ không cứu được

1. **Hai tấm khác tông hoặc khác đường chân trời.** Game đổi qua lại ngay
   tại chỗ, lệch là giật.
2. **Vùng giữa có vật thể.** Cây hay đá rơi vào `y 334–870` của tấm A, hay
   vào mặt đường của tấm B, là lính đứng đè lên.
3. **Hai bên lật gương giống nhau.** Đó đúng là lỗi tôi đang muốn thoát.

Sai kích thước thì tôi co lại, miễn **đúng tỉ lệ 2.8 : 1**. Thà to hơn.

---

## 7. Đặt file ở đâu

```
artsrc/bg_prep_wide_ai.png
artsrc/bg_field_wide_ai.png
```

---

## 8. Xong rồi tôi làm gì

```
1. do lai bon phan so luoi theo khung 3024 (chi doi mau so, bo cuc giu nguyen)
2. XOA SideStrips.cs va make_side_strips.py — khong con viec gi
3. bo zoom 1.5 cua anh giao tranh — anh moi da dung ti le roi
4. noi rong camera: thoi cat ve 16:9, cho toi 2.8:1
   (van giu LetterboxCamera cho man rong hon 2.8)
5. render thu ca man hinh gui ban xem
```
