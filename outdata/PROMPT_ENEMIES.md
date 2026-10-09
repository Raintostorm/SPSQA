# Yêu cầu art — Ba con địch đầu tiên

Đợt 7. Địch tách hẳn khỏi phe ta, dùng chung bộ xương và chung animation.

---

## 1. Vì sao lính hiện tại "trông tù"

Mở ảnh thân ra xem thì rõ: **thân là một quả trứng trơn**, không tay,
không chân. Cả 16 con dùng **đúng một đường viền**, chỉ khác hoạ tiết
quần áo vẽ lên trứng, cộng cái đầu và món vũ khí.

Hình bóng là thứ mắt đọc trước tiên. Mười sáu hình bóng giống hệt nhau
thì không màu nào, chi tiết nào cứu được.

**Nhưng thân chỉ là một sprite có điểm xoay ở chân — nó KHÔNG bắt buộc
phải là quả trứng.** Đó là chỗ để phá.

---

## 2. Ba slot, và hai điểm nối bắt buộc

Xem `outdata/layout_enemy_slots.png`.

| slot | khung | điểm xoay |
|---|---|---|
| BODY | **131 × 165** | 0.5 , 0.0 — bàn chân |
| HEAD | **131 × 135** | 0.5 , 0.0 — cổ |
| WEAPON cận chiến | **97 × 169** | 0.5 , 0.04 — chỗ cầm |
| WEAPON súng | **154 × 154** | 0.35 , 0.5 — chỗ cầm |

Trên khung thân 131 × 165, hai điểm này **không được xê dịch**:

```
CO   : chinh giua,  y = 115 px tinh tu DAY khung
TAY  : x = 107 px tu trai,  y = 68 px tu DAY khung
```

Toạ độ này lấy thẳng từ prefab, không phải ước lượng. Lệch là lúc đứng
thở cái đầu rời khỏi thân — lỗi im lặng, không có cảnh báo nào.

**Thân vẽ KHÔNG CÓ ĐẦU.** Đầu là ảnh riêng dán đè lên.

**Vũ khí không dính vào cánh tay nào cả.** Rig không có sprite tay — vũ
khí lơ lửng ở chấm xanh rồi vung quanh đó. Nên đừng vẽ tay cầm.

Đầu **được phép to hơn đầu người**, miễn cổ đúng chỗ. Đầu to thân nhỏ đọc
ra "không phải người" ngay.

---

## 3. Ba con

| | loại đánh | clip | ghi chú |
|---|---|---|---|
| 1. Bộ xương | cận chiến | `Attack` | kiếm xương |
| 2. Quái đầu gấu | cận chiến | `Attack` | vuốt thay vũ khí |
| 3. Người áo đen | đánh xa | `AttackGun` | súng, khung 154×154 |

**Ba hình bóng phải khác nhau rõ rệt.** Phép kiểm tôi sẽ chạy: tô đen cả
con thành một vệt. Không phân biệt được thì chưa đạt, dù vẽ đẹp tới đâu.

Nên: bộ xương **cao, gầy nhom, sọ to**. Gấu **đồ sộ, gù vai, bè ngang**.
Người áo đen **tầm thường** — nó là mốc để hai con kia tương phản.

---

## 4. Prompt — dán vào công cụ sinh ảnh

Đính kèm `outdata/layout_enemy_slots.png` và một ảnh lính hiện có làm
tham chiếu phong cách.

> **Một tấm 1920 × 2400, nền magenta đặc #FF00FF, chia lưới 3 cột × 3 hàng,
> mỗi ô 640 × 800. Chín món, mỗi món nằm giữa một ô, không chạm nhau.**
>
> Cartoon game sprites, side view facing RIGHT. Flat cartoon style with THICK
> black outlines, bold simple shapes, soft cel shading, saturated colours —
> the same style as the attached reference.
>
> Column 1 = SKELETON. Column 2 = BEAR BEAST. Column 3 = BLACK-CLAD GUNMAN.
> Row 1 = BODY. Row 2 = HEAD. Row 3 = WEAPON.
>
> **Row 1, the BODIES — each is a torso with NO HEAD and NO WEAPON.** The neck
> ends in a clean flat stump at the top. Draw no arms holding anything. Each
> body must have its OWN distinct outline — do not make them all the same egg
> or oval shape; the silhouettes must be told apart as pure black shapes.
> 1. Skeleton body: a tall, narrow, gaunt ribcage and spine, bone white, gaps
>    between the ribs, hip bones flaring at the bottom.
> 2. Bear body: a massive, barrel-chested, hunched mound of brown fur, much
>    wider than tall-looking, shaggy fur spikes breaking the outline.
> 3. Gunman body: an ordinary human torso in a black tactical coat with straps
>    and buckles, narrow and upright.
>
> **Row 2, the HEADS — each head alone, neck at the bottom centre of its cell.**
> 1. A bare skull, slightly oversized, empty black eye sockets with two small
>    glowing dots, jaw slightly open.
> 2. A big brown bear head with a broad snout, small angry eyes, open mouth
>    showing fangs — clearly an animal, oversized relative to a human head.
> 3. A human head almost fully hidden: black hood and a dark mask covering the
>    mouth, only a narrow strip showing two fierce glowing eyes.
>
> **Row 3, the WEAPONS — each object alone, grip at the BOTTOM of its cell,
> blade or barrel pointing UP.**
> 1. A crude sword carved from bone, pale ivory, jagged edge, wrapped grip.
> 2. A single huge bear paw with four long curved claws, brown fur, seen from
>    the side, wrist at the bottom. It is a weapon object, not an arm.
> 3. A compact black submachine gun, held level, dark metal with a few grey
>    highlights.
>
> All nine items must be drawn at a CONSISTENT scale relative to one another,
> as if from the same game.
>
> Solid magenta #FF00FF background. No magenta, pink or purple in the artwork
> itself. No text, no numbers, no frames, no drop shadows, no ground, no
> watermark. Nothing may touch the edge of its cell.

---

## 5. Ba thứ hậu kỳ không cứu được

1. **Thân vẽ kèm đầu.** Đầu là slot riêng; thân có đầu thì ra hai đầu.
2. **Ba thân cùng một hình bóng.** Đó đúng là lỗi đang muốn thoát.
3. **Ba con khác tỉ lệ nhau.** Tôi ép được về cùng khung, nhưng ép thì con
   vẽ to bị bóp méo.

Nền magenta cứ để nguyên — tôi tách được cả rìa đã khử răng cưa.

---

## 6. Đặt file ở đâu

```
artsrc/enemies/enemies_ai.png
```

---

## 7. Xong rồi tôi làm gì

```
1. tach nen magenta, cat 9 o
2. ep ve dung ba khung 131x165 / 131x135 / 97x169 va 154x154,
   neo CO va TAY vao dung toa do  (normalize_slot.py da co san phep nay)
3. chay phep kiem hinh bong: to den ca ba con, xem co phan biet duoc khong
4. tao 3 UnitDefinition moi + mot UnitPool RIENG cho dich
5. doi BattleSetup lay dich tu pool do, giu RollRewardPlacer va
   EvolutionSystem o pool phe ta
6. render thu ca man hinh gui ban xem truoc khi dung vao Unity
```
