#!/usr/bin/env python3
"""Assemble eight isolated Royal Hero attack poses from clean generated takes."""

from pathlib import Path

import cv2
import numpy as np
from PIL import Image


ROOT = Path(__file__).resolve().parents[1]
GEN = Path("/Users/vuhagiabao/.codex/generated_images/01a0f0cc-8798-79b0-af42-83a55131c482")
TAKE_A = GEN / "exec-fa44334d-755b-456e-b847-f82c4606a2e8.png"
TAKE_B = GEN / "exec-8dc904cb-d910-4c5e-87fb-bbaecb874afb.png"
REFERENCE = ROOT / "outdata/fix_poses/human_legendary.png"
OUT = ROOT / "artsrc/fixposes/human_legendary_attack_v2.png"


def key(image):
    a = np.asarray(Image.open(image).convert("RGBA")).copy()
    rgb = a[..., :3]
    bg = (rgb[..., 0] > 218) & (rgb[..., 1] < 75) & (rgb[..., 2] > 205)
    a[bg] = 0
    return a


def components(image):
    n, labels, stats, _ = cv2.connectedComponentsWithStats(
        (image[..., 3] > 20).astype(np.uint8), 8)
    ids = [i for i in range(1, n) if stats[i, cv2.CC_STAT_AREA] > 1000]
    ids.sort(key=lambda i: stats[i, cv2.CC_STAT_LEFT])
    out = []
    for label in ids:
        x, y, w, h = map(int, stats[label, :4])
        crop = image[y:y+h, x:x+w].copy()
        crop[labels[y:y+h, x:x+w] != label] = 0
        out.append(crop)
    return out


def main():
    first = components(key(TAKE_A))[:4]
    last = components(key(TAKE_B))[-3:]
    ref = key(REFERENCE)[50:282, 500:735]
    yy, xx = np.where(ref[..., 3] > 20)
    ref = ref[yy.min():yy.max()+1, xx.min():xx.max()+1]
    poses = first + [ref] + last
    if len(poses) != 8:
        raise RuntimeError(f"Expected 8 poses, found {len(poses)}")
    canvas = np.zeros((724, 2172, 4), np.uint8)
    ground = 482
    for i, pose in enumerate(poses):
        centre = round((i + .5) * canvas.shape[1] / 8)
        x = centre - pose.shape[1] // 2
        y = ground - pose.shape[0]
        canvas[y:y+pose.shape[0], x:x+pose.shape[1]] = pose
    Image.fromarray(canvas, "RGBA").save(OUT)
    print(OUT)


if __name__ == "__main__":
    main()
