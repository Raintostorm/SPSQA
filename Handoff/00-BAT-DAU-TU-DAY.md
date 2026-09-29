# Bàn giao SpinSquad — Bắt đầu từ đây

> Viết ngày 2026-09-29, từ việc đọc trực tiếp codebase `SpinSquadClone`
> (branch `codex/2d-animation-reset`, commit `d555d6f1`).
> Mục đích: đủ để bạn **làm lại project từ đầu, tự code**, không cần mở lại repo cũ.

## Đọc theo thứ tự nào

| File | Nội dung | Khi nào đọc |
|---|---|---|
| `01-GAME-LA-GI.md` | Luật chơi + **toàn bộ con số cân bằng** đã chốt | Đọc đầu tiên, đọc kỹ |
| `02-CHAN-DOAN-BAN-CU.md` | Bản cũ hỏng ở đâu, vì sao | Đọc để không lặp lại |
| `03-KIEN-TRUC-MOI.md` | Cách chia code cho bản mới | Trước khi gõ dòng đầu tiên |
| `04-PIPELINE-CHARACTER-2D.md` | **Phần bạn đang thiếu**: model + animation 2D | Phần dài nhất, quan trọng nhất |
| `05-CODE-MANG-SANG.md` | File nào copy nguyên, file nào viết lại | Khi dựng project mới |
| `06-LO-TRINH.md` | 8 milestone, mỗi cái có tiêu chí "xong" | Bám theo hằng ngày |
| `07-TU-LAM-VS-NHO-AI.md` | Ranh giới + mẫu prompt | Khi định nhờ AI |
| `08-DON-DEP-REPO.md` | Git, dung lượng, .gitignore | Lúc tạo repo mới |

## Tóm tắt một đoạn

SpinSquad là **auto-battler roll-based**, Unity 2D, màn hình dọc, mobile.
Người chơi tiêu coin để quay gacha 6 ô → nhận lính/buff/coin → xếp lính lên
lưới 4×2 → bấm Start → lính tự đánh → qua wave → nhận thưởng.

Bản cũ có **logic đúng và đáng giữ** (gacha, stats, merge, meta) nhưng
**phần trình bày thì hỏng**: một file `DuelDirector.cs` 5.765 dòng ôm hết mọi
thứ kể cả dựng UI bằng code, scene rỗng 7 KB, và ba pipeline nhân vật chồng
nhau chưa cái nào xong.

Bản mới: giữ nguyên **luật chơi và con số** (file 01), viết lại **cấu trúc**
(file 03), và làm cho xong **một nhân vật 2D hoàn chỉnh** trước khi làm gì khác
(file 04).

## Quy tắc vàng cho bản mới

1. **Không file nào quá 400 dòng.** Chạm 400 là dấu hiệu phải tách.
2. **UI dựng trong Editor, không dựng bằng `new GameObject()`.**
3. **Một nhân vật chạy đẹp trước, rồi mới nhân bản ra cả roster.**
4. **Không commit file nhị phân lớn** (`.psd`, `.kra`, sheet PNG nguồn).
5. **Chưa thấy chạy trong Play Mode thì chưa gọi là xong.** Compile ≠ xong.

## Thông tin kỹ thuật cố định

- Unity **6000.4.1f1** (Unity 6), template **2D**
- Ngôn ngữ: **C#**
- Target: **mobile portrait**, mốc kiểm tra **iPhone X (1125×2436, tỉ lệ 9:19.5)**
- Camera: **Orthographic**
- Ngôn ngữ hiển thị trong game: **tiếng Anh**
