#!/usr/bin/env python3
"""Cat tam 4 icon buff va CAN LAI cho chung bang nhau truoc khi nhap.

Tam AI tra ve dung dinh dang (RGBA that, 256 muc alpha, 0% magenta) nhung
co thi lech nhieu: rong 208-310px, cao 191-291px, dien tich 28k-50k.

Can bang bang DIEN TICH chu khong bang khung bao. Bon hinh khac han nhau
ve dang — trai tim beo ngang, thanh kiem dai doc — nen ep chung cung mot
khung bao la ep sai: kiem se thanh to beo, tim thanh nho xiu. Dien tich
muc thi khong phu thuoc dang, nen scale theo sqrt(dien tich) cho ra bon
icon "nang" bang nhau truoc mat nguoi nhin, du hinh khac nhau.

KHONG phong to: moi icon chi bi THU NHO ve muc nho nhat. Phong to la tu
bia ra diem anh khong co that, va o day khong can — chi can bang nhau.

Chay:  python3 tools/import_buff_icons.py            # do va in
       python3 tools/import_buff_icons.py --write
"""

import hashlib
import sys
from pathlib import Path

import numpy as np
from PIL import Image
from scipy import ndimage

sys.path.insert(0, str(Path(__file__).parent))
from import_ui_art import META   # noqa: E402

SRC = Path("artsrc/ui/buff_icons.png")
OUT = Path("Assets/_Project/Art/UI")

# Thu tu trai sang phai tren tam: tim, kiem, set, bia.
NAMES = ["icon_hp", "icon_dmg", "icon_speed", "icon_crit"]
PAD = 6          # le trong quanh khung chung, de icon khong cham mep
MIN_AREA = 2000  # bo cac dom le, giu dung 4 cum that


def clusters(alpha):
    """Tach cac cum lien thong, tra ve theo thu tu trai sang phai."""
    lab, n = ndimage.label(alpha > 0.04)
    out = []
    for i in range(1, n + 1):
        ys, xs = np.where(lab == i)
        if len(ys) < MIN_AREA:
            continue
        out.append((xs.min(), (lab == i), ys.min(), ys.max(), xs.min(), xs.max()))
    out.sort(key=lambda t: t[0])
    return out


def main():
    write = "--write" in sys.argv
    if not SRC.exists():
        print(f"Khong thay {SRC}")
        return 1

    im = Image.open(SRC).convert("RGBA")
    arr = np.asarray(im).astype(float)
    rgb, alpha = arr[..., :3], arr[..., 3] / 255.0
    print(f"anh {im.width}x{im.height}   {len(np.unique(arr[..., 3]))} muc alpha")

    cs = clusters(alpha)
    if len(cs) != 4:
        print(f"Mong doi 4 icon, thay {len(cs)} — dung lai.")
        return 1

    # Cat sat vien tung icon, giu nguyen alpha khu rang cua.
    cut = []
    for _, mask, y0, y1, x0, x1 in cs:
        a = (alpha * mask)[y0:y1 + 1, x0:x1 + 1]
        c = rgb[y0:y1 + 1, x0:x1 + 1]
        cut.append((c, a))

    # sqrt(dien tich muc) = thuoc do "do nang" khong phu thuoc dang hinh.
    weights = [np.sqrt(a.sum()) for _, a in cut]
    target = min(weights)
    print(f"\ndo nang truoc khi can (sqrt dien tich muc):")
    for n, w in zip(NAMES, weights):
        print(f"  {n:12s} {w:7.1f}   lech {w / target:5.2f}x")

    scaled = []
    for (c, a), w in zip(cut, weights):
        k = target / w          # luon <= 1, khong bao gio phong to
        h, wd = a.shape
        nh, nw = max(1, round(h * k)), max(1, round(wd * k))
        src = Image.fromarray(
            np.dstack([c, a * 255]).astype(np.uint8), "RGBA")
        scaled.append(np.asarray(
            src.resize((nw, nh), Image.LANCZOS)).astype(float))

    # Mot khung vuong chung cho ca bon: chung nam canh nhau tren mot thanh,
    # khung le nhau la lam chung so le khi RectTransform keo ve cung co.
    side = max(max(s.shape[:2]) for s in scaled) + PAD * 2
    print(f"\nkhung chung {side}x{side}px")

    OUT.mkdir(parents=True, exist_ok=True)
    for name, s in zip(NAMES, scaled):
        h, w = s.shape[:2]
        top, left = (side - h) // 2, (side - w) // 2
        canvas = np.zeros((side, side, 4))
        canvas[top:top + h, left:left + w] = s
        after = np.sqrt((canvas[..., 3] / 255.0).sum())
        print(f"  {name:12s} muc {w}x{h}   do nang {after:6.1f}")
        if not write:
            continue
        Image.fromarray(canvas.astype(np.uint8), "RGBA").save(
            OUT / f"{name}.png")
        guid = hashlib.md5(f"spinsquad/ui/{name}".encode()).hexdigest()
        (OUT / f"{name}.png.meta").write_text(META.format(
            guid=guid, ppu="100", bx=0, by=0, bz=0, bw=0))

    print("\n" + ("da ghi 4 file" if write else "chua ghi — chay lai voi --write"))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
