#!/usr/bin/env python3
"""Nhap toan bo art cho cac trang Home / Treasure / Upgrade / Shop.

Mot tool cho 14 tam, vi chung chia chung ba phep xu ly:

  1. TACH bang gan nhan lien thong, roi sap HANG truoc COT sau.
     Khong cat theo luoi cung — luoi cung la thu da got cut duoi xe tang
     o dot art truoc.

  2. CAN DO NANG bang sqrt(dien tich muc). Khung bao khong dung duoc:
     thanh kiem chia ra lam khung to len ma hinh khong nang hon. Can xong
     thi cac icon nhin ra bang nhau du hinh khac nhau.

  3. EP CHUNG KHUNG cho nhung bo thay nhau TAI CHO (nut thuong/nut bam,
     ruong dong/ruong mo). Lech mot pixel la luc doi no nhay.

Dung lai va bao neu so cum tach duoc khac so mong doi — KHONG nhap bua
roi lech chi so. "Coc go sut" mang chi so cua "Day chuyen xuong" la loi
khong ai nhin man hinh ma thay duoc.

    python3 tools/import_pages_art.py            # do va in
    python3 tools/import_pages_art.py --write
"""

import hashlib
import sys
from pathlib import Path

import numpy as np
from PIL import Image
from scipy import ndimage

sys.path.insert(0, str(Path(__file__).parent))
from import_ui_art import META   # noqa: E402

SRC = Path("artsrc/ui")
OUT = Path("Assets/_Project/Art/UI")
BG_OUT = Path("Assets/_Project/Art/Backgrounds")

PAD = 6          # le trong quanh khung chung
MIN_AREA = 2500  # bo dom le

# Nen: anh dac, chep thang. PPU cho be ngang 1125px phu dung DesignWidth 5.5
BG_PPU = round(1125 / 5.5, 2)
BACKGROUNDS = ["bg_home_portrait", "bg_treasure_portrait",
               "bg_upgrade_portrait", "bg_shop_portrait"]

# (file, [ten tung manh], thay-nhau-tai-cho, khung-vuong)
SHEETS = [
    ("resources", ["icon_gold", "icon_key", "chest_closed", "chest_open"], False, True),
    ("button_treasure", ["btn_treasure", "btn_treasure_down"], True, True),
    # Nut quay la CHU NHAT — khung vuong lam no co mat mot nua.
    ("buttons_roll", ["btn_roll1", "btn_roll1_down", "btn_roll10",
                      "btn_roll10_down", "btn_roll100", "btn_roll100_down"], True, False),
    ("item_slots", ["slot_locked", "slot_common", "slot_rare",
                    "slot_epic", "slot_legendary"], True, True),
    ("items_common", [f"item_c{i}" for i in range(1, 13)], False, True),
    ("items_rare", [f"item_r{i}" for i in range(1, 9)], False, True),
    ("items_epic", [f"item_e{i}" for i in range(1, 7)], False, True),
    ("items_legendary", [f"item_l{i}" for i in range(1, 5)], False, True),
]


def row_major(parts):
    """Sap HANG truoc COT sau, gom hang theo DO CHONG LAN DOC.

    Ban truoc gom bang round(y_dau / (cao_trung_vi * 0.7)). Sai: bon icon
    trong resources.png nam cung mot hang nhung cao thap khac nhau
    (y_dau 132, 145, 160, 226), phep lam tron nem chung vao HAI hang gia —
    thu tu ra 'key, chest_open, pouch, chest_closed' thay vi
    'pouch, key, chest_closed, chest_open'.

    Hau qua: icon vang hien ra cai chia khoa. Va no sai IM LANG — so manh
    van dung, ten van du, chi la gan nham hinh.

    Cach dung: hai hinh cung hang khi khoang doc cua chung CHONG LEN NHAU
    qua nua be cao cua hinh thap hon. Khong phu thuoc chung cao bao nhieu.
    """
    out = []
    for p in sorted(parts, key=lambda p: (p[0] + p[1]) * 0.5):
        placed = False
        for row in out:
            y0 = min(r[0] for r in row)
            y1 = max(r[1] for r in row)
            overlap = min(y1, p[1]) - max(y0, p[0])
            if overlap > 0.5 * min(y1 - y0, p[1] - p[0]):
                row.append(p)
                placed = True
                break
        if not placed:
            out.append([p])
    flat = []
    for row in out:
        flat.extend(sorted(row, key=lambda r: r[2]))
    return flat


def split(path, want):
    """Tach thanh cac manh roi, sap hang truoc cot sau."""
    im = Image.open(path).convert("RGBA")
    arr = np.asarray(im).astype(float)
    lab, n = ndimage.label(arr[..., 3] > 10)
    parts = []
    for i in range(1, n + 1):
        ys, xs = np.where(lab == i)
        if len(ys) < MIN_AREA:
            continue
        parts.append((ys.min(), ys.max(), xs.min(), xs.max(), i))
    if len(parts) != want:
        raise RuntimeError(
            f"{path.name}: tach ra {len(parts)} manh, mong doi {want}. "
            f"Dung lai — nhap tiep la lech ten voi hinh.")
    parts = row_major(parts)
    out = []
    for y0, y1, x0, x1, i in parts:
        sub = arr[y0:y1 + 1, x0:x1 + 1].copy()
        sub[lab[y0:y1 + 1, x0:x1 + 1] != i] = 0      # bo hinh ben canh lan vao
        out.append(sub)
    return out


def weight(piece):
    """sqrt(dien tich muc) — do nang khong phu thuoc dang hinh."""
    return float(np.sqrt((piece[..., 3] / 255.0).sum()))


def balance(pieces):
    """Thu nho cho moi manh nang bang manh nhe nhat. KHONG phong to."""
    ws = [weight(p) for p in pieces]
    target = min(ws)
    out = []
    for p, w in zip(pieces, ws):
        k = target / w
        h, wd = p.shape[:2]
        img = Image.fromarray(p.astype(np.uint8), "RGBA")
        out.append(np.asarray(img.resize(
            (max(1, round(wd * k)), max(1, round(h * k))), Image.LANCZOS)).astype(float))
    return out, ws, target


def frame(pieces, square=True):
    """Dem moi manh vao MOT khung chung, hinh nam giua.

    square=True  -> khung vuong. Dung cho icon se nam trong o vuong.
    square=False -> khung bang khung bao lon nhat cua ca nhom, giu nguyen
                    ti le. Dung cho nut chu nhat: ep nut 322x178 vao khung
                    vuong 334x334 la them 156px dem dọc, gan nua tam la
                    khoang trong, va vao RectTransform thi nut co lai con
                    mot nua.
    """
    hs = [p.shape[0] for p in pieces]
    ws = [p.shape[1] for p in pieces]
    if square:
        fh = fw = max(max(hs), max(ws)) + PAD * 2
    else:
        fh, fw = max(hs) + PAD * 2, max(ws) + PAD * 2
    out = []
    for p in pieces:
        h, w = p.shape[:2]
        c = np.zeros((fh, fw, 4))
        c[(fh - h) // 2:(fh - h) // 2 + h, (fw - w) // 2:(fw - w) // 2 + w] = p
        out.append(c)
    return out, (fw, fh)


def mend_seam(path, blend=40):
    """Keo hang DAU va hang CUOI ve dung mot gia tri, roi tan dan vao trong.

    Do duoc: hai mep trai/phai lech 32 va 88 tren 255, phan giua thi khop.
    Xep chong hai ban la thay vach gay o ria.

    Cach lam: lay trung binh hang dau va hang cuoi, ep CA HAI ve do. Sai
    lech vua ep vao duoc tan deu qua {blend} hang ke tiep nen khong tao
    bac. Sau phep nay hang dau va hang cuoi BANG NHAU dung bang 0.

    (Ban truoc toi tron hang dau voi hang thu {blend} tu duoi len — sai
    huong hoan toan, do ra 19.9 thanh 25.6, TE HON luc chua va.)
    """
    a = np.asarray(Image.open(path).convert("RGBA")).astype(float)
    h = a.shape[0]
    mask = (a[0][..., 3] > 20) | (a[-1][..., 3] > 20)
    before = float(np.abs(a[0] - a[-1])[mask].mean())

    out = a.copy()
    avg = (a[0] + a[-1]) * 0.5
    d_top, d_bot = avg - a[0], avg - a[-1]
    for i in range(blend):
        w = 1.0 - i / blend          # 1 o ngay mep, 0 o cuoi dai tan
        out[i] += d_top * w
        out[h - 1 - i] += d_bot * w

    after = float(np.abs(out[0] - out[-1])[mask].mean())
    return np.clip(out, 0, 255), before, after


def save(arr, name, directory, ppu, write):
    if not write:
        return
    directory.mkdir(parents=True, exist_ok=True)
    Image.fromarray(np.clip(arr, 0, 255).astype(np.uint8), "RGBA").save(
        directory / f"{name}.png")
    guid = hashlib.md5(f"spinsquad/ui/{name}".encode()).hexdigest()
    (directory / f"{name}.png.meta").write_text(
        META.format(guid=guid, ppu=ppu, bx=0, by=0, bz=0, bw=0))


def import_bar(write):
    """Thanh 9 manh: cat sat vien, thu nho, va DO viền thay vì đoán.

    Viền 9 mảnh phải tính bằng PIXEL CỦA SPRITE. Đoán sai thì hai nắp kéo
    giãn theo (viền quá hẹp) hoặc chồng lên nhau (viền quá rộng) — cả hai
    đều chỉ lộ ra khi nhìn trên máy, không lộ lúc biên dịch.

    Cách đo: cột nào GIỐNG cột giữa thì thuộc phần ruột kéo giãn được;
    phần còn lại hai đầu là nắp. Đo ra 362px trái, 366px phải trên 1976.

    Thu nhỏ về 600px vì ô dùng nó chỉ rộng 430: để nguyên 1976 thì hai nắp
    362+366 = 728 > 430, bốn góc chồng lên nhau.
    """
    src = SRC / "bar_resource.png"
    if not src.exists():
        print("  bar_resource.png: THIEU — bo qua")
        return
    a = np.asarray(Image.open(src).convert("RGBA")).astype(float)
    ys, xs = np.where(a[..., 3] > 20)
    c = a[ys.min():ys.max() + 1, xs.min():xs.max() + 1]
    h, w = c.shape[:2]

    mid = c[:, w // 2]
    d = np.abs(c - mid[:, None, :]).mean(axis=(0, 2))
    left = next(x for x in range(w) if d[x] < 6.0)
    right = next(x for x in range(w - 1, -1, -1) if d[x] < 6.0)

    target_w = 600
    k = target_w / w
    img = Image.fromarray(c.astype(np.uint8), "RGBA").resize(
        (target_w, max(1, round(h * k))), Image.LANCZOS)
    bx, bz = round(left * k), round((w - 1 - right) * k)
    print(f"  bar_resource {w}x{h} -> {img.width}x{img.height}"
          f"   vien 9 manh: trai {bx}px, phai {bz}px")
    if not write:
        return
    OUT.mkdir(parents=True, exist_ok=True)
    img.save(OUT / "bar_resource.png")
    guid = hashlib.md5(b"spinsquad/ui/bar_resource").hexdigest()
    (OUT / "bar_resource.png.meta").write_text(
        META.format(guid=guid, ppu=100, bx=bx, by=0, bz=bz, bw=0))


def main():
    write = "--write" in sys.argv
    print(f"{'=' * 62}\nNEN (anh dac, PPU {BG_PPU})\n{'=' * 62}")
    for n in BACKGROUNDS:
        p = SRC / f"{n}.png"
        if not p.exists():
            print(f"  {n:26s} THIEU — bo qua")
            continue
        im = Image.open(p).convert("RGBA")
        flag = "" if (im.width, im.height) == (1125, 2436) else "  !!! SAI CO"
        print(f"  {n:26s} {im.width}x{im.height}{flag}")
        save(np.asarray(im).astype(float), n, BG_OUT, BG_PPU, write)

    print(f"\n{'=' * 62}\nTAM GHEP\n{'=' * 62}")
    for file, names, same_frame, square in SHEETS:
        p = SRC / f"{file}.png"
        if not p.exists():
            print(f"\n{file}: THIEU — bo qua")
            continue
        pieces = split(p, len(names))
        pieces, ws, target = balance(pieces)
        pieces, (fw, fh) = frame(pieces, square)
        print(f"\n{file}.png -> {len(names)} manh, khung chung {fw}x{fh}"
              f"{'  (bat buoc: chung thay nhau tai cho)' if same_frame else ''}")
        print(f"  do nang truoc khi can: {min(ws):.0f}-{max(ws):.0f} "
              f"(chenh {max(ws)/min(ws):.2f} lan) -> sau: {target:.0f} deu")
        for nm, pc in zip(names, pieces):
            print(f"    {nm:18s} do nang {weight(pc):6.1f}")
            save(pc, nm, OUT, 100, write)

    print(f"\n{'=' * 62}\nTHANH 9 MANH\n{'=' * 62}")
    import_bar(write)

    print(f"\n{'=' * 62}\nDAI TRANG TRI (vá đường nối)\n{'=' * 62}")
    p = SRC / "side_pillar.png"
    if p.exists():
        arr, before, after = mend_seam(p)
        ys, xs = np.where(arr[..., 3] > 10)
        arr = arr[ys.min():ys.max() + 1, xs.min():xs.max() + 1]
        print(f"  duong noi: {before:.1f} -> {after:.1f} tren 255"
              f"   {'OK' if after < 6 else 'VAN CON THAY VACH'}")
        print(f"  cat sat vien: {arr.shape[1]}x{arr.shape[0]}")
        save(arr, "side_pillar", OUT, 100, write)

    print("\n" + ("da ghi." if write else "chua ghi — chay lai voi --write"))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
