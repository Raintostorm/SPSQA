#!/usr/bin/env python3
"""Extract complete explicit regions and preview at fixed scale/root.

No asset writes: stage under outdata. Manifest declares source, ppu,
frames[{rect,anchor,duration}], impact_step. Never fit each pose to a box.
"""
import argparse
import json
from pathlib import Path
import numpy as np
from PIL import Image, ImageDraw
from validate_attack_regions import extract

ROOT = Path(__file__).resolve().parents[1]


def review(doc, output):
    src = np.array(Image.open(ROOT / doc['source']).convert('RGBA'))
    if not np.any(src[..., 3] == 0):
        raise ValueError('Real alpha required; no colour key is applied')
    poses = extract(src, doc['frames'])
    ppu = float(doc['ppu'])
    if ppu <= 0:
        raise ValueError('Positive shared PPU required')
    output.mkdir(parents=True, exist_ok=True)
    # One uniform display scale, including weapons. Union bounds only
    # choose the camera, not a different size or centre for each frame.
    scale = 240 / ppu
    bounds = []
    for pixels, (px, py) in poses:
        h, w = pixels.shape[:2]
        ys, xs = np.where(pixels[..., 3] > 20)
        # Ignore almost invisible generation specks for CAMERA size only.
        # Sprite files retain every alpha pixel; no per-frame resizing.
        bounds.append(((xs.min()-px*w)*scale, (ys.min()-(1-py)*h)*scale,
                       (xs.max()+1-px*w)*scale, (ys.max()+1-(1-py)*h)*scale))
    left = int(np.floor(min(b[0] for b in bounds))) - 30
    top = int(np.floor(min(b[1] for b in bounds))) - 45
    right = int(np.ceil(max(b[2] for b in bounds))) + 30
    bottom = int(np.ceil(max(b[3] for b in bounds))) + 45
    root = (-left, -top)
    frames = []
    durations = []
    for index, ((pixels, (px, py)), info) in enumerate(zip(poses, doc['frames'])):
        sprite = Image.fromarray(pixels)
        sprite.save(output / f'attack_{index+1}.png')
        canvas = Image.new('RGBA', (right-left, bottom-top), '#263442')
        draw = ImageDraw.Draw(canvas)
        draw.line((0, root[1], canvas.width, root[1]), fill='#778899')
        draw.line((root[0], 20, root[0], canvas.height), fill='#405366')
        resized = sprite.resize((round(sprite.width*scale), round(sprite.height*scale)), Image.Resampling.LANCZOS)
        canvas.alpha_composite(resized, (round(root[0]-px*resized.width),
                                        round(root[1]-(1-py)*resized.height)))
        draw.text((10, 10), f'{index+1}/{len(poses)}' +
                  ('  IMPACT' if index == doc['impact_step'] else ''), fill='white')
        frames.append(canvas.convert('RGB'))
        durations.append(round(info['duration'] * 1000))
    frames[0].save(output/'preview.gif', save_all=True, append_images=frames[1:],
                   duration=durations, loop=0, disposal=2)
    strip = Image.new('RGB', (frames[0].width*len(frames), frames[0].height))
    for i, frame in enumerate(frames):
        strip.paste(frame, (i*frame.width, 0))
    strip.save(output/'contact_sheet.png')
    # Export exact pivots for a later approved import; no guessing from bbox.
    (output/'review.json').write_text(json.dumps(dict(source=doc['source'], ppu=ppu,
        impact_step=doc['impact_step'], frames=[dict(file=f'attack_{i+1}.png',
        pivot=list(pivot), duration=doc['frames'][i]['duration'])
        for i, (_, pivot) in enumerate(poses)]), indent=2)+'\n')
    print(output/'preview.gif')


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('manifest', type=Path)
    parser.add_argument('output', type=Path)
    args = parser.parse_args()
    review(json.loads(args.manifest.read_text()), args.output)
