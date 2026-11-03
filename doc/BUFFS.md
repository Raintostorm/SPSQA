# Buff và hiệu ứng đang gắn
Cập nhật: 2026-11-03. Nguồn: EffectBuffCatalog/System/Scaling/Ground/Unit và các hook combat.

## Chọn / level / HUD
- Chọn 1 trong 3 khác loại, cùng độ hiếm của kết quả roll. Epic tím, Legendary vàng.
- Có 6 Epic / 9 Legendary; 10 loại duy trì có level. Chọn lại loại duy trì tăng level; thưởng tức thời không tạo icon duy trì giả.
- HUD: tối đa 5 ô. Nếu >5 loại: 4 icon + “+N”, N = tổng loại−4. Bấm mở phần còn lại; tăng level không tăng số loại.
- Scope box dưới card chỉ hiện khi có Mythic liên quan. Hiện đang giới hạn 3 nhóm; UI-002 sẽ xử lý overflow.
- Reset ván xóa buff, pending reward, growth, trạng thái chiến đấu. Không lưu buff ván vào meta save.

## Epic — thông số Lv.1
| Buff | Hành vi | Khi lên level |
|---|---|---|
| Blood Tile | Quân được gán ở ô Prep hồi 1% max HP/s +15% hút máu trong wave | +1% hồi và +15% hút máu mỗi level |
| Gold Tile | Quân được gán ở ô Prep last-hit enemy nhận 4 coin | +4 coin mỗi level |
| Double Strike | Mỗi đòn basic gây damage thứ 4: thêm hit 50% ATK | +50% ATK mỗi level |
| Bodyguard | Đầu wave nhận shield 20% max HP | +20% max HP mỗi level |
| Frost Touch | Mỗi hit basic thứ 4: giảm tốc đi/atk 20% trong 2s | +20%, cap giảm 80% |
| Ember Touch | Mỗi hit basic thứ 4 gây burn 20% ATK/s trong 2s | +20% ATK/s mỗi level |

Ô Máu/Vàng không cùng cell, giữ vị trí cả ván; marker chỉ hiện Prep và ẩn khi battle. Quân đứng ô lúc Marching được ghi nhận assignment; không phải theo marker trên đường trong battle. Tick heal chỉ Fighting; đầy HP vẫn có số xanh +N, không tăng max HP.

## Legendary
| Buff | Hành vi |
|---|---|
| Grand Ascension | Quân thường sống trên board lên 1 rarity, dừng ở Legendary; không biến thành Mythic; preflight placement |
| Reinforcements | Nhận 3 Legendary ngẫu nhiên; đầy board giữ queue |
| Weapon Supplies | Nhận 2 basic item; đầy 10 ô giữ queue |
| War Chest | +200 battle coin ngay |
| Second Wind | Tự cứu viện một lần khi sắp thua wave sắp tới; hết wave không dùng thì hết hiệu lực |
| Mana Flow | +20% mana nhận mỗi level; vẫn tôn trọng khóa hồi mana |
| Wave Dividend | Thắng wave: 3 × số wave vừa thắng × level buff coin; Lv.1 wave 5 = 15 |
| Mythic Might | Mythic +25% ATK mỗi level |
| Soul Harvest | Mỗi enemy bị hạ: tích +1 điểm % ATK/HP toàn ally × level, chỉ áp dụng từ wave sau; cộng dồn không cap trong ván |

Soul Harvest tích pending theo mạng chết, khi vào Prep kế tiếp chuyển sang active; tooltip hiện active và pending. Pending nhận trước khi có buff không được tính ngược.

## Presentation
- Burn: đỏ thân, icon lửa động trên đầu; DOT tick bằng game time, refresh không hoãn tick, xóa khi chết/reset/ra ngoài combat.
- Nhận mana thật của ally: crystal orbit trước/sau thân rồi thu vào người; icon mana+ có lifetime riêng. Nhận liên tiếp không restart orbit đang chạy. Các slot burn, mana, shield, slow có thể cùng hiển thị.
- Hiệu ứng chấp hành pause và setting Effects; nhiều icon xếp tối đa 4/cụm mỗi hàng, tránh HP/MP bars.
- Resource đã gắn: `Assets/_Project/Resources/EffectBuffs/Vfx/`; demo duyệt: `artsrc/ui/buff-proc-review-v3/`, `buff-proc-review-v4/`. User đang test hình ảnh thật, chưa có xác nhận cuối.

## Checks
`tools/test_effect_buffs.sh`, `tools/test_status_tint.sh`, `tools/test_stage_buff_roll.sh`. Cần combat QA cho tử vong/hồi sinh, multiple status, lật hướng, x1–x3 và nhiều ally; không thay thế bằng GIF demo.
