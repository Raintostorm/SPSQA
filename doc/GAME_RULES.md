# Luật hiện hành và cấu hình test
Cập nhật: 2026-11-03. Thay các bảng cân bằng/handoff cũ; xác nhận mã nguồn trước mỗi thay đổi gameplay.

## Art / UI
- Fantasy 2D chibi, mặt đơn giản, màu phẳng, outline đậm; không tăng chi tiết kiểu realistic.
- iPhone X 1125×2436 là mốc demo; phải kiểm tra safe area. Nhân vật trong upgrade dùng idle.
- Home dock ưu tiên icon trực quan. Collection allies/relics/shop: 3 cột; ally card dùng đầu/thân trên.
- Mercenaries là danh sách; Upgrade là screen con khi chọn ally. 8 stat, hiện tăng ở level tiếp; nguyên liệu ở thanh trên, nút chỉ giá gold.
- Leave confirmation không có X trên cùng. Roll/Start dùng chung vị trí giữa; hết coin roll thì chuyển Start.

## Economy / progression
- Ally summon: 10 gems/lần, 100/10 lần. Runtime hiện tại cấp 20 shards/lần; stone outcome cấp 20 Mythic Stones dùng chung. Có yêu cầu cũ từng nói 10 shards: không tự đổi runtime 20 nếu chưa có chỉ đạo mới.
- Mythic Stone hiếm hơn Legendary; rate do AllySummon.StoneChance tính; không đưa rates lại lên màn Summon chính.
- Allies/Mythics max level 30. Upgrade dùng gold + shard của ally, hoặc gold + shared stone cho Mythic.
- Relic: 1 shard/roll; Open All dùng toàn bộ keys; nâng thủ công bằng gold+shards, không auto-upgrade.
- Chapter 1-1 thưởng 80 gems, 1-2 thưởng 250 gems mỗi lần thắng.
- Shop: 9 món/ngày, 3 cột, mỗi món mua một lần/ngày. Gold đổi từ gems; mua gems hiện là miễn phí để test vì chưa có thanh toán.

## Mythic / item / combat
- Một Mythic ghép từ đúng 3 ally theo công thức riêng; ít nhất 1 Epic trở lên. Rẻ nhất Epic+2 Common, đắt nhất 3 Legendary; cấm cả 3 nguyên liệu cùng nhân vật.
- Mythic tối đa 1 con/cell; cap trong trận bắt đầu 2, relic Legendary tăng +1 cap mỗi 2 level. Mythic không buộc thuộc 5 tộc thường.
- Item chỉ Mythic mang; túi 10 ô. Kunai +10% ATK SPD; Mana Stone +20 starting MP.
- Mana Heart: +50% mana nhận từ mọi nguồn. Kunai+mana: khi người trang bị last-hit, đánh một enemy khác với 20% ATK. Kunai+kunai: thêm +50% ATK SPD theo runtime; khi không có speed bonus khác tương đương ×1.5. Có buff tốc khác thì đang cộng bonus, không phải nhân toàn bộ tốc hiện có lên 1.5. Đây là khác biệt cần chốt riêng nếu user muốn nhân cuối cùng.
- Guardian aura: mỗi giây gây 20% ATK quanh mình, hồi 50% damage gây ra; tiêu 15 MP/s, không hồi MP khi aura hoạt động; vẫn đi/atk sau cast.
- Undead Mythic: hồi sinh Common/Elite địch, không Boss; mỗi ally hồi sinh tối đa một lần/trận. Clock dừng địch trong vùng 5s. Mimic payout +5 coin/mỗi spell hợp lệ ở wave sau.
- Target đã chết: không nhận damage lại. Attack đã commit vẫn kết thúc anim ở điểm đã khóa; projectile theo original target, target chết thì tới death position và biến mất không impact.
- Skill/effect damage hiện tím; basic hạ gục enemy không hiện số mới. HP bar chỉ hiện khi mất máu; hồi HP đầy lại ẩn. Máu thực luôn cap ở max.

## Test đang bật — không được nhầm là luật release
- Chapter 1-3: 6 lựa chọn buff miễn phí đầu ván, không trùng loại duy trì; roll trả phí đầu tiên vẫn ép Epic.
- Chapter 1-3: tạm cấp 1 Guardian đầu ván/restart để user kiểm tra.
- Chapter 1-2 có starter recipe/item fixture riêng; giữ nguyên trừ khi user yêu cầu bỏ.
- Gỡ/cô lập các test hook là task release riêng, không âm thầm bỏ trong lúc UI QA.
