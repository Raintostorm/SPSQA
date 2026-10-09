#!/usr/bin/env python3
"""Import clean 8-step Legendary clips while preserving six-frame skills."""

import hashlib
import json
import re

import numpy as np
from PIL import Image

from import_remaining_rare import ROOT, MANIFEST, DUR, PAD, split_unit, clip, write_asset
from import_remaining_common import UNIT_ROOT, bbox, largest, write_meta


CONFIG = {
    "dead_legendary": ("dead_legendary_smooth_v1.png", "dead/legendary/assets/sheet-v2.png", ("dead",)),
    "elf_legendary": ("elf_legendary_smooth_v1.png", "elf/legendary/assets/sheet-v2-simple-eyes.png", ()),
    "human_legendary": ("human_legendary_smooth_v1.png", "human/legendary/assets/sheet-v2.png", ("walk", "attack", "dead")),
    "machine_legendary": ("machine_legendary_smooth_v1.png", "machine/legendary/assets/sheet-v2.png", ("dead",)),
    "monster_legendary": ("monster_legendary_smooth_v1.png", "monster/legendary/assets/sheet-v2-simple-eyes.png", ("attack", "dead")),
}

SKILL_DUR = [0.1, 0.12, 0.12, 0.1, 0.12, 0.12, 0.16, 0.2]


def split_skill(key, ppu):
    if key != "dead_legendary":
        return expand_clean_skill(key)
    image = Image.open(ROOT / "artsrc/animation" / f"{key}_skill_v1.png").convert("RGBA")
    rgba = np.asarray(image)
    xs = [round(i * image.width / 8) for i in range(9)]
    template = (UNIT_ROOT / key / "idle_1.png.meta").read_text()
    files = []
    for col in range(8):
        cell = rgba[:, xs[col]:xs[col + 1]].copy()
        cell[cell[..., 3] < 18] = 0
        cell = largest(cell)
        x0, y0, x1, y1 = bbox(cell[..., 3])
        crop = cell[y0:y1, x0:x1]
        out = np.zeros((crop.shape[0] + PAD * 2, crop.shape[1] + PAD * 2, 4), np.uint8)
        out[PAD:PAD + crop.shape[0], PAD:PAD + crop.shape[1]] = crop
        px = ((xs[col + 1] - xs[col]) * .5 - x0 + PAD) / out.shape[1]
        py = (y1 + PAD - (image.height - 7)) / out.shape[0]
        rel = f"{key}/skill_smooth_{col + 1}.png"
        path = UNIT_ROOT / rel
        Image.fromarray(out, "RGBA").save(path)
        guid = hashlib.md5(f"spinsquad/unit-smooth/{rel}".encode()).hexdigest()
        write_meta(template, path, guid, ppu, min(1, max(0, px)), min(1, max(0, py)))
        files.append(rel)
    return files


def expand_clean_skill(key):
    """Map six authored, clean skill poses onto an eight-step timeline."""
    template = (UNIT_ROOT / key / "idle_1.png.meta").read_text()
    files = []
    for col, source_index in enumerate((1, 2, 2, 3, 4, 4, 5, 6), 1):
        source = UNIT_ROOT / key / f"skill_{source_index}.png"
        source_meta = (UNIT_ROOT / key / f"skill_{source_index}.png.meta").read_text()
        source_ppu = float(re.search(r"spritePixelsToUnits: ([0-9.]+)", source_meta).group(1))
        pivot = re.search(r"spritePivot: \{x: ([0-9.]+), y: ([0-9.]+)\}", source_meta)
        rel = f"{key}/skill_smooth_{col}.png"
        path = UNIT_ROOT / rel
        clean = largest(np.asarray(Image.open(source).convert("RGBA")).copy())
        Image.fromarray(clean, "RGBA").save(path)
        guid = hashlib.md5(f"spinsquad/unit-smooth/{rel}".encode()).hexdigest()
        write_meta(template, path, guid, source_ppu, float(pivot.group(1)), float(pivot.group(2)))
        files.append(rel)
    return files


def main():
    manifest = json.loads(MANIFEST.read_text())
    by_key = {entry["key"]: entry for entry in manifest}
    for key, (sheet, original, fallback) in CONFIG.items():
        entry = by_key[key]
        ppu, files = split_unit(key, sheet, original, fallback, entry["world_h"])
        skill_files = split_skill(key, ppu)
        entry["ppu"] = ppu
        entry["anims"] = {
            state: clip(files[state], DUR[state], state != "dead", state in ("idle", "walk"))
            for state in DUR
        }
        entry["anims"]["skill"] = clip(skill_files, SKILL_DUR, False)
        write_asset(key, entry)
        print(f"{key}: PPU {ppu}, 32 frames + preserved skill")
    MANIFEST.write_text(json.dumps(manifest, ensure_ascii=False, indent=1) + "\n")


if __name__ == "__main__":
    main()
