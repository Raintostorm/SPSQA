"""Cut approved sheets chat/check/002-buttons.png and 003-summon-kit.png into sprites.
No redrawing: crop by alpha components, keep alpha, keep existing Unity GUIDs on re-run."""
from pathlib import Path
import json, re, shutil, uuid
import numpy as np
from PIL import Image
from scipy import ndimage
ROOT = Path(__file__).resolve().parents[1]
TEMPLATE = (ROOT / 'Assets/_Project/Art/UI/btn_blue.png.meta').read_text()

def parts(path):
    im = Image.open(path).convert('RGBA')
    lab, _ = ndimage.label(np.array(im)[:, :, 3] > 40)
    boxes = [(s[1].start, s[0].start, s[1].stop, s[0].stop) for s in ndimage.find_objects(lab)
             if (s[0].stop - s[0].start) * (s[1].stop - s[1].start) > 5000]
    return im, sorted(boxes, key=lambda b: (b[1] // 60, b[0]))

def folder_meta(folder):
    meta = Path(str(folder) + '.meta')
    if not meta.exists():
        meta.write_text('fileFormatVersion: 2\nguid: ' + uuid.uuid4().hex + '\nfolderAsset: yes\nDefaultImporter:\n  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n')

def save(im, box, out, name, border):
    pad = 2
    crop = im.crop((max(0, box[0] - pad), max(0, box[1] - pad), min(im.width, box[2] + pad), min(im.height, box[3] + pad)))
    path = out / (name + '.png'); crop.save(path)
    meta = Path(str(path) + '.meta')
    if not meta.exists():
        text = re.sub(r'^guid: .*$', 'guid: ' + uuid.uuid4().hex, TEMPLATE, flags=re.M)
        meta.write_text(re.sub(r'spriteBorder: \{[^}]+\}', 'spriteBorder: {x: %d, y: %d, z: %d, w: %d}' % border, text))
    return {'name': name, 'size': list(crop.size), 'border_left_bottom_right_top': list(border), 'source_box': list(box)}

def run(sheet, out_dir, archive, names, borders):
    im, boxes = parts(ROOT / sheet)
    assert len(boxes) == len(names), (sheet, len(boxes))
    out = ROOT / out_dir; out.mkdir(parents=True, exist_ok=True); folder_meta(out)
    manifest = {'source': 'source.png', 'assets': []}
    for box, name in zip(boxes, names):
        if name: manifest['assets'].append(save(im, box, out, name, borders[name]))
    arch = ROOT / archive; arch.mkdir(parents=True, exist_ok=True)
    shutil.copy2(ROOT / sheet, arch / 'source.png')
    (arch / 'crop-manifest.json').write_text(json.dumps(manifest, indent=2) + '\n')

# 002: 4 rows (gold, blue, slate, red) x 3 states (normal, pressed, disabled). One grey is enough.
cap = (75, 62, 75, 62)  # semicircle ends + top highlight / bottom lip stay fixed
names = []
for color in ['gold', 'blue', 'slate', 'red']:
    names += [color, color + '-pressed', 'disabled' if color == 'gold' else None]
run('chat/check/002-buttons.png', 'Assets/_Project/Resources/Buttons', 'artsrc/ui/buttons-v1', names,
    {n: cap for n in names if n})

# 003: summon x1, summon x10, price pill, 5 rarity card frames.
run('chat/check/003-summon-kit.png', 'Assets/_Project/Resources/SummonKit', 'artsrc/ui/summon-kit-v1',
    ['summon-x1', 'summon-x10', 'price-pill', 'card-common', 'card-rare', 'card-epic', 'card-legendary', 'card-stone'],
    {'summon-x1': (280, 70, 70, 70), 'summon-x10': (300, 70, 150, 70), 'price-pill': (60, 45, 60, 45),
     **{n: (0, 0, 0, 0) for n in ['card-common', 'card-rare', 'card-epic', 'card-legendary', 'card-stone']}})
print('ok')

# 004 (CHỐT 2026-11-03): shop packs. Sparkle stars are separate alpha islands, so group
# islands by 3x2 grid cell and crop the union per cell.
def grid_parts(path, cols, rows):
    im = Image.open(path).convert('RGBA')
    lab, _ = ndimage.label(np.array(im)[:, :, 3] > 40)
    cells = {}
    for s in ndimage.find_objects(lab):
        if (s[0].stop - s[0].start) * (s[1].stop - s[1].start) < 400: continue
        cx, cy = (s[1].start + s[1].stop) / 2, (s[0].start + s[0].stop) / 2
        key = (int(cy * rows / im.height), int(cx * cols / im.width))
        b = cells.get(key, (s[1].start, s[0].start, s[1].stop, s[0].stop))
        cells[key] = (min(b[0], s[1].start), min(b[1], s[0].start), max(b[2], s[1].stop), max(b[3], s[0].stop))
    return im, [cells[k] for k in sorted(cells)]

im, boxes = grid_parts(ROOT / 'chat/check/004-shop-packs.png', 3, 2)
names = ['gold-small', 'gold-medium', 'gold-large', 'gems-small', 'gems-medium', 'gems-large']
assert len(boxes) == 6, len(boxes)
out = ROOT / 'Assets/_Project/Resources/ShopPacks'; out.mkdir(parents=True, exist_ok=True); folder_meta(out)
manifest = {'source': 'source.png', 'assets': [save(im, b, out, n, (0, 0, 0, 0)) for b, n in zip(boxes, names)]}
arch = ROOT / 'artsrc/ui/shop-packs-v1'; arch.mkdir(parents=True, exist_ok=True)
shutil.copy2(ROOT / 'chat/check/004-shop-packs.png', arch / 'source.png')
(arch / 'crop-manifest.json').write_text(json.dumps(manifest, indent=2) + '\n')
print('packs ok')

# 003-v2 (CHỐT 2026-11-03): flat rarity frames, navy window + dark name band. Replaces the
# ornate 003 cards in code; the old SummonKit/card-* files stay for reference.
run('chat/check/003-v2-cards.png', 'Assets/_Project/Resources/RewardCardsV2', 'artsrc/ui/reward-cards-v2',
    ['card-common', 'card-rare', 'card-epic', 'card-legendary', 'card-stone'],
    {n: (0, 0, 0, 0) for n in ['card-common', 'card-rare', 'card-epic', 'card-legendary', 'card-stone']})
print('cards v2 ok')

# 006 + 007 (CHỐT 2026-11-03): roster tiles (info band = bottom ~1/3) and status badges.
tiles = ['tile-common', 'tile-rare', 'tile-epic', 'tile-legendary', 'tile-mythic', 'tile-locked']
run('chat/check/006-roster-tiles.png', 'Assets/_Project/Resources/RosterTilesV1', 'artsrc/ui/roster-tiles-v1',
    tiles, {n: (0, 0, 0, 0) for n in tiles})
badges = ['dot', 'upgrade', 'check', 'lock', 'new']
run('chat/check/007-badges.png', 'Assets/_Project/Resources/Badges', 'artsrc/ui/badges-v1',
    badges, {n: (0, 0, 0, 0) for n in badges})
print('tiles + badges ok')

# 008 (CHỐT 2026-11-03): 8 buff icons that used to be cut from the opaque catalog proposal.
im, boxes = grid_parts(ROOT / 'chat/check/008-buff-icons.png', 4, 2)
names = ['double-strike', 'bodyguard', 'frost-touch', 'grand-ascension', 'reinforcements', 'weapon-supplies', 'war-chest', 'second-wind']
assert len(boxes) == 8, len(boxes)
out = ROOT / 'Assets/_Project/Resources/EffectBuffs/icons'; out.mkdir(parents=True, exist_ok=True); folder_meta(out)
manifest = {'source': 'source.png', 'assets': [save(im, b, out, n, (0, 0, 0, 0)) for b, n in zip(boxes, names)]}
arch = ROOT / 'artsrc/ui/buff-icons-v2'; arch.mkdir(parents=True, exist_ok=True)
shutil.copy2(ROOT / 'chat/check/008-buff-icons.png', arch / 'source.png')
(arch / 'crop-manifest.json').write_text(json.dumps(manifest, indent=2) + '\n')
print('buff icons ok')
