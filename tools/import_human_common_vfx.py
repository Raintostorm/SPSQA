#!/usr/bin/env python3
"""Tach sheet VFX 8x3 cua human_common thanh sprite Unity."""

import hashlib
from pathlib import Path

import numpy as np
from PIL import Image


ROOT = Path(__file__).resolve().parents[1]
SOURCE = ROOT / "artsrc/vfx/human_common_vfx_v1.png"
SOUL_SOURCE = ROOT / "artsrc/vfx/human_common_soul_subtle_v2.png"
OUT = ROOT / "Assets/_Project/Art/VFX/human_common"
META_TEMPLATE = ROOT / "Assets/_Project/Art/Units/human_common/idle_smooth_1.png.meta"
ROWS = (("dust", 0.14), ("slash", 0.50), ("soul", 0.08))
PPU = 260.0


def replace_line(text: str, prefix: str, replacement: str) -> str:
    lines = text.splitlines()
    for i, line in enumerate(lines):
        if line.startswith(prefix):
            lines[i] = replacement
            return "\n".join(lines) + "\n"
    raise RuntimeError(f"Khong tim thay dong meta: {prefix}")


def main() -> int:
    image = Image.open(SOURCE).convert("RGBA")
    rgba = np.asarray(image).copy()
    # Xoa lop alpha qua mong do AI de lai; giu hat bui/sang co alpha ro rang.
    rgba[rgba[..., 3] < 18] = 0
    image = Image.fromarray(rgba, "RGBA")
    xs = [round(i * image.width / 8) for i in range(9)]
    ys = [round(i * image.height / 3) for i in range(4)]
    template = META_TEMPLATE.read_text()
    OUT.mkdir(parents=True, exist_ok=True)

    soul = Image.open(SOUL_SOURCE).convert("RGBA")
    soul_xs = [round(i * soul.width / 8) for i in range(9)]

    for row, (name, pivot_y) in enumerate(ROWS):
        for column in range(8):
            if name == "soul":
                frame = soul.crop((soul_xs[column], 0, soul_xs[column + 1], soul.height))
                frame = quiet_soul(frame)
            else:
                frame = image.crop((xs[column], ys[row], xs[column + 1], ys[row + 1]))
            filename = f"{name}_{column + 1}.png"
            frame.save(OUT / filename)
            guid = hashlib.md5(f"spinsquad/vfx/human_common/{filename}".encode()).hexdigest()
            meta = replace_line(template, "guid:", f"guid: {guid}")
            meta = replace_line(meta, "  spritePixelsToUnits:", f"  spritePixelsToUnits: {PPU:.2f}")
            meta = replace_line(
                meta,
                "  spritePivot:",
                f"  spritePivot: {{x: 0.5, y: {pivot_y:.5f}}}",
            )
            (OUT / f"{filename}.meta").write_text(meta)

    print(f"Da tach 24 sprite VFX vao {OUT}")
    return 0


def quiet_soul(frame: Image.Image) -> Image.Image:
    """Lam hon don sac, mo va cat gon khoang trong cua strip sinh boi AI."""
    a = np.asarray(frame).copy()
    alpha = a[..., 3]
    visible = alpha >= 18
    if not visible.any():
        return Image.new("RGBA", (32, 32))

    lum = (a[..., 0].astype(np.uint16) * 30 +
           a[..., 1].astype(np.uint16) * 59 +
           a[..., 2].astype(np.uint16) * 11) // 100
    a[..., 0] = (lum * 88 // 100).astype(np.uint8)
    a[..., 1] = (lum * 92 // 100).astype(np.uint8)
    a[..., 2] = lum.astype(np.uint8)
    a[..., 3] = (alpha.astype(np.float32) * 0.62).astype(np.uint8)
    a[a[..., 3] < 12] = 0

    yy, xx = np.where(a[..., 3] > 0)
    pad = 8
    x0, x1 = max(0, int(xx.min()) - pad), min(a.shape[1], int(xx.max()) + pad + 1)
    y0, y1 = max(0, int(yy.min()) - pad), min(a.shape[0], int(yy.max()) + pad + 1)
    return Image.fromarray(a[y0:y1, x0:x1], "RGBA")


if __name__ == "__main__":
    raise SystemExit(main())
