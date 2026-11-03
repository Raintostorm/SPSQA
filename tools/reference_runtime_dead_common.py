"""Build exact runtime-frame reference, without cropping or changing originals."""
from pathlib import Path
import re
import sys
from PIL import Image

root=Path(__file__).resolve().parents[1]
unit_name=sys.argv[1] if len(sys.argv)>1 else 'dead_common'
assert re.fullmatch(r'(human|elf|dead|machine|monster)_(common|rare|epic|legendary)',unit_name)
unit=root/'Assets/_Project/Art/Units'/unit_name
lookup={re.search(r'^guid: (\w+)',p.read_text(),re.M)[1]:p.with_suffix('') for p in unit.glob('*.png.meta')}
text=(root/f'Assets/_Project/Data/Units/Anim_{unit_name}.asset').read_text()
out=root/'artsrc/units_v2'
for name in ['Idle','Walk','Attack','Dead']:
    block=re.search(r'^  '+name+r':\n(.*?)(?=^  \w+:|\Z)',text,re.M|re.S)[1]
    guids=re.findall(r'fileID: 21300000, guid: (\w+)',block)
    images=[Image.open(lookup[g]).convert('RGBA') for g in guids]
    # Full rectangular source extents, never square cropping. Same scale.
    widths=[im.width for im in images]; height=max(im.height for im in images)
    canvas=Image.new('RGBA',(sum(widths)+100*(len(images)+1),height+200))
    x=100
    for im in images:
        canvas.alpha_composite(im,(x,100+height-im.height)); x+=im.width+100
    canvas.save(out/f'{unit_name}_runtime_{name.lower()}_reference.png')
    print(name,[(lookup[g].name,im.size) for g,im in zip(guids,images)])
