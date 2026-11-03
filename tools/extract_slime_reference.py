"""Extract original supplied-APK sprites for reference only, never runtime import."""
import io
import json
import re
import struct
import zipfile
from pathlib import Path
import UnityPy

ROOT = Path(__file__).resolve().parents[1]
OUT = ROOT / 'outdata/reference/slimes-original'
OUT.mkdir(parents=True, exist_ok=True)
with zipfile.ZipFile('/Users/vuhagiabao/spin/Spin+Squad!_2.5.1_APKPure.xapk') as outer:
    with zipfile.ZipFile(io.BytesIO(outer.read('base_assets.apk'))) as apk:
        prefix = 'assets/bin/Data/'
        parts = sorted((n for n in apk.namelist() if re.search(r'sharedassets0\.assets\.split\d+$', n)), key=lambda n: int(n.rsplit('split', 1)[1]))
        env = UnityPy.Environment()
        env.load_file(b''.join(apk.read(n) for n in parts), name='sharedassets0.assets')
        env.load_file(apk.read(prefix+'sharedassets0.resource'), name='sharedassets0.resource')
        rows = []
        for obj in env.objects:
            if obj.type.name != 'Sprite':
                continue
            sprite = obj.read()
            name = sprite.m_Name
            if re.search(r'slime|mon_20100[1-4]_|mon_20101[12]_|mon_20200[12]_', name, re.I) or obj.path_id in [1825,3272,2679,1922]:
                try:
                    dest = OUT / (re.sub(r'[^\w.-]', '_', name) + '_' + str(obj.path_id) + '.png')
                    sprite.image.save(dest)
                    tree = obj.read_typetree()
                    rows.append({'name':name,'id':obj.path_id,'file':dest.name,'rect':tree['m_Rect'],'pivot':tree['m_Pivot'],'offset':tree['m_RD']['textureRectOffset']})
                    print(name, obj.path_id, dest.name, flush=True)
                except Exception as exc:
                    print('ERROR', name, str(exc)[:200], flush=True)
        (OUT/'inventory.json').write_text(json.dumps(rows,indent=2))
        clipenv = UnityPy.load(apk.read(prefix+'c8d660a9d3f6b3c4b95e8dbb8ee63a26'))
        for obj in clipenv.objects:
            if obj.type.name != 'AnimationClip':
                continue
            tree = obj.read_typetree()
            raw = tree['m_MuscleClip']['m_Clip']['data']['m_StreamedClip']['data']
            raw = struct.pack('<'+'I'*len(raw), *raw)
            offset = 0
            keys = []
            while offset < len(raw):
                time,count = struct.unpack_from('<fi',raw,offset)
                offset += 8
                for _ in range(count):
                    index,*coeff = struct.unpack_from('<i4f',raw,offset)
                    offset += 20
                    if 0 <= time < 10:
                        keys.append({'time':time,'index':index,'coeff':coeff})
            ids = [p['m_PathID'] for p in tree['m_ClipBindingConstant']['pptrCurveMapping']]
            result = {'duration':tree['m_MuscleClip']['m_StopTime'],'keys':keys,'sprites':[next(r for r in rows if r['id']==pid) for pid in ids]}
            (OUT/'attack-data.js').write_text('const originalAttack='+json.dumps(result)+';')
            (OUT/'201001-attack.json').write_text(json.dumps(tree,indent=2))
