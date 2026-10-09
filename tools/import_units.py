#!/usr/bin/env python3
"""Nhap 20model vao Unity: 350 khung hinh thanh sprite roi + mot ban ke.

KHONG sinh .anim, KHONG sinh AnimatorController. Khung hinh do mot component
tu chay (FrameUnitVisual), vi ba ly do:
  - khong con animation event, ma do la thu da gay loi im lang truoc day
    (clip khong ban su kien ket thuc -> linh dung chet cung, log sach tron)
  - doi nhip danh chi la nhan mot so, khong phai dung Animator.speed
  - bot 20 controller + 90 clip phai sinh va phai kiem

Ba quyet dinh ve kich thuoc:

1. KHONG PHONG TO khung nao. Manifest co 'normalization' de quy cac pose ve
   cung mot khong gian, nhung vai he so > 1 (human/common la 1.5616) — nhan
   thang la upsample, anh mo di ma khong duoc gi. Thay vao do chia cho he so
   LON NHAT cua chinh con do: ti le tuong doi giu nguyen, khung to nhat giu
   nguyen pixel goc, khong khung nao bi phong.

2. PPU tinh tu khung IDLE dau tien, mot gia tri cho ca con. Dat PPU theo
   tung khung thi moi chuyen dong len xuong bi chuan hoa mat.

3. TU DO ma sua, khong tin so trong manifest. dead/common co khung di bo ve
   o 2.43 lan khung dung ma 'normalization' van ghi 1.0 — nhap nguyen si thi
   buoc mot cai la phinh gap doi.

    python3 tools/import_units.py          # chi do
    python3 tools/import_units.py --write  # ghi that
"""

import hashlib
import json
import sys
from pathlib import Path

import numpy as np
from PIL import Image

sys.path.insert(0, str(Path(__file__).parent))
from read20 import load_all                       # noqa: E402
from import_ui_art import META                    # noqa: E402

SRC = "20model"
OUT = Path("Assets/_Project/Art/Units")
LEDGER = Path("tools/units_manifest.json")

# Chieu cao the gioi cua mot con bac COMMON, don vi Unity.
# O luoi cao 0.83 — linh cao hon o mot chut la dung, giong he cu.
COMMON_WORLD_H = 0.86
RANK_BASE_PX = 80.0          # 'target_idle_height_px' cua bac common

# Ngoai nguong nay thi coi la LOI DU LIEU va tu sua. Dat rong de khong dung
# vao dong tac that: lao nguoi luc danh do duoc toi 1.32, chung goi 1.26.
FIX_LO, FIX_HI = 0.67, 1.50
# 'dead' co lun xuong that (con linh nga), 'skill' co vuon nguoi — khong sua.
FIXABLE = ("walk", "attack")

_cache = {}


def sheet(path):
    p = str(path)
    if p not in _cache:
        _cache[p] = Image.open(p).convert("RGBA")
    return _cache[p]


def vis_height(im):
    a = np.asarray(im)[..., 3] > 80
    ys = np.where(a.any(axis=1))[0]
    return int(ys.max() - ys.min() + 1) if len(ys) else 0


def cut(f, k):
    """Cat mot khung roi thu nho theo k. Tra ve (anh, pivot_x, pivot_y)."""
    x, y, w, h = f["rect"]
    im = sheet(f["sheet"]).crop((x, y, x + w, y + h))
    if abs(k - 1.0) > 1e-6:
        im = im.resize((max(1, round(w * k)), max(1, round(h * k))), Image.LANCZOS)
    ax, ay = f["anchor"]
    ax = (ax - x) * k if ax is not None else im.width / 2.0
    ay = (ay - y) * k if ay is not None else im.height
    # Unity tinh pivot tu goc DUOI-TRAI; anchor_y do tu tren xuong.
    return im, ax / im.width, 1.0 - ay / im.height


def main():
    do_write = "--write" in sys.argv
    chars = load_all(SRC)
    ledger = []
    total = 0

    for c in chars:
        key = f"{c['race']}_{c['rank']}"
        # 1. he so chuan hoa lon nhat cua con nay -> moc "khong phong to"
        kmax = max(f["norm"] for a in c["anims"].values() for f in a["frames"])
        scale = {nm: 1.0 for nm in c["anims"]}

        # 2. do chieu cao hinh bong that de bat loi du lieu
        heights = {}
        for nm, a in c["anims"].items():
            hs = [vis_height(cut(f, f["norm"] / kmax)[0]) for f in a["frames"]]
            heights[nm] = sum(hs) / max(1, len(hs))
        base = heights.get("idle") or next(iter(heights.values()))
        for nm in FIXABLE:
            if nm not in heights or heights[nm] <= 0:
                continue
            r = heights[nm] / base
            if r < FIX_LO or r > FIX_HI:
                scale[nm] = 1.0 / r
                print(f"  ! {key}/{nm}: cao gap {r:.2f} lan khung dung mà manifest "
                      f"ghi normalization binh thuong — tu sua x{scale[nm]:.3f}")

        # 3. PPU tu khung idle dau, mot gia tri cho ca con
        world_h = COMMON_WORLD_H * (c["target_h"] / RANK_BASE_PX)
        idle0 = c["anims"]["idle"]["frames"][0]
        ppu = vis_height(cut(idle0, idle0["norm"] / kmax)[0]) / world_h

        entry = dict(key=key, race=c["race"], rank=c["rank"], name=c["name"],
                     combat=c["combat"], weapon=c["weapon"],
                     ppu=round(ppu, 2), world_h=round(world_h, 4), anims={})
        folder = OUT / key

        for nm, a in c["anims"].items():
            k0 = scale[nm] / kmax
            files = []
            for i, f in enumerate(a["frames"]):
                im, px, py = cut(f, f["norm"] * k0)
                rel = f"{key}/{nm}_{i + 1}.png"
                files.append(rel)
                total += 1
                if not do_write:
                    continue
                folder.mkdir(parents=True, exist_ok=True)
                im.save(OUT / rel)
                guid = hashlib.md5(f"spinsquad/unit/{rel}".encode()).hexdigest()
                meta = META.format(guid=guid, ppu=f"{ppu:.2f}", bx=0, by=0, bz=0, bw=0)
                meta = meta.replace("spritePivot: {x: 0.5, y: 0.5}",
                                    f"spritePivot: {{x: {px:.5f}, y: {py:.5f}}}")
                (OUT / (rel + ".meta")).write_text(meta)
            entry["anims"][nm] = dict(files=files,
                                      durations=[round(d, 4) for d in a["durations"]],
                                      sequence=a["sequence"], loop=a["loop"])
        ledger.append(entry)
        print(f"{key:20} ppu {ppu:7.1f}  cao {world_h:.3f}  "
              f"{sum(len(v['files']) for v in entry['anims'].values()):2} khung  "
              f"{c['combat']}")

    if do_write:
        LEDGER.write_text(json.dumps(ledger, ensure_ascii=False, indent=1))
        print(f"\nDa ghi {total} sprite vao {OUT}/ va ban ke {LEDGER}")
    else:
        print(f"\n{total} khung. Chua ghi gi — them --write.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
