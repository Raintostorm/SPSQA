#!/usr/bin/env python3
"""Import clean 8-step Epic clips, falling back to authored poses when AI poses overlap."""

import json

from import_remaining_rare import ROOT, MANIFEST, DUR, split_unit, clip, write_asset


CONFIG = {
    "dead_epic": ("dead_epic_smooth_v1.png", "dead/epic/assets/sheet-v1.png", ("attack", "dead")),
    "elf_epic": ("elf_epic_smooth_v1.png", "elf/epic/assets/sheet-v1-simple-eyes.png", ("attack", "dead")),
    "human_epic": ("human_epic_smooth_v1.png", "human/epic/assets/sheet-proportions-v2.png", ("idle", "walk", "attack", "dead")),
    "machine_epic": ("machine_epic_smooth_v1.png", "machine/epic/assets/sheet-v1.png", ("attack", "dead")),
    "monster_epic": ("monster_epic_smooth_v1.png", "monster/epic/assets/sheet-v1-simple-eyes.png", ("attack", "dead")),
}


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
