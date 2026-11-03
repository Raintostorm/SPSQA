"""Read-only image analysis; write Sprite.Create rectangles, never change PNG alpha."""
import json
from pathlib import Path
import numpy as np
from PIL import Image
from scipy import ndimage

root=Path(__file__).resolve().parents[1]
folder=root/'Assets/_Project/Resources/ChapterOneSlimes'
for path in folder.glob('*.png'):
    im=Image.open(path).convert('RGBA'); w,h=im.size
    six=path.stem.startswith('green_') or path.stem.endswith('_bite')
    cols,rows=(3,2) if six else (4,4)
    labels,n=ndimage.label(np.array(im)[:,:,3]>220)
    groups=[[] for _ in range(cols*rows)]
    for ident,sl in enumerate(ndimage.find_objects(labels),1):
        if sl is None:continue
        y,x=sl
        if (labels[sl]==ident).sum()<100:continue
        cx=(x.start+x.stop)/2;cy=(y.start+y.stop)/2
        slot=min(rows-1,int(cy/h*rows))*cols+min(cols-1,int(cx/w*cols))
        groups[slot].append((x.start,y.start,x.stop,y.stop))
    entries=[]
    for slot,boxes in enumerate(groups):
        if not boxes:raise RuntimeError(f'{path.name}: missing pose {slot}')
        # Largest component is body. Tiny same-cell projectile is included in rect.
        body=max(boxes,key=lambda b:(b[2]-b[0])*(b[3]-b[1]))
        x=max(0,min(b[0] for b in boxes)-3);top=max(0,min(b[1] for b in boxes)-3)
        right=min(w,max(b[2] for b in boxes)+3);bottom=min(h,max(b[3] for b in boxes)+3)
        entries.append(dict(x=x,y=h-bottom,w=right-x,h=bottom-top,
            pivotX=((slot%cols+.5)*w/cols-x)/(right-x),
            pivotY=(bottom-body[3])/(bottom-top),bodyHeight=body[3]-body[1],bodyBottom=body[3]))
    # Walk hops retain lift relative to grounded poses in each logical row.
    for slot,e in enumerate(entries):
        walk=path.stem=='green_walk' or (not six and slot//cols==1)
        if walk:
            ground=max(entries[k]['bodyBottom'] for k in range(slot//cols*cols,(slot//cols+1)*cols))
            if path.stem=='green_walk':ground=max(q['bodyBottom']%(h/rows) for q in entries)+slot//cols*h/rows
            lift=round(ground-e['bodyBottom'])
            oldh=e['h'];oldpivot=e['pivotY']*oldh
            e['y']-=lift;e['h']+=lift
            e['pivotY']=oldpivot/e['h']
    (folder/(path.stem+'_layout.json')).write_text(json.dumps({'frames':entries},indent=2))
    print(path.name,len(entries),'poses',flush=True)
