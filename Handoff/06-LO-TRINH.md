# 06 — Lộ trình 8 milestone

Nguyên tắc: **mỗi milestone phải chạy được và nhìn thấy được.**
Không có milestone nào kiểu "viết xong 5 class chưa test".

Không có deadline trong file này — bạn đang học, tốc độ không quan trọng bằng
việc hiểu. Nhưng **thứ tự thì quan trọng**, đừng đảo.

---

## M1 — Dựng khung, chưa có game

**Làm gì**

- Tạo project Unity 2D mới, `.gitignore`, commit đầu tiên
- Dựng cây thư mục (file 03)
- Copy `Gacha/`, `Rarity.cs`, `UnitTeamKind.cs`
- Viết `GameBalance.cs`, tạo asset, chỉnh thử vài số trong Inspector
- Viết `UnitDefinition.cs` gọn, tạo **1** unit asset

**Xong khi**

```
□ Project mở không lỗi Console
□ Có 1 file GameBalance.asset chỉnh được bằng Inspector
□ Có 1 file UnitDefinition.asset
□ git log có ít nhất 3 commit
```

**Học được**: ScriptableObject, cấu trúc project, git cơ bản.

---

## M2 — Một nhân vật động đậy

> Milestone quan trọng nhất cả lộ trình. Đừng vội qua M3.

**Làm gì**

- Copy 28 frame `FramesV2` từ bản cũ sang
- Import đúng chuẩn: PPU 128, pivot `(0.5, 0.1875)`
- Dựng prefab 2 tầng: root + con `Body`
- Tạo 5 clip **bằng tay** trong cửa sổ Animation
- Đặt 2 Animation Event lên clip Attack
- Viết `IUnitVisual.cs` + `SpriteUnitVisual.cs` (code có sẵn ở file 04 mục 6)
- Dựng scene `CharacterLab` với 8 nút bấm

**Xong khi** — đủ 6 cổng duyệt ở file 04 mục 7

```
□ Bấm từng nút, nhân vật phát đúng animation
□ Chạy 0.25×, xác nhận event rơi đúng frame chạm
□ Debug.Log đếm được: 1 lần Attack = đúng 1 lần OnAttackEvent
□ Dead giữ pose cuối, không đứng dậy
□ Console 0 error
```

**Học được**: Animation Clip, Animation Event, Animator, prefab, sprite import.
Đây là phần bạn đang thiếu nhất — đầu tư thời gian ở đây.

---

## M3 — Hai nhân vật đánh nhau

**Làm gì**

- `Unit.cs` — HP, ATK, faction, `TakeDamage()`, event `Died`
- `UnitBrain.cs` — tìm địch gần nhất, đi tới, vào tầm thì `PlayAttack`
- Nối `onImpact` → `target.TakeDamage()`
- Scene test: 1 ally, 1 enemy, không UI, không lưới

**Xong khi**

```
□ Hai con đi tới nhau, đánh nhau, một con chết và giữ pose nằm
□ Damage rơi đúng lúc vũ khí chạm (nhìn ở 0.25×)
□ Con thắng quay về Idle, không đứng đánh vào không khí
```

**Học được**: vòng lặp Update, vector, callback, event.
**Ước chừng**: `Unit.cs` ~120 dòng, `UnitBrain.cs` ~180 dòng.

---

## M4 — Lưới và kéo thả

**Làm gì**

- `BattleGrid.cs` — **chỉ** chuyển đổi `(row, col) ↔ Vector3`, không vẽ gì
- Vẽ lưới bằng **sprite trong scene**, không bằng code
- `UnitDragHandler.cs` — kéo thả lính giữa các ô
- Spawn vài lính cố định để test

**Xong khi**

```
□ Mở scene trong Editor NHÌN THẤY lưới (không cần Play)
□ Kéo lính từ ô này sang ô khác, lính snap đúng tâm ô
□ Không kéo được vào ô đã có lính
```

**Học được**: chuyển đổi toạ độ, input chuột/chạm, quan trọng hơn cả là
**thói quen dựng hình trong Editor thay vì bằng code**.

---

## M5 — Gacha nối vào game

**Làm gì**

- Dựng `RollPanel` UI **trong Editor** (6 ô, nút Roll, hiển thị coin)
- Nối vào `RollGenerator` / `RollResolver` đã copy ở M1
- Ô Ally → spawn lính thật lên lưới
- `MergeSystem.cs` — 3 cùng dòng cùng rarity → gộp

**Xong khi**

```
□ Bấm Roll trừ 20 coin, hiện 6 ô, ra thưởng đúng bảng ở file 01 mục 3.3
□ Ô Ally sinh lính thật lên bàn
□ 3 lính giống nhau tự gộp thành 1 con rarity cao hơn
□ Hết coin thì nút Roll tự disable
```

**Học được**: UI trong Editor, binding code ↔ UI, event.

---

## M6 — Vòng lặp trận hoàn chỉnh

**Làm gì**

- `BattleFlow.cs` — máy trạng thái 6 state (file 03 mục 5)
- Nút Start → countdown 2s → Combat
- Đếm wave, wave clear +40 coin, hết 10 wave → Level clear
- Thua → màn hình Defeat
- Thêm `.asmdef` cho `Gacha/` và `Data/`

**Xong khi**

```
□ Chơi liền mạch: prep → roll → xếp → đánh → wave 2 → ... → wave 10
□ Thua được, thắng được, cả hai đều có màn hình kết thúc
□ Bấm Restart chơi lại sạch (buff không rò từ trận trước)
```

**Học được**: state machine, coroutine, kiến trúc hệ thống.

---

## M7 — Meta và save

**Làm gì**

- Scene `Home` — hiện gold, key, chọn level
- `SaveSystem` JSON (file 03 mục 6)
- Level clear → +gold, +key, mở khoá level kế
- Chuyển scene Home ↔ Battle

**Xong khi**

```
□ Thoát game mở lại, gold/key/level còn nguyên
□ Mở được file save.json bằng text editor, đọc hiểu được
□ Level 2 khoá cho tới khi clear level 1
```

**Học được**: scene management, serialization, persistence.

---

## M8 — Mở rộng roster

> Chỉ tới đây mới được làm nhân vật thứ hai. **Không sớm hơn.**

**Làm gì**

- Nhân vật thứ 2 (enemy melee) theo đúng pipeline M2
- Nhân vật thứ 3 (ally ranged) + đạn
- Bây giờ mới nhờ AI viết Editor script tự động hoá bước tạo clip
- 4 rarity của cùng một dòng: đổi màu/trang bị, **dùng chung animation**

**Xong khi**

```
□ 3+ nhân vật, tất cả qua cổng duyệt Character Lab
□ Editor script tạo clip tự động chạy đúng
□ Combat ranged hoạt động (dừng ở tầm, bắn đạn)
```

---

## Sau M8

Những thứ cố tình để lại sau cùng, vì chúng **không dạy bạn gì mới** và dễ
làm bạn sa đà:

- Âm thanh
- VFX, particle, camera shake
- Skill riêng cho Epic/Legendary
- Màn hình Upgrade / Treasure
- Cân bằng số liệu thật sự
- Build ra máy điện thoại

---

## Bảng theo dõi

```
M1  Khung project           □
M2  Một nhân vật động đậy   □   ← đầu tư nhiều nhất ở đây
M3  Hai con đánh nhau       □
M4  Lưới + kéo thả          □
M5  Gacha                   □
M6  Vòng lặp trận           □
M7  Meta + save             □
M8  Mở rộng roster          □
```

---

## Ba cái bẫy

**1. Nhảy sang M5 vì gacha "vui hơn" M2.**
Gacha đã có code sẵn, copy là xong — nó không dạy bạn gì. M2 mới là chỗ bạn
đang thiếu kiến thức. Làm M2 cho đến khi thật sự hiểu.

**2. Làm 10 nhân vật ở M2 vì "đang quen tay".**
Đúng một con. Nhân bản khi chưa chắc pipeline = nhân bản cả lỗi.

**3. Tối ưu sớm.**
Object pooling, ECS, Addressables ở M3 là lãng phí. Game của bạn có ~20 lính
trên màn hình. Unity xử lý được gấp trăm lần thế mà không cần tối ưu gì.
