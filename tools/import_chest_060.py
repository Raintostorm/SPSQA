#!/usr/bin/env python3
"""Preview pending approval: preserve Common/chest-light GUID when imported."""
from pathlib import Path
import argparse,json,shutil
R=Path(__file__).resolve().parents[1];W=R/'artsrc/ui/chest-open-060';p=argparse.ArgumentParser();p.add_argument('--apply',action='store_true');p.add_argument('--approval-receipt',type=Path);a=p.parse_args();m=json.loads((W/'manifest.json').read_text())
if a.apply:
 assert a.approval_receipt and a.approval_receipt.exists(),'User approval receipt required'
 x=json.loads(a.approval_receipt.read_text());assert x.get('approved_by')=='user' and x.get('message')
 for r in m['files']:
  src=R/r['source'];dst=R/r['target'];dst.parent.mkdir(parents=True,exist_ok=True);meta=Path(str(dst)+'.meta');old=meta.read_bytes() if meta.exists() else None
  if dst.exists():
   backup=R/'artsrc/ui/retired-2026-11-05/before-060'/dst.relative_to(R/'Assets/_Project/Resources');backup.parent.mkdir(parents=True,exist_ok=True)
   if not backup.exists():shutil.copy2(dst,backup)
   if meta.exists() and not Path(str(backup)+'.meta').exists():shutil.copy2(meta,str(backup)+'.meta')
  shutil.copy2(src,dst)
  if old is None:shutil.copy2(str(src)+'.meta',meta)
  else:assert meta.read_bytes()==old
print('APPLIED' if a.apply else 'DRY RUN / pending approval',[(x['target'],x['size']) for x in m['files']])
