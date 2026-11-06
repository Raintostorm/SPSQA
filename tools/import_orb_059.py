#!/usr/bin/env python3
"""Import INPUT059 only after explicit approval. Preserve existing orb-core GUID."""
from pathlib import Path
import argparse,json,shutil,uuid
R=Path(__file__).resolve().parents[1];W=R/'artsrc/ui/summon-orb-059';dst=R/'Assets/_Project/Resources/Anim/Summon'
p=argparse.ArgumentParser();p.add_argument('--apply',action='store_true');p.add_argument('--approval-receipt',type=Path);a=p.parse_args();m=json.loads((W/'manifest.json').read_text())
if a.apply:
 assert a.approval_receipt and a.approval_receipt.exists(),'Need genuine user approval before overwrite'
 receipt=json.loads(a.approval_receipt.read_text());assert receipt.get('approved_by')=='user' and receipt.get('message'),'Record user actual approval'
 dst.mkdir(parents=True,exist_ok=True)
 for r in m['files']:
  src=W/'staged'/(r['slug']+'.png');target=dst/src.name;meta=Path(str(target)+'.meta');old=meta.read_bytes() if meta.exists() else None
  if target.exists():
   back=R/'artsrc/ui/retired-2026-11-05/orb-before-059'/target.name;back.parent.mkdir(parents=True,exist_ok=True)
   if not back.exists():shutil.copy2(target,back)
   if meta.exists() and not Path(str(back)+'.meta').exists():shutil.copy2(meta,str(back)+'.meta')
  shutil.copy2(src,target)
  if old is None:shutil.copy2(str(src)+'.meta',meta)
  else:assert meta.read_bytes()==old
print('Applied' if a.apply else 'DRY RUN / pending user approval',[(r['slug'],r['size']) for r in m['files']])
