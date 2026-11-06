# Kế hoạch cải thiện UI (2026-11-03)

Mục tiêu: một ngôn ngữ hình ảnh chung cho cả game, rồi sửa từng màn theo ảnh user chụp 03/11.
Mỗi bước: Claude sửa → `tools/csharp_check.sh` → user chụp lại vào `chat/check/` → Claude ghi CHỐT/SỬA ở `chat/claude.md` → sang bước kế. Art mới đi qua `chat/chatgpt.md`.

Ký hiệu: ✅ xong + user thấy · 🔧 đã code, chờ user chụp · ⬜ chưa làm

## Quy tắc chung (áp cho mọi màn)
| Thành phần | Quy tắc | Code |
|---|---|---|
| Font | Lilita One cho tiêu đề/nút/số; Fredoka SemiBold cho câu mô tả, chữ ≤24 | `UiFonts` |
| Tiêu đề màn | Ruy băng navy, chữ Lilita trắng viền đen, cùng độ cao | `ScreenChrome.Title` |
| Tiêu đề màn chọn buff | Ruy băng tím, "CHOOSE A BUFF" | `ScreenChrome.Banner.Buff` |
| Tiêu đề mục | Bảng navy viền vàng | `ScreenChrome.Banner.Section` |
| Back | MỘT nút tròn xanh góc trên trái; không Back dưới đáy, không X | `ScreenChrome.Back` |
| Màu nút (UI-006) | Vàng = chính · Xanh = phụ · Xám tối = huỷ · Đỏ = nguy hiểm | chưa có |
| Phần thưởng 0 | Không hiện | |

## Giai đoạn 1 — Nền tảng (UI-005) · ✅ XONG 2026-11-03 (user xác nhận)
- ✅ Font Lilita/Fredoka cho toàn bộ Text
- ✅ Số damage hiện mọi đòn (kể cả kết liễu)
- ✅ `ScreenChrome` tiêu đề + back
- ✅ Rewards: ruy băng, back trên, bỏ Back dưới, tiêu đề mục bằng plaque, ẩn thưởng 0 (kéo theo Monster Codex, Monster Details cùng khung)
- ✅ Select Chapter: ruy băng, back trên thay X đỏ, i góc phải, ô Rewards navy + ẩn thưởng 0; Codex COMMON/ELITE/BOSS thành plaque
- ✅ Summon: bỏ hộp SUMMON, ruy băng, back trên, bỏ Back dưới, nút Summon dời xuống cho lộ lều
- ✅ Summon kit (art 003): nút ×1/×10 mới + pill giá; Summon Results ruy băng + khung thẻ theo độ hiếm; sửa icon shard trắng
- ✅ Shop: back trên (ví dời phải), ruy băng SHOP trên quầy, bỏ thanh dưới 1 tab, "Daily Shop/Gold/Gems" thành plaque, vùng cuộn dài thêm
- ✅ Relics: ruy băng + back chung, plaque COMMON/RARE…, ô relic dùng roster tiles (art 006) + Lv góc + ⬆ khi đủ shard, chưa sở hữu = ô xám
- ✅ Mercenaries / Upgrade: ruy băng, back trên (ví dời phải), bỏ Back dưới, danh sách cuộn dài thêm. 🔧 ô ally dùng khung roster (art 006) + đầu v3 + badge ⬆ (art 007)
- ✅ Chọn buff: ruy băng "CHOOSE A BUFF" (tím Epic / navy Legendary) thay nút UPGRADE; Mythic liên quan nằm TRONG thẻ, tối đa 3 + "+N" (một phần UI-002); 8 icon buff vẽ lại nền trong suốt (art 008) — dùng chung thẻ, HUD, proc
- ⬜ Battle Result: kiểm tra tiêu đề Victory/Defeat theo cùng kiểu (dời sang giai đoạn 6)
- ⬜ Nhỏ: dải Mythic trong thẻ buff — chibi còn nhỏ vì portrait có lề trong suốt

## Giai đoạn 2 — Hệ thống nút (UI-006)
- ✅ Art bộ nút (chat 002) đã cắt vào `Resources/Buttons/`
- ✅ `ScreenChrome.Role` + `ScreenChrome.Action(button, role)`: một chỗ định nghĩa vai trò → art + trạng thái
- 🔧 28 nút hành động đã chuyển: Home BATTLE, Select Chapter CONFIRM/Close, Rewards/Codex tabs, Summon ×1/×10 + Continue, Shop (mua, Exchange, FREE, Confirm/Cancel), Relics (Open All, Upgrade, Continue, Close), Mercenaries (Upgrade, sắp xếp, Close), Result (Next/Retry/Home), Settings (Settings/Contribution/Leave, Stay/Leave), Mythic fusion Summon, Cell (Sell/Merge), modal Resume/Close
- ✅ Trạng thái: pressed đổi sprite + hạ nội dung 5 px (`ScreenChrome.Press`), disabled dùng sprite xám chung; `KeepButtonScale` giữ tỉ lệ khi nút đổi cỡ
- ⬜ Battle dock (Roll/Start/x1/Enemy) để UI-007 làm cùng HUD
- ⬜ Tap target ≥ 44pt: kiểm trong UI-004

## Giai đoạn 3 — Battle HUD (UI-007)
- ⬜ Ẩn stat +0 %
- ⬜ Nút tốc độ cố định một chỗ ở prep và fight
- ⬜ Gold gộp vào cụm Roll; chữ hướng dẫn đủ to
- ⬜ Hàng icon buff không đè cảnh
- ⬜ Bảng Damage thu gọn, sân đánh rộng hơn

## Giai đoạn 4 — Báo hiệu (UI-008)
- ⬜ Chấm đỏ trên dock Home (relic nâng được, shop mới)
- ⬜ Relic đủ shard: mũi tên ⬆ (art `Badges/upgrade` đã có), thanh đổi màu
- ⬜ Rewards: dòng đã nhận mờ + ✓
- ⬜ Select Chapter: ✓ màn đã qua

## Giai đoạn 5 — Thẻ buff (UI-002)
- ⬜ Đưa thông tin Mythic vào trong thẻ; +N khi >3 nhóm
- ⬜ Thẻ cân đối, bớt khoảng trống

## Giai đoạn 6 — Sửa lẻ (UI-009)
- ⬜ Home: bớt trống nửa trên, chọn chapter cạnh BATTLE, chữ dưới icon dock
- ✅ Shop: khung thẻ phẳng v2 (chung Summon Results), head v3 đồng đều, relic to theo phần nhìn thấy (SpriteFit), badge ×N, nút giá vàng, đã bán = mờ + xám; gói Gold/Gems 3 cỡ (art 004); Haunted Tank cắt lại. head Furnace Crawler đã cắt lại
- ⬜ Summon: nút ⓘ tỉ lệ + bộ đếm bảo hiểm (cần user duyệt — GAME_RULES đang ghi không đưa rates lên màn Summon chính)
- ⬜ Kiểm số wave 1-2 = 10

## Giai đoạn 7 — QA thiết bị (UI-003, UI-004)
- ⬜ iPhone X + một tỉ lệ khác: safe area, tap target, chữ tràn

## Giai đoạn 8 — Hoạt ảnh (ANIM-001 … 006)

Khảo sát 04/11: `GemEconomyScreen`, `SummonResultsView`, `MythicPresentation` **không có
một dòng hoạt ảnh nào** — bấm Summon là kết quả hiện ra tức thì. `RollPanel` và
`JackpotPresentation` thì đã có (vòng quay trong trận). Tức là khoảnh khắc đáng ăn mừng
nhất của game lại là khoảnh khắc phẳng nhất.

### Ba ràng buộc, áp cho mọi hoạt ảnh

1. **Dùng `unscaledDeltaTime`, không dùng `deltaTime`.** Nút tốc độ đổi `Time.timeScale`
   (x1/x2/x3); nếu UI ăn theo đó thì bật x3 là hoạt ảnh menu chạy nhanh gấp ba.
2. **Chạm một cái là bỏ qua ngay, nhảy thẳng tới trạng thái cuối.** Đây là auto-battler,
   người chơi sẽ xem cùng một màn triệu hồi hàng trăm lần. Hoạt ảnh không bỏ qua được sẽ
   thành thứ bị ghét.
3. **Không khoá thao tác.** Hoạt ảnh chạy xong thì nút mới sáng, nhưng người chơi phải
   bấm được nút Back bất cứ lúc nào.

### ANIM-001 · Hạ tầng (làm trước, các phần sau đều dựa vào)
- ⬜ Một chỗ duy nhất: `UiTween` — đổi cỡ, mờ dần, trượt, số chạy; có `Ease`; chạy bằng
  thời gian không đổi tốc.
- ⬜ Cơ chế bỏ qua dùng chung: mọi chuỗi hoạt ảnh đăng ký một `Skip()`.
- ⬜ Một bảng thời lượng chuẩn (nhanh 0.12s / vừa 0.25s / chậm 0.5s), không rải số lẻ
  khắp nơi.

### ANIM-002 · Triệu hồi — phần đáng làm nhất
- ⬜ Bấm nút → quả cầu sáng dần, rung nhẹ, rồi loé.
- ⬜ Thẻ hiện **lần lượt** chứ không ùa ra một lượt (cách nhau ~0.08s).
- ⬜ Độ hiếm quyết định nhịp: Common lật nhanh, Legendary/Mythic dừng lâu hơn, loé mạnh.
  **Không được đoán trước kết quả bằng hiệu ứng trước khi thẻ lật** — hiệu ứng hiện
  *cùng lúc* thẻ lật, không sớm hơn.
- ⬜ ×10: hiện dần theo lưới, có nút "bỏ qua".

### ANIM-003 · Mở rương relic
- ⬜ Rương rung → nắp bật → shard bay ra.
- ⬜ Shard bay vào thanh đếm rồi số mới nhảy lên — nối nhân quả cho người chơi thấy.
- ⬜ Open All nhiều chìa: dồn thành một chuỗi, không lặp N lần đầy đủ.

### ANIM-004 · Nâng cấp và gộp
- ⬜ Thanh shard chạy đầy, số cũ → số mới chạy dần (đã có `AnimatedCounter`, dùng lại).
- ⬜ Lên level: loé trắng + nảy nhẹ.
- ⬜ Gộp Mythic: hai thẻ chụm vào nhau rồi hoá một (xem `MergeReveal` đã có sẵn).

### ANIM-005 · Thưởng cuối trận
- ⬜ Coin/gem bay từ bảng kết quả vào ví trên đỉnh (`CoinRewardEffect` đã có, kiểm lại).
- ⬜ Số trong ví chạy dần, không nhảy cóc.

### ANIM-006 · Chuyển màn
- ⬜ Mờ dần thay cho cắt cứng khi đổi scene.
- ⬜ Bảng/popup: hiện ra bằng cách phóng nhẹ từ 0.92 lên 1, không bật ra đột ngột.

**Thứ tự đề xuất:** ANIM-001 → 002 → 003 → 004 → 005 → 006.
002 và 003 ăn tiền nhất; 001 phải xong trước vì cả năm phần sau đều gọi vào nó.

## Giai đoạn 9 — Soát lại bộ icon (UI-010)

Khảo sát 04/11 bằng `tools/icon_contact_sheet.py` — dán toàn bộ icon lên một tấm, ở cỡ
gần với cỡ hiển thị thật. **Xem file gốc 512px là xem nhầm cỡ**: game chỉ vẽ chúng ở
48–170px, và lỗi "chi tiết nát thành lấm tấm" chỉ lộ ra ở cỡ đó.

### Nhóm 1 — Một khái niệm, hai hình. **Không cần vẽ mới, chỉ cần chọn.**

Đây là nhóm đáng sửa nhất: cùng một thứ mà mỗi màn vẽ một kiểu, người chơi phải học hai
lần cho cùng một khái niệm.

| Khái niệm | Các bản đang tồn tại | |
|---|---|---|
| Relic | `UiRefresh/relics` (la bàn tím) · `HomeChrome/relics` (bình vàng) | ⬜ chọn 1 |
| Chìa khoá | `UiRefresh/key` (viền vàng) · `HomeChrome/key` (đặc) | ⬜ chọn 1 |
| Vàng | `UiRefresh/gold` (túi) · `Shop/gold` (đống xu) · `HomeChrome/coin` (đồng xu) | ⬜ chọn 1–2 |
| Nâng cấp | `SharedUI/upgrade` (búa + mũi tên) · `Badges/upgrade` (vòng xanh) | ⬜ giữ cả 2 nhưng tách nghĩa rõ |

### Nhóm 2 — Lạc concept, **cần generate lại**

| Icon | Vấn đề |
|---|---|
| `restart`, `resume` | viền mảnh kiểu cyan / tam giác vàng trơn — không có viền đen dày như phần còn lại |
| `sound` | cùng họ cyan mảnh nói trên |
| `vibration` | điện thoại hiện đại trong game fantasy chibi |
| `close` | dấu X đỏ mảnh — **đã có `Glyphs/close` mới**, chỉ cần chuyển sang dùng, không phải vẽ |

### Nhóm 3 — Art chết, **xoá**

- ✅ `SharedUI/settings-polish` và `HomeChrome/recruit`: 0 chỗ dùng trong mã, 0 tham chiếu
  GUID trong scene/prefab. Đã dời sang `artsrc/ui/retired-2026-11-04/` (dời, không xoá —
  còn lấy lại được).
- ❌ `btn_blue`, `btn_primary`: **KHÔNG phải art chết**. Kiểm lại thấy `TreasureSceneSetup.cs`,
  `UpgradeSceneSetup.cs` và `tools/import_art_v2.py` vẫn dùng. Giữ nguyên.
  (Và `Assets/_Project/Art/UI/btn_blue.png.meta` là file khác nữa, đang làm khuôn `.meta`
  cho các script cắt art — đừng xoá nhầm.)

### Thứ tự
Nhóm 3 (xoá, không rủi ro) → Nhóm 1 (chọn, không cần art) → Nhóm 2 (cần generate).
Chỉ Nhóm 2 mới phải nhờ ChatGPT; hai nhóm đầu làm bằng mã.

### Đã soát và **ĐẠT**, không đụng
`EffectBuffs/icons` (vẽ lại ở chat 008) · `ButtonIcons` (010) · `Glyphs` (012, 014) ·
`Badges` (007) · `ShopPacks` (004 + 013) · bộ icon dock Home (user giữ bản cũ 04/11) ·
`StatIcons` (tim, kiếm, khiên, giọt nước — đồng bộ, đúng concept).
