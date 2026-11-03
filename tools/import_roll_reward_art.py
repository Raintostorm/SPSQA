from pathlib import Path
from PIL import Image
import hashlib,re
root=Path(__file__).resolve().parents[1]
template=(root/'Assets/_Project/Art/UnitsSimple/dead_legendary/skill_010.png.meta').read_text()
for name in ['coin','buff','recruit','slot-flash']:
 sheet=Image.open(root/f'artsrc/ui/roll-rewards-v1/{name}.png').convert('RGBA')
 out=root/f'Assets/_Project/Resources/RollRewards/{name}';out.mkdir(parents=True,exist_ok=True)
 for i in range(6):
  path=out/f'frame_{i}.png';sheet.crop((i%3*512,i//3*512,(i%3+1)*512,(i//3+1)*512)).save(path)
  meta=re.sub(r'^guid: .*','guid: '+hashlib.md5(f'roll-reward-v1/{name}/{i}'.encode()).hexdigest(),template,flags=re.M)
  meta=re.sub(r'spritePixelsToUnits: .*','spritePixelsToUnits: 512',meta)
  meta=re.sub(r'spritePivot: .*','spritePivot: {x: 0.5, y: 0.5}',meta)
  meta=re.sub(r'spriteAlignment: .*','spriteAlignment: 9',meta)
  Path(str(path)+'.meta').write_text(meta)
 print(name,'6 sprites')
