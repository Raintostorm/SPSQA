#!/usr/bin/env python3
"""Lossless arbitrary pose regions, never square cells or largest components.

JSON input: {"source": "relative/path.png", "frames": [{"rect": [x,y,w,h],
"anchor": [source_x,source_y]}]}. No asset writes; reject incomplete extraction.
"""
import argparse
import json
from pathlib import Path
import numpy as np
from PIL import Image, ImageDraw

ROOT = Path(__file__).resolve().parents[1]


def extract_seeded(src, frames, padding=8):
    """Explicit reviewed seed per pose; assign every alpha pixel once.

    Threshold controls connectivity ONLY. It never deletes alpha pixels.
    Large unclaimed disconnected parts require their own explicit seed.
    """
    import cv2
    from scipy.ndimage import distance_transform_edt
    n, labels, stats, _ = cv2.connectedComponentsWithStats(
        (src[..., 3] > 20).astype(np.uint8), 8)
    owner = np.zeros(labels.shape, dtype=np.int32)
    claimed = set()
    for index, frame in enumerate(frames, 1):
        for x, y in frame['seeds']:
            if not (0 <= x < src.shape[1] and 0 <= y < src.shape[0]):
                raise ValueError('Seed outside source')
            label = int(labels[y, x])
            if label == 0 or label in claimed:
                raise ValueError(f'frame {index}: empty or shared seed')
            claimed.add(label)
            owner[labels == label] = index
    unclaimed = [i for i in range(1, n) if i not in claimed and stats[i, 4] >= 64]
    if unclaimed:
        raise ValueError(f'Unclaimed detached components: {unclaimed}; add explicit seeds')
    # Tiny specks/soft alpha assigned to nearest reviewed full silhouette.
    # Their ORIGINAL RGBA is preserved, not keyed or feathered away.
    _, nearest = distance_transform_edt(owner == 0, return_indices=True)
    owner = owner[tuple(nearest)]
    poses = []
    for index, frame in enumerate(frames, 1):
        mask = (owner == index) & (src[..., 3] > 0)
        ys, xs = np.where(mask)
        x, y, right, bottom = int(xs.min()), int(ys.min()), int(xs.max()+1), int(ys.max()+1)
        pixels = src[y:bottom, x:right].copy()
        pixels[~mask[y:bottom, x:right]] = 0
        ax, ay = frame['anchor']
        out = np.pad(pixels, ((padding,padding),(padding,padding),(0,0)))
        pivot = ((ax-x+padding)/out.shape[1], 1-(ay-y+padding)/out.shape[0])
        poses.append((out, pivot))
    if sum(int(p[...,3].sum()) for p, _ in poses) != int(src[...,3].sum()):
        raise ValueError('Alpha conservation failed')
    return poses


def extract(src, frames, padding=8):
    if frames and all('seeds' in frame for frame in frames):
        return extract_seeded(src, frames, padding)
    height, width = src.shape[:2]
    assigned = np.zeros((height, width), dtype=bool)
    poses = []
    for index, frame in enumerate(frames, 1):
        x, y, w, h = frame['rect']
        if min(x, y) < 0 or min(w, h) <= 0 or x+w > width or y+h > height:
            raise ValueError(f'frame {index}: region outside source')
        pixels = src[y:y+h, x:x+w].copy()
        if 'polygon' in frame:
            mask_image = Image.new('L', (w, h))
            ImageDraw.Draw(mask_image).polygon(
                [(px-x, py-y) for px, py in frame['polygon']], fill=255)
            mask = np.array(mask_image) > 0
            pixels[~mask] = 0
        alpha = pixels[..., 3] > 0
        if 'polygon' in frame:
            # A polygon can cut through art without touching the outer
            # rectangle. Reject such separators before any import.
            import cv2
            inside = np.array(mask_image) > 0
            border = inside & ~cv2.erode(inside.astype(np.uint8),
                                        np.ones((3, 3), np.uint8)).astype(bool)
            if np.any(border & (pixels[..., 3] > 20)):
                raise ValueError(f'frame {index}: polygon cuts a visible part')
        region = assigned[y:y+h, x:x+w]
        if not alpha.any():
            raise ValueError(f'frame {index}: empty')
        if np.any(region & (pixels[..., 3] > 20)):
            raise ValueError(f'frame {index}: shared visible pixels')
        # Inclusive polygon edges may share faint alpha noise: retain
        # those pixels exactly once, in their first assigned region.
        pixels[region] = 0
        alpha = pixels[..., 3] > 0
        visible_edge = pixels[..., 3] > 20
        if any(edge.any() for edge in (visible_edge[0], visible_edge[-1], visible_edge[:,0], visible_edge[:,-1])):
            raise ValueError(f'frame {index}: possible cut part at region edge')
        ax, ay = frame['anchor']
        if not (x <= ax <= x+w and y <= ay <= y+h):
            raise ValueError(f'frame {index}: anchor outside region')
        region |= alpha
        # Keep ALL components, including detached blade, arrow and flame.
        out = np.pad(pixels, ((padding,padding),(padding,padding),(0,0)))
        pivot = ((ax-x+padding)/out.shape[1], 1-(ay-y+padding)/out.shape[0])
        poses.append((out, pivot))
    if np.any((src[...,3] > 0) & ~assigned):
        raise ValueError('Unassigned visible pixels: part omitted or background present')
    return poses


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('manifest', type=Path)
    args = parser.parse_args()
    doc = json.loads(args.manifest.read_text())
    src = np.array(Image.open(ROOT/doc['source']).convert('RGBA'))
    if not np.any(src[...,3] == 0):
        raise ValueError('Source must have real transparency')
    poses = extract(src, doc['frames'])
    print(f'{len(poses)} poses validated: every visible pixel retained exactly once')


if __name__ == '__main__':
    main()
