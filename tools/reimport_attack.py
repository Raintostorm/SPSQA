#!/usr/bin/env python3
"""Nhap lai 8 pose don danh, sua ba loi cua lan nhap truoc.

1. ALPHA NHI PHAN. remove_magenta() cu chi dat alpha = 0 cho diem khop
   magenta, con lai giu 255. Do duoc: sprite ra chi co 2 MUC alpha, trong
   khi sprite tu anh nguon RGBA co 239 muc. Vien rang cua, va 691–2048 diem
   con nhiem hong vi diem ria la mau TRON giua magenta va nhan vat, khong
   khop nguong nen duoc giu nguyen.
   -> dung key_out() cua import_ui_art: tinh alpha mem theo khoang cach toi
      mau key, GIAI NGUOC phep tron o ria, roi lan mau vung dac ra phu ria.

2. DIEM XOAY TINH TU O GIA DINH. root_x cu = (i+0.5)*w/8, tuc gia su 8 pose
   chia deu be ngang. AI khong xep deu bao gio.
   -> lay TAM BAN CHAN: trung binh hoanh do cua 15% hang duoi cung. Do la
      diem cham dat, no dung yen qua ca 8 pose.

3. KEP root_x VAO [0,1]. Khi pose lech khoi o gia dinh, phep kep cho ra dung
   1.0 — diem xoay nhay ra mep phai anh. Chinh la loi da phai sua tay cho
   120 khung.
   -> bo phep kep. Tinh tu ban chan thi khong bao giờ ra ngoai khoang.

    python3 tools/reimport_attack.py          # chi do
    python3 tools/reimport_attack.py --write  # ghi that
"""

import hashlib
import json
import re
import sys
from pathlib import Path

import numpy as np
from PIL import Image
from scipy import ndimage

sys.path.insert(0, str(Path(__file__).parent))
from import_ui_art import defringe              # noqa: E402

SRC = Path("artsrc/fixposes")
ART = Path("Assets/_Project/Art/Units")
LEDGER = Path("tools/units_manifest.json")
KEY = np.array([255.0, 0.0, 255.0])
PAD = 8
FOOT_ROWS = 0.15        # bao nhieu phan duoi cung tinh la "ban chan"


def feather(alpha, sigma=0.7):
    """Lam mem vien cho mat na NHI PHAN.

    Khong phai phuc hoi — thong tin khu rang cua da mat o nguon, khong lay
    lai duoc. Day la vet va: lam mo nhe mat na roi giu nguyen duong bien
    alpha = 0.5, nen hinh bong khong phinh khong top, chi het bac thang.

    Chi chay khi nguon co <= 4 muc alpha. Nguon co ria that (anh 20model goc
    co 239 muc) thi KHONG dung toi — mo them la lam hong ria that.
    """
    from scipy import ndimage as nd
    return np.clip(nd.gaussian_filter(alpha.astype(np.float64), sigma), 0.0, 1.0)


def key_soft(rgb, key, hard=70.0, soft=190.0):
    """Tach nen magenta GIU LAI ria khu rang cua.

    Khong dung key_out() cua import_ui_art o day. Ham do co mot bo loc sac
    quet magenta sot: g < r-55 va g < b-55. Dung cho art UI, nhung voi anh
    nhan vat thi diem ria CHINH LA mau tron giua magenta va nhan vat — no
    khop dung dieu kien ay va bi xoa ve alpha 0.
    Do duoc: tam nguon co 0.4–1.6% diem tron, ma sprite ra chi con 2 muc
    alpha. Toan bo ria bi vut.

    O day bo loc sac chi dong vao diem GAN MAGENTA THUAN (g < 80 va ca do
    lan lam > 200), nen ria duoc giu.
    """
    d = np.linalg.norm(rgb - key, axis=2)
    a = np.clip((d - hard) / (soft - hard), 0.0, 1.0)

    r, g, b = rgb[..., 0], rgb[..., 1], rgb[..., 2]
    a[(r > 200) & (b > 200) & (g < 80)] = 0.0

    # Giai nguoc phep tron o ria: moi diem ria la (that*a + key*(1-a)).
    safe = np.maximum(a, 0.25)[..., None]
    un = (rgb - key * (1.0 - safe)) / safe
    out = np.where(a[..., None] > 0.004, un, rgb)
    return defringe(np.clip(out, 0, 255), a), a


def main():
    do_write = "--write" in sys.argv
    ledger = {r["key"]: r for r in json.loads(LEDGER.read_text())}
    sheets = sorted(SRC.glob("*_attack_v2.png"))
    if not sheets:
        print(f"Khong thay tam nao trong {SRC}")
        return 1

    for sheet in sheets:
        key = sheet.name.replace("_attack_v2.png", "")
        if key not in ledger:
            print(f"  ! {key}: khong co trong ban ke, bo qua")
            continue

        raw = Image.open(sheet)
        rgba = np.asarray(raw.convert("RGBA")).astype(float)

        # Cong cu sinh anh XUAT DUOC alpha that — co tam tra ve RGBA, co tam
        # tra ve RGB nen magenta. Dung alpha san co thi tot hon han: ria la
        # ria goc, khong phai ria dung lai tu phep tron.
        native = (rgba[..., 3] < 10).mean()
        if native > 0.20:
            alpha = rgba[..., 3] / 255.0
            # Van phai khu quang: co tam RGBA ma ria VAN con dinh magenta —
            # do duoc human_legendary co 16% diem vien ngả hong, va alpha chi
            # 2 muc, tuc no da bi cat cung san roi moi them kenh alpha.
            # defringe() chi lap mau o diem alpha CHUA DAC. Tam nay alpha
            # nhi phan nen diem hong dang dung o alpha 255, bi coi la mau
            # that. Phai danh dau chung la KHONG DANG TIN truoc da.
            rgb0 = rgba[..., :3]
            r0, g0, b0 = rgb0[..., 0], rgb0[..., 1], rgb0[..., 2]
            tainted = ((r0 > 110) & (b0 > 110) &
                       (g0 < r0 - 45) & (g0 < b0 - 25) & (alpha > 0.5))
            trust = np.where(tainted, 0.5, alpha)
            colour = defringe(rgb0, trust)
            lv = len(np.unique(rgba[..., 3]))
            note = ""
            if lv <= 4:
                alpha = feather(alpha)
                note = "  — ria bi cat cung o nguon, da lam mem lai"
            how = f"alpha san co ({native * 100:.0f}% trong suot, {lv} muc){note}"
        else:
            colour, alpha = key_soft(rgba[..., :3], KEY)
            how = "tach nen magenta"
        print(f"{key}: {how}")

        lab, n = ndimage.label(alpha > 0.5)
        parts = [i for i in range(1, n + 1) if (lab == i).sum() > 1000]
        if len(parts) != 8:
            print(f"  ! {key}: tim thay {len(parts)} cum, can dung 8 — "
                  f"co the hai pose dinh nhau. Bo qua.")
            continue
        parts.sort(key=lambda i: np.where(lab == i)[1].min())

        boxes = []
        for i in parts:
            ys, xs = np.where(lab == i)
            boxes.append((xs.min(), ys.min(), xs.max() + 1, ys.max() + 1, i))
        ground = int(np.median([b[3] for b in boxes]))
        ppu = float(ledger[key]["ppu"])

        for idx, (x0, y0, x1, y1, i) in enumerate(boxes):
            a = np.where(lab[y0:y1, x0:x1] == i, alpha[y0:y1, x0:x1], 0.0)
            c = colour[y0:y1, x0:x1]
            out = np.zeros((y1 - y0 + PAD * 2, x1 - x0 + PAD * 2, 4), np.uint8)
            out[PAD:PAD + (y1 - y0), PAD:PAD + (x1 - x0), :3] = c.clip(0, 255).astype(np.uint8)
            out[PAD:PAD + (y1 - y0), PAD:PAD + (x1 - x0), 3] = (a * 255).clip(0, 255).astype(np.uint8)

            # Tam ban chan: 15% hang duoi cung cua hinh bong.
            m = out[..., 3] > 128
            ys, xs = np.where(m)
            cut = ys.max() - (ys.max() - ys.min()) * FOOT_ROWS
            foot = xs[ys >= cut]
            px = float(foot.mean()) / out.shape[1]
            py = (y1 + PAD - ground) / out.shape[0]

            rel = f"{key}/attack_smooth_{idx + 1}.png"
            print(f"  {rel:42} {out.shape[1]:4}x{out.shape[0]:<4} "
                  f"pivot {px:.4f},{py:.4f}  alpha {len(np.unique(out[...,3])):3} muc")
            if not do_write:
                continue
            Image.fromarray(out, "RGBA").save(ART / rel)
            meta = (ART / (rel + ".meta")).read_text()
            meta = re.sub(r"spritePivot: \{x: [-\d.]+, y: [-\d.]+\}",
                          f"spritePivot: {{x: {px:.5f}, y: {py:.5f}}}", meta)
            (ART / (rel + ".meta")).write_text(meta)

    print("\nDa ghi." if do_write else "\nChua ghi gi. Them --write.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
