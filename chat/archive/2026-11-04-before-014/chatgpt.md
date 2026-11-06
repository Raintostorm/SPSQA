# Claude → Codex: INPUT / OUTPUT
010 đã CHỐT; lịch sử lưu archive/2026-11-04-010-approved/.

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

## OUTPUT · Codex → Claude · 011 + 012 · CHỜ REVIEW · 2026-11-04
Đã đọc doc/UI_ART_SPEC.md và generate bằng built-in imagegen. PNG gốc alpha thật(0–255):
- check/030-buttons-normal.png:2032×774,5cột normal.
- check/030-buttons-pressed.png:2033×773,4cột pressed.
- check/031-round-plates.png:1914×822,4cột×2hàng, blue/navy/red/gold, mặtthường/bấm.
- check/031-glyphs.png:1487×1058,6glyph back/next/menu/close/plus/info, bạc trắng, không bóng.

Giới hạn kỹ thuật đã đo, KHÔNG coi là asset production-ready: 030normal mỗi button bbox rộng~340px, chưa260×150. Hai sheet khác1px canvas và không khớp footprint tuyệtđối. Đặc biệt đã so sánh cộtpixel trungtâm liềnkề: cả5normal đều KHÔNG giống nhaubyte-for-byte. Nghĩa là ràngbuộc middle10pxuniform CHƯA đạt dù prompt đã nêu rõ; cần chuẩn hóa raster saureview (kíchthước, copycùng1cột vào10px giữa), rồi kéo9slice kiểm seam. Không lấy nguyênsheet và gán125border trước khi xửlý. Pressed lip mỏng hơn nhưng độ tối chưa đo/chốt12%; cần dùng cùngpalette khi chuẩn hóa, không nhận là đã đạt thôngsố.
031 có độdày rõ, chưa150² tuyệtđối; glyph cóalpha để cắt và normalize. Theo INPUT012 dùnground150² dù UI_ART_SPEC mục4 nói fixednonsquare—ưu tiên yêu cầucụthể lần này. Chưa cắt/gắn/ghiđè002. Đềxuất sauduyệt Resources/ButtonsV2 và SmallButtonsV1; Claude chốtđườngdẫn trước chuyểncode.

---

## INPUT 013 — Gói chìa khoá cho Shop

Shop vừa có thêm mục **Keys** (đổi gems lấy chìa khoá mở rương). Cần art cho 3 gói,
**cùng bộ với `ShopPacks/gold-*` và `gems-*` đã duyệt ở chat 004** — nhìn phải ra cùng
một dãy hàng, không lạc kiểu.

| file | nội dung |
|---|---|
| `keys-small` | 1–2 chìa khoá nằm rời |
| `keys-medium` | một nắm 5–6 chìa, có cái chồng lên nhau |
| `keys-large` | một đống chìa tràn ra khỏi túi hoặc rương nhỏ |

**Quy tắc quan trọng — lượng phải đọc được ngay.** Nhìn ba ảnh cạnh nhau phải thấy rõ
ít → vừa → nhiều, giống cách `gold-small/medium/large` tăng dần từ xu lẻ → túi → rương.
Đừng chỉ phóng to cùng một hình.

**Chìa khoá phải trùng kiểu với chìa trong game**: thân tròn có ba lỗ, màu vàng đồng,
viền đen dày — xem `Assets/_Project/Resources/HomeChrome/key.png` để lấy đúng dáng.

**Phong cách.** Chibi phẳng, viền đen đều, không bevel kim loại, không gradient nhiều
tầng. Nền **trong suốt** — ảnh sẽ nằm trên thẻ navy của Shop.

**Xuất.** Một sheet PNG `3 cột × 1 hàng`, mỗi ô **380×380**, các ô cách nhau ≥ 64px,
nền sheet trong suốt.

**Lưu vào** `chat/check/033-key-packs.png`.

## OUTPUT · Codex → Claude · 013 · CHỜ REVIEW · 2026-11-04
- `check/033-key-packs.png`: PNG gốc1568×1003 RGBA, alpha0–255; đủ3gói một hàng:2chìa rời → nắm6chìa → túinâu đầy chìa+cácchìa rơi. Lượng tăng rõ, không phóngto cùnghình. Dùng key.png và gold-medium/gems-large làm reference trực tiếp trong built-in imagegen.
- Sai khác INPUT: key.png thật có đầu tròn1lỗ (không3lỗ); ưu tiên ảnhgốc đúng kiểu đangtronggame, đãgenerate đầu1lỗ. Không chữ/giá/khung/nềnthẻ. Kíchthướccell chưa380² tuyệtđối; cắt theoalpha rồi normalize cùngfootprint khiđượcduyệt.
- Chưa cắt/gắn. Đềxuất cùng bộ `Resources/ShopPacks/keys-{small,medium,large}.png` sauClaude/userchốt. Không đụng UI code đangcókhóa.

---

## INPUT 014 — Icon thanh Home + 2 ký hiệu còn thiếu

Đọc `doc/UI_ART_SPEC.md` mục 2 và 3 trước.

### Phần A — 2 ký hiệu bổ sung cho bộ 031

Màn kết quả trận đang dùng **ký tự Unicode** `↻` và `⌂` do font hệ thống vẽ — lạc hẳn
khỏi game. Cần vẽ thật, **giống hệt phong cách bộ `031-glyphs` đã duyệt**: trắng, viền
đen dày đều, phẳng, không đổ bóng, đọc được trên cả nền vàng/xanh/xám/đỏ.

| slug | hình |
|---|---|
| `replay` | mũi tên vòng tròn, đầu nhọn rõ |
| `home` | ngôi nhà, mái tam giác, thân vuông |

Mỗi cái **150×150**, nền trong suốt. Sheet `2 cột × 1 hàng`, cách nhau ≥ 64px.
**Lưu vào** `chat/check/037-glyphs-extra.png`.

### Phần B — Bộ icon thanh Home, có bản chọn / không chọn

Bộ hiện tại còn từ lượt cũ, mỗi cái một cỡ và **không có bản "đang chọn"**.

5 icon: `shop` (quầy hàng) · `summon` (quả cầu pha lê) · `mercenaries` (tốp lính) ·
`relics` (rương báu) · `menu` (ba vạch).

**Mỗi icon 2 bản, đúng theo game cùng thể loại:**
- `-on`: màu đầy đủ, tươi, như đang được chọn.
- `-off`: **cùng hình y hệt**, chỉ bạc màu và tối đi (~35% độ bão hoà). Không đổi hình,
  không đổi dáng — người chơi phải nhận ra ngay đó vẫn là cùng một icon.

Mỗi ô **150×150**, nền trong suốt, canh giữa, phần nhìn thấy cao xấp xỉ bằng nhau giữa
các icon — nếu cái cao cái thấp thì đặt lên thanh sẽ so le.

**Mức chi tiết — đây là chỗ dễ sai nhất.** Icon chỉ cao 150px trên máy thật, nên mỗi
icon **tối đa 3 màu + viền đen**, bóng đổ bỏ hẳn, không vẽ chi tiết nhỏ hơn 10px.
Hình phải đọc được khi thu còn 48px. Bộ cũ hỏng đúng ở chỗ này: vẽ như tranh minh hoạ
thu nhỏ, nên ở cỡ thật thành một đám lấm tấm.

Phong cách: chibi phẳng, viền đen đều, cùng tông với bộ icon `027` và `031` đã duyệt.

**Xuất.** Sheet `5 cột × 2 hàng` (hàng trên `-on`, hàng dưới `-off`), thẳng cột,
mỗi ô 150×150, cách nhau ≥ 64px, nền trong suốt.
**Lưu vào** `chat/check/038-home-icons.png`.
