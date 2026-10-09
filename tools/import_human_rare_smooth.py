#!/usr/bin/env python3
"""Tach animation va spear VFX 8-frame cua human_rare vao Unity."""

import hashlib
from pathlib import Path

import cv2
import numpy as np
from PIL import Image


ROOT = Path(__file__).resolve().parents[1]
SHEET = ROOT / "artsrc/animation/human_rare_smooth_v1.png"
SPEAR_VFX = ROOT / "artsrc/vfx/human_rare_spear_v1.png"
UNIT_OUT = ROOT / "Assets/_Project/Art/Units/human_rare"
VFX_OUT = ROOT / "Assets/_Project/Art/VFX/human_rare"
META_TEMPLATE = ROOT / "Assets/_Project/Art/Units/human_rare/idle_1.png.meta"
WORLD_HEIGHT = 0.903
PAD = 8


def replace_line(text: str, prefix: str, replacement: str) -> str:
    lines = text.splitlines()
    for i, line in enumerate(lines):
        if line.startswith(prefix):
            lines[i] = replacement
            return "\n".join(lines) + "\n"
    raise RuntimeError(f"Khong tim thay dong meta: {prefix}")


def bounds(alpha: np.ndarray):
    yy, xx = np.where(alpha > 16)
    if not len(xx):
        return 0, 0, alpha.shape[1], alpha.shape[0]
    return int(xx.min()), int(yy.min()), int(xx.max()) + 1, int(yy.max()) + 1


def keep_largest(cell: np.ndarray) -> np.ndarray:
    """Bo dau giao cua o ke ben neu no vuon qua bien cell."""
    count, labels, stats, _ = cv2.connectedComponentsWithStats(
        (cell[..., 3] > 16).astype(np.uint8), 8)
    if count <= 1:
        return cell
    label = 1 + int(np.argmax(stats[1:, cv2.CC_STAT_AREA]))
    out = cell.copy()
    out[labels != label] = 0
    return out


def meta(template: str, guid: str, ppu: float, px: float, py: float) -> str:
    text = replace_line(template, "guid:", f"guid: {guid}")
    text = replace_line(text, "  spritePixelsToUnits:", f"  spritePixelsToUnits: {ppu:.2f}")
    return replace_line(text, "  spritePivot:",
                        f"  spritePivot: {{x: {px:.5f}, y: {py:.5f}}}")


def main() -> int:
    image = Image.open(SHEET).convert("RGBA")
    rgba = np.asarray(image)
    xs = [round(i * image.width / 8) for i in range(9)]
    ys = [round(i * image.height / 4) for i in range(5)]
    names = ("idle", "walk", "attack", "dead")
    template = META_TEMPLATE.read_text()
    UNIT_OUT.mkdir(parents=True, exist_ok=True)

    # Chieu cao hinh bong idle dau tien dat kich thuoc the gioi cho ca bo.
    first = rgba[ys[0]:ys[1], xs[0]:xs[1]]
    x0, y0, x1, y1 = bounds(first[..., 3])
    ppu = (y1 - y0) / WORLD_HEIGHT

    for row, name in enumerate(names):
        ground = ys[row + 1] - 7
        for col in range(8):
            cell = rgba[ys[row]:ys[row + 1], xs[col]:xs[col + 1]].copy()
            cell[cell[..., 3] < 18] = 0
            cell = keep_largest(cell)
            x0, y0, x1, y1 = bounds(cell[..., 3])
            crop = cell[y0:y1, x0:x1]
            out = np.zeros((crop.shape[0] + PAD * 2, crop.shape[1] + PAD * 2, 4), np.uint8)
            out[PAD:PAD + crop.shape[0], PAD:PAD + crop.shape[1]] = crop
            root_x = (xs[col] + xs[col + 1]) * 0.5
            pivot_x = (root_x - (xs[col] + x0) + PAD) / out.shape[1]
            pivot_y = ((ys[row] + y1) + PAD - ground) / out.shape[0]
            filename = f"{name}_smooth_{col + 1}.png"
            Image.fromarray(out, "RGBA").save(UNIT_OUT / filename)
            guid = hashlib.md5(f"spinsquad/unit-smooth/human_rare/{filename}".encode()).hexdigest()
            (UNIT_OUT / f"{filename}.meta").write_text(
                meta(template, guid, ppu, min(1, max(0, pivot_x)), min(1, max(0, pivot_y))))

    split_spear_vfx(template)
    print(f"Da tach 32 frame human_rare (PPU {ppu:.2f}) va 8 spear VFX")
    return 0


def split_spear_vfx(template: str):
    image = Image.open(SPEAR_VFX).convert("RGBA")
    rgba = np.asarray(image)
    xs = [round(i * image.width / 8) for i in range(9)]
    VFX_OUT.mkdir(parents=True, exist_ok=True)
    for col in range(8):
        cell = rgba[:, xs[col]:xs[col + 1]].copy()
        cell[cell[..., 3] < 18] = 0
        x0, y0, x1, y1 = bounds(cell[..., 3])
        crop = cell[y0:y1, x0:x1]
        filename = f"spear_thrust_{col + 1}.png"
        Image.fromarray(crop, "RGBA").save(VFX_OUT / filename)
        guid = hashlib.md5(f"spinsquad/vfx/human_rare/{filename}".encode()).hexdigest()
        (VFX_OUT / f"{filename}.meta").write_text(meta(template, guid, 260, 0.5, 0.5))


if __name__ == "__main__":
    raise SystemExit(main())
