# 05 — Code mang sang từ bản cũ

Đường dẫn gốc: `/Users/vuhagiabao/SpinSquadClone`

---

## 1. Copy gần như nguyên xi ✅

Những file này viết sạch, độc lập, có comment tiếng Việt tốt.
**Đọc hiểu rồi mới copy** — đừng copy mù.

### Gacha — `Assets/SpinSquad/Scripts/Runtime/Gacha/`

| File | Dòng | Ghi chú khi copy |
|---|---|---|
| `RollCellKind.cs` | ~10 | nguyên xi |
| `SixSlotRollGenerator.cs` | ~90 | nguyên xi. **Thuật toán hay nhất project** |
| `SixSlotRollResolver.cs` | ~180 | đưa các số hardcode ra `GameBalance` |
| `RollPayout.cs` | ~45 | nguyên xi |
| `BuffStatKind.cs` | ~12 | nguyên xi |
| `BattleRunBuffs.cs` | ~60 | **bỏ `static`** — xem cảnh báo bên dưới |
| `RollWallet.cs` | ~50 | **viết lại** — xem cảnh báo bên dưới |

Toàn bộ thư mục này gần như không phụ thuộc UnityEngine → dễ viết unit test,
dễ hiểu, dễ sửa. Đây là phần tốt nhất của bản cũ.

### Data — `Assets/SpinSquad/Scripts/Runtime/Data/`

| File | Ghi chú |
|---|---|
| `Rarity.cs` | nguyên xi |
| `UnitTeamKind.cs` | nguyên xi |
| `UnitDefinition.cs` | **dọn bớt** — xem mục 3 |

### Gameplay — `Assets/SpinSquad/Scripts/Runtime/Gameplay/`

| File | Ghi chú |
|---|---|
| `AllyStatScaling.cs` | giữ hàm `ScaleStats`, bỏ các hằng số "giữ cho tương thích" |
| `AllyMergeRules.cs` | giữ `NextRarity`, `CanMerge`; bỏ phần Mythic Combine chưa làm |
| `IBattleVisualDriver.cs` | đổi tên `IUnitVisual`, thêm `PlayHit`/`PlayDeath` (file 03) |

### Character

| File | Ghi chú |
|---|---|
| `Characters/HumanCommonClubFighter/Runtime/HumanCommonClubFighterVisual.cs` | bản đã dọn sẵn nằm ở file 04 mục 6 |
| `Characters/HumanCommonClubFighter/Editor/HumanCommonClubFighterBuilder.cs` | **đừng copy** — dùng làm *mẫu tham khảo* cho Editor script sau này |

---

## 2. Hai cảnh báo bắt buộc sửa khi copy

### ⚠️ `RollWallet` dùng `PlayerPrefs` cho coin trong trận

```csharp
// BẢN CŨ — sai
public static int Balance
{
    get => PlayerPrefs.GetInt("SpinSquad_RollCoins", 40);
    set { PlayerPrefs.SetInt(...); PlayerPrefs.Save(); }   // ghi đĩa MỖI LẦN
}
```

Ba vấn đề: coin trong trận là state tạm mà lại ghi xuống đĩa; `PlayerPrefs.Save()`
mỗi lần đổi coin là I/O thừa; và test tự động sẽ làm bẩn PlayerPrefs thật.

**Bản mới**: coin là field thường trong `BattleFlow` hoặc một `RunState` object.

```csharp
public sealed class RunState
{
    public int Coins { get; private set; }
    public void Reset(int starting) => Coins = starting;
    public bool TrySpend(int amount)
    {
        if (Coins < amount) return false;
        Coins -= amount;
        return true;
    }
    public void Add(int amount) => Coins = Mathf.Max(0, Coins + amount);
}
```

### ⚠️ `BattleRunBuffs` là `static class` với state

```csharp
public static class BattleRunBuffs
{
    public static float HpPct { get; private set; }   // state toàn cục
    ...
}
```

State static sống xuyên suốt cả lần chạy game. Nếu quên gọi `Clear()` đúng
chỗ, buff của trận trước rò sang trận sau. Bản cũ phải nhớ clear thủ công
trong `SpawnAndRegisterFighters` — đúng loại bug khó tìm.

**Bản mới**: làm thành object thường, đặt trong `RunState`, tạo mới mỗi trận.
Không cần `Clear()` thì không thể quên `Clear()`.

---

## 3. Dọn `UnitDefinition`

Bản cũ có 14 field, nhiều cái là vết tích:

```csharp
❌ Object battlePrefab              + string battlePrefabResourcesPath
   → hai đường load song song cho cùng một thứ. Giữ MỘT: GameObject thường.

❌ string uiAnimatorResourcesPath   → animator cho portrait UI, chưa dùng
❌ string rangedBoltTextureResourcesPath → đưa vào prefab đạn
❌ UnitBodyShape bodyShape          → dùng để vẽ hình khối fallback, bỏ được
❌ OnValidate() ép Triangle = ranged → logic ẩn, gây bất ngờ
❌ #if UNITY_EDITOR SetEditorData()  → chỉ cần khi sinh asset bằng script
```

Bản mới gọn lại:

```csharp
[CreateAssetMenu(menuName = "SpinSquad/Unit Definition")]
public sealed class UnitDefinition : ScriptableObject
{
    public string UnitId;
    public string DisplayName;
    public int LineIndex;              // 0–4, KHAI THẲNG, không suy từ chuỗi id
    public Rarity BaseRarity;
    public UnitTeam Team;

    [Header("Chỉ số tham chiếu (mốc Common = 100 HP / 15 ATK)")]
    public float ReferenceHp = 100f;
    public float ReferenceAttack = 15f;

    [Header("Kiểu đánh")]
    public bool IsRanged;
    public float AttackRange = 1.65f;
    public float AttackInterval = 0.52f;

    [Header("Hình ảnh")]
    public GameObject BattlePrefab;
    public Sprite Portrait;
}
```

14 field → 12, nhưng không còn field nào mơ hồ.

---

## 4. Đọc tham khảo, viết lại từ đầu 📖

| File cũ | Vì sao không copy |
|---|---|
| `MetaProgressionStore.cs` (645 dòng) | logic gold/key/unlock đúng nhưng trộn với PlayerPrefs và UI. Đọc để lấy **luật**, viết lại dùng JSON |
| `BattleGrid.cs` (406 dòng) | ý tưởng đúng (toạ độ ô ↔ world) nhưng ôm luôn cả việc **vẽ** lưới và panel. Tách: toạ độ ← code, hình ảnh ← prefab |
| `DuelActor.cs` (604 dòng) | ba nhánh combat ở đầu file là **spec quý**, đọc kỹ. Nhưng code đã chắp vá nhiều lớp cooldown |
| `AllyBenchInventory.cs` (234 dòng) | luật túi lính đúng, code rối |

---

## 5. Không mang sang ❌

| Đường dẫn | Lý do |
|---|---|
| `Scripts/Runtime/Gameplay/DuelDirector.cs` | 5.765 dòng god class. **Tuyệt đối không** |
| `Assets/Spine/` (~130 file) | thư viện Spine, tốn bản quyền, chưa dùng |
| `Assets/SpinSquad/SpinForge/` | pipeline dở dang |
| `Assets/SpinSquad/SpinAssetLab/` | mocap 3D, sai hướng |
| `Scripts/Runtime/Presentation/*25D.cs` | vết tích hướng 2.5D đã bỏ |
| `Scripts/Runtime/UI/*UiSprites.cs` (7 file) | bảng tra sprite hardcode, dùng Addressables hoặc gán Inspector |
| `Scripts/Editor/*` (17 file) | công cụ vá lỗi cho vấn đề của bản cũ |
| Mọi file `.md` ở root và `Docs/` | đã thay bằng folder `Handoff/` này |

---

## 6. Asset mang sang

### Mang ✅

```
Assets/SpinSquad/Characters/HumanCommonClubFighter/Art/FramesV2/
    → 28 frame (Idle 6, Walk 8, Attack 8, Dead 6), đã chuẩn 320×320
    → nhân vật duy nhất chạy được. Dùng làm mẫu chuẩn cho mọi con sau

Assets/SpinSquad/Resources/UI/ButtonKit/
Assets/SpinSquad/Resources/UI/Hud/
Assets/SpinSquad/Resources/UI/Backgrounds/
    → UI art dùng được ngay

Assets/SpinSquad/Resources/Characters/HumanEpicArcaneMage/Frames/Individual/
    → nhiều frame (Attack 20+, Walk, Idle, Die, Skill)
    → CHƯA kiểm chuẩn canvas/pivot. Kiểm trước khi tin dùng
```

### Không mang ❌

```
*.psd, *.kra                     → file nguồn, để ngoài repo
Assets/SpinSquad/Art/UI/SourceSheets/   → sheet nguồn 2MB+, để ngoài repo
Assets/SpinSquad/Data/alliesUI/         → tile trích từ kit ngoài, bản quyền không rõ
Assets/SpinSquad/Resources/UI/AnimalKitLocal/  → kit ngoài, đã gitignore
```

---

## 7. Checklist khi tạo project mới

```
□ Unity Hub → New Project → template 2D (Built-in Render Pipeline) → 6000.4.1f1
□ Tạo cây thư mục theo file 03 mục 1
□ .gitignore chuẩn Unity (xem file 08) — LÀM TRƯỚC commit đầu tiên
□ git init, commit đầu tiên khi project còn rỗng
□ Copy thư mục Gacha/ → sửa 2 cảnh báo ở mục 2
□ Copy Rarity.cs, UnitTeamKind.cs
□ Viết GameBalance.cs (file 03 mục 3), tạo 1 asset từ nó
□ Viết UnitDefinition.cs gọn (mục 3 file này)
□ Chưa viết gì về battle. Milestone 1 ở file 06 sẽ nói làm gì tiếp.
```
