"""Replay source sprite, cubic curves and prefab hierarchy. Local reference only."""
import copy
import json
import math
import struct
import zlib
from pathlib import Path
import numpy as np
from PIL import Image, ImageDraw

ROOT = Path(__file__).resolve().parents[1]
SRC = ROOT / 'outdata/reference/bandit_rig_study'
CLIPS = ROOT / 'outdata/reference/spinsquad_motion_1005'
nodes = json.loads((SRC / '1005_bandit_rig.json').read_text())
metadata = json.loads((SRC / 'sprite_metadata.json').read_text())
defaults = {}
for node in nodes:
    if node.get('type') not in ('Transform', 'SpriteRenderer', 'GameObject'):
        continue
    path = node['path'].split('/', 1)[-1] if '/' in node['path'] else ''
    defaults.setdefault(path, {}).update(node['data'])
hashes = {zlib.crc32(path.encode()): path for path in defaults}
images = {key: Image.open(SRC / (m['name'] + '.png')).convert('RGBA') for key, m in metadata.items()}


def decode(name):
    original = json.loads((CLIPS / f'1005_bandit_{name}.json').read_text())
    data = original['m_MuscleClip']['m_Clip']['data']
    stream = data['m_StreamedClip']
    # Unity stores discrete sprite channels AFTER normal streamed curves.
    # curveCount alone excludes them; treating them as constants freezes art.
    streamed_count = stream['curveCount'] + stream.get('discreteCurveCount', 0)
    raw = struct.pack('<' + 'I' * len(stream['data']), *stream['data'])
    offset, curves = 0, {}
    while offset < len(raw):
        time, count = struct.unpack_from('<fi', raw, offset)
        offset += 8
        for _ in range(count):
            index, *coeff = struct.unpack_from('<i4f', raw, offset)
            offset += 20
            if time < 100:
                curves.setdefault(index, []).append((time, coeff))
    slots = []
    for binding in original['m_ClipBindingConstant']['genericBindings']:
        width = {1: 3, 2: 4, 3: 3, 4: 3}.get(binding['attribute'], 1) if binding['typeID'] == 4 else 1
        slots.extend((binding, component) for component in range(width))
    assert len(slots) == streamed_count + data['m_DenseClip']['m_CurveCount'] + len(data['m_ConstantClip']['data']), name
    def value(index, t):
        if index >= streamed_count:
            ci = index - streamed_count - data['m_DenseClip']['m_CurveCount']
            constants = data['m_ConstantClip']['data']
            return constants[ci] if 0 <= ci < len(constants) else 0
        keys = curves[index]
        time, co = keys[0]
        for key_time, key_co in keys:
            if key_time <= t:
                time, co = key_time, key_co
        dt = max(0, t - time) if time >= 0 else 0
        return ((co[0] * dt + co[1]) * dt + co[2]) * dt + co[3]
    return original, slots, value


def render(name, t):
    original, slots, value = decoders[name]
    states = copy.deepcopy(defaults)
    for index, (binding, component) in enumerate(slots):
        path = hashes.get(binding['path'])
        if path not in states:
            continue
        state = states[path]
        v = value(index, t)
        if binding['isPPtrCurve']:
            pointers = original['m_ClipBindingConstant']['pptrCurveMapping']
            state['m_Sprite'] = pointers[max(0, min(len(pointers) - 1, round(v)))]
        elif binding['typeID'] == 4:
            key = {1: 'm_LocalPosition', 3: 'm_LocalScale', 4: 'euler'}.get(binding['attribute'])
            if key:
                state.setdefault(key, {'x': 0, 'y': 0, 'z': 0})['xyz'[component]] = v
        elif binding['typeID'] == 1:
            state['m_IsActive'] = v > .5
        elif binding['typeID'] == 212 and binding['customType'] == 26:
            state['alpha'] = max(0, min(1, v))
    matrices = {}
    visible = {}
    for path in sorted(states, key=lambda p: p.count('/') if p else -1):
        state = states[path]
        parent = path.rsplit('/', 1)[0] if '/' in path else ''
        pos = state.get('m_LocalPosition', {'x': 0, 'y': 0})
        scale = state.get('m_LocalScale', {'x': 1, 'y': 1})
        q = state.get('m_LocalRotation', {'z': 0, 'w': 1})
        angle = math.radians(state['euler']['z']) if 'euler' in state else 2 * math.atan2(q['z'], q['w'])
        c, s = math.cos(angle), math.sin(angle)
        local = np.array([[c * scale['x'], -s * scale['y'], pos['x']],
                          [s * scale['x'], c * scale['y'], pos['y']], [0, 0, 1]])
        matrices[path] = matrices.get(parent, np.eye(3)) @ local if path else local
        visible[path] = bool(state.get('m_IsActive', True)) and (visible.get(parent, True) if path else True)
    canvas = Image.new('RGBA', (600, 340), (213, 230, 187, 255))
    draw = ImageDraw.Draw(canvas)
    draw.rectangle((0, 255, 600, 340), fill=(220, 182, 111))
    draw.text((20, 16), f'BANDIT / {name.upper()} / {t:.2f}s', fill=(25, 30, 35))
    draw.text((20, 38), 'APK sprites + native pivots + prefab transforms + source curves', fill=(25, 30, 35))
    screen = np.array([[220, 0, 300], [0, -220, 255], [0, 0, 1]])
    for path in ['character/body', 'character/Weapon_slash/Weapon']:
        state = states[path]
        pid = str(state['m_Sprite']['m_PathID'])
        if not visible[path] or pid not in images:
            continue
        image, meta = images[pid], metadata[pid]
        ppu, pivot = meta['ppu'], meta['pivot']
        pixels = np.array([[1 / ppu, 0, -image.width * pivot['x'] / ppu],
                           [0, -1 / ppu, image.height * (1 - pivot['y']) / ppu], [0, 0, 1]])
        inverse = np.linalg.inv(screen @ matrices[path] @ pixels)
        layer = image.transform(canvas.size, Image.Transform.AFFINE, tuple(inverse[:2].flatten()), Image.Resampling.BICUBIC)
        if state.get('alpha', 1) < 1:
            layer.putalpha(layer.getchannel('A').point(lambda a: round(a * state['alpha'])))
        canvas.alpha_composite(layer)
    return canvas.convert('RGB')


decoders = {name: decode(name) for name in ('idle', 'walk', 'attack', 'skill', 'Dead')}
for name, probes in {
    'idle': [(0, '1005_idle_1'), (.2, '1005_idle_2'), (.4, '1005_idle_3')],
    'walk': [(0, '1005_idle_1'), (.6, '1005_walk')],
    'attack': [(.6, '1005_attack_1'), (.9, '1005_attack_2'), (1.1, '1005_attack_3')],
    'Dead': [(.3, '1005_dead')]
}.items():
    original, slots, evaluate = decoders[name]
    index = next(i for i, (b, _) in enumerate(slots) if b['path'] == 898039781 and b['isPPtrCurve'])
    pointers = original['m_ClipBindingConstant']['pptrCurveMapping']
    for time, expected in probes:
        pid = str(pointers[round(evaluate(index, time))]['m_PathID'])
        assert metadata[pid]['name'] == expected, (name, time, pid, expected)
print('PASS: curve slot totals and exact body sprite changes at source key times')
combined, delays = [], []
for name in decoders:
    duration = decoders[name][0]['m_MuscleClip']['m_StopTime']
    frames = [render(name, min(i / 30, duration)) for i in range(math.ceil(duration * 30) + 1)]
    hold = [33] * len(frames)
    if name == 'Dead':
        hold[-1] = 1200
    frames[0].save(SRC / f'{name.lower()}_rig.gif', save_all=True, append_images=frames[1:], duration=hold, loop=0, disposal=2)
    combined.extend(frames * (1 if name in ('Dead', 'skill') else 2))
    delays.extend(hold * (1 if name in ('Dead', 'skill') else 2))
    print(name, duration, len(frames), 'frames')
combined[0].save(SRC / 'bandit_full_rig_v2.gif', save_all=True, append_images=combined[1:], duration=delays, loop=0, disposal=2)
render('attack', .95).save(SRC / 'attack_sample.png')
render('Dead', .82).save(SRC / 'dead_sample.png')
model = {
    'source': 'User-supplied Spin+Squad! 2.5.1 APK; local reference study only',
    'rig': defaults,
    'sprite_metadata': metadata,
    'clips': {name: decoders[name][0] for name in decoders},
    'limitations': 'Replays extracted visual curves, not Android gameplay, combat AI or root locomotion.'
}
(SRC / 'bandit_model.json').write_text(json.dumps(model, indent=2))
