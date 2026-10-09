# Yêu cầu art UI — đưa cho AI sinh ảnh

Mọi con số dưới đây lấy từ `BattleHudBuilder.cs`, không phải ước lượng.
Khung tham chiếu **1920×1080**.

---

## 0. Điều quan trọng nhất: ảnh bị KÉO GIÃN

Game dùng **9 mảnh (9-slice)**: bốn góc giữ nguyên, phần giữa bị kéo dài ra.
Nhờ vậy một ảnh nút dùng được cho cả nút rộng 150 lẫn nút rộng 440.

Hệ quả, và đây là chỗ AI hay làm hỏng:

| | |
|---|---|
| **Hai đầu trái–phải** | được phép chạm trổ, đinh tán, bo góc — tuỳ ý |
| **Khúc GIỮA** | phải **đều tăm tắp theo chiều ngang** |

Khúc giữa mà có hoa văn, vết xước, hay vân gỗ chạy dọc thì khi kéo dài ra nó
sẽ nhoè thành vệt. Hình dung một cuộn giấy: hai đầu có hoa văn, thân giữa
trơn — kéo thân ra bao nhiêu cũng không lộ.

**Chiều dọc không bao giờ bị kéo.** Mọi nút cao đúng 112, mọi bảng cao đúng 76.
Nên theo chiều dọc thì vẽ thoải mái: gờ nổi, bóng đổ, vát mép.

---

## ĐỢT 1 — XONG ✅

Art đã nhập, đo được:

| | cỡ ảnh | PPU | viền 9 mảnh | hai đầu |
|---|---|---|---|---|
| `panel` | 1947×245 | 322.37 | 235 px | 72.9 đơn vị |
| `button` | 1949×243 | 216.96 | 242 px | **111.5 đơn vị** |
| `button_down` | 1947×243 | 216.96 | 242 px | 111.5 đơn vị |

Khúc giữa đo ra lệch tối đa **2%** — đều, kéo bao nhiêu cũng không lộ.

**Bài học áp vào layout:** hai nắp chiếm 111.5 đơn vị mỗi bên, nên **không
nút nào được hẹp hơn 223**. Nút tốc độ đang 150 đã phải nới lên 280, và nút
XUẤT QUÂN nới 440 → 520 cho vừa chữ.

**Nền magenta không sao** — `tools/import_ui_art.py` tách được, kể cả phần
rìa đã khử răng cưa (giải ngược phép trộn, không phải xoá thẳng). Lần sau cứ
để magenta, chỉ cần **trong hình đừng có màu hồng/tím** nào thật.

---

## 1. Danh sách cần — ĐỢT 1 (đang hiển thị trên màn)

Xin **PNG nền trong suốt**, vẽ ở **3 lần** kích thước dùng thật để còn nét.

| tên file | cỡ ảnh xin | dùng cho | kích thước thật trên màn |
|---|---|---|---|
| `panel.png` | **900 × 228** | bảng chữ, ô đếm quân, ví coin | 820×76, 250×76, 230×76 |
| `button.png` | **900 × 336** | nút XUẤT QUÂN, ROLL, tốc độ | 440×112, 300×112, 150×112 |
| `button_down.png` | **900 × 336** | chính nút đó lúc đang bị nhấn | như trên |

Ba ảnh phải **cùng một bộ**: cùng chất liệu, cùng hướng sáng, cùng độ dày viền.

`button_down.png` không phải ảnh khác — là **đúng nút đó** nhưng tối hơn và
như bị lún xuống (gờ nổi đảo chiều, bóng đổ co lại). Vẽ lại từ đầu là hai nút
trông như hai bộ khác nhau.

### Vùng được chạm trổ

```
900 px
|<-150->|<------------ 600 giua, PHAI DEU ---------->|<-150->|
 hai dau                                               hai dau
```

150 px mỗi bên ở cỡ 3× ⟶ tôi khai `spriteBorder = 150`, `PPU = 300`.

### Vì sao đúng 228 và 336

Không phải số đẹp bịa ra. Canvas của Unity quy đổi theo
`referencePixelsPerUnit / spritePixelsToUnits` = `100 / 300` = **1/3**:

| ảnh | ÷3 | mục tiêu trên màn |
|---|---|---|
| cao 228 | **76.0** | bảng cao 76 ✓ |
| cao 336 | **112.0** | nút cao 112 ✓ |
| viền 150 | **50.0** | hai đầu 50 đơn vị |

Chia hết, không làm tròn. Và khúc giữa còn lại cho mọi phần tử đều dương:

| | rộng | hai đầu | khúc giữa | bị kéo |
|---|---|---|---|---|
| nút tốc độ | 150 | 50+50 | 50 | 0.25× (bị **nén**) |
| ví coin | 230 | 50+50 | 130 | 0.65× |
| nút ROLL | 300 | 50+50 | 200 | 1.0× |
| XUẤT QUÂN | 440 | 50+50 | 340 | 1.7× |
| bảng chữ | 820 | 50+50 | **720** | **3.6×** |

Bảng chữ trên cùng kéo khúc giữa ra **3.6 lần** — đó là chỗ hoa văn sẽ lộ
rõ nhất nếu AI vẽ lén vào giữa. Nhìn chỗ đó đầu tiên khi nghiệm thu.

---

## 2. Danh sách cần — ĐỢT 2 (màn roll, làm ngay sau)

| tên file | cỡ xin | dùng cho |
|---|---|---|
| `reel_slot.png` | 300 × 300 | một trong 6 ô của vòng quay |
| `reel_slot_win.png` | 300 × 300 | ô đó khi thuộc cụm thắng — viền sáng |
| `icon_coin.png` | 192 × 192 | ô Coin, và icon cạnh số tiền |
| `icon_ally.png` | 192 × 192 | ô Ally |
| `icon_buff.png` | 192 × 192 | ô Buff |
| `icon_hp.png` | 144 × 144 | buff máu |
| `icon_dmg.png` | 144 × 144 | buff sát thương |
| `icon_speed.png` | 144 × 144 | buff tốc đánh |
| `icon_crit.png` | 144 × 144 | buff chí mạng |

Icon là ảnh **vuông, không 9 mảnh**, nên không có ràng buộc "giữa phải đều".
Vẽ thoải mái, chỉ cần nền trong suốt và hình nằm gọn trong khung, chừa đều
khoảng 10% mỗi mép.

---

## 2b. Prompt cho ĐỢT 2

Icon là ảnh **vuông, KHÔNG 9 mảnh** — không vướng ràng buộc "giữa phải đều".

> A set of 2D game UI icons, same art style as the attached image: flat
> cartoon, thick dark outlines, warm wood and aged bronze, soft shading,
> light from above.
>
> Each icon centred in a square canvas with about 10% empty margin on every
> side. Solid magenta background (#FF00FF) is fine. **No magenta, pink or
> purple anywhere in the artwork itself** — it is used as the key colour.
> No text, no numbers, no frame.
>
> 1. `reel_slot` — an empty square socket, recessed, wooden frame with metal
>    corner brackets. Something sits inside it later, so the inside must be
>    dark and plain.
> 2. `reel_slot_win` — the same socket lit up: glowing golden rim, brighter
>    inside. Identical silhouette and size.
> 3. `icon_coin` — a stack of gold coins.
> 4. `icon_ally` — a simple knight helmet seen from the front.
> 5. `icon_buff` — a glowing upward arrow or a small potion bottle.
> 6. `icon_hp` — a red heart.
> 7. `icon_dmg` — a crossed sword and axe.
> 8. `icon_speed` — a winged boot, or a lightning bolt.
> 9. `icon_crit` — a target with an arrow in the centre.

Đính kèm `artsrc/ui/ui_sheet.png` làm tham chiếu phong cách.

Xin **chung một tấm** cũng được, như đợt 1 — tôi cắt ra.

---

## 3. Prompt — dán vào công cụ sinh ảnh

> A set of 2D game UI pieces for a cartoon medieval battle game.
>
> **Style:** flat cartoon with thick dark outlines and soft shading — matching
> a bright, friendly fantasy battlefield with green grass, dirt paths, wooden
> fences and cloth banners. Warm wood and aged bronze materials. Light comes
> from the top. No gradients that look glossy or plasticky, no neon, no
> photorealism.
>
> **Critical layout rule:** each piece is a horizontally stretchable bar. The
> LEFT 150 pixels and the RIGHT 150 pixels may carry decoration — carved ends,
> rivets, metal caps. The MIDDLE 600 pixels must be perfectly UNIFORM along
> the horizontal axis: the same cross-section repeated, no ornament, no
> lettering, no diagonal grain, no highlights that travel along the length.
> Vertical detail is encouraged — raised bevel, inner shadow, a rim.
>
> **Transparent background (alpha), PNG.** No drop shadow spilling outside the
> shape. No text of any kind. No border frame around the image. One piece per
> image, centred, touching the left and right edges exactly.
>
> Produce these three, as one consistent set:
> 1. `panel` — 900×228 — a dark recessed plaque to put light text on. Reads as
>    a carved wooden sign or a dark slate inlay with a metal rim.
> 2. `button` — 900×336 — the same family but raised and inviting: warm amber
>    or honey-coloured face, clear bevel at the top edge, darker base.
> 3. `button_down` — 900×336 — the identical button pressed in: darker face,
>    the bevel inverted so the top edge is now in shadow, the whole face
>    shifted down slightly. Same silhouette, same decoration, same width.

### Nếu công cụ cho đính ảnh tham chiếu

Đính `Assets/_Project/Art/Backgrounds/bg_prep.png` kèm câu:

> Match the art style, outline weight and colour palette of the attached
> background image.

---

## 4. Ba thứ hậu kỳ không cứu được

1. **Khúc giữa có hoa văn.** Cắt không được, làm mờ thì thành vệt bẩn. Đây là
   lỗi hay gặp nhất.
2. **Nền không trong suốt**, hoặc có bóng đổ loang ra ngoài hình. Tách nền tự
   động sẽ ăn mất viền.
3. **Dính sẵn chữ.** Chữ do game vẽ, cỡ và nội dung đổi theo lúc chạy.

Hai thứ *cứu được*, không cần xin lại: sai kích thước (tôi co lại), và lệch
màu nhẹ (tôi chỉnh).

**Về kích thước, nói trước cho đỡ mất công:** hầu hết công cụ sinh ảnh chỉ
cho vài tỉ lệ cố định, không nhận được `900×228` (tỉ lệ 3.95:1). Cứ xin tỉ
lệ ngang nhất mà nó có, miễn là **rộng hơn cao ít nhất 3 lần** — tôi cắt và
co về đúng cỡ. Thà ảnh to hơn cần còn hơn nhỏ hơn.

---

## 5. Đặt file ở đâu

```
artsrc/ui/panel.png
artsrc/ui/button.png
artsrc/ui/button_down.png
```

`artsrc/` là chỗ để **bản gốc AI trả về**. Tôi xử lý rồi xuất sang
`Assets/_Project/Art/UI/`, giữ nguyên bản gốc để còn đường lùi — giống cách
đã làm với `artsrc/bg_prep_ai.png`.

---

## 6. Xong rồi tôi làm gì

```
do lai be rong hai dau that su can      -> dat spriteBorder cho dung
dat PPU 300 de 3x thu ve dung co man
bat mipmap (tools/set_sprite_quality.py)
ghep thu voi chu that, doi 3 co nut     -> xem khuc giua co nhoe khong
```

Bước thứ tư mới là bước thật: tôi dựng ảnh mô phỏng đủ cả ba cỡ nút cạnh
nhau. Khúc giữa có đều hay không, nhìn một cái là biết — và biết **trước**
khi bạn phải mở Unity.

---

## 7. Những thứ sẽ cần sau, chưa phải bây giờ

Liệt kê để bạn gom một lượt khi nhờ AI, đỡ phải quay lại:

| | dùng khi |
|---|---|
| khung thẻ lính + 4 viền theo thời đại | hiện kết quả roll, xem chỉ số lính |
| huy hiệu thời đại (đá / trung cổ / hiện đại / tương lai) | góc thẻ lính |
| khung thanh máu | thay `bar_bg` / `bar_fill` tôi vẽ tạm |
| nút nhỏ vuông 112×112 | bán lính, đóng bảng |
| nền mờ che toàn màn | khi mở bảng roll |
| biểu ngữ THẮNG / THUA | hết trận |

Chưa xin ngay vì mấy cái đó còn phụ thuộc luật chơi chưa chốt — xin sớm thì
sai cỡ, lại phải xin lại.
