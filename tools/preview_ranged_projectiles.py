#!/usr/bin/env python3
"""Build a quick contact-sheet GIF of every ranged unit firing its projectile."""

import json
import math
from pathlib import Path

from PIL import Image, ImageDraw, ImageFont


ROOT = Path(__file__).resolve().parents[1]
UNITS = ROOT / "Assets/_Project/Art/Units"
PROJECTILES = ROOT / "Assets/_Project/Art/Projectiles"
OUT = ROOT / "outdata/ranged_projectiles_demo.gif"

ROWS = (
    ("dead_rare", "psychic_orb", "DEAD RARE", 18, 0),
    ("dead_epic", "ghost_skull", "DEAD EPIC", 17, .02),
    ("elf_common", "wood_arrow", "ELF COMMON", 22, .03),
    ("elf_epic", "crescent_bolt", "ELF EPIC", 24, 0),
    ("elf_legendary", "royal_arrow", "ELF LEGEND", 24, .02),
    ("human_epic", "crystal_bolt", "HUMAN EPIC", 21, 0),
    ("machine_rare", "brass_bullet", "MACHINE RARE", 32, 0),
    ("monster_common", "web_glob", "MONSTER COMMON", 16, .04),
)


def fit(im, size):
    im = im.copy()
    im.thumbnail(size, Image.Resampling.LANCZOS)
    return im


def main():
    manifest = {r["key"]: r for r in json.loads((ROOT / "tools/units_manifest.json").read_text())}
    frames = []
    total = 32
    for tick in range(total):
        canvas = Image.new("RGBA", (1200, 650), (31, 25, 46, 255))
        draw = ImageDraw.Draw(canvas)
        draw.text((30, 18), "RANGED PROJECTILE DEMO", fill=(255, 226, 120, 255))
        draw.text((30, 42), "attack -> release -> flight", fill=(185, 177, 205, 255))
        for i, (key, projectile, label, speed, arc) in enumerate(ROWS):
            col, row = i % 4, i // 4
            ox, oy = 25 + col * 295, 85 + row * 270
            draw.rounded_rectangle((ox, oy, ox + 275, oy + 245), 16,
                                   fill=(48, 40, 67, 255), outline=(91, 78, 119, 255), width=2)
            attack = manifest[key]["anims"]["attack"]["files"]
            attack_index = min(7, tick // 2) if tick < 16 else 7
            unit = fit(Image.open(UNITS / attack[attack_index]).convert("RGBA"), (120, 145))
            ux, uy = ox + 17, oy + 58 + (145 - unit.height)
            canvas.alpha_composite(unit, (ux, uy))

            release = 10
            flight_frames = round(160 / speed)
            if release <= tick <= release + flight_frames:
                p = fit(Image.open(PROJECTILES / f"{projectile}.png").convert("RGBA"), (74, 48))
                progress = min(1, (tick - release) / flight_frames)
                px = int(ox + 112 + progress * 135)
                py = int(oy + 132 - math.sin(progress * math.pi) * arc * 260)
                canvas.alpha_composite(p, (px, py - p.height // 2))
            draw.text((ox + 13, oy + 13), label, fill=(255, 255, 255, 255))
            draw.text((ox + 13, oy + 218), f"speed {speed:g}  arc {arc:g}", fill=(167, 205, 255, 255))
        frames.append(canvas.convert("P", palette=Image.Palette.ADAPTIVE))
    OUT.parent.mkdir(exist_ok=True)
    frames[0].save(OUT, save_all=True, append_images=frames[1:], duration=65, loop=0, disposal=2)
    print(OUT)


if __name__ == "__main__":
    main()
