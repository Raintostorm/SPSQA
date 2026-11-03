# Approved simple unit concept — Bandit / future human-common

Approval recorded after Bandit V4 slash demo, 2026-10-10.

## Source of truth and rollout

- Approved art and demos: `bandit_trial_v4/` (relative to this document).
- Original art: `bandit_trial_v4/poses.png`; same art as V2/V3. V4 changes only procedural slash rendering.
- Art-generation edit prompt: `bandit_trial_v2/PROMPT.md`. Earlier versions remain for comparison.
- Reproduction: `python3 tools/preview_simple_bandit.py --directory artsrc/units_simple/bandit_trial_v4 --effects`.
- Renderer: `tools/preview_simple_bandit.py`; exact motion, cropping, pivots and effect formulas are in that file.
- Bandit will REPLACE `human-common` / `human_common`, not become an extra unit.
- DEMO APPROVED, NOT IMPORTED. Keep current Unity units unchanged until the entire replacement set is finished and reviewed; integrate the set together, once. Preserve gameplay IDs, stats and existing references when integrating.

## Visual concept

Original cute, compact chibi bandit, facing RIGHT in a consistent slightly three-quarter side view. Orange tied bandana, completely blank cream face, teal tunic, brown pants/boots, short curved silver dagger. Large rounded head, short sturdy limbs, simple readable silhouette.

Heavy pure-black outlines around every main shape, approximately 6% of standing height as a design target. Judge at actual 70px height, not only enlarged. Flat colour areas, minimal detail; no newly added texture, shading or decorative clutter. The approved bitmap, rather than numerical claims about perfect palette compliance, is the visual reference.

Other units should share this proportion, outline weight, blank-face treatment and simplicity, but retain their own silhouette, colours, equipment and attack behaviour. Do not copy the Bandit's dagger effect onto ranged or blunt attacks.

## Non-negotiable anatomy and asset rules

- Weapon stays in the SAME anatomical RIGHT / near-side hand in EVERY pose. Never swap hands or mirror the character to produce an opposite stride. Check the actual grip and arm, not screen-left/right alone.
- Off-hand remains off-hand. Two-handed equipment must be explicitly designed as two-handed for its entire action.
- Facing and camera angle never change between poses. Weapon remains connected to the correct grip.
- Complete silhouettes, including boots, weapon, bandana tails and outline. NO forced square cells and NO clipping to a uniform sprite rectangle.
- True transparent, antialiased alpha; no magenta residue or thresholded edges.
- One common scale across poses. Do not resize each pose independently to fit its bounding box.
- Ground pivot is based on planted feet, not the weapon or total bounding-box centre. Preserve ground contact; no horizontal teleport when changing sprites.

## Approved animation construction

Six complete authored poses, reading order: idle, walk, windup, strike, recovery, hurt. Demos combine sprite changes with modest transform motion; this is NOT eight newly drawn frames and NOT the original APK animation curves.

| Clip | Construction |
| --- | --- |
| Idle | Idle pose; gentle 0.9s breathing cycle, approximately 1% scale variation. |
| Walk | Walk/idle poses; 0.5s cycle, slight squash/stretch and small bob. Dust emitted at ground-contact cycle boundaries, not the raised portion. Demo translation is presentation movement, not a required gameplay velocity. |
| Attack | 1.15s cycle: windup 0–0.20s, strike 0.20–0.34s, recovery 0.34–0.56s, idle remainder. Small lunge and tilt. Coherent anticipation → hit → recovery; never unrelated poses. |
| Dead | Idle until 0.12s, then hurt pose; fall rotates during 0.28–0.66s, then final corpse remains still. Ground correction prevents the rotated silhouette sinking below the floor. |

Do not force the same pose count onto every unit. Use only the poses needed for a readable action. Preserve unit-specific melee/ranged/skill logic. This Bandit demo has no approved skill or death-soul effect.

## Approved separate VFX

- WALK: subtle small cream/brown puffs at BOTH feet; lifetime 0.22s, one contact event per 0.5s cycle. Positioned at the contact location, drift slightly back/up and fade. Foreground compositing prevents boots hiding every puff. Keep this approved walk effect unchanged unless requested.
- DAGGER ATTACK: short moving, tapered cream/white slash with dark edge, not a fixed full circle or luminous aura. Visible ONLY during 0.20–0.34s strike window. Ribbon progresses toward the strike blade tip and disappears before recovery.
- Strike tip is estimated from the silver blade pixels, then transformed with the same sprite scale, pivot, rotation and lunge. Curve bridges the discrete windup/strike change; it does not pretend additional blade poses were drawn.
- Effects remain separate from sprite art so timing, layering and size can be tuned without repainting character poses.

## Review gate for every next unit

1. Compare art with the approved Bandit at 70px and enlarged.
2. Inspect complete idle–walk–attack–dead demo, not just the sheet.
3. Scrub every attack transition: same hand, same grip, same facing, sensible attack arc and recovery.
4. Check all silhouette extents, stable scale and feet/ground pivots; verify no parts clipped and no coloured background residue.
5. Verify VFX only at the appropriate contact/hit time and specific to that unit's weapon.
6. Obtain approval before marking the unit ready. Keep Unity untouched until the full replacement set is approved.
