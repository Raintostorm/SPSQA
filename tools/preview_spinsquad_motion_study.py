"""Local curve replay study. Excludes prefab hierarchy and slash VFX."""
from pathlib import Path
import json,math
import sys
import numpy as np
from scipy import ndimage
from PIL import Image,ImageDraw

root=Path(__file__).resolve().parents[1]
folder=root/'outdata/reference/spinsquad_motion_1005'
summary=json.loads((folder/'motion_summary.json').read_text())
ids={1959:'1005_idle_1',4282:'1005_idle_2',5013:'1005_idle_3',4525:'1005_walk',4278:'1005_attack_1',4119:'1005_attack_2',4230:'1005_attack_3',5409:'1005_dead'}
frames=[]; durations=[]
skeleton='--skeleton' in sys.argv
art=root/'artsrc/units_v4/spinsquad_motion_trial'
poses={}
if skeleton:
    for clip in ['idle','walk','attack','dead']:
        im=Image.open(art/f'dead_common_{clip}.png').convert('RGBA')
        lab,_=ndimage.label(np.array(im)[:,:,3]>32)
        boxes=[]
        for ident,sl in enumerate(ndimage.find_objects(lab),1):
            if sl is not None and (lab[sl]==ident).sum()>1000:
                y,x=sl; boxes.append((x.start,y.start,x.stop,y.stop))
        boxes.sort()
        assert len(boxes)=={'idle':3,'walk':2,'attack':3,'dead':2}[clip],(clip,boxes)
        poses[clip]=[im.crop(b) for b in boxes]
    target=150/poses['idle'][0].height
    for clip in poses:
        poses[clip]=[im.resize((round(im.width*target),round(im.height*target)),Image.Resampling.LANCZOS) for im in poses[clip]]
for name in ['idle','walk','attack','Dead']:
    c=summary[name]; original=json.loads((folder/f'1005_bandit_{name}.json').read_text())
    slots=[]
    for b in c['bindings']:
        width=1 if b['isPPtrCurve'] or b['typeID']!=4 else {1:3,2:4,3:3,4:3}.get(b['attribute'],1)
        slots.extend([(b,j) for j in range(width)])
    curves={}
    for f in c['streamed_frames']:
        if f['time']>100: continue
        for k in f['keys']: curves.setdefault(k['index'],[]).append((f['time'],k['coeff']))
    constants=c['constant']['data']
    def value(index,t):
        keys=curves.get(index)
        if not keys:
            ci=index-len(curves)
            return constants[ci] if 0<=ci<len(constants) else 0
        time,co=keys[0]
        for kt,kc in keys:
            if kt<=t: time,co=kt,kc
        dt=t-time if time>=0 else 0
        return ((co[0]*dt+co[1])*dt+co[2])*dt+co[3]
    clipframes=[]
    for step in range(math.ceil(c['duration']*30)):
        t=step/30; sx=sy=1.; rot=0.; sprite='1005_idle_1'
        for index,(b,j) in enumerate(slots):
            if b['path']!=898039781: continue
            v=value(index,t)
            if b['isPPtrCurve']:
                ps=original['m_ClipBindingConstant']['pptrCurveMapping']; ix=round(v)
                if 0<=ix<len(ps): sprite=ids.get(ps[ix]['m_PathID'],sprite)
            elif b['typeID']==4 and b['attribute']==3:
                if j==0:sx=v
                if j==1:sy=v
            elif b['typeID']==4 and b['attribute']==4 and j==2:rot=v
        if skeleton:
            if sprite=='1005_walk': im=poses['walk'][1]
            elif sprite=='1005_dead': im=poses['dead'][0] # upright death pose; original curve supplies fall, not a second rotation
            elif 'attack' in sprite: im=poses['attack'][int(sprite[-1])-1]
            else: im=poses['idle'][int(sprite[-1])-1]
            im=im.resize((max(1,round(im.width*abs(sx))),max(1,round(im.height*abs(sy)))),Image.Resampling.LANCZOS)
        else:
            im=Image.open(folder/(sprite+'.png')).convert('RGBA')
            im=im.resize((max(1,round(im.width*2.3*abs(sx))),max(1,round(im.height*2.3*abs(sy)))),Image.Resampling.NEAREST)
        im=im.rotate(rot,expand=True,resample=Image.Resampling.BICUBIC)
        canvas=Image.new('RGB',(360,260),(225,230,235)); canvas.paste(im,((360-im.width)//2,220-im.height),im)
        ImageDraw.Draw(canvas).text((12,10),name.upper()+' / local curve study',fill=(20,25,30))
        clipframes.append(canvas)
    hold=[33]*len(clipframes)
    if name=='Dead':hold[-1]=1500
    destination=art if skeleton else folder
    clipframes[0].save(destination/f'{name.lower()}_curve_study.gif',save_all=True,append_images=clipframes[1:],duration=hold,loop=0,disposal=2)
    frames.extend(clipframes);durations.extend(hold)
frames[0].save(destination/'full_curve_study.gif',save_all=True,append_images=frames[1:],duration=durations,loop=0,disposal=2)
