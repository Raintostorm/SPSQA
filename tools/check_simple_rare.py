"""Read-only checks: six silhouettes, unclipped extents and exact crop alpha."""
import json
import argparse
from pathlib import Path
import numpy as np
from PIL import Image
from scipy import ndimage
from import_pages_art import row_major

root = Path(__file__).resolve().parents[1] / 'artsrc/units_simple'
parser=argparse.ArgumentParser()
parser.add_argument('--revision',type=int,default=1)
parser.add_argument('--tier',choices=['rare','epic','legendary'],default='rare')
parser.add_argument('--race',choices=['human','elf','dead','monster','machine'])
args=parser.parse_args()
for race in ([args.race] if args.race else ['human', 'elf', 'dead', 'monster', 'machine']):
    folder = root / (race+f'_{args.tier}_trial_v{args.revision}')
    source = np.array(Image.open(folder/'poses.png').convert('RGBA'))
    labels, _ = ndimage.label(source[...,3]>5)
    parts=[]
    for index, bounds in enumerate(ndimage.find_objects(labels),1):
        if bounds is None or (labels[bounds]==index).sum()<1000:
            continue
        y,x=bounds
        parts.append((y.start,y.stop-1,x.start,x.stop-1,index))
    expected_count = 9 if args.tier=='legendary' else 6
    pose_names = ['idle','walk','windup','strike','recovery','hurt']
    if args.tier=='legendary':
        pose_names += ['skill_windup','skill_hit','skill_recovery']
    assert len(parts)==expected_count, (race,'unexpected source parts',len(parts))
    mask=source[...,3]>5
    assert not(mask[0].any() or mask[-1].any() or mask[:,0].any() or mask[:,-1].any()), (race,'touches sheet edge')
    for name,(y0,y1,x0,x1,index) in zip(pose_names,row_major(parts)):
        image=np.array(Image.open(folder/(name+'.png')).convert('RGBA'))
        box=(max(0,x0-4),max(0,y0-4),min(source.shape[1],x1+5),min(source.shape[0],y1+5))
        x,y,r,b=box
        expected=(labels[y:b,x:r]==index)
        assert (image[...,3]>5).sum()==expected.sum(), (race,name,'foreign or missing silhouette pixels')
        assert np.array_equal(image[...,3][expected],source[y:b,x:r,3][expected]), (race,name,'alpha altered')
        assert image[...,3].max()>=250, (race,name,'unexpectedly translucent body')
    rig=json.loads((folder/'rig.json').read_text())
    assert len(rig['pixel_pivots'])==expected_count
    clips = ['idle','walk','attack','dead','full'] + (['skill'] if args.tier=='legendary' else [])
    for clip in clips:
        with Image.open(folder/(clip+'_demo.gif')) as gif:
            assert gif.n_frames>1, (race,clip)
    print('PASS',race+'_'+args.tier+f': {expected_count} complete poses, exact silhouette alpha, no neighbouring parts; {len(clips)} demos')
