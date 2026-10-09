#!/usr/bin/env python3
"""Tinh scale rieng cho tung nhan vat sao cho vua o luoi, ghi vao manifest.

Vi sao khong dung chung mot so: luc nhap, moi nhan vat duoc chuan hoa theo
CHIEU CAO (world_h = 0.86) nen be ngang tha noi hoan toan. Do ra, than
human_legendary rong 1.86 don vi con elf_common 0.62 — gap ba lan. Mot
UnitScale chung phuc vu duoc con nay thi hong con kia.

Do be ngang bang THAN (dai chua 90% luong muc), khong bang khung bao.
Da nhin tan mat 4 con co ti le bbox/than cao nhat: machine_rare chia nong
sung, elf_rare chia kiem, dead_rare toa lua hai ben, human_common chia luoi
hai. Phan chia ra deu MONG va deu that — ep ca no vao trong o la bat ca
nhan vat nho lai vi mot mui kiem, trong khi mui kiem vuot vien da trong
hoan toan binh thuong.

Chieu cao thi van do bang khung bao: tran len hang tren lo hon nhieu, va
SortingOrderFor ve con dung thap de len con dung cao nen cho chong len
trong rat sai.

Keo theo: can giua THAN chu khong can giua khung bao. Can giua khung bao
la day than lech sang mot ben de nhuong cho cho thanh kiem.

Chan 0.55 va tran 0.85: khong de con nao to hon hien tai, cung khong de
con nao nho toi muc mat chi tiet. Con nao cham chan 0.55 ma van tran thi
art phai ve lai — chinh so khong cuu duoc.

Chay:  python3 tools/fit_cell_scale.py            # do va in
       python3 tools/fit_cell_scale.py --write
"""

import glob
import json
import re
import sys
from pathlib import Path

import numpy as np
from PIL import Image

ROOT = Path(__file__).resolve().parents[1]
MANIFEST = ROOT / "tools/units_manifest.json"
UNITS = ROOT / "Assets/_Project/Art/Units"

# PHAI khop BattleSceneSetup.cs. Doi ben do ma quen ben nay thi linh tran
# lai, va khong co gi keu — nen co assert o cuoi file doi chieu.
DESIGN_W = 5.5          # LetterboxCamera.DesignWidth
PITCH_X = 286 / 1242    # BattleSceneSetup.GridPitchXFrac
PITCH_Y = 277 / 1242    # GridPitchYFrac quy ve cung mau so voi be ngang
SPREAD_X, SPREAD_Y = 0.12, 0.13
FLOOR, CEIL = 0.45, 0.85
PX_PER_UNIT = 2436 / 11.909     # chi de IN ra co tren man iPhone X

# Linh phai nam trong LONG o dat, khong phai trong buoc luoi.
# Hai lan thu hep giua hai con so do:
#   CellTiles.fill = 0.94  — o ve ra nho hon buoc luoi
#   vien da cua tile_ally  — an them 4.4% moi ben, do tren chinh file anh
# Bo qua hai cai nay la ngan sach rong hon that 21%, va day la dung loi
# toi mac lan truoc: 10 con bao cao "vua khit 1.00x" ma tren man van de len
# vien da.
TILE_FILL = 0.94        # CellTiles.fill
TILE_INNER = 0.912      # do tren Assets/_Project/Art/UI/tile_ally.png


PIVOT_RE = re.compile(r"spritePivot: \{x: ([-\d.]+)")


CORE = 0.05        # bo 5% luong muc moi dau -> con lai la THAN


def art_size(key, ppu):
    """Do mot nhan vat qua tam khung idle.

    Tra ve (than_rong, bbox_rong, cao, lech_x, lech_y) — don vi the gioi o
    scale 1, hai so lech tinh tu DIEM XOAY (= cho chan dat).

    "Than" = dai cot chua 90% luong muc, bo 5% moi dau. Bo theo LUONG MUC
    chu khong theo be ngang: mot mui kiem dai 80px nhung mong chi gop vai
    phan tram luong muc, nen no bi bo; mot canh tay dang rong thi khong.
    """
    core = w = 0.0
    cx = None
    heights = []
    for f in sorted(glob.glob(str(UNITS / key / "idle_*.png"))):
        im = Image.open(f)
        alpha = np.asarray(im.convert("RGBA"))[..., 3] / 255.0
        if alpha.sum() == 0:
            continue
        col = alpha.sum(axis=0)
        xs = np.where(col > 0)[0]
        ys = np.where((alpha > 0.08).any(axis=1))[0]

        cs = np.cumsum(col) / col.sum()
        lo, hi = np.searchsorted(cs, CORE), np.searchsorted(cs, 1.0 - CORE)
        fcore = (hi - lo + 1) / ppu
        if fcore > core:
            core = fcore
            px = float(PIVOT_RE.search(Path(f + ".meta").read_text()).group(1))
            cx = ((lo + hi + 1) * 0.5 - px * im.width) / ppu
        w = max(w, (xs.max() - xs.min() + 1) / ppu)

        heights.append((ys.max() - ys.min() + 1) / ppu)
    h = max(heights)
    # Can theo khung TRUNG VI, khong theo khung cao nhat.
    #
    # Cac khung idle chenh nhau toi 85px (human_common) — khung nhac chan,
    # khung gio vu khi. Can theo khung cao nhat thi moi khung thap hon deu
    # TUT XUONG dung nua phan chenh: do duoc trung binh 25.6px, nhieu nhat
    # 40.8px tren mot o cao 215px. Mat nhin ra ngay.
    #
    # Trung vi chu khong trung binh: mot khung gio kiem len troi keo trung
    # binh lech ca con, trung vi thi khong.
    #
    # 'h' van lay max — do la so dung de tinh VUA O, con 'cy' la so dung de
    # CAN GIUA. Hai cau hoi khac nhau, hai so khac nhau.
    cy = float(np.median(heights)) * 0.5
    return core, w, h, cx or 0.0, cy


def main():
    write = "--write" in sys.argv
    cell_w, cell_h = DESIGN_W * PITCH_X, DESIGN_W * PITCH_Y
    inner_w, inner_h = cell_w * TILE_FILL * TILE_INNER, cell_h * TILE_FILL * TILE_INNER
    # Tru san mot buoc xep cum: ba con trong mot o cach nhau dung mot buoc,
    # nen be ngang cum = be rong mot con + mot buoc.
    bud_w = inner_w - cell_w * SPREAD_X
    bud_h = inner_h - cell_h * SPREAD_Y
    print(f"buoc luoi {cell_w:.3f} x {cell_h:.3f}   "
          f"long o {inner_w:.3f} x {inner_h:.3f}")
    print(f"ngan sach mot con {bud_w:.3f} x {bud_h:.3f}\n")

    rows = json.loads(MANIFEST.read_text())
    still = []
    print(f"{'nhan vat':22s} {'than':>6s} {'bbox':>6s} {'cao':>6s} "
          f"{'scale':>6s} {'tren man':>7s}")
    for r in rows:
        core, w, h, cx, cy = art_size(r["key"], float(r["ppu"]))
        if core == 0:
            continue
        fit = min(bud_w / core, bud_h / h)
        scale = round(min(CEIL, max(FLOOR, fit)), 3)
        over = core * scale / bud_w
        note = f"  TRAN {over:.2f}x" if over > 1.02 else ""
        print(f"{r['key']:22s} {core:6.3f} {w:6.3f} {h:6.3f} "
              f"{scale:6.3f} {h * scale * PX_PER_UNIT:5.0f}px{note}")
        if over > 1.02:
            still.append((r["key"], over))
        r["cell_scale"] = scale
        # Day TAM KHUNG BAO ve tam o. Nhan san scale vi no cong thang vao
        # transform.position, ma localScale KHONG an vao vi tri cua chinh
        # transform do — chi an vao con cua no.
        r["cell_offset"] = {"x": round(-cx * scale, 4),
                            "y": round(-cy * scale, 4)}

    if still:
        print(f"\n{len(still)} con cham chan {FLOOR} ma van tran — can ve lai art:")
        for key, over in sorted(still, key=lambda t: -t[1]):
            print(f"  {key:22s} than gap {over:.2f} lan o")

    if write:
        MANIFEST.write_text(json.dumps(rows, ensure_ascii=False, indent=1) + "\n")
        print(f"\nda ghi cell_scale + cell_offset cho {len(rows)} nhan vat")
    else:
        print("\nchua ghi — chay lai voi --write")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
