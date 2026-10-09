# Yêu cầu art — 30 icon vật phẩm Treasure

Bốn tấm, làm **từng tấm một**. Một tấm 30 hình thì công cụ sinh ảnh vẽ ẩu,
không hình nào ra hồn — đợt trước đã chứng minh điều đó.

Mỗi hình sẽ được code đặt vào khung ô **180×180px** (xem `item_slots.png`),
phần lọt lòng khung nhỏ nhất đo được là **209/304 ≈ 0.69** của khung, tức
icon hiển thị ở khoảng **124×124px**. Nên vẽ to thoải mái, tool sẽ thu nhỏ
và cân lại — nhưng **đừng vẽ chi tiết nhỏ hơn 1/20 bề ngang hình**, thu
xuống 124px là mất sạch.

---

## Bốn ràng buộc — áp cho cả bốn tấm

1. **Alpha trong suốt thật** — không magenta, không trắng, không kẻ ô.
2. **Alpha khử răng cưa** — viền mờ dần qua ~1 điểm ảnh, đừng ngưỡng hoá.
3. **Không cắt cụt** — mỗi hình trọn vẹn, lề trong suốt ≥60px, các hình
   **không được chạm nhau**, kể cả phần phát sáng.
4. **Không chữ, không nhãn, không số, không watermark, không bóng đổ.**

**Phong cách:** viền đen đậm đều nét, tô khối phẳng có chuyển sáng nhẹ,
game mobile casual. Không pixel art, không tả thực.

**Góc nhìn:** mọi vật nhìn chính diện hoặc chếch nhẹ, **cùng một góc cho
cả 30 món**. Khác góc là xếp vào lưới nhìn loạn ngay.

---


## Tấm Common — 12 món · 3 hàng × 4 cột

| # | tên | chỉ số cấp 1 | mỗi cấp |
|---|---|---|---|
| 1 | **Cốc gỗ sứt** | ATK +3% | +0.3% |
| 2 | **Ly thiếc móp** | Máu +3% | +0.3% |
| 3 | **Gậy gỗ sồi** | ATK +3% | +0.3% |
| 4 | **Áo choàng vải thô** | Giáp +1.5điểm | +0.15điểm |
| 5 | **Ấn đất nung** | Máu +3% | +0.3% |
| 6 | **Bùa cỏ bện** | Tốc đánh +3% | +0.3% |
| 7 | **Huy hiệu đồng** | Chí mạng +1.5điểm | +0.15điểm |
| 8 | **Nhẫn sắt** | ATK +3% | +0.3% |
| 9 | **Chuông đồng nhỏ** | Tốc đánh +3% | +0.3% |
| 10 | **Sách da cũ** | ST chí mạng +3% | +0.3% |
| 11 | **Mũ da** | Giáp +1.5điểm | +0.15điểm |
| 12 | **Dây chuyền xương** | Máu +3% | +0.3% |

> Prompt:
>
> Draw 12 separate 2D game item icons on ONE image, arranged as a
> 3 row x 4 column grid,
> on a fully transparent background.
>
> Read left to right, top to bottom, the items are:
>
> 1. a chipped wooden drinking cup, plain and worn
> 2. a dented tin tankard with a simple handle
> 3. a plain oak walking staff, bark still on it
> 4. a coarse brown burlap cloak with a rope tie
> 5. a small terracotta clay seal stamp
> 6. a charm braided from dry grass and twine
> 7. a plain round bronze badge, slightly tarnished
> 8. a plain iron band ring, no gem
> 9. a small bronze handbell
> 10. a worn leather-bound book, pages frayed
> 11. a simple brown leather cap
> 12. a necklace of small bone beads on a cord
>
> All 12 items share this material language: brown wood, dull iron, rough cloth, bone. Worn and humble. NO gold, NO gems, NO glow.
>
> Every item is drawn COMPLETE, from the same viewing angle (straight
> on or slightly tilted), at roughly the same visual size on screen.
> Leave at least 80 pixels of empty transparent space between
> neighbouring items — nothing may touch or overlap, including any
> glow. Leave at least 60 pixels of transparent margin around the
> whole grid.
>
> Do NOT draw cells, boxes, frames, slots or backgrounds behind the
> items. Do NOT crop any item at an edge.
>
> Style: bold even black outline, flat shading with soft gradients,
> mobile casual game art. Not pixel art, not photorealistic.
>
> TRUE TRANSPARENT alpha background — no magenta, no white, no
> checkerboard. The alpha channel must be ANTI-ALIASED: outlines fade
> over about one pixel. Do not threshold or posterise the transparency.
>
> No text, no labels, no numbers, no watermark, no drop shadow.

Đặt file: `artsrc/ui/items_common.png`

---

## Tấm Rare — 8 món · 2 hàng × 4 cột

| # | tên | chỉ số cấp 1 | mỗi cấp |
|---|---|---|---|
| 1 | **Chén bạc** | Máu +6% | +0.6% |
| 2 | **Trượng khắc rune** | ATK +6% | +0.6% |
| 3 | **Áo choàng lông sói** | Giáp +3điểm | +0.3điểm |
| 4 | **Ấn triện bạc** | ST chí mạng +6% | +0.6% |
| 5 | **Bùa mắt quạ** | Chí mạng +3điểm | +0.3điểm |
| 6 | **Huy hiệu kỵ sĩ** | Tốc đánh +6% | +0.6% |
| 7 | **Đồng hồ cát** | Tốc đánh +6% | +0.6% |
| 8 | **Nhẫn hồng ngọc** | ATK +6% | +0.6% |

> Prompt:
>
> Draw 8 separate 2D game item icons on ONE image, arranged as a
> 2 row x 4 column grid,
> on a fully transparent background.
>
> Read left to right, top to bottom, the items are:
>
> 1. a polished silver goblet with an engraved rim
> 2. a dark wooden staff carved with glowing blue runes
> 3. a grey wolf-fur cloak with a silver clasp
> 4. a silver seal stamp with a scrollwork handle
> 5. an amulet shaped like a raven's eye, black feather attached
> 6. a silver winged knight's badge
> 7. a small hourglass in a silver frame, blue sand inside
> 8. a silver ring set with a single red ruby
>
> All 8 items share this material language: polished silver, dark hardwood, one small coloured gem each. A little shine, NO strong glow.
>
> Every item is drawn COMPLETE, from the same viewing angle (straight
> on or slightly tilted), at roughly the same visual size on screen.
> Leave at least 80 pixels of empty transparent space between
> neighbouring items — nothing may touch or overlap, including any
> glow. Leave at least 60 pixels of transparent margin around the
> whole grid.
>
> Do NOT draw cells, boxes, frames, slots or backgrounds behind the
> items. Do NOT crop any item at an edge.
>
> Style: bold even black outline, flat shading with soft gradients,
> mobile casual game art. Not pixel art, not photorealistic.
>
> TRUE TRANSPARENT alpha background — no magenta, no white, no
> checkerboard. The alpha channel must be ANTI-ALIASED: outlines fade
> over about one pixel. Do not threshold or posterise the transparency.
>
> No text, no labels, no numbers, no watermark, no drop shadow.

Đặt file: `artsrc/ui/items_rare.png`

---

## Tấm Epic — 6 món · 2 hàng × 3 cột

| # | tên | chỉ số cấp 1 | mỗi cấp |
|---|---|---|---|
| 1 | **Chén thánh vàng** | Máu +10%, Hút máu +10điểm | +1%, +1điểm |
| 2 | **Trượng tinh thể** | ATK +10%, ST chí mạng +10% | +1%, +1% |
| 3 | **Áo choàng bóng tối** | Giáp +10điểm, Tốc đánh +10% | +1điểm, +1% |
| 4 | **Ấn triện rồng** | ATK +10%, Chí mạng +10điểm | +1%, +1điểm |
| 5 | **Sách cấm** | ST chí mạng +10%, Chí mạng +10điểm | +1%, +1điểm |
| 6 | **Đèn thần** | Máu +10%, Giáp +10điểm | +1%, +1điểm |

> Prompt:
>
> Draw 6 separate 2D game item icons on ONE image, arranged as a
> 2 row x 3 column grid,
> on a fully transparent background.
>
> Read left to right, top to bottom, the items are:
>
> 1. an ornate golden chalice brimming with glowing red liquid
> 2. a gold staff topped with a large purple crystal, faint glow
> 3. a black cloak dissolving into purple smoke at the hem
> 4. a gold seal stamp with a coiled dragon handle
> 5. a chained grimoire with a purple eye on the cover, glowing
> 6. a golden genie lamp with blue smoke curling from the spout
>
> All 6 items share this material language: gold and purple crystal, visible magical glow around the object.
>
> Every item is drawn COMPLETE, from the same viewing angle (straight
> on or slightly tilted), at roughly the same visual size on screen.
> Leave at least 80 pixels of empty transparent space between
> neighbouring items — nothing may touch or overlap, including any
> glow. Leave at least 60 pixels of transparent margin around the
> whole grid.
>
> Do NOT draw cells, boxes, frames, slots or backgrounds behind the
> items. Do NOT crop any item at an edge.
>
> Style: bold even black outline, flat shading with soft gradients,
> mobile casual game art. Not pixel art, not photorealistic.
>
> TRUE TRANSPARENT alpha background — no magenta, no white, no
> checkerboard. The alpha channel must be ANTI-ALIASED: outlines fade
> over about one pixel. Do not threshold or posterise the transparency.
>
> No text, no labels, no numbers, no watermark, no drop shadow.

Đặt file: `artsrc/ui/items_epic.png`

---

## Tấm Legendary — 4 món · 1 hàng × 4 cột

| # | tên | chỉ số cấp 1 | mỗi cấp |
|---|---|---|---|
| 1 | **Vương miện bất tử** | Máu +16%, Hút máu +14điểm | +1.6%, +1.4điểm |
| 2 | **Trượng hỗn mang** | ATK +16%, ST chí mạng +16% | +1.6%, +1.6% |
| 3 | **Ấn triện thần** | Chí mạng +14điểm, Tốc đánh +16% | +1.4điểm, +1.6% |
| 4 | **Giáp hoàng kim** | Giáp +14điểm, Máu +16% | +1.4điểm, +1.6% |

> Prompt:
>
> Draw 4 separate 2D game item icons on ONE image, arranged as a
> 1 row x 4 column grid,
> on a fully transparent background.
>
> Read left to right, top to bottom, the items are:
>
> 1. a radiant golden crown set with red gems, bright light rays
> 2. a twisted black-and-gold staff with a swirling void orb, radiant
> 3. a divine gold seal stamp with a winged handle, blazing light
> 4. an ornate golden breastplate with a sunburst emblem, radiant
>
> All 4 items share this material language: radiant solid gold and jewels, strong light, visible light rays.
>
> Every item is drawn COMPLETE, from the same viewing angle (straight
> on or slightly tilted), at roughly the same visual size on screen.
> Leave at least 80 pixels of empty transparent space between
> neighbouring items — nothing may touch or overlap, including any
> glow. Leave at least 60 pixels of transparent margin around the
> whole grid.
>
> Do NOT draw cells, boxes, frames, slots or backgrounds behind the
> items. Do NOT crop any item at an edge.
>
> Style: bold even black outline, flat shading with soft gradients,
> mobile casual game art. Not pixel art, not photorealistic.
>
> TRUE TRANSPARENT alpha background — no magenta, no white, no
> checkerboard. The alpha channel must be ANTI-ALIASED: outlines fade
> over about one pixel. Do not threshold or posterise the transparency.
>
> No text, no labels, no numbers, no watermark, no drop shadow.

Đặt file: `artsrc/ui/items_legendary.png`

---


## Vì sao ghi rõ từng món thay vì "12 món kho báu"

Đợt làm pose nhân vật, prompt viết chung chung thì công cụ tự bịa, và bịa
sai. Đợt này mỗi món có một dòng mô tả riêng, đánh số đúng thứ tự đọc —
tool nhập sẽ tách theo thứ tự đó và gán đúng chỉ số. Sai thứ tự là "Cốc gỗ
sứt" mang chỉ số của "Dây chuyền xương", mà nhìn màn hình không ai phát
hiện ra.

## Sau khi có bốn tấm

Tool nhập sẽ:
- tách bằng **gán nhãn liên thông**, rồi sắp theo hàng trước cột sau
- **dừng lại và báo** nếu số cụm tách được khác số món của tấm đó — chứ
  không nhập bừa rồi lệch chỉ số
- cân cả 30 icon về cùng độ nặng bằng `sqrt(diện tích mực)`, như đã làm
  với 4 icon buff (kết quả lệch 0.2%)
- đưa về khung vuông chung vừa lọt lòng `item_slots`
- đo và in số **trước khi ghi**, để bạn duyệt
