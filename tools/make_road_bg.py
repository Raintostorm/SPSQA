#!/usr/bin/env python3
"""Ve nen tran co san con duong, mot anh moi thoi.

Khong nho AI sinh. Phong cach tham chieu (Forge Master) la MANG MAU PHANG —
co xanh, dai dat nau, vai bui cay va tang da vien den. Thu do ve bang code
chinh xac hon ta bang chu, va khop phong cach linh hon anh AI ve kieu tranh.

Vi tri con duong KHONG phai so dep tuy chon: no tinh tu camera that
(ortho 3.26) va tu laneY trong BattleLane, nen linh dung dung tren mat duong.

Chay:  python3 tools/make_road_bg.py
Ra:    Assets/_Project/Art/Backgrounds/bg_<thoi>.png   1920x1080
"""

import random
from pathlib import Path

from PIL import Image, ImageDraw

W, H = 1920, 1080
ORTHO = 3.26                 # Camera.orthographicSize, do tu BattleSceneSetup
PX = H / (ORTHO * 2)         # 165.6 pixel moi don vi the gioi

LANE_Y = -0.5                # BattleLane.laneY — chan linh dat o day
ROAD_TOP_Y = LANE_Y + 0.70   # mep tren duong, ngang hong linh
ROAD_BOT_Y = LANE_Y - 0.95   # mep duoi, chua chan hang sau

# Luoi 4x4 luc chuan bi: origin.y = -1.5, 4 hang, o rong 1 -> tam o -1.5..+1.5,
# men o -2.0..+2.0. Con duong NAM TRONG day (co y), nhung no phang va nhat nen
# khong tranh nhin. Cay coi thi co — nen moi thu noi khoi mat dat chi duoc
# dat NGOAI hai men nay. So lay tu BattleSceneSetup, khong phai uoc luong.
GRID_TOP_Y, GRID_BOT_Y = 2.0, -2.0

SS = 2                       # ve gap doi roi thu nho -> vien khong rang cua
INK_W = 6                    # be day vien den, tinh theo pixel goc


def y2px(y):
    """Toa do the gioi -> pixel, tren cung la 0."""
    return (ORTHO - y) * PX


# Moi thoi mot bang mau. 'ink' la mau vien — khong dung den tuyet doi,
# den tuyet doi lam nen nang hon linh.
PALETTE = {
    "stoneage": dict(grass=(126, 176, 78), grass2=(104, 152, 64), road=(198, 150, 92),
                     road2=(176, 128, 74), rock=(150, 158, 162), leaf=(92, 142, 68),
                     trunk=(118, 86, 58), ink=(40, 36, 30)),
    "medieval": dict(grass=(112, 166, 86), grass2=(92, 144, 72), road=(186, 146, 98),
                     road2=(162, 122, 78), rock=(158, 164, 168), leaf=(72, 128, 72),
                     trunk=(110, 80, 54), ink=(36, 34, 30)),
    "modern":   dict(grass=(122, 150, 110), grass2=(100, 128, 92), road=(136, 136, 140),
                     road2=(114, 114, 120), rock=(150, 152, 156), leaf=(86, 122, 86),
                     trunk=(96, 78, 62), ink=(34, 34, 36)),
    "future":   dict(grass=(92, 120, 138), grass2=(74, 100, 118), road=(118, 132, 150),
                     road2=(96, 110, 130), rock=(140, 172, 196), leaf=(84, 136, 148),
                     trunk=(70, 86, 104), ink=(26, 30, 38)),
}


def blob(d, x, y, r, fill, ink):
    """Mot hinh tron co vien. Ve tron den to hon truoc, roi to mau de len."""
    d.ellipse([x - r - INK_W * SS, y - r - INK_W * SS,
               x + r + INK_W * SS, y + r + INK_W * SS], fill=ink)
    d.ellipse([x - r, y - r, x + r, y + r], fill=fill)


def bush(d, x, y, s, P):
    """Ba cuc tron chum lai. Ve vien ca ba TRUOC roi moi to mau ca ba —
    lam tung cai mot thi vien cua cuc sau cat ngang mau cuc truoc."""
    lobes = [(-0.55, 0.05, 0.52), (0.0, -0.14, 0.64), (0.58, 0.08, 0.48)]
    for dx, dy, r in lobes:
        R = r * s
        d.ellipse([x + dx * s - R - INK_W * SS, y + dy * s - R - INK_W * SS,
                   x + dx * s + R + INK_W * SS, y + dy * s + R + INK_W * SS], fill=P["ink"])
    for dx, dy, r in lobes:
        R = r * s
        d.ellipse([x + dx * s - R, y + dy * s - R,
                   x + dx * s + R, y + dy * s + R], fill=P["leaf"])


def rock(d, x, y, s, P):
    pts = [(x - s, y + s * .5), (x - s * .7, y - s * .4), (x - s * .1, y - s * .78),
           (x + s * .65, y - s * .35), (x + s, y + s * .5)]
    d.polygon(pts, fill=P["ink"])
    k = 0.76                                   # co lai ve tam -> ra vien deu
    d.polygon([(x + (px - x) * k, y + (py - y) * k) for px, py in pts], fill=P["rock"])


def tree(d, x, y, s, P):
    w = s * 0.17
    d.rectangle([x - w - INK_W * SS, y - s * .95, x + w + INK_W * SS, y], fill=P["ink"])
    d.rectangle([x - w, y - s * .95, x + w, y], fill=P["trunk"])
    crown = [(-0.50, -1.18, 0.62), (0.50, -1.18, 0.58), (0.0, -1.58, 0.72)]
    for dx, dy, r in crown:
        R = r * s
        d.ellipse([x + dx * s - R - INK_W * SS, y + dy * s - R - INK_W * SS,
                   x + dx * s + R + INK_W * SS, y + dy * s + R + INK_W * SS], fill=P["ink"])
    for dx, dy, r in crown:
        R = r * s
        d.ellipse([x + dx * s - R, y + dy * s - R,
                   x + dx * s + R, y + dy * s + R], fill=P["leaf"])


def tuft(d, x, y, s, P):
    for dx, h in [(-0.5, 0.8), (0.0, 1.0), (0.5, 0.75)]:
        d.line([(x + dx * s, y), (x + dx * s * 1.6, y - h * s)],
               fill=P["ink"], width=max(2, 3 * SS))


def draw_era(era, P):
    rnd = random.Random(sum(era.encode()))     # cung ten thi cung bo cuc
    im = Image.new("RGB", (W * SS, H * SS), P["grass"])
    d = ImageDraw.Draw(im)
    top, bot = y2px(ROAD_TOP_Y) * SS, y2px(ROAD_BOT_Y) * SS

    d.rectangle([0, bot, W * SS, H * SS], fill=P["grass2"])          # co gan ta dam hon
    d.rectangle([0, top, W * SS, bot], fill=P["road"])
    d.rectangle([0, top, W * SS, top + INK_W * SS], fill=P["ink"])
    d.rectangle([0, bot - INK_W * SS, W * SS, bot], fill=P["ink"])

    for _ in range(10):                                              # vet mon tren duong
        x, y = rnd.uniform(0, W * SS), rnd.uniform(top + 40 * SS, bot - 40 * SS)
        r = rnd.uniform(14, 36) * SS
        d.ellipse([x - r, y - r * .42, x + r, y + r * .42], fill=P["road2"])

    # Trang tri CHI o hai dai NGOAI luoi. Mot cai cay cao bang nguoi dat giua
    # luoi se che mat o va linh dung trong o do.
    gtop, gbot = y2px(GRID_TOP_Y) * SS, y2px(GRID_BOT_Y) * SS

    def x_rand():
        return rnd.uniform(60 * SS, (W - 60) * SS)

    for _ in range(6):                                   # cay: chan sat men tren luoi
        tree(d, x_rand(), rnd.uniform(gtop - 20 * SS, gtop), rnd.uniform(34, 52) * SS, P)
    for _ in range(7):                                   # bui thap xen vao
        bush(d, x_rand(), rnd.uniform(gtop - 55 * SS, gtop - 6 * SS), rnd.uniform(24, 38) * SS, P)
    for _ in range(6):                                   # da va bui o tien canh
        (rock if rnd.random() < 0.5 else bush)(
            d, x_rand(), rnd.uniform(gbot + 55 * SS, (H - 60) * SS), rnd.uniform(30, 50) * SS, P)
    for _ in range(18):
        y = (rnd.uniform(40 * SS, gtop - 10 * SS) if rnd.random() < .5
             else rnd.uniform(gbot + 25 * SS, (H - 25) * SS))
        tuft(d, rnd.uniform(0, W * SS), y, rnd.uniform(10, 16) * SS, P)

    return im.resize((W, H), Image.LANCZOS)


def main():
    out = Path("Assets/_Project/Art/Backgrounds")
    out.mkdir(parents=True, exist_ok=True)
    for era, P in PALETTE.items():
        draw_era(era, P).save(out / f"bg_{era}.png")
    print(f"4 anh -> {out}")
    print(f"mat duong: the gioi y {ROAD_BOT_Y:+.2f} .. {ROAD_TOP_Y:+.2f}"
          f"  = pixel {y2px(ROAD_TOP_Y):.0f} .. {y2px(ROAD_BOT_Y):.0f}")
    print(f"chan linh (laneY {LANE_Y:+.2f}) o pixel {y2px(LANE_Y):.0f} — nam trong mat duong")
    print(f"vung luoi  : pixel {y2px(GRID_TOP_Y):.0f} .. {y2px(GRID_BOT_Y):.0f} — khong dat cay coi vao day")


if __name__ == "__main__":
    main()
