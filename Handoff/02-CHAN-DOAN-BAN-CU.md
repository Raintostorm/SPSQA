# 02 — Bản cũ hỏng ở đâu

Không phải để chê. Để bạn nhận ra **dấu hiệu sớm** và dừng lại trước khi
bản mới đi vào đúng vết xe đó.

---

## Hỏng #1 — God class `DuelDirector.cs`

```
5.765 dòng · 147 method · 159 chỗ new GameObject/AddComponent · 1 file
```

Một class ôm hết:

- máy trạng thái trận (prep / combat / wave clear / defeat)
- spawn ally + enemy
- vòng lặp combat
- kéo thả lính
- luật merge
- kinh tế coin
- **và dựng toàn bộ UI bằng code**

### Vì sao nó giết project

Sửa một thứ gãy thứ khác, vì mọi thứ dùng chung state trong cùng một class.
Bạn không thể test riêng phần nào. AI cũng không đọc nổi toàn bộ file trong
một lần, nên mỗi lần sửa nó chỉ nhìn được một mảnh → sinh ra bản vá chồng bản vá.

### Dấu hiệu sớm ở bản mới

> Một file chạm **400 dòng** → dừng lại, tách ngay. Không chờ đến 1.000.

---

## Hỏng #2 — Scene rỗng, UI dựng bằng code

Mọi file `.unity` chỉ **6–8 KB**. Scene thật của một game Unity phải nặng
hàng trăm KB đến vài MB.

Nghĩa là scene gần như trống, mọi thứ được `new GameObject()` lúc runtime.
Hậu quả:

- Mở scene trong Editor → **không thấy gì cả**
- Không dùng được Inspector để chỉnh số
- Không dùng được prefab đúng nghĩa
- Muốn đổi vị trí một nút → phải sửa code, compile, Play, nhìn, sửa lại
- Mất hoàn toàn lợi thế của Unity

### Quy tắc bản mới

> **UI và layout thuộc về Editor.** Code chỉ *điền dữ liệu* và *bắt sự kiện*.
> Nếu bạn đang viết `new GameObject("Button")` — bạn đang làm sai.

---

## Hỏng #3 — Ba pipeline nhân vật chồng nhau, không cái nào xong

| Pipeline | Vị trí | Hướng | Trạng thái |
|---|---|---|---|
| SpinAssetLab Milestone1 | `Assets/SpinSquad/SpinAssetLab/` | mocap CMU `.amc` → skeleton | bỏ dở |
| SpinForge | `Assets/SpinSquad/SpinForge/` | factory ghép part rời | bỏ dở |
| Characters/Human* | `Assets/SpinSquad/Characters/` | sprite từng frame | **chạy được** |
| Spine | `Assets/Spine/` (~130 file) | runtime Spine | import rồi không dùng |

Bốn hướng cùng lúc, không hướng nào tới đích. Cộng với việc từng thử 3D rồi
reset về 2D — project mang theo xác của mọi lần thử.

### Quy tắc bản mới

> **Chọn đúng một pipeline. Làm xong một nhân vật. Rồi mới bàn cái thứ hai.**
> File 04 chọn sẵn cho bạn: **sprite từng frame**.

---

## Hỏng #4 — 32 file markdown mâu thuẫn

Rải ở 3 nơi: root, `Docs/`, `Assets/SpinSquad/Docs/`.

Có file nói 3D, có file nói 2.5D, có file nói 2D. Có file tự ghi ở đầu
*"Mapping trong tài liệu này đã bị thay thế..."*. Có `CURRENT_WORK_HANDOFF.md`
dài 200 dòng bàn về hai con robot **đã bị xoá khỏi project**.

### Quy tắc bản mới

> Tài liệu **sống** thì cập nhật, **chết** thì xoá. Không để file lỗi thời nằm đó.
> Tối đa ~8 file md. Đúng bằng folder `Handoff/` này.

---

## Hỏng #5 — Fallback che mất lỗi

Trong `DuelActor`:

```csharp
// Combat must remain functional even when a unit is temporarily missing
// its presentation driver. The animation is optional; the hit is not.
if (!TryPlayLine1AttackAnimation(onHit, onComplete))
{
    onHit?.Invoke();       // vẫn gây damage dù không có animation
    onComplete?.Invoke();
}
```

Ý tưởng đúng về mặt kỹ thuật. Nhưng hậu quả thực tế: **lính không có
animation vẫn đánh bình thường**, nên bạn không biết prefab đang thiếu driver.
Cả roster rơi về hình khối hình học mà game vẫn "chạy".

### Quy tắc bản mới

> Giữ fallback, nhưng **kèm `Debug.LogWarning` một lần mỗi prefab**.
> Im lặng nuốt lỗi là cách bug sống lâu nhất.

---

## Hỏng #6 — Nặng vì thư mục, không phải vì game

| | |
|---|---|
| Tổng thư mục | **14 GB** |
| `Tools/LocalAssetAI/` (ComfyUI + model AI) | 7,5 GB |
| `Library/` (cache Unity, xoá được) | 3 GB |
| `.git/` | **2,1 GB** ← bất thường |
| File `.xapk` game gốc tham khảo | 1,1 GB |
| **Code + asset thật của game** | **~115 MB** |

`.git` 2,1 GB cho 2.366 file / 37 commit là do lịch sử từng chứa file lớn:
`.psd` 7,8 MB, `.kra` 2,1 MB, hàng loạt PNG 2 MB. Git giữ vĩnh viễn mọi phiên
bản của chúng — xoá file bây giờ **không** làm `.git` nhẹ đi.

Cách xử lý ở file `08-DON-DEP-REPO.md`.

---

## Tóm tắt: 6 quy tắc rút ra

1. File chạm 400 dòng → tách.
2. UI dựng trong Editor, không dựng bằng code.
3. Một pipeline, một nhân vật, làm cho xong.
4. Tài liệu chết thì xoá.
5. Fallback phải kêu, không được im.
6. Không bao giờ commit file nhị phân lớn.
