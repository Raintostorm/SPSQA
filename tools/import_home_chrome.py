from pathlib import Path
from PIL import Image
import re,hashlib
root=Path(__file__).resolve().parents[1]
sheet=Image.open(root/'artsrc/ui/home-chrome-v1/icons.png').convert('RGBA')
w,h=sheet.size
template=(root/'Assets/_Project/Resources/SharedUI/battle.png.meta').read_text()
out=root/'Assets/_Project/Resources/HomeChrome';out.mkdir(parents=True,exist_ok=True)
for i,name in enumerate(['shop','recruit','mercenaries','relics','coin','key']):
    im=sheet.crop((i%3*w//3,i//3*h//2,(i%3+1)*w//3,(i//3+1)*h//2))
    path=out/f'{name}.png';im.save(path)
    meta=re.sub(r'^guid: .*','guid: '+hashlib.md5(f'home-chrome-v1/{name}'.encode()).hexdigest(),template,flags=re.M)
    Path(str(path)+'.meta').write_text(meta)
print('Imported 6 Home icons')
