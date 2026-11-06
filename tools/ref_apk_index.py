"""Doc catalogue sprite UI cua game tham khao (Spin Squad 2.5.1) tu file .xapk.

Chi doc TEN, KICH THUOC va VIEN 9-slice de hoc ti le va muc do chi tiet.
Khong trich xuat anh: art cua game khac khong duoc dua vao du an nay.

Dung:  python3 tools/ref_apk_index.py ~/spin/Spin+Squad!_2.5.1_APKPure.xapk
File .xapk la zip long nhau, va cac file .assets bi cat thanh *.splitN phai noi
lai DUNG THU TU SO (split0, split1, ... split10) — sort theo chu se sai va
UnityPy bao "Negative length read from TypeTree".
"""
import re, shutil, subprocess, sys, tempfile, zipfile
from pathlib import Path

OUT = Path(__file__).resolve().parents[1] / 'doc/reference/spinsquad-apk-ui-index.md'
KEYS = re.compile(r'btn|button|frame|panel|ribbon|banner|tab|popup|window|slot', re.I)


def unpack(xapk, work):
    with zipfile.ZipFile(xapk) as z:
        inner = next(n for n in z.namelist() if n.endswith('base_assets.apk'))
        z.extract(inner, work)
    data = work / 'data'
    with zipfile.ZipFile(work / inner) as z:
        for n in z.namelist():
            if '/Data/sharedassets' in n:
                z.extract(n, data)
    folder = next(p for p in data.rglob('bin/Data'))
    joined = work / 'joined'
    joined.mkdir()
    bases = {p.name.split('.split')[0] for p in folder.glob('*.split*')}
    for base in bases:
        parts = sorted(folder.glob(base + '.split*'), key=lambda p: int(p.name.rsplit('split', 1)[1]))
        with open(joined / base, 'wb') as out:
            for part in parts:
                shutil.copyfileobj(open(part, 'rb'), out)
    for p in folder.glob('*.resource'):
        shutil.copy(p, joined / p.name)
    return joined


def main():
    import UnityPy
    xapk = Path(sys.argv[1]).expanduser()
    work = Path(tempfile.mkdtemp(prefix='refapk-'))
    try:
        joined = unpack(xapk, work)
        rows = set()
        for f in sorted(joined.glob('*.assets')):
            try:
                env = UnityPy.load(str(f))
            except Exception:
                continue
            for o in env.objects:
                if o.type.name != 'Sprite':
                    continue
                try:
                    d = o.read()
                except Exception:
                    continue
                if not KEYS.search(d.m_Name or ''):
                    continue
                b = d.m_Border
                rows.add((d.m_Name, int(d.m_Rect.width), int(d.m_Rect.height),
                          int(b.x), int(b.y), int(b.z), int(b.w)))
        lines = ['# Catalogue sprite UI — game tham khảo Spin Squad 2.5.1', '',
                 'Sinh bằng `tools/ref_apk_index.py`. **Chỉ là số đo để học tỉ lệ.**',
                 'Art của game khác không được đưa vào dự án này.', '',
                 f'Nguồn: `{xapk.name}` · {len(rows)} sprite UI', '',
                 '| tên | rộng | cao | viền 9-slice (l,b,r,t) |', '|---|---|---|---|']
        for name, w, h, x, y, z, t in sorted(rows):
            border = 'không (cỡ cố định)' if (x, y, z, t) == (0, 0, 0, 0) else f'{x}, {y}, {z}, {t}'
            lines.append(f'| `{name}` | {w} | {h} | {border} |')
        OUT.write_text('\n'.join(lines) + '\n')
        print(f'{len(rows)} sprite -> {OUT}')
    finally:
        shutil.rmtree(work, ignore_errors=True)


main()
