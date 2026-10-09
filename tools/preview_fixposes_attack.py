#!/usr/bin/env python3
"""Render the ten repaired attack clips as a compact review GIF."""

from pathlib import Path
from PIL import Image, ImageDraw

ROOT = Path(__file__).resolve().parents[1]
ART = ROOT / "Assets/_Project/Art/Units"
OUT = ROOT / "outdata/fixposes_attack_preview.gif"
UNITS = (
    "dead_common", "dead_epic", "elf_epic", "elf_rare", "human_epic",
    "human_legendary", "machine_epic", "monster_common", "monster_epic",
    "monster_legendary",
)

frames = []
for index in range(8):
    canvas = Image.new("RGBA", (1400, 620), (29, 24, 43, 255))
    draw = ImageDraw.Draw(canvas)
    for n, key in enumerate(UNITS):
        col, row = n % 5, n // 5
        x, y = 10 + col * 278, 10 + row * 305
        draw.rounded_rectangle((x, y, x + 265, y + 290), 12,
                               fill=(45, 37, 62, 255), outline=(91, 75, 116, 255), width=2)
        sprite = Image.open(ART / key / f"attack_smooth_{index + 1}.png").convert("RGBA")
        sprite.thumbnail((235, 225), Image.Resampling.LANCZOS)
        canvas.alpha_composite(sprite, (x + 132 - sprite.width // 2, y + 252 - sprite.height))
        draw.text((x + 10, y + 9), key, fill="white")
        draw.text((x + 225, y + 9), f"{index + 1}/8", fill=(255, 215, 98, 255))
    frames.append(canvas.convert("P", palette=Image.Palette.ADAPTIVE))
frames[0].save(OUT, save_all=True, append_images=frames[1:], duration=110,
               loop=0, disposal=2)
print(OUT)
