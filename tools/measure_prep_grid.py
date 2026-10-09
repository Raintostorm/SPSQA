#!/usr/bin/env python3
"""Do xem 16 o nam o dau trong anh nen chuan bi.

O duoc VE SAN trong anh, nen o logic phai bam theo o ve — lech la linh dung
chech, va khong co gi bao. Cong cu nay tra ve TI LE (khong phai pixel) de
BattleSceneSetup nhan voi kich thuoc anh da phong ra toa do the gioi.

Cach do: o ben trai XANH hon dat, o ben phai DO hon dat. Tach bang ti le
g/r chu khong bang nguong mau tuyet doi — nguong tuyet doi vo ngay khi anh
sang toi khac di.

    python3 tools/measure_prep_grid.py Assets/_Project/Art/Backgrounds/bg_prep.png
"""

import sys

import numpy as np
from PIL import Image


def profile(mask, axis):
    return mask.mean(axis=axis)


def fit(prof, n=4, fill=0.80):
    """Tim (mep dau, buoc) cho n o deu nhau.

    Diem = trung binh TRONG o tru trung binh NGOAI o. Toi da hoa hieu nay
    thi mo hinh vua om het o vua tranh khe — khong phai chon nguong nao.
    """
    L = len(prof)
    best = None
    for start in range(0, L - 40):
        for pitch in range(25, (L - start) // n + 1):
            inside = np.zeros(L, bool)
            w = pitch * fill
            for i in range(n):
                c = start + pitch * (i + 0.5)
                inside[int(c - w / 2):int(c + w / 2)] = True
            if not inside.any() or inside.all():
                continue
            sc = prof[inside].mean() - prof[~inside].mean()
            if best is None or sc > best[0]:
                best = (sc, start, pitch)
    return best


def main():
    path = sys.argv[1] if len(sys.argv) > 1 else "Assets/_Project/Art/Backgrounds/bg_prep.png"
    a = np.asarray(Image.open(path).convert("RGB")).astype(float)
    H, W, _ = a.shape
    r, g, b = a[..., 0], a[..., 1], a[..., 2]
    ratio = g / np.maximum(r, 1)
    on = (r > 120) & (r < 238) & (b < 135)

    right = on & (ratio < 0.715)
    ys = np.where(right.mean(axis=1) > right.mean(axis=1).max() * 0.3)[0]
    xs = np.where(right.mean(axis=0) > right.mean(axis=0).max() * 0.3)[0]

    _, x0, pw = fit(profile(right[ys[0]:ys[-1]], 0)[xs[0]:xs[-1] + 1]); x0 += xs[0]
    _, y0, ph = fit(profile(right[:, xs[0]:xs[-1]], 1)[ys[0]:ys[-1] + 1]); y0 += ys[0]

    # Luoi TRAI nhieu hon han (o xanh lan voi bui co), nen suy tu doi xung
    # qua tam anh roi doi chieu, khong do truc tiep.
    inner = x0 - W / 2
    print(f"anh {W}x{H}  ti le {W/H:.3f}")
    print(f"  luoi phai : mep trong {x0}px, buoc ngang {pw}px")
    print(f"  hang      : tam hang tren {y0 + ph/2:.1f}px, buoc doc {ph}px")
    print()
    print("Hang so cho BattleSceneSetup:")
    print("  (!) Cong cu nay de do anh AI THO, truoc khi chuan hoa.")
    print("      bg_prep.png hien tai KHONG co o nao ve san — 32 o la sprite roi")
    print("      dung lattice, nen do lai no chi de kiem — ra sai vai pixel la")
    print("      do phep do nay buoc 0.25px va nguong tho, khong phai anh sai.")
    print(f"  GridInnerFrac  = {inner:.0f}f / {W}f")
    print(f"  GridPitchXFrac = {pw}f / {W}f")
    print(f"  GridRow0Frac   = {y0 + ph/2:.1f}f / {H}f")
    print(f"  GridPitchYFrac = {ph}f / {H}f")


if __name__ == "__main__":
    main()
