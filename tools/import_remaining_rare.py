#!/usr/bin/env python3
"""Import 8-frame animation for the four remaining rare units."""

import hashlib
import json
from pathlib import Path

import cv2
import numpy as np
from PIL import Image

from import_remaining_common import write_meta, write_asset, clip


ROOT = Path(__file__).resolve().parents[1]
UNIT_ROOT = ROOT / "Assets/_Project/Art/Units"
MANIFEST = ROOT / "tools/units_manifest.json"
PAD = 8
DUR = {
    "idle": [0.125] * 8,
    "walk": [0.1] * 8,
    "attack": [0.1, 0.12, 0.12, 0.1, 0.08, 0.12, 0.16, 0.2],
    "dead": [0.125] * 8,
}
CONFIG = {
    "dead_rare": ("dead_rare_smooth_v1.png", "dead/rare/assets/sheet-chibi-v2.png", ()),
    "elf_rare": ("elf_rare_smooth_v1.png", "elf/rare/assets/sheet-simple-eyes.png", ("attack",)),
    "machine_rare": ("machine_rare_smooth_v1.png", "machine/rare/assets/sheet.png", ()),
    "monster_rare": ("monster_rare_smooth_v1.png", "monster/rare/assets/sheet-chibi-v2-simple-eyes.png", ()),
}


def grouped_frames(rgba, y0, y1, count=8):
    """Assign every detached part to its nearest nominal frame centre."""
    row = rgba[y0:y1].copy()
    row[row[..., 3] < 18] = 0
    n, labels, stats, centroids = cv2.connectedComponentsWithStats(
        (row[..., 3] > 16).astype(np.uint8), 8)
    centres = np.array([(i + .5) * row.shape[1] / count for i in range(count)])
    groups = [[] for _ in range(count)]
    for label in range(1, n):
        area = stats[label, cv2.CC_STAT_AREA]
        top = stats[label, cv2.CC_STAT_TOP]
        bottom = top + stats[label, cv2.CC_STAT_HEIGHT]
        if area < 20:
            continue
        # Small pieces touching a row boundary belong to the pose above/below,
        # not this animation row. Keep large bodies even when their feet sit on
        # the lower boundary.
        if area < 2000 and (top == 0 or bottom >= row.shape[0] - 1):
            continue
        slot = int(np.argmin(abs(centres - centroids[label, 0])))
        groups[slot].append(label)
    if any(not group for group in groups):
        raise RuntimeError("Generated row does not contain eight isolated frames")
    return row, labels, stats, groups


def original_frames(path, row):
    image = Image.open(path).convert("RGBA")
    rgba = np.asarray(image)
    xs = [round(i * image.width / 4) for i in range(5)]
    ys = [round(i * image.height / 4) for i in range(5)]
    frames = []
    for col in range(4):
        cell = rgba[ys[row]:ys[row + 1], xs[col]:xs[col + 1]].copy()
        cell[cell[..., 3] < 18] = 0
        n, labels, stats, _ = cv2.connectedComponentsWithStats(
            (cell[..., 3] > 16).astype(np.uint8), 8)
        for label in range(1, n):
            area = stats[label, cv2.CC_STAT_AREA]
            top = stats[label, cv2.CC_STAT_TOP]
            bottom = top + stats[label, cv2.CC_STAT_HEIGHT]
            if area < 2000 and (top == 0 or bottom >= cell.shape[0] - 1):
                cell[labels == label] = 0
        n, labels, stats, _ = cv2.connectedComponentsWithStats(
            (cell[..., 3] > 16).astype(np.uint8), 8)
        if n > 1:
            main = 1 + int(np.argmax(stats[1:, cv2.CC_STAT_AREA]))
            cell[labels != main] = 0
        frames.extend([(cell, (xs[col] + xs[col + 1]) * .5, ys[row + 1] - 7)] * 2)
    return frames


def generated_frames(rgba, row):
    h, w = rgba.shape[:2]
    ys = [round(i * h / 4) for i in range(5)]
    image, labels, stats, groups = grouped_frames(rgba, ys[row], ys[row + 1])
    frames = []
    for col, group in enumerate(groups):
        keep = np.isin(labels, group)
        yy, xx = np.where(keep)
        x0, x1, y0, y1 = xx.min(), xx.max() + 1, yy.min(), yy.max() + 1
        crop = image[y0:y1, x0:x1].copy()
        crop[~keep[y0:y1, x0:x1]] = 0
        frames.append((crop, (col + .5) * w / 8 - x0, ys[row + 1] - 7 - (ys[row] + y0)))
    return frames


def monster_dead_frames(rgba, original):
    """Fall backward and remain on the back; never flip to a prone corpse."""
    h, w = rgba.shape[:2]
    ys = [round(i * h / 4) for i in range(5)]
    image = rgba[ys[3]:ys[4]].copy()
    image[image[..., 3] < 18] = 0
    n, labels, stats, _ = cv2.connectedComponentsWithStats(
        (image[..., 3] > 16).astype(np.uint8), 8)
    parts = [i for i in range(1, n) if stats[i, cv2.CC_STAT_AREA] >= 5000]
    parts.sort(key=lambda i: stats[i, cv2.CC_STAT_LEFT])
    if len(parts) < 5:
        raise RuntimeError(f"Expected at least 5 clean monster death poses, found {len(parts)}")
    frames = []
    for col, label in enumerate(parts[:5]):
        x0 = int(stats[label, cv2.CC_STAT_LEFT]); y0 = int(stats[label, cv2.CC_STAT_TOP])
        x1 = x0 + int(stats[label, cv2.CC_STAT_WIDTH]); y1 = y0 + int(stats[label, cv2.CC_STAT_HEIGHT])
        crop = image[y0:y1, x0:x1].copy()
        crop[labels[y0:y1, x0:x1] != label] = 0
        frames.append((crop, (col + .5) * w / 8 - x0, ys[4] - 7 - (ys[3] + y0)))
    frames.extend([frames[-1]] * 3)
    return frames


def split_unit(key, sheet, original, fallback, world_h):
    image = Image.open(ROOT / "artsrc/animation" / sheet).convert("RGBA")
    rgba = np.asarray(image)
    template = (UNIT_ROOT / key / "idle_1.png.meta").read_text()
    files = {}
    ppu = None
    for row, state in enumerate(("idle", "walk", "attack", "dead")):
        if key == "monster_rare" and state == "dead":
            prepared = monster_dead_frames(rgba, ROOT / "20model" / original)
        elif state in fallback:
            raw = original_frames(ROOT / "20model" / original, row)
            prepared = []
            for cell, root_x, ground in raw:
                yy, xx = np.where(cell[..., 3] > 16)
                x0, x1, y0, y1 = xx.min(), xx.max() + 1, yy.min(), yy.max() + 1
                prepared.append((cell[y0:y1, x0:x1], root_x - x0, ground - y0))
        else:
            prepared = generated_frames(rgba, row)
        if key == "dead_rare" and state == "attack":
            prepared[4], prepared[5] = prepared[5], prepared[4]
        files[state] = []
        for col, (crop, root_x, ground_y) in enumerate(prepared):
            out = np.zeros((crop.shape[0] + PAD * 2, crop.shape[1] + PAD * 2, 4), np.uint8)
            out[PAD:PAD + crop.shape[0], PAD:PAD + crop.shape[1]] = crop
            if ppu is None:
                ppu = crop.shape[0] / world_h
            px = (root_x + PAD) / out.shape[1]
            py = (crop.shape[0] + PAD - ground_y) / out.shape[0]
            rel = f"{key}/{state}_smooth_{col + 1}.png"
            path = UNIT_ROOT / rel
            Image.fromarray(out, "RGBA").save(path)
            guid = hashlib.md5(f"spinsquad/unit-smooth/{rel}".encode()).hexdigest()
            write_meta(template, path, guid, ppu, min(1, max(0, px)), min(1, max(0, py)))
            files[state].append(rel)
    return round(ppu, 2), files


def main():
    manifest = json.loads(MANIFEST.read_text())
    by_key = {entry["key"]: entry for entry in manifest}
    for key, (sheet, original, fallback) in CONFIG.items():
        entry = by_key[key]
        ppu, files = split_unit(key, sheet, original, fallback, entry["world_h"])
        entry["ppu"] = ppu
        entry["anims"] = {
            state: clip(files[state], DUR[state], state != "dead", state in ("idle", "walk"))
            for state in DUR
        }
        write_asset(key, entry)
        print(f"{key}: PPU {ppu}, 32 frames")
    MANIFEST.write_text(json.dumps(manifest, ensure_ascii=False, indent=1) + "\n")


if __name__ == "__main__":
    main()
