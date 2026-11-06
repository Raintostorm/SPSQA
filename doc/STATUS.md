# Trạng thái hiện tại
Cập nhật: 2026-11-03. Không coi compile pass là đã kiểm tra hình ảnh trong game.

## Việc vừa làm
- Đã gắn 5 buff mới: Ember Touch, Mana Flow, Wave Dividend, Mythic Might, Soul Harvest. Tổng 6 Epic / 9 Legendary, gồm 10 loại duy trì.
- Đã gắn khung chọn tím/vàng, bỏ scope box trống; chỉ hiện Mythic liên quan khi có quân thật.
- Đầu chapter 1-3 có 6 lượt chọn buff duy trì khác nhau, xen kẽ 3 Epic/3 Legendary. 6 loại trên HUD thành 4 icon + “+2”.
- Đã gắn burn: thân đỏ, lửa động trên đầu, DOT mỗi giây; nhận lại burn kéo dài thời gian, không hoãn tick.
- Đã gắn mana: crystal bay quanh rồi nhập vào người; icon mana+ trên đầu, nhiều trạng thái có slot riêng. Mana nhận liên tục không khởi động lại orbit đang chạy. Không hồi mana trong khi Guardian duy trì aura.
- Đã sửa MaterialPropertyBlock có thể null sau reload mã ở Play Mode.
- Chapter 1-3 tạm có sẵn 1 Guardian mỗi ván mới/restart, bên cạnh quân mặc định; không cấp unlock/shard/level vào save.

- CORE-001 (Claude Code): save ghi bằng `File.Replace`, giữ `save.prev.json`; Load thử save.json → .tmp → .prev và chuẩn hoá số âm/danh sách null. Mở rương qua `GameProgress.TryOpenChest` (trừ chìa + quà + bảo hiểm ghi một lần; kho thiếu bậc thì huỷ, không mất chìa). Thưởng qua màn phát trong `BattleFlow` lúc vào LevelCleared, `ResultPanel` chỉ hiển thị `flow.Settlement`. Chỉ mới compile + test model, chưa chạy Play Mode.

## Người dùng đang kiểm tra
Người dùng chưa xác nhận VFX mới chạy/đẹp trong combat thật; đã yêu cầu Guardian sẵn để tự test và sẽ báo lại. Guardian đã được thấy trong preview Mythic của màn chọn buff sau khi vào lượt mới. Đừng ghi “combat VFX đã được user duyệt trong Unity” ở thời điểm này.

## Kiểm tra đã chạy
- `bash tools/csharp_check.sh`: 169 nguồn tại lần kiểm tra gần nhất, sạch lỗi kiểu/cú pháp runtime; không thay cho import shader, Editor scripts hay device QA.
- `bash tools/test_effect_buffs.sh`: pool/choice, tick burn khi refresh và catch-up, growth chờ wave sau/reset/không cap, coin theo wave và luật vùng.
- `bash tools/test_status_tint.sh`: màu ổn định, alpha, tắt Effects và burn fallback.
- `bash tools/test_save_transactions.sh`: khôi phục sau crash, rollback giao dịch, mở rương và qua màn; đã thử cài lại 4 lỗi cũ, test đều bắt được.
- `bash tools/test_stage_buff_roll.sh`: 6 lượt đầu chapter 1-3, roll đầu Epic, stage khác giữ nguyên.
- Ranged/melee effect đã được người dùng xác nhận ở x1/x2/x3 trước vòng buff này; chưa có benchmark thiết bị để kết luận tối ưu FPS.

## Mã cần biết
| Mảng | Điểm vào |
|---|---|
| Buff / vùng / trạng thái | `Assets/_Project/Scripts/Battle/EffectBuff*.cs`, `BurnClock.cs`, `HarvestGrowth.cs` |
| Status VFX / màu | `Assets/_Project/Scripts/Visual/BuffProcVisual.cs`, `BuffStatusArt.cs`, `SlowFrostVisual.cs`, `StatusTint.cs`, `Assets/_Project/Art/Shaders/SpriteFlash.shader` |
| Mana / kỹ năng | `MythicCombat.cs`, `LegendaryCombat.cs` trong thư mục Battle |
| Guardian test / buff đầu ván | `BattleSetup.cs`, `Assets/_Project/Scripts/Gacha/StageRollPreview.cs` |
| UI buff | `Assets/_Project/Scripts/UI/EffectBuffChoiceView.cs`, `EffectBuffHud.cs` |
| Asset đã gắn | `Assets/_Project/Resources/EffectBuffs/Vfx/` |
| Nguồn demo được duyệt | `artsrc/ui/buff-proc-review-v3/`, `artsrc/ui/buff-proc-review-v4/` |

Tiến độ code/asset/tools/art đã được checkpoint theo yêu cầu user ngày 2026-11-03. Kế hoạch và mức kiểm tra tại PUBLISH_PLAN.md. Các lần làm việc tiếp theo vẫn phải tôn trọng thay đổi chưa commit của bên khác.


## PUBLISH-065 · 2026-11-06
User authorized publishing all remaining code, runtime assets, art sources and coordination files in grouped commits.
- 13/13 shell test suites pass. Ally stat harness repaired to compile production Rank/MythicKind and cover all Mythic mana roles.
- tools/csharp_check.sh: 179 C# sources compile clean. This is not a Unity build or physical-device QA.
- Chapter 1-2: GameBalance.WavesForLevel(2)=10; BattleFlow fallback also 10, and victory condition compares Wave against that count.
- Unity Home observed in Play mode; chapter navigation click did not change screen. No claim of completing ten waves. Physical-device QA remains pending.
- Ignored Unity crash recovery/cache files remain excluded intentionally.
