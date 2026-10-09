#!/usr/bin/env python3
"""Dua diem xoay ngang ve dung tam than nhan vat.

Trieu chung: cum ba con dung lech han sang mot ben o, tran ra ngoai mep.

Nguyen nhan: diem xoay ngang lay tu 'anchor_x' trong manifest cua bo 20model.
Con so do la diem hoa si danh dau, khong nhat thiet la tam khoi cua than —
do duoc lech toi 0.127 don vi tren machine_epic, 0.115 tren dead_common.
Trong o rong 1.19 thi 0.115 la mot phan muoi o, du de nhin ra.

Cach sua: do tam khoi hinh bong cua clip DUNG, roi dich diem xoay cua MOI
clip cua con do di cung mot luong.

Vi sao dich ca con chu khong chinh tung clip: chinh tung clip thi moi lan
doi animation nhan vat nhay ngang mot cai. Dich deu thi chuyen dong trong
clip giu nguyen, chi ca con dich sang.

    python3 tools/center_pivots.py          # chi do
    python3 tools/center_pivots.py --write  # ghi that
"""

import json
import re
import sys
from pathlib import Path

import numpy as np
from PIL import Image

ART = Path("Assets/_Project/Art/Units")
LEDGER = Path("tools/units_manifest.json")
PIVOT_RE = re.compile(r"spritePivot: \{x: ([-\d.]+), y: ([-\d.]+)\}")
PPU_RE = re.compile(r"spritePixelsToUnits: ([\d.]+)")

# Duoi muc nay thi khong dang dich — tranh sua lung tung cho 1030 file.
TOL = 0.02


def read(rel):
    im = Image.open(ART / rel).convert("RGBA")
    a = np.asarray(im)[..., 3] > 80
    meta = (ART / (rel + ".meta")).read_text()
    px, py = (float(v) for v in PIVOT_RE.search(meta).groups())
    ppu = float(PPU_RE.search(meta).group(1))
    return im, a, px, py, ppu, meta


def main():
    do_write = "--write" in sys.argv
    ledger = json.loads(LEDGER.read_text())
    moved = frames = 0

    for c in ledger:
        offs = []
        for rel in c["anims"]["idle"]["files"]:
            im, a, px, py, ppu, _ = read(rel)
            xs = np.where(a)[1]
            offs.append((xs.mean() - px * im.width) / ppu)
        dx = float(np.median(offs))
        if abs(dx) < TOL:
            continue

        moved += 1
        print(f"  {c['key']:20} than lech {dx:+.3f} don vi -> dich diem xoay")
        for an in c["anims"].values():
            for rel in an["files"]:
                frames += 1
                if not do_write:
                    continue
                im, _, px, py, ppu, meta = read(rel)
                # Doi sang TI LE khung cua chinh anh nay: moi khung mot co,
                # cung mot do dai the gioi ra ti le khac nhau.
                npx = px + dx * ppu / im.width
                (ART / (rel + ".meta")).write_text(
                    PIVOT_RE.sub(f"spritePivot: {{x: {npx:.5f}, y: {py:.5f}}}", meta))

    print(f"\n{moved} nhan vat, {frames} khung " +
          ("da dich." if do_write else "se dich. Them --write."))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
