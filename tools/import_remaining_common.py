#!/usr/bin/env python3
"""Nhap 4 unit common con lai: 8-frame animation + VFX rieng tung vu khi."""

import hashlib
import json
import re
from pathlib import Path

import cv2
import numpy as np
from PIL import Image


ROOT = Path(__file__).resolve().parents[1]
UNIT_ROOT = ROOT / "Assets/_Project/Art/Units"
VFX_ROOT = ROOT / "Assets/_Project/Art/VFX"
DATA_ROOT = ROOT / "Assets/_Project/Data/Units"
MANIFEST = ROOT / "tools/units_manifest.json"
VFX_SHEET = ROOT / "artsrc/vfx/common_attack_vfx_v1.png"
MACHINE_ATTACK = ROOT / "artsrc/animation/machine_common_attack_v2.png"
ORIGINAL_ROOT = ROOT / "20model"
PAD = 8
SEQ = list(range(8))
DUR = {
    "idle": [0.125] * 8,
    "walk": [0.1] * 8,
    "attack": [0.1, 0.12, 0.12, 0.1, 0.08, 0.12, 0.16, 0.2],
    "dead": [0.125] * 8,
}
CONFIG = {
    "dead_common": dict(sheet="dead_common_smooth_v1.png", effect="sword_swipe", row=0,
                        original="dead/common/assets/base-sheet.png",
                        original_states=("attack", "dead"),
                        attack_offset=(0.23, 0.32), attack_scale=0.36,
                        foot_spacing=0.09, dust_scale=0.24, dust_alpha=0.25, soul=True),
    "elf_common": dict(sheet="elf_common_smooth_v1.png", effect="bow_release", row=1,
                       original="elf/common/assets/elf-archer-sheet-v2.png",
                       original_states=("dead",),
                       attack_offset=(0.48, 0.35), attack_scale=0.34,
                       foot_spacing=0.10, dust_scale=0.25, dust_alpha=0.26, soul=True),
    "machine_common": dict(sheet="machine_common_smooth_v1.png", effect="saw_sparks", row=2,
                           original="machine/common/assets/saw-crawler-sheet-v1.png",
                           original_states=(),
                           attack_offset=(0.44, 0.15), attack_scale=0.28,
                           foot_spacing=0.16, dust_scale=0.18, dust_alpha=0.16, soul=False),
    "monster_common": dict(sheet="monster_common_smooth_v1.png", effect="web_spit", row=3,
                           original="monster/common/assets/web-spider-sheet-v1-simple-eyes.png",
                           original_states=("attack", "dead"),
                           attack_offset=(0.44, 0.18), attack_scale=0.32,
                           foot_spacing=0.17, dust_scale=0.20, dust_alpha=0.20, soul=True),
}


def replace_line(text, prefix, replacement):
    lines = text.splitlines()
    for i, line in enumerate(lines):
        if line.startswith(prefix):
            lines[i] = replacement
            return "\n".join(lines) + "\n"
    raise RuntimeError(prefix)


def bbox(alpha):
    yy, xx = np.where(alpha > 16)
    if not len(xx): return 0, 0, alpha.shape[1], alpha.shape[0]
    return int(xx.min()), int(yy.min()), int(xx.max()) + 1, int(yy.max()) + 1


def largest(cell):
    n, labels, stats, _ = cv2.connectedComponentsWithStats(
        (cell[..., 3] > 16).astype(np.uint8), 8)
    if n <= 1: return cell
    label = 1 + int(np.argmax(stats[1:, cv2.CC_STAT_AREA]))
    out = cell.copy(); out[labels != label] = 0
    return out


def machine_attack_parts(cell):
    """Giu than may + luoi cua roi ben phai; bo luoi cua cua o truoc lot ben trai."""
    n, labels, stats, centroids = cv2.connectedComponentsWithStats(
        (cell[..., 3] > 16).astype(np.uint8), 8)
    if n <= 1: return cell
    main = 1 + int(np.argmax(stats[1:, cv2.CC_STAT_AREA]))
    keep = labels == main
    for label in range(1, n):
        if label == main or stats[label, cv2.CC_STAT_AREA] < 80: continue
        if centroids[label][0] >= cell.shape[1] * .48:
            keep |= labels == label
    out = cell.copy(); out[~keep] = 0
    return out


def write_meta(template, path, guid, ppu, px, py):
    text = replace_line(template, "guid:", f"guid: {guid}")
    text = replace_line(text, "  spritePixelsToUnits:", f"  spritePixelsToUnits: {ppu:.2f}")
    text = replace_line(text, "  spritePivot:", f"  spritePivot: {{x: {px:.5f}, y: {py:.5f}}}")
    Path(str(path) + ".meta").write_text(text)


def split_unit(key, cfg, world_h):
    src = Image.open(ROOT / "artsrc/animation" / cfg["sheet"]).convert("RGBA")
    rgba = np.asarray(src); xs = [round(i * src.width / 8) for i in range(9)]
    ys = [round(i * src.height / 4) for i in range(5)]
    original = Image.open(ORIGINAL_ROOT / cfg["original"]).convert("RGBA")
    original_rgba = np.asarray(original)
    original_xs = [round(i * original.width / 4) for i in range(5)]
    original_ys = [round(i * original.height / 4) for i in range(5)]
    template = (UNIT_ROOT / key / "idle_1.png.meta").read_text()
    first = largest(rgba[ys[0]:ys[1], xs[0]:xs[1]].copy())
    _x0, y0, _x1, y1 = bbox(first[..., 3]); ppu = (y1 - y0) / world_h
    files = {}
    for row, state in enumerate(("idle", "walk", "attack", "dead")):
        files[state] = []
        use_original = state in cfg["original_states"]
        state_xs = original_xs if use_original else xs
        state_ys = original_ys if use_original else ys
        state_rgba = original_rgba if use_original else rgba
        ground = state_ys[row + 1] - 7
        row_labels = row_parts = None
        if key == "machine_common" and state != "attack":
            row_image = state_rgba[state_ys[row]:state_ys[row + 1]].copy()
            row_image[row_image[..., 3] < 18] = 0
            n, row_labels, row_stats, _ = cv2.connectedComponentsWithStats(
                (row_image[..., 3] > 16).astype(np.uint8), 8)
            row_parts = [i for i in range(1, n) if row_stats[i, cv2.CC_STAT_AREA] >= 100]
            row_parts.sort(key=lambda i: row_stats[i, cv2.CC_STAT_LEFT])
            if len(row_parts) != 8:
                raise RuntimeError(f"Expected 8 {key} {state} silhouettes, found {len(row_parts)}")
        for col in range(8):
            # The AI-generated 8-frame sheets occasionally draw a pose across
            # the next cell. Once two poses overlap in a flattened PNG, no crop
            # can recover the hidden pixels. For affected states, use the four
            # clean authored frames twice each; this preserves the 8-step clip
            # timing without leaking pixels from adjacent poses.
            source_col = col // 2 if use_original else col
            if row_parts is not None:
                label = row_parts[col]
                x0 = int(row_stats[label, cv2.CC_STAT_LEFT])
                y0 = int(row_stats[label, cv2.CC_STAT_TOP])
                x1 = x0 + int(row_stats[label, cv2.CC_STAT_WIDTH])
                y1 = y0 + int(row_stats[label, cv2.CC_STAT_HEIGHT])
                crop = row_image[y0:y1, x0:x1].copy()
                crop[row_labels[y0:y1, x0:x1] != label] = 0
                cell_origin_x = 0
            else:
                cell = state_rgba[
                    state_ys[row]:state_ys[row + 1],
                    state_xs[source_col]:state_xs[source_col + 1],
                ].copy()
                cell[cell[..., 3] < 18] = 0
                cell = machine_attack_parts(cell) if key == "machine_common" and state == "attack" else largest(cell)
                x0, y0, x1, y1 = bbox(cell[..., 3]); crop = cell[y0:y1, x0:x1]
                cell_origin_x = state_xs[source_col]
            out = np.zeros((crop.shape[0] + PAD * 2, crop.shape[1] + PAD * 2, 4), np.uint8)
            out[PAD:PAD + crop.shape[0], PAD:PAD + crop.shape[1]] = crop
            root_x = (state_xs[source_col] + state_xs[source_col + 1]) * .5
            px = (root_x - (cell_origin_x + x0) + PAD) / out.shape[1]
            py = ((state_ys[row] + y1) + PAD - ground) / out.shape[0]
            rel = f"{key}/{state}_smooth_{col + 1}.png"; path = UNIT_ROOT / rel
            path.parent.mkdir(parents=True, exist_ok=True); Image.fromarray(out, "RGBA").save(path)
            guid = hashlib.md5(f"spinsquad/unit-smooth/{rel}".encode()).hexdigest()
            write_meta(template, path, guid, ppu, min(1, max(0, px)), min(1, max(0, py)))
            files[state].append(rel)
    if key == "machine_common":
        files["attack"] = split_machine_attack(template, ppu)
    return round(ppu, 2), files


def split_machine_attack(template, ppu):
    src = Image.open(MACHINE_ATTACK).convert("RGBA"); rgba = np.asarray(src)
    xs = [round(i * src.width / 8) for i in range(9)]
    # Frame 5 is wider than its nominal 1/8 cell. Splitting at equal x
    # boundaries cuts its saw blade into frames 5 and 6. The source contains
    # eight genuinely separate silhouettes, so extract those components from
    # the full strip instead of trusting cell boundaries.
    mask = (rgba[..., 3] > 16).astype(np.uint8)
    n, labels, stats, _ = cv2.connectedComponentsWithStats(mask, 8)
    parts = [i for i in range(1, n) if stats[i, cv2.CC_STAT_AREA] >= 100]
    parts.sort(key=lambda i: stats[i, cv2.CC_STAT_LEFT])
    if len(parts) != 8:
        raise RuntimeError(f"Expected 8 machine attack silhouettes, found {len(parts)}")
    ground = max(int(stats[i, cv2.CC_STAT_TOP] + stats[i, cv2.CC_STAT_HEIGHT]) for i in parts)
    files = []
    for col, label in enumerate(parts):
        x0 = int(stats[label, cv2.CC_STAT_LEFT]); y0 = int(stats[label, cv2.CC_STAT_TOP])
        x1 = x0 + int(stats[label, cv2.CC_STAT_WIDTH]); y1 = y0 + int(stats[label, cv2.CC_STAT_HEIGHT])
        crop = rgba[y0:y1, x0:x1].copy()
        crop[labels[y0:y1, x0:x1] != label] = 0
        out = np.zeros((crop.shape[0] + PAD * 2, crop.shape[1] + PAD * 2, 4), np.uint8)
        out[PAD:PAD + crop.shape[0], PAD:PAD + crop.shape[1]] = crop
        root_x = (xs[col] + xs[col + 1]) * .5
        px = (root_x - x0 + PAD) / out.shape[1]
        py = (y1 + PAD - ground) / out.shape[0]
        rel = f"machine_common/attack_smooth_{col + 1}.png"; path = UNIT_ROOT / rel
        Image.fromarray(out, "RGBA").save(path)
        guid = hashlib.md5(f"spinsquad/unit-smooth/{rel}".encode()).hexdigest()
        write_meta(template, path, guid, ppu, min(1,max(0,px)), min(1,max(0,py)))
        files.append(rel)
    return files


def split_attack_vfx(key, cfg):
    src = Image.open(VFX_SHEET).convert("RGBA"); rgba = np.asarray(src)
    xs = [round(i * src.width / 8) for i in range(9)]
    ys = [round(i * src.height / 4) for i in range(5)]
    template = (UNIT_ROOT / key / "idle_1.png.meta").read_text(); files = []
    for col in range(8):
        cell = rgba[ys[cfg["row"]]:ys[cfg["row"] + 1], xs[col]:xs[col + 1]].copy()
        cell[cell[..., 3] < 18] = 0; x0, y0, x1, y1 = bbox(cell[..., 3]); crop = cell[y0:y1, x0:x1]
        rel = f"{key}/{cfg['effect']}_{col + 1}.png"; path = VFX_ROOT / rel
        path.parent.mkdir(parents=True, exist_ok=True); Image.fromarray(crop, "RGBA").save(path)
        guid = hashlib.md5(f"spinsquad/vfx/{rel}".encode()).hexdigest()
        write_meta(template, path, guid, 260, .5, .5); files.append(rel)
    return files


def clip(files, durations, loop, forward=False):
    d = dict(files=files, durations=durations, sequence=SEQ, loop=loop)
    if forward: d["playback"] = "forward"
    return d


def guid_for(root, rel):
    text = Path(str(root / rel) + ".meta").read_text()
    return re.search(r"^guid: ([0-9a-f]{32})$", text, re.M).group(1)


def clip_yaml(name, data, root):
    lines = [f"  {name}:"]
    if data and data.get("files"):
        lines.append("    Frames:")
        for rel in data["files"]:
            lines.append(f"    - {{fileID: 21300000, guid: {guid_for(root, rel)}, type: 3}}")
        lines += ["    Durations: [" + ", ".join(str(x) for x in data["durations"]) + "]",
                  "    Sequence: 0000000001000000020000000300000004000000050000000600000007000000",
                  f"    Loop: {1 if data.get('loop') else 0}"]
        if data.get("playback"):
            lines += ["    UseExplicitPlayback: 1", f"    PingPong: {1 if data['playback']=='ping_pong' else 0}"]
    else:
        lines += ["    Frames: []", "    Durations: []", "    Sequence: ", "    Loop: 0"]
    return lines


def write_asset(key, entry):
    old = (DATA_ROOT / f"Anim_{key}.asset").read_text()
    script_guid = re.search(r"m_Script: \{fileID: 11500000, guid: ([0-9a-f]{32})", old).group(1)
    head = ["%YAML 1.1", "%TAG !u! tag:unity3d.com,2011:", "--- !u!114 &11400000", "MonoBehaviour:",
            "  m_ObjectHideFlags: 0", "  m_CorrespondingSourceObject: {fileID: 0}",
            "  m_PrefabInstance: {fileID: 0}", "  m_PrefabAsset: {fileID: 0}", "  m_GameObject: {fileID: 0}",
            "  m_Enabled: 1", "  m_EditorHideFlags: 0",
            f"  m_Script: {{fileID: 11500000, guid: {script_guid}, type: 3}}",
            f"  m_Name: Anim_{key}", "  m_EditorClassIdentifier: Assembly-CSharp::Project.Data.UnitAnimationSet"]
    lines = head
    for state, label in (("idle", "Idle"), ("walk", "Walk"), ("attack", "Attack"), ("dead", "Dead")):
        lines += clip_yaml(label, entry["anims"][state], UNIT_ROOT)
    lines += clip_yaml("Skill", entry.get("anims", {}).get("skill"), UNIT_ROOT)
    v = entry.get("vfx", {})
    lines += clip_yaml("WalkVfx", v.get("walk"), VFX_ROOT)
    lines += clip_yaml("AttackVfx", v.get("attack"), VFX_ROOT)
    lines += clip_yaml("DeathVfx", v.get("dead"), VFX_ROOT)
    for field, key2, default in (("WalkVfxOffset", "walk_offset", {"x":0,"y":0}),
                                  ("AttackVfxOffset", "attack_offset", {"x":0,"y":0}),
                                  ("DeathVfxOffset", "dead_offset", {"x":0,"y":0})):
        q=v.get(key2,default); lines.append(f"  {field}: {{x: {q['x']}, y: {q['y']}}}")
    lines += [f"  WalkVfxFootSpacing: {v.get('foot_spacing', .1)}",
              f"  WalkVfxScale: {v.get('walk_scale', 1)}", f"  AttackVfxScale: {v.get('attack_scale', 1)}",
              f"  DeathVfxScale: {v.get('dead_scale', 1)}", f"  WalkVfxAlpha: {v.get('walk_alpha', 1)}",
              f"  AttackVfxAlpha: {v.get('attack_alpha', 1)}", f"  DeathVfxAlpha: {v.get('dead_alpha', 1)}",
              f"  ImpactStep: {entry.get('impact_step', 4)}"]
    (DATA_ROOT / f"Anim_{key}.asset").write_text("\n".join(lines) + "\n")


def main():
    manifest = json.loads(MANIFEST.read_text())
    by_key = {e["key"]: e for e in manifest}
    for key, cfg in CONFIG.items():
        e = by_key[key]; ppu, files = split_unit(key, cfg, e["world_h"]); attack = split_attack_vfx(key, cfg)
        e["ppu"] = ppu
        e["anims"] = {s: clip(files[s], DUR[s], s != "dead", s in ("idle", "walk")) for s in DUR}
        vfx = {
            "walk": clip([f"human_common/dust_{i}.png" for i in range(1,9)], DUR["walk"], True),
            "attack": clip(attack, DUR["attack"], False),
            "walk_offset": {"x":0.0,"y":-0.02}, "foot_spacing": cfg["foot_spacing"],
            "attack_offset": {"x":cfg["attack_offset"][0],"y":cfg["attack_offset"][1]},
            "dead_offset": {"x":0.0,"y":0.05}, "walk_scale":cfg["dust_scale"],
            "attack_scale":cfg["attack_scale"], "dead_scale":.38,
            "walk_alpha":cfg["dust_alpha"], "attack_alpha":.65, "dead_alpha":.55,
        }
        if cfg["soul"]:
            vfx["dead"] = clip([f"human_common/soul_{i}.png" for i in range(1,9)], DUR["dead"], False)
        e["vfx"] = vfx; write_asset(key, e)
        print(f"{key}: PPU {ppu}, 32 frame + 8 attack VFX")
    MANIFEST.write_text(json.dumps(manifest, ensure_ascii=False, indent=1) + "\n")


if __name__ == "__main__": main()
