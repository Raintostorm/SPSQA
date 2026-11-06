# INPUT063 — timing chuyển động đề xuất
2026-11-06. Preview bằng Pillow, không phải Unity. Claude dùng thời gian UI unscaled để x1/x2/x3 battle không đổi nhịp UI. Chưa gắn runtime.
GIF25fps, frame40ms. Chỉ chuyển động lớp đã có, không generate chuỗi pose. Các GIF lặp lại để review, việc lặp không phải yêu cầu game.

## 1. Lên cấp quân — tổng0.88s
| Thời điểm | Hành vi |
|---|---|
|0.00–0.10|Ellipse dưới chân fade0→1 bằng smoothstep; target preview260×87|
|0.06–0.20|Pillar hắt lên, alpha0→0.58 smoothstep; target210×420; đặt đáy ở chân, không kéo nhân vật|
|0.16+i×0.012, i=0..7|8sao bắn ra, mỗi sao sống0.60s; size22/32px xen kẽ; góc−160..160°, vx=sin(góc)×190; vy=−500−abs(cos(góc))×100; y=330+vy×u+700×u²; x=256+vx×u; xoay90°/s; alpha giảm theo lifetime|
|0.34–0.54|Chevron70px hiện trên đầu, nhảy35px lên bằng smoothstep|
|0.68–0.88|Mọi lớp tan bằng1−smoothstep; nhân vật giữ nguyên|

Vị trí preview512²: chânunit=(256,396),groundcenter=(256,398),pillarcenter=(256,270),chevroncenter=(256,190−35×ease). Khi gắn thật lấy bounds unit, không chép hardcode tọa độ preview.
Cột/ring tint preview vàng; PNG trắng để code tô màu. Không buộc làm rung/đổi màu thân quân.

## 2. Relic — tổng0.72s
| Thời điểm | Hành vi |
|---|---|
|0.00–0.35|Stripe30° quét trái→phải, previewcenterX105→405,y256; clip alpha theo silhouette relic; alpha0.8|
|0.30–0.65|Glint ở góc trên phải; alpha=sin(πu),size35+30sin(πu); u=(t−0.30)/0.35|
|0.65–0.72|Giữ icon gốc, hiệu ứng đã tan|

Không vẽ stripe ra cả card/background. Không thay sprite relic hoặc scale bóp méo nó.

## 3. Victory — tổng1.76s demo
| Thời điểm | Hành vi |
|---|---|
|0.00–1.76|20confetti trang trí, dùng4sprite quay/rơi; size18–26px, tốc rơi150–206px/s, drift14px, alpha0.85|
|0.18–0.42|Sao1 đập vào: alpha nhanh, scale1.35→1 sau đó nhún nhẹ0.92→1|
|0.40–0.64|Sao2 cùng nhịp|
|0.62–0.86|Sao3 cùng nhịp|
|0.86–1.76|Giữ3sao, confetti tiếp tục rơi|

Star-empty luôn làm nền rating, star-big phủ lên khi đạt. Preview3sao chỉ là mẫu, code phải đọc đánh giá thật, không tự cấp3sao cho mọi ván.

## Lifecycle để Claude nối
- Hiệu ứng chỉ trình bày; không callback cấp gold/shard/thưởng lần nữa.
- Cleanup layer/tween khi đóng màn hoặc destroy; không để raycast chặn input sau animation.
- Layer rời + easing nội suy theo thời gian. Không phát texture sprite sheet liên tục để giả chuyển động nhân vật.
