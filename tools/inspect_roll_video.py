"""Read-only source-video contact sheets for animation review."""
import cv2
from PIL import Image, ImageDraw
from pathlib import Path
import sys

source=sys.argv[1];out=Path('/private/tmp/spinsquad-video-review');out.mkdir(exist_ok=True)
cap=cv2.VideoCapture(source)
fps=cap.get(cv2.CAP_PROP_FPS);seconds=cap.get(cv2.CAP_PROP_FRAME_COUNT)/fps
interval=float(sys.argv[2]) if len(sys.argv)>2 else 3
start=float(sys.argv[3]) if len(sys.argv)>3 else 0
end=float(sys.argv[4]) if len(sys.argv)>4 else seconds
samples=[];t=start
while t<end:
 cap.set(cv2.CAP_PROP_POS_MSEC,t*1000);ok,frame=cap.read()
 if not ok:break
 im=Image.fromarray(cv2.cvtColor(frame,cv2.COLOR_BGR2RGB))
 if len(sys.argv)>5: im=im.crop(tuple(map(int,sys.argv[5].split(','))))
 im.thumbnail((280,410 if len(sys.argv)>5 else 210))
 samples.append((t,im));t+=interval
for offset in range(0,len(samples),20):
 group=samples[offset:offset+20];cellh=440 if len(sys.argv)>5 else 240
 sheet=Image.new('RGB',(1120,cellh*((len(group)+3)//4)), '#222222');draw=ImageDraw.Draw(sheet)
 for i,(t,im)in enumerate(group):
  x=i%4*280;y=i//4*cellh;sheet.paste(im,(x,y+24));draw.text((x+4,y+4),f'{t:.2f}s',fill='white')
 path=out/f'sheet-{start:g}-{interval:g}-{offset//20}.jpg';sheet.save(path);print(path)
print('duration',seconds,'fps',fps)
