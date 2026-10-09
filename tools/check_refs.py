#!/usr/bin/env python3
"""Tim tham chieu tro vao hu khong.

Unity noi moi thu bang GUID nam trong file .meta. Xoa hoac tao lai mot .meta
la moi tham chieu toi no chet IM LANG — Inspector chi hien o trong, khong bao
loi. Script nay quet toan bo project va chi ra cho hong.

    python3 tools/check_refs.py
"""
import re, glob, os, sys

ROOT = os.path.join(os.path.dirname(os.path.abspath(__file__)), '..')
os.chdir(ROOT)

own = set()
for m in glob.glob('Assets/**/*.meta', recursive=True) + \
         glob.glob('Packages/**/*.meta', recursive=True):
    mm = re.search(r'^guid: ([0-9a-f]{32})', open(m, errors='ignore').read(), re.M)
    if mm:
        own.add(mm.group(1))

targets = []
for pat in ('Assets/_Project/**/*.unity', 'Assets/_Project/**/*.prefab',
            'Assets/_Project/**/*.asset', 'Assets/_Project/**/*.controller',
            'Assets/_Project/**/*.anim',  'Assets/_Project/**/*.meta'):
    targets += glob.glob(pat, recursive=True)

bad = {}
for f in targets:
    txt = open(f, errors='ignore').read()
    for g in set(re.findall(r'guid: ([0-9a-f]{32})', txt)):
        if g in own or g.startswith('0000000000000000'):
            continue            # GUID noi bo cua Unity
        bad.setdefault(f, set()).add(g)

# --- phep kiem 2: dong YAML hong ---
# "khoa:" xuong dong roi moi "[]" -> Unity doc "[]" la mot khoa moi va bao
# "Expect ':' between key and value". Loi nay da xay ra HAI lan, xem Handoff/11 muc A1.
yaml_bad = {}
for f in targets:
    if f.endswith('.meta'): continue
    for i, l in enumerate(open(f, errors='ignore'), 1):
        if l.strip() in ('[]', '{}') and l != l.lstrip():
            yaml_bad.setdefault(f, []).append(i)

# --- phep kiem 3: fileID tran kieu int64 ---
# Unity doc fileID bang so nguyen 64-bit co dau. Vuot qua thi no kep ve
# 9223372036854775807 va bao "Broken text PPtr ... doesn't exist".
INT64_MAX = 2**63 - 1
overflow = {}
for f in targets:
    if f.endswith('.meta'): continue
    over = {m for m in re.findall(r'(?:&|fileID: )(-?\d{10,})', open(f, errors='ignore').read())
            if abs(int(m)) > INT64_MAX}
    if over: overflow[f] = sorted(over)

print(f'Quet {len(targets)} file, project so huu {len(own)} GUID.')
if overflow:
    print('=> fileID TRAN KIEU int64 (gioi han %d):' % INT64_MAX)
    for f, ids in overflow.items():
        print(' ', f, ids)
if yaml_bad:
    print('=> DONG YAML HONG ("[]" dung mot minh sau dau hai cham):')
    for f, lines in yaml_bad.items():
        print(' ', f, 'dong', lines)
if not bad:
    print('=> Khong co tham chieu hong.')
    sys.exit(0)

print('=> THAM CHIEU HONG:')
for f, gs in sorted(bad.items()):
    print(' ', f)
    for g in sorted(gs):
        print('     ', g)
sys.exit(1)
