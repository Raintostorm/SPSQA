# Roll reward source art v1

Built-in imagegen; reference: actual human_legendary/idle_000.png, style only. All sheets 1536x1024 RGBA, true alpha, 3x2 equal cells. Not imported into runtime yet.

Prompt set: flat cartoon mobile reward assets, bold dark outlines, readable at 40px, no labels/background/character redraw; transparent gutters.

- coin.png: six poses of one rotating gold coin, crown motif; animate travel in code. Align pivots and apparent height before playback (generated spacing varies slightly).
- buff.png: independent HP heart, attack sword, speed lightning, crit target, armor shield, neutral pulse. Not a six-frame animation.
- recruit.png: independent neutral comet, burst, arrival ellipse, motes, impact, halo. Tint these in runtime by rarity; no new body art.
- slot-flash.png: six stages of empty rounded-square reward highlight. Output is warm white/gold, rather than requested strictly neutral white; use for general winning-slot feedback, not rarity identity. Fixed-bounds normalization and small-size QA required before import.

Full integration/verification plan: outdata/ROLL_REWARD_INTEGRATION_PLAN.md. No balance or equipment mechanics changed by generating this art.
