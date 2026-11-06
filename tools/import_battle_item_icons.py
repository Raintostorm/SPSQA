"""Cut isolated item sheet into equal padded transparent sprites; keep GUIDs."""
from pathlib import Path
from PIL import Image,ImageDraw
import re,uuid,json
R=Path(__file__).resolve().parents[1];src=Image.open(R/'artsrc/ui/battle-items-alpha-v1/source.png').convert('RGBA')
out=R/'Assets/_Project/Resources/BattleItems/Icons';out.mkdir(exist_ok=True)
template=(R/'Assets/_Project/Resources/HomeChrome/coin.png.meta').read_text()
preview=Image.new('RGBA',(1000,420),'#24374b');draw=ImageDraw.Draw(preview);manifest=[]
for i,name in enumerate(['kunai','mana_stone','twin_fang','runic_kunai','mana_heart']):
 x=(i%3)*512;y=(i//3)*512;im=src.crop((x,y,x+512,y+512))
 # Remove low-opacity generated halo; retain opaque black outline and edge antialias.
 a=im.getchannel('A').point(lambda v:0 if v<200 else min(255,(v-200)*255//54));im.putalpha(a)
 box=a.getbbox();im=im.crop(box);im.thumbnail((272,272),Image.Resampling.LANCZOS)
 canvas=Image.new('RGBA',(320,320));canvas.alpha_composite(im,((320-im.width)//2,(320-im.height)//2));p=out/(name+'.png');canvas.save(p)
 meta=p.with_suffix('.png.meta')
 if not meta.exists():
  s=re.sub(r'^guid: .*$', 'guid: '+uuid.uuid4().hex,template,flags=re.M);s=re.sub(r'spriteBorder: \{[^}]*\}','spriteBorder: {x: 0, y: 0, z: 0, w: 0}',s);meta.write_text(s)
 t=canvas.copy();t.thumbnail((180,180));preview.alpha_composite(t,(i*200+10,30));draw.text((i*200+10,220),name,fill='white')
 # A second light substrate proves absence of opaque navy rectangles.
 draw.rectangle((i*200+8,255,i*200+192,410),fill='#e5d8b0');t=canvas.copy();t.thumbnail((140,140));preview.alpha_composite(t,(i*200+30,262))
 manifest.append({'name':name,'sourceCell':[x,y,512,512],'bbox':box,'size':[320,320]})
folder=out.with_suffix('.meta')
if not folder.exists():folder.write_text('fileFormatVersion: 2\nguid: '+uuid.uuid4().hex+'\nfolderAsset: yes\nDefaultImporter:\n  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n')
preview.save(R/'chat/check/048-items-transparent-preview.png');(R/'artsrc/ui/battle-items-alpha-v1/manifest.json').write_text(json.dumps(manifest,indent=2))
