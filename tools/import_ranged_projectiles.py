#!/usr/bin/env python3
"""Split ranged projectile art, create prefabs, and assign them to ranged units."""

import hashlib
import re
from pathlib import Path

import cv2
import numpy as np
from PIL import Image


ROOT = Path(__file__).resolve().parents[1]
SHEET = ROOT / "artsrc/vfx/ranged_projectiles_v1.png"
ART = ROOT / "Assets/_Project/Art/Projectiles"
PREFABS = ROOT / "Assets/_Project/Prefabs/Units"
DATA = ROOT / "Assets/_Project/Data/Units"
META_TEMPLATE = (ROOT / "Assets/_Project/Art/Characters/Ammo/ammo_modern_1.png.meta").read_text()
PREFAB_TEMPLATE = (PREFABS / "Projectile_modern_1.prefab").read_text()

ITEMS = (
    ("dead_rare", "psychic_orb", 18.0, 0.0),
    ("dead_epic", "ghost_skull", 17.0, 0.02),
    ("elf_common", "wood_arrow", 22.0, 0.03),
    ("elf_epic", "crescent_bolt", 24.0, 0.0),
    ("elf_legendary", "royal_arrow", 24.0, 0.02),
    ("human_epic", "crystal_bolt", 21.0, 0.0),
    ("machine_rare", "brass_bullet", 32.0, 0.0),
    ("monster_common", "web_glob", 16.0, 0.04),
)


def largest(cell):
    n, labels, stats, _ = cv2.connectedComponentsWithStats(
        (cell[..., 3] > 16).astype(np.uint8), 8)
    label = 1 + int(np.argmax(stats[1:, cv2.CC_STAT_AREA]))
    out = cell.copy(); out[labels != label] = 0
    return out


def guid(kind, name):
    return hashlib.md5(f"spinsquad/{kind}/{name}".encode()).hexdigest()


def main():
    ART.mkdir(parents=True, exist_ok=True)
    image = Image.open(SHEET).convert("RGBA")
    rgba = np.asarray(image)
    xs = [round(i * image.width / 4) for i in range(5)]
    ys = [round(i * image.height / 2) for i in range(3)]

    for index, (unit, name, speed, arc) in enumerate(ITEMS):
        row, col = divmod(index, 4)
        cell = rgba[ys[row]:ys[row + 1], xs[col]:xs[col + 1]].copy()
        cell[cell[..., 3] < 18] = 0
        cell = largest(cell)
        yy, xx = np.where(cell[..., 3] > 16)
        crop = cell[yy.min():yy.max() + 1, xx.min():xx.max() + 1]

        sprite_path = ART / f"{name}.png"
        Image.fromarray(crop, "RGBA").save(sprite_path)
        sprite_guid = guid("projectile-sprite", name)
        meta = re.sub(r"^guid: [0-9a-f]+$", f"guid: {sprite_guid}", META_TEMPLATE, flags=re.M)
        meta = re.sub(r"spritePivot: \{x: [^}]+\}", "spritePivot: {x: 0.5, y: 0.5}", meta)
        (Path(str(sprite_path) + ".meta")).write_text(meta)

        prefab_name = f"Projectile_{name}"
        prefab_path = PREFABS / f"{prefab_name}.prefab"
        prefab = PREFAB_TEMPLATE.replace("m_Name: Projectile_modern_1", f"m_Name: {prefab_name}")
        prefab = re.sub(r"m_Sprite: \{fileID: 21300000, guid: [0-9a-f]+, type: 3\}",
                        f"m_Sprite: {{fileID: 21300000, guid: {sprite_guid}, type: 3}}", prefab)
        prefab_path.write_text(prefab)
        prefab_guid = guid("projectile-prefab", name)
        (Path(str(prefab_path) + ".meta")).write_text(
            "fileFormatVersion: 2\n" + f"guid: {prefab_guid}\n" +
            "PrefabImporter:\n  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n")

        unit_path = DATA / f"Unit_{unit}.asset"
        text = unit_path.read_text()
        text = re.sub(r"ProjectilePrefab: \{fileID: [^}]+\}",
                      f"ProjectilePrefab: {{fileID: 1100000000000000001, guid: {prefab_guid}, type: 3}}", text)
        text = re.sub(r"ProjectileSpeed: [0-9.]+", f"ProjectileSpeed: {speed:g}", text)
        text = re.sub(r"ProjectileArcHeight: [0-9.]+", f"ProjectileArcHeight: {arc:g}", text)
        unit_path.write_text(text)
        print(f"{unit}: {name}")


if __name__ == "__main__":
    main()
