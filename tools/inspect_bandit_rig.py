"""Inspect the supplied APK locally, including sprite pivots and prefab transforms."""
import json
import sys
from pathlib import Path
import UnityPy

source = Path(sys.argv[1])
out = Path('outdata/reference/bandit_rig_study')
out.mkdir(parents=True, exist_ok=True)
for path in ([source] if source.is_file() else sorted(source.iterdir())):
    if not path.is_file():
        continue
    try:
        env = UnityPy.load(str(path))
    except Exception:
        continue
    sprites = {}
    rigs = []
    for obj in env.objects:
        if obj.type.name not in ('Sprite', 'GameObject', 'AnimationClip'):
            continue
        try:
            data = obj.read()
            name = data.m_Name
        except Exception:
            continue
        if obj.type.name == 'Sprite' and (name.startswith('1005_') or obj.path_id in (570, 1917)):
            tree = obj.read_typetree()
            sprites[str(obj.path_id)] = {'name': name, 'rect': tree.get('m_Rect'),
                'pivot': tree.get('m_Pivot'), 'ppu': tree.get('m_PixelsToUnits')}
            data.image.save(out / (name + '.png'))
        if obj.type.name == 'GameObject' and ('bandit' in name.lower() or name.startswith('1005')):
            rigs.append((name, obj))
    if sprites:
        (out / 'sprite_metadata.json').write_text(json.dumps(sprites, indent=2))
        print('SPRITES', path.name, sprites, flush=True)
    for name, obj in rigs:
        nodes = []
        visited = set()
        def walk(go, parent=''):
            if go.path_id in visited:
                return
            visited.add(go.path_id)
            d = go.read()
            full = parent + '/' + d.m_Name if parent else d.m_Name
            nodes.append({'path': full, 'type': 'GameObject', 'data': go.read_typetree()})
            for component in d.m_Component:
                pointer = component.component
                try:
                    reader = pointer.deref()
                    tree = reader.read_typetree()
                    nodes.append({'path': full, 'type': reader.type.name, 'data': tree})
                    if reader.type.name == 'Transform':
                        for child in pointer.read().m_Children:
                            walk(child.read().m_GameObject.deref(), full)
                except Exception as error:
                    nodes.append({'path': full, 'error': str(error)})
        walk(obj)
        safe = name.replace('/', '_')
        (out / (safe + '_rig.json')).write_text(json.dumps(nodes, indent=2, default=str))
        print('RIG', path.name, name, len(nodes), flush=True)
    if sprites and len(sprites) >= 8:
        break
