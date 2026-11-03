"""Crop the four generated buttons; retain alpha, existing GUIDs and 9-slice corners."""
import re
import shutil
from pathlib import Path
import numpy as np
from PIL import Image
from import_ui_v4 import split,frame_pair,corner_radius

ROOT=Path(__file__).resolve().parents[1]
pieces=split(ROOT/'artsrc/ui/buttons_v5.png',4)
pieces[:2]=frame_pair(*pieces[:2])
pieces[2:]=frame_pair(*pieces[2:])
names=['btn_primary','btn_primary_down','btn_blue','btn_blue_down']
pair_borders=[max(corner_radius(p[...,3]) for p in pieces[:2])+4,
              max(corner_radius(p[...,3]) for p in pieces[2:])+4]
out=ROOT/'Assets/_Project/Art/UI'
backup=ROOT/'outdata/buttons_v5_backup'
backup.mkdir(parents=True,exist_ok=True)
for name,arr in list(zip(names,pieces))+[('btn_small',pieces[2])]:
    path=out/(name+'.png')
    meta=Path(str(path)+'.meta')
    for old in (path,meta):
        if not (backup/old.name).exists(): shutil.copy2(old,backup/old.name)
    border=min(pair_borders[0 if name.startswith('btn_primary') else 1],min(arr.shape[:2])//2-1)
    Image.fromarray(arr.astype(np.uint8),'RGBA').save(path)
    text=meta.read_text()
    text=re.sub(r'spriteBorder: .*',f'spriteBorder: {{x: {border}, y: {border}, z: {border}, w: {border}}}',text)
    text=re.sub(r'enableMipMap: \d+','enableMipMap: 0',text)
    text=re.sub(r'filterMode: \d+','filterMode: 1',text)
    text=re.sub(r'textureCompression: \d+','textureCompression: 0',text)
    meta.write_text(text)
    print(f'{name}: {arr.shape[1]}x{arr.shape[0]}, slice {border}px, GUID retained')
