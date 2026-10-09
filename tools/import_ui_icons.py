#!/usr/bin/env python3
"""Cat tam 3x3 icon do AI tra ve thanh 9 file rieng.

Dung lai phep tach nen cua import_ui_art.py: nen magenta dac, ria da khu
rang cua nen phai GIAI NGUOC phep tron chu khong xoa thang.

Khac dot truoc: day la anh VUONG, khong phai 9 manh — khong co spriteBorder,
va PPU de 100 vi be rong tren man do RectTransform quyet dinh.

Hai o vong quay (reel_slot va reel_slot_win) duoc dem ve CUNG MOT khung
vuong. Chung thay nhau tai cho khi o thang cum; lech mot pixel la luc sang
len no nhay.

Chay:  python3 tools/import_ui_icons.py            # do va in
       python3 tools/import_ui_icons.py --write
"""

import hashlib
import sys
from pathlib import Path

import numpy as np
from PIL import Image

sys.path.insert(0, str(Path(__file__).parent))
from import_ui_art import META, key_out, row_runs   # noqa: E402

SRC = Path("artsrc/ui/icons_sheet.png")
OUT = Path("Assets/_Project/Art/UI")

# Thu tu doc tu trai sang phai, tren xuong duoi.
NAMES = [
    "reel_slot", "reel_slot_win", "icon_coin",
    "icon_ally", "icon_buff", "icon_hp",
    "icon_dmg", "icon_speed", "icon_crit",
]
PAIR = ("reel_slot", "reel_slot_win")   # phai cung khung


def square(colour, alpha, side=None):
    """Cat sat vien roi dem ve khung VUONG, hinh nam chinh giua."""
    ys, xs = np.where(alpha > 0.5)
    y0, y1, x0, x1 = ys.min(), ys.max(), xs.min(), xs.max()
    C = colour[y0:y1 + 1, x0:x1 + 1]
    A = alpha[y0:y1 + 1, x0:x1 + 1]
    h, w = A.shape
    s = side or max(h, w)
    top, left = (s - h) // 2, (s - w) // 2
    oc = np.zeros((s, s, 3)); oa = np.zeros((s, s))
    oc[top:top + h, left:left + w] = C
    oa[top:top + h, left:left + w] = A
    return oc, oa


def main():
    write = "--write" in sys.argv
    if not SRC.exists():
        print(f"Khong thay {SRC}")
        return 1

    rgb = np.asarray(Image.open(SRC).convert("RGB")).astype(float)
    key = np.median(rgb[:8, :8].reshape(-1, 3), axis=0)
    colour, alpha = key_out(rgb, key)
    print(f"mau nen: {key.astype(int)}   anh {rgb.shape[1]}x{rgb.shape[0]}")

    rows = row_runs((alpha > 0.5).any(axis=1), min_h=40)
    if len(rows) != 3:
        print(f"Mong doi 3 hang, thay {len(rows)} — dung lai.")
        return 1

    cells = []
    for ri, (y0, y1) in enumerate(rows):
        band = alpha[y0:y1 + 1]
        cols = row_runs((band > 0.5).any(axis=0), min_h=40)
        if len(cols) != 3:
            print(f"Hang {ri}: mong doi 3 cot, thay {len(cols)} — dung lai.")
            return 1
        for x0, x1 in cols:
            cells.append((colour[y0:y1 + 1, x0:x1 + 1], band[:, x0:x1 + 1]))

    cut = {n: square(c, a) for n, (c, a) in zip(NAMES, cells)}

    # Hai o vong quay ve chung mot khung vuong, lay cai to hon.
    side = max(cut[PAIR[0]][1].shape[0], cut[PAIR[1]][1].shape[0])
    for n, (c, a) in zip(NAMES, cells):
        if n in PAIR:
            cut[n] = square(c, a, side)

    OUT.mkdir(parents=True, exist_ok=True)
    for n in NAMES:
        c, a = cut[n]
        mark = "  (cung khung voi doi con lai)" if n in PAIR else ""
        print(f"  {n:16s} {a.shape[1]}x{a.shape[0]}{mark}")
        if not write:
            continue
        Image.fromarray(np.dstack([c, a * 255]).astype(np.uint8), "RGBA").save(
            OUT / f"{n}.png")
        guid = hashlib.md5(f"spinsquad/ui/{n}".encode()).hexdigest()
        (OUT / f"{n}.png.meta").write_text(META.format(
            guid=guid, ppu="100", bx=0, by=0, bz=0, bw=0))

    print("\n" + ("da ghi 9 file" if write else "chua ghi — chay lai voi --write"))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
