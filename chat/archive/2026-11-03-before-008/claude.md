# Claude xem ảnh và chốt

Mỗi ảnh trong `chat/check/` được xem ở đây TRƯỚC khi đưa vào game. Ghi: đạt gì, lỗi gì, quyết định (CHỐT / SỬA + prompt sửa ở `chatgpt.md`), và dùng ở đâu.

---

Luật dọn chat: khi một yêu cầu đã xong (có output, đã CHỐT và đã cắt/gắn), XOÁ mục đó khỏi `chatgpt.md` và `claude.md` để lượt sau bắt đầu gọn. Quyết định lâu dài ghi ở `doc/UI_PLAN.md` hoặc README trong `artsrc/ui/<tên>-vN/` trước khi xoá. Ảnh trong `check/` giữ lại.

---

_(Chưa có ảnh nào đang chờ duyệt.)_

## 012 · Shop trong game · SỬA (layout) + chờ art 004/005
- Đạt: back + ví + ruy băng SHOP, plaque mục, khung độ hiếm, badge ×N, nút giá vàng, gói Gold/Gems nền navy.
- Sửa layout (đã code): tên dài bị cắt ("Chipped Wooden") → best-fit; icon relic quá nhỏ trong ô → 225; thẻ to hơn 300×438 cho kín hàng, hàng cách 535 để nút không chạm thẻ dưới.
- Chờ art: 004 gói Gold/Gems 3 cỡ; 005 cắt lại đầu Haunted Tank.

## 004 · Gói Gold/Gems · CHỐT (2026-11-03)
- Ghép lên nền sáng và nền navy: không thấy quầng màu (28k pixel bán trong suốt chỉ là viền khử răng cưa). 3 cỡ đọc rõ nhỏ → vừa → lớn, đúng nét game.
- Claude đã cắt: `Resources/ShopPacks/{gold,gems}-{small,medium,large}.png` (nguồn `artsrc/ui/shop-packs-v1/`), đã nối vào gói Gold/Gems trong DailyShopScreen.

## 005 · Đầu Haunted Tank · CHỐT
- Bản mới nhận ra là xe tăng (đầu lâu + nòng + lửa xanh), đáy phẳng. Hình hơi nhỏ, nằm nửa dưới ô 280 — chấp nhận; trong khung chân dung được đặt sát đáy.

## 013 · Shop sau sửa (ảnh user) · SỬA → đã code
- Đạt: gói Gold/Gems 3 cỡ; Haunted Tank mới; tên dài vừa bảng tên; hàng/nút không chạm.
- User thấy: relic bé, đầu allies căn lệch (Crescent Mage nhỏ lơ lửng, Royal Hero tràn). Nguyên nhân: PNG có lề trong suốt khác nhau, code đặt theo cả khung PNG.
- Sửa: `SpriteFit` đặt theo phần nhìn thấy (lề đo sẵn bằng `tools/build_sprite_bounds.py` → `Resources/UiSpriteBounds.txt`); head phủ ~96% bề ngang, vai chạm đáy ô; relic ~80% ô, căn giữa. Áp cho Shop và Summon Results. Thêm head/relic mới thì chạy lại tool.

## 003-v2 · Khung thẻ phẳng · CHỐT (user duyệt 2026-11-03)
- Đạt: 5 khung cùng hình, chỉ khác màu; ô navy + quầng nhẹ; band tên tối nên chữ trắng đọc rõ; không còn kim loại/sao/kim cương.
- Đã cắt: `Resources/RewardCardsV2/card-*.png` (382×577; nguồn `artsrc/ui/reward-cards-v2/`). Đã thay vào Shop + Summon Results; `SummonKit/card-*` cũ giữ làm tham chiếu.

## Heads v3 · CHỐT (user duyệt)
- 19/20 head cắt lại đồng đều từ idle_000 (cùng tỉ lệ, ô 210 px từ đỉnh, căn giữa đầu) → ghi đè `Resources/ShopV2/Heads/` giữ .meta; bản cũ ở `artsrc/ui/heads-v2-old/`. machine_epic chưa có nguồn idle — giữ bản cũ.

## 017–019 · Shop / Results / Mercenaries / Upgrade (ảnh user)
- Shop + Results: CHỐT — khung v2 phẳng hợp nền navy, head v3 cùng cỡ, relic to rõ. Furnace Crawler vẫn head cũ (phóng to) — chờ nguồn idle.
- Mercenaries/Upgrade: tiêu đề chữ trơn + nút Back to dưới đáy → đã code ruy băng + back góc trên, bỏ Back dưới. Ô ally còn kiểu cũ (nền xám/xanh phẳng, chữ READY đè đầu) → đề xuất làm lại bằng khung v2 ở UI-009.

## 021 · Mercenaries / Upgrade sau sửa · CHỐT
- Ruy băng 110 px nằm gọn giữa hàng ví và khung cảnh; back thẳng hàng ví; không còn Back dưới. Còn: ô ally kiểu cũ → chờ art 006.

## 006 · Ô allies (roster tiles) · CHỐT (2026-11-03)
- Đạt: cùng bộ phẳng với khung v2, 6 màu đúng (Mythic crimson, Locked xám). Vùng info thực tế ~1/3 dưới (63–92%), không phải 45% → Lv đặt ở góc ô chân dung, band chỉ chứa tên + thanh shard.
- Đã cắt `Resources/RosterTilesV1/tile-*.png` (357×398), đã nối Mercenaries (ô 300×334, hàng 350). `tile-locked` chưa dùng (chưa có luật "chưa sở hữu").

## 007 · Huy hiệu · CHỐT
- Đạt: dot, upgrade ⬆, check ✓, lock, new — nét dày, đọc được ở cỡ nhỏ. Đã cắt `Resources/Badges/{dot,upgrade,check,lock,new}.png`. Đã dùng `upgrade` thay chữ READY ở Mercenaries; phần còn lại cho UI-008.

## Furnace Crawler head · xong
- Nguồn Codex tìm: `Art/Units/machine_epic/idle_smooth_1.png` → override trong `tools/build_uniform_heads.py`, head v3 đã ghi đè (giữ .meta), bounds cập nhật.

## 022 · Mercenaries với ô mới · CHỐT
- Đạt: khung roster cùng bộ, đầu v3 đều, ⬆ thay READY, Mythic crimson tách nhóm. Sửa nhỏ: tên hơi đè mép ô chân dung → cỡ 26, hạ xuống 4 px.

## 023 · Relics · CHỐT
- Đạt: ruy băng + plaque hạng, ô roster cùng bộ Mercenaries, icon to đều, ⬆ đúng chỗ đủ shard, chưa sở hữu = ô xám + "Not owned". Sửa: nút back đè icon xu của ví Gold → ví dời phải.

## 024 · Chọn buff + HUD (ảnh user) · SỬA
- Đạt: ruy băng CHOOSE A BUFF (tím Epic / navy Legendary), Mythic nằm trong thẻ.
- Lỗi user khoanh: icon có nền. Kiểm 15 icon: 8 icon (DoubleStrike, Bodyguard, FrostTouch, GrandAscension, Reinforcements, WeaponSupplies, WarChest, SecondWind) cắt từ ảnh đề xuất `catalog.bytes` (RGB, không alpha, ~200×137) → có nền navy + dính viền thẻ. 7 icon còn lại trong suốt. Yêu cầu 008 vẽ lại 8 icon.
- Thấy thêm: dải Mythic trong thẻ quá nhỏ (chibi ~30 px, "×1" khó đọc) và nằm sát đáy; khoảng trống lớn giữa tên và mô tả vẫn còn → sửa layout thẻ khi gắn icon mới.
