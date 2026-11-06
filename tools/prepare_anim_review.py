#!/usr/bin/env python3
"""Reproducible PREVIEW extraction; never writes runtime Resources or Unity metadata."""
from pathlib import Path
from PIL import Image, ImageDraw
import json, shutil
ROOT=Path(__file__).resolve().parents[1]
OUT=ROOT/'artsrc/ui/anim-review-v1'; AS=OUT/'assets'; AS.mkdir(parents=True,exist_ok=True)
records=[]
def export(name,source,rect,size,white=False,fit=False):
    im=Image.open(ROOT/source).convert('RGBA').crop(rect)
    # White VFX carry brightness in alpha, no gray RGB contamination when tinted.
    if white:
        a=im.getchannel('A'); im=Image.new('RGBA',im.size,'white'); im.putalpha(a)
    if fit:
        b=im.getchannel('A').point(lambda a:255 if a>16 else 0).getbbox()
        if b: im=im.crop(b)
    im.thumbnail((int(size[0]*.88),int(size[1]*.88)),Image.Resampling.LANCZOS)
    canvas=Image.new('RGBA',size);canvas.alpha_composite(im,((size[0]-im.width)//2,(size[1]-im.height)//2))
    canvas.save(AS/(name+'.png'))
    a=canvas.getchannel('A'); records.append(dict(name=name,source=source,crop=rect,size=size,white=white,previewOnly=True,alphaRange=a.getextrema(),bounds=a.getbbox()))
export('orb','Assets/_Project/Resources/Summon/orb.png',(0,0,*Image.open(ROOT/'Assets/_Project/Resources/Summon/orb.png').size),(512,512),fit=True)
export('card','chat/check/051-card-back.png',(0,0,1060,1484),(400,560),fit=True)
for name,r in [('glow',(0,0,768,512)),('rays',(768,0,1536,512)),('sparkle',(0,512,768,1024)),('chest-light',(768,512,1536,1024))]:
    export(name,'chat/check/051-light-layers-v2.png',r,(512,512),white=True)
# Equal 6-cell source registration retained; do not fit each phase to identical bounds.
for i in range(6):
    export('burst-'+str(i),'chat/check/051-burst-v2.png',(round(i*2172/6),0,round((i+1)*2172/6),724),(512,512),white=True)
for name,r in [('chest-closed',(35,235,586,690)),('chest-lid',(635,96,1180,385)),('chest-body',(632,400,1180,730)),('shard',(1400,572,1555,735))]:
    # Exploded layers use same coordinate-to-pixel scale, no independent fitting.
    export(name,'chat/check/052-chest-anim.png',r,(600,400) if name=='chest-body' else (600,340) if name=='chest-lid' else (600,500) if name=='chest-closed' else (96,96))
for i,(x,y) in enumerate([(0,0),(512,0),(1024,0),(0,512)]):
    export('flash-'+str(i),'chat/check/053-levelup-anim.png',(x,y,x+512,y+512),(256,256),white=True)
export('arrow','chat/check/053-levelup-anim.png',(512,512,1024,1024),(256,256),fit=True)
export('swirl','chat/check/053-levelup-anim.png',(1024,512,1536,1024),(512,512),white=True)
(OUT/'manifest.json').write_text(json.dumps(dict(status='PREVIEW_ONLY_NOT_APPROVED_FOR_RUNTIME',issues=['Chest exploded layers not mechanically verified; demo does not certify hinge fit.','Generated glow gutters may contain neighboring halos; needs visual approval and final isolated export.','Burst source centers require final registration review.'],assets=records),indent=2))
# Real alpha on contrasting backgrounds; visual review evidence.
sheet=Image.new('RGB',(1200,800),'#101d34');d=ImageDraw.Draw(sheet)
for i,n in enumerate(['orb','chest-closed','arrow','glow','chest-light','swirl']):
    x=(i%3)*400;y=(i//3)*400
    d.rectangle((x,y,x+399,y+399),fill='#101d34' if i<3 else '#e8dfc9')
    im=Image.open(AS/(n+'.png'));im.thumbnail((330,330));sheet.paste(im,(x+(400-im.width)//2,y+30),im)
    d.text((x+20,y+375),n,fill='white' if i<3 else 'black')
sheet.save(ROOT/'chat/check/056-layers-proof.png')
print(f'{len(records)} preview layers → {OUT}')
