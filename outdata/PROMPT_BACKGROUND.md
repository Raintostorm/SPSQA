# Yêu cầu art — Nền trận đấu mở rộng

Đợt 4. Thay cho `bg_prep.png` hiện tại, và thêm dải trang trí hai bên.

---

## 1. Vì sao xin HAI tấm chứ không một tấm siêu rộng

Màn hình mỗi máy một tỉ lệ. Cửa sổ của tôi đang 2.57:1, máy khác 16:9,
16:10, 21:9. Một tấm nền cố định sẽ thừa hoặc thiếu ở mọi tỉ lệ trừ đúng
một cái.

Nên tách:

| tên file | cỡ xin | vai trò |
|---|---|---|
| `bg_prep.png` | **1920 × 1080** | sân đấu, luôn hiện đủ, không bao giờ bị cắt |
| `bg_side.png` | **700 × 1080** | dải trang trí, tôi **lật gương** đắp vào phần thừa hai bên |

Dải bên rộng bao nhiêu cũng lấp được, vì tôi kéo và lặp nó. Sân 16:9 thì
giữ nguyên tỉ lệ — mọi toạ độ ô trong code tính theo nó.

---

## 2. Vì sao AI ĐỪNG vẽ lưới ô

Tấm lưới lần trước phải xoá đi vẽ lại. Lý do đo được: AI vẽ lưới có **phối
cảnh** — bước cột từ trên xuống là 133 / 141 / 148 / 153 px, ô dưới to hơn ô
trên 15%. Còn `BattleGrid` trong game là **lưới chữ nhật cứng**, bước đều
tăm tắp. Không có bộ số nào khớp được cả 16 ô; xấp xỉ tốt nhất vẫn lệch
~30px, tức là lính đứng trượt ra ngoài ô.

Nên lần này: **AI vẽ mặt đất phẳng, không ô nào cả.** Script
`tools/align_prep_grid.py` vẽ 32 ô vào đúng toạ độ lattice. Khớp tuyệt đối.

---

## 3. `bg_prep.png` — bố cục bắt buộc

Xem ảnh sơ đồ gửi kèm. Toạ độ tính trên khung 1920 × 1080:

| vùng | px | yêu cầu |
|---|---|---|
| dải HUD trên | y 0 – 170 | **yên tĩnh** — trời, núi xa, sương. Bảng wave và bảng buff đè lên |
| sân xếp quân | y 364 – 892 | **phải phẳng và đều** — không vật thể, không bóng cây, không vũng nước |
| lằn giữa | x 788 – 1132 | ranh giới hai phe: con đường, con suối, hoặc vệt đất mòn chạy dọc |
| dải HUD dưới | y 828 – 1080 | **yên tĩnh** — nút XUẤT QUÂN và ROLL đè lên |
| rìa trái/phải | x 0 – 99, 1821 – 1920 | được phép rậm: cây, hàng rào, cờ |

Khối ô phe ta: x 99 – 788. Khối ô phe địch: x 1132 – 1821. Hai khối đối
xứng qua tâm.

**"Phẳng và đều" là yêu cầu nghiêm ngặt nhất.** Tôi vẽ 32 ô bán trong suốt
lên đó. Nền chỗ nào đậm chỗ nào nhạt thì ô cũng đậm nhạt theo, nhìn ra
ngay là dán lên.

---

## 4. Prompt — dán vào công cụ sinh ảnh

Đính kèm `Assets/_Project/Art/Backgrounds/bg_prep.png` (bản cũ) và sơ đồ
`layout_bg.png` làm tham chiếu.

> **A — `bg_prep.png`, 1920×1080 landscape**
>
> A top-down-ish battlefield ground for a cartoon medieval strategy game.
> Flat cartoon style, thick dark outlines, bright saturated greens and warm
> browns, soft shading, light coming from the upper left.
>
> The CENTRE of the image, a wide horizontal band covering the middle half of
> the height, must be an EVEN, FLAT expanse of short grass and packed earth —
> uniform in tone and texture, no objects, no tree shadows, no puddles, no
> rocks, no paths crossing it. Game pieces will be placed on this band and it
> must read as a single continuous surface.
>
> A narrow vertical dirt track runs down the exact centre of the image,
> dividing the flat band into a left half and a right half.
>
> The top strip is distant scenery only: pale sky, hazy blue mountains, a
> treeline far away, all low contrast.
>
> The bottom strip is also calm: more of the same flat ground, fading slightly
> darker.
>
> Only the far left and far right edges may be detailed: pine trees, wooden
> fences, banners on poles, bushes.
>
> IMPORTANT: do NOT draw any grid, squares, tiles, hexagons, platforms, wooden
> boards, trays, or placement markers of any kind. The ground must be bare.
>
> No characters, no creatures, no UI, no text, no watermark, no border.
>
> ---
>
> **B — `bg_side.png`, 700×1080 portrait**
>
> A vertical strip of dense cartoon forest edge, same art style and palette as
> image A: flat cartoon, thick dark outlines, saturated greens, soft shading.
>
> Pine trees of varying heights, ferns, bushes, mossy rocks, a wooden fence
> post or two, stacked from the bottom of the frame to the top so the strip
> is full and has no empty gaps.
>
> The LEFT edge of the strip must be the darkest and densest; the RIGHT edge
> should thin out slightly and get lighter, so it can blend into open ground.
>
> Slightly darker and more desaturated overall than image A — this sits in
> the periphery and must not pull the eye.
>
> Must tile VERTICALLY: the very top row of pixels and the very bottom row
> must match, so the strip can repeat seamlessly up and down.
>
> No characters, no UI, no text, no watermark, no border, no sky, no horizon.

---

## 5. Ba thứ hậu kỳ không cứu được

1. **Vùng giữa có vật thể.** Cây, đá, vũng nước nằm trong y 364–892 là hỏng
   — xoá đi để lại vết, mà để nguyên thì ô đè lên trông bẩn.
2. **AI vẫn vẽ lưới.** Nếu nó cứng đầu vẽ ô, bảo nó "bare ground only, no
   tiles" và sinh lại. Tôi xoá được nhưng tốn một lượt và để lại vết mờ.
3. **`bg_side.png` không nối được theo chiều dọc.** Mép trên và mép dưới
   lệch thì chỗ nối hiện thành một đường ngang rõ mồn một.

Sai kích thước thì không sao, tôi co lại. Thà to hơn còn hơn nhỏ hơn.

---

## 6. Đặt file ở đâu

```
artsrc/bg_prep_ai2.png     <- tam A, de nguyen, chua cat chua sua
artsrc/bg_side_ai.png      <- tam B
```

Giữ nguyên `artsrc/bg_prep_ai.png` cũ — đó là đường lui.

---

## 7. Xong rồi tôi làm gì

```
1. chay align_prep_grid.py tren tam A  -> ve 32 o dung lattice
2. viet SideStrips.cs: lat guong bg_side dap vao phan thua hai ben,
   theo ti le cua so, thay cho camera to mau phang
3. them bong tiep dat cho linh
4. render thu ca man hinh gui ban xem truoc khi dung vao Unity
```
