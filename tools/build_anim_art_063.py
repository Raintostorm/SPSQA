#!/usr/bin/env python3
"""INPUT063 independent chibi layers + deterministic preview GIFs. No runtime writes."""
from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,json,math,re,uuid,hashlib
R=Path(__file__).resolve().parents[1];W=R/'artsrc/ui/anim-art-063';S=W/'staged';C=R/'chat/check';navy='#1B2440';records=[];assets={}
def smooth(a,b,x):u=np.clip((x-a)/(b-a),0,1);return u*u*(3-2*u)
def save(im,key,source,light=False):
 if light:
  aa=np.array(im.getchannel('A'));ys,xs=np.where(aa>6);mid=((xs.min()+xs.max())/2,(ys.min()+ys.max())/2);shift=(round(im.width/2-mid[0]),round(im.height/2-mid[1]));aligned=Image.new('RGBA',im.size,'white');aligned.putalpha(Image.new('L',im.size));aligned.alpha_composite(im,shift);im=aligned
 p=S/(key+'.png');p.parent.mkdir(parents=True,exist_ok=True);im.save(p);a=np.array(im);assert not np.any(a[0,:,3]) and not np.any(a[-1,:,3]);assert not np.any(a[:,0,3]) and not np.any(a[:,-1,3])
 yy,xx=np.where(a[:,:,3]>6);center=[float((xx.min()+xx.max())/2),float((yy.min()+yy.max())/2)];err=max(abs(center[0]-im.width/2),abs(center[1]-im.height/2))
 if light:assert np.all(a[a[:,:,3]>0,:3]==255);assert err<=1
 template=(R/'Assets/_Project/Resources/HomeChrome/coin.png.meta').read_text();meta=Path(str(p)+'.meta')
 if not meta.exists():meta.write_text(re.sub(r'^guid: .*$',f'guid: {uuid.uuid4().hex}',template,flags=re.M))
 records.append(dict(slug=key,source=source,target='Assets/_Project/Resources/Anim/'+key+'.png',size=im.size,light=light,center=center,max_center_error=err if light else None,border=[0]*4,sha256=hashlib.sha256(p.read_bytes()).hexdigest()));assets[key]=im;return im
def obj(sheet,i,cols,size,key):
 raw=Image.open(W/'source'/sheet).convert('RGBA');w,h=raw.size;im=raw.crop((round(i*w/cols),0,round((i+1)*w/cols),h));a=np.array(im);alpha=a[:,:,3];alpha=np.where(alpha<180,0,np.clip((alpha.astype(float)-180)/74*255,0,255)).astype('uint8');im.putalpha(Image.fromarray(alpha));im=im.crop(im.getchannel('A').getbbox());im.thumbnail((round(size*.84),round(size*.84)),Image.Resampling.LANCZOS);out=Image.new('RGBA',(size,size));out.alpha_composite(im,((size-im.width)//2,(size-im.height)//2));return save(out,key,sheet)
star=obj('objects.png',0,3,128,'LevelUp/lu-star');save(star.resize((64,64),Image.Resampling.LANCZOS),'LevelUp/lu-star-small','same silhouette as lu-star')
obj('objects.png',1,3,128,'LevelUp/lu-chevron');big=obj('objects.png',2,3,256,'Result/star-big')
a=np.array(big);rgb=a[:,:,:3].astype(float);lum=rgb[:,:,0]*.2126+rgb[:,:,1]*.7152+rgb[:,:,2]*.0722;mask=(a[:,:,3]>0)&(lum>35);gray=np.uint8(np.clip(np.array([74,85,112])[None,None,:]*(lum[:,:,None]/150),0,255));a[mask,:3]=gray[mask];empty=save(Image.fromarray(a),'Result/star-empty','star-big shape; slate recolor');assert np.array_equal(a[:,:,3],np.array(big)[:,:,3])
for i in range(4):obj('confetti.png',i,4,64,'Result/confetti-'+str(i))
def white(alpha):im=Image.new('RGBA',(alpha.shape[1],alpha.shape[0]),'white');im.putalpha(Image.fromarray(np.uint8(np.round(np.clip(alpha,0,1)*255))));return im
# White light layers analytic, no generated noise or painted texture.
y,x=np.mgrid[:768,:384];t=np.clip((y-64)/640,0,1);hw=118+22*(1-t);alpha=(1-smooth(.65,1,abs(x-191.5)/hw))*smooth(0,.7,t)*(1-smooth(.88,1,t));save(white(alpha),'LevelUp/lu-pillar','analytic soft column',True)
y,x=np.mgrid[:256,:768];dx=x-383.5;dy=y-127.5;rx,ry=310,66;q=np.sqrt((dx/rx)**2+(dy/ry)**2);grad=np.sqrt((dx/rx**2)**2+(dy/ry**2)**2)/np.maximum(q,.0001);dist=abs(q-1)/np.maximum(grad,.0001);ring=np.exp(-.5*(dist/2.8)**2);save(white(ring),'LevelUp/lu-ground','analytic constant-width ellipse ring',True)
y,x=np.mgrid[:512,:256];dx=x-127.5;dy=y-255.5;d=dx*math.cos(math.pi/6)-dy*math.sin(math.pi/6);sweep=np.exp(-.5*(d/13)**2)*(1-smooth(90,126,abs(dx)))*(1-smooth(205,250,abs(dy)));save(white(sweep),'Relic/shine-sweep','analytic30degree stripe with soft edges',True)
# Crisp four-point glint, rasterized4x for smooth contour.
im=Image.new('L',(512,512));d=ImageDraw.Draw(im);d.polygon([(256,52),(281,226),(460,256),(281,281),(256,460),(230,281),(52,256),(230,226)],fill=255);im=im.resize((128,128),Image.Resampling.LANCZOS);glint=Image.new('RGBA',(128,128),'white');glint.putalpha(im);save(glint,'Relic/glint','analytic supersampled4point glint',True)
# Approval sheet all13layers and actual48px object readability strip.
board=Image.new('RGB',(1280,1150),navy);draw=ImageDraw.Draw(board)
for i,r in enumerate(records):
 im=assets[r['slug']].copy();im.thumbnail((215,220));x=(i%5)*256;y=(i//5)*290;board.paste(im,(x+(256-im.width)//2,y+12+(220-im.height)//2),im);draw.text((x+10,y+245),r['slug'],fill='white');draw.text((x+10,y+263),str(r['size']),fill='#aabbd4')
objs=[r for r in records if not r['light']]
for i,r in enumerate(objs):
 im=assets[r['slug']].resize((48,48),Image.Resampling.LANCZOS);board.paste(im,(35+i*135,960),im);draw.text((22+i*135,1025),r['slug'].split('/')[-1],fill='white')
draw.text((30,1105),'BOTTOM ROW: actual48px object test / all VFX lights white for code tint',fill='white');board.save(C/'063-board.png')
def tinted(im,color,opacity=1):o=Image.new('RGBA',im.size,color);o.putalpha(im.getchannel('A').point(lambda a:round(a*max(0,min(1,opacity)))));return o
def place(dst,im,center,size=None,angle=0,opacity=1,color=None):
 im=im.copy()
 if size:im=im.resize(size if isinstance(size,tuple) else (size,size),Image.Resampling.LANCZOS)
 if color:im=tinted(im,color,opacity)
 elif opacity!=1:im.putalpha(im.getchannel('A').point(lambda a:round(a*max(0,min(1,opacity)))))
 if angle:im=im.rotate(angle,Image.Resampling.BICUBIC,expand=True)
 dst.alpha_composite(im,(round(center[0]-im.width/2),round(center[1]-im.height/2)))
def gif(frames,path,ms):
 sample=Image.new('RGB',(512,512*len(frames)))
 for i,f in enumerate(frames):sample.paste(f.convert('RGB'),(0,512*i))
 pal=sample.quantize(colors=255);ims=[f.convert('RGB').quantize(palette=pal,dither=Image.Dither.NONE) for f in frames];ims[0].save(path,save_all=True,append_images=ims[1:],duration=ms,loop=0,disposal=2)
unit=Image.open(R/'Assets/_Project/Art/UnitsSimple/human_common/idle_000.png').convert('RGBA');unit=unit.crop(unit.getchannel('A').getbbox());unit.thumbnail((155,190))
frames=[]
for f in range(22):
 t=f*.04;o=Image.new('RGBA',(512,512),navy);fade=1-float(smooth(.68,.88,t));appear=float(smooth(0,.10,t));place(o,assets['LevelUp/lu-ground'],(256,398),(260,87),opacity=appear*fade,color='#ffd56d')
 if t>.06:place(o,assets['LevelUp/lu-pillar'],(256,270),(210,420),opacity=float(smooth(.06,.20,t))*fade*.58,color='#ffd16a')
 o.alpha_composite(unit,(256-unit.width//2,396-unit.height))
 for i in range(8):
  u=max(0,t-.16-i*.012);life=u/.6
  if 0<life<1:
   angle=(-160+i*320/7)*math.pi/180;vx=math.sin(angle)*190;vy=-500-abs(math.cos(angle))*100;pos=(256+vx*u,330+vy*u+700*u*u);size=22 if i%2 else 32;place(o,star,pos,size,angle=u*90,opacity=(1-life)*fade)
 if t>=.34:
  u=float(smooth(.34,.54,t));place(o,assets['LevelUp/lu-chevron'],(256,190-35*u),70,opacity=fade)
 frames.append(o)
gif(frames,C/'063-levelup.gif',40)
# Resolve real relic icon via serializedGUID, no screenshot crop.
guid='5e14d1f512db63ad96d6db87b9997d7d';found=[p for p in (R/'Assets').rglob('*.png.meta') if guid in p.read_text(errors='replace')];assert found;relicpath=Path(str(found[0])[:-5]);relic=Image.open(relicpath).convert('RGBA');relic=relic.crop(relic.getchannel('A').getbbox());relic.thumbnail((215,215));base=Image.new('RGBA',(512,512));base.alpha_composite(relic,(256-relic.width//2,256-relic.height//2))
frames=[]
for f in range(18):
 t=f*.04;o=Image.new('RGBA',(512,512),navy);o.alpha_composite(base)
 if t<=.35:
  sweepLayer=Image.new('RGBA',(512,512));place(sweepLayer,assets['Relic/shine-sweep'],(105+300*t/.35,256),(180,360),opacity=.8)
  al=np.array(sweepLayer.getchannel('A'),float)*np.array(base.getchannel('A'),float)/255;sweepLayer.putalpha(Image.fromarray(np.uint8(al)));o.alpha_composite(sweepLayer)
 if .30<=t<=.65:
  u=(t-.30)/.35;place(o,assets['Relic/glint'],(315,180),round(35+30*math.sin(u*math.pi)),opacity=math.sin(u*math.pi))
 frames.append(o)
gif(frames,C/'063-relic.gif',40)
frames=[]
for f in range(44):
 t=f*.04;o=Image.new('RGBA',(512,512),navy)
 for j in range(20):
  x=20+(j*83)%470;y=-75+((j*41)%170)+t*(150+j%5*14);place(o,assets['Result/confetti-'+str(j%4)],(x+14*math.sin(t*3+j),y),18+j%3*4,angle=j*27+t*(50+j*4),opacity=.85)
 for j in range(3):
  center=(140+j*116,260);place(o,empty,center,108)
  u=(t-(.18+j*.22))/.24
  if u>=0:
   if u<.55:sc=1.35-.35*float(smooth(0,.55,u))
   else:sc=1-.08*math.sin(min(1,(u-.55)/.45)*math.pi) if u<1 else 1
   place(o,big,center,round(108*sc),opacity=min(1,u*4))
 frames.append(o)
gif(frames,C/'063-victory.gif',40)
manifest=dict(status='PREVIEW_PENDING_USER_APPROVAL',files=records,unit_preview=str((R/'Assets/_Project/Art/UnitsSimple/human_common/idle_000.png').relative_to(R)),relic_preview=str(relicpath.relative_to(R)),unity_run=False);(W/'manifest.json').write_text(json.dumps(manifest,indent=2));(C/'063-checks.json').write_text(json.dumps(dict(asset_count=len(records),light_rgb_white=True,light_centers_within_1px=True,all_edges_alpha_zero=True,star_empty_alpha_matches_big=True,object_readability_preview='063-board.png bottom48px row',runtime_imported=False,unity_run=False),indent=2));print(len(records),'layers;3GIFs;board+48px strip;runtime untouched')
