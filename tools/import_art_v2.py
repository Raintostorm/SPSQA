#!/usr/bin/env python3
"""Nhap bo art v2: 6 nen phang + 12 manh UI phang.

Hai quyet dinh dang noi:

1. GHI DE DUNG TEN CU, va GIU NGUYEN GUID doc tu file .meta dang co.
   Doi ten moi thi moi tham chieu trong 4 scene phai gan lai bang tay.
   Giu guid thi Unity coi day la cung mot asset, chi la noi dung moi —
   bam Play la thay ngay, khong phai dung lai scene nao.
   KHONG tinh guid bang md5: hai nen cu duoc nhap boi tool khac nen
   guid khong theo cong thuc nao ca. Doc tu file la cach duy nhat dung.

2. Hai nut "bam xuong" duoc SINH TU nut thuong bang cach lam toi,
   khong lay tu tam AI. Do duoc: tam tra ve cho o so 3 va so 5 la hai
   nut KHAC MAU chu khong phai trang thai bam. Ma hai anh nay thay nhau
   tai cho luc bam — lech mot pixel la nut giat.

    python3 tools/import_art_v2.py            # do va in
    python3 tools/import_art_v2.py --write
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
UI = Path("Assets/_Project/Art/UI")
BG = Path("Assets/_Project/Art/Backgrounds")

BACKGROUNDS = {
    "bg_prep_v2": "bg_prep_portrait",
    "bg_battle_v2": "bg_battle_portrait",
    "bg_home_v2": "bg_home_portrait",
    "bg_treasure_v2": "bg_treasure_portrait",
    "bg_upgrade_v2": "bg_upgrade_portrait",
    "bg_shop_v2": "bg_shop_portrait",
}
BG_PPU = round(1125 / 5.5, 2)

# Thu tu doc trai->phai, tren->duoi. Hai o '_down' bi bo qua khi cat va
# sinh lai tu o thuong — xem ghi chu dau file.
UI_NAMES = ["pill_bar", "btn_primary", "btn_primary_down", "btn_blue",
            "btn_blue_down", "btn_disabled", "btn_small", "card",
            "slot_empty", "slot_owned", "hp_bg", "hp_fill"]
DERIVED = {"btn_primary_down": "btn_primary", "btn_blue_down": "btn_blue"}
DARKEN = 0.70

GUID_RE = re.compile(r"^guid: ([0-9a-f]{32})", re.M)


def keep_guid(meta_path, fallback_key):
    """Guid cu neu co, khong thi tao moi tu ten."""
    if meta_path.exists():
        m = GUID_RE.search(meta_path.read_text())
        if m:
            return m.group(1), True
    return hashlib.md5(f"spinsquad/ui/{fallback_key}".encode()).hexdigest(), False


def corner_radius(alpha):
    """Ban kinh goc bo tron, suy ra tu chieu rong tung hang.

    Hinh bo tron: hang tren cung hep, rong dan, roi thang. Cho no dat
    98% be rong lon nhat chinh la cho goc ket thuc.

    Can so nay de dat vien 9 manh: vien HEP hon ban kinh goc thi Unity
    keo gian chinh cai goc, va goc bien dang khi nut doi co.
    """
    w = (alpha > 128).sum(axis=1)
    full = w.max()
    for y, v in enumerate(w):
        if v >= full * 0.98:
            return y
    return 0


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


def save(arr, name, directory, ppu, border, write):
    path = directory / f"{name}.png"
    guid, kept = keep_guid(Path(str(path) + ".meta"), name)
    if write:
        directory.mkdir(parents=True, exist_ok=True)
        Image.fromarray(np.clip(arr, 0, 255).astype(np.uint8), "RGBA").save(path)
        Path(str(path) + ".meta").write_text(META.format(
            guid=guid, ppu=ppu, bx=border, by=border, bz=border, bw=border))
    return kept


def main():
    write = "--write" in sys.argv

    print("=== NEN (ghi de dung ten cu, giu guid) ===")
    for src, dst in BACKGROUNDS.items():
        p = SRC / f"{src}.png"
        if not p.exists():
            print(f"  {src:18s} THIEU")
            continue
        im = Image.open(p).convert("RGBA")
        bad = "" if (im.width, im.height) == (1125, 2436) else "  !!! SAI CO"
        kept = save(np.asarray(im).astype(float), dst, BG, BG_PPU, 0, write)
        print(f"  {src:18s} -> {dst:22s} {im.width}x{im.height}{bad}"
              f"  guid {'GIU' if kept else 'MOI'}")

    print("\n=== UI PHANG ===")
    pieces = split(SRC / "flat_ui_candidate.png", len(UI_NAMES))
    by_name = dict(zip(UI_NAMES, pieces))

    for name in UI_NAMES:
        if name in DERIVED:
            base = by_name[DERIVED[name]].copy()
            base[..., :3] *= DARKEN            # chi lam toi, hinh giu nguyen
            by_name[name] = base

    for name in UI_NAMES:
        a = by_name[name]
        r = corner_radius(a[..., 3])
        # Them 2px cho chac: vien den nam NGOAI cung goc, khong tinh vao
        # ban kinh nhung van khong duoc keo gian.
        border = min(r + 2, min(a.shape[:2]) // 2 - 1)
        kept = save(a, name, UI, 100, border, write)
        src = f"  (sinh tu {DERIVED[name]}, toi {DARKEN:.0%})" if name in DERIVED else ""
        print(f"  {name:18s} {a.shape[1]:4d}x{a.shape[0]:<4d} "
              f"goc {r:3d}px -> vien 9 manh {border:3d}px  guid {'GIU' if kept else 'MOI'}{src}")

    print("\n" + ("da ghi." if write else "chua ghi — chay lai voi --write"))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
