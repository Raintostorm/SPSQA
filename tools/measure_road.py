#!/usr/bin/env python3
"""Do xem dai duong nam o dau trong mot anh nen.

Vi sao can: linh phai dung TREN mat duong. Uoc bang mat thi lech, va lech
thi nhin nhu linh lo lung tren co. Cong cu nay tra ve hai ti le, roi
BattleSceneSetup tu tinh ra laneY.

Tra ve TI LE chu khong phai pixel: anh nen co the thay bang anh khac kich
thuoc khac, ti le thi van dung.

    python3 tools/measure_road.py Assets/_Project/Art/Backgrounds/bg_field.png
"""

import sys
from pathlib import Path

import numpy as np
from PIL import Image


def measure(path, min_cover=0.80):
    """Hang nao co tren min_cover be ngang la mau dat thi tinh la mat duong."""
    a = np.asarray(Image.open(path).convert("RGB")).astype(int)
    r, g, b = a[..., 0], a[..., 1], a[..., 2]
    # Dat: do nhieu hon luc, luc nhieu hon lam. Co xanh truot vi luc vuot do.
    # Duong be tong/kim loai KHONG bat duoc bang phep nay — thoi Modern va
    # Future phai do bang cach khac, hoac ghi thang ti le vao.
    soil = (r > g + 20) & (g > b + 20)
    rows = np.where(soil.mean(axis=1) > min_cover)[0]
    if len(rows) == 0:
        return None
    h = a.shape[0]
    return rows[0] / h, (rows[-1] + 1) / h


def main():
    if len(sys.argv) < 2:
        print(__doc__)
        return 1
    for p in sys.argv[1:]:
        res = measure(p)
        if res is None:
            print(f"{Path(p).name}: khong thay dai dat nao")
            continue
        top, bot = res
        print(f"{Path(p).name}")
        print(f"  mep tren duong : {top:.4f}  (ti le tu TREN anh xuong)")
        print(f"  mep duoi duong : {bot:.4f}")
        print(f"  be day         : {bot - top:.4f} chieu cao anh")
    return 0


if __name__ == "__main__":
    sys.exit(main())
