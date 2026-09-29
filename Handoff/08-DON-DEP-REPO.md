# 08 — Dọn dẹp repo & thiết lập git

---

## 1. Vấn đề của repo cũ

| | |
|---|---|
| Tổng thư mục | 14 GB |
| `.git/` | **2,1 GB** cho chỉ 2.366 file / 37 commit |
| Code + asset thật | ~115 MB |

`.git` phình vì lịch sử từng chứa file lớn: `legendsrobot.psd` 7,8 MB,
`SpinSquad_ButtonKit_Master.kra` 2,1 MB, và hàng chục PNG 2 MB.

**Git giữ vĩnh viễn mọi phiên bản của mọi file đã từng commit.**
Xoá file bây giờ không làm `.git` nhẹ đi — bản cũ vẫn nằm trong lịch sử.

Đây là lý do mạnh nhất để **bắt đầu repo mới** thay vì dọn repo cũ.

---

## 2. `.gitignore` — tạo TRƯỚC commit đầu tiên

Copy nguyên khối này vào `.gitignore` ở gốc project mới:

```gitignore
# ── Unity ─────────────────────────────────────────
/[Ll]ibrary/
/[Tt]emp/
/[Oo]bj/
/[Bb]uild/
/[Bb]uilds/
/[Ll]ogs/
/[Uu]ser[Ss]ettings/
/[Mm]emoryCaptures/
/[Rr]ecordings/
/[Aa]ssets/AssetStoreTools*
/[Aa]ssets/Plugins/Editor/JetBrains*

# ── IDE ───────────────────────────────────────────
.vs/
.idea/
.vscode/
.cursor/
*.csproj
*.unityproj
*.sln
*.slnx
*.suo
*.user
*.userprefs
*.pidb
*.booproj
*.svd
*.pdb
*.mdb
*.opendb
*.VC.db

# ── Build output ──────────────────────────────────
*.apk
*.aab
*.xapk
*.unitypackage
*.app
ExportedObj/
crashlytics-build.properties
sysinfo.txt

# ── Addressables ──────────────────────────────────
/[Aa]ssets/[Aa]ddressable[Aa]ssets[Dd]ata/*/*.bin*
/[Aa]ssets/[Ss]treamingAssets/aa.meta
/[Aa]ssets/[Ss]treamingAssets/aa/*

# ── File nguồn art: KHÔNG commit ──────────────────
# Chỉ commit PNG đã xuất. File nguồn để ở Google Drive / ổ ngoài.
*.psd
*.kra
*.xcf
*.clip
*.ai
*.sai

# ── Python / tooling ──────────────────────────────
**/__pycache__/
*.pyc
.venv/
*.log

# ── macOS ─────────────────────────────────────────
.DS_Store
**/.DS_Store
```

### Hai dòng quan trọng nhất

```gitignore
*.psd
*.kra
```

Chính hai loại file này làm `.git` cũ phình lên 2 GB. File nguồn art
**không thuộc về git** — để chúng ở Google Drive, Dropbox, hoặc ổ cứng ngoài.
Repo chỉ chứa PNG đã xuất.

---

## 3. Ngưỡng dung lượng

| Loại file | Ngưỡng | Vượt thì làm gì |
|---|---|---|
| PNG sprite lẻ | < 500 KB | nén lại, hoặc giảm kích thước canvas |
| PNG atlas / sheet | < 2 MB | cắt nhỏ ra |
| Bất kỳ file nào | **> 5 MB** | **dừng lại, hỏi: file này có cần trong git không?** |
| Toàn repo (không tính Library) | < 500 MB | rà soát |

### Kiểm tra trước khi commit

```bash
find Assets -size +5M -not -name "*.meta"
```

Lệnh này liệt kê mọi file trên 5 MB trong `Assets/`. Chạy nó trước mỗi commit
lớn. Nếu có kết quả → cân nhắc kỹ trước khi `git add`.

---

## 4. Quy trình git hằng ngày

```bash
git status                       # luôn xem trước khi add
git add Assets/_Project/Scripts  # add có chọn lọc, KHÔNG dùng "git add ."
git commit -m "M2: attack clip + animation event"
```

### Lý do không dùng `git add .`

Unity sinh ra rất nhiều file rác. `git add .` một lần là bạn kéo vào đủ thứ
không mong muốn — và với git, mọi thứ đã vào thì ở lại mãi.

### Quy ước commit message

```
M2: thêm clip Attack + animation event
M3: sửa lỗi damage rơi 2 lần mỗi cú đánh
M4: lưới snap đúng tâm ô
```

Có tiền tố milestone → sau này `git log` đọc ra được tiến trình học của bạn.

---

## 5. File Unity BẮT BUỘC commit

Người mới hay bỏ sót, gây lỗi "prefab mất reference":

```
✅ Assets/**/*.meta          ← BẮT BUỘC. Không có .meta là Unity mất liên kết
✅ ProjectSettings/          ← toàn bộ
✅ Packages/manifest.json
✅ Packages/packages-lock.json
❌ Library/                  ← không bao giờ
❌ Temp/, obj/, Logs/
❌ UserSettings/             ← thiết lập riêng máy bạn
```

> **Mất file `.meta` = Unity coi asset đó là file mới** → mọi prefab/scene
> trỏ tới nó đều đứt liên kết. Đây là lỗi kinh điển. `.gitignore` ở mục 2
> đã xử lý đúng, đừng tự sửa phần đó.

---

## 6. Xử lý repo cũ

Đừng xoá. Giữ làm tài liệu tham khảo.

```bash
# Giải phóng 3 GB ngay, không mất gì (Unity tự sinh lại khi mở project)
rm -rf /Users/vuhagiabao/SpinSquadClone/Library

# File .xapk game gốc 1,1 GB — chuyển ra ổ ngoài nếu còn cần tham khảo
# Tools/LocalAssetAI 7,5 GB — giữ nếu còn dùng ComfyUI để sinh art
```

Đổi tên thư mục cho khỏi nhầm với project mới:

```bash
mv /Users/vuhagiabao/SpinSquadClone /Users/vuhagiabao/SpinSquadClone_OLD_REFERENCE
```

Sau khi xong **M8**, nếu không mở lại lần nào nữa thì mới xoá hẳn.

---

## 7. Sao lưu

Repo mới sẽ nhẹ (< 500 MB), nên push GitHub thoải mái.

```bash
gh repo create spinsquad --private --source=. --push
```

Ghi chú từ bản cũ: *"xác thực GitHub qua terminal có thể lỗi trên máy này;
GitHub Desktop là đường push ổn định"*. Nếu `gh` không chạy, dùng GitHub Desktop.

**File nguồn art** (`.psd`, `.kra`) — vì không nằm trong git, phải sao lưu
riêng. Google Drive hoặc ổ ngoài. Đừng để chúng chỉ tồn tại một bản trên máy.
