# Yêu cầu art — vẽ lại 20 nhân vật (bản 4)

> Sinh từ `tools/make_unit_prompts.py`. **Đừng sửa tay file này** — tên và
> danh sách clip lấy thẳng từ `tools/units_manifest.json`, sửa tay là tài
> liệu lệch với game mà không ai biết.

**Đính kèm `outdata/reference/style_target.png` vào mọi prompt.**

Giữ nguyên: tên nhân vật, danh sách animation, số khung. Đổi: cách vẽ.

---

## Vì sao có lưới 5 × 4

Đo bóng hình 20 nhân vật hiện tại ở cỡ 50px:

```
cung chung toc  : 0.64
khac chung toc  : 0.60      <- chenh 0.04
28/190 cap trung nhau tren 70%
```

Chủng tộc **không tạo ra khác biệt bóng hình nào**. Hai chục con phân biệt
bằng màu và chi tiết — đúng thứ biến mất ở 50px. Chibi hoá mà không sửa
chuyện này thì chúng còn giống nhau hơn, vì chibi là bỏ bớt.

Nên ba trục, độc lập nhau:

| trục | theo | cho ra |
|---|---|---|
| **Dáng thân** | chủng tộc | 5 bóng hình khác hẳn |
| **Dải màu** | chủng tộc | 5 vùng màu không đụng |
| **Phụ kiện nhô ra** | bậc | 4 mức, cộng dồn |

Hai dáng quan trọng nhất là **monster bốn chân** và **machine không chân**
— chúng không thể lẫn với dáng người dù thu nhỏ bao nhiêu.

Bảng màu từng con rút từ chính art cũ rồi **dịch về dải của chủng tộc**:
14/20 con hiện dồn vào dải nâu, giữ nguyên là tái tạo đúng cái đang hỏng.

---

## Thứ tự làm

**Một con trước.** `dead_common`. Đo đạt mới làm tiếp.

| phép đo | đạt khi |
|---|---|
| Tách được đúng số khung | đúng số clip yêu cầu |
| Nét đen chiếm | 35–45% |
| Số màu >2% diện tích | 5–6 |
| Bề dày nét / chiều cao | 3–5% |
| Chân cùng mặt đất | lệch < 1% |
| **Bóng hình khác mọi con đã có** | IoU < 0.70 |

Dòng cuối là dòng mới, và là lý do của cả tài liệu này.

---


## dead_common — Skeleton Warrior

`dead` · `common` · ? · idle×3 walk×3 attack×3 dead×2

**Bốn màu** (ngoài đen): `#6B3FA0` chủ đạo, `#57473F` `#ECDAC0` `#AC9A89`

> Draw the game character **Skeleton Warrior** in 3 poses of a WALK CYCLE, as ONE
> horizontal row of 3 figures, on a fully transparent background.
>
> **BODY SHAPE — this decides whether players can tell it apart**
> a THIN SKELETAL two-legged figure with VISIBLE GAPS between the bones, so daylight shows through the ribcage and the limbs.
>
> **RANK FEATURES**
> Nothing extra. Keep the silhouette plain.
>
> **COLOUR**
> BONE WHITE body and DEEP PURPLE cloth. Use exactly these four colours plus pure black:
> `#6B3FA0` (the dominant one), `#57473F`, `#ECDAC0`, `#AC9A89`.
>
> **STYLE — measured from the attached reference, these are hard numbers**
>
> * Use EXACTLY five colours in the whole drawing: pure black #000000 for
>   every line, plus the four listed below. No sixth colour. No tinting,
>   no blending — each area is one flat colour with a hard edge.
> * Every outline is PURE BLACK #000000. Never grey, never a dark version
>   of the fill, never coloured.
> * The line has ONE CONSTANT THICKNESS everywhere, about 4% of the
>   figure's total height. If a figure is 1000 pixels tall, its outline is
>   about 40 pixels thick. This is VERY thick.
> * Interior lines — ribs, straps, a helmet rim — use the SAME thickness
>   as the outer silhouette. The line never tapers, fades or breaks.
> * When finished, roughly 40% of each figure's area should be black line.
>   If it looks like less than a third, the lines are too thin.
> * CHIBI proportions: head about one third of the total height, short
>   thick limbs, small feet.
> * Face: two solid black eye shapes and one simple mouth line. No nose,
>   no pupils, no teeth, no eyebrows.
>
> **MUST NOT APPEAR**
> No gradients, no airbrush, no soft shading, no blur, no glow, no
> specular highlight, no rim light, no drop shadow. No fabric weave, no
> metal scratches, no wood grain, no surface texture. No ornament,
> rivet, stud, buckle or strap thinner than one fifteenth of the figure's
> height. No background, no ground, no scenery.
>
> **DISPLAY SIZE**
> This character is shown about 50 pixels tall in the game, with twenty
> others around it. Anything that would vanish at that size must be left
> out. The figure must be recognisable from its solid black silhouette
> alone.
>
> **THE SEQUENCE**
> The 3 figures form a smooth loopable walk — the last figure leads
> back into the first. Side view, facing RIGHT, slightly three-quarter. The body
> bobs across the cycle, but the FEET all rest on one shared ground
> line: the lowest point of every figure is at the same height.
>
> **LAYOUT**
> At least 80 pixels of empty transparent space between neighbouring
> figures — nothing may touch, including weapons. At least 60 pixels of
> transparent margin around the whole row. Every figure drawn COMPLETE,
> nothing cropped at any edge.
>
> TRUE TRANSPARENT alpha background — no magenta, no white, no
> checkerboard. The alpha must be ANTI-ALIASED, fading over about one
> pixel; do not threshold or posterise it.
>
> No text, no labels, no numbers, no watermark, no ground shadow.

Rồi lặp lại cho `idle`, `attack`, `dead` — đổi
đúng một dòng **THE SEQUENCE**, giữ nguyên mọi ràng buộc khác.

Đặt file: `artsrc/units_v4/dead_common_<clip>.png`

---

## dead_rare — Spirit Wisp

`dead` · `rare` · ranged · idle×3 walk×3 attack×3 dead×2

**Bốn màu** (ngoài đen): `#6B3FA0` chủ đạo, `#381E87` `#6B52D0` `#56CAF8`

> Draw the game character **Spirit Wisp** in 3 poses of a WALK CYCLE, as ONE
> horizontal row of 3 figures, on a fully transparent background.
>
> **BODY SHAPE — this decides whether players can tell it apart**
> a THIN SKELETAL two-legged figure with VISIBLE GAPS between the bones, so daylight shows through the ribcage and the limbs.
>
> **RANK FEATURES**
> Add ONE large object held in the hands, big enough to change the silhouette — an oversized weapon, tool or instrument.
>
> **COLOUR**
> BONE WHITE body and DEEP PURPLE cloth. Use exactly these four colours plus pure black:
> `#6B3FA0` (the dominant one), `#381E87`, `#6B52D0`, `#56CAF8`.
>
> **STYLE — measured from the attached reference, these are hard numbers**
>
> * Use EXACTLY five colours in the whole drawing: pure black #000000 for
>   every line, plus the four listed below. No sixth colour. No tinting,
>   no blending — each area is one flat colour with a hard edge.
> * Every outline is PURE BLACK #000000. Never grey, never a dark version
>   of the fill, never coloured.
> * The line has ONE CONSTANT THICKNESS everywhere, about 4% of the
>   figure's total height. If a figure is 1000 pixels tall, its outline is
>   about 40 pixels thick. This is VERY thick.
> * Interior lines — ribs, straps, a helmet rim — use the SAME thickness
>   as the outer silhouette. The line never tapers, fades or breaks.
> * When finished, roughly 40% of each figure's area should be black line.
>   If it looks like less than a third, the lines are too thin.
> * CHIBI proportions: head about one third of the total height, short
>   thick limbs, small feet.
> * Face: two solid black eye shapes and one simple mouth line. No nose,
>   no pupils, no teeth, no eyebrows.
>
> **MUST NOT APPEAR**
> No gradients, no airbrush, no soft shading, no blur, no glow, no
> specular highlight, no rim light, no drop shadow. No fabric weave, no
> metal scratches, no wood grain, no surface texture. No ornament,
> rivet, stud, buckle or strap thinner than one fifteenth of the figure's
> height. No background, no ground, no scenery.
>
> **DISPLAY SIZE**
> This character is shown about 50 pixels tall in the game, with twenty
> others around it. Anything that would vanish at that size must be left
> out. The figure must be recognisable from its solid black silhouette
> alone.
>
> **THE SEQUENCE**
> The 3 figures form a smooth loopable walk — the last figure leads
> back into the first. Side view, facing RIGHT, slightly three-quarter. The body
> bobs across the cycle, but the FEET all rest on one shared ground
> line: the lowest point of every figure is at the same height.
>
> **LAYOUT**
> At least 80 pixels of empty transparent space between neighbouring
> figures — nothing may touch, including weapons. At least 60 pixels of
> transparent margin around the whole row. Every figure drawn COMPLETE,
> nothing cropped at any edge.
>
> TRUE TRANSPARENT alpha background — no magenta, no white, no
> checkerboard. The alpha must be ANTI-ALIASED, fading over about one
> pixel; do not threshold or posterise it.
>
> No text, no labels, no numbers, no watermark, no ground shadow.

Rồi lặp lại cho `idle`, `attack`, `dead` — đổi
đúng một dòng **THE SEQUENCE**, giữ nguyên mọi ràng buộc khác.

Đặt file: `artsrc/units_v4/dead_rare_<clip>.png`

---

## dead_epic — Haunted Tank

`dead` · `epic` · ranged · idle×3 walk×3 attack×3 dead×2

**Bốn màu** (ngoài đen): `#6B3FA0` chủ đạo, `#3C3D4B` `#C7CEB9` `#5E5158`

> Draw the game character **Haunted Tank** in 3 poses of a WALK CYCLE, as ONE
> horizontal row of 3 figures, on a fully transparent background.
>
> **BODY SHAPE — this decides whether players can tell it apart**
> a THIN SKELETAL two-legged figure with VISIBLE GAPS between the bones, so daylight shows through the ribcage and the limbs.
>
> **RANK FEATURES**
> Keep the rare hand object, AND add something standing UP off the head: a horn, a tall pointed hat, an antenna, a crest.
>
> **COLOUR**
> BONE WHITE body and DEEP PURPLE cloth. Use exactly these four colours plus pure black:
> `#6B3FA0` (the dominant one), `#3C3D4B`, `#C7CEB9`, `#5E5158`.
>
> **STYLE — measured from the attached reference, these are hard numbers**
>
> * Use EXACTLY five colours in the whole drawing: pure black #000000 for
>   every line, plus the four listed below. No sixth colour. No tinting,
>   no blending — each area is one flat colour with a hard edge.
> * Every outline is PURE BLACK #000000. Never grey, never a dark version
>   of the fill, never coloured.
> * The line has ONE CONSTANT THICKNESS everywhere, about 4% of the
>   figure's total height. If a figure is 1000 pixels tall, its outline is
>   about 40 pixels thick. This is VERY thick.
> * Interior lines — ribs, straps, a helmet rim — use the SAME thickness
>   as the outer silhouette. The line never tapers, fades or breaks.
> * When finished, roughly 40% of each figure's area should be black line.
>   If it looks like less than a third, the lines are too thin.
> * CHIBI proportions: head about one third of the total height, short
>   thick limbs, small feet.
> * Face: two solid black eye shapes and one simple mouth line. No nose,
>   no pupils, no teeth, no eyebrows.
>
> **MUST NOT APPEAR**
> No gradients, no airbrush, no soft shading, no blur, no glow, no
> specular highlight, no rim light, no drop shadow. No fabric weave, no
> metal scratches, no wood grain, no surface texture. No ornament,
> rivet, stud, buckle or strap thinner than one fifteenth of the figure's
> height. No background, no ground, no scenery.
>
> **DISPLAY SIZE**
> This character is shown about 50 pixels tall in the game, with twenty
> others around it. Anything that would vanish at that size must be left
> out. The figure must be recognisable from its solid black silhouette
> alone.
>
> **THE SEQUENCE**
> The 3 figures form a smooth loopable walk — the last figure leads
> back into the first. Side view, facing RIGHT, slightly three-quarter. The body
> bobs across the cycle, but the FEET all rest on one shared ground
> line: the lowest point of every figure is at the same height.
>
> **LAYOUT**
> At least 80 pixels of empty transparent space between neighbouring
> figures — nothing may touch, including weapons. At least 60 pixels of
> transparent margin around the whole row. Every figure drawn COMPLETE,
> nothing cropped at any edge.
>
> TRUE TRANSPARENT alpha background — no magenta, no white, no
> checkerboard. The alpha must be ANTI-ALIASED, fading over about one
> pixel; do not threshold or posterise it.
>
> No text, no labels, no numbers, no watermark, no ground shadow.

Rồi lặp lại cho `idle`, `attack`, `dead` — đổi
đúng một dòng **THE SEQUENCE**, giữ nguyên mọi ràng buộc khác.

Đặt file: `artsrc/units_v4/dead_epic_<clip>.png`

---

## dead_legendary — Skeleton King

`dead` · `legendary` · melee · idle×3 walk×3 attack×3 dead×2 skill×4

**Bốn màu** (ngoài đen): `#6B3FA0` chủ đạo, `#FBE3B6` `#BD803D` `#6D3A3A`

> Draw the game character **Skeleton King** in 3 poses of a WALK CYCLE, as ONE
> horizontal row of 3 figures, on a fully transparent background.
>
> **BODY SHAPE — this decides whether players can tell it apart**
> a THIN SKELETAL two-legged figure with VISIBLE GAPS between the bones, so daylight shows through the ribcage and the limbs.
>
> **RANK FEATURES**
> Keep the rare hand object and the epic head piece, AND add a mass BEHIND the body: a cape, wings, a chimney, a banner.
>
> **COLOUR**
> BONE WHITE body and DEEP PURPLE cloth. Use exactly these four colours plus pure black:
> `#6B3FA0` (the dominant one), `#FBE3B6`, `#BD803D`, `#6D3A3A`.
>
> **STYLE — measured from the attached reference, these are hard numbers**
>
> * Use EXACTLY five colours in the whole drawing: pure black #000000 for
>   every line, plus the four listed below. No sixth colour. No tinting,
>   no blending — each area is one flat colour with a hard edge.
> * Every outline is PURE BLACK #000000. Never grey, never a dark version
>   of the fill, never coloured.
> * The line has ONE CONSTANT THICKNESS everywhere, about 4% of the
>   figure's total height. If a figure is 1000 pixels tall, its outline is
>   about 40 pixels thick. This is VERY thick.
> * Interior lines — ribs, straps, a helmet rim — use the SAME thickness
>   as the outer silhouette. The line never tapers, fades or breaks.
> * When finished, roughly 40% of each figure's area should be black line.
>   If it looks like less than a third, the lines are too thin.
> * CHIBI proportions: head about one third of the total height, short
>   thick limbs, small feet.
> * Face: two solid black eye shapes and one simple mouth line. No nose,
>   no pupils, no teeth, no eyebrows.
>
> **MUST NOT APPEAR**
> No gradients, no airbrush, no soft shading, no blur, no glow, no
> specular highlight, no rim light, no drop shadow. No fabric weave, no
> metal scratches, no wood grain, no surface texture. No ornament,
> rivet, stud, buckle or strap thinner than one fifteenth of the figure's
> height. No background, no ground, no scenery.
>
> **DISPLAY SIZE**
> This character is shown about 50 pixels tall in the game, with twenty
> others around it. Anything that would vanish at that size must be left
> out. The figure must be recognisable from its solid black silhouette
> alone.
>
> **THE SEQUENCE**
> The 3 figures form a smooth loopable walk — the last figure leads
> back into the first. Side view, facing RIGHT, slightly three-quarter. The body
> bobs across the cycle, but the FEET all rest on one shared ground
> line: the lowest point of every figure is at the same height.
>
> **LAYOUT**
> At least 80 pixels of empty transparent space between neighbouring
> figures — nothing may touch, including weapons. At least 60 pixels of
> transparent margin around the whole row. Every figure drawn COMPLETE,
> nothing cropped at any edge.
>
> TRUE TRANSPARENT alpha background — no magenta, no white, no
> checkerboard. The alpha must be ANTI-ALIASED, fading over about one
> pixel; do not threshold or posterise it.
>
> No text, no labels, no numbers, no watermark, no ground shadow.

Rồi lặp lại cho `idle`, `attack`, `dead`, `skill` — đổi
đúng một dòng **THE SEQUENCE**, giữ nguyên mọi ràng buộc khác.

Đặt file: `artsrc/units_v4/dead_legendary_<clip>.png`

---

## elf_common — Thorn Scout

`elf` · `common` · ranged · idle×3 walk×3 attack×3 dead×2

**Bốn màu** (ngoài đen): `#3E8E41` chủ đạo, `#684F2E` `#F1CD96` `#FCE2B4`

> Draw the game character **Thorn Scout** in 3 poses of a WALK CYCLE, as ONE
> horizontal row of 3 figures, on a fully transparent background.
>
> **BODY SHAPE — this decides whether players can tell it apart**
> a TALL, SLENDER two-legged humanoid with LONG POINTED EARS and long hair that both stick out clearly from the silhouette.
>
> **RANK FEATURES**
> Nothing extra. Keep the silhouette plain.
>
> **COLOUR**
> pale skin and LEAF GREEN cloth. Use exactly these four colours plus pure black:
> `#3E8E41` (the dominant one), `#684F2E`, `#F1CD96`, `#FCE2B4`.
>
> **STYLE — measured from the attached reference, these are hard numbers**
>
> * Use EXACTLY five colours in the whole drawing: pure black #000000 for
>   every line, plus the four listed below. No sixth colour. No tinting,
>   no blending — each area is one flat colour with a hard edge.
> * Every outline is PURE BLACK #000000. Never grey, never a dark version
>   of the fill, never coloured.
> * The line has ONE CONSTANT THICKNESS everywhere, about 4% of the
>   figure's total height. If a figure is 1000 pixels tall, its outline is
>   about 40 pixels thick. This is VERY thick.
> * Interior lines — ribs, straps, a helmet rim — use the SAME thickness
>   as the outer silhouette. The line never tapers, fades or breaks.
> * When finished, roughly 40% of each figure's area should be black line.
>   If it looks like less than a third, the lines are too thin.
> * CHIBI proportions: head about one third of the total height, short
>   thick limbs, small feet.
> * Face: two solid black eye shapes and one simple mouth line. No nose,
>   no pupils, no teeth, no eyebrows.
>
> **MUST NOT APPEAR**
> No gradients, no airbrush, no soft shading, no blur, no glow, no
> specular highlight, no rim light, no drop shadow. No fabric weave, no
> metal scratches, no wood grain, no surface texture. No ornament,
> rivet, stud, buckle or strap thinner than one fifteenth of the figure's
> height. No background, no ground, no scenery.
>
> **DISPLAY SIZE**
> This character is shown about 50 pixels tall in the game, with twenty
> others around it. Anything that would vanish at that size must be left
> out. The figure must be recognisable from its solid black silhouette
> alone.
>
> **THE SEQUENCE**
> The 3 figures form a smooth loopable walk — the last figure leads
> back into the first. Side view, facing RIGHT, slightly three-quarter. The body
> bobs across the cycle, but the FEET all rest on one shared ground
> line: the lowest point of every figure is at the same height.
>
> **LAYOUT**
> At least 80 pixels of empty transparent space between neighbouring
> figures — nothing may touch, including weapons. At least 60 pixels of
> transparent margin around the whole row. Every figure drawn COMPLETE,
> nothing cropped at any edge.
>
> TRUE TRANSPARENT alpha background — no magenta, no white, no
> checkerboard. The alpha must be ANTI-ALIASED, fading over about one
> pixel; do not threshold or posterise it.
>
> No text, no labels, no numbers, no watermark, no ground shadow.

Rồi lặp lại cho `idle`, `attack`, `dead` — đổi
đúng một dòng **THE SEQUENCE**, giữ nguyên mọi ràng buộc khác.

Đặt file: `artsrc/units_v4/elf_common_<clip>.png`

---

## elf_rare — Leaf Blade

`elf` · `rare` · melee · idle×3 walk×3 attack×3 dead×2

**Bốn màu** (ngoài đen): `#3E8E41` chủ đạo, `#DEC2AF` `#EEDED1` `#AA9286`

> Draw the game character **Leaf Blade** in 3 poses of a WALK CYCLE, as ONE
> horizontal row of 3 figures, on a fully transparent background.
>
> **BODY SHAPE — this decides whether players can tell it apart**
> a TALL, SLENDER two-legged humanoid with LONG POINTED EARS and long hair that both stick out clearly from the silhouette.
>
> **RANK FEATURES**
> Add ONE large object held in the hands, big enough to change the silhouette — an oversized weapon, tool or instrument.
>
> **COLOUR**
> pale skin and LEAF GREEN cloth. Use exactly these four colours plus pure black:
> `#3E8E41` (the dominant one), `#DEC2AF`, `#EEDED1`, `#AA9286`.
>
> **STYLE — measured from the attached reference, these are hard numbers**
>
> * Use EXACTLY five colours in the whole drawing: pure black #000000 for
>   every line, plus the four listed below. No sixth colour. No tinting,
>   no blending — each area is one flat colour with a hard edge.
> * Every outline is PURE BLACK #000000. Never grey, never a dark version
>   of the fill, never coloured.
> * The line has ONE CONSTANT THICKNESS everywhere, about 4% of the
>   figure's total height. If a figure is 1000 pixels tall, its outline is
>   about 40 pixels thick. This is VERY thick.
> * Interior lines — ribs, straps, a helmet rim — use the SAME thickness
>   as the outer silhouette. The line never tapers, fades or breaks.
> * When finished, roughly 40% of each figure's area should be black line.
>   If it looks like less than a third, the lines are too thin.
> * CHIBI proportions: head about one third of the total height, short
>   thick limbs, small feet.
> * Face: two solid black eye shapes and one simple mouth line. No nose,
>   no pupils, no teeth, no eyebrows.
>
> **MUST NOT APPEAR**
> No gradients, no airbrush, no soft shading, no blur, no glow, no
> specular highlight, no rim light, no drop shadow. No fabric weave, no
> metal scratches, no wood grain, no surface texture. No ornament,
> rivet, stud, buckle or strap thinner than one fifteenth of the figure's
> height. No background, no ground, no scenery.
>
> **DISPLAY SIZE**
> This character is shown about 50 pixels tall in the game, with twenty
> others around it. Anything that would vanish at that size must be left
> out. The figure must be recognisable from its solid black silhouette
> alone.
>
> **THE SEQUENCE**
> The 3 figures form a smooth loopable walk — the last figure leads
> back into the first. Side view, facing RIGHT, slightly three-quarter. The body
> bobs across the cycle, but the FEET all rest on one shared ground
> line: the lowest point of every figure is at the same height.
>
> **LAYOUT**
> At least 80 pixels of empty transparent space between neighbouring
> figures — nothing may touch, including weapons. At least 60 pixels of
> transparent margin around the whole row. Every figure drawn COMPLETE,
> nothing cropped at any edge.
>
> TRUE TRANSPARENT alpha background — no magenta, no white, no
> checkerboard. The alpha must be ANTI-ALIASED, fading over about one
> pixel; do not threshold or posterise it.
>
> No text, no labels, no numbers, no watermark, no ground shadow.

Rồi lặp lại cho `idle`, `attack`, `dead` — đổi
đúng một dòng **THE SEQUENCE**, giữ nguyên mọi ràng buộc khác.

Đặt file: `artsrc/units_v4/elf_rare_<clip>.png`

---

## elf_epic — Moon Archer

`elf` · `epic` · ranged · idle×3 walk×3 attack×3 dead×2

**Bốn màu** (ngoài đen): `#3E8E41` chủ đạo, `#F9D6C4` `#444C64` `#938592`

> Draw the game character **Moon Archer** in 3 poses of a WALK CYCLE, as ONE
> horizontal row of 3 figures, on a fully transparent background.
>
> **BODY SHAPE — this decides whether players can tell it apart**
> a TALL, SLENDER two-legged humanoid with LONG POINTED EARS and long hair that both stick out clearly from the silhouette.
>
> **RANK FEATURES**
> Keep the rare hand object, AND add something standing UP off the head: a horn, a tall pointed hat, an antenna, a crest.
>
> **COLOUR**
> pale skin and LEAF GREEN cloth. Use exactly these four colours plus pure black:
> `#3E8E41` (the dominant one), `#F9D6C4`, `#444C64`, `#938592`.
>
> **STYLE — measured from the attached reference, these are hard numbers**
>
> * Use EXACTLY five colours in the whole drawing: pure black #000000 for
>   every line, plus the four listed below. No sixth colour. No tinting,
>   no blending — each area is one flat colour with a hard edge.
> * Every outline is PURE BLACK #000000. Never grey, never a dark version
>   of the fill, never coloured.
> * The line has ONE CONSTANT THICKNESS everywhere, about 4% of the
>   figure's total height. If a figure is 1000 pixels tall, its outline is
>   about 40 pixels thick. This is VERY thick.
> * Interior lines — ribs, straps, a helmet rim — use the SAME thickness
>   as the outer silhouette. The line never tapers, fades or breaks.
> * When finished, roughly 40% of each figure's area should be black line.
>   If it looks like less than a third, the lines are too thin.
> * CHIBI proportions: head about one third of the total height, short
>   thick limbs, small feet.
> * Face: two solid black eye shapes and one simple mouth line. No nose,
>   no pupils, no teeth, no eyebrows.
>
> **MUST NOT APPEAR**
> No gradients, no airbrush, no soft shading, no blur, no glow, no
> specular highlight, no rim light, no drop shadow. No fabric weave, no
> metal scratches, no wood grain, no surface texture. No ornament,
> rivet, stud, buckle or strap thinner than one fifteenth of the figure's
> height. No background, no ground, no scenery.
>
> **DISPLAY SIZE**
> This character is shown about 50 pixels tall in the game, with twenty
> others around it. Anything that would vanish at that size must be left
> out. The figure must be recognisable from its solid black silhouette
> alone.
>
> **THE SEQUENCE**
> The 3 figures form a smooth loopable walk — the last figure leads
> back into the first. Side view, facing RIGHT, slightly three-quarter. The body
> bobs across the cycle, but the FEET all rest on one shared ground
> line: the lowest point of every figure is at the same height.
>
> **LAYOUT**
> At least 80 pixels of empty transparent space between neighbouring
> figures — nothing may touch, including weapons. At least 60 pixels of
> transparent margin around the whole row. Every figure drawn COMPLETE,
> nothing cropped at any edge.
>
> TRUE TRANSPARENT alpha background — no magenta, no white, no
> checkerboard. The alpha must be ANTI-ALIASED, fading over about one
> pixel; do not threshold or posterise it.
>
> No text, no labels, no numbers, no watermark, no ground shadow.

Rồi lặp lại cho `idle`, `attack`, `dead` — đổi
đúng một dòng **THE SEQUENCE**, giữ nguyên mọi ràng buộc khác.

Đặt file: `artsrc/units_v4/elf_epic_<clip>.png`

---

## elf_legendary — Verdant Archer

`elf` · `legendary` · ranged · idle×3 walk×3 attack×3 dead×2 skill×4

**Bốn màu** (ngoài đen): `#3E8E41` chủ đạo, `#CC7A3A` `#FDCC90` `#775227`

> Draw the game character **Verdant Archer** in 3 poses of a WALK CYCLE, as ONE
> horizontal row of 3 figures, on a fully transparent background.
>
> **BODY SHAPE — this decides whether players can tell it apart**
> a TALL, SLENDER two-legged humanoid with LONG POINTED EARS and long hair that both stick out clearly from the silhouette.
>
> **RANK FEATURES**
> Keep the rare hand object and the epic head piece, AND add a mass BEHIND the body: a cape, wings, a chimney, a banner.
>
> **COLOUR**
> pale skin and LEAF GREEN cloth. Use exactly these four colours plus pure black:
> `#3E8E41` (the dominant one), `#CC7A3A`, `#FDCC90`, `#775227`.
>
> **STYLE — measured from the attached reference, these are hard numbers**
>
> * Use EXACTLY five colours in the whole drawing: pure black #000000 for
>   every line, plus the four listed below. No sixth colour. No tinting,
>   no blending — each area is one flat colour with a hard edge.
> * Every outline is PURE BLACK #000000. Never grey, never a dark version
>   of the fill, never coloured.
> * The line has ONE CONSTANT THICKNESS everywhere, about 4% of the
>   figure's total height. If a figure is 1000 pixels tall, its outline is
>   about 40 pixels thick. This is VERY thick.
> * Interior lines — ribs, straps, a helmet rim — use the SAME thickness
>   as the outer silhouette. The line never tapers, fades or breaks.
> * When finished, roughly 40% of each figure's area should be black line.
>   If it looks like less than a third, the lines are too thin.
> * CHIBI proportions: head about one third of the total height, short
>   thick limbs, small feet.
> * Face: two solid black eye shapes and one simple mouth line. No nose,
>   no pupils, no teeth, no eyebrows.
>
> **MUST NOT APPEAR**
> No gradients, no airbrush, no soft shading, no blur, no glow, no
> specular highlight, no rim light, no drop shadow. No fabric weave, no
> metal scratches, no wood grain, no surface texture. No ornament,
> rivet, stud, buckle or strap thinner than one fifteenth of the figure's
> height. No background, no ground, no scenery.
>
> **DISPLAY SIZE**
> This character is shown about 50 pixels tall in the game, with twenty
> others around it. Anything that would vanish at that size must be left
> out. The figure must be recognisable from its solid black silhouette
> alone.
>
> **THE SEQUENCE**
> The 3 figures form a smooth loopable walk — the last figure leads
> back into the first. Side view, facing RIGHT, slightly three-quarter. The body
> bobs across the cycle, but the FEET all rest on one shared ground
> line: the lowest point of every figure is at the same height.
>
> **LAYOUT**
> At least 80 pixels of empty transparent space between neighbouring
> figures — nothing may touch, including weapons. At least 60 pixels of
> transparent margin around the whole row. Every figure drawn COMPLETE,
> nothing cropped at any edge.
>
> TRUE TRANSPARENT alpha background — no magenta, no white, no
> checkerboard. The alpha must be ANTI-ALIASED, fading over about one
> pixel; do not threshold or posterise it.
>
> No text, no labels, no numbers, no watermark, no ground shadow.

Rồi lặp lại cho `idle`, `attack`, `dead`, `skill` — đổi
đúng một dòng **THE SEQUENCE**, giữ nguyên mọi ràng buộc khác.

Đặt file: `artsrc/units_v4/elf_legendary_<clip>.png`

---

## human_common — Dân quân cầm liềm

`human` · `common` · melee · idle×3 walk×3 attack×3 dead×2

**Bốn màu** (ngoài đen): `#2E5BA8` chủ đạo, `#F4CAAC` `#91614D` `#C29A85`

> Draw the game character **Dân quân cầm liềm** in 3 poses of a WALK CYCLE, as ONE
> horizontal row of 3 figures, on a fully transparent background.
>
> **BODY SHAPE — this decides whether players can tell it apart**
> an ordinary two-legged humanoid with square shoulders and a broad chest.
>
> **RANK FEATURES**
> Nothing extra. Keep the silhouette plain.
>
> **COLOUR**
> warm tan skin and ROYAL BLUE cloth. Use exactly these four colours plus pure black:
> `#2E5BA8` (the dominant one), `#F4CAAC`, `#91614D`, `#C29A85`.
>
> **STYLE — measured from the attached reference, these are hard numbers**
>
> * Use EXACTLY five colours in the whole drawing: pure black #000000 for
>   every line, plus the four listed below. No sixth colour. No tinting,
>   no blending — each area is one flat colour with a hard edge.
> * Every outline is PURE BLACK #000000. Never grey, never a dark version
>   of the fill, never coloured.
> * The line has ONE CONSTANT THICKNESS everywhere, about 4% of the
>   figure's total height. If a figure is 1000 pixels tall, its outline is
>   about 40 pixels thick. This is VERY thick.
> * Interior lines — ribs, straps, a helmet rim — use the SAME thickness
>   as the outer silhouette. The line never tapers, fades or breaks.
> * When finished, roughly 40% of each figure's area should be black line.
>   If it looks like less than a third, the lines are too thin.
> * CHIBI proportions: head about one third of the total height, short
>   thick limbs, small feet.
> * Face: two solid black eye shapes and one simple mouth line. No nose,
>   no pupils, no teeth, no eyebrows.
>
> **MUST NOT APPEAR**
> No gradients, no airbrush, no soft shading, no blur, no glow, no
> specular highlight, no rim light, no drop shadow. No fabric weave, no
> metal scratches, no wood grain, no surface texture. No ornament,
> rivet, stud, buckle or strap thinner than one fifteenth of the figure's
> height. No background, no ground, no scenery.
>
> **DISPLAY SIZE**
> This character is shown about 50 pixels tall in the game, with twenty
> others around it. Anything that would vanish at that size must be left
> out. The figure must be recognisable from its solid black silhouette
> alone.
>
> **THE SEQUENCE**
> The 3 figures form a smooth loopable walk — the last figure leads
> back into the first. Side view, facing RIGHT, slightly three-quarter. The body
> bobs across the cycle, but the FEET all rest on one shared ground
> line: the lowest point of every figure is at the same height.
>
> **LAYOUT**
> At least 80 pixels of empty transparent space between neighbouring
> figures — nothing may touch, including weapons. At least 60 pixels of
> transparent margin around the whole row. Every figure drawn COMPLETE,
> nothing cropped at any edge.
>
> TRUE TRANSPARENT alpha background — no magenta, no white, no
> checkerboard. The alpha must be ANTI-ALIASED, fading over about one
> pixel; do not threshold or posterise it.
>
> No text, no labels, no numbers, no watermark, no ground shadow.

Rồi lặp lại cho `idle`, `attack`, `dead` — đổi
đúng một dòng **THE SEQUENCE**, giữ nguyên mọi ràng buộc khác.

Đặt file: `artsrc/units_v4/human_common_<clip>.png`

---

## human_rare — Lion Spear Knight

`human` · `rare` · melee · idle×3 walk×3 attack×3 dead×2

**Bốn màu** (ngoài đen): `#2E5BA8` chủ đạo, `#8A4C31` `#EFD3B2` `#AF907C`

> Draw the game character **Lion Spear Knight** in 3 poses of a WALK CYCLE, as ONE
> horizontal row of 3 figures, on a fully transparent background.
>
> **BODY SHAPE — this decides whether players can tell it apart**
> an ordinary two-legged humanoid with square shoulders and a broad chest.
>
> **RANK FEATURES**
> Add ONE large object held in the hands, big enough to change the silhouette — an oversized weapon, tool or instrument.
>
> **COLOUR**
> warm tan skin and ROYAL BLUE cloth. Use exactly these four colours plus pure black:
> `#2E5BA8` (the dominant one), `#8A4C31`, `#EFD3B2`, `#AF907C`.
>
> **STYLE — measured from the attached reference, these are hard numbers**
>
> * Use EXACTLY five colours in the whole drawing: pure black #000000 for
>   every line, plus the four listed below. No sixth colour. No tinting,
>   no blending — each area is one flat colour with a hard edge.
> * Every outline is PURE BLACK #000000. Never grey, never a dark version
>   of the fill, never coloured.
> * The line has ONE CONSTANT THICKNESS everywhere, about 4% of the
>   figure's total height. If a figure is 1000 pixels tall, its outline is
>   about 40 pixels thick. This is VERY thick.
> * Interior lines — ribs, straps, a helmet rim — use the SAME thickness
>   as the outer silhouette. The line never tapers, fades or breaks.
> * When finished, roughly 40% of each figure's area should be black line.
>   If it looks like less than a third, the lines are too thin.
> * CHIBI proportions: head about one third of the total height, short
>   thick limbs, small feet.
> * Face: two solid black eye shapes and one simple mouth line. No nose,
>   no pupils, no teeth, no eyebrows.
>
> **MUST NOT APPEAR**
> No gradients, no airbrush, no soft shading, no blur, no glow, no
> specular highlight, no rim light, no drop shadow. No fabric weave, no
> metal scratches, no wood grain, no surface texture. No ornament,
> rivet, stud, buckle or strap thinner than one fifteenth of the figure's
> height. No background, no ground, no scenery.
>
> **DISPLAY SIZE**
> This character is shown about 50 pixels tall in the game, with twenty
> others around it. Anything that would vanish at that size must be left
> out. The figure must be recognisable from its solid black silhouette
> alone.
>
> **THE SEQUENCE**
> The 3 figures form a smooth loopable walk — the last figure leads
> back into the first. Side view, facing RIGHT, slightly three-quarter. The body
> bobs across the cycle, but the FEET all rest on one shared ground
> line: the lowest point of every figure is at the same height.
>
> **LAYOUT**
> At least 80 pixels of empty transparent space between neighbouring
> figures — nothing may touch, including weapons. At least 60 pixels of
> transparent margin around the whole row. Every figure drawn COMPLETE,
> nothing cropped at any edge.
>
> TRUE TRANSPARENT alpha background — no magenta, no white, no
> checkerboard. The alpha must be ANTI-ALIASED, fading over about one
> pixel; do not threshold or posterise it.
>
> No text, no labels, no numbers, no watermark, no ground shadow.

Rồi lặp lại cho `idle`, `attack`, `dead` — đổi
đúng một dòng **THE SEQUENCE**, giữ nguyên mọi ràng buộc khác.

Đặt file: `artsrc/units_v4/human_rare_<clip>.png`

---

## human_epic — Crescent Mage

`human` · `epic` · ranged · idle×3 walk×3 attack×3 dead×2

**Bốn màu** (ngoài đen): `#2E5BA8` chủ đạo, `#A2372E` `#5F2867` `#E6CA86`

> Draw the game character **Crescent Mage** in 3 poses of a WALK CYCLE, as ONE
> horizontal row of 3 figures, on a fully transparent background.
>
> **BODY SHAPE — this decides whether players can tell it apart**
> an ordinary two-legged humanoid with square shoulders and a broad chest.
>
> **RANK FEATURES**
> Keep the rare hand object, AND add something standing UP off the head: a horn, a tall pointed hat, an antenna, a crest.
>
> **COLOUR**
> warm tan skin and ROYAL BLUE cloth. Use exactly these four colours plus pure black:
> `#2E5BA8` (the dominant one), `#A2372E`, `#5F2867`, `#E6CA86`.
>
> **STYLE — measured from the attached reference, these are hard numbers**
>
> * Use EXACTLY five colours in the whole drawing: pure black #000000 for
>   every line, plus the four listed below. No sixth colour. No tinting,
>   no blending — each area is one flat colour with a hard edge.
> * Every outline is PURE BLACK #000000. Never grey, never a dark version
>   of the fill, never coloured.
> * The line has ONE CONSTANT THICKNESS everywhere, about 4% of the
>   figure's total height. If a figure is 1000 pixels tall, its outline is
>   about 40 pixels thick. This is VERY thick.
> * Interior lines — ribs, straps, a helmet rim — use the SAME thickness
>   as the outer silhouette. The line never tapers, fades or breaks.
> * When finished, roughly 40% of each figure's area should be black line.
>   If it looks like less than a third, the lines are too thin.
> * CHIBI proportions: head about one third of the total height, short
>   thick limbs, small feet.
> * Face: two solid black eye shapes and one simple mouth line. No nose,
>   no pupils, no teeth, no eyebrows.
>
> **MUST NOT APPEAR**
> No gradients, no airbrush, no soft shading, no blur, no glow, no
> specular highlight, no rim light, no drop shadow. No fabric weave, no
> metal scratches, no wood grain, no surface texture. No ornament,
> rivet, stud, buckle or strap thinner than one fifteenth of the figure's
> height. No background, no ground, no scenery.
>
> **DISPLAY SIZE**
> This character is shown about 50 pixels tall in the game, with twenty
> others around it. Anything that would vanish at that size must be left
> out. The figure must be recognisable from its solid black silhouette
> alone.
>
> **THE SEQUENCE**
> The 3 figures form a smooth loopable walk — the last figure leads
> back into the first. Side view, facing RIGHT, slightly three-quarter. The body
> bobs across the cycle, but the FEET all rest on one shared ground
> line: the lowest point of every figure is at the same height.
>
> **LAYOUT**
> At least 80 pixels of empty transparent space between neighbouring
> figures — nothing may touch, including weapons. At least 60 pixels of
> transparent margin around the whole row. Every figure drawn COMPLETE,
> nothing cropped at any edge.
>
> TRUE TRANSPARENT alpha background — no magenta, no white, no
> checkerboard. The alpha must be ANTI-ALIASED, fading over about one
> pixel; do not threshold or posterise it.
>
> No text, no labels, no numbers, no watermark, no ground shadow.

Rồi lặp lại cho `idle`, `attack`, `dead` — đổi
đúng một dòng **THE SEQUENCE**, giữ nguyên mọi ràng buộc khác.

Đặt file: `artsrc/units_v4/human_epic_<clip>.png`

---

## human_legendary — Royal Hero

`human` · `legendary` · melee · idle×3 walk×3 attack×3 dead×2 skill×4

**Bốn màu** (ngoài đen): `#2E5BA8` chủ đạo, `#A14925` `#BA8355` `#EFCDA2`

> Draw the game character **Royal Hero** in 3 poses of a WALK CYCLE, as ONE
> horizontal row of 3 figures, on a fully transparent background.
>
> **BODY SHAPE — this decides whether players can tell it apart**
> an ordinary two-legged humanoid with square shoulders and a broad chest.
>
> **RANK FEATURES**
> Keep the rare hand object and the epic head piece, AND add a mass BEHIND the body: a cape, wings, a chimney, a banner.
>
> **COLOUR**
> warm tan skin and ROYAL BLUE cloth. Use exactly these four colours plus pure black:
> `#2E5BA8` (the dominant one), `#A14925`, `#BA8355`, `#EFCDA2`.
>
> **STYLE — measured from the attached reference, these are hard numbers**
>
> * Use EXACTLY five colours in the whole drawing: pure black #000000 for
>   every line, plus the four listed below. No sixth colour. No tinting,
>   no blending — each area is one flat colour with a hard edge.
> * Every outline is PURE BLACK #000000. Never grey, never a dark version
>   of the fill, never coloured.
> * The line has ONE CONSTANT THICKNESS everywhere, about 4% of the
>   figure's total height. If a figure is 1000 pixels tall, its outline is
>   about 40 pixels thick. This is VERY thick.
> * Interior lines — ribs, straps, a helmet rim — use the SAME thickness
>   as the outer silhouette. The line never tapers, fades or breaks.
> * When finished, roughly 40% of each figure's area should be black line.
>   If it looks like less than a third, the lines are too thin.
> * CHIBI proportions: head about one third of the total height, short
>   thick limbs, small feet.
> * Face: two solid black eye shapes and one simple mouth line. No nose,
>   no pupils, no teeth, no eyebrows.
>
> **MUST NOT APPEAR**
> No gradients, no airbrush, no soft shading, no blur, no glow, no
> specular highlight, no rim light, no drop shadow. No fabric weave, no
> metal scratches, no wood grain, no surface texture. No ornament,
> rivet, stud, buckle or strap thinner than one fifteenth of the figure's
> height. No background, no ground, no scenery.
>
> **DISPLAY SIZE**
> This character is shown about 50 pixels tall in the game, with twenty
> others around it. Anything that would vanish at that size must be left
> out. The figure must be recognisable from its solid black silhouette
> alone.
>
> **THE SEQUENCE**
> The 3 figures form a smooth loopable walk — the last figure leads
> back into the first. Side view, facing RIGHT, slightly three-quarter. The body
> bobs across the cycle, but the FEET all rest on one shared ground
> line: the lowest point of every figure is at the same height.
>
> **LAYOUT**
> At least 80 pixels of empty transparent space between neighbouring
> figures — nothing may touch, including weapons. At least 60 pixels of
> transparent margin around the whole row. Every figure drawn COMPLETE,
> nothing cropped at any edge.
>
> TRUE TRANSPARENT alpha background — no magenta, no white, no
> checkerboard. The alpha must be ANTI-ALIASED, fading over about one
> pixel; do not threshold or posterise it.
>
> No text, no labels, no numbers, no watermark, no ground shadow.

Rồi lặp lại cho `idle`, `attack`, `dead`, `skill` — đổi
đúng một dòng **THE SEQUENCE**, giữ nguyên mọi ràng buộc khác.

Đặt file: `artsrc/units_v4/human_legendary_<clip>.png`

---

## machine_common — Rust Roller

`machine` · `common` · melee · idle×3 walk×3 attack×3 dead×2

**Bốn màu** (ngoài đen): `#E07A24` chủ đạo, `#AD917E` `#5D4B44` `#DCC4AB`

> Draw the game character **Rust Roller** in 3 poses of a WALK CYCLE, as ONE
> horizontal row of 3 figures, on a fully transparent background.
>
> **BODY SHAPE — this decides whether players can tell it apart**
> a machine with NO LEGS AT ALL — a rounded body riding on wheels or a tread or hovering on a base. It must never read as a person.
>
> **RANK FEATURES**
> Nothing extra. Keep the silhouette plain.
>
> **COLOUR**
> STEEL GREY body and ORANGE trim. Use exactly these four colours plus pure black:
> `#E07A24` (the dominant one), `#AD917E`, `#5D4B44`, `#DCC4AB`.
>
> **STYLE — measured from the attached reference, these are hard numbers**
>
> * Use EXACTLY five colours in the whole drawing: pure black #000000 for
>   every line, plus the four listed below. No sixth colour. No tinting,
>   no blending — each area is one flat colour with a hard edge.
> * Every outline is PURE BLACK #000000. Never grey, never a dark version
>   of the fill, never coloured.
> * The line has ONE CONSTANT THICKNESS everywhere, about 4% of the
>   figure's total height. If a figure is 1000 pixels tall, its outline is
>   about 40 pixels thick. This is VERY thick.
> * Interior lines — ribs, straps, a helmet rim — use the SAME thickness
>   as the outer silhouette. The line never tapers, fades or breaks.
> * When finished, roughly 40% of each figure's area should be black line.
>   If it looks like less than a third, the lines are too thin.
> * CHIBI proportions: head about one third of the total height, short
>   thick limbs, small feet.
> * Face: two solid black eye shapes and one simple mouth line. No nose,
>   no pupils, no teeth, no eyebrows.
>
> **MUST NOT APPEAR**
> No gradients, no airbrush, no soft shading, no blur, no glow, no
> specular highlight, no rim light, no drop shadow. No fabric weave, no
> metal scratches, no wood grain, no surface texture. No ornament,
> rivet, stud, buckle or strap thinner than one fifteenth of the figure's
> height. No background, no ground, no scenery.
>
> **DISPLAY SIZE**
> This character is shown about 50 pixels tall in the game, with twenty
> others around it. Anything that would vanish at that size must be left
> out. The figure must be recognisable from its solid black silhouette
> alone.
>
> **THE SEQUENCE**
> The 3 figures form a smooth loopable walk — the last figure leads
> back into the first. Side view, facing RIGHT, slightly three-quarter. The body
> bobs across the cycle, but the FEET all rest on one shared ground
> line: the lowest point of every figure is at the same height.
>
> **LAYOUT**
> At least 80 pixels of empty transparent space between neighbouring
> figures — nothing may touch, including weapons. At least 60 pixels of
> transparent margin around the whole row. Every figure drawn COMPLETE,
> nothing cropped at any edge.
>
> TRUE TRANSPARENT alpha background — no magenta, no white, no
> checkerboard. The alpha must be ANTI-ALIASED, fading over about one
> pixel; do not threshold or posterise it.
>
> No text, no labels, no numbers, no watermark, no ground shadow.

Rồi lặp lại cho `idle`, `attack`, `dead` — đổi
đúng một dòng **THE SEQUENCE**, giữ nguyên mọi ràng buộc khác.

Đặt file: `artsrc/units_v4/machine_common_<clip>.png`

---

## machine_rare — Gear Gunner

`machine` · `rare` · ranged · idle×3 walk×3 attack×3 dead×2

**Bốn màu** (ngoài đen): `#E07A24` chủ đạo, `#544133` `#983C1C` `#956E47`

> Draw the game character **Gear Gunner** in 3 poses of a WALK CYCLE, as ONE
> horizontal row of 3 figures, on a fully transparent background.
>
> **BODY SHAPE — this decides whether players can tell it apart**
> a machine with NO LEGS AT ALL — a rounded body riding on wheels or a tread or hovering on a base. It must never read as a person.
>
> **RANK FEATURES**
> Add ONE large object held in the hands, big enough to change the silhouette — an oversized weapon, tool or instrument.
>
> **COLOUR**
> STEEL GREY body and ORANGE trim. Use exactly these four colours plus pure black:
> `#E07A24` (the dominant one), `#544133`, `#983C1C`, `#956E47`.
>
> **STYLE — measured from the attached reference, these are hard numbers**
>
> * Use EXACTLY five colours in the whole drawing: pure black #000000 for
>   every line, plus the four listed below. No sixth colour. No tinting,
>   no blending — each area is one flat colour with a hard edge.
> * Every outline is PURE BLACK #000000. Never grey, never a dark version
>   of the fill, never coloured.
> * The line has ONE CONSTANT THICKNESS everywhere, about 4% of the
>   figure's total height. If a figure is 1000 pixels tall, its outline is
>   about 40 pixels thick. This is VERY thick.
> * Interior lines — ribs, straps, a helmet rim — use the SAME thickness
>   as the outer silhouette. The line never tapers, fades or breaks.
> * When finished, roughly 40% of each figure's area should be black line.
>   If it looks like less than a third, the lines are too thin.
> * CHIBI proportions: head about one third of the total height, short
>   thick limbs, small feet.
> * Face: two solid black eye shapes and one simple mouth line. No nose,
>   no pupils, no teeth, no eyebrows.
>
> **MUST NOT APPEAR**
> No gradients, no airbrush, no soft shading, no blur, no glow, no
> specular highlight, no rim light, no drop shadow. No fabric weave, no
> metal scratches, no wood grain, no surface texture. No ornament,
> rivet, stud, buckle or strap thinner than one fifteenth of the figure's
> height. No background, no ground, no scenery.
>
> **DISPLAY SIZE**
> This character is shown about 50 pixels tall in the game, with twenty
> others around it. Anything that would vanish at that size must be left
> out. The figure must be recognisable from its solid black silhouette
> alone.
>
> **THE SEQUENCE**
> The 3 figures form a smooth loopable walk — the last figure leads
> back into the first. Side view, facing RIGHT, slightly three-quarter. The body
> bobs across the cycle, but the FEET all rest on one shared ground
> line: the lowest point of every figure is at the same height.
>
> **LAYOUT**
> At least 80 pixels of empty transparent space between neighbouring
> figures — nothing may touch, including weapons. At least 60 pixels of
> transparent margin around the whole row. Every figure drawn COMPLETE,
> nothing cropped at any edge.
>
> TRUE TRANSPARENT alpha background — no magenta, no white, no
> checkerboard. The alpha must be ANTI-ALIASED, fading over about one
> pixel; do not threshold or posterise it.
>
> No text, no labels, no numbers, no watermark, no ground shadow.

Rồi lặp lại cho `idle`, `attack`, `dead` — đổi
đúng một dòng **THE SEQUENCE**, giữ nguyên mọi ràng buộc khác.

Đặt file: `artsrc/units_v4/machine_rare_<clip>.png`

---

## machine_epic — Furnace Crawler

`machine` · `epic` · melee · idle×3 walk×3 attack×3 dead×2

**Bốn màu** (ngoài đen): `#E07A24` chủ đạo, `#5F3D29` `#B63919` `#E8B966`

> Draw the game character **Furnace Crawler** in 3 poses of a WALK CYCLE, as ONE
> horizontal row of 3 figures, on a fully transparent background.
>
> **BODY SHAPE — this decides whether players can tell it apart**
> a machine with NO LEGS AT ALL — a rounded body riding on wheels or a tread or hovering on a base. It must never read as a person.
>
> **RANK FEATURES**
> Keep the rare hand object, AND add something standing UP off the head: a horn, a tall pointed hat, an antenna, a crest.
>
> **COLOUR**
> STEEL GREY body and ORANGE trim. Use exactly these four colours plus pure black:
> `#E07A24` (the dominant one), `#5F3D29`, `#B63919`, `#E8B966`.
>
> **STYLE — measured from the attached reference, these are hard numbers**
>
> * Use EXACTLY five colours in the whole drawing: pure black #000000 for
>   every line, plus the four listed below. No sixth colour. No tinting,
>   no blending — each area is one flat colour with a hard edge.
> * Every outline is PURE BLACK #000000. Never grey, never a dark version
>   of the fill, never coloured.
> * The line has ONE CONSTANT THICKNESS everywhere, about 4% of the
>   figure's total height. If a figure is 1000 pixels tall, its outline is
>   about 40 pixels thick. This is VERY thick.
> * Interior lines — ribs, straps, a helmet rim — use the SAME thickness
>   as the outer silhouette. The line never tapers, fades or breaks.
> * When finished, roughly 40% of each figure's area should be black line.
>   If it looks like less than a third, the lines are too thin.
> * CHIBI proportions: head about one third of the total height, short
>   thick limbs, small feet.
> * Face: two solid black eye shapes and one simple mouth line. No nose,
>   no pupils, no teeth, no eyebrows.
>
> **MUST NOT APPEAR**
> No gradients, no airbrush, no soft shading, no blur, no glow, no
> specular highlight, no rim light, no drop shadow. No fabric weave, no
> metal scratches, no wood grain, no surface texture. No ornament,
> rivet, stud, buckle or strap thinner than one fifteenth of the figure's
> height. No background, no ground, no scenery.
>
> **DISPLAY SIZE**
> This character is shown about 50 pixels tall in the game, with twenty
> others around it. Anything that would vanish at that size must be left
> out. The figure must be recognisable from its solid black silhouette
> alone.
>
> **THE SEQUENCE**
> The 3 figures form a smooth loopable walk — the last figure leads
> back into the first. Side view, facing RIGHT, slightly three-quarter. The body
> bobs across the cycle, but the FEET all rest on one shared ground
> line: the lowest point of every figure is at the same height.
>
> **LAYOUT**
> At least 80 pixels of empty transparent space between neighbouring
> figures — nothing may touch, including weapons. At least 60 pixels of
> transparent margin around the whole row. Every figure drawn COMPLETE,
> nothing cropped at any edge.
>
> TRUE TRANSPARENT alpha background — no magenta, no white, no
> checkerboard. The alpha must be ANTI-ALIASED, fading over about one
> pixel; do not threshold or posterise it.
>
> No text, no labels, no numbers, no watermark, no ground shadow.

Rồi lặp lại cho `idle`, `attack`, `dead` — đổi
đúng một dòng **THE SEQUENCE**, giữ nguyên mọi ràng buộc khác.

Đặt file: `artsrc/units_v4/machine_epic_<clip>.png`

---

## machine_legendary — Titan Pugilist

`machine` · `legendary` · melee · idle×3 walk×3 attack×3 dead×2 skill×4

**Bốn màu** (ngoài đen): `#E07A24` chủ đạo, `#934D19` `#454A42` `#E8BD6B`

> Draw the game character **Titan Pugilist** in 3 poses of a WALK CYCLE, as ONE
> horizontal row of 3 figures, on a fully transparent background.
>
> **BODY SHAPE — this decides whether players can tell it apart**
> a machine with NO LEGS AT ALL — a rounded body riding on wheels or a tread or hovering on a base. It must never read as a person.
>
> **RANK FEATURES**
> Keep the rare hand object and the epic head piece, AND add a mass BEHIND the body: a cape, wings, a chimney, a banner.
>
> **COLOUR**
> STEEL GREY body and ORANGE trim. Use exactly these four colours plus pure black:
> `#E07A24` (the dominant one), `#934D19`, `#454A42`, `#E8BD6B`.
>
> **STYLE — measured from the attached reference, these are hard numbers**
>
> * Use EXACTLY five colours in the whole drawing: pure black #000000 for
>   every line, plus the four listed below. No sixth colour. No tinting,
>   no blending — each area is one flat colour with a hard edge.
> * Every outline is PURE BLACK #000000. Never grey, never a dark version
>   of the fill, never coloured.
> * The line has ONE CONSTANT THICKNESS everywhere, about 4% of the
>   figure's total height. If a figure is 1000 pixels tall, its outline is
>   about 40 pixels thick. This is VERY thick.
> * Interior lines — ribs, straps, a helmet rim — use the SAME thickness
>   as the outer silhouette. The line never tapers, fades or breaks.
> * When finished, roughly 40% of each figure's area should be black line.
>   If it looks like less than a third, the lines are too thin.
> * CHIBI proportions: head about one third of the total height, short
>   thick limbs, small feet.
> * Face: two solid black eye shapes and one simple mouth line. No nose,
>   no pupils, no teeth, no eyebrows.
>
> **MUST NOT APPEAR**
> No gradients, no airbrush, no soft shading, no blur, no glow, no
> specular highlight, no rim light, no drop shadow. No fabric weave, no
> metal scratches, no wood grain, no surface texture. No ornament,
> rivet, stud, buckle or strap thinner than one fifteenth of the figure's
> height. No background, no ground, no scenery.
>
> **DISPLAY SIZE**
> This character is shown about 50 pixels tall in the game, with twenty
> others around it. Anything that would vanish at that size must be left
> out. The figure must be recognisable from its solid black silhouette
> alone.
>
> **THE SEQUENCE**
> The 3 figures form a smooth loopable walk — the last figure leads
> back into the first. Side view, facing RIGHT, slightly three-quarter. The body
> bobs across the cycle, but the FEET all rest on one shared ground
> line: the lowest point of every figure is at the same height.
>
> **LAYOUT**
> At least 80 pixels of empty transparent space between neighbouring
> figures — nothing may touch, including weapons. At least 60 pixels of
> transparent margin around the whole row. Every figure drawn COMPLETE,
> nothing cropped at any edge.
>
> TRUE TRANSPARENT alpha background — no magenta, no white, no
> checkerboard. The alpha must be ANTI-ALIASED, fading over about one
> pixel; do not threshold or posterise it.
>
> No text, no labels, no numbers, no watermark, no ground shadow.

Rồi lặp lại cho `idle`, `attack`, `dead`, `skill` — đổi
đúng một dòng **THE SEQUENCE**, giữ nguyên mọi ràng buộc khác.

Đặt file: `artsrc/units_v4/machine_legendary_<clip>.png`

---

## monster_common — Cave Creeper

`monster` · `common` · ranged · idle×3 walk×3 attack×3 dead×2

**Bốn màu** (ngoài đen): `#B5367A` chủ đạo, `#955D89` `#4E2C4C` `#6E3F6B`

> Draw the game character **Cave Creeper** in 3 poses of a WALK CYCLE, as ONE
> horizontal row of 3 figures, on a fully transparent background.
>
> **BODY SHAPE — this decides whether players can tell it apart**
> a LOW FOUR-LEGGED BEAST, wide and close to the ground. NOT humanoid. It must never read as a person standing upright.
>
> **RANK FEATURES**
> Nothing extra. Keep the silhouette plain.
>
> **COLOUR**
> MAGENTA-PINK hide and dark plum shadow. Use exactly these four colours plus pure black:
> `#B5367A` (the dominant one), `#955D89`, `#4E2C4C`, `#6E3F6B`.
>
> **STYLE — measured from the attached reference, these are hard numbers**
>
> * Use EXACTLY five colours in the whole drawing: pure black #000000 for
>   every line, plus the four listed below. No sixth colour. No tinting,
>   no blending — each area is one flat colour with a hard edge.
> * Every outline is PURE BLACK #000000. Never grey, never a dark version
>   of the fill, never coloured.
> * The line has ONE CONSTANT THICKNESS everywhere, about 4% of the
>   figure's total height. If a figure is 1000 pixels tall, its outline is
>   about 40 pixels thick. This is VERY thick.
> * Interior lines — ribs, straps, a helmet rim — use the SAME thickness
>   as the outer silhouette. The line never tapers, fades or breaks.
> * When finished, roughly 40% of each figure's area should be black line.
>   If it looks like less than a third, the lines are too thin.
> * CHIBI proportions: head about one third of the total height, short
>   thick limbs, small feet.
> * Face: two solid black eye shapes and one simple mouth line. No nose,
>   no pupils, no teeth, no eyebrows.
>
> **MUST NOT APPEAR**
> No gradients, no airbrush, no soft shading, no blur, no glow, no
> specular highlight, no rim light, no drop shadow. No fabric weave, no
> metal scratches, no wood grain, no surface texture. No ornament,
> rivet, stud, buckle or strap thinner than one fifteenth of the figure's
> height. No background, no ground, no scenery.
>
> **DISPLAY SIZE**
> This character is shown about 50 pixels tall in the game, with twenty
> others around it. Anything that would vanish at that size must be left
> out. The figure must be recognisable from its solid black silhouette
> alone.
>
> **THE SEQUENCE**
> The 3 figures form a smooth loopable walk — the last figure leads
> back into the first. Side view, facing RIGHT, slightly three-quarter. The body
> bobs across the cycle, but the FEET all rest on one shared ground
> line: the lowest point of every figure is at the same height.
>
> **LAYOUT**
> At least 80 pixels of empty transparent space between neighbouring
> figures — nothing may touch, including weapons. At least 60 pixels of
> transparent margin around the whole row. Every figure drawn COMPLETE,
> nothing cropped at any edge.
>
> TRUE TRANSPARENT alpha background — no magenta, no white, no
> checkerboard. The alpha must be ANTI-ALIASED, fading over about one
> pixel; do not threshold or posterise it.
>
> No text, no labels, no numbers, no watermark, no ground shadow.

Rồi lặp lại cho `idle`, `attack`, `dead` — đổi
đúng một dòng **THE SEQUENCE**, giữ nguyên mọi ràng buộc khác.

Đặt file: `artsrc/units_v4/monster_common_<clip>.png`

---

## monster_rare — Armored Bear

`monster` · `rare` · melee · idle×3 walk×3 attack×3 dead×2

**Bốn màu** (ngoài đen): `#B5367A` chủ đạo, `#CC7137` `#BB612C` `#804528`

> Draw the game character **Armored Bear** in 3 poses of a WALK CYCLE, as ONE
> horizontal row of 3 figures, on a fully transparent background.
>
> **BODY SHAPE — this decides whether players can tell it apart**
> a LOW FOUR-LEGGED BEAST, wide and close to the ground. NOT humanoid. It must never read as a person standing upright.
>
> **RANK FEATURES**
> Add ONE large object held in the hands, big enough to change the silhouette — an oversized weapon, tool or instrument.
>
> **COLOUR**
> MAGENTA-PINK hide and dark plum shadow. Use exactly these four colours plus pure black:
> `#B5367A` (the dominant one), `#CC7137`, `#BB612C`, `#804528`.
>
> **STYLE — measured from the attached reference, these are hard numbers**
>
> * Use EXACTLY five colours in the whole drawing: pure black #000000 for
>   every line, plus the four listed below. No sixth colour. No tinting,
>   no blending — each area is one flat colour with a hard edge.
> * Every outline is PURE BLACK #000000. Never grey, never a dark version
>   of the fill, never coloured.
> * The line has ONE CONSTANT THICKNESS everywhere, about 4% of the
>   figure's total height. If a figure is 1000 pixels tall, its outline is
>   about 40 pixels thick. This is VERY thick.
> * Interior lines — ribs, straps, a helmet rim — use the SAME thickness
>   as the outer silhouette. The line never tapers, fades or breaks.
> * When finished, roughly 40% of each figure's area should be black line.
>   If it looks like less than a third, the lines are too thin.
> * CHIBI proportions: head about one third of the total height, short
>   thick limbs, small feet.
> * Face: two solid black eye shapes and one simple mouth line. No nose,
>   no pupils, no teeth, no eyebrows.
>
> **MUST NOT APPEAR**
> No gradients, no airbrush, no soft shading, no blur, no glow, no
> specular highlight, no rim light, no drop shadow. No fabric weave, no
> metal scratches, no wood grain, no surface texture. No ornament,
> rivet, stud, buckle or strap thinner than one fifteenth of the figure's
> height. No background, no ground, no scenery.
>
> **DISPLAY SIZE**
> This character is shown about 50 pixels tall in the game, with twenty
> others around it. Anything that would vanish at that size must be left
> out. The figure must be recognisable from its solid black silhouette
> alone.
>
> **THE SEQUENCE**
> The 3 figures form a smooth loopable walk — the last figure leads
> back into the first. Side view, facing RIGHT, slightly three-quarter. The body
> bobs across the cycle, but the FEET all rest on one shared ground
> line: the lowest point of every figure is at the same height.
>
> **LAYOUT**
> At least 80 pixels of empty transparent space between neighbouring
> figures — nothing may touch, including weapons. At least 60 pixels of
> transparent margin around the whole row. Every figure drawn COMPLETE,
> nothing cropped at any edge.
>
> TRUE TRANSPARENT alpha background — no magenta, no white, no
> checkerboard. The alpha must be ANTI-ALIASED, fading over about one
> pixel; do not threshold or posterise it.
>
> No text, no labels, no numbers, no watermark, no ground shadow.

Rồi lặp lại cho `idle`, `attack`, `dead` — đổi
đúng một dòng **THE SEQUENCE**, giữ nguyên mọi ràng buộc khác.

Đặt file: `artsrc/units_v4/monster_rare_<clip>.png`

---

## monster_epic — Lava Scorpion

`monster` · `epic` · melee · idle×3 walk×3 attack×3 dead×2

**Bốn màu** (ngoài đen): `#B5367A` chủ đạo, `#62302F` `#D97322` `#B42910`

> Draw the game character **Lava Scorpion** in 3 poses of a WALK CYCLE, as ONE
> horizontal row of 3 figures, on a fully transparent background.
>
> **BODY SHAPE — this decides whether players can tell it apart**
> a LOW FOUR-LEGGED BEAST, wide and close to the ground. NOT humanoid. It must never read as a person standing upright.
>
> **RANK FEATURES**
> Keep the rare hand object, AND add something standing UP off the head: a horn, a tall pointed hat, an antenna, a crest.
>
> **COLOUR**
> MAGENTA-PINK hide and dark plum shadow. Use exactly these four colours plus pure black:
> `#B5367A` (the dominant one), `#62302F`, `#D97322`, `#B42910`.
>
> **STYLE — measured from the attached reference, these are hard numbers**
>
> * Use EXACTLY five colours in the whole drawing: pure black #000000 for
>   every line, plus the four listed below. No sixth colour. No tinting,
>   no blending — each area is one flat colour with a hard edge.
> * Every outline is PURE BLACK #000000. Never grey, never a dark version
>   of the fill, never coloured.
> * The line has ONE CONSTANT THICKNESS everywhere, about 4% of the
>   figure's total height. If a figure is 1000 pixels tall, its outline is
>   about 40 pixels thick. This is VERY thick.
> * Interior lines — ribs, straps, a helmet rim — use the SAME thickness
>   as the outer silhouette. The line never tapers, fades or breaks.
> * When finished, roughly 40% of each figure's area should be black line.
>   If it looks like less than a third, the lines are too thin.
> * CHIBI proportions: head about one third of the total height, short
>   thick limbs, small feet.
> * Face: two solid black eye shapes and one simple mouth line. No nose,
>   no pupils, no teeth, no eyebrows.
>
> **MUST NOT APPEAR**
> No gradients, no airbrush, no soft shading, no blur, no glow, no
> specular highlight, no rim light, no drop shadow. No fabric weave, no
> metal scratches, no wood grain, no surface texture. No ornament,
> rivet, stud, buckle or strap thinner than one fifteenth of the figure's
> height. No background, no ground, no scenery.
>
> **DISPLAY SIZE**
> This character is shown about 50 pixels tall in the game, with twenty
> others around it. Anything that would vanish at that size must be left
> out. The figure must be recognisable from its solid black silhouette
> alone.
>
> **THE SEQUENCE**
> The 3 figures form a smooth loopable walk — the last figure leads
> back into the first. Side view, facing RIGHT, slightly three-quarter. The body
> bobs across the cycle, but the FEET all rest on one shared ground
> line: the lowest point of every figure is at the same height.
>
> **LAYOUT**
> At least 80 pixels of empty transparent space between neighbouring
> figures — nothing may touch, including weapons. At least 60 pixels of
> transparent margin around the whole row. Every figure drawn COMPLETE,
> nothing cropped at any edge.
>
> TRUE TRANSPARENT alpha background — no magenta, no white, no
> checkerboard. The alpha must be ANTI-ALIASED, fading over about one
> pixel; do not threshold or posterise it.
>
> No text, no labels, no numbers, no watermark, no ground shadow.

Rồi lặp lại cho `idle`, `attack`, `dead` — đổi
đúng một dòng **THE SEQUENCE**, giữ nguyên mọi ràng buộc khác.

Đặt file: `artsrc/units_v4/monster_epic_<clip>.png`

---

## monster_legendary — Lion Warlord

`monster` · `legendary` · melee · idle×3 walk×3 attack×3 dead×2 skill×4

**Bốn màu** (ngoài đen): `#B5367A` chủ đạo, `#B7400F` `#C46B32` `#7D3A1C`

> Draw the game character **Lion Warlord** in 3 poses of a WALK CYCLE, as ONE
> horizontal row of 3 figures, on a fully transparent background.
>
> **BODY SHAPE — this decides whether players can tell it apart**
> a LOW FOUR-LEGGED BEAST, wide and close to the ground. NOT humanoid. It must never read as a person standing upright.
>
> **RANK FEATURES**
> Keep the rare hand object and the epic head piece, AND add a mass BEHIND the body: a cape, wings, a chimney, a banner.
>
> **COLOUR**
> MAGENTA-PINK hide and dark plum shadow. Use exactly these four colours plus pure black:
> `#B5367A` (the dominant one), `#B7400F`, `#C46B32`, `#7D3A1C`.
>
> **STYLE — measured from the attached reference, these are hard numbers**
>
> * Use EXACTLY five colours in the whole drawing: pure black #000000 for
>   every line, plus the four listed below. No sixth colour. No tinting,
>   no blending — each area is one flat colour with a hard edge.
> * Every outline is PURE BLACK #000000. Never grey, never a dark version
>   of the fill, never coloured.
> * The line has ONE CONSTANT THICKNESS everywhere, about 4% of the
>   figure's total height. If a figure is 1000 pixels tall, its outline is
>   about 40 pixels thick. This is VERY thick.
> * Interior lines — ribs, straps, a helmet rim — use the SAME thickness
>   as the outer silhouette. The line never tapers, fades or breaks.
> * When finished, roughly 40% of each figure's area should be black line.
>   If it looks like less than a third, the lines are too thin.
> * CHIBI proportions: head about one third of the total height, short
>   thick limbs, small feet.
> * Face: two solid black eye shapes and one simple mouth line. No nose,
>   no pupils, no teeth, no eyebrows.
>
> **MUST NOT APPEAR**
> No gradients, no airbrush, no soft shading, no blur, no glow, no
> specular highlight, no rim light, no drop shadow. No fabric weave, no
> metal scratches, no wood grain, no surface texture. No ornament,
> rivet, stud, buckle or strap thinner than one fifteenth of the figure's
> height. No background, no ground, no scenery.
>
> **DISPLAY SIZE**
> This character is shown about 50 pixels tall in the game, with twenty
> others around it. Anything that would vanish at that size must be left
> out. The figure must be recognisable from its solid black silhouette
> alone.
>
> **THE SEQUENCE**
> The 3 figures form a smooth loopable walk — the last figure leads
> back into the first. Side view, facing RIGHT, slightly three-quarter. The body
> bobs across the cycle, but the FEET all rest on one shared ground
> line: the lowest point of every figure is at the same height.
>
> **LAYOUT**
> At least 80 pixels of empty transparent space between neighbouring
> figures — nothing may touch, including weapons. At least 60 pixels of
> transparent margin around the whole row. Every figure drawn COMPLETE,
> nothing cropped at any edge.
>
> TRUE TRANSPARENT alpha background — no magenta, no white, no
> checkerboard. The alpha must be ANTI-ALIASED, fading over about one
> pixel; do not threshold or posterise it.
>
> No text, no labels, no numbers, no watermark, no ground shadow.

Rồi lặp lại cho `idle`, `attack`, `dead`, `skill` — đổi
đúng một dòng **THE SEQUENCE**, giữ nguyên mọi ràng buộc khác.

Đặt file: `artsrc/units_v4/monster_legendary_<clip>.png`

---

## Đổi dòng THE SEQUENCE cho từng clip

| clip | số khung | thay bằng |
|---|---|---|
| `idle` | 3 | a subtle idle breathing loop, standing in place, weight shifting slightly |
| `attack` | 3 | one attack swing or shot: wind-up, strike, follow-through |
| `dead` | 2 | one stagger and one final pose lying still on the ground |
| `skill` | 4 | one big special move, larger and slower than the normal attack |

## Vì sao 3 khung, không phải 8

Đếm trên 108 nhân vật chơi được trong game tham chiếu:

```
idle    trung vi 3 khung
attack  trung vi 3
walk    trung vi 2
dead    trung vi 1
skill   trung vi 4
```

Tổng mỗi nhân vật **trung vị 11 khung**. Bộ cũ của ta dùng **34**.

Họ ít khung mà vẫn mượt vì đếm được **40 663 `Animator`** bên cạnh **18 762
`SpriteRenderer`** — chuyển động nằm ở phép biến đổi, không ở khung hình.

`FrameUnitVisual` của ta **đã làm đúng việc đó**: nhún khi đứng, nhún và
nghiêng khi đi, lao người khi đánh. Vẽ 8 khung là trả tiền hai lần cho
cùng một chuyển động.

**680 khung → ~230 khung.** Và ít khung còn dễ đạt chuẩn hơn: ràng buộc
khó nhất là mọi khung cùng cỡ, cùng nhân vật, chân cùng mặt đất — ba khung
thì công cụ sinh ảnh giữ nhất quán dễ hơn tám.

`dead` là clip duy nhất **không** lặp, và là clip duy nhất chân **không**
cùng một mặt đất ở khung cuối — bỏ ràng buộc đó cho riêng nó.

## ~230 khung, vẫn đừng làm một lượt

20 nhân vật × 4–5 clip. Làm từng clip một, đo từng tấm. Sinh hàng loạt rồi
phát hiện sai ở tấm thứ 40 là mất cả buổi.
