# Yêu cầu — Trang chủ màn dọc (1125×2436)

## 0. Tôi đã check bản cũ. Không có gì để chép.

Bạn bảo "trong spinsquad clone có đó". Tôi tìm hết `Handoff/`, đây là
toàn bộ những gì bản cũ ghi lại về hai màn này:

> `01-GAME-LA-GI.md` dòng 190:
> *"Có màn hình **Upgrade** và **Treasure** (bản cũ mới có khung, chưa hoàn chỉnh)"*

> `06-LO-TRINH.md` dòng 201, trong danh sách **cố tình để lại sau cùng**:
> *"Màn hình Upgrade / Treasure — không dạy bạn gì mới và dễ làm bạn sa đà"*

Tức là **không có spec, không có số, không có luật** — chỉ có cái khung
rỗng. Khác hẳn phần Battle, nơi bản cũ để lại cả bảng số đã cân mấy tháng.

Nên file này **định nghĩa mới**, không phải chép lại. Mục 5 liệt kê những
chỗ tôi phải tự quyết — bạn đọc và chốt, đừng để tôi chốt hộ.

Thứ duy nhất bản cũ để lại dùng được là bộ khung save, ở `03-KIEN-TRUC-MOI.md`:

```csharp
public sealed class SaveData
{
    public int Gold;
    public int TreasureKeys;
    public int HighestLevelUnlocked = 1;
}
```

---

## 1. Trang chủ hiện có gì

Chạy `SpinSquad ▸ Dựng TRANG CHỦ` ra đúng bốn thứ:

- tiêu đề `SPINSQUAD`
- 3 nút chọn level (khoá thì hiện 🔒)
- một dòng gợi ý
- ảnh nền `bg_home.png`

Chưa có: vàng, chìa, Upgrade, Treasure.

**Nền chưa dùng được.** `bg_home.png` là **1672×941**, tỉ lệ 1.777 — ảnh
ngang, từ hồi game còn màn ngang. Màn dọc cần 0.462. Kéo giãn cái này ra
dọc là méo gấp gần 4 lần. Phải vẽ lại.

---

## 2. Bố cục đề xuất — toạ độ thật, 1125×2436

```
┌─────────────────────────────┐  y=0 (đỉnh)
│  [🪙 1,250]      [🗝 3]      │  thanh tài nguyên, cao 128, cách đỉnh 40
│                             │
│         SPINSQUAD           │  tiêu đề, y≈320
│                             │
│   ┌───────────────────┐     │
│   │   LEVEL 1    ✓    │     │  thẻ level, 900×240, cách nhau 40
│   ├───────────────────┤     │  y≈760 / 1040 / 1320
│   │   LEVEL 2         │     │
│   ├───────────────────┤     │
│   │   LEVEL 3    🔒   │     │
│   └───────────────────┘     │
│                             │
│    ┌─────┐     ┌─────┐      │  hai nút tròn 256×256
│    │ 🔨  │     │ 📦  │      │  y≈1800, cách tâm ±200
│    │NÂNG │     │RƯƠNG│      │
│    └─────┘     └─────┘      │
│                             │
│   ┌───────────────────┐     │
│   │      CHƠI         │     │  nút to 1040×160, y=2436−40−160
│   └───────────────────┘     │
└─────────────────────────────┘
```

Vì sao hai nút tròn nằm **giữa** chứ không ở góc: ngón cái cầm điện thoại
một tay với tới vùng đó dễ nhất. Góc trên là vùng khó nhất.

Vì sao nút CHƠI nằm **đáy** cùng toạ độ với nút `XUẤT QUÂN` trong trận:
hai màn dùng chung một chỗ cho "hành động chính", tay không phải học lại.

---

## 3. Art — đã có và còn thiếu

**Đã có, không cần làm lại:**

| file | dùng cho |
|---|---|
| `artsrc/ui/resources.png` | túi vàng, chìa, rương đóng, rương mở |
| `artsrc/ui/bar_resource.png` | khung 9 mảnh cho thanh tài nguyên |
| `artsrc/ui/button_treasure.png` | nút rương, 2 trạng thái |
| `Art/UI/panel.png`, `button.png`, `button_down.png` | thẻ level, nút CHƠI |

**Còn thiếu — ba tấm:**

### Tấm A — nền trang chủ dọc

Chính xác **1125×2436**.

Khác nền trận: nền trận là bãi cỏ nhìn từ trên, nền trang chủ nên là một
**khung cảnh có chiều sâu** — nhìn ra xa, có đường chân trời. Người chơi
nhìn nó lâu hơn bất kỳ màn nào khác.

Vùng **y = 600…1600** phải **trống và ít tương phản** — ba thẻ level đè
lên đó. Chi tiết đẹp để ở trên đỉnh và dưới đáy.

> Prompt:
>
> A vertical 2D mobile game home screen background, exactly 1125x2436
> pixels, portrait orientation.
>
> A fantasy battle camp seen from a slightly elevated angle: grassy
> foreground at the bottom with a few tents and banners, a winding path
> leading away, rolling hills and a distant castle on the horizon, soft
> clouds and warm late-afternoon sky in the upper third.
>
> CRITICAL — the vertical band from 25% to 66% of the image height must
> stay visually QUIET: low contrast, no bright highlights, no busy
> detail, no important subject. UI cards will sit on top of it and must
> remain readable. Put the interesting detail in the top quarter and the
> bottom quarter instead.
>
> Style: hand-painted 2D cartoon, bold shapes, saturated but not neon,
> matching a casual mobile game. Not pixel art, not photorealistic.
>
> Fill the entire 1125x2436 canvas edge to edge. No border, no frame,
> no vignette, no text, no UI elements, no characters, no watermark.

Đặt file: `artsrc/ui/bg_home_portrait.png`

### Tấm B — nút Nâng cấp, 2 trạng thái

Phải **cùng bộ** với `button_treasure.png` đã có: cũng tròn, cũng nền gỗ
viền kim loại vàng, cũng hai trạng thái thường/bấm. Chỉ khác cái hình ở
giữa — búa và đe thay cho rương.

> Hai nút đứng cạnh nhau trên màn. Lệch cỡ hay lệch tông là nhìn ra ngay.
> Nên kèm luôn `artsrc/ui/button_treasure.png` làm ảnh tham chiếu khi hỏi.

> Prompt (đính kèm `button_treasure.png`):
>
> The attached image shows an existing pair of round game buttons.
>
> Draw a NEW pair in exactly the same style, size, framing and colour
> palette — same circular wooden face, same gold metal rim, same rivets,
> same lighting — but replace the treasure chest in the centre with a
> blacksmith's ANVIL and HAMMER.
>
> Two buttons side by side on a fully transparent background, each
> 256x256 pixels, 80 pixels of transparent space between them.
> Button 1 is the normal state. Button 2 is the EXACT same button
> pressed: darker, inner bevel inverted.
>
> The two buttons must align pixel for pixel: identical size, identical
> centre, identical anvil position. Only the shading changes.
>
> At least 40px of empty transparent margin around each. TRUE TRANSPARENT
> alpha background — no magenta, no white. The alpha must be
> ANTI-ALIASED, fading over about one pixel; do not threshold or
> posterise it.
>
> No text, no labels, no watermark, no drop shadow.

Đặt file: `artsrc/ui/button_upgrade.png`

### Tấm C — bốn bậc rương

Một hàng ngang **8 hình**: bốn cặp đóng/mở, theo thứ tự
Common → Rare → Epic → Legendary.

**Cùng một cái rương, bốn lớp vỏ khác nhau.** Không phải bốn cái rương khác
hình — người chơi phải nhận ra ngay "vẫn là rương, nhưng xịn hơn":

| bậc | vỏ |
|---|---|
| Common | gỗ nâu, đai sắt xỉn |
| Rare | gỗ sẫm, đai bạc, khảm đá xanh |
| Epic | đai vàng, khảm đá tím, có ánh phát sáng nhẹ |
| Legendary | pha lê/vàng ròng, rực sáng, có tia sáng toả ra |

Mỗi cặp đóng/mở phải **cùng góc, cùng cỡ, cùng vị trí thân rương** — chỉ
khác cái nắp và ánh sáng. Chúng thay nhau tại chỗ lúc mở; lệch một chút
là lúc đổi nó nhảy.

> Prompt:
>
> A horizontal row of EIGHT 2D cartoon treasure chest icons on a fully
> transparent background, in four pairs, left to right:
>
> 1-2. COMMON: brown wood, dull iron bands — closed, then open
> 3-4. RARE: dark wood, silver bands, blue gem inlay — closed, then open
> 5-6. EPIC: gold bands, purple gem inlay, faint magical glow — closed,
>      then open
> 7-8. LEGENDARY: crystal and solid gold, radiant, light rays — closed,
>      then open
>
> All eight are the SAME chest shape at the SAME angle and the SAME size.
> Only the materials, colours and glow change between tiers. Within each
> pair, the chest body must be identical and only the lid and the spilling
> light differ.
>
> Each open chest spills gold coins and warm light.
>
> Style: bold even black outline, flat shading with soft gradients, mobile
> casual game art. Not pixel art, not photorealistic.
>
> Each icon drawn COMPLETE with at least 40px of empty transparent margin
> around it; leave at least 80px of transparent space between neighbours.
> No cells, no frames, no boxes, no ground shadow.
>
> TRUE TRANSPARENT alpha background — no magenta, no white, no
> checkerboard. The alpha channel must be ANTI-ALIASED: outlines fade over
> about one pixel. Do not threshold or posterise the transparency.
>
> No text, no labels, no watermark.

Đặt file: `artsrc/ui/chests_tiers.png`

### Bốn ràng buộc, áp cho cả ba tấm

Đây là bốn thứ đã khiến phải nhập lại ba lần ở các đợt trước:

1. **Alpha trong suốt thật** — không magenta, không trắng, không kẻ ô
2. **Alpha khử răng cưa** — viền mờ dần qua ~1 điểm ảnh, không ngưỡng hoá
3. **Không cắt cụt** — chừa lề trong suốt ≥40px, không đóng khung
4. **Không chữ, không nhãn, không watermark, không bóng đổ**

(Nền trang chủ là ngoại lệ của điều 1 và 3: nó phủ kín khung, không có
alpha, không có lề.)

---

## 4. Yêu cầu code — thứ tự bắt buộc

Phải theo đúng thứ tự này. Làm UI trước khi có chỗ lưu là dựng xong rồi
phải tháo ra lắp lại.

**Bước 1 — `SaveSystem` + `SaveData`, bỏ PlayerPrefs**

Hiện `GameProgress` lưu **đúng một số nguyên** trong PlayerPrefs. Comment
trong chính file đó đã viết: *"Handoff/03 nói nên lưu bằng JSON file.
Đúng — nhưng đó là khi có nhiều thứ để lưu."*

Thêm Gold + Keys **chính là lúc đó**. Không đổi bây giờ thì thành 3–4 key
PlayerPrefs rời rạc, không ai xem được, không backup được.

Dùng đúng khung ở `Handoff/03` mục 6. Ghi ra `Application.persistentDataPath`.

**Bước 2 — kiếm được Gold và Key**

`BattleFlow` đã bắn `Phase.LevelCleared`. Móc vào đó. Chưa có gì ở
`ResultPanel` ngoài `GameProgress.Clear`.

**Bước 3 — thanh tài nguyên + nền mới trên trang chủ**

Chỉ hiển thị. Không có nút nào ăn vào nó. Chạy được ngay, thấy được ngay.

**Bước 4 — Treasure**

Hook duy nhất bản cũ để lại: `RunState.LegendaryLimit`, kèm comment
*"Treasure nâng cái này"*. Nhưng grep cả dự án, **không có gì tên
Treasure** — chỉ có chỗ đọc giá trị khởi đầu. Toàn bộ luật "mở rương
được gì" đang trống.

**Bước 5 — Upgrade**

Trống hoàn toàn. Xem mục 5.

---

## 5. Bốn quyết định — ĐÃ CHỐT

**(a) Upgrade nâng TỪNG NHÂN VẬT.** 20 con, mỗi con một cấp riêng. Cần một
màn hình danh sách quân cuộn được.

Chân dung lấy **khung idle đầu tiên** của chính con đó. Không tốn art:
20/20 con đã có `idle_1.png`. Độ nặng chúng chênh nhau 1.48 lần (khung bao
196–390px rộng), nên tool sẽ cân lại bằng `sqrt(diện tích mực)` rồi đặt vào
khung vuông chung — đúng cách đã làm với 4 icon buff, kết quả lệch 0.2%.

**(b) Mở treasure là một lượt gacha.** Phần thưởng chia 4 bậc
Common → Rare → Epic → Legendary, bậc sau mạnh hơn hẳn. Bậc càng hiếm tỉ
lệ roll càng thấp. Có nút **x10** và **x100** quay liên tục.

**(c) Level đầu (3 wave) cho 1000 vàng + 3 chìa.**

**(d) Chơi lại vẫn thưởng đủ.**

---

## 5b. Hệ quả — đọc rồi hẵng làm

### Vàng thành vô hạn

(c) + (d) ghép lại: level 1 có 3 wave, cho 1000 vàng, chơi lại bao nhiêu
lần cũng được.

Ước lượng thô — `WaveSpawnInterval` 0.7s/ô, `AttackCooldown` 0.7–0.95s,
`MoveSpeed` 2.0 — một wave khoảng 20–40 giây, cả level kể cả xếp quân
khoảng **2–3 phút**. Tức **~400 vàng/phút, không có trần**.

Hệ quả: giá tiền nâng cấp **thôi không còn là ràng buộc**. Thứ duy nhất
chặn người chơi là thời gian ngồi cày. Nếu một cấp nâng giá 500 vàng thì
nó là "chơi lại level 1 một lần rưỡi", không phải một lựa chọn.

Đây là quyết định của bạn và tôi làm đúng như vậy. Nhưng tôi sẽ để **cả
hai con số trong `GameBalance`** — tiền thưởng lần đầu và tiền thưởng chơi
lại tách riêng. Hôm nào thấy lạm phát thì hạ cái thứ hai xuống 20%, sửa
một chỗ, không đụng code.

### Mở rương là một lượt gacha

**Chốt:** 1 chìa = 1 lượt. Bậc càng hiếm tỉ lệ càng thấp. Có **x10** và
**x100** quay liên tục.

Bốn bậc KHÔNG phải bốn cái rương bạn chọn mua — chúng là **bậc phần
thưởng roll ra được**. Nên bộ art 8 hình rương (mục 3c) dùng để hiện
**kết quả**: rương bật ra mang màu của bậc vừa trúng.

Máy gacha tái dùng được gần hết. `UnitPool.Random(Rank, rng)` đã có sẵn,
bốc ngẫu nhiên một con theo bậc. `GameBalance` cũng đã quen dùng bảng
`int[]` trọng số (`AllyCountWeights`, `ClusterRewardCoin`...).

#### Bảng tỉ lệ đề xuất

| bậc | tỉ lệ | x10 trung bình | x100 trung bình |
|---|---|---|---|
| Common | 70% | 7.0 | 70 |
| Rare | 23% | 2.3 | 23 |
| Epic | 6% | 0.6 | 6 |
| Legendary | 1% | 0.1 | 1 |

#### Hai chỗ bảng trần này hỏng — phải có bảo hiểm

Tính thẳng từ bảng trên:

```
xac suat x10  KHONG ra noi 1 Epic tro len : 48.4%
xac suat x100 KHONG ra noi 1 Legendary    : 36.6%
```

Gần **một nửa** lượt x10 không ra nổi thứ gì trên Rare. Và x100 tốn 100
chìa — ở mức 3 chìa/level là **33 lần clear level** — mà hơn một phần ba
số lần vẫn trắng tay Legendary. Đó là kiểu hỏng khiến người chơi bỏ game,
không phải kiểu hỏng khiến họ cố thêm.

Đề xuất hai lớp bảo hiểm, đều là chuẩn của thể loại:

**1. x10 bảo đảm ít nhất 1 Epic trở lên.** Lượt nào roll trần không ra
thì nâng con cuối cùng lên Epic. Xoá hẳn 48.4% lượt chết.

**2. Bảo hiểm cứng 80 lượt cho Legendary.** Đếm số lượt kể từ Legendary
gần nhất; chạm 80 thì ép ra Legendary. Mô phỏng 200 000 lượt:

| | trung bình mỗi Legendary | quy ra lần clear level |
|---|---|---|
| không bảo hiểm | 99.7 lượt | 33.2 |
| bảo hiểm 100 | 63.8 lượt | 21.3 |
| **bảo hiểm 80** | **55.2 lượt** | **18.4** |
| bảo hiểm 60 | 45.5 lượt | 15.2 |

Chọn 80: x100 khi đó **luôn** có ít nhất một Legendary (100 > 80), và
người chơi biết chắc đường về đích thay vì phó mặc.

Bộ đếm bảo hiểm phải nằm trong `SaveData` — sống qua lần tắt game. Để
trong RAM thì thoát game là mất, và mất âm thầm.

#### Chìa kiếm chậm so với x100 — ĐÃ CHỐT: để nguyên

100 chìa = 33 lần clear level ở mức 3 chìa/level. x100 là **mốc dài hạn**,
không phải nút bấm được sớm. Cả hai số vẫn nằm trong `GameBalance`.

---

### Danh sách vật phẩm — ĐÃ CHỐT

| bậc | số món |
|---|---|
| Common | 12 |
| Rare | 8 |
| Epic | 6 |
| Legendary | 4 |
| **tổng** | **30** |

#### Bao lâu thì sưu tầm đủ

Bài toán người sưu tầm tem: để gom đủ `n` món trong một bậc cần trung bình
`n × H(n)` lần trúng bậc đó.

| bậc | số món | lượt roll cần | quy ra clear level |
|---|---|---|---|
| Common | 12 | 53 | 18 |
| Rare | 8 | 95 | 32 |
| Epic | 6 | 245 | 82 |
| Legendary | 4 | 460 (có bảo hiểm 80) | **153** |

Đủ cả 30 món ≈ **153 lần clear level**. Không có bảo hiểm thì con số là
278 — thêm một lý do nữa để có bảo hiểm.

Đây là mục tiêu dài hạn hợp lý cho một game sưu tầm, không phải vấn đề.
Nêu ra để bạn biết thang thời gian mình đang thiết kế.

#### Trùng lặp là chuyện thường xuyên, phải có luật

Trong 100 lượt roll, trung bình ra 70 món Common cho **12 loại** — tức mỗi
loại khoảng **5.8 bản sao**. Người chơi sẽ gặp đồ trùng liên tục ngay từ
giờ đầu, không phải thỉnh thoảng.

Không có luật xử lý thì mỗi lượt roll thứ hai trở đi là một lần "lại nó".
Ba cách thường dùng:

| cách | việc phải làm |
|---|---|
| **Trùng → lên cấp món đó** (bản sao thứ n làm món mạnh hơn) | vừa, cần thêm 1 trường cấp mỗi món trong save |
| Trùng → đổi thành vàng | nhẹ nhất, nhưng vàng đã vô hạn rồi nên gần như vô nghĩa |
| Trùng → mảnh ghép, đủ mảnh đổi món tự chọn | nặng nhất, cần thêm một loại tài nguyên |

Đề xuất **cách 1**: nó biến đồ trùng thành tiến bộ thật, và dùng lại đúng
khung "mỗi món một cấp" mà Upgrade nhân vật cũng cần — một cơ chế, hai chỗ
dùng.

#### Vật phẩm làm gì — ĐÃ CHỐT: cộng dồn thụ động

Có món là cả đội được cộng, vĩnh viễn. Không có UI lắp đồ, không kéo thả.
(Buff kích hoạt trong trận tính sau, không phải bây giờ.)

---

### Bảy chỉ số đề xuất

Tôi tra từng chỗ trong code chiến đấu. Năm chỉ số **đã nối sẵn**, hai cái
cần thêm vài dòng:

| chỉ số | hiện trạng | việc phải làm |
|---|---|---|
| **ATK %** | `RunBuffs.DmgPct` → `Unit.AttackPower` | 0 dòng |
| **Máu %** | `RunBuffs.HpPct` → `Unit.MaxHp` | 0 dòng |
| **Tốc đánh %** | `RunBuffs.AtkSpeedPct` → chia nhịp nghỉ | 0 dòng |
| **Chí mạng %** | `RunBuffs.CritChance` → `UnitBrain.RollDamage` | 0 dòng |
| **Sát thương chí mạng %** | `RunBuffs.CritMultiplier` có sẵn nhưng là **hằng số**, chưa thứ gì nâng nó | ~1 dòng |
| **Hút máu %** | chưa có | ~3 dòng, xem dưới |
| **Giáp %** | chưa có | ~3 dòng trong `TakeDamage` |

#### Hút máu gần như miễn phí — và nó cứu một tính năng đang chết

`Unit.Heal()` đã tồn tại, đã cộng vào bộ đếm `Healing`. `TakeDamage` đã
biết **ai** gây ra đòn (`source`) và **bao nhiêu máu mất thật** (`real`).
Hút máu chỉ là thêm một dòng ngay cạnh chỗ ghi công:

```csharp
source?.Heal(real * RunBuffs.LifeSteal(source.Team));
```

Và đây là chỗ đáng nói: comment trong `Unit.cs` dòng 140 viết

> *"Hiện luôn bằng 0: game chưa có cơ chế hồi phục nào."*

Tức **bảng HỒI PHỤC trên HUD trận đấu — một trong ba tab bạn đặt làm —
đang luôn hiện 0 cho mọi con, mọi trận.** Nó là một cột trống. Hút máu là
thứ đầu tiên làm nó có nghĩa.

#### Hai thang giá trị, không được trộn

Đây là chỗ dễ làm hỏng cân bằng nhất:

- **ATK, Máu, Tốc đánh, Sát thương chí mạng** là **phần trăm của chỉ số
  gốc**. +10% ATK cộng vào một con 1000 ATK khác hẳn cộng vào con 90 ATK.
- **Chí mạng, Hút máu, Giáp** là **điểm phần trăm tuyệt đối**. +10% chí
  mạng là +10 điểm, bất kể con nào.

Cho hai loại cùng một con số là cái bẫy: +10% ATK thì nhẹ, +10 điểm chí
mạng thì nặng gấp mấy lần. Nên hai thang riêng:

| bậc | số món | mỗi món có | thang NHÂN | thang XÁC SUẤT |
|---|---|---|---|---|
| Common | 12 | 1 chỉ số | +3% | +1.5 điểm |
| Rare | 8 | 1 chỉ số | +6% | +3 điểm |
| Epic | 6 | 2 chỉ số | +10% | +5 điểm |
| Legendary | 4 | 2 chỉ số | +18% | +8 điểm |

#### Đủ cả 30 món thì mạnh cỡ nào

Chia 40 ô chỉ số cho 7 chỉ số:

```
moi chi so NHAN    : +50%
moi chi so XAC SUAT: +24 diem %
```

Quy ra sức mạnh thật:

| | |
|---|---|
| Sát thương/giây | **×3.33** (ATK ×1.5, tốc đánh ×1.5, chí mạng 24% ×3.0) |
| Bền bỉ | **×1.97** (máu ×1.5, giáp −24% sát thương nhận) |
| **Tổng** | **mạnh hơn tay không ~6.6 lần** |

6.6 lần sau 153 lần clear level là đường cong hợp lý cho game sưu tầm.
Nếu thấy quá mạnh thì hạ **thang giá trị**, đừng hạ số món — số món là
nội dung, thang giá trị là cái vặn được.

Cả bốn con số mỗi thang nằm trong `GameBalance`, sửa một chỗ.

#### 30 vật phẩm = 30 icon art

Tấm lớn nhất dự án, gấp đôi mọi tấm trước cộng lại. Chia **4 tấm theo bậc**
(12 / 8 / 6 / 4), làm từng tấm một — một tấm 30 hình thì công cụ sinh ảnh
vẽ ẩu, không hình nào ra hồn.

Chủ đề: kho báu lẫn lộn — cốc, ly, trượng, áo choàng, ấn triện, bùa chú,
huy hiệu, nhẫn, vương miện, sách cổ, đèn thần, chuông. Càng bậc cao càng
nhiều kim loại quý, đá quý và ánh phát sáng.

Prompt cho 4 tấm này viết sau khi bạn duyệt danh sách tên 30 món — tôi sẽ
đề xuất danh sách kèm chỉ số của từng món ở bước tiếp theo.

### Rương 4 bậc cần thêm art

`resources.png` hiện chỉ có **một** cái rương (đóng + mở). Bốn bậc cần
bốn bộ. Yêu cầu nằm ở mục 3c dưới đây.

---

## 6. Sau khi có art

Báo tôi. Tool nhập sẽ:
- tách bằng **gán nhãn liên thông**, không cắt theo lưới cứng
- ép cặp nút thường/bấm của Upgrade về **chung khung** với cặp Treasure
  đã có, vì bốn nút này đứng cạnh nhau
- cân bốn icon tài nguyên theo `sqrt(diện tích mực)`, như đã làm với icon buff
- đo và in số **trước khi ghi**, để bạn duyệt
