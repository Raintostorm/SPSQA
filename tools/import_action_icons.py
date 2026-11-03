"""Extract approved 4x3 sheet; preserve source alpha and existing Unity GUIDs."""
from pathlib import Path
import hashlib
import re
import shutil
from PIL import Image

root = Path(__file__).resolve().parents[1]
out = root / 'Assets/_Project/Resources/SharedUI'
backup = root / 'outdata/action_icons_v2_backup'
backup.mkdir(parents=True, exist_ok=True)
sheet = Image.open(root / 'artsrc/ui/action_icons_v2.png').convert('RGBA')
template = (root / 'Assets/_Project/Art/UI/btn_blue.png.meta').read_text()
names = ('settings', 'enemy', 'upgrade', 'relics', 'home', 'restart',
         'battle', 'roll', 'resume', 'sound', 'vibration', 'close')
for i, name in enumerate(names):
    path = out / (name + '.png')
    if path.exists() and not (backup / path.name).exists():
        shutil.copy2(path, backup / path.name)
    x, y = i % 4, i // 4
    # The generated row gutters are at y=366 and 700, not exact thirds.
    ys = (0, round(sheet.height * 366 / 1024), round(sheet.height * 700 / 1024), sheet.height)
    cell = sheet.crop((round(x * sheet.width / 4), ys[y],
                       round((x + 1) * sheet.width / 4), ys[y + 1]))
    cell.save(path)
    meta = Path(str(path) + '.meta')
    if not meta.exists():
        data = re.sub(r'^guid: .*', 'guid: ' + hashlib.md5(('shared-ui/' + path.name).encode()).hexdigest(), template, flags=re.M)
        data = re.sub(r'spriteBorder: .*', 'spriteBorder: {x: 0, y: 0, z: 0, w: 0}', data)
        meta.write_text(data)
    print(name, cell.size, cell.getchannel('A').getextrema())

for name in ('gold', 'key'):
    path = out / (name + '.png')
    shutil.copy2(root / ('Assets/_Project/Art/UI/icon_' + name + '.png'), path)
    meta = Path(str(path) + '.meta')
    if not meta.exists():
        data = re.sub(r'^guid: .*', 'guid: ' + hashlib.md5(('shared-ui/' + path.name).encode()).hexdigest(), template, flags=re.M)
        data = re.sub(r'spriteBorder: .*', 'spriteBorder: {x: 0, y: 0, z: 0, w: 0}', data)
        meta.write_text(data)
