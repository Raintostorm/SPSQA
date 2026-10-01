# 10 — Pipeline art 2D bằng AI

> Chốt 2026-09-30, sau khi thử 4 công cụ.
> Quy trình này dùng lại cho **cả 20 nhân vật**.

---

## 1. Công cụ: Gemini image **editing** (không phải text-to-image)

| Công cụ | Kết quả | |
|---|---|---|
| ChatGPT / DALL-E | ra nhân vật KHÁC hẳn | ❌ |
| Leonardo (Auto model) | ra nhân vật KHÁC hẳn | ❌ |
| ComfyUI local | chưa thử, là phương án dự phòng | — |
| **Gemini editing** | **giữ nhân vật, lệch 0px** | ✅ |

### Vì sao chỉ Gemini editing chạy được

| Cơ chế | Kết quả |
|---|---|
| Text-to-image | model "hiểu" ảnh rồi **vẽ mới** → mỗi lần một nhân vật khác |
| **Image editing** | model **sửa tại chỗ** → thứ không được yêu cầu đổi thì giữ nguyên pixel |

Đo thực tế giữa 2 lần sinh độc lập: chiều cao 733 vs 733, đáy chân y=888
vs y=888 — **lệch 0 pixel**.

### Hai luật bắt buộc

1. **Mỗi frame bắt đầu lại từ ảnh GỐC.** Không nối tiếp từ ảnh vừa sinh —
   sai lệch và đổ bóng sẽ cộng dồn qua từng frame.
2. **KHÔNG sinh hàng loạt / không yêu cầu sprite sheet.** Bảo nó tạo sheet
   là chuyển cơ chế từ *sửa* sang *vẽ bố cục mới*, mất luôn độ chính xác.
   Một lần gửi = một frame.

---

## 2. Khuôn prompt

Phần đầu và cuối GIỮ NGUYÊN tuyệt đối. Chỉ dòng POSE thay đổi.

```
Edit this image. Keep the character EXACTLY as it is — identical colors,
identical green hood, identical pointed ear, identical dagger, identical
outline thickness, identical proportions and size.

Change ONLY the pose: «««POSE»»»

Both legs clearly separated with a visible gap. The character must stay
the same height and stand on the same ground line.

Use completely FLAT solid colors with NO shading, NO highlights, NO
gradients, NO glossy reflections. Every color area must be one single
uniform tone.

Do not add any sparkles, stars, particles, glows, text, letters or
decorations anywhere in the image.

Replace the background with solid flat magenta #FF00FF.
Remove the ground shadow completely.
Output a square image with the character centered.
```

Đổi `green hood / pointed ear / dagger` theo từng nhân vật.

### Vì sao từng dòng tồn tại

| Dòng | Chặn lỗi gì |
|---|---|
| `FLAT solid colors, NO shading` | Gemini tự thêm vệt sáng → frame này có, frame kia không → animation nhấp nháy |
| `no sparkles, stars, text` | nó hay thêm ngôi sao lấp lánh và chữ rác |
| `magenta #FF00FF` | màu không có trong bảng màu nhân vật → tách nền sạch. KHÔNG dùng trắng (mặt nhân vật màu kem) |
| `Remove the ground shadow` | bóng nằm trong sprite sẽ lao theo nhân vật khi đánh → sai vật lý |
| `same height, same ground line` | lệch = đổi animation là nhân vật nhảy chỗ |

---

## 3. Danh sách POSE

### Walk — 8 frame

| # | POSE |
|---|---|
| 0 | `left leg forward with heel down, right leg back on the toe, body at mid height` |
| 1 | `the front leg is planted flat under the body carrying all the weight, the back leg is lifting off behind on its toe. The body is at its LOWEST point, knees bent.` |
| 2 | `the back leg swings forward and passes right beside the planted leg, nearly together. The body is rising from its lowest point.` |
| 3 | `the swinging leg reaches forward ahead of the body while the other leg pushes off behind on its toe. The body is at its HIGHEST point, legs almost straight.` |
| 4 | `the OPPOSITE stride from the original pose — the legs have SWAPPED. The leg that was behind is now forward with its heel down, and the leg that was forward is now back on its toe. Same stride width as before.` |
| 5 | *(giống frame 1)* |
| 6 | *(giống frame 2)* |
| 7 | *(giống frame 3)* |

**Frame 4 (`Walk_04`) là frame duy nhất có thể làm hỏng cả clip.**
Sinh xong đặt cạnh `Walk_00` và kiểm: chân nào ra trước? Cùng một chân →
sinh lại trước khi chạy tiếp. Nửa sau chu kỳ chỉ đúng nếu frame này đúng.

### Attack — 7 frame

| # | POSE |
|---|---|
| 0 | `neutral standing stance, weapon held at rest` |
| 1 | `pulling the weapon back, weight shifting onto the back foot` |
| 2 | `weapon fully cocked back behind the shoulder, body coiled and leaning back` |
| 3 | `lunging forward with the weapon fully extended at maximum reach — the most dramatic pose` |
| 4 | `weapon following through past the target, arm still extended` |
| 5 | `weapon dropping low, body settling back` |
| 6 | `back to the neutral standing stance, identical to the first frame` |

Frame cuối = frame đầu để clip nối về Idle không giật.

### Hit — 3 frame

| # | POSE |
|---|---|
| 0 | `head snapped back, body recoiling backward, arms flung out` |
| 1 | `staggering one step back, off balance` |
| 2 | `almost back to neutral stance` |

### Dead — 6 frame

| # | POSE |
|---|---|
| 0 | `body jolting from a strike, head back` |
| 1 | `knees buckling, body starting to drop` |
| 2 | `tipping backward, weapon slipping from the hand` |
| 3 | `body low near the ground, weapon separated and falling` |
| 4 | `LYING FLAT on the ground, horizontal` |
| 5 | `LYING FLAT on the ground, completely still, weapon on the ground beside the body` |

Viết hoa `LYING FLAT` vì model rất hay vẽ nhân vật vẫn đứng khi nghe "dead".

### Idle — 1 frame

Dùng luôn ảnh tham chiếu gốc. KHÔNG cần nhiều frame.
Bản cũ dùng 6 bản vẽ Idle không liên quan nhau → nhân vật rung như co giật.

---

## 4. Đặt tên file

```
Art/Characters/<TenNhanVat>/
├── Idle/   Idle_00.png
├── Walk/   Walk_00.png … Walk_07.png
├── Attack/ Attack_00.png … Attack_06.png
├── Hit/    Hit_00.png … Hit_02.png
└── Dead/   Dead_00.png … Dead_05.png
```

**Hai chữ số, có số 0 đứng đầu.** Code sắp theo thứ tự chuỗi:
`Walk_10` đứng TRƯỚC `Walk_2` nếu thiếu số 0.

Tải file **PNG**, đừng lấy WebP — nén mất mát làm magenta lem vào viền đen,
script tách nền sẽ để lại rìa tím quanh người.

---

## 5. Hậu xử lý (script Python)

Ảnh Gemini xuất ra chưa dùng được: 1024×1024, nền magenta, có rác.
Script làm 4 việc:

| | |
|---|---|
| **Giữ mảng lớn nhất** | quét connected-component, vứt mọi mảng rời. Ngôi sao rác (707px) và pixel lạc tự biến mất — không cần cãi nhau với prompt |
| **Tách nền** | magenta → alpha. Ngưỡng rộng: đo thực tế ra `(251,35,243)` và `(240,9,251)`, không phải `(255,0,255)` thuần |
| **Dò đáy chân** | pixel không trong suốt thấp nhất |
| **Chuẩn hoá** | resize sao cho nhân vật cao 180px, đặt vào canvas **320×320**, đáy chân tại **y=60**, đệm ≥16px |

Bước cuối quyết định pipeline sống hay chết — nó đảm bảo **mọi frame cùng
pivot**, nên đổi animation nhân vật không nhảy chỗ.

---

## 6. Chi phí

```
Idle 1 + Walk 8 + Attack 7 + Hit 3 + Dead 6 = 25 lần sửa / nhân vật
× 20 nhân vật = 500 lần
```

Nên **kiểm chứng pipeline Unity với 1 nhân vật trước**, rồi mới nhân lên.

---

## 7. Cách tự kiểm mỗi frame

1. **Cùng nhân vật không?** mũ cùng dáng, tai cùng màu, vũ khí cùng kiểu
2. **Hai chân có khe hở không?** che phần trên, nhìn từ thắt lưng xuống
3. **Có bị thêm đổ bóng không?** vệt sáng trên mũ là dấu hiệu
4. **Đo bằng script**: chiều cao và đáy chân lệch quá vài pixel → sinh lại
