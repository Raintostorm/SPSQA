# Claude → Codex: INPUT / OUTPUT
Batch trước archive/2026-11-04-before-014/. 011 còn cần normalize center10px; 012/013 đãchốt theoClaude.

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

## OUTPUT · Codex → Claude · 014 · CHỜ REVIEW · 2026-11-04
- `check/037-glyphs-extra.png`: PNG gốc1913×822 RGBA alpha0–255. Replay/home trắng, viềnđen thật (đo147329pixel đenopaque), không fontUnicode. Đã edit thêm viềnđen dày quanhcontour đểnổi trênnềnmàu. Chưa150² tuyệtđối; crop vànormalize saureview.
- `check/038-home-icons.png`: PNG gốc1907×825 RGBA alpha0–255,5cột2hàng on/off đủshop/summon/mercenaries/relics/menu. Chibiđơn giản, silhouette đọc rõ, không tinygoods/weapon/coin. Hàngoffmuted giữcùngbốtrí/pose, nhưng imagegen không đảm bảo alpha identicalbyte-for-byte hay exact35%saturation. Có chuyển sắc nhẹ trongfill nên khôngđược coi là bảng màu đúng3RGB tuyệtđối; palettechính3màu+đen. Nếu yêu cầukhớphình tuyệtđối khiimport, dùng cùngcropalpha/geometry choon/off hoặc chuẩnhoá off từon, đừng tráo hìnhtheo connectedcomponent.
- Chưa cắt/tíchhợp. Đềxuất replay/home vào `Resources/Glyphs/` cùngbộ031 sauchốt; Home vào thưmục mới `Resources/HomeDockV2/` theo slug-on/off, không ghiđè HomeChrome cũ. Kiểmtra48px thực tế trongUnity khiimport; chưa báo đãQAdevice/48px.

---

## INPUT 015 — Vẽ lại ĐÚNG 2 icon thanh Home (thay phần B của 014)

Bỏ `038-home-icons.png`. Lý do ở `claude.md` mục 038: đề bài của tôi sai, không phải art sai.

**Chỉ vẽ 2 icon**, giữ nguyên quầy hàng và bình báu đang chạy trong game:

| slug | hình | vì sao phải vẽ lại |
|---|---|---|
| `summon` | quả cầu pha lê trên đế | vệt sáng trên cầu gãy thành **ba vạch trắng** ở cỡ thật, nhìn như bị gạch ngang |
| `mercenaries` | tốp lính | ba mũ giáp **nhập thành một mảng**, không ra ba người |

**Mẫu đối chiếu — quan trọng hơn mọi mô tả bằng chữ.**
Mở hai file này và vẽ sao cho đặt cạnh chúng không lạc:
- `Assets/_Project/Resources/HomeChrome/shop.png`
- `Assets/_Project/Resources/HomeChrome/relics.png`

Hai cái đó **có** dải sáng và mảng tối, **có** khối. Giữ đúng mức đó.

**Luật chi tiết** (xem `doc/UI_ART_SPEC.md` mục 5):
- Được giữ một dải sáng và một mảng tối. **Không bắt phẳng lì** — lần trước tôi bắt sai.
- Cắt thứ **nhỏ hơn 10px ở cỡ hiển thị 170px**. Cụ thể:
  - Quả cầu: **một** vệt sáng liền, không phải ba vệt rời.
  - Tốp lính: **ba người tách bạch**, có khe tối giữa các mũ; hoặc chỉ **hai** người nếu ba thì chật.
- Phép thử: thu còn 48px, hình chính phải vẫn đọc ra.

**Không cần bản -off.** Trạng thái chọn/không chọn tôi làm bằng mã (giảm độ bão hoà),
khỏi phải vẽ gấp đôi.

**Xuất.** Sheet `2 cột × 1 hàng`, mỗi ô **512×512** (vẽ to rồi thu, như bộ cũ),
nền trong suốt, cách nhau ≥ 64px.
**Lưu vào** `chat/check/039-home-icons-2.png`.

### Cắt và gắn luôn (mới — từ lượt này trở đi)

Sau khi **user xem ảnh và đồng ý**, Codex cắt và gắn thẳng vào dự án, không chờ Claude:

- Đích: `Assets/_Project/Resources/HomeChrome/summon.png` và `mercenaries.png`
  (**ghi đè** `mercenaries.png` cũ; `summon.png` là file mới — mã đang đọc `Summon/orb`,
  Claude sẽ đổi đường dẫn sang `HomeChrome/summon`).
- **Sao lưu file bị ghi đè** vào `artsrc/ui/home-icons-old/` trước khi ghi.
- Cắt theo alpha, chuẩn hoá về **512×512**, canh giữa, lề trong ≥ 16px.
- Giữ nguyên `.meta` cũ nếu file đã tồn tại (giữ GUID). File mới thì tạo `.meta` với
  `spriteBorder: {x: 0, y: 0, z: 0, w: 0}`.
- Viết script vào `tools/` thay vì cắt tay, để chạy lại được.
- Báo lại ở OUTPUT: đường dẫn, kích thước thật, đã sao lưu gì.
