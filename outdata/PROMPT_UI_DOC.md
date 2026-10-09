# Yêu cầu art — bộ UI màn dọc (1125×2436)

Game đã chuyển hẳn sang màn dọc iPhone X. Bộ UI hiện tại được vẽ cho màn
ngang rồi kéo sang, nên tỉ lệ sai chỗ và thiếu hẳn mấy mảnh.

**Đã có, không cần vẽ lại:** `panel`, `button`, `button_down`, `grid_cell`,
`bar_bg`, `bar_fill`, `stat_frame`, `reel_slot`, `reel_slot_win`,
`tile_ally`, `tile_enemy`, 4 icon buff (tim / kiếm / sét / bia).

---

## Nguyên tắc chung — áp cho MỌI tấm dưới đây

Bốn điều này là lý do đợt trước phải nhập lại ba lần. Đọc kỹ.

1. **Nền alpha trong suốt thật.** Không nền magenta, không nền trắng,
   không nền kẻ ô. Magenta là phép mất mát: điểm rìa khử răng cưa chính
   là màu trộn giữa magenta và hình, tách ra luôn còn quầng hồng.
2. **Alpha phải khử răng cưa** — viền mờ dần qua khoảng một điểm ảnh.
   Đừng ngưỡng hoá, đừng posterise. Đợt trước nhận về alpha chỉ có 2 mức,
   viền răng cưa thấy rõ trên máy thật.
3. **Không cắt cụt.** Mỗi hình vẽ TRỌN VẸN, chừa lề trong suốt ít nhất
   40px quanh. Không đóng khung, không kẻ ô, không viền ngoài.
4. **Không chữ, không nhãn, không watermark, không bóng đổ xuống nền.**

**Phong cách:** khớp bộ hiện có — hoạt hoạ 2D, viền đen đậm đều nét,
tô khối phẳng có chuyển sáng nhẹ, bảng màu gỗ–đá–vàng đồng. Nhìn như
game mobile casual, không phải pixel art, không phải tả thực.

---

## Tấm 1 — Tài nguyên mới (bắt buộc)

Một hàng ngang 4 hình, nền trong suốt, mỗi hình **cao khoảng 280px**,
cách nhau ít nhất 80px trong suốt.

| thứ tự | hình | ghi chú |
|---|---|---|
| 1 | **Túi vàng** — tiền chính, giữ qua các ván | đống tiền vàng tràn miệng túi vải, dây buộc. Khác hẳn đồng xu đơn `icon_coin` đang có — xu là tiền trong một ván, túi là tiền lâu dài |
| 2 | **Chìa khoá** — mở rương | chìa cổ, cán hình vòng, răng chìa rõ, ánh kim vàng |
| 3 | **Rương đóng** | rương gỗ đai sắt, khoá vàng ở giữa, nắp đóng |
| 4 | **Rương mở** | CÙNG cái rương đó, nắp bật, ánh sáng vàng hắt ra, vài đồng tiền và tia sáng |

> Hình 3 và 4 phải là **cùng một cái rương**, cùng góc nhìn, cùng cỡ,
> cùng vị trí thân rương — chỉ khác cái nắp và ánh sáng. Chúng thay nhau
> tại chỗ khi người chơi mở, lệch một chút là lúc đổi nó nhảy.

> Prompt:
>
> A horizontal row of four separate 2D cartoon game icons on a fully
> transparent background: (1) a bulging cloth coin pouch overflowing with
> gold coins, drawstring tied; (2) an ornate golden key with a ring-shaped
> bow and clear teeth; (3) a closed wooden treasure chest with iron bands
> and a gold lock; (4) THE SAME chest, lid open, warm golden light and a
> few coins and sparkles spilling out.
>
> Items 3 and 4 must be the identical chest at the identical angle, size
> and body position — only the lid and the light differ.
>
> Style: bold even black outline, flat shading with soft gradients, warm
> wood / iron / gold palette, mobile casual game art. Not pixel art, not
> photorealistic.
>
> Each icon drawn COMPLETE with at least 40px of empty transparent margin
> around it; leave at least 80px of transparent space between neighbours.
> No cells, no frames, no boxes, no ground shadow.
>
> TRUE TRANSPARENT alpha background — no magenta, no white, no checkerboard.
> The alpha channel must be ANTI-ALIASED: outlines fade over about one
> pixel. Do not threshold or posterise the transparency.
>
> No text, no labels, no watermark.

Đặt file: `artsrc/ui/resources.png`

---

## Tấm 2 — Thanh tài nguyên trên đỉnh màn

Màn dọc rộng 1125px. Đỉnh màn cần một thanh ngang chứa ba ô số: vàng,
chìa, và một ô trống để sau này thêm. Bộ hiện có chỉ có `panel` (khung 9
mảnh dài) — dùng tạm được nhưng nó là khung chữ, không phải ô đựng số có
icon.

Vẽ **một khung 9 mảnh** cỡ **384×128px**, hình viên thuốc bo tròn, nền
gỗ sẫm, hai đầu bịt nắp kim loại trang trí.

> **Quan trọng — đây là khung 9 mảnh:** phần GIỮA phải là hoạ tiết lặp
> được theo chiều ngang (gỗ trơn hoặc vân dọc đều), vì nó sẽ bị kéo giãn.
> Hai nắp đầu rộng đúng 48px mỗi bên và KHÔNG bị kéo. Hoạ tiết ở giữa
> không được có chi tiết bất đối xứng (đinh tán lệch, vết xước riêng lẻ) —
> kéo giãn là nó nhoè thành vệt.

> Prompt:
>
> A single horizontal 9-slice UI frame for a mobile game, 384x128 pixels,
> fully transparent background. Rounded pill shape: dark polished wood
> centre panel, decorative metal end caps exactly 48 pixels wide on the
> left and right.
>
> CRITICAL for 9-slice tiling: the centre section must be a uniform,
> horizontally repeatable texture — plain wood or even vertical grain.
> No rivets, scratches, highlights or any asymmetric detail in the centre,
> because that region gets stretched. All decoration belongs in the two
> end caps.
>
> Style: bold even black outline, flat shading with soft gradients, warm
> wood and aged brass palette, mobile casual game art.
>
> Draw the frame COMPLETE with at least 40px of empty transparent margin
> around it. TRUE TRANSPARENT alpha background — no magenta, no white.
> The alpha must be ANTI-ALIASED, fading over about one pixel; do not
> threshold or posterise it.
>
> No text, no numbers, no labels, no watermark, no drop shadow.

Đặt file: `artsrc/ui/bar_resource.png`

---

## Tấm 3 — Nút rương và nút treasure

Hai nút tròn, mỗi cái **256×256px**, một hàng ngang, cách nhau 80px.

1. **Nút rương** — nền tròn gỗ viền kim loại, giữa là cái rương đóng nhỏ
2. **Cùng nút đó ở trạng thái BẤM XUỐNG** — tối hơn, viền trong lõm vào

> Hai cái phải **trùng khít** từng điểm ảnh về bố cục. Chúng thay nhau
> tại chỗ lúc bấm; lệch một chút là nút giật.

> Prompt:
>
> Two round mobile-game buttons side by side on a fully transparent
> background, each 256x256 pixels, 80 pixels of transparent space between
> them.
>
> Button 1: a circular wooden button with a metal rim, a small closed
> treasure chest in the centre.
> Button 2: the EXACT same button in its pressed state — darker, the inner
> bevel inverted so it reads as pushed in.
>
> The two buttons must align pixel for pixel: identical size, identical
> centre, identical chest position. Only the shading changes.
>
> Style: bold even black outline, flat shading with soft gradients, warm
> wood and brass palette, mobile casual game art.
>
> At least 40px of empty transparent margin around each. TRUE TRANSPARENT
> alpha background — no magenta, no white. The alpha must be ANTI-ALIASED,
> fading over about one pixel; do not threshold or posterise it.
>
> No text, no labels, no watermark, no drop shadow.

Đặt file: `artsrc/ui/button_treasure.png`

---

## Sau khi có file

Báo tôi, tôi viết tool cắt và nhập. Tool sẽ:
- tách bằng **gán nhãn liên thông** (không cắt theo lưới cứng — lưới cứng
  là thứ đã làm đuôi xe tăng bị xén phẳng đợt trước)
- cân bốn icon tài nguyên về cùng độ nặng bằng `sqrt(diện tích mực)`,
  giống cách đã làm cho 4 icon buff
- ép cặp rương-đóng / rương-mở và cặp nút thường / nút-bấm về **chung một
  khung**, vì chúng thay nhau tại chỗ
- đo lại và in số trước khi ghi, để bạn duyệt
