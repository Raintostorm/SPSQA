#!/usr/bin/env python3
"""Nhap art man doc vao Assets: hai tam nen + bo ba bang chi so.

Hai tam nen chep thang, chi them .meta. Bo bang thi phai CAT: mot tam
1400x900 chua ba the va mot khung bang, cat bang phep dan nhan cum chu
khong chia luoi — hoa si khong xep deu bao gio.

Khung bang dung 9 MANH (keo gian giu nguyen bon goc), nen phai do be rong
vien. Ba the thi KHONG keo gian — chung luon mot co — nen vien de 0.

    python3 tools/import_portrait.py --write
"""

import hashlib
import sys
from pathlib import Path

import numpy as np
from PIL import Image
from scipy import ndimage

sys.path.insert(0, str(Path(__file__).parent))
from import_ui_art import META                      # noqa: E402

BG_SRC = Path("artsrc/portrait")
BG_OUT = Path("Assets/_Project/Art/Backgrounds")
UI_SRC = Path("artsrc/ui/stat_panels.png")
UI_OUT = Path("Assets/_Project/Art/UI")

BG = {"bg_prep_portrait": "bg_prep_portrait.png",
      "bg_battle_portrait": "bg_battle_portrait.png"}
TABS = ("tab_damage", "tab_taken", "tab_healing")
FRAME = "stat_frame"


def write(im, path, ppu, pivot, border, do_write):
    px, py = pivot
    bx, by, bz, bw = border
    print(f"  {path.name:26} {im.width:5}x{im.height:<5} ppu {ppu:6.1f}  "
          f"pivot {px},{py}  vien {bx},{by},{bz},{bw}")
    if not do_write:
        return
    im.save(path)
    guid = hashlib.md5(f"spinsquad/portrait/{path.name}".encode()).hexdigest()
    meta = META.format(guid=guid, ppu=f"{ppu:.2f}", bx=bx, by=by, bz=bz, bw=bw)
    meta = meta.replace("spritePivot: {x: 0.5, y: 0.5}",
                        f"spritePivot: {{x: {px}, y: {py}}}")
    path.with_suffix(".png.meta").write_text(meta)


def border_of(im, inset_ratio=0.5):
    """Be rong vien 9 manh: quet tu mep vao toi khi gap RUOT phang va toi.

    Do tu chinh anh chu khong go tay: doi anh la so nay phai doi theo, ma
    go tay thi khong ai nho sua.
    """
    a = np.asarray(im.convert("RGB")).astype(float)
    H, W = a.shape[:2]
    RUN = 40          # ruot phai keo dai it nhat ngan nay moi tinh

    def first_run(vals):
        """Chi so dau tien ma TU DO TRO DI co RUN diem lien tiep deu la ruot.

        Khong dung "diem toi dau tien": khung go co net vien den, mot diem
        den o mep la du lam phep do tra ve 0. Da vap hai lan.
        """
        dark = vals < 90
        run = 0
        for i, v in enumerate(dark):
            run = run + 1 if v else 0
            if run >= RUN:
                return i - RUN + 1
        return 0

    row = a[H // 2].mean(axis=1)
    left = first_run(row)
    right = first_run(row[::-1])
    col = a[:, W // 2].mean(axis=1)
    topb = first_run(col)
    botb = first_run(col[::-1])
    return max(left, right), max(topb, botb)


def main():
    do_write = "--write" in sys.argv

    print("nen:")
    for name, f in BG.items():
        src = BG_SRC / f
        if not src.exists():
            print(f"  ! khong thay {src}")
            continue
        im = Image.open(src).convert("RGB")
        # PPU sao cho be NGANG anh = 5.5 don vi the gioi (be ngang khung nhin)
        write(im, BG_OUT / f"{name}.png", im.width / 5.5, (0.5, 0.5),
              (0, 0, 0, 0), do_write)

    if not UI_SRC.exists():
        print(f"! khong thay {UI_SRC}")
        return 1

    print("bang chi so:")
    a = np.asarray(Image.open(UI_SRC).convert("RGBA"))
    lab, n = ndimage.label(a[..., 3] > 128)
    objs = []
    for i in range(1, n + 1):
        ys, xs = np.where(lab == i)
        if len(ys) < 4000:
            continue
        objs.append((xs.min(), ys.min(), xs.max() + 1, ys.max() + 1))
    if len(objs) != 4:
        print(f"  ! tim thay {len(objs)} mon, can dung 4")
        return 1
    objs.sort(key=lambda b: (b[1] // 300, b[0]))
    full = Image.open(UI_SRC).convert("RGBA")

    # Ba the: ep ve CUNG mot khung. Chung thay nhau tai cho khi doi bang,
    # lech mot pixel la luc bam the bi nhay.
    tabs = objs[:3]
    tw = max(b[2] - b[0] for b in tabs)
    th = max(b[3] - b[1] for b in tabs)
    for name, b in zip(TABS, tabs):
        c = full.crop(b)
        box = Image.new("RGBA", (tw, th), (0, 0, 0, 0))
        box.alpha_composite(c, ((tw - c.width) // 2, (th - c.height) // 2))
        write(box, UI_OUT / f"{name}.png", 100.0, (0.5, 0.5), (0, 0, 0, 0), do_write)

    fr = full.crop(objs[3])
    bx, by = border_of(fr)
    write(fr, UI_OUT / f"{FRAME}.png", 100.0, (0.5, 0.5),
          (bx, by, bx, by), do_write)

    print("\nDa ghi." if do_write else "\nChua ghi gi. Them --write.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
