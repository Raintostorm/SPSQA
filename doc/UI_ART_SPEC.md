# Chuẩn art UI — SpinSquad

Rút ra ngày 2026-11-04 từ `doc/reference/spinsquad-apk-ui-index.md` (257 sprite UI của
game tham khảo). **Chỉ học logic và tỉ lệ. Không dùng art của game khác.**

## 1. Nút chữ nhật — khoá hai đầu, chỉ kéo phần giữa

Unity 9-slice giữ nguyên bốn góc và kéo dãn phần giữa. Phần khoá càng hẹp thì góc bo,
dải sáng và gờ đáy càng bị kéo mỏng khi nút rộng ra.

| | game tham khảo | bộ 002 cũ của ta | chuẩn mới |
|---|---|---|---|
| ảnh gốc | 248×175 | 692×142 | **260×150** |
| khoá mỗi đầu | 120px = 48% | 75px = 11% | **125px = 48%** |

Vẽ nút **hẹp** rồi khoá gần hết, thay vì vẽ nút rộng rồi kéo. Viền 9-slice:
`x=125, y=60, z=125, w=30`. Dải kéo chỉ còn 10px ở giữa nên hai đầu luôn đúng hình,
ở mọi chiều rộng từ 260 đến 1000.

**Hệ quả cho code:** bỏ `pixelsPerUnitMultiplier` theo chiều cao (`KeepButtonScale`).
Bộ cũ phải scale vì viền quá hẹp; bộ mới để `ppu = 1`, 9-slice tự lo cả hai chiều.

## 2. Icon vuông — một cỡ chuẩn duy nhất

- **150×150** cho mọi icon vuông: dock, tab, nút chế độ.
- **180×180** cho loại nổi bật hơn.
- **124×128** cho icon nhỏ nằm trong ô (kỹ năng, buff).

Không để mỗi màn một cỡ. Cùng tầng thì cùng cỡ.

## 3. Tab xuất thành cặp On/Off, hai ảnh riêng

Game tham khảo có `Icon_Tab_*_On` và `Icon_Tab_*_Off`, cùng 150×150. Trạng thái chọn
không làm bằng cách đổi màu trong code — vẽ hẳn hai ảnh thì khác biệt mới đủ rõ.

## 4. Nút nhỏ vẽ cố định, và **không vuông**

`Btn_Close 88×98` · `button_arrow 58×103` · `button_info 87×93` — đều cao hơn rộng,
đều không 9-slice. Ta đang ép chúng vào ô vuông nên trông bẹt.

Trong 257 sprite UI của game tham khảo, **160 cái là cỡ cố định**, chỉ 97 cái 9-slice.
9-slice dành cho nút có chữ dài ngắn thay đổi; còn lại vẽ riêng từng cái.

## 5. Mức chi tiết tính theo **cỡ hiển thị thật**, không phải theo số màu

Sai lầm 04/11: để chữa icon "quá rối", INPUT 014 ghi *"tối đa 3 màu, bỏ hẳn bóng đổ"*.
Kết quả (`check/038-home-icons.png`) đúng y đề bài nhưng **mất hết chất của game** —
trông như bộ icon tải về, không phải icon SpinSquad.

Luật đúng là:
- Mỗi icon **được giữ một dải sáng và một mảng tối** — đó là chất chibi của game,
  bỏ đi là thành phẳng lì.
- Thứ phải cắt là **chi tiết nhỏ hơn 10px ở cỡ hiển thị thật**, vì ở cỡ đó chúng nhập
  thành vệt lấm tấm. Icon thanh Home hiện ra ở 170px, không phải 512px.
- Phép thử: thu icon còn 48px, **hình chính phải vẫn đọc ra**. Không đọc ra thì bớt chi
  tiết, **không** bớt màu.

Và: **đừng vẽ lại cả bộ khi chỉ vài cái hỏng.** Trong bộ icon Home, quầy hàng và bình
báu đọc tốt ở cỡ thật; chỉ quả cầu và tốp lính bị nát. Thay cả bộ là vứt luôn hai cái
đang ổn.
