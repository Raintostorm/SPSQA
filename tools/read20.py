#!/usr/bin/env python3
"""Doc bo 20model: lam phang BON bien the manifest thanh mot dang duy nhat.

Bo nay khong dong nhat — README cua chinh no da canh bao. Do duoc:
  - anh nguon tro bang 3 kieu: sources[] + source:int | source:"path" | sheet
  - khung hinh nam o 'poses' hoac 'frames'
  - thoi luong o 'frame_durations' hoac 'frame_durations_seconds' hoac khong co
  - 'sequence' co the vang mat (coi nhu 1..n)

Khong sua file goc. Moi cho doan phai GHI RO ra, khong im lang.
"""

import json
from pathlib import Path

ANIMS = ("idle", "walk", "attack", "dead", "skill")


def _frames(a):
    return a.get("poses") or a.get("frames") or []


def _durations(a, n):
    d = a.get("frame_durations") or a.get("frame_durations_seconds")
    if d and len(d) == n:
        return list(d)
    total = a.get("duration_seconds", 1.0)
    return [total / max(1, n)] * n


def _sequence(a, n):
    s = a.get("sequence")
    if s and len(s) == n:
        return [i - 1 for i in s]          # manifest dem tu 1
    return list(range(n))


def _source_list(doc):
    """Danh sach anh ma 'source: <so>' tro toi.

    Co manifest khai han 'sources': [...]. Co manifest KHONG khai, chi co
    'sheet' va 'walk_correction_sheet' — ma pose van ghi 'source: 1'. Luc do
    chi so 1 nghia la tam thu hai, tuc tam sua loi.

    Da vap: khong dung luat nay thi elf/common lay nham tam, rect tro ra
    ngoai anh (can toi x=2116 tren tam chi rong 1254) nen khung thu tu ra
    RONG — va khong co loi nao bao ca.
    """
    srcs = doc.get("sources")
    if isinstance(srcs, list) and srcs:
        return list(srcs)
    out = []
    for key in ("sheet", "base_sheet"):
        if doc.get(key):
            out.append(doc[key])
            break
    for key in ("walk_correction_sheet", "correction_sheet"):
        if doc.get(key):
            out.append(doc[key])
    return out


def _sheet_for(doc, root, pose, anim_name):
    """Duong dan anh cua mot khung hinh, theo ca ba kieu tro."""
    src = pose.get("source")
    if isinstance(src, str):
        return root / src
    srcs = _source_list(doc)
    if isinstance(src, int):
        if src < len(srcs):
            return root / srcs[src]
        raise KeyError(f"'{anim_name}': source={src} ma chi co {len(srcs)} tam")
    if srcs:
        return root / srcs[0]
    raise KeyError(f"khong tim duoc anh nguon cho '{anim_name}'")


def load(manifest):
    """Tra ve dict phang: id, race, rank, combat, target_h, anims{name:[frame...]}"""
    p = Path(manifest)
    doc = json.loads(p.read_text())
    root = p.parent
    out = dict(
        id=doc.get("id", p.parent.parent.name + "-" + p.parent.name),
        name=doc.get("name", ""),
        race=doc.get("race", p.parts[-3]),
        rank=doc.get("rank", p.parts[-2]),
        combat=doc.get("combat", "?"),
        weapon=doc.get("weapon", "?"),
        target_h=doc.get("sizing_policy", {}).get("target_idle_height_px", 80.0),
        quirks=[],
        anims={},
    )
    if "sources" not in doc:
        out["quirks"].append("khong co sources[]")

    for name in ANIMS:
        a = doc["animations"].get(name)
        if a is None:
            continue
        fr = _frames(a)
        if not fr:
            continue
        if "poses" not in a:
            out["quirks"].append(f"{name}: dung 'frames'")
        if "sequence" not in a:
            out["quirks"].append(f"{name}: thieu 'sequence'")
        if "frame_durations" not in a and "frame_durations_seconds" not in a:
            out["quirks"].append(f"{name}: thieu 'frame_durations'")

        durs = _durations(a, len(fr))
        seq = _sequence(a, len(fr))
        frames = []
        for f in fr:
            frames.append(dict(
                sheet=_sheet_for(doc, root, f, name),
                rect=f["rect"],
                anchor=(f.get("anchor_x"), f.get("anchor_y")),
                norm=f.get("normalization", 1.0),
            ))
        out["anims"][name] = dict(frames=frames, durations=durs, sequence=seq,
                                  loop=a.get("loop", True))
    return out


def load_all(base="20model"):
    base = Path(base)
    return [load(m) for m in sorted(base.glob("*/*/character.json"))]


if __name__ == "__main__":
    import sys
    base = sys.argv[1] if len(sys.argv) > 1 else "20model"
    tot = 0
    for c in load_all(base):
        n = sum(len(v["frames"]) for v in c["anims"].values())
        tot += n
        q = ("   ← " + "; ".join(c["quirks"])) if c["quirks"] else ""
        print(f"{c['race']:8} {c['rank']:10} {c['combat']:7} "
              f"{len(c['anims'])} anim, {n:2} khung{q}")
    print(f"\ntong {tot} khung hinh")
