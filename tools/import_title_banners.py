"""Crop approved sheet without redrawing; preserve alpha and existing Unity GUIDs."""
from pathlib import Path
import json, re, shutil, uuid
from PIL import Image, ImageDraw, ImageFont
ROOT = Path(__file__).resolve().parents[1]
source = ROOT / 'chat/check/001-title-banners.png'
out = ROOT / 'Assets/_Project/Resources/TitleBanners'
archive = ROOT / 'artsrc/ui/title-banners-v1'
out.mkdir(parents=True, exist_ok=True); archive.mkdir(parents=True, exist_ok=True)
im = Image.open(source).convert('RGBA')
a = im.getchannel('A')
rows = [y for y in range(im.height) if sum(v > 128 for v in a.crop((0,y,im.width,y+1)).getdata()) > 200]
groups=[]
for y in rows:
    if not groups or y > groups[-1][-1]+1: groups.append([])
    groups[-1].append(y)
groups=[g for g in groups if len(g)>100]
assert len(groups)==3, groups
base=(ROOT/'Assets/_Project/Art/UI/btn_blue.png.meta').read_text()
manifest={'source':'source.png','assets':[]}
for name,ys in zip(['navy-ribbon','purple-ribbon','navy-plaque'],groups):
    strip=im.crop((0,max(0,ys[0]-2),im.width,min(im.height,ys[-1]+3)))
    bbox=strip.getchannel('A').point(lambda v:255 if v>128 else 0).getbbox()
    crop=strip.crop(bbox)
    # Fixed end caps preserve ribbon folds; long central section stretches horizontally.
    border=(260,26,260,26) if 'ribbon' in name else (70,38,70,38)
    p=out/(name+'.png');crop.save(p)
    mp=Path(str(p)+'.meta')
    if not mp.exists():
        meta=re.sub(r'^guid: .*$', 'guid: '+uuid.uuid4().hex, base, flags=re.M)
        meta=re.sub(r'spriteBorder: \{[^}]+\}', 'spriteBorder: {x: %d, y: %d, z: %d, w: %d}'%border,meta)
        mp.write_text(meta)
    manifest['assets'].append({'name':name,'size':list(crop.size),'border_left_bottom_right_top':border,'source_bbox':[bbox[0],max(0,ys[0]-2)+bbox[1],bbox[2],max(0,ys[0]-2)+bbox[3]]})
folder=Path(str(out)+'.meta')
if not folder.exists():folder.write_text('fileFormatVersion: 2\nguid: '+uuid.uuid4().hex+'\nfolderAsset: yes\nDefaultImporter:\n  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n')
shutil.copy2(source,archive/'source.png')
(archive/'crop-manifest.json').write_text(json.dumps(manifest,indent=2)+'\n')
# Actual cropped assets displayed at normal/wider width, using the same horizontal slicing.
canvas=Image.new('RGBA',(1125,900),(32,38,51,255));draw=ImageDraw.Draw(canvas)
font=ImageFont.truetype(str(ROOT/'Assets/_Project/Resources/Fonts/LilitaOne-Regular.ttf'),38)
for i,entry in enumerate(manifest['assets']):
    art=Image.open(out/(entry['name']+'.png')); w,h=art.size;l,b,r,t=entry['border_left_bottom_right_top'];height=108;scale=height/h
    for j,width in enumerate([680,1020]):
        dest=Image.new('RGBA',(width,height));cap=round(l*scale);rcap=round(r*scale)
        for srcbox,box in [((0,0,l,h),(0,0,cap,height)),((l,0,w-r,h),(cap,0,width-rcap,height)),((w-r,0,w,h),(width-rcap,0,width,height))]:
            dest.alpha_composite(art.crop(srcbox).resize((box[2]-box[0],height),Image.Resampling.LANCZOS),(box[0],0))
        x=(1125-width)//2;y=20+i*290+j*130;canvas.alpha_composite(dest,(x,y))
        text=['SUMMON','CHOOSE A BUFF','DAILY SHOP'][i];draw.text((562,y+43),text,font=font,anchor='mm',fill='white',stroke_width=3,stroke_fill='black')
canvas.convert('RGB').save(ROOT/'chat/check/001-title-banners-sliced-preview.png')
print(json.dumps(manifest,indent=2))
