"""Animation preview only; do not modify or import production sprites."""
from pathlib import Path
import json
from PIL import Image

root = Path(__file__).resolve().parents[1]
folder = root / 'artsrc/units_v2'
source = Image.open(folder / 'dead_common_walk.png').convert('RGBA')
boxes = json.loads((folder / 'dead_common_walk_metrics.json').read_text())['boxes']
assert len(boxes) == 8
# Same scale across poses, preserving body bob. Bounding boxes only locate each
# complete figure; padded crops are not fixed square animation cells.
scale = 150 / max(b[3]-b[1] for b in boxes)
frames = []
for x0, y0, x1, y1 in boxes:
    crop = source.crop((max(0,x0-10), max(0,y0-10), min(source.width,x1+10),min(source.height,y1+10)))
    crop = crop.resize((round(crop.width*scale),round(crop.height*scale)),Image.Resampling.LANCZOS)
    frame = Image.new('RGBA',(260,210),(230,235,240,255))
    # Shared baseline with natural changes in pose height; no per-frame scaling.
    frame.alpha_composite(crop,((260-crop.width)//2,180-crop.height))
    frames.append(frame.convert('RGB'))
frames[0].save(folder/'dead_common_walk_preview.gif',save_all=True,
               append_images=frames[1:],duration=100,loop=0,disposal=2)
