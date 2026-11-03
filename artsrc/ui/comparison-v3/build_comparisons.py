from pathlib import Path
from PIL import Image
import base64,io,shutil
root=Path('/Users/vuhagiabao/SpinsquadAgain/artsrc/ui/comparison-v3')
out=Path('/Users/vuhagiabao/.codex/visualizations/2026/09/30/01a0f0cc-8798-79b0-af42-83a55131c482')
generated=Path('/Users/vuhagiabao/.codex/generated_images/01a0f0cc-8798-79b0-af42-83a55131c482')
rows=[('recipe','Recipe · Mythic Allies','exec-08ad4afe-134c-44e3-a218-780c44a83b19.png'),('item','Recipe · Item','exec-1cc6b68d-011f-4ba7-a4fa-d7d4f3cec13e.png'),('settings','Settings','exec-a7d804d4-6894-4184-addd-ccbe09fe0266.png'),('contribution','Contribution','exec-5e1a8980-1946-44e3-a29b-f602b3dd2e22.png')]
def embed(p):
    im=Image.open(p).convert('RGB');buf=io.BytesIO();im.save(buf,format='JPEG',quality=80,optimize=True)
    return 'data:image/jpeg;base64,'+base64.b64encode(buf.getvalue()).decode()
template=(root/'compare-template.html').read_text()
for key,title,src in rows:
    new=root/(key+'-proposal.png');shutil.copy2(generated/src,new)
    html=template.replace('KEY',key).replace('SCREEN',title).replace('OLD_IMAGE',embed(root/(key+'-current-full.png'))).replace('NEW_IMAGE',embed(new))
    path=out/('ui-compare-'+key+'.html');path.write_text(html)
    print(key,len(html.encode()),Image.open(root/(key+'-current-full.png')).size,Image.open(new).size)
