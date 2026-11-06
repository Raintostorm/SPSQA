#!/usr/bin/env python3
"""INPUT060: edit-derived upper chest + exact original body; review only."""
from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,json,hashlib,re,uuid
R=Path(__file__).resolve().parents[1];W=R/'artsrc/ui/chest-open-060';OUT=W/'staged';OUT.mkdir(exist_ok=True);C=R/'chat/check';navy='#1B2440'
bodypath=R/'Assets/_Project/Resources/Anim/Chest/chest-body.png';body=Image.open(bodypath).convert('RGBA');closed=Image.open(R/'artsrc/ui/anim-057/staged/Chest/chest-master.png').convert('RGBA');fit=json.loads((C/'057-chest-fit.json').read_text());seam=fit['seam_y']
raw=Image.open(W/'source/chest-open-raw.png').convert('RGBA');a=np.array(raw);a[a[:,:,3]<200]=0;raw=Image.fromarray(a)
# Measured generated rim anchors x85..1170, front horizontal rim y598 -> original x32..480,y236.
scale=448/1085;sz=(round(raw.width*scale),round(raw.height*scale));raw=raw.resize(sz,Image.Resampling.LANCZOS);offset=(round(32-85*scale),round(seam-598*scale));opened=Image.new('RGBA',(512,512));opened.alpha_composite(raw,offset)
o=np.array(opened);b=np.array(body);yy,xx=np.mgrid[:512,:512];mask=(yy>=seam)|(b[:,:,3]>0)
# Exact replacement, including body edge alpha: no alpha-composite blend can alter source pixels.
o[mask]=b[mask];o[o[:,:,3]==0]=0;opened=Image.fromarray(o);opened.save(OUT/'chest-open.png')
c=np.array(closed);delta=np.abs(o.astype(int)-c.astype(int));assert not np.any(delta[mask]);bodydiff=int(np.any(delta!=0,axis=2)[mask].sum())
# Analytic continuous light column; NO generated-light source is read.
y,x=np.mgrid[:512,:512];center=255.5;top=64.0;bottom=452.0;t=np.clip((y-top)/(bottom-top),0,1)
def smoothstep(a,b,v):
 u=np.clip((v-a)/(b-a),0,1);return u*u*(3-2*u)
# Half width increases gently from125px at foot to150px toward upper end.
half_width=125+25*(1-t)
side=1-smoothstep(.68,1.0,np.abs(x-center)/half_width)
upward_fade=smoothstep(0,.68,t)
soft_foot=1-smoothstep(.84,1,t)
alpha=np.uint8(np.round(255*.92*side*upward_fade*soft_foot))
assert np.array_equal(alpha,alpha[:,::-1])
light=Image.new('RGBA',(512,512),'white');light.putalpha(Image.fromarray(alpha));light.save(OUT/'chest-light.png')

la=np.array(light);assert np.all(la[la[:,:,3]>0,:3]==255);assert not np.any(la[0,:,3]);assert not np.any(la[-1,:,3]);assert not np.any(la[:,0,3]);assert not np.any(la[:,-1,3])
# Proof: same positions and scale, body diff black field with exact count.
proof=Image.new('RGB',(1536,430),navy);d=ImageDraw.Draw(proof)
for i,(im,label) in enumerate([(closed,'CLOSED'),(opened,'OPEN / ORIGINAL BODY'),(Image.fromarray(np.dstack([np.where(mask[:,:,None],delta[:,:,:3],0).astype('uint8'),np.full((512,512),255,dtype='uint8')])),'BODY REGION DIFF = 0'),(light,'ONE SOFT WHITE COLUMN')]):
 v=im.copy();v.thumbnail((360,360));proof.paste(v,(i*384+(384-v.width)//2,20),v);d.text((i*384+12,400),label,fill='white')
proof.save(C/'060-chest-open.png')
# Combined user approval board for059+060.
orb=Image.open(C/'059-orb-stack.png').convert('RGB');new=Image.new('RGB',(1536,1040),navy);new.paste(proof,(0,0));new.paste(orb,(80,460));d=ImageDraw.Draw(new);d.text((650,490),'059 ENERGY ORB + 060 OPEN CHEST / LIGHT',fill='white')
# Show column tinted gold BEHIND the opened chest as presentation example only.
scene=Image.new('RGBA',(512,512),navy);scene.alpha_composite(opened);beam=Image.new('RGBA',light.size,'#ffd775');beam.putalpha(light.getchannel('A').point(lambda v:round(v*.5)));beam=beam.resize((512,300),Image.Resampling.LANCZOS);scene.alpha_composite(beam,(0,-22));scene.alpha_composite(body);new.paste(scene.convert('RGB'),(850,460));new.save(C/'060-review-with-059.png')
# Finite preview: white flash masks pose switch, no squashing or stretching the lid.
frames=[]
for i in range(60):
 t=i*.04;scene=Image.new('RGBA',(512,512),navy)
 if t<.55:scene.alpha_composite(closed)
 else:
  a=min(1,(t-.55)/.35);scene.alpha_composite(opened);beam=Image.new('RGBA',light.size,'#ffd775');beam.putalpha(light.getchannel('A').point(lambda v:round(v*a*.5)));beam=beam.resize((512,300),Image.Resampling.LANCZOS);scene.alpha_composite(beam,(0,-22));scene.alpha_composite(body)
 flash=max(0,1-abs(t-.55)/.16)
 if flash>0:scene=Image.alpha_composite(scene,Image.new('RGBA',(512,512),(255,255,255,round(flash*240))))
 frames.append(scene.convert('RGB'))
palette=frames[-1].quantize(colors=255);ims=[x.quantize(palette=palette,dither=Image.Dither.NONE) for x in frames];ims[0].save(C/'060-chest-open.gif',save_all=True,append_images=ims[1:],duration=40,loop=0,disposal=2)
files=[];template=(R/'Assets/_Project/Resources/HomeChrome/coin.png.meta').read_text()
for slug,target in [('chest-open','Anim/Chest/chest-open.png'),('chest-light','Anim/Common/chest-light.png')]:
 p=OUT/(slug+'.png');meta=Path(str(p)+'.meta')
 if not meta.exists():meta.write_text(re.sub(r'^guid: .*$',f'guid: {uuid.uuid4().hex}',template,flags=re.M))
 files.append(dict(source=str(p.relative_to(R)),target='Assets/_Project/Resources/'+target,size=[512,512],sha256=hashlib.sha256(p.read_bytes()).hexdigest(),border=[0]*4))
checks=dict(body_region_different_pixels=bodydiff,body_region_max_channel_error=int(delta[mask].max()),body_mask='all rows y>=236 plus original body nonzero alpha (including lock above seam)',body_source=str(bodypath.relative_to(R)),body_source_sha256=hashlib.sha256(bodypath.read_bytes()).hexdigest(),upper_source_transform=dict(scale=scale,offset=offset,rim_source_y=598),light_source='PROCEDURAL smoothstep field; no image source',light_bottom_fade_pixels=62.08,light_half_width_top=150,light_half_width_bottom=125,light_horizontal_symmetry_exact=True,light_visible_rgb_white=True,light_edges_alpha_zero=True,runtime_imported=False,unity_run=False)
(C/'060-checks.json').write_text(json.dumps(checks,indent=2));(W/'manifest.json').write_text(json.dumps(dict(status='USER_APPROVED_062_BUILD',files=files,checks=checks),indent=2));print('2 sprites512² staged; body region RGBA diff0; white column verified; joint059+060 preview ready')
