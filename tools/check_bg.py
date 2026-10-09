#!/usr/bin/env python3
"""Kiem anh nen truoc khi dua vao Assets.

Ba thu hau ky khong cuu duoc (xem artsrc/bg/PROMPT.md):
  1. sai ti le khung
  2. co net vien den -> linh chim vao nen
  3. chi tiet nam giua khung -> dai hanh dong bi roi

    python3 tools/check_bg.py artsrc/bg/bg_medieval.png
"""
import sys, os
import numpy as np
from PIL import Image

# do tu camera that: ortho 4.0, man 1920x1080
QUIET = (217, 270, 1702, 810)      # x0, y0, x1, y1 — vung phai yen tinh

def check(path):
    im = Image.open(path).convert('RGB')
    W, H = im.size
    a = np.asarray(im).astype(float)
    ok = True

    print(f'\n{os.path.basename(path)}  {W}x{H}')

    # --- 1. ti le ---
    ar = W / H
    good = abs(ar - 16/9) < 0.02
    print(f'  ti le        {ar:.3f}  (can 1.778)        {"OK" if good else "SAI"}')
    ok &= good

    # --- 2. net den trong vung yen tinh ---
    sx, sy = W / 1920, H / 1080
    x0, y0, x1, y1 = (int(QUIET[0]*sx), int(QUIET[1]*sy),
                      int(QUIET[2]*sx), int(QUIET[3]*sy))
    mid = a[y0:y1, x0:x1]
    dark = (mid.max(axis=2) < 50).mean() * 100
    good = dark < 2.0
    print(f'  pixel rat toi {dark:5.2f}%  (can < 2%)        {"OK" if good else "QUA NHIEU"}')
    ok &= good

    # --- 3. do tuong phan cuc bo trong vung yen tinh ---
    g = mid.mean(axis=2)
    # chenh lech giua o 16x16 lien ke — do "roi mat"
    bs = 16
    h2, w2 = g.shape[0]//bs*bs, g.shape[1]//bs*bs
    blocks = g[:h2, :w2].reshape(h2//bs, bs, w2//bs, bs).mean(axis=(1, 3))
    busy = (np.abs(np.diff(blocks, axis=1)).mean() +
            np.abs(np.diff(blocks, axis=0)).mean()) / 2
    good = busy < 6.0
    print(f'  do roi mat   {busy:5.2f}   (can < 6.0)        {"OK" if good else "QUA ROI"}')
    ok &= good

    # --- 4. nen co toi hon o giua khong (de linh noi len) ---
    centre = g[g.shape[0]//4:g.shape[0]*3//4, g.shape[1]//4:g.shape[1]*3//4].mean()
    whole  = a.mean()
    print(f'  sang giua/toan  {centre:.0f} / {whole:.0f}   '
          f'({"giua toi hon — tot" if centre < whole else "giua sang hon — linh se chim"})')

    print(f'  => {"DUNG DUOC" if ok else "NEN SINH LAI"}')
    return ok

if __name__ == '__main__':
    if len(sys.argv) < 2:
        print(__doc__); sys.exit(2)
    sys.exit(0 if all(check(p) for p in sys.argv[1:]) else 1)
