"""Read-only APK audit: print relic/localization evidence, never import runtime art."""
import io,re,zipfile,collections
import UnityPy

with zipfile.ZipFile('/Users/vuhagiabao/spin/Spin+Squad!_2.5.1_APKPure.xapk') as outer:
    with zipfile.ZipFile(io.BytesIO(outer.read('base_assets.apk'))) as apk:
        files=[n for n in apk.namelist() if re.fullmatch(r'assets/bin/Data/[0-9a-f]{32}',n)]
        print('Bundles to inspect:',len(files),flush=True)
        for index,name in enumerate(files):
            try: env=UnityPy.load(apk.read(name))
            except Exception:continue
            for obj in env.objects:
                if obj.type.name not in ['TextAsset','MonoBehaviour','MonoScript']:continue
                try:
                    data=obj.read();title=getattr(data,'m_Name','')
                    if obj.type.name=='TextAsset':
                        content=getattr(data,'m_Script','')
                        if isinstance(content,bytes):content=content.decode('utf-8',errors='replace')
                        if re.search(r'Half Gloves|Werewolf Coat|POS Device|Critical Chance|equipped mercenary|relic|artifact',content,re.I) or re.search(r'localiz|language|relic|artifact|item',title,re.I):
                            print('\nTEXT',name,title,'length',len(content),flush=True)
                            for hit in re.finditer(r'.{0,120}(?:Half Gloves|Werewolf Coat|POS Device|Critical Chance|equipped mercenary|relic|artifact).{0,250}',content,re.I):print(hit.group(0),flush=True)
                    elif re.search(r'relic|artifact|treasure|localiz|language|equipment|craft|itemdetail',title,re.I):
                        print('\nOBJECT',name,obj.type.name,title,flush=True)
                        if obj.type.name=='MonoBehaviour':
                            try:print(str(obj.read_typetree())[:12000],flush=True)
                            except Exception as exc:print(type(exc).__name__,flush=True)
                except Exception:pass
            if index%100==0:print('Scanned',index,flush=True)
