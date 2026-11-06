"""Cat bo nut hep (chat 011) vao Resources/ButtonsV2.

Theo doc/UI_ART_SPEC.md muc 1: nut ve hep 260x150, khoa 125px moi dau.
Dai 10px chinh giua bi Unity keo ra toi 750px, nen moi cot pixel trong dai do
phai giong het nhau — imagegen khong lam duoc chinh xac, nen o day tu chuan hoa:
chep mot cot duy nhat de len ca dai. Lech mot chut se thanh vet ban chay doc nut.
"""
from pathlib import Path
import re, uuid
import numpy as np
from PIL import Image

ROOT = Path(__file__).resolve().parents[1]
TEMPLATE = (ROOT / 'Assets/_Project/Art/UI/btn_blue.png.meta').read_text()
OUT = ROOT / 'Assets/_Project/Resources/ButtonsV2'
W, H = 260, 150
BORDER = (125, 60, 125, 30)          # trai, day, phai, dinh
MID = (BORDER[0], W - BORDER[2])     # dai bi keo
SHEETS = [
    ('chat/check/030-buttons-normal.png', ['gold', 'blue', 'slate', 'red', 'disabled']),
    ('chat/check/030-buttons-pressed.png', ['gold-pressed', 'blue-pressed', 'slate-pressed', 'red-pressed']),
]


def columns(alpha, gap=24):
    present = alpha.any(0)
    runs, start = [], None
    for i, on in enumerate(present):
        if on and start is None:
            start = i
        elif not on and start is not None:
            runs.append([start, i]); start = None
    if start is not None:
        runs.append([start, len(present)])
    merged = [runs[0]]
    for a, b in runs[1:]:
        if a - merged[-1][1] < gap:
            merged[-1][1] = b
        else:
            merged.append([a, b])
    return merged


def main():
    OUT.mkdir(parents=True, exist_ok=True)
    meta = Path(str(OUT) + '.meta')
    if not meta.exists():
        meta.write_text('fileFormatVersion: 2\nguid: ' + uuid.uuid4().hex +
                        '\nfolderAsset: yes\nDefaultImporter:\n  externalObjects: {}\n'
                        '  userData: \n  assetBundleName: \n  assetBundleVariant: \n')
    for sheet, names in SHEETS:
        im = Image.open(ROOT / sheet).convert('RGBA')
        alpha = np.array(im)[:, :, 3] > 16
        spans = columns(alpha)
        assert len(spans) == len(names), f'{sheet}: {len(spans)} o, can {len(names)}'
        for name, (x0, x1) in zip(names, spans):
            rows = np.where(alpha[:, x0:x1].any(1))[0]
            cell = im.crop((x0, rows.min(), x1, rows.max() + 1)).resize((W, H), Image.LANCZOS)
            px = np.array(cell)
            seed = px[:, (MID[0] + MID[1]) // 2].copy()          # mot cot duy nhat
            px[:, MID[0]:MID[1]] = seed[:, None, :]              # phu kin dai bi keo
            path = OUT / (name + '.png')
            Image.fromarray(px).save(path)
            m = Path(str(path) + '.meta')
            if not m.exists():
                text = re.sub(r'^guid: .*$', 'guid: ' + uuid.uuid4().hex, TEMPLATE, flags=re.M)
                m.write_text(re.sub(r'spriteBorder: \{[^}]+\}',
                                    'spriteBorder: {x: %d, y: %d, z: %d, w: %d}' % BORDER, text))
            print(name, f'{W}x{H}', 'border', BORDER)


main()
