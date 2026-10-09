# Yêu cầu art — bù pose còn thiếu (bản 2)

Phần `attack` đã xong hết, 20/20 nhân vật có 8 pose thật. Còn lại 20
animation, thiếu 71 pose.

Ảnh tham chiếu: `outdata/fix_poses/<tên>.png` — 14 file, mỗi file một nhân
vật, đã có **vạch tím đánh dấu mặt đất** và lề rộng quanh mỗi pose.

---

## Về nền: PNG trong suốt là đúng, magenta là thừa

Bản 1 tôi xin nền magenta. Thừa, và có hại. Đo được: cả 20 tấm gốc trong
`20model/` đều là **RGBA, 0% magenta** — công cụ xuất alpha thật từ đầu. Tôi
bê quy ước magenta từ đợt art giao diện sang mà không kiểm lại.

Cái giá: tách magenta là phép **mất mát**. Điểm rìa khử răng cưa chính là màu
trộn giữa magenta và nhân vật, nên nó vừa khó tách vừa để lại quầng hồng —
đo được 15.8% điểm viền của `human_legendary` còn ngả hồng.

**Nhưng còn một yêu cầu nữa, quan trọng không kém.** Đợt vừa rồi tuy đã là
PNG trong suốt nhưng alpha chỉ có **2 mức** — cắt cứng, viền răng cưa. Tấm
`20model` gốc có **239 mức**. Nên prompt phải nói thẳng: giữ rìa khử răng cưa,
đừng ngưỡng hoá kênh alpha.

---

## Vì sao bản 1 cho ra ảnh bị gọt — lỗi của tôi

Ô trong ảnh tham chiếu bản 1 rộng **234px**, còn nhân vật vẽ ra **300–463px**.
Tôi đã cắt cụt chúng ngay trong chính file tham chiếu, rồi trong prompt lại
viết *"keep EXACTLY the same character design"* — nên công cụ sinh ảnh chép
lại đúng vết cắt đó. Đuôi xe tăng bị xén phẳng là vì vậy.

Bản 2: ô tính từ bề rộng THẬT của khung rộng nhất, cộng lề 60px mỗi bên. Và
prompt có thêm ba câu về lề, về việc không được cắt, và về khoảng cách giữa
các pose.

---

## Danh sách

| nhân vật | animation | pose đang có | cần |
|---|---|---|---|
| `dead_common` · Skeleton Warrior | **dead** | 4 | 8 |
| `dead_epic` · Haunted Tank | **dead** | 4 | 8 |
| `dead_legendary` · Skeleton King | **dead** | 4 | 8 |
| `elf_common` · elf_common | **dead** | 4 | 8 |
| `elf_epic` · Moon Archer | **dead** | 4 | 8 |
| `elf_legendary` · Verdant Archer | **skill** | 6 | 8 |
| `human_epic` · Crescent Mage | **dead** | 4 | 8 |
| `human_epic` · Crescent Mage | **idle** | 4 | 8 |
| `human_epic` · Crescent Mage | **walk** | 4 | 8 |
| `human_legendary` · Royal Hero | **dead** | 4 | 8 |
| `human_legendary` · Royal Hero | **walk** | 4 | 8 |
| `human_legendary` · Royal Hero | **skill** | 6 | 8 |
| `machine_epic` · Furnace Crawler | **dead** | 4 | 8 |
| `machine_legendary` · Titan Pugilist | **dead** | 4 | 8 |
| `machine_legendary` · Titan Pugilist | **skill** | 6 | 8 |
| `monster_common` · monster_common | **dead** | 4 | 8 |
| `monster_epic` · Lava Scorpion | **dead** | 4 | 8 |
| `monster_legendary` · Lion Warlord | **dead** | 4 | 8 |
| `monster_legendary` · Lion Warlord | **skill** | 6 | 8 |
| `monster_rare` · Armored Bear | **dead** | 5 | 8 |

Nặng nhất: **human_epic** (3 animation), **human_legendary** (3).

`dead` chạy một lần rồi xác nằm im nên ít lộ nhất — làm sau cùng cũng được.
`human_epic` idle và walk thì lộ suốt trận, nên làm trước.

---

## Prompt — dán vào công cụ sinh ảnh

Đính kèm file tham chiếu của đúng nhân vật, và làm **từng animation một**.

> The attached image shows existing animation poses for one game character.
> A purple horizontal line marks the ground; every
> figure's feet rest on it.
>
> For the row labelled **DEAD** (hoặc IDLE / WALK / SKILL), draw a complete
> **8-pose sequence** of that same character performing that action, output as
> ONE horizontal row of 8 figures.
>
> Keep exactly the same character design, colours, outline weight and shading
> as the reference, and the same overall size. Same viewing angle, facing RIGHT.
>
> CRITICAL — do NOT use fixed square cells or constrain a pose inside a box:
> * Every figure must be drawn COMPLETE. No part of any figure may be cut off
>   or cropped, at the image edges or anywhere else.
> * Leave clear transparent space between neighbouring figures — they must
>   not touch or overlap, not even their weapons, flames or effects.
> * Weapons, capes, tails, barrels and effects may extend naturally as far as
>   the action requires. Never cut them at an artificial cell edge.
>
> All eight figures stand on the SAME ground line: the lowest point of each
> figure must be at the same height.
>
> TRUE TRANSPARENT alpha background. No magenta backdrop.
>
> The alpha channel must be ANTI-ALIASED: the outline should fade smoothly
> over about one pixel, not be a hard on/off cutout. Do not threshold or
> posterise the transparency.
>
> No text, no labels, no frames, no ground shadow, no watermark.

`dead`: tám pose là một cú ngã liên tục, pose cuối là xác nằm yên trên đất.
`skill`: giữ nguyên 6 pose đang có, chèn thêm 2 pose nối cho đủ 8.

---

## Đặt file ở đâu

```
artsrc/fixposes/<ten>_<anim>_v2.png
```

---

## Một lỗi hình học còn lại

`human_legendary/skill` có chân trôi **0.216 đơn vị** qua 8 khung — lính
trượt lên xuống. Các clip lành đo được dưới 0.03. Khi vẽ lại clip này, giữ
chân đúng trên vạch tím.
