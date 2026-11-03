"""Extract supplied-package sprites/clips for local motion study only."""
from pathlib import Path
import json,re,sys
import UnityPy

src=Path(sys.argv[1]); out=Path(sys.argv[2]); out.mkdir(parents=True,exist_ok=True)
seen=set()
for f in sorted(src.iterdir()):
    if not f.is_file(): continue
    try: env=UnityPy.load(str(f))
    except Exception: continue
    hits=[]
    for o in env.objects:
        if o.type.name not in ('Sprite','AnimationClip','GameObject'): continue
        try: d=o.read(); name=d.m_Name
        except Exception: continue
        if o.type.name=='Sprite' and re.match(r'^1005_(idle|walk|attack|dead)(?:_\d+)?$',name) and name not in seen:
            try:
                d.image.save(out/(name+'.png')); seen.add(name)
                print('SPRITE',name,f.name,flush=True)
            except Exception as e: print('ERROR',name,str(e),flush=True)
        if o.type.name=='AnimationClip' and re.search(r'1005|human.*common|common.*human',name,re.I):
            hits.append((name,o))
        if o.type.name=='GameObject' and re.search(r'human.*common|common.*human',name,re.I):
            print('GAMEOBJECT',name,f.name,flush=True)
    for name,o in hits:
        try:
            (out/(re.sub(r'[^\w.-]','_',name)+'.json')).write_text(json.dumps(o.read_typetree(),indent=2,default=str))
            print('CLIP',name,f.name,flush=True)
        except Exception as e: print('CLIP_ERROR',name,str(e),flush=True)
    if len(seen)>=8 and hits: break
print('DONE',sorted(seen),flush=True)
