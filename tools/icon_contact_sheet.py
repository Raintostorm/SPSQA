"""Dan icon thanh mot tam de soi bang mat, o co gan voi co hien thi that.

Vi sao can: icon trong khong co o co goc 512px, nhung game chi ve chung o 48-170px.
Loi "chi tiet nat thanh lam tam" chi lo ra o co that. Xem file goc la xem nham co.

Dung:  python3 tools/icon_contact_sheet.py <file-ra.png> <thu-muc-trong-Resources> ...
Vi du: python3 tools/icon_contact_sheet.py /tmp/a.png StatIcons SharedUI HomeChrome
"""
import sys, os
from PIL import Image, ImageDraw
R='Assets/_Project/Resources/'
out=sys.argv[1]; folders=sys.argv[2:]
items=[]
for f in folders:
    d=os.path.join(R,f)
    for n in sorted(os.listdir(d)):
        if n.endswith('.png') or n.endswith('.bytes'):
            try: im=Image.open(os.path.join(d,n)).convert('RGBA')
            except Exception: continue
            items.append((f+'/'+n.rsplit('.',1)[0], im))
CELL,PAD=104,10
cols=8
rows=(len(items)+cols-1)//cols
sheet=Image.new('RGBA',(cols*(CELL+PAD)+PAD,rows*(CELL+PAD+14)+PAD),(46,36,72,255))
dr=ImageDraw.Draw(sheet)
for i,(name,im) in enumerate(items):
    c,r=i%cols,i//cols
    x=PAD+c*(CELL+PAD); y=PAD+r*(CELL+PAD+14)
    t=im.copy(); t.thumbnail((CELL,CELL),Image.LANCZOS)
    sheet.alpha_composite(t,(x+(CELL-t.width)//2,y+(CELL-t.height)//2))
    dr.text((x,y+CELL+2),name.split('/')[-1][:16],fill=(200,200,220,255))
sheet.save(out); print(len(items),'icons ->',out)
