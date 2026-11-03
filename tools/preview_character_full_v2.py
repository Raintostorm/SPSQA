"""Complete pose extents; one scale, no fixed square source slicing."""
from pathlib import Path
import json
import sys
import re
import numpy as np
from PIL import Image, ImageDraw
from scipy import ndimage

folder=Path(__file__).resolve().parents[1]/'artsrc/units_v2'
names=['idle','walk','attack','dead']
chibi='--chibi' in sys.argv
unit_name='human_common' if '--human' in sys.argv else 'dead_common'
if '--unit' in sys.argv:
    unit_name=sys.argv[sys.argv.index('--unit')+1]
asset=(folder.parents[1]/f'Assets/_Project/Data/Units/Anim_{unit_name}.asset').read_text() if chibi else ''
tag='chibi' if chibi else 'handfix'
data={}
for name in names:
    path=folder/f'{unit_name}_runtime_{name}_chibi_candidate.png' if chibi else folder/f'{unit_name}_{name}_handfix.png'
    if '--attack-v2' in sys.argv and name=='attack':
        path=folder/f'{unit_name}_runtime_attack_chibi_hand_v2.png'
    im=Image.open(path).convert('RGBA')
    a=np.array(im)
    labels,_=ndimage.label(a[...,3]>32)
    boxes=[]; ids=[]
    for i,sl in enumerate(ndimage.find_objects(labels),1):
        if sl is not None and (labels[sl]==i).sum()>1000:
            y,x=sl; boxes.append(((x.start,y.start,x.stop,y.stop),i))
    boxes.sort(); ids=[v[1] for v in boxes]; boxes=[v[0] for v in boxes]
    assert len(boxes)==8,(name,len(boxes))
    nearest=ndimage.distance_transform_edt(labels==0,return_distances=False,return_indices=True)
    owners=labels[tuple(nearest)]
    data[name]=(im,boxes,owners,ids)

# Shared scale for all clips; attack weapon arc must not shrink entire figure.
reference_h=np.median([b[3]-b[1] for b in data['walk'][1]])
scale=150/reference_h
previews={}
clip_times={}
metrics={}
for name,(im,boxes,owners,ids) in data.items():
    frames=[]
    for b,ident in zip(boxes,ids):
        x0,y0,x1,y1=b
        # Use the centre of bottom foot-region to anchor standing poses. Dead
        # keeps full corpse extent visible, rather than normalising its height.
        a=np.array(im)
        if name!='dead':
            region=a[max(y0,y1-12):y1,x0:x1,3]>128
            ys,xs=np.where(region)
            anchor=x0+float(np.median(xs)) if len(xs) else (x0+x1)/2
        else:
            anchor=(x0+x1)/2
        cx0=max(0,x0-8); cy0=max(0,y0-8)
        cx1=min(im.width,x1+8); cy1=min(im.height,y1+8)
        # Adjacent extended poses overlap in x, so a rectangle alone would
        # capture neighbouring limbs. Ownership separates complete components;
        # alpha values remain unmodified for each figure, including AA edges.
        pixels=np.array(im)[cy0:cy1,cx0:cx1].copy()
        pixels[owners[cy0:cy1,cx0:cx1]!=ident,3]=0
        crop=Image.fromarray(pixels)
        crop=crop.resize((round(crop.width*scale),round(crop.height*scale)),Image.Resampling.LANCZOS)
        frame=Image.new('RGBA',(360,260),(230,235,240,255))
        px=round(180-(anchor-cx0)*scale)
        py=round(220-(y1-cy0)*scale)
        assert px>=0 and py>=0 and px+crop.width<=360 and py+crop.height<=260,(name,b)
        frame.alpha_composite(crop,(px,py))
        ImageDraw.Draw(frame).text((12,10),name.upper(),fill=(35,40,50,255))
        frames.append(frame.convert('RGB'))
    previews[name]=frames
    durations=[120]*8 if name=='idle' else [100]*8
    if name=='attack': durations=[160,100,100,70,80,100,120,180]
    if name=='dead': durations=[150,120,120,120,120,120,160,1500]
    if chibi:
        block=re.search(r'^  '+name.capitalize()+r':\n(.*?)(?=^  \w+:|\Z)',asset,re.M|re.S)[1]
        ds=re.search(r'    Durations:\n(.*?)    Sequence:',block,re.S)[1]
        durations=[round(float(v)*1000) for v in re.findall(r'- ([\d.]+)',ds)]
        assert len(durations)==8
    clip_times[name]=durations.copy()
    if name=='dead': durations[-1]=2000
    output_tag=tag+'_hand_v2' if '--attack-v2' in sys.argv else tag
    frames[0].save(folder/f'{unit_name}_{name}_{output_tag}_preview.gif',save_all=True,append_images=frames[1:],duration=durations,loop=0,disposal=2)
    metrics[name]={'boxes':boxes,'feet_spread':max(b[3] for b in boxes)-min(b[3] for b in boxes)}

combined=[]; timing=[]
for name,repeats in [('idle',2),('walk',3),('attack',2),('dead',1)]:
    for _ in range(repeats):
        combined.extend(previews[name]); timing.extend(clip_times[name])
timing[-1]=2000
combined[0].save(folder/f'{unit_name}_full_{output_tag}_preview.gif',save_all=True,append_images=combined[1:],duration=timing,loop=0,disposal=2)
(folder/f'{unit_name}_full_{output_tag}_metrics.json').write_text(json.dumps(metrics,indent=2)+'\n')
print(json.dumps(metrics))
