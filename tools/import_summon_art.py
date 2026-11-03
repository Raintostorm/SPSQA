from pathlib import Path
from PIL import Image
import re,hashlib
root=Path(__file__).resolve().parents[1]
sheet=Image.open(root/'artsrc/ui/summon-v2/icons.png').convert('RGBA');w,h=sheet.size
out=root/'Assets/_Project/Resources/Summon';out.mkdir(parents=True,exist_ok=True)
template=(root/'Assets/_Project/Resources/SharedUI/battle.png.meta').read_text()
for i,name in enumerate(['orb','gem']):
    path=out/f'{name}.png';sheet.crop((i*w//2,0,(i+1)*w//2,h)).save(path)
    meta=re.sub(r'^guid: .*','guid: '+hashlib.md5(f'summon-v2/{name}'.encode()).hexdigest(),template,flags=re.M)
    Path(str(path)+'.meta').write_text(meta)
print('Imported empty oracle ball and gem')
