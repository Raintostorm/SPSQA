#!/usr/bin/env python3
"""Import one reviewed complete attack clip; preserve other animations/GUIDs."""
import argparse
import json
import re
import shutil
from datetime import datetime
from pathlib import Path
import numpy as np
from PIL import Image
from validate_attack_regions import extract
from import_remaining_common import write_meta

ROOT = Path(__file__).resolve().parents[1]


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('manifest', type=Path)
    parser.add_argument('--write', action='store_true')
    args = parser.parse_args()
    doc = json.loads(args.manifest.read_text())
    key = doc['key']
    if doc.get('reviewed') is not True:
        raise ValueError('Review complete motion, full parts, scale and anchors first')
    src = np.array(Image.open(ROOT/doc['source']).convert('RGBA'))
    if not np.any(src[...,3] == 0):
        raise ValueError('True alpha required')
    poses = extract(src, doc['frames'])
    count = len(poses)
    if not (0 <= doc['impact_step'] < count and doc['ppu'] > 0):
        raise ValueError('Invalid timing or scale')
    if any(f['duration'] <= 0 for f in doc['frames']):
        raise ValueError('Invalid duration')
    folder = ROOT/'Assets/_Project/Art/Units'/key
    asset = ROOT/'Assets/_Project/Data/Units'/f'Anim_{key}.asset'
    ledger = ROOT/'tools/units_manifest.json'
    rows = json.loads(ledger.read_text())
    row = next(r for r in rows if r['key'] == key)
    destinations = [folder/f'attack_smooth_{i+1}.png' for i in range(count)]
    # Read every metadata file first: no half import on missing identity.
    metas = [Path(str(p)+'.meta') for p in destinations]
    templates = [p.read_text() for p in metas]
    guids = [re.search(r'^guid: (\w+)', t, re.M).group(1) for t in templates]
    old_asset = asset.read_text()
    durations = [f['duration'] for f in doc['frames']]
    block = '  Attack:\n    Frames:\n' + ''.join(
        f'    - {{fileID: 21300000, guid: {g}, type: 3}}\n' for g in guids)
    block += f'    Durations: {durations}\n    Sequence: ' + ''.join(
        i.to_bytes(4,'little').hex() for i in range(count)) + '\n'
    block += '    Loop: 0\n    UseExplicitPlayback: 1\n    PingPong: 0\n'
    new_asset, replacements = re.subn(r'  Attack:\n.*?(?=  Dead:)', block, old_asset, flags=re.S)
    if replacements != 1:
        raise ValueError('Attack block missing/ambiguous')
    new_asset, replacements = re.subn(r'^  ImpactStep: \d+',
        f'  ImpactStep: {doc["impact_step"]}', new_asset, flags=re.M)
    if replacements != 1:
        raise ValueError('ImpactStep missing/ambiguous')
    if not args.write:
        print(f'{key}: validated {count} complete frames; no writes')
        return
    backup = ROOT/'outdata'/f'attack_import_backup_{datetime.now():%Y%m%d_%H%M%S}'
    backup.mkdir(parents=True)
    shutil.copy2(asset, backup/asset.name)
    shutil.copy2(ledger, backup/ledger.name)
    for path, meta in zip(destinations, metas):
        shutil.copy2(path, backup/path.name)
        shutil.copy2(meta, backup/meta.name)
    for path, template, guid, (pixels, pivot) in zip(destinations, templates, guids, poses):
        Image.fromarray(pixels).save(path)
        write_meta(template, path, guid, doc['ppu'], *pivot)
    row['anims']['attack'] = dict(files=[f'{key}/{p.name}' for p in destinations],
        durations=durations, sequence=list(range(count)), loop=False, playback='forward')
    row['impact_step'] = doc['impact_step']
    # Unit ppu remains untouched: other clips keep their existing scale.
    row['anims']['attack']['ppu'] = doc['ppu']
    ledger.write_text(json.dumps(rows, ensure_ascii=False, indent=1)+'\n')
    asset.write_text(new_asset)
    print(f'{key}: imported {count} frames; backup {backup}')


if __name__ == '__main__':
    main()
