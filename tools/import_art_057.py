#!/usr/bin/env python3
"""Dry-run by default. --apply requires a user approval receipt file; no C# changes."""
from pathlib import Path
import json,hashlib,shutil,argparse,re,uuid
R=Path(__file__).resolve().parents[1];W=R/'artsrc/ui/anim-057';res=R/'Assets/_Project/Resources'
def sha(p):return hashlib.sha256(p.read_bytes()).hexdigest()
p=argparse.ArgumentParser();p.add_argument('--apply',action='store_true');p.add_argument('--approval-receipt',type=Path);a=p.parse_args()
m=json.loads((W/'manifest.json').read_text());rows=[]
for x in m['files']:
 key=x['path']
 if key in ['Chest/chest-master.png','ResultScreen/defeat-candidate.png']:continue
 if key.startswith(('ButtonIcons/','Glyphs/')):dest=res/key
 else:dest=res/'Anim'/key
 rows.append(dict(source=str((W/'staged'/key).relative_to(R)),target=str(dest.relative_to(R)),size=x['size'],pivot=x['pivot'],border=x['border'],sha256=x['sha256'],existing=dest.exists()))
rows.append(dict(source='artsrc/ui/anim-057/atlas-candidate-058.png',target='Assets/_Project/Resources/ResultScreen/atlas.png',rect=[793,669,707,216],backup='artsrc/ui/retired-2026-11-05/057-backup/atlas.png',existing=True))
if a.apply:
 if not a.approval_receipt or not a.approval_receipt.exists():p.error('Provide genuine user approval receipt; do not fabricate approval.')
 receipt=json.loads(a.approval_receipt.read_text());assert receipt.get('approved_by')=='user' and receipt.get('message'), 'User approval evidence required'
 approved=set(receipt.get('targets',[]));assert approved,'Receipt must explicitly enumerate approved targets'
 for x in rows:
  if x['target'] not in approved:continue
  src=R/x['source'];dst=R/x['target'];dst.parent.mkdir(parents=True,exist_ok=True)
  # New folders need Unity metadata; existing GUIDs stay intact.
  parent=dst.parent
  while parent!=res and res in parent.parents:
   meta=Path(str(parent)+'.meta')
   if not meta.exists():meta.write_text('fileFormatVersion: 2\nguid: '+uuid.uuid4().hex+'\nfolderAsset: yes\nDefaultImporter:\n  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n')
   parent=parent.parent
  if dst.exists():
   backup=R/'artsrc/ui/retired-2026-11-05/057-backup'/dst.relative_to(res);backup.parent.mkdir(parents=True,exist_ok=True)
   if not backup.exists():shutil.copy2(dst,backup)
   if Path(str(dst)+'.meta').exists() and not Path(str(backup)+'.meta').exists():shutil.copy2(str(dst)+'.meta',str(backup)+'.meta')
  if x.get('rect'):
   proof=json.loads((R/'chat/check/057-atlas-proof.json').read_text());assert sha(dst)==proof['original_sha256'],'Atlas changed since snapshot; rebuild candidate, do not overwrite Claude work'
  before=Path(str(dst)+'.meta').read_bytes() if Path(str(dst)+'.meta').exists() else None
  shutil.copy2(src,dst)
  if before is None:shutil.copy2(str(src)+'.meta',str(dst)+'.meta')
  else:assert Path(str(dst)+'.meta').read_bytes()==before
(W/'import-plan.json').write_text(json.dumps(dict(applied=a.apply,applied_targets=[x['target'] for x in rows if a.apply and x['target'] in approved],files=rows),indent=2))
print(('APPLIED APPROVED TARGETS' if a.apply else 'DRY RUN / WAITING USER APPROVAL'),sum(1 for x in rows if a.apply and x['target'] in approved) if a.apply else len(rows),'targets; no C#')
