# Screen / panel đã cải thiện
Cập nhật: 2026-11-03.

16 nhóm screen đã nhận UI work; đây không phải chứng nhận tất cả screen đã đạt device QA. Result win/loss tính một nhóm; collection/detail tính riêng. Chỉ tính một lần cho nhiều revision.

| # | Screen | Implemented baseline | Remaining work |
|---|---|---|---|
| 1 | Home | Marching allies, themed scenery, currency icons, icon dock, chapter/wave/Battle | Verify latest safe area and navigation |
| 2 | Battle | Shared Roll/Start position, Recipe/Item dock, Mythic strip, HUD, item feedback | Touch drag/drop, crowded combat and effect readability |
| 3 | Select Chapter | Navy background, selected stage, reward preview, Rewards/Codex shortcuts, gold Confirm | Fresh Play Mode visual QA |
| 4 | Rewards | Victory currency tiles and one-time encounter rewards; no equipment drop section | Fresh visual QA |
| 5 | Monster Codex | Six slime definitions, Common/Elite/Boss groups, discovery state | Fresh visual QA; unseen creatures remain hidden |
| 6 | Monster Details | Base stats, basic/Double Strike description, one-time reward state | Fresh visual QA |
| 7 | Shop | Nine daily offers, 3 columns, head portraits, Gold/Gems cards, grey sold strip, navy footer | Structured purchase confirmation implemented: portrait, quantity, gem cost, before/after balance, insufficient funds state; fresh visual QA pending |
| 8 | Summon | Witch tent, oracle sphere, 1x/10x, gem and shared stone wallets | Navy header, blue actions/Back and separate shard/stone feedback implemented; fresh visual QA pending |
| 9 | Summon Results | Grouped 3-column reward cards, scrolling, Continue | Head crops, separate shard/stone totals, 3-column grid and blue Continue implemented; fresh visual QA pending |
| 10 | Mercenaries | Navy frame, larger shard/level numbers, head crops, cyan selection, Mythic section | Latest frame compile passed; fresh visual QA pending |
| 11 | Upgrade | Separate detail, faction stage idle, 8 stats and green next-level delta, full-width shard bar, gold-only action | Latest frame compile passed; fresh visual QA pending |
| 12 | Relics | Three-column collection, rarity grouping, Open All | Latest scroll/empty/MAX-state device QA |
| 13 | Relic Details | Material bar, current buff and next-level delta, manual gold+shard upgrade | Navy fixed frame and scrolling stat body implemented; latest runtime visual QA pending |
| 14 | Relic Results | Grouped reward presentation | Latest long-result/device QA |
| 15 | Battle Results | Navy Victory/Defeat, real waves/time/rewards, Next/Retry/Home | Keep reward settlement separate from display |
| 16 | Effect Buff Choice | 3 thẻ chọn, khung tím/vàng, level, scope Mythic khi liên quan | Overflow >3 Mythic (UI-002), user đang kiểm tra VFX trong combat |

## Panel bổ sung (không cộng thành full screen)
Recipe có hai tab Mythic/Item; Item inventory 10 ô và drag/equip/combine; battle unit info; Settings; Contribution; Leave confirmation.

## Mức xác minh và việc tiếp theo
- Projectile và melee VFX đã được người dùng xác nhận x1/x2/x3. Status VFX burn/mana mới gắn vẫn chờ user test.
- CellPanel đã dùng derived ATK SPD (attacks/s), không còn mô tả sai raw cooldown là interval.
- Summon/results, Relic Details và Shop confirm đã gắn frame/UI mới; các audit 29/10 không chứng minh lỗi cũ còn tồn tại hôm nay.
- Cần kiểm tra lại Relic detail close/prev/next, tap target, modal/raycast, long-scroll, MAX/unowned, full materials, sold-out, wallet thiếu tiền.
- Cần kiểm tra Recipe/Item overlay không che drop target, 10 ô không tràn, head portraits đúng và Back không mất vị trí scroll.
- Một font/number style và state buttons cần audit trên screen thật trước khi generate thêm bộ art.
- Events chưa có thiết kế chốt; không tự thêm screen, currency hay reward.

Nguồn screenshot/asset: README trong từng thư mục `artsrc/ui/`. Giữ các manifest crop và nguồn generate; không xóa art theo tuổi file. Các ảnh proposal có thể không phải bản runtime mới nhất.
