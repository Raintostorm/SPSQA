#!/usr/bin/env python3
"""Sua clip bi phong to: dat lai PPU cho dung ti le voi clip DUNG.

Trieu chung: bam Play thi con skeleton dang danh tu nhien to vot len, ca hai
phe. Do duoc 10 animation tren 20 nhan vat lech 29–71%.

Nguyen nhan: chieu cao tren man = so diem anh / PPU. Art dot sau ve TO HON
art goc, nhung hai bo nhap dot sau (import_fixposes_attack.py va
reimport_attack.py) lay thang PPU trong ban ke — PPU do suy ra tu clip DUNG.
Nhieu diem anh chia cho cung mot mau so thi con linh phinh ra.

Cach sua: nhan PPU len dung ti le. Moc do la CAN BAC HAI DIEN TICH hinh
bong, lay trung vi ca clip.

Vi sao khong dung chieu cao:
  - chieu cao TRUNG BINH: clip 'dead' co con nga ra, thap di that, bi bao
    nham la lech
  - chieu cao NHO NHAT: clip danh co khung chung goi that, cung bi bao nham
    (human_common/attack ra 0.81 trong khi no khong he sai)

Dien tich it doi theo tu the hon han: con nga ra thi thap nhung be ngang
tang, dien tich gan nhu giu nguyen. Do thu ca 85 animation thi phan tach
ro rang: clip LANH ra 0.93–0.99, clip HONG ra >= 1.20, khong con nao o giua.

Khong dung toi file PNG, chi ghi lai .meta. Khong mat chat luong.

    python3 tools/fix_clip_scale.py          # chi do
    python3 tools/fix_clip_scale.py --write  # ghi that
"""

import json
import re
import sys
from pathlib import Path

import numpy as np
from PIL import Image

ART = Path("Assets/_Project/Art/Units")
LEDGER = Path("tools/units_manifest.json")
PPU_RE = re.compile(r"spritePixelsToUnits: ([\d.]+)")

# Ngoai khoang nay thi coi la loi. Dat rong de khong dung vao dong tac that.
LO, HI = 0.90, 1.10


def scale_of(files):
    """Co cua clip: trung vi cua can bac hai dien tich hinh bong."""
    out = []
    for rel in files:
        a = np.asarray(Image.open(ART / rel).convert("RGBA"))[..., 3] > 80
        out.append(float(np.sqrt(a.sum())))
    return float(np.median(out)) if out else 0.0


def ppu_of(rel):
    return float(PPU_RE.search((ART / (rel + ".meta")).read_text()).group(1))


def main():
    do_write = "--write" in sys.argv
    ledger = json.loads(LEDGER.read_text())
    fixed = frames = 0

    for c in ledger:
        anims = c["anims"]
        if "idle" not in anims:
            continue
        base = scale_of(anims["idle"]["files"])
        base_ppu = ppu_of(anims["idle"]["files"][0])
        if base <= 0:
            continue

        for name, an in anims.items():
            if name == "idle":
                continue
            cur = scale_of(an["files"])
            if cur <= 0:
                continue
            ratio = cur / base
            old = ppu_of(an["files"][0])

            # Quyet dinh CO SUA HAY KHONG phai nhin co THAT TREN MAN
            # (diem anh / PPU), khong nhin ti le diem anh. Ti le diem anh
            # khong bao gio doi, nen lay no lam dieu kien thi script bao
            # "can sua" mai mai du da sua xong.
            now = (cur / old) / (base / base_ppu)
            if LO <= now <= HI:
                continue

            # Moc la PPU cua clip DUNG, khong phai PPU hien tai cua clip nay.
            # Co clip da mang san mot PPU khac (dot nhap don danh dat lai
            # rieng); nhan vao PPU do la nhan loi hai lan — da vap, sau clip
            # tot nghiem nho con 0.70 lan.
            #
            # Cong thuc nay khong phu thuoc gia tri cu, nen chay lai bao nhieu
            # lan cung ra cung mot ket qua.
            new = base_ppu * ratio
            print(f"  {c['key']:20}{name:8} co {ratio:.2f}x so voi clip dung   "
                  f"ppu {old:.0f} -> {new:.0f}")
            fixed += 1
            for rel in an["files"]:
                frames += 1
                if not do_write:
                    continue
                m = (ART / (rel + ".meta")).read_text()
                (ART / (rel + ".meta")).write_text(
                    PPU_RE.sub(f"spritePixelsToUnits: {new:.2f}", m))

    print(f"\n{fixed} animation, {frames} khung " +
          ("da sua." if do_write else "se sua. Them --write."))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
