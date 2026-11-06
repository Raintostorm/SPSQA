"""Install approved gallery and rarity vault walls; never overwrite existing GUIDs."""
from pathlib import Path
from PIL import Image
import uuid,re,json
ROOT=Path(__file__).resolve().parents[1]
out=ROOT/'Assets/_Project/Resources/RelicVault';out.mkdir(exist_ok=True)
template=(ROOT/'Assets/_Project/Resources/HomeChrome/coin.png.meta').read_text()
def install(name,im,border):
 p=out/(name+'.png')
 if p.exists():
  backup=ROOT/'artsrc/ui/retired-2026-11-04/relic-vault';backup.mkdir(parents=True,exist_ok=True)
  if not (backup/p.name).exists():(backup/p.name).write_bytes(p.read_bytes())
 im.save(p)
 meta=p.with_suffix('.png.meta')
 if not meta.exists():
  s=re.sub(r'^guid: .*$', 'guid: '+uuid.uuid4().hex,template,flags=re.M)
  s=re.sub(r'spriteBorder: \{[^}]*\}', 'spriteBorder: {x: %s, y: %s, z: %s, w: %s}'%border,s)
  meta.write_text(s)
wall=Image.open(ROOT/'artsrc/ui/relic-vault-v1/walls-source.png').convert('RGBA')
for i,name in enumerate(['common','rare','epic','legendary']):
 install(name,wall.crop((round(i*wall.width/4),0,round((i+1)*wall.width/4),wall.height)),(65,105,65,105))
# User revision046: matte aged brass, same runtime dimensions and GUID.
matte=ROOT/'artsrc/ui/relic-vault-v1/legendary-matte-source.png'
if matte.exists():
 install('legendary',Image.open(matte).convert('RGBA').resize((round(wall.width/4),wall.height),Image.Resampling.LANCZOS),(65,105,65,105))
gallery=Image.open(ROOT/'chat/check/044-relic-gallery-demo.png').convert('RGBA')
install('gallery',gallery.crop((0,185,852,456)),(0,0,0,0))
folder=out.with_suffix('.meta')
if not folder.exists():folder.write_text('fileFormatVersion: 2\nguid: '+uuid.uuid4().hex+'\nfolderAsset: yes\nDefaultImporter:\n  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n')
(ROOT/'artsrc/ui/relic-vault-v1/manifest.json').write_text(json.dumps({'source':wall.size,'panels':['common','rare','epic','legendary'],'galleryCrop':[0,185,852,456],'border':[65,105,65,105]},indent=2))
