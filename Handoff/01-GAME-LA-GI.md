# 01 — Game là gì (luật + con số)

Tất cả số trong file này **lấy trực tiếp từ code bản cũ đã chạy được**.
Đây là phần giá trị nhất của project — bạn mất mấy tháng mới cân được.
Bản mới nên giữ nguyên rồi mới tinh chỉnh.

---

## 1. Vòng lặp

```
Homepage
  └─ chọn level (1..3)
       └─ vào Battle
            ├─ PREP: roll gacha, xếp lính lên lưới, merge
            ├─ bấm START
            ├─ COMBAT: lính tự đánh (không điều khiển)
            ├─ Wave clear → +coin → quay lại PREP (wave kế)
            └─ hết 10 wave → Level clear → +gold, +treasure key
                 └─ về Homepage, mở khoá level kế
```

- `MaxWaves = 10` mỗi level
- `MaxLevels = 3`
- Thua bất kỳ wave nào → hết trận

---

## 2. Bàn đấu

| Thông số | Giá trị |
|---|---|
| Lưới mỗi phe | **4 hàng × 2 cột** = 8 ô |
| Kích thước ô | `0.62` world unit |
| Túi lính (bench) | **4 × 4** = 16 slot, đặt dưới lưới ally |
| Kích thước ô bench | `0.62 × 0.42 = 0.2604` |
| Ally ở bên | trái · Enemy ở bên | phải |
| Lính mỗi ô (ally) | **tối đa 1** |
| Lính mỗi ô (enemy) | tối đa 3 (stack) |

Lính spawn trong ô của mình rồi **tự bước sang phía đối phương** khi combat bắt đầu.
Có **delay 2 giây** sau khi bấm Start trước khi lính thật sự lao vào
(`CombatEngageDelaySeconds = 2f`) — để người chơi kịp nhìn đội hình.

---

## 3. Gacha 6 ô — đây là trái tim của game

### 3.1 Cách sinh 6 ô

> ⚠️ **PHẦN DƯỚI ĐÂY ĐÃ LỖI THỜI — xem `09-QUYET-DINH-THIET-KE.md` mục 9.**
> Mô tả cũ (`mỗi số từ 1..4`) **mâu thuẫn với chính bảng thưởng ở mục 3.3**:
> nếu cả ba số đều ≥1 thì số lớn nhất chỉ tới `6-1-1 = 4`, nên cụm 5 và 6
> KHÔNG BAO GIỜ xảy ra — hàng "Epic", "Legendary" và cờ Jackpot là dữ liệu chết.
> Bản mới cho phép số 0 và đổi trọng số.

Ba loại ô: **Coin**, **Ally**, **Buff**.

Thuật toán **không random từng ô độc lập**. Nó random cái *phân hoạch*:

1. Liệt kê mọi bộ ba `(coin, ally, buff)` với mỗi số từ `1..4` và tổng `= 6`.
2. **Loại bỏ `(2,2,2)`** → đảm bảo luôn có ít nhất một loại đạt ≥ 3 ô.
3. Gán trọng số cho mỗi bộ ba:
   ```
   weight = 1.78^ally  ×  0.9^(coin + buff)
   ```
   → nghiêng về việc ra nhiều ô Ally.
4. Random theo trọng số, rồi **xáo trộn (Fisher–Yates)** 6 ô.

> Điểm hay: người chơi **luôn** nhận được thứ gì đó. Không có lượt trắng tay.
> Đây là lý do game "sướng tay". Giữ nguyên.

### 3.2 Giá roll

| | |
|---|---|
| Giá 1 lần roll | **20 coin** |
| Ví coin khởi đầu mỗi trận | **40** (= đúng 2 lần roll) |
| Coin thưởng mỗi wave clear | **+40** |
| Bán 1 ally | **+12 coin** |

Coin roll **chỉ tồn tại trong trận**, khác với gold ở meta.

### 3.3 Thưởng theo cụm (cluster)

Loại nào có **nhiều ô nhất** thì ăn thưởng. Cụm phải **≥ 3 ô** mới có thưởng.
Hoà thì cả hai cùng ăn.

| Số ô cùng loại | Nhãn | Coin | Ally rarity | Buff HP/Dmg | Buff AtkSpd | Buff Crit |
|---|---|---|---|---|---|---|
| 3 | Common | **+15** | Common | +10% | +5% | +1% |
| 4 | Rare | **+40** | Rare | +25% | +15% | +2% |
| 5 | Epic | **+80** | Epic | +100% | +50% | +5% |
| 6 | Legendary | **+200** | Legendary | +200% | +100% | +15% |

- 6 ô cùng loại → cờ **Jackpot**
- Ô Ally → chọn ngẫu nhiên 1 trong 5 dòng, rarity theo bảng
- Ô Buff → chọn ngẫu nhiên **đều** 1 trong 4 stat: `HpPct`, `DmgPct`, `AtkSpeedPct`, `CritChance`
- Buff **cộng dồn cả run**, không mất khi qua wave, reset khi restart trận

---

## 4. Chỉ số lính

### 4.1 Bảng gốc theo rarity

| Rarity | HP | ATK |
|---|---|---|
| Common | 100 | 15 |
| Rare | 300 | 45 |
| Epic | 900 | 135 |
| Legendary | 2.700 | 305 |
| Mythic | 8.100 | 915 |

### 4.2 Công thức thật

Chỉ số **không phải** lấy thẳng từ bảng. Nó nhân theo "cá tính" của từng dòng lính:

```
hpLine  = def.MaxHitPoints / 100      // 100 = mốc Common tham chiếu
atkLine = def.Attack       / 15       // 15  = mốc Common tham chiếu

HP  cuối = BảngHP[rarity]  × hpLine
ATK cuối = BảngATK[rarity] × atkLine
```

Nghĩa là: trong `UnitDefinition` bạn khai một Tank có `MaxHitPoints = 150`,
`Attack = 10` → nó luôn trâu gấp 1.5× và đấm yếu bằng 0.67× so với chuẩn,
**ở mọi bậc rarity**. Rất gọn. Giữ nguyên cách này.

### 4.3 Năm dòng lính (ngũ hành)

| Line | Id | Nguyên tố |
|---|---|---|
| 0 | `ally_moc` | Mộc |
| 1 | `ally_hoa` | Hỏa |
| 2 | `ally_kim` | Kim |
| 3 | `ally_thuy` | Thủy |
| 4 | `common_melee` | Thổ |

> Bản cũ hàm map id → line đã thành một đống `if` chắp vá (`unit_slip_slinger`,
> `unit_iron_guard`...). Bản mới: **để `lineIndex` thành một field trong
> `UnitDefinition`**, đừng suy ra từ chuỗi id.

---

## 5. Merge

- **3 lính cùng dòng + cùng rarity** trên bàn → tự động gộp thành **1 lính rarity kế tiếp**
- Chỉ merge được khi rarity **< Legendary** (tức Common→Rare→Epic→Legendary thì dừng)
- Mythic có cơ chế riêng ("Combine"), bản cũ chưa làm xong — **bỏ qua ở bản mới**
- Lính sau merge **tính lại stat từ bảng 4.1**, không nhân hệ số cũ

---

## 6. Combat

Auto hoàn toàn. Không điều khiển sau khi bấm Start.

### 6.1 Thông số mặc định

| Thông số | Giá trị | Ý nghĩa |
|---|---|---|
| `moveSpeed` | `1.35` | tốc độ đi bộ |
| `attackRange` | `1.65` | tầm đánh (ranged) |
| `touchStrikeInterval` | `0.24s` | chu kỳ chạm-đánh melee vs melee |
| `mixedClinchMeleeStrikeInterval` | `0.45s` | chu kỳ melee đánh khi cận chiến hỗn hợp |
| `rangedShotInterval` | `0.52s` | chu kỳ bắn |
| `contactSkinWidth` | `0.02` | khe hở khi chạm |

### 6.2 Ba nhánh xử lý

1. **Melee vs Melee** — chạm nhau rồi dính (clinch), đổi damage theo chu kỳ, **không bật lùi**.
2. **Hỗn hợp (melee vs ranged)** — cũng dính, melee gõ **100% damage** theo chu kỳ;
   ranged vẫn bắn theo interval của nó.
3. **Ranged tầm xa** — dừng ở `attackRange`, khoá 1 mục tiêu, bắn đạn.

### 6.3 Buff áp vào đâu

- `HpPct`, `DmgPct` → nhân thẳng vào HP/ATK khi spawn
- `AtkSpeedPct` → **chia** interval đánh (đánh nhanh hơn)
- `CritChance` → roll xác suất mỗi nhát khi gây damage

---

## 7. Meta (ngoài trận)

- **Gold** và **Treasure Key** — tài nguyên lâu dài, lưu vào save
- Clear level → +gold, +key, mở khoá level kế
- Có màn hình **Upgrade** và **Treasure** (bản cũ mới có khung, chưa hoàn chỉnh)
- Bản cũ lưu bằng `PlayerPrefs` — **bản mới nên dùng JSON file**, xem file 03

---

## 8. Những gì bản cũ CHƯA có (đừng tưởng là có)

- Âm thanh (thư mục `Audio/` rỗng)
- Hệ thống skill riêng cho Epic/Legendary (đã spec, chưa code)
- Cân bằng wave thật sự (enemy hiện gần như hardcode)
- Mythic combine
- Bất kỳ nhân vật nào hoàn chỉnh cả 5 animation ngoài 1 con — xem file 04
