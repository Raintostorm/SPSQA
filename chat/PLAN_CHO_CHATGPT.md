# Kế hoạch art UI cho SpinSquad — bản tự chạy

File này đủ để làm việc **không cần hỏi lại Claude**. Đọc hết mục 1–3 trước khi vẽ bất cứ
thứ gì, rồi làm từng giai đoạn ở mục 4.

---

## 1. Bối cảnh

- Game: **SpinSquad**, auto-battler chibi 2D, Unity, màn dọc, chuẩn **1125×2436** (iPhone X).
- Art đích đến: `Assets/_Project/Resources/<thư mục>/`.
- Ảnh duyệt để ở `chat/check/`, đánh số tăng dần (`041-...`, `042-...`).
- Người dùng xem ảnh và gật trước khi cắt. **Bước này không bỏ được.**

### Bảng màu
`#1B2440` navy · `#F5B728` vàng · `#3FA9F5` xanh · `#4A5570` xám đá · `#E24B4B` đỏ

### Phong cách
Chibi phẳng, **viền đen dày đều**, bo góc tròn, **một dải sáng mỏng ở đỉnh và một mảng
tối ở đáy**. KHÔNG bevel kim loại, KHÔNG gradient nhiều tầng, KHÔNG sao/kim cương kiểu
thẻ bài TCG.

---

## 2. Bốn luật đã trả giá mới rút ra được

Đây là những lần đã làm hỏng. Đọc kỹ, đừng lặp lại.

### 2.1 Mức chi tiết tính theo **cỡ hiển thị thật**, không theo số màu

Icon nằm trong kho ở 512px, nhưng game chỉ vẽ chúng ở **48–170px**. Lỗi "chi tiết nát
thành lấm tấm" chỉ lộ ra ở cỡ thật.

- Cắt thứ **nhỏ hơn 10px ở cỡ hiển thị**, không phải cắt màu.
- **Được giữ** một dải sáng và một mảng tối — đó là chất của game.
- Phép thử bắt buộc: thu icon còn **48px**, hình chính phải vẫn đọc ra.

> Lần hỏng: yêu cầu "tối đa 3 màu, bỏ hẳn bóng đổ" → ra bộ icon phẳng lì, đúng y đề bài
> mà mất hết chất game, phải bỏ cả bộ.

### 2.2 Đừng vẽ lại cả bộ khi chỉ vài cái hỏng

> Lần hỏng: xin vẽ lại cả 5 icon thanh Home, trong khi quầy hàng và bình báu vốn đang
> rất tốt. Kết quả là vứt luôn hai cái đang ổn, và cuối cùng người dùng quay về bộ cũ.

Soát từng cái ở cỡ thật trước. Chỉ thay cái thật sự hỏng.

### 2.3 Nút chữ nhật: vẽ **hẹp** rồi khoá gần hết

Unity 9-slice giữ nguyên hai đầu và **kéo dãn phần giữa**.

| | bộ cũ (hỏng) | chuẩn hiện tại |
|---|---|---|
| ảnh gốc | 692×142 | **260×150** |
| khoá mỗi đầu | 75px = 11% | **125px = 48%** |

Viền 9-slice: `x=125, y=60, z=125, w=30`.

Hệ quả bắt buộc: **mọi chi tiết tạo hình phải nằm trong 125px mỗi đầu**. Dải 10px chính
giữa bị kéo ra tới 750px, nên **mỗi cột pixel trong dải đó phải giống hệt cột bên cạnh**.
Không hoạ tiết ở giữa, không gradient ngang. Gradient dọc thì được.

### 2.4 Bản "bấm" chỉ được **tối đi**, giữ nguyên tông màu

> Lần hỏng: bản bấm trả về vàng → nâu bùn, xanh → xanh đục, và **icon không tối theo**.
> Nhìn ra "nút bị khoá", không phải "nút đang bấm".

Bản bấm: mặt nút lún sát chân, gờ đáy gần như mất, tối hơn ~12%, **cùng tông màu**.
Icon bên trong tối theo cùng mức.

---

## 3. Hợp đồng cắt và gắn

Mỗi giai đoạn ở mục 4 đều có sẵn phần "Cắt và gắn". Làm đúng 6 điều sau:

1. **Sao lưu trước khi ghi đè** vào `artsrc/ui/retired-<ngày>/`. Dời, không xoá.
2. **Giữ `.meta` cũ** nếu file đã tồn tại — mất GUID là Unity đứt hết tham chiếu trong scene.
   File mới thì tạo `.meta` với `spriteBorder` ghi trong đề bài.
3. **Viết script vào `tools/`**, đừng cắt tay — để chạy lại được.
4. Cắt theo **alpha**, rồi chuẩn hoá vào khung vuông đã nêu, canh giữa.
5. **Không gom theo "mảng pixel dính nhau"** nếu icon có mảnh rời (tia sáng quả cầu, vạch
   của dấu tick). Gom theo **khe trống giữa các cột**. Khe hẹp nhất từng gặp là **34px**,
   nên ngưỡng gom phải nhỏ hơn thế.
6. **Không sửa file mã.** Nếu cần đổi đường dẫn trong code, ghi vào `chat/chatgpt.md`
   phần OUTPUT để Claude làm.

### Công cụ có sẵn
```bash
# Dán toàn bộ icon lên một tấm, ở cỡ gần cỡ hiển thị thật, để soi bằng mắt
python3 tools/icon_contact_sheet.py /tmp/a.png StatIcons SharedUI HomeChrome
```
```bash
# Khuôn cắt icon vuông — sửa đường dẫn sheet và danh sách tên rồi chạy
python3 tools/import_button_icons.py
```

---

## 4. Các giai đoạn

### Giai đoạn A, B — ĐÃ ĐÓNG 04/11, **không vẽ nữa**

Bản soát asset (`chat/check/042-assets-audit.md`) cho thấy cả hai giai đoạn đều **không
cần art mới**. Đây là kết quả tốt: luật 2.2 đã chạy đúng — soát trước, không vẽ thừa.

| việc | kết luận |
|---|---|
| `restart` | dùng lại `Glyphs/replay` — đã gắn |
| `resume`, `sound` | bản cũ **có viền đen, đọc rõ ở 48px** — tôi mô tả sai trong bản kế hoạch đầu. Giữ nguyên |
| `vibration` | vẫn là **điện thoại hiện đại**, lạc thế giới — nhưng chuông dễ bị đọc nhầm thành "thông báo". **Chờ người dùng chốt biểu tượng**, chưa vẽ |
| `victory` / `defeat` | **đã có sẵn** trong `ResultScreen/atlas.png`, `ResultPanel` đang dùng qua `ResultScreenArt.Get(0/1)`. Giai đoạn B huỷ |

Bài học ghi lại: **soát asset đang có trước khi mô tả thứ cần vẽ.** Hai giai đoạn trong
bản kế hoạch đầu đều dựa trên mô tả sai về bản cũ.

---

### Giai đoạn C — Tự soát và tự đề xuất *(làm đi làm lại được)*

Đây là phần "tự improve". Quy trình:

```bash
python3 tools/icon_contact_sheet.py /tmp/soat.png StatIcons SharedUI HomeChrome Badges Summon Shop ButtonIcons Glyphs EffectBuffs/icons
```

Mở `/tmp/soat.png` và soi, tìm đúng **ba loại lỗi** sau — không tìm "đẹp/xấu" chung chung:

1. **Một khái niệm, hai hình.** Cùng một thứ mà hai màn vẽ hai kiểu. Người chơi phải học
   hai lần. *(Đã tìm thấy và đã xử lý: relic, chìa khoá. Còn lại: vàng — `UiRefresh/gold`
   túi tiền vs `Shop/gold` đống xu vs `HomeChrome/coin` đồng xu.)*
2. **Lạc phong cách.** Viền mảnh, không viền đen, tông cyan nhạt, hoặc vật thể hiện đại
   trong thế giới fantasy.
3. **Nát ở cỡ thật.** Thu còn 48px mà không đọc ra hình chính.

Tìm được thì ghi vào `chat/chatgpt.md` theo mẫu:

```
## ĐỀ XUẤT · ChatGPT → Claude · <số> · <ngày>
- Icon: <đường dẫn>
- Loại lỗi: 1 / 2 / 3
- Bằng chứng: <đo được gì, ở cỡ nào>
- Đề nghị: giữ bản nào / vẽ lại thế nào
```

**Đừng tự vẽ lại trước khi người dùng gật.** Luật 2.2: chỉ thay cái thật sự hỏng.

### Đã soát và ĐẠT — đừng đụng

`EffectBuffs/icons` · `ButtonIcons` · `Glyphs` · `Badges` · `ShopPacks` · `ButtonsV2` ·
`SmallButtons` · `StatIcons` · **bộ icon thanh Home** (người dùng đã quyết giữ bản cũ,
04/11 — đã thử thay hai lần và đều bị từ chối).

---

## 5. Phần Claude làm, đừng đụng vào

- Mọi file `.cs`.
- Hợp nhất icon trùng ở mức **mã** (đổi đường dẫn nạp).
- Giai đoạn hoạt ảnh (`ANIM-001…006` trong `doc/WORK_QUEUE.md`) — toàn bộ là mã.
- `doc/UI_PLAN.md`, `doc/WORK_QUEUE.md`, `doc/UI_ART_SPEC.md`.

Xong một giai đoạn thì ghi OUTPUT vào `chat/chatgpt.md` rồi để người dùng đưa Claude xem.
