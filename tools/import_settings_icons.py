"""Crop approved sheet cells without altering their antialiased alpha."""
from pathlib import Path
import hashlib
import re
import shutil
from PIL import Image

root = Path(__file__).resolve().parents[1]
out = root / 'Assets/_Project/Resources/SharedUI'
out.mkdir(parents=True, exist_ok=True)
sheet = Image.open(root / 'artsrc/ui/settings_icons_concept_v1.png').convert('RGBA')
template = (root / 'Assets/_Project/Art/UI/btn_blue.png.meta').read_text()
for i, name in enumerate(('settings', 'sound', 'vibration', 'resume', 'restart', 'home')):
    x, y = (i % 3) * 512, (i // 3) * 512
    # Keep the whole cell including all low-alpha edge pixels. No colour key.
    sheet.crop((x, y, x + 512, y + 512)).save(out / (name + '.png'))
for name in ('btn_blue', 'btn_primary'):
    shutil.copy2(root / f'Assets/_Project/Art/UI/{name}.png', out / (name + '.png'))
for path in out.glob('*.png'):
    meta = Path(str(path) + '.meta')
    if meta.exists():
        continue
    data = re.sub(r'^guid: .*', 'guid: ' + hashlib.md5(('shared-ui/' + path.name).encode()).hexdigest(), template, flags=re.M)
    if not path.stem.startswith('btn_'):
        data = re.sub(r'spriteBorder: .*', 'spriteBorder: {x: 0, y: 0, z: 0, w: 0}', data)
    meta.write_text(data)
print('Imported 6 pictograms and 2 explicit button backgrounds into Resources/SharedUI')
