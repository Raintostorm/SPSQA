#!/usr/bin/env python3
"""Replace the legacy magenta field in fix-pose sheets with real alpha."""

from pathlib import Path
import numpy as np
from PIL import Image

ROOT = Path(__file__).resolve().parents[1]

for path in sorted((ROOT / "artsrc/fixposes").glob("*_v2.png")):
    rgba = np.asarray(Image.open(path).convert("RGBA")).copy()
    rgb = rgba[..., :3]
    magenta = (rgb[..., 0] > 205) & (rgb[..., 1] < 90) & (rgb[..., 2] > 190)
    rgba[magenta] = 0
    Image.fromarray(rgba, "RGBA").save(path)
    print(path.relative_to(ROOT))
