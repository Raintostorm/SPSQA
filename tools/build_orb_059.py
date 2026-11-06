#!/usr/bin/env python3
"""INPUT059: generated layered orb normalization + preview; no runtime writes."""
from pathlib import Path
from PIL import Image, ImageDraw,ImageFilter
import numpy as np,json,math,hashlib,re,uuid
R=Path(__file__).resolve().parents[1];W=R/'artsrc/ui/summon-orb-059';OUT=W/'staged';OUT.mkdir(exist_ok=True);C=R/'chat/check';records=[]
def radial_gradient(size,slug):
 # Analytic radial field: no source image, noise, random seed, or handpainted texture.
 width,height=size;y,x=np.mgrid[:height,:width];r=np.sqrt((x-(width-1)/2)**2+(y-(height-1)/2)**2)/(min(size)/2)
 outer={'orb-core':.84,'mote':.82,'flash':.94}[slug];inner={'orb-core':.10,'mote':.04,'flash':.22}[slug]
 u=np.clip((r-inner)/(outer-inner),0,1);smooth=u*u*(3-2*u);alpha=np.uint8(np.round(255*(1-smooth)**2))
 rgb=np.full((height,width,3),255,dtype=np.uint8)
 if slug=='orb-core':
  color_u=np.clip(r/.65,0,1);color_u=color_u*color_u*(3-2*color_u);rgb=np.uint8(np.round(255+(np.array([191,239,255])-255)*color_u[:,:,None]))
 im=Image.fromarray(np.dstack([rgb,alpha]),'RGBA');path=OUT/(slug+'.png');im.save(path)
 assert np.array_equal(alpha,np.rot90(alpha));assert np.array_equal(alpha,alpha[:,::-1]);assert not np.any(alpha[0]);assert not np.any(alpha[-1]);assert not np.any(alpha[:,0]);assert not np.any(alpha[:,-1])
 assert np.all(np.diff(alpha[height//2,width//2:].astype(int))<=0)
 records.append(dict(slug=slug,source='PROCEDURAL:radial_gradient (no generated image)',size=size,measured_center=[(width-1)/2,(height-1)/2],max_center_error=.5,white_rgb=slug!='orb-core',alpha_edges_zero=True,radial_symmetry_exact=True,monotonic_center_to_edge=True,inner_radius=inner,outer_radius=outer,sha256=hashlib.sha256(path.read_bytes()).hexdigest()))
 return im

def extract(source,rect,size,slug,kind='white'):
 if slug in ['orb-core','mote','flash']:return radial_gradient(size,slug)
 im=Image.open(W/'source'/source).convert('RGBA');im=im.crop(rect) if rect else im
 a=np.array(im).astype(float);lum=a[:,:,0]*.2126+a[:,:,1]*.7152+a[:,:,2]*.0722
 if kind=='ring':alpha=a[:,:,3]*np.clip((lum-218)/32,0,1)
 else:alpha=a[:,:,3]*np.clip((lum-100)/155,0,1)
 alpha=np.uint8(np.clip(alpha,0,255));alpha=Image.fromarray(alpha).filter(ImageFilter.GaussianBlur(.65 if kind=='ring' else 2))
 b=alpha.point(lambda x:255 if x>6 else 0).getbbox();assert b
 alpha=alpha.crop(b);alpha.thumbnail((round(size[0]*.80),round(size[1]*.80)),Image.Resampling.LANCZOS)
 canvas=Image.new('L',size);canvas.paste(alpha,((size[0]-alpha.width)//2,(size[1]-alpha.height)//2))
 # Register actual visible bbox center, not sheet-cell center.
 aa=np.array(canvas);yy,xx=np.where(aa>6);mid=((xx.min()+xx.max())/2,(yy.min()+yy.max())/2);delta=(round(size[0]/2-mid[0]),round(size[1]/2-mid[1]));centered=Image.new('L',size);centered.paste(canvas,delta);canvas=centered
 out=Image.new('RGBA',size,'white')
 if kind=='core':
  # Cyan exception, no painted reflections; raw generated illumination remains in alpha.
  y,x=np.mgrid[:size[1],:size[0]];radius=np.sqrt((x-size[0]/2)**2+(y-size[1]/2)**2)/(size[0]*.38);mix=np.clip(radius,0,1)[:,:,None];rgb=np.uint8(255*(1-mix)+np.array([191,239,255])[None,None,:]*mix);out=Image.fromarray(np.dstack([rgb,np.array(canvas)]),'RGBA')
 else:out.putalpha(canvas)
 path=OUT/(slug+'.png');out.save(path)
 aa=np.array(out);yy,xx=np.where(aa[:,:,3]>6);mid=[float((xx.min()+xx.max())/2),float((yy.min()+yy.max())/2)];err=max(abs(mid[0]-size[0]/2),abs(mid[1]-size[1]/2));assert err<=1
 if kind!='core':assert np.all(aa[aa[:,:,3]>0,:3]==255)
 assert max(aa[0,:,3].max(),aa[-1,:,3].max(),aa[:,0,3].max(),aa[:,-1,3].max())==0
 records.append(dict(slug=slug,source=source,source_crop=rect,alpha_crop=b,size=size,measured_center=mid,max_center_error=err,white_rgb=kind!='core',alpha_edges_zero=True,sha256=hashlib.sha256(path.read_bytes()).hexdigest()))
 return out
core=extract('core.png',None,(512,512),'orb-core','core')
swirl=Image.open(W/'source/swirl-rune.png');width,height=swirl.size
A=extract('swirl-rune.png',(0,0,width//3,height),(512,512),'orb-swirl-a','ring')
B=extract('swirl-rune.png',(width//3,0,width*2//3,height),(512,512),'orb-swirl-b','ring')
rune=extract('swirl-rune.png',(width*2//3,0,width,height),(768,768),'rune-circle','ring')
p=Image.open(W/'source/particles.png');width,height=p.size
mote=extract('particles.png',(0,0,width//3,height),(64,64),'mote')
streak=extract('particles.png',(width//3,0,width*2//3,height),(256,64),'streak')
flash=extract('particles.png',(width*2//3,0,width,height),(1024,1024),'flash')
# Metadata prepared only in staging; target existing .meta retained at approval-time import.
template=(R/'Assets/_Project/Resources/HomeChrome/coin.png.meta').read_text()
for r in records:
 meta=OUT/(r['slug']+'.png.meta')
 if not meta.exists():meta.write_text(re.sub(r'^guid: .*$',f'guid: {uuid.uuid4().hex}',template,flags=re.M))
# Contact sheet on exact requested navy.
navy='#1B2440';contact=Image.new('RGB',(1120,620),navy);d=ImageDraw.Draw(contact)
for i,r in enumerate(records):
 im=Image.open(OUT/(r['slug']+'.png'));im.thumbnail((245,245));x=(i%4)*280;y=(i//4)*310;contact.paste(im,(x+(280-im.width)//2,y+18+(245-im.height)//2),im);d.text((x+16,y+274),r['slug']+' '+str(r['size'][0])+'x'+str(r['size'][1]),fill='white')
contact.save(C/'059-orb-layers.png')
def tint(im,color,opacity=1):
 out=Image.new('RGBA',im.size,color);out.putalpha(im.getchannel('A').point(lambda v:round(v*opacity)));return out
def place(dst,im,size,angle=0,opacity=1,color=None):
 im=im.resize((size,size),Image.Resampling.LANCZOS) if isinstance(size,int) else im.resize(size,Image.Resampling.LANCZOS)
 if color:im=tint(im,color,opacity)
 elif opacity!=1:im.putalpha(im.getchannel('A').point(lambda v:round(v*opacity)))
 if angle:im=im.rotate(angle,Image.Resampling.BICUBIC,expand=False)
 dst.alpha_composite(im,((dst.width-im.width)//2,(dst.height-im.height)//2))
def stack(t):
 o=Image.new('RGBA',(512,512),navy)
 place(o,rune,480,t*20,.45,'#65d9ff');place(o,B,380,-t*60,.65,'#338edc');place(o,A,350,t*90,.85,'#82efff');place(o,core,round(225*(1+.05*math.sin(t*math.tau))))
 return o
stack(0).convert('RGB').save(C/'059-orb-stack.png')
frames=[]
for f in range(90):
 t=f*.04;im=stack(t)
 for j in range(6):
  phase=(t*.55+j/6)%1;radius=220*(1-phase);ang=j*math.tau/6+.3;px=256+math.cos(ang)*radius;py=256+math.sin(ang)*radius
  dot=tint(mote,'#b8f5ff',math.sin(phase*math.pi)*.9).resize((13,13),Image.Resampling.LANCZOS);im.alpha_composite(dot,(round(px-6),round(py-6)))
  trail=tint(streak,'#73dfff',math.sin(phase*math.pi)*.65).resize((38,10),Image.Resampling.LANCZOS).rotate(-math.degrees(ang),Image.Resampling.BICUBIC,expand=True);im.alpha_composite(trail,(round(px-trail.width/2),round(py-trail.height/2)))
 frames.append(im.convert('RGB'))
# Fixed palette prevents per-frame color palette flicker.
pal=contact.quantize(colors=255);indexed=[im.quantize(palette=pal,dither=Image.Dither.NONE) for im in frames]
indexed[0].save(C/'059-orb-stack.gif',save_all=True,append_images=indexed[1:],duration=40,loop=0,disposal=2)
manifest=dict(status='USER_APPROVED_061_BUILD',files=records,order=['rune-circle','orb-swirl-b','orb-swirl-a','orb-core'],preview_motion=dict(rune_deg_per_s=20,swirl_a=90,swirl_b=-60,core_scale=[.95,1.05],gif_fps=25,frames=90),unity_run=False)
(W/'manifest.json').write_text(json.dumps(manifest,indent=2));(C/'059-checks.json').write_text(json.dumps(dict(files=records,all_centers_within_1px=True,white_vfx_rgb_verified=True,edges_alpha_zero=True,runtime_imported=False,unity_run=False),indent=2))
print('7 layers staged, centers <=1px, white RGB verified, navy contact+stack+25fps GIF; runtime untouched')
