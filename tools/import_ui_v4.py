#!/usr/bin/env python3
"""Nhap bo UI doi 3: 12 hinh nut/khung + 4 hinh o va thanh.

Ba viec rieng cua bo nay:

1. GIU GUID doc tu file .meta dang co. Ten trung voi bo cu nghia la Unity
   coi day la cung mot asset, chi noi dung moi — khong phai dung lai scene.

2. EP CHUNG KHUNG cho cap slot_empty / slot_owned. Do duoc 300x295 va
   320x297 — lech 20px. Chung thay nhau tai cho khi nhan duoc mon do,
   lech la o nhay.

3. DO ban kinh goc roi dat vien 9 manh theo no. Vien hep hon ban kinh
   thi Unity keo gian chinh cai goc, va goc bien dang khi nut doi co.

    python3 tools/import_ui_v4.py            # do va in
    python3 tools/import_ui_v4.py --write
"""

import hashlib
import re
import sys
from pathlib import Path

import numpy as np
from PIL import Image
from scipy import ndimage

sys.path.insert(0, str(Path(__file__).parent))
from import_ui_art import META   # noqa: E402

SRC = Path("artsrc/ui")
OUT = Path("Assets/_Project/Art/UI")
GUID_RE = re.compile(r"^guid: ([0-9a-f]{32})", re.M)

SHEETS = [
    ("ui_v4_shapes", ["btn_primary", "btn_primary_down", "btn_blue",
                      "btn_blue_down", "btn_disabled", "btn_small", "pill_bar",
                      "card", "card_rare", "card_epic", "card_legend",
                      "grid_cell"]),
    ("ui_v4_extra_shapes", ["slot_empty", "slot_owned", "bar_resource",
                            "pill_small"]),
]

# Cap thay nhau tai cho -> phai chung mot khung.
PAIRS = [("slot_empty", "slot_owned"),
         ("btn_primary", "btn_primary_down"),
         ("btn_blue", "btn_blue_down")]


def split(path, want):
    im = Image.open(path).convert("RGBA")
    arr = np.asarray(im).astype(float)
    lab, n = ndimage.label(arr[..., 3] > 10)
    sz = ndimage.sum(np.ones_like(lab), lab, range(1, n + 1))
    parts = []
    for i in range(1, n + 1):
        if sz[i - 1] < 3000:
            continue
        ys, xs = np.where(lab == i)
        parts.append((ys.min(), ys.max(), xs.min(), xs.max(), i))
    if len(parts) != want:
        raise RuntimeError(f"{path.name}: {len(parts)} manh, mong doi {want}")

    rows = []
    for p in sorted(parts, key=lambda p: (p[0] + p[1]) * 0.5):
        for r in rows:
            y0 = min(x[0] for x in r)
            y1 = max(x[1] for x in r)
            if min(y1, p[1]) - max(y0, p[0]) > 0.5 * min(y1 - y0, p[1] - p[0]):
                r.append(p)
                break
        else:
            rows.append([p])

    out = []
    for r in rows:
        for y0, y1, x0, x1, i in sorted(r, key=lambda x: x[2]):
            sub = arr[y0:y1 + 1, x0:x1 + 1].copy()
            sub[lab[y0:y1 + 1, x0:x1 + 1] != i] = 0
            out.append(sub)
    return out


def corner_radius(alpha):
    """Ban kinh goc bo tron: hang dau tien dat 98% be rong lon nhat."""
    w = (alpha > 128).sum(axis=1)
    full = w.max()
    for y, v in enumerate(w):
        if v >= full * 0.98:
            return y
    return 0


def frame_pair(a, b):
    """Dem hai hinh vao cung mot khung, moi hinh nam giua."""
    h = max(a.shape[0], b.shape[0])
    w = max(a.shape[1], b.shape[1])
    out = []
    for p in (a, b):
        c = np.zeros((h, w, 4))
        oy, ox = (h - p.shape[0]) // 2, (w - p.shape[1]) // 2
        c[oy:oy + p.shape[0], ox:ox + p.shape[1]] = p
        out.append(c)
    return out


def main():
    write = "--write" in sys.argv
    pieces = {}
    for sheet, names in SHEETS:
        p = SRC / f"{sheet}.png"
        if not p.exists():
            print(f"{sheet}.png: THIEU")
            return 1
        for nm, arr in zip(names, split(p, len(names))):
            pieces[nm] = arr
        print(f"{sheet}.png -> {len(names)} manh")

    for a, b in PAIRS:
        if a in pieces and b in pieces and pieces[a].shape != pieces[b].shape:
            old = (pieces[a].shape[1], pieces[a].shape[0],
                   pieces[b].shape[1], pieces[b].shape[0])
            pieces[a], pieces[b] = frame_pair(pieces[a], pieces[b])
            print(f"  ep chung khung {a}/{b}: "
                  f"{old[0]}x{old[1]} & {old[2]}x{old[3]} -> "
                  f"{pieces[a].shape[1]}x{pieces[a].shape[0]}")

    print(f"\n{'ten':18s} {'co':>10s} {'goc':>5s} {'vien':>5s}  guid")
    OUT.mkdir(parents=True, exist_ok=True)
    for nm, arr in pieces.items():
        r = corner_radius(arr[..., 3])
        border = min(r + 2, min(arr.shape[:2]) // 2 - 1)
        path = OUT / f"{nm}.png"
        meta = Path(str(path) + ".meta")
        m = GUID_RE.search(meta.read_text()) if meta.exists() else None
        guid = m.group(1) if m else hashlib.md5(
            f"spinsquad/ui/{nm}".encode()).hexdigest()
        print(f"{nm:18s} {arr.shape[1]:4d}x{arr.shape[0]:<4d} {r:5d} {border:5d}"
              f"  {'GIU' if m else 'MOI'}")
        if not write:
            continue
        Image.fromarray(np.clip(arr, 0, 255).astype(np.uint8), "RGBA").save(path)
        meta.write_text(META.format(guid=guid, ppu=100,
                                    bx=border, by=border, bz=border, bw=border))

    print("\n" + ("da ghi." if write else "chua ghi — chay lai voi --write"))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
