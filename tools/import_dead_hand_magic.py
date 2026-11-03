"""Import approved effect-only sheet without changing character frames/pivots."""
from pathlib import Path
import hashlib
import json
import re
from PIL import Image

root = Path(__file__).resolve().parents[1]
body = root / 'Assets/_Project/Art/UnitsSimple/dead_legendary'
out = root / 'Assets/_Project/Art/VFX/DeadHandMagic'
out.mkdir(parents=True, exist_ok=True)
sheet = Image.open(root / 'artsrc/vfx/dead-current-v3/hand-magic.png').convert('RGBA')
template = (body / 'skill_010.png.meta').read_text()
guids = []
for i in range(6):
    guid = hashlib.md5(('dead-hand-magic/' + str(i)).encode()).hexdigest()
    guids.append(guid)
    path = out / f'frame_{i}.png'
    sheet.crop((i % 3 * 512, i // 3 * 512, (i % 3 + 1) * 512, (i // 3 + 1) * 512)).save(path)
    meta = re.sub(r'^guid: .*', 'guid: ' + guid, template, flags=re.M)
    meta = re.sub(r'spritePixelsToUnits: .*', 'spritePixelsToUnits: 512', meta)
    meta = re.sub(r'spritePivot: .*', 'spritePivot: {x: 0.5, y: 0.5}', meta)
    meta = re.sub(r'spriteAlignment: .*', 'spriteAlignment: 9', meta)
    Path(str(path) + '.meta').write_text(meta)

frames = json.loads((body / 'frames.json').read_text())['clips']['skill']
offsets = []
for i, frame in enumerate(frames):
    x = y = 0.0
    if 9 <= i < 14:
        im = Image.open(body / frame['file']).convert('RGBA')
        w, h = im.size
        pixels = im.load()
        points = [(px, py) for py in range(int(h * .1), int(h * .65))
                  for px in range(int(w * .72), w)
                  if pixels[px, py][3] > 180 and pixels[px, py][0] > 190
                  and pixels[px, py][1] > 160 and pixels[px, py][2] > 120]
        assert points, frame['file']
        edge = max(p[0] for p in points)
        points = [p for p in points if p[0] >= edge - 18]
        ppu = float(re.search(r'spritePixelsToUnits: ([\d.]+)', Path(str(body / frame['file']) + '.meta').read_text())[1])
        x = (sum(p[0] for p in points) / len(points) - frame['pivot'][0] * w) / ppu
        y = ((1 - frame['pivot'][1]) * h - sum(p[1] for p in points) / len(points)) / ppu + .114
    offsets.append(f'  - {{x: {x:.7f}, y: {y:.7f}}}')
asset = root / 'Assets/_Project/Data/Units/Anim_dead_legendary.asset'
data = asset.read_text()
# Replace only this importer's appended configuration; all existing clips stay untouched.
data = data.split('  SkillVfx:\n')[0]
data += '  SkillVfx:\n    Frames:\n'
data += ''.join(f'    - {{fileID: 21300000, guid: {guid}, type: 3}}\n' for guid in guids)
data += '    Durations:\n' + '    - 0.027777778\n' * 6
data += '    Sequence: []\n    Loop: 0\n    UseExplicitPlayback: 1\n    PingPong: 0\n'
data += '  SkillVfxStartStep: 9\n  SkillVfxEndStep: 14\n  SkillVfxScale: 0.33\n  SkillImpactStep: 11\n  SkillVfxOffsets:\n'
data += '\n'.join(offsets) + '\n'
asset.write_text(data)
print('Imported six approved magic frames; anchored to five existing open-palm poses.')
