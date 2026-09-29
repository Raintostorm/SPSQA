# 07 — Tự làm vs nhờ AI

Bản cũ hỏng phần lớn vì AI viết quá nhiều mà không ai nắm được toàn cảnh —
kể cả bạn, kể cả AI. File này là ranh giới để điều đó không lặp lại.

---

## 1. Câu hỏi quyết định

> **"Nếu code này hỏng lúc 11 giờ đêm, tôi có tự sửa được không?"**

- Có → nhờ AI viết cũng được
- Không → **tự viết**, dù chậm

Đơn giản vậy thôi. Mọi thứ trong bảng dưới đều suy ra từ câu này.

---

## 2. Bạn tự viết 🧑‍💻

| Việc | Vì sao |
|---|---|
| `Unit.cs`, `UnitBrain.cs` | trái tim game, bạn sẽ sửa nó hàng trăm lần |
| `BattleFlow.cs` | hiểu sai state machine là hỏng cả kiến trúc |
| `MergeSystem.cs` | luật chơi của riêng bạn |
| `BattleGrid.cs` | toán toạ độ, phải tự thấy nó rõ trong đầu |
| Dựng scene, prefab, UI trong Editor | **AI không làm được**, và đây là kỹ năng Unity cốt lõi |
| Tạo AnimationClip **nhân vật đầu tiên** | phải tự tay làm một lần mới hiểu |
| Mọi thứ ở M2, M3, M4 | ba milestone dạy nhiều nhất |

---

## 3. Nhờ AI 🤖

| Việc | Vì sao |
|---|---|
| Editor script tạo clip hàng loạt (**từ nhân vật thứ 3**) | lặp lại thuần tuý |
| Batch import/rename/resize sprite | lặp lại thuần tuý |
| Script Python cắt sprite sheet, kiểm canvas/pivot | ngoài Unity, không ảnh hưởng kiến trúc |
| **Giải thích** API Unity | nhanh hơn đọc docs |
| **Review** code bạn vừa viết | rất đáng, xem mục 5 |
| Tìm bug từ log lỗi | AI đọc stack trace nhanh |
| Viết unit test cho `Gacha/` | test là code lặp lại điển hình |
| Chuyển `MetaProgressionStore` cũ sang JSON | dịch máy móc từ code có sẵn |

---

## 4. Ranh giới mờ — quyết theo tình huống

| Việc | Cách xử lý |
|---|---|
| UI code (binding, cập nhật text/số) | AI viết bản đầu → **bạn đọc hiểu từng dòng** trước khi giữ |
| Save/load | AI viết, nhưng bạn tự thiết kế cấu trúc `SaveData` |
| Combat VFX | để sau M8, lúc đó nhờ AI thoải mái |

---

## 5. Cách nhờ AI đúng

### ❌ Sai

> "Viết cho tôi hệ thống combat"

Bạn sẽ nhận 800 dòng không hiểu gì, và đó chính là cách `DuelDirector`
phình lên 5.765 dòng.

### ✅ Đúng — 4 kiểu prompt

**Kiểu A — Giải thích**

> Giải thích `AnimationUtility.SetObjectReferenceCurve` trong Unity 6.
> Tham số `EditorCurveBinding.PPtrCurve("Body", typeof(SpriteRenderer), "m_Sprite")`
> nghĩa là gì? Đừng viết code hộ tôi, tôi muốn hiểu để tự viết.

**Kiểu B — Review**

> Đây là `UnitBrain.cs` tôi tự viết. Chỉ ra bug và chỗ khó bảo trì.
> **Đừng viết lại hộ tôi** — nói tôi sai ở đâu và vì sao.
>
> ```csharp
> [dán code]
> ```

**Kiểu C — Việc lặp lại, có giới hạn rõ**

> Viết một Editor script Unity 6 làm **đúng** việc sau, không hơn:
> đọc mọi PNG trong `Art/Characters/<tên>/Walk/`, đặt Sprite mode Single,
> PPU 128, pivot custom (0.5, 0.1875), rồi tạo một AnimationClip loop
> 8 frame trỏ vào child tên `Body`.
> Giới hạn: **dưới 120 dòng**, một file, không tạo prefab, không tạo scene.

**Kiểu D — Debug**

> Lính của tôi ở trạng thái Walk nhưng chỉ dậm chân tại chỗ, không qua frame 2.
> Đây là `SpriteUnitVisual.cs` và cách `UnitBrain` gọi nó.
> Nguyên nhân là gì?
>
> ```csharp
> [dán code]
> ```

### Ba luật khi nhờ AI

1. **Luôn kèm giới hạn dòng.** "Dưới 120 dòng" ép AI giữ phạm vi.
2. **Luôn nói rõ việc gì KHÔNG làm.** "Không tạo prefab, không sửa file khác."
3. **Không dán code AI viết mà chưa đọc hết.** Không hiểu dòng nào → hỏi lại
   dòng đó. Đây là luật quan trọng nhất.

---

## 6. Ba dấu hiệu bạn đang nhờ AI quá nhiều

1. Có file bạn **không dám sửa** vì sợ gãy.
2. Có file bạn **không nhớ nó làm gì**.
3. Bạn sửa bug bằng cách **dán lỗi cho AI** thay vì tự đọc code.

Gặp bất kỳ dấu hiệu nào → **dừng lại, đọc file đó từ đầu đến cuối, viết lại
bằng tay.** Chậm hơn hôm nay, nhanh hơn rất nhiều trong ba tháng tới.

---

## 7. Mẫu prompt mở session mới

Copy nguyên khối này khi bắt đầu chat mới:

```
Tôi đang làm lại game auto-battler 2D bằng Unity 6 + C#, TỰ CODE để học.
Đọc folder Handoff/ trước (00 → 08) để nắm bối cảnh.

Tôi đang ở milestone M__, cụ thể đang làm: ______________

Vai trò của bạn:
- Giải thích và review, KHÔNG viết hộ logic gameplay
- Chỉ viết code khi tôi nói rõ "viết hộ tôi", và phải kèm giới hạn phạm vi
- Nếu tôi hỏi cách làm, hãy chỉ hướng để tôi tự làm, đừng đưa code hoàn chỉnh

Quy tắc project:
- Không file nào quá 400 dòng
- UI dựng trong Editor, không dựng bằng new GameObject()
- Một file = một việc, mô tả được bằng một câu không có chữ "và"
```
