# 30 vật phẩm Treasure — bản chốt

> Sinh ra từ `tools/treasure_items.py`. **Đừng sửa tay file này** — sửa
> nguồn rồi chạy lại, không thì file này và `PROMPT_ITEM_ICONS.md` lệch
> tên nhau mà không ai biết.

- Lên cấp N tốn **N mảnh** · **mỗi cấp cộng thêm 10% giá trị gốc**
- Cộng dồn **thụ động** — có món là cả đội được cộng, không cần lắp đồ

## Thang giá trị

| bậc | kiểu NHÂN | kiểu XÁC SUẤT | cấp tối đa |
|---|---|---|---|
| Common | +3% | +1.5 điểm | 10 |
| Rare | +6% | +3 điểm | 8 |
| Epic | +10% | +10 điểm | 6 |
| Legendary | +16% | +14 điểm | 5 |

**NHÂN** = ATK, Máu, Tốc đánh, ST chí mạng (phần trăm chỉ số gốc)
**XÁC SUẤT** = Chí mạng, Hút máu, Giáp (điểm phần trăm tuyệt đối)

Ô Epic +10 điểm là con số bạn đưa; mọi ô khác suy ra từ đó.

---


## Common — 12 món

| # | tên | chỉ số cấp 1 | mỗi cấp | hình |
|---|---|---|---|---|
| 1 | **Cốc gỗ sứt** | ATK +3% | +0.3% | a chipped wooden drinking cup, plain and worn |
| 2 | **Ly thiếc móp** | Máu +3% | +0.3% | a dented tin tankard with a simple handle |
| 3 | **Gậy gỗ sồi** | ATK +3% | +0.3% | a plain oak walking staff, bark still on it |
| 4 | **Áo choàng vải thô** | Giáp +1.5đ | +0.15đ | a coarse brown burlap cloak with a rope tie |
| 5 | **Ấn đất nung** | Máu +3% | +0.3% | a small terracotta clay seal stamp |
| 6 | **Bùa cỏ bện** | Tốc đánh +3% | +0.3% | a charm braided from dry grass and twine |
| 7 | **Huy hiệu đồng** | Chí mạng +1.5đ | +0.15đ | a plain round bronze badge, slightly tarnished |
| 8 | **Nhẫn sắt** | ATK +3% | +0.3% | a plain iron band ring, no gem |
| 9 | **Chuông đồng nhỏ** | Tốc đánh +3% | +0.3% | a small bronze handbell |
| 10 | **Sách da cũ** | ST chí mạng +3% | +0.3% | a worn leather-bound book, pages frayed |
| 11 | **Mũ da** | Giáp +1.5đ | +0.15đ | a simple brown leather cap |
| 12 | **Dây chuyền xương** | Máu +3% | +0.3% | a necklace of small bone beads on a cord |

## Rare — 8 món

| # | tên | chỉ số cấp 1 | mỗi cấp | hình |
|---|---|---|---|---|
| 1 | **Chén bạc** | Máu +6% | +0.6% | a polished silver goblet with an engraved rim |
| 2 | **Trượng khắc rune** | ATK +6% | +0.6% | a dark wooden staff carved with glowing blue runes |
| 3 | **Áo choàng lông sói** | Giáp +3đ | +0.3đ | a grey wolf-fur cloak with a silver clasp |
| 4 | **Ấn triện bạc** | ST chí mạng +6% | +0.6% | a silver seal stamp with a scrollwork handle |
| 5 | **Bùa mắt quạ** | Chí mạng +3đ | +0.3đ | an amulet shaped like a raven's eye, black feather attached |
| 6 | **Huy hiệu kỵ sĩ** | Tốc đánh +6% | +0.6% | a silver winged knight's badge |
| 7 | **Đồng hồ cát** | Tốc đánh +6% | +0.6% | a small hourglass in a silver frame, blue sand inside |
| 8 | **Nhẫn hồng ngọc** | ATK +6% | +0.6% | a silver ring set with a single red ruby |

## Epic — 6 món

| # | tên | chỉ số cấp 1 | mỗi cấp | hình |
|---|---|---|---|---|
| 1 | **Chén thánh vàng** | Máu +10%, Hút máu +10đ | +1%, +1đ | an ornate golden chalice brimming with glowing red liquid |
| 2 | **Trượng tinh thể** | ATK +10%, ST chí mạng +10% | +1%, +1% | a gold staff topped with a large purple crystal, faint glow |
| 3 | **Áo choàng bóng tối** | Giáp +10đ, Tốc đánh +10% | +1đ, +1% | a black cloak dissolving into purple smoke at the hem |
| 4 | **Ấn triện rồng** | ATK +10%, Chí mạng +10đ | +1%, +1đ | a gold seal stamp with a coiled dragon handle |
| 5 | **Sách cấm** | ST chí mạng +10%, Chí mạng +10đ | +1%, +1đ | a chained grimoire with a purple eye on the cover, glowing |
| 6 | **Đèn thần** | Máu +10%, Giáp +10đ | +1%, +1đ | a golden genie lamp with blue smoke curling from the spout |

## Legendary — 4 món

| # | tên | chỉ số cấp 1 | mỗi cấp | hình |
|---|---|---|---|---|
| 1 | **Vương miện bất tử** | Máu +16%, Hút máu +14đ | +1.6%, +1.4đ | a radiant golden crown set with red gems, bright light rays |
| 2 | **Trượng hỗn mang** | ATK +16%, ST chí mạng +16% | +1.6%, +1.6% | a twisted black-and-gold staff with a swirling void orb, radiant |
| 3 | **Ấn triện thần** | Chí mạng +14đ, Tốc đánh +16% | +1.4đ, +1.6% | a divine gold seal stamp with a winged handle, blazing light |
| 4 | **Giáp hoàng kim** | Giáp +14đ, Máu +16% | +1.4đ, +1.6% | an ornate golden breastplate with a sunburst emblem, radiant |

---

## Tự kiểm

| chỉ số | đủ bộ cấp 1 | đủ bộ cấp tối đa |
|---|---|---|
| Máu | +67.0% | +102.1% |
| ATK | +57.0% | +89.9% |
| ST chí mạng | +45.0% | +68.3% |
| Tốc đánh | +44.0% | +69.2% |
| Giáp | +40.0điểm | +60.4điểm |
| Chí mạng | +38.5điểm | +57.5điểm |
| Hút máu | +24.0điểm | +34.6điểm |

| | đủ bộ cấp 1 | đủ bộ cấp tối đa |
|---|---|---|
| Sát thương/giây | **3.9×** | **7.6×** |
| Bền bỉ | **2.3×** | **3.2×** |
| Tổng | **~9×** | **~25×** |

### Giáp phải bão hoà

```
giảm = giáp / (giáp + 100)
```

60 điểm → giảm 38%. Cộng thẳng thì quá 100 điểm là **bất tử** —
công thức đó luôn vỡ ở một mức nào đó.

### Thời gian

| mốc | lần clear level |
|---|---|
| Đủ bộ 30 món (cấp 1) | **153** |
| Nâng tối đa 1 món Legendary | **1 030** |
| Nâng tối đa cả 30 món | **15 071** |

Mốc cuối ~600 giờ — là trần, không phải đích. Muốn nó với tới được thì hạ
cấp tối đa xuống 6/5/4/3, tổng còn khoảng 1/4.
