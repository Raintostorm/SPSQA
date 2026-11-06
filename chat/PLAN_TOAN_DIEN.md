# SpinSquad — Kế hoạch toàn diện (05/11/2026)

Đọc kèm `chat/PLAN_CHO_CHATGPT.md` (bối cảnh, bảng màu, 4 luật art, hợp đồng cắt-gắn).
File này chia việc theo **ai làm** và **làm theo lô**: mỗi lô giao một lượt, xong cả lô
mới đưa Claude kiểm.

Ký hiệu: 🎨 ChatGPT/Codex vẽ + cắt + gắn · 💻 Claude viết mã · 🧑 người dùng chốt/kiểm Unity

---

## 0. Cần CHỐT trước (🧑) — trả lời một lần là mở khoá nhiều lô

| # | Câu hỏi | Ảnh hưởng |
|---|---|---|
| Q1 | Nút **Contribution** đổi icon kiếm → biểu đồ 3 cột? | Lô G1 |
| Q2 | Màn Summon có hiện **tỉ lệ + bộ đếm bảo hiểm** không? (`GAME_RULES.md` đang nói KHÔNG hiện tỉ lệ ở màn chính) | UI-009, lô G2 |
| Q3 | Thẻ buff có >3 nhóm Mythic: **"+N"** hay **cuộn ngang**? | UI-002 |
| Q4 | Màu ru-băng **Defeat**: giữ tím hay đổi xám đá `#4A5570`? | lô G1 |
| Q5 | Hoạt ảnh triệu hồi: **cầu sáng vỡ ra** hay **cổng phép mở**? | lô A1 |

---

## 1. XOÁ / DỌN (🎨 Codex soát, 💻 Claude đổi mã nếu cần)

Luật: chỉ dời vào `artsrc/ui/retired-<ngày>/` khi **0 tham chiếu trong mã** VÀ **0 GUID
trong `.unity/.prefab/.asset`**. Không xoá hẳn.

**Lô D1 — Codex làm một lượt, chỉ báo cáo, chưa dời:**

| Thư mục nghi cũ | Lý do nghi |
|---|---|
| `Resources/UiRefresh/` | icon kiểu cũ (.bytes), đã thay bằng Glyphs/ButtonIcons |
| `Resources/Buttons/`, `Resources/ButtonFamilies/` | bộ nút cũ, đã thay bằng `ButtonsV2` |
| `Resources/SummonKit/` | nút summon đã chuyển sang `ButtonIcons/summon-*` |
| `Resources/SharedUI/btn_*`, `SharedUI/gold`, `SharedUI/key` | đã có bản thay; **`btn_blue/btn_primary` còn dùng ở TreasureSceneSetup/UpgradeSceneSetup — giữ** |
| `SharedUI/buff_speed` | 0 chỗ dùng ở lần soát trước |

Cách soát (chép nguyên):
```bash
grep -rn "UiRefresh/\|\"Buttons/\|ButtonFamilies/\|SummonKit/" Assets/_Project/Scripts
# với từng file: lấy guid trong .meta rồi
grep -rl "<guid>" Assets --include=*.unity --include=*.prefab --include=*.asset
```
**Xuất:** bảng `chat/check/049-cleanup.md` gồm 3 cột: *đường dẫn · số chỗ mã · số GUID*.
Chỗ nào mã > 0 → ghi rõ file:dòng để Claude đổi; Codex **không sửa .cs**.

---

## 2. GENERATE art tĩnh (🎨)

**Lô G1 — icon & nền lẻ** (một sheet, sau khi chốt Q1, Q4):

| slug | đích | cỡ | ghi chú |
|---|---|---|---|
| `contribution` | `ButtonIcons/contribution.png` | 256² | biểu đồ 3 cột cao dần, bạc trắng viền đen (giống bộ ButtonIcons) |
| `sort-up` | `Glyphs/sort.png` | 150² | MỘT mũi tên lên; Claude xoay 180° cho "xuống" |
| `defeat` (nếu Q4 = đổi) | thay ô 1 trong `ResultScreen/atlas.png` | giữ nguyên Rect cũ | **giữ đúng toạ độ cắt** của `ResultScreenArt` |

**Lô G2 — Summon (sau Q2):** chỉ khi Q2 = có: khung nhỏ hiện tỉ lệ (`SummonKit` không dùng
lại) 600×120, border 9-slice `{60,30,60,30}` → `Summon/rate-frame.png`.

Mỗi lô: **soát asset có sẵn trước** (bài học 042: phần lớn "thiếu art" là mô tả sai).

---

## 3. FIX UI (💻 Claude, sau khi 🧑 chụp lỗi)

| ID | Việc | Phụ thuộc |
|---|---|---|
| UI-002 | thẻ buff mất nhóm Mythic thứ 4+ | Q3 |
| UI-009a | Home: bỏ trống nửa trên, chọn chapter cạnh BATTLE, chữ dưới dock | ảnh 🧑 |
| UI-009b | Shop: thẻ navy, crop Haunted Tank, bỏ thanh dưới khi 1 tab | — |
| UI-009c | Rewards: ẩn thưởng 0, chữ đè ảnh | — |
| UI-009d | kiểm số wave chapter 1-2 = 10 | — |
| 047-6 | mũi tên sort ↑↓ → sprite | lô G1 |
| UI-003/004 | QA iPhone X + một tỉ lệ khác, tap ≥ 44pt | **làm cuối** |

🧑 **Ngay bây giờ:** chạy Unity chụp Settings, kết quả trận, cuộn Relics, item trận,
dock Home (ô ☰), nút › Mythic → `chat/check/050-*.png`.

---

## 4. HOẠT ẢNH — chia việc đúng sức

**Nguyên tắc quan trọng nhất:** máy tạo ảnh **KHÔNG giữ được nhân vật/vật thể giống nhau
qua nhiều khung hình** — sheet 8 khung sẽ ra 8 cái rương khác nhau. Vì vậy:

- 🎨 **ChatGPT vẽ LỚP RỜI** (thân rương, nắp rương, quầng sáng, tia, hạt) — mỗi lớp là
  một ảnh tĩnh.
- 💻 **Claude ghép và làm chuyển động bằng mã** (xoay, phóng, mờ, bay, rung) qua `UiTween`.
- Chỉ dùng khung-hình-liên-tiếp cho thứ **không cần giống nhau giữa các khung**: loé sáng,
  khói, tia nổ (mỗi khung khác nhau là bình thường).

### Lô A1 — Triệu hồi (ANIM-002) 🎨

| slug | vẽ gì | cỡ |
|---|---|---|
| `orb-core` | quả cầu phép, tâm sáng | 512² |
| `orb-glow` | quầng sáng tròn mềm, **trắng thuần** (Claude tô màu theo độ hiếm bằng mã) | 512² |
| `burst-0..5` | 6 khung tia nổ to dần rồi tan | 6 × 512², sheet 6×1 |
| `card-back` | mặt sau thẻ triệu hồi | 400×560 |
| `rarity-ray` | một chùm tia toả, **trắng thuần** | 512² |
| `sparkle` | một hạt lấp lánh 4 cánh | 64² |

→ `chat/check/051-summon-anim.png` → cắt vào `Resources/Anim/Summon/`

### Lô A2 — Mở rương relic (ANIM-003) 🎨

| slug | vẽ gì | cỡ |
|---|---|---|
| `chest-body` | thân rương **không có nắp** | 512² |
| `chest-lid` | nắp rương rời, **cùng góc nhìn**, mép dưới thẳng để làm trục xoay | 512×256 |
| `chest-light` | luồng sáng hắt lên từ miệng rương, trắng thuần | 512² |
| `shard` | một mảnh shard nhỏ | 96² |

→ `chat/check/052-chest-anim.png` → `Resources/Anim/Chest/`
**Bắt buộc:** vẽ thân + nắp trên CÙNG một ảnh ở tư thế đóng, rồi tách — để khớp mép.

### Lô A3 — Lên cấp / gộp / thưởng (ANIM-004, 005) 🎨

| slug | vẽ gì | cỡ |
|---|---|---|
| `levelup-flash-0..3` | 4 khung loé vòng tròn, trắng thuần | 4 × 256² |
| `levelup-arrow` | mũi tên lên vàng viền đen | 256² |
| `merge-swirl` | xoáy năng lượng tròn, trắng thuần (mã sẽ quay) | 512² |

→ `chat/check/053-levelup-anim.png` → `Resources/Anim/LevelUp/`

### Prompt mẫu cho máy tạo ảnh (chép, thay phần <...>)

```
Game UI asset sheet for a 2D chibi fantasy mobile game.
Transparent background (PNG alpha), NO background color, NO text, NO watermark.
<N> separate objects in a single row, evenly spaced, at least 64px empty gap between objects.
Objects, left to right: <liệt kê từng slug + mô tả ngắn>.
Style: flat cel-shaded, thick even black outline, rounded shapes, one thin highlight on top,
one shadow band at the bottom. No metallic bevel, no multi-step gradients.
Glow / light / ray objects: pure white to transparent, soft edges, NO outline, no color tint.
Same camera angle and same lighting for every object. Front 3/4 view.
Palette: navy #1B2440, gold #F5B728, blue #3FA9F5, slate #4A5570, red #E24B4B.
```

Mẹo đã đúc kết:
- Lớp sáng phải **trắng thuần** — Claude tô màu theo độ hiếm bằng mã, một ảnh dùng cho 5 độ hiếm.
- Viết rõ "**separate objects, gap ≥ 64px**", nếu không nó vẽ dính nhau, không cắt được.
- Khung tia nổ: ghi "**frame 1 smallest … frame 6 faded out**", và chấp nhận mỗi khung khác nhau.
- Không viết chữ vào ảnh (còn đổi ngôn ngữ).

### Phần mã của Claude 💻 (không chờ art — có art giả thì thay sau)

ANIM-001 `UiTween` (unscaledDeltaTime, nút bỏ qua) → ANIM-006 chuyển màn/popup →
ANIM-002 summon → ANIM-003 rương → ANIM-004/005. Claude viết mã đọc
`Resources/Anim/...`, **thiếu ảnh thì tự dùng hình tạm**, nên hai bên làm song song được.

---

## 5. Thứ tự lô đề xuất

| Lượt | 🧑 | 🎨 ChatGPT/Codex | 💻 Claude |
|---|---|---|---|
| 1 | trả lời Q1–Q5, chụp ảnh 050 | **D1 + G1 + A1 + A2** (một lượt) | ANIM-001 + ANIM-006 |
| 2 | duyệt ảnh 049/051/052 | **A3 + G2** + cắt-gắn những gì đã duyệt | ANIM-002, 003, sửa mã từ 049, 047-6 |
| 3 | duyệt 053, chụp ảnh anim | soát lại (giai đoạn C) | ANIM-004/005, UI-002, UI-009 |
| 4 | QA máy thật | — | UI-003/004 |

Mỗi lô 🎨 xong ghi một mục `## OUTPUT · <lô> · <ngày>` vào `chat/chatgpt.md`:
đường dẫn, cỡ thật, đã sao lưu gì, **"chưa chạy Unity"** nếu chưa chạy.

---

## 6. TRẠNG THÁI 06/11 + CHẶNG TIẾP

### Đã xong (user đã chạy thử, "tạm ổn")
ANIM-001 UiTween · ANIM-002 triệu hồi (cầu 7 lớp) · ANIM-003 mở rương (đổi ảnh đóng→mở) ·
ANIM-004 lên cấp quân/relic (lô 063) · ANIM-005 ví chạy số · ANIM-006 mờ màn + popup ·
ANIM-007 màn kết quả (tia, confetti, số thưởng) · icon Contribution/sort · stat icon mới qua
ApprovedUiArt · sửa NullReference khi sửa code lúc Play.

### Chặng 1 — Dọn và chốt (nhỏ, làm ngay)
| # | Việc | Ai |
|---|---|---|
| 1.1 | **Commit** toàn bộ việc từ 03/11 (chưa commit gì) — chia theo nhóm, không `git add -A` bừa | 🧑 + 💻 |
| 1.2 | Gật ru-băng Defeat xám (058) → ghi vào atlas | 🧑 → 🎨 |
| 1.3 | Xác nhận icon chỉ số relic đã đúng kiểu mới → dời 9 file UiRefresh (atk, defense, hp, mp, range, speed, coin, gold, battle) | 🧑 → 🎨 |
| 1.4 | Chốt luật **sao đánh giá trận** (hoặc bỏ). Art sao đã có | 🧑 |

### Chặng 2 — UI-009 (màn còn thô)
| # | Việc | Phụ thuộc |
|---|---|---|
| 2.1 | Home: bỏ khoảng trống nửa trên, chọn chapter cạnh BATTLE, chữ dưới icon dock | ảnh hiện trạng 🧑 |
| 2.2 | Summon: hiện **bộ đếm bảo hiểm (pity)**, KHÔNG hiện tỉ lệ (Q2) | — |
| 2.3 | Shop: thẻ navy, crop Haunted Tank, bỏ thanh dưới khi chỉ 1 tab | — |
| 2.4 | Rewards/Codex: ẩn thưởng 0, chữ đè ảnh | — |
| 2.5 | Kiểm số wave chapter 1-2 = 10 | — |
🎨 song song: giai đoạn C (tự soát icon, chỉ báo cáo).

### Chặng 3 — QA thiết bị (cuối)
UI-003/004: iPhone X + một tỉ lệ khác, safe area, chạm ≥ 44pt, popup/raycast, kéo thả item.
TEST-001: sửa stub `MythicKind` để `tools/test_ally_stats.sh` chạy lại.
