"""Embed actual sprites into the review fragment; no generated replacement bodies."""
from pathlib import Path
from PIL import Image
import json,base64,io,hashlib
root=Path(__file__).resolve().parents[1]
data={'images':{},'clips':{}}
seen={}
def add(key,path,size=160):
 im=Image.open(path).convert('RGBA');im.thumbnail((size,size));buf=io.BytesIO();im.save(buf,format='WEBP',quality=78)
 data['images'][key]='data:image/webp;base64,'+base64.b64encode(buf.getvalue()).decode()
for race in ['human','elf','machine','dead','monster']:
 folder=root/f'Assets/_Project/Art/UnitsSimple/{race}_legendary'
 frames=json.loads((folder/'frames.json').read_text())['clips']['skill'];data['clips'][race]=[]
 for i,f in enumerate(frames):
  key=f'{race}{i}';path=folder/f['file'];im=Image.open(path)
  digest=hashlib.sha256(path.read_bytes()).hexdigest()
  if digest in seen: key=seen[digest]
  else: add(key,path,80);seen[digest]=key
  data['clips'][race].append(dict(key=key,pivot=f['pivot'],w=im.width,h=im.height))
for race,name in [('human','human-shockwave'),('elf','elf-frost-rain'),('machine','machine-shield'),('dead','dead-summon'),('monster','monster-roar')]:
 add(race+'Fx',root/f'artsrc/vfx/legend-skills-v4/{name}.png',480)
add('deadIdle',root/'Assets/_Project/Art/UnitsSimple/dead_legendary/idle_000.png')
for key,name in [('rollIcon','roll'),('battleIcon','battle'),('settingsIcon','settings'),('homeIcon','home'),('statsIcon','upgrade')]:
 add(key,root/f'Assets/_Project/Resources/SharedUI/{name}.png',64)
template=(root/'tools/skill_roll_review.template.html').read_text()
out=Path('/Users/vuhagiabao/.codex/visualizations/2026/09/30/01a0f0cc-8798-79b0-af42-83a55131c482/skill-roll-review.html')
out.write_text(template.replace('__ASSETS__',json.dumps(data,separators=(',',':'))))
assert out.stat().st_size<1000000
print(out, out.stat().st_size)
