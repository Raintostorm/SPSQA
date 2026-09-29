# 03 — Kiến trúc bản mới

Mục tiêu: **không bao giờ có file 5.000 dòng nữa**, và bạn luôn biết một thứ
nằm ở đâu mà không cần tìm.

---

## 1. Cây thư mục

```
Assets/
├── _Project/                    ← tất cả code & asset của bạn, gạch dưới để nổi lên đầu
│   ├── Scripts/
│   │   ├── Data/                ← ScriptableObject, enum. KHÔNG có logic.
│   │   │   ├── Rarity.cs
│   │   │   ├── UnitDefinition.cs
│   │   │   ├── UnitTeam.cs
│   │   │   └── GameBalance.cs           ← mọi con số ở file 01, một chỗ duy nhất
│   │   ├── Gacha/               ← thuần C#, KHÔNG dùng UnityEngine (trừ Mathf)
│   │   │   ├── RollCellKind.cs
│   │   │   ├── RollGenerator.cs
│   │   │   ├── RollResolver.cs
│   │   │   ├── RollPayout.cs
│   │   │   └── RunBuffs.cs
│   │   ├── Battle/              ← logic trận, mỗi file một việc
│   │   │   ├── BattleState.cs           ← enum: Prep, Countdown, Combat, WaveClear, Defeat
│   │   │   ├── BattleFlow.cs            ← MonoBehaviour: chuyển state, đếm wave
│   │   │   ├── UnitSpawner.cs           ← chỉ spawn
│   │   │   ├── BattleGrid.cs            ← toạ độ ô ↔ world, không vẽ gì
│   │   │   ├── Unit.cs                  ← HP, ATK, faction, chết
│   │   │   ├── UnitBrain.cs             ← tìm mục tiêu, đi, đánh
│   │   │   ├── MergeSystem.cs
│   │   │   └── UnitDragHandler.cs
│   │   ├── Visual/              ← cầu nối logic ↔ animation
│   │   │   ├── IUnitVisual.cs           ← interface, xem mục 4
│   │   │   └── SpriteUnitVisual.cs      ← bản cài đặt sprite-frame
│   │   ├── Meta/
│   │   │   ├── SaveData.cs
│   │   │   └── SaveSystem.cs
│   │   └── UI/                  ← mỗi màn hình một file, chỉ bind, không dựng
│   │       ├── HomeScreen.cs
│   │       ├── BattleHud.cs
│   │       └── RollPanel.cs
│   ├── Art/
│   │   ├── Characters/<TenNhanVat>/     ← xem file 04
│   │   └── UI/
│   ├── Prefabs/
│   │   ├── Units/
│   │   └── UI/
│   ├── Data/                    ← file .asset của ScriptableObject
│   └── Scenes/
│       ├── Home.unity
│       ├── Battle.unity
│       └── CharacterLab.unity   ← scene duyệt animation, xem file 04
└── Plugins/                     ← thư viện bên thứ ba, nếu có
```

**Vì sao `_Project/`**: mọi asset của bạn nằm gọn một chỗ, tách hẳn khỏi
thư viện import về. Bản cũ trộn lẫn nên không biết cái gì là của mình.

---

## 2. Nguyên tắc chia file

### Quy tắc một câu

> Mỗi file phải mô tả được bằng **một câu không có chữ "và"**.

- `UnitSpawner.cs` — "tạo lính lên bàn" ✅
- `BattleManager.cs` — "quản lý trận **và** UI **và** spawn **và** merge" ❌

### Ba tầng, không được đi tắt

```
   DATA  (ScriptableObject, struct thuần)
     ↑ đọc
   LOGIC (Gacha, Battle, Meta)      ← không biết UI tồn tại
     ↑ event
   VIEW  (UI, Visual, animation)    ← chỉ đọc logic, không sửa
```

Luật: **tầng dưới không được gọi tầng trên.**
`UnitBrain` không được `GetComponent<BattleHud>()`. Nó chỉ bắn event,
HUD tự đăng ký nghe.

Cách làm đơn giản nhất trong Unity:

```csharp
// trong Unit.cs
public event Action<float> HealthChanged;
public event Action Died;

// trong UnitHealthBar.cs (View)
void OnEnable()  => unit.HealthChanged += Redraw;
void OnDisable() => unit.HealthChanged -= Redraw;
```

---

## 3. `GameBalance.cs` — một chỗ cho mọi con số

Đây là thứ bản cũ thiếu nhất. Các con số nằm rải trong 20 file.

```csharp
using UnityEngine;

namespace Project.Data
{
    [CreateAssetMenu(menuName = "SpinSquad/Game Balance")]
    public sealed class GameBalance : ScriptableObject
    {
        [Header("Roll")]
        public int RollCost = 20;
        public int StartingCoins = 40;
        public int CoinPerWaveClear = 40;
        public int SellRefund = 12;

        [Header("Coin theo cụm 3/4/5/6")]
        public int[] CoinByCluster = { 15, 40, 80, 200 };

        [Header("Stat gốc theo rarity")]
        public float[] BaseHp  = { 100f, 300f, 900f, 2700f, 8100f };
        public float[] BaseAtk = {  15f,  45f, 135f,  305f,  915f };

        [Header("Combat")]
        public float MoveSpeed = 1.35f;
        public float MeleeStrikeInterval = 0.24f;
        public float RangedShotInterval = 0.52f;
        public float DefaultAttackRange = 1.65f;
        public float EngageDelay = 2f;

        [Header("Wave")]
        public int WavesPerLevel = 10;
        public int LevelCount = 3;
    }
}
```

Tạo **một** file `.asset` từ đây, kéo vào scene. Chỉnh số bằng Inspector,
không cần compile lại. Bạn sẽ cảm ơn mình vì điều này.

---

## 4. `IUnitVisual` — cầu nối logic ↔ animation

Phần này bản cũ làm **đúng**, giữ nguyên tinh thần. Đây là hợp đồng giữa
combat và animation, cho phép bạn đổi pipeline animation mà không sửa combat.

```csharp
using System;

namespace Project.Visual
{
    /// <summary>Combat gọi interface này. Combat KHÔNG biết bên dưới là
    /// sprite-frame, Spine hay gì khác.</summary>
    public interface IUnitVisual
    {
        /// <param name="onImpact">Gọi ĐÚNG MỘT LẦN tại frame chạm. Đây là lúc trừ máu.</param>
        /// <param name="onFinished">Gọi khi clip attack kết thúc.</param>
        void PlayAttack(Action onImpact, Action onFinished = null);

        void SetMoving(bool isMoving);
        void PlayIdle();
        void PlayHit();
        void PlayDeath();

        /// <param name="dirX">&lt;0 quay trái, &gt;0 quay phải.</param>
        void SetFacing(float dirX);
    }
}
```

### Vì sao `onImpact` là callback chứ không phải `Invoke` ngay

Vì damage phải rơi đúng **khoảnh khắc gậy chạm người**, không phải lúc bắt đầu
vung. Cảm giác đánh đấm của game nằm ở chỗ này. Xem chi tiết ở file 04 mục 5.

### Bản cũ có 5 method, bản mới đề xuất 6

Thêm `PlayHit()` và `PlayDeath()` — bản cũ để combat tự xử lý chết bằng
`UnitDeathSequence` riêng, gây rối. Gom vào interface cho gọn.

---

## 5. Máy trạng thái trận

```csharp
public enum BattleState
{
    Prep,        // roll, kéo thả, merge. Thời gian không chảy cho combat.
    Countdown,   // 2s sau khi bấm Start, lính đứng yên
    Combat,      // lính tự đánh
    WaveClear,   // banner + thưởng, chờ bấm tiếp
    LevelClear,
    Defeat
}
```

`BattleFlow.cs` chỉ làm **một việc**: giữ state hiện tại và quyết định
khi nào chuyển. Mọi hệ thống khác *hỏi* state, không *đổi* nó.

```csharp
public sealed class BattleFlow : MonoBehaviour
{
    public BattleState State { get; private set; } = BattleState.Prep;
    public int Wave { get; private set; } = 1;
    public event Action<BattleState> StateChanged;

    void SetState(BattleState next)
    {
        if (State == next) return;
        State = next;
        StateChanged?.Invoke(next);
    }
}
```

Ước chừng: **dưới 150 dòng**. So với 5.765.

---

## 6. Save — bỏ PlayerPrefs

Bản cũ dùng `PlayerPrefs` cho cả coin lẫn meta. Không mở rộng được,
không xem được, không backup được.

```csharp
[Serializable]
public sealed class SaveData
{
    public int Gold;
    public int TreasureKeys;
    public int HighestLevelUnlocked = 1;
}

public static class SaveSystem
{
    static string Path => Application.persistentDataPath + "/save.json";

    public static void Save(SaveData d) =>
        File.WriteAllText(Path, JsonUtility.ToJson(d, true));

    public static SaveData Load() =>
        File.Exists(Path)
            ? JsonUtility.FromJson<SaveData>(File.ReadAllText(Path))
            : new SaveData();
}
```

File JSON đọc được bằng mắt → debug dễ gấp mười lần.

---

## 7. Assembly Definition (làm sau, milestone 6)

Khi code đã ổn định, thêm `.asmdef` cho `Gacha/` và `Data/`. Lợi ích:

- compile nhanh hơn nhiều (sửa UI không phải build lại gacha)
- Unity **chặn** bạn vô tình tham chiếu ngược tầng

Chưa cần ngay. Đừng làm ở milestone 1 — sẽ vướng.
