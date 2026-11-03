#!/usr/bin/env python3
"""Sinh outdata/PROMPT_UNITS_V4.md — 20 prompt nhan vat, tu du lieu that.

Vi sao sinh bang script chu khong go tay: ten, danh sach clip va so khung
lay tu tools/units_manifest.json. Go tay 20 lan thi sau mot lan doi
manifest la tai lieu lech voi game, va lech im lang.

Ba truc tach bach, doc lap nhau:
  CHUNG  -> dang than  (5 dang khac han ve bong hinh)
  CHUNG  -> dai mau    (5 dai mau khac han)
  BAC    -> phu kien   (4 muc nho ra khoi bong hinh, cong don)
5 x 4 = 20 o, moi o khac nhau theo CAU TAO chu khong nho may man.
"""

import json
from pathlib import Path

MAN = Path("tools/units_manifest.json")
OUT = Path("outdata/PROMPT_UNITS_V4.md")

# Do duoc tren art hien tai: cung chung toc 0.64, khac chung toc 0.60 —
# chenh 0.04, tuc chung toc KHONG tao khac biet bong hinh nao. Day la
# cho sua.
BODY = {
 "human":   "an ordinary two-legged humanoid with square shoulders and a broad chest",
 "elf":     "a TALL, SLENDER two-legged humanoid with LONG POINTED EARS and long hair "
            "that both stick out clearly from the silhouette",
 "dead":    "a THIN SKELETAL two-legged figure with VISIBLE GAPS between the bones, "
            "so daylight shows through the ribcage and the limbs",
 "monster": "a LOW FOUR-LEGGED BEAST, wide and close to the ground. NOT humanoid. "
            "It must never read as a person standing upright",
 "machine": "a machine with NO LEGS AT ALL — a rounded body riding on wheels or "
            "a tread or hovering on a base. It must never read as a person",
}

HUE = {
 "human":   ("warm tan skin and ROYAL BLUE cloth", "#2E5BA8"),
 "elf":     ("pale skin and LEAF GREEN cloth", "#3E8E41"),
 "dead":    ("BONE WHITE body and DEEP PURPLE cloth", "#6B3FA0"),
 "monster": ("MAGENTA-PINK hide and dark plum shadow", "#B5367A"),
 "machine": ("STEEL GREY body and ORANGE trim", "#E07A24"),
}

RANK = {
 "common":    "Nothing extra. Keep the silhouette plain.",
 "rare":      "Add ONE large object held in the hands, big enough to change the "
              "silhouette — an oversized weapon, tool or instrument.",
 "epic":      "Keep the rare hand object, AND add something standing UP off the "
              "head: a horn, a tall pointed hat, an antenna, a crest.",
 "legendary": "Keep the rare hand object and the epic head piece, AND add a mass "
              "BEHIND the body: a cape, wings, a chimney, a banner.",
}

# So khung moi clip — do tren game tham chieu, 108 nhan vat choi duoc:
#   idle trung vi 3, attack 3, walk 2, dead 1, skill 4
# Bo cu cua ta dung 8 khung cho MOI clip, tuc gap ~3 lan.
#
# Ho it khung ma van muot vi dem duoc 40663 Animator ben canh 18762
# SpriteRenderer — chuyen dong nam o phep bien doi, khong o khung hinh.
# FrameUnitVisual cua ta da lam dung viec do (nhun, nghieng, lao nguoi),
# nen ve 8 khung la tra tien hai lan cho cung mot chuyen dong.
#
# Lay cao hon trung vi mot chut cho walk va dead: walk 1-2 khung thi ping-pong
# khong con gi de pha, va dead 1 khung thi mat han cu nga.
FRAMES = {"idle": 3, "walk": 3, "attack": 3, "dead": 2, "skill": 4}

NAMES = {   # ba con chua co ten trong manifest
 "elf_common": "Thorn Scout",
 "machine_common": "Rust Roller",
 "monster_common": "Cave Creeper",
}

STYLE = """> **STYLE — measured from the attached reference, these are hard numbers**
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
> alone."""


def main():
    rows = json.loads(MAN.read_text())
    rows.sort(key=lambda r: (r["race"], ["common", "rare", "epic", "legendary"].index(r["rank"])))
    pal = json.loads(Path("tools/unit_palettes.json").read_text())

    L = [f"""# Yêu cầu art — vẽ lại 20 nhân vật (bản 4)

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
"""]

    for r in rows:
        key, race, rank = r["key"], r["race"], r["rank"]
        name = r.get("name") or NAMES.get(key, key)
        clips = [(k, FRAMES[k]) for k, v in r["anims"].items() if v]
        hue, accent = HUE[race]
        colours = pal[key][1:]

        L.append(f"\n## {key} — {name}\n")
        L.append(f"`{race}` · `{rank}` · {r.get('combat','?')} · "
                 + " ".join(f"{k}×{n}" for k, n in clips))
        L.append(f"\n**Bốn màu** (ngoài đen): `{accent}` chủ đạo, "
                 + " ".join(f"`{c}`" for c in colours[:3]))
        nw = FRAMES["walk"]
        L.append(f"""
> Draw the game character **{name}** in {nw} poses of a WALK CYCLE, as ONE
> horizontal row of {nw} figures, on a fully transparent background.
>
> **BODY SHAPE — this decides whether players can tell it apart**
> {BODY[race]}.
>
> **RANK FEATURES**
> {RANK[rank]}
>
> **COLOUR**
> {hue}. Use exactly these four colours plus pure black:
> `{accent}` (the dominant one), `{colours[0]}`, `{colours[1]}`, `{colours[2]}`.
>
{STYLE}
>
> **THE SEQUENCE**
> The {nw} figures form a smooth loopable walk — the last figure leads
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

Rồi lặp lại cho `{"`, `".join(k for k, _ in clips if k != "walk")}` — đổi
đúng một dòng **THE SEQUENCE**, giữ nguyên mọi ràng buộc khác.

Đặt file: `artsrc/units_v4/{key}_<clip>.png`

---""")

    L.append("""
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
""")
    OUT.write_text("\n".join(L))
    print(f"da viet {OUT} — {len(rows)} nhan vat")


if __name__ == "__main__":
    main()
