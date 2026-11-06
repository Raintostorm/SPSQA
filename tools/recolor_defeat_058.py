#!/usr/bin/env python3
"""Exact-shape recolor of ORIGINAL Defeat; preview only, never writes runtime atlas."""
from pathlib import Path
from PIL import Image,ImageDraw
import numpy as np,json,hashlib
R=Path(__file__).resolve().parents[1];p=R/'Assets/_Project/Resources/ResultScreen/atlas.png'
full=Image.open(p).convert('RGBA');rect=(793,full.height-669-216,1500,full.height-669);old=full.crop(rect);a=np.array(old);out=a.copy()
# Transfer original illumination to slate base. Preserve black outlines and alpha exactly.
rgb=a[:,:,:3].astype(float);lum=rgb[:,:,0]*.2126+rgb[:,:,1]*.7152+rgb[:,:,2]*.0722;colored=(a[:,:,3]>0)&((rgb.max(axis=2)-rgb.min(axis=2))>20)&(lum>30)
base=np.array([0x4a,0x55,0x70],float);reference=125.0
mapped=np.clip(base[None,None,:]*(lum[:,:,None]/reference),0,255).round().astype('uint8');out[colored,:3]=mapped[colored]
new=Image.fromarray(out);assert np.array_equal(out[:,:,3],a[:,:,3]);assert np.array_equal(out[~colored],a[~colored])
patched=full.copy();patched.paste(new,rect[:2]);patched.save(R/'artsrc/ui/anim-057/atlas-candidate-058.png')
outside=np.array(full)!=np.array(patched);outside[rect[1]:rect[3],rect[0]:rect[2]]=False;assert not np.any(outside)
dest=R/'artsrc/ui/anim-057/defeat-slate-v2.png';new.save(dest)
canvas=Image.new('RGB',(1480,290),'#162841');d=ImageDraw.Draw(canvas)
for i,(im,title) in enumerate([(old,'ORIGINAL PURPLE'),(new,'SLATE #4A5570 - SAME SHAPE')]):canvas.paste(im,(i*740+16,16),im);d.text((i*740+18,255),title,fill='white')
canvas.save(R/'chat/check/058-defeat-slate-v2.png')
proof={'source':str(p.relative_to(R)),'source_sha256':hashlib.sha256(p.read_bytes()).hexdigest(),'unity_rect':[793,669,707,216],'size':new.size,'alpha_identical':True,'silhouette_changed_pixels':int(np.count_nonzero(a[:,:,3]!=out[:,:,3])),'unchanged_outline_and_uncolored_pixels':True,'method':'original per-pixel luminance transferred onto slate RGB(74,85,112); no geometry resampling, no gold trim, no new highlights','runtime_atlas_modified':False,'pending_user_approval':True}
(R/'chat/check/058-defeat-proof.json').write_text(json.dumps(proof,indent=2));print('Defeat recolored: same alpha pixel-for-pixel, original outline retained; atlas untouched.')
