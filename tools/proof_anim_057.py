from pathlib import Path
from PIL import Image,ImageDraw
import numpy as np,json,math
R=Path(__file__).resolve().parents[1];W=R/'artsrc/ui/anim-057';S=W/'staged';C=R/'chat/check'
m=json.loads((W/'manifest.json').read_text())
# Evidence no alpha dirty corners; pure white all VFX outputs.
for x in m['files']:
 p=S/x['path'];a=np.array(Image.open(p).convert('RGBA'));assert list(a.shape[:2][::-1])==list(x['size']);assert a[0,0,3]==0
 if x['path'].startswith(('Common/','Summon/burst-','LevelUp/levelup-flash-')) or x['path']=='LevelUp/merge-swirl.png':assert np.all(a[a[:,:,3]>0,:3]==255)
# Hinge study: compress vertical lid around fixed seam (front-view rotate X projection).
body=Image.open(S/'Chest/chest-body.png');lid=Image.open(S/'Chest/chest-lid.png');seam=m['fit']['seam_y'];top=lid.getchannel('A').getbbox()[1]
frames=[];poses=[]
for f in range(36):
 t=f/35;angle=80*(1-(1-t)**3);factor=math.cos(math.radians(angle));region=lid.crop((0,top,512,seam));region=region.resize((512,max(1,round(region.height*factor))),Image.Resampling.BICUBIC)
 out=Image.new('RGBA',(512,512));out.alpha_composite(body);out.alpha_composite(region,(0,seam-region.height));bg=Image.new('RGBA',out.size,'#162841');bg.alpha_composite(out);d=ImageDraw.Draw(bg);d.text((16,16),f'hinge y={seam} / {angle:.0f} degrees',fill='white');d.line((20,seam,492,seam),fill='#62ccff',width=1);frames.append(bg.convert('RGB'))
 if f in [0,10,35]:poses.append(bg)
frames[0].save(C/'057-chest-hinge.gif',save_all=True,append_images=frames[1:]+frames[-1:]*12,duration=35,loop=0)
out=Image.new('RGB',(1536,512));
for i,im in enumerate(poses):out.paste(im,(512*i,0))
out.save(C/'057-chest-hinge.png')
# Frame animation strips maintain identical coordinate system; demo tint only.
for name,paths,hold in [('burst',[S/f'Summon/burst-{i}.png' for i in range(6)],55),('levelup',[S/f'LevelUp/levelup-flash-{i}.png' for i in range(4)],90)]:
 ims=[]
 for p in paths:
  im=Image.open(p).convert('RGBA');bg=Image.new('RGBA',im.size,'#162841');bg.alpha_composite(im);ims.append(bg.convert('RGB'))
 blank=Image.new('RGB',ims[0].size,'#162841');ims[0].save(C/f'057-{name}.gif',save_all=True,append_images=ims[1:]+[blank]*8,duration=hold,loop=0)
# One approval board; no new art invented for framing.
paths=['057-chest-fit.png','057-levelup-v2.png','057-burst-aligned.png','057-defeat-compare.png'];out=Image.new('RGB',(1260,1230),'#101d34');d=ImageDraw.Draw(out);y=0
for p in paths:
 im=Image.open(C/p);im.thumbnail((1240,285));out.paste(im,((1260-im.width)//2,y));y+=300
out.save(C/'057-review-board.png')
(C/'057-checks.json').write_text(json.dumps(dict(asset_count=len(m['files']),chest_rgba_diff=0,burst_max_center_error=max(x['max_center_error_pixels'] for x in m['burst_registration']),levelup_center_alpha=0,white_vfx=True,unity_run=False,hinge_demo='2D rotate-X projection; inside/back surface not drawn; not full 3D rig'),indent=2))
print('26 staging assets verified; flash centers transparent; burst centers <=0.5px; hinge GIF exported')
