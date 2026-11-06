# INPUT 063 — Art hiệu ứng: lên cấp, relic, màn thắng · LÔ LỚN · LÀM HẾT MỚI BÁO

Đọc trước: `chat/PLAN_CHO_CHATGPT.md` §1–3 (bảng màu, luật art, hợp đồng cắt-gắn).
Claude viết mã chuyển động; bạn chỉ vẽ **LỚP RỜI** (không vẽ chuỗi khung hình của cùng một vật —
máy tạo ảnh không giữ được vật giống nhau qua các khung, chạy lên sẽ giật).

## Vì sao cần
User chạy thử: hiệu ứng lên cấp hiện tại **lạc quẻ và sượng** — nó dùng các lớp sáng trắng chung
chung (vòng tròn, hạt mềm), không có chất chibi viền đen của game. Cần bộ lớp **có phong cách
game**: hình khối rõ, viền đen dày ở vật thể, chỉ phần ánh sáng mới mềm.

## 1. Lên cấp quân (đích `Resources/Anim/LevelUp/`)
| slug | vẽ gì | cỡ | ghi chú |
|---|---|---|---|
| `lu-pillar` | cột sáng vàng hắt thẳng lên, đáy sáng nhất, tan dần lên trên | 384×768 | ánh sáng: **trắng thuần, chỉ alpha**, mép mềm, không viền |
| `lu-ground` | vòng phép **hình elip dẹt** (nhìn nghiêng ~30°) nằm dưới chân quân, nét đều | 768×256 | trắng thuần, chỉ alpha |
| `lu-star` | ngôi sao 4 cánh **vàng `#F5B728`, viền đen dày**, một dải sáng đỉnh | 128² | vật thể: có màu, có viền |
| `lu-star-small` | sao 4 cánh nhỏ, cùng kiểu | 64² | |
| `lu-chevron` | **mũi tên chữ V kép** (»xếp dọc, hướng lên) xanh lá `#61EE80`, viền đen | 128² | thay mũi tên to cũ; hiện ở ~70px |

## 2. Nâng cấp relic (đích `Resources/Anim/Relic/`)
| slug | vẽ gì | cỡ |
|---|---|---|
| `shine-sweep` | **một vệt sáng chéo** (dải nghiêng 30°, giữa sáng, hai mép tan) — Claude quét qua icon relic | 256×512, trắng thuần |
| `glint` | lấp lánh 4 cánh nhọn mảnh kiểu "ting!" | 128², trắng thuần |

## 3. Màn thắng (đích `Resources/Anim/Result/`)
| slug | vẽ gì | cỡ |
|---|---|---|
| `confetti-0..3` | 4 mảnh giấy màu khác dáng (chữ nhật, xoắn, tròn, tam giác), màu bảng màu game, **viền đen mảnh** | 64² mỗi mảnh |
| `star-big` | sao 5 cánh vàng viền đen dày (đánh giá trận) | 256² |
| `star-empty` | cùng dáng, rỗng xám đá `#4A5570` | 256² |

## 4. Preview chuyển động (BẮT BUỘC, để user duyệt **trước** khi Claude viết mã)
Dựng GIF ghép các lớp trên với nhịp đề xuất:
- `063-levelup.gif`: vòng elip hiện dưới chân → cột sáng vụt lên (0.2s) → 6–8 sao bắn lên toả ra rồi rơi nhẹ → chevron nhảy lên trên đầu → tan. Tổng ≤ 0.9s. Dùng một hình chibi bất kỳ của game làm nền.
- `063-relic.gif`: vệt sáng quét chéo qua icon relic (0.35s) → glint "ting" ở góc → tan.
- `063-victory.gif`: confetti rơi + 3 sao lần lượt đập vào chỗ.
Ghi kèm thời lượng từng bước trong `063-timing.md` — Claude sẽ chép đúng nhịp đó vào mã.

## 5. Kiểm và gắn
- Lớp sáng: ép RGB = 255,255,255, chỉ alpha mang hình; tâm trùng tâm ảnh ±1px.
- Vật thể có viền: thử thu về 48px vẫn đọc ra hình.
- Ảnh duyệt: `chat/check/063-board.png` (mọi lớp trên nền navy `#1B2440`) + 3 GIF. **Gom hết, user duyệt một lần.**
- Sau khi user gật: gắn vào đích ở trên, `.meta` border 0, backup nếu ghi đè, giữ GUID.

## 6. Báo cáo `## OUTPUT · 063` trong `chat/chatgpt.md`
Bảng XONG / BỊ CHẶN từng mục, đường dẫn + cỡ thật, ghi **"chưa chạy Unity"**.

**Không làm:** sửa `.cs`; vẽ lại rương / cầu triệu hồi / burst đã đạt; đụng icon dock Home.
