"""Copy approved Home layers into Resources; preserve the generated RGBA pixels."""
from pathlib import Path
import shutil,re,hashlib
root=Path(__file__).resolve().parents[1]
out=root/'Assets/_Project/Resources/HomeWalk'
out.mkdir(parents=True,exist_ok=True)
template=(root/'Assets/_Project/Resources/SharedUI/battle.png.meta').read_text()
for name in ['sky','trees','ground']:
    dest=out/f'{name}.png'
    shutil.copyfile(root/f'artsrc/ui/home-walk-v1/{name}.png',dest)
    meta=re.sub(r'^guid: .*','guid: '+hashlib.md5(f'home-walk-v1/{name}'.encode()).hexdigest(),template,flags=re.M)
    Path(str(dest)+'.meta').write_text(meta)
print('Imported 3 Home layers without modifying source pixels')
