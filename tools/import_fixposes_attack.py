#!/usr/bin/env python3
"""Import the approved eight-pose attack replacement sheets."""

import hashlib
import json
from pathlib import Path

import cv2
import numpy as np
from PIL import Image

from import_remaining_common import write_meta


ROOT = Path(__file__).resolve().parents[1]
SOURCE = ROOT / "artsrc/fixposes"
UNIT_ROOT = ROOT / "Assets/_Project/Art/Units"
MANIFEST = ROOT / "tools/units_manifest.json"
PAD = 8
UNITS = (
    "dead_common", "dead_epic", "elf_epic", "elf_rare", "human_epic",
    "human_legendary", "machine_epic", "monster_common", "monster_epic",
    "monster_legendary",
)


def remove_magenta(rgba):
    rgb = rgba[..., :3].astype(np.int16)
    # Backward compatibility for old sheets. New sheets use real alpha.
    bg = (rgb[..., 0] > 205) & (rgb[..., 1] < 90) & (rgb[..., 2] > 190)
    rgba = rgba.copy()
    rgba[bg] = 0
    rgba[rgba[..., 3] < 20] = 0
    return rgba


def main():
    manifest = {row["key"]: row for row in json.loads(MANIFEST.read_text())}
    for key in UNITS:
        src = np.asarray(Image.open(SOURCE / f"{key}_attack_v2.png").convert("RGBA"))
        src = remove_magenta(src)
        h, w = src.shape[:2]
        template = (UNIT_ROOT / key / "idle_1.png.meta").read_text()
        ppu = float(manifest[key]["ppu"])
        n, labels, stats, _ = cv2.connectedComponentsWithStats(
            (src[..., 3] > 20).astype(np.uint8), 8)
        parts = [i for i in range(1, n) if stats[i, cv2.CC_STAT_AREA] > 1000]
        parts.sort(key=lambda i: stats[i, cv2.CC_STAT_LEFT])
        if len(parts) != 8:
            raise RuntimeError(f"{key}: expected 8 isolated poses, found {len(parts)}")
        bottoms = [int(stats[i, cv2.CC_STAT_TOP] + stats[i, cv2.CC_STAT_HEIGHT]) for i in parts]
        common_ground = int(np.median(bottoms))
        for index, label in enumerate(parts):
            x0 = int(stats[label, cv2.CC_STAT_LEFT]); y0 = int(stats[label, cv2.CC_STAT_TOP])
            x1 = x0 + int(stats[label, cv2.CC_STAT_WIDTH]); y1 = y0 + int(stats[label, cv2.CC_STAT_HEIGHT])
            crop = src[y0:y1, x0:x1].copy()
            crop[labels[y0:y1, x0:x1] != label] = 0
            out = np.zeros((crop.shape[0] + PAD * 2, crop.shape[1] + PAD * 2, 4), np.uint8)
            out[PAD:PAD + crop.shape[0], PAD:PAD + crop.shape[1]] = crop
            # No fixed cells: anchor on the complete silhouette's actual
            # ground contact. Long weapons, capes, tails and barrels remain
            # outside the body bounds without being cut away.
            local_alpha = crop[..., 3] > 20
            _contact_y, contact_x = np.where(local_alpha[max(0, crop.shape[0] - 12):])
            anchor_x = float(np.median(contact_x)) if len(contact_x) else crop.shape[1] * .5
            root_x = (anchor_x + PAD) / out.shape[1]
            root_y = (y1 + PAD - common_ground) / out.shape[0]
            rel = f"{key}/attack_smooth_{index + 1}.png"
            path = UNIT_ROOT / rel
            Image.fromarray(out, "RGBA").save(path)
            guid = hashlib.md5(f"spinsquad/unit-smooth/{rel}".encode()).hexdigest()
            write_meta(template, path, guid, ppu,
                       min(1, max(0, root_x)), min(1, max(0, root_y)))
        drift = max(bottoms) - min(bottoms)
        print(f"{key}: imported 8 isolated attack poses at PPU {ppu:g}; source ground drift {drift}px")


if __name__ == "__main__":
    main()
