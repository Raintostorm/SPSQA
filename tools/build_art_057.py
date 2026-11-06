#!/usr/bin/env python3
"""INPUT057 reproducible staging and pixel proofs. No runtime write without --apply-approved."""
from pathlib import Path
from PIL import Image,ImageDraw,ImageChops,ImageFont
import numpy as np,json,hashlib,re,shutil,argparse,uuid
R=Path(__file__).resolve().parents[1];W=R/'artsrc/ui/anim-057';S=W/'staged';S.mkdir(parents=True,exist_ok=True);C=R/'chat/check';records=[]
def sha(p):return hashlib.sha256(Path(p).read_bytes()).hexdigest()
def save(im,key,pivot=(.5,.5),note=''):
 p=S/(key+'.png');p.parent.mkdir(parents=True,exist_ok=True);im.save(p)
 a=np.array(im.getchannel('A'));records.append(dict(path=key+'.png',size=im.size,pivot=pivot,border=[0]*4,sha256=sha(p),note=note,alpha_min=int(a.min()),alpha_max=int(a.max())))
 return im
def white(im):
 a=im.getchannel('A');o=Image.new('RGBA',im.size,'white');o.putalpha(a);return o
def sheet(ims,labels,dest,cell=(310,340)):
 out=Image.new('RGB',(cell[0]*len(ims),cell[1]),'#162841');d=ImageDraw.Draw(out)
 for i,(im,label) in enumerate(zip(ims,labels)):
  v=im.copy();v.thumbnail((cell[0]-20,cell[1]-55));out.paste(v,(i*cell[0]+(cell[0]-v.width)//2,20+(cell[1]-60-v.height)//2),v);d.text((i*cell[0]+12,cell[1]-25),label,fill='white')
 out.save(dest)
def clean(im,threshold=20):
 a=np.array(im.getchannel('A'));a=np.where(a<threshold,0,a).astype('uint8');im.putalpha(Image.fromarray(a));return im
# Master normalized ONCE before splitting. Generated halo removed, never regenerate separate halves.
raw=Image.open(W/'source/chest-master-raw.png').convert('RGBA');raw=clean(raw,200)
b=raw.getchannel('A').getbbox();scale=448/(b[2]-b[0]);src=raw.crop(b);src=src.resize((448,round(src.height*scale)),Image.Resampling.LANCZOS)
master=Image.new('RGBA',(512,512));offset=(32,(512-src.height)//2);master.paste(src,offset)
seam=round((476-b[1])*scale)+offset[1]
mask=Image.new('L',master.size);md=ImageDraw.Draw(mask);md.rectangle((0,seam,511,511),fill=255)
keypoly=[(665,476),(665,471),(704,432),(829,432),(871,472),(871,476)]
keypoly=[(round((x-b[0])*scale)+offset[0],round((y-b[1])*scale)+offset[1]) for x,y in keypoly];md.polygon(keypoly,fill=255)
m=np.array(master);bm=np.array(mask)>0;body=m.copy();lid=m.copy();body[~bm]=0;lid[bm]=0
body=Image.fromarray(body);lid=Image.fromarray(lid)
# Complementary hard masks allow exact RGBA reconstruction, including semi-transparent outline.
joined=Image.alpha_composite(body,lid);diff=np.array(joined).astype(int)-np.array(master).astype(int)
# Canonical transparent RGB avoids invisible-RGB-only differences.
master_array=np.array(master);master_array[master_array[:,:,3]==0]=0;master=Image.fromarray(master_array)
diff=np.array(joined).astype(int)-master_array.astype(int)
assert not np.any(diff)
save(master,'Chest/chest-master');save(body,'Chest/chest-body');save(lid,'Chest/chest-lid',(.5,1-seam/512),'Unity pivot at master horizontal seam; full 512 canvas, same scale')
sheet([master,joined,Image.fromarray(np.dstack([np.abs(diff[:,:,:3]).astype('uint8'),np.full((512,512),255,dtype='uint8')]))],['MASTER','BODY + LID','RGBA diff = 0'],C/'057-chest-fit.png',(410,460))
fit=dict(different_pixels=int(np.any(diff!=0,axis=2).sum()),max_channel_error=int(abs(diff).max()),master_crop=b,scale=scale,offset=offset,seam_y=seam,lock_polygon=keypoly,pivot_pixels=[256,seam]);(C/'057-chest-fit.json').write_text(json.dumps(fit,indent=2))
# Four hollow ring phases: common source scale, explicit transparent center cleanup.
raw=Image.open(W/'source/levelup-raw.png').convert('RGBA');centers=[(272,364),(815,364),(1354,364),(1901,364)];radii=[45,101,165,184];flashes=[]
for i,((cx,cy),rad) in enumerate(zip(centers,radii)):
 im=white(clean(raw.crop((cx-256,cy-256,cx+256,cy+256)),90));a=im.getchannel('A');d=ImageDraw.Draw(a);inner=rad*.65;d.ellipse((256-inner,256-inner,256+inner,256+inner),fill=0);im.putalpha(a)
 im=im.resize((256,256),Image.Resampling.LANCZOS);a=np.array(im.getchannel('A'));a[112:144,112:144]=0;im.putalpha(Image.fromarray(a));assert not np.any(np.array(im.getchannel('A'))[112:144,112:144]);flashes.append(save(im,f'LevelUp/levelup-flash-{i}',note='source registration fixed; central alpha zero'))
sheet(flashes,['0 hollow','1 expand','2 break','3 fade'],C/'057-levelup-v2.png',(280,310))
# Burst measured centers, same 0.9 source-to-destination scale for ALL frames.
raw=Image.open(R/'chat/check/051-burst-v2.png').convert('RGBA');centers=[(191,358),(546,358),(902,358),(1262,355),(1616,357),(1988,355)];half=[110,165,176,197,172,122];bursts=[];registration=[]
for i,((cx,cy),h) in enumerate(zip(centers,half)):
 im=white(clean(raw.crop((cx-h,cy-h,cx+h,cy+h)),65));im=im.resize((round(2*h*.9),round(2*h*.9)),Image.Resampling.LANCZOS);o=Image.new('RGBA',(512,512));o.alpha_composite(im,(256-im.width//2,256-im.height//2))
 a=np.array(o.getchannel('A'));ys,xs=np.where(a>40);measured=[float((xs.min()+xs.max())/2),float((ys.min()+ys.max())/2)];shift=[round(256-measured[0]),round(256-measured[1])]
 fixed=Image.new('RGBA',(512,512));fixed.alpha_composite(o,tuple(shift));o=fixed
 a=np.array(o.getchannel('A'));ys,xs=np.where(a>40);actual=[float((xs.min()+xs.max())/2),float((ys.min()+ys.max())/2)];assert max(abs(v-256) for v in actual)<=.5
 bursts.append(save(o,f'Summon/burst-{i}'));registration.append(dict(frame=i,source_center=[cx,cy],source_half_extent=h,scale=.9,correction_pixels=shift,measurement='visible alpha > 40 bounding symmetry midpoint',actual_center=actual,destination_center=[256,256],max_center_error_pixels=max(abs(v-256) for v in actual)))
over=Image.new('RGBA',(512,512));colors=['#fa6270','#68e090','#59c4ff','#f9d359','#c18aff','#ffffff']
for im,col in zip(bursts,colors):
 t=Image.new('RGBA',im.size,col);t.putalpha(im.getchannel('A').point(lambda a:round(a*.22)));over=Image.alpha_composite(over,t)
d=ImageDraw.Draw(over);d.line((236,256,276,256),fill='red',width=2);d.line((256,236,256,276),fill='red',width=2)
sheet(bursts+[over],[str(i) for i in range(6)]+['6 overlay / center'],C/'057-burst-aligned.png',(180,220));(C/'057-burst-registration.json').write_text(json.dumps(registration,indent=2))
# Reuse previously extracted approved-style artwork, names match INPUT057; all remain staging until approval.
old=R/'artsrc/ui/anim-review-v1/assets'
for oldname,key in [('orb','Summon/orb-core'),('card','Summon/card-back'),('glow','Common/glow'),('swirl','Common/swirl'),('chest-light','Common/chest-light'),('sparkle','Common/sparkle'),('rays','Common/rarity-ray'),('arrow','LevelUp/levelup-arrow'),('swirl','LevelUp/merge-swirl'),('shard','Chest/shard')]:
 im=Image.open(old/(oldname+'.png')).convert('RGBA');save(im,key,note='reused 056 preview, no new generation')
# G1 isolated glyphs; preserve silhouette alpha, no template plates.
raw=Image.open(R/'chat/check/054-g1-glyphs.png').convert('RGBA')
for key,rect,size in [('ButtonIcons/contribution',(0,0,1086,724),256),('Glyphs/sort',(1086,0,2172,724),150)]:
 im=clean(raw.crop(rect),100);im=im.crop(im.getchannel('A').getbbox());im.thumbnail((round(size*.88),round(size*.88)),Image.Resampling.LANCZOS);o=Image.new('RGBA',(size,size));o.alpha_composite(im,((size-im.width)//2,(size-im.height)//2));save(o,key)
# Atlas candidate only, backup BEFORE any runtime replacement. Outside rect exact equality proof.
atlaspath=R/'Assets/_Project/Resources/ResultScreen/atlas.png';atlas=Image.open(atlaspath).convert('RGBA');H=atlas.height;rect=(793,H-669-216,1500,H-669)
raw=Image.open(R/'chat/check/054-g1-banner-source.png').convert('RGBA');ribbon=raw.crop((0,512,1536,1024));ribbon=clean(ribbon,200);ribbon=ribbon.crop(ribbon.getchannel('A').getbbox());ribbon=ribbon.resize((707,216),Image.Resampling.LANCZOS)
patched=atlas.copy();patched.paste(ribbon,rect[:2]);save(ribbon,'ResultScreen/defeat-candidate')
p=W/'atlas-candidate.png';patched.save(p);outside=np.array(atlas)!=np.array(patched);outside[rect[1]:rect[3],rect[0]:rect[2]]=False;assert not np.any(outside)
backup=R/'artsrc/ui/retired-2026-11-05/057-backup';backup.mkdir(parents=True,exist_ok=True)
for q in [atlaspath,Path(str(atlaspath)+'.meta')]:
 dest=backup/q.name
 if dest.exists():assert sha(dest)==sha(q),'Backup exists with different contents; stop'
 else:shutil.copy2(q,dest)
(C/'057-atlas-proof.json').write_text(json.dumps(dict(atlas_size=atlas.size,unity_rect=[793,669,707,216],top_left_rect=rect,outside_changed_pixels=int(outside.any(axis=2).sum()),original_sha256=sha(atlaspath),candidate_sha256=sha(p),runtime_modified=False),indent=2))
sheet([atlas.crop(rect),ribbon],['OLD Defeat','STAGED slate Defeat'],C/'057-defeat-compare.png',(740,260))
# New sprite metas in staging use stable GUIDs; final import copies them once and preserves existing metadata.
template=(R/'Assets/_Project/Resources/HomeChrome/coin.png.meta').read_text()
for x in records:
 p=S/x['path'];mp=Path(str(p)+'.meta')
 if not mp.exists():
  t=re.sub(r'^guid: .*$',f'guid: {uuid.uuid4().hex}',template,flags=re.M);t=re.sub(r'spritePivot: .*',f'spritePivot: {{x: {x["pivot"][0]}, y: {x["pivot"][1]}}}',t);mp.write_text(t)
(W/'manifest.json').write_text(json.dumps(dict(status='STAGED_WAITING_USER_APPROVAL',files=records,fit=fit,burst_registration=registration),indent=2))
print(f'{len(records)} staging assets + .meta; chest RGBA diff0; outside-atlas diff0; runtime NOT modified')
