# Yêu cầu art — đổi hướng sang phẳng-viền-dày (1125×2436)

Thay cả hướng nghệ thuật, không phải sửa màu. Lý do và số đo ở mục 1.

**Ảnh tham chiếu để đính kèm:** `outdata/reference/spinsquad_*.jpg`

> Một lưu ý trước: ba ảnh đó là ảnh chụp game của hãng khác, lấy từ trang
> App Store. Dùng chúng để chỉ **hướng** — vector phẳng, viền đen dày, màu
> sáng — thì không sao. Đừng bảo công cụ "chép y hệt game này": vừa dễ ra
> art đụng hàng, vừa là chuyện bản quyền không cần dính vào.

---

## 1. Vì sao đổi — số đo, không phải cảm giác

Đo trên ảnh game thật so với art hiện tại:

| | game tham chiếu | của ta | lệch |
|---|---|---|---|
| **Điểm ảnh là viền sắc nét** | **6–11%** | **0.3–1.3%** | **gấp 8–20 lần** |
| Chuyển sắc nhẹ (tô khối mềm) | 13–18% | 8–9% | — |
| Bão hoà | 0.57 | 0.49 | nhẹ |
| Độ sáng | 0.58 | 0.43 | ta tối hơn 1.3× |

Màu chỉ lệch nhẹ. **Chỗ lệch gấp hàng chục lần là viền.**

Đó chính là thứ bị gọi là "sặc mùi AI": công cụ sinh ảnh mặc định vẽ **tô
khối mềm, không viền**, còn game casual vẽ **mảng phẳng, viền đen dày đều**.

Nên mọi prompt dưới đây đều có một đoạn bắt buộc về viền. Đừng bỏ nó.

---

## 2. Đoạn bắt buộc — dán vào MỌI prompt

```
STYLE — this is the most important constraint:

Flat vector cartoon, like a modern casual mobile game.
* Every shape has a THICK, UNIFORM BLACK OUTLINE, clearly visible.
  The outline weight must be the same on large and small shapes.
* Fill each shape with 2 or 3 FLAT tones only. A light tone, a base
  tone, and one shadow tone. Hard edges between tones.
* NO soft gradients. NO airbrushed shading. NO rim light. NO glow
  except where explicitly asked for. NO texture, NO noise, NO grain.
* Bright and cheerful: mid-to-high brightness, clearly saturated
  colours. Not muddy, not dark, not sepia, not "antique".

Not pixel art. Not painterly. Not photorealistic. Not 3D render.
```

Đoạn trên nói đúng bốn thứ mà số đo chỉ ra là đang sai.

---

## 3. Thứ tự làm — và vì sao

Rẻ trước, đắt sau. Mỗi bước nhìn thấy kết quả trước khi trả tiền cho bước kế.

| # | việc | số tấm | ghi chú |
|---|---|---|---|
| 1 | **Bộ UI phẳng** | 1 | rẻ nhất, đổi nhiều nhất |
| 2 | **Nền** | 5 | |
| 3 | **MỘT nhân vật thử** | 1 | **dừng lại xem trước khi làm tiếp** |
| 4 | 19 nhân vật còn lại | 19 | chỉ làm nếu bước 3 đạt |

Bước 3 là chốt chặn. 20 con × 350 khung là thứ đắt nhất dự án, và làm xong
toàn bộ rồi mới phát hiện chưa đúng là mất hai tuần.

---

## 4. Tấm 1 — Bộ UI phẳng

Một tấm, **12 hình**, bố cục 3 hàng × 4 cột, nền trong suốt.

Game tham chiếu **không có một khung gỗ chạm trổ nào**. Thanh buff của họ
là một viên thuốc xám đậm bo góc. Nút chính là một chữ nhật vàng bo góc.
Nút phụ là một ô vuông xám. Hết.

Màn Upgrade của ta hiện có **63 khung gỗ**. Đó là nguồn của chữ "rườm rà".

| # | hình | dùng cho |
|---|---|---|
| 1 | Viên thuốc bo góc, **xám đậm trong suốt 80%**, viền đen | thanh buff, thanh tài nguyên |
| 2 | Chữ nhật bo góc, **vàng sáng**, viền đen, mặt phẳng trống | nút chính (CHƠI, XUẤT QUÂN) |
| 3 | Như #2, **bấm xuống** — tối hơn, lún vào | — |
| 4 | Chữ nhật bo góc, **xanh dương**, viền đen | nút phụ |
| 5 | Như #4, bấm xuống | — |
| 6 | Chữ nhật bo góc, **xám**, viền đen | nút bị khoá |
| 7 | Ô vuông bo góc nhỏ, **xám đậm**, viền đen | nút ✕, nút tạm dừng |
| 8 | Chữ nhật bo góc, **nền kem nhạt**, viền đen | thẻ/dòng danh sách |
| 9 | Ô vuông bo góc, **nền tối**, viền đen dày | ô vật phẩm — chưa có |
| 10 | Như #9, viền **vàng** | ô vật phẩm — đã có |
| 11 | Thanh ngang mảnh, **nền tối**, viền đen | nền thanh máu |
| 12 | Thanh ngang mảnh, **xanh lá sáng**, không viền | ruột thanh máu |

> Prompt:
>
> Draw 12 flat UI shapes for a casual mobile game, arranged as a
> 3 row x 4 column grid on a fully transparent background.
>
> Read left to right, top to bottom:
> 1. a wide rounded-pill bar, dark grey, slightly transparent
> 2. a rounded rectangle button, bright warm yellow
> 3. the same yellow button in its pressed state: darker, pushed in
> 4. a rounded rectangle button, bright blue
> 5. the same blue button pressed: darker, pushed in
> 6. a rounded rectangle button, flat grey (disabled look)
> 7. a small rounded square button, dark grey
> 8. a wide rounded rectangle card, pale cream
> 9. a rounded square item slot, dark fill, thick black border
> 10. the same item slot but with a bright gold border
> 11. a thin horizontal bar, dark fill (a health bar background)
> 12. a thin horizontal bar, bright green, no border (a health bar fill)
>
> Every shape is a SIMPLE geometric form: rounded corners, ONE flat fill
> colour, and a thick uniform black outline. The face of each shape must
> be completely EMPTY and evenly coloured — text and icons are drawn on
> top by the game. No ornament, no wood grain, no metal studs, no rivets,
> no bevels, no engraving, no decorative end caps of any kind.
>
> [DÁN ĐOẠN BẮT BUỘC Ở MỤC 2 VÀO ĐÂY]
>
> At least 60 pixels of empty transparent space between neighbours, and
> at least 40 pixels of transparent margin around the whole grid. Nothing
> may touch or overlap.
>
> TRUE TRANSPARENT alpha background — no magenta, no white, no
> checkerboard. The alpha must be ANTI-ALIASED, fading over about one
> pixel; do not threshold or posterise it.
>
> No text, no labels, no numbers, no watermark, no drop shadow.

Đặt file: `artsrc/ui/flat_ui.png`

---

## 5. Tấm 2–6 — Năm nền

Mỗi tấm đúng **1125×2436**.

Nền game tham chiếu là **từng dải màu phẳng**: trời một màu + mấy cụm mây
đơn giản, núi một màu, hàng cây hai tông, mặt đất hai tông. Không vân,
không chuyển sắc, không ánh sáng tụ.

Quy tắc vùng trống vẫn giữ nguyên — nó đã cho kết quả tốt (đo ra độ lệch
chuẩn 0.74, đạt ngay lần đầu):

| nền | dải phải phẳng và ít tương phản |
|---|---|
| `bg_prep_portrait` | y = 900…2000 (lưới 4×4 nằm đó) |
| `bg_battle_portrait` | y = 1300…1950 (con đường) |
| `bg_home_portrait` | y = 600…2280 |
| `bg_treasure_portrait` | y = 1060…2436 |
| `bg_upgrade_portrait` | y = 400…2436 |
| `bg_shop_portrait` | y = 400…2436 |

> Prompt (đổi phần CẢNH theo từng nền):
>
> A vertical 2D mobile game background, exactly 1125x2436 pixels.
>
> SCENE: [chọn một]
>   · prep/battle — a sunny grassy battlefield seen from a high angle:
>     flat blue sky with a few simple rounded clouds at the top, a row
>     of flat-toned mountains, a band of simple trees, then a wide open
>     dirt-and-grass field filling the lower two thirds
>   · home — the same world seen from further away: camp tents on the
>     left, a path leading to distant hills
>   · treasure — a stone vault interior, warm torches, flat stone walls
>   · upgrade — a blacksmith forge, glowing coals, flat stone walls
>   · shop — a market stall, striped awning, flat wooden wall
>
> Build the image from BROAD FLAT BANDS of colour. Each band is one flat
> tone with at most one darker tone for shadow. Hard edges between bands.
> Simple shapes only: round clouds, triangular mountains, rounded trees.
>
> The vertical band from y=[A] to y=[B] must be a SINGLE EVEN FLAT
> COLOUR: no detail, no objects, no texture, no gradient, no lighting
> change. UI will cover it and must stay readable. This large plain
> region is intentional and required.
>
> [DÁN ĐOẠN BẮT BUỘC Ở MỤC 2 VÀO ĐÂY]
>
> Fill the entire 1125x2436 canvas edge to edge. No border, no frame, no
> vignette, no text, no UI, no characters, no watermark.

Đặt file: `artsrc/ui/bg_<tên>_v2.png`

---

## 6. Tấm 7 — MỘT nhân vật thử

**Chỉ làm một con. Dừng lại và xem.**

Chọn `dead_common` (Skeleton Warrior) — nó là con hiện ra nhiều nhất, và
là con duy nhất ở bộ hiện tại đang vừa ô mà không phải thu nhỏ nhiều.

### Thứ phải đổi: lính sẽ NHỎ ĐI ba lần

Lính game tham chiếu cao **~2% chiều cao màn** (khoảng 50px trên 2796).
Của ta cao **6–7%** (143–183px trên 2436).

Ở cỡ 50px thì chi tiết không còn quan trọng — chỉ cần **bóng hình rõ và
hai ba mảng màu**. Ở cỡ 180px thì mọi nét "AI" lộ hết.

Nên yêu cầu quan trọng nhất của tấm này là: **đọc được ở 50px**.

> Prompt:
>
> Draw one 2D game character in 8 animation poses, arranged as ONE
> horizontal row of 8 figures on a fully transparent background.
>
> The character: a small cartoon skeleton warrior wearing a simple iron
> helmet, carrying a round wooden shield and a short sword. Friendly and
> cute, not scary.
>
> The 8 poses are a WALK CYCLE, facing RIGHT, seen from the side at a
> slight three-quarter angle.
>
> CRITICAL — this character will be displayed very small, about 50
> pixels tall on a phone screen. Therefore:
> * Design it as a BOLD SILHOUETTE that stays readable when shrunk.
> * Use large simple shapes. No small details, no fine lines, no tiny
>   ornaments — anything thinner than 1/15th of the figure's height will
>   disappear.
> * Give it 3 or 4 colours TOTAL, clearly different from each other.
> * Big head, small body, short limbs — chunky proportions read better
>   when small.
>
> [DÁN ĐOẠN BẮT BUỘC Ở MỤC 2 VÀO ĐÂY]
>
> All eight figures must be the SAME character at the SAME size, with
> the lowest point of every figure at the SAME height (a shared ground
> line). Leave at least 80 pixels of transparent space between figures —
> nothing may touch, including the sword and shield.
>
> Every figure drawn COMPLETE. Nothing cropped at any edge.
>
> TRUE TRANSPARENT alpha background — no magenta, no white. The alpha
> must be ANTI-ALIASED, fading over about one pixel; do not threshold or
> posterise it.
>
> No text, no labels, no watermark, no ground shadow.

Đặt file: `artsrc/units_v2/dead_common_walk.png`

### Sau khi có tấm này

Tôi sẽ:
1. Đo lại **tỉ lệ điểm ảnh là viền** — phải đạt 6–11%, không phải 0.3%
2. Thu nó xuống đúng 50px và nhìn — nếu ở cỡ đó không đọc ra hình gì thì
   prompt còn sai, sửa prompt chứ không làm tiếp 19 con
3. Nhập vào game, đặt cạnh lính cũ để bạn so trực tiếp

Chỉ khi cả ba bước đạt mới làm 19 con còn lại.

---

## 7. Thứ tôi làm bằng code, không cần prompt

- **Số sát thương, thanh máu, chữ** — đã có, chỉ đổi màu
- **Thu nhỏ lính và xếp nhiều hơn trên đường** — đổi `cell_scale` và
  khoảng cách, không đụng art
- **Bỏ 63 khung gỗ khỏi màn Upgrade** — thay sprite, một lần sửa builder

Nếu bạn muốn, bộ UI ở mục 4 tôi cũng **vẽ thẳng bằng Python** được: 12
hình chữ nhật bo góc một màu thì code ra chuẩn từng pixel, bán kính góc
và bề dày viền giống hệt nhau, và đổi màu chỉ sửa một dòng. Công cụ sinh
ảnh sẽ cho ra 12 hình bo góc hơi khác nhau.
