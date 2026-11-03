# Yêu cầu art — nhân vật v2, vẽ như người làm

Chỉ làm **MỘT con** trước. Lý do ở mục 4.

---

## 1. "Giống AI" là gì — gọi tên ra thì mới chặn được

Sáu dấu hiệu, và chúng đều đo được hoặc thấy được:

| dấu hiệu | vì sao AI hay làm vậy |
|---|---|
| **Tô khối mềm khắp nơi** | mặc định của mô hình là đổ bóng mượt |
| **Vật gì cũng có ánh kim, ánh viền** | nó học từ ảnh render 3D |
| **Viền mỏng dần, đứt quãng** | viền với nó là bóng tối, không phải nét vẽ |
| **Chi tiết vụn khắp mặt** | nhiều chi tiết = trông "công phu" |
| **Bảng màu nâu-vàng chung chung** | "fantasy" trong dữ liệu huấn luyện là màu đó |
| **Đứng chính diện, cân đối** | ảnh chân dung chiếm đa số dữ liệu |

Người vẽ game thì làm ngược lại — và **ngược lại vì có lý do**, không phải vì phong cách:

- Nét viền **đều một bề dày**, vì vẽ bằng một cỡ bút
- **Ít màu**, vì mỗi màu thêm vào là một màu phải nhớ khi vẽ 20 con
- Bóng là **một mảng cứng**, vì tô mảng nhanh hơn tô chuyển sắc
- **Bỏ bớt chi tiết**, vì ở 50px nó biến thành nhiễu
- **Dáng lệch**, vì bóng hình phải đọc ra ngay

---

## 2. Khối STYLE mới — dán vào mọi prompt

Khác bản trước: bản trước chỉ nói *không làm gì*. Bản này nói *làm như thế nào*.

```
STYLE — follow these as hard rules, not suggestions:

LINE
* Draw with ONE constant line weight, as if using a single pen nib.
  The outline around the figure and the lines inside it are the SAME
  thickness everywhere. It never tapers, never fades, never breaks.
* The outline is a very dark version of the colour it encloses — dark
  brown inside brown areas, dark blue inside blue areas. Not pure
  black everywhere.

COLOUR
* Use EXACTLY 5 colours for the whole character, plus the outline.
  List them mentally before drawing and reuse them. No sixth colour.
* Fill each area with ONE flat colour. Shadow is ONE extra flat shape
  with a hard edge, not a gradient.
* Light comes from the UPPER LEFT, consistently, on every pose.

SHAPE
* Build the figure from big simple shapes: circle, rounded square,
  trapezoid. A viewer must recognise it from its silhouette alone.
* Chunky proportions: large head, short thick limbs, small feet.
* Pose the figure ASYMMETRICALLY — weight on one leg, head tilted,
  one arm forward. Never a straight symmetrical front-facing stance.

LEAVE OUT
* No gradients, no airbrush, no soft shading, no blur.
* No specular highlights, no rim light, no glow, no lens flare.
* No fabric weave, no metal scratches, no wood grain, no surface
  texture of any kind.
* No tiny ornaments, no filigree, no rivets, no studs, no straps
  smaller than one fifteenth of the figure's height.

Not pixel art. Not painterly. Not photorealistic. Not a 3D render.
Think printed sticker, or a vinyl toy, or a flat vector mascot.
```

Ba câu quan trọng nhất trong khối trên:

- **"EXACTLY 5 colours"** — ràng buộc đếm được. Bảng màu lan man là dấu hiệu AI rõ nhất, và một con số thì công cụ tuân theo được.
- **"outline is a dark version of the colour it encloses"** — mẹo thật của người vẽ. Viền đen tuyền khắp nơi làm hình bị bẹt và chết.
- **"light from the UPPER LEFT, consistently"** — AI đổi hướng sáng theo từng khung. Tám khung khác hướng sáng là thứ mắt thấy ngay mà không gọi tên được.

---

## 3. Ràng buộc quyết định: phải đọc được ở 50px

Lính trong game sẽ cao khoảng **50 điểm ảnh** trên màn.

```
SIZE — this is the most important constraint of all:

This character will be shown about 50 pixels tall on a phone screen,
with twenty others around it. Design for THAT size, not for this canvas.

* Before adding any detail, ask: would it still be visible at 50px?
  If not, leave it out. An empty shape beats a muddy one.
* The character must be recognisable from its SILHOUETTE in solid
  black. Test it mentally: fill the whole figure with one colour —
  can you still tell what it is and which way it faces?
* Give it ONE strong identifying feature that survives shrinking:
  a distinctive hat, a big weapon, an unusual body shape.
* Faces: two dots and a simple mouth. Nothing more. Eyebrows, noses,
  teeth and pupils all disappear at 50px and only add mud.
```

---

## 4. Tấm thử — `dead_common`, tám khung đi bộ

Chọn con này vì nó xuất hiện nhiều nhất và đang là con vừa ô nhất.

> Prompt:
>
> Draw one game character in 8 poses of a WALK CYCLE, arranged as ONE
> horizontal row of 8 figures, on a fully transparent background.
>
> THE CHARACTER: a small, friendly cartoon skeleton soldier. Bone-white
> body, a dented iron pot helmet, a round wooden shield on the left arm,
> a short sword in the right hand. Cute and a bit clumsy, never scary.
> Its five colours are: bone white, warm grey (helmet and sword), medium
> brown (shield), dark teal (eye sockets and shadow), and one small
> accent of dull red (a cloth band).
>
> [DÁN KHỐI STYLE Ở MỤC 2]
>
> [DÁN KHỐI SIZE Ở MỤC 3]
>
> THE SEQUENCE: the 8 figures form a smooth, loopable walk — figure 8
> must lead naturally back into figure 1. Side view, facing RIGHT,
> slightly three-quarter. The body bobs up and down across the cycle,
> but the FEET all rest on the same ground line: the lowest point of
> every figure is at the same height.
>
> LAYOUT: at least 80 pixels of empty transparent space between
> neighbouring figures — nothing may touch, including the sword and
> shield. At least 60 pixels of transparent margin around the whole row.
> Every figure drawn COMPLETE, nothing cropped at any edge.
>
> TRUE TRANSPARENT alpha background — no magenta, no white, no
> checkerboard. The alpha must be ANTI-ALIASED, fading over about one
> pixel; do not threshold or posterise it.
>
> No text, no labels, no numbers, no watermark, no ground shadow.

Đặt file: `artsrc/units_v2/dead_common_walk.png`

---

## 5. Tôi sẽ kiểm bằng gì

Không nhìn bằng cảm tính. Bốn phép đo, và **tấm nào trượt thì sửa prompt chứ không làm tiếp 19 con**:

| phép đo | đạt khi |
|---|---|
| Tách được 8 cụm rời | đúng 8, không dính nhau |
| **Số màu riêng biệt** | ≈ 5–7 (kể cả viền và khử răng cưa) |
| **Tỉ lệ điểm ảnh là viền sắc** | 6–11%, như vùng có lính của game tham chiếu |
| **Chân cùng một mặt đất** | lệch dưới 1% chiều cao |

Rồi hai phép kiểm bằng mắt:

1. **Thu xuống đúng 50px** và nhìn. Không đọc ra hình gì thì prompt còn sai.
2. **Đổ đặc một màu** thành bóng hình đen. Không nhận ra là con gì, quay hướng nào, thì bóng hình chưa đạt — và đó là lỗi nặng nhất ở cỡ nhỏ.

Cả bốn số và hai phép nhìn đều đạt mới làm 19 con còn lại.

---

## 6. Nếu muốn làm lại bộ UI cho đồng bộ

Bộ UI phẳng hiện tại đã nhập và dùng được. Nhưng nếu nhân vật v2 ra theo
lối "viền cùng tông, 5 màu" thì bộ UI nên theo cùng luật — viền của nút
vàng là nâu sẫm, không phải đen tuyền.

Chờ xem nhân vật thử ra sao rồi quyết, đừng làm song song: hai bộ sinh
riêng rẽ theo hai lượt prompt khác nhau thì gần như chắc chắn lệch tông.
