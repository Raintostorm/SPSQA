# 09 — Quyết định thiết kế (ghi đè Handoff 00–08)

> Cập nhật 2026-10-01. Viết lại toàn bộ — bản trước mô tả 5 chủng tộc và
> merge tự động, cả hai đều đã bị thay.
>
> Folder `Handoff/` 00–08 chép số từ **code bản cũ đã chạy**.
> File này ghi **thiết kế hiện tại**. **Khi mâu thuẫn, file này thắng.**

---

## 1. Bảng ghi đè

| Điểm | Handoff 00–08 | Chốt lại |
|---|---|---|
| Bậc sức mạnh | rarity Common→Rare→Epic→Legendary | **thời đại**: StoneAge → Medieval → Modern → Future (+ Mythic) |
| Phân loại lính | 5 chủng tộc ngũ hành | **3 vai trò**: Damage · Range · Tank |
| Số lính gốc | 5 × 4 = 20 | **3 × 4 = 12** (Mythic riêng) |
| Hệ số stat theo bậc | `BaseHp[]` nhân hệ số dòng lính | **bỏ công thức** — mỗi lính khai tay `BaseHp` + `BaseAttack` |
| Gộp lính | "merge", tự động | **"evolution"**, **thủ công** |
| Lưới mỗi phe | 4 × 2 = 8 ô | **4 × 4 = 16 ô** |
| Sức chứa 1 ô | 1 lính | **3 lính**, hoặc **1 Mythic** |
| Túi lính (bench) | 4×4 = 16 slot | **bỏ hẳn** — vốn để đựng weapon |
| Bán lính | 12 coin cố định | **mảng theo bậc**: 2 / 5 / 10 / 20 / Mythic không bán |
| Pipeline animation | sprite từng frame | **Spine 3.8 (skeletal)** |
| Nhân vật | có chân, có walk cycle | **không chân** — thân/đầu/tay rời |
| Số animation | 5 clip (idle/walk/attack/hit/dead) | **2 clip**: idle + attack |
| Thưởng cụm 6 ô | 200 coin | 200 (giữ) |

---

## 2. Cấu trúc lính

**Mỗi cặp `(UnitRole, Era)` ứng với ĐÚNG MỘT `UnitDefinition`.**

```
3 vai × 4 thời đại = 12 lính gốc      (Mythic tính riêng)
```

| | StoneAge | Medieval | Modern | Future |
|---|---|---|---|---|
| **Damage** | rìu đá | kiếm | dao chiến đấu | kiếm năng lượng |
| **Range** | lao | cung | súng trường | súng năng lượng |
| **Tank** | chuỳ gỗ | chuỳ gai | dùi cui | búa điện |

Hệ quả:
- "Damage Medieval" là **một** con duy nhất.
- So sánh hai lính có trùng nhau không → **so tham chiếu `UnitDefinition`**.
- Pool mỗi thời đại có **3 ứng viên** (một per vai).

### Enum trong code

```csharp
enum Era      { StoneAge=0, Medieval=1, Modern=2, Future=3, Mythic=4 }
enum UnitRole { Damage=0, Range=1, Tank=2 }
```

**Thứ tự KHÔNG được đổi** — `(int)Era` là index tra mảng trong `GameBalance`,
và số đã serialize vào file `.asset`. Chèn giá trị vào giữa = mọi asset đổi
nghĩa âm thầm.

---

## 3. Evolution (trước gọi là merge)

| | |
|---|---|
| Kích hoạt | **thủ công** — bấm vào ô có đủ 3 lính |
| Điều kiện | 3 con **cùng vai + cùng thời đại** (đúng bằng lúc ô đầy) |
| UI hiện ra | 3 nút: **ⓘ** thông tin · **Sell** · **Evolve** |
| Kết quả | **1 lính NGẪU NHIÊN** ở thời đại kế tiếp — bốc 1 trong 3 vai |
| Vị trí kết quả | nằm lại **chính ô đó** |

> Kết quả ngẫu nhiên chính là chữ "spin" trong tên game.
> Người chơi **không nhắm được** con cụ thể.

**Hệ quả kiến trúc**: không dùng field trỏ 1-1 tới bậc kế tiếp.
Cần một **pool tra theo Era** — và gacha (ô Ally) cần đúng cấu trúc đó.
Một `UnitPool` phục vụ cả hai. Làm ở M5.

**Tầng đúng**: logic **trả lời**, UI **hỏi**.

```
UI hỏi:    "ô này evolve được không?"  → EvolutionSystem trả true/false → hiện/ẩn nút
Người bấm: "evolve ô này"              → EvolutionSystem thực hiện
```

`EvolutionSystem` **không được** đi tìm cái nút để ẩn nó.

---

## 4. Luật Mythic

| | |
|---|---|
| Chiếm chỗ | **trọn một ô**, không lính nào khác vào chung |
| Giới hạn mỗi trận | **2** lúc đầu, buff treasure tăng thêm |
| Cách đếm | theo **tổng số Mythic** trên sân; trùng loại vẫn tính |
| Bán / phân giải | **không được** — UI không hiện nút Sell |
| Khi đủ giới hạn | **chỉ ô Future** ẩn nút Evolve; bậc dưới evolve bình thường |

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

**Thưởng wave đặc biệt**:
- wave chia hết 5 nhưng lẻ (5, 15, 25…) → **1 weapon**
- wave chia hết 10 → **1 weapon + 1 buff mạnh** (mức buff 6-roll Future)

**Weapon**: gắn với lính bậc Mythic. Để sau M7.

---

## 7. Nhân vật & animation

### Cấu trúc — không có chân

Lấy cảm hứng từ **Forge Master** (Lessmore GmbH). Lính không có chân, không
đi lại. Thân là một khối bo tròn, đầu nhỏ nằm trên, một cánh tay cầm vũ khí.

```
root
└── main
    ├── body   ← slot: bare / armor_stoneage / armor_medieval / ...
    ├── head   ← slot: head
    │   └── helmet ← slot: none / helmet_stoneage / ...
    └── arm    ← slot: arm
        └── weapon ← slot: damage_stoneage / range_medieval / ...
```

5 xương, 5 slot. Mỗi slot nhiều attachment, đổi bằng `SetAttachment` lúc chạy.

**Giáp THAY THẾ thân**, không đè lên. Thân trần (da) chỉ hiện khi chưa trang bị.

### Chỉ cần 2 clip

| Animation | Cách làm | Cần clip? |
|---|---|---|
| **idle** | đầu nhún 2 keyframe | ✅ |
| **attack** | xoay xương `arm`, event `impact` tại khung vươn xa nhất | ✅ |
| **hit** | đổi `Skeleton.R/G/B` sang trắng 0,1s rồi trả về | ❌ code |
| **dead** | spawn prefab "hồn thoát xác", thân mờ dần (`Skeleton.A`) | ❌ VFX dùng chung |

Hit và dead **dùng chung cho cả roster** — không phụ thuộc hình dáng lính.

### Spine 3.8

Runtime `spine-unity 3.8.0` lấy từ repo cũ, **compile sạch trên Unity 6.4**
(0 lỗi, 56 cảnh báo API lỗi thời). Cảnh báo nói "will be removed in a future
version" → **nâng lên Spine 4.x là việc sẽ phải làm**, không urgent.

---

## 8. Pipeline art

Xem `10-ART-PIPELINE.md`. Tóm tắt:

- Sinh sheet nền **magenta `#FF00FF`**, các part **rời nhau** (không chạm)
- `tools/split_sheet.py` tách thành PNG trong suốt, 3 chế độ
- 5 lượt sinh ảnh → **35 file part** → tổ hợp ra **192 diện mạo**

```
artsrc/base/    head · body · hand
artsrc/armor/   4 thời đại
artsrc/helmet/  4 thời đại
artsrc/weapon/  12 món (3 vai × 4 thời đại)
```

---

## 9. Thuật toán roll 6 ô

### Lỗi đã sửa của bản cũ

Bản cũ ép cả ba số `(coin, ally, buff)` phải ≥1. Hệ quả số học: số lớn nhất
chỉ tới `6-1-1 = 4`.

| Hệ quả | |
|---|---|
| Cụm 5 và 6 ô | **không bao giờ xảy ra** |
| Cờ Jackpot | code chết |
| Hoà (hai loại cùng ≥3) | cần `(3,3,0)` → không tồn tại |
| Roll ra lính bậc cao | **không thể** |

`Handoff/01` mục 3.1 mô tả đúng thuật toán nhưng mục 3.3 lại kẻ bảng thưởng
có hàng 5 và 6 ô. Hai phần trong **cùng một file** mâu thuẫn nhau nhiều tháng.

**Chốt**: mỗi số chạy **0..6**, tổng = 6, vẫn loại `(2,2,2)` → **27 bộ ba**.

### Trọng số: bỏ công thức, dùng bảng

Số **bộ ba** ứng với mỗi số ô Ally là cố định bởi toán học:

```
a=0: 7 bộ ba   a=1: 6   a=2: 4   a=3: 4   a=4: 3   a=5: 2   a=6: 1
```

Với `P(a) ∝ count(a) · r^a`, ràng buộc chặt nhất là `4r³ > 3r⁴` → `r < 1.333`
thì Rare mới thấp hơn Common. Nhưng kéo `r` xuống lại làm "roll không ra ô
Ally nào" tăng vọt. **Một cái vặn, hai thứ dính liền.**

Lý do gốc: `r^a` là hàm **đơn điệu** — chỉ *nghiêng đường thẳng*, không
*uốn cong*. Thứ cần là một **cái bướu**: thấp ở cả hai đầu, cao ở giữa.

**Giải pháp**: bảng trọng số theo số ô Ally — 7 số độc lập.

```csharp
public int[] AllyCountWeights = { 1, 5, 20, 40, 24, 8, 2 };  // index = số ô Ally 0..6
```

Sau chuẩn hoá, **mảng đó CHÍNH LÀ phân bố**.

| | |
|---|---|
| Không ra ô Ally (coin/buff thắng) | 26% |
| StoneAge (3 ô) | **40%** |
| Medieval (4 ô) | **24%** |
| Modern (5 ô) | **8%** |
| Future (6 ô) | **2%** |

### Ba điểm kỹ thuật bắt buộc nhớ

**① Phải chia cho `count(a)`.** Thanh thước weighted-random dựng từ **từng bộ
ba**, không phải từng nhóm. Gán thẳng `w = W[a]` thì nhóm `a=3` (4 bộ ba) ăn
`40×4 = 160` trong khi `a=6` (1 bộ) chỉ được 2 → sai gấp 4 lần.

```
w = Math.Max(1, W[a] * 1000 / countByAlly[a])
```

**② `* 1000` là bắt buộc.** `w` là `int`. `a=0` có `W=1`, `count=7` →
`1/7 = 0`, nhóm đó **biến mất**. Nhân trước khi chia thì `1000/7 = 142`.

**③ Đếm phải cùng điều kiện lọc.** `countByAlly[a]++` đặt **sau** cả hai câu
`continue`. Đặt trước thì `a=2` đếm ra 5 thay vì 4 (vì `(2,2,2)` bị loại).

### Cạm bẫy

`Math.Max(1, ...)` khiến **không thể tắt hẳn một mức** bằng trọng số 0 —
`Max(1, 0)` = 1, mức đó vẫn xảy ra ~0,001%.

### Hướng phụ thuộc

`SixSlotRollGenerator` là **C# thuần**, test được không cần Unity. Giữ như vậy:
generator **nhận** trọng số qua tham số, **không đi tìm** `GameBalance`.

### Đã kiểm chứng

Mô phỏng 200.000 lượt bằng đúng thuật toán (kể cả chia số nguyên):
`1,0 / 5,0 / 19,9 / 40,0 / 24,1 / 8,0 / 2,0` — lệch tối đa 0,006 điểm %.

---

## 10. Quy tắc dữ liệu rút ra

1. **Đừng suy dữ liệu từ tên.** `UnitRole`/`Era` là field khai thẳng.
   `UnitId` chỉ để **đọc** (save, log) — không bao giờ parse ra thứ khác.
   *(Bản cũ suy ngũ hành từ chuỗi id → hàm map phình thành một đống `if`.)*

2. **Cấu trúc dữ liệu lo an toàn, code lo diễn đạt luật.**
   `SellPriceByEra` có 5 phần tử để không `IndexOutOfRangeException`;
   luật "Mythic không bán" viết thành điều kiện rõ ràng, không dựa vào số 0.

3. **`GameBalance` là CHỈ ĐỌC lúc chạy.** Sửa ScriptableObject lúc runtime thì
   trong Editor nó **ghi thật vào file `.asset`** và ở lại sau khi Stop — nhưng
   khi build ra máy thật lại không ghi được. Bug chỉ xảy ra một bên.
   Giá trị thay đổi trong trận thuộc về **`RunState`** — object thường, sinh ra
   mỗi trận, không cần `Clear()` thì không thể quên `Clear()`.

4. **Thứ tự enum không được đổi.** Giá trị đã serialize vào `.asset`.

5. **Fallback phải KÊU.** Bản cũ nuốt lỗi im lặng → cả roster rơi về hình khối
   mà game vẫn "chạy". `SpineUnitVisual` cảnh báo khi thiếu animation hoặc
   thiếu event `impact`.

6. **Ngưỡng dòng là cái chuông, không phải mục tiêu.** `GameBalance` quá
   80 dòng ≈ có số đã len vào nhầm chỗ.

---

## 11. Còn treo, chưa quyết

- **Lính có di chuyển không?** Quyết định này chặn `UnitBrain` ở M3.
  Nhân vật không có chân → nghiêng về **đứng yên**, nhưng chưa xác nhận.
  Nếu đứng yên: bỏ `MoveSpeed` khỏi `UnitDefinition`, `UnitBrain` ~90 dòng
  thay vì ~180.
- **Chỉ số thật**: đang dùng placeholder **200 HP / 30 ATK**.
  Mốc khuyến nghị nếu phải tự đặt: **×3 mỗi bậc** — khớp tỉ lệ evolution
  3-con-thành-1. Đã tra mạng: SpinSquad (tên cũ **Lucky Offense**) và
  Forge Master **không có bảng chỉ số công khai**.
- **"Bàn đầy" định nghĩa chính xác** — xem mục 5.
- **16 ô × 3 lính = 48 lính/phe.** Con số lý thuyết, kiểm chứng ở M3–M4.
- **Có bao nhiêu Mythic** — chưa chốt.
