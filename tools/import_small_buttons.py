"""Cat nut tron va ky hieu (chat 012) vao Resources/SmallButtons + Resources/Glyphs.

doc/UI_ART_SPEC.md muc 2: moi icon vuong dung mot co chuan 150x150.
Sheet nut tron co 2 hang (thuong / bam) nen phai gom theo CA hang va cot.
"""
from pathlib import Path
import re, uuid
import numpy as np
from PIL import Image

ROOT = Path(__file__).resolve().parents[1]
TEMPLATE = (ROOT / 'Assets/_Project/Art/UI/btn_blue.png.meta').read_text()
SIDE = 150
FIT = 142          # phan nhin thay, chua le 4px moi ben


def bands(mask, gap):
    runs, start = [], None
    for i, on in enumerate(mask):
        if on and start is None:
            start = i
        elif not on and start is not None:
            runs.append([start, i]); start = None
    if start is not None:
        runs.append([start, len(mask)])
    merged = [runs[0]]
    for a, b in runs[1:]:
        if a - merged[-1][1] < gap:
            merged[-1][1] = b
        else:
            merged.append([a, b])
    return merged


def save(cell, out, name):
    scale = FIT / max(cell.size)
    cell = cell.resize((max(1, round(cell.width * scale)), max(1, round(cell.height * scale))), Image.LANCZOS)
    canvas = Image.new('RGBA', (SIDE, SIDE), (0, 0, 0, 0))
    canvas.paste(cell, ((SIDE - cell.width) // 2, (SIDE - cell.height) // 2))
    out.mkdir(parents=True, exist_ok=True)
    folder = Path(str(out) + '.meta')
    if not folder.exists():
        folder.write_text('fileFormatVersion: 2\nguid: ' + uuid.uuid4().hex +
                          '\nfolderAsset: yes\nDefaultImporter:\n  externalObjects: {}\n'
                          '  userData: \n  assetBundleName: \n  assetBundleVariant: \n')
    path = out / (name + '.png')
    canvas.save(path)
    m = Path(str(path) + '.meta')
    if not m.exists():
        text = re.sub(r'^guid: .*$', 'guid: ' + uuid.uuid4().hex, TEMPLATE, flags=re.M)
        m.write_text(re.sub(r'spriteBorder: \{[^}]+\}', 'spriteBorder: {x: 0, y: 0, z: 0, w: 0}', text))
    print(name, f'{SIDE}x{SIDE}')


def main():
    # --- nut tron: 4 cot x 2 hang ---
    im = Image.open(ROOT / 'chat/check/031-round-plates.png').convert('RGBA')
    alpha = np.array(im)[:, :, 3] > 16
    rows, cols = bands(alpha.any(1), 40), bands(alpha.any(0), 40)
    assert len(rows) == 2 and len(cols) == 4, f'{len(cols)} cot x {len(rows)} hang'
    colours = ['blue', 'navy', 'red', 'gold']
    for r, (y0, y1) in enumerate(rows):
        for c, (x0, x1) in enumerate(cols):
            sub = alpha[y0:y1, x0:x1]
            ys = np.where(sub.any(1))[0]; xs = np.where(sub.any(0))[0]
            cell = im.crop((x0 + xs.min(), y0 + ys.min(), x0 + xs.max() + 1, y0 + ys.min() + (ys.max() - ys.min() + 1)))
            save(cell, ROOT / 'Assets/_Project/Resources/SmallButtons',
                 colours[c] + ('-pressed' if r else ''))

    # --- ky hieu: 6 cot x 1 hang ---
    im = Image.open(ROOT / 'chat/check/031-glyphs.png').convert('RGBA')
    alpha = np.array(im)[:, :, 3] > 16
    # Khe hep nhat giua hai ky hieu chi 34px, nen nguong gom phai nho hon the.
    cols = bands(alpha.any(0), 24)
    names = ['back', 'next', 'menu', 'close', 'plus', 'info']
    assert len(cols) == len(names), f'{len(cols)} ky hieu'
    for name, (x0, x1) in zip(names, cols):
        ys = np.where(alpha[:, x0:x1].any(1))[0]
        save(im.crop((x0, ys.min(), x1, ys.max() + 1)), ROOT / 'Assets/_Project/Resources/Glyphs', name)


main()
