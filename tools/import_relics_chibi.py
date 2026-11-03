"""Import reviewed chibi relic sheets only; preserve GUIDs and all alpha edges."""
import argparse
import shutil
from pathlib import Path

import numpy as np
from PIL import Image
from scipy import ndimage
from import_pages_art import row_major

ROOT = Path(__file__).resolve().parents[1]
OUT = ROOT / 'Assets/_Project/Art/UI'
BACKUP = ROOT / 'artsrc/ui/relics_before_chibi'
GROUPS = [('common', 'c', 12), ('rare', 'r', 8), ('epic', 'e', 6), ('legendary', 'l', 4)]


def collect():
    items = []
    for tier, prefix, count in GROUPS:
        a = np.asarray(Image.open(ROOT / f'artsrc/ui/relics_{tier}_chibi.png').convert('RGBA'))
        labels, _ = ndimage.label(a[..., 3] > 5)
        parts = []
        for index, bounds in enumerate(ndimage.find_objects(labels), 1):
            if bounds is None or np.count_nonzero(labels[bounds] == index) < 1000:
                continue
            y, x = bounds
            parts.append((y.start, y.stop - 1, x.start, x.stop - 1, index))
        if len(parts) != count:
            raise ValueError(f'{tier}: {len(parts)} components, expected {count}')
        for i, (y0, y1, x0, x1, _) in enumerate(row_major(parts), 1):
            # Padding retains alpha 1..5 around AA edges. No thresholding,
            # recoloring, square clipping or replacement of source pixels.
            crop = a[max(0, y0 - 4):min(a.shape[0], y1 + 5),
                     max(0, x0 - 4):min(a.shape[1], x1 + 5)].copy()
            name = f'item_{prefix}{i}'
            if not (OUT / f'{name}.png.meta').exists():
                raise ValueError(f'Missing existing GUID for {name}')
            weight = np.sqrt((crop[..., 3] / 255).sum())
            items.append((name, crop, weight))
    return items


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('--write', action='store_true')
    args = parser.parse_args()
    items = collect()  # Validate all sheets before any replacement.
    target = min(weight * 232 / max(a.shape[:2]) for _, a, weight in items)
    if args.write:
        BACKUP.mkdir(parents=True, exist_ok=True)
        for name, _, _ in items:
            for suffix in ('.png', '.png.meta'):
                src = OUT / (name + suffix)
                dst = BACKUP / (name + suffix)
                if not dst.exists():
                    shutil.copy2(src, dst)
    for name, a, weight in items:
        scale = target / weight
        h, w = a.shape[:2]
        image = Image.fromarray(a).resize((max(1, round(w * scale)), max(1, round(h * scale))), Image.Resampling.LANCZOS)
        canvas = Image.new('RGBA', (256, 256))
        canvas.paste(image, ((256 - image.width) // 2, (256 - image.height) // 2))
        print(f'{name}: {w}x{h} -> {image.width}x{image.height}; alpha preserved')
        if args.write:
            canvas.save(OUT / f'{name}.png')
    print(f'{len(items)} icons {"imported" if args.write else "validated"}; existing .meta files untouched.')


if __name__ == '__main__':
    main()
