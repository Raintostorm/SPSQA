"""Combine existing full demos without changing or repainting sprite art."""
from pathlib import Path
import argparse
from bisect import bisect_right
from PIL import Image, ImageDraw

root = Path(__file__).resolve().parents[1] / 'artsrc/units_simple'
parser = argparse.ArgumentParser()
parser.add_argument('--tier', choices=['common', 'rare', 'epic', 'legendary'], default='common')
parser.add_argument('--revision', type=int, default=1)
args = parser.parse_args()
names = (['dead_common_trial_v1', 'monster_common_trial_v1', 'machine_common_trial_v1']
         if args.tier == 'common' else [race+f'_{args.tier}_trial_v{args.revision}' for race in ['human','elf','dead','monster','machine']])
gifs = [Image.open(root / name / 'full_demo.gif') for name in names]
sequences, endpoints = [], []
for gif in gifs:
    pictures, ends, elapsed = [], [], 0
    for i in range(gif.n_frames):
        gif.seek(i)
        pictures.append(gif.convert('RGB').crop((0, 0, 400, 360)))
        elapsed += gif.info.get('duration', 30)
        ends.append(elapsed)
    sequences.append(pictures)
    endpoints.append(ends)
frames, durations = [], []
height = 360 if args.tier=='common' else 720
for elapsed in range(0, max(ends[-1] for ends in endpoints), 30):
    canvas = Image.new('RGB', (1200, height), (225,237,216))
    for column, name in enumerate(names):
        index=min(bisect_right(endpoints[column],elapsed),len(sequences[column])-1)
        panel = sequences[column][index].copy()
        draw = ImageDraw.Draw(panel)
        draw.rectangle((0, 0, 400, 40), fill=(225, 237, 216))
        draw.text((20, 18), name.split('_trial')[0].upper() + ' / REVIEW', fill=(25, 30, 35))
        canvas.paste(panel, ((column%3)*400, (column//3)*360))
    frames.append(canvas)
    durations.append(30)
rare_name=f'{args.tier}_group_demo.gif' if args.revision==1 else f'{args.tier}_group_demo_v{args.revision}.gif'
frames[0].save(root / ('common_remaining_demo.gif' if args.tier=='common' else rare_name), save_all=True,
               append_images=frames[1:], duration=durations, loop=0, disposal=2)
print(args.tier, 'group demo written; source art untouched.')
