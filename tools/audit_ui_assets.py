"""Read-only UI art review. Never rewrites runtime assets or Unity meta."""
from pathlib import Path
from PIL import Image, ImageDraw, ImageFont
import hashlib,json
ROOT=Path(__file__).resolve().parents[1];R=ROOT/'Assets/_Project/Resources';O=ROOT/'chat/check'
font=ImageFont.truetype(str(R/'Fonts/Fredoka-SemiBold.ttf'),22)
small=ImageFont.truetype(str(R/'Fonts/Fredoka-SemiBold.ttf'),17)
rows=[
('Restart','SharedUI/restart.png','Glyphs/replay.png','REUSE replay; no generation'),
('Resume','SharedUI/resume.png','SharedUI/resume.png','KEEP for review: outlined play triangle'),
('Sound','SharedUI/sound.png','SharedUI/sound.png','KEEP for review: speaker + waves'),
('Vibration','SharedUI/vibration.png','SharedUI/vibration.png','PHONE: design decision; no bell yet'),
('Home','SharedUI/home.png','Glyphs/home.png','DIFFERENT meaning: tent / navigation home'),
('Battle','SharedUI/battle.png','ButtonIcons/battle.png','SAME action; white version exists'),
('Close','SharedUI/close.png','Glyphs/close.png','SAME action; white version exists'),
('Upgrade','SharedUI/upgrade.png','ButtonIcons/upgrade.png','EXISTING alternatives; no generation'),
('Key','SharedUI/key.png','HomeChrome/key.png','Two designs: unify at usage if needed'),
('Wallet gold','SharedUI/gold.png','HomeChrome/coin.png','BAG vs COIN; code alignment, not new art'),
('Shop package','Shop/gold.png','ShopPacks/gold-large.png','VALID distinct role: package amount'),
]
W=1220;rh=140;im=Image.new('RGBA',(W,100+len(rows)*rh),(28,35,53,255));d=ImageDraw.Draw(im)
d.text((20,12),'ASSET AUDIT - existing files only / no runtime changes',font=font,fill='white')
d.text((220,53),'CURRENT 112px | 48px',font=small,fill='#b6c9ee');d.text((590,53),'REUSE / COMPARISON 112px | 48px',font=small,fill='#b6c9ee')
manifest=[]
for i,(title,left,right,note) in enumerate(rows):
 y=100+i*rh;d.rectangle((10,y,W-10,y+rh-6),fill=(39,48,67,255));d.text((20,y+10),title,font=font,fill='white')
 for path,x in [(left,230),(right,620)]:
  src=Image.open(R/path).convert('RGBA')
  # Crop transparent margin only for fair visible-size legibility comparison.
  alpha=src.getchannel('A').point(lambda v:255 if v>40 else 0);bbox=alpha.getbbox();src=src.crop(bbox)
  for size,dx in [(112,0),(48,140)]:
   t=src.copy();t.thumbnail((size,size),Image.Resampling.LANCZOS);im.alpha_composite(t,(x+dx+(size-t.width)//2,y+5+(112-t.height)//2))
 d.text((810,y+12),note,font=small,fill='#e8d696')
 d.text((215,y+119),left,font=small,fill='#b6c9ee');d.text((605,y+119),right,font=small,fill='#b6c9ee')
 manifest.append({'topic':title,'current':left,'comparison':right,'finding':note})
im.convert('RGB').save(O/'042-assets-audit.png')
# Existing result banners, crop using actual ResultScreenArt Unity rects (bottom origin).
a=Image.open(R/'ResultScreen/atlas.png').convert('RGBA');p=Image.new('RGBA',(1125,440),(28,35,53,255));dr=ImageDraw.Draw(p)
for i,(x,y,w,h) in enumerate([(29,668,708,214),(793,669,707,216)]):
 t=a.crop((x,a.height-y-h,x+w,a.height-y));t.thumbnail((520,230),Image.Resampling.LANCZOS);p.alpha_composite(t,(20+i*560,75));dr.text((25+i*560,20),['EXISTING VICTORY','EXISTING DEFEAT'][i],font=font,fill='white')
dr.text((25,335),'Already loaded by ResultPanel -> ResultScreenArt.Get(0 / 1)',font=font,fill='white');dr.text((25,380),'New result ribbons are optional, not missing assets.',font=font,fill='#e8d696');p.convert('RGB').save(O/'042-existing-result-banners.png')
(O/'042-assets-audit.json').write_text(json.dumps(manifest,indent=2)+'\n')
print('Saved audit/comparison and existing-banner proof. No runtime asset writes.')
