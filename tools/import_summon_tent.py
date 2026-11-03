"""Import approved Summon art without modifying pixels or transparent alpha."""
from pathlib import Path
import hashlib
import re
import shutil

root = Path(__file__).resolve().parents[1]
source = root / 'artsrc/ui/summon-tent-v1'
target = root / 'Assets/_Project/Resources/Summon'
template = (target / 'orb.png.meta').read_text()
for name, filename in [('witch-tent', 'background.png'), ('orb', 'orb.png')]:
    dest = target / (name + '.png')
    shutil.copyfile(source / filename, dest)
    meta = Path(str(dest) + '.meta')
    if not meta.exists():
        guid = hashlib.md5(('summon-tent-v1/' + name).encode()).hexdigest()
        meta.write_text(re.sub(r'^guid: .*', 'guid: ' + guid, template, flags=re.M))
print('Imported witch tent background and simple oracle icon; existing GUIDs preserved.')
