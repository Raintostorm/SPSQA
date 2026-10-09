#!/usr/bin/env python3
"""Sua 120 khung bi dat pivot x = 1.0.

Trieu chung: linh nhay ngang gan tron mot o giua chung animation. Do duoc:
23 animation troi ngang qua 0.25 don vi, 20 trong so do troi hon 0.7 — ma o
luoi chi rong 0.92.

Nguyen nhan: nhung khung do co spritePivot x DUNG BANG 1.0000, tuc diem xoay
nam o MEP PHAI anh. Khung lanh lan cua cung animation co pivot 0.44–0.50.
1.0 chan la gia tri bi kep tran, khong phai so do duoc.

Cach sua: lay cac khung LANH trong chinh animation do lam moc. Do khoang
cach tu tam khoi toi diem xoay cua chung (don vi the gioi), roi dat lai pivot
cua khung hong sao cho khoang cach ay bang nhau. Khong dung mot con so chung
cho moi con — moi con mot the dung khac nhau.

    python3 tools/fix_pivots.py          # chi do
    python3 tools/fix_pivots.py --write  # ghi that
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

BROKEN = 0.99          # pivot x tu day tro len la bi kep tran


def read(rel):
    im = Image.open(ART / rel).convert("RGBA")
    a = np.asarray(im)[..., 3] > 80
    meta = (ART / (rel + ".meta")).read_text()
    px, py = (float(v) for v in PIVOT_RE.search(meta).groups())
    ppu = float(PPU_RE.search(meta).group(1))
    cx = float(np.where(a)[1].mean()) if a.any() else im.width / 2.0
    return dict(im=im, px=px, py=py, ppu=ppu, cx=cx, meta=meta)


def main():
    do_write = "--write" in sys.argv
    ledger = json.loads(LEDGER.read_text())
    fixed = total = 0

    for c in ledger:
        for name, anim in c["anims"].items():
            frames = [(f, read(f)) for f in anim["files"]]
            broken = [t for t in frames if t[1]["px"] >= BROKEN]
            if not broken:
                continue
            good = [t for t in frames if t[1]["px"] < BROKEN]
            if not good:
                print(f"  ! {c['key']}/{name}: KHONG con khung lanh nao de lam moc "
                      f"— phai nhap lai clip nay.")
                continue

            # Khoang cach tam khoi -> diem xoay o cac khung lanh, don vi the gioi.
            offs = [(g["cx"] - g["px"] * g["im"].width) / g["ppu"] for _, g in good]
            want = float(np.median(offs))
            spread = max(offs) - min(offs)

            for rel, f in broken:
                px = (f["cx"] - want * f["ppu"]) / f["im"].width
                total += 1
                if do_write:
                    new = PIVOT_RE.sub(
                        f"spritePivot: {{x: {px:.5f}, y: {f['py']:.5f}}}", f["meta"])
                    (ART / (rel + ".meta")).write_text(new)
                fixed += 1
            print(f"  {c['key']:20}{name:8}{len(broken)}/{len(frames)} khung  "
                  f"-> pivot {broken[0][1]['px']:.3f} thanh "
                  f"{(broken[0][1]['cx'] - want * broken[0][1]['ppu']) / broken[0][1]['im'].width:.3f}"
                  f"   (moc tu {len(good)} khung lanh, lech {spread:.3f})")

    print(f"\n{fixed} khung " + ("da sua." if do_write else "se sua. Them --write."))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
