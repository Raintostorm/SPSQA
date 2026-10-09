# Chuyển sang màn dọc (iPhone X — 1125×2436)

Bản kế hoạch. Chưa sửa gì.

---

## 0. Ngã rẽ phải chốt trước mọi thứ

Mọi khâu bên dưới phụ thuộc vào một câu hỏi: **trận đánh còn là trái↔phải,
hay thành dưới↕trên?**

| | A. giữ trận ngang | B. xoay trận dọc |
|---|---|---|
| dải chơi trong màn dọc | 1125×633 — **26% chiều cao** | dùng hết màn |
| bỏ phí | **74%** | ~0 |
| art nền | giữ được 2 tấm hiện tại | **phải vẽ lại 2 tấm** |
| mã phải sửa | gần như không | `UnitBrain`, `BattleLane`, `BattleFlow` |
| sprite nhân vật | đúng hướng sẵn | **nhìn ngang mà đi dọc** |

**Đề xuất: B.** Phương án A để trống ba phần tư màn hình — đó không phải
chuyển sang màn dọc, đó là nhét màn ngang vào khung dọc.

Số đo cho B, bề ngang khung nhìn 5.5 đơn vị:

```
cao khung nhin  11.9 don vi
8 hang o         6.6
lan giao tranh   1.5
hai dai HUD      2.0
                ----
                10.1   <= 11.9   con du
o rong 1.24 don vi (hien tai 0.92) — ro hon han
```

---

## 1. Rủi ro lớn nhất, và cách thử rẻ

**Sprite vẽ NHÌN NGANG mà lính đi theo chiều DỌC.** Cả 20 nhân vật đều là
hình chiếu cạnh, quay mặt sang phải. Trong trận dọc, lính đi lên/xuống mà
thân vẫn quay ngang.

Rất nhiều game mobile làm đúng như vậy và nhìn vẫn ổn, vì quân nhỏ và mắt
đọc theo hàng lối chứ không theo hướng mặt. Nhưng đây là thứ **không đoán
được, phải nhìn mới biết**.

Cách thử rẻ nhất, làm TRƯỚC khi đụng một dòng mã: dựng một ảnh tĩnh mô
phỏng cả màn dọc — nền tạm, 8 hàng ô, hai phe, vài con đang đi. Nếu nhìn
sai thì ta biết ngay, chưa mất gì.

---

## 2. Các khâu, theo thứ tự nên làm

### Khâu 1 — Ảnh mô phỏng (không đụng mã)
Dựng ảnh tĩnh ở 1125×2436. Chốt: bề ngang khung nhìn, cỡ ô, chỗ đặt HUD,
và trả lời được câu hỏi hướng sprite ở mục 1.

### Khâu 2 — Camera và tỉ lệ
`LetterboxCamera` hiện kẹp tỉ lệ vào `[1.78, 2.80]`. Màn dọc là `0.46` —
nằm ngoài hẳn. Phải đổi sang **lấy BỀ NGANG làm chuẩn** thay vì chiều cao:
hiện `orthographicSize` định chiều cao, trong khi màn dọc thì bề ngang mới
là thứ phải giữ cố định.

Đụng cả `BattleBackground.FitDesign` — nó phóng ảnh theo `DesignAspect`.

### Khâu 3 — Art nền (một vòng AI)
Hai tấm mới, dọc. Đề xuất **1242×2688** (đủ dư cho máy cao hơn):
* `bg_prep` — sân xếp quân, hai khối 4×4 xếp trên dưới, lằn ngang chia giữa
* `bg_field` — mặt đường **nằm NGANG** cắt giữa màn, lính đi lên

Cùng ràng buộc như lần trước: vùng đặt ô phải phẳng và đều, AI không vẽ ô,
hai tấm cùng đường chân trời.

### Khâu 4 — Lưới
`BattleSceneSetup` có 4 hằng phân số (`GridInnerFrac`, `GridPitchXFrac`,
`GridRow0Frac`, `GridPitchYFrac`). Đổi nghĩa: `Inner` giờ là nửa bề cao lằn
giữa, hai khối xếp trên dưới thay vì trái phải.

`BattleGrid.origin` và `CellToWorld` không phải sửa — chúng đã là lưới
tổng quát.

**Ô đất**: `tile_ally`/`tile_enemy` tỉ lệ 1.03. Ô mới 1.24 × 0.83 là tỉ lệ
1.49 — kéo 45%, viền đá sẽ méo. Hoặc xin ô mới, hoặc chọn cỡ ô giữ tỉ lệ.

### Khâu 5 — Trục giao tranh (phần mã gọn nhất, nhưng dễ sót)
Chỗ giả định trục ngang, đã rà hết:

```
UnitBrain.cs:215   Chase()      p.x = MoveTowards(p.x, t.x, ...)
UnitBrain.cs:186   FaceTarget() SetFacing(target.x - self.x)
UnitBrain.cs:147   Watchdog     lastX
BattleLane.cs:16   laneY, allyEntryX, enemyEntryX
BattleFlow.cs:414  entry + FormationOffset
```

Cách sửa sạch nhất: đưa **trục giao tranh thành dữ liệu** (`Vector2 axis`),
không viết cứng `.x` hay `.y`. Làm vậy thì đổi hướng trận sau này không phải
sửa mã lần nữa.

`BattleGrid.SortingOrderFor` dùng `-y*100` — trong trận dọc nó vẫn đúng và
còn hợp lý hơn: con đứng thấp che con đứng cao.

`formationSpread` hai trục đổi vai cho nhau.

### Khâu 6 — HUD
`BattleHudBuilder` neo mọi thứ theo khung tham chiếu `1920×1080`. Đổi sang
`1125×2436`. Phần lớn là cơ học, trừ hai chỗ phải thiết kế lại:
* **bảng roll 6 ô** đang xếp một hàng ngang — màn dọc hẹp, nên 2×3 hoặc 3×2
* **thanh chỉ số buff** đang một dòng dài — phải xuống hai dòng

Chú ý cái bẫy cũ: giá trị đã serialize trong scene **không** đổi theo giá
trị mặc định trong mã. Mọi thứ phải đặt thẳng từ script dựng scene.

### Khâu 7 — Đạn bay
`Projectile.ProjectileArcHeight` tính vòng cung trên mặt phẳng ngang. Bắn
theo chiều dọc mà vẫn cộng vòng cung thì đạn vẽ cung sang bên. Phải xoay
mặt phẳng cung theo trục giao tranh.

### Khâu 8 — Trang chủ và bảng kết quả
`bg_home.png` đang 1920×1080. `HomeScreen`, `ResultPanel` neo theo khung
ngang. Một vòng art nữa cho nền dọc.

---

## 3. Thứ KHÔNG phải đụng tới

Nói ra để khỏi lo:

* **20 nhân vật và 680 khung hình** — không liên quan tỉ lệ màn
* `FrameUnitVisual`, `UnitAnimationSet` — không liên quan
* Toàn bộ gacha: `RollService`, `RollResolver`, `RunState`, `EvolutionSystem`
* `BattleBoard`, `BattleGrid` phần toán ô ↔ toạ độ
* `Unit`, `RunBuffs`, `HealthBar`

Tức phần *luật chơi* đứng yên hoàn toàn. Việc này là **bố cục và trình bày**,
không phải viết lại game.

---

## 4. Ước lượng

| khâu | nặng | cần art mới |
|---|---|---|
| 1. ảnh mô phỏng | nhẹ | không |
| 2. camera | nhẹ | không |
| 3. nền | nhẹ (chờ art) | **2 tấm** |
| 4. lưới | vừa | có thể cần ô mới |
| 5. trục giao tranh | vừa | không |
| 6. HUD | **nặng nhất** | không |
| 7. đạn | nhẹ | không |
| 8. trang chủ | vừa | **1 tấm** |

Đường găng là **khâu 6**, và nút thắt chờ đợi là **khâu 3**.

---

## 5. Hai câu hỏi cần bạn trả lời trước khi bắt đầu

1. **Phương án A hay B?** Tôi đề xuất B.
2. **Chỉ màn dọc, hay cả hai?** Giữ cả hai thì mọi hằng số bố cục phải thành
   hai bộ và mọi thứ phải kiểm hai lần. Bỏ hẳn màn ngang thì rẻ hơn nhiều.
