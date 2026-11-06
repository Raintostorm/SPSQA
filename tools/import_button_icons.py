"""Cat sheet chat/check/027-button-icons.png thanh 8 icon nut.

Khong group theo connected component: tia sang cua qua cau va vach xanh cua dau tick
la nhung manh roi, label se tach nhau. Thay vao do gom theo khoang trong doc cot.
Moi icon duoc dat vao khung vuong 256x256, canh dai nhat = 224, giu ti le.
Chay lai van giu nguyen GUID cu de khong dut tham chieu trong Unity.
"""
from pathlib import Path
import json, re, uuid
import numpy as np
from PIL import Image

ROOT = Path(__file__).resolve().parents[1]
TEMPLATE = (ROOT / 'Assets/_Project/Art/UI/btn_blue.png.meta').read_text()
SHEET = ROOT / 'chat/check/027-button-icons.png'
OUT = ROOT / 'Assets/_Project/Resources/ButtonIcons'
NAMES = ['battle', 'summon-1', 'summon-10', 'key', 'upgrade', 'continue', 'confirm', 'cancel']
BOX, FIT = 256, 224


def columns(alpha, gap=24):
    """Cac khoang cot co pixel, bo qua khe hep hon `gap`."""
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
    im = Image.open(SHEET).convert('RGBA')
    alpha = np.array(im)[:, :, 3] > 16
    spans = columns(alpha)
    assert len(spans) == len(NAMES), f'tim thay {len(spans)} icon, can {len(NAMES)}'
    OUT.mkdir(parents=True, exist_ok=True)
    meta = Path(str(OUT) + '.meta')
    if not meta.exists():
        meta.write_text('fileFormatVersion: 2\nguid: ' + uuid.uuid4().hex +
                        '\nfolderAsset: yes\nDefaultImporter:\n  externalObjects: {}\n'
                        '  userData: \n  assetBundleName: \n  assetBundleVariant: \n')
    report = []
    for name, (x0, x1) in zip(NAMES, spans):
        rows = np.where(alpha[:, x0:x1].any(1))[0]
        crop = im.crop((x0, rows.min(), x1, rows.max() + 1))
        scale = FIT / max(crop.size)
        crop = crop.resize((max(1, round(crop.width * scale)), max(1, round(crop.height * scale))), Image.LANCZOS)
        canvas = Image.new('RGBA', (BOX, BOX), (0, 0, 0, 0))
        canvas.paste(crop, ((BOX - crop.width) // 2, (BOX - crop.height) // 2))
        path = OUT / (name + '.png')
        canvas.save(path)
        m = Path(str(path) + '.meta')
        if not m.exists():
            text = re.sub(r'^guid: .*$', 'guid: ' + uuid.uuid4().hex, TEMPLATE, flags=re.M)
            m.write_text(re.sub(r'spriteBorder: \{[^}]+\}', 'spriteBorder: {x: 0, y: 0, z: 0, w: 0}', text))
        report.append({'name': name, 'source_x': [int(x0), int(x1)], 'visible': list(crop.size)})
    (OUT / 'manifest.json').write_text(json.dumps(report, indent=1))
    for row in report:
        print(row['name'], row['visible'])


main()
