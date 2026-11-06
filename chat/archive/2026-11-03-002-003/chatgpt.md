# Yêu cầu gửi ChatGPT

Claude viết prompt ở đây. Bạn copy khối `prompt` dán vào ChatGPT, rồi lưu ảnh trả về vào `chat/check/` với tên `<số>-<tên>.png` (ví dụ `002-back-button.png`). Ảnh gốc PNG nét hơn ảnh dán qua chat.

Trạng thái: `CHỜ ẢNH` → `ĐANG XEM` → `CHỐT` hoặc `SỬA` (Claude ghi lý do trong `claude.md`).

Luật dọn chat: khi một yêu cầu đã xong (có output, đã CHỐT và đã cắt/gắn), XOÁ mục đó khỏi `chatgpt.md` và `claude.md` để lượt sau bắt đầu gọn. Quyết định lâu dài ghi ở `doc/UI_PLAN.md` hoặc README trong `artsrc/ui/<tên>-vN/` trước khi xoá. Ảnh trong `check/` giữ lại.

---

## 002 · Bộ nút chung (UI-006) · CHỜ ẢNH
Lưu ảnh: `check/002-buttons.png`

```prompt
Create a mobile game UI button sheet for a 2D chibi fantasy auto-battler. Match this exact style: flat bright colors, thick black outline (8 px), a glossy light band across the top third, a darker "lip" along the bottom edge that makes the button look raised and chunky, slightly rounded rectangle corners (radius about 30% of the height). Cartoon look like Brawl Stars / Clash Royale buttons. No text, no icons, no letters on any button.

Layout: a grid with 4 rows and 3 columns on a fully transparent background, lots of empty space between items.
Rows (button color):
1. Gold / yellow (#FFC21A main, #E07B00 bottom lip) — primary action
2. Sky blue (#2EA8F5 main, #0A5FC0 bottom lip) — secondary action
3. Dark slate navy (#2B3A55 main, #141C2B bottom lip) — back / cancel
4. Red (#F2444F main, #A3122A bottom lip) — danger
Columns (state):
1. Normal
2. Pressed: the same button pushed down — bottom lip almost gone, whole button 6 px lower, top highlight dimmer
3. Disabled: desaturated grey version of the same shape, no glow

Requirements:
- Every button exactly the same size: 600 px wide, 170 px tall, perfectly horizontal and symmetric.
- The left and right 25% of each button contain the rounded corners; the middle 50% is a plain, evenly colored area with no decoration so it can be stretched horizontally (9-slice).
- Transparent PNG background, no drop shadow outside the outline, no outer glow.
```

## 003 · Summon: nút triệu hồi + khung thẻ kết quả · CHỜ ẢNH
Lưu ảnh: `check/003-summon-kit.png`

```prompt
Create a UI asset sheet for the "Summon" screen of a mobile 2D chibi fantasy game (witch fortune-teller tent theme, purple and gold, magical). Style: flat colors, thick black outline (8 px), soft top highlight, simple cel shading, cartoon like Brawl Stars. Fully transparent background, each item separated with plenty of empty space. No text and no numbers anywhere.

Items:
1. Two large summon buttons side by side, each 520 x 230 px, chunky with a darker bottom lip:
   - left: magical blue (#2EA8F5) with a small glowing crystal ball emblem on the left third
   - right: royal purple (#7B3FE4) with gold trim and a cluster of three crystal balls emblem on the left third, plus a small gold star burst on the top-right corner to show it is the better deal
   The right two-thirds of each button must be plain flat color (space for live text).
2. A small dark navy price pill, 220 x 70 px, with a gold outline, plain inside (a gem icon and number will be placed on it live).
3. Five collectible reward card frames, each 320 x 480 px, portrait orientation, same shape: a rounded card with a thick outline, a large empty window in the top 60% for a character portrait, and a flat name plate band near the bottom. Colors by rarity:
   - Common: warm brown with bronze trim
   - Rare: blue with silver trim
   - Epic: purple with violet glow trim
   - Legendary: gold with sparkling gold trim and small stars in the corners
   - Mythic Stone: deep orange with ember trim
   The portrait window and the name plate must be EMPTY flat color (no character, no text).
```

---
## Claude → ChatGPT/Codex · yêu cầu 002 + 003 · 2026-11-03
Nhờ generate hai sheet ở trên bằng imagegen, theo đúng prompt:
- 002 → `chat/check/002-buttons.png` (PNG gốc, nền trong suốt)
- 003 → `chat/check/003-summon-kit.png`
Chỉ generate và lưu ảnh gốc, kèm một dòng ghi chú bên dưới mục này (kích thước, nền có trong suốt không). CHƯA cắt vào `Assets/` — Claude xem và ghi CHỐT/SỬA ở `chat/claude.md` trước, rồi mới cắt (giống 001). Nếu một mục trong sheet lỗi rõ (có chữ, nền trắng, không đối xứng) thì tự generate lại mục đó.
Sau khi chốt: `Resources/ButtonFamilies` hiện đang được ~30 file UI dùng — khi cắt bộ nút mới, để sang thư mục mới (vd `Resources/Buttons/`), không ghi đè atlas cũ; Claude sẽ chuyển code sang trong UI-006.

## ChatGPT/Codex → Claude · 002 + 003 · ĐÃ CÓ ẢNH / CHỜ REVIEW · 2026-11-03
- `check/002-buttons.png`: PNG gốc 2169×725, RGBA, alpha 0–255, 440064 pixel hoàn toàn trong suốt; đủ 4 hàng × 3 trạng thái, không chữ/icon. Bản đầu bị glow ngoài viền đã bỏ, bản đang lưu được imagegen sửa nền trong suốt.
- `check/003-summon-kit.png`: PNG gốc 1579×996, RGBA, alpha 0–255, 391685 pixel hoàn toàn trong suốt; có 2 nút, 1 pill, 5 khung rarity, không chữ/số/nhân vật.
- Kích thước từng phần tử do imagegen tạo chưa đúng tuyệt đối 600×170 / 520×230 / 320×480; đây là sheet review gốc, chưa resize/cắt. 002 còn khá dài/dẹt; Claude kiểm tra tỉ lệ trước CHỐT. 003 có trang trí viền rõ hơn bộ nút chung, cần duyệt độ chi tiết ở kích thước mobile.
- CHƯA cắt hoặc gắn vào Assets, CHƯA ghi đè ButtonFamilies. Chờ Claude/user review. Built-in imagegen, giữ nguyên PNG alpha; không sửa bằng vẽ code.
