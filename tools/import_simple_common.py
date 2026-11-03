"""Install the five approved Common demos; preserve definitions and old art.

Motion is baked by the existing preview renderer, not recreated from GIFs.
Variable rectangles retain complete art, alpha and a shared world anchor.
"""
import hashlib
import json
import re
import shutil
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
TRIALS = [('human','bandit_trial_v5','bandit'),('elf','elf_common_trial_v2','elf'),
          ('dead','dead_common_trial_v1','skeleton'),('monster','monster_common_trial_v1','spider'),
          ('machine','machine_common_trial_v1','machine')]
if '--remaining' in sys.argv:
    TRIALS = [('human_rare','human_rare_trial_v3','spear'),('elf_rare','elf_rare_trial_v2','skeleton'),
              ('dead_rare','dead_rare_trial_v2','wisp'),('monster_rare','monster_rare_trial_v2','bear'),
              ('machine_rare','machine_rare_trial_v2','gunner'),
              ('human_epic','human_epic_trial_v1','mage'),('elf_epic','elf_epic_trial_v1','elf'),
              ('dead_epic','dead_epic_trial_v1','haunted'),('monster_epic','monster_epic_trial_v1','scorpion'),
              ('human_legendary','human_legendary_trial_v3','royal_plant'),
              ('elf_legendary','elf_legendary_trial_v1','verdant'),('dead_legendary','dead_legendary_trial_v1','king'),
              ('monster_legendary','monster_legendary_trial_v1','lion'),('machine_legendary','machine_legendary_trial_v2','titan_guard')]

def replace_field(text,key,value):
    result,n = re.subn(r'^'+re.escape(key)+r':.*$',key+': '+value,text,flags=re.M)
    if n != 1:
        raise RuntimeError(f'Missing/duplicate field {key}')
    return result

for race,trial,profile in TRIALS:
    unit = race if '--remaining' in sys.argv else race+'_common'
    dest = ROOT/'Assets/_Project/Art/UnitsSimple'/unit
    command = [sys.executable,str(ROOT/'tools/preview_simple_bandit.py'),
                    '--directory','artsrc/units_simple/'+trial,'--profile',profile,
                    '--effects','--expressive','--strong-fx','--export-unity',str(dest)]
    if unit.endswith('_legendary'): command.append('--skills')
    if '--idle-only' in sys.argv: command.append('--idle-only')
    subprocess.run(command,check=True)
    data = json.loads((dest/'frames.json').read_text())
    template = (ROOT/f'Assets/_Project/Art/Units/{unit}/idle_1.png.meta').read_text()
    clips = {}
    for clip,frames in data['clips'].items():
        if '--idle-only' in sys.argv and clip != 'idle': continue
        refs = []
        for frame in frames:
            guid = hashlib.md5(f'simple-approved/{unit}/{frame["file"]}'.encode()).hexdigest()
            meta = replace_field(template,'guid',guid)
            meta = replace_field(meta,'  spritePixelsToUnits',str(data['height']/.86))
            meta = replace_field(meta,'  spritePivot','{x: %.8f, y: %.8f}'%tuple(frame['pivot']))
            meta = replace_field(meta,'    enableMipMap','0')
            meta = replace_field(meta,'    filterMode','1')
            (dest/(frame['file']+'.meta')).write_text(meta)
            refs.append('    - {fileID: 21300000, guid: '+guid+', type: 3}')
        clips[clip] = '\n'.join(['  '+clip.capitalize()+':','    Frames:']+refs+
                      ['    Durations:']+['    - %.8f'%f['duration'] for f in frames]+
                      ['    Sequence:','    Loop: '+('1' if clip in ('idle','walk') else '0'),
                       '    UseExplicitPlayback: 1','    PingPong: 0'])+'\n'
    asset = ROOT/f'Assets/_Project/Data/Units/Anim_{unit}.asset'
    backup = ROOT/'outdata/simple_import_backup'/asset.name
    backup.parent.mkdir(parents=True,exist_ok=True)
    if not backup.exists(): shutil.copy2(asset,backup)
    # Match the previous idle's world height, not an arbitrary new unit size.
    old_guid = re.search(r'Idle:\n    Frames:\n    - .*guid: ([0-9a-f]+)',backup.read_text()).group(1)
    from PIL import Image
    old_height = None
    for old_meta in (ROOT/'Assets/_Project/Art/Units'/unit).glob('*.png.meta'):
        old_text = old_meta.read_text()
        if f'guid: {old_guid}' in old_text:
            old_image = Image.open(str(old_meta)[:-5]).convert('RGBA')
            box = old_image.getchannel('A').getbbox()
            old_height = (box[3]-box[1])/float(re.search(r'spritePixelsToUnits: ([\d.]+)',old_text).group(1))
            break
    if old_height is None: raise RuntimeError(f'Cannot measure old scale: {unit}')
    for new_meta in dest.glob('*.png.meta'):
        if '--idle-only' in sys.argv and not new_meta.name.startswith('idle_'): continue
        new_meta.write_text(replace_field(new_meta.read_text(),'  spritePixelsToUnits',str(data['height']/old_height)))
    text = asset.read_text()
    for clip,block in clips.items():
        text,n = re.subn(r'^  '+clip.capitalize()+r':\n.*?(?=^  \w+:|\Z)',lambda _:block,text,flags=re.M|re.S)
        if n!=1: raise RuntimeError(f'Clip missing {asset}/{clip}')
    for vfx in ('WalkVfx','AttackVfx'):
        if '--idle-only' in sys.argv: continue
        text = re.sub(r'^  '+vfx+r':\n.*?(?=^  \w+:|\Z)',
                      '  '+vfx+':\n    Frames: []\n    Durations: []\n    Sequence:\n    Loop: 0\n',text,flags=re.M|re.S)
    if '--idle-only' not in sys.argv: text = replace_field(text,'  ImpactStep','6')
    if '  BakedMotion:' in text: text=replace_field(text,'  BakedMotion','1')
    else: text+='  BakedMotion: 1\n'
    asset.write_text(text)
    print(f'Installed {unit}: {sum(len(x) for x in data["clips"].values())} frames; old definitions/GUIDs preserved')
