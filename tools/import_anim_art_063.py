#!/usr/bin/env python3
"""Import only after user approves INPUT063 board+GIFs; metadata GUIDs preserved."""
from pathlib import Path
import json,argparse,shutil,uuid
R=Path(__file__).resolve().parents[1];W=R/'artsrc/ui/anim-art-063';p=argparse.ArgumentParser();p.add_argument('--apply',action='store_true');p.add_argument('--approval-receipt',type=Path);a=p.parse_args();m=json.loads((W/'manifest.json').read_text())
if a.apply:
 assert a.approval_receipt and a.approval_receipt.exists(),'User approval required'
 receipt=json.loads(a.approval_receipt.read_text());assert receipt.get('approved_by')=='user' and receipt.get('message')
 for r in m['files']:
  src=W/'staged'/(r['slug']+'.png');dst=R/r['target'];dst.parent.mkdir(parents=True,exist_ok=True);meta=Path(str(dst)+'.meta');old=meta.read_bytes() if meta.exists() else None
  folder=dst.parent
  while folder!=R/'Assets/_Project/Resources' and (R/'Assets/_Project/Resources') in folder.parents:
   mp=Path(str(folder)+'.meta')
   if not mp.exists():mp.write_text('fileFormatVersion: 2\nguid: '+uuid.uuid4().hex+'\nfolderAsset: yes\nDefaultImporter:\n  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n')
   folder=folder.parent
  if dst.exists():
   b=R/'artsrc/ui/retired-2026-11-06/before-063'/dst.relative_to(R/'Assets/_Project/Resources');b.parent.mkdir(parents=True,exist_ok=True)
   if not b.exists():shutil.copy2(dst,b)
   if meta.exists() and not Path(str(b)+'.meta').exists():shutil.copy2(meta,str(b)+'.meta')
  shutil.copy2(src,dst)
  if old is None:shutil.copy2(str(src)+'.meta',meta)
  else:assert meta.read_bytes()==old
print('APPLIED' if a.apply else 'DRY RUN pending user review',len(m['files']),'sprites')
