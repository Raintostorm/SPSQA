"""Mechanical six-cell import; retain generated alpha and fixed canvas anchors."""
from pathlib import Path
from PIL import Image
import re
import hashlib

root = Path(__file__).resolve().parents[1]
template = (root/'Assets/_Project/Art/UnitsSimple/dead_legendary/skill_010.png.meta').read_text()
for name in ['elf-frost-rain', 'human-shockwave', 'machine-shield', 'monster-roar', 'dead-summon']:
    sheet = Image.open(root/f'artsrc/vfx/legend-skills-v4/{name}.png').convert('RGBA')
    out = root/f'Assets/_Project/Resources/SkillVfx/{name}'
    out.mkdir(parents=True, exist_ok=True)
    for i in range(6):
        path = out/f'frame_{i}.png'
        sheet.crop((i%3*512,i//3*512,(i%3+1)*512,(i//3+1)*512)).save(path)
        data = re.sub(r'^guid: .*', 'guid: '+hashlib.md5(f'legend-skill-v4/{name}/{i}'.encode()).hexdigest(),template,flags=re.M)
        data = re.sub(r'spritePixelsToUnits: .*','spritePixelsToUnits: 512',data)
        # Elliptical ground centres are below the canvas centre in source art.
        pivot = '.27' if name in ['elf-frost-rain','human-shockwave'] else '.5'
        if name == 'dead-summon': pivot = '.14'
        data = re.sub(r'spritePivot: .*',f'spritePivot: {{x: 0.5, y: {pivot}}}',data)
        data = re.sub(r'spriteAlignment: .*','spriteAlignment: 9',data)
        Path(str(path)+'.meta').write_text(data)
    print(name, '6 sprites imported')
