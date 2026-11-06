# Claude review
Batch cũ đã lưu archive/2026-11-03-before-008/. 006/007 và Relics đã chốt theo lịch sử.

## 024 · Chọn buff + HUD (ảnh user) · SỬA
- Đạt: ruy băng CHOOSE A BUFF (tím Epic / navy Legendary), Mythic nằm trong thẻ.
- Lỗi user khoanh: icon có nền. Kiểm 15 icon: 8 icon (DoubleStrike, Bodyguard, FrostTouch, GrandAscension, Reinforcements, WeaponSupplies, WarChest, SecondWind) cắt từ ảnh đề xuất `catalog.bytes` (RGB, không alpha, ~200×137) → có nền navy + dính viền thẻ. 7 icon còn lại trong suốt. Yêu cầu 008 vẽ lại 8 icon.
- Thấy thêm: dải Mythic trong thẻ quá nhỏ (chibi ~30 px, "×1" khó đọc) và nằm sát đáy; khoảng trống lớn giữa tên và mô tả vẫn còn → sửa layout thẻ khi gắn icon mới.

## 008 · 8 icon buff · CHỐT (2026-11-03)
- Đạt: nền trong suốt thật, không viền thẻ, cùng nét với ember/mana-flow, đủ 8 đúng thứ tự, WeaponSupplies đúng 1 kunai + 1 crystal. Nhỏ: Reinforcements có mắt (nhân vật game mặt trơn) — chấp nhận ở cỡ icon.
- Claude đã cắt theo cụm alpha từng ô (sao lấp lánh đi kèm icon): `Resources/EffectBuffs/icons/<slug>.png`, nguồn `artsrc/ui/buff-icons-v2/`. `EffectBuffArt` dùng bản mới trước, crop từ catalog chỉ còn làm dự phòng → áp chung cho thẻ chọn buff, HUD, icon proc trên đầu unit (BuffProcVisual tự scale theo bounds nên không đổi cỡ).
