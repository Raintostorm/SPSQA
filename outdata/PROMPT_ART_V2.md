# Sinh lại art — bản độ phân giải cao

Bộ hiện tại cao **~180px**, đủ để vẽ nhân vật 106px trên màn nhưng vỡ khi
phóng to kiểm tra. Bản mới cần **gấp 3**.

## Kích thước yêu cầu

Đây là kích thước **phần vật liệu** (không tính viền trong suốt):

| Nhóm | Hiện tại | Cần | Ghi chú |
|---|---|---|---|
| `body` | ~121 × 181 | **360 × 540** | thân đã có sẵn quần áo |
| `head` | ~113 × 146 | **340 × 440** | đầu đã có sẵn mũ |
| `mele` | ~139 × 217 | **420 × 650** | dựng đứng |
| `ranged` | ~131 × 208 | **400 × 620** | |
| `ammo` | ~120 × 130 | **360 × 390** | |
| `hand` | 76 × 63 | **230 × 190** | |

Xin **PNG nền trong suốt**, không nền trắng, không viền khung.

## Giữ nguyên mọi thứ khác

Bộ hiện tại **đúng phong cách rồi** — đừng đổi. Chỉ cần lớn hơn.

> Same flat cartoon style as the reference images: thick black outlines,
> flat fills with soft shading, chibi proportions. Do not change the art
> style, colours, or character design — only increase the resolution.

Nếu công cụ cho đính ảnh tham chiếu, đính chính ảnh cũ kèm câu trên.

## Giữ nguyên cách đặt tên

```
<thoi>_<loai>_<so>.png

thoi   stone | iron | modern | future      (giu nguyen, toi tu doi sang medieval)
loai   body | head | mele | ranged | ammo
so     1..4  (body/head),  tuy nhom (vu khi)
```

Số lượng giữ nguyên để không phải sửa 16 `UnitDefinition`:

```
body    4 moi thoi  = 16
head    4 moi thoi  = 16
mele    stone 3, iron 3, modern 2, future 1 = 9
ranged  stone 1, iron 1, modern 2, future 3 = 7
ammo    stone 2, medival 2, modern 4, future 6 = 14
hand    1
```

## Ba thứ hậu kỳ không cứu được

1. **Nền không trong suốt.** Tách nền tự động sẽ ăn mất viền đen.
2. **Mỗi ảnh một hướng vẽ.** Súng phải chĩa cùng một phía, vũ khí cận chiến
   phải dựng đứng. Bộ cũ lẫn lộn, tôi phải đo trục chính từng cái rồi xoay.
3. **Nhiều món trong một ảnh.** Mỗi món một file.

## Sau khi có

Nhắn tôi đường dẫn zip. Toàn bộ quy trình đã chạy một lần nên lần hai chỉ
là chạy lại script:

```
doi ten chuan        iron -> medieval, sua typo
xoay ve cung huong   do truc chinh tung anh
nen vao dai kich thuoc
chuan hoa khung chung + pivot
nhap vao Assets, PPU tinh lai theo kich thuoc moi
dung lai prefab + 16 UnitDefinition + prefab dan
```

Animation **không phải làm lại** — 5 clip ràng buộc theo tên object
(`Body`, `Body/Head`, `Body/Arm`, `Body/Arm/Weapon`), không theo sprite.
