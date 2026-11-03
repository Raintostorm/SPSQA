"""Original six-pose bandit trial: crop intact art and render authored motion."""
import json
import math
import argparse
import sys
from pathlib import Path
import numpy as np
from scipy import ndimage
from PIL import Image, ImageDraw
from import_pages_art import row_major

ROOT = Path(__file__).resolve().parents[1]
parser = argparse.ArgumentParser()
parser.add_argument('--directory', default='artsrc/units_simple/bandit_trial')
parser.add_argument('--effects', action='store_true')
parser.add_argument('--profile', choices=['bandit', 'elf', 'skeleton', 'spider', 'machine', 'spear', 'wisp', 'bear', 'gunner', 'mage', 'haunted', 'scorpion', 'flamethrower', 'royal', 'royal_plant', 'verdant', 'king', 'lion', 'titan', 'titan_guard'], default='bandit')
parser.add_argument('--skills', action='store_true', help='Nine authored poses, including three dedicated skill poses')
parser.add_argument('--expressive', action='store_true', help='Subtle attack squash/stretch, grounded at feet')
parser.add_argument('--strong-fx', action='store_true', help='Readable weapon afterimages for Rare revision 2')
parser.add_argument('--export-unity', help='Export transparent baked motion frames to this directory instead of GIFs')
parser.add_argument('--idle-only', action='store_true', help='Rebake idle without touching other animations')
parser.add_argument('--readable-trial', action='store_true', help='Human Common motion and ground-ring preview only')
parser.add_argument('--demo-output', help='Separate output directory for a review trial')
args = parser.parse_args()
OUT = ROOT / args.directory
a = np.array(Image.open(OUT / 'poses.png').convert('RGBA'))
labels, _ = ndimage.label(a[..., 3] > 5)
# A wide spear bbox can contain a neighbour's plume despite separate figures.
# Assign only transparent-edge pixels to their nearest connected silhouette;
# retain the ORIGINAL alpha values, never threshold/posterise the output alpha.
nearest = ndimage.distance_transform_edt(labels == 0, return_distances=False, return_indices=True)
owners = labels[tuple(nearest)]
parts = []
for index, bounds in enumerate(ndimage.find_objects(labels), 1):
    if bounds is None or np.count_nonzero(labels[bounds] == index) < 1000:
        continue
    y, x = bounds
    parts.append((y.start, y.stop - 1, x.start, x.stop - 1, index))
expected = 9 if args.skills else 6
assert len(parts) == expected, f'Expected {expected} independent complete poses, got {len(parts)}'
names = ['idle', 'walk', 'windup', 'strike', 'recovery', 'hurt']
if args.skills:
    names += ['skill_windup', 'skill_hit', 'skill_recovery']
sprites, pivots = [], []
for name, (y0, y1, x0, x1, component) in zip(names, row_major(parts)):
    box = (max(0, x0 - 4), max(0, y0 - 4), min(a.shape[1], x1 + 5), min(a.shape[0], y1 + 5))
    image = Image.fromarray(a).crop(box)
    cropped = np.array(image)
    x_start, y_start, x_stop, y_stop = box
    owned = owners[y_start:y_stop, x_start:x_stop] == component
    cropped[...,3] = np.where(owned, cropped[...,3], 0)
    image = Image.fromarray(cropped)
    alpha = np.array(image)[..., 3]
    # The planted foot region supplies a ground pivot, not the weapon bbox.
    ys, xs = np.where(alpha > 80)
    bottom = ys.max()
    if args.skills:
        # Forward sword/mace tips must not become the ground anchor.
        rows, columns = np.indices(alpha.shape)
        feet = (alpha>80)&(columns<alpha.shape[1]*.72)&(rows>alpha.shape[0]*.65)
        if feet.any():
            bottom = np.where(feet)[0].max()
    foot = (alpha > 80) & (np.indices(alpha.shape)[0] >= bottom - 12) & (np.indices(alpha.shape)[0] <= bottom)
    if args.skills:
        foot &= np.indices(alpha.shape)[1]<alpha.shape[1]*.72
    px = float(np.where(foot)[1].mean())
    pivots.append((px, float(bottom)))
    sprites.append(image)
    image.save(OUT / (name + '.png'))
base_height = float(np.ptp(np.where(np.array(sprites[0])[..., 3] > 80)[0]) + 1)


def smooth(t):
    t = max(0, min(1, t))
    return t * t * (3 - 2 * t)


def motion(clip, t):
    if clip == 'skill':
        phase = t % 1.4
        if args.profile == 'royal_plant':
            # Grounded preparation, vertical plant, readable hold, withdrawal.
            if phase < .28:
                return 6, 1, 1, 0, 0, 0
            if phase < .90:
                return 7, 1, 1, 0, 0, 0
            if phase < 1.12:
                return 8, 1, 1, 0, 0, 0
            return 0, 1, 1, 0, 0, 0
        if args.profile == 'titan_guard':
            # Brace -> raise/cross forearms -> hold virtual armor -> lower.
            # No punch, piston extension, forward lunge or impact recoil.
            if phase < .25:
                return 6, 1, 1, 0, 0, 0
            if phase < .90:
                return 7, 1, 1, 0, 0, 0
            if phase < 1.10:
                return 8, 1, 1, 0, 0, 0
            return 0, 1, 1, 0, 0, 0
        if phase < .28:
            force = smooth(phase/.28)
            return 6, 1-.025*force, 1+.045*force, 0, -3*force, 0
        hit_end = .58 if args.profile in ('royal', 'titan') else .46
        if phase < hit_end:
            hit = smooth((phase-.28)/.18)
            if args.profile == 'royal':
                return 7, 1, 1, 0, 4, -20*(1-hit)
            if args.profile == 'verdant':
                return 7, 1-.035*(1-hit), 1+.035*(1-hit), 0, -3, 0
            return 7, 1, 1, 0, 5*hit if args.profile == 'titan' else 0, 0
        if phase < .74:
            return 8, 1, 1, 0, 0, 0
        return 0, 1, 1, 0, 0, 0
    if clip == 'idle':
        breath = math.sin(t * math.tau / .9)
        # Stronger breathing, scaled around the unchanged ground anchor.
        return 0, 1 - .035 * breath, 1 + .045 * breath, 0, 0, -5*breath if args.profile=='wisp' else 0
    if clip == 'walk':
        phase = (t % .5) / .5
        hop = math.sin(math.pi * phase)
        if args.readable_trial and args.profile == 'bandit':
            return (1 if phase < .5 else 0), 1 - .045 * hop, 1 + .055 * hop, 2*math.sin(math.tau*phase), 60*math.sin(t*.7), -5*hop
        if args.profile in ('spider', 'machine', 'gunner', 'wisp', 'haunted', 'scorpion', 'flamethrower'):
            return (1 if phase < .5 else 0), 1, 1, 0, 60 * math.sin(t * .7), -1.5 * hop
        return (1 if phase < .5 else 0), 1 - .025 * hop, 1 + .025 * hop, 0, 60 * math.sin(t * .7), -3 * hop
    if clip == 'attack':
        t %= 1.15
        if args.readable_trial and args.profile == 'bandit':
            if t < .20:
                force = smooth(t/.20)
                return 2, 1-.065*force, 1+.14*force, -6*force, -5*force, 0
            if t < .30:
                hit = smooth((t-.20)/.10)
                return 3, 1.09, .94, 7+9*hit, 12*hit, 0
            if t < .39:
                return 3, 1.09, .94, 16, 12, 0
            if t < .64:
                settle = 1-smooth((t-.39)/.25)
                return 4, 1+.03*settle, 1-.025*settle, 5*settle, 12*settle, 0
            return 0, 1, 1, 0, 0, 0
        if args.profile in ('titan', 'titan_guard', 'lion'):
            if t < .20:
                return 2, 1-.03*smooth(t/.20), 1+.03*smooth(t/.20), 0, -3, 0
            if t < .34:
                return 3, 1, 1, 0, 6*smooth((t-.20)/.14), 0
            if t < .56:
                return 4, 1, 1, 0, 3*(1-smooth((t-.34)/.22)), 0
            return 0, 1, 1, 0, 0, 0
        if args.profile == 'flamethrower':
            # Fixed nozzle: brace, sustain flame, shut off, settle. No ram.
            if t < .20:
                return 2, 1, 1, 0, -2*smooth(t/.20), 0
            if t < .62:
                return 3, 1, 1, 0, -3, 0
            if t < .80:
                return 4, 1, 1, 0, -3*(1-smooth((t-.62)/.18)), 0
            return 0, 1, 1, 0, 0, 0
        if args.profile in ('spear', 'bear', 'wisp', 'gunner', 'mage', 'haunted'):
            if t < .2:
                force = smooth(t/.2)
                return 2, 1-.04*force, 1+.06*force, 0, -4*force, 0
            if t < .34:
                hit = smooth((t-.2)/.14)
                recoil = args.profile in ('wisp', 'gunner', 'mage', 'haunted')
                return 3, 1.05 if not recoil else .97, .97 if not recoil else 1.04, 0, (-4 if recoil else 10)*hit, 0
            if t < .56:
                return 4, 1, 1, 0, 5*(1-smooth((t-.34)/.22)) if args.profile in ('spear','bear') else 0, 0
            return 0, 1, 1, 0, 0, 0
        if args.profile in ('spider', 'machine', 'scorpion'):
            if t < .2:
                force = smooth(t/.2)
                return 2, 1-.04*force, 1+.025*force, 0, -4*force, 0
            if t < .34:
                return 3, 1.04, .98, 0, 10*smooth((t-.2)/.14), 0
            if t < .56:
                return 4, 1, 1, 0, 10*(1-smooth((t-.34)/.22)), 0
            return 0, 1, 1, 0, 0, 0
        if args.profile in ('elf', 'verdant'):
            # Bow hand stays planted: draw, release, lower bow, rest.
            if args.expressive:
                if t < .2:
                    force = smooth(t / .2)
                    return 2, 1 + .09 * force, 1 - .045 * force, 0, 0, 0
                if t < .34:
                    recoil = 1 - smooth((t - .2) / .14)
                    return 3, 1 - .035 * recoil, 1 + .06 * recoil, 0, -3 * recoil, 0
                if t < .56:
                    return 4, 1, 1, 0, 0, 0
                return 0, 1, 1, 0, 0, 0
            return (2 if t < .2 else 3 if t < .34 else 4 if t < .56 else 0), 1, 1, 0, 0, 0
        if args.expressive:
            if t < .2:
                force = smooth(t / .2)
                return 2, 1 - .04 * force, 1 + .10 * force, -4 * force, -4, 0
            if t < .34:
                hit = smooth((t - .2) / .14)
                return 3, 1.06, .96, 7 + 8 * hit, 8 * hit, 0
            if t < .56:
                settle = 1 - smooth((t - .34) / .22)
                return 4, 1 + .025 * settle, 1 - .015 * settle, 4 * settle, 8 * settle, 0
            return 0, 1, 1, 0, 0, 0
        if t < .2:
            return 2, 1, 1, -4 * smooth(t / .2), -4, 0
        if t < .34:
            return 3, 1.03, .98, 3, 8 * smooth((t - .2) / .14), 0
        if t < .56:
            return 4, 1, 1, 1, 8 * (1 - smooth((t - .34) / .22)), 0
        return 0, 1, 1, 0, 0, 0
    if t < .12:
        return 0, 1, 1, 0, 0, 0
    fall = smooth((t - .28) / .38)
    if args.readable_trial and args.profile == 'bandit':
        fall = smooth((t-.18)/.46)
        return 5, 1+.025*fall, 1-.035*fall, 90*fall, 0, 0
    if args.profile in ('spider', 'machine', 'gunner', 'wisp', 'haunted', 'scorpion', 'flamethrower', 'titan', 'titan_guard'):
        return 5, 1 + .035 * fall, 1 - .20 * fall, 0, 0, 0
    return 5, 1, 1, 90 * fall, 0, 0


def place(canvas, clip, t, x, ground, height):
    index, sx, sy, angle, dx, dy = motion(clip, t)
    image = sprites[index]
    px, py = pivots[index]
    scale = height / base_height
    theta = math.radians(angle)
    c, s = math.cos(theta), math.sin(theta)
    transform = np.array([[c * sx, -s * sy], [s * sx, c * sy]]) * scale
    # Keep a falling pose above ground when its side becomes the new base.
    correction = 0
    if angle:
        ys, xs = np.where(np.array(image)[..., 3] > 80)
        low = (transform[1, 0] * (xs - px) + transform[1, 1] * (ys - py)).max()
        correction = max(0, low)
    center = np.array([x + dx * height / 140, ground + dy * height / 140 - correction])
    origin = center - transform @ np.array([px, py])
    matrix = np.eye(3)
    matrix[:2, :2] = transform
    matrix[:2, 2] = origin
    inv = np.linalg.inv(matrix)
    layer = image.transform(canvas.size, Image.Transform.AFFINE, tuple(inv[:2].flatten()), Image.Resampling.BICUBIC)
    if clip == 'dead' and args.profile == 'wisp':
        # A spirit dissipates; do not invent feet or rotate it like a corpse.
        opacity = 1 - .88 * smooth((t-.65)/.5)
        layer.putalpha(layer.getchannel('A').point(lambda value: round(value*opacity)))
    # Full-canvas drawing: never clip art into fixed square animation cells.
    canvas.alpha_composite(layer)


def effects(canvas, clip, t, x, ground, height):
    layer = Image.new('RGBA', canvas.size)
    draw = ImageDraw.Draw(layer)
    if clip == 'skill' and args.profile == 'titan_guard':
        phase = t % 1.4
        if .28 <= phase < 1.0:
            opacity = smooth((phase-.28)/.10)*(1-smooth((phase-.90)/.10))
            # A compact cyan virtual shield in front of the blocking arms.
            # Faint interior preserves robot readability, no full-body glow.
            cx,cy = x+height*.44,ground-height*.48
            width,hh = height*.25,height*.37
            shield = [(cx-width*.7,cy-hh),(cx+width*.55,cy-hh*.86),
                      (cx+width,cy-hh*.10),(cx+width*.62,cy+hh*.63),
                      (cx-width*.25,cy+hh),(cx-width*.8,cy+hh*.20)]
            draw.polygon(shield,fill=(85,211,239,round(45*opacity)))
            draw.line(shield+[shield[0]],fill=(27,99,121,round(220*opacity)),width=max(3,round(height*.033)))
            draw.line(shield+[shield[0]],fill=(150,239,248,round(230*opacity)),width=max(1,round(height*.014)))
    elif clip == 'skill' and args.profile == 'royal_plant':
        phase = t % 1.4
        if .28 <= phase < .62:
            pixels = np.array(sprites[7])
            rgb = pixels[..., :3].astype(int)
            silver = (pixels[...,3]>180)&(rgb.max(axis=2)-rgb.min(axis=2)<35)&(rgb.min(axis=2)>100)
            ys,xs = np.where(silver)
            if len(xs):
                bottom = ys>ys.max()-6
                px,py = pivots[7]
                cx = x+(float(xs[bottom].mean())-px)*height/base_height
                cy = ground+(float(ys[bottom].mean())-py)*height/base_height
                age = (phase-.28)/.34
                radius = height*(.06+.24*age)
                draw.ellipse((cx-radius,cy-height*.025,cx+radius,cy+height*.025),outline=(232,179,67,round(200*(1-age))),width=max(1,round(height*.02)))
    elif clip == 'skill':
        phase = t % 1.4
        if .28 <= phase < .68:
            age = phase-.28
            # Impact remains at the hit pose, not dragged back by recovery.
            _, sx, sy, _, dx, _ = motion('skill', .45)
            pixels = np.array(sprites[7])
            ys, xs = np.where(pixels[...,3]>80)
            px, py = pivots[7]
            tip = xs.max()
            cx = x+dx*height/140+(tip-px)*sx*height/base_height
            cy = ground+(float(ys[xs>=tip-5].mean())-py)*sy*height/base_height
            alpha = round(240*(1-age/.40))
            if args.profile == 'verdant':
                # Three fast, straight arrows; no bow-hand swapping or gravity.
                release = np.array(sprites[7])
                rgb = release[..., :3].astype(int)
                green = (release[...,3]>180)&(rgb[...,1]>90)&(rgb[...,1]>rgb[...,0]*1.25)&(rgb[...,1]>rgb[...,2]*1.2)
                gy,gx = np.where(green)
                if len(gx):
                    bow = gx>gx.max()-release.shape[1]*.15
                    px,py = pivots[7]
                    cx = x-3*height/140+(float(gx[bow].mean())-px)*height/base_height
                    cy = ground+(float(gy[bow].mean())-py)*height/base_height
                for offset in (-.06,0,.06):
                    tx = cx+age*height*8
                    ty = cy+offset*height
                    draw.line((tx-height*.27,ty,tx,ty),fill=(42,67,35,255),width=max(2,round(height*.025)))
                    draw.polygon([(tx,ty),(tx-height*.07,ty-height*.025),(tx-height*.07,ty+height*.025)],fill=(242,222,126,255))
            elif args.profile == 'royal':
                # Ground contact happens after the descending sword settles.
                if phase >= .42:
                    life = smooth((phase-.42)/.26)
                    center = cx
                    for sign in (-1,1):
                        draw.line((center,ground-2,center+sign*height*(.12+.28*life),ground-2),fill=(232,179,67,alpha),width=max(2,round(height*.025)))
                        draw.polygon([(center+sign*height*.10,ground-2),
                                      (center+sign*height*(.15+.17*life),ground-height*.13*(1-life)),
                                      (center+sign*height*.20,ground-2)],fill=(250,229,157,alpha))
            elif args.profile == 'king':
                # Compact purple ground pulse, no huge glow/aura.
                radius = height*(.12+.42*smooth(age/.40))
                draw.arc((x-radius,ground-height*.14-radius*.25,x+radius,ground-height*.14+radius*.25),0,180,fill=(135,91,184,alpha),width=max(2,round(height*.03)))
            elif args.profile == 'lion':
                # Roar waves originate near the muzzle, not the mace.
                cx,cy = x+height*.30,ground-height*.70
                for offset in (0,.12):
                    radius = height*(.09+offset+.25*smooth(age/.40))
                    draw.arc((cx-radius,cy-radius,cx+radius,cy+radius),-50,50,fill=(251,224,151,alpha),width=max(2,round(height*.035)))
            elif args.profile == 'titan':
                # Short punch trail followed by local impact rays.
                draw.line((cx-height*.32,cy,cx-height*.03,cy),fill=(240,220,147,alpha),width=max(2,round(height*.035)))
                for angle in (-.65,0,.65):
                    distance = height*(.04+.16*smooth(age/.40))
                    draw.line((cx+math.cos(angle)*distance,cy+math.sin(angle)*distance,
                               cx+math.cos(angle)*(distance+height*.08),cy+math.sin(angle)*(distance+height*.08)),fill=(246,208,103,alpha),width=max(2,round(height*.025)))
    elif clip == 'walk' and args.profile != 'wisp':
        # Emit tiny puffs at alternating foot contacts, attached to the ground
        # where the contact happened rather than following the moving body.
        for step in range(max(0, int(t / .5) - 1), int(t / .5) + 1):
            age = t - step * .5
            if not 0 <= age < .22:
                continue
            life = age / .22
            contact = x + 60 * math.sin(step * .5 * .7) * height / 140
            for foot_offset in (-.17, .12):
                radius = height * (.029 + .016 * life)
                cx = contact + height * (foot_offset - .075 * life)
                cy = ground - height * (.012 + .025 * life)
                draw.ellipse((cx - radius, cy - radius, cx + radius, cy + radius),
                             fill=(255, 235, 186, round(220 * (1 - life))),
                             outline=(159, 119, 64, round(150 * (1 - life))), width=1)
    elif clip == 'attack' and args.profile in ('spear', 'bear', 'wisp', 'gunner', 'mage', 'haunted'):
        phase = t % 1.15
        if .20 <= phase < .49:
            _, sx, sy, _, dx, _ = motion('attack', phase)
            pixels = np.array(sprites[3])
            ys, xs = np.where(pixels[...,3]>80)
            px, py = pivots[3]
            front = xs.max()
            if args.profile == 'mage':
                # Release from cyan staff crystal, not the hat or free hand.
                rgb = pixels[..., :3].astype(int)
                crystal = (pixels[...,3]>180)&(rgb[...,1]>140)&(rgb[...,2]>150)&(rgb[...,0]<120)
                crystal_y, crystal_x = np.where(crystal)
                if len(crystal_x):
                    xs, ys = crystal_x, crystal_y
                    front = xs.max()
            if args.profile == 'bear':
                # The muzzle can protrude farther than the paw. Anchor claw
                # streaks to the grey claws, never the bear's nose/face.
                rgb=pixels[...,:3].astype(int)
                claw=(pixels[...,3]>180)&(rgb.min(axis=2)>35)&(rgb.max(axis=2)<145)&(np.ptp(rgb,axis=2)<45)
                claw_y,claw_x=np.where(claw)
                if len(claw_x):
                    xs,ys=claw_x,claw_y
                    front=xs.max()
            cx = x + dx*height/140 + (front-px)*sx*height/base_height
            cy = ground + (float(ys[xs>=front-6].mean())-py)*sy*height/base_height
            age = phase-.20
            if args.profile in ('wisp', 'gunner', 'mage', 'haunted'):
                # Projectile origin is locked at release, not dragged by recoil.
                cx = x + (front-px)*(.97)*height/base_height + age*height*10
                cy = ground + (float(ys[xs>=front-6].mean())-py)*1.04*height/base_height
                if args.profile in ('wisp', 'mage', 'haunted'):
                    radius=height*.045
                    if args.profile == 'haunted':
                        radius = height*.065
                        draw.line((cx-height*.20,cy,cx-radius,cy),fill=(114,205,217,180),width=max(2,round(height*.035)))
                    draw.ellipse((cx-radius,cy-radius,cx+radius,cy+radius),fill=(82,221,242,255),outline=(22,70,102,255),width=max(1,round(height*.012)))
                else:
                    draw.line((cx-height*.14,cy,cx,cy),fill=(250,220,110,255),width=max(2,round(height*.025)))
                    if age < .05:
                        muzzle_x = x + dx*height/140 + (front-px)*sx*height/base_height
                        size = height*.055*(1-age/.05)
                        draw.polygon([(muzzle_x,cy),(muzzle_x+size,cy-size),
                                      (muzzle_x+size*.65,cy),(muzzle_x+size,cy+size)],
                                     fill=(255,233,133,255))
            elif phase < .34:
                alpha=round(230*math.sin(math.pi*age/.14))
                if args.profile=='spear':
                    if args.strong_fx:
                        length=height*.42
                        width=height*.045
                        ribbon=[(cx-length,cy-width*.25),(cx,cy-width),(cx+height*.11,cy),
                                (cx,cy+width),(cx-length,cy+width*.25)]
                        draw.polygon(ribbon,fill=(255,247,211,alpha))
                        draw.line(ribbon+[ribbon[0]],fill=(79,78,59,alpha),width=1)
                        # Fading spear-tip impressions on the SAME thrust axis.
                        for behind,strength in ((.18,.55),(.31,.30)):
                            tip=cx-height*behind
                            draw.polygon([(tip-height*.10,cy-height*.027),(tip,cy),
                                          (tip-height*.10,cy+height*.027)],
                                         fill=(239,244,235,round(alpha*strength)))
                    else:
                        draw.line((cx-height*.15,cy,cx+height*.09,cy),fill=(252,249,225,alpha),width=max(2,round(height*.03)))
                else:
                    for offset in (-.04,0,.04):
                        if args.strong_fx:
                            # Three tapered claw trails follow the SAME paw;
                            # upper-left tail -> lower-right leading tip.
                            tail=(cx-height*.015,cy+height*offset-height*.055)
                            tip=(cx+height*.23,cy+height*offset+height*.16)
                            width=height*.022
                            poly=[tail,(cx+height*.075-width,cy+height*offset+height*.045),tip,
                                  (cx+height*.075+width,cy+height*offset+height*.005)]
                            draw.polygon(poly,fill=(255,249,225,alpha))
                            draw.line(poly+[poly[0]],fill=(61,67,68,alpha),width=1)
                        else:
                            draw.line((cx-height*.10,cy+height*offset-height*.10,cx+height*.035,cy+height*offset),fill=(246,242,224,alpha),width=max(1,round(height*.018)))
    elif clip == 'attack' and args.profile == 'scorpion':
        phase = t % 1.15
        if .20 <= phase < .34:
            pixels = np.array(sprites[3])
            rgb = pixels[..., :3].astype(int)
            orange = (pixels[...,3]>180)&(rgb[...,0]>170)&(rgb[...,1]>55)&(rgb[...,1]<200)&(rgb[...,2]<95)
            # The yellow leading edge marks the tail stinger, not a pincer.
            stinger = (pixels[...,3]>180)&(rgb[...,0]>220)&(rgb[...,1]>180)&(rgb[...,2]<130)
            if stinger.any():
                orange = stinger
            ys, xs = np.where(orange)
            if len(xs):
                # Forward-most orange tip belongs to the attacking stinger.
                tip_x = xs.max()
                tip_y = float(ys[xs>=tip_x-5].mean())
                px, py = pivots[3]
                _, sx, sy, _, dx, _ = motion('attack', phase)
                end = np.array([x+dx*height/140+(tip_x-px)*sx*height/base_height,
                                ground+(tip_y-py)*sy*height/base_height])
                start = end + np.array([-height*.34,-height*.32])
                control = end + np.array([-height*.03,-height*.35])
                progress = smooth((phase-.20)/.08)
                tail = max(0.,progress-.55)
                alpha = round(230*math.sin(math.pi*(phase-.20)/.14))
                points = [(1-u)**2*start+2*(1-u)*u*control+u*u*end for u in np.linspace(tail,progress,20)]
                draw.line([tuple(p) for p in points],fill=(70,39,25,alpha),width=max(3,round(height*.055)))
                draw.line([tuple(p) for p in points],fill=(255,212,130,alpha),width=max(2,round(height*.035)))
    elif clip == 'attack' and args.profile == 'flamethrower':
        phase = t % 1.15
        if .20 <= phase < .62:
            pixels = np.array(sprites[3])
            ys, xs = np.where(pixels[...,3]>80)
            px, py = pivots[3]
            _, sx, sy, _, dx, _ = motion('attack', phase)
            tip = xs.max()
            cx = x+dx*height/140+(tip-px)*sx*height/base_height
            cy = ground+(float(ys[xs>=tip-5].mean())-py)*sy*height/base_height
            envelope = smooth((phase-.20)/.05)*(1-smooth((phase-.55)/.07))
            flicker = math.sin((phase-.20)*math.tau*13)
            length = height*(.76+.055*flicker)*envelope
            spread = height*(.14+.012*flicker)*envelope
            alpha = round(255*envelope)
            # A continuous attached jet, hard-edge red/orange/yellow tongues.
            outer = [(cx-1,cy-height*.025),(cx+length*.28,cy-spread*.48),
                     (cx+length*.60,cy-spread),(cx+length*.55,cy-spread*.34),
                     (cx+length,cy-spread*.32),(cx+length*.78,cy),
                     (cx+length*.95,cy+spread*.55),(cx+length*.56,cy+spread*.40),
                     (cx+length*.62,cy+spread),(cx+length*.26,cy+spread*.45),
                     (cx-1,cy+height*.025)]
            draw.polygon(outer,fill=(235,70,20,alpha))
            draw.line(outer+[outer[0]],fill=(64,34,23,alpha),width=max(1,round(height*.015)))
            inner = [(cx,cy-height*.015),(cx+length*.35,cy-spread*.36),
                     (cx+length*.79,cy-spread*.19),(cx+length*.60,cy),
                     (cx+length*.76,cy+spread*.37),(cx+length*.30,cy+spread*.29),
                     (cx,cy+height*.015)]
            draw.polygon(inner,fill=(255,150,24,alpha))
            draw.polygon([(cx,cy-height*.011),(cx+length*.48,cy-spread*.11),
                          (cx+length*.61,cy),(cx+length*.37,cy+spread*.14),
                          (cx,cy+height*.011)],fill=(255,231,105,alpha))
    elif clip == 'attack' and args.profile in ('spider', 'machine'):
        phase = t % 1.15
        if .24 <= phase < .34:
            life = (phase-.24)/.10
            cx, cy = x + height*.65, ground-height*.38
            if args.profile == 'machine':
                alpha = np.array(sprites[3])[...,3]
                ys, xs = np.where(alpha > 80)
                _, sx, sy, _, dx, _ = motion('attack', phase)
                px, py = pivots[3]
                cx = x + dx*height/140 + (xs.max()-px)*sx*height/base_height
                cy = ground + (float(ys[xs>=xs.max()-8].mean())-py)*sy*height/base_height
            # Small local fang impact / saw sparks, never a weapon slash.
            for angle in (-.65, 0, .65):
                distance = height*(.035+.08*life)
                dx, dy = math.cos(angle), math.sin(angle)
                draw.line((cx+dx*distance, cy+dy*distance,
                           cx+dx*(distance+height*.05), cy+dy*(distance+height*.05)),
                          fill=((255,210,80,round(230*(1-life))) if args.profile=='machine' else (244,239,219,round(210*(1-life)))),
                          width=max(1,round(height*.018)))
    elif clip == 'attack' and args.profile in ('lion', 'titan', 'titan_guard'):
        phase = t % 1.15
        if .20 <= phase < .34:
            pixels = np.array(sprites[3])
            ys,xs = np.where(pixels[...,3]>80)
            px,py = pivots[3]
            tip = xs.max()
            _,sx,sy,_,dx,_ = motion('attack',phase)
            cx = x+dx*height/140+(tip-px)*sx*height/base_height
            cy = ground+(float(ys[xs>=tip-5].mean())-py)*sy*height/base_height
            age = (phase-.20)/.14
            alpha = round(230*math.sin(math.pi*age))
            if args.profile == 'lion':
                # Blunt mace sweep, not a sword slash.
                draw.arc((cx-height*.42,cy-height*.50,cx+height*.04,cy+height*.10),-110,30,fill=(246,225,175,alpha),width=max(2,round(height*.045)))
            else:
                draw.line((cx-height*.22,cy,cx,cy),fill=(248,231,184,alpha),width=max(2,round(height*.035)))
            for angle in (-.6,0,.6):
                draw.line((cx+height*.025,cy,cx+math.cos(angle)*height*.13,cy+math.sin(angle)*height*.13),fill=(255,223,135,alpha),width=max(1,round(height*.016)))
    elif clip == 'attack' and args.profile in ('elf', 'verdant'):
        phase = t % 1.15
        if .20 <= phase < .49:
            # Fast straight arrow, released once; no melee slash or gravity.
            travel = (phase - .20) * height * 9
            cx, cy = x + height * .50 + travel, ground - height * .40
            if OUT.name.startswith('elf_epic') or args.profile=='verdant':
                # Cyan bow tips bracket the grip; ignore the forehead gem.
                pixels = np.array(sprites[3])
                rgb = pixels[..., :3].astype(int)
                columns = np.indices(pixels.shape[:2])[1]
                bow = (pixels[...,3]>180)&(rgb[...,0]<110)&(rgb[...,1]>145)&(rgb[...,2]>170)&(columns>pixels.shape[1]*.72)
                if args.profile == 'verdant':
                    green = (pixels[...,3]>180)&(rgb[...,1]>90)&(rgb[...,1]>rgb[...,0]*1.25)&(rgb[...,1]>rgb[...,2]*1.2)
                    gy,gx = np.where(green)
                    if len(gx):
                        bow = green&(columns>gx.max()-pixels.shape[1]*.15)
                ys, xs = np.where(bow)
                if len(xs):
                    px, py = pivots[3]
                    cx = x+(float(xs.mean())-px)*height/base_height+travel
                    cy = ground+(float(ys.mean())-py)*height/base_height
            length = height * .27
            draw.line((cx - length, cy, cx, cy), fill=(31, 27, 21, 255), width=max(2, round(height * .027)))
            draw.polygon([(cx, cy), (cx - height * .07, cy - height * .035),
                          (cx - height * .07, cy + height * .035)], fill=(35, 35, 35, 255))
            draw.line((cx-length, cy-height*.03, cx-length+height*.05, cy), fill=(249, 236, 194, 255), width=max(1, round(height*.018)))
    elif clip == 'attack':
        phase = t % 1.15
        if .20 <= phase < .34:
            # A short moving blade-tip trail, not a stationary full crescent.
            # Silver pixels locate the actual strike tip; use the sprite's
            # same scale, pivot, rotation and lunge to keep the endpoint aligned.
            pixels = np.array(sprites[3])
            rgb = pixels[..., :3].astype(int)
            blade = (pixels[..., 3] > 180) & (rgb.min(axis=2) > 130) & (np.ptp(rgb, axis=2) < 35)
            ys, xs = np.where(blade)
            tip_x = xs.max()
            tip_y = float(ys[xs >= tip_x - 2].mean())
            px, py = pivots[3]
            _, sx, sy, angle, dx, _ = motion('attack', phase)
            theta = math.radians(angle)
            tx, ty = (tip_x - px) * sx, (tip_y - py) * sy
            end = np.array([x + dx * height / 140,
                            ground]) + height / base_height * np.array([
                                math.cos(theta) * tx - math.sin(theta) * ty,
                                math.sin(theta) * tx + math.cos(theta) * ty])
            if angle:
                body_y, body_x = np.where(pixels[..., 3] > 80)
                low = height / base_height * (math.sin(theta)*sx*(body_x-px) + math.cos(theta)*sy*(body_y-py)).max()
                end[1] -= max(0, low)
            start = np.array([x - height * .08, ground - height * (1.09 if args.expressive else 1.02)])
            control = np.array([end[0] + height * .08, ground - height * 1.02])
            progress = min(1., (phase - .20) / .07)
            tail = max(0., progress - .68 + max(0., phase - .27) * 6)
            alpha = round(255 * min(1., (phase - .20) / .015) * (1 - max(0., phase - .29) / .05))
            points = []
            for u in np.linspace(tail, progress, 24):
                point = (1-u)**2 * start + 2*(1-u)*u * control + u*u * end
                points.append(tuple(point))
            # A readable tapered slash ribbon: thin trailing end, broad
            # middle, pointed leading end at the blade. No glow or ring.
            edges = [[], []]
            for j, point in enumerate(points):
                tangent = np.array(points[min(j + 1, len(points) - 1)]) - np.array(points[max(0, j - 1)])
                normal = np.array([-tangent[1], tangent[0]]) / max(.001, np.linalg.norm(tangent))
                width = height * (.060 if args.strong_fx else .043) * math.sin(math.pi * j / (len(points) - 1))
                edges[0].append(tuple(np.array(point) + normal * width))
                edges[1].append(tuple(np.array(point) - normal * width))
            outline = edges[0] + edges[1][::-1]
            draw.polygon(outline, fill=(255, 247, 209, alpha))
            draw.line(outline + [outline[0]], fill=(49, 65, 67, alpha), width=max(1, round(height * .012)))
            draw.line(points[5:-4], fill=(255, 255, 255, alpha), width=max(1, round(height * .02)))
            if args.strong_fx:
                # A pale displaced blade-tip path behind the main slash,
                # not a second solid sword or a whole-character duplicate.
                ghost=[(point[0]-height*.035,point[1]-height*.025) for point in points[3:-3]]
                draw.line(ghost,fill=(231,243,235,round(alpha*.65)),width=max(2,round(height*.033)))
    canvas.alpha_composite(layer)


if args.export_unity:
    export = ROOT / args.export_unity
    export.mkdir(parents=True, exist_ok=True)
    manifest_path = export/'frames.json'
    manifest = json.loads(manifest_path.read_text())['clips'] if args.idle_only and manifest_path.exists() else {}
    export_clips = [('idle', .9)] if args.idle_only else [('idle', .9), ('walk', .5), ('attack', .7), ('dead', 1.2)] + ([('skill', 1.4)] if args.skills else [])
    for clip, duration in export_clips:
        entries = []
        for i in range(round(duration*30)):
            t = i/30
            anchor_x, anchor_y, height = 400, 480, 240
            canvas = Image.new('RGBA', (1000, 600))
            # Locomotion belongs to UnitBrain. Keep foot bob, not demo travel.
            dx = motion(clip,t)[4]*height/140 if clip == 'walk' else 0
            place(canvas,clip,t,anchor_x-dx,anchor_y,height)
            # Ranged missiles are spawned by gameplay, not baked into sprites.
            if args.effects and clip in ('walk','attack','skill') and not (clip in ('attack','skill') and args.profile in ('elf','machine','gunner','mage','wisp','verdant','haunted')):
                effects(canvas,clip,t,anchor_x-dx,anchor_y,height)
            bounds = canvas.getbbox()
            if bounds is None or bounds[0] == 0 or bounds[1] == 0 or bounds[2] == 1000 or bounds[3] == 600:
                raise RuntimeError(f'Empty or cropped export: {clip}/{i}')
            # Variable rectangle includes the anchor; never clip into square cells.
            left,top,right,bottom = bounds
            left,top = min(left,anchor_x)-4,min(top,anchor_y)-4
            right,bottom = max(right,anchor_x+1)+4,max(bottom,anchor_y+1)+4
            name = f'{clip}_{i:03d}.png'
            canvas.crop((left,top,right,bottom)).save(export/name)
            entries.append({'file':name,'pivot':[(anchor_x-left)/(right-left),1-(anchor_y-top)/(bottom-top)],'duration':1/30})
        manifest[clip] = entries
    (export/'frames.json').write_text(json.dumps({'height':240,'clips':manifest},indent=2))
    print(f'PASS: transparent baked frames exported to {export}')
    sys.exit(0)

all_frames, times = [], []
if args.demo_output:
    OUT = ROOT / args.demo_output
    OUT.mkdir(parents=True, exist_ok=True)
clips = [('idle', 1.6), ('walk', 2), ('attack', 2.3)]
if args.skills:
    clips.append(('skill',2.8))
clips.append(('dead',1.8))
for clip, duration in clips:
    frames = []
    for i in range(round(duration * 30)):
        t = i / 30
        canvas = Image.new('RGBA', (800, 360), (225, 237, 216, 255))
        draw = ImageDraw.Draw(canvas)
        draw.rectangle((0, 282, 800, 360), fill=(223, 190, 127))
        unit_label = OUT.name.split('_trial')[0].upper()
        draw.text((20, 18), f'{unit_label} / {clip.upper()}' + (' / FX' if args.effects else ''), fill=(25, 30, 35))
        draw.text((135, 65), 'Game size: 70px', fill=(25, 30, 35))
        draw.text((490, 65), 'Zoom: 140px', fill=(25, 30, 35))
        if args.readable_trial:
            for x, height in ((210,70),(570,140)):
                # A world-space cue: never inherits breathing, rotation or lunge.
                if clip == 'idle':
                    draw.ellipse((x-height*.35,280-height*.055,x+height*.35,280+height*.055),
                                 outline=(36,139,198),width=max(1,round(height*.025)))
        place(canvas, clip, t, 210, 280, 70)
        place(canvas, clip, t, 570, 280, 140)
        if args.effects and clip in ('walk', 'attack', 'skill'):
            effects(canvas, clip, t, 210, 280, 70)
            effects(canvas, clip, t, 570, 280, 140)
            if clip == 'attack' and i == 8:
                canvas.save(OUT / 'attack_fx_preview.png')
            if clip == 'walk' and i == 17:
                canvas.save(OUT / 'walk_fx_preview.png')
            if clip == 'skill' and i == 12:
                canvas.save(OUT / 'skill_fx_preview.png')
        frames.append(canvas.convert('RGB'))
    frames[0].save(OUT / f'{clip}_demo.gif', save_all=True, append_images=frames[1:], duration=33, loop=0, disposal=2)
    all_frames.extend(frames)
    times.extend([33] * len(frames))
    if clip == 'dead':
        times[-1] = 1000
all_frames[0].save(OUT / 'full_demo.gif', save_all=True, append_images=all_frames[1:], duration=times, loop=0, disposal=2)
all_frames[0].save(OUT / 'size_preview.png')
(OUT / 'rig.json').write_text(json.dumps({'pose_order': names, 'pixel_pivots': pivots,
    'scale_reference_height': base_height, 'profile': args.profile, 'expressive': args.expressive, 'strong_fx': args.strong_fx,
    'notes': 'One common scale across authored poses. Fixed equipment ownership (elf left bow/right draw; melee right weapon/left offhand; mechanical attachments unchanged). Full canvas transforms; no square pose cropping. Authored trial motion, not APK curves.'}, indent=2))
print(f'PASS: {expected} complete poses, preserved alpha, fixed clip scale; demos written.')
