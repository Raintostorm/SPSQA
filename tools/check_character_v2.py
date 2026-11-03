"""Read-only metrics plus requested 50px/silhouette diagnostic previews.
No square cells, quantisation, alpha thresholding or edits to source art.
"""
from pathlib import Path
import json
import numpy as np
from PIL import Image
from scipy import ndimage

root = Path(__file__).resolve().parents[1]
path = root / 'artsrc/units_v2/dead_common_walk.png'
im = Image.open(path).convert('RGBA')
arr = np.array(im)
# Threshold only for measurement; preserve original antialiased alpha in previews.
labels, count = ndimage.label(arr[..., 3] > 32)
boxes = []
for i, sl in enumerate(ndimage.find_objects(labels), 1):
    if sl is not None and np.count_nonzero(labels[sl] == i) >= 1000:
        y, x = sl
        boxes.append((x.start, y.start, x.stop, y.stop))
boxes.sort()
heights = [b[3]-b[1] for b in boxes]
feet = [b[3] for b in boxes]
opaque = arr[..., 3] > 250
unique = len(np.unique(arr[opaque, :3], axis=0))
# No claim to implement the unspecified 'sharp outline' metric from prompt.
result = dict(size=im.size, major_components=len(boxes), boxes=boxes,
              foot_spread_px=max(feet)-min(feet) if feet else None,
              foot_spread_percent=100*(max(feet)-min(feet))/np.median(heights) if feet else None,
              opaque_unique_rgb=unique, alpha_levels=len(np.unique(arr[..., 3])),
              sharp_outline_percent=None,
              outline_note='Original metric algorithm/denominator not provided; not certified.')
print(json.dumps(result, indent=2))
out = path.parent
(out/'dead_common_walk_metrics.json').write_text(json.dumps(result, indent=2)+'\n')
if len(boxes) == 8:
    scale = 50 / max(heights)
    frames=[]
    for b in boxes:
        # Diagnostic crop has generous padding; NOT production frame slicing.
        crop=im.crop((max(0,b[0]-8), max(0,b[1]-8), min(im.width,b[2]+8),min(im.height,b[3]+8)))
        frames.append(crop.resize((round(crop.width*scale),round(crop.height*scale)),Image.Resampling.LANCZOS))
    preview=Image.new('RGBA',(sum(f.width for f in frames)+20*9,74),(230,235,240,255))
    silhouette=preview.copy()
    x=20
    for f in frames:
        preview.alpha_composite(f,(x,10))
        black=Image.new('RGBA',f.size,(0,0,0,0)); black.putalpha(f.getchannel('A'))
        silhouette.alpha_composite(black,(x,10)); x+=f.width+20
    preview.save(out/'dead_common_walk_50px.png')
    silhouette.save(out/'dead_common_walk_silhouette_50px.png')
