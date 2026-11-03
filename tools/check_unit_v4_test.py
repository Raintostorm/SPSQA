"""Read-only diagnostics; never recolour/threshold the delivered asset."""
import json
import sys
import numpy as np
from PIL import Image
from scipy import ndimage

im=np.array(Image.open(sys.argv[1]).convert('RGBA'))
mask=im[:,:,3]>128
lab,_=ndimage.label(mask)
boxes=[]
for ident,sl in enumerate(ndimage.find_objects(lab),1):
    if sl is not None and (lab[sl]==ident).sum()>1000:
        y,x=sl
        boxes.append([x.start,y.start,x.stop,y.stop])
boxes.sort()
rgb=im[:,:,:3][im[:,:,3]>250]
colors,counts=np.unique(rgb,axis=0,return_counts=True)
order=np.argsort(-counts)
result={'size':[im.shape[1],im.shape[0]],'components':len(boxes),'boxes':boxes,
        'transparent_pixels':int((im[:,:,3]==0).sum()),
        'antialiased_pixels':int(((im[:,:,3]>0)&(im[:,:,3]<255)).sum()),
        'near_black_percent':round(100*(rgb.max(axis=1)<25).mean(),2),
        'colors_above_2_percent':[{'rgb':colors[i].tolist(),'percent':round(100*counts[i]/len(rgb),2)} for i in order if counts[i]/len(rgb)>.02]}
if boxes:
    result['baseline_spread_px']=max(b[3] for b in boxes)-min(b[3] for b in boxes)
    result['baseline_spread_percent']=round(100*result['baseline_spread_px']/np.median([b[3]-b[1] for b in boxes]),2)
print(json.dumps(result,indent=2))
