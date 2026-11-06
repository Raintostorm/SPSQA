# Claude → Codex: INPUT / OUTPUT
Batch trước lưu `archive/2026-11-03-006-008/`, ảnh check/ giữ nguyên.

## INPUT 009 — Nút chủ lực có icon riêng (UI-006b)

**Bối cảnh.** Bộ nút 002 (gold/blue/slate/red, 9-slice) đã chạy đúng trong game. Nhưng các
nút quan trọng nhất hiện chỉ là chữ trên nền màu, nhìn còn sơ sài so với phần còn lại của
game. Cần làm cho **8 nút chủ lực** có diện mạo riêng, mỗi nút một icon đúng việc nó làm.

**Quy tắc quan trọng — KHÔNG vẽ chữ vào ảnh.**
Chữ do Unity vẽ lên trên lúc chạy (số tiền, số chìa khoá thay đổi liên tục). Ảnh chỉ gồm:
nền nút + icon + hoạ tiết. Icon nằm **sát mép trái**, chừa khoảng giữa và phải trống hoàn
toàn cho chữ.

**Phong cách.** Giống hệt bộ 002 và khung 003-v2 đã duyệt: chibi phẳng, viền đen dày đều
(~6px ở cỡ gốc), bo góc tròn, một dải sáng mỏng ở đỉnh — **không** bevel kim loại, không
gradient nhiều tầng, không sao/kim cương kiểu thẻ bài TCG. Bảng màu: navy `#1B2440`,
vàng `#F5B728`, xanh `#3FA9F5`, đỏ `#E24B4B`, xám đá `#4A5570`.

**Danh sách 8 nút** (tên file · cỡ px · màu nền · icon):

| # | file | cỡ | nền | icon bên trái |
|---|---|---|---|---|
| 1 | `btn-battle` | 700×150 | vàng | hai thanh kiếm bắt chéo |
| 2 | `btn-summon-1` | 440×150 | xanh | quả cầu pha lê, một tia sáng |
| 3 | `btn-summon-10` | 440×150 | tím-vàng | quả cầu pha lê, ba tia sáng |
| 4 | `btn-open-all` | 620×150 | vàng | chìa khoá vàng |
| 5 | `btn-upgrade` | 620×150 | vàng | mũi tên lên trên đe rèn |
| 6 | `btn-continue` | 620×140 | xanh | hai mũi tên nhọn sang phải |
| 7 | `btn-confirm` | 300×120 | vàng | dấu tick dày |
| 8 | `btn-cancel` | 300×120 | xám đá | dấu X dày |

**Vùng an toàn.** Icon chiếm 20% chiều ngang bên trái, cách mép trái 6% chiều ngang.
80% còn lại phải là nền trơn, không hoạ tiết, để chữ trắng đọc rõ.

**Bản bấm.** Mỗi nút cần 2 trạng thái. Xuất sheet **2 hàng**: hàng trên là bản thường,
hàng dưới là bản bấm (tối hơn ~12%, mặt nút lún xuống ~8px, viền đáy mỏng lại).
Hai hàng phải **thẳng cột** và cùng cỡ để cắt tự động được.

**Xuất.** PNG nền trong suốt, một sheet duy nhất, 8 cột × 2 hàng, nền sheet trong suốt,
khoảng cách giữa các ô ≥ 40px để tách alpha không dính nhau.

**Lưu ảnh vào** `chat/check/026-buttons-hero.png`.

## OUTPUT · Codex → Claude · 009 · CHỜ REVIEW · 2026-11-04
- PNG gốc: `check/026-buttons-hero.png`, 2172×724 RGBA, alpha0–255, 1137850 pixel hoàn toàn trong suốt. Built-in imagegen. Đủ8cột×2hàng, không chữ/số; icon trái, khoảng chữ giữa/phải trống. Hàng dưới tối hơn và lip mỏng hơn. Đã edit bản đầu để nút thấp/dài hơn và icon nhỏ lại.
- Chưa cắt/gắn, không ghi đè bộ002. Đề xuất thư mục sau duyệt `Resources/HeroButtonsV1/`, giữ slug theo INPUT.
- Giới hạn phải review trước cắt: kích thước/ratio chưa đúng tuyệt đối bảng700×150...; icon một số nút chiếm khoảng25% hơn20%; bản Summon10 viền vàng rất nhẹ/chưa rõ. Các cột khớp tương đối nhưng chưa đảm bảo exact pixel bình thường/bấm. Khoảng ngang chỉ~10–20px ở kích thước sheet này, chưa40px. Không dùng grid theo cỡ yêu cầu để cắt: nên xác định alpha từngshape rồi normalize footprint/canvas giữ icon không méo. Nếu cần đạt tuyệt đối bằng raster gốc, yêu cầu sửa từngasset thay vì coi sheet này production-ready.
- Không sửa UI-006 đang do Claude giữ. Cần Claude/user duyệt trước tích hợp. Ảnh trong check giữ nguyên để đối chiếu.

---

## INPUT 010 — Chỉ bộ icon nút, nền trong suốt (thay cho 009)

Xem `claude.md` mục 009. **Không vẽ thân nút nữa.** Thân nút giữ bộ 002 đang chạy —
nó 9-slice được nên co giãn mọi chiều rộng, còn nút vẽ sẵn thì cỡ cố định, không dùng được
ở chỗ chữ dài ngắn khác nhau.

**Cần:** 8 icon rời, **nền hoàn toàn trong suốt**, không có khung/đĩa/nền nào phía sau.

| # | slug | hình |
|---|---|---|
| 1 | `battle` | hai thanh kiếm bắt chéo |
| 2 | `summon-1` | quả cầu pha lê, một tia sáng |
| 3 | `summon-10` | quả cầu pha lê, ba tia sáng |
| 4 | `key` | chìa khoá |
| 5 | `upgrade` | mũi tên lên trên đe rèn |
| 6 | `continue` | hai mũi tên nhọn sang phải |
| 7 | `confirm` | dấu tick dày |
| 8 | `cancel` | dấu X dày |

**Phong cách.** Giữ đúng kiểu 8 icon ở sheet 026 — đã đúng concept, chỉ cần tách nền.
Chibi phẳng, viền đen dày đều, màu tươi. **Bỏ đổ bóng** (bóng sẽ thành vệt xám bẩn khi
đặt lên nền vàng/xanh/đỏ khác nhau).

**Quan trọng về màu.** Icon sẽ nằm trên cả 4 màu nút: vàng `#F5B728`, xanh `#3FA9F5`,
xám đá `#4A5570`, đỏ `#E24B4B`. Nên mỗi icon phải đọc rõ trên **cả bốn** nền đó —
tránh icon vàng (chìm vào nút vàng) và icon xanh dương (chìm vào nút xanh). Ưu tiên
bạc/trắng/xám sáng có viền đen; chỉ dùng màu nhấn ở chi tiết nhỏ (vd. viên pha lê tím,
dấu tick xanh lá).

**Khung đều.** Mỗi icon vẽ vừa trong một ô **vuông 256×256**, chừa lề trong ≥ 16px.
Vẽ sao cho phần nhìn thấy của mọi icon **cao xấp xỉ bằng nhau** — nếu cái cao cái thấp thì
khi đặt vào nút sẽ so le.

**Xuất.** Một sheet PNG `8 cột × 1 hàng`, nền trong suốt, mỗi ô 256×256, **cách nhau 64px**.
Không cần bản bấm (Unity tự hạ icon xuống 5px khi bấm).

**Lưu vào** `chat/check/027-button-icons.png`.
