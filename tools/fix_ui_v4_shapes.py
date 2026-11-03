"""User-authorised code repair: rounded geometry, AA alpha and ordered names.

Never stretch the existing corner pixels or threshold the delivered alpha.
Does not write Unity Assets.
"""
from pathlib import Path
import json
import numpy as np
from PIL import Image, ImageDraw
from scipy import ndimage

ROOT=Path(__file__).resolve().parents[1]
SRC=ROOT/'artsrc/ui/ui_v4_shapes.png'
BACKUP=ROOT/'artsrc/ui/ui_v4_shapes_before_code_fix.png'
OUT=ROOT/'artsrc/ui/ui_v4_parts'
NAMES=['btn_primary','btn_primary_down','btn_secondary','btn_secondary_down',
       'btn_disabled','btn_square','pill_bar','card_common','card_rare',
       'card_epic','card_legendary','grid_cell']

def rounded(w,h,r,palette,stroke=10,opacity=1):
    """Native curves at 4x resolution, then area-resampled AA."""
    s=4
    canvas=Image.new('RGBA',(w*s,h*s))
    draw=ImageDraw.Draw(canvas)
    draw.rounded_rectangle((0,0,w*s-1,h*s-1),r*s,fill=palette[0])
    mask=Image.new('L',canvas.size)
    md=ImageDraw.Draw(mask)
    md.rounded_rectangle((stroke*s,stroke*s,(w-stroke)*s-1,(h-stroke)*s-1),max(1,r-stroke)*s,fill=255)
    fill=Image.new('RGBA',canvas.size,palette[2])
    fd=ImageDraw.Draw(fill)
    split=round((stroke+.4*(h-2*stroke))*s)
    fd.rectangle((0,0,w*s,split),fill=palette[1])
    # Flat thin ledges, clipped to the same inner native curve.
    fd.rectangle((0,stroke*s,w*s,(stroke+6)*s),fill=palette[3])
    fd.rectangle((0,(h-stroke-6)*s,w*s,(h-stroke)*s),fill=palette[4])
    if opacity!=1:
        mask=mask.point(lambda a: round(a*opacity))
        # Erase underlying opaque black inside; keep the outline opaque.
        interior=Image.new('L',canvas.size)
        ImageDraw.Draw(interior).rounded_rectangle((stroke*s,stroke*s,(w-stroke)*s-1,(h-stroke)*s-1),max(1,r-stroke)*s,fill=255)
        canvas.paste((0,0,0,0),(0,0),interior)
    fill.putalpha(mask)
    canvas.alpha_composite(fill)
    return canvas.resize((w,h),Image.Resampling.LANCZOS)

def main():
    if not BACKUP.exists():
        BACKUP.write_bytes(SRC.read_bytes())
    source=Image.open(BACKUP).convert('RGBA')
    a=np.array(source)
    lab,_=ndimage.label(a[:,:,3]>5)
    boxes=[]
    for ident,sl in enumerate(ndimage.find_objects(lab),1):
        if sl is not None and (lab[sl]==ident).sum()>1000:
            y,x=sl; boxes.append((x.start,y.start,x.stop,y.stop))
    assert len(boxes)==12,len(boxes)
    # Three well-separated row bands, then left-to-right: never x-only sorting.
    boxes.sort(key=lambda b:(round(b[1]/260),b[0]))
    parts=[source.crop(b) for b in boxes]
    parts[5]=rounded(184,184,37,[(0,0,0,255),(73,79,87,255),(44,48,54,255),(111,117,125,255),(25,28,32,255)])
    parts[6]=rounded(525,184,92,[(0,0,0,255),(40,51,67,255),(28,36,48,255),(79,92,110,255),(14,20,29,255)],opacity=.85)
    # Faint grid is a quiet overlay, not a solid disabled button.
    s=4; cell=Image.new('RGBA',(184*s,184*s))
    cd=ImageDraw.Draw(cell)
    cd.rounded_rectangle((0,0,184*s-1,184*s-1),37*s,fill=(0,0,0,51))
    cd.rounded_rectangle((2*s,2*s,182*s-1,182*s-1),35*s,fill=(0,0,0,32))
    parts[11]=cell.resize((184,184),Image.Resampling.LANCZOS)
    OUT.mkdir(parents=True,exist_ok=True)
    sheet=Image.new('RGBA',source.size)
    entries=[]
    for i,(name,part,b) in enumerate(zip(NAMES,parts,boxes)):
        part.save(OUT/(name+'.png'))
        cx=(b[0]+b[2])/2; cy=(b[1]+b[3])/2
        x=round(cx-part.width/2);y=round(cy-part.height/2)
        sheet.alpha_composite(part,(x,y))
        entries.append({'index':i+1,'name':name,'file':name+'.png','rect':[x,y,part.width,part.height]})
    sheet.save(SRC)
    (OUT/'manifest.json').write_text(json.dumps(entries,indent=2)+'\n')
    # Verify pixel geometry / intended alpha without altering assets.
    assert parts[5].size==parts[11].size==(184,184)
    pill=np.array(parts[6]);assert pill[92,262,3]==217
    assert pill[92,1,3]>240
    assert np.array(parts[11])[92,92,3]==32
    # View faint overlays on both bright and dark backgrounds.
    preview=Image.new('RGBA',(source.width,source.height*2))
    for row,color in enumerate([(237,229,199,255),(42,55,45,255)]):
        bg=Image.new('RGBA',source.size,color);bg.alpha_composite(sheet)
        preview.alpha_composite(bg,(0,row*source.height))
    preview.save(ROOT/'artsrc/ui/ui_v4_alpha_preview.png')
    print(json.dumps({'parts':len(parts),'square':[184,184],'pill_face_alpha':217,'grid_fill_alpha':32,'grid_edge_alpha':51}))

if __name__=='__main__':main()
