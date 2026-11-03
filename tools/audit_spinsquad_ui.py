"""Read supplied XAPK UI/animation names for a qualified screen-gap audit.

No APK assets are imported into our game. Asset presence is evidence, not
proof that the corresponding feature is reachable in this package version.
"""
import io,json,re,zipfile
from pathlib import Path
import UnityPy

ROOT=Path(__file__).resolve().parents[1]
pattern=re.compile(r'ui|popup|panel|scene|screen|lobby|shop|gacha|roll|spin|dice|slot|chest|treasure|relic|quest|mission|daily|reward|achievement|chapter|stage|result|victory|defeat|setting|rank|guild|mail|summon|recruit|tutorial|collection|inventory|pass|roulette',re.I)
rows=[]; containers=[]
with zipfile.ZipFile('/Users/vuhagiabao/spin/Spin+Squad!_2.5.1_APKPure.xapk') as outer:
    with zipfile.ZipFile(io.BytesIO(outer.read('base_assets.apk'))) as apk:
        files=[i for i in apk.infolist() if i.filename.startswith('assets/bin/Data/')]
        for index,info in enumerate(files):
            try: env=UnityPy.load(apk.read(info))
            except Exception: continue
            for key in env.container:
                if pattern.search(key): containers.append({'path':key,'bundle':info.filename})
            for obj in env.objects:
                if obj.type.name not in ('GameObject','AnimationClip','MonoScript'): continue
                try:
                    d=obj.read(); name=d.m_Name
                except Exception: continue
                if not pattern.search(name): continue
                row={'name':name,'type':obj.type.name,'bundle':info.filename,'id':obj.path_id}
                if obj.type.name=='AnimationClip':
                    try: row['duration']=obj.read_typetree()['m_MuscleClip']['m_StopTime']
                    except Exception: pass
                rows.append(row)
            if index%500==0: print('scanned',index,'/',len(files),'matches',len(rows),flush=True)
out=ROOT/'outdata/reference/spinsquad_ui_inventory.json'
out.write_text(json.dumps({'source':'Spin+Squad! 2.5.1 supplied XAPK','containers':containers,'objects':rows},indent=2))
print('DONE',len(rows),'objects',len(containers),'paths',out,flush=True)
