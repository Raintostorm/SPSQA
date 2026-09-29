# 04 — Pipeline nhân vật 2D

> Đây là phần bạn nói đang thiếu, và cũng là phần bản cũ chết nhiều lần nhất.
> File dài nhất trong bộ. Đọc hết trước khi vẽ/đặt hàng bất kỳ tấm art nào.

---

## 1. Chốt kỹ thuật: **Sprite từng frame**

Bản cũ thử 4 hướng, không hướng nào tới đích. Chốt lại:

| Hướng | Đánh giá | Quyết định |
|---|---|---|
| **Sprite từng frame** | đơn giản nhất, Unity hỗ trợ sẵn, đã chạy được ở bản cũ | ✅ **Dùng cái này** |
| Cutout 2D (Sprite Library + bone) | ít art hơn nhưng phải rig, khó với chibi | ❌ sau này tính |
| Spine | mạnh nhưng **tốn tiền bản quyền**, và bạn chưa cần | ❌ gỡ khỏi project |
| Mocap 3D → 2D | sai hướng nghệ thuật hoàn toàn | ❌ xoá |

**Lý do chọn sprite frame**: bạn đang học. Sprite frame cho bạn thấy kết quả
trong ngày đầu. Rig 2D cho bạn thấy kết quả sau hai tuần — và có thể là kết
quả sai.

---

## 2. Bộ animation tối thiểu mỗi nhân vật

| Clip | Số frame gợi ý | Loop | Yêu cầu |
|---|---|---|---|
| **Idle** | 1–6 | ✅ | Đọc được ở cỡ điện thoại. Có thể chỉ 1 frame + nhún nhẹ. |
| **Walk** | 8 | ✅ | Chân phải **thật sự đổi bước**, không chỉ nhún cả người |
| **Attack** | 8 | ❌ | Có anticipation → impact → recovery. Event hit **đúng 1 lần** |
| **Hit** | 2–3 | ❌ | Chớp/giật ngắn, **không được cắt callback của Attack** |
| **Dead** | 6 | ❌ | Đổ xuống pose nằm rõ, **giữ frame cuối** |
| **Skill** | 8+ | ❌ | Chỉ bắt buộc từ Epic trở lên. Common được phép dùng lại Attack |

### Mẹo từ bản cũ (đã kiểm chứng)

Con `HumanCommonClubFighter` chỉ có **6 bản vẽ Idle không liên quan nhau**,
ghép lại thì nhân vật bị "rung giật" như bị co giật. Giải pháp đã dùng:

> **Idle = 1 bản vẽ duy nhất + curve scale Y dao động 1,2%.**
> Kết quả mượt hơn hẳn 6 bản vẽ chắp vá.

Áp dụng luôn cho bản mới: Idle không cần nhiều frame. Cần **đúng**.

---

## 3. Chuẩn file art — làm sai chỗ này là hỏng cả pipeline

Đây là nguyên nhân gốc của mọi lỗi "lính lệch vị trí", "lính bay lơ lửng",
"đổi animation là nhảy chỗ" mà bản cũ vá đi vá lại bằng offset thủ công.

### Bảng chuẩn

| Thông số | Giá trị | Vì sao |
|---|---|---|
| Kích thước canvas | **320 × 320 px**, giống nhau **mọi frame** | frame khác cỡ → đổi clip là nhảy chỗ |
| Nền | **alpha thật** (PNG trong suốt) | không dùng nền trắng/xanh rồi key sau |
| Đệm mép | **≥ 16 px** trống mọi phía | tránh bị cắt khi vung vũ khí |
| Chiều cao nhân vật đứng | **180 px** (mốc tham chiếu) | mọi action scale theo mốc này |
| Pivot | **(0.5, 60/320)** = (0.5, 0.1875) | 60 px từ đáy = **mặt đất dưới chân** |
| Pixels Per Unit | **128** | 320px ÷ 128 = 2.5 world unit |
| Filter Mode | Bilinear | |
| Compression | Uncompressed (lúc dev) | đổi sang Compressed khi build |
| Mipmap | **tắt** | sprite 2D không cần |

### Quy tắc pivot — quan trọng nhất cả file

> **Pivot là điểm chân chạm đất. Mọi frame của mọi action dùng CÙNG một pivot.**

Nếu Attack vươn người cao hơn Idle → **không được** scale riêng frame Attack.
Bạn giữ nguyên tỉ lệ và để nhân vật vươn ra ngoài. Canvas 320×320 đủ chỗ.

Bản cũ từng scale riêng từng action theo bounding box → mỗi lần đổi clip là
nhân vật đổi kích cỡ. Mất rất nhiều thời gian mới tìm ra nguyên nhân.

### Đặt tên file

```
Art/Characters/HumanCommonClubber/
├── Idle/   Idle_00.png
├── Walk/   Walk_00.png … Walk_07.png
├── Attack/ Attack_00.png … Attack_07.png
├── Hit/    Hit_00.png … Hit_02.png
└── Dead/   Dead_00.png … Dead_05.png
```

Số **hai chữ số, có số 0 đứng đầu**. Vì code sắp xếp theo thứ tự chuỗi:
`Walk_10` sẽ đứng trước `Walk_2` nếu bạn đặt tên không có `0`.

---

## 4. Cấu trúc prefab — hai tầng, không được gộp

```
HumanCommonClubber          ← ROOT: Animator, UnitVisual, Unit, UnitBrain
│                              Transform này thuộc về GAMEPLAY.
│                              Vị trí trên lưới, hướng quay (scale.x âm).
│
└── Body                    ← CON: SpriteRenderer
                               Transform này thuộc về ANIMATION.
                               Mọi curve trong clip trỏ vào đây.
```

### Vì sao phải tách

Đây là bài học đắt nhất của bản cũ.

Nếu clip animation ghi đè trực tiếp lên transform của root, thì mỗi frame
Animator sẽ **ghi đè vị trí gameplay** của lính. Lính đang đi sẽ bị giật về
chỗ cũ. Bản cũ đã vá bằng đủ kiểu offset và flip chồng nhau, sinh ra lỗi
"nhiều lớp flip chồng lên nhau" ghi trong tài liệu bàn giao của chính nó.

Luật:

| Thuộc về | Ai được động vào |
|---|---|
| `root.position` | **chỉ** `UnitBrain` (di chuyển) |
| `root.localScale.x` | **chỉ** `SetFacing()` (một chỗ duy nhất trong cả project) |
| `Body.*` (sprite, scale, position) | **chỉ** AnimationClip |

> Chỉ **một** tầng chịu trách nhiệm lật hướng. Không bao giờ dùng đồng thời
> `SpriteRenderer.flipX` và `scale.x` âm.

---

## 5. Timing — nơi tạo ra "cảm giác đánh"

Đây là con số thật từ clip Attack đã chạy được, **frameRate = 60**:

```
t=0.00s  frame 0   ┐
t=0.10s  frame 1   ├─ ANTICIPATION (lấy đà, kéo gậy ra sau)
t=0.24s  frame 2   ┘
t=0.30s  frame 4   ←── ⚡ IMPACT — Animation Event "OnAttackEvent" ở ĐÂY
t=0.35s  frame 3   ┐
t=0.44s  frame 5   ├─ FOLLOW-THROUGH (gậy đi hết đà)
t=0.60s  frame 6   ┘
t=0.78s  frame 0   ←── 🏁 Animation Event "OnAnimationComplete", về Idle
```

### Ba điều rút ra

**1. Thứ tự frame không bắt buộc phải tăng dần.**
Thứ tự thật là `0,1,2,4,3,5,6,0`. Frame 4 được đẩy lên trước frame 3 để
**cú chạm ngang xảy ra trước khi gậy bổ xuống**. Đọc đúng hơn hẳn.

Bạn có toàn quyền sắp lại thứ tự frame trong clip. Đừng nghĩ phải theo số.

**2. Impact ở ~38% thời lượng clip** (0.30 / 0.78).
Anticipation dài, recovery dài, impact đến sớm. Đây là tỉ lệ dễ đọc.
Impact ở giữa hoặc cuối clip sẽ cho cảm giác "nặng nề, trễ".

**3. Clip kết thúc bằng frame 0** để nối liền về Idle, không giật.

### Cách đặt Animation Event

Trong Unity: mở clip trong cửa sổ Animation → thanh thời gian → chuột phải →
`Add Animation Event` → gõ tên hàm.

Hàm phải là `public void` **không tham số**, nằm trên component của chính
GameObject có `Animator`:

```csharp
public void OnAttackEvent()   { ... }   // trừ máu ở đây
public void OnAnimationComplete() { ... }  // về Idle ở đây
```

---

## 6. `SpriteUnitVisual.cs` — bản cài đặt đầy đủ

Đây là file của bản cũ đã chạy tốt, dọn lại cho bản mới. Copy được ngay.

```csharp
using System;
using UnityEngine;

namespace Project.Visual
{
    [RequireComponent(typeof(Animator))]
    public sealed class SpriteUnitVisual : MonoBehaviour, IUnitVisual
    {
        const string StateIdle   = "Idle";
        const string StateWalk   = "Walk";
        const string StateAttack = "Attack";
        const string StateHit    = "Hit";
        const string StateDead   = "Dead";

        Animator animator;
        string currentState;
        Action pendingImpact;
        Action pendingFinished;
        bool isDead;

        void Awake() => animator = GetComponent<Animator>();

        void Play(string state, bool restart = false)
        {
            if (isDead) return;                      // chết rồi thì không đổi nữa
            if (!restart && currentState == state) return;   // ⚠ then chốt, xem ghi chú
            currentState = state;
            animator.Play(state, 0, 0f);
        }

        public void PlayIdle()                 { ClearPending(); Play(StateIdle); }
        public void SetMoving(bool isMoving)   { ClearPending(); Play(isMoving ? StateWalk : StateIdle); }
        public void PlayHit()                  { Play(StateHit, true); }   // KHÔNG clear pending

        public void PlayAttack(Action onImpact, Action onFinished = null)
        {
            pendingImpact = onImpact;
            pendingFinished = onFinished;
            Play(StateAttack, restart: true);
        }

        public void PlayDeath()
        {
            ClearPending();
            isDead = true;
            currentState = StateDead;
            animator.Play(StateDead, 0, 0f);
        }

        public void SetFacing(float dirX)
        {
            if (Mathf.Abs(dirX) < 0.01f) return;
            var s = transform.localScale;
            s.x = Mathf.Abs(s.x) * (dirX < 0f ? -1f : 1f);
            transform.localScale = s;
        }

        // ── Animation Events gọi vào đây ──────────────────────────────
        public void OnAttackEvent()
        {
            var impact = pendingImpact;
            pendingImpact = null;          // nhả trước khi gọi → không thể gọi 2 lần
            impact?.Invoke();
        }

        public void OnAnimationComplete()
        {
            var finished = pendingFinished;
            pendingFinished = null;
            if (currentState == StateAttack) Play(StateIdle);
            finished?.Invoke();
        }

        void ClearPending()
        {
            pendingImpact = null;
            pendingFinished = null;
        }
    }
}
```

### Ba chi tiết dễ bỏ sót, cả ba đều là bug bản cũ từng dính

**a) `if (currentState == state) return;`**
Nếu thiếu dòng này, `SetMoving(true)` được gọi mỗi frame sẽ **restart clip Walk
mỗi frame** → nhân vật đứng yên tại chỗ dậm chân, không bao giờ qua frame 2.
Đây chính xác là lỗi "walk trông như không đi" của bản cũ.

**b) `pendingImpact = null` trước khi `Invoke()`**
Nếu `Invoke()` trước rồi mới gán null, và callback đó lỡ gọi lại `PlayAttack`,
bạn sẽ mất callback mới. Nhả trước, gọi sau.

**c) `PlayHit()` KHÔNG gọi `ClearPending()`**
Bị đánh giữa lúc đang vung gậy thì cú vung đó vẫn phải ra damage. Nếu Hit
xoá pending, lính bị đánh liên tục sẽ **không bao giờ gây damage được**.

---

## 7. Character Lab — scene bắt buộc

Bản cũ có cái này và nó là thứ **đúng đắn nhất** trong toàn project. Giữ lại.

Một scene riêng `CharacterLab.unity`, chỉ có:

- Camera Orthographic, `orthographicSize = 2.5`, nền màu nhạt dễ nhìn
- Một nhân vật ở giữa
- Panel nút: `Idle` `Walk` `Attack` `Hit` `Dead` `Pause` `0.25×` `1×`

Nút `0.25×` (đặt `Time.timeScale = 0.25f`) là thứ giúp bạn kiểm tra
Animation Event có rơi đúng frame impact không. Không có nó thì bạn chỉ
đoán mò.

### Cổng duyệt — 6 mục, không đạt thì không được đưa vào battle

```
□ 1. Mọi frame cùng canvas 320×320, alpha thật, đệm ≥16px
□ 2. Lab phát được từng state độc lập, Console 0 error
□ 3. Silhouette đọc được ở cỡ thật trên viewport dọc điện thoại
□ 4. Chạy 0.25×: Animation Event rơi đúng frame chạm
□ 5. Attack event phát ĐÚNG MỘT LẦN (đếm bằng Debug.Log)
□ 6. Dead giữ pose cuối, không quay về Idle
```

Chỉ khi đủ 6 dấu tick mới gán prefab vào `UnitDefinition`.

---

## 8. Nên dựng clip bằng tay hay bằng script

| | Bằng tay (Editor) | Bằng script |
|---|---|---|
| Nhân vật đầu tiên | ✅ **làm cách này** | ❌ |
| Từ nhân vật thứ 3 trở đi | tốn thời gian | ✅ chuyển sang script |

**Nhân vật đầu tiên phải làm bằng tay.** Bạn cần tự kéo thả sprite vào cửa sổ
Animation, tự đặt event, tự thấy nó sai và tự sửa. Đó là lúc bạn học.

Từ con thứ 3, khi bạn đã hiểu rõ mọi bước, **nhờ AI viết Editor script**
làm tự động — đó đúng là việc lặp lại. Bản cũ có sẵn một cái chạy tốt:
`HumanCommonClubFighterBuilder.cs` (211 dòng, menu `SpinSquad/Characters/Build...`).
Nó tự động: import sprite đúng chuẩn → tạo 5 clip → tạo Animator Controller
→ tạo prefab 2 tầng → dựng scene Lab. Đưa file đó cho AI làm mẫu.

---

## 9. Thứ tự làm nhân vật đầu tiên

```
1. Chuẩn bị art          → 320×320, alpha, cùng baseline 180px
2. Import vào Unity      → Sprite, PPU 128, pivot custom (0.5, 0.1875)
3. Tạo prefab 2 tầng     → root (Animator) + con "Body" (SpriteRenderer)
4. Tạo 5 clip            → kéo sprite vào cửa sổ Animation
5. Đặt Animation Event   → OnAttackEvent, OnAnimationComplete
6. Tạo Animator Controller → 5 state rời, KHÔNG cần transition (dùng Play())
7. Gắn SpriteUnitVisual  → vào root
8. Mở Character Lab      → duyệt qua 6 cổng ở mục 7
9. Gán vào UnitDefinition → giờ mới được vào battle
```

### Ghi chú về Animator Controller

**Không cần vẽ transition giữa các state.** Vì code gọi `animator.Play(state)`
trực tiếp. 5 state rời rạc, `Idle` đặt làm default. Đơn giản hơn nhiều so với
mạng lưới parameter/transition, và dễ debug hơn hẳn.

---

## 10. Art lấy từ đâu

Bạn đang thiếu art, không thiếu code. Vài hướng thực tế:

| Cách | Ưu | Nhược |
|---|---|---|
| Mua asset pack có sẵn animation | nhanh nhất, chuẩn nhất | tốn tiền, khó đồng bộ style cả roster |
| Thuê artist | đúng ý | đắt, chậm |
| AI sinh ảnh (`Tools/LocalAssetAI` bạn đã có ComfyUI 7,5GB) | rẻ, nhanh | **rất khó giữ nhất quán giữa các frame** |
| Tự vẽ | học được nhiều | chậm |

### Nếu dùng AI sinh ảnh — đọc kỹ

Bản cũ đã đi đường này và tài liệu review của chính nó kết luận:

> *"Các bản vẽ nguồn vẫn có sai khác silhouette, và hai nửa của Walk trông
> quá giống nhau. Trích xuất và timing đúng cũng không thể bịa ra những bản
> vẽ chân đối diện còn thiếu."*

Nghĩa là: AI sinh **từng frame rời** thì mỗi frame là một nhân vật hơi khác
nhau → animation rung. Cách dùng AI hiệu quả hơn:

- Dùng AI sinh **một pose chuẩn duy nhất** (T-pose / đứng thẳng)
- Rồi **cắt rời các bộ phận** (đầu, thân, tay, chân) và tự xoay/dịch chúng
- Hoặc dùng AI làm concept, còn frame animation thì vẽ tay từ concept đó

Đừng kỳ vọng AI sinh thẳng ra 8 frame walk cycle dùng được.

---

## 11. Lộ trình mở rộng roster

Đừng làm cả 5 chủng tộc × 4 rarity = 20 nhân vật ngay.

```
Milestone A:  1 ally melee Common          ← chạy hết cổng duyệt
Milestone B:  1 enemy melee                ← đủ để có trận đấu thật
Milestone C:  1 ally ranged                ← test nhánh combat thứ 2
Milestone D:  tô màu / đổi trang bị cho 4 rarity của cùng 1 dòng
Milestone E:  mới mở sang chủng tộc thứ 2
```

**Mẹo tiết kiệm lớn**: 4 rarity của cùng một dòng lính **dùng chung bộ
animation**, chỉ khác màu sắc và trang bị. Bản cũ đã định hướng vậy
(phân biệt bằng "silhouette, kích thước, trang bị, mật độ chi tiết").
Bạn chỉ cần vẽ lại lớp màu, không cần vẽ lại 28 frame.

---

## 12. Bảng lỗi thường gặp

| Triệu chứng | Nguyên nhân gần như chắc chắn |
|---|---|
| Đổi animation là nhân vật nhảy vị trí | frame khác canvas, hoặc pivot khác nhau |
| Walk dậm chân tại chỗ, không qua frame 2 | thiếu `if (currentState == state) return;` |
| Nhân vật giật về chỗ cũ khi đang đi | clip ghi đè transform **root** thay vì con `Body` |
| Đánh nhưng không trừ máu | Animation Event sai tên hàm, hoặc component không nằm cùng GameObject với Animator |
| Trừ máu 2 lần mỗi cú | event bị đặt 2 lần trong clip, hoặc `Invoke` trước khi nhả `pending` |
| Lính quay ngược hướng | hai tầng flip chồng nhau (`flipX` + `scale.x`) |
| Chết rồi đứng dậy | clip Dead bật `loopTime`, hoặc thiếu cờ `isDead` |
| Nhân vật lơ lửng trên mặt đất | pivot không ở đáy chân |
| Vũ khí bị cắt cụt khi vung | đệm mép < 16px |
