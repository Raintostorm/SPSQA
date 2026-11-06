"""Conservative retirement audit: code literals + dynamic loader prefixes + serialized GUIDs. Moves nothing."""
from pathlib import Path
import re
R=Path(__file__).resolve().parents[1];res=R/'Assets/_Project/Resources'
roots=['UiRefresh','Buttons','ButtonFamilies','SummonKit']
files=[]
for folder in roots:
 if (res/folder).exists():files.extend(p for p in (res/folder).rglob('*') if p.is_file() and p.suffix!='.meta')
files.extend(p for p in (res/'SharedUI').glob('*') if p.is_file() and p.suffix!='.meta' and (p.stem.startswith('btn_') or p.stem in ['gold','key','buff_speed']))
scripts=[(p,list(enumerate(p.read_text(errors='replace').splitlines(),1))) for p in (R/'Assets').rglob('*.cs')]
serialized=[(p,p.read_text(errors='replace')) for p in (R/'Assets').rglob('*') if p.suffix in ['.unity','.prefab','.asset']]
rows=[];details=[]
for p in sorted(set(files)):
 rel=p.relative_to(res).with_suffix('').as_posix();folder=rel.rsplit('/',1)[0]+'/'
 refs=[]
 for code,lines in scripts:
  for n,line in lines:
   # Exact key, dynamic loader prefix, or helper slug when a generic folder loader exists.
   if rel in line or ('"'+folder+'"') in line or (folder in ['UiRefresh/','SharedUI/'] and ('"'+p.stem+'"') in line):refs.append(f'{code.relative_to(R)}:{n}: {line.strip()}')
 meta=p.with_suffix(p.suffix+'.meta');m=re.search(r'^guid: (\w+)',meta.read_text(),re.M) if meta.exists() else None
 hits=[str(q.relative_to(R)) for q,t in serialized if m and m[1] in t]
 rows.append(f'| `{p.relative_to(R)}` | {len(refs)} | {len(hits)} |')
 details.append('### '+str(p.relative_to(R))+'\n'+('\n'.join('- `'+x.replace('`','')+'`' for x in refs) if refs else '- Không tìm thấy chỗ mã theo phép dò bảo thủ.')+'\n'+('\n'.join('- GUID: `'+x+'`' for x in hits) if hits else '- 0 serialized GUID hits.')+'\n')
text='# D1 — chỉ báo cáo, không dời/xóa\n\n2026-11-05. Số chỗ mã là số dòng ứng viên (bao gồm helper/dynamic-prefix/comment), KHÔNG phải số runtime uses. Mã>0 cần Claude đọc callsite; không tự kết luận asset chết. Serialized GUID quét toàn Assets .unity/.prefab/.asset. Cả hai bằng0 chỉ là ứng viên, còn tools/ và Resources load ghép tên cần kiểm riêng trước dời.\n\n| Đường dẫn | Số chỗ mã (bảo thủ) | Số file chứa GUID |\n|---|---:|---:|\n'+'\n'.join(rows)+'\n\n## Bằng chứng file:dòng\n\n'+'\n'.join(details)
(R/'chat/check/049-cleanup.md').write_text(text)
print(f'{len(rows)} assets; no files moved. Report chat/check/049-cleanup.md')
