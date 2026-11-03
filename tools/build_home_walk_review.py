from pathlib import Path
from PIL import Image
import base64,io,json
root=Path(__file__).resolve().parents[1]
data={}
def add(key,path,size):
    image=Image.open(path).convert('RGBA');image.thumbnail(size)
    buf=io.BytesIO();image.save(buf,format='WEBP',quality=82)
    data[key]='data:image/webp;base64,'+base64.b64encode(buf.getvalue()).decode()
for name in ['sky','trees','ground']:add(name,root/f'artsrc/ui/home-walk-v1/{name}.png',(1400,500))
for unit in ['dead_common','human_legendary','elf_rare']:
    for i in range(15):add(unit+str(i),root/f'Assets/_Project/Art/UnitsSimple/{unit}/walk_{i:03}.png',(128,128))
for name in ['battle','gold','key','home','relics']:add(name,root/f'Assets/_Project/Resources/SharedUI/{name}.png',(90,90))
out=Path('/Users/vuhagiabao/.codex/visualizations/2026/09/30/01a0f0cc-8798-79b0-af42-83a55131c482/home-walking-loop.html')
out.write_text((root/'tools/home_walk_review.template.html').read_text().replace('__ASSETS__',json.dumps(data,separators=(',',':'))))
assert out.stat().st_size<1000000,out.stat().st_size
print(out,out.stat().st_size)
