#!/usr/bin/env python3
"""Bat mipmap cho moi sprite trong du an.

Vi sao can: art nhan vat cao 165px nhung ve ra man chi 37-98px, tuc thu
nho 1.7 den 4.5 lan. KHONG co mipmap thi moi pixel dau ra chi lay 4 texel
trong so hai chuc texel ma no dang dai dien — phan con lai bi vut di. Ket
qua la ria rang cua va nhap nhay khi vat the nhuc nhich.

Co mipmap thi Unity dung san ban thu nho da lay trung binh ca vung.

Ba khoa:
  enableMipMap            1  — sinh cac ban thu nho
  mipMapsPreserveCoverage 0  — TAT. No danh cho vat lieu alpha-test (cutout),
                               noi tung texel chi co dac hoac trong. Unity
                               nhan alpha len cho ti le texel vuot nguong 0.5
                               khop ban goc — voi sprite alpha-BLEND thi phep
                               nhan do chi lam phinh ria ra. Do thu tren art
                               nay thi he so chi 0.96, gan nhu vo hai, nhung
                               dat sai thi som muon cung can.
  filterMode              2  — Trilinear, hoa giua hai muc mipmap; de
                               Bilinear thi thay "nay" muc khi doi kich thuoc

KHONG dung toi guid. Doi guid la moi tham chieu trong scene va prefab chet
im lang — xem loi A2 trong Handoff/11.

    python3 tools/set_sprite_quality.py          # xem se doi gi
    python3 tools/set_sprite_quality.py --write
"""

import re
import sys
from pathlib import Path

ROOT = Path("Assets/_Project/Art")

KEYS = {
    "enableMipMap": "1",
    "mipMapsPreserveCoverage": "0",
    "filterMode": "2",
}


def patch(text):
    out, changed = text, []
    for key, want in KEYS.items():
        m = re.search(rf"^(\s*){key}: (.*)$", out, re.M)
        if m is None:
            continue
        if m.group(2).strip() == want:
            continue
        out = out[:m.start()] + f"{m.group(1)}{key}: {want}" + out[m.end():]
        changed.append(f"{key} {m.group(2).strip()}->{want}")
    return out, changed


def main():
    write = "--write" in sys.argv
    metas = sorted(ROOT.rglob("*.png.meta"))
    touched = 0
    for m in metas:
        text = m.read_text()
        if "TextureImporter" not in text:
            continue
        new, changed = patch(text)
        if not changed:
            continue
        touched += 1
        if write:
            m.write_text(new)
        print(f"  {m.relative_to(ROOT)}  {', '.join(changed)}")
    print(f"\n{touched}/{len(metas)} file {'da sua' if write else 'se sua'}"
          f"{'' if write else ' — chay lai voi --write'}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
