from PIL import Image
from collections import deque
import json
root='artsrc/units/mythic-guardian-v1/'
result=[];audit=[]
for name,limit in [('base',12),('skill',4),('dead',4)]:
 im=Image.open(root+'v4/'+name+'.png');a=im.getchannel('A');w,h=im.size;mask=bytearray(v>100 for v in a.getdata());comps=[]
 for k in range(w*h):
  if not mask[k]:continue
  mask[k]=0;q=deque([k]);n=0;x0=w;y0=h;x1=y1=0
  while q:
   j=q.popleft();x=j%w;y=j//w;n+=1;x0=min(x0,x);y0=min(y0,y);x1=max(x1,x+1);y1=max(y1,y+1)
   for nx,ny in [(x-1,y),(x+1,y),(x,y-1),(x,y+1)]:
    if 0<=nx<w and 0<=ny<h:
     z=ny*w+nx
     if mask[z]:mask[z]=0;q.append(z)
  if n>100:comps.append((n,x0,y0,x1,y1))
 if name=='base':
  selected=[]
  for low,high in [(0,280),(280,550),(550,820)]:selected+=sorted([c for c in comps if low<=c[2]<high],key=lambda c:c[1])
 else:selected=sorted([c for c in comps if c[2]<650],key=lambda c:c[1])+sorted([c for c in comps if c[2]>=650],key=lambda c:c[1])
 assert len(selected)==limit,(name,len(selected))
 for n,x0,y0,x1,y1 in selected:
  x0=max(0,x0-1);y0=max(0,y0-1);x1=min(w,x1+1);y1=min(h,y1+1)
  intruders=[c for c in comps if c not in selected or c!=(n,x0+1,y0+1,x1-1,y1-1)]
  # Compare actual significant-alpha pixels in crop with component pixel count.
  count=sum(v>100 for v in a.crop((x0,y0,x1,y1)).getdata())
  assert count==n,(name,(x0,y0,x1,y1),count,n)
  result.append([x0,y0,x1-x0,y1-y0]);audit.append({'source':name,'rect':[x0,y0,x1-x0,y1-y0],'significantPixels':n,'extraPixelsFromOtherFrames':count-n})
json.dump(result,open(root+'v6/bounds.json','w'));json.dump(audit,open(root+'v6/crop-audit.json','w'),indent=2)
print('PASS: 20 crops, each contains all significant pixels of exactly one complete character; 0 neighboring-frame pixels.')
