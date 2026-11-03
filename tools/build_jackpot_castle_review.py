from pathlib import Path
from PIL import Image
import io,base64,json
root=Path(__file__).resolve().parents[1]
paths={'bg':'Assets/_Project/Art/Backgrounds/bg_prep_portrait.png','human':'Assets/_Project/Art/UnitsSimple/human_legendary/idle_000.png','elf':'Assets/_Project/Art/UnitsSimple/elf_legendary/idle_000.png','machine':'Assets/_Project/Art/UnitsSimple/machine_legendary/idle_000.png','roll':'Assets/_Project/Resources/SharedUI/roll.png','battle':'Assets/_Project/Resources/SharedUI/battle.png','ribbon':'artsrc/ui/jackpot-v2/ribbon.png','fireworks':'artsrc/ui/jackpot-v2/fireworks.png'}
data={}
paths['castle']='artsrc/ui/jackpot-v3/castle.png'
for k,p in paths.items():
 im=Image.open(root/p).convert('RGBA')
 if k in ['ribbon','castle']: im=im.crop(im.getchannel('A').point(lambda a:255 if a>15 else 0).getbbox())
 im.thumbnail((800,900) if k=='bg' else (900,600) if k=='ribbon' else (768,512) if k=='fireworks' else (160,160))
 b=io.BytesIO();im.save(b,format='WEBP',quality=82);data[k]='data:image/webp;base64,'+base64.b64encode(b.getvalue()).decode()
out=Path('/Users/vuhagiabao/.codex/visualizations/2026/09/30/01a0f0cc-8798-79b0-af42-83a55131c482/jackpot-castle-review.html')
out.write_text((root/'tools/jackpot_castle_review.template.html').read_text().replace('__ASSETS__',json.dumps(data,separators=(',',':'))))
assert out.stat().st_size<1000000
print(out,out.stat().st_size)
