import json,pathlib,shutil
root=pathlib.Path(__file__).resolve().parents[1]; out=root/'Assets/_Project/Resources/Mythics';out.mkdir(parents=True,exist_ok=True)
def clips(folder,file,source=None):
 data=json.load(open(root/folder/file))
 data=[({'rect':e,'pivot':[e[2]/2,e[3]/2]} if isinstance(e,list) else e) for e in data]
 for e in data:
  e['source']=e.get('file',source);e['scale']=e.get('actorScale',1)
  src=root/folder/e['source'];dest=out/(src.stem+".bytes")
  shutil.copyfile(src,dest);e['source']=dest.stem
  # Unity JsonUtility supports arrays of structs, not nested arrays.
  e['runs']=[{'x':s[0],'y':s[1],'w':s[2]} for s in e.pop('spans',[])]
 return data
roster='artsrc/units/mythic-roster-v1';new='artsrc/units/mythic-newcomers-v1'
entries=[]
for id,name,file,folder,source in [('undead','Grave Warden','undead-clips.json',roster,'undead.png'),('robot','Aegis Engine','robot-clips.json',roster,'robot.png'),('elf','Moonwood Oracle','elf-v2-clips.json',roster,None),('clock','Chronomancer','clock-final-clips.json',new,None),('mimic','Mimic','mimic-clips.json',new,'mimic.png'),('swordsman','Masked Ronin','swordsman-clips.json',new,'swordsman.png')]:
 c=clips(folder,file,source)
 if id=='undead':
  for i,p in enumerate([[150,917],[475,917],[760,917],[1100,917]],8):c[i]['pivot']=[p[0]-c[i]['rect'][0],p[1]-c[i]['rect'][1]]
 entries.append({'id':id,'name':name,'frames':c,'passive':id=='undead'})
b=clips(roster,'bear-extra-clips.json','bear-extra.png');a=clips(roster,'bear-attack-clips.json','bear-attack.png')
for e,p in zip(a,[[292,363],[861,363],[247,649],[859,649],[293,1023],[862,1032],[245,1300],[857,1305]]):e['pivot']=[p[0]-e['rect'][0],p[1]-e['rect'][1]]
for e in a:e['scale']=.56/.78
entries.append({'id':'bear','name':'Thunder Bear','frames':b[:8]+a[:4]+b[8:],'empowered':a[4:]})
pivots=[[150,264],[440,264],[713,264],[1004,264],[170,542],[440,544],[703,544],[1003,544],[140,817],[415,817],[685,817],[1000,817],[360,594],[945,594],[360,1157],[939,1157],[365,587],[970,639],[330,1138],[940,1138]]
g=[]
for i,(r,p) in enumerate(zip(json.load(open(root/'artsrc/units/mythic-guardian-v1/v6/bounds.json')),pivots)):
 src=['base','skill','dead'][0 if i<12 else 1 if i<16 else 2];dest='guardian-'+src;shutil.copyfile(root/f'artsrc/units/mythic-guardian-v1/v4/{src}.png',out/f'{dest}.bytes');g.append({'source':dest,'rect':r,'pivot':[p[0]-r[0],p[1]-r[1]],'scale':.75 if i<12 else .43 if i<16 else .4,'runs':[]})
entries.insert(0,{'id':'guardian','name':'Dawn Guardian','frames':g})
# Normalize all poses against each character idle height, using approved per-pose scale where specified.
for e in entries:
 c=e['frames'];ref=c[0]['rect'][3]*c[0]['scale'];
 for f in c+e.get('empowered',[]): f['ppu']=ref/(1.25*f['scale'])
# Approved 4-frame Guardian replacement: fixed foot pivot, independent cells.
guardian=next(e for e in entries if e['id']=='guardian')
for i in range(4):
 src=root/f'artsrc/units/mythic-guardian-atk-v3/atk-{i}.png';dest=f'guardian-attack-v3-{i}'
 shutil.copyfile(src,out/f'{dest}.bytes')
 guardian['frames'][8+i]={'source':dest,'rect':[0,0,860,600],'pivot':[430,540],'ppu':288,'runs':[]}
(out/'catalog.json').write_text(json.dumps({'units':entries},separators=(',',':')))
fx=clips(new,'effects-clips.json','effects.png')
(out/'vfx-manifest.json').write_text(json.dumps({'frames':fx},separators=(',',':')))
for name in ['support-effects','combat-effects','capacity-relic']:
 shutil.copyfile(root/'artsrc/units/mythic-integration'/f'{name}.png',out/f'{name}.bytes')
print('Prepared',len(entries),'Mythics')
