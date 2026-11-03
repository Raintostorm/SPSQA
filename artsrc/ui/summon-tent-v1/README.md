# Approved Summon tent UI

Built-in imagegen was used (not CLI). `approved-demo.png` is the approved layout,
not a baked runtime interface. Runtime uses `background.png` plus real text,
wallet and buttons. The simpler transparent `orb.png` replaces the Home nav icon.
Original previous art remains under `../summon-v2/`.

## Final production background prompt

Use case: precise-object-edit. Convert this approved mobile SUMMON screen into a
clean production BACKGROUND ONLY asset. Remove ALL UI: top-right gem wallet
including gem, number and dark rectangle; SUMMON title; Summon ally shards caption;
both purple summon buttons including gems and text; bottom Back button and text.
Seamlessly fill these removed regions with the existing sky and dark blue ground.
Keep the witch, tent, table, small crystal ball, cyan magic wisps, moon banners,
lamps, trees, bushes, rocks EXACTLY in the same positions, scale and simple cartoon
black-outline style. Preserve full portrait framing and aspect ratio. Lower third
remains calm mostly dark navy-blue ground suitable for overlaying real UI. No
letters, numbers, buttons, panels, watermarks or new objects. Do not crop or enlarge
the tent. Only remove UI and restore scenery.

## Simple icon prompt

Single small mobile game bottom navigation icon, transparent background. Very
simple crystal ball SUMMON icon consistent with flat chibi cartoon game UI. One
round cyan crystal ball, thick near-black outline, one pale blue curved highlight
and one white four-point star inside, solid purple crescent base with two tiny
feet. Restrained cel shading, clear silhouette at 48px. Ball empty: no person,
soldier, face, swirling galaxy, runes, filigree, gemstones or particles. Centered
isolated icon with transparent padding, no badge, text or watermark.

Run `python3 tools/import_summon_tent.py` to reimport. Rates and pity labels are
hidden only in the UI; AllySummon economy rules are unchanged.
