# 09 — Quyết định thiết kế (ghi đè Handoff 00–08)

> Chốt ngày 2026-09-29, trong quá trình làm M1.
> Folder `Handoff/` 00–08 chép số từ **code bản cũ đã chạy**.
> File này ghi những chỗ **thiết kế mới khác bản cũ**.
> **Khi mâu thuẫn, file này thắng.**

---

## 1. Bảng ghi đè

| Điểm | Handoff 00–08 nói | Chốt lại |
|---|---|---|
| Hệ số stat theo rarity | `BaseHp[]`/`BaseAtk[]` nhân với hệ số dòng lính | **Bỏ công thức.** Mỗi lính khai tay `BaseHp` + `BaseAttack` |
| Phân loại lính | ngũ hành Mộc/Hỏa/Kim/Thủy/Thổ, `LineIndex:int` | **5 chủng tộc**: Human, Elf, Necro, Robot, Beast → `enum Race` |
| Lưới mỗi phe | 4 hàng × 2 cột = 8 ô | **4 × 4 = 16 ô** |
| Sức chứa 1 ô (ally) | tối đa 1 lính | **3 lính** (Common→Legendary), hoặc **1 Mythic** |
| Túi lính (bench) | 4×4 = 16 slot | **Bỏ hẳn.** Bench vốn để đựng weapon, không phải ally |
| Merge | tự động khi đủ 3 con trên bàn | **Thủ công**, bấm vào ô |
| Bán lính | 12 coin cố định | **Mảng theo rarity**: 2 / 5 / 10 / 20 / Mythic không bán |
| moveSpeed | 1.35 chung | **Riêng từng lính**; GameBalance chỉ giữ mốc mặc định |
| Số wave mỗi level | cố định 10 | **Tuỳ level** → sau này cần `LevelDefinition` |
| Buff AtkSpeed | chia interval đếm giờ | **Phải tua nhanh animation** (`animator.speed`), bội số 1/x |
| Thưởng cụm 6 ô | 200 coin | **200** (giữ nguyên) |

---

## 2. Cấu trúc lính

**Mỗi cặp `(Race, Rarity)` ứng với ĐÚNG MỘT `UnitDefinition`.**

```
5 race × 4 rarity = 20 lính gốc      (Mythic tính riêng)
```

Hệ quả:

- "Human Rare" là **một** con duy nhất. Kiểu đánh melee/ranged là thuộc tính
  **cố định** của nó, **không** tham gia định danh.
- So sánh hai lính có "trùng" nhau không → so **tham chiếu `UnitDefinition`**,
  không cần so từng field.
- Pool mỗi rarity có **5 ứng viên** (một per race).

---

## 3. Luật merge

| | |
|---|---|
| Kích hoạt | **thủ công** — bấm vào ô có đủ 3 lính |
| Điều kiện | 3 con **cùng chủng tộc + cùng rarity** (đúng bằng lúc ô đầy) |
| UI hiện ra | 3 nút: **ⓘ** thông tin · **Sell** · **Merge** |
| Kết quả | **1 lính NGẪU NHIÊN** ở rarity kế tiếp — random trong 5 race |
| Vị trí kết quả | nằm lại **chính ô đó** |

> Merge ra con ngẫu nhiên chính là ý nghĩa chữ "spin" trong tên game.
> Người chơi **không nhắm được** con cụ thể.

**Hệ quả kiến trúc**: không dùng field `NextRarityUnit` trỏ 1-1.
Cần một **pool tra theo rarity** — và gacha (ô Ally) cần đúng cấu trúc đó.
Một `UnitPool` phục vụ cả hai. Làm ở M5, không làm sớm.

**Tầng đúng**: logic **trả lời**, UI **hỏi**.

```
UI hỏi:    "ô này merge được không?"  → MergeSystem trả true/false → hiện/ẩn nút
Người bấm: "merge ô này"              → MergeSystem thực hiện
```

`MergeSystem` **không được** đi tìm cái nút để ẩn nó.

---

## 4. Luật Mythic

| | |
|---|---|
| Chiếm chỗ | **trọn một ô**, không lính nào khác vào chung |
| Giới hạn mỗi trận | **2** lúc đầu, buff treasure tăng thêm |
| Cách đếm | theo **tổng số Mythic** trên sân; trùng loại vẫn tính |
| Bán / phân giải | **không được** — UI không hiện nút Sell |
| Khi đủ giới hạn | **chỉ ô Legendary** ẩn nút Merge; rarity dưới merge bình thường |

---

## 5. Luật đặt lính & roll

**Spawn lính mới**: ưu tiên nhét vào ô đã có lính **trùng** và **chưa đủ 3**.
Mọi ô trùng đều đầy → tìm ô trống.

**Bàn đầy**: phải **bán** một ô cho trống mới roll tiếp. Nút Roll **tối đi**.

> Cần chốt: "bàn đầy" = **không còn ô trống nào** (cách đọc an toàn, không bao
> giờ kẹt), hay tính theo chỗ trống trong ô? Chưa quyết dứt khoát.

---

## 6. Kinh tế

| | |
|---|---|
| Roll trong trận | **Coin** (chỉ tồn tại trong trận) |
| Roll treasure | **Key** (tài nguyên meta) |
| Ngoài trận | **Gold** |

**Giá bán cố ý rẻ** (2–20 so với 20 coin một lần roll): bán là lối thoát khi
bàn đầy, **không phải** chiến thuật kiếm tiền.

**Thưởng wave đặc biệt** (handoff không nhắc):

- wave chia hết 5 nhưng lẻ (5, 15, 25…) → **1 weapon**
- wave chia hết 10 (10, 20, 30…) → **1 weapon + 1 buff mạnh** (mức buff 6-roll Legendary)

**Weapon**: gắn với ally bậc 5 (Mythic). Để sau M7.

---

## 7. Quy tắc dữ liệu rút ra

1. **Đừng suy dữ liệu từ tên.** `Race`/`Rarity` là field khai thẳng.
   `UnitId` chỉ để **đọc** (save, log) — không bao giờ parse ra thứ khác.
   *(Bản cũ suy ngũ hành từ chuỗi id → hàm map phình thành một đống `if`.)*

2. **Cấu trúc dữ liệu lo an toàn, code lo diễn đạt luật.**
   `SellPriceByRarity` có 5 phần tử để không `IndexOutOfRangeException`;
   luật "Mythic không bán" viết thành điều kiện rõ ràng, không dựa vào số 0.

3. **`GameBalance` là CHỈ ĐỌC lúc chạy.** Sửa ScriptableObject lúc runtime
   thì trong Editor nó **ghi thật vào file `.asset`** và ở lại sau khi Stop —
   nhưng khi build ra máy thật lại không ghi được. Bug chỉ xảy ra một bên.
   Giá trị thay đổi trong trận (buff giảm giá roll, crit, tốc đánh) thuộc về
   **`RunState`** — object thường trong RAM, sinh ra mỗi trận.

4. **Thứ tự enum không được đổi.** Giá trị đã serialize vào file `.asset`.
   Chèn giá trị mới vào giữa = mọi asset đổi nghĩa âm thầm.

5. **Ngưỡng dòng là cái chuông, không phải mục tiêu.** `GameBalance` quá
   80 dòng ≈ có số đã len vào nhầm chỗ (số riêng của lính / level / weapon).

---

## 8. Còn treo, chưa quyết

- **Chỉ số thật**: đang dùng placeholder **200 HP / 30 ATK**. User tự trích
  xuất data từ game gốc, sẽ có spec chi tiết sau.
  Mốc khuyến nghị nếu phải tự đặt: **×3 mỗi bậc rarity** — khớp đúng tỉ lệ
  merge 3-con-thành-1.
  Đã tra mạng: SpinSquad (tên cũ **Lucky Offense**) **không có bảng HP/ATK
  công khai**, chỉ có tier list chủ quan.
- **Chủng tộc có tác dụng cơ chế gì không** (khắc chế? cộng hưởng?) — chưa rõ.
  Hiện chỉ dùng làm một nửa điều kiện merge.
- **Có bao nhiêu Mythic** — chưa chốt.
- **"Bàn đầy" định nghĩa chính xác** — xem mục 5.
- **16 ô × 3 lính = 48 lính/phe, 96 sprite trên màn hình dọc.** Con số lý
  thuyết. Chỉ kiểm chứng được ở M3–M4 khi có lính thật.
- **`IUnitVisual` cần thêm method chỉnh tốc độ phát animation** (cho buff
  AtkSpeed). Handoff mục 04.6 chưa có. Lưu ý ở M2:
  `animator.speed` là **một** giá trị cho **cả Animator** — đặt speed=2 để
  đánh nhanh thì clip Walk cũng chạy gấp đôi. Cách xử lý: đặt `animator.speed`
  **mỗi lần đổi state** trong `SpriteUnitVisual.Play()`.
