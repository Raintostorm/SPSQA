from PIL import Image
from collections import deque
from pathlib import Path
import json,sys
for name in sys.argv[1:]:
 p=Path('artsrc/units/mythic-newcomers-v1');im=Image.open(p/(name+'.png'));w,h=im.size;a=im.getchannel('A');mask=bytearray(v>100 for v in a.getdata());cs=[]
 for k in range(w*h):
  if not mask[k]:continue
  mask[k]=0;q=deque([k]);pts=[];x0=w;y0=h;x1=y1=0
  while q:
   j=q.popleft();pts.append(j);x=j%w;y=j//w;x0=min(x0,x);y0=min(y0,y);x1=max(x1,x+1);y1=max(y1,y+1)
   for nx,ny in [(x-1,y),(x+1,y),(x,y-1),(x,y+1)]:
    if 0<=nx<w and 0<=ny<h:
     z=ny*w+nx
     if mask[z]:mask[z]=0;q.append(z)
  if len(pts)>100:cs.append({'pixels':pts,'rect':[x0,y0,x1-x0,y1-y0]})
 print(name,im.size,[(len(c['pixels']),c['rect']) for c in cs if len(c['pixels'])>1000])
 json.dump([{'count':len(c['pixels']),'rect':c['rect']} for c in cs],open(p/(name+'-audit.json'),'w'))
 # Full bodies should dominate isolated tiny particles; missing bodies trigger repair.
 main=[c for c in cs if len(c['pixels'])>6000]
 ncols,nrows={'clock-skill':(2,2)}.get(name,(4,4 if name in ['undead','bear-extra'] else 5))
 assert len(main)==ncols*nrows,(name,'expected bodies',ncols*nrows,'found',len(main))
 # Order each row by its foot/bottom y, avoids raised weapons changing min-y order.
 # Keep disconnected ornaments (Elf crescent) with their own body, not discarded.
 for part in cs:
  if part in main:continue
  x,y,pw,ph=part['rect'];cx=x+pw/2;cy=y+ph/2
  def distance(c):
   a,b,cw,ch=c['rect'];return max(a-cx,0,cx-a-cw)**2+max(b-cy,0,cy-b-ch)**2+.001*(cx-a-cw/2)**2
  closest=min(main,key=distance);closest['pixels']+=part['pixels']
 # Sort original body bounds first, ornaments may raise top of frame.
 main.sort(key=lambda c:c['rect'][1]+c['rect'][3]);ordered=[]
 for r in range(nrows):ordered+=sorted(main[r*ncols:r*ncols+ncols],key=lambda c:c['rect'][0])
 out=[]
 for c in ordered:
  x0=min(j%w for j in c['pixels']);y0=min(j//w for j in c['pixels']);x1=max(j%w for j in c['pixels'])+1;y1=max(j//w for j in c['pixels'])+1;cw=x1-x0;ch=y1-y0;c['rect']=[x0,y0,cw,ch];rows={} 
  for j in c['pixels']:rows.setdefault(j//w,[]).append(j%w)
  spans=[]
  for y,xs in rows.items():
   xs.sort();start=last=xs[0]
   for x in xs[1:]:
    if x>last+1:spans.append([start-x0,y-y0,last-start+1]);start=x
    last=x
   spans.append([start-x0,y-y0,last-start+1])
  # Ground contact is lowest substantial span, x median of bottom body pixels.
  bottoms=[j%w for j in c['pixels'] if j//w>=y0+ch-8];bottoms.sort();px=bottoms[len(bottoms)//2] if bottoms else x0+cw/2
  out.append({'rect':c['rect'],'spans':spans,'pivot':[px-x0,ch-2]})
 json.dump(out,open(p/(name+'-clips.json'),'w'))
 print('PASS',name,len(out),'isolated complete body components; per-frame clipping excludes all neighboring components')
