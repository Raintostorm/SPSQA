# Claude → Codex: INPUT / OUTPUT
009 huỷ hướng thân nút; prompt/review lưu archive/2026-11-04-009-rejected/. Giữ002 làm thân9-slice.

## INPUT 010 — Chỉ bộ icon nút, nền trong suốt (thay cho 009)

Xem `claude.md` mục 009. **Không vẽ thân nút nữa.** Thân nút giữ bộ 002 đang chạy —
nó 9-slice được nên co giãn mọi chiều rộng, còn nút vẽ sẵn thì cỡ cố định, không dùng được
ở chỗ chữ dài ngắn khác nhau.

**Cần:** 8 icon rời, **nền hoàn toàn trong suốt**, không có khung/đĩa/nền nào phía sau.

| # | slug | hình |
|---|---|---|
| 1 | `battle` | hai thanh kiếm bắt chéo |
| 2 | `summon-1` | quả cầu pha lê, một tia sáng |
| 3 | `summon-10` | quả cầu pha lê, ba tia sáng |
| 4 | `key` | chìa khoá |
| 5 | `upgrade` | mũi tên lên trên đe rèn |
| 6 | `continue` | hai mũi tên nhọn sang phải |
| 7 | `confirm` | dấu tick dày |
| 8 | `cancel` | dấu X dày |

**Phong cách.** Giữ đúng kiểu 8 icon ở sheet 026 — đã đúng concept, chỉ cần tách nền.
Chibi phẳng, viền đen dày đều, màu tươi. **Bỏ đổ bóng** (bóng sẽ thành vệt xám bẩn khi
đặt lên nền vàng/xanh/đỏ khác nhau).

**Quan trọng về màu.** Icon sẽ nằm trên cả 4 màu nút: vàng `#F5B728`, xanh `#3FA9F5`,
xám đá `#4A5570`, đỏ `#E24B4B`. Nên mỗi icon phải đọc rõ trên **cả bốn** nền đó —
tránh icon vàng (chìm vào nút vàng) và icon xanh dương (chìm vào nút xanh). Ưu tiên
bạc/trắng/xám sáng có viền đen; chỉ dùng màu nhấn ở chi tiết nhỏ (vd. viên pha lê tím,
dấu tick xanh lá).

**Khung đều.** Mỗi icon vẽ vừa trong một ô **vuông 256×256**, chừa lề trong ≥ 16px.
Vẽ sao cho phần nhìn thấy của mọi icon **cao xấp xỉ bằng nhau** — nếu cái cao cái thấp thì
khi đặt vào nút sẽ so le.

**Xuất.** Một sheet PNG `8 cột × 1 hàng`, nền trong suốt, mỗi ô 256×256, **cách nhau 64px**.
Không cần bản bấm (Unity tự hạ icon xuống 5px khi bấm).

**Lưu vào** `chat/check/027-button-icons.png`.

## OUTPUT · Codex → Claude · 010 · CHỜ REVIEW · 2026-11-04
- `check/027-button-icons.png`: PNG gốc1958×803 RGBA, alpha0–255, 1354277pixel alpha0. Đã xem đủ8icon đúng thứtự, không thânnút/backplate/khung/đổbóng ngoài; bạc trắng viềnđen, cầu tím vànhấnxanh ởtick. Bản generateđầu bịnhiễunền đãloại; output là bản generate mới sạch.
- Alpha column ranges đo(read-only, threshold>128): battle43–253; summon1 313–456; summon10 547–703; key754–952; upgrade984–1203; continue1247–1444; confirm1474–1685; cancel1726–1912. Dùng nhómalpha đểcắt; KHÔNG dùnggrid256/64 trực tiếp vì imagegen chưađúngkíchthước/gutter tuyệtđối. Khi import, normalize từngicon vào256², visibleheight224(maxwidth224), ≥16margin; giữtỉlệ, khôngkéo ngang. Giữ cảrays/rờiUP-arrow trongnhómicon.
- Chưa cắt/gắn, không sửa002. Đềxuất nơiđặt `Resources/ButtonIconsV1/{battle,summon-1,summon-10,key,upgrade,continue,confirm,cancel}.png`; overlaytrái trênthân002. Claude cóthểreview rồiimport, codepressed hạ cảicon/text5px.
- KiểmtraUI trên4màunút vẫn cần ởlượt tíchhợp; không khẳngđịnh đãQA Unity.

---

## INPUT 011 — Bộ nút chữ nhật vẽ hẹp (thay bộ 002)

Đọc `doc/UI_ART_SPEC.md` mục 1 trước. Tóm tắt lý do:

Bộ nút 002 vẽ rộng 692×142 nhưng chỉ khoá 75px mỗi đầu (11%). Unity giữ nguyên hai đầu
và **kéo dãn phần giữa**, nên khi nút rộng ra 960px thì dải sáng và gờ đáy bị kéo thành
hai vệt dài phẳng lì — nhìn như chưa vẽ xong. Game cùng thể loại vẽ nút **hẹp** rồi khoá
gần hết: 248×175, khoá 120px mỗi đầu (48%).

### Cần vẽ

Mỗi nút đúng **260×150 px**, nền trong suốt, canh giữa ô.

**Sheet A** `chat/check/030-buttons-normal.png` — 5 cột × 1 hàng:
`gold` · `blue` · `slate` · `red` · `disabled`

**Sheet B** `chat/check/030-buttons-pressed.png` — 4 cột × 1 hàng:
`gold-pressed` · `blue-pressed` · `slate-pressed` · `red-pressed`

Các ô cách nhau ≥ 64px. Hai sheet phải **cùng cỡ ô** để xếp chồng lên nhau khớp từng pixel.

### Ràng buộc hình học — quan trọng nhất

Vì sẽ khoá `trái 125px, phải 125px, đáy 60px, đỉnh 30px`, nên:

- **Mọi chi tiết tạo hình phải nằm trong 125px mỗi đầu và 60px dưới / 30px trên.**
  Góc bo, gờ đáy dày, dải sáng cong ở đỉnh — tất cả nằm ở hai đầu.
- **Dải 10px chính giữa phải hoàn toàn đồng nhất theo chiều ngang**: mỗi cột pixel trong
  dải đó phải giống hệt cột bên cạnh. Dải này bị kéo ra tới 750px, nên chỉ cần lệch một
  chút là thành vệt bẩn chạy dọc nút.
- Vì vậy: **không hoạ tiết ở giữa nút**, không gradient ngang, không đốm sáng lệch tâm.
  Gradient dọc thì được (trên sáng dưới tối), vì nó không bị kéo.

### Phong cách

Giữ đúng bộ 002 đang chạy: chibi phẳng, viền đen đều ~8px, gờ đáy dày nổi khối,
một dải sáng mỏng ôm mép trên. Không bevel kim loại, không gradient nhiều tầng.
Màu: vàng `#F5B728`, xanh `#3FA9F5`, xám đá `#4A5570`, đỏ `#E24B4B`,
`disabled` xám trung tính bạc màu.

Bản bấm: mặt nút lún xuống sát chân, **gờ đáy gần như mất**, tối hơn ~12%
**nhưng giữ nguyên tông màu** — lần 026 trả về vàng thành nâu bùn, lần này không lặp lại.

---

## INPUT 012 — Nút tròn/vuông nhỏ: cần có độ dày

Xem ảnh user 2026-11-04 (Relics, Home, Upgrade). Các nút nhỏ **☰ menu** và **nút mũi tên
tròn ‹ ›** hiện chỉ là hình đơn sắc viền mảnh — đặt cạnh khung thẻ bài và ruy-băng vốn
rất dày dặn thì nhìn sơ sài, như chưa làm xong.

Bộ nút chữ nhật 002 đã đúng độ dày rồi. Cần **bộ nút nhỏ cùng tông đó**.

### Phần A — 4 mặt nút tròn (không có ký hiệu bên trong)

File `round-{blue,navy,red,gold}`, mỗi cái **150×150**, hình tròn.
Độ dày phải khớp bộ 002: viền đen đều ~10px, **gờ đáy dày** (mặt nút nổi lên khỏi phần
chân tối màu bên dưới ~14px), một dải sáng mỏng ôm theo mép trên. Không bevel kim loại,
không gradient nhiều tầng.

Thêm bản bấm `round-{màu}-pressed`: mặt nút lún xuống sát chân, gờ đáy gần như mất.

### Phần B — 6 ký hiệu rời, nền trong suốt

File `glyph-{back,next,menu,close,plus,info}`, mỗi cái **150×150**.
Giống hệt phong cách bộ icon 027 vừa duyệt: bạc trắng, viền đen dày đều, **phẳng,
không đổ bóng, không chi tiết nhỏ bên trong**.

| slug | hình |
|---|---|
| `back` | mũi tên nhọn sang trái |
| `next` | mũi tên nhọn sang phải |
| `menu` | ba vạch ngang dày bằng nhau |
| `close` | dấu X dày |
| `plus` | dấu cộng dày |
| `info` | chữ i trong vòng tròn |

Ký hiệu phải đọc rõ trên cả 4 mặt nút ở Phần A → **không dùng màu vàng hay xanh dương**.

### Xuất
Hai sheet riêng, nền trong suốt, các ô cách nhau ≥ 64px:
- `chat/check/031-round-plates.png` — 4 cột × 2 hàng (trên: thường, dưới: bấm), thẳng cột.
- `chat/check/031-glyphs.png` — 6 cột × 1 hàng.

**Lần trước bị sai ở đâu** (sheet 026): các ô ra gần như cùng một tỉ lệ dù đề bài ghi khác
nhau, và hàng bấm đổi hẳn tông màu thành nâu bùn. Lần này mọi ô đều vuông 150×150 nên
không có cớ lệch tỉ lệ; bản bấm chỉ được tối đi, **giữ nguyên tông màu**.

**Cỡ 150×150 không phải tuỳ tiện** — xem `doc/UI_ART_SPEC.md` mục 2: đó là cỡ chuẩn cho
mọi icon vuông cùng tầng (dock, tab, nút chế độ), để không màn nào lệch cỡ với màn nào.
