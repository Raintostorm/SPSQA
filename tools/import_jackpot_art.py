"""Import approved transparent castle Jackpot assets."""
from pathlib import Path
import hashlib,re
from PIL import Image
root=Path(__file__).resolve().parents[1]
template=(root/'Assets/_Project/Art/UnitsSimple/dead_legendary/skill_010.png.meta').read_text()
out=root/'Assets/_Project/Resources/Jackpot'
out.mkdir(parents=True,exist_ok=True)
def save(image,name):
    path=out/f'{name}.png'
    image.save(path)
    meta=re.sub(r'^guid: .*','guid: '+hashlib.md5(f'jackpot-castle-v3/{name}'.encode()).hexdigest(),template,flags=re.M)
    meta=re.sub(r'spritePixelsToUnits: .*','spritePixelsToUnits: 100',meta)
    meta=re.sub(r'spritePivot: .*','spritePivot: {x: 0.5, y: 0.5}',meta)
    meta=re.sub(r'spriteAlignment: .*','spriteAlignment: 9',meta)
    Path(str(path)+'.meta').write_text(meta)
for name,source in [('castle','jackpot-v3'),('ribbon','jackpot-v2')]:
    image=Image.open(root/f'artsrc/ui/{source}/{name}.png').convert('RGBA')
    image=image.crop(image.getchannel('A').point(lambda a:255 if a>15 else 0).getbbox())
    image.thumbnail((1024,640))
    save(image,name)
sheet=Image.open(root/'artsrc/ui/jackpot-v2/fireworks.png').convert('RGBA')
w,h=sheet.size
for i in range(6):
    save(sheet.crop((i%3*w//3,i//3*h//2,(i%3+1)*w//3,(i//3+1)*h//2)),f'fireworks_{i}')
print('Imported castle, ribbon and 6 fireworks frames')
