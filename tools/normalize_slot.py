#!/usr/bin/env python3
"""
Chuan hoa moi sprite cua CUNG MOT SLOT ve chung mot khung.

Van de: slot "body" co the mang body tran (349x371) hoac giap (272x419).
Doi sprite thi DINH than dich 58px → dau bay lo lung hoac lun vao than.

Cach giai: scale moi sprite ve CUNG CHIEU CAO, dat len cung mot canvas,
can giua theo chieu ngang, can day theo chieu doc.

Ket qua: pivot (0.5, 0) luon la cung mot diem cham dat, va DINH canvas luon
la cung mot do cao → vi tri dau/tay KHONG doi khi thay trang bi.

    python3 normalize_slot.py out/ a.png b.png c.png
"""
import argparse, os, sys
from PIL import Image


def content_box(im):
    """Khung bao phan khong trong suot."""
    a = im.getchannel("A")
    bb = a.getbbox()
    return bb or (0, 0, im.width, im.height)


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("out")
    ap.add_argument("files", nargs="+")
    ap.add_argument("--by", choices=["height", "width"], default="height",
                    help=("truc de dong bo. 'height' cho slot body (than phai "
                          "cung cao). 'width' cho slot helmet — mu phai cung "
                          "BE RONG de om vua dau, con mao thi duoc phep cao thap "
                          "khac nhau."))
    ap.add_argument("--size", type=int, default=0,
                    help="kich thuoc dich tren truc do (mac dinh: lon nhat trong bo)")
    ap.add_argument("--pad", type=int, default=4, help="dem quanh canvas")
    args = ap.parse_args()

    imgs = []
    for f in args.files:
        im = Image.open(f).convert("RGBA")
        x0, y0, x1, y1 = content_box(im)
        imgs.append((f, im.crop((x0, y0, x1, y1))))

    if args.by == "height":
        target = args.size or max(im.height for _, im in imgs)
    else:
        target = args.size or max(im.width for _, im in imgs)

    scaled = []
    for f, im in imgs:
        k = target / (im.height if args.by == "height" else im.width)
        nw, nh = max(1, round(im.width * k)), max(1, round(im.height * k))
        scaled.append((f, im.resize((nw, nh), Image.LANCZOS), k))

    canvas_w = max(im.width for _, im, _ in scaled) + args.pad * 2
    canvas_h = max(im.height for _, im, _ in scaled) + args.pad * 2

    os.makedirs(args.out, exist_ok=True)
    print(f"Canvas chung: {canvas_w} x {canvas_h}   (dong bo theo {args.by} = {target})\n")
    print(f"{'file':<26}{'goc':>12}{'scale':>8}{'→ trong canvas':>18}")
    print("─" * 66)

    for f, im, k in scaled:
        out = Image.new("RGBA", (canvas_w, canvas_h), (0, 0, 0, 0))
        x = (canvas_w - im.width) // 2          # can giua ngang
        y = canvas_h - args.pad - im.height     # can day doc
        out.paste(im, (x, y), im)
        name = os.path.basename(f)
        out.save(os.path.join(args.out, name))
        orig = Image.open(f)
        print(f"  {name:<24}{orig.width:>5}x{orig.height:<6}{k:>7.2f}x"
              f"{im.width:>8}x{im.height:<6} tai x={x}")

    print(f"\nTat ca → {args.out}")
    print("Pivot dung (0.5, 0) cho MOI file trong slot nay.")


if __name__ == "__main__":
    main()
