from pathlib import Path
import json,struct
from PIL import Image,ImageDraw

folder=Path(__file__).resolve().parents[1]/'outdata/reference/spinsquad_motion_1005'
ids={1959:'1005_idle_1',4282:'1005_idle_2',5013:'1005_idle_3',4525:'1005_walk',4278:'1005_attack_1',4119:'1005_attack_2',4230:'1005_attack_3',5409:'1005_dead'}
summary={}
for name in ['idle','walk','attack','Dead']:
    d=json.loads((folder/f'1005_bandit_{name}.json').read_text())
    data=d['m_MuscleClip']['m_Clip']['data']; stream=data['m_StreamedClip']
    raw=struct.pack('<'+'I'*len(stream['data']),*stream['data']); offset=0; frames=[]
    while offset<len(raw):
        time,count=struct.unpack_from('<fi',raw,offset); offset+=8; keys=[]
        for _ in range(count):
            index,*coeff=struct.unpack_from('<i4f',raw,offset); offset+=20
            keys.append({'index':index,'coeff':coeff})
        frames.append({'time':time,'keys':keys})
    # Bindings describe component slots; sprite slots follow transform slots.
    slots=[]
    for b in d['m_ClipBindingConstant']['genericBindings']:
        width=1 if b['isPPtrCurve'] or b['typeID']!=4 else {1:3,2:4,3:3,4:3}.get(b['attribute'],1)
        slots.extend([b]*width)
    pointers=d['m_ClipBindingConstant']['pptrCurveMapping']
    swaps=[]
    for f in frames:
        for k in f['keys']:
            if k['index']<len(slots) and slots[k['index']]['isPPtrCurve'] and f['time']>=0 and f['time']<100:
                val=round(k['coeff'][3]); pid=pointers[val]['m_PathID'] if val<len(pointers) else None
                swaps.append({'time':f['time'],'sprite':ids.get(pid,str(pid))})
    summary[name]={'duration':d['m_MuscleClip']['m_StopTime'],'sprite_keys':swaps,'streamed_frames':frames,'bindings':d['m_ClipBindingConstant']['genericBindings'],'constant':data.get('m_ConstantClip'),'dense':data['m_DenseClip']}
    print(name,'duration',summary[name]['duration'],'sprite keys',swaps)
    pictures=[Image.open(folder/(s['sprite']+'.png')).convert('RGBA') for s in swaps if (folder/(s['sprite']+'.png')).exists()]
    if pictures:
        strip=Image.new('RGBA',(len(pictures)*180,200))
        for i,im in enumerate(pictures):
            scale=120/max(im.size); im=im.resize((round(im.width*scale),round(im.height*scale)),Image.Resampling.NEAREST)
            strip.alpha_composite(im,(i*180+(180-im.width)//2,170-im.height))
        strip.save(folder/f'{name.lower()}_pose_reference.png')
(folder/'motion_summary.json').write_text(json.dumps(summary,indent=2))
