"""Read-only 48px icon audit; preserves every runtime asset."""
from pathlib import Path
from PIL import Image,ImageDraw,ImageFont
R=Path('Assets/_Project/Resources'); O=Path('chat/check')
folders=['StatIcons','SharedUI','HomeChrome','Badges','Summon','Shop','ButtonIcons','Glyphs','EffectBuffs/icons']
items=[]
for folder in folders:
 for p in sorted((R/folder).glob('*')):
  if p.suffix not in ('.png','.bytes'):continue
  try:im=Image.open(p).convert('RGBA')
  except Exception:continue
  items.append((str(p.relative_to(R)),im))
f=ImageFont.truetype(str(R/'Fonts/Fredoka-SemiBold.ttf'),14)
out=Image.new('RGBA',(1200,((len(items)+5)//6)*106+45),'#1b2440');d=ImageDraw.Draw(out)
d.text((14,10),'STAGE C / all icons at 48px visible bounds / existing assets only',font=f,fill='white')
for i,(name,im) in enumerate(items):
 x=10+(i%6)*200;y=45+(i//6)*106
 bbox=im.getchannel('A').point(lambda a:255 if a>40 else 0).getbbox()
 if bbox:im=im.crop(bbox)
 im.thumbnail((48,48),Image.Resampling.LANCZOS);out.alpha_composite(im,(x+(180-im.width)//2,y))
 a,b=name.rsplit('/',1);d.text((x,y+51),a,font=f,fill='#aec4e8');d.text((x,y+69),b,font=f,fill='white')
out.save(O/'043-icons-48px.png')
