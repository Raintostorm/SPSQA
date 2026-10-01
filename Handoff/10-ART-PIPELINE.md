# 10 — Pipeline art 2D

> Cập nhật 2026-10-01. Viết lại — bản trước mô tả quy trình sinh **frame
> animation** bằng Gemini, đã bị thay bằng sinh **part rời** cho Spine.

---

## 1. Nguyên tắc: sinh PART, không sinh FRAME

Bài học đắt nhất của dự án: **AI không sinh được chuỗi frame animation nhất
quán.** Đã thử 4 công cụ:

| Công cụ | Kết quả |
|---|---|
| ChatGPT / DALL-E | ra nhân vật KHÁC hẳn ảnh tham chiếu | ❌ |
| Leonardo (Auto model) | ra nhân vật KHÁC hẳn | ❌ |
| Gemini **image editing** | giữ được nhân vật, lệch 0px giữa 2 lần sinh | ✅ |
| Gemini **sinh sheet nhiều frame** | 9 ô → chỉ 3 tư thế thật sự khác nhau | ❌ |

Kể cả Gemini editing — thứ duy nhất giữ được nhân vật — cũng **không tạo nổi
một walk cycle đọc được**: xin 9 frame thì 6 frame trùng nhau.

**Giải pháp**: bỏ hẳn animation vẽ tay. Sinh **part rời**, rig trong Spine,
animation sinh ra từ xoay/dịch xương. Một bộ part → vô hạn tư thế.

---

## 2. Quy tắc sinh ảnh

### Ba điều KHÔNG THỂ sửa ở hậu xử lý

| | Vì sao |
|---|---|
| **Part phải RỜI NHAU** | script tách theo mảng liên thông. Dính nhau = một mảng |
| **Nền magenta `#FF00FF`** | màu không có trong bảng màu nhân vật → tách sạch. KHÔNG dùng trắng (mặt màu kem) |
| **Tư thế / nội dung** | script không bịa thêm được |

### Mọi thứ khác script lo

nền · ngôi sao rác · chữ rác · kích thước · viền tím khử răng cưa · đặt tên

### Khung prompt

```
Generate ONE image containing EXACTLY <N> ... 
<N> items. Not one. Not two. <N>.

<mô tả nội dung từng ô>

SHAPE / STRUCTURE:
- <ràng buộc hình học, bằng SỐ không bằng tính từ>
- <liệt kê thẳng những thứ KHÔNG được vẽ>

STYLE:
- AT MOST <k> flat color areas per item.
- NO emblems, NO logos, NO patterns, NO belts, NO buckles, NO straps,
  NO pouches, NO rivets, NO panel lines, NO trim.
- Very thick, heavy, uniform black outline on every shape.
- Flat solid colors ONLY. No gradients, no shading, no highlights, no texture.
- Will be displayed about 100 pixels tall, so keep detail minimal.

Flat pure magenta background #FF00FF filling the whole image.
No ground shadow. No sparkles, stars, glows or particles.
No text, letters, numbers, labels or watermarks.
No grid lines, no cell borders, no dividers.
```

### Bốn mẹo đã kiểm chứng

**① Lặp lại số lượng.** `EXACTLY FOUR ... Not one. Not two. Four.`
Model rất hay gộp về một nhân vật.

**② Trỏ vào phần nó đã làm đúng.**
> *"The ARM must be a separate floating shape, **exactly like the head is
> separate**."*

Cụ thể hơn mọi mô tả trừu tượng.

**③ Chỉ đích danh chỗ hỏng.** Đo được tay dính ở vùng vai (`y=600..645`) →
viết `INCLUDING ABOVE IT at the shoulder`.

**④ Mô tả hình bằng VẬT THỂ và SỐ, không bằng tính từ.**
"slim" ra mỗi lần một kiểu. `like a bowling pin` + `about 20% taller than wide`
thì đo được, và câu phủ định `do NOT make it a wide squat dome` chặn đúng cái
nó vừa vẽ sai.

---

## 3. Script tách part

`tools/split_sheet.py` — 3 chế độ:

```bash
# Sheet nhân vật: mỗi ô 3 mảng → head/body/hand
python3 tools/split_sheet.py sheet.png -o out/ --names a b c d

# Sheet 1 hàng, mỗi ô 1 món → file phẳng
python3 tools/split_sheet.py armor.png -o artsrc/armor \
  --part armor --names stoneage medieval modern future

# Sheet dạng lưới → hang_cot.png
python3 tools/split_sheet.py weapon.png -o artsrc/weapon \
  --grid 3x4 --rows damage range tank --cols stoneage medieval modern future
```

### Nó làm gì

| | |
|---|---|
| Quét **mảng liên thông** | ngôi sao rác và pixel lạc tự bị loại (chỉ giữ mảng > 300px) |
| **Bào mòn 2px** | pixel khử răng cưa của ảnh gốc là màu PHA giữa nhân vật và magenta — không đủ hồng để bị loại nên lọt vào part, tạo viền tím. Viền đen dày ~10px nên bỏ 2px không nhìn thấy |
| Crop **theo mảng**, không theo khung bao | bounding box của `body` chồng lên `arm`, nhưng tay không dính vào ảnh thân |
| Xuất `offsets.json` | vị trí tương đối giữa các part, để rap lại trong Spine |

> `offsets.json` là **điểm xuất phát**, không phải luật. Bố cục trong ảnh nguồn
> chỉ phục vụ việc cắt — bạn tự đặt lại trong Spine.

### Kiểm nhanh sau mỗi lần chạy

Script in ra số mảng tìm được. Phải khớp số mong đợi:

```
sheet nhân vật 4 con × 3 part  →  12 mảng
sheet giáp / mũ 4 món          →   4 mảng
sheet vũ khí 3×4               →  12 ô có nội dung
```

Lệch là có chỗ dính — nhìn ảnh tìm chỗ hai hình chạm nhau.

---

## 4. Bộ art hiện có

```
artsrc/sheets/     5 sheet gốc, giữ để chạy lại
artsrc/base/       head · body · hand        (nhân vật trần)
artsrc/armor/      4 thời đại
artsrc/helmet/     4 thời đại
artsrc/weapon/     12 món (3 vai × 4 thời đại)
```

**5 lượt sinh ảnh → 35 file part → 4×4×12 = 192 diện mạo.**

Đặt ngoài `Assets/` vì đây là **nguồn** cho Spine. Thứ vào Unity là file export
của Spine (`.json` + `.atlas.txt` + `.png`).

---

## 5. Ràng buộc cho mọi bộ art sau

Mọi nhân vật dùng chung skeleton phải cắt thành **đúng cùng bộ part, cùng tên**:

```
head · body · hand
```

Trang bị là slot riêng: `armor` (thay body) · `helmet` (đè head) · `weapon`
(gắn vào arm).

Thứ tự vẽ (sau ra trước):

```
body → armor → head → helmet → hand → weapon
```

`weapon` vẽ **sau** `hand` nên tự che tay khi cần — cầm đá thì không thấy tay,
đúng như Forge Master.

---

## 6. Tỉ lệ

Đo từ ảnh gameplay Forge Master và từ bộ art đã sinh:

| | |
|---|---|
| Đầu / thân (rộng) | ~50% — thân là khối chính, đầu nhỏ |
| Thân cao / rộng | ~1.2 (cao hơn rộng chút) |
| Tay dài | ~50% chiều cao thân |
| Tay dày | ~20% bề rộng thân |

> **Tỉ lệ KHÔNG phải thứ cần giải ở khâu sinh ảnh.** Mỗi attachment trong Spine
> có `ScaleX/ScaleY` riêng, lưu theo từng skin. Thu đầu nhỏ 20% là kéo một tay
> cầm. Đừng đốt lượt sinh để chỉnh tỉ lệ.
>
> Thứ **chỉ** sửa được ở khâu sinh: part có rời nhau không, màu có phẳng không,
> có chi tiết thừa không.
