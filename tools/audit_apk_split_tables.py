import io,zipfile,re,json,collections,UnityPy
from pathlib import Path
out=Path('outdata/reference/apk-deep');out.mkdir(exist_ok=True)
report=[]
with zipfile.ZipFile('/Users/vuhagiabao/spin/Spin+Squad!_2.5.1_APKPure.xapk') as z:
 with zipfile.ZipFile(io.BytesIO(z.read('base_assets.apk'))) as a:
  names=a.namelist();roots=sorted(set(n.split('.split')[0] for n in names if re.search(r'(sharedassets\d+\.assets|globalgamemanagers\.assets|level\d+)(\.split\d+)?$',n)))
  for root in roots:
   parts=sorted([n for n in names if n.startswith(root+'.split')],key=lambda x:int(x.rsplit('split',1)[1]))
   b=b''.join(a.read(n) for n in parts) if parts else a.read(root)
   try:e=UnityPy.load(b)
   except Exception as ex:report.append({'file':root,'error':str(ex)});continue
   counts=collections.Counter(o.type.name for o in e.objects);print(root,len(b),dict(counts),flush=True)
   report.append({'file':root,'counts':dict(counts)})
   for o in e.objects:
    if o.type.name not in ('TextAsset','MonoBehaviour'):continue
    try:
     d=o.read();name=getattr(d,'m_Name','')
     if o.type.name=='TextAsset':
      s=d.m_Script
      if isinstance(s,str):s=s.encode('utf8','surrogateescape')
      fn=out/(root.rsplit('/',1)[1]+'_'+str(o.path_id)+'.bin');fn.write_bytes(s)
      report.append({'textasset':name,'path':str(fn),'size':len(s),'head':repr(s[:70])})
     elif re.search('table|resource|config|perbase|localiz',name,re.I):report.append({'mono':name,'tree':str(o.read_typetree())[:8000]})
    except Exception as ex:report.append({'object':o.path_id,'error':str(ex)[:100]})
(out/'inventory.json').write_text(json.dumps(report,indent=2))
