#!/usr/bin/env python3
"""Reviewed D1 dependency decisions + per-candidate evidence. Default dry-run; --apply moves only proven unused."""
from pathlib import Path
import json,re,hashlib,shutil,sys
R=Path(__file__).resolve().parents[1];report=(R/'chat/check/049-cleanup.md').read_text();res=R/'Assets/_Project/Resources'
active=set('atk back battle close coin cyan defense dice gold hp info inventory menu mp navy next previous range recipe speed'.split())
# Reviewed actual helper argument flows, not arbitrary token grep.
reasons={
'ButtonFamilies/atlas':'ButtonFamilySkin.Get loads atlas; BattleUiSkin.Skin calls Get(6/7) and Red/Purple/Info/etc use Get. Live and fallback paths.',
'SharedUI/btn_blue':'SharedSettings.Start literal Resources.Load<Sprite>(SharedUI/btn_blue); keep even if later role skin replaces it.',
'SharedUI/btn_primary':'SharedSettings.Button Resume literal load; valid initialization path before role skin.',
'SharedUI/buff_speed':'Visual/MonsterBuffVisual.cs MakeBadge AttackSpeed literal load.',
'SummonKit/price-pill':'GemEconomyScreen.SummonButton, SummonResultsView count badge, DailyShopScreen count badge literal loads.'}
# 049 contains 47 assets, 31 with candidate refs, not 31 lines.
sections=re.split(r'^### ',report,flags=re.M)[1:];assets=[];alllines=[];moves=[]
scripts=list((R/'Assets').rglob('*.cs'));serialized=[p for p in (R/'Assets').rglob('*') if p.is_file() and p.suffix in {'.unity','.prefab','.asset','.mat','.anim','.controller','.overrideController','.playable','.json'}];texts={p:p.read_text(errors='replace') for p in serialized}
for sec in sections:
 name=sec.splitlines()[0];p=R/name;key=p.relative_to(res).with_suffix('').as_posix();slug=p.stem
 used=key in reasons or (key.startswith('UiRefresh/') and slug in active)
 reason=reasons.get(key,'ApprovedUiArt.Get/Apply/StatTexture active argument maps to '+key if used else 'No reachable loader argument found after reviewing actual helper calls; 049 tokens refer to other resources/comments or an uncalled method.')
 if key.startswith('UiRefresh/') and not used:
  reason={'key':'ResultPanel reward key and HomeChrome wallet key use HomeChrome/key, not ApprovedUiArt; generic UiRefresh loader alone is not an argument.', 'panel':'No ApprovedUiArt call passes panel. BattleSceneSetup panel is a serialized property name, unrelated.', 'pressed':'Only ApprovedUiArt.GoldStates references this slug; no code calls GoldStates (one comment says deliberately not used).', 'disabled':'Only ApprovedUiArt.GoldStates references this slug; no code calls GoldStates.'}.get(slug,reason)
 if key in ['SharedUI/gold','SharedUI/key']:reason='BattleUiSkin.Icon real arguments: settings,battle,enemy,upgrade,qualified Glyphs/home/close and HomeChrome/coin. SharedSettings generic pictogram args use home/close/restart/resume; toggle args sound/vibration; no gold/key. CoinRewardEffect now HomeChrome/coin. Old token matches are other loaders.'
 meta=Path(str(p)+'.meta');guid=re.search(r'^guid: (\w+)',meta.read_text(),re.M).group(1) if meta.exists() else None
 hits=[str(q.relative_to(R)) for q,t in texts.items() if guid and guid in t]
 candidates=[]
 for line in sec.splitlines()[1:]:
  m=re.match(r'- `([^`]+\.cs):(\d+): (.*)`$',line)
  if not m:continue
  file,oldnum,code=m.groups();path=R/file;lines=path.read_text().splitlines();found=[i+1 for i,t in enumerate(lines) if t.strip()==code.strip()];num=found[0] if found else int(oldnum)
  relevant=False;why='Token is not a load of this asset.';load=key
  if 'ApprovedUiArt.cs' in file and '"UiRefresh/" + name' in code:
   relevant=used;why='Generic loader, reviewed actual caller argument set: '+('slug reachable.' if used else 'slug not passed by live callers.')
  elif key=='ButtonFamilies/atlas': relevant=True;why='Literal atlas load reached from ButtonFamilySkin.Get.'
  elif key=='SummonKit/price-pill' and 'SummonKit/price-pill' in code:relevant=True;why='Literal Resources key.'
  elif key.startswith('UiRefresh/'):
   if 'GoldStates' in code or ('ApprovedUiArt.cs' in file and ('Get("pressed"' in code or 'Apply(button' in code)):why='GoldStates method has zero callers; not a live path.'
   elif 'ApprovedUiArt.' in code and ('"'+slug+'"') in code:relevant=used;why='Helper call argument resolves to '+key
   elif any(x in code for x in ['string[] icons=','string statIcon=']) and slug in active:relevant=True;why='Stat mapping flows into ApprovedUiArt.StatTexture → Get first, StatIcons fallback second.'
   elif 'string[] icons=' in code and 'hp' in code:relevant=used;why='Stat mapping reviewed with following StatTexture call.'
  elif key in ['SharedUI/btn_blue','SharedUI/btn_primary'] and key in code:relevant=True;why='Exact SharedUI Resources load (not Art/UI template).'
  elif key=='SharedUI/buff_speed' and ('SharedUI/' in code or '"buff_speed"' in code):relevant=True;why='Check live evidence MonsterBuffVisual; generic helper string alone not sufficient.'
  status='có' if relevant else 'không'
  row=dict(asset=name,location=f'{file}:{num}',old_line=int(oldnum),code=code,loader=load,can_execute=status,reason=why);candidates.append(row);alllines.append(row)
 tools=[]
 for q in (R/'tools').rglob('*.py'):
  if 'audit' in q.name or q.name=='build_art_057.py':continue
  t=q.read_text(errors='replace')
  if key in t or ('Resources/'+key.split('/')[0]) in t:tools.append(str(q.relative_to(R)))
 # Legacy importer only WRITES Buttons and SummonKit, reads independent raw source + Art/UI template.
 if key in ['SharedUI/gold','SharedUI/key']:
  tools=['tools/build_home_walk_review.py']; tool_note='Actual tool input: build_home_walk_review.py:13 reads SharedUI/{name}.png with names gold/key; retain to keep reproducible demo.'
 else:
  tool_note='Legacy importer output targets only; source images and Art/UI/btn_blue.png.meta template retained.' if tools and key.startswith(('Buttons/','SummonKit/')) else ('No tool input dependency found.' if not tools else 'Review tool references: '+', '.join(tools))
 tool_block=bool(tools) and not key.startswith(('Buttons/','SummonKit/'))
 decision='KEEP' if used or hits else 'KEEP_TOOL' if tool_block else 'RETIRE'
 item=dict(asset=name,key=key,decision=decision,reason=reason,guid=guid,guid_consumers=hits,tools=tools,tools_note=tool_note,candidates=candidates)
 if p.exists():
  item['sha256']=hashlib.sha256(p.read_bytes()).hexdigest()
  if decision=='RETIRE':
   dst=R/'artsrc/ui/retired-2026-11-05'/p.relative_to(R/'Assets/_Project/Resources');item['retired_to']=str(dst.relative_to(R));moves.append(item)
 elif decision=='RETIRE':item['already_absent']=True
 assets.append(item)
if '--apply' in sys.argv:
 for x in moves:
  p=R/x['asset'];dst=R/x['retired_to'];dst.parent.mkdir(parents=True,exist_ok=True)
  assert not dst.exists(),str(dst)+' already exists'
  shutil.move(str(p),str(dst));mp=Path(str(p)+'.meta')
  if mp.exists():shutil.move(str(mp),str(dst)+'.meta')
  assert hashlib.sha256(dst.read_bytes()).hexdigest()==x['sha256']
  assert not p.exists()
body=['# D1 verified 057','2026-11-05. Static reachable-path review, **chưa chạy Unity**. “có” means a valid code path can load, not PlayMode observation. All 049 candidate lines classified below.','## Asset decisions','| Asset | Decision | GUID consumers | Reason |','|---|---|---:|---|']
for x in assets:body.append(f'| `{x["asset"]}` | {x["decision"]} | {len(x["guid_consumers"])} | {x["reason"]} {x["tools_note"]} |')
body+=['\n## Each 049 code candidate','| Asset | file:line | Resource key | có/không/không rõ | Evidence |','|---|---|---|---|---|']
for x in alllines:body.append(f'| `{x["asset"]}` | `{x["location"]}` | `{x["loader"]}` | {x["can_execute"]} | {x["reason"]} |')
body+=['\n## Newly discovered live evidence missing from 049','- `Assets/_Project/Scripts/Visual/MonsterBuffVisual.cs:37`: `Resources.Load<Sprite>("SharedUI/buff_speed")` — KEEP.','- `Assets/_Project/Scripts/UI/DailyShopScreen.cs:93`: SummonKit/price-pill — KEEP.','\n## Moves','Applied: '+str('--apply' in sys.argv), 'GUID scan: serialized Assets .unity/.prefab/.asset/.mat/.anim/.controller/.overrideController/.playable/.json; excludes .meta self and retired art. Legacy importers may recreate old output targets if rerun; not deleted or rewritten.']
for x in moves:body.append(f'- `{x["asset"]}` → `{x["retired_to"]}` + .meta; SHA256 `{x["sha256"]}`')
(R/'chat/check/057-d1-verified.md').write_text('\n'.join(body)+'\n');(R/'chat/check/057-d1-verified.json').write_text(json.dumps(dict(applied='--apply' in sys.argv,assets=assets,candidates=alllines,moves=moves),indent=2))
print(len(assets),'assets;',len(alllines),'candidate lines;',len(moves),'safe retirement candidates;',sum(x['decision']=='KEEP' for x in assets),'KEEP')
