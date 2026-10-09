#!/usr/bin/env python3
"""Cat 4 o dat tu tiles_ai.png thanh 4 sprite rieng.

Khac voi import_ui_art.py: o dat KHONG dung 9 manh. Nut bam phai co dan
theo chu ben trong, con o dat thi luon dung mot co — co dinh boi luoi. Keo
9 manh o day chi lam vien da bi keo dai ra khong deu.

Ba viec:
  1. tach nen magenta + khu vien  (dung lai phep cua import_ui_art)
  2. ep ca 4 o ve CUNG mot khung — chung thay nhau tai cho luc bam chon,
     lech mot pixel la o nhay
  3. bop ve dung ti le o that trong game

    python3 tools/import_tiles.py          # chi do, khong ghi
    python3 tools/import_tiles.py --write  # ghi that
"""

import hashlib
import sys
from pathlib import Path

import numpy as np
from PIL import Image

sys.path.insert(0, str(Path(__file__).parent))
from import_ui_art import META, key_out        # noqa: E402

SRC = Path("artsrc/ui/tiles_ai.png")
OUT = Path("Assets/_Project/Art/UI")
KEY = np.array([255.0, 0.0, 255.0])

# Ti le o that trong game, lay tu BattleSceneSetup:
#   PITCH_X = 121/1672 cua be ngang, PITCH_Y = 118/941 cua be cao, FILL 0.94
# Quy ve khung 1920x1080 la 131 x 127. Khong tu go so o day — doc ra tu
# chinh hai phan so do, de sua luoi mot lan la ca hai noi cung doi.
PITCH_X_FRAC = 121.0 / 1672.0
PITCH_Y_FRAC = 118.0 / 941.0
ASPECT = (PITCH_X_FRAC * 1920.0) / (PITCH_Y_FRAC * 1080.0)

HEIGHT = 310          # cao cua sprite xuat ra; ~2.4 lan co tren man hinh

NAMES = ["tile_ally", "tile_ally_sel", "tile_enemy", "tile_enemy_sel"]


def quadrants(alpha, W, H):
    """Khung bao cua tung o, chia tam theo nua tren/duoi va trai/phai.

    Khong dung phep danh nhan cum: hai o chon co quang sang lan ra, cum co
    the dinh vao nhau. Chia tam thi khong bao gio nham, mien la AI xep
    dung 2x2 nhu da yeu cau.
    """
    out = []
    for r, c in ((0, 0), (0, 1), (1, 0), (1, 1)):
        y0, y1 = (0, H // 2) if r == 0 else (H // 2, H)
        x0, x1 = (0, W // 2) if c == 0 else (W // 2, W)
        m = alpha[y0:y1, x0:x1] > 0.5
        if not m.any():
            raise SystemExit(f"O ({r},{c}) rong — AI khong xep 2x2 nhu yeu cau.")
        ys, xs = np.where(m)
        out.append((x0 + xs.min(), y0 + ys.min(), x0 + xs.max() + 1, y0 + ys.max() + 1))
    return out


def main():
    write = "--write" in sys.argv
    if not SRC.exists():
        print(f"Khong thay {SRC}")
        return 1

    rgb = np.asarray(Image.open(SRC).convert("RGB")).astype(float)
    H, W = rgb.shape[:2]
    colour, alpha = key_out(rgb, KEY)

    boxes = quadrants(alpha, W, H)
    ws = [b[2] - b[0] for b in boxes]
    hs = [b[3] - b[1] for b in boxes]
    print(f"tam {W}x{H}, nen magenta {(alpha < 0.5).mean() * 100:.0f}%")
    print(f"{'o':16}{'rong':>7}{'cao':>7}{'ti le':>8}")
    for n, w, h in zip(NAMES, ws, hs):
        print(f"{n:16}{w:7}{h:7}{w / h:8.3f}")
    print(f"lech: rong {max(ws) - min(ws)}px, cao {max(hs) - min(hs)}px")
    print(f"ti le trung binh {np.mean([w / h for w, h in zip(ws, hs)]):.3f} "
          f"-> ep ve {ASPECT:.3f}")

    # Khung chung = o to nhat. Dem o nho hon vao giua chu KHONG phong to:
    # phong to mot o len cho bang o khac la lam no mo hon ba o con lai.
    fw, fh = max(ws), max(hs)
    out_w = int(round(HEIGHT * ASPECT))

    rgba = np.dstack([colour, alpha * 255.0]).clip(0, 255).astype(np.uint8)
    full = Image.fromarray(rgba, "RGBA")

    for name, (x0, y0, x1, y1) in zip(NAMES, boxes):
        frame = Image.new("RGBA", (fw, fh), (0, 0, 0, 0))
        frame.alpha_composite(full.crop((x0, y0, x1, y1)),
                              ((fw - (x1 - x0)) // 2, (fh - (y1 - y0)) // 2))
        tile = frame.resize((out_w, HEIGHT), Image.LANCZOS)
        if write:
            tile.save(OUT / f"{name}.png")
            guid = hashlib.md5(f"spinsquad/ui/{name}".encode()).hexdigest()
            # spriteBorder toan 0: o dat khong keo 9 manh.
            (OUT / f"{name}.png.meta").write_text(META.format(
                guid=guid, ppu="100.00", bx=0, by=0, bz=0, bw=0))
        print(f"  {name}.png  {out_w}x{HEIGHT}")

    print("\nDa ghi." if write else "\nChua ghi gi. Them --write de ghi that.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
