"""Visible (alpha) bounds for UI sprites whose PNGs carry transparent margins.
Unity imports them FullRect + not readable, so runtime cannot measure pixels.
Writes Assets/_Project/Resources/UiSpriteBounds.txt, one line per sprite: name xMin yMin xMax yMax (0..1, y up)."""
from pathlib import Path
import json, re
from PIL import Image
ROOT = Path(__file__).resolve().parents[1]
guid_to_png = {}
for meta in (ROOT / 'Assets').rglob('*.png.meta'):
    m = re.search(r'^guid: (\w+)', meta.read_text(errors='ignore'), re.M)
    if m: guid_to_png[m.group(1)] = Path(str(meta)[:-5])
pngs = list((ROOT / 'Assets/_Project/Resources/ShopV2/Heads').glob('*.png'))
relics = (ROOT / 'Assets/_Project/Resources/Shop/Relics.asset').read_text()
pngs += [guid_to_png[g] for g in re.findall(r'Icon: \{fileID: \d+, guid: (\w+)', relics) if g in guid_to_png]
bounds = {}
for p in sorted(set(pngs)):
    im = Image.open(p).convert('RGBA'); box = im.getchannel('A').point(lambda v: 255 if v > 24 else 0).getbbox()
    if not box: continue
    w, h = im.size
    bounds[p.stem] = [round(box[0] / w, 4), round(1 - box[3] / h, 4), round(box[2] / w, 4), round(1 - box[1] / h, 4)]
out = ROOT / 'Assets/_Project/Resources/UiSpriteBounds.txt'
out.write_text(''.join(f'{k} {v[0]} {v[1]} {v[2]} {v[3]}\n' for k, v in sorted(bounds.items())))
print(len(bounds), 'sprites')
