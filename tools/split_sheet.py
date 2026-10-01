#!/usr/bin/env python3
"""
Tach sprite sheet nen magenta thanh tung part PNG trong suot.

Dung cho sheet nhan vat do AI sinh: moi nhan vat gom 3 mang roi nhau
(head / body / arm). Script tim cac mang lien thong, gom theo nhan vat,
nhan dien part theo vi tri, roi xuat PNG + file offset JSON.

File offset quan trong khong kem anh: no giu VI TRI TUONG DOI giua cac
part, de rap lai trong Spine dung y nhu ban goc.

    python3 split_sheet.py sheet.png -o out/ --names stoneage medieval modern future
"""
import argparse, json, os, sys
from collections import deque
from PIL import Image

MAGENTA_TOL = 110          # khoang cach toi (255,0,255) coi la nen
MIN_COMPONENT = 300        # mang nho hon coi la rac
PAD = 4                    # dem quanh part khi crop


def is_bg(p):
    r, g, b = p[0], p[1], p[2]
    return r > 255 - MAGENTA_TOL and g < MAGENTA_TOL and b > 255 - MAGENTA_TOL


def find_components(px, W, H):
    seen = bytearray(W * H)
    comps = []
    for sy in range(H):
        row = sy * W
        for sx in range(W):
            if seen[row + sx] or is_bg(px[sx, sy]):
                continue
            q = deque([(sx, sy)])
            seen[row + sx] = 1
            pts = []
            while q:
                x, y = q.popleft()
                pts.append((x, y))
                for dx, dy in ((1, 0), (-1, 0), (0, 1), (0, -1)):
                    nx, ny = x + dx, y + dy
                    if 0 <= nx < W and 0 <= ny < H:
                        i = ny * W + nx
                        if not seen[i] and not is_bg(px[nx, ny]):
                            seen[i] = 1
                            q.append((nx, ny))
            if len(pts) >= MIN_COMPONENT:
                comps.append(pts)
    return comps


def bbox(pts):
    xs = [p[0] for p in pts]; ys = [p[1] for p in pts]
    return min(xs), min(ys), max(xs), max(ys)


def erode(pts, times=2):
    """Bao mon vien. Pixel khu rang cua cua anh goc la mau PHA giua nhan vat
    va magenta — chung khong du magenta de bi loai, nen lot vao part va tao
    vien tim. Vien den cua style nay day ~10px, bo 2px khong nhin thay."""
    s = set(pts)
    for _ in range(times):
        s = {(x, y) for (x, y) in s
             if (x+1, y) in s and (x-1, y) in s and (x, y+1) in s and (x, y-1) in s}
    return list(s)


def cut(src, pts, W, H):
    """Crop mot mang thanh anh RGBA trong suot. Chi giu pixel CUA MANG DO —
    nen mang khac chong len bounding box cung khong bi dinh theo."""
    pts = erode(pts)
    if not pts:
        return Image.new("RGBA", (1, 1), (0, 0, 0, 0)), (0, 0, 0, 0)
    x0, y0, x1, y1 = bbox(pts)
    x0, y0 = max(0, x0 - PAD), max(0, y0 - PAD)
    x1, y1 = min(W - 1, x1 + PAD), min(H - 1, y1 + PAD)
    out = Image.new("RGBA", (x1 - x0 + 1, y1 - y0 + 1), (0, 0, 0, 0))
    op = out.load(); sp = src.load()
    for (x, y) in pts:
        r, g, b = sp[x, y][:3]
        op[x - x0, y - y0] = (r, g, b, 255)
    return out, (x0, y0, x1, y1)


def classify(group):
    """head = cao nhat · arm = nho nhat trong hai cai con lai · body = cai con lai"""
    by_top = sorted(group, key=lambda c: bbox(c)[1])
    head = by_top[0]
    rest = by_top[1:]
    if not rest:
        return {"head": head}
    if len(rest) == 1:
        return {"head": head, "body": rest[0]}
    rest_by_size = sorted(rest, key=len)
    return {"head": head, "hand": rest_by_size[0], "body": rest_by_size[-1]}


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("sheet")
    ap.add_argument("-o", "--out", default="out")
    ap.add_argument("--names", nargs="*", default=None,
                    help="ten tung nhan vat, trai sang phai")
    ap.add_argument("--role", default="", help="tien to vai tro, vd melee")
    ap.add_argument("--grid", default="", help=(
        "sheet dang luoi, vd 3x4. Chia anh thanh o roi lay mang lon nhat "
        "moi o. Dung voi --rows va --cols de dat ten."))
    ap.add_argument("--rows", nargs="*", default=None, help="ten tung hang")
    ap.add_argument("--cols", nargs="*", default=None, help="ten tung cot")
    ap.add_argument("--part", default="", help=(
        "sheet mot-part (giap, mu, vu khi): moi o chi co 1 mang. "
        "Xuat file phang <ten>.png thay vi thu muc head/body/hand."))
    args = ap.parse_args()

    src = Image.open(args.sheet).convert("RGB")
    W, H = src.size
    px = src.load()

    comps = find_components(px, W, H)
    print(f"Anh {W}x{H} — tim thay {len(comps)} mang roi nhau")
    if not comps:
        sys.exit("Khong thay mang nao. Kiem tra mau nen co phai magenta khong.")

    # ── Che do LUOI: chia o co dinh, moi o lay mang lon nhat ──────────
    if args.grid:
        R, C = (int(v) for v in args.grid.lower().split("x"))
        rows = args.rows or [f"r{i+1}" for i in range(R)]
        cols = args.cols or [f"c{i+1}" for i in range(C)]
        ch, cw2 = H / R, W / C
        os.makedirs(args.out, exist_ok=True)
        cells = {}
        for pts in comps:
            x0, y0, x1, y1 = bbox(pts)
            r = min(R - 1, int(((y0 + y1) / 2) // ch))
            c = min(C - 1, int(((x0 + x1) / 2) // cw2))
            # giu mang lon nhat moi o (loai manh vun)
            if (r, c) not in cells or len(pts) > len(cells[(r, c)]):
                cells[(r, c)] = pts
        mf2 = {}
        print(f"Luoi {R}x{C} — tim duoc {len(cells)}/{R*C} o co noi dung\n")
        for r in range(R):
            for c in range(C):
                if (r, c) not in cells:
                    print(f"   {rows[r]}_{cols[c]:<12} TRONG")
                    continue
                img, _ = cut(src, cells[(r, c)], W, H)
                name = f"{rows[r]}_{cols[c]}"
                fn = os.path.join(args.out, f"{name}.png")
                img.save(fn)
                mf2[name] = {"w": img.width, "h": img.height}
                print(f"   {name:<22} {img.width:>4} x {img.height:<4}  → {fn}")
        with open(os.path.join(args.out, "offsets.json"), "w") as f:
            json.dump(mf2, f, indent=2)
        return

    # gom theo nhan vat: cum theo tam x
    comps.sort(key=lambda c: (bbox(c)[0] + bbox(c)[2]) / 2)
    groups, cur = [], [comps[0]]
    for c in comps[1:]:
        prev_right = max(bbox(p)[2] for p in cur)
        if bbox(c)[0] > prev_right:          # khong chong lan -> nhan vat moi
            groups.append(cur); cur = [c]
        else:
            cur.append(c)
    groups.append(cur)
    print(f"Gom thanh {len(groups)} nhan vat\n")

    names = args.names or [f"unit{i+1}" for i in range(len(groups))]
    if len(names) < len(groups):
        names += [f"unit{i+1}" for i in range(len(names), len(groups))]

    os.makedirs(args.out, exist_ok=True)
    manifest = {}

    for gi, group in enumerate(groups):
        name = names[gi]
        prefix = f"{args.role}_{name}" if args.role else name
        gx0 = min(bbox(c)[0] for c in group)
        gy0 = min(bbox(c)[1] for c in group)

        # Sheet mot-part: xuat file phang, khong gom vao thu muc con
        if args.part:
            img, (x0, y0, x1, y1) = cut(src, group[0], W, H)
            fn = os.path.join(args.out, f"{prefix}.png")
            img.save(fn)
            manifest[prefix] = {args.part: {"x": 0, "y": 0,
                                            "w": img.width, "h": img.height}}
            print(f"   {prefix:<12} {img.width:>4} x {img.height:<4}  → {fn}")
            continue

        parts = classify(group)
        d = os.path.join(args.out, prefix)
        os.makedirs(d, exist_ok=True)
        entry = {}
        print(f"── {prefix}")
        for pname, pts in parts.items():
            img, (x0, y0, x1, y1) = cut(src, pts, W, H)
            fn = os.path.join(d, f"{pname}.png")
            img.save(fn)
            # offset: goc trai-tren cua part so voi goc trai-tren cua nhan vat
            entry[pname] = {"x": x0 - gx0, "y": y0 - gy0,
                            "w": img.width, "h": img.height}
            print(f"   {pname:<5} {img.width:>4} x {img.height:<4}"
                  f"  offset ({x0-gx0:>4}, {y0-gy0:>4})  → {fn}")
        manifest[prefix] = entry

    mf = os.path.join(args.out, "offsets.json")
    with open(mf, "w") as f:
        json.dump(manifest, f, indent=2)
    print(f"\nOffset luu tai {mf}")
    print("Dung file nay de dat part dung vi tri trong Spine.")


if __name__ == "__main__":
    main()
