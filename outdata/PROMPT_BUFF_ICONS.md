# Yêu cầu art — Bốn icon chỉ số buff

Thanh buff đang là chữ: `MÁU +0 %  SÁT THƯƠNG +0 %  TỐC ĐÁNH +0 %  CHÍ MẠNG +0 %`.
Bốn cụm chữ dài, đọc chậm, và tràn sát hai mép trên màn dọc 1125 đơn vị.
Đổi thành icon + số: `♥ +25%   ⚔ +10%   ⚡ +5%   ◈ +2%`.

---

## 1. Cần gì

Một tấm **1200 × 320**, **nền trong suốt** (PNG có alpha thật, KHÔNG dùng
magenta), chứa **4 icon** xếp một hàng ngang, mỗi icon khoảng **240 × 240**.

| thứ tự | chỉ số | ý nghĩa | gợi ý hình |
|---|---|---|---|
| 1 | `HpPct` | MÁU | trái tim đỏ |
| 2 | `DmgPct` | SÁT THƯƠNG | thanh kiếm, hoặc hai kiếm bắt chéo |
| 3 | `AtkSpeedPct` | TỐC ĐÁNH | tia sét |
| 4 | `CritChance` | CHÍ MẠNG | tâm bia có mũi tên trúng hồng tâm |

Thứ tự trong tấm **phải đúng như bảng** — tôi cắt theo thứ tự trái sang phải.

---

## 2. Ba ràng buộc

**a. Bốn icon cùng khung, cùng cỡ.** Chúng nằm cạnh nhau trên một thanh;
lệch cỡ là nhìn ra ngay. Vẽ mỗi icon lọt trong một ô vuông tưởng tượng
240×240, chạm mép trên và mép dưới của ô đó.

**b. Đọc được ở 40px.** Trên màn thật mỗi icon chỉ cao chừng 40–48 điểm ảnh.
Hình phải nhận ra ở cỡ đó: khối lớn, viền dày, **không chi tiết nhỏ**, không
hoa văn bên trong, không chữ.

**c. Mỗi icon một màu chủ đạo khác hẳn nhau** — đỏ, bạc/cam, vàng, lục. Màu
là thứ mắt bắt trước hình khi icon bé.

---

## 3. Prompt — dán vào công cụ sinh ảnh

Đính kèm `artsrc/ui/ui_sheet.png` làm tham chiếu phong cách.

> One image, 1200 × 320 pixels, **transparent background — a PNG with a true
> alpha channel. Do NOT fill the background with any colour.** The alpha must
> be ANTI-ALIASED: edges fade smoothly over about one pixel, never a hard
> on/off cutout, never posterised.
>
> Four game UI icons in a single horizontal row, evenly spaced, each about
> 240 × 240 pixels, with clear transparent space between them and a margin
> around the whole row. All four must share the same visual weight and fill
> their square the same amount.
>
> Left to right:
> 1. A bold red HEART.
> 2. A single upright SWORD with a golden hilt, or two crossed swords.
> 3. A bright yellow LIGHTNING BOLT.
> 4. A round TARGET with an arrow struck in the bullseye, green and white.
>
> Art style: flat cartoon game UI, THICK dark outlines, bold simple shapes,
> soft cel shading, strongly saturated colours, light from the upper left —
> matching the attached UI reference.
>
> These are displayed very small, about 44 pixels tall. Keep each shape big
> and simple: no fine detail, no inner patterns, no gradients with thin
> features, no text, no numbers, no frames, no drop shadows.

---

## 4. Ba thứ hậu kỳ không cứu được

1. **Bốn icon khác cỡ nhau.** Chúng đứng cạnh nhau trên một thanh.
2. **Chi tiết nhỏ.** Ở 44px thì chi tiết thành một vệt nhiễu.
3. **Hai icon cùng tông màu.** Mất luôn tác dụng phân biệt nhanh.

Nền **trong suốt**, không magenta — công cụ xuất alpha thật được, mà tách
magenta là phép mất mát: rìa khử răng cưa chính là màu trộn với nền.

---

## 5. Đặt file ở đâu

```
artsrc/ui/buff_icons.png
```

---

## 6. Xong rồi tôi làm gì

```
1. tach 4 icon bang phep dan nhan cum, ep ve CUNG mot khung
2. doi thanh buff tu chu sang icon + so
3. render thu gui ban xem
```
