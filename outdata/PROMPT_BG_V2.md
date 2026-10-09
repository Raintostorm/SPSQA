# Prompt sinh nền trận — có sẵn con đường

Không làm sprite con đường rời. Vẽ **con đường vào luôn trong ảnh nền** —
ít một asset, và đường hoà vào cảnh thay vì dán lên trên.

Đo từ camera thật: `ortho 3.26`, màn `1920×1080`, `166 px` mỗi đơn vị.

```
+-------------------------------------------------- 1920 ---+
|                                                            |  0
|   chan troi / nha cua / nui — CHI TIET o day               |
|                                                            |
+--- 209 ------------------------------------------------- --+
|                                                            |
|   VUNG LUOI 4x4 hien luc chuan bi — PHAI YEN TINH          |
|                                                            |
+--- 623 -------- MAT DUONG, linh di tren day -------------- +
|                                                            |
|   tien canh — co the co chi tiet nhe                       |
+--------------------------------------------------------1080+
```

| Vùng | px theo chiều dọc | Yêu cầu |
|---|---|---|
| Trời / xa | 0 – 209 | được phép chi tiết |
| Vùng lưới | 209 – 871 | **phải yên tĩnh**, lưới và lính nằm đè lên |
| Mặt đường | **~560 – 690** | dải đường chạy ngang hết bề ngang |
| Tiền cảnh | 871 – 1080 | chi tiết nhẹ |

Mặt đường nằm **giữa** vùng lưới, nên nó phải **nhạt và phẳng** — chỉ đủ để
người chơi hiểu "lính đi trên đây", không được tranh nhìn với lính.

---

## Prompt — dán vào công cụ sinh ảnh

> A 2D game battle background, landscape 16:9, 1920x1080 pixels.
>
> **Composition:** a horizontal dirt road or paved path crossing the entire
> width of the image, centred around 58% of the image height. The road is
> flat, plain and low-contrast — a simple band, no cobblestones picked out,
> no strong texture, no cracks, no debris. Everything from 20% to 80% of the
> height must stay visually quiet; the characters fight there.
>
> **Style:** flat vector illustration, soft gradients, NO black outlines
> anywhere. Muted and desaturated — a backdrop, not the subject.
>
> **Lighting:** slightly darker along the road band so bright characters
> standing on it pop out. Brighter near the top and bottom edges.
>
> **Palette:** cool muted blues and dusty greys, low saturation. Avoid
> saturated reds, oranges and greens — the characters use those.
>
> **Setting:** SEE THE TABLE BELOW.
>
> No characters, no creatures, no weapons, no UI, no text, no watermark,
> no borders, no vignette frame.

## Bốn thời — chỉ thay đoạn Setting

| Thời | Setting |
|---|---|
| StoneAge | `a bare dirt track across a prehistoric valley, cracked earth, scattered boulders, distant volcanic hills in haze` |
| Medieval | `a packed earth road across a trampled field, faded banners far off, a stone castle wall barely visible through haze` |
| Modern | `a cracked asphalt road with faded centre line, distant grey city blocks in fog` |
| Future | `a smooth metallic walkway with faint glowing seams, distant dark skyline with soft cyan lights` |

Thời Future vẫn giữ **ít sáng** — đèn xanh trong nền sẽ đấu với tia laser của lính Future.

---

## Ba thứ hậu kỳ không cứu được

1. **Sai tỉ lệ khung** — phải đúng 16:9.
2. **Có nét viền đen** — lính chìm vào nền, không tách ra được bằng chỉnh màu.
3. **Đường quá nhiều chi tiết** — cắt không được, làm mờ thì thành vết bẩn.

## Lưu ở đâu

```
artsrc/bg/bg_stoneage.png
artsrc/bg/bg_medieval.png
artsrc/bg/bg_modern.png
artsrc/bg/bg_future.png
```

Xong nhắn tôi. Tôi chạy `tools/check_bg.py` đo tỉ lệ, độ tối, độ rối ở dải
giữa, rồi ghép thử với lính thật trước khi đưa vào `Assets/`.
