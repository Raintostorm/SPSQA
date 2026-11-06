"""Preview: uniform head crops for all allies, same scale for everyone.
Old ShopV2/Heads were cropped at different zooms (Royal Hero / Gear Gunner fill the frame,
Crescent Mage is small). Rule: idle_000 frame (all ~250 px tall at the same draw scale),
one fixed SIDE x SIDE window from the top of the sprite, centred on the head, scaled to 280.
Writes artsrc/ui/heads-v3/<id>.png + chat/check/016-heads-compare.png. Does NOT touch Assets
(approved set was copied into Resources/ShopV2/Heads by hand, keeping .meta files)."""
from pathlib import Path
from PIL import Image, ImageDraw
ROOT = Path(__file__).resolve().parents[1]
SIDE, OUT = 210, 280
src = ROOT / 'Assets/_Project/Art/UnitsSimple'
old = ROOT / 'Assets/_Project/Resources/ShopV2/Heads'
dst = ROOT / 'artsrc/ui/heads-v3'; dst.mkdir(parents=True, exist_ok=True)
ids = sorted(p.stem for p in old.glob('*.png'))
OVERRIDES = {'machine_epic': ROOT / 'Assets/_Project/Art/Units/machine_epic/idle_smooth_1.png'}
pairs = []
for uid in ids:
    # Furnace Crawler has no UnitsSimple folder; its Anim Idle frame 0 lives in Art/Units (found by Codex).
    frame = OVERRIDES.get(uid, src / uid / 'idle_000.png')
    if not frame.exists(): pairs.append((uid, None)); continue
    im = Image.open(frame).convert('RGBA'); a = im.getchannel('A').point(lambda v: 255 if v > 24 else 0)
    l, t, r, b = a.getbbox()
    head = a.crop((0, t, im.width, t + int((b - t) * .38))).getbbox()  # top band = head/hat
    cx = (head[0] + head[2]) / 2
    box = (int(cx - SIDE / 2), t - 6, int(cx + SIDE / 2), t - 6 + SIDE)
    canvas = Image.new('RGBA', (SIDE, SIDE)); canvas.alpha_composite(im.crop((max(0, box[0]), max(0, box[1]), min(im.width, box[2]), min(im.height, box[3]))), (max(0, -box[0]), max(0, -box[1])))
    out = canvas.resize((OUT, OUT), Image.Resampling.LANCZOS); out.save(dst / (uid + '.png')); pairs.append((uid, out))
cell = 150; sheet = Image.new('RGB', (cell * 2 * 4 + 40, (cell + 24) * ((len(ids) + 3) // 4) + 10), (22, 34, 58)); d = ImageDraw.Draw(sheet)
for i, (uid, new) in enumerate(pairs):
    x, y = (i % 4) * (cell * 2 + 10), (i // 4) * (cell + 24)
    for j, img in enumerate([Image.open(old / (uid + '.png')).convert('RGBA'), new]):
        if img is None: continue
        bg = Image.new('RGBA', (cell, cell), (40, 60, 100, 255)); bg.alpha_composite(img.resize((cell, cell))); sheet.paste(bg.convert('RGB'), (x + j * cell, y))
    d.text((x + 4, y + cell + 4), uid + ('  (old | new)' if new else '  (no idle frame)'), fill='white')
sheet.save(ROOT / 'chat/check/016-heads-compare.png'); print(len([p for p in pairs if p[1]]), 'of', len(ids))
