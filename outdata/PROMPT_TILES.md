# Yêu cầu art — Ô đất đặt quân

Đợt 5. Dùng chung với nền ở `PROMPT_BACKGROUND.md`.

---

## 1. Vì sao xin ô RỜI chứ không vẽ vào nền

Nền `bg_prep_ai2.png` cố ý **không có ô nào**. Lưới do AI vẽ luôn có phối
cảnh — lần trước đo được bước cột 133/141/148/153px từ trên xuống, ô dưới
to hơn ô trên 15%. Mà `BattleGrid` trong game là lưới chữ nhật cứng, bước
đều tăm tắp. Không bộ số nào khớp nổi cả 16 ô.

Nên xin **ô rời**, tôi nhân lên 32 lần đặt vào đúng toạ độ lattice. Khớp
tuyệt đối, và sau này đổi cỡ lưới không phải vẽ lại nền.

---

## 2. Cần gì

Một tấm **1500 × 1460**, nền magenta `#FF00FF`, chứa **4 ô xếp 2×2**:

```
+-------------------+-------------------+
|  1 PHE TA         |  2 PHE TA         |
|    binh thuong    |    DANG CHON      |
+-------------------+-------------------+
|  3 PHE DICH       |  4 PHE DICH       |
|    binh thuong    |    DANG CHON      |
+-------------------+-------------------+
```

Mỗi ô khoảng **600 × 580 px**.

### Tỉ lệ là con số quan trọng nhất ở đây

Ô thật trong game là **131 × 127**, tức **1.03 : 1 — gần vuông**.

Vẽ sai tỉ lệ thì tôi phải bóp méo, mà bóp méo ô thì viền đá bên trái phải
dày mỏng khác nhau, nhìn ra ngay.

(Con số này vừa đổi: lưới mới thu lại để không đạp vào cây cối hai bên.
Nếu bạn còn giữ bản yêu cầu cũ ghi 640×500 thì **bỏ đi**, nó sai tỉ lệ.)

**Bốn ô phải cùng hình dáng, cùng kích thước, cùng góc nhìn.** Chúng thay
nhau tại chỗ — lệch một chút là lúc bấm chọn ô bị giật.

---

## 3. Ba ràng buộc, theo thứ tự quan trọng

**a. Giữa ô phải phẳng và trống.** Lính đứng lên đó. Hoa văn, vết nứt, cỏ
mọc giữa ô sẽ chạy xuyên qua chân lính.

**b. Viền mỏng, khoảng 6% bề ngang ô.** Trong game các ô nằm **gần như sát
nhau** — khe giữa hai ô chỉ 8px trên tổng 139px bước. Hai viền kề nhau là
đã thành một dải rồi; viền dày nữa thì cả bảng thành một khối rối, mất hết
mặt ô.

**c. Không phối cảnh trong từng ô.** Ô là hình chữ nhật nhìn hơi chếch từ
trên, bốn cạnh song song. Không hình thang, không nghiêng.

---

## 4. Prompt — dán vào công cụ sinh ảnh

Đính kèm `artsrc/bg_prep_ai2.png` làm tham chiếu phong cách và bảng màu.

> **Một tấm 1500×1460, nền magenta đặc #FF00FF, chứa 4 ô xếp lưới 2×2.**
>
> Each of the four items is a placement tile for a cartoon medieval strategy
> game: a shallow patch of packed dirt set into the ground, seen from a
> slightly tilted top-down angle. Same art style as the attached background:
> flat cartoon, thick dark outlines, warm earthy browns and saturated greens,
> soft shading, light from the upper left.
>
> Each tile is ALMOST SQUARE — about 600 wide by 580 tall — with softly
> rounded corners. All four edges parallel; no perspective distortion, no
> trapezoid, no tilt.
>
> The CENTRE of every tile must be FLAT, EVEN, plain packed earth — no cracks,
> no grass tufts, no pebbles, no runes, no patterns, no inner frame. Game
> characters stand there. Only a very faint, uniform soil grain.
>
> All detail belongs to the RIM, and the rim must be THIN — about 6% of the
> tile width. In game these tiles sit almost flush against each other in a 4x4
> block, so two neighbouring rims already read as one band; a thick rim makes
> the board unreadable.
>
> The four states:
> 1. ALLY, idle — rim of cool grey stone blocks with a thin blue-steel edge.
> 2. ALLY, selected — identical tile, but the rim glows warm gold, and a soft
>    gold light spills a short way inward onto the earth from the rim.
> 3. ENEMY, idle — identical silhouette, rim of dark reddish-brown stone with
>    a rusted iron edge.
> 4. ENEMY, selected — identical to 3, but the rim glows red.
>
> All four MUST share the exact same outline, size, corner radius and viewing
> angle. Only colour and glow differ. Arrange them in a clean 2x2 grid with a
> wide magenta gap between them, nothing overlapping, nothing touching the
> edge of the sheet.
>
> Solid magenta #FF00FF background. No magenta, pink or purple anywhere in the
> artwork itself. No characters, no numbers, no text, no icons, no drop shadow
> outside the tile, no watermark, no frame around the whole sheet.

---

## 5. Ba thứ hậu kỳ không cứu được

1. **Bốn ô khác hình dáng nhau.** Chúng thay nhau tại chỗ. Tôi ép được về
   cùng khung, nhưng ép thì ô nào vẽ to hơn bị bóp méo.
2. **Giữa ô có hoa văn.** Lính đứng đè lên, nhìn như chân mọc rễ.
3. **Viền dày.** Ô sát nhau, viền dày thì mất hết mặt ô.

Nền magenta cứ để nguyên — tôi tách được cả phần rìa đã khử răng cưa. Chỉ
cần trong hình đừng có màu hồng/tím thật.

Sai kích thước thì không sao, tôi co lại — miễn **đúng tỉ lệ gần vuông**.
Thà to hơn còn hơn nhỏ hơn.

---

## 6. Đặt file ở đâu

```
artsrc/ui/tiles_ai.png
```

---

## 7. Xong rồi tôi làm gì

```
1. tach nen magenta, khu vien  (tools/import_ui_art.py da co san phep nay)
2. cat 4 o, ep ve CUNG mot khung  (giong cach da lam voi reel_slot)
3. dat 32 o luc chay game, KHONG ve vao anh nen nua
   -> bo luon buoc draw_cells trong align_prep_grid.py
   -> o doi duoc trang thai khi bam chon, viec anh tinh khong lam duoc
4. render thu ca ban roi gui ban xem truoc khi dung vao Unity
```
