from pathlib import Path
from PIL import Image
import hashlib,re,shutil
root=Path(__file__).resolve().parents[1]
out=root/'Assets/_Project/Resources/ChapterSelect';out.mkdir(parents=True,exist_ok=True)
template=(root/'Assets/_Project/Resources/SharedUI/battle.png.meta').read_text()
def save(im,name):
    p=out/f'{name}.png';im.save(p)
    meta=re.sub(r'^guid: .*','guid: '+hashlib.md5(f'chapter-select-v1/{name}'.encode()).hexdigest(),template,flags=re.M)
    Path(str(p)+'.meta').write_text(meta)
save(Image.open(root/'artsrc/ui/chapter-select-v1/banner.png'),'banner')
sheet=Image.open(root/'artsrc/ui/chapter-select-v1/icons.png').convert('RGBA');w,h=sheet.size
for i,name in enumerate(['rewards','codex','missions']):save(sheet.crop((i*w//3,0,(i+1)*w//3,h)),name)
print('Imported chapter banner and 3 icons')
