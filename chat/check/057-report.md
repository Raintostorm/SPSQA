# INPUT 057 — kết quả toàn lô
2026-11-05 · Codex · **chưa chạy Unity**. Không tuyên bố production-ready.

| Mục | Trạng thái | Bằng chứng / blocker |
|---|---|---|
| 1 Rương | XONG phần sửa/cắt và proof | Master front-view mới; body+lid RGBA diff=0. 057-chest-fit.png/json. Hinge GIF là phép chiếu xoay X 2D; không có mặt trong mới. |
| 2 Level-up | XONG phần sửa/cắt | 4 vòng trắng rỗng; tâm alpha=0; 057-levelup-v2.png và 057-levelup.gif. |
| 3 Burst | XONG phần căn | 6 frame512²; alpha>40 bbox midpoint lệch tâm tối đa0.5px; một scale0.9. 057-burst-aligned.png/json + GIF. |
| 4 Import | BỊ CHẶN — chờ user duyệt bộ ảnh | 26 PNG + metadata staging; 25 đích import sẵn. Chưa ghi runtime, chưa thay atlas. Đã backup PNG/meta atlas, candidate diff ngoài rect=0. |
| 5 D1 | XONG | 47 asset,131 candidate lines;20 asset+meta retired;25 KEEP và2 KEEP_TOOL; không sửa C#. |
| 6 Báo cáo | XONG | File này, manifest/check JSON/D1 evidence, OUTPUT057 trong chat và MESSAGES. |

## Bảng file PNG staging đã ghi
| Đường dẫn | Cỡ thật | Pivot |
|---|---|---|
| `artsrc/ui/anim-057/staged/Chest/chest-master.png` | 512×512 | [0.5, 0.5] |
| `artsrc/ui/anim-057/staged/Chest/chest-body.png` | 512×512 | [0.5, 0.5] |
| `artsrc/ui/anim-057/staged/Chest/chest-lid.png` | 512×512 | [0.5, 0.5390625] |
| `artsrc/ui/anim-057/staged/LevelUp/levelup-flash-0.png` | 256×256 | [0.5, 0.5] |
| `artsrc/ui/anim-057/staged/LevelUp/levelup-flash-1.png` | 256×256 | [0.5, 0.5] |
| `artsrc/ui/anim-057/staged/LevelUp/levelup-flash-2.png` | 256×256 | [0.5, 0.5] |
| `artsrc/ui/anim-057/staged/LevelUp/levelup-flash-3.png` | 256×256 | [0.5, 0.5] |
| `artsrc/ui/anim-057/staged/Summon/burst-0.png` | 512×512 | [0.5, 0.5] |
| `artsrc/ui/anim-057/staged/Summon/burst-1.png` | 512×512 | [0.5, 0.5] |
| `artsrc/ui/anim-057/staged/Summon/burst-2.png` | 512×512 | [0.5, 0.5] |
| `artsrc/ui/anim-057/staged/Summon/burst-3.png` | 512×512 | [0.5, 0.5] |
| `artsrc/ui/anim-057/staged/Summon/burst-4.png` | 512×512 | [0.5, 0.5] |
| `artsrc/ui/anim-057/staged/Summon/burst-5.png` | 512×512 | [0.5, 0.5] |
| `artsrc/ui/anim-057/staged/Summon/orb-core.png` | 512×512 | [0.5, 0.5] |
| `artsrc/ui/anim-057/staged/Summon/card-back.png` | 400×560 | [0.5, 0.5] |
| `artsrc/ui/anim-057/staged/Common/glow.png` | 512×512 | [0.5, 0.5] |
| `artsrc/ui/anim-057/staged/Common/swirl.png` | 512×512 | [0.5, 0.5] |
| `artsrc/ui/anim-057/staged/Common/chest-light.png` | 512×512 | [0.5, 0.5] |
| `artsrc/ui/anim-057/staged/Common/sparkle.png` | 512×512 | [0.5, 0.5] |
| `artsrc/ui/anim-057/staged/Common/rarity-ray.png` | 512×512 | [0.5, 0.5] |
| `artsrc/ui/anim-057/staged/LevelUp/levelup-arrow.png` | 256×256 | [0.5, 0.5] |
| `artsrc/ui/anim-057/staged/LevelUp/merge-swirl.png` | 512×512 | [0.5, 0.5] |
| `artsrc/ui/anim-057/staged/Chest/shard.png` | 96×96 | [0.5, 0.5] |
| `artsrc/ui/anim-057/staged/ButtonIcons/contribution.png` | 256×256 | [0.5, 0.5] |
| `artsrc/ui/anim-057/staged/Glyphs/sort.png` | 150×150 | [0.5, 0.5] |
| `artsrc/ui/anim-057/staged/ResultScreen/defeat-candidate.png` | 707×216 | [0.5, 0.5] |

Mỗi PNG staging có `.meta`; border0; GUID staging ổn định khi chạy lại. Chưa tạo Resources/Anim.

## Đích sẵn để import sau duyệt
| Đích | Cỡ/Rect |
|---|---|
| `Assets/_Project/Resources/Anim/Chest/chest-body.png` | [512, 512] |
| `Assets/_Project/Resources/Anim/Chest/chest-lid.png` | [512, 512] |
| `Assets/_Project/Resources/Anim/LevelUp/levelup-flash-0.png` | [256, 256] |
| `Assets/_Project/Resources/Anim/LevelUp/levelup-flash-1.png` | [256, 256] |
| `Assets/_Project/Resources/Anim/LevelUp/levelup-flash-2.png` | [256, 256] |
| `Assets/_Project/Resources/Anim/LevelUp/levelup-flash-3.png` | [256, 256] |
| `Assets/_Project/Resources/Anim/Summon/burst-0.png` | [512, 512] |
| `Assets/_Project/Resources/Anim/Summon/burst-1.png` | [512, 512] |
| `Assets/_Project/Resources/Anim/Summon/burst-2.png` | [512, 512] |
| `Assets/_Project/Resources/Anim/Summon/burst-3.png` | [512, 512] |
| `Assets/_Project/Resources/Anim/Summon/burst-4.png` | [512, 512] |
| `Assets/_Project/Resources/Anim/Summon/burst-5.png` | [512, 512] |
| `Assets/_Project/Resources/Anim/Summon/orb-core.png` | [512, 512] |
| `Assets/_Project/Resources/Anim/Summon/card-back.png` | [400, 560] |
| `Assets/_Project/Resources/Anim/Common/glow.png` | [512, 512] |
| `Assets/_Project/Resources/Anim/Common/swirl.png` | [512, 512] |
| `Assets/_Project/Resources/Anim/Common/chest-light.png` | [512, 512] |
| `Assets/_Project/Resources/Anim/Common/sparkle.png` | [512, 512] |
| `Assets/_Project/Resources/Anim/Common/rarity-ray.png` | [512, 512] |
| `Assets/_Project/Resources/Anim/LevelUp/levelup-arrow.png` | [256, 256] |
| `Assets/_Project/Resources/Anim/LevelUp/merge-swirl.png` | [512, 512] |
| `Assets/_Project/Resources/Anim/Chest/shard.png` | [96, 96] |
| `Assets/_Project/Resources/ButtonIcons/contribution.png` | [256, 256] |
| `Assets/_Project/Resources/Glyphs/sort.png` | [150, 150] |
| `Assets/_Project/Resources/ResultScreen/atlas.png` | [793, 669, 707, 216] |

## Nguồn mới / sửa
- `artsrc/ui/anim-057/source/chest-master-raw.png`:1536×1024 RGBA; builtin image_gen, tham chiếu Art/UI/chest_closed.png; tạo đúng một master, sau đó normalize một lần rồi mask/cắt.
- `artsrc/ui/anim-057/source/levelup-raw.png`:2172×724 RGBA; builtin image_gen; center cleanup alpha bằng script.
- Burst tái dùng051, glyph/ribbon tái dùng054; các lớp khác tái dùng056, không generate lại bộ đã đạt.

## Script
- `tools/build_art_057.py`: nguồn→staging/proof/metadata; không runtime.
- `tools/proof_anim_057.py`: alpha/size/white/animation evidence + contact sheet.
- `tools/import_art_057.py`: dry-run; apply chỉ cho target được user duyệt và ghi receipt thật; giữ meta có sẵn, sao lưu trước ghi đè; atlas hash guard.
- `tools/audit_retire_057.py`: quyết định phụ thuộc đã review + từng dòng ứng viên; mặc định dry-run, --apply dời file/meta. Không rerun legacy importer trừ khi muốn tái sinh output cũ.

## Backup / dời
- Atlas gốc và .meta: `artsrc/ui/retired-2026-11-05/057-backup/atlas.png[.meta]`; hash gốc trong057-atlas-proof.json.
- D1 moves có old/new/hash/GUID trong057-d1-verified.json; 20PNG/bytes +20meta đã kiểm giữ nguyên hash/GUID.
- SharedUI/gold,key giữ vì tools/build_home_walk_review.py:13 là input; Art/UI/btn_blue.png.meta không đụng.

## Các check
- Body+lid trùng RGBA master:0pixel khác, maxchannelerror0.
- Burst measured centers<=0.5px; canvas512²; cùng scale; frame cuối mờ nên dùng ngưỡng alpha40 chung.
- Flash center32×32 alpha0, RGB visible của VFX trắng255.
- Atlas candidate giữ toàn bộ pixel ngoài rect; runtime atlas nguyên vẹn.
- 173 nguồn C# compile sạch sau D1; không sửa .cs. **chưa chạy Unity**.
- Chưa kiểm user-review hoặc rendering Unity/device; import paths chưa được C# animation gọi. Không tự đánh dấu ANIM001–006 xong.

## Cần user duyệt một lần
Xem057-review-board.png và cácGIF. Sau duyệt mới apply25target (bao gồm atlas). Nếu chỉ duyệt một phần thì ghi rõ target; không tự duyệt phần còn lại.
