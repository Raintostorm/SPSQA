# 11 — Lỗi đã gặp, và cách nhận ra lần sau

> Sổ ghi lỗi. Mỗi mục: **triệu chứng** → **nguyên nhân** → **cách chặn**.
> Chỉ ghi lỗi đã thật sự xảy ra trong dự án này, không ghi lỗi lý thuyết.
>
> Điểm chung của gần hết danh sách: **Unity không báo gì cả**. Đó là lý do
> sổ này tồn tại.

---

## A. Lỗi khi sinh file YAML bằng script

### A1. `m_Children:` xuống dòng rồi mới `[]`

```
Unable to parse file ...: Parser Failure at line N:
Expect ':' between key and value within mapping
Restored Transform child parent pointer from NULL
```

Sinh ra:
```yaml
  m_Children:
  []
```
YAML đọc `[]` thành một **khoá mới** thiếu dấu hai chấm.

**Đúng:** `m_Children: []` trên **một dòng**.

**Chặn:** sau khi sinh file, quét mọi dòng — dòng nào chỉ có `[]` hoặc `{}`
đứng một mình là sai.

---

### A2. Xoá `.meta` là xoá danh tính của asset

**Triệu chứng:** ô tham chiếu trong Inspector bỗng trống, không báo lỗi.
Script chạy với dữ liệu rỗng (`Máu = 0`, lính chết ngay cú đầu).

**Nguyên nhân:** GUID nằm trong `.meta`, không nằm trong file asset. Một dòng
dọn dẹp kiểu `rm Unit_*.asset.meta` rồi sinh lại là mọi tham chiếu chết.

**Chặn:**
- Đổi tên asset thì đổi **cả hai** file, **giữ nguyên nội dung `.meta`**.
- Chạy `python3 tools/check_refs.py` sau mỗi lần sinh file hàng loạt.

---

### A3. `fileID` trùng trong một file

`fileID` chỉ cần duy nhất **trong một file**, nhưng trùng thì Unity nối sai
component vào sai GameObject — và không báo.

**Chặn:** sinh xong thì đếm, mọi `--- !u!N &id` phải có `id` khác nhau.

---

## B. Lỗi Animation

### B1. Xoá tên hàm không xoá được Animation Event

```
'Unit' AnimationEvent has no function name specified!
```

Xoá chữ trong ô tên hàm chỉ để lại event **rỗng**, dấu event vẫn nằm đó.

**Đúng:** chuột phải vào dấu event → Delete.

---

### B2. Nút Record đỏ: mọi chỉnh sửa Inspector chui vào clip

**Triệu chứng:** chỉnh `Position` của một object, lưu, nhưng object vẫn giá trị
cũ. Nhìn trong cửa sổ Animation thấy mọc thêm `X : Position`, `X : Sprite`…

**Nguyên nhân:** Record đang bật. Unity biến mọi thay đổi thành **keyframe**
chứ không ghi vào object.

**Chặn:** tắt Record trước khi sửa object. Ô nào bị clip điều khiển sẽ có
**viền xanh lá** trong Inspector.

---

### B3. Clip ràng buộc theo CHUỖI đường dẫn

Đổi tên object (`Arm` → `arm`) là mọi đường cong trỏ vào nó chết im lặng.

**Chặn:** đừng đổi tên object đã có clip trỏ tới. Phải đổi thì sửa `path:`
trong `.anim` cùng lúc.

---

### B4. Tên STATE khác tên CLIP

`animator.Play()` nhận tên **state**, không phải tên clip. Trạng thái tên
`Attack_bow` có thể chứa clip `Attack_spear`.

**Chặn:** dùng `animator.HasState(0, Animator.StringToHash(name))` để kiểm,
đừng so với `clip.name`. Và đặt tên state **trùng** tên clip.

---

### B5. Write Defaults tranh quyền với code

Thuộc tính được animate ở **một** state sẽ bị ghi về **giá trị mặc định** khi
chạy state khác (với `Write Defaults = 1`, mặc định của Unity).

Hệ quả thật: `UnitEquipment` tắt `Weapon.enabled`, rồi `Idle` bật lại.

**Chặn:** một thuộc tính chỉ nên có **một** chủ — hoặc clip, hoặc code.

---

### B6. Thứ tự Awake giữa hai GameObject là KHÔNG xác định

**Triệu chứng:** `NullReferenceException` ở một field được gán trong `Awake`,
và nó **lúc có lúc không** — tuỳ Unity tình cờ chạy object nào trước.

Đã vấp hai lần trong một ngày:
- `BattleBoard.grid` gán trong `Awake` → script Editor đọc khi chưa Play, nhận null
- `BattleBoard.cells` cấp trong `Awake` → `BattleSetup.Awake` gọi trước, nhận null

Lần thứ hai còn do chính tôi chữa lần thứ nhất sai cách: dời `BattleSetup`
từ `Start` sang `Awake` để chạy trước `BattleFlow.Start` — vá được chỗ này
thì vỡ chỗ kia.

**Chặn:** thứ nào người khác có thể hỏi tới thì **cấp phát lười**, đừng cấp
trong `Awake`:

```csharp
List<Unit>[,] Cells
{
    get
    {
        if (cells != null) return cells;
        cells = new List<Unit>[Grid.Rows, Grid.Cols];
        ...
        return cells;
    }
}
```

Lấy lười thì thứ tự không còn ý nghĩa — và không cần nhớ quy tắc nào.

Chỉ dùng `[DefaultExecutionOrder]` khi thật sự không tránh được; nó là một
ràng buộc vô hình, đọc code không thấy.

---

### B7. Clip "đúng dữ liệu" mà vẫn sai chuyển động

**Triệu chứng:** con cầm ná vung tay y như con cầm chuỳ. Kiểm thì mọi thứ
đều đúng — asset ghi `AttackBow`, controller có state `AttackBow` chứa clip
`AttackBow`, spawner có gọi `SetAttackState`. Không chỗ nào sai.

**Nguyên nhân:** sai nằm trong *nội dung* clip, không nằm ở đường dẫn tới nó.
`AttackBow` đi `-30 → -16 → -36 → -30`: tốc độ đều, không có nhịp dừng.

Điều phân biệt "bắn" với "chém" **không phải biên độ** mà là **nhịp**:

| | hình dáng theo thời gian |
|---|---|
| chém | một mạch đều từ đầu đến cuối |
| bắn | giương chậm → **đứng yên ngắm** → giật ngược rất nhanh |

Thiếu nhịp đứng yên thì mọi cử động tay đều đọc ra là vung. Tăng biên độ
còn làm nó giống chém **hơn**.

**Chặn:** đo nhịp, đừng chỉ đo biên độ — hai key liên tiếp cách nhau
≥ 0.15 s mà giá trị gần bằng nhau thì clip đó có nhịp giữ.
`tools/mkclip.py` in ra sẵn.

---

### B9. Kiểm tra dữ liệu trong Awake là báo oan

`Unit.Awake` báo lỗi khi thiếu `UnitDefinition`. Nhưng `UnitSpawner` làm:

```
Instantiate(prefab)      <- Awake chay NGAY TRONG day
unit.Initialize(def, …)  <- definition moi duoc gan o day
```

Sáu con lính sinh ra là sáu dòng đỏ, trong khi chẳng con nào thiếu gì.

**Chặn:** kiểm tra dữ liệu ở `Start`, không ở `Awake`. `Start` chạy sau toàn
bộ `Awake` của khung hình đó, nên lúc ấy mọi thứ gán sau `Instantiate` đã
xong. Con nào tới `Start` vẫn trống thì là thiếu thật.

Cùng họ với [B6] — thứ tự khởi tạo — nhưng biểu hiện ngược lại: B6 làm hỏng
chức năng, cái này chỉ làm Console đầy báo động giả, mà báo động giả thì che
mất lỗi thật.

---

### B8. Dấu của góc xoay phải ĐO, không được đoán

Viết bản nháp `AttackBow` tôi cho tay về `-44` để "giương lên ngắm". Sai
dấu: đối chiếu `Idle` (`-30 → -42` là tay buông lơi **xuống**) thì âm hơn
nghĩa là tay **hạ xuống**. Bản nháp đó cho con lính chúi ná xuống đất.

**Chặn:** trước khi viết key mới, đọc một clip đang chạy đúng và suy ra quy
ước dấu từ nó. Mốc hiện tại: `Body/Arm` nghỉ ở `-30`, **dương = nâng lên**,
**âm = hạ xuống**, chém bổ xuống chạm `-60`.

---

## C. Lỗi hình ảnh

### C1. `SpriteRenderer.color` là phép NHÂN — không sáng lên được

`r.color = Color.white` nhân với texture ra **đúng texture**. Code chạy,
không lỗi, không ai thấy gì.

**Chặn:** muốn sáng phải dùng shader lerp về màu sơn. Xem
`Assets/_Project/Art/Shaders/SpriteFlash.shader`.

---

### C2. Shader hồng cánh sen = không biên dịch được

Unity **không** hiện lỗi ở Console cho shader hỏng, chỉ tô hồng.

Lỗi đã gặp: `#pragma vertex vert` — trong `UnitySprites.cginc` hàm tên là
**`SpriteVert`**. Và `_Color`, `_RendererColor`, `_Flip`,
`_EnableExternalAlpha` đã được include khai sẵn, khai lại là xung đột.

**Chặn:** mở file `.cginc` thật trong thư mục Unity ra đối chiếu tên hàm,
đừng viết theo trí nhớ. Đường dẫn:
`<Unity>/Contents/Resources/CGIncludes/UnitySprites.cginc`

---

### C3. Pivot là TÂM XOAY, không chỉ là điểm neo

Và Unity **bỏ qua** `spritePivot` nếu `alignment` khác `9` (Custom).

Pivot là thứ duy nhất không tự động hoá được — nó là điểm bàn tay nắm, phụ
thuộc **hình dáng** chứ không phụ thuộc kích thước. Cây cung nắm giữa thân,
vũ khí cán nắm gần chuôi.

---

### C4. Chỉnh transform dùng chung để vừa MỘT món

`Weapon` là object trên prefab dùng chung. Căn nó vừa cây lao là **11 vũ khí
kia lệch theo**. Scale hai trục lệch nhau còn **bóp méo hình**.

**Chặn:**

| Chỉnh ở đâu | Cho cái gì |
|---|---|
| Transform của `Weapon` | **chung cả roster** |
| Pivot + kích thước file PNG | **riêng từng món** |

---

### C5. Hai lính chồng nhau thì bộ phận cắt ngang nhau

**Triệu chứng:** ba lính cùng ô — tay con này xuyên qua thân con kia, đầu con
sau nổi lên trước mặt con trước.

**Nguyên nhân:** mỗi lính có 7 bộ phận với `Order in Layer` **cố định**
(thân 0, giáp 1, đầu 2, mũ 3, vũ khí 4, tay 5). Hai lính đứng cạnh nhau thì
Unity so từng SpriteRenderer với nhau — tay con A (order 5) đè lên đầu con B
(order 2), bất kể ai đứng trước.

**Sai lầm dễ mắc:** đi sửa order của từng SpriteRenderer theo vị trí. Làm thế
là phá mất thứ tự **bên trong** một con.

**Đúng:** gắn `SortingGroup` vào gốc lính. Nó gom cả cây thành **một khối**;
Unity sắp xếp khối với khối, bên trong vẫn giữ nguyên 0..6.

```csharp
group.sortingOrder = Mathf.RoundToInt(-pos.y * 100f);   // thap hon = ve de len
```

---

### C6. Transform.position CỘNG với Origin

`Grid_Ally` ở `(-1.5, -3)` và `Origin (-1.5, -3)` → lưới thật nằm ở `(-3, -6)`.

**Chặn:** để Transform ở `(0,0)`, chỉ dùng `Origin`. Một nguồn sự thật.

---

### C7. Khung 9 mảnh không vẽ được nhỏ hơn hai lần bề rộng viền

Sprite 9 mảnh giữ nguyên bốn góc, chỉ kéo giãn phần giữa. Đặt khung thấp hơn
`2 × spriteBorder` là bốn góc **đè lên nhau** — viền méo, góc bo bị cắt.

Đã vấp: viền 36px, ô đếm quân đặt cao 64px. Hạ viền về 26px nên ngưỡng còn
52px, và `BattleHudBuilder` cảnh báo khi đặt nhỏ hơn.

Hai ràng buộc đi kèm nhau:

```
RADIUS   < BORDER          khong thi goc bo bi cat lam doi
be nho nhat >= 2 * BORDER  khong thi bon goc chong len nhau
```

---

### C8. EventSystem sai module thì nút không ăn, và không báo gì

Dự án đặt `activeInputHandler: 1` (chỉ Input System mới). Dùng
`StandaloneInputModule` cũ thì nút UI **không nhận cú bấm nào**.

**Đúng:** `InputSystemUIInputModule`. Và thiếu hẳn `EventSystem` cũng cho ra
đúng triệu chứng đó — bấm không ăn, Console im lặng.

---

### C9. "Nhìn lộn xộn" thường là lỗi ĐƯỜNG ĐI, không phải lỗi đội hình

Lính xếp tam giác đẹp trong ô, ra đường là tan thành một đám.

Nguyên nhân không nằm ở chỗ xếp mà ở `Chase`:

```csharp
MoveTowards(transform.position, target.transform.position, …)   // CA HAI TRUC
```

Đi cả hai trục về phía địch thì mọi con **hội tụ về đúng một điểm**, bao
nhiêu công xếp đội hình cũng mất.

**Đúng với trận đường thẳng:** chỉ đi theo x, giữ nguyên y của mình.

```csharp
p.x = Mathf.MoveTowards(p.x, target.transform.position.x, speed * dt);
```

Độ lệch đội hình được bảo toàn suốt chặng đi. Hợp lệ vì hai phe luôn ở hai
đầu trục x — không bao giờ cần đi lên xuống để tới được nhau.

---

### C10. Đội hình phải theo HÌNH DẠNG của ô, không giãn đều hai trục

"Ô nhỏ hơn 3 lính" — nhưng đo ra thì một con cao 0.589, ô cao 0.732: con
lính **vừa ô**, thấp hơn 20%.

Chỗ vỡ là đội hình. `formationSpread` là MỘT số dùng cho cả hai trục:

| | cụm 3 con | ô | |
|---|---|---|---|
| rộng | 0.696 | 1.033 | phí 33% |
| cao | 0.776 | 0.732 | **tràn 6%** |

Ô rộng-và-thấp, đội hình hẹp-và-cao. Tách thành `Vector2` rồi chọn theo
kích thước ô thì cụm vừa khít: 1.023 × 0.732.

Và điểm neo phải tính cho **cả cụm**, không phải một con: con đứng trước
lệch xuống `spread.y`, nên neo ở đúng đáy ô là chân nó thòi ra ngoài.
Nâng neo lên đúng `spread.y` thì chân con thấp nhất chạm mép dưới, đầu con
cao nhất chạm mép trên.

**Chặn:** bảng `Formations` chỉ chứa giá trị trong [-1, 1] — "chạm mép chỗ
cho phép" — còn kích thước thật nằm hết trong `formationSpread`. Đổi cỡ ô
thì không phải đụng vào bảng.

---

## D. Lỗi số liệu

### D1. Số chép từ bản cũ dùng PPU khác

Bản cũ PPU 128, bản này PPU 512 → mọi thứ bé đi 4 lần trong hệ toạ độ.
`AttackRange = 1.65` vốn là tầm bắn xa, giờ thành tầm **cận chiến**
(1.5 lần chiều cao nhân vật).

**Đã bỏ khỏi `GameBalance`:** `MeleeStrikeInterval`, `RangedShotInterval`,
`DefaultAttackRange`.

**Chặn:** mọi con số có đơn vị **độ dài** chép từ bản cũ đều phải đo lại.
Mốc hiện tại: nhân vật cao **1.10 đơn vị**.

---

### D2. Tên không nói rõ mốc đo

`AttackInterval` — tính từ lúc bắt đầu vung, hay sau khi clip xong? Hai cách
hiểu lệch nhau đúng bằng độ dài clip.

**Đã đổi thành** `AttackCooldown` = thời gian **nghỉ sau khi clip chạy xong**.

**Chặn:** dùng `[FormerlySerializedAs]` khi đổi tên field để không mất dữ
liệu — nhưng nhớ rằng **giá trị cũ được giữ lại theo nghĩa cũ**, phải tự
kiểm lại từng con số.

---

### D4. Đổi ĐƠN VỊ của một con số mà quên chỗ dùng nó

`WaveSpawnInterval` từ "giây mỗi CON" đổi sang "giây mỗi Ô". Cùng tên, cùng
kiểu, trình biên dịch im lặng.

Chỗ vỡ là `BattleLane.EntryPoint(team, index)`: nó lùi ô ra sau
`entrySpacing × index`. Hồi index đếm con thì 0.45 là hợp lý. Giờ index đếm
ô, nhưng 16 ô × 0.45 là ô cuối sinh ra ở x = −12.7 — ngoài khung nhìn 7 đơn
vị, phải đi thêm 3 giây mới vào tới trận.

**Chặn:** đổi đơn vị của một con số thì phải đi theo nó tới **mọi** chỗ đọc,
kể cả chỗ chỉ dùng nó làm chỉ số. Ở đây cách đúng là bỏ hẳn phép lùi — các
ô đã cách nhau bằng THỜI GIAN rồi, trong 0.45 giây ô trước đã đi 1.0 đơn vị.

---

### D5. Một trận 38 giây là quá chậm, và nó cộng dồn từ bốn chỗ

Phản hồi "animation quá chậm, user cần game nó nhanh" không chữa được bằng
một con số. Đo ra thì nó cộng từ bốn chỗ độc lập:

| | trước | sau |
|---|---|---|
| ra quân | 0.70s × 48 con = 33.6s | 0.45s × 16 ô = 7.2s |
| đi vào giữa | 4.1s (tốc độ 1.35) | 2.5s (tốc độ 2.2) |
| clip đánh | 1.00s | 0.62s (`attackSpeedMultiplier` 1.6) |
| hồi sau đòn | 1.00s | 0.45s |
| **một trận** | **~38s** | **~10s** |

Chỗ ăn thời gian lớn nhất không phải animation mà là **ra quân từng con**.

Và nút x1/x2/x3 dùng `Time.timeScale`, KHÔNG nhân tay từng chỗ: timeScale co
giãn `Time.deltaTime` nên bước chân, hồi chiêu, clip và đạn bay nhanh theo
cùng một nhịp. Nhân tay thì chắc chắn sót một chỗ, mà sót chỗ nào thì chỗ
đó lệch pha.

`Time.timeScale` là biến TOÀN CỤC và sống dai hơn scene — phải trả về 1
trong `OnDisable`, không thì thoát Play lúc đang x3 là lần sau chạy x3 ngay
từ đầu mà không gì báo.

---

### D3. Quy ước ngầm "đúng ba lần rồi sai lần thứ tư"

Tôi lấy *"khung áp chót"* làm điểm `OnAttackEvent` cho gọn. Đúng với cung và
lao, **sai với súng** — khung áp chót của súng là lúc hồi về.

**Chặn:** ghi rõ từng giá trị, đừng suy ra từ vị trí. Thêm `assert`.

---

## E. Ranh giới của Claude

| | |
|---|---|
| **Scene** (`.unity`) | **KHÔNG sửa được** — Unity giữ bản trong bộ nhớ, `Cmd+S` là đè mất |
| Prefab, clip, controller, `.asset`, `.mat`, shader, script | sửa được, Unity nạp lại từ đĩa |
| Console | đọc được qua `~/Library/Logs/Unity/Editor.log` |
| Cửa sổ Game lúc Play | **không thấy** — dùng `BattleProbe` để nó tự thuật lại |

---

## F. Lỗi khi tự viết công cụ sinh file

### F1. `L += [...]` trong hàm lồng nhau

```python
def build():
    L = []
    def helper():
        L += ["x"]        # UnboundLocalError
```

`L += ...` là phép **gán** (`L = L.__iadd__(...)`), nên Python coi `L` là biến
cục bộ của `helper`. `L.append()` / `L.extend()` thì không gán tên nên vẫn
trỏ ra ngoài.

---

### F2. `flags: 16` trên đường cong góc

Trong `m_EditorCurves`, `localEulerAnglesRaw.*` mang `flags: 16`, còn
`m_LocalPosition.*` mang `flags: 0`. Thiếu cờ đó thì cửa sổ Animation đọc ba
trục như ba số rời, không ghép lại thành một phép xoay.

---

### F3. Glob `Unity.*.dll` trượt `UnityEngine.UI.dll`

`tools/csharp_check.sh` nạp DLL package bằng `Library/ScriptAssemblies/Unity.*.dll`.
Mẫu đó đòi **dấu chấm ngay sau chữ Unity**, nên nó bỏ sót `UnityEngine.UI.dll`
và `UnityEditor.UI.dll`. Hệ quả: mọi file dùng uGUI báo
`namespace 'UI' does not exist` dù Unity thật biên dịch được.

Đã đổi thành `*.dll` và loại `Assembly-CSharp*` (đó là bản biên dịch của
chính các file đang kiểm — nạp vào là báo trùng định nghĩa kiểu).

**Bài học chung:** công cụ tự kiểm báo lỗi thì phải hỏi *"lỗi của mã, hay
lỗi của công cụ?"* trước khi đi sửa mã.

---

### F4. Cách duy nhất để tin một generator

Đừng sinh ra rồi mở Unity xem. Làm thế này:

1. Khai báo lại **file đang chạy đúng** bằng chính generator
2. So **từng byte** với bản trên đĩa
3. Chỉ khi khớp tuyệt đối mới đổi số

`tools/mkclip.py` giữ cả 5 clip, nên `python3 tools/mkclip.py` (không cờ)
là một bài kiểm tra hồi quy: sửa nhầm clip nào nó báo ngay clip đó.


---

### F4b. `replace()` không khớp thì IM LẶNG — và biên dịch không bắt được

Tôi sửa `BattleFlow.ReturnToBoard` để lính về đúng ô cũ, chạy
`csharp_check.sh` thấy "biên dịch sạch", rồi **báo với người dùng là xong**.

Phép thay chưa bao giờ khớp. Chuỗi đối chiếu của tôi chứa `phải kêu`, còn
file chứa `phải kеu` — chữ `е` Cyrillic lọt vào từ một lần sửa trước. Hai
chuỗi nhìn y hệt nhau.

File không đổi gì, nên dĩ nhiên nó biên dịch sạch. Người dùng chơi hai wave
rồi báo vị trí vẫn nhảy.

**Chặn — bắt buộc, không có ngoại lệ:**

```python
def sub(path, pairs):
    s = open(path).read()
    for a, b in pairs:
        assert a in s,            f'KHONG TIM THAY: {a[:120]}'
        assert s.count(a) == 1,   f'{s.count(a)} lan, mong doi 1'
        s = s.replace(a, b)
    open(path, 'w').write(s)
```

Hai dòng assert đó bắt cả hai kiểu hỏng: không khớp, và khớp nhiều chỗ.

**Bài học lớn hơn:** "biên dịch sạch" chứng minh mã HỢP LỆ, không chứng minh
mã ĐÃ ĐỔI. Sau mỗi lần sửa phải kiểm đúng thứ mình vừa sửa có mặt không —
`grep -c` một chuỗi đặc trưng là đủ.

---

### F5. Vá file bằng chỉ số dòng là tự chuốc lỗi

Sửa `BattleSceneSetup.cs` tôi cắt dán theo số dòng, và làm vỡ dấu ngoặc
**ba lần liên tiếp** — mỗi lần trình biên dịch chỉ chỉ ra triệu chứng ở cuối
file, không chỉ ra chỗ gây lỗi.

Hai điều rút ra:

1. **Thay bằng chuỗi nhận dạng được**, đừng thay bằng chỉ số dòng. Chỉ số
   dịch đi ngay sau lần sửa đầu tiên.
2. **Đếm ngoặc sau mỗi lần sửa**, đừng đợi trình biên dịch:

```python
d = 0
for i, l in enumerate(lines, 1):
    c = l.split('//')[0]
    d += c.count('{') - c.count('}')
    if d < 0: print('AM o dong', i)
print('cuoi file:', d)       # phai bang 0
```

Và kiểm độ sâu tại mỗi khai báo thành viên — phải đồng loạt bằng 2
(namespace 1 + class 1). Chỗ nào lệch là chỗ đó vỡ.

---

### F6. Số đo nghệ thuật phải ĐO bằng công cụ, và công cụ phải ở lại repo

16 ô và mặt đường đều **vẽ sẵn trong ảnh nền**. Ô logic phải trùng ô vẽ,
lệch là lính đứng chệch — mà không có gì báo, vì logic vẫn đúng.

Cách làm đã dùng:

```
do bang script  ->  ghi lai thanh TI LE (khong phai pixel)
                ->  nhan voi kich thuoc anh DA PHONG
                ->  ve o logic de len anh, nhin bang mat
```

Ba điểm quan trọng:

- **Tỉ lệ, không phải pixel.** Ảnh được phóng cho phủ kín khung nhìn, nên
  pixel trong file gốc chẳng nói lên gì.
- **Một công thức phóng duy nhất.** `BattleBackground.CoverSize` là `public
  static` để script Editor gọi chung. Hai bên tự tính là sớm muộn cũng lệch.
- **Công cụ đo ở lại repo** (`tools/measure_prep_grid.py`,
  `tools/measure_road.py`). Đổi ảnh nền thì chạy lại, không phải đo lại bằng mắt.

---

### F7. Ảnh AI không có hình học đều — đừng bắt logic chạy theo

16 ô vẽ trong `bg_prep.png` nhìn như lưới, nhưng đo ra thì **có phối cảnh**:

| | hàng 1 (trên) | hàng 2 | hàng 3 | hàng 4 |
|---|---|---|---|---|
| bước cột | 133 | 141 | 148 | 153 |
| bước hàng | — | 94 | 104 | 114 |

Ô dưới to hơn ô trên 15%. `BattleGrid` là lưới chữ nhật cứng nên **không bộ
số nào khớp được cả 16 ô** — xấp xỉ tốt nhất vẫn lệch 44px (0.3 đơn vị trên
ô rộng 1.03).

Tôi đã mất mấy vòng đo đi đo lại trước khi nhận ra điều đó. Dấu hiệu nhận
biết sớm: **hai phép đo độc lập cho ra hai kết quả khác nhau mà không cái
nào sai rõ ràng**. Lúc đó đừng tinh chỉnh tiếp — mô hình sai, không phải
phép đo sai.

**Cách giải:** đổi chiều phụ thuộc. Thay vì bắt lưới logic đuổi theo nét vẽ,
`tools/align_prep_grid.py` **vẽ lại 32 ô vào đúng lattice**, giữ nguyên phần
còn lại của ảnh. Sai số còn 1.1px = 0.008 đơn vị. Bản AI gốc để ở
`artsrc/bg_prep_ai.png`.

Hai chi tiết khi vẽ đè lên art có sẵn:

- **Bóng ma.** Lần đầu tôi giữ lại mọi điểm cỏ nên viền cỏ mỏng của ô cũ
  sống sót, vẽ ô mới lên thì thấy hai đường viền song song. Phép **mở**
  (co lại rồi nở ra) xoá viền mỏng mà giữ bụi cỏ to.
- **Lát ảnh thật thì lặp.** Mảng đất lấy mẫu có hòn sỏi, lát ra là hòn sỏi
  đó xuất hiện ở mọi ô. Đất thật đo được độ lệch chuẩn chỉ 3–4, nên sinh
  hạt ngẫu nhiên đúng biên độ đó là xong.

---

### F8. Một phép đo chỉ đáng tin khi có phép thứ hai xác nhận

Suốt lượt này cái gì cũng phải hai đường:

| | |
|---|---|
| vị trí ô | khớp lattice **và** trọng tâm từng ô |
| hai lưới | đo riêng từng bên rồi đối chiếu chéo |
| kết quả | chạy lại công cụ đo lên chính ảnh đã sửa |

Phép đo cuối là rẻ nhất mà mạnh nhất: **đo lại thứ mình vừa tạo ra**. Nếu nó
không trả về đúng con số đã dùng để tạo thì có chỗ sai trong chuỗi.

---

### D6. Luật xác suất không kiểm được bằng cách chơi thử

Roll là hệ xác suất. Bấm tay mười lượt rồi thấy "trông hợp lý" chẳng chứng
minh gì — một bảng trọng số sai vẫn cho kết quả có vẻ đúng ở mười lượt đầu
và chỉ lộ sau vài trăm lượt, tức là lộ với người chơi chứ không lộ với mình.

`SpinSquad → Kiểm phân bố roll` chạy **100 000 lượt** bằng ĐÚNG code của
game (không phải bản mô phỏng) rồi đối chiếu:

- tần suất mỗi số ô Ally **so với** `AllyCountWeights` đã khai
- phân bố cụm lớn nhất, tỉ lệ Jackpot
- coin thu về mỗi lượt so với giá roll — biết game đang lạm phát hay thắt
- **số lượt trắng tay phải bằng 0**; khác 0 là luật loại (2,2,2) hỏng

Hạt giống cố định nên chạy lại ra y hệt, và so được hai lần chạy với nhau.

**Nguyên tắc:** code nào có chữ `Random` thì phải có một công cụ đếm đi kèm.

---

### B10. Xoá danh sách đăng ký sự kiện là chặn luôn đường hồi sinh

`Unit.Die()` làm:

```csharp
var handler = Died;
Died = null;              // "nha truoc khi goi -> khong the ban lan hai"
handler?.Invoke(this);
```

Biện pháp phòng thừa: chốt thật đã nằm ở `if (!IsAlive) return` đầu
`TakeDamage`, cộng `IsAlive = false` đặt ngay trên. `Die()` không thể vào
lần hai.

Nhưng khi luật đổi thành "lính chết quay lại ở wave sau", dòng `Died = null`
thành lỗi thật: `HealthBar` mất đăng ký vĩnh viễn, nên con từng chết thì lần
chết sau thanh máu không ẩn nữa. `UnitBrain` thì may mắn thoát vì nó
`enabled = false/true` mỗi wave nên tự đăng ký lại.

**Bài học:** một lớp phòng thừa không vô hại. Nó là ràng buộc ngầm, và khi
luật đổi thì nó cản đường mà chẳng ai nhớ nó ở đó.

---

## G. Lỗi API Unity

### G0. `Camera.aspect` không đáng tin, đừng cho nó vào công thức

Nền được phóng cho "phủ kín khung nhìn", hệ số phụ thuộc `Camera.aspect`.
Hai chỗ đọc ra hai giá trị khác nhau:

- script Editor đọc lúc **chưa Play** — khi đó nó có thể là tỉ lệ của Scene
  view, không phải Game view
- Game view để **"Free Aspect"** thì nó đổi theo mỗi lần kéo cửa sổ

Kết quả: gốc lưới nướng theo một tỉ lệ, nền co theo tỉ lệ khác, lính đứng
chệch khỏi ô vẽ. Không có gì báo.

**Chặn:** mọi con số mà cả Editor lẫn runtime cùng cần phải tính từ thứ
**không đổi theo cửa sổ**. Ở đây là `ortho`:

```csharp
k = ortho * 2f / sprite.bounds.size.y;   // chi phu thuoc ortho
```

Đổi lại thì cửa sổ rộng hơn tỉ lệ ảnh sẽ hở hai bên — **cố ý để lộ**, hơn là
âm thầm xô lệch cả bàn cờ. `BattleBackground.WarnIfTooWide` kêu khi gặp.

---

### G2. Khoá tỉ lệ khung nhìn, đừng đọc `Camera.aspect`

Nối tiếp [G0]. Lần đó tôi chữa bằng cách neo ảnh theo CHIỀU CAO — bỏ được
`Camera.aspect` khỏi công thức, nhưng đẻ ra triệu chứng mới: cửa sổ rộng hơn
16:9 thì ảnh chỉ phủ 69% bề ngang, hai bên lòi ra **màu nền camera**. Nhìn
y như lỗi đồ hoạ, và người dùng báo đúng như thế.

Chữa triệu chứng thì ra triệu chứng khác. Gốc là: **tỉ lệ khung nhìn không
được phép là một biến**.

`LetterboxCamera` ép `camera.rect` về đúng 16:9 trong mọi cửa sổ. Từ đó:

- mọi phép tính dùng một hằng số `DesignAspect`, Editor và runtime luôn trùng
- ảnh phủ kín khung 16:9, không hở
- cửa sổ lạ tỉ lệ thì có viền ĐEN — đó là letterbox, thứ mọi game đều làm,
  và nó đọc ra là cố ý chứ không phải hỏng

---

### G3. "Đồ hoạ xấu" — đừng đoán nguyên nhân, bắt game tự khai

Tôi đã đoán sai một lần: quy cho thanh **Scale** của cửa sổ Game, nói rằng
2x nghĩa là render ở nửa độ phân giải. Người dùng trả lời **"scale min ở đây
đã là 2 rồi"** — trên màn Retina thì 2x chính là gốc, không phải nửa.

Nguyên nhân thật đơn giản hơn: **ô cửa sổ Game quá nhỏ**. Vùng 16:9 chỉ có
chừng 730×408 pixel, nên một con lính cao 165px bị vẽ xuống **37px** — thu
4.5 lần. Ở 1920×1080 thì nó là 98px, thu 1.7 lần, và đẹp.

Nên `LetterboxCamera.ReportResolution` giờ **in ra con số** mỗi lần Play:

```
Vung ve 16:9 that: 730x408. Mot con linh cao 37 px.
```

Có con số thì hết phải đoán. Và chiều cao khung nhìn truyền vào chứ không
viết cứng 6.52 — đổi camera là con số kia sai mà không ai biết.

---

### G4. Sprite bị thu nhỏ mà không bật mipmap thì rìa răng cưa

Toàn bộ art để `enableMipMap: 0`. Thu nhỏ 4.5 lần mà không có mipmap thì
mỗi pixel đầu ra chỉ lấy 4 texel trong số hai chục texel nó đang đại diện —
phần còn lại bị vứt. Rìa răng cưa, và nhấp nháy khi vật thể nhúc nhích.

Ba khoá, đặt bằng `tools/set_sprite_quality.py`:

| | | |
|---|---|---|
| `enableMipMap` | 1 | sinh các bản thu nhỏ đã lấy trung bình cả vùng |
| `mipMapsPreserveCoverage` | 1 | giữ độ phủ alpha; thiếu nó thì rìa trong suốt bị ăn mòn dần |
| `filterMode` | 2 | Trilinear, hoà giữa hai mức mipmap |

`mipMapsPreserveCoverage` để **0**. Nó dành cho vật liệu alpha-test (cutout),
nơi mỗi texel chỉ đặc hoặc trong. Với sprite alpha-**blend** thì phép nhân
alpha của nó chỉ làm phình rìa. Đo trên art này ra hệ số 0.96 — gần như vô
hại — nhưng đặt sai thì sớm muộn cũng cắn.

**Nhớ vá cả công cụ sinh `.meta`.** `make_ui.py` tự ghi `.meta` theo mẫu cũ,
nên chạy lại nó là mất hết thiết lập vừa đặt.

Mipmap làm sạch rìa, nó **không** làm 37px thành 98px. Hai việc khác nhau.

---

### C11. Tách nền magenta cần BA bước, không phải một

AI trả art trên nền magenta đặc. "Xoá điểm nào màu magenta" là chưa đủ —
lần đầu tôi làm thế và còn sót **34 651 điểm** trên 12 file.

| bước | chữa kiểu sót nào |
|---|---|
| 1. khoảng cách tới màu key, ngưỡng **rộng** (90…150) | nhiễu nén WebP đẩy điểm nền lệch hàng chục đơn vị — ngưỡng hẹp là chúng sống sót **nguyên vẹn**, magenta ĐỤC alpha 255, rải khắp ảnh |
| 2. theo **sắc**: Lam và Đỏ cao mà Lục thấp | bắt nốt chỗ bước 1 không với tới |
| 3. **giải ngược phép trộn** ở rìa | rìa đã khử răng cưa nên mỗi điểm là `thật×a + key×(1−a)`; xoá thẳng thì còn viền hồng |
| 4. **lan màu** từ vùng đặc ra rìa | bước 3 chia cho alpha, alpha nhỏ thì kết quả nhiễu. Thay hẳn màu rìa bằng màu hàng xóm đặc là hết phép chia |

Ngưỡng rộng an toàn vì màu thật gần magenta nhất (đỏ của trái tim) vẫn cách
**206** — đo trước rồi mới chọn ngưỡng, đừng đoán.

Kiểm bằng cách đếm điểm còn sót, không bằng mắt: `0 — SACH`.

---

### C12. Hai sprite thay nhau tại chỗ phải CÙNG khuôn

`button` và `button_down` dùng chung một `Image`, chỉ đổi sprite khi nhấn.
AI trả về cao 239 và 243, viền đo ra 242 và 240 — khác PPU, khác
`spriteBorder`, nên lúc nhấn nút sẽ **nhảy cỡ**.

Phải đệm về cùng chiều cao và ép cùng một `spriteBorder`. Cùng họ: hai ô
`reel_slot` / `reel_slot_win` cũng vậy.

---

### C13. Art quyết định layout, không phải ngược lại

Hai nắp trang trí của bộ art này chiếm **112 đơn vị mỗi bên**, nên không nút
nào được hẹp hơn 224. Nút tốc độ tôi đặt 150 — hai nắp chồng lên nhau.

Và chữ không được phủ kín khung: nó phải **thụt vào** đúng bề rộng nắp, còn
không thì chuỗi dài leo lên mặt gỗ. Kèm `resizeTextForBestFit` để chuỗi dài
tự co — không có nó thì mỗi lần đổi chữ lại phải dò cỡ bằng tay.

Cách phát hiện: ghép thử **mọi cỡ** rồi nhìn, trước khi mở Unity.
`SliceBorder` trong code phải bằng số đo từ ảnh, nên đo bằng công cụ chứ
đừng gõ tay.

---

### G5. Ảnh chụp màn hình bị nén thì không dùng để soi lỗi đồ hoạ được

Có báo "viền magenta quanh nhân vật". Soi ra thì đúng là có vệt tím ở rìa,
kênh Lam vọt lên **123** trong khi đất là 89 và thân lính là 35 — **cao hơn
cả hai bên**, không phép trộn nào cho ra thế.

Nhưng đo tiếp ở những rìa khác trong cùng tấm ảnh:

| rìa | vọt lố kênh Lam |
|---|---|
| quanh con lính | +36 |
| mép đường / cỏ | **+59** |
| viền cây thông | **+44** |

Vọt lố có ở **mọi** rìa sắc nét, và ở chỗ chắc chắn không phải lỗi game thì
còn mạnh hơn. Đó là ringing của nén ảnh, không phải của engine.

**Chặn:** trước khi tin một màu lạ trong ảnh chụp, đo cùng hiện tượng ở một
rìa đối chứng — chỗ mà ta biết chắc không có lỗi. Nếu nó cũng xuất hiện ở đó
thì thủ phạm là khâu nén, không phải thứ đang xem.

Và đừng vội kết luận ngược lại: so hai ảnh trước/sau cùng một vùng đã cho
thấy mipmap xoá được một quầng **đen răng cưa** hoàn toàn có thật.

---

### G1. `??` KHÔNG dùng được với đối tượng Unity

```csharp
go.GetComponent<T>() ?? Undo.AddComponent<T>(go)   // SAI
```

`??` của C# so với null **thật**. Nó không gọi toán tử `==` mà Unity nạp
chồng. Một component đã bị huỷ ở phía C++ vẫn còn vỏ bọc C# khác null thật,
nên `??` trả về cái vỏ rỗng đó thay vì thêm component mới.

Triệu chứng đã gặp: object `Background` giữ một `SpriteRenderer` đã huỷ.

```
Undo object may not be null.
MissingComponentException: There is no 'SpriteRenderer' attached to
the "Background" game object, but a script is trying to access it.
```

Và vì nó **ném ngoại lệ giữa lệnh dựng scene**, mọi bước sau đó — lưới, Lane,
Spawner, HUD — không chạy. Một dòng `??` làm hỏng cả lệnh, mà thông báo lỗi
không hề nhắc tới nó.

**Đúng:**

```csharp
var c = go.GetComponent<T>();
return c != null ? c : Undo.AddComponent<T>(go);   // != goi toan tu cua Unity
```

Quy tắc chung: với bất cứ thứ gì kế thừa `UnityEngine.Object`, dùng
`!= null` / `== null`, **không bao giờ** dùng `??` hay `?.`.

---

## Công cụ tự kiểm

```bash
./tools/csharp_check.sh      # bien dich C# ma khong can tat Unity
python3 tools/check_refs.py  # tim GUID tro vao hu khong
python3 tools/mkclip.py      # 5 clip con khop voi ban mo ta khong
python3 tools/set_sprite_quality.py   # moi sprite da bat mipmap chua
```

Chạy cả bốn sau mỗi lần sinh file hàng loạt.
