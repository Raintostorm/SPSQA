#!/usr/bin/env python3
"""Tach sheet 8x4 da duyet thanh 32 sprite human_common.

Khong cat theo o cung vi vu khi o cac pose tan cong vuon qua tam o. Thay vao
do, script lay 32 mang alpha lon, xep theo 4 hang x 8 cot, roi giu mot diem
neo co dinh o tam o/duong dat. Nho vay frame doi kich thuoc van khong lam
nhan vat nhay vi tri.
"""

import hashlib
import sys
from pathlib import Path

import cv2
import numpy as np
from PIL import Image


ROOT = Path(__file__).resolve().parents[1]
DEFAULT_SHEET = ROOT / "artsrc/animation/human_common_smooth_v1.png"
OUT = ROOT / "Assets/_Project/Art/Units/human_common"
TEMPLATE_META = OUT / "idle_1.png.meta"

NAMES = ("idle", "walk", "attack", "dead")
GROUND_Y = (230, 453, 678, 880)  # pixel Y tu tren sheet goc
WORLD_HEIGHT = 0.86
PAD = 8


def main() -> int:
    src = Path(sys.argv[1]) if len(sys.argv) > 1 else DEFAULT_SHEET
    image = Image.open(src).convert("RGBA")
    rgba = np.asarray(image)
    alpha = (rgba[..., 3] > 16).astype(np.uint8)
    count, labels, stats, centroids = cv2.connectedComponentsWithStats(alpha, 8)

    parts = []
    for label in range(1, count):
        x, y, w, h, area = (int(v) for v in stats[label])
        if area >= 100:
            parts.append((label, x, y, w, h, area, centroids[label]))
    if len(parts) != 32:
        raise RuntimeError(f"Can dung 32 hinh lon, tim thay {len(parts)} trong {src}")

    parts.sort(key=lambda p: p[6][1])
    rows = []
    for row_index in range(4):
        row = sorted(parts[row_index * 8:(row_index + 1) * 8], key=lambda p: p[6][0])
        rows.append(row)

    # Khung idle dau tien dinh ty le the gioi cua ca bo, khong upsample anh.
    ppu = rows[0][0][4] / WORLD_HEIGHT
    template = TEMPLATE_META.read_text()
    OUT.mkdir(parents=True, exist_ok=True)

    for row_index, row in enumerate(rows):
        name = NAMES[row_index]
        ground_y = GROUND_Y[row_index]
        for column, (label, x, y, w, h, _area, _centroid) in enumerate(row, 1):
            isolated = np.zeros((h + PAD * 2, w + PAD * 2, 4), dtype=np.uint8)
            crop = rgba[y:y + h, x:x + w].copy()
            mask = labels[y:y + h, x:x + w] == label
            crop[~mask] = 0
            isolated[PAD:PAD + h, PAD:PAD + w] = crop

            # Tam o tren sheet la root X; duong dat la root Y. Pivot Unity tinh
            # tu goc duoi-trai cua sprite da cat.
            cell_center_x = (column - 0.5) * image.width / 8.0
            pivot_x = (cell_center_x - x + PAD) / isolated.shape[1]
            pivot_y = (y + h + PAD - ground_y) / isolated.shape[0]
            pivot_x = min(1.0, max(0.0, pivot_x))
            pivot_y = min(1.0, max(0.0, pivot_y))

            rel = f"{name}_smooth_{column}.png"
            Image.fromarray(isolated, "RGBA").save(OUT / rel)
            guid = hashlib.md5(f"spinsquad/unit-smooth/human_common/{rel}".encode()).hexdigest()
            meta = template
            meta = replace_line(meta, "guid:", f"guid: {guid}")
            meta = replace_line(meta, "  spritePixelsToUnits:", f"  spritePixelsToUnits: {ppu:.2f}")
            meta = replace_line(
                meta,
                "  spritePivot:",
                f"  spritePivot: {{x: {pivot_x:.5f}, y: {pivot_y:.5f}}}",
            )
            (OUT / f"{rel}.meta").write_text(meta)

    print(f"Da tach 32 sprite tu {src}")
    print(f"PPU dung chung: {ppu:.2f}; output: {OUT}")
    return 0


def replace_line(text: str, prefix: str, replacement: str) -> str:
    lines = text.splitlines()
    for i, line in enumerate(lines):
        if line.startswith(prefix):
            lines[i] = replacement
            return "\n".join(lines) + "\n"
    raise RuntimeError(f"Khong tim thay dong meta: {prefix}")


if __name__ == "__main__":
    raise SystemExit(main())
